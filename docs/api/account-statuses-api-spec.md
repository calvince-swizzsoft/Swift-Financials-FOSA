# Account Statuses API

Read-only Customer 360 inquiry implementing `WebApplication1/Areas/Account Statuses.md`
and the legacy Dashboard `AccountStatusesController` intent.

## Security

- Both endpoints require bearer authentication.
- The module exposes no POST, PUT, PATCH, or DELETE operation. It cannot be
  used to transact on an employee's own account.
- When the caller is an employee and the selected customer is another
  employee, the caller must belong to a role mapped to
  `SystemPermissionType.EmployeeCustomerAccountViewing`.
- The cross-employee check is enforced by the API and returns HTTP `403`; it
  is not merely a frontend visibility rule.

## Search customers

```http
GET /api/accounts/account-statuses/customers?text={text}&customerFilter=0&pageIndex=0&pageSize=20
```

Uses the established customer record provider, including its searchable
name, identity, payroll/employment and account-reference filters. `pageSize`
is limited to 100.

## Customer status overview

```http
GET /api/accounts/account-statuses/customers/{customerId}
```

Returns the standard `{ success, message, data }` envelope. `data` contains:

- `customer`, `accounts`, and `referees`
- `signatories`, `standingOrders`, and `alternateChannels`
- `unclearedCheques` and `fixedDeposits`
- `electronicFundsTransfers`
- `loansGuaranteed` and `loanGuarantors`
- `isEmployeeAccount` and `isOwnEmployeeAccount`

Account management history remains sourced from the existing read endpoint:

```http
GET /api/accounts/customer-accounts/{customerAccountId}/history
```

Specimen availability is represented by the customer's passport, signature,
and identity-card image identifiers. This endpoint does not duplicate or
embed potentially large image binaries.
