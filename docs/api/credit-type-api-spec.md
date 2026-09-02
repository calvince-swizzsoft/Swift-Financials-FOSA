# Credit Type API

Credit Types configure salary, dividend, casual-pay and similar credit flows.
The domain aggregate is `CreditType`; all persistence and relationship rules
are owned by `ICreditTypeAppService`. The API controller only translates HTTP
requests and delegates to focused AppServices.

Base route: `api/accounts/credittypes` (Bearer authentication required).

## Endpoints

- `GET /` — unpaged list for selectors.
- `GET /paged?text=&pageIndex=0&pageSize=20` — searchable administration list.
- `GET /{id}` — one credit type.
- `POST /` — create the header and replace all supplied relationships.
- `PUT /{id}` — update the header and replace all supplied relationships.
- `GET /{id}/configuration` — commissions, direct debits, attached products,
  and concession-exempt products.
Direct-debit selector data comes from its focused `api/accounts/directdebits`
API; Credit Types only owns the selected relationship IDs.

Success uses `{ success, message, data }`.

## Save request

```json
{
  "CreditType": {
    "Description": "Salary",
    "ChartOfAccountId": "00000000-0000-0000-0000-000000000000",
    "TransactionOwnership": 0,
    "IsLocked": false
  },
  "Commissions": [{ "Id": "..." }],
  "DirectDebits": [{ "Id": "..." }],
  "AttachedProducts": {
    "LoanProductCollection": [{ "Id": "..." }],
    "InvestmentProductCollection": [{ "Id": "..." }],
    "SavingsProductCollection": [{ "Id": "..." }]
  },
  "ConcessionExemptProducts": {
    "LoanProductCollection": [{ "Id": "..." }]
  }
}
```

`TransactionOwnership`: `0` = Beneficiary Branch (Customer), `1` = Initiating
Branch (Employee). Relationship arrays use full-replace semantics.
