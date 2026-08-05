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
| Customer verification (maker-checker) | `api/administration/workflows` (generic engine, used with a specific permission type) | [`customer-verification-api-spec.md`](customer-verification-api-spec.md) |
| Customer accounts (base resource) | `api/accounts/customer-accounts` | [`customer-accounts-api-spec.md`](customer-accounts-api-spec.md) |
| Customer account management (activate/freeze/close/remark) | `api/accounts/customer-accounts/{id}/...` | [`customer-account-management-api-spec.md`](customer-account-management-api-spec.md) |
| Customer account signatories | `api/accounts/customer-accounts/{id}/signatories` | [`customer-account-signatory-api-spec.md`](customer-account-signatory-api-spec.md) |
| Customer account verification (maker-checker) | `api/administration/workflows` (generic engine, used with a specific permission type) | [`customer-account-verification-api-spec.md`](customer-account-verification-api-spec.md) |
| Customer account statements | `api/accounts/statements/customer-account` | [`customer-account-statement-api-spec.md`](customer-account-statement-api-spec.md) |
| General ledger statements | `api/accounts/statements/gl-account` | [`general-ledger-statement-api-spec.md`](general-ledger-statement-api-spec.md) |
| Standing orders | `api/accounts/standingorders` | [`standing-order-api-spec.md`](standing-order-api-spec.md) |
| Standing order execution (batch triggers) | `api/accounts/standingorders/execution` | [`standing-order-execution-api-spec.md`](standing-order-execution-api-spec.md) |
| Companies | `api/administration/companies` | [`company-api-spec.md`](company-api-spec.md) |
| Branches | `api/administration/branches` | [`branch-api-spec.md`](branch-api-spec.md) |
| Text alerts | `api/messaging/textalert` | [`textalert-api-spec.md`](textalert-api-spec.md) |

## Changelog — what's new and what needs frontend action

Newest first. Each entry says what to build and, where relevant, what to
change in code that already exists.

### Text Alert API — new

`api/messaging/textalert` — list/search, get-by-id, and manually create a
text alert, routed through the existing `ITextAlertAppService` (no new
backend service needed). No update/delete — see
`textalert-api-spec.md` for why, and for the DTO's server-assigned fields
on create.

### Workflow reference numbers — fixed, were always `0`

Every approval request (`Workflow`) created via `CustomerVerification` or
`CustomerAccountVerification` origination was left with `referenceNumber: 0`
(`paddedReferenceNumber: "0000000"`) — nothing populated it, so every
pending item in a checker inbox looked identical on that field, and
searching `GET /items?text=...` by reference number matched everything.
`WorkflowAppService.AddNewWorkflow` now server-generates a real sequential
reference number (`MAX(x)+1`, same convention as every other
auto-numbered field in this API) whenever the caller doesn't supply one.
Existing `Workflow` rows created before this fix still show `0` — this
only affects newly created ones.

### Workflow manual-match recovery endpoint — new

`POST /api/administration/workflows/{workflowId}/match` — for a `Workflow`
that's reached `Approved`/`Rejected` but is still stuck at
`matchedStatus: 0` (the async dispatcher never processed it — not running,
queue message lost, etc.). Runs the same processing the dispatcher would
have, synchronously, bypassing the queue. `404` unknown id, `400` if the
workflow hasn't reached a final status yet, no-op success if already
matched. Applies to any permission type on the generic workflow engine.

**Fixed, only affected rejections**: `WorkflowAppService.UpdateWorkflow`
only enqueued a workflow for the dispatcher when
`workflowDTO.Status == (int)WorkflowRecordStatus.Approved || ... .Rejected`
— but `Status` is actually set using a *different* enum,
`WorkflowApprovalOption`. `Approved` happens to be `2` in both enums, so
approvals enqueued fine by coincidence; `WorkflowRecordStatus.Rejected` was
`3` while `WorkflowApprovalOption.Rejected` is `1`, so **a rejected
workflow never got enqueued at all** and sat at `matchedStatus: 0` forever.
The guard now compares against `WorkflowApprovalOption` correctly. Any
already-rejected workflow from before this fix that's still stuck can be
cleared with the manual-match endpoint above; new rejections enqueue
correctly going forward.

### Workflow checker/queueable endpoints — paging bug fixed

`GET /items`, `/items/mine`, and `/queueable` under
`api/administration/workflows` all defaulted `pageIndex` to `1`, but
`AllMatchingPaged` is 0-based (`Skip(pageSize * pageIndex)`) everywhere
else in this API — same as every other paged endpoint. Practical effect:
call any of these three without an explicit `pageIndex` and, with the
default `pageSize=20`, anything up to 20 matching rows silently came back
as an **empty `pageCollection` with a correct nonzero `itemsCount`** (the
count is computed before the skip/take, so it wasn't wrong — just
misleadingly paired with zero rows). Fixed to default `pageIndex = 0`. If
you were explicitly passing `pageIndex=1` to work around/mimic this,
switch to `pageIndex=0`; if you were relying on the default, no client
change needed.

### Workflow checker inbox — new unified endpoint

`GET /api/administration/workflows/items/mine` — a superadmin/checker inbox
across **every** permission type the caller's role(s) can act on in one
call, with no `systemPermissionType` param. If you were calling the
existing `GET /items` endpoint with `systemPermissionType=0` (or looping it
over every known permission type) to build a general "my approvals" screen,
**switch to `/items/mine`** — same query params (`status`, `text`,
`startDate`, `endDate`, `pageIndex`, `pageSize`), just drop
`systemPermissionType`. `GET /items?systemPermissionType=X` is unchanged
and still the right call for a single-type/tabbed view. See
`customer-verification-api-spec.md` §2 or
`customer-account-verification-api-spec.md` §2 for the full shape (both
apply equally to `/items/mine`).

### Branch API — rebuilt on the domain layer, breaking changes

`BranchController` (was `BranchesController`) has been rebuilt from scratch
against `IBranchAppService` — the old controller routed through a raw-SQL
class (`WebApplication1/Services/BranchService.cs`, now **deleted**) that
bypassed validation, audit trails, and auth entirely. If you integrated
against the old one: **auth is now required** (it was previously
`[AllowAnonymous]` with wildcard CORS), **`DELETE /{id}` is gone** — use the
new `PATCH /{id}/toggle-lock` instead, matching the lock/unlock convention
every other aggregate here uses, and **`POST`/`PUT` now validate** and
reject with `400` instead of silently accepting bad data. Full reference:
`branch-api-spec.md`.

### Company API — new

`CompanyController` documented for the first time (the controller itself
isn't new, the doc was just missing). List/search/create/update a company,
plus its two sub-resources: mandatory debit types and mandatory attached
(savings/investment) products. Note the old MVC admin screen silently
forced every new company's `recoveryPriority` to `"DirectDebits"` — that
hack was **not** carried forward into this API; set it explicitly in your
create payload if you need it. Full reference: `company-api-spec.md`.

### Customer verification (maker-checker) — new

Sibling to customer account verification below, but for the *customer*
record itself (`Customer.recordStatus`, independent of any of their
accounts). Controlled by a new, separate per-company flag,
`Company.enforceCustomerMakerChecker` (set via the Company API above) —
off by default, same "nothing to build" story as customer account
verification when off. When on, build a checker-inbox screen against the
same generic workflow API filtered to `systemPermissionType=44858`. Full
reference: `customer-verification-api-spec.md`.

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
