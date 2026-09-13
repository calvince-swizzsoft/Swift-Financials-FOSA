# Known Issues

Latent bugs and bug *patterns* worth knowing about — either already fixed
somewhere and worth watching for elsewhere, or deliberately left in a broken
state and tracked here so it isn't forgotten before shipping.

## `Enum.IsDefined` type mismatch on byte-backed enum properties

**Status:** all currently-known occurrences fixed. Documented here as a
pattern to avoid reintroducing, not as an open bug.

`Enum.IsDefined(Type, object)` requires the *boxed type* of the second
argument to exactly match the enum's underlying type (`int` by default,
unless the enum is explicitly declared `: byte`/`: short`/etc.). Passing a
boxed `byte` against an `int`-backed enum throws at runtime:

```
System.ArgumentException: Enum underlying type and the object must be same
type or object must be a String. Type passed in was 'System.Byte'; the enum
underlying type was 'System.Int32'.
```

This surfaced because several DTO description-getters guard a cast with
`Enum.IsDefined` (to avoid throwing on out-of-range stored values) but the
backing property is `byte` (stored compactly), while the enum itself is a
plain `int`-backed enum — so the guard itself threw on every call.

**Fixed:**
- `Application.MainBoundedContext.DTO/AccountsModule/CustomerAccountDTO.cs`
  — 8 properties (`CustomerType`, `CustomerIndividualSalutation`,
  `CustomerIndividualGender`, `CustomerIndividualMaritalStatus`,
  `CustomerIndividualIdentityCardType`, `CustomerIndividualNationality`,
  `CustomerIndividualType`, `CustomerIndividualClassification`).
- `Application.MainBoundedContext.DTO/AccountsModule/BrokerRequestDTO.cs`
  and `BrokerRequestBindingModel.cs` — `Status` property
  (`BrokerRequestStatus`).

Fix applied in each case: cast the property to `(int)` before the
`Enum.IsDefined` call, e.g.
`Enum.IsDefined(typeof(CustomerType), (int)CustomerType)`.

**Audit scope:** the bug pattern was isolated to
`Application.MainBoundedContext.DTO` — that's the one project where option/
status values are stored as `byte` for compactness. A full sweep of every
other project (`Application.MainBoundedContext`,
`Infrastructure.Data.MainBoundedContext`, `DistributedServices.MainBoundedContext`,
`WebApplication1` and its API mirrors, presentation/shared projects, SQLCLR)
found zero occurrences — everywhere else, option/status values are passed
around as plain `int`.

**If you add a new byte-backed enum property with a `Description`-style
getter guarded by `Enum.IsDefined`, cast to `(int)` before the call** — this
is the mistake to not repeat.

## Maker-checker approval guard disabled (`WorkflowAppService`)

**Status:** open — must be re-enabled before shipping or before any real
maker-checker enforcement is relied on.

`Application.MainBoundedContext/AdministrationModule/Services/WorkflowAppService.cs`,
`ApproveWorkflowItem` (marked `// TODO(maker-checker)`): the guard requiring
the workflow item's approver to be a different user from its initiator is
currently commented out. Right now the same user can create *and* approve
their own workflow item — the sequential maker-checker control is not
actually enforced.

This was disabled deliberately during hands-on testing (no separate
maker/checker test accounts were provisioned at the time). The check itself
was not deleted, just commented out — re-enabling is a one-line uncomment of
the `if (IsUserLatestApproverOfWorkflowItemEntry(...))` guard immediately
above the `TODO(maker-checker)` comment.

## Company validation throws generic `InvalidOperationException`

**Status:** open — client-side validation currently prevents the known
company form errors, but the API must remain authoritative and still validates
every request.

`Application.MainBoundedContext/AdministrationModule/Services/CompanyAppService.cs`
projects `CompanyDTO` to `CompanyBindingModel`, calls `ValidateAll()`, then
throws a generic `InvalidOperationException` containing the joined validation
messages when `HasErrors` is true. This makes an expected caller-correctable
validation result appear as a thrown runtime exception in the debugger and
does not provide field-keyed validation errors.

`WebApplication1/Areas/Admin/Controllers/CompanyController.cs` currently
translates that exception into a standardized HTTP `400 VALIDATION_FAILED`
response and preserves the safe message. The React create/update company forms
also mirror the presently-known required-field, email, and international-mobile
checks so users receive feedback before submission.

The eventual fix is to replace the generic exception with the API's classified
validation mechanism and return structured `validationErrors` keyed by field.
Do not remove the AppService validation when doing that; client-side validation
is only a usability layer and can be bypassed.

## Customer registration images cannot access SQL FILESTREAM

**Status:** open infrastructure issue — customer data is committed, but image
storage can fail with `System.ComponentModel.Win32Exception: Access is denied`.

Customer registration stages images under `WebApplication1/App_Data`, then
`MediaAppService.PostImage` inserts the BLOB metadata and opens the SQL Server
FILESTREAM path through `SqlFileStream`. Staging succeeds, but the FILESTREAM
open is denied. The configured `BLOBStore` connection currently uses SQL
authentication; SQL FILESTREAM streaming access requires an appropriately
configured Windows security context and filesystem/share permissions for both
the IIS application-pool identity and SQL Server service.

The API now treats this as partial success: it returns the created customer and
an explicit image-storage warning so the operator does not retry registration
and create a duplicate customer. The underlying deployment fix is to configure
FILESTREAM access with Windows authentication and grant the relevant service
identities access to the FILESTREAM share/container. After that is verified,
remove any orphaned staging images from `WebApplication1/App_Data` through a
separate, deliberate cleanup.

## Check-off credit batches are not scoped to an employer

**Status:** open design issue — operators may create one check-off batch per
employer, but the data model does not record or enforce that convention.

`CreditBatch` has no direct `EmployerId`. An entry's employer can only be
derived through Customer Account → Customer → Station → Zone → Division →
Employer. Consequently, a single check-off batch can contain customers from
different employers, and payroll-number matching is not restricted to the
employer that supplied the remittance.

The intended future design is to add an explicit employer relationship to
check-off batches, restrict payroll-number matching to customers belonging to
the selected employer, and validate that every batch entry resolves to that
employer. It should also reconcile the employer's remitted batch total against
allocated member contributions while preserving discrepancy handling for
missing or ambiguous matches. Migration and backward-compatibility behavior
for existing batches must be defined before implementation.


## Budget control does not automatically prevent overspending

**Status:** open — documented on 2026-09-11; enforcement is not implemented by this change.

Budget appropriation stores allocations and exposes balance tracking. It must
not be relied on as a transaction-time spending/disbursement limit. The company
`EnforceBudgetControl` setting and `BudgetAppService.FetchBudgetBalance` exist,
but the active API/posting services do not wire that lookup into a universal
overspending guard. `WebApplication1/Areas/BackOffice/Controllers/LoanCaseController.cs`
explicitly leaves `BranchBudgetBalance` and `BranchCompanyEnforceBudgetControl`
unpopulated, so `LoanCaseDTO.ValidateBudgetBalance` does not run its intended check.

Future enforcement must be implemented in the AppService transaction/posting
path, using authoritative balances and appropriate concurrency control, covering
the intended expense and loan-disbursement routes. UI checks or enabling the
company flag alone are insufficient. No automatic spending block was added.

## G/L budget remaining balances reverse debit/credit signs

**Status:** fixed on 2026-09-11 in `BudgetAppService.FetchBudgetEntryBalances`.
Regression coverage: `tools/tests/Budget.Tests` (10 scenarios plus domain journal sign verification).

`BudgetAppService.FetchBudgetEntryBalances` previously subtracted the negative of
expense actuals, but subtracted income actuals without normalization. The actual
domain posting convention in `Journal.PostDoubleEntries` is debit-positive and
credit-negative. Given raw signed G/L balances, remaining expense budget must
be `Amount - ActualToDate`; remaining income target must be `Amount + ActualToDate`.
Do not use absolute values: refunds and reversals must restore/reduce usage.

A read-only regression probe invoked the compiled AppService with a stub SQL
balance provider and verified real domain journal signs:

| Case | Budget | Signed G/L actual | Previous remaining | Correct remaining |
| --- | ---: | ---: | ---: | ---: |
| Expense debit | 15,000 | 4,000 | 19,000 | 11,000 |
| Income credit | 15,000 | -4,000 | 19,000 | 11,000 |
| Expense refund (net credit) | 15,000 | -1,000 | 14,000 | 16,000 |

The query currently requests the branch, account, posting period and today's
cutoff using CreatedDate. Effective-date treatment for backdated transactions
is unchanged by this sign fix and remains a separate follow-up.
`ActualToDate` still exposes the raw signed ledger balance; only budget usage is
normalized. The missing procedure below still blocks database retrieval locally.

## Required G/L budget balance stored procedure is missing locally

**Status:** installation pending — `docs/database/install-gl-budget-balance.sql` now provides the procedure (2026-09-12). Verified with temporary SQL fixtures for branch/account/period isolation, signed amounts, both date filters, full end-date inclusion, empty results and invalid filters. The script has not been installed in the application database.

Originally verified against the configured `SwiftFin_Dev` connection
(database `SwiftFinancialsDB_Live`) on 2026-09-11. Other deployments were not checked.

`SqlCommandAppService.FindGlAccountBalance(branchId, chartOfAccountId,
postingPeriodId, cutoff, dateFilter, header)` executes
`dbo.sp_GetGlAccountBalanceByBranchAndPostingPeriod`. A read-only `sys.procedures`
check returned no such procedure; database VIEW DEFINITION permission was
confirmed. Thus actual G/L budget balance retrieval cannot complete through
this dependency in the checked database. The procedure was absent from the checked-in SQL scripts at verification time.

Restore a version-controlled implementation or replace the dependency with an
AppService-owned query, then verify branch/period isolation, effective dates,
signed amounts and the budget arithmetic together. The sign probe above used
a stub balance provider; it was not an end-to-end database budget calculation.
No database objects or ledger data were changed during verification.
