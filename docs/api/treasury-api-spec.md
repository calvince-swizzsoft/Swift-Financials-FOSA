# Treasury Master Data API — Client Integration Spec

Audience: back-office/admin screens that create and maintain `Treasury`
vaults (a branch's cash vault, the counterpart to a `Teller`). This is
**master data** — not the day-to-day cash movement in and out of a vault,
which is a separate, front-office endpoint (see below).

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/TreasurysController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/TreasuryAppService.cs`.
- DTO: `Application.MainBoundedContext.DTO/AccountsModule/TreasuryDTO.cs`.
- Domain: `Domain.MainBoundedContext/AccountsModule/Aggregates/TreasuryAgg`
  (same aggregate the front-office cash-movement flow reads/writes).
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2. Every endpoint below requires
  `[Authorize]`.

Related but distinct: `POST /api/frontoffice/cashmanagement` — moving cash
between bank/treasury/teller — is documented in `frontoffice-api-spec.md`
§5. That endpoint posts against an *existing* Treasury; this doc is about
creating/editing the Treasury record itself.

## 1. Response envelope, paging, casing

`{ success: boolean, message: string, data: T | null }` on every endpoint —
same as `docs/api/README.md`. The list endpoint returns
`PageCollectionInfo<TreasuryDTO>` (`{ PageIndex, PageSize, PageCollection,
ItemsCount }`) under `data`. `pageIndex` (query param) is 0-based, defaults
to `0`; `pageSize` defaults to `20`.

Casing: this project has no `CamelCasePropertyNamesContractResolver`
configured anywhere (checked directly against `WebApiConfig.cs`/
`Global.asax.cs`), so `TreasuryDTO` and `PageCollectionInfo<T>` serialize
with their real C# property names — **PascalCase**, both request and
response. Only the envelope (`success`/`message`/`data`) is lowercase,
because those are literal anonymous-object property names written directly
in the controller, not DTO fields.

## 2. `TreasuryDTO` field reference

| Field | Type | Editable? | Notes |
|---|---|---|---|
| `Id` | Guid | No | Server-assigned on create. Omit/ignore on create; required implicitly via the URL on update. |
| `BranchId` | Guid | Create only | Must not be `Guid.Empty` (`[ValidGuid]`, the only server-enforced requirement on this DTO besides `ChartOfAccountId`). **Immutable after creation** — see §4. |
| `BranchDescription` | string | No (display) | Populated server-side on read. On create, worth sending anyway even though it's not persisted — see the duplicate-branch error message caveat in §3.3. |
| `ChartOfAccountId` | Guid | Yes | Must not be `Guid.Empty` (`[ValidGuid]`). The G/L cash account this vault posts against. Editable on update. |
| `ChartOfAccountAccountType` / `ChartOfAccountAccountCode` / `ChartOfAccountAccountName` | int / int / string | No | Display-only, populated server-side from the chart of account. |
| `ChartOfAccountName` | string | No | Computed (`"{type}-{code} {name}"`), read-only. |
| `ChartOfAccountCostCenterId` / `ChartOfAccountCostCenterDescription` | Guid? / string | No | Display-only. |
| `Code` | int | **No — server-generated** | `MAX(Code)+1` across all treasuries, assigned inside `AddNewTreasury`. Don't send it, don't let the UI imply it's user-choosable. |
| `PaddedCode` | string | No | Computed (`Code` left-padded to 3 digits), read-only. |
| `Description` | string | Yes | The vault's name. No `[Required]` attribute — see §4 for why you should still require it in the UI. Must be unique across all treasuries (server-enforced, see §4). |
| `RangeLowerLimit` / `RangeUpperLimit` | decimal | Yes | No `[Required]`/range attributes — same caveat. |
| `IsLocked` | bool | Yes | Respected on both create and update (`Lock()`/`UnLock()` on the aggregate). |
| `CreatedDate` | DateTime | No | Server-assigned. |
| `BookBalance` | decimal | No | Server-computed running balance. |
| `ErrorMessageResult` | string | No | Internal — see §4. Never populated on a *successful* response; the controller translates it into a `409` before it would reach a client. |
| `Treasury` | TreasuryDTO | — | Self-referencing property on the DTO class; unused by the controller/app service. Ignore it — don't send it, don't expect it populated. |

## 3. Endpoints — `api/accounts/treasurys`

### 3.1 List — `GET /?text=&pageIndex=&pageSize=`

Paged/filtered list. `text` matches **`Description` only** (not `Code`,
not branch) — the underlying spec is a single `Description.Contains(text)`
filter, so a search box should be labeled/scoped accordingly.

### 3.2 Get one — `GET /{id}`

Single treasury. `404` if `id` doesn't resolve.

### 3.3 Create — `POST /`

Body: `TreasuryDTO` — send `BranchId`, `ChartOfAccountId`, `Description`,
`RangeLowerLimit`, `RangeUpperLimit`, optionally `IsLocked`. Don't send
`Code` (server-generated) or `Id`.

- **`200`** — created. `data` is the full `TreasuryDTO` including the
  server-generated `Id`/`Code`/`CreatedDate`.
- **`400`** — `ValidateAll()` failed (`BranchId`/`ChartOfAccountId` empty),
  `data: null`, `message` is the semicolon-joined validation errors.
- **`409`** — a business rule blocked creation, `data: null`:
  - the branch already has a treasury linked to it (**one treasury per
    branch** is enforced), or
  - a treasury with that exact `Description` already exists (**description
    must be unique**).

  `message` is the server's own message for either case (e.g. `"Sorry, but
  another treasury has already been linked to branch <name>"` or `"Sorry,
  but Treasury \"<NAME>\" already exists!"`).

  The branch name in that first message is interpolated from whatever
  `BranchDescription` the **client** sent — the duplicate check runs before
  any server-side branch lookup, so it echoes the request, not the
  database. If the create payload omits `BranchDescription` (reasonable,
  since the field table above lists it as display-only), that message
  reads `"...already been linked to branch "` with nothing after it. Send
  `BranchDescription` on create anyway, purely so this one error message
  reads correctly — it has no other effect.

The reference MVC app split this into a multi-page, session-driven wizard
(pick branch → pick chart of account → confirm, each step stashed in
`Session[...]`). That doesn't translate to a stateless JSON API and isn't
reproduced here — submit the fully-assembled `TreasuryDTO` in one call.
Build any multi-step picker UI client-side against whatever branch/
chart-of-account lookup endpoints you're already using elsewhere; there's
no server-side wizard state to coordinate with.

### 3.4 Update — `PUT /{id}`

Body: `TreasuryDTO`. The route `id` is authoritative — assigned onto the
body DTO before validation/update, so a mismatched `Id` in the body is
overwritten rather than silently ignored.

- **`200`** — `data` is the freshly re-fetched `TreasuryDTO` reflecting
  what was actually saved.
- **`400`** — validation failure, same shape as create.
- **`404`** — `id` doesn't resolve to an existing treasury.

**`BranchId` cannot actually be changed by this endpoint** — the update
path re-derives the treasury from its *persisted* branch regardless of
whatever `BranchId` is in the request body; only `ChartOfAccountId`,
`Description`, `RangeLowerLimit`/`RangeUpperLimit`, and `IsLocked` are
genuinely applied. The request still requires a non-empty `BranchId` to be
present (or the call short-circuits), so send the treasury's existing
`BranchId` back — but disable/hide that field in an edit UI so it doesn't
imply changing branch is possible.

## 4. Business rules worth designing the UI around

- **One treasury per branch.** Enforced on create only (checked before the
  insert). A branch picker on the create screen should ideally exclude
  branches that already have a treasury — that requires cross-referencing
  the list endpoint (§3.1) client-side, since there's no "branches without
  a treasury" endpoint.
- **Unique `Description`.** Enforced on create only, not on update (the
  update path never checks it). Treat the create-screen name field as
  required and unique in the UI even though the DTO has no `[Required]`
  attribute — the two checks above are the only server-side guards, and
  neither is a `[Required]`/uniqueness *validation* error (they don't set
  `HasErrors`/`ErrorMessages`); they surface as a `409` instead (see §3.3).
- **`RangeLowerLimit`/`RangeUpperLimit` are not server-validated at all** —
  no `[Required]`, no "lower ≤ upper" check. Enforce sane values
  client-side; the server will accept `0`/`0` or an inverted range without
  complaint.
- **`Code` is never user input.** Don't build a "Code" field into the
  create form; show `PaddedCode` as a read-only badge once the record
  exists.

## 5. Not reproduced from the reference app

The reference app actually had **two** controllers managing this same
entity — `Areas/FrontOffice/Controllers/TreasuryController.cs` and
`Areas/Accounts/Controllers/TreasuriesController.cs` — both doing the same
CRUD through the same underlying service calls, just with different UX (a
single-page form vs. the session wizard mentioned in §3.3). This API
exposes one endpoint for both; there's no functional gap, just two old
screens converging on one resource.

## 6. JSON examples

### `GET /?text=main&pageIndex=0&pageSize=20`

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
        "Id": "a1b2c3d4-e5f6-4a7b-8c9d-000000000001",
        "Treasury": null,
        "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
        "BranchDescription": "Nairobi Main",
        "ChartOfAccountId": "b2c3d4e5-f6a7-4b8c-9d0e-000000000002",
        "ChartOfAccountAccountType": 1,
        "ChartOfAccountAccountCode": 1001,
        "ChartOfAccountAccountName": "Vault Cash",
        "ChartOfAccountName": "1-1001 Vault Cash",
        "ChartOfAccountCostCenterId": null,
        "ChartOfAccountCostCenterDescription": null,
        "Code": 4,
        "PaddedCode": "004",
        "Description": "Nairobi Main Treasury",
        "RangeLowerLimit": 50000.00,
        "RangeUpperLimit": 2000000.00,
        "IsLocked": false,
        "CreatedDate": "2026-01-15T09:00:00",
        "BookBalance": 875000.00,
        "ErrorMessageResult": null
      }
    ]
  }
}
```

### `GET /{id}`

Same single-object shape as one `PageCollection` entry above, under `data`
directly (no paging wrapper). `404` with an empty body if `id` doesn't
resolve.

### `POST /` — success

Request:
```json
{
  "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
  "ChartOfAccountId": "b2c3d4e5-f6a7-4b8c-9d0e-000000000002",
  "Description": "Nairobi Main Treasury",
  "RangeLowerLimit": 50000.00,
  "RangeUpperLimit": 2000000.00,
  "IsLocked": false
}
```

Response `200`:
```json
{
  "success": true,
  "message": "Operation Success",
  "data": {
    "Id": "a1b2c3d4-e5f6-4a7b-8c9d-000000000001",
    "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
    "BranchDescription": "Nairobi Main",
    "ChartOfAccountId": "b2c3d4e5-f6a7-4b8c-9d0e-000000000002",
    "ChartOfAccountAccountType": 1,
    "ChartOfAccountAccountCode": 1001,
    "ChartOfAccountAccountName": "Vault Cash",
    "ChartOfAccountName": "1-1001 Vault Cash",
    "Code": 4,
    "PaddedCode": "004",
    "Description": "Nairobi Main Treasury",
    "RangeLowerLimit": 50000.00,
    "RangeUpperLimit": 2000000.00,
    "IsLocked": false,
    "CreatedDate": "2026-08-07T10:00:00",
    "BookBalance": 0.00,
    "ErrorMessageResult": null
  }
}
```

### `POST /` — branch already has a treasury (`409`)

Request: same shape, `BranchId` pointing at a branch that already has one.

Response `409`:
```json
{
  "success": false,
  "message": "Sorry, but another treasury has already been linked to branch Nairobi Main",
  "data": null
}
```

### `POST /` — validation failure (`400`)

Request: `BranchId` omitted (defaults to `Guid.Empty`) — the only two
fields with a real server-side validation attribute are `BranchId` and
`ChartOfAccountId` (`[ValidGuid]`); its message is generated from the
DTO's `[Display(Name=...)]` on that property, not hand-written per field.

Response `400`:
```json
{
  "success": false,
  "message": "The Branch identifier is invalid!",
  "data": null
}
```

### `PUT /{id}` — success

Request (`BranchId` included but ignored server-side — see §3.4):
```json
{
  "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
  "ChartOfAccountId": "b2c3d4e5-f6a7-4b8c-9d0e-000000000002",
  "Description": "Nairobi Main Treasury (Renamed)",
  "RangeLowerLimit": 100000.00,
  "RangeUpperLimit": 2500000.00,
  "IsLocked": false
}
```

Response `200` — `data` is the re-fetched record, so `BranchId`/
`BranchDescription` reflect the unchanged original branch even though the
request carried one:
```json
{
  "success": true,
  "message": "Operation Success",
  "data": {
    "Id": "a1b2c3d4-e5f6-4a7b-8c9d-000000000001",
    "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
    "BranchDescription": "Nairobi Main",
    "ChartOfAccountId": "b2c3d4e5-f6a7-4b8c-9d0e-000000000002",
    "Code": 4,
    "PaddedCode": "004",
    "Description": "Nairobi Main Treasury (Renamed)",
    "RangeLowerLimit": 100000.00,
    "RangeUpperLimit": 2500000.00,
    "IsLocked": false,
    "CreatedDate": "2026-08-07T10:00:00",
    "BookBalance": 875000.00,
    "ErrorMessageResult": null
  }
}
```
