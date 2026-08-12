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
| Electronic statement orders (recurring statement subscriptions) | `api/accounts/electronicstatementorders` | [`electronic-statement-order-api-spec.md`](electronic-statement-order-api-spec.md) |
| Electronic statement order execution (batch triggers) | `api/accounts/electronicstatementorders/execution` | [`electronic-statement-order-execution-api-spec.md`](electronic-statement-order-execution-api-spec.md) |
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
| Batch procedures — all nine types (Credit, Debit, Wire Transfer, Journal Reversal, Refund, Loan Disbursement, Journal Voucher, General Ledger, Inter Account Transfer) | `api/accounts/{creditbatches,debitbatches,wiretransferbatches,journalreversalbatches,overdeductionbatches,loandisbursementbatches,journalvouchers,generalledgers,interaccounttransferbatches}` | [`batch-procedures-api-spec.md`](batch-procedures-api-spec.md) |
| Text alerts | `api/messaging/textalert` | [`textalert-api-spec.md`](textalert-api-spec.md) |
| Front office (teller transactions, treasury, cheques, EOD, account closure, fixed deposits, expense payables, sundry payments, in-house cheques, automated clearing, fiscal counts) | `api/frontoffice/*` | [`frontoffice-api-spec.md`](frontoffice-api-spec.md) |
| Loan case registration + appraisal + approval (back office — loan origination pipeline) | `api/backoffice/loancases` | [`loan-case-api-spec.md`](loan-case-api-spec.md) |

## Changelog — what's new and what needs frontend action

Newest first. Each entry says what to build and, where relevant, what to
change in code that already exists.

### Loan Case API — approval added, plus another guard-clause bug fixed

`POST /{id}/approve` added onto `LoanCaseController` — the third stage of
the loan origination pipeline. No new worksheet endpoint; everything real
an approver needs is already on the loan case from registration/appraisal.

**Found while building, not reproduced**: the reference
`ApproveLoanController.Approve` action re-copies the same ~40 loan-product
fields `Create` already snapshots, right before calling
`ApproveLoanCaseAsync` — but `ApproveLoanCase` never reads any of them off
the incoming DTO, only the approval-outcome fields
(`approvedAmount`/`approvedAmountRemarks`/`approvedPrincipalPayment`/
`approvedInterestPayment`/`monthlyPaybackAmount`/`totalPaybackAmount`/
`approvalRemarks`) and the persisted entity's own `Id`/`Status`. Pure
busywork in the reference. Also not reproduced: `loanCaseDTO.ValidateAll()`
called and never checked — its rules were already meaningfully enforced
once, at `Create`; running them again here against a lean approve-request
payload would just be noise. Real requirements are explicit instead:
`approvalRemarks` always required, `approvedAmount > 0` required only when
approving (the reference required it even to reject/defer, which reads
like blanket MVC form validation, not a deliberate rule).

**Real bug fixed in `LoanCaseAppService.ApproveLoanCase`/`Async`
themselves**, same shape as the appraisal fix below: the guard clause
force-set the expected prior status onto the fetched entity before even
null-checking it. Fixed the same way. The identical bug shape is still
unfixed in `AuditLoanCase`/`MarkLoanCaseDisbursed`.

Worth knowing: if the loan product has `LoanRegistrationBypassAudit` set, a
successful Approve auto-chains straight into `AuditLoanCase` in the same
call — the response may already be `Audited`, not `Approved`; the response
`message` says so explicitly.

Full reference: `loan-case-api-spec.md` §9.

### Loan Case API — appraisal added, plus a real NullReferenceException/guard-clause bug fixed

`POST /{id}/appraise`, `GET /{id}/appraisal-worksheet`, and
`GET /{id}/appraisal-factors` added onto the existing `LoanCaseController`
— appraisal as lifecycle actions on the same resource, not a separate
controller (the reference app's `AppraiseLoanController` is a different
screen, but this repo's convention is one controller per resource). The
worksheet endpoint reproduces the real, computable part of the reference
`GET Appraise` action (maximum loan via investments multiplier, outstanding
balance, maximum entitled, amortization `PMT`); a composite standing-
orders/payouts/loan-applications view-model and one literally-empty
`foreach { }` loop in that same reference action were not reproduced.

**Real bug fixed in `LoanCaseAppService.AppraiseLoanCase`/`Async`
themselves**, same shape as the Inter Account Transfer force-set-before-check
fix: the guard clause force-set the expected prior status onto the fetched
entity *before even null-checking it* — appraising a nonexistent loan case
id threw a raw `NullReferenceException` instead of a clean `404`, and the
"must be Registered or Deferred" precondition was tautologically always
true. Fixed. The identical bug shape is still unfixed in
`ApproveLoanCase`/`AuditLoanCase`/`MarkLoanCaseDisbursed` — flagged for
whoever builds those next.

Full reference: `loan-case-api-spec.md` §7-8.

### Loan Case API — new, first controller in the Back Office / loan origination pipeline

`LoanCaseController` (`api/backoffice/loancases`) — loan case registration:
CRUD reads, a guarantor eligibility lookup, and a `Create` that registers a
case with its guarantors and collateral in one call. This is the intake
stage only — appraisal, approval, and audit/verification are separate,
not-yet-built stages; see `WebApplication1/Areas/BackOffice/WORKFLOW.md`
for the whole module's design and current status.

Unlike the Batch Procedures module, `ILoanCaseAppService.AddNewLoanCase`
itself enforces almost none of the real business rules — the reference MVC
controller's session-driven wizard owned the ~40-field
loan-product-at-registration-time snapshot, guarantor
count/self-guarantee/share-sufficiency checks, and the
minimum-membership-period gate, so all of that had to be reproduced in this
controller rather than assumed to already exist server-side.

Two things fixed rather than ported forward:
- **Real bug in `LoanCaseAppService.UpdateLoanCaseAsync` itself**: two
  lines re-stamped `persisted.CreatedDate` to `DateTime.UtcNow` immediately
  after the method had already restored it, and unconditionally stamped
  `CancelledBy` on every plain update, not just cancellations — both
  contradicted the method's own preceding comment and were removed.
- The reference `Create` action called `loanCaseDTO.ValidateAll()` but
  never checked `HasErrors`, silently discarding every `CustomValidation`
  rule on the DTO (amount-applied range, retirement age, security
  sufficiency). This controller checks it and returns a real `400`.

Guarantor share values (`totalShares`/`committedShares`/`appraisalFactor`)
are computed server-side, not trusted from the request body — same
reasoning as the Inter Account Transfer `availableBalance` fix below.

Full reference: `loan-case-api-spec.md`.

### Batch Procedures API — Inter Account Transfer added, module complete (ninth of nine types), plus a real control-bypass bug fixed

`InterAccountTransferBatchController` (`api/accounts/interaccounttransferbatches`)
— new, and the last type in the "Batch Procedures" module. One source
customer account transfers its balance out to however many entries you
attach, each targeting either a customer account or a raw G/L account
(`apportionTo`) — genuinely consulted server-side, unlike Voucher's
lookalike-but-dead per-entry fields.

**Real bug found and fixed — this one matters more than the others**:
`AuthorizeInterAccountTransferBatch` force-set the batch's status to
`Audited` *before* checking it was already `Audited` (and before
null-checking the fetched entity). That made the "must be Audited first"
guard tautologically always true — a batch could be authorized, its
journals posted and real money moved, straight from `Pending`, completely
skipping the maker-checker Audit step this whole module exists to enforce.
Fixed to check first, matching every sibling type's pattern.

Also flagged (not fixed — out of scope, needs real business logic, not a
one-line fix): **no control-total validation exists anywhere for this
type**. `availableBalance` has no backing column and was only ever a
client-side display value in the reference app; nothing server-side stops
an entry's `principal + interest` from exceeding what the source account
can actually cover.

Posting is synchronous on `Authorize` (like Refund/Voucher/General Ledger)
but structurally distinct: each entry gets its own call to the shared
`IJournalAppService.AddNewJournal` entry point (not `BulkSave`), and any
attached `DynamicCharge`s (`PUT /{id}/dynamiccharges`) are real transfer-fee
tariffs fed into that call, not decorative.

**This completes all nine Batch Procedures types.** Full detail:
`batch-procedures-api-spec.md` §9; functional overview across the whole
module: `WebApplication1/Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md`.

### Batch Procedures API — General Ledger added (eighth of nine types, Group B complete)

`GeneralLedgerController` (`api/accounts/generalledgers`) — new, and the
second half of Group B. Not to be confused with
`GeneralLedgerStatementController` (`api/accounts/statements/gl-account`)
— that's a read-only report, this is the maker-checker-authorizer batch
that actually posts entries.

Verified directly against `AuthorizeGeneralLedger` before building against
it, same discipline the Voucher correction established — the original
"each entry is a self-contained double-entry transfer" read held up this
time, plus one detail the DTO alone wouldn't reveal: **every entry posts as
its own separate `Journal`**, not shared legs on one journal the way
Voucher works. The header carries no account fields of its own at all —
unlike Voucher, there's no "primary" account, just a container for entries
that are each already self-balancing.

One asymmetry worth knowing if you're calling `Authorize` directly:
**out-of-balance posts throw a server-side exception here instead of
quietly returning `false`** like every sibling type — the controller
catches it and returns the normal `409` shape, so nothing extra needed
client-side, but worth knowing this is the one type where that path is a
thrown exception under the hood.

This completes Group B. Full detail: `batch-procedures-api-spec.md` §8.
Only Inter Account Transfer remains of the original nine.

### Batch Procedures API — Journal Voucher added (seventh of nine types, Group B started), plus a corrected mental model

`JournalVoucherController` (`api/accounts/journalvouchers`) — new, and the
first of "Group B" (Voucher, General Ledger). Building it against the real
`AuthorizeJournalVoucher` code turned up something worth flagging loudly:
the earlier "settled" writeup in `BATCH-PROCEDURES-CONCEPTS.md` §5
(written when Voucher vs. General Ledger redundancy was first checked)
described Voucher as a free-form N-line journal where each line
independently picks debit or credit — inferred from `JournalVoucherEntryDTO`
carrying its own `type`/`entryType` fields. **That was wrong.** Those
per-entry fields are never read anywhere in `JournalVoucherAppService`. The
real shape: one **primary** account (the header, at `totalValue`) on one
side, and however many **entries** — each its own account and amount —
collectively on the other side; the header's single `type` sets direction
for the header leg *and* every entry leg at once. §5 has been corrected
with the verified version and a note on the lesson: a DTO's fields aren't
proof of behavior here — check the app service directly.

Also **fixed, not just documented**: `AddNewJournalVoucher`'s out-of-range
`valueDate` guard set `ErrorMessageResult` via a `string.Format` call
missing its `{0}` placeholder, so it always returned the literal text
`"ValueDate"` instead of the real error message — fixed to assign the
message directly. And: `IJournalVoucherAppService` has two genuinely
identical bulk-entry-replace methods (`UpdateJournalVoucherEntryCollection`
and `UpdateJournalVoucherEntries`) — only the former is exposed
(`PUT /{id}/entries`), matching what the reference controller called; nothing
is missing by not exposing the duplicate.

Same posting-timing note as Refund: `Authorize` posts synchronously, inline,
no async broker dispatch — safe to assume `Posted` immediately after it
succeeds. Full detail: `batch-procedures-api-spec.md` §7.

### Batch Procedures API — Loan Disbursement added (sixth of nine types)

`LoanDisbursementBatchController` (`api/accounts/loandisbursementbatches`)
— new, and the deepest per-entry posting logic in this module so far. An
entry picks an already-Audited, not-yet-batched `LoanCase`; posting one
(async, off a message queue after `Authorize`, same shape as Debit/Wire
Transfer/Reversal) resolves or creates the customer's loan and savings
accounts, posts the disbursement journal, recovers any upfront dynamic
charges on the loan product, marks the loan case `Disbursed` for real, and
creates/updates a `StandingOrder` for the repayment schedule.

**Two things in the reference app deliberately not ported**: all three
reference MVC controllers hand-roll raw SQL directly against
`swiftFin_LoanCases` to stamp batch numbers, bypassing the domain layer —
not needed, the real app service already does this correctly. More
importantly, the reference `Authorize` action loops every entry **in the
MVC controller itself** afterward and sends an SMS, calls an MPESA B2C
helper with a phone number that's declared but never assigned (always
`""`), and flips a local in-memory DTO's status that's never saved — none
of that is real or trustworthy, and none of it is reproduced here. If
SMS/MPESA notification on disbursement is actually wanted, that needs a
real implementation and a product decision, not a port of dead code.

Also flagged: `batchTotal`/`startDate`/`endDate` on the DTO have no backing
column at all (nothing to fix, unlike Journal Reversal Batch's `Remarks2`
bug); `DisburseMicroLoan` (a separate real-time/alternate-channel
disbursement path) and CSV import (doesn't exist on this interface) are
both out of scope.

Full detail: `batch-procedures-api-spec.md` §6.

### Batch Procedures API — Refund added (fifth of nine types, Group A complete)

`OverDeductionBatchController` (`api/accounts/overdeductionbatches`) — new,
and the last of the five structurally-similar-to-Credit types (Debit, Wire
Transfer, Reversal, Refund). Refunds a prior over-collection back to the
affected member; an entry pairs a debit `CustomerAccount` and a credit
`CustomerAccount` plus `principal`/`interest` — both real, trustworthy
amount fields, unlike Credit/Debit's dead or computed-only equivalents.

**The one thing worth knowing before building against this**:
`AuthorizeOverDeductionBatch` posts every entry's journal(s)
**synchronously, inline, in the same call** — the only type in this module
where that's true. Every other type built so far (Credit's Payout/CheckOff,
Debit, Wire Transfer, Journal Reversal) queues entries onto an async
message broker and posts them later, out of band; Refund does not. It's
safe to assume every entry is `Posted` immediately after `Authorize`
returns `success: true` here — the opposite assumption from every sibling.
Consistent with that, there's no `PostEntry`/queueable/single-entry-lookup
surface at all on this app service — nothing is ever left half-posted to
browse or retry.

Also note: `Update`'s boolean return here means "entries now sum to
exactly `totalValue`", not "the save succeeded" — a `false`/non-error
response just means the batch isn't balanced yet.

Full detail: `batch-procedures-api-spec.md` §5. This completes "Group A"
(Credit, Debit, Wire Transfer, Reversal, Refund) — the five types
structurally closest to Credit. Remaining: Disbursement, Voucher, General
Ledger, Inter Account Transfer.

### Batch Procedures API — Journal Reversal added (fourth of nine types), plus a real bug fix

`JournalReversalBatchController` (`api/accounts/journalreversalbatches`) —
new. Only one reference controller exists for this type
(`BatchOrigination_Reversal`, largely copy-pasted from Disbursement/Wire
Transfer with the copy-paste leftovers still commented out in source, and
missing) — Verification/Authorization dead nav links notwithstanding, that
one controller already covers Create/Verify/Authorize, folded into this one
controller same as everywhere else. It also has **no entry-adding UI at
all**, so the entry shape came from reading `JournalReversalBatchAppService`
directly: an entry is just `{ journalId, remarks }` — pick an
already-posted `Journal` and reverse it; no amount, no tariffs.

**Real bug found and fixed, not just ported forward**:
`JournalReversalBatchAppService.UpdateJournalReversalBatch` fetched the
persisted batch and saved without ever copying the incoming DTO's fields
onto it — every `PUT` silently did nothing while reporting `success: true`.
Fixed to copy `remarks`/`priority`, matching every sibling `Update*Batch`
method in this module. Also flagged (not fixed — no backing column exists
to fix it against): `remarks2` on the DTO is `[Required]` by validation but
is a dead field, never persisted.

Full detail: `batch-procedures-api-spec.md` §4.

### Batch Procedures API — Wire Transfer added (third of nine types)

`WireTransferBatchController` (`api/accounts/wiretransferbatches`) — new.
No plain unified reference controller existed for this type (only the
three-way `BatchOrigination_WireTransfer`/`BatchVerification_WireTransfer`/
`BatchAuthorization_WireTransfer` split), folded into one controller here
same as every other type in this module. Blends traits from both Credit and
Debit: has a real `TotalValue` control-total (like Credit) but strictly
requires `Audited` before `Authorize` and always queues every entry on
authorize with no type carve-out (like Debit). One new thing worth flagging
if you're building against it: `POST /entries/{entryId}/post` posts a real
GL journal (debits the customer, credits the wire-transfer-type's clearing
account) but **does not call any external MPESA/EFT gateway** despite the
type naming — `thirdPartyResponse` on the entry DTO is never populated.
If a customer's balance can't cover an entry, it's auto-rejected outright
rather than partially processed. Full detail: `batch-procedures-api-spec.md` §3.

### Batch Procedures API — new, first two of nine types (Credit, Debit)

The reference app's "Batch Procedures" menu (Origination/Verification/
Authorization × nine types) is a much bigger module than any single
controller pass so far — see
`WebApplication1/Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md` for the
functional basis (a maker-checker-authorizer control on bulk GL postings)
and per-type purpose, and `batch-procedures-api-spec.md` for the route
reference. Each type is one unified controller covering all three stages
via `/{id}/audit` and `/{id}/authorize` — the reference app's three-way
split per type is a menu/role-routing artifact, not a reason for three
controllers each.

- **`CreditBatchController`** (`api/accounts/creditbatches`) — already
  existed (built to unblock the FrontOffice Cash Pickup picker, see the
  Front Office entry below); now cross-referenced from here too.
- **`DebitBatchController`** (`api/accounts/debitbatches`) — new. Real
  differences from Credit worth knowing if you're building against it:
  no `TotalValue` control-total anywhere, `Authorize` genuinely refuses a
  batch that isn't already `Audited` (Credit's equivalent guard is
  commented out in source), entries have no amount-shaped field you can
  trust before posting (`multiplier`/`basisValue` feed a server-side tariff
  computation, capped against available balance), and posting is always
  async off a message queue once authorized, with no per-type carve-out
  the way Credit's Cash Pickup has. Full detail: `batch-procedures-api-spec.md` §2.

Remaining seven types (Refund, Wire Transfer, Disbursement, Reversal,
Voucher, General Ledger, Inter Account Transfer) not started — their app
services all already exist and are Unity-registered, so this is
controller-adaptation work only, no backend gaps to fill. See the concepts
doc for what each is for and the settled Voucher-vs-General-Ledger
distinction.

### Electronic Statement Order API — new (split into two controllers, same reasoning as Standing Orders)

`ElectronicStatementOrderController` (`api/accounts/electronicstatementorders`)
and `ElectronicStatementOrderExecutionController`
(`.../electronicstatementorders/execution`) documented and built for the
first time — `IElectronicStatementOrderAppService` was already fully built
but had no controller anywhere, only reachable through the legacy
`ElectronicStatementOrderService.svc.cs` WCF passthrough. This manages a
**subscription** (recurring "email this account a statement on a schedule"),
not statement content — it has no overlap with the already-shipped
`CustomerAccountStatementController` (`docs/api/customer-account-statement-api-spec.md`),
confirmed by reading both the reference `CoA_eStatementsController` and the
app service; they don't share a DTO, an app service, or a single line of
logic despite both having "statement" in the name.

Split into two controllers rather than one, mirroring the existing
`StandingOrderController`/`StandingOrderExecutionController` split and for
the identical reason: the actual batch-execution capability
(`ExecuteElectronicStatementOrders`) lives on `IRecurringBatchAppService`,
a different app service than the CRUD one. Full reference:
`electronic-statement-order-api-spec.md` /
`electronic-statement-order-execution-api-spec.md`.

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
