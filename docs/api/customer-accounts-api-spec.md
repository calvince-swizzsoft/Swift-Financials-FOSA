# Customer Accounts API — Client Integration Spec

Audience: any screen that lists, looks up, or creates customer accounts
(savings/loan/investment accounts held against a customer) — the base
resource that the management, signatory, statement, and verification APIs
below all build on top of.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/CustomerAccountsController.cs`
- Domain service: `Application.MainBoundedContext/AccountsModule/Services/ICustomerAccountAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO.AccountsModule/CustomerAccountDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

Related, built on this same resource:
- `docs/api/customer-account-management-api-spec.md` — activate/freeze/close/remark
- `docs/api/customer-account-signatory-api-spec.md` — signatories
- `docs/api/customer-account-statement-api-spec.md` — statements/mini-statement/PDF
- `docs/api/customer-account-verification-api-spec.md` — maker-checker approval

## History note (why this doc exists now)

This controller used to call a parallel raw-SQL class
(`WebApplication1/Services/CustomerAccountService.cs`, plus a
`WebApplication.Services.CustomerService`-adjacent `DataRecordExtensions`
helper) instead of the proper `ICustomerAccountAppService` domain layer for
several of its actions. That raw path had real bugs — a column-ordinal
lookup that called the wrong helper method (silently corrupting every
string field), and a balance-fetch call that threw `InvalidCastException`
on brand-new accounts with no transaction history yet. Both classes have
been **deleted**. Every action on this controller now goes through
`ICustomerAccountAppService` (plus `ICustomerAppService`/`IBranchAppService`/
`ICompanyAppService` for the one bulk-create action that needs to resolve a
company's attached products). If you were working around either of those
bugs client-side, you can remove that workaround now.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/customer-accounts` |
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

- `200 OK` — success, or a caught business error (`success: false`).
- `201 Created` — successful `POST` (account(s) created).
- `400 Bad Request` — missing required field.
- `404 Not Found` — id doesn't resolve (account, customer, or branch,
  depending on the endpoint).
- `409 Conflict` — `POST /` only: the customer already has an account for
  that product (see §4.4).
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

## 3. Paging shape

`GET /`, `GET /customer/{customerId}` return `PageCollectionInfo<CustomerAccountDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

## 4. Endpoints

All routes below are relative to `/api/accounts/customer-accounts`.

### 4.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `text`, `customerFilter` (int — see `CustomerFilter` enum,
`docs/api/customer-api-spec.md` §7). Omitting `text` returns the plain
paged listing; supplying it runs a filtered search.

### 4.2 List all (unpaged) — `GET /all`

Every customer account in the system, no filter, no paging. Returns
`ApiEnvelope<CustomerAccountDTO[]>`. Avoid for UI listings on a live system
— prefer §4.1.

### 4.3 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<CustomerAccountDTO>`.

Deliberately a lightweight lookup with **no balance fields populated**
(`availableBalance`/`bookBalance`/etc. will be `0`/default) — it's a direct
entity projection, not the balance-fetching variant, specifically to avoid
the raw stored-proc calls that assume the account already has transaction
history. If you need balances, use the statement endpoints
(`docs/api/customer-account-statement-api-spec.md`) which compute them
per-product correctly.

### 4.4 Create — `POST /`

Body: `CustomerAccountDTO` with `customerId`, `branchId`, and
`customerAccountTypeTargetProductId` all required (`400` if any is
missing/empty). Creates a single account for one specific product.

A duplicate account for the same customer + product is **not** a thrown
error — the service returns the DTO with `errorMessageResult` set instead,
which the controller surfaces as `409`:
```json
{ "success": false, "message": "Sorry, but customer already has an account for the selected product please choose another product!  <product description>" }
```

Success → `201`:
```json
{ "success": true, "message": "Customer account created successfully", "data": CustomerAccountDTO }
```

### 4.5 Bulk-create for a customer — `POST /customer/{customerId}/branch/{branchId}`

No body. Creates one account per product the `branchId`'s company has
configured as "attached" (`ICompanyAppService.FindCachedAttachedProducts`)
that the customer doesn't already have an account for — covers
savings, investment, and loan products in one call. `404` if `customerId`
or `branchId` don't resolve.

`data` is always the customer's **current full account list** (re-fetched
after the operation), not just the accounts created by this call — there's
no way to distinguish "just created" from "already existed" in the
response. If you need that distinction, diff against a `GET /customer/{customerId}`
call made before this one.

`201` if at least one account was created; `200` with
`success: true` and an explanatory message if nothing new was created
(customer already has everything, or the company has no attached products
configured) — check the `message`, not just the status code, if that
distinction matters to your UI.

### 4.6 Get a customer's accounts (unpaged) — `GET /{id}/accounts`

`id` is the **customerId** (despite the path segment name matching the
account routes above — this is a sub-resource of *customer*, not of
*account*). Returns `ApiEnvelope<CustomerAccountDTO[]>`. Each account includes
its current balance fields. For savings accounts, both `bookBalance` and
`availableBalance` are populated; loan and investment balance fields follow
the product-specific rules applied by `ICustomerAccountAppService`.

### 4.7 Get a customer's accounts (paged) — `GET /customer/{customerId}`

Same balance-enriched data as §4.6, paged. Query: `pageIndex` (default `0`), `pageSize`
(default `20`). Prefer this one for UI listings; §4.6 is unpaged and
exists mainly for internal/bulk use.

### 4.8 Get by account number — `GET /account-number/{accountNumber}`

`accountNumber` is the full formatted account number (branch-serial-product-target
format, e.g. `001-0000123-004-012`). `404` if not found. Returns
`ApiEnvelope<CustomerAccountDTO>`.
