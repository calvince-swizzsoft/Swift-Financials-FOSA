# UnPay Reason API — Client Integration Spec

Audience: admin screens that configure `UnPayReason` records — the reasons
a teller can select when reversing (`UnPay`-ing) an external cheque that
already passed clearance, and the `ChequesController`'s `POST
/api/frontoffice/cheques/clear` `"unpay"` flow, which needs a valid
`UnPayReasonDTO` up front (see `docs/api/frontoffice-api-spec.md` §8 and
`WebApplication1/Areas/FrontOffice/CHEQUE-PROCESSING-ANALYSIS.md`).

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/UnPayReasonController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/UnPayReasonAppService.cs`
  (`IUnPayReasonAppService`).
- DTOs: `UnPayReasonDTO`, `CommissionDTO` (`Application.MainBoundedContext.DTO/AccountsModule/`).
- Auth: same JWT bearer scheme as every other controller — `[Authorize]`.

## History note — two things deliberately not ported from the reference app

The reference MVC `UnPayReasonController.cs` (Areas/Accounts) has working
CRUD plus an attached-commissions picker, but:

- **Its `Edit` POST never calls `ValidateAll()`** before checking
  `HasErrors` — unlike its own `Create`, which does. `UnPayReasonDTO`'s
  `[Required]` on `Description` was therefore never actually enforced on
  edit; an edit could silently blank the description. This API's `PUT
  /{id}` calls `ValidateAll()` first, same as every other controller's
  Update action.
- **Its Create/Edit attached-commissions flow took a comma-separated
  `SelectedIds` string and resolved each id to a full `CommissionDTO` via
  a round trip** (`_channelService.FindCommissionAsync` per id) purely to
  hand the list to `UpdateCommissionsByUnPayReasonIdAsync` — which only
  ever reads `CommissionDTO.Id`
  (`UnPayReasonAppService.UpdateCommissions` →
  `UnPayReasonCommissionFactory.CreateUnPayReasonCommission(persisted.Id,
  item.Id)`). This API takes commission ids directly (`Guid[]`) and builds
  bare `CommissionDTO { Id = ... }` locally — no per-id lookup.

Commissions are a sub-resource (`GET`/`PUT .../commissions`), not folded
into Create/Update — same pattern as `LevyController`'s splits and
`CommissionController`'s own graduated-scales/splits/levies sub-resources.

## 1. Response envelope

`{ success: boolean, message: string, data: T | null }`.

## 2. `UnPayReasonDTO` — core fields

| Field | Notes |
|---|---|
| `Id` | Server-assigned on create. |
| `Code` | Plain `int`; no server-side uniqueness check. |
| `Description` | `[Required]`. **Checked for uniqueness** — see `ErrorMessageResult` below. |
| `IsLocked` | Plain bool. |
| `ErrorMessageResult` | Set (non-null) on `POST /` only when `Description` already exists on another `UnPayReason` — surfaced as `409 Conflict`, `data: null`, `message` = this field's value. Always `null` on a successful create/update or on any `GET`. |

## 3. Endpoints — `api/accounts/unpayreasons`

### 3.1 List (unpaged) — `GET /`

For pickers — e.g. building the "unpay reason" dropdown before calling
`ChequesController`'s clear/unpay endpoint.

```json
{ "success": true, "message": "", "data": UnPayReasonDTO[] }
```

### 3.2 List (paged/search) — `GET /paged?text=&pageIndex=&pageSize=`

`data` is `PageCollectionInfo<UnPayReasonDTO>`.

### 3.3 Get one — `GET /{id}`

`404` if not found.

### 3.4 Create — `POST /`

Body:
```ts
interface CreateUnPayReasonRequest {
  unPayReason: UnPayReasonDTO;
  commissionIds?: string[];  // optional Guid[] — commissions to attach
}
```

`400` if `unPayReason` is missing/fails `ValidateAll()`. `409` (`data:
null`) if `Description` already exists on another `UnPayReason` (see
`ErrorMessageResult` above). On success, `commissionIds` (if sent) is
saved as a full replace against the new `UnPayReason` (same as `PUT
/{id}/commissions`), then the record is re-fetched so `data` reflects
what was actually persisted.

### 3.5 Update — `PUT /{id}`

Body: `UnPayReasonDTO`. **Only updates the reason's own fields** — does
not touch attached commissions (see history note; use §3.6). `400` if
validation fails. `404` if `id` doesn't resolve. No `409` duplicate check
on update (only `Create` checks — same as the underlying app service).

### 3.6 Attached commissions — `GET`/`PUT /{id}/commissions`

`PUT` body: `Guid[]` (commission ids) — **full replace**, every existing
attachment for this `UnPayReason` is deleted and replaced with exactly
what you send (`[]` clears all). `404` if the `UnPayReason` id doesn't
resolve. `GET` returns the full `CommissionDTO[]` currently attached.
