# Debit Types

Canonical implementation: `DebitTypeAppService` / `IDebitTypeAppService`,
`DebitType` and `DebitTypeCommission` domain aggregates and factories.
The controller delegates all persistence and configuration validation to AppService.

A Debit Type defines a name, one target CustomerAccountType (product category,
product Id and product code), IsMandatory and IsLocked. Its commissions determine
the tariffs. It has no independent fixed amount, G/L account or credit-type-style
attached-product collections. Product accounting uses the target product configuration.
There is no canonical delete operation; use Locked to retire a type.

Debit batches use these types to compute tariffs. Customer registration applies
company-configured and selected additional debit types. Company associations are
managed through the existing company debit-types sub-resource.

## API

Authenticated base: `/api/accounts/debittypes`.

| Method | Path | Operation |
|---|---|---|
| GET | / | Selector list, including locked records |
| GET | /paged?text=&pageIndex=0&pageSize=20 | Search by name; size 1–200 |
| GET | /{id} | Header with target-product description |
| GET | /{id}/configuration | `{ Commissions: [...] }` |
| POST | / | Create header and commissions atomically |
| PUT | /{id} | Update header and replace commissions atomically |

Save body:
```json
{
  "DebitType": {
    "Description": "Registration fee",
    "CustomerAccountTypeProductCode": 1,
    "CustomerAccountTypeTargetProductId": "<savings-product-guid>",
    "IsMandatory": false,
    "IsLocked": false
  },
  "Commissions": [{ "Id": "<commission-guid>" }]
}
```

Product categories: 1 Savings, 2 Loan, 3 Investment. AppService resolves the target
product's actual Code; client-supplied TargetProductCode is not trusted. It validates
name, product existence/category, duplicate names and valid distinct commission IDs.
Commissions is required; [] deliberately clears charges. Failed validation does not
persist header or relationship changes. Missing records return 404; invalid inputs
return 400. Responses use `{ success, message, data }` with PascalCase DTO fields.

## Navigation and UI

Canonical NavigationMenu DebitType code: **23026** (0x59D8 + 26).
Module route: `/Accounts/DebitTypes`. The setup page supports search, create/edit,
product selection, flags and commissions. Debit-batch creation selects a debit type
by name, and Company setup supports its existing debit-types association API.

Customer-registration debit-type charges post under SystemTransactionCode.MembershipApproval
(72), rather than Unclassified (0). This is the posting/authority category and does
not approve the customer record. The employee designation must authorize this
category for the charge amount; transaction-authority validation remains enforced.
