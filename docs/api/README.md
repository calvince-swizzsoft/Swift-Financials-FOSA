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
| Treasury master data | `api/accounts/treasurys` | [`treasury-api-spec.md`](treasury-api-spec.md) |
| Chart of accounts (+ system G/L account mapping) | `api/accounts/chartofaccounts` | [`chartofaccount-api-spec.md`](chartofaccount-api-spec.md) |
| Cost centers | `api/accounts/costcenters` | [`costcenter-api-spec.md`](costcenter-api-spec.md) |
| Companies | `api/administration/companies` | [`company-api-spec.md`](company-api-spec.md) |
| Branches | `api/administration/branches` | [`branch-api-spec.md`](branch-api-spec.md) |
| Banks (+ bank branches) | `api/administration/banks` | [`bank-api-spec.md`](bank-api-spec.md) |
| Bank linkages (branch ↔ external bank ↔ G/L account) | `api/accounts/banklinkages` | [`bank-linkage-api-spec.md`](bank-linkage-api-spec.md) |
| Text alerts | `api/messaging/textalert` | [`textalert-api-spec.md`](textalert-api-spec.md) |
| Front office (teller transactions, treasury, cheques, EOD, account closure, fixed deposits, expense payables, sundry payments, in-house cheques, automated clearing, fiscal counts) | `api/frontoffice/*` | [`frontoffice-api-spec.md`](frontoffice-api-spec.md) |

## Changelog — what's new and what needs frontend action

Newest first. Each entry says what to build and, where relevant, what to
change in code that already exists.

### Bank + Bank Linkage APIs — new, plus a DTO split and a dead-dependency fix

`BankController` (`api/administration/banks`) and `BankLinkageController`
(`api/accounts/banklinkages`) documented for the first time. A "bank" here
is an external institution a customer's bank account/cheque is held at —
not the same thing as `branch-api-spec.md` (this SACCO's own operating
branches). A "bank linkage" maps one of this SACCO's own branches to an
external bank account + G/L account, used by front-office cash movement
between a teller/treasury and an external bank.

Three things to know if you touch either area:
- **`BankDTO` and `BankLinkageDTO` used to be one overloaded type.**
  `BankDTO` carried a pasted-in copy of every linkage field
  (`bankName`, `branchId`, `chartOfAccountId`, ...), which meant its
  `[Required]` attributes didn't match what a real "create a bank" payload
  looks like. They're now separate DTOs — send bank fields to
  `api/administration/banks`, linkage fields to
  `api/accounts/banklinkages`. If you previously worked around the mixed
  DTO client-side, you can drop that workaround.
- **`CashManagementController`'s bank-linkage lookups were previously
  guaranteed to `500`** (a `NullReferenceException` from an unassigned
  `IBankLinkageAppService` field) on the `BankToTreasury`/`TreasuryToBank`
  cash-movement paths in `POST api/frontoffice/cashmanagement/...`. Fixed;
  no client-side change needed, but if you had a workaround for those
  calls always failing, it's no longer necessary.
- The reference MVC controllers' raw-SQL `DeleteBank` (which actually
  deleted a *branch* row despite its name, bypassing the domain layer) and
  session-based branch-staging (`Session["bankBranches"]`,
  `Session["chartOfAccountId"]`, ...) were **not** carried forward for
  either controller — branches/linkage fields are now just part of the
  create/update request body, and neither controller has a delete endpoint
  (neither `IBankAppService` nor `IBankLinkageAppService` expose one).

Full reference: `bank-api-spec.md`, `bank-linkage-api-spec.md`.

### Chart of Accounts + Cost Centers — new

Two new controllers under `api/accounts`. Both follow the same envelope,
paging, and business-rule-reporting conventions established for Treasury
(§ above): duplicate-key failures on create return `409` with `data: null`
rather than a false `success: true`, and `PUT` returns the freshly
re-fetched entity rather than a bare boolean.

- **`api/accounts/chartofaccounts`** — the reference app split this across
  three screens (`ChartOfAccountController` plus two near-duplicates,
  `GLAccountController`/`SystemGeneralLedgerAccountMappingController`, that
  both just wrapped the same system→G/L-account mapping calls). This API
  folds the mapping concept into one controller as a sub-resource
  (`GET`/`PUT /systemgeneralledgermappings/...`), matching how the
  app-service layer already groups them. Also exposes `GET /tree` — a
  separate, correctly depth-populated hierarchical read model, since the
  flat CRUD endpoints never maintain `Depth`/`Children`. Full reference:
  `chartofaccount-api-spec.md`.
- **`api/accounts/costcenters`** — small CRUD, a FK dependency of chart of
  accounts (`ChartOfAccountDTO.CostCenterId`). Full reference:
  `costcenter-api-spec.md`.

Deliberately **not** covered by either: the reference app's
`AddGeneralLedgerController`/`JournalVoucherController` and the
`BatchOrigination_*`/`BatchAuthorization_*`/`BatchVerification_*` family —
multi-line GL/journal-voucher batches with their own maker-checker
lifecycle, a separate and substantially larger feature, not part of chart
of account master data. Flagged as a future pass, not started.

### Treasury master data — moved out of Front Office, breaking route change

`TreasurysController` moved from `Areas/FrontOffice/Controllers` to
`Areas/Accounts/Controllers` — it's pure admin CRUD for the `Treasury`
vault record itself (no teller/cash-cycle behavior), so it belongs with the
other Accounts-area master data, not front office. **Route changed:
`api/frontoffice/treasurys` → `api/accounts/treasurys`.** If you already
integrated against the old path, update it. Two response-shape fixes came
out of writing the full spec for this move, so check these even if you
already wired up the old routes:
- `POST /` now returns `409` (not a false `200 success:true`) when the
  branch already has a treasury or the description isn't unique — it used
  to always report success even when creation silently failed.
- `PUT /{id}` now returns the updated `TreasuryDTO` in `data` — it used to
  return a bare `true`/`false`.

Full reference, including the field table and business rules a create/edit
screen needs: `treasury-api-spec.md`. `frontoffice-api-spec.md` §5 (Treasury
*cash movement*, `CashManagementController`) is unaffected and stays put;
only the master-data CRUD moved. New doc: `treasury-api-spec.md`.

### Front Office API — new, plus breaking fixes to what already existed

All 15 front-office functional areas (teller transactions, treasury, cheque
banking/clearance, end of day, account closure, fixed deposits, expense
payables, sundry payments/customer receipts, in-house cheques, automated
clearing, fiscal counts) now have a documented `ApiController`. Full
reference: `frontoffice-api-spec.md`; functional/process design:
`WebApplication1/Areas/FrontOffice/WORKFLOW.md`.

If you already integrated against the 7 controllers that existed before
this pass (`api/frontoffice/{requests,cashmanagement,cheques,transfers,
tellers,treasurys,endofday}`), several things changed under you:

- **Auth is now required** on all of them (was `[AllowAnonymous]` with
  wildcard CORS on 6 of the 7 — local-testing scaffolding that shipped by
  mistake). Send a bearer JWT or every call now `401`s.
- **`POST /api/frontoffice/requests/authorize` is gone.** It bypassed the
  generic maker-checker engine. Approve/reject a pending cash deposit or
  withdrawal request through `POST /api/administration/workflows/items/approve`
  instead — see `frontoffice-api-spec.md` §18.
- **`GET /api/frontoffice/requests` and `GET /api/frontoffice/cheques` are
  now paged.** Both used to return the full unpaged table in `data` as a
  bare array; they now return `PageCollectionInfo<T>` under `data`, and
  `requests` defaults to the `Pending` queue unless you pass `status`
  explicitly.
- **`CashDepositController.Create`'s dialog response is now nested under
  `data`.** Fields like `cashTransactionRequestId`/`transactionCategory`
  used to sit at the top level of the JSON response alongside `success`;
  they're now under `data`, matching every other endpoint's envelope.
- **Receipts**: there is no server-side print endpoint anymore (the old
  one drove `System.Drawing.Printing` against a hardcoded local printer
  name, which only worked if the API process and the printer were on the
  same machine — never true for a browser client). Deposit/withdrawal
  posting and End of Day close now return the full journal in `data`;
  render/print the receipt client-side from that.
- If you called `TransfersController`/`EndOfDayController` and relied on
  "current teller" resolving to a specific fixed identity: it no longer
  does — both now resolve the teller from the caller's own JWT, same as
  every other endpoint in this area.

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
