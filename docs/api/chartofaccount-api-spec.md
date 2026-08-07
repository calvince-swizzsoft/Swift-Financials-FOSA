# Chart of Account API — Client Integration Spec

Audience: back-office/admin screens that maintain the chart of accounts (the
G/L account tree every posting in the system ultimately debits/credits)
and the system-to-G/L-account mapping table (which G/L account a given
system transaction type posts to by default).

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/ChartOfAccountController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/ChartOfAccountAppService.cs`.
- DTOs: `Application.MainBoundedContext.DTO/AccountsModule/ChartOfAccountDTO.cs`,
  `Application.MainBoundedContext.DTO/AccountsModule/SystemGeneralLedgerAccountMappingDTO.cs`,
  `Application.MainBoundedContext.DTO/GeneralLedgerAccount.cs` (tree read model).
- Domain: `Domain.MainBoundedContext/AccountsModule/Aggregates/ChartOfAccountAgg`.
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2. Every endpoint below requires
  `[Authorize]`.

The reference app split this across two duplicate screens
(`GLAccountController`, `SystemGeneralLedgerAccountMappingController` —
same underlying calls, different views) plus the real Chart of Account
screen (`ChartOfAccountController`). This API folds the mapping concept
into this one controller as a sub-resource, matching how the app-service
layer already groups them (`IChartOfAccountAppService` owns both) —
there's no separate mapping controller here.

## 1. Response envelope, paging, casing

`{ success: boolean, message: string, data: T | null }` on every endpoint —
same as `docs/api/README.md`. Paged endpoints return
`PageCollectionInfo<T>` (`{ PageIndex, PageSize, PageCollection,
ItemsCount }`) under `data`. `pageIndex` (query param) is 0-based, defaults
to `0`; `pageSize` defaults to `20`.

Casing: PascalCase on every DTO field (no camelCase resolver configured in
this project — verified directly against `WebApiConfig.cs`/
`Global.asax.cs`, same finding as every other spec in this set). Only the
envelope (`success`/`message`/`data`) is lowercase.

## 2. `ChartOfAccountDTO` field reference

| Field | Editable? | Notes |
|---|---|---|
| `Id` | No | Server-assigned |
| `ParentId` | Create only | Nullable — omit for a root/top-level account. The controller validates it resolves to an existing account (`400` if not) before calling the app service, because the app service itself silently falls back to creating a root account on an unresolvable `ParentId` rather than erroring |
| `ParentAccountName`, `Parent` | No | Display-only; `Parent` isn't even serialized (`[DataMember]` is commented out on the DTO) |
| `CostCenterId` | Yes, conditionally | **Forced to `null` server-side whenever `IsControlAccount` is `true`**, regardless of what's sent. Disable/clear that field in the UI when Control Account is checked |
| `AccountType` | **Root accounts only** | If `ParentId` is set, the domain factory ignores whatever `AccountType` is sent and inherits it from the parent (`chartOfAccount.AccountType = parent.AccountType`). Hide/disable the Account Type selector once a parent is picked and display the parent's type instead |
| `AccountCategory` | Yes | No active server-side validation — the DTO has `[CustomValidation]` scaffolding for this but it's commented out. Enforce a valid `ChartOfAccountCategory` value client-side |
| `AccountCode` | Yes | **Must be unique across the whole chart of accounts** — see §4 |
| `AccountName` | Yes | The only `[Required]` field on this DTO |
| `Depth` | No | **Not maintained by create/update at all.** Only `GET /tree` populates it correctly — don't trust `Depth` off the flat list/get-by-id endpoints |
| `IsControlAccount` / `IsReconciliationAccount` / `PostAutomaticallyOnly` / `IsLocked` | Yes | Plain booleans |
| `CreatedDate` | No | Server-assigned |
| `Children` | No | Empty on flat CRUD responses; only `GET /tree` populates it |
| `ErrorMessageResult` | No | Business-rule-failure channel on create — see §4 |

## 3. Endpoints — `api/accounts/chartofaccounts`

### 3.1 List (flat) — `GET /?text=&pageIndex=&pageSize=`

Paged/filtered flat list.

### 3.2 Get one — `GET /{id}`

Single account. `404` if `id` doesn't resolve.

### 3.3 Tree — `GET /tree`

`data` is a `List<GeneralLedgerAccount>` — a **different shape** from
`ChartOfAccountDTO`: `Id`, `ParentId`, `Category`/`CategoryDescription`,
`Type`/`TypeDescription`, `Code`, `Description`, plus depth/balance fields
correctly populated. Use this endpoint (not the flat list) whenever the UI
needs to render the account hierarchy or show `Depth`.

### 3.4 Create — `POST /`

Body: `ChartOfAccountDTO`. On success (`200`), `data` is the full created
record. Failure modes:
- **`400`** — validation failed (`AccountName` missing), or `ParentId` was
  supplied but doesn't resolve to an existing account.
- **`409`** — `AccountCode` already exists elsewhere in the chart of
  accounts.

### 3.5 Update — `PUT /{id}`

Body: `ChartOfAccountDTO`. Route `id` is authoritative (assigned onto the
body before validation). On success (`200`), `data` is the freshly
re-fetched record (the app-service call itself returns `bool`, not the
entity). Failure modes:
- **`400`** — validation failed, or `ParentId` doesn't resolve.
- **`404`** — `id` doesn't resolve to an existing account.
- **`409`** — `AccountCode` collides with another account. Note this is
  reported completely differently from create at the app-service layer —
  create sets `ErrorMessageResult`, update throws `InvalidOperationException`
  — the controller normalizes both to the same `409` shape, so you don't
  need to care about the difference as a client.

### 3.6 List system→G/L mappings — `GET /systemgeneralledgermappings?pageIndex=&pageSize=`

`data` is `PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO>` — each
row is one `SystemGeneralLedgerAccountCode` (e.g. "Cash Deposit") mapped to
the `ChartOfAccountId` it posts to by default.

### 3.7 Upsert one mapping — `PUT /systemgeneralledgermappings/{systemGeneralLedgerAccountCode}`

`systemGeneralLedgerAccountCode` is an int route segment (a
`SystemGeneralLedgerAccountCode` enum value). **Body is a raw JSON string,
not an object** — ASP.NET Web API's `[FromBody]` simple-type binding for a
`Guid` parameter expects the entire request body to be the JSON-quoted
value itself:

```
PUT /api/accounts/chartofaccounts/systemgeneralledgermappings/48826
Content-Type: application/json

"b2c3d4e5-f6a7-4b8c-9d0e-000000000002"
```

Not `{ "chartOfAccountId": "..." }` — that will fail to bind and the
`Guid` parameter will come through as `Guid.Empty`, which the controller
rejects as `400`. This single endpoint handles both creating a new mapping
and changing an existing one — `MapSystemGeneralLedgerAccountCodeToChartOfAccount`
already does update-if-exists/create-if-not internally. `400` if the
`chartOfAccountId` is empty or doesn't resolve to a real chart of account.

## 4. Business rules worth designing the UI around

- **`AccountCode` uniqueness**, enforced two different ways depending on
  the operation (the controller normalizes both to `409`, documented here
  so you understand what you're seeing if you inspect the raw app-service
  behavior): create reports it via `ErrorMessageResult` on the returned
  DTO; update throws `InvalidOperationException`.
- **Child accounts inherit `AccountType` from their parent** — don't let
  the UI imply a child account can have a different type than its parent.
- **Control accounts can't have a cost center** — `CostCenterId` is
  discarded server-side whenever `IsControlAccount` is `true`.
- **`Depth`/`Children` are tree-only fields** — they come back
  zero/empty from every flat CRUD endpoint. Always use `GET /tree` for
  hierarchy-aware rendering.
- **An invalid `ParentId` is caught by this controller, not the app
  service** — `AddNewChartOfAccount`/`UpdateChartOfAccount` themselves
  silently create a root account if `ParentId` doesn't resolve, which
  would be a confusing silent failure mode for a client that intended to
  nest an account. The controller pre-checks this and returns `400`
  instead.

## 5. JSON examples

### `GET /?text=cash&pageIndex=0&pageSize=20`

```json
{
  "success": true,
  "message": "",
  "data": {
    "PageIndex": 0,
    "PageSize": 20,
    "ItemsCount": 1,
    "PageCollection": [
      {
        "Id": "c1d2e3f4-a5b6-4c7d-8e9f-000000000010",
        "ParentId": "d2e3f4a5-b6c7-4d8e-9f0a-000000000011",
        "ParentAccountName": null,
        "CostCenterId": null,
        "CostCenterDescription": null,
        "AccountType": 1000,
        "AccountTypeDescription": "Asset",
        "AccountCategory": 4097,
        "AccountCategoryDescription": "DetailAccount",
        "AccountCode": 1001,
        "AccountName": "Vault Cash",
        "Depth": 0,
        "IsControlAccount": false,
        "IsReconciliationAccount": false,
        "PostAutomaticallyOnly": false,
        "IsLocked": false,
        "CreatedDate": "2026-01-10T09:00:00",
        "Children": [],
        "ErrorMessageResult": null
      }
    ]
  }
}
```

### `GET /tree`

```json
{
  "success": true,
  "message": "",
  "data": [
    {
      "Id": "d2e3f4a5-b6c7-4d8e-9f0a-000000000011",
      "ParentId": null,
      "Category": 4096,
      "CategoryDescription": "HeaderAccount",
      "Type": 1000,
      "TypeDescription": "Asset",
      "Code": 1000,
      "Description": "Assets"
    },
    {
      "Id": "c1d2e3f4-a5b6-4c7d-8e9f-000000000010",
      "ParentId": "d2e3f4a5-b6c7-4d8e-9f0a-000000000011",
      "Category": 4097,
      "CategoryDescription": "DetailAccount",
      "Type": 1000,
      "TypeDescription": "Asset",
      "Code": 1001,
      "Description": "Vault Cash"
    }
  ]
}
```

### `POST /` — success (root account)

Request:
```json
{
  "AccountType": 1000,
  "AccountCategory": 4096,
  "AccountCode": 1000,
  "AccountName": "Assets",
  "IsControlAccount": true,
  "IsReconciliationAccount": false,
  "PostAutomaticallyOnly": false,
  "IsLocked": false
}
```

Response `200`:
```json
{
  "success": true,
  "message": "Operation Success",
  "data": {
    "Id": "d2e3f4a5-b6c7-4d8e-9f0a-000000000011",
    "ParentId": null,
    "CostCenterId": null,
    "AccountType": 1000,
    "AccountTypeDescription": "Asset",
    "AccountCategory": 4096,
    "AccountCategoryDescription": "HeaderAccount",
    "AccountCode": 1000,
    "AccountName": "Assets",
    "Depth": 0,
    "IsControlAccount": true,
    "IsReconciliationAccount": false,
    "PostAutomaticallyOnly": false,
    "IsLocked": false,
    "CreatedDate": "2026-08-07T10:00:00",
    "Children": [],
    "ErrorMessageResult": null
  }
}
```

### `POST /` — duplicate `AccountCode` (`409`)

Response `409`:
```json
{
  "success": false,
  "message": "Sorry, but Account Code 1000 already exists!",
  "data": null
}
```

### `POST /` — `ParentId` doesn't resolve (`400`)

Response `400`:
```json
{
  "success": false,
  "message": "Parent chart of account not found.",
  "data": null
}
```

### `PUT /{id}` — duplicate `AccountCode` (`409`, thrown path)

Response `409`:
```json
{
  "success": false,
  "message": "Sorry, but Account Code 1001 already exists!",
  "data": null
}
```

### `GET /systemgeneralledgermappings?pageIndex=0&pageSize=20`

```json
{
  "success": true,
  "message": "",
  "data": {
    "PageIndex": 0,
    "PageSize": 20,
    "ItemsCount": 1,
    "PageCollection": [
      {
        "Id": "e3f4a5b6-c7d8-4e9f-8a0b-000000000012",
        "SystemGeneralLedgerAccountCode": 48826,
        "SystemGeneralLedgerAccountCodeDescription": "Payables Control",
        "ChartOfAccountId": "c1d2e3f4-a5b6-4c7d-8e9f-000000000010",
        "ChartOfAccountAccountType": 1000,
        "ChartOfAccountAccountCode": 1001,
        "ChartOfAccountAccountName": "Vault Cash",
        "ChartOfAccountName": "1-1001 Vault Cash",
        "ChartOfAccountCostCenterId": null,
        "ChartOfAccountCostCenterDescription": null,
        "CreatedDate": "2026-01-10T09:05:00",
        "IsLocked": false,
        "ErrorMessageResult": null
      }
    ]
  }
}
```

### `PUT /systemgeneralledgermappings/48826` — success

Request body (raw quoted string, see §3.7):
```json
"c1d2e3f4-a5b6-4c7d-8e9f-000000000010"
```

Response `200`:
```json
{ "success": true, "message": "Operation Success", "data": null }
```
