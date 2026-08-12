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
| Cheque types (master data) | `api/accounts/chequetypes` | [`cheque-type-api-spec.md`](cheque-type-api-spec.md) |
| Cheque books (issuance + payment vouchers) | `api/accounts/chequebooks` | [`chequebook-api-spec.md`](chequebook-api-spec.md) |
| Commissions (+ graduated scales/splits/levies) | `api/accounts/commissions` | [`commission-api-spec.md`](commission-api-spec.md) |
| Levies (+ splits) | `api/accounts/levies` | [`levy-api-spec.md`](levy-api-spec.md) |
| UnPay reasons (+ attached commissions) | `api/accounts/unpayreasons` | [`unpayreason-api-spec.md`](unpayreason-api-spec.md) |
| Text alerts | `api/messaging/textalert` | [`textalert-api-spec.md`](textalert-api-spec.md) |
| Front office (teller transactions, treasury, cheques, EOD, account closure, fixed deposits, expense payables, sundry payments, in-house cheques, automated clearing, fiscal counts) | `api/frontoffice/*` | [`frontoffice-api-spec.md`](frontoffice-api-spec.md) |

## Proposed / in design — not yet implemented

Not part of the table above because no controller exists yet — these are
target designs to build against, not live API surface.

| Area | Proposed base path | Doc |
|---|---|---|
| WhatsApp banking (self-service: onboard, accounts, deposit, withdraw) | `api/whatsappbanking` | [`whatsapp-banking-api-spec.md`](whatsapp-banking-api-spec.md) |

## Changelog — what's new and what needs frontend action

Newest first. Each entry says what to build and, where relevant, what to
change in code that already exists.

### WhatsApp Banking — proposed design, revised after finding existing Alternate Channels infrastructure

New self-service channel: a customer registers, gets accounts opened, links
their number, and deposits/withdraws by chatting with the SACCO's WhatsApp
number, no branch visit. Unlike everything else in this index, there's no
reference MVC controller to adapt — this is a **design spec**, not a build
record. Full functional design:
`WebApplication1/Areas/WhatsAppBanking/WORKFLOW.md`. API contract:
`whatsapp-banking-api-spec.md`.

**Revised from its first draft**: before shipping, an existing **Alternate
Channels** module was found already living in this codebase
(`AlternateChannel` linking, `AlternateChannelKnownChargeType` fees,
`MobileToBankRequest`/`BankToMobileRequest` mobile-money C2B/B2C plumbing)
— the same problem this design first solved from scratch, already partially
built for other channels (Sacco Link, Sparrow, M-Co-op Cash, ...). WhatsApp
Banking is now designed as a **new `AlternateChannelType`** reusing that
linking/fee/logging framework instead of duplicating it — a customer PIN
(set at linking) authenticates ongoing sessions, not OTP-per-session as
the first draft had it. Full comparison: `WORKFLOW.md` §3.

**What that reuse does and doesn't solve**: linking, per-channel fees, and
deposit-matching logic are real and get reused directly. What's still
missing, confirmed by reading the code rather than assumed: no inbound REST
webhook exists anywhere in this codebase for a mobile-money provider to
actually post a C2B deposit confirmation, and the project meant to make the
outbound B2C withdrawal-payout call
(`SwiftFinancials.BankToMobileHostInterface`) is an empty, unimplemented
stub. Both are real, scoped backend work the spec calls out explicitly
per-endpoint rather than glossing over. Loan application via this channel
remains explicitly out of scope for this phase (`WORKFLOW.md` §7).

### UnPay Reason API — new

`UnPayReasonController` (`api/accounts/unpayreasons`) documented for the
first time — `IUnPayReasonAppService` was already fully built but had no
controller anywhere, only reachable through the legacy
`UnPayReasonService.svc.cs` WCF passthrough (same "missing controller"
shape as the earlier Cheque Book API entry below). This is the master data
`ChequesController`'s `POST /api/frontoffice/cheques/clear` `"unpay"` flow
needs a valid `UnPayReasonDTO` from — if your UI builds that flow, you now
have somewhere to source/manage the picker list from.

Two reference-app things fixed rather than ported: `Edit` never called
`ValidateAll()` (so `Description` was never actually required on edit —
now fixed), and the attached-commissions flow no longer does a
resolve-by-id round trip per commission (send `commissionIds: Guid[]`
directly). Full reference: `unpayreason-api-spec.md`. The API areas table
above was also missing rows for the existing Commission/Levy APIs
(`commission-api-spec.md`/`levy-api-spec.md`) — added alongside this entry.

### Four more front-office bugs fixed — Treasury, Transfers, EOD, withdrawal settlement

Continuing the front-office audit (14 functional areas total — 8 previously
confirmed faithful or fixed, see prior entries below), the remaining 6 areas
were checked and 4 real bugs found and fixed. Teller master data and
standalone fiscal count CRUD were confirmed faithful, no changes.

- **`CashManagementController` (§5) silently no-opped on out-of-scope
  transaction types.** `TreasuryTransactionType` has 6 members;
  `Create`'s switch only handled 4, with no `default` case. Sending
  `TellerToTreasury`(=16) or `TellerCashTransfer`(=32) — real enum values,
  just owned by End of Day close and cash transfer requests respectively,
  not this endpoint — fell through untouched and returned
  `success: true, "Operation Success..."` with **nothing posted**. Fixed:
  now rejects anything outside the 4 supported types with a clear
  `success: false` message instead.
- **`TransfersController`'s `/cash` and `/cash/acknowledge` (§7) never
  validated.** Both gated on `cashTransferRequestDTO.HasErrors` but never
  called `ValidateAll()` first, so the gate always passed regardless of
  input. Fixed — `Amount` must now be greater than zero, as originally
  intended.
- **`EndOfDayController` (§9) trusted a client-supplied value for its
  cheque-transfer precondition.** `UntransferredChequesValue` from the
  request body was copied straight into the check instead of being
  independently verified — a teller could send `0` and bypass "transfer
  your cheques first" regardless of reality. Fixed: now queries
  `IExternalChequeAppService.FindUnTransferredExternalChequesByTellerId`
  server-side.
- **Withdrawal settlement could mark an unrelated request `Paid` (§4).**
  When resubmitting `POST /api/frontoffice/requests` for an already-`Authorized`
  withdrawal, the deposit path correctly scoped to
  `CustomerTransactionModel.CashDepositRequestId`; the withdrawal path had
  no equivalent filter and just acted on the first `Authorized` request it
  found for the customer. A customer with two pending withdrawal requests
  could have the wrong one silently settled. Fixed to scope by
  `CashWithdrawalRequestId`, matching deposits.

Full findings and reasoning for both audit passes (all 14 areas):
this session's chat history — not yet consolidated into a standalone doc
the way the cheque subsystem was (`CHEQUE-PROCESSING-ANALYSIS.md`).

### Account closure payout — critical gap fixed, `/settle` clarified

Audit of the remaining front-office nav items found that **nothing in this
API could actually pay a customer out on account closure** —
`POST /api/frontoffice/accountclosures/{id}/settle` only ever flipped the
request to `Settled`, and `SundryPaymentsController`'s switch had no case
for `GeneralTransactionType.CashPaymentAccountClosure` (`= 32`) — the
reference app's actual payout mechanism — so a client attempting it got
`400 "Unsupported transaction type"`. A closure request could be walked
all the way through Create → Approve → Verify → Settle with `success: true`
at every step and the customer's remaining balance never left the SACCO.

Fixed: `SundryPaymentsController` now handles `transactionType: 32`
(mirrors the existing `CashPickup` case's debit/credit direction — debit
the resolved chart of account, credit the teller). This restores, rather
than invents, the reference app's design: `/settle` was **always** just a
status transition there too — payout was always a separate, manually
performed sundry-payment transaction, not something `/settle` did
automatically. `frontoffice-api-spec.md` §10's claim that `/settle` "pays
out remaining balance" was wrong and has been corrected; §10 and §13 now
document the two-call sequence (`GET .../accountclosures/{id}` to resolve
`chartOfAccountId`/`totalValue`, then `POST .../sundrypayments` with
`transactionType: 32`) needed to actually complete a payout. **If your UI
already calls `/settle` and stops, it needs the follow-up sundry-payment
call added** — nothing was paying customers out before this fix regardless
of what the UI did.

Same audit pass also confirmed Customer Receipts, Fixed Deposits, Expense
Payables, and Automated Clearing are all faithfully implemented — no
changes needed there.

### Cheque clearance sequencing + a critical customer double-credit fix — breaking behavior change

Two more cheque-subsystem bugs found and fixed after the Cheque Book API
pass below, both in `frontoffice-api-spec.md` §4/§8. Full trace and
reasoning: `WebApplication1/Areas/FrontOffice/CHEQUE-PROCESSING-ANALYSIS.md`
Findings #9–#10.

- **`ChequeDeposit` no longer credits the customer's spendable balance
  immediately.** This is the important one for any UI showing balances
  right after a deposit. Previously, depositing a cheque credited the
  customer's real product GL exactly like a cash deposit — then `Pay`
  clearance credited them **a second time** for the same cheque days later,
  when it actually cleared. Fixed: `ChequeDeposit` now posts to
  `ExternalChequesControl` (a suspense account, still linked to the
  customer for statement purposes) instead. **If your UI showed a cheque
  deposit's amount as available funds right after `POST /api/frontoffice/requests`
  the way it does for a cash deposit, that's no longer correct** — show it
  as pending until the cheque is transferred, banked, and Pay-cleared.
  `POST /` can now also fail with "Sorry, but the external cheques control
  account has not been setup!" if that account isn't mapped — an
  admin/setup issue, not a per-request one.
- **Clearing (`POST /api/frontoffice/cheques/clear`) now requires a cheque
  to be transferred and banked first**, matching what `unpay` already
  required — previously `clear`(`Pay`) had no such check and could clear a
  cheque straight out of deposit. The candidate list this endpoint offers
  is not filtered on `IsBanked` server-side, so check `IsBanked` on each
  `ExternalChequeDTO` yourself before offering the Clear action, or handle
  the new failure message.

### Cheque Book API — new, plus two cheque-subsystem validation bugs fixed

`ChequeBookController` (`api/accounts/chequebooks`) documented for the first
time — `IChequeBookAppService` was already fully built (issuance, per-leaf
payment vouchers, activate/lock, pay/flag) but had no controller anywhere,
only reachable through the legacy `ChequeBookService.svc.cs` WCF passthrough.
Full reference: `chequebook-api-spec.md`; the `cheque-type-api-spec.md`
table row above was also missing from this index and has been added.

Two real bugs turned up in the cheque subsystem while building this and were
fixed — full trace and GL-wiring detail in
`WebApplication1/Areas/FrontOffice/CHEQUE-PROCESSING-ANALYSIS.md`:
- **`ExternalChequeDTO.ChequeTypeId`** (`api/frontoffice/cheques` deposit
  flow) is optional by design (a cheque with no type matures the same day
  it's deposited), but its `[ValidGuid]` attribute rejected `null` as well
  as `Guid.Empty` — so depositing a cheque **without** selecting a cheque
  type always failed validation, the opposite of the intended behavior. Root
  cause fixed in the shared `ValidGuidAttribute` (now treats `null` as
  valid), which also silently repairs the same bug on every other optional
  `[ValidGuid]` field across the API, not just this one.
- **`InHouseChequeDTO.debitChartOfAccountId`** (`api/frontoffice/inhousecheques`)
  is used for live GL posting but had zero server-side validation — its
  `[ValidGuid]` attribute was commented out in source, and
  `InHouseController.Create` never called `ValidateAll()` at all. Both
  fixed: the attribute restored, and `Create` now validates each cheque in
  the batch before submitting.

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
