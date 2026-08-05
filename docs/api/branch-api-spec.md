# Branch API — Client Integration Spec

Audience: admin screens that list, create, or edit branches (the physical
locations a company operates from — customers, employees, and most other
records are scoped to one).

Source of truth:
- Controller: `WebApplication1/Areas/Admin/Controllers/BranchController.cs`
- Domain service: `Application.MainBoundedContext/AdministrationModule/Services/IBranchAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO.AdministrationModule/BranchDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

## History note

This replaces a prior `BranchesController` (note the old plural name) that
routed through a hand-rolled `WebApplication1/Services/BranchService.cs`
doing raw `SqlConnection`/`SqlCommand` queries directly against the
`swiftFin_Branches`/`swiftFin_Companies` tables — bypassing the domain layer
entirely (no validation, no `CreatedBy` audit trail, a hard `DELETE`
inconsistent with how every other aggregate in this codebase handles removal,
and `[AllowAnonymous]` + wildcard CORS that exempted it from the auth
requirement every other controller enforces). Both the old controller and the
raw-SQL service class have been **deleted**. This controller now routes
entirely through `IBranchAppService`, same as everything else.

**Behavior changes from the old controller, if you integrated against it:**
- **Auth is now required** — the old `[AllowAnonymous]`/wildcard-CORS
  exemption is gone. If you were calling this without a bearer token, that
  will now fail.
- **`DELETE /{id}` no longer exists.** Branches follow the same soft-lock
  convention as every other aggregate here (`Company`, `Customer`,
  `SavingsProduct`, ...) — there is no hard delete. Use
  `PATCH /{id}/toggle-lock` instead.
- **Validation is now enforced** — `POST /` and `PUT /{id}` now run
  `BranchDTO.ValidateAll()` and reject with `400` on failure; the old
  raw-SQL path accepted anything.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/administration/branches` |
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
- `400 Bad Request` — missing/invalid body, id mismatch on `PUT`, or
  `BranchDTO.ValidateAll()` failure (`message` is the joined validation
  error text).
- `404 Not Found` — id/code doesn't resolve.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

## 3. Paging shape

`GET /` returns `PageCollectionInfo<BranchDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

Sorted by `sequentialId` ascending (insertion order, oldest first) — same
convention as `company-api-spec.md` §3.

## 4. Endpoints

All routes below are relative to `/api/administration/branches`.

### 4.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `text`. Omitting `text` returns the plain paged listing; supplying it
runs `BranchSpecifications.BranchFullText` (matches on branch description).

### 4.2 List all (unpaged) — `GET /all`

Every branch in the system, no filter, no paging. Returns
`ApiEnvelope<BranchDTO[]>`. Fine for a dropdown; prefer §4.1 for a primary
listing screen.

### 4.3 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<BranchDTO>`.

`BranchDTO` includes a large set of flattened `companyXxx` fields (company
description/address, and every `companyEnforceXxx`/`companyBypassXxxAudit`
config flag) — these are read-only denormalizations of the branch's parent
company, populated automatically; don't attempt to write them back through
this controller, they're ignored on `POST`/`PUT`.

### 4.4 Get by code — `GET /by-code/{code}`

`code` is the branch's short numeric code (not its GUID id). `404` if not
found. Returns `ApiEnvelope<BranchDTO>`.

### 4.5 Get by company — `GET /by-company/{companyId}`

All branches belonging to one company. Returns `ApiEnvelope<BranchDTO[]>`
(empty array, not `null`, if the company has none — safe to iterate
directly).

### 4.6 Create — `POST /`

Body: `BranchDTO` with `description` populated (`400` with the specific
validation message if `ValidateAll()` fails — it's the only field
`[Required]`-annotated on the DTO). `companyId` isn't annotated required, but
a branch with no company is meaningless — always send it. `code` is
server-assigned
(`MAX(Code)+1` per branch, same pattern as `Company`/`SavingsProduct`) — any
value you send is ignored.

Success → `201`:
```json
{ "success": true, "message": "Branch created successfully", "data": BranchDTO }
```

### 4.7 Update — `PUT /{id}`

Body: full `BranchDTO`, with `id` matching the path segment (`400` if
missing/mismatched, `404` if the id doesn't resolve). Runs the same
`ValidateAll()` check as create. Returns the re-fetched
`ApiEnvelope<BranchDTO>` on success.

### 4.8 Toggle lock — `PATCH /{id}/toggle-lock`

No body. Flips `isLocked` on the branch and returns the updated
`ApiEnvelope<BranchDTO>`. `404` if the id doesn't resolve.
