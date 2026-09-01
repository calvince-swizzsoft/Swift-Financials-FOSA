# Customer API — Client Integration Spec

Audience: the React (Vite) ERP frontend that creates and processes customer
records against `WebApplication1`'s `CustomerController`.

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Registry/Controllers/CustomerController.cs`
- Domain service it calls: `Application.MainBoundedContext/RegistryModule/Services/ICustomerAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO/RegistryModule/CustomerDTO.cs`
- Auth: `WebApplication1/Areas/Auth/Controllers/AuthController.cs`, `WebApplication1/Areas/Auth/JwtAuthenticationHandler.cs`

If the controller changes, regenerate this doc from source rather than editing
it out of sync — several endpoints below intentionally omit fields the backend
doesn't validate, and that only stays true as long as it's re-checked against
the controller.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/registry/customer` |
| Transport | HTTPS only |
| Content type | `application/json` (request and response bodies) |
| CORS | `http://localhost:5173` is allow-listed in `WebApiConfig.cs` (Vite dev server). Any other dev origin needs to be added there — it isn't wildcard-permissive for credentials. |
| Auth | Bearer JWT on every request except `POST /api/auth/login` |

This is a **separate route from the older `CustomersController`**
(`api/registry/customers`, plural). That controller is a legacy, raw-SQL-backed
implementation with its own duplicate-check/SMS-on-create logic. Do not mix
the two in the same feature — this spec covers `api/registry/customer`
(singular) only, which is backed by the proper `ICustomerAppService` domain
layer.

## 2. Authentication

```
POST /api/auth/login
Content-Type: application/json

{ "userName": "string", "password": "string" }
```

Response `200`:
```json
{ "token": "<jwt>", "userName": "string", "roles": ["string", "..."] }
```
`401` if the credentials are invalid.

Every subsequent request must carry:
```
Authorization: Bearer <token>
```

`JwtAuthenticationHandler` rejects anything that isn't exactly `Bearer <token>`
in the `Authorization` header — no cookie-based fallback exists. The server
derives `ApplicationUserName` and `ApplicationUserRoles` for every call from
the validated token's claims (`WebApplication1/Helpers/Utils.cs`,
`CreateServiceHeader()`); the client never sends identity fields itself, and
the `CustomerController` currently has no `[Authorize(Roles=...)]` restriction
of its own — any authenticated user can call any endpoint on it. If
role-gating is required (e.g. "only branch staff can create"), it must be
enforced client-side for now, or the controller needs role attributes added —
flag this to backend before shipping a creation flow that assumes it's
enforced server-side.

Store the token in memory (or a short-lived storage mechanism) rather than
`localStorage` if XSS exposure is a concern for this ERP — the codebase makes
no assumption either way.

## 3. Response envelope

Every endpoint returns the same shape:

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success, or a caught business error (`success: false`, human
  readable `message`, no `data`). **Check `success`, not just HTTP status** —
  the current `Create` endpoint (below) is the only one that returns non-200
  on business failure; most read endpoints return `200` even when they
  internally caught an exception.
- `201 Created` — successful `POST` (customer created).
- `400 Bad Request` — validation error (missing/mismatched fields).
- `404 Not Found` — id doesn't resolve to a customer.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message` from the server. Don't render this string directly to end
  users — log it and show a generic failure message.

## 4. Paging shape

Endpoints that return a page of customers return `PageCollectionInfo<CustomerDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
  // totalCount / totalPages are present on the type but not populated by
  // CustomerAppService's current implementation — don't rely on them for
  // paging UI; derive total pages from itemsCount / pageSize instead.
}
```

`pageIndex` is 0-based.

## 5. Endpoints

All routes below are relative to `/api/registry/customer`.

### 5.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `text`, `customerFilter` (int).

- Omitting `text` calls the plain paged listing.
- Supplying `text` runs a filtered search; `customerFilter` selects which
  field `text` matches against (see `CustomerFilter` enum, §7).

Returns `ApiEnvelope<PageCollectionInfo<CustomerDTO>>`.

### 5.2 Get one — `GET /{id}`

`id` is a GUID. Returns `404` if not found. On success:

```json
{ "success": true, "message": "...", "data": { "customer": CustomerDTO, "nextOfKins": NextOfKinDTO[] } }
```

### 5.3 Count — `GET /count`

`data` is a plain `number`.

### 5.4 By type — `GET /by-type/{type}`

`type` is the integer `CustomerType` code (§7). Same optional query params as
§5.1 (`text`, `customerFilter`, `pageIndex`, `pageSize`). Returns a paged
result.

### 5.5 By record status — `GET /by-record-status/{recordStatus}`

`recordStatus` is the integer `RecordStatus` code (§7). Same paging/filter
params as above.

### 5.6 By station — `GET /by-station/{stationId}`

`stationId` is a GUID. Same paging/filter params as above.

### 5.7 Search by identity card — `GET /search/identity-card`

Query: `identityCardNumber` (required), `exactMatch` (bool, default `false`).
`400` if `identityCardNumber` is blank. Returns `ApiEnvelope<CustomerDTO[]>`.

### 5.8 Search by ID number — `GET /search/id-number/{identityCardNumber}`

Returns `ApiEnvelope<CustomerDTO[]>`.

### 5.9 Search by serial number — `GET /search/serial-number/{serialNumber}`

`serialNumber` is an int. Returns `ApiEnvelope<CustomerDTO[]>`.

### 5.10 Search by payroll numbers — `GET /search/payroll-numbers`

Query: `payrollNumbers` (required, comma-delimited per legacy convention),
`matchExact` (bool, default `false`). `400` if blank. Returns
`ApiEnvelope<CustomerDTO[]>`.

### 5.11 Sub-resources of a customer

All take `id` (GUID) as the path segment:

| Route | Returns |
|---|---|
| `GET /{id}/next-of-kin` | `NextOfKinDTO[]` |
| `GET /{id}/account-alerts` | `AccountAlertDTO[]` |
| `GET /{id}/partnership-members` | `PartnershipMemberDTO[]` |
| `GET /{id}/corporation-members` | `CorporationMemberDTO[]` |
| `GET /{id}/referees` | `RefereeDTO[]` |
| `GET /{id}/credit-types` | `CreditTypeDTO[]` |

### 5.12 Create — `POST /`

```ts
interface CreateCustomerRequest {
  customer: CustomerDTO;                       // required
  additionalDebitTypes?: DebitTypeDTO[];
  additionalInvestmentProducts?: InvestmentProductDTO[];
  additionalSavingsProducts?: SavingsProductDTO[];
  partnershipMembers?: PartnershipMemberDTO[];
  corporationMembers?: CorporationMemberDTO[];
  referees?: RefereeDTO[];
  moduleNavigationItemCode?: number;
}
```

`400` if `customer` is missing. `customer.branchId` must resolve to a real
branch — `ICustomerAppService.AddNewCustomerAsync` silently skips all
debit-type/account/product creation (and the welcome SMS) if the branch
lookup fails, so an invalid/empty `branchId` looks like a successful create
with nothing attached.

  The default savings product, all globally mandatory savings/investment
  products, and products attached to the customer's company are resolved from
  authoritative server records and attached **server-side automatically**.
  Registration rejects missing default configuration, unknown IDs, locked
  products, and products without a valid code or G/L account before the
  customer is persisted. `additionalDebitTypes` /
`additionalInvestmentProducts` / `additionalSavingsProducts` are only for
items the user explicitly wants attached *on top of* the mandatory set (e.g.
checkboxes the user opted into beyond what's pre-selected); omit them
  entirely for the common case.

  Product IDs are deduplicated across the default, global mandatory,
  company-attached, and additional collections. A post-save account-provisioning
  failure returns `409` with the created customer in `data` and an explicit
  instruction not to retry registration; operations staff should repair the
  missing customer accounts instead.

The registration request can persist the type-specific related collections in
the same workflow. Partnership registrations use `partnershipMembers` (direct
person details plus `signatory`); corporation registrations use
`corporationMembers` (an existing `customerId`, remarks, and `signatory`);
`referees` use an existing customer as `witnessId`.

`customer.passportBuffer`, `signatureBuffer`,
`identityCardFrontSideBuffer`, and `identityCardBackSideBuffer` accept base64
image bytes. When present, the API writes them to `BLOBStore` under the image
IDs assigned to the new customer. The current registration UI limits each
image to 5 MB.

`moduleNavigationItemCode` gates which module-nav permission check the
service applies; source the correct value from
`GET /api/administration/modules` rather than hardcoding it — no fixed
constant is documented for "Registry/Customer" in this codebase.

Success → `201`:
```json
{ "success": true, "message": "Customer created successfully", "data": CustomerDTO }
```

### 5.12a Registration debit types — `GET /registration/debit-types`

Returns `ApiEnvelope<DebitTypeDTO[]>`. Use this for the optional debit-type
selection tab. Company-mandatory types are still attached server-side and do
not need to be selected by the user.

### 5.13 Update — `PUT /{id}`

Body: full `CustomerDTO`. `id` in the URL must equal `customer.id` in the body
(`400` otherwise, mirroring the `Create` mismatch check). `404` if the
customer doesn't exist. Returns the refreshed `CustomerDTO` on success.

### 5.14 Update next of kin — `PUT /{id}/next-of-kin`

Body: `NextOfKinDTO[]` (replaces the full collection for that customer, not a
partial patch). Returns the refreshed collection.

### 5.15 Update account alerts — `PUT /{id}/account-alerts`

Body: `AccountAlertDTO[]` (full replace, same semantics as above).

### 5.16 Update station — `PUT /{id}/station`

Body: full `CustomerDTO`; only `stationId` actually changes server-side, but
the whole object is required and `id`/`customer.id` must match.

## 6. Typical frontend flows

**Create customer**
1. `POST /api/auth/login` → store token.
2. Collect branch, customer-type-specific fields, mobile number, images, and
   the applicable partnership/corporation member details.
3. `POST /` with the complete registration request — mandatory debit types/products attach
   automatically; only add `additionalDebitTypes` /
   `additionalInvestmentProducts` / `additionalSavingsProducts` if the user
   picked extras beyond the mandatory set (see §5.12).
4. On `201`, use `data.id` to immediately fetch next-of-kin/account-alert
   sub-resources if the onboarding flow collects them in a later step.

**Edit customer**
1. `GET /{id}` to hydrate the form (also gives you current next-of-kin).
2. `PUT /{id}` with the full edited `CustomerDTO`.
3. If next-of-kin changed, `PUT /{id}/next-of-kin` separately — it's not
   included in the customer update call.

**List / search screen**
- Drive the grid off `GET /` with `pageIndex`/`pageSize`; wire the search box
  to `text` + a `customerFilter` dropdown (§7). Use dedicated `by-*` routes
  only for a fixed filter chip (e.g. "by branch station"), not for the
  primary search box — those route params aren't a free-form filter.

## 7. Enum reference

Numeric codes the frontend needs for dropdowns / route params. Source:
`Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs`.

**CustomerType** (`customer.type`, and the `by-type/{type}` route param)
| Code | Label |
|---|---|
| 0 | Individual |
| 1 | Partnership |
| 2 | Corporation |
| 3 | Microcredit |

> The legacy plural `CustomersController` checks `Type == 1` for "Individual"
> and `Type == 3` for "Corporation" — that does **not** match this enum. If
> you ever touch both controllers from the same UI, do not assume they agree
> on type codes; this spec's numbering (0/1/2/3 above) is the one
> `CustomerAppService`/the domain layer actually uses.

**RecordStatus** (`customer.recordStatus`, and `by-record-status/{recordStatus}`)
| Code | Label |
|---|---|
| 0 | New |
| 1 | Edited |
| 2 | Approved |
| 3 | Rejected |

**IndividualType** (`customer.individualType`, only meaningful when `type` is Individual)
| Code | Label |
|---|---|
| 0 | Adult |
| 1 | Minor |

**IdentityCardType** (`customer.individualIdentityCardType`)
| Code | Label |
|---|---|
| 0 | (none) |
| 1 | National ID |
| 2 | Passport |
| 3 | Alien ID |
| 4 | Birth Certificate |

**Gender** (`customer.individualGender`)
| Code | Label |
|---|---|
| 0 | (none) |
| 1 | Male |
| 2 | Female |
| 3 | Non-Binary |

**MaritalStatus** (`customer.individualMaritalStatus`)
| Code | Label |
|---|---|
| 0 | (none) |
| 1 | Married |
| 2 | Single |
| 3 | Divorced |
| 4 | Separated |

**Salutation** (`customer.individualSalutation`) — see `Salutation` enum in
`Enumerations.cs` for the full list (Mr/Mrs/Miss/Dr/Prof/Rev/Eng/Hon/Cllr/…);
not reproduced in full here since it's a long, non-critical-path list.

**CustomerFilter** (the `customerFilter` query param used with `text` search)
| Code | Label |
|---|---|
| 0 | Serial Number |
| 1 | Personal Identification # |
| 2 | First Name |
| 3 | Last Name |

(check `CustomerFilter` in `Enumerations.cs` if more values are added later —
only the first four were confirmed at spec time.)

`CustomerDTO` also carries `typeDescription`, `individualGenderDescription`,
etc. — server-computed human-readable labels for every coded field. Prefer
rendering those directly in read-only views instead of re-implementing the
enum → label mapping in the frontend; reserve the tables above for populating
`<select>` options on write forms.

## 8. `CustomerDTO` — fields the frontend actually needs

`CustomerDTO` has ~80 properties (full list in
`Application.MainBoundedContext.DTO/RegistryModule/CustomerDTO.cs`). The
following are the ones a create/edit/list UI will touch; everything else
(biometric buffers, image ids, computed `*Description` fields, `age`,
`fullName`, etc.) is either server-computed, a legacy/unused field, or only
relevant to a specific back-office screen — read the source file directly
before wiring up a field not listed here.

| Field | Type | Notes |
|---|---|---|
| `id` | guid | server-generated on create |
| `branchId` | guid | required |
| `type` | byte | `CustomerType`, §7 |
| `individualFirstName` / `individualLastName` | string | required when `type` is Individual |
| `individualIdentityCardType` | byte | §7 |
| `individualIdentityCardNumber` | string | required when `type` is Individual |
| `individualGender` / `individualMaritalStatus` / `individualSalutation` | byte | §7 |
| `individualBirthDate` | date? | required by the registration UI for Individuals |
| `nonIndividualDescription` | string | required when `type` is not Individual (entity name) |
| `nonIndividualRegistrationNumber` | string | required when `type` is not Individual |
| `addressMobileLine` | string | used for SMS in the legacy plural controller; not currently sent by this controller |
| `addressEmail`, `addressAddressLine1/2`, `addressCity`, `addressPostalCode` | string | |
| `stationId` | guid? | |
| `reference1` / `reference2` / `reference3` | string | account number / membership number / personal file number |
| `recordStatus` | byte | §7 — server sets this; don't let the client set it directly on create |
| `passportBuffer` / `signatureBuffer` | byte[]? | optional base64 image content persisted to BLOBStore on create |
| `identityCardFrontSideBuffer` / `identityCardBackSideBuffer` | byte[]? | optional base64 image content persisted to BLOBStore on create |
| `nextOfKins` | `NextOfKinDTO[]` | populated on read; use the dedicated next-of-kin endpoints to write |

Field-level required-ness above is **not enforced by this controller** — it's
carried over from what the legacy plural controller validated, and is listed
here only as guidance for client-side form validation.

## 9. Operational notes

1. `CustomerController` requires authentication but has no controller-wide
   role restriction; customer editing has its own permission check.
2. The domain service rejects duplicate individual identity-card numbers and
   duplicate partnership/corporation registration numbers.
3. Account provisioning, mandatory product/debit-type attachment, and welcome
   notifications are performed server-side after the base customer is saved.
