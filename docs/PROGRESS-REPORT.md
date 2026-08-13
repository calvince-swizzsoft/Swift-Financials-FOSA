# SwiftFinancialz — Core Banking Platform Progress Report

**As of 2026-08-13.** Covers the new `WebApplication1` Web API surface
being built to replace the legacy WCF (`DistributedServices.MainBoundedContext`)
layer, across Front Office, Back Office/Loaning, Batch Procedures, the
general Registry/Accounts/Admin data model, the Workflows maker-checker
engine, and WhatsApp Banking.

## Branch state — read this first

Almost everything described as "built" below is on `dev` (commit
`e7416ad`). Three branches that looked like separate unmerged work
(`main`, `feature/registry-api-controllers`,
`feature/role-scoped-workflow-approvals`) are **already fully merged into
`dev`** — they add nothing on top of it. `feature/frontoffice-fidelity`
adds one documentation-only commit not in `dev` (the actual fidelity-pass
*code* it describes is already in `dev`).

**The one real exception is `feature/whatsapp-banking`** (5 commits ahead
of `dev`, not yet merged) — the generic `AlternateChannelController` and
the entire `Areas/WhatsAppBanking` bot-facing API described in §2.6 exist
**only on that branch**. Merging it into `dev` is itself a to-do item, not
a formality — see §4.

Also flagged: the root `README.md` on `dev` is stale — it still describes
Back Office as "only disbursement batching is live, everything upstream
unbuilt," which predates the registration→appraisal→approval→audit
pipeline described in §2.2. Worth a follow-up pass; not corrected by this
report.

## 1. Executive summary

The new API surface has grown to **72 controllers on `dev`** (77 including
the unmerged WhatsApp Banking branch), backed by **31 published client
integration specs** under `docs/api/`. Front Office is functionally
complete across all 14 of its transaction areas, including a real
security-fidelity pass (six controllers were found `[AllowAnonymous]` with
wildcard CORS — fixed). Batch Procedures — the nine money-movement batch
types (Credit, Debit, Wire Transfer, Journal Reversal, Over-Deduction,
Loan Disbursement, Journal Voucher, General Ledger, Inter-Account
Transfer) — is complete, and building it surfaced and fixed two real
maker-checker bypass bugs where a batch could be authorized straight from
`Pending` without ever being audited.

Back Office / Loan Origination has its **core pipeline** live —
registration, appraisal, approval, audit/verification, and disbursement —
but that's the spine, not the whole body: guarantor and collateral
management beyond initial attach, loan restructuring/cancellation, loan
request intake, and the underlying data-attachment-period infrastructure
appraisal depends on are all still unbuilt (§3.1). This is the single
largest block of remaining work for a complete Back Office.

Two structural gaps apply system-wide, not to any one module: there is
**no REST controller anywhere for opening/closing an accounting posting
period** (§3.3) — every batch and journal posting in this system depends
on `IPostingPeriodAppService.FindCurrentPostingPeriod` returning a real,
current period, and right now the only way to manage that is the legacy
WCF service. And there is **no reporting controller** — no REST path to
the `Report`/`ReportTemplate` WCF services at all (§3.4).

## 2. What's built

### 2.1 Front Office — complete, one known gap

All 14 functional areas are live: teller master data, deposits/withdrawals
(with a request queue), treasury cash movement, cash-transfer and
cheque-transfer batches, cheque banking/clearance, end-of-day close,
automated/image clearing, account closure, expense payables, fixed
deposits, sundry payments, customer receipts, in-house cheques, fiscal
counts. See `docs/api/frontoffice-api-spec.md` and
`Areas/FrontOffice/WORKFLOW.md`.

A later fidelity pass (already merged) found and fixed real problems, not
just gaps: six controllers were `[AllowAnonymous]` with a wildcard CORS
policy — pending cash requests were readable with zero authentication.
Cash-withdrawal requests were never being enqueued into the maker-checker
Workflow engine despite cash-deposit requests already doing so (an
inconsistent, silently-weaker control on withdrawals). Teller identity was
hardcoded in places it shouldn't have been.

**Known, documented, not fixed**: `CustomerReceiptsController` and
`SundryPaymentsController` can only post a single-line GL voucher. The
reference app split one receipt across multiple chart-of-account lines via
an `AddJournalWithApportionmentsAsync`-shaped call — no such overload
exists on `IJournalAppService` in this codebase today. This is real
domain-layer work (extending the journal-posting app service), not a
controller-porting task.

### 2.2 Back Office / Loan Origination — core pipeline live

`LoanCaseController` carries the full registration → appraisal → approval
→ audit/verification lifecycle, plus disbursement via the Batch Procedures
module (§2.3). Supporting catalogues (loan purpose, loaning remarks,
income adjustment factors) are live. See `Areas/BackOffice/WORKFLOW.md`
§14 for the full design and the reference app's 23-controller inventory
this was adapted against.

Several real bugs were found and fixed while building this pipeline —
notably a guard-clause pattern repeated across `AppraiseLoanCase`,
`ApproveLoanCase`, and `AuditLoanCase` that force-set the expected prior
status onto a fetched entity *before* null-checking it, making the
precondition check tautologically always true (same category of bug found
independently in the Batch Procedures work), and a disbursement bug where
`MarkLoanCaseDisbursed` only matched `LoanCaseStatus.Approved` when a
correctly-audited case is actually `Audited` by the time it disburses —
silently leaving disbursed loans without their repayment standing order.

**Not built** — see §3.1 for the full list and why it matters.

### 2.3 Batch Procedures — all 9 types complete

Credit, Debit, Wire Transfer, Journal Reversal, Over-Deduction/Refund,
Loan Disbursement, Journal Voucher, General Ledger, Inter-Account
Transfer. See `Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md` and
`docs/api/batch-procedures-api-spec.md`.

Two real maker-checker bypass bugs were found and fixed here: Credit
batch's `AuthorizeCreditBatch` had its "must already be Audited"
precondition entirely commented out in source; Inter-Account Transfer's
equivalent method force-set the batch to `Audited` *before* checking it
was already `Audited`, making the guard tautological. Both allowed a
batch to be authorized — and its journals posted — straight from
`Pending`, completely bypassing the audit step the maker-checker design
exists to enforce.

### 2.4 Registry, Accounts, Admin — the core data model

Customer, Zone, Division, Employer, Station, Company, Bank, Bank Linkage,
Cheque Type, Cheque Book, Loan Product (full CRUD across the product
definition and every sub-collection — dynamic charges, loan cycles,
appraisal factors/products, deductibles, commissions), Commission, Levy,
UnPayReason, Electronic Statement Order (+ execution), Text Alert,
Customer Account Statement, Standing Order (+ execution). All live, all
documented under `docs/api/`.

### 2.5 Workflows — the generic maker-checker engine

`Areas/Workflows/Controllers/WorkflowController.cs` — one generic engine
(`api/administration/workflows`: create, get-by-record, in-progress check,
queueable items, my-items/all-items, approve, entries, settings) that
Customer verification, CustomerAccount verification, and several Front
Office request types (cash deposit/withdrawal, expense payables) all
route through. Later hardened to scope approval requests and navigation
menus by the caller's role, server-side, rather than trusting the client.

### 2.6 WhatsApp Banking — built, not yet merged into `dev`

Two pieces, on `feature/whatsapp-banking`:

- **Generic `AlternateChannelController`** (`Areas/Accounts`) — staff-facing
  linking/approval/fee-configuration for every `AlternateChannelType`
  (Sacco Link, Sparrow, MCo-op Cash, SpotCash, Citius, Agency Banking,
  PesaPepe, ABC Bank, Broker, WhatsApp Banking), not scoped to any one
  channel — the reference app never builds this per-channel either.
- **`Areas/WhatsAppBanking`** — the bot-facing API: OTP + PIN identity,
  customer registration + channel linking, balance/deposit-instructions/
  withdrawal, and an inbound C2B confirmation webhook.

Building this surfaced two real, previously-undocumented gaps that were
fixed rather than just flagged: `IBankToMobileRequestAppService`'s
existing method didn't actually debit an account or post a journal
despite its name (a new `RequestPayout` method was added that does the
real double-entry posting), and `AlternateChannelDTO.MobilePIN` had no
persistence path anywhere in the app service layer (added, hashed via the
same utility already used for staff credentials). See
`WebApplication1/Areas/WhatsAppBanking/WORKFLOW.md` for the full design
history and `docs/api/whatsapp-banking-api-spec.md` /
`docs/api/alternate-channel-api-spec.md` for the integration guides.

## 3. What's left

### 3.1 Back Office / Loaning — the largest remaining block

Verbatim from `Areas/BackOffice/WORKFLOW.md`'s status table, nothing built
yet for any of these:

- **Loan request intake** — the entry point before a case is even
  registered.
- **Guarantor management beyond initial attach**: `AddCollateralController`,
  `AttachGuarantorController`/`GuarantorManagementController`, guarantor
  attachment history, guarantor relieving, guarantor substitution,
  `LoanGuarantorController` (CRUD/search).
- **Restructuring and cancellation** of an existing loan case.
- **Data attachment period infrastructure** — open/edit
  (`DataCaptureController`), entry capture (`DataProcessingController`),
  close (`ClosingController`), browse (`CatalogueController`). This
  underpins appraisal data capture; without it, appraisal is working off
  whatever's already in the system, not a real capture workflow.
- **Loan product appraisal budget**, **repayment schedule preview**, and
  **loan reporting by status**.

Practical read: the pipeline that exists (registration → disbursement) is
the spine a loan case moves through, but a real back office needs to
originate loans (request intake), secure them properly (guarantor/
collateral beyond a bare initial attach), and handle the exceptions every
lending operation has (restructuring, cancellation) — none of which exist
yet.

### 3.2 Front Office — apportioned journal posting

`CustomerReceiptsController`/`SundryPaymentsController` need a
multi-chart-of-account journal split the current `IJournalAppService`
doesn't support (§2.1). Scoped, real domain work — extend the journal
app service, not a controller port.

### 3.3 Core banking operations — no posting-period management (flagged prominently)

Checked directly: there is no `PostingPeriodController` anywhere under
`WebApplication1/Areas` on `dev` — only `IPostingPeriodAppService`/
`PostingPeriodAppService` (used internally by every batch and journal
posting call in this system) and the legacy `PostingPeriodService.svc.cs`.
This is worth calling out on its own, separate from the "still uses WCF"
list below: **every single batch/journal posting operation across Front
Office, Back Office, and Batch Procedures depends on a current posting
period existing**, and right now the only way for staff to open, close,
or manage one is the WCF service this whole effort is trying to retire.
This is a strong candidate for priority attention (§4).

### 3.4 Reporting — no REST surface at all

No `ReportController`/`ReportTemplateController` exists. The `Report`/
`ReportTemplate` WCF services are the only current entry point. Whatever
reporting story SwiftFinancialz needs for a real deployment — statements,
regulatory reports, management reports — has to be designed and built
from scratch on the REST side; there's no partial adaptation to build on.

### 3.5 WhatsApp Banking — remaining gaps (already flagged in its own docs)

From `docs/api/whatsapp-banking-api-spec.md` §8: outbound B2C payout
automation (`SwiftFinancials.BankToMobileHostInterface` is still an empty
stub — withdrawal debits are real, payout isn't), fee-charging
(`BalanceInquiryCharges`/`DepositCharges`/`WithdrawalCharges`/
`PINResetCharges` are looked up but never posted), PIN retry-lockout
(`AlternateChannel.IsLocked` exists as a field, nothing sets it), and a
genuine product/security decision — whether an ungated `approve`/`reject`
status flip is an acceptable maker-checker substitute for a channel a
customer links to themselves. Plus: this whole branch needs merging into
`dev` before any of it is part of the "real" system (see Branch state,
above).

### 3.6 Cross-cutting: how much of the legacy WCF layer is actually retired

**131 `.svc.cs` files remain** in `DistributedServices.MainBoundedContext`
against 72 live REST controllers. That ratio overstates the gap, though —
CLAUDE.md's policy is deliberate non-deletion of old WCF contracts even
after a REST equivalent ships (so the two coexist by design during
migration), and many of those 131 already have a live REST controller
sitting next to them. The parts that genuinely have **no REST equivalent
at all** cluster into a few coherent groups, roughly in order of how much
they matter to "a good core banking system":

- **Posting period management** (§3.3) and **reporting** (§3.4) — core
  banking operations, no REST path, called out separately above because
  of how central they are.
- **HR/Payroll**: employee appraisal (period/target), leave
  application/type, pay slips, salary card/group/head/period, training
  period, employee exit + exit interview, holidays, imprest, employee
  documents. A real, sizeable sub-system, but adjacent to — not part of —
  the front/back office core banking scope this report is about.
- **Purchasing/Sales/Inventory**: purchase order/invoice/credit memo,
  sales order/invoice/credit memo, inventory, category, media, file
  register/upload. Same read — real, but ERP-adjacent, not core banking.
- **Membership/Education**: membership + membership manager, education
  register/venue, micro-credit group/officer.
- **Misc unmigrated Accounts-adjacent pieces**: bank reconciliation
  period, commission exemption, conditional lending, direct debit,
  dynamic charge (standalone CRUD, distinct from the sub-resource
  endpoints that already exist on LoanProduct/ChequeType), electronic
  journal, external cheque, fixed deposit type, funeral rider claim,
  super saver payable, wire transfer type, withdrawal notification, AR
  customer, broker request, budget, delegate, insurance company, payment,
  utility.

None of this last group was individually confirmed as "definitely has no
REST equivalent yet" with full certainty — a few names are plausibly
already covered under a broader existing controller (e.g. commission
exemption might already ride on `CommissionController`) and would need a
quick check before being treated as confirmed backlog, not just assumed
uncovered.

### 3.7 Housekeeping

- Root `README.md` is stale on the Back Office status (see Branch state,
  above) — worth a follow-up correction pass.
- `feature/whatsapp-banking` needs merging into `dev` (§2.6).

## 4. Recommended priority order

This is a recommendation, not a decision already made — flagging the
trade-offs for whoever prioritizes next:

1. **Merge `feature/whatsapp-banking` into `dev`.** It's finished,
   self-contained, and builds clean — leaving it stranded on its own
   branch is pure risk (drift, conflicts accumulating) for no benefit.
2. **Posting period management controller** (§3.3). Every other piece of
   this system's money-movement logic already assumes this exists and
   works; it's the one core-banking operational gap with no REST path
   and the highest blast radius if it's ever needed under time pressure
   (e.g. period-end close).
3. **Back Office guarantor/collateral + loan request intake** (§3.1). The
   loan pipeline's spine works, but a lender can't actually originate and
   secure loans the way a real SACCO needs to without these — this is the
   largest coherent block of missing *core banking* functionality (as
   opposed to ERP-adjacent modules) in the whole system.
4. **Apportioned journal posting** for Customer Receipts/Sundry Payments
   (§3.2) — scoped, well-understood, real domain work.
5. **Reporting** (§3.4) — no REST surface exists at all; needs its own
   design pass before "port the WCF service" is even the right frame.
6. **Restructuring/cancellation, data attachment periods, loan reporting**
   (§3.1 remainder) — real but lower-urgency than origination/security.
7. **WhatsApp Banking's own open items** (§3.5) — outbound payout host,
   fee-charging, PIN lockout, the maker-checker policy question — once
   the branch is merged and the SACCO is actually ready to pilot the
   channel.
8. **HR/Payroll, Purchasing/Sales/Inventory, Membership/Education** — real
   systems, but outside "front office/back office core banking," so
   sequenced after the core banking gaps above unless the business has a
   specific near-term need for one of them.
