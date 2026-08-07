# Bank API — Client Integration Spec

Audience: admin screens that list, create, or edit banks and their branches
(used elsewhere to identify where a customer's external bank account/cheque
is held — not to be confused with `branch-api-spec.md`, which covers this
SACCO's own operating branches).

Source of truth:
- Controller: `WebApplication1/Areas/Admin/Controllers/BankController.cs`
- Domain service: `Application.MainBoundedContext/AdministrationModule/Services/IBankAppService.cs`
- Core DTOs: `Application.MainBoundedContext.DTO.AdministrationModule/BankDTO.cs`,
  `BankBranchDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

## History note

The reference MVC controller this replaces (`SwiftFinancials.Web` Admin
area) mixed several patterns that were **not** carried forward:

- **`DeleteBank(Guid id)`** ran a raw `SqlCommand` `DELETE FROM
  swiftFin_BankBranches WHERE Id = @Id` directly against the database,
  bypassing the domain layer entirely — and despite its name it deleted a
  *branch* row, not a bank. It wasn't wired to any reachable route in the
  old app. **Not ported.** There is no delete endpoint for either a bank or
  a branch in this API — `IBankAppService` has no delete operation to route
  through, and reintroducing raw SQL to fake one isn't in scope. If a real
  delete/lock-style operation for banks is needed later, it needs to be
  added to `IBankAppService` first.
- **Branch editing was session-based**: the old `Create`/`Edit` screens
  staged the branch list in `Session["bankBranches"]` via separate
  `Add`/`Remove` AJAX calls, then saved everything together on final
  submit. That has no place in a stateless API — branches are now just part
  of the `POST`/`PUT` request body (see §4.6/§4.7).
- **`GET /` paginated in memory**: the old `Index` action always fetched
  every row (`pageSize = int.MaxValue`) from the service and paged with
  in-process `Skip`/`Take`. This API pushes `pageIndex`/`pageSize` straight
  through to `IBankAppService`, same as every other paged endpoint.

- **`BankDTO` no longer carries bank-*linkage* fields.** It used to also
  have a long tail of unrelated fields (`bankName`, `branchId`,
  `chartOfAccountId`, ...) bolted on from a different screen — those have
  been split out onto their own `BankLinkageDTO`, served by a dedicated
  [`bank-linkage-api-spec.md`](bank-linkage-api-spec.md). `BankDTO` now only
  carries real bank fields, and `POST`/`PUT` validate it with the normal
  `BankDTO.ValidateAll()` (see §4.5) — no more hand-rolled field checks.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/administration/banks` |
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
- `201 Created` — successful `POST`.
- `400 Bad Request` — missing/invalid body, id mismatch on `PUT`, or a bank
  validation failure (`message` is the joined validation error text — see
  §4.6 for exactly which fields are checked).
- `404 Not Found` — id doesn't resolve.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

## 3. Paging shape

`GET /` returns `PageCollectionInfo<BankDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

Sorted by `sequentialId` ascending (insertion order, oldest first), same
convention as `branch-api-spec.md` §3.

## 4. Endpoints

All routes below are relative to `/api/administration/banks`.

### 4.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `text`. Omitting `text` returns the plain paged listing; supplying it
runs `BankSpecifications.BankFullText` (matches the bank's numeric `code`
or its `description`).

### 4.2 List all (unpaged) — `GET /all`

Every bank in the system, no filter, no paging. Returns
`ApiEnvelope<BankDTO[]>` (empty array, not `null`, if there are none —
safe to iterate directly). Fine for a dropdown; prefer §4.1 for a primary
listing screen.

### 4.3 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<BankDTO>`.

`BankDTO` fields: `id`, `code`, `paddedCode` (computed, zero-padded
`code`), `description`, `address`, `city`, `ibanNo`, `swiftCode`,
`createdDate`, `no` (identity-generated). This endpoint does **not**
populate the DTO's branch collections (`bankBranche`/`bankBranchesDTO`) —
use §4.4 to fetch a bank's branches. To link a bank to one of this SACCO's
own branches + a G/L account, see `bank-linkage-api-spec.md`.

### 4.4 Get a bank's branches — `GET /{id}/branches`

`404` if the bank id doesn't resolve. Returns
`ApiEnvelope<BankBranchDTO[]>` (empty array, not `null`, if the bank has no
branches).

### 4.5 Create — `POST /`

Body:
```ts
interface CreateBankRequest {
  bank: BankDTO;      // description, address, city, ibanNo, swiftCode required
  branches?: BankBranchDTO[];  // optional; omit or send [] for none
}
```

`bank.code` is not server-assigned — send an explicit numeric code (the old
reference app's DataTables grid displayed it zero-padded via
`bank.paddedCode`, but nothing computes/reserves it for you; picking a
unique one is the caller's responsibility, same as the underlying domain
factory expects).

Runs `BankDTO.ValidateAll()` — `description`, `address`, `city`, `ibanNo`,
`swiftCode` are `[Required]`; returns `400` with the joined validation
message if any are missing.

If `branches` is supplied and non-empty, they're saved immediately after
the bank is created (a second call under the hood, not atomic with the
bank insert — if branch save fails the bank still exists; re-`PUT` the
branches to retry).

Success → `201`:
```json
{
  "success": true,
  "message": "Bank created successfully",
  "data": { "bank": BankDTO, "branches": BankBranchDTO[] }
}
```

### 4.6 Update — `PUT /{id}`

Body: same `CreateBankRequest` shape as create, with `bank.id` matching the
path segment (`400` if missing/mismatched, `404` if the id doesn't
resolve). Same field-level validation as create.

`branches` fully **replaces** the bank's existing branch list — every
current branch is removed and the supplied list is re-created in its
place. Omit `branches` entirely (leave it `undefined`/absent) to leave the
existing branches untouched; send `branches: []` to clear them all.

Returns the re-fetched bank plus its current branches:
```json
{
  "success": true,
  "message": "Bank updated successfully",
  "data": { "bank": BankDTO, "branches": BankBranchDTO[] }
}
```
