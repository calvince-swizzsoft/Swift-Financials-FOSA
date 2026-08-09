# Levy API — Client Integration Spec

Audience: admin screens that configure `Levy` records — statutory/
government-style charges (e.g. VAT, excise duty) computed against a
`Commission`'s leviable amount, never against a raw transaction directly.
See `COMMISSION-LEVY-CHARGE-CONCEPTS.md` §2 for how Levy relates to
Commission, and `docs/api/commission-api-spec.md` §5.8 for attaching a levy
to a commission.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/LevyController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/LevyAppService.cs`
  (`ILevyAppService`).
- DTOs: `LevyDTO`, `LevySplitDTO` (`Application.MainBoundedContext.DTO/AccountsModule/`).
- Auth: same JWT bearer scheme as every other controller — `[Authorize]`.

## History note — two reference-app bugs deliberately not ported

The reference MVC `LevyController.cs` (Areas/Accounts) has full, mostly-
working CRUD, but two real bugs:

- **Its `Edit` POST wipes splits on every edit.** It calls
  `UpdateLevySplitsByLevyIdAsync(levyDTO.Id, new ObservableCollection<LevySplitDTO>(), ...)`
  — a freshly-empty collection, unconditionally, on every single edit. This
  API's `PUT /{id}` only touches the levy's own fields; splits have their
  own sub-resource endpoint (§4.6) and are never touched by the main
  update.
- **Its `Create` POST hardcodes `levyDTO.LevySplitsTotalPercentage = 100`
  unconditionally**, right before calling `ValidateAll()`. That property
  exists solely to drive `LevyDTO`'s own
  `[CustomValidation(..., "CheckLevySplitsTotalPercentage")]` attribute
  (requires it equal exactly `100`) — hardcoding it makes that validation a
  permanent no-op regardless of what the splits actually sum to. This API
  computes it from the real submitted `LevySplits` instead (§4.4), so the
  validation is meaningful.

Also worth knowing: unlike `CommissionController.Create`,
**`LevyController.Create` has no duplicate-`Description` check.** Checked
directly: `LevyAppService.AddNewLevy`'s only write to `ErrorMessageResult`
happens unconditionally on every call (not gated behind any duplicate
check) and is immediately discarded anyway — the returned `LevyDTO` comes
from `AutoMapper` projecting the persisted `Levy` entity, which has no
`ErrorMessageResult` column, so the field is always `null` on the response
regardless. There is no `409` case for this endpoint.

## 1. Response envelope

`{ success: boolean, message: string, data: T | null }`.

## 2. `LevyDTO` — core fields

| Field | Notes |
|---|---|
| `Id` | Server-assigned on create. |
| `Description` | `[Required]`. **Not** checked for uniqueness (see history note). |
| `ChargeType` | `ChargeType` enum: `Percentage` or `FixedAmount`. |
| `ChargeValue` | Convenience input field — server maps it into `ChargePercentage` or `ChargeFixedAmount` based on `ChargeType` (same mapping the reference app did correctly; send this one field, not both underlying ones). |
| `ChargePercentage` / `ChargeFixedAmount` | Populated server-side from `ChargeValue` — you can also set the relevant one directly instead of `ChargeValue`. |
| `IsLocked` | Plain bool. |
| `LevySplitsTotalPercentage` | **Set by the server on `POST`/`PUT`, not the caller** — drives the DTO's own validation attribute. Computed from the real `LevySplits` sum on create; fixed at `100` on update (since update never touches splits — see history note). |
| `ErrorMessageResult` | Always `null` from this controller (no duplicate check exists). |

## 3. `LevySplitDTO` — how a computed levy amount divides across GL accounts

| Field | Notes |
|---|---|
| `Id` | Server-assigned. |
| `ChartOfAccountId` | `[ValidGuid]`. |
| `Description` | `[Required]`. |
| `Percentage` | This split's share, `0`–`100`. **All splits for a levy must sum to 100%** — enforced server-side (§4.6). |
| `LevyId` | Set server-side from the route — ignored on write. |

## 4. Endpoints — `api/accounts/levies`

### 4.1 List (unpaged) — `GET /`

```json
{ "success": true, "message": "", "data": LevyDTO[] }
```

### 4.2 List (paged/search) — `GET /paged?text=&pageIndex=&pageSize=`

`data` is `PageCollectionInfo<LevyDTO>`.

### 4.3 Get one — `GET /{id}`

`404` if not found.

### 4.4 Create — `POST /`

Body:
```ts
interface CreateLevyRequest {
  levy: LevyDTO;
  levySplits?: LevySplitDTO[];  // optional; if sent, must sum to 100%
}
```

`400` if `levy` is missing/fails `ValidateAll()`, or `levySplits` is
non-empty and doesn't sum to 100%. No `409` case (see history note).

On success, `levySplits` is saved as a full replace (same as
`PUT /{id}/splits`), then the levy is re-fetched so `data` reflects what
was actually persisted.

### 4.5 Update — `PUT /{id}`

Body: `LevyDTO`. **Only updates the levy's own fields** — does not touch
splits (see history note; this is the fix for the reference app's
edit-wipes-splits bug). `404` if `id` doesn't resolve.

### 4.6 Splits — `GET`/`PUT /{id}/splits`

`PUT` body: `LevySplitDTO[]`. **Full replace** — every existing split for
this levy is deleted and replaced with exactly what you send (`[]` clears
all). `400` if the list is non-empty and percentages don't sum to 100%
(±0.01 tolerance). `404` if the levy id doesn't resolve.
