# Budget API

Base route: `/api/accounts/budgets`; authentication required.
Budget Periods browses saved budgets and their allocations/actual balances.
Budget Appropriation originates and edits budgets. Saving does not post journals
or enforce spending limits.

GET `/`: text, zero-based pageIndex and pageSize (1–100; default 20).
GET `/all`: all budget headers. GET `/{id}`: one header.
GET `/{id}/entries?includeBalances=false`: allocations without querying actuals.
The default `includeBalances=true` calculates actual and remaining balances.
Editing should request false so it is independent of balance reporting setup.

POST `/` creates; PUT `/{id}` replaces header and all allocation lines.
Both accept `{ Budget: { Description, TotalValue, PostingPeriodId, BranchId },
Entries: [{ Type, ChartOfAccountId, LoanProductId, Amount, Reference }] }`.
Send the unused target as null, not an empty GUID string. POST ignores a supplied
header ID; PUT uses the route ID. Entry IDs are regenerated on replacement.

The AppService validates before writes and commits header and entries together
under a serializable transaction. Validation includes:

- Name: 1–256 characters after trimming; reference: at most 256 characters.
- Positive amounts, at most two decimal places, maximum 9,999,999,999,999.99.
- At least one line and an exact decimal sum equal to TotalValue.
- Existing branch and posting period; one budget per branch and period.
- Type 0: an existing leaf income/expense G/L account (4000/5000), no loan product.
- Type 1: an existing loan product, no G/L account.

Success returns `{ success: true, message, data }`.
Errors use the standard API envelope with message, error.code,
error.validationErrors and correlationId:

- 400 VALIDATION_FAILED: header/line/amount/target errors; line keys are zero-based
  (`Entries[0].Amount`) and readable messages use Line 1.
- 404 NOT_FOUND: selected budget no longer exists.
- 409 BUDGET_ALREADY_EXISTS: edit the existing branch/period budget.
- 409 BUDGET_CONFLICT: concurrent database conflict; refresh before retrying.
- 503 BUDGET_BALANCES_UNAVAILABLE: missing balance procedure. Allocation editing
  remains available; install `docs/database/install-gl-budget-balance.sql`.
- 500 BUDGET_OPERATION_FAILED: unexpected error logged server-side; no raw database
  or exception details returned. Check saved state before retrying after an
  uncertain response. The client retains its form on errors.

Regression coverage: tools/tests/Budget.Tests checks financial signs, invalid
headers/lines, mismatched totals, duplicates, missing targets, create/update save
ordering and failure before commit. Repository/scope test doubles are used;
these tests do not post transactions or change the application database.


## Budget versus actual report

GET `/{id}/actuals?asAt=YYYY-MM-DD` returns `{ Budget, AsAt, Lines, Totals }`
inside the success envelope. AsAt is required and must fall within the budget's
posting-period dates. Branch and posting period come from the saved budget.

Full-period allocations are compared against period-start through the full
AsAt day. G/L actuals use journal ValueDate (CreatedDate fallback), with both
branch and PostingPeriodId filters. Income credits are normalized positive;
expense debits are positive; refunds keep their signs. Loans sum DisbursedAmount
by recorded DisbursedDate and branch over the same dates. Loan cases do not have
PostingPeriodId, so their period boundary is date-based. This measures recorded
loan-case disbursements, not journal-adjusted loan balances.

Repeated allocations to the same target/section are grouped before adding actuals.
Unbudgeted accounts/products with nonzero activity are included with Budget=0.
Sections are Income, Expenses, and Loan disbursements. Lines include TargetId,
Code, Description, Budget, Actual, Difference, Percentage, and Unbudgeted.
Difference is Budget minus Actual. Percentage is Actual/Budget as a fraction,
or null when Budget=0. Section totals are calculated server-side, with no combined
income/expense/disbursement grand total. Invalid legacy targets return validation
errors rather than silently disappearing from the report.

The React report is linked from Budget Periods/Appropriation and available at
`/Accounts/BudgetManagement/Actuals`. Its Excel export uses the exact loaded report
snapshot, including filters, all rows, section totals, numeric formats and cached
formulas for differences, percentages and totals. Export is unavailable while
loading, on errors, or after the filter changes until the new report is loaded.
The new report uses AppService-owned parameterized queries, so it does not depend
on either of the legacy budget balance stored procedures.
