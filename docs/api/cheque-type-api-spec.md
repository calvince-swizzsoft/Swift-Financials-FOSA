# Cheque Type API — Client Integration Spec

Audience: admin screens that manage `ChequeType` master data (e.g. "Standard
Cheque", "Bankers Cheque") — the maturity period a deposited cheque of that
type takes to clear, which commissions apply to it, and which loan/investment
products it's allowed to fund.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/ChequeTypeController.cs`
- Domain service: `Application.MainBoundedContext/AccountsModule/Services/IChequeTypeAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO/AccountsModule/ChequeTypeDTO.cs`
- Related: `ChequeTypeCommissionDTO`/`ChequeTypeAttachedProductDTO` (join
  records, not exposed directly — see §4), `ProductCollectionInfo`
  (`Application.MainBoundedContext.DTO/ProductCollectionInfo.cs`).
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

## History note

`ChequeTypeDTO`, its domain aggregate, and `IChequeTypeAppService` (plus the
commission-linking and attached-product-linking pieces) already existed in
this codebase before this doc — they just had no dedicated controller.

The reference MVC controller (`SwiftFinancials.Web` Accounts area,
`ChequeTypeController`) was **not** ported structurally. It's a session-heavy
multi-step wizard built for a WCF `_channelService` proxy this codebase
doesn't have: the create screen posts selected commissions and selected
loan/investment products to three separate session-staging endpoints
(`StoreSelectedApplicableCharges`, `StoreSelectedLoanProducts`,
`StoreSelectedInvestmentProducts`) before the real `POST Create` reads all
three back out of `Session[...]`. There's no session here, so `POST /`
takes one request body carrying the cheque type plus both selections
together — same validation rule the reference `Create` enforced (charges
required, and at least one loan or investment product required), just
without the session round-trips.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/chequetypes` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | Bearer JWT on every request |

## 2. Response envelope

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success.
- `400 Bad Request` — missing/invalid body, a `ChequeTypeDTO.ValidateAll()`
  failure (`message` is the joined validation error text), or `POST /`
  missing commissions/products (see §5.3).
- `404 Not Found` — id doesn't resolve.
- `500 Internal Server Error` — unhandled exception, or a cheque type that
  posted successfully but whose commission/product save failed (`message`
  explains which; `data` still carries the created `ChequeTypeDTO` in that
  last case so the client isn't left without the new id).

## 3. Paging shape

`GET /` returns `PageCollectionInfo<ChequeTypeDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

## 4. `ChequeTypeDTO` field reference

| Field | Notes |
|---|---|
| `id` | Server-assigned on create. |
| `description` | `[Required]`. E.g. "Standard Cheque". |
| `maturityPeriod` | Int, days until a deposited cheque of this type clears. No range validation on the DTO — the app service accepts any value including `0`/negative. |
| `chargeRecoveryMode` | `ChequeTypeChargeRecoveryMode`: `0`=`OnChequeDeposit`, `1`=`OnChequeClearance`. |
| `chargeRecoveryModeDescription` | Computed, read-only — ignored on write. |
| `isLocked` | Plain bool — **not** wired to any lock/unlock endpoint here, unlike `Branch`'s `PATCH /toggle-lock`. Settable via `PUT` like any other field. |
| `createdDate` | Server-assigned on create. |

Commissions and attached products are **not** fields on `ChequeTypeDTO` —
they're separate join tables (`ChequeTypeCommission`, one row per
`chequeTypeId`+`commissionId`; `ChequeTypeAttachedProduct`, one row per
`chequeTypeId`+`productCode`+`targetProductId`), managed through their own
sub-resource endpoints (§5.4–§5.7).

## 5. Endpoints

All routes below are relative to `/api/accounts/chequetypes`.

### 5.1 List / search — `GET /`

Query params (all optional): `text`, `pageIndex` (default `0`), `pageSize`
(default `20`). Omitting `text` returns the plain paged listing; supplying
it runs the underlying full-text spec on the cheque type.

### 5.2 List all (unpaged) — `GET /all`

Every cheque type in the system, no filter, no paging. Returns
`ApiEnvelope<ChequeTypeDTO[]>` (empty array, not `null`, if there are none).

### 5.3 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<ChequeTypeDTO>`.

### 5.4 Create — `POST /`

Body:
```ts
interface CreateChequeTypeRequest {
  chequeType: ChequeTypeDTO;
  commissions: CommissionDTO[];   // at least one required
  attachedProducts: ProductCollectionInfo; // at least one loan or investment product required
}
```

`400` (`data: null`) if:
- `chequeType` is missing, or fails `ValidateAll()` (missing `description`).
- `commissions` is missing or empty — `message`: `"No charges selected"`.
- `attachedProducts` has no entries in **both**
  `loanProductCollection` and `investmentProductCollection` —
  `message`: `"No products selected"`. `savingsProductCollection` and the
  other `ProductCollectionInfo` fields are accepted but ignored by this
  check (matches `IChequeTypeAppService.FindAttachedProducts`, which only
  ever returns loan/investment products for a cheque type).

On success the cheque type is created first, then commissions and attached
products are saved as a **full replace** (see §5.5/§5.7 — there's no
"append" semantics). If either of those two saves fails after the cheque
type itself was created, the response is `500` but `data` still carries the
created `ChequeTypeDTO` — the record exists, only its commissions/products
are incomplete; re-`PUT` §5.5/§5.7 individually to fix up rather than
retrying `POST /` (which would create a duplicate cheque type).

Success:
```json
{ "success": true, "message": "Cheque type, commissions, and attached products successfully created/updated", "data": ChequeTypeDTO }
```

### 5.5 Update — `PUT /{id}`

Body: `ChequeTypeDTO` (`id` in the body is overwritten from the path
segment — no mismatch check needed). `400` with the joined validation
message on a `ValidateAll()` failure, `404` if the id doesn't resolve.
Returns the re-fetched `ApiEnvelope<ChequeTypeDTO>` on success. Does **not**
touch commissions or attached products — use §5.6/§5.7 for those.

### 5.6 Commissions

- `GET /{id}/commissions` — `ApiEnvelope<CommissionDTO[]>` (empty array if
  none set).
- `PUT /{id}/commissions` — body: `CommissionDTO[]`. **Full replace**: every
  existing commission link for this cheque type is deleted and replaced
  with exactly what you send (send `[]` to clear all). Only each
  `CommissionDTO.id` is read — the rest of the object is ignored. Returns
  the re-fetched list. `500` if the cheque type id doesn't resolve or the
  save otherwise fails.

### 5.7 Attached products

- `GET /{id}/attached-products` — `ApiEnvelope<ProductCollectionInfo>`.
  Only `loanProductCollection` and `investmentProductCollection` are ever
  populated by this endpoint (matches what `UpdateAttachedProducts`
  persists) — the other `ProductCollectionInfo` fields (`savingsProductCollection`,
  the `EligibileIncomeDeduction*` fields, `concessionLoanProductCollection`)
  are always empty here, they belong to other aggregates that happen to
  reuse the same wrapper type.
- `PUT /{id}/attached-products` — body: `ProductCollectionInfo`. **Full
  replace** of both the loan and investment product links for this cheque
  type (send empty arrays to clear). Returns the re-fetched value. `500` if
  the cheque type id doesn't resolve or the save otherwise fails.

No delete endpoint for the cheque type itself — `IChequeTypeAppService`
doesn't expose one.
