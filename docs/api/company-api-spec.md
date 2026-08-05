# Company API — Client Integration Spec

Audience: admin screens that list, create, or edit companies (the tenant/SACCO
entity that branches, customers, and products all roll up to), plus the two
company-level sub-resources — mandatory debit types and mandatory attached
products (savings/investment) — that get provisioned onto every new customer
at that company.

Source of truth:
- Controller: `WebApplication1/Areas/Admin/Controllers/CompanyController.cs`
- Domain service: `Application.MainBoundedContext/AdministrationModule/Services/ICompanyAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO.AdministrationModule/CompanyDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

Related:
- `docs/api/customer-verification-api-spec.md` and
  `docs/api/customer-account-verification-api-spec.md` — the two
  maker-checker flags this controller lets you set
  (`enforceCustomerMakerChecker`, `enforceCustomerAccountMakerChecker`)
  live on the `CompanyDTO` this controller returns/accepts.

## History note

This controller is a from-scratch adaptation of the old MVC
`Areas/Admin/Controllers/CompanyController.cs` (routed everything through
the monolithic `_channelService` and rendered Razor views). The new one
routes through `ICompanyAppService` directly and drops the reference
controller's dead code: an unused `Companylogo` upload parameter and a set
of raw-`SqlConnection` passport/signature/ID-photo capture helpers
(`GetDocumentsAsync`/`ProcessDocumentUpload`/`SaveDocumentAsync`) that were
defined but never actually called from any action in the old controller.

**One behavior was *not* carried forward, deliberately left for the
frontend to decide**: the old controller's `Create` action unconditionally
overwrote `RecoveryPriority = "DirectDebits"` on every company, regardless
of what was submitted. That looked like a leftover hack rather than an
intentional default, so the new `POST /` endpoint saves whatever
`recoveryPriority` you send as-is (including empty/null if you don't set
it). If your UI relied on the server silently forcing that value, set it
explicitly in the request body now.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/administration/companies` |
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
- `404 Not Found` — `GET /{id}` only, when the id doesn't resolve.
- `400 Bad Request` — `POST /` with no `company` in the body, or `PUT /{id}`
  with a missing/mismatched `id`.
- `500 Internal Server Error` — either an unhandled exception (`message` is
  the raw `ex.Message`), or a caught-but-failed update
  (`UpdateCompany`/`UpdateDebitTypes`/`UpdateAttachedProducts` returning
  `false`) surfaced as a generic `"Failed to update ..."` message. There's
  no `409`-style distinction here between "validation failed" and "DB write
  failed" — check `message`.

## 3. Paging shape

`GET /` returns `PageCollectionInfo<CompanyDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

Sorted by `sequentialId` ascending (insertion order, oldest first) — not
`createdDate` descending (newest first) like the old MVC grid was. If your
UI expects newest-first like the old grid, sort client-side.

## 4. Endpoints

All routes below are relative to `/api/administration/companies`.

### 4.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `text`. Omitting `text` returns the plain paged listing; supplying it
runs `CompanySpecifications.CompanyFullText` (matches across the company's
description/registration fields).

### 4.2 List all (unpaged) — `GET /all`

Every company in the system, no filter, no paging. Returns
`ApiEnvelope<CompanyDTO[]>`. Fine for an admin dropdown (companies are a
low-cardinality tenant list); don't use it for a primary listing screen —
prefer §4.1.

### 4.3 Count — `GET /count`

Returns `ApiEnvelope<number>` — total company count, unfiltered.

### 4.4 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<CompanyDTO>`.

`CompanyDTO` is large (60+ fields) — mostly per-company toggles
(`bypassJournalVoucherAudit`, `enforceTellerLimits`,
`enforceTwoFactorAuthentication`, etc.) plus the two maker-checker flags
called out above. It does **not** include the company's debit types or
attached products inline — those are separate calls, §4.6–4.9.

### 4.5 Create — `POST /`

Body:
```ts
interface CreateCompanyRequest {
  company: CompanyDTO;               // required — 400 if missing
  mandatoryDebitTypes?: DebitTypeDTO[];
  mandatoryProducts?: ProductCollectionInfo;
}
```

`ProductCollectionInfo` here only needs `investmentProductCollection` and
`savingsProductCollection` populated (each entry only needs `id` set — see
§4.9); the other collections on that type are ignored by this endpoint.

Creates the company, then — if provided — attaches the mandatory debit
types and mandatory products in the same call (two follow-up internal
calls, not atomic with the company create: if those fail, the company
still exists but the response won't reflect it since `data` is just the
created `CompanyDTO`, not the attached products/debit types).

Success:
```json
{ "success": true, "message": "...", "data": CompanyDTO }
```

### 4.6 Update — `PUT /{id}`

Body: full `CompanyDTO`, with `id` matching the path segment (`400` if
missing or mismatched). Updates the company record itself only — debit
types and attached products are **not** touched by this call even if
present in the body; use §4.8/§4.9 for those. Returns the re-fetched
`ApiEnvelope<CompanyDTO>` on success.

Setting `isLocked: true` on a previously-unlocked company locks it as part
of this call (`ICompanyAppService.UpdateCompany` calls `LockCompany`
internally when it detects that transition) — there's no separate
lock/unlock endpoint.

### 4.7 Get debit types — `GET /{id}/debit-types`

Returns `ApiEnvelope<DebitTypeDTO[]>` — the company's currently-mandatory
debit types.

### 4.8 Replace debit types — `PUT /{id}/debit-types`

Body: `DebitTypeDTO[]` (each entry only needs `id` set). **Full replace**,
not a merge/diff — every existing mandatory debit type for the company is
deleted and re-inserted from this list, including sending `[]`/`null` to
clear them all. Returns the re-fetched list on success.

### 4.9 Get attached products — `GET /{id}/attached-products`

Returns `ApiEnvelope<ProductCollectionInfo>` — the company's currently
mandatory savings/investment products (`investmentProductCollection` /
`savingsProductCollection`; other fields on `ProductCollectionInfo` are
unused by this endpoint and come back empty).

### 4.10 Replace attached products — `PUT /{id}/attached-products`

Body: `ProductCollectionInfo` (only `investmentProductCollection` /
`savingsProductCollection` matter; each entry only needs `id` set). Same
**full replace** semantics as §4.8 — everything currently attached is
deleted and re-inserted from this payload. Returns the re-fetched value on
success.
