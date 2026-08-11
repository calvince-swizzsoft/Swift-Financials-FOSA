# Commission API — Client Integration Spec

Audience: admin screens that configure `Commission` records — the fee/tariff
master data almost every other module attaches to (cheque types, savings
products, loan products, credit types, alternate channels, wire transfers,
unpay reasons, text alerts). For how Commission relates to Levy/Charge/
DynamicCharge conceptually, see `COMMISSION-LEVY-CHARGE-CONCEPTS.md`. For
Levy's own CRUD, see `docs/api/levy-api-spec.md`.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/CommissionController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/CommissionAppService.cs`
  (`ICommissionAppService`).
- DTOs: `CommissionDTO`, `GraduatedScaleDTO`, `CommissionSplitDTO`, `LevyDTO`
  (`Application.MainBoundedContext.DTO/AccountsModule/`).
- Auth: same JWT bearer scheme as every other controller — `[Authorize]`.

## History note

Previously this controller had one endpoint (`GET /`, unpaged — kept as-is
below, see §5.1) added to unblock `ChequeTypeController`'s Create picker.
Full CRUD plus the graduated-scale/split/levy sub-resources were added by
checking the reference MVC app (`SwiftFinancials.Web` Areas/Accounts).

**What the reference app actually had, and what wasn't ported:**
- `CommissionController.cs` — real, working CRUD, but `Edit` never touched
  splits/levies (only `Create` did).
- `ChargesController.cs` — a second, mostly-duplicate reimplementation of
  the same Commission CRUD, session/TempData-heavy, with dead/commented
  validation code and an inconsistency where one `Create` branch
  (`splitCount == 1`) saves graduated scales + levies + splits, but the
  other (`splitCount > 1`) only saves splits. **Not ported** — confirmed
  redundant with `CommissionController`.
- `TiersController.cs` (meant to own `GraduatedScale`) — **never actually
  worked.** Its `Create` POST has the real persistence call commented out.
  **Not ported.**

Given `GraduatedScale`/`CommissionSplit` never had a clean, working,
standalone screen, they're sub-resources of `Commission` here (§5.5–§5.7),
matching the pattern already established by `ChequeTypeController`'s
commissions/attached-products sub-resources.

## 1. Response envelope

`{ success: boolean, message: string, data: T | null }`, same as every
other spec in this set.

## 2. `CommissionDTO` — core fields

The full DTO carries a lot of legacy/unused fields (see the source) — these
are the ones that matter:

| Field | Notes |
|---|---|
| `Id` | Server-assigned on create. |
| `Description` | `[Required]`. Must be unique on **create** — see §5.4. |
| `MaximumCharge` | Caps the computed charge from whichever `GraduatedScale` bracket applies. |
| `RoundingType` | `RoundingType` enum. |
| `IsLocked` | Plain bool. |
| `ChargeTypeDescription`, `ChargeBenefactorDescription`, `ChargeBasisValueDescription`, `KnownChargeTypeDescription`, `ComplementTypeDescription` | Computed, read-only. |
| `ErrorMessageResult` | Duplicate-`Description` channel on create (§5.4) — always `null` otherwise. |
| `ChartOfAccountId` | Optional, unvalidated. Despite the `[Display(Name = "G/L Account")]`, this value is never read by `CommissionAppService` — GL posting actually uses each `CommissionSplitDTO.ChartOfAccountId` (§4). Previously carried a `[ValidGuid]` attribute that wrongly forced every create request to supply a dummy GUID here just to pass validation; removed since the field is dead. |

`GraduatedScale`, `CommissionSplit`, and `Levy` are **not** fields on this
DTO in practice (the DTO has some legacy list properties like `Levies`/
`CommissionSplits` but this controller does not read or populate them) —
use the sub-resource endpoints.

## 3. `GraduatedScaleDTO` — rate by amount bracket

| Field | Notes |
|---|---|
| `Id` | Server-assigned. |
| `RangeLowerLimit` / `RangeUpperLimit` | The transaction-amount bracket this rate applies to. |
| `ChargeType` | `ChargeType` enum: `Percentage` or `FixedAmount`. |
| `ChargePercentage` / `ChargeFixedAmount` | Only the one matching `ChargeType` is used server-side (`Domain.MainBoundedContext.ValueObjects.Charge`'s constructor zeroes the other). |
| `CommissionId` | Set server-side from the route — ignored on write. |

## 4. `CommissionSplitDTO` — how the computed amount divides across GL accounts

| Field | Notes |
|---|---|
| `Id` | Server-assigned. |
| `ChartOfAccountId` | `[ValidGuid]`. Which account this portion posts to. |
| `Description` | `[Required]`. |
| `Percentage` | This split's share. **All splits for a commission must sum to 100%** — enforced server-side (§5.6), the one real validation rule kept from the reference app's `ChargesController` (most of its other percentage-checking code was dead/commented out). |
| `Leviable` | If `true`, this split's amount feeds the Levy calculation (`COMMISSION-LEVY-CHARGE-CONCEPTS.md` §2) — a levy is computed as a percentage/fixed-amount of the sum of `Leviable` splits, never of the raw transaction. |
| `CommissionId` | Set server-side from the route — ignored on write. |

## 5. Endpoints — `api/accounts/commissions`

### 5.1 List (unpaged) — `GET /`

**Unchanged contract** — do not add paging here, it's relied on by pickers
(`ChequeTypeController`'s Create form). Returns every commission:

```json
{ "success": true, "message": "", "data": CommissionDTO[] }
```

### 5.2 List (paged/search) — `GET /paged?text=&pageIndex=&pageSize=`

For the admin screen's own listing. `text` optional; `pageIndex` defaults
`0`, `pageSize` defaults `20`. `data` is `PageCollectionInfo<CommissionDTO>`.

### 5.3 Get one — `GET /{id}`

`404` if not found.

### 5.4 Create — `POST /`

Body:
```ts
interface CreateCommissionRequest {
  commission: CommissionDTO;
  graduatedScales?: GraduatedScaleDTO[];  // optional — stage without a rate yet
  splits?: CommissionSplitDTO[];          // optional; if sent, must sum to 100%
  levies?: LevyDTO[];                     // optional — only .id is read, links existing Levy records
}
```

`400` if `commission` is missing/fails `ValidateAll()` (missing
`Description`), or `splits` is non-empty and doesn't sum to 100%. `409` if
`Description` already exists (`ErrorMessageResult` echoed as `message`,
`data: null`) — matches `CostCenterController`'s duplicate-description
pattern.

On success, graduated scales/splits/levies are saved as a **full replace**
each (same as their individual `PUT` sub-resource endpoints below), then
the commission is re-fetched so `data` reflects everything that was
actually persisted.

### 5.5 Update — `PUT /{id}`

Body: `CommissionDTO`. **Only updates the commission's own fields** —
does not touch graduated scales, splits, or levies (the reference app's
`Edit` didn't either; made explicit here rather than silently inconsistent
with `Create`). `404` if `id` doesn't resolve. No duplicate-`Description`
check on update (only on create — same asymmetry as `CostCenterController`).

### 5.6 Graduated scales — `GET`/`PUT /{id}/graduated-scales`

`PUT` body: `GraduatedScaleDTO[]`. **Full replace** — every existing
bracket for this commission is deleted and replaced with exactly what you
send (`[]` clears all). `404` if the commission id doesn't resolve.

### 5.7 Splits — `GET`/`PUT /{id}/splits`

`PUT` body: `CommissionSplitDTO[]`. **Full replace**, same semantics as
graduated scales. `400` if the list is non-empty and percentages don't sum
to 100% (±0.01 tolerance). `404` if the commission id doesn't resolve.

### 5.8 Levies — `GET`/`PUT /{id}/levies`

`PUT` body: `LevyDTO[]` — only each entry's `.id` is read (this links
*existing* `Levy` records to the commission via the `CommissionLevy` join;
it does not create or edit levies — use `LevyController` for that). **Full
replace**, same semantics as the other two sub-resources. `404` if the
commission id doesn't resolve.
