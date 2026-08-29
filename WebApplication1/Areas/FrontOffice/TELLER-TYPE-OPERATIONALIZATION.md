# Teller Type Operationalization Recommendation

## Purpose

The system exposes four `TellerType` values:

| Value | Type |
|---:|---|
| 0 | Employee |
| 1 | ATM Terminal |
| 2 | POS Terminal (In-house) |
| 3 | POS Terminal (Agent) |

Only the **Employee** type currently has a complete operational path through
cash transactions, transfers, balance checks, journal posting, and end of day.
The other three types have partial domain/configuration scaffolding but should
not be presented as operational channels until the capabilities described in
this document are implemented.

## Current state and gaps

### Employee

Employee tellers are resolved through `FindTellerByEmployeeId` and are used by
the working Front Office transaction controllers. Employee-linked G/L,
shortage, and excess accounts support normal transaction posting and end-of-day
variance handling.

### ATM Terminal

The domain can persist an ATM teller with a G/L account, and
`TellerAppService.FetchTellerBalances` can calculate its G/L balance. There is
no ATM authentication, transaction ingestion, cash-dispense confirmation,
automatic reversal, cassette management, settlement, or reconciliation path.

### POS Terminal (In-house)

The domain persists an in-house POS teller in the same general shape as an ATM
and can calculate its G/L balance. There is no terminal identity, operator
session, terminal-based transaction routing, shift close, settlement, or
reconciliation path.

### POS Terminal (Agent)

The domain contains `FloatCustomerAccountId` and
`CommissionCustomerAccountId`, but the current operational path is incomplete:

- the create UI does not collect these accounts;
- balance calculation has no Agent POS implementation;
- transaction controllers do not resolve an agent terminal;
- no cash-in, cash-out, float top-up, commission, settlement, or reconciliation
  application-service workflow exists.

### Current client/API contract issue

The teller creation client sends `TellerType`, while `TellerDTO` expects
`Type`. Consequently, selecting a non-employee type currently defaults to
`Type = 0` during model binding and is treated as an Employee teller. This
property mismatch must be corrected before enabling any additional type.

## Recommended shared architecture

Do not implement terminal behavior directly in API controllers. Introduce a
channel transaction application service, for example:

```csharp
public interface ITerminalTransactionAppService
{
    TerminalTransactionResult AuthorizeTransaction(...);
    TerminalTransactionResult PostWithdrawal(...);
    TerminalTransactionResult PostDeposit(...);
    TerminalTransactionResult ReverseTransaction(...);
    TerminalTransactionDTO FindTransaction(...);
    ReconciliationResult ReconcileTransactions(...);
}
```

This service should own validation, balance checks, journal construction, and
persistence within one database transaction. API controllers should
authenticate the terminal/operator, validate the request shape, create the
`ServiceHeader`, and delegate to the application service.

Add a persisted terminal-transaction aggregate containing at least:

- teller/terminal identifier;
- unique terminal transaction reference;
- customer account identifier;
- transaction type and amount;
- pending, posted, declined, reversed, and failed statuses;
- journal identifier;
- external/switch reference and response code;
- created, posted, settled, and reversed timestamps;
- reference to the original transaction for reversals.

The unique terminal reference must have a database uniqueness constraint.
Retries must return the original result instead of posting a duplicate journal.

## Shared operational requirements

All three terminal types require:

1. Terminal registration, activation, locking, and credential rotation.
2. A terminal identity independent of an employee record.
3. Explicit branch and currency assignment.
4. Operator/device authentication and authorization.
5. Per-transaction and daily limits.
6. Lower/upper balance or float-limit enforcement.
7. Idempotent transaction posting.
8. Reversal support linked to the original journal.
9. Auditable status transitions and correlation IDs.
10. Settlement and reconciliation APIs and screens.
11. Monitoring for failed, timed-out, unmatched, and reversed transactions.
12. Maker-checker controls for configuration and manual adjustments.

## ATM Terminal requirements

An ATM pathway requires:

1. Integration with a card-management system or transaction switch.
2. Card and PIN authentication without exposing PIN data to application logs.
3. Account selection and withdrawal authorization.
4. Product, per-transaction, daily, and channel-limit checks.
5. ATM G/L balance and physical cassette availability checks.
6. Atomic customer-account debit and ATM cash G/L credit.
7. Dispense request and dispense-confirmation processing.
8. Automatic reversal when the account is debited but cash is not dispensed.
9. Cassette loading/unloading with denomination-level counts.
10. Electronic-journal ingestion, settlement, and physical cash
    reconciliation.

The existing teller G/L can represent the ATM's cash position, but it is not a
substitute for cassette inventory and dispense-state tracking.

## In-house POS requirements

In-house POS is the best first implementation because it can reuse most of the
existing employee-teller posting rules. It requires:

1. Registered device identity and branch assignment.
2. Operator authentication or a controlled terminal service identity.
3. An explicit list of permitted operations, such as deposits, withdrawals,
   receipts, and payments.
4. Teller resolution by terminal ID rather than `FindTellerByEmployeeId`.
5. Posting against the configured POS G/L account.
6. Lower and upper range enforcement.
7. Shift opening, shift closing, and cash balancing.
8. Transaction reconciliation and reversal support.

Shared cash validation and journal-building logic should be extracted from the
current controllers into application services so Employee and In-house POS
channels use the same accounting rules.

## Agent POS requirements

Agent POS requires a separate float-based accounting model:

1. Agent organization, operator, and device registration.
2. Required float and commission customer accounts.
3. Cash-in and cash-out workflows.
4. Real-time available-float checks.
5. Customer authorization using an approved PIN, OTP, or external-channel
   mechanism.
6. Atomic posting between customer, clearing, agent float, and commission
   accounts.
7. Configurable customer charges and agent commissions.
8. Float top-up and float-withdrawal operations.
9. Per-transaction and daily agent limits.
10. Agent suspension and transaction blocking.
11. Settlement, reconciliation, exception handling, and reversal workflows.

`TellerAppService.FetchTellerBalances` must calculate Agent POS balances from
the float customer account rather than a teller G/L account.

## Teller configuration changes

The API and client must apply type-specific validation:

| Field | Employee | ATM | In-house POS | Agent POS |
|---|---:|---:|---:|---:|
| Employee | Required | Not applicable | Optional/operator-specific | Not applicable |
| Cash G/L account | Required | Required | Required | Not the primary balance source |
| Shortage G/L | Required | Reconciliation policy | Required | Not applicable |
| Excess G/L | Required | Reconciliation policy | Required | Not applicable |
| Float customer account | Not applicable | Not applicable | Not applicable | Required |
| Commission account | Not applicable | Not applicable | Not applicable | Required |
| Lower/upper limits | Required | Required | Required | Apply to float limits |

Required changes include:

- send `Type`, not `TellerType`, from the client;
- render conditional fields based on the selected type;
- validate applicable fields in both the DTO/application service and client;
- correct `AddNewTeller` and `UpdateTeller` guards so they do not impose
  Employee requirements on other types;
- prevent type changes after transactions exist, or implement a controlled
  migration workflow;
- reject unsupported types at transaction entry points until their channel is
  explicitly enabled.

## Recommended delivery order

1. **In-house POS** — reuse existing transaction and G/L behavior after adding
   terminal identity, routing, shift close, and reconciliation.
2. **ATM Terminal** — add switch/card integration, cassette management,
   dispense confirmation, reversals, and settlement.
3. **Agent POS** — add the float/commission accounting model, agent controls,
   channel authentication, settlement, and reconciliation.

Until a type meets its acceptance criteria, hide or disable it in the teller
creation UI rather than allowing configuration that appears operational.

## Minimum acceptance criteria per enabled type

A teller type is operational only when all of the following are demonstrated:

- it can be created with correct type-specific validation;
- a terminal/operator can authenticate without an employee-only lookup;
- successful transactions create balanced journals exactly once;
- insufficient balance/float and configured limits are enforced server-side;
- timeouts and duplicate requests do not duplicate postings;
- failed external actions can be reversed safely;
- end-of-shift/day settlement and reconciliation are available;
- transactions are searchable and auditable by terminal reference;
- automated tests cover success, decline, duplicate, timeout, reversal, and
  concurrent-balance scenarios.

