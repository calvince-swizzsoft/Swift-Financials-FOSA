# SwiftFinancialz — Project Notes for Claude

## What this repo is

A large, long-running .NET Framework financial services (SACCO/microfinance) system
built on a fairly classic layered/DDD architecture:

```
Domain.MainBoundedContext        aggregates, factories, specifications (per module: RegistryModule, AccountsModule, ...)
Application.MainBoundedContext   app services (business logic), one interface+impl per aggregate/feature
Application.MainBoundedContext.DTO  DTOs + BindingModels (validation) shared across all front ends
Infrastructure.Data.MainBoundedContext  EF mapping / repositories
DistributedServices.MainBoundedContext  legacy WCF (.svc) layer — being phased out, see below
SwiftFinancials.Web (old repo, see "Reference codebase" below)  legacy ASP.NET MVC front end
WebApplication1                  the NEW project — ASP.NET Web API, this is where we're doing active work
```

Old-style (non-SDK) `.csproj` files throughout — every new `.cs` file needs an
explicit `<Compile Include="...">` entry added manually, the compiler will not
pick it up otherwise.

No top-level `.sln`; there's `SwiftFinancialz.slnx`. For one-off verification
builds, invoke MSBuild directly against a single `.csproj` via the PowerShell
tool (Bash mangles `/p:` switches into path expansions):
```
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" "<Project>\<Project>.csproj" /p:Configuration=Debug /nologo /v:minimal /t:Build
```

## The two goals driving current work

1. **Expose the solution as a proper Web API.** `WebApplication1` is a new
   project standing up `System.Web.Http` `ApiController`s, organized into the
   same `Areas/<Module>/Controllers` layout the old MVC app used (e.g.
   `Areas/Registry/Controllers`, `Areas/Admin/Controllers`). Most of this work
   is **adapting an existing MVC controller** from the reference codebase
   below into an API controller here — same underlying operations, but
   `ApiController` + `[RoutePrefix]`/`[Route]` attribute routing instead of
   views, and a uniform JSON response envelope (see below) instead of
   `ActionResult`/`RedirectToAction`.

2. **Retire the WCF wiring.** `DistributedServices.MainBoundedContext`
   (`.svc` files, `IChannelService`-style contracts) is legacy plumbing we're
   trying to eliminate. Progress on both goals is incremental and
   intentionally not synchronized: some old MVC controllers, WCF contracts,
   and the monolithic `IChannelService` facade are still present and still
   referenced elsewhere in the solution, kept around on purpose so the rest of
   the system keeps working while the new API surface is built out and
   verified. Don't delete old code just because a new controller supersedes
   its one call site — that's a separate, deliberate cleanup pass once
   there's confidence nothing else depends on it.

## Reference codebase (read-only, do not edit)

`C:\Users\ckoda\Desktop\source\SwiftFinancialsNew\SwiftFinancialsSolution\SwiftFinancials.Web\Areas\<Area>\Controllers`

This is the old MVC app in a sibling checkout. When asked to build a new API
controller for module X, the MVC controller of the same name here is the
adaptation source — read it first to see what operations/routes it exposes.

Important: **don't port it literally.** The old controllers route everything
through a monolithic `IChannelService`/`_channelService` facade that doesn't
exist in the new architecture (superseded by focused per-aggregate app
services in `Application.MainBoundedContext/<Module>/Services`). Also expect
dead/commented-out code in the reference controllers (e.g. `EmployerController.Create`
accepted a `divisions` parameter but the code that used it was commented out,
silently discarding it) — check whether behavior actually executes before
reproducing it; fix obvious bugs rather than porting them forward.

## Adapting a controller — the actual workflow

1. Read the reference MVC controller in the old repo to see what operations
   it needs (CRUD, sub-resource lookups, cascading deletes, etc).
2. Check whether the operations already exist on a per-aggregate app service
   in `Application.MainBoundedContext/<Module>/Services` (e.g.
   `IZoneAppService`, `IEmployerAppService`, `ICustomerAppService`). Sibling
   aggregates often already own related lifecycle logic — e.g. `Station`
   create/update/remove and `Division` cascading removal both already lived
   inside `IZoneAppService` before a dedicated `StationController` existed, so
   we extended that interface rather than inventing a new `StationAppService`.
3. If the operation genuinely doesn't exist anywhere, add it to the
   appropriate app service (or create a new one, e.g. `IDivisionAppService`)
   following the exact shape of a sibling method in the same file —
   `ProjectedAs<XBindingModel>().ValidateAll()` for DTOs that aren't
   themselves a `BindingModelBase<T>`, `IDbContextScopeFactory` scope per
   method, `Specifications` for filtering, `AllMatchingPagedAsync` for paged
   results.
4. Build the `ApiController` in `WebApplication1/Areas/<Area>/Controllers`
   following the established shape (see `ZoneController.cs`,
   `CustomerController.cs`, `DivisionController.cs`,
   `EmployerController.cs`, `StationController.cs`): constructor-injected app
   service(s) with null-checks, `[RoutePrefix("api/registry/<name>")]`,
   private `ApiResponse`/`ErrorResponse` helpers returning
   `{ success, message, data }`, `Utils.CreateServiceHeader()` per action, and
   a wrapper request DTO (`CreateXRequest`) only when create needs more than
   one payload shape (e.g. an entity plus a related list).
5. Register any new app service in Unity — **both**
   `WebApplication1/App_Start/UnityConfig.cs` (used by this API project) and
   `DistributedServices.MainBoundedContext/UnityContainers/Container.cs`
   (still feeds the WCF layer that hasn't been retired yet).
6. Add the new `.cs` file(s) to the relevant old-style `.csproj`
   (`<Compile Include="...">`), and build the touched projects individually
   with MSBuild to confirm before calling it done.

## Response envelope convention

Successful API controller endpoints return:
```json
{ "success": true, "message": "...", "data": { } }
```
Existing private `ApiResponse` helpers may remain while their controller is
unmigrated. Errors are standardized through
`WebApplication1/Infrastructure/Errors`: use `ApiErrorResponses` for expected
controller failures and classified `ApiException`s for failures crossing
layers. The global Web API exception handler sanitizes unexpected exceptions,
and `CorrelationIdHandler` adds `X-Correlation-ID`. Do not add new
`InternalServerError(ex)` responses or return raw exception messages. See
`docs/API_ERROR_HANDLING_STRATEGY.md` for the contract and migration order.

## Client-facing docs

`docs/api/*.md` holds hand-written integration specs for frontend consumers
(see `docs/api/customer-api-spec.md`). Regenerate/update these from the
controller source when a controller's contract changes — don't let them
drift out of sync with the actual code.

## Controllers adapted so far

- `Areas/Registry/Controllers/CustomerController.cs`
- `Areas/Registry/Controllers/ZoneController.cs`
- `Areas/Registry/Controllers/DivisionController.cs` (new `IDivisionAppService`)
- `Areas/Registry/Controllers/EmployerController.cs`
- `Areas/Registry/Controllers/StationController.cs` (extended `IZoneAppService`)
- `Areas/Admin/Controllers/CompanyController.cs`
- `Areas/Admin/Controllers/BankController.cs` (existing `IBankAppService`)
- `Areas/Accounts/Controllers/BankLinkageController.cs` (existing
  `IBankLinkageAppService`; also split `BankLinkageDTO`'s fields off of
  `BankDTO`, which had been sharing them, and fixed a dead/unassigned
  `IBankLinkageAppService` field in `CashManagementController`)
- `Areas/Accounts/Controllers/ChequeTypeController.cs` (existing
  `IChequeTypeAppService`; reference controller's session-staged
  charges/products wizard collapsed into one `CreateChequeTypeRequest` body —
  see `docs/api/cheque-type-api-spec.md`)
- `Areas/Accounts/Controllers/ChequeBookController.cs` (existing
  `IChequeBookAppService` — was fully built with no controller anywhere,
  only reachable via the legacy `ChequeBookService.svc.cs` WCF passthrough;
  reference controller's `Edit` action had a copy-paste bug — validated and
  saved a `CustomerAccountDTO` instead of the `ChequeBookDTO` it took in, so
  it never actually updated a chequebook — not ported; see
  `docs/api/chequebook-api-spec.md` and
  `Areas/FrontOffice/CHEQUE-PROCESSING-ANALYSIS.md`)
- `Areas/Accounts/Controllers/LoanProductController.cs` (existing
  `ILoanProductAppService`; full CRUD + dynamic-charges/loan-cycles/
  auxiliary-conditions/deductibles/auxiliary-appraisal-factors/
  appraisal-products/commissions sub-resources) — started as a read-only
  list endpoint to unblock `ChequeTypeController`'s Create picker (no
  working route existed before), later completed into a full adaptation.
  The reference MVC controller's Create/Edit views are a session-staged
  wizard (several small AJAX round-trips into `Session`, flushed by one
  `Create` POST via `UpdateAssociatedData`) with no real behavior beyond
  "attach these sub-collections" — collapsed into direct sub-resource
  `GET`/`PUT` routes, same pattern as `CommissionController`'s
  graduated-scales/splits/levies, with `Create` additionally accepting all
  sub-collections up front in one request instead of a session round-trip.
  Not ported: seven pure session-staging actions with no persistence
  behavior of their own (superseded by the sub-resource endpoints), and
  four near-duplicate single-record lookups (`GetInvestmentProductDetails`/
  `GetSavingDetails`/`GetloanDetails`/`GetLoanProductDetails`) used only to
  populate a description label while picking a linked product in the
  wizard — redundant with this controller's own `GET {id}` (and
  `SavingsProductController`'s, for the savings case). See
  `docs/api/loan-product-api-spec.md`.
- `Areas/Accounts/Controllers/CommissionController.cs` (existing
  `ICommissionAppService`; full CRUD + graduated-scales/splits/levies
  sub-resources) and `Areas/Accounts/Controllers/LevyController.cs` (new,
  existing `ILevyAppService`; full CRUD + splits sub-resource) — reference
  app has a redundant, buggier duplicate (`ChargesController`) and a
  non-functional one (`TiersController`, its persistence call is commented
  out) that weren't ported; see history notes in
  `docs/api/commission-api-spec.md` / `docs/api/levy-api-spec.md` and
  `COMMISSION-LEVY-CHARGE-CONCEPTS.md`
- `Areas/Accounts/Controllers/UnPayReasonController.cs` (new, existing
  `IUnPayReasonAppService`; full CRUD + attached-commissions sub-resource) —
  previously only reachable via the legacy `UnPayReasonService.svc.cs` WCF
  passthrough, no controller existed; fixed a missing-`ValidateAll()` bug on
  edit rather than porting it (see `docs/api/unpayreason-api-spec.md`)
- `Areas/Accounts/Controllers/ElectronicStatementOrderController.cs` (new,
  existing `IElectronicStatementOrderAppService`; CRUD + due/skipped/history
  listings — a recurring statement-emailing *subscription*, not statement
  content, no overlap with `CustomerAccountStatementController` below
  despite the shared "statement" name) and
  `Areas/Accounts/Controllers/ElectronicStatementOrderExecutionController.cs`
  (batch triggers via `IRecurringBatchAppService`) — split into two
  controllers for the same reason as Standing Orders below; previously only
  reachable via the legacy `ElectronicStatementOrderService.svc.cs` WCF
  passthrough, no controller existed; see
  `docs/api/electronic-statement-order-api-spec.md` /
  `docs/api/electronic-statement-order-execution-api-spec.md`
- `Areas/Messaging/Controllers/TextAlertController.cs` (existing `ITextAlertAppService`)
- `Areas/FrontOffice/Controllers/*` — teller transactions, treasury, cheques,
  end of day, account closure, fixed deposits, expense payables, sundry
  payments/customer receipts, in-house cheques, automated clearing, fiscal
  counts. See `Areas/FrontOffice/WORKFLOW.md` for the functional design and
  `docs/api/frontoffice-api-spec.md` for the endpoint reference.
- Also live but missing from this list until now (documentation gap, not a
  build gap): `Areas/Accounts/Controllers/CustomerAccountStatementController.cs`
  (on-demand mini/full statement + PDF print, `docs/api/customer-account-statement-api-spec.md`)
  and `Areas/Accounts/Controllers/StandingOrderController.cs` +
  `StandingOrderExecutionController.cs` (`docs/api/standing-order-api-spec.md` /
  `docs/api/standing-order-execution-api-spec.md`).
- `Areas/Accounts/Controllers/CreditBatchController.cs` (new, existing
  `ICreditBatchAppService`; batch CRUD/audit/authorize + entry CRUD/browse/
  post) — the reference controller only covered the batch header lifecycle
  through `_channelService`; entry browsing for a pickup queue was never its
  own endpoint anywhere in the reference app (the reference
  `SundryPaymentsController` had a private `FetchCreditBatchEntriesTable`
  action instead). Built specifically to unblock the FrontOffice "Sundry
  Receipts/Payments → Cash Pickup" screen (`entries/type/{creditBatchType}`
  + `entries/{entryId}/post`); `SundryPaymentsController`'s `CashPickup` case
  now calls `PostCreditBatchEntry` after a successful payment so a picked
  entry can't be paid twice. Discrepancy browsing/matching and CSV batch
  import were originally left unexposed as separate reconciliation/upload
  concerns not needed for Cash Pickup — **discrepancy browsing/matching is
  now exposed** (`GET .../{id}/discrepancies`, `POST .../match`,
  `.../match-gl`, `.../reject`) since it's what turns a CheckOff/Payout
  import's unmatched rows into real, product-allocated entries; see
  `docs/api/batch-procedures-api-spec.md` §1.3 for the CheckOffEntryType
  branching this exposes. CSV import itself has its own controller,
  `CreditBatchImportController.cs` — see
  `docs/api/frontoffice-api-spec.md` §13.3. **Real bug found and fixed,
  found much later** (on a follow-up audit pass triggered by the
  `LoanDisbursementBatch`/`MarkLoanCaseDisbursed` bug — see the
  `LoanCaseController` entries further below): `AuthorizeCreditBatch`'s
  precondition that the batch already be `Audited` was entirely commented
  out in source (`//if (persisted == null || persisted.Status !=
  (int)BatchStatus.Audited) //    return result;`, replaced by a bare null
  check) — a Credit batch could be authorized, and its journals/queued
  entries posted, straight from `Pending`, completely bypassing the
  maker-checker Audit step this whole module exists to enforce. Same
  category of control-bypass as the `InterAccountTransferBatch` fix.
  Restored the real check.
- `Areas/Accounts/Controllers/DebitBatchController.cs` (new, existing
  `IDebitBatchAppService`; batch CRUD/audit/authorize + entry
  add/remove/browse/post) — first of the wider "Batch Procedures" module
  (see `Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md` for the functional
  basis and `docs/api/batch-procedures-api-spec.md` for the full route
  reference). Real asymmetries from `CreditBatchController` worth knowing:
  no `TotalValue` control-total exists for this type at all, `Authorize`
  genuinely refuses a batch that isn't already `Audited` — **Credit's
  equivalent guard used to be commented out in source (`AuthorizeCreditBatch`
  could post straight from `Pending`, skipping the maker-checker Audit
  step entirely); found and fixed on a later pass, see the
  `CreditBatchController` entry above** — entries have no amount-shaped
  field trustworthy before posting (`Multiplier`/`BasisValue` feed a
  server-side tariff computation at post-time, capped against available
  balance), and `Authorize` always queues every entry for async posting
  via a message broker with no per-type carve-out (unlike Credit, where
  only `Payout`/`CheckOff` get queued and `CashPickup` stays manual). No
  single-entry lookup or entry-status-update exists on this app service,
  unlike Credit's — not built to fake one.
- `Areas/Accounts/Controllers/WireTransferBatchController.cs` (new, existing
  `IWireTransferBatchAppService`; batch CRUD/audit/authorize + entry
  add/update/remove/browse/post) — third of the "Batch Procedures" module
  (see `docs/api/batch-procedures-api-spec.md` §3). No plain unified
  reference controller existed for this type, only the three-way
  `BatchOrigination_WireTransfer`/`BatchVerification_WireTransfer`/
  `BatchAuthorization_WireTransfer` split — folded into one, same as every
  other type here. Blends Credit and Debit traits: real `TotalValue`
  control-total like Credit, but `Authorize` strictly requires `Audited`
  first and always queues every entry for async posting with no per-type
  carve-out, like Debit. `POST .../post` posts a real GL journal (debit
  customer, credit the wire-transfer-type's clearing G/L account) but
  **does not call any external MPESA/EFT gateway** despite the type naming
  (MPESA B2C/B2B/EFT) — `ThirdPartyResponse` on the entry DTO is never set
  anywhere in the app service. Insufficient balance auto-rejects the entry
  outright, unlike Debit's partial-deduction behavior.
- `Areas/Accounts/Controllers/JournalReversalBatchController.cs` (new,
  existing `IJournalReversalBatchAppService`; batch CRUD/audit/authorize +
  entry add/bulk-add/remove/browse/post) — fourth of the "Batch Procedures"
  module (see `docs/api/batch-procedures-api-spec.md` §4). Only one
  reference controller exists for this type (`BatchOrigination_Reversal`,
  largely copy-pasted from Disbursement/Wire Transfer with the leftovers
  still commented out) and it has no entry-adding UI at all, so the entry
  shape — `{ journalId, remarks }`, picking an already-posted `Journal` to
  reverse, no amount/tariffs — came from reading
  `JournalReversalBatchAppService` directly rather than the reference
  screen. **Real bug found and fixed**: `UpdateJournalReversalBatch` used to
  fetch the batch and save without copying any DTO fields onto it — every
  update silently did nothing. Fixed to copy `remarks`/`priority`, matching
  every sibling `Update*Batch` method. Also flagged (can't be fixed, no
  backing column exists): `JournalReversalBatchDTO.Remarks2` is `[Required]`
  by validation but is a dead field, never persisted. `Authorize` requires
  `Audited` first and always queues every entry for async posting, same as
  Debit/WireTransfer; `PostEntry` just calls the existing
  `IJournalAppService.ReverseJournals` — no balance checks or partial
  processing, simplest posting mechanic in this module.
- `Areas/Accounts/Controllers/OverDeductionBatchController.cs` (new,
  existing `IOverDeductionBatchAppService`; batch CRUD/audit/authorize +
  entry add/remove/browse) — fifth of the "Batch Procedures" module and the
  last of "Group A" (Credit, Debit, WireTransfer, Reversal, Refund — see
  `docs/api/batch-procedures-api-spec.md` §5). Refunds a prior
  over-collection back to a member; an entry pairs a debit and credit
  `CustomerAccount` plus real `Principal`/`Interest` amounts. **The one
  type in this module where `Authorize` posts every entry's journal(s)
  synchronously, inline, in the same call** — no async message-broker
  dispatch at all, unlike Credit/Debit/WireTransfer/Reversal — so it's safe
  to assume entries are `Posted` immediately after `Authorize` succeeds
  here, the opposite assumption from every sibling. Consistent with that,
  there's no `PostEntry`/queueable/single-entry-lookup on this app service
  at all. `Update`'s boolean return means "entries now sum to exactly
  `TotalValue`" (equality, not Credit's "does not exceed"), not "save
  failed" — reflected in the controller's response message, not treated as
  an error.
- `Areas/Accounts/Controllers/LoanDisbursementBatchController.cs` (new,
  existing `ILoanDisbursementBatchAppService` — lives in `BackOfficeModule`,
  unlike every other service in this module; batch CRUD/audit/authorize +
  entry add/bulk-add/update/remove/browse/post) — sixth of the "Batch
  Procedures" module and the deepest per-entry posting logic in it (see
  `docs/api/batch-procedures-api-spec.md` §6). An entry is meant to pick an
  already-`Audited`, not-yet-batched `LoanCase` — the reference app's own
  picker screen filters on that status — but this is only a client-side
  convention, not a server-side guarantee; see the corrected note below.
  **Two things deliberately
  not ported from the reference app**: raw SQL hacks against
  `swiftFin_LoanCases` to stamp batch numbers (the real app service already
  does this correctly through the domain layer), and — more importantly —
  the reference `Authorize` action's post-authorize loop in the MVC
  controller itself, which sends an SMS and calls an MPESA B2C helper with
  a phone number that's declared but never assigned (always `""`), plus
  flips a local in-memory DTO's status that's never saved. None of that is
  real; none of it was reproduced. Posting one entry (async, off the
  message queue after `Authorize`, same as Debit/WireTransfer/Reversal)
  resolves/creates the customer's loan and savings accounts, posts the
  disbursement journal, recovers upfront dynamic charges, marks the loan
  case `Disbursed` for real, and creates/updates a `StandingOrder` for the
  repayment schedule. `DisburseMicroLoan` (separate real-time/
  alternate-channel path) and CSV import (doesn't exist on this interface)
  are both out of scope; `batchTotal`/`startDate`/`endDate` on the DTO have
  no backing column at all. **Real bug found and fixed, found later** (on a
  follow-up pass once the loan-case appraisal/approval/audit pipeline below
  was fully built — `docs/api/batch-procedures-api-spec.md` §6.3 has full
  detail): `ILoanCaseAppService.MarkLoanCaseDisbursed`, called from
  `PostLoanDisbursementBatchEntry` after the disbursement journal already
  posts real money, only matched `case LoanCaseStatus.Approved:` in its
  switch — but a loan case that went through the intended pipeline is
  `Audited` (a distinct enum value) by the time it's disbursed. Silently
  returned `false` for every correctly-audited case, so the loan case never
  flipped to `Disbursed` and its repayment `StandingOrder` never got
  created, even though the money had already moved. Fixed to match
  `Audited`. Also corrected above: `AddNewLoanDisbursementBatchEntry`/
  `UpdateLoanDisbursementBatchEntries` never actually check the `LoanCase`'s
  `Status` at all, only that it isn't already batched — an earlier version
  of this note implied that requirement was server-enforced; it isn't.
- `Areas/Accounts/Controllers/JournalVoucherController.cs` (new, existing
  `IJournalVoucherAppService`; voucher CRUD/audit/authorize + entry
  add/replace/remove/browse) — seventh of the "Batch Procedures" module and
  the first of "Group B" (Voucher, General Ledger — see
  `docs/api/batch-procedures-api-spec.md` §7). Corrected a wrong assumption
  from `BATCH-PROCEDURES-CONCEPTS.md` §5's first pass: Voucher is not a
  free-form N-line journal with independent per-line debit/credit — that
  was inferred from `JournalVoucherEntryDTO`'s `type`/`entryType` fields,
  which `JournalVoucherAppService` never actually reads. The real shape,
  confirmed by reading `AuthorizeJournalVoucher` directly: one primary
  account (the header, at `TotalValue`) on one side, versus however many
  entries (each its own account + amount) on the other side, with the
  header's single `Type` setting direction for every leg at once. `§5` is
  now corrected, with a note that a DTO's fields aren't proof of behavior
  in this codebase. **Fixed, not just documented**:
  `AddNewJournalVoucher`'s out-of-range `ValueDate` guard set
  `ErrorMessageResult` via a `string.Format` call missing its `{0}`
  placeholder, always returning the literal text `"ValueDate"` instead of
  the real message — fixed to assign the message directly. Also found (not
  fixed, nothing to fix): `IJournalVoucherAppService` has two genuinely
  identical bulk-entry-replace methods
  (`UpdateJournalVoucherEntryCollection`/`UpdateJournalVoucherEntries`) —
  only the former is exposed, matching the reference controller. `Authorize`
  posts synchronously like Refund, no async broker dispatch.
- `Areas/Accounts/Controllers/GeneralLedgerController.cs` (new, existing
  `IGeneralLedgerAppService`; ledger CRUD/audit/authorize + entry
  add/replace/remove/browse) — eighth of the "Batch Procedures" module,
  completing "Group B" (see `docs/api/batch-procedures-api-spec.md` §8).
  Not the same thing as `GeneralLedgerStatementController` (read-only
  reporting, `api/accounts/statements/gl-account`) despite the name
  overlap. Verified directly against `AuthorizeGeneralLedger` before
  building (same discipline the Voucher correction established) — the
  original "each entry is a self-contained double-entry transfer" read
  held up, plus one thing the DTO alone wouldn't reveal: every entry posts
  as its own separate `Journal`, not shared legs on one journal like
  Voucher. The header carries no account fields of its own — no "primary"
  account the way Voucher has one, just a container for
  already-self-balancing entries. `Authorize` posts synchronously like
  Refund/Voucher, but throws a server-side exception on an out-of-balance
  Post instead of quietly returning `false` (every sibling type does the
  latter) — caught in the controller and normalized to the usual `409`.
- `Areas/Accounts/Controllers/InterAccountTransferBatchController.cs` (new,
  existing `IInterAccountTransferBatchAppService`; batch CRUD/audit/authorize
  + entry add/replace/remove/browse + `DynamicCharges` sub-resource) —
  ninth and last of the "Batch Procedures" module, which is now complete
  (see `docs/api/batch-procedures-api-spec.md` §9 and
  `Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md`). One source customer
  account transfers its balance out to entries each targeting a customer
  account or G/L account (`apportionTo`, genuinely consulted server-side).
  **Real bug found and fixed — the most consequential one in this
  module**: `AuthorizeInterAccountTransferBatch` force-set the batch's
  status to `Audited` *before* checking it was already `Audited` (and
  before null-checking), making the "must be Audited first" guard
  tautologically always true — a batch could be authorized and its
  journals posted straight from `Pending`, completely bypassing the
  maker-checker Audit step. Fixed to check first, matching every sibling.
  Also flagged (not fixed, needs real business logic): no control-total
  validation exists anywhere for this type — `AvailableBalance` has no
  backing column and was only ever a client-side display value in the
  reference app. Posting is synchronous on `Authorize` like
  Refund/Voucher/General Ledger, but each entry gets its own call to the
  shared `IJournalAppService.AddNewJournal` (not `BulkSave`), with any
  attached `DynamicCharge`s fed in as real transfer-fee tariffs.

- `Areas/BackOffice/Controllers/LoanCaseController.cs` (new, existing
  `ILoanCaseAppService`; loan case CRUD reads + a `Create` that registers a
  case with guarantors and collateral in one call, plus a guarantor
  eligibility lookup) — start of the loan origination pipeline
  (`BackOfficeModule`); see `Areas/BackOffice/WORKFLOW.md` §14.1 for the
  full account of what was adapted. Unlike the Batch Procedures module,
  `AddNewLoanCase` itself enforces almost none of the real business rules —
  the ~40-field loan-product-at-registration-time snapshot, guarantor
  count/self-guarantee/share-sufficiency checks, and the minimum-membership-
  period gate all lived only in the reference MVC controller's session-
  driven wizard, so they had to be reproduced here rather than assumed to
  already exist server-side. **Real bug found and fixed in
  `LoanCaseAppService.UpdateLoanCaseAsync`**: two lines immediately after
  restoring `persisted.CreatedDate` re-stamped it to `DateTime.UtcNow` right
  back, and unconditionally set `CancelledBy` on every plain update, not
  just cancellations — both directly contradicted the method's own
  preceding comment and were removed. Also fixed: the reference `Create`
  action called `loanCaseDTO.ValidateAll()` but never checked `HasErrors`,
  silently discarding every `CustomValidation` rule on the DTO — this
  controller checks it and returns 400 with the real messages. Guarantor
  share values (`TotalShares`/`CommittedShares`/`AppraisalFactor`) are
  computed server-side, not trusted from the request body, same reasoning
  as the `InterAccountTransferBatch` fix. **Appraisal added onto the same
  controller** (`POST .../{id}/appraise`, `GET .../{id}/appraisal-worksheet`,
  `GET .../{id}/appraisal-factors`) rather than a separate controller — see
  `Areas/BackOffice/WORKFLOW.md` §14.2. `appraisal-worksheet` reproduces the
  real, computable part of the reference `AppraiseLoanController`'s `GET
  Appraise` action (maximum loan via investments multiplier, outstanding
  balance, maximum entitled, amortization `PMT`); the composite standing-
  orders/payouts/loan-applications padding and a literally-empty
  `foreach { }` loop in that same reference action were not reproduced.
  **Real bug found and fixed in `LoanCaseAppService.AppraiseLoanCase`/
  `Async` themselves**: same guard-clause shape as the
  `InterAccountTransferBatch` fix — the code force-set the expected prior
  status onto the fetched entity *before even null-checking it*, so
  appraising a missing loan case id threw a `NullReferenceException`
  instead of a clean 404, and the "must be Registered or Deferred"
  precondition was tautologically always true. (`MarkLoanCaseDisbursed`
  turned out not to share this bug — see the disbursement note above; it
  had a different, more consequential one instead.) **Approval added
  the same way**
  (`POST .../{id}/approve`, same guard-clause bug found and fixed in
  `ApproveLoanCase`/`Async` too) — see
  `Areas/BackOffice/WORKFLOW.md` §14.3. No separate worksheet endpoint here;
  everything real an approver needs is already on the loan case from
  registration/appraisal. **Found, not reproduced**: the reference
  `ApproveLoanController.Approve` action re-copies the same ~40 loan-product
  fields `Create` already snapshots, right before calling
  `ApproveLoanCaseAsync` — but `ApproveLoanCase` never reads any of them off
  the incoming DTO, only the approval-outcome fields and the persisted
  entity's own `Id`/`Status`. Pure busywork in the reference, not ported.
  Also worth knowing: if the loan product has `LoanRegistrationBypassAudit`
  set, a successful `Approve` auto-chains straight into `AuditLoanCase` in
  the same call — the response may already be `Audited`, not `Approved`;
  the endpoint's `message` field says so explicitly. **Audit/verification
  added the same way** (`POST .../{id}/audit`, same guard-clause bug found
  and fixed in `AuditLoanCase`/`Async` too) — see
  `Areas/BackOffice/WORKFLOW.md` §14.4. This is the consequential
  transition: `AuditLoanCase` creates the customer's loan/savings
  `CustomerAccount`s if missing, computes the repayment PV/PMT, recovers
  upfront dynamic charges, and builds/updates the repayment
  `StandingOrder` — real, business-critical domain logic left as a black
  box here, same discipline `LoanDisbursementBatchController` already uses
  for `PostLoanDisbursementBatchEntry`. Needs almost no request body
  (`{ option, auditRemarks }`) since `AuditLoanCase` reads nothing else off
  the DTO — same "found, not reproduced" pattern as approval's pointless
  loan-product re-snapshot and unchecked `ValidateAll()` call. **This
  completes the core loan origination pipeline** (Registered → Appraised →
  Approved → Audited → Disbursed, disbursement already live) — remaining
  work is guarantor sub-flows beyond initial attach, cancellation/
  restructuring, loan request intake, and payroll check-off capture.
- `Areas/BackOffice/Controllers/LoanPurposeController.cs`,
  `LoaningRemarkController.cs`, `IncomeAdjustmentController.cs` (all new,
  existing `ILoanPurposeAppService`/`ILoaningRemarkAppService`/
  `IIncomeAdjustmentAppService`; full CRUD — `docs/api/loan-backoffice-catalogues-api-spec.md`)
  and `Areas/Registry/Controllers/CustomerDocumentController.cs` (new,
  existing `ICustomerDocumentAppService`; read-only —
  `docs/api/loan-case-api-spec.md` §11) — the reference-data catalogues and
  collateral-document picker `Areas/BackOffice/WORKFLOW.md` §15.2 flagged
  as missing once the loan-case screens doc was written; the loan-case
  registration/appraisal forms genuinely can't build real pickers for
  `loanPurposeId`/`registrationRemarkId`/collateral documents/appraisal
  income-adjustment factors without them. `CustomerDocumentController`
  deliberately doesn't expose document upload
  (`AddNewCustomerDocument`/`UpdateCustomerDocument`, which take a
  `fileUploadDirectory` and are a real photo/ID-scan feature) — separate,
  larger work, not needed for this picker.

- `Areas/Channels/Controllers/CanonicalAccountsController.cs` (new,
  `v1/accounts/balance` + `v1/accounts/transactions` — BALANCE and
  MINI_STATEMENT only) — not an adapted MVC controller like everything else
  in this list; implements the fixed HTTP contract required to register
  SwiftFinancialz as an institution on
  `C:\Users\ckoda\source\repos\SwizzChannels`, a separate connector platform
  that puts WhatsApp banking (and later USSD/Telegram/web) in front of any
  registered institution with no per-institution code on its side. Uses that
  platform's envelope (`{ success, data }` / `{ success, error }`), not this
  project's `{ success, message, data }` convention — see
  `docs/api/channels-canonical-api-spec.md`. `customerReference` resolves via
  `ICustomerAppService.FindCustomerBySerialNumber`, `accountReference` via
  `ICustomerAccountAppService.FindCustomerAccountDTO(fullAccountNumber)`,
  cross-checked against each other before any data returns. Not built yet:
  `CUSTOMER_VERIFICATION` (needs a new OTP/challenge mechanism — nothing in
  this domain does this today; `MobileToBankRequestAuthOption.Verify` is an
  unrelated M-Pesa reconciliation concept, checked and ruled out) and
  institution-side call-in authentication (SwizzChannels calls in via OAuth2
  client-credentials; this project has no token endpoint for that, only
  human username/password login). Both deferred to a follow-up pass.

- `Areas/BackOffice/Controllers/LoanCaseController.cs` extended with two
  more lifecycle routes: `PUT {id}/collaterals` (full-replace via the
  already-existing `UpdateLoanCollaterals`, previously only reachable
  internally from `Create`) and `POST {id}/cancel` (`CancelLoanCase`/
  `LoanCancellationOption` — Defer/Reject, only valid against an Audited
  case; on Reject the app service releases the case's guarantors itself).
  The reference `AddCollateralController` (despite its name) never touches
  `LoanCollateralDTO` or any real collateral operation at all — confirmed
  dead/mislabeled guarantor-attach code, not ported.
- `Areas/BackOffice/Controllers/LoanGuarantorController.cs` (new, existing
  `ILoanCaseAppService`; standalone `LoanGuarantorDTO` search/read/create) —
  the reference controller's Edit view has its POST entirely commented out
  (no `UpdateLoanGuarantorAsync` call exists), so there's no Update action
  here either.
- `Areas/BackOffice/Controllers/LoanGuarantorAttachmentController.cs` (new,
  existing `ILoanCaseAppService`; post-registration guarantor attach,
  attachment-history browse/entries, relieve, substitute) — consolidates
  three reference controllers
  (`GuarantorAttachmentController`/`GuarantorRelievingController`/
  `GuarantorSubstitutionController`) that all operate on the same
  `LoanGuarantorAttachmentHistory`-family resource into one controller, per
  `LoanCaseController`'s own "one controller per resource" convention.
  `GuarantorAttachmentController`'s raw ADO.NET queries against
  `swiftFin_LoanGuarantors`/`swiftFin_Customers` were not reproduced (the
  real equivalent is `LoanGuarantorController`'s own endpoints), and
  `GuarantorManagementController` was skipped entirely — its `Add()`
  accumulates guarantors in `Session` with real validation, but the
  `Create(LoanCaseDTO)` POST that should commit them does nothing
  (`Session["LoanProductId"] = null; return View();`, no persistence call
  at all) — dead/non-functional in the reference itself, and a
  near-duplicate of `LoanCaseController.EnrichAndValidateGuarantors`
  besides.
- `Areas/BackOffice/Controllers/LoanRestructuringController.cs` (new,
  existing `ILoanCaseAppService`; single `RestructureLoan` action) — acts
  on a disbursed loan's `CustomerAccountId` directly (new term/payment via
  `NPer`/`Pmt`), unlike every other lifecycle action in this module which
  is keyed by `LoanCaseId`. The reference screen's picker lookups
  (customer accounts by product code, loan product detail) are already
  covered by existing `CustomerAccountController`/`LoanProductController`
  endpoints elsewhere in this repo, so only the real operation is exposed.
- `Areas/BackOffice/Controllers/LoanRequestController.cs` (new, existing
  `ILoanRequestAppService`; full CRUD + register/cancel lifecycle) — the
  optional pre-case intake stage before a real `LoanCase` is registered.
  The reference `Create` action is a four-screen Session wizard (Customers
  -> LoanProducts -> LoansPurpose -> Create submit) with no behavior beyond
  populating four ids onto one DTO — collapsed into a single `Create` call.
  `RegisterLoanRequest`/`CancelLoanRequest`/`RemoveLoanRequest` exist on
  the app service but have no reference MVC screen at all (the reference
  app never actually converts a `LoanRequest` into a `LoanCase`, despite
  `LoanRequestDTO` carrying `LoanCaseId`/`LoanCaseNumber` fields for
  exactly that) — exposed here as real lifecycle actions since the
  app-service methods themselves are real.

This completes the guarantor substitution/relieving/CRUD/attachment-
history, collateral-beyond-initial-attach, restructuring, and loan-request-
intake gaps. Not yet started: payroll/check-off data capture
(`DataCaptureController`, `DataProcessingController`, `ClosingController`,
`CatalogueController`), `LoanProductAppraisalController`,
`RepaymentScheduleController` preview, `ReportsController`, the "composite
customer 360 view" gap, document/photo retrieval, server-side `BranchId`
resolution, branch budget-balance check, and SMS/MPESA disbursement
notification (`BackOfficeModule`). Before starting any of the above, read
`Areas/BackOffice/WORKFLOW.md` for the module's full design and the
reference MVC app's 23-controller `Areas/Loaning` inventory.
