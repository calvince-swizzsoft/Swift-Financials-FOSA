# SwiftFinancialz Web API — Frontend Reference Index

Single entry point for everything the frontend needs to integrate against
`WebApplication1`. Each linked doc is the source of truth for its area —
this page is a map plus a changelog of what's new/changed, so you know
what to go update.

## Conventions shared by every endpoint below

- **Base host**: `https://<host>` + the base path listed per area.
- **Auth**: Bearer JWT on every request except `POST /api/auth/login`. Full
  login flow and token handling: `customer-api-spec.md` §2.
- **Response envelope**: `{ success: boolean, message: string, data: T | null }`
  on every JSON endpoint unless a doc says otherwise (the two exceptions
  are binary downloads — statement PDF, and any future export endpoint —
  which return the raw file with its own content type).
- **Paging shape**: `{ pageIndex, pageSize, pageCollection: T[], itemsCount }`
  (`pageIndex` is 0-based) wherever a doc says an endpoint returns
  `PageCollectionInfo<T>`.
- **Status codes**: `400` validation, `404` not found, `409` conflict
  (duplicate / business-rule block), `500` unhandled exception with the raw
  `ex.Message` — call out per-endpoint deviations are noted in each doc.

## API areas

| Area | Base path | Doc |
|---|---|---|
| Customers | `api/registry/customer` | [`customer-api-spec.md`](customer-api-spec.md) |
| Customer accounts (base resource) | `api/accounts/customer-accounts` | [`customer-accounts-api-spec.md`](customer-accounts-api-spec.md) |
| Customer account management (activate/freeze/close/remark) | `api/accounts/customer-accounts/{id}/...` | [`customer-account-management-api-spec.md`](customer-account-management-api-spec.md) |
| Customer account signatories | `api/accounts/customer-accounts/{id}/signatories` | [`customer-account-signatory-api-spec.md`](customer-account-signatory-api-spec.md) |
| Customer account verification (maker-checker) | `api/administration/workflows` (generic engine, used with a specific permission type) | [`customer-account-verification-api-spec.md`](customer-account-verification-api-spec.md) |
| Customer account statements | `api/accounts/statements/customer-account` | [`customer-account-statement-api-spec.md`](customer-account-statement-api-spec.md) |
| General ledger statements | `api/accounts/statements/gl-account` | [`general-ledger-statement-api-spec.md`](general-ledger-statement-api-spec.md) |
| Standing orders | `api/accounts/standingorders` | [`standing-order-api-spec.md`](standing-order-api-spec.md) |
| Standing order execution (batch triggers) | `api/accounts/standingorders/execution` | [`standing-order-execution-api-spec.md`](standing-order-execution-api-spec.md) |

## Changelog — what's new and what needs frontend action

Newest first. Each entry says what to build and, where relevant, what to
change in code that already exists.

### Customer accounts — bug fixes + one response shape change

- **Fixed**: `GET /{id}` and account creation no longer route through a
  buggy raw-SQL path. If you had client-side workarounds for garbled
  string fields (names/remarks/descriptions coming back wrong) or
  unexplained `500`s right after creating a new account, **remove them** —
  both root causes are gone.
- **Changed**: `POST /customer/{customerId}/branch/{branchId}` (bulk-create)
  now returns the customer's **full current account list** in `data`, not
  just the accounts created by that specific call. If your UI needs to
  know which ones are new, diff against a `GET /customer/{customerId}`
  taken before the call.
- Full reference: `customer-accounts-api-spec.md`.

### Customer account verification (maker-checker) — new

New account approval flow for savings accounts. Whether it applies at all
depends on a per-company setting:
- Company has maker-checker **off** (typical default): nothing to build —
  new accounts are usable immediately, same as before.
- Company has it **on**: new savings accounts start `recordStatus: 0` (New)
  and block cash deposits until approved. Build a checker-inbox screen
  against the *existing, generic* workflow API (`api/administration/workflows`)
  filtered to `systemPermissionType=44857` — see the doc for the exact
  request/response shapes and the async-approval caveat (approval doesn't
  take effect until a separate backend service processes it — poll
  `GET /api/accounts/customer-accounts/{id}` and check `recordStatus`).

### Customer account management — new

Five action buttons plus a history/audit view for an account: activate,
freeze, close, remark, signing-instructions. All under
`api/accounts/customer-accounts/{id}/...`. Note `/activate` is an
*unfreeze*, not a first-time activation — calling it on an account that's
never been frozen returns `409`, by design (see the doc for the exact
error and why).

### Customer account signatories — new

List/add/bulk-remove authorized signatories on an account. No update or
single-remove operation exists (domain limitation) — to edit a signatory,
remove and re-add.

### Customer account statements — new

Mini-statement, full date-range statement, and a printable PDF for one
account. The PDF endpoint returns raw `application/pdf` bytes, not the
JSON envelope — check `Content-Type` before parsing. `chargeForPrinting=true`
posts a real fee to the account; don't default it on.

### General ledger statements — new

Back-office ledger view for a chart-of-accounts (G/L) account, plus an
unscoped "all transactions in a date range" audit browse. Not
customer-facing.

### Standing orders — new

Full CRUD/search over standing orders (recurring transfers between
accounts) at `api/accounts/standingorders`.

### Standing order execution — new

Admin-only manual triggers (`execute`, `fix-skipped`, `sweep`, `payout`)
for the batch runs that otherwise only fire on a cron. Every response is
`{ success, message, data: boolean }` — `data: false` means "ran fine,
nothing matched," not an error.
