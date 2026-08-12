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

Every API controller endpoint returns:
```json
{ "success": true, "message": "...", "data": { } }
```
via the `ApiResponse`/`ErrorResponse` private helpers repeated per controller
(no shared base class currently — that's consistent with the rest of the
codebase, not an oversight to "fix").

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
  `ILoanProductAppService`; read-only list endpoint added to unblock
  `ChequeTypeController`'s Create picker — no working route existed before,
  see `docs/api/loan-product-api-spec.md`)
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
  import were deliberately not exposed — separate reconciliation/upload
  concerns, not needed for Cash Pickup; see
  `docs/api/frontoffice-api-spec.md` §13.3.
- `Areas/Accounts/Controllers/DebitBatchController.cs` (new, existing
  `IDebitBatchAppService`; batch CRUD/audit/authorize + entry
  add/remove/browse/post) — first of the wider "Batch Procedures" module
  (see `Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md` for the functional
  basis and `docs/api/batch-procedures-api-spec.md` for the full route
  reference). Real asymmetries from `CreditBatchController` worth knowing:
  no `TotalValue` control-total exists for this type at all, `Authorize`
  genuinely refuses a batch that isn't already `Audited` (Credit's
  equivalent guard is commented out in source), entries have no
  amount-shaped field trustworthy before posting (`Multiplier`/`BasisValue`
  feed a server-side tariff computation at post-time, capped against
  available balance), and `Authorize` always queues every entry for async
  posting via a message broker with no per-type carve-out (unlike Credit,
  where only `Payout`/`CheckOff` get queued and `CashPickup` stays manual).
  No single-entry lookup or entry-status-update exists on this app service,
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
