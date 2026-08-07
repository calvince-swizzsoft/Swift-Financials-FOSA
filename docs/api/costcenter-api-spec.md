# Cost Center API — Client Integration Spec

Audience: back-office/admin screens that maintain cost centers — a small
lookup dimension chart-of-accounts entries can optionally be tagged with
(`ChartOfAccountDTO.CostCenterId`).

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/CostCenterController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/CostCenterAppService.cs`.
- DTO: `Application.MainBoundedContext.DTO/AccountsModule/CostCenterDTO.cs`.
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2. Every endpoint below requires
  `[Authorize]`.

## 1. Response envelope, paging, casing

`{ success: boolean, message: string, data: T | null }`, same as
`docs/api/README.md`. The list endpoint returns `PageCollectionInfo<T>`
(`{ PageIndex, PageSize, PageCollection, ItemsCount }`) under `data`.
`pageIndex` (query param) is 0-based, defaults to `0`; `pageSize` defaults
to `20`. PascalCase on every DTO field — same casing note as every other
spec in this set (no camelCase resolver configured anywhere in this
project).

## 2. `CostCenterDTO` field reference

This is a small DTO — four real fields:

| Field | Editable? | Notes |
|---|---|---|
| `Id` | No | Server-assigned |
| `Description` | Yes | The only `[Required]` field. Must be unique — see §4 |
| `IsLocked` | Yes | Plain boolean |
| `CreatedDate` | No | Server-assigned |
| `ErrorMessageResult` | No | Business-rule-failure channel on create — see §4 |

## 3. Endpoints — `api/accounts/costcenters`

### 3.1 List — `GET /?text=&pageIndex=&pageSize=`

Paged/filtered list.

### 3.2 Get one — `GET /{id}`

Single cost center. `404` if `id` doesn't resolve.

### 3.3 Create — `POST /`

Body: `CostCenterDTO` — send `Description`, optionally `IsLocked`.

- **`200`** — `data` is the created record.
- **`400`** — `Description` missing.
- **`409`** — a cost center with that exact `Description` already exists.

### 3.4 Update — `PUT /{id}`

Body: `CostCenterDTO`. Route `id` is authoritative. On success (`200`),
`data` is the freshly re-fetched record (`UpdateCostCenter` itself returns
`bool`, not the entity). `404` if `id` doesn't resolve.

## 4. Business rules worth designing the UI around

- **Unique `Description` is enforced on create only.** Unlike Treasury and
  Chart of Account, `UpdateCostCenter` doesn't re-check uniqueness at all —
  you can rename a cost center to collide with another one's name via
  `PUT` and it will succeed. This matches the reference app's own
  behavior; flag to product if that's not actually desired.

## 5. JSON examples

### `GET /?text=branch&pageIndex=0&pageSize=20`

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
        "Id": "f4a5b6c7-d8e9-4f0a-8b1c-000000000013",
        "Description": "Nairobi Branch Operations",
        "IsLocked": false,
        "CreatedDate": "2026-01-05T09:00:00",
        "ErrorMessageResult": null
      }
    ]
  }
}
```

### `POST /` — success

Request:
```json
{
  "Description": "Nairobi Branch Operations",
  "IsLocked": false
}
```

Response `200`:
```json
{
  "success": true,
  "message": "Operation Success",
  "data": {
    "Id": "f4a5b6c7-d8e9-4f0a-8b1c-000000000013",
    "Description": "Nairobi Branch Operations",
    "IsLocked": false,
    "CreatedDate": "2026-08-07T10:00:00",
    "ErrorMessageResult": null
  }
}
```

### `POST /` — duplicate `Description` (`409`)

Response `409`:
```json
{
  "success": false,
  "message": "Sorry, but Cost Center \"NAIROBI BRANCH OPERATIONS\" already exists!",
  "data": null
}
```

### `PUT /{id}` — success

Request:
```json
{
  "Description": "Nairobi Branch Operations (Renamed)",
  "IsLocked": true
}
```

Response `200`:
```json
{
  "success": true,
  "message": "Operation Success",
  "data": {
    "Id": "f4a5b6c7-d8e9-4f0a-8b1c-000000000013",
    "Description": "Nairobi Branch Operations (Renamed)",
    "IsLocked": true,
    "CreatedDate": "2026-08-07T10:00:00",
    "ErrorMessageResult": null
  }
}
```
