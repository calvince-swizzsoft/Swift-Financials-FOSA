# Bank reconciliation API

Authenticated base route: `/api/accounts/bank-reconciliations`.
Successful responses use `{ success: true, message, data }` (balance responses omit message).

| Method | Route | Input / result |
| --- | --- | --- |
| GET | `/balance` | `bankLinkageId`, `endDate` (ISO date); returns decimal end-date G/L balance |
| GET | `/periods` | `text`, zero-based `pageIndex`, `pageSize` (default 20); paged periods |
| GET | `/periods/all` | All periods; summary fields are not populated by list endpoints |
| GET | `/periods/{id}` | Period with complete reconciliation totals |
| POST | `/periods` | BankReconciliationPeriodDTO; creates an open period |
| PUT | `/periods/{id}` | BankReconciliationPeriodDTO; updates an open period |
| GET | `/periods/{id}/entries` | `text`, zero-based `pageIndex`, `pageSize` (1–100, default 20); PageCollection and ItemsCount |
| POST | `/periods/{id}/entries` | BankReconciliationEntryDTO; adds to an open period |
| DELETE | `/periods/{periodId}/entries/{entryId}` | Removes an entry belonging to that open period |
| POST | `/periods/{id}/close` | `{ AuthOption: 1, AuthorizationRemarks }` posts/closes; option 2 suspends/rejects |

## Period balances and dates

Creation/update requires BankLinkageId, PostingPeriodId, DurationStartDate,
DurationEndDate, and BankAccountBalance; Remarks is optional. The AppService
resolves BranchId, ChartOfAccountId, and BankAccountNumber from the bank linkage,
and reads GeneralLedgerAccountBalance itself. Client-supplied identity/balance
values do not override that resolution. Same-day periods are valid.

The G/L balance is the sum of signed journal-entry amounts for the bank's G/L
account and branch, using ValueDate (falling back to CreatedDate when null),
strictly before the day following DurationEndDate. This is a cumulative closing
balance, not movement since DurationStartDate. Open-period detail requests and
closing refresh it. A closed period retains its pre-adjustment snapshot so its
own posted adjustments are not counted twice in the summary.

Detail and creation responses include EntryCount, BankAdjustments,
GeneralLedgerAdjustments, AdjustedBankBalance, AdjustedGeneralLedgerBalance,
and UnreconciledBalance. These cover ALL saved entries, regardless of the page
being displayed. List endpoints do not calculate these summary fields.

## Adjustment types

Value must be positive. ChartOfAccountId is the contra account.

| AdjustmentType | Reconciliation effect | Journal on closing |
| --- | --- | --- |
| 0 BankAccountDebit | Increase statement balance | None; timing/explanatory item |
| 1 BankAccountCredit | Decrease statement balance | None; timing/explanatory item |
| 2 GeneralLedgerAccountDebit | Increase bank G/L balance | Debit bank, credit contra |
| 3 GeneralLedgerAccountCredit | Decrease bank G/L balance | Credit bank, debit contra |

Types 2/3 require an existing leaf/posting contra account different from the bank
G/L account. Types 0/1 discard a supplied contra when added and never post a
journal. Optional cheque fields and Remarks remain supported.

UnreconciledBalance = (BankAccountBalance + BankAdjustments) minus
(GeneralLedgerAccountBalance + GeneralLedgerAdjustments).

For example, a statement of 950 and ledger of 1,000 with an unrecorded bank
charge of 50 requires type 3 with the bank-charges expense account as contra.
Closing credits the bank 50 and debits that expense 50.

## Closing guarantees and errors

The AppService revalidates all entries and fresh balances under a serializable
transaction. An absolute difference of 0.005 or more prevents closing. Only G/L
adjustments create journals, dated exactly DurationEndDate and carrying the
BankReconciliation transaction code. Journal persistence and the status change
share one transaction. A posting failure rolls back; an already closed or
suspended period cannot post again. Rejection changes status without posting.

Malformed inputs return 400, missing periods 404, and expected reconciliation
validation conflicts 409 with a safe message. Unexpected errors follow the API's
normal exception handling.

This correction does not migrate or reverse historical closed reconciliations.
Review existing open adjustments against the type definitions before closing.
There is no bank-statement import or automatic matching in this workflow.

## Verification

`tools/tests/BankReconciliation.Tests` runs without database writes and verifies
posting directions using real journal double-entry logic, selected posting dates,
all-entry totals above 100 rows, SQL date/account parameters, rejection of invalid
adjustments, posting-failure ordering, and repeat-close prevention. Repository and
scope dependencies are test doubles; it is not a SQL transaction integration test.


### Adjustment request binding fix — 13 September 2026

`POST periods/{id}/entries` binds an `AddBankReconciliationEntryRequest` containing only AdjustmentType, ChartOfAccountId, Value, ChequeNumber, ChequeDrawee, ChequeDate and Remarks. The route supplies BankReconciliationPeriodId when constructing the AppService DTO. A body period ID, if present from older clients, cannot override the route. This avoids rejecting a valid request with the DTO's ValidGuid validator before the route period was assigned.

The frontend sends an empty optional ChequeDate as null and validates positive amounts and a contra account for G/L adjustments before submission. Server DTO and AppService checks remain in force. Invalid model binding now uses the standard error envelope, including field details and support reference, instead of the generic “Invalid reconciliation request.” message. Raw parser exceptions are not returned.

Verified with a contract regression reproducing the original missing-period failure, route precedence and invalid-value rejection, and frontend payload tests. Testing did not post adjustments or journals.


### Contra-account persistence correction — 13 September 2026

The domain entry factory now retains ChartOfAccountId for G/L debit/credit adjustments (2/3) and leaves it null for bank-side timing differences (0/1). Previously its switch assigned the contra to the wrong adjustment types, discarding valid G/L contra selections after AppService validation. Optional cheque details are now retained for all four types. Regression tests create entries through the real factory and close them, checking the resulting contra journal lines and no-posting behavior for timing differences.

Previously saved G/L entries with a missing contra cannot be reconstructed automatically: the selected account was never persisted. Remove and re-add those entries in an open period with the intended contra before closing. The UI identifies such entries instead of displaying an ambiguous dash. All adjustment inputs now include explanatory popovers.
