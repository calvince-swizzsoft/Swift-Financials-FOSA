# Cheque Processing — Architecture & Wiring Analysis

Full-stack trace of every cheque-related capability in the system — domain,
application, DTO, infrastructure, WCF, and the new Web API — plus how GL
accounts are wired at each stage of a cheque's lifecycle. Companion to
`CHEQUE-TYPE-FUNCTIONAL-REQUIREMENTS.md` (Areas/Accounts) and
`WORKFLOW.md` (this folder), which cover functional design and endpoint
reference respectively; this document covers whether the layers are wired
together *correctly*.

Eight real bugs and one missing controller were found here and have since
been fixed (see "Wiring Correctness Findings" below) — this doc is kept as
the record of the audit and the reasoning behind those fixes, not just a
point-in-time snapshot. **Finding #10 was the most serious**: cheque
deposits were crediting the customer's real balance immediately (like
cash), and `Pay` clearance credited them a second time for the same
cheque — confirmed and fixed after tracing independent corroborating
evidence in the statement-generation code, not guessed at. **Finding #12**
(discovered live, debugging a real transfer that reported success but
posted no journal) is a reminder that this codebase's `AddNewJournal`
fails silently (returns `null`) rather than throwing — every call site
that doesn't check the return value is a candidate for the same class of
bug; `ClearExternalCheque` still has it, unfixed.

## 1. ChequeType — classification master data

- **Domain**: `Domain.MainBoundedContext/AccountsModule/Aggregates/ChequeTypeAgg/ChequeType.cs`
  — anemic entity: `Description`, `MaturityPeriod` (days), `ChargeRecoveryMode`
  (`ChequeTypeChargeRecoveryMode`: `0`=OnChequeDeposit, `1`=OnChequeClearance),
  `IsLocked` (lock/unlock methods exist but the flag is never enforced anywhere
  — dead, see Finding #3). Two join aggregates: `ChequeTypeCommissionAgg`
  (links to `Commission`) and `ChequeTypeAttachedProductAgg` (links to
  loan/investment products).
- **Application**: `ChequeTypeAppService`/`IChequeTypeAppService` — standard
  CRUD + `FindAttachedProducts`.
- **DTO**: `ChequeTypeDTO.cs` — only `Description` is `[Required]`; no
  validation bug here.
- **API**: `WebApplication1/Areas/Accounts/Controllers/ChequeTypeController.cs`,
  `[RoutePrefix("api/accounts/chequetypes")]`. Registered in both Unity
  containers (`UnityConfig.cs`, `Container.cs`) and in `WebApplication1.csproj`.
- **WCF**: `DistributedServices.MainBoundedContext/ChequeTypeService.svc.cs`
  — still present, legacy-but-intentional passthrough.

## 2. ChequeBook — savings-account chequebook issuance

- **Domain**: `ChequeBookAgg/ChequeBook.cs` + `PaymentVoucherAgg` (one voucher
  row per cheque leaf).
- **Application**: `ChequeBookAppService.cs` is fully built —
  `AddNewChequeBook` computes a serial number via raw SQL, creates one
  `PaymentVoucher` per leaf, computes chequebook-issuance tariffs
  (`ComputeTariffsBySavingsProduct(..., SavingsProductKnownChargeType.ChequeBookCharges, ...)`)
  and posts a journal per tariff. `PayVoucher`/`FlagVoucher`/`ActivateChequeBook`
  (auto-deactivates sibling chequebooks on activation) all exist too.
- **API**: `WebApplication1/Areas/Accounts/Controllers/ChequeBookController.cs`
  (`api/accounts/chequebooks`) — list/search, get, create, update
  (activate/lock), per-chequebook voucher listing, voucher match-by-number,
  pay voucher, flag/unflag voucher. Placed under `Areas/Accounts` (not
  `Areas/FrontOffice`) to match where `IChequeBookAppService` actually lives
  (`AccountsModule`) and where the reference MVC controller
  (`CoA_ChequeBooksController`) lived, even though it's grouped thematically
  with the rest of the cheque lifecycle in this document. See
  `docs/api/chequebook-api-spec.md` for the full contract. Previously **no
  API controller existed** for this at all — only reachable via the legacy
  `ChequeBookService.svc.cs` WCF passthrough (see Finding #4, now fixed).

## 3. ExternalCheque — inbound customer cheque deposit → transfer → bank → clear

- **Domain**: `ExternalChequeAgg/ExternalCheque.cs` — `IsCleared`/`IsBanked`/
  `IsTransferred` flags each with a `*By`/`*Date` audit pair; links `Teller`,
  `ChequeType`, `CustomerAccount`, `BankLinkageChartOfAccountId`.
- **Application**: `ExternalChequeAppService.cs` (983 lines) — the heaviest
  cheque service:
  - `AddNewExternalCheque` — server-derives `MaturityDate` from
    `ChequeType.MaturityPeriod`; explicitly treats a `null`/`Guid.Empty`
    `ChequeTypeId` as "no cheque type selected, matures same day" (already
    correct at this layer — see Finding #1, the bug was one layer up, in DTO
    validation).
  - `TransferExternalCheques` — batch-transfer step required before EOD.
  - `BankExternalCheques` — moves cheques from holding into the bank-linkage
    GL.
  - `ClearExternalCheque` — branches on `ProductCode` (Savings/Loan/
    Investment) × `ExternalChequeClearanceOption` (`Pay`=1/`UnPay`=2), plus a
    loan/investment-arrears-recovery sub-flow (`RecoverAttachedLoans`/
    `RecoverAttachedInvestments`) that builds `Journal` domain objects
    directly and `BulkSave`s them outside the main `dbContextScope`.
  - `UpdateExternalChequePayables` — enforces `ChequeTypeAttachedProduct`
    restrictions per `CHEQUE-TYPE-FUNCTIONAL-REQUIREMENTS.md` §3.3.
- **DTO**: `ExternalChequeDTO.cs` — see **Finding #1** (fixed).
- **API**: `WebApplication1/Areas/FrontOffice/Controllers/ChequesController.cs`
  (`api/frontoffice/cheques`) handles list/bank/clear/untransferred; deposit
  itself happens through `CashDepositController`
  (`FrontOfficeTransactionType.ChequeDeposit`); batch-transfer happens
  through `TransfersController.TransferSelectedChequesAsync`.
- **WCF**: `ExternalChequeService.svc.cs` — thin passthrough, no duplicated
  logic.

## 4. TruncatedCheque / Automated Clearing — national clearing-house (KBACTS)

- **Domain**: `TruncatedChequeAgg/TruncatedCheque.cs` — carries the full
  clearing-house record shape (front/rear images, presenting bank/branch,
  destination account, etc.) plus `Status` (`New`/`Processed`) and
  `UnPaidCode`/`UnPaidReason`. No `ChequeType` reference — a separate concern
  from ExternalCheque.
- **Application**: folded into `ElectronicJournalAppService.cs` (841 lines)
  rather than a dedicated service — `ParseElectronicJournalImport` (KBACTS
  file parse → one `TruncatedCheque` per record, auto-matches against
  `ChequeBook` payment vouchers by serial number), `ClearTruncatedCheque`
  (Pay/UnPay), `CloseElectronicJournal` (PGP-encrypts and exports unpaid-item
  extract).
- **API**: `AutomatedClearingController.cs` (`api/frontoffice/automatedclearing`)
  — upload/close/clear/match-voucher. Two reference-MVC bugs were fixed
  rather than ported (wrong app-service call, discarded query result — see
  `WORKFLOW.md` §11).
- **WCF**: no dedicated `.svc` — consistent with this being a file-driven
  batch flow rather than an interactive one.

## 5. InHouseCheque — SACCO-issued outward cheques

- **Domain**: `InHouseChequeAgg/InHouseCheque.cs` — `DebitChartOfAccountId`
  (non-nullable `Guid`, always required), `DebitCustomerAccountId`
  (nullable), `Funding` (`DebitCustomerAccount`=1 /
  `DebitGeneralLedgerAccount`=2), `Chargeable`, `IsPrinted`/`PrintedNumber`/
  `PrintedBy`/`PrintedDate`.
- **Application**: `InHouseChequeAppService.cs` — `AddNewInHouseCheque` posts
  Credit=`InHouseChequesControl` / Debit=`DebitChartOfAccountId` immediately
  at write time (not gated by `ChequeTypeChargeRecoveryMode` — confirmed
  intentional); `PrintInHouseCheque` posts a second journal
  (Credit=bank-linkage GL / Debit=`InHouseChequesControl`) when the cheque is
  actually printed. See **Finding #8** for an asymmetry in the `Chargeable`
  flag between the two `Funding` branches (open, low severity).
- **DTO**: `InHouseChequeDTO.cs` — see **Finding #2** (fixed).
- **API**: `InHouseController.cs` (`api/frontoffice/inhousecheques`) — batch
  create + print.
- **WCF**: `InHouseChequeService.svc.cs` — legacy passthrough, intentional.

## GL Account Wiring

Everything routes through
`IJournalAppService.AddNewJournal(..., creditChartOfAccountId, debitChartOfAccountId, ...)`
→ `IJournalEntryPostingService.PerformDoubleEntry` →
`Journal.PostDoubleEntries(debitChartOfAccountId, creditChartOfAccountId, ...)`
— shared infrastructure (not cheque-specific), param order verified
preserved correctly through the chain, and exercised the same way by
Commission/Levy postings.

GL account sourcing per stage, all resolved via
`IChartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode`
against fixed `SystemGeneralLedgerAccountCode` enum members:

| Stage | Credit | Debit |
|---|---|---|
| Cheque deposited | `ExternalChequesControl` (tagged to the customer for statement visibility, not yet their spendable balance — see Finding #10) | Teller's cash GL |
| Batch-transfer (teller → holding) | Teller's cash GL | `ExternalChequesInHand` |
| Banked (holding → bank) | `ExternalChequesInHand` | Bank-linkage GL |
| Clearance — Pay (savings) | Customer's product GL | `ExternalChequesControl` |
| Clearance — UnPay (savings) | `ExternalChequesControl`, then reversed | Customer's product GL (both legs) |
| Loan/investment arrears recovery on clearance | Loan/Investment product GL | Customer's savings GL |
| Truncated-cheque clearance — Pay | `TruncatedChequesSettlement` | Customer's product GL |
| Truncated-cheque clearance — UnPay | Tariff-driven only (no principal movement) | — |
| In-house cheque written | `InHouseChequesControl` | `InHouseChequeDTO.DebitChartOfAccountId` (client-supplied) |
| In-house cheque printed | Bank-linkage GL | `InHouseChequesControl` |
| ChequeBook issuance charge | Tariff-driven | Tariff-driven |

All directions read as internally consistent (money-in credits the
customer/increases the liability side, money-out/suspense debits move the
other way) and match the pattern already verified for Commission/Levy.

## Wiring Correctness Findings

| # | Severity | Status | Finding |
|---|---|---|---|
| 1 | Real bug | **Fixed** | `ExternalChequeDTO.ChequeTypeId` is `Guid?` decorated with `[ValidGuid]`. `ValidGuidAttribute.IsValid` converted the raw value via `Convert.ToString`, and for `null` that yields `""`, which failed the `IsNullOrWhiteSpace` check — so the attribute rejected `null`, not just `Guid.Empty`. Result: a teller could not deposit a cheque without selecting a cheque type, even though `ExternalChequeAppService.AddNewExternalCheque` and the functional spec both treat "no cheque type" as valid ("matures same day"). `CustomerTransactionModel.ChequeType` was also a non-nullable `Guid` (defaulting to `Guid.Empty` when omitted from the request body), a second way into the same failure. **Fix**: `ValidGuidAttribute.IsValid` now returns `Success` immediately when `value == null` (`Infrastructure.Crosscutting.Framework/Attributes/ValidGuidAttribute.cs`) — root-cause fix that also silently repairs the same latent bug on every other nullable `[ValidGuid]` field in the codebase (`CustomerBindingModel.StationId`, `LoanCaseDTO.LoanPurposeId`, `BudgetEntryDTO.ChartOfAccountId`, ~15 DTOs total), not just cheques. `CustomerTransactionModel.ChequeType` changed from `Guid` to `Guid?` so an omitted cheque type reaches `ExternalChequeDTO.ChequeTypeId` as `null` instead of `Guid.Empty`. |
| 2 | Real bug | **Fixed** | `InHouseChequeDTO.BranchId`/`DebitChartOfAccountId`/`ChequeTypeId` had `[ValidGuid]` commented out in source. `DebitChartOfAccountId` is used live for GL posting (`InHouseChequeAppService.cs`, `AddNewJournal(..., inHouseChequeDTO.DebitChartOfAccountId, ...)`) with zero server-side format validation. Deeper issue found during the fix: restoring the attribute alone was cosmetic — `InHouseController.Create` never called `ValidateAll()`/checked `HasErrors` anywhere in the chain (`InHouseChequeAppService.AddNewInHouseCheque(s)` doesn't either), so nothing would have enforced it. **Fix**: restored `[ValidGuid]` on all three properties, and added a validation loop in `InHouseController.Create` (`WebApplication1/Areas/FrontOffice/Controllers/InHouseController.cs`) — `cheque.ValidateAll()` / `HasErrors` check per batch entry, returning 400 with joined error messages — matching the pattern every other controller in this codebase uses (e.g. `CommissionController.Create`). |
| 3 | Dead code | Open | `ChequeType.IsLocked` is fully unenforced — no caller outside `ChequeTypeAppService` itself reads it. Already an open item in `CHEQUE-TYPE-FUNCTIONAL-REQUIREMENTS.md` §3.4/§5. |
| 4 | Undocumented gap | **Fixed** | `ChequeBook` had a fully-built domain + app-service + GL-posting path with zero API controller (see §2 above). **Fix**: added `ChequeBookController.cs` (`api/accounts/chequebooks`) covering every `IChequeBookAppService` operation — list/search/get/create/update/vouchers/pay/flag/match. Documented in `docs/api/chequebook-api-spec.md` and cross-referenced from `docs/api/README.md` and `CLAUDE.md`'s adapted-controllers list. |
| 5 | Correctness risk | **Fixed** | Clearance now accepts one authoritative `clearingOption` (`Pay=1`, `UnPay=2`); the redundant `actionType` field was removed from the request and frontend. The controller validates the option and requires an unpaid reason for `UnPay`. |
| 6 | Efficiency | Open | `ChequesController.BankSelectedCheques`/`ClearSelectedCheques` load the entire unbanked/uncleared cheque table (`pageSize: int.MaxValue`) and filter selected IDs client-side in memory instead of a targeted query. Functionally correct, doesn't scale. |
| 7 | Convention drift | Open | `ChequesController`'s failure responses omit `data`, breaking the `{success, message, data}` envelope every other controller in the codebase follows. |
| 8 | Behavioral asymmetry | Open | `InHouseChequeAppService.AddNewInHouseCheque`'s two `Funding` branches treat `Chargeable` inconsistently: the `DebitGeneralLedgerAccount` branch only posts commission tariffs `if (Chargeable)`; the `DebitCustomerAccount` branch posts tariffs unconditionally, never checking `Chargeable` at all. If `Chargeable` is meant to be a universal opt-out, it silently doesn't work for customer-funded in-house cheques. |
| 9 | Real bug | **Fixed** | **Bank → Clear was not sequence-gated, unlike Transfer → Bank.** `ChequesController.BankSelectedCheques` sources candidates from `FindUnBankedExternalCheques`, backed by `ExternalChequeSpecifications.UnBankedExternalCheques` (`x.IsTransferred && !x.IsBanked && x.BankedDate == null`) — so Transfer → Bank is correctly enforced: a cheque can't be banked before it's transferred. But the `Pay` branch of `ExternalChequeAppService.ClearExternalCheque` only required `!persisted.IsCleared`, so a cheque could be Pay-cleared straight out of deposit, having never been transferred or banked — inconsistent with the `UnPay` branch of the same method, which already required `persisted.IsTransferred && persisted.IsBanked && persisted.BankLinkageChartOfAccountId != null`. **Fix**: wrapped the `Pay` case body in `if (persisted.IsTransferred && persisted.IsBanked)` (`ExternalChequeAppService.cs`, `ClearExternalCheque`), mirroring `UnPay`'s existing precondition and its existing failure signaling (unmet precondition → nothing persisted → `SaveChanges()` returns `0` → `result` is `false` → caller sees "Failed to clear cheque"). Fixed at the app-service write boundary rather than by tightening the shared `FindUnClearedExternalCheques`/`UnClearedExternalCheques` spec, since that spec is also exposed through the WCF layer for general uncleared-cheque listings unrelated to the clearing action — narrowing it risked hiding legitimately-uncleared-but-not-yet-banked cheques from those other consumers. This closes the gap for every caller (new REST controller and the legacy WCF passthrough alike), not just `ChequesController`. |
| 10 | **Critical — customer balance overstatement** | **Fixed** | **`Pay` clearance credited the customer a second time for the same cheque.** `CashDepositController.cs`'s `ChequeDeposit` case set `CreditChartOfAccountId = targetSavingsProduct.ChartOfAccountId` — **identical** to what `CashDeposit` does — crediting the customer's own savings product GL immediately on deposit, with no distinction from real cash. `ExternalChequeAppService.ClearExternalCheque`'s `Pay`/`Savings` branch then separately posts `Credit = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId` (`ExternalChequeAppService.cs:500`) — the same customer GL account, a second time — when the cheque later clears. Net effect: a customer whose cheque clears via `Pay` was credited twice. **Root cause identified**: `ChequeDeposit`'s crediting was the wrong side, confirmed by independent evidence in `JournalEntryAppService.cs:858,999` — a dedicated `CustomerAccountStatementType.ChequeDepositStatement` that filters a customer's journal entries against `SystemGeneralLedgerAccountCode.ExternalChequesControl`, which only makes sense if deposit-time entries were meant to post against `ExternalChequesControl` (tagged to the customer for statement visibility) rather than their real product GL. With that reading, `Pay`/`UnPay` clearance (which already credits/debits `ExternalChequesControl` correctly) needed no changes — the account nets to zero across a cheque's full lifecycle once `Deposit` posts the matching other half. **Fix**: `CashDepositController.cs`'s `ChequeDeposit` case now resolves `SystemGeneralLedgerAccountCode.ExternalChequesControl` (via a newly-injected `IChartOfAccountAppService`, same resolution pattern `ExternalChequeAppService` already uses) and credits that instead of `targetSavingsProduct.ChartOfAccountId`, with a guard returning a clear error if the mapping isn't configured (matching `TransferExternalCheques`/`BankExternalCheques`'s existing `InvalidOperationException` convention for the same class of missing-mapping failure, though this path returns a JSON error response instead since it's mid-request-processing rather than a fresh call). `CreditCustomerAccountId`/`CreditCustomerAccount` are still set to the customer so the entry remains visible on their `ChequeDepositStatement` mini-statement even though it's no longer their spendable balance until `Pay` clears it. Net result across a cheque's lifecycle: customer credited exactly once (on `Pay`) or net zero (on `UnPay`, credited then reversed) — `ExternalChequesControl` now nets to zero per cheque instead of growing one-directionally. |
| 11 | Efficiency / redundancy | **Fixed** | The controller no longer calls `MarkExternalChequeCleared` after `ClearExternalCheque`; the application service remains the single owner of the cleared state and audit fields. |
| 12 | Real bug | **Fixed (Transfer/Bank); Open (Clear)** | **`TransferExternalCheques`/`BankExternalCheques` discarded `AddNewJournal`'s return value and set the transferred/banked flag unconditionally.** `JournalAppService.AddNewJournal` (`JournalAppService.cs:99-127`) returns `null` — no exception — whenever there's no current posting period configured (or `SaveChanges` returns negative), rather than throwing. Both methods set `persisted.IsTransferred`/`IsBanked` = `true` *before* calling `AddNewJournal`, discarding whatever it returned. Net effect: with no current posting period configured, transferring/banking a cheque reported `success: true`, the cheque disappeared from the untransferred/unbanked queue, but **no journal was ever posted** — a silent no-op reported as success, same bug shape as the earlier Treasury/Account-Closure findings. **Fix**: both methods now capture the `AddNewJournal` result and only flip the flag/audit fields when it's non-null (`ExternalChequeAppService.cs`, `TransferExternalCheques`/`BankExternalCheques`) — a failed journal now correctly leaves the cheque un-transferred/un-banked and the caller sees `success: false`. **`ClearExternalCheque` has the same underlying pattern (discarded `AddNewJournal` return values in most branches) but was not fixed here** — it's more entangled: `primaryJournal` (Pay/Savings branch) is captured and later dereferenced (`primaryJournal.Id`) inside `RecoverAttachedLoans`/`RecoverAttachedInvestments`, so a null journal there risks a `NullReferenceException` rather than a clean no-op, and the method already has its own upfront "no posting period → fail cleanly" guard (`ClearExternalCheque`, lines 465-467) that Transfer/Bank lacked — so the specific "no posting period" failure mode is already caught there; only other `AddNewJournal` failure causes (not yet identified) would still slip through silently in the Loan/Investment branches and the UnPay tariff loops. Flagged for a follow-up pass, not fixed. |
| 13 | Real bug | **Fixed** | **`ChequesController.BankSelectedCheques`/`ClearSelectedCheques` hardcoded `ModuleNavigationItemCode` to placeholder values instead of taking it from the client.** Every other FrontOffice controller that posts a journal (`SundryPaymentsController`, `CustomerReceiptsController`, `InHouseController`, `FixedDepositController`) accepts `ModuleNavigationItemCode` in its request body, so the resulting `Journal` row carries a real reference back to the `NavigationItem` (screen) the user was on. `ChequesController` instead hardcoded `123` (bank) and `1` (clear/unpay) — literally marked with a `/* ModuleNavigationItemCode */` comment as placeholders — and neither value corresponds to any seeded `NavigationItem.Code` (the real "Cheques" item is `0x000061A8 + 11`, see `NavigationMenu.cs:502`). Every journal posted from bank/clear carried a meaningless navigation reference, silently breaking that audit trail. **Fix**: added `ModuleNavigationItemCode` to `ChequeBankingRequest`/`ChequeClearingRequest` and passed the client-supplied value through to `BankExternalCheques`/`ClearExternalCheque` in place of the hardcoded literals — matching the sibling-controller pattern. See `docs/api/frontoffice-api-spec.md` §8. |

**Confirmed correct, not issues:**
- Unity registrations for `IChequeTypeAppService`, `IChequeBookAppService`,
  `IExternalChequeAppService`, `IInHouseChequeAppService`,
  `IElectronicJournalAppService` are present in **both**
  `WebApplication1/App_Start/UnityConfig.cs` and
  `DistributedServices.MainBoundedContext/UnityContainers/Container.cs`.
- All five cheque-related controllers (`ChequeTypeController`,
  `ChequeBookController`, `ChequesController`, `InHouseController`,
  `AutomatedClearingController`) have `<Compile Include>` entries in
  `WebApplication1.csproj`.
- All four legacy WCF `.svc` contracts (`ChequeBookService`,
  `ChequeTypeService`, `ExternalChequeService`, `InHouseChequeService`) are
  thin passthroughs to the same app services the new API uses — no
  duplicated/conflicting business logic found.
- `ChequeType.ChargeRecoveryMode` (deposit vs. clearance commission timing)
  has real, working code paths for both values, verified against
  `CHEQUE-TYPE-FUNCTIONAL-REQUIREMENTS.md`'s claims.
- Navigation is reachable: one live `NavigationItem` ("Cheques",
  `Code = 0x000061A8 + 11`, `ControllerName=Cheques`/`ActionName=Index`,
  `NavigationMenu.cs:502`) covers the whole `ChequesController` screen
  (list/bank/clear), matching the one-item-per-screen pattern every other
  multi-action FrontOffice controller uses (`Transfers`, `SundryPayments`,
  `FixedDeposit`, etc. each get a single item too). A more granular
  "Cheques → External → Banking/Clearance/Catalogue" sub-menu also exists
  in the same file but is commented out with unfilled placeholder
  `ControllerName = "Controller"` (`NavigationMenu.cs:506-515`) — an
  abandoned earlier design, superseded by the flat item that's actually
  seeded; not a gap.
