# Direct Debit API

Direct Debits are reusable one-off deduction definitions applied during a
Credit Batch. They do not execute independently. Domain construction uses
`DirectDebitFactory`, `CustomerAccountType`, and `Charge`; validation and
persistence are canonical in `IDirectDebitAppService`.

Base route: `api/accounts/directdebits` (Bearer authentication required).

- `GET /` — all direct debits for selectors.
- `GET /paged?text=&pageIndex=0&pageSize=20` — searchable administration list.
- `GET /{id}` — one direct debit.
- `POST /` — create.
- `PUT /{id}` — update.

Write fields:

- `Description` — required and unique.
- `CustomerAccountTypeProductCode` — `1` Savings, `2` Loan, `3` Investment.
- `CustomerAccountTypeTargetProductId` — must exist in the selected category.
- `CustomerAccountTypeTargetProductCode` — the selected product's code.
- `ChargeType` — `1` Percentage or `2` Fixed Amount.
- `ChargePercentage` — greater than 0 and at most 100 for percentage charges.
- `ChargeFixedAmount` — greater than 0 for fixed charges.
- `IsLocked` — administrative status.

Responses use `{ success, message, data }`. Duplicate names return `409`.
