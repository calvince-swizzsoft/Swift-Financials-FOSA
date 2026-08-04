# Customer Account Signatory API — Client Integration Spec

Audience: screens that manage who is authorized to sign/operate on a
customer account (joint accounts, group/corporate accounts, next-of-kin
style authorized signatories) — separate from the *account itself* and
separate from the account-management "signing instructions" action (see
`docs/api/customer-account-management-api-spec.md` §3).

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Accounts/Controllers/CustomerAccountSignatoryController.cs`
- Domain service it calls: `Application.MainBoundedContext/AccountsModule/Services/ICustomerAccountAppService.cs`
  (`FindCustomerAccountSignatoriesByCustomerAccountId`, `AddNewCustomerAccountSignatory`, `RemoveCustomerAccountSignatories`)
- DTO: `Application.MainBoundedContext.DTO/AccountsModule/CustomerAccountSignatoryDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/customer-accounts` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | Bearer JWT on every request |

Shares its base path with `CustomerAccountsController` and
`CustomerAccountManagementController` — these are additional sub-routes on
the same resource.

## 2. Response envelope

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success, or a caught business error (`success: false`).
- `201 Created` — successful `POST` (signatory added).
- `400 Bad Request` — validation error, or an empty id list on delete.
- `404 Not Found` — `customerAccountId` doesn't resolve to an account.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

## 3. No update endpoint

The underlying app service only has Add and (bulk) Remove — there is no
"update a signatory" or "remove one signatory by id" operation. To edit a
signatory's details, remove it (§4.4, single-element array) and re-add it
(§4.3). This is a limitation of the domain layer as it exists today, not
something the controller works around.

## 4. Endpoints

All routes below are relative to `/api/accounts/customer-accounts`.

### 4.1 List (paged) — `GET /{customerAccountId}/signatories`

Query: `pageIndex` (default `0`), `pageSize` (default `20`). `404` if the
account doesn't exist. Returns `ApiEnvelope<PageCollectionInfo<CustomerAccountSignatoryDTO>>`.

### 4.2 List (all) — `GET /{customerAccountId}/signatories/all`

Same, unpaged. Returns `ApiEnvelope<CustomerAccountSignatoryDTO[]>`. Prefer
§4.1 for anything rendered as a table; use this only where you genuinely
need the full set (e.g. a signature-count validation before a withdrawal).

### 4.3 Add — `POST /{customerAccountId}/signatories`

Body: `CustomerAccountSignatoryDTO` (the controller overwrites
`customerAccountId` from the URL, so you don't need to set it in the body).
Required fields (server-validated via `ValidateAll()`, `400` with joined
`ErrorMessages` if invalid): `firstName`, `lastName`, `identityCardNumber`.

```ts
interface CustomerAccountSignatoryDTO {
  customerAccountId: string;         // set from the URL, ignore on the way in
  salutation: number;                // Salutation enum — see customer-api-spec.md §7
  gender: number;                    // Gender enum — see customer-api-spec.md §7
  relationship: number;              // SignatoryRelationship enum, see §5 below
  firstName: string;                 // required
  lastName: string;                  // required
  identityCardType: number;          // IdentityCardType enum — see customer-api-spec.md §7
  identityCardNumber: string;        // required
  addressAddressLine1?: string;
  addressAddressLine2?: string;
  addressStreet?: string;
  addressPostalCode?: string;
  addressCity?: string;
  addressEmail?: string;             // validated email format if supplied
  addressLandLine?: string;
  addressMobileLine?: string;        // must start with "+" and country code if supplied
  remarks?: string;
}
```

Success → `201`:
```json
{ "success": true, "message": "Signatory added successfully", "data": CustomerAccountSignatoryDTO }
```

### 4.4 Remove (bulk) — `DELETE /signatories`

Not scoped under a `customerAccountId` — signatory ids are globally unique,
so this takes a flat array of ids and removes whichever exist:

```ts
type RemoveSignatoriesRequest = string[];  // array of signatory GUIDs
```

`400` if the array is empty/missing. `data` is `null`; `success: true`
means at least one signatory was removed — ids that don't resolve are
silently skipped rather than erroring, so a partial match still returns
`success: true`. If you need to confirm exactly which ones were removed,
re-fetch the list (§4.1/§4.2) afterward.

## 5. `SignatoryRelationship` enum

| Value | Description |
|---|---|
| `0` | Unknown |
| `57023` | Father |
| `57024` | Mother |
| `57025` | Brother |
| `57026` | Sister |
| `57027` | Wife |
| `57028` | Husband |
| `57029` | Son |
| `57030` | Daughter |
