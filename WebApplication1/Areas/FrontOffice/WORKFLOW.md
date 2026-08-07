# Front Office — Functional Workflow

Audience: anyone building or reviewing the `Areas/FrontOffice` API surface
who needs to understand *what the front office is supposed to do*, not just
what one endpoint returns. This is a functional/process reference, not an
API spec — see `docs/api/frontoffice-api-spec.md` for request/response
shapes.

Source of truth:
- Functional design: reference MVC app,
  `SwiftFinancials.Web/Areas/FrontOffice/{Controllers,Views}/*` (read-only,
  sibling checkout — see root `CLAUDE.md`).
- Domain aggregates: `Domain.MainBoundedContext/FrontOfficeModule/Aggregates/*`
  and `Domain.MainBoundedContext/AccountsModule/Aggregates/TellerAgg/Teller.cs`.
- App services: `Application.MainBoundedContext/FrontOfficeModule/Services/*`.
- This repo's API controllers: `WebApplication1/Areas/FrontOffice/Controllers/*`.
- Enums referenced throughout:
  `Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs`.

## 1. What "Front Office" means here

Everything that touches **physical/counter cash or cheques and their GL
posting**: a teller taking a deposit or paying out a withdrawal, treasury
moving cash to/from the bank and out to tellers, cheques being banked and
cleared, and the daily till close-out. This is distinct from the
`Registry`/`Admin` back-office modules (customer records, products, company
config) already documented elsewhere in `CLAUDE.md`.

There is **no separate "front office session" concept** (no login-to-till,
no explicit open-till action). A teller is a standing GL-linked identity
(`Teller` aggregate); anyone assigned to it can transact against it every
day until it is administratively locked, and the *day* boundary is enforced
by the End of Day process (§8), not by a session object.

## 2. End-to-end functional map

```mermaid
flowchart TD
    subgraph setup["1 · Setup (admin, infrequent)"]
        A1["Create Treasury\n(branch cash vault)"]
        A2["Create Teller\n(GL wiring by TellerType,\nassign Employee)"]
    end

    subgraph daily["2 · Daily transaction cycle (per teller, repeats all day)"]
        B1["Customer presents at counter"]
        B2["Resolve customer account,\ncompute tariffs, classify"]
        B3{"Within limits?"}
        B4["Post directly to GL"]
        B5["Raise authorization request\n(maker step)"]
        B6["Checker authorizes / rejects"]
        B7["Post / pay authorized request\n(poster step)"]
        B8["Print receipt, send SMS"]
    end

    subgraph treasury["3 · Treasury cash movement (as needed)"]
        C1["Bank ⇄ Treasury ⇄ Teller\ncash movement + denomination count"]
    end

    subgraph cheques["4 · Cheque lifecycle (as needed)"]
        D1["Cheque deposited\n(part of §2)"]
        D2["Batch-transfer cheques\n(must precede EOD)"]
        D3["Bank selected cheques"]
        D4["Clear / unpay cheques"]
        D5["Automated image clearing\n(truncated cheques, parallel path)"]
    end

    subgraph eod["5 · End of Day (once per teller per day)"]
        E1["Cheques transferred?\nNot already closed today?"]
        E2["Count denominations"]
        E3["Compute Balanced /\nShortage / Excess"]
        E4["Post EOD journal\n(+ suspense entry if unbalanced)"]
        E5["Print EOD receipt"]
    end

    subgraph ancillary["6 · Ancillary / periodic processes"]
        F1["Account closure\n(Create→Verify→Approve→Settle)"]
        F2["Expense payable\n(Create→Verify→Approve)"]
        F3["Fixed deposit\n(Create→Verify→Terminate/Liquidate)"]
        F4["Sundry payments /\ncredit batches"]
        F5["In-house cheque\nissuance + printing"]
    end

    A1 --> A2 --> B1
    B1 --> B2 --> B3
    B3 -- yes --> B4 --> B8
    B3 -- no --> B5 --> B6
    B6 -- authorized --> B7 --> B8
    B6 -- rejected --> B1
    B4 -.-> D1
    B7 -.-> D1
    D1 --> D2 --> D3 --> D4
    D1 -.parallel path.-> D5
    treasury -.funds teller.-> daily
    daily --> E1
    D2 --> E1
    E1 --> E2 --> E3 --> E4 --> E5
    daily -.-> ancillary
```

## 3. Actors

| Actor | Role |
|---|---|
| Teller | Maker — enters transactions at the counter, holds a GL-linked `Teller` identity |
| Supervisor / checker | Authorizes above-limit deposits, withdrawals, transfers, EOD variances go to suspense automatically (no separate approval) |
| Treasury officer | Moves cash between bank, vault (treasury), and tellers |
| Back-office verifier/approver | Second/third maker-checker stage on account closure and expense payables (distinct from the teller's own checker) |
| Customer | Initiates deposits, withdrawals, cheque deposits, account closure requests |

## 4. Teller & till model

`Teller` (`Domain.MainBoundedContext/AccountsModule/Aggregates/TellerAgg/Teller.cs`)
is the only real invariant-bearing object here:

- `Type` (`TellerType`: InhousePointOfSale / ATM / AgentPointOfSale) drives
  which chart-of-account the till's cash is wired to at creation time.
- `ChartOfAccountId` — the till's cash GL account.
- `ShortageChartOfAccountId` / `ExcessChartOfAccountId` — suspense accounts
  that automatically absorb EOD variances (§8).
- `Range` — a lower-limit value object; any deposit/withdrawal that would
  push the till's book balance below `RangeLowerLimit` is blocked outright.
- `IsLocked` (`Lock()` / `UnLock()`) — a locked teller cannot post anything;
  every transaction path checks this before touching the GL account.

There is no "open till for the day" step — a teller is simply available
(not locked) or not. The daily boundary is entirely owned by whether
`IsEndOfDayExecutedAsync` has already run for that teller today (§8).

## 5. Core transaction cycle — deposit / withdrawal / cheque deposit

One screen (old repo: `CashDepositController.Create`; this repo: unified
`CashDepositController` under `api/frontoffice/requests`) handles all three
transaction types via `FrontOfficeTransactionType`
(`CashDeposit`, `CashWithdrawal`, `ChequeDeposit`, `CashWithdrawalPaymentVoucher`).

```mermaid
sequenceDiagram
    actor C as Customer
    participant T as Teller (maker)
    participant S as System (app services)
    participant K as Checker/Supervisor

    C->>T: Presents account + transaction
    T->>S: Resolve current teller, posting period,\ncustomer account, tariffs
    S-->>T: Classification (see rules below)
    alt Within limits
        T->>S: Post directly\n(AddJournalWithCustomerAccountAndTariffsAsync)
        S-->>T: Journal posted
        T->>C: Receipt + SMS notification
    else Above limit / below minimum / overdraft / payment voucher
        T->>S: PlaceCash{Deposit,Withdrawal}AuthorizationRequestAsync\n(status: Pending)
        S-->>K: Appears in approval queue
        K->>S: AuthorizeCash{Deposit,Withdrawal}Request\n(status: Authorized | Rejected)
        alt Authorized
            T->>S: ProcessAuthorized / PayAuthorized request\n(status: Posted | Paid)
            S-->>T: Journal posted
            T->>C: Receipt + SMS notification
        else Rejected
            S-->>T: Request closed, nothing posted
        end
    end
```

**Classification rules** (enforced in the transaction-posting code, not as
named domain methods):

| Transaction | Blocked outright when | Requires authorization (maker-checker) when | Posts directly when |
|---|---|---|---|
| Cash deposit | Teller is locked | Amount > product's maximum allowed deposit (`CashDepositCategory.AboveMaximumAllowed`) | Amount ≤ product maximum (`WithinLimits`) |
| Cash withdrawal | Teller book balance < amount (till doesn't have the cash); would breach `Teller.RangeLowerLimit` | Above product maximum, below customer minimum balance, would overdraw, or is a payment voucher (`AboveMaximumAllowed`/`BelowMinimumBalance`/`Overdraw`/`PaymentVoucher`) | Within product limits and doesn't breach minimum balance (`WithinLimits`) |
| Cheque deposit | — | — | Always posts directly; creates `ExternalCheque` + `ExternalChequePayable`, then feeds the cheque lifecycle (§7) |

`CashDepositRequestAuthStatus` / `CashWithdrawalRequestAuthStatus` are the
underlying state machines behind the "requires authorization" branch:

```mermaid
stateDiagram-v2
    [*] --> Pending: maker places request
    Pending --> Authorized: checker approves
    Pending --> Rejected: checker rejects
    Authorized --> Posted: teller posts (deposit)
    Authorized --> Paid: teller pays out (withdrawal,\nincl. Payment Voucher sub-flow)
    Rejected --> [*]
    Posted --> [*]
    Paid --> [*]
```

A payment-voucher withdrawal is a distinct posting path
(`PayCashWithdrawalRequestAsync` with a validated `PaymentVoucherDTO`)
rather than a straight cash payout — used when the withdrawal is settled by
issuing a voucher instead of physical cash.

## 6. Treasury cash movement

Treasury (`Treasury` aggregate — the branch's cash vault) moves cash in four
directions, all through one screen (`CashManagementController.Create`,
`TreasuryTransactionType`): `BankToTreasury`, `TreasuryToBank`,
`TreasuryToTeller`, `TreasuryToTreasury`. Outgoing transfers are blocked if
`ActiveTreasury.BookBalance` is insufficient. Every movement — and every EOD
close (§8) — is recorded with a **denomination breakdown**
(`FiscalCountDTO`: 1000/500/200/100/50/40/20/10/5/1/50-cent note+coin
counts), not just a total, so physical cash counted at the counter can be
reconciled against the GL figure.

```mermaid
flowchart LR
    Bank((Bank)) <--> Treasury[(Treasury\nvault)]
    Treasury <--> Teller1[Teller A]
    Treasury <--> Teller2[Teller B]
    Treasury <--> Treasury2[Other branch\ntreasury]
```

## 7. Cheque lifecycle

```mermaid
flowchart LR
    Deposit["Cheque deposited\nat counter (§5)"] --> Transfer["Batch-transfer\nuntransferred cheques"]
    Transfer --> Bank2["Bank selected\ncheques"]
    Bank2 --> Clear["Clear or unpay\n(with reason code\nif dishonored)"]
    Deposit -. parallel path .-> Image["Upload scanned image\n(ElectronicJournal)"]
    Image --> Processing["Automated/image\nclearing queue"]
```

A teller **cannot close their day** (§8) while they still hold untransferred
cheques — the transfer batch step is a hard EOD precondition, not optional
housekeeping.

## 8. End of Day close

The one real state machine in the front office, enforced in order:

```mermaid
stateDiagram-v2
    [*] --> CheckCheques
    CheckCheques --> Blocked_HasCheques: TellerTotalCheques != 0
    CheckCheques --> CheckAlreadyClosed: cheques all transferred
    Blocked_HasCheques --> [*]: "transfer your cheques first"

    CheckAlreadyClosed --> Blocked_AlreadyClosed: EOD already run today
    CheckAlreadyClosed --> CountCash: not yet closed today
    Blocked_AlreadyClosed --> [*]: "already closed your day"

    CountCash --> Reconcile: denomination count entered
    Reconcile --> Balanced: counted == book balance
    Reconcile --> Shortage: counted < book balance
    Reconcile --> Excess: counted > book balance

    Balanced --> PostJournal
    Shortage --> PostJournal: + suspense debit to\nTeller.ShortageChartOfAccountId
    Excess --> PostJournal: + suspense credit to\nTeller.ExcessChartOfAccountId

    PostJournal --> PrintReceipt
    PrintReceipt --> [*]
```

Shortages/excesses are posted automatically to the teller's own dedicated
suspense GL accounts — there is no manual out-of-band adjustment step for an
unbalanced till.

## 9. Ancillary / periodic processes

These sit in the same area but run far less often than the daily cycle, and
each has its own multi-stage maker-checker shape:

| Process | Stages | Purpose |
|---|---|---|
| Account closure | Create → **Approve** → **Verify** → **Settle** | Pays out remaining balance and closes a customer account (4 stages — the only front-office flow with a distinct approve *and* verify step, plus a final settlement). Note: this is the order `AccountClosureRequestAppService` actually enforces via its status gating (Approve only accepts Registered/Deferred, Verify/Audit only accepts Approved, Settle only accepts Audited) — the reference MVC controller's action naming order (Create→Verify→Approve→Settle) does not match |
| Expense payable | Create → Verify → Approve | Petty-cash / expense voucher with multiple GL lines (header + entries pattern) |
| Fixed deposit | Create → Verify → **Terminate** (early, batch-capable) or **Liquidate** (at maturity, batch-capable) | Counter-originated fixed deposit product lifecycle |
| Sundry payments / credit batches | Single-stage entry | General GL voucher postings; also surfaces `CreditBatchType.Payout`/`CheckOff` batch entries (e.g. payroll check-off) |
| Customer receipts | Single-stage entry | Free-form multi-line GL receipt/voucher not tied to a specific account transaction type |
| In-house cheque | Build batch → Print preview → Print | SACCO-issued outward cheques (e.g. loan disbursement payouts), with payee/GL-account lookups and a PDF cheque template |

## 10. Implementation status in this repo

All 14 functional areas now have a live `ApiController`. Every controller
requires `[Authorize]` (a valid JWT); the six that were `[AllowAnonymous]`
with wildcard CORS, plus `CashDepositController` (anonymous by omission),
were locked down as part of the fidelity pass described in §11.

Treasury *master data* (create/list/update a `Treasury` vault itself) isn't
in this table — despite the reference app's version living under
`Areas/FrontOffice/Controllers/TreasuryController.cs`, it's pure admin
CRUD with no teller/cash-cycle behavior, so this repo's equivalent now lives
at `Areas/Accounts/Controllers/TreasurysController.cs`
(`api/accounts/treasurys` — see `docs/api/treasury-api-spec.md`). Treasury
*cash movement* (the row below) stays here — that's the actual front-office
behavior.

| Functional area | Old MVC controller (reference) | This repo | Notes |
|---|---|---|---|
| Teller master data | `TellerController` | `Areas/FrontOffice/Controllers/TellerController.cs` | Live |
| Deposit/withdrawal transaction + request queue | `CashDepositController`, `CashWithdrawalController`, `CashWithdrawalRequestController` | `Areas/FrontOffice/Controllers/CashDepositController.cs` (unified) | Live. Maker-checker fully routes through the generic `Workflow` engine now (§11) |
| Treasury cash movement | `CashManagementController` | `Areas/FrontOffice/Controllers/CashManagementController.cs` | Live |
| Cash transfer requests + cheque transfer batch | `CashTransferController`, `TransfersController` | `Areas/FrontOffice/Controllers/TransfersController.cs` (both folded in) | Live |
| Cheque banking & clearance | `ChequesController` | `Areas/FrontOffice/Controllers/ChequesController.cs` | Live |
| End of day close | `EndOfDayController` | `Areas/FrontOffice/Controllers/EndOfDayController.cs` | Live — local-printer `PrintReceipt` removed, journal returned in `data` for client-side receipt rendering |
| Automated/image cheque clearing | `AutomatedClearingController` | `Areas/FrontOffice/Controllers/AutomatedClearingController.cs` | Live |
| Account closure | `AccountClosureController` | `Areas/FrontOffice/Controllers/AccountClosureController.cs` | Live |
| Expense payable | `ExpensePayableController` | `Areas/FrontOffice/Controllers/ExpensePayableController.cs` | Live — Verify enqueues into the generic `Workflow` engine (was already wired in `WorkflowProcessorAppService`, just never enqueued into) |
| Fixed deposit | `FixedDepositController` | `Areas/FrontOffice/Controllers/FixedDepositController.cs` | Live |
| Sundry payments / credit batches | `SundryPaymentsController` | `Areas/FrontOffice/Controllers/SundryPaymentsController.cs` | Live — single-line GL voucher only (see §11, credit-batch-entry queue not reproduced) |
| Customer receipts | `CustomerReceiptsController` | `Areas/FrontOffice/Controllers/CustomerReceiptsController.cs` | Live — single-line only, multi-account apportionment not reproduced (§11, real gap) |
| In-house cheque issuance/printing | `InHouseController` | `Areas/FrontOffice/Controllers/InHouseController.cs` | Live — printing posts status+journal only, no local print driver |
| Standalone fiscal count CRUD | `FiscalCountController` | `Areas/FrontOffice/Controllers/FiscalCountController.cs` | Live |

Domain aggregates and app services for every row above already existed,
byte-identical to the old repo, under `Domain.MainBoundedContext/FrontOfficeModule`
and `Application.MainBoundedContext/FrontOfficeModule/Services` before any of
this controller work started — none of it required app-service or domain
changes, only new `ApiController`s (plus the `Workflow`-engine wiring fix in
§11, which touched an existing controller, not the domain layer).

Full request/response reference: `docs/api/frontoffice-api-spec.md`.

## 11. Fidelity pass — what was fixed (2026-08-06)

A deeper pass compared what was live against both the reference app's
*intent* and this repo's own web-API conventions, and found the front
office was a half-migration. Fixed, on branch `feature/frontoffice-fidelity`:

- **Maker-checker consistency.** Cash *deposit* requests already enqueued
  into the generic `Workflow` engine on creation; cash *withdrawal*
  requests never did, despite `WorkflowProcessorAppService` already having
  a ready `CashWithdrawalRequestAuthorization` case. Both now enqueue.
  `ExpensePayableController.Verify` does the same for expense payables.
  The ad-hoc `POST .../authorize` endpoint on `CashDepositController`
  (which bypassed the `Workflow` engine — no multi-level approval
  counting, no audit row) was removed; checkers approve through the
  existing generic `POST /api/administration/workflows/items/approve`.
- **Teller identity.** `TransfersController`/`EndOfDayController` resolved
  "current teller" from a hardcoded GUID; both now read the `EmployeeId`
  claim off the caller's JWT, like `CashDepositController` already did.
  `EndOfDayController.Create` no longer trusts a client-supplied
  `TellerId` either.
- **Auth.** `[Authorize]` added everywhere; wildcard CORS/`[AllowAnonymous]`
  removed (was local-testing scaffolding — pending cash-request data was
  readable with zero authentication before this).
- **Receipts.** Local-printer code (`System.Drawing.Printing` against a
  hardcoded printer name, only reachable from the same machine as the
  process) removed from `EndOfDayController`. Deposit/withdrawal/EOD
  posting endpoints now return the full `JournalDTO` under `data`,
  which is what the React client renders/prints from — see
  `docs/api/frontoffice-api-spec.md` for the exact fields.
- **Web-consumption shape.** Every list endpoint now returns real paging
  (`PageCollectionInfo<T>`) instead of a bare unpaged array, and every
  response uses the standard `{ success, message, data }` envelope.
- **Known, documented gap, not silently dropped:**
  `CustomerReceiptsController` posts a single-line GL voucher only. The
  reference MVC controller split one receipt total across multiple
  chart-of-account lines (`AddJournalWithApportionmentsAsync`), but
  `IJournalAppService` has no apportioned-posting overload in this repo —
  adding real multi-line apportionment support is app-service/domain
  work, not a controller-layer port. Same caveat applies to
  `SundryPaymentsController`'s credit-batch-entry queue (`CreditBatchType`
  browse/pickup screens) — not reproduced, since the underlying batch
  browse endpoints live outside `IJournalAppService`/`ITellerAppService`
  and weren't in scope for this pass.
- Two reference-controller bugs fixed rather than ported forward:
  `AutomatedClearingController` and `FiscalCountController`'s reference
  MVC grid actions both called `FindTreasuriesByFilterInPageAsync` (the
  wrong service — copy-paste from a sibling controller) and, in
  `AutomatedClearingController`'s case, never even returned the query
  result to the view. Both now call their correct respective app services.
