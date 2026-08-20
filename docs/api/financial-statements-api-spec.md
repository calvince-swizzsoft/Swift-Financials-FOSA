# Financial Statements API

Faithful REST exposure of the legacy financial-position procedures.

## Source of business behaviour

- `dbo.sp_FinancialSummary(@EndDate, @Type)`
  - `1`: Trial Balance
  - `2`: Income and Expenditure
  - `3`: Balance Sheet
- `dbo.sp_FinancialStatementBranch(@Enddate, @Branch)`: branch financial position.

The controller deliberately does not recalculate balances in C#. Account hierarchy,
sign handling, profit/loss inclusion, cost-centre fallback, zero-balance exclusion,
and end-of-day behaviour remain owned by the legacy stored procedures.

## Controller

`WebApplication1/Areas/Accounts/Controllers/FinancialStatementsController.cs`

Base route: `api/accounts/financial-statements`

All endpoints require bearer authentication and return the standard
`{ success, message, data }` envelope.

## Consolidated endpoints

```http
GET /api/accounts/financial-statements/trial-balance?endDate=2026-08-17
GET /api/accounts/financial-statements/income-expenditure?endDate=2026-08-17
GET /api/accounts/financial-statements/balance-sheet?endDate=2026-08-17
```

Compatibility endpoint retaining the legacy numeric type:

```http
GET /api/accounts/financial-statements/summary?endDate=2026-08-17&type=1
```

Each `data` object contains `statementType`, `statementName`, `endDate`,
`totalDebit`, `totalCredit`, `difference`, and `rows`. Rows contain:

`accountCode`, `accountName`, `parentCode`, `parentName`, `debit`, `credit`,
`costCenter`, `type`, and `typeName`.

## Branch endpoint

```http
GET /api/accounts/financial-statements/branch?endDate=2026-08-17&branchId={guid}
```

Rows preserve the branch procedure shape: `accountType`, `accountTypeCode`,
`shortCode`, `code`, and `balance`.

## Validation and failures

- Missing dates or branch IDs return `400`.
- A summary type outside `1..3` returns `400`.
- If a faithful procedure has not been installed in the selected database, the
  endpoint returns `501` with the missing procedure named explicitly.
- SQL execution timeout is 120 seconds because these procedures traverse the
  complete chart of accounts and aggregate journal history.

