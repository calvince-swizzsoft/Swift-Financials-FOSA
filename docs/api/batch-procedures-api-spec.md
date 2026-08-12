# Batch Procedures API

Functional basis, the maker-checker-authorizer lifecycle every type here
shares, and why each type exists:
`WebApplication1/Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md`. Read that
first if you're building a screen against any of these — this doc is just
the route/field reference.

All nine types the reference app's "Batch Procedures" menu covers are now built:

| Type | Controller | Status |
|---|---|---|
| Credit | `CreditBatchController` (`api/accounts/creditbatches`) | Built — §1 |
| Debit | `DebitBatchController` (`api/accounts/debitbatches`) | Built — §2 |
| Wire Transfer | `WireTransferBatchController` (`api/accounts/wiretransferbatches`) | Built — §3 |
| Reversal | `JournalReversalBatchController` (`api/accounts/journalreversalbatches`) | Built — §4 |
| Refund | `OverDeductionBatchController` (`api/accounts/overdeductionbatches`) | Built — §5 |
| Disbursement | `LoanDisbursementBatchController` (`api/accounts/loandisbursementbatches`) | Built — §6 |
| Voucher | `JournalVoucherController` (`api/accounts/journalvouchers`) | Built — §7 |
| General Ledger | `GeneralLedgerController` (`api/accounts/generalledgers`) | Built — §8 |
| Inter Account Transfer | `InterAccountTransferBatchController` (`api/accounts/interaccounttransferbatches`) | Built — §9 |

CSV batch import (`ParseXImport` on the app service, where it exists) is
deliberately not exposed on any of these — no controller in this project
has a file-upload pattern yet; it's one shared initiative later rather than
several ad hoc ones now.

---

## 1. Credit Batch — `api/accounts/creditbatches`

Controller: `CreditBatchController.cs`, existing `ICreditBatchAppService`.
`CreditBatchType`: `0xDADA`=Payout, `+1`=CheckOff, `+2`=CashPickup,
`+3`=SundryPayments (see concepts doc §2 for what each means).

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending` |
| `/{id}` | PUT | Update batch's own fields — does not touch entries |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`, only if entries total ≤ batch `TotalValue`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; for `Payout`/`CheckOff` batches this also queues every entry for async GL posting — see §1.1), `2`=Reject. **Not gated on the batch already being `Audited`** — the guard exists in the reference app's source but is commented out, so this will authorize a still-`Pending` batch if you call it out of order |
| `/{id}/entries?text=&filter=&pageIndex=&pageSize=` | GET | Entries within one batch |
| `/entries/type/{creditBatchType}?startDate=&endDate=&text=&filter=&pageIndex=&pageSize=` | GET | Entries across all batches of a `CreditBatchType` — the Cash Pickup picker uses `creditBatchType=8`, see `frontoffice-api-spec.md` §13.3 |
| `/entries/customer/{customerId}?creditBatchType=` | GET | Entries for one customer (`Payout`/`CheckOff` — entries there are tied to a customer account, unlike Cash Pickup) |
| `/entries/{entryId}` | GET | Single entry |
| `/{id}/entries` | POST | Add an entry to a batch |
| `/entries/{entryId}` | PUT | Update an entry (status is forward-only: `Pending → Posted/Rejected`) |
| `/entries/remove` | POST | Batch-remove entries (`List<CreditBatchEntryDTO>`) |
| `/entries/{entryId}/post` | POST | `{ moduleNavigationItemCode }` — marks one entry `Posted`. For `CashPickup`/`SundryPayments` this **only** flips status, no GL journal — the journal for those two types is posted by `SundryPaymentsController` (`frontoffice-api-spec.md` §13.1), which calls this endpoint itself right after |

### 1.1 Posting timing — synchronous vs. async

`Authorize` with `option: 1` on a `Payout`/`CheckOff` batch does **not**
post journals synchronously as part of that call. It enqueues every entry
onto an async message queue (`BrokerService.ProcessCreditBatchEntries`);
a separate background consumer processes them, eventually calling
`PostCreditBatchEntry` per entry. Don't assume a batch's entries are
`Posted` immediately after `Authorize` returns `success: true` — poll
`GET /{id}/entries` if the UI needs to reflect real posting status.

`CashPickup`/`SundryPayments` batches are **not** queued this way (the
guard on the async dispatch explicitly excludes them) — those entries stay
`Pending` until a teller picks one and pays it through
`api/frontoffice/sundrypayments`, exactly as documented in §13 of
`frontoffice-api-spec.md`.

**Known gap**: `entry.amount` on `CreditBatchEntryDTO` is never populated —
`CreditBatchEntry` has no `Amount` column on the domain entity, so the
AutoMapper projection always leaves it `0`. Use `principal + interest`
instead.

---

## 2. Debit Batch — `api/accounts/debitbatches`

Controller: `DebitBatchController.cs`, existing `IDebitBatchAppService`.
No sub-type enum — every Debit batch is the same shape (unlike Credit's
four `CreditBatchType`s).

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending` |
| `/{id}` | PUT | Update batch's own fields |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; queues every entry for async posting — see §2.1), `2`=Reject. **Unlike Credit, this genuinely refuses if the batch isn't already `Audited`** — the guard is live here, not commented out |
| `/{id}/entries?text=&pageIndex=&pageSize=` | GET | Entries within one batch (no `filter` param — Credit's equivalent has one, Debit's doesn't) |
| `/entries/queueable?pageIndex=&pageSize=` | GET | Entries ready to post, across all batches — no type restriction (nothing to filter on) |
| `/entries/customer/{customerId}` | GET | Entries for one customer |
| `/{id}/entries` | POST | Add an entry to a batch |
| `/entries/remove` | POST | Batch-remove entries (`List<DebitBatchEntryDTO>`) |
| `/entries/{entryId}/post` | POST | `{ moduleNavigationItemCode }` — computes and posts the entry's actual deduction (see §2.2) |

Deliberately absent, unlike Credit's entry sub-resource: no
`GET /entries/{entryId}` (single-entry lookup) and no `PUT /entries/{entryId}`
(status update) — `IDebitBatchAppService` doesn't expose either method.
`POST .../post`'s response has no entry payload to return for the same
reason; re-browse `GET /{id}/entries` or `GET /entries/queueable` to see an
entry's new status.

### 2.1 Posting timing — always async, no type filter

`Authorize` with `option: 1` queues **every** entry in the batch onto an
async message queue (`BrokerService.ProcessDebitBatchEntries` →
`DebitBatchPostingQueuePath`) regardless of anything about the batch —
there's no `Payout`/`CheckOff`-style carve-out here since Debit has no
sub-type. A separate background consumer is expected to call
`POST /entries/{entryId}/post` per entry. As with Credit, don't assume
`Posted` immediately after `Authorize` succeeds.

`POST /entries/{entryId}/post` is exposed both as what that consumer calls
and as a manual retry for any entry that didn't post — there is no teller
"pick one and pay it" screen for Debit the way Cash Pickup has for Credit;
normal processing is meant to be fully automatic once a batch is
authorized.

### 2.2 There is no pre-computed entry amount — at all

Unlike Credit (where the amount is at least readable off `principal +
interest` once you know to skip the broken `amount` field), `DebitBatchEntryDTO`
has **no amount-shaped field you can trust before posting**. Entries carry
`multiplier` and `basisValue` instead; the real deduction — possibly
several separate tariff-line journals — is computed only inside
`PostDebitBatchEntry`, via the entry's `DebitType`'s tariff structure
(`ComputeTariffsByDebitType`), and is capped against the customer's
available balance unless the branch's company record has
`allowDebitBatchToOverdrawAccount` set. **Don't build a UI that shows "this
entry will deduct X" before it's posted** — there is no X to show; only
`multiplier`/`basisValue` (the *inputs* to the calculation) are known
up front.

There is also no `TotalValue` field on `DebitBatchDTO` at all, so unlike
every other batch type in this module, there is no "entries total must not
exceed the batch's declared total" control-total check anywhere in
`DebitBatchAppService` — nothing to validate client-side there either.

---

## 3. Wire Transfer Batch — `api/accounts/wiretransferbatches`

Controller: `WireTransferBatchController.cs`, existing
`IWireTransferBatchAppService`. `WireTransferBatchType` on the header:
`0`=MPESA B2C, `1`=MPESA B2B, `2`=EFT — not used to gate anything
server-side, just a label.

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list. **`status` is required** — there is no status-less paged overload on this app service, unlike Credit/Debit |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending` |
| `/{id}` | PUT | Update batch's own fields (`totalValue`, `reference`, `priority`) |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`, only if entries total ≤ `TotalValue`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; queues every entry for async posting — see §3.1), `2`=Reject. **Refuses outright if the batch isn't already `Audited`** |
| `/{id}/entries?text=&pageIndex=&pageSize=` | GET | Entries within one batch (no `filter` param) |
| `/entries/queueable?pageIndex=&pageSize=` | GET | Entries ready to post, across all batches — no type restriction |
| `/entries/{entryId}` | GET | Single entry |
| `/{id}/entries` | POST | Add an entry to a batch |
| `/entries/{entryId}` | PUT | Update an entry (status is forward-only: `Pending → Posted/Rejected`) |
| `/entries/remove` | POST | Batch-remove entries (`List<WireTransferBatchEntryDTO>`) |
| `/entries/{entryId}/post` | POST | `{ moduleNavigationItemCode }` — posts the entry's GL journal, or auto-rejects it — see §3.2 |

No customer-scoped entry browse (`.../entries/customer/{customerId}`)
exists for this type, unlike Credit/Debit — `IWireTransferBatchAppService`
doesn't expose one.

### 3.1 Posting timing — always async, no type filter

Same shape as Debit: `Authorize` with `option: 1` queues **every** entry
onto an async message queue (`BrokerService.ProcessWireTransferBatchEntries`
→ `WireTransferBatchPostingQueuePath`) regardless of `WireTransferBatchType`.
Don't assume `Posted` immediately after `Authorize` succeeds.

### 3.2 What posting an entry actually does — and doesn't do

Unlike Debit, `WireTransferBatchEntryDTO.amount` **is** a real, trustworthy,
client-supplied figure — no tariff-basis computation needed to know an
entry's value up front. `POST /entries/{entryId}/post` debits the
customer's product account for `amount` plus the wire-transfer-type's own
tariffs, and credits the `WireTransferType`'s G/L account (a
suspense/clearing account) — one journal, same shape as Credit's Payout
posting.

**Despite the MPESA B2C/B2B/EFT type naming, no external gateway call
happens anywhere in this flow.** `WireTransferBatchEntryDTO.thirdPartyResponse`
exists on the DTO but is never set by `WireTransferBatchAppService` —
posting here only moves the money to a clearing G/L account internally;
actually dispatching to Mpesa or an EFT/SWIFT network, if required, is a
separate integration this API does not perform.

If the customer's available balance can't cover `amount + tariffs`, the
entry is **auto-rejected outright** — a different failure mode from Debit,
which caps and partially deducts instead of rejecting.

---

## 4. Journal Reversal Batch — `api/accounts/journalreversalbatches`

Controller: `JournalReversalBatchController.cs`, existing
`IJournalReversalBatchAppService`. Pick one or more already-posted
`Journal`s and reverse them in a batch under the same three-stage control
as every other type. An entry is just `{ journalId, remarks }` — no amount
field, no tariffs; the amount reversed is implicitly the referenced
journal's own amount.

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list. **`status` is required**, same as Wire Transfer |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending` |
| `/{id}` | PUT | Update batch's `remarks`/`priority` (see §4.1 — this used to silently do nothing) |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; queues every entry for async posting), `2`=Reject. **Refuses outright if the batch isn't already `Audited`** |
| `/{id}/entries?text=&pageIndex=&pageSize=` | GET | Batch entries (the `{ journalId, remarks }` picks) |
| `/{id}/journal-entries?text=&pageIndex=&pageSize=` | GET | The actual G/L lines (`JournalEntryDTO`, not batch entries) across every `Journal` this batch's entries reference — a "here's exactly what will be reversed" preview |
| `/entries/queueable?pageIndex=&pageSize=` | GET | Entries ready to post, across all batches — no type restriction |
| `/entries/{entryId}` | GET | Single entry |
| `/{id}/entries` | POST | Pick a single `Journal` to reverse and attach it to this batch |
| `/{id}/entries/bulk` | POST | `List<JournalReversalBatchEntryDTO>` — bulk-add convenience. **Insert-only**: diffs the list against the batch's existing entries by `journalId` and inserts whatever's new; entries missing from the list are *not* removed despite the underlying method's name (`UpdateJournalReversalBatchEntries`) suggesting a full replace |
| `/entries/remove` | POST | Batch-remove entries (`List<JournalReversalBatchEntryDTO>`) |
| `/entries/{entryId}/post` | POST | `{ moduleNavigationItemCode }` — reverses the entry's referenced journal via the existing `IJournalAppService.ReverseJournals`; no balance checks, no partial processing, no rejection path — it either reverses cleanly or the call fails |

No CSV import exists for this type at all (`ParseJournalReversalBatchImport`
isn't on the interface, unlike Credit/Debit/Wire Transfer) — nothing was
excluded here, there was never anything to expose.

### 4.1 `Remarks2` is a dead field, and `Update` used to be a no-op

`JournalReversalBatchDTO.remarks2` is `[Required]` by validation but has
**no backing column** on the `JournalReversalBatch` domain entity — send
something to pass `ValidateAll()`, but don't expect it to persist or come
back on a subsequent `GET`.

**Fixed, not just documented**: `JournalReversalBatchAppService.UpdateJournalReversalBatch`
used to fetch the persisted batch and call `SaveChanges` without ever
copying any of the incoming DTO's fields onto it — a silent no-op that
would return `success: true` while changing nothing. Every sibling
`Update*Batch` method in this module copies its editable fields first; this
one just didn't. Fixed to copy `remarks`/`priority` (not `branchId` —
treated as immutable post-creation, same as every sibling).

### 4.2 Posting timing and mechanics

Same async-queue-everything shape as Debit/Wire Transfer: `Authorize` with
`option: 1` queues every entry onto a message queue
(`BrokerService.ProcessJournalReversalBatchEntries` →
a dedicated posting queue path) with no type filter (there's no sub-type
here to filter on). Don't assume `Posted` immediately after `Authorize`
succeeds.

`POST /entries/{entryId}/post` is the simplest posting mechanic in this
whole module — no tariffs, no balance checks, no partial-processing or
auto-reject branch. It resolves the entry's `journalId` to a `JournalDTO`
and calls `IJournalAppService.ReverseJournals` on it. If that journal can't
be resolved, the call just fails (`409`); there's no other failure mode to
plan a UI around.

---

## 5. Refund (Over Deduction) Batch — `api/accounts/overdeductionbatches`

Controller: `OverDeductionBatchController.cs`, existing
`IOverDeductionBatchAppService`. Refunds a prior over-collection (from a
Credit/Debit/CheckOff run) back to the affected member. An entry pairs a
debit side and a credit side — both real `CustomerAccount`s — plus
`principal`/`interest`, the amount to move. Both amount fields are real and
trustworthy, unlike Credit/Debit's dead/computed-only equivalents.

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list. **`status` is required**, same as Wire Transfer/Reversal |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending` |
| `/{id}` | PUT | Update `totalValue`. Response message flags whether entries now sum to it exactly — see §5.1 |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; **posts every entry's journal(s) synchronously, in this same call** — see §5.2), `2`=Reject. Refuses outright if the batch isn't already `Audited` |
| `/{id}/entries?text=&pageIndex=&pageSize=` | GET | Entries within one batch |
| `/{id}/entries` | POST | Add an entry to a batch |
| `/entries/remove` | POST | Batch-remove entries (`List<OverDeductionBatchEntryDTO>`) |

That's the full route surface — no `entries/{entryId}`, no
`entries/queueable`, no `entries/{entryId}/post`. `IOverDeductionBatchAppService`
doesn't expose any of them, and §5.2 explains why there's nothing to browse
or post individually here.

### 5.1 `Update`'s return value means "balanced", not "succeeded"

`UpdateOverDeductionBatch` really does copy `totalValue` onto the persisted
batch (unlike Journal Reversal Batch's bug, already fixed) — but its
boolean return is `persisted.TotalValue == sum(entries.principal + entries.interest)`,
an **exact equality** check, not Credit's "does not exceed". Getting back
`false` from this endpoint doesn't mean the save failed; it means the batch
isn't balanced yet (entries still need adding/adjusting before it can be
audited). The controller reflects this in the response `message` rather
than treating it as an error.

### 5.2 The only type in this module where Authorize posts synchronously

Every other type built so far (Credit's Payout/CheckOff, Debit,
Wire Transfer, Journal Reversal) queues its entries onto an async message
broker on `Authorize` and posts them later, out of band.
**`AuthorizeOverDeductionBatch` does not** — it loops every entry inline,
in the same call, and posts real journals via `BulkSave` before returning.
It is safe (and correct) to treat every entry as `Posted` immediately after
`Authorize` returns `success: true` here — the opposite assumption from
every sibling type.

What gets posted per entry, for context (not something a client needs to
compute — this is server-side only):
- **Savings/Investment** debit accounts: one journal moving `principal +
  interest` from the debit account's product G/L to the credit account's.
- **Loan** debit accounts: three separate journals — an interest
  receivable entry, an interest received/charged reversal, and a principal
  entry — to properly unwind a loan repayment's interest recognition
  rather than just moving a lump sum.

---

## 6. Loan Disbursement Batch — `api/accounts/loandisbursementbatches`

Controller: `LoanDisbursementBatchController.cs`, existing
`ILoanDisbursementBatchAppService` — the one app service in this module
that lives in `BackOfficeModule`, not `AccountsModule` (the controller
still sits under `Areas/Accounts`, matching where the reference screens
live). An entry is `{ loanDisbursementBatchId, loanCaseId, reference }` —
pick an already-**Audited**, not-yet-batched `LoanCase` (browse those via
the existing loan-case endpoints, filtered client-side to
`LoanCaseStatus.Audited && !isBatched`) and attach it.

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list. **`status` is required**, same as Wire Transfer/Reversal/Refund |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending` |
| `/{id}` | PUT | Update `reference`/`priority` only — `branchId`/`type`/`loanProductCategory` are immutable post-creation |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; queues every entry for async posting — see §6.2), `2`=Reject. Refuses outright if the batch isn't already `Audited` |
| `/{id}/exceeds-threshold?designationId=&transactionThresholdType=` | GET | Whether any entry in the batch exceeds the transaction threshold configured for a `Designation` (role) — an AML/compliance pre-check, not a hard gate elsewhere in this flow |
| `/{id}/entries?text=&pageIndex=&pageSize=` | GET | Entries within one batch |
| `/entries/type/{disbursementType}?startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Entries across all batches of a `DisbursementType` (`1`=Normal, `2`=Express, `4`=Waiver) |
| `/entries/customer/{customerId}?disbursementType=` | GET | Entries for one customer (`disbursementType` required) |
| `/entries/queueable?pageIndex=&pageSize=` | GET | Entries ready to post, across all batches — no type restriction |
| `/entries/{entryId}` | GET | Single entry |
| `/{id}/entries` | POST | Pick a single `LoanCase` and attach it to this batch. Refuses (server error) if that loan case is already batched elsewhere |
| `/{id}/entries/bulk` | POST | `List<LoanDisbursementBatchEntryDTO>` — bulk-add convenience, **insert-only** (same caveat as Journal Reversal Batch's equivalent — entries missing from the list you send are not removed). Also silently skips any `LoanCase` already batched, or whose `loanProductCategory` doesn't match this batch's own |
| `/entries/{entryId}` | PUT | Update an entry (status is forward-only: `Pending → Posted/Rejected`) |
| `/entries/remove` | POST | Batch-remove entries (`List<LoanDisbursementBatchEntryDTO>`). Also un-flags the underlying `LoanCase` (`isBatched=false`, `batchNumber=0`) so it becomes eligible for a different batch again |
| `/entries/{entryId}/post` | POST | `{ moduleNavigationItemCode }` — disburses the entry, see §6.3 |

### 6.1 Two things in the reference app deliberately not ported

All three reference MVC controllers (`BatchOrigination_Disbursement`,
`BatchVerification_Disbursement`, `BatchAuthorization_Disbursement`)
hand-roll raw ADO.NET SQL directly against the `swiftFin_LoanCases` table
to stamp a loan case's batch number, bypassing the domain layer entirely.
Not needed here — `AddNewLoanDisbursementBatchEntry`/
`UpdateLoanDisbursementBatchEntries` already do this correctly through the
domain layer (flip `LoanCase.IsBatched`/`BatchNumber`/`BatchedBy`, and
refuse a loan case that's already batched).

More importantly: `BatchAuthorization_Disbursement`'s `Authorize` action,
after calling the real authorize, loops every entry **in the MVC
controller itself** and (a) sends an SMS, (b) calls an MPESA B2C helper
with a phone-number variable that is declared, never assigned, and passed
in as `""`, and (c) sets `.Status = Disbursed` on a local in-memory DTO
that's never saved anywhere. None of that is real or trustworthy — it's
dead/broken presentation-layer code, not domain logic, and it is **not**
reproduced here. Real disbursement work happens entirely server-side (see
§6.3). If SMS/MPESA notification on disbursement is actually wanted, that
needs a real implementation and a product decision — not a port of this.

### 6.2 Posting timing — always async, no type filter

Same shape as Debit/Wire Transfer/Reversal: `Authorize` with `option: 1`
queues every entry onto an async message queue
(`BrokerService.ProcessLoanDisbursementBatchEntries`) regardless of
`DisbursementType`. Don't assume `Posted` immediately after `Authorize`
succeeds.

### 6.3 What posting one entry actually does

This is the most substantial per-entry posting logic in the whole module —
treat it as a black box, not something to precompute or second-guess
client-side:

1. Resolves the customer's loan account and savings account, **creating
   either one if it doesn't already exist**.
2. Posts the disbursement journal — approved principal moves from the loan
   account to the savings account.
3. Recovers any upfront dynamic charges configured on the loan product
   (`ComputeTariffsByLoanProduct`, recovery source `LoanAccount`, mode
   `Upfront`) as additional journal lines against the loan account.
4. Marks the `LoanCase` `Disbursed` (for real, through
   `ILoanCaseAppService.MarkLoanCaseDisbursed` — unlike the reference
   controller's dead in-memory status flip).
5. Computes the repayment schedule and creates (or updates, if one already
   exists between the same two accounts) a `StandingOrder` that will
   collect the loan's periodic payments going forward.

### 6.4 Two dead fields, and one thing deliberately not exposed

`LoanDisbursementBatchDTO.batchTotal`, `.startDate`, and `.endDate` have
**no backing column** on the `LoanDisbursementBatch` domain entity at all —
send whatever you like to satisfy the client-side form, but don't expect
any of them to persist or come back on a `GET`. Unlike Journal Reversal
Batch's `Remarks2`, there's no fix to apply here — there's no column to
copy a value into.

Not exposed: `DisburseMicroLoan` — a separate real-time/alternate-channel
(USSD/API) disbursement path, given away by its `alternateChannelLogId`
parameter. Unrelated to this batch screen; needs its own product decision
if/when an alternate-channel disbursement API is wanted. There is also no
CSV import for this type — `ParseLoanDisbursementBatchImport` doesn't exist
on the interface at all, so nothing was excluded here that could otherwise
have been built.

---

## 7. Journal Voucher — `api/accounts/journalvouchers`

Controller: `JournalVoucherController.cs`, existing
`IJournalVoucherAppService`. First of Group B — see
`BATCH-PROCEDURES-CONCEPTS.md` §5 for the corrected (verified against
`AuthorizeJournalVoucher` directly, not inferred from the DTO) explanation
of what a voucher actually is: **one primary account** (the header's own
`chartOfAccountId` + optional `customerAccountId`, at `totalValue`) on one
side, and however many **entries** — each its own `chartOfAccountId` +
optional `customerAccountId` + own `amount` — collectively on the other
side. The header's single `type` (`JournalVoucherType`: `0`=DebitGLAccount,
`1`=CreditGLAccount, `2`=DebitCustomerAccount, `3`=CreditCustomerAccount)
sets the direction for the header leg *and every entry leg at once* — there
is no per-entry direction control, despite each entry carrying its own
`type`/`entryType` fields (`JournalVoucherEntryType`: `1`=GLAccount,
`2`=Customer). Those two entry-level fields are **never read** anywhere in
`JournalVoucherAppService` — don't build a per-entry debit/credit picker
against them, they're decorative.

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every voucher |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged voucher list. Unlike Wire Transfer/Reversal/Refund/Disbursement, `status` here is **optional** — omit it and supply `startDate`/`endDate` for a date-range search instead, or neither for a plain paged/text list. Four overloads exist server-side; the controller picks the right one for whatever combination you send |
| `/{id}` | GET | Single voucher |
| `/` | POST | Create voucher → `Pending`. Fails `400` if `postingPeriodId` doesn't resolve, or if `valueDate` falls outside that posting period (or is in the future) — the latter comes back with `data`: the voucher DTO you sent, not `null`; check `errorMessageResult` on it (see §7.2) |
| `/{id}` | PUT | Update. Return message reflects whether entries now sum to *exactly* `totalValue` (same "balanced, not success" semantics as Refund's `Update` — see §7.1) |
| `/{id}/audit` | POST | `{ option, remarks }` — `JournalVoucherAuthOption`: `1`=Post (→ `Audited`), `2`=Reject. Only accepts `Pending`. Note this is its own enum, not the shared `BatchAuthOption` the rest of this module uses (same values, different type) |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; **posts synchronously, inline, in this same call** — same as Refund, no async broker dispatch — only if entries sum to exactly `totalValue`), `2`=Reject. Refuses outright if the voucher isn't already `Audited` |
| `/{id}/entries?pageIndex=&pageSize=` | GET | Entries on one voucher (no `text` search param — simpler than most of this module's entry-browse endpoints) |
| `/{id}/entries` | POST | Add a single entry |
| `/{id}/entries` | PUT | **Full replace** — every existing entry is deleted and the given list recreated in its place. Unlike Journal Reversal/Loan Disbursement Batch's insert-only bulk methods, this one really does replace the whole collection (see §7.3 for why the route only exposes one of two identical app-service methods) |
| `/entries/remove` | POST | Batch-remove entries (`List<JournalVoucherEntryDTO>`) |

No `PostEntry`, no queueable browse, no single-entry lookup — a voucher
posts as one atomic unit on `Authorize`; there is nothing to post or browse
individually.

### 7.1 `Update`'s return value means "balanced", not "succeeded"

Same pattern as Refund (§5.1): `UpdateJournalVoucher`'s boolean return is
`persisted.TotalValue == sum(entries.amount)`, an exact equality check.
`false` doesn't mean the save failed — it means the voucher isn't balanced
yet. Reflected in the controller's response `message`, not treated as an
error.

### 7.2 Fixed: a validation error used to come back unreadable

`AddNewJournalVoucher`'s out-of-range `valueDate` guard used to set
`ErrorMessageResult` via `string.Format("ValueDate", "Sorry, but value
date is out of range!")` — with no `{0}` placeholder in the format string,
that call just returned the literal text `"ValueDate"`, silently
discarding the real message. **Fixed** to assign the real message
directly — a client hitting this path now gets the actual explanation
instead of a bare field name.

### 7.3 Two identical bulk-replace methods exist; only one is exposed

`IJournalVoucherAppService` has both `UpdateJournalVoucherEntryCollection`
and `UpdateJournalVoucherEntries` — reading both confirms they are genuine
duplicates, identical delete-then-recreate logic under two different
names (down to calling two separately-duplicated `Find` methods to fetch
the "existing" set first). Only `UpdateJournalVoucherEntryCollection` is
exposed here, via `PUT /{id}/entries` — matching what the reference
controller actually called. No functionality is missing by not exposing
the second one; it does the same thing.

---

## 8. General Ledger — `api/accounts/generalledgers`

Controller: `GeneralLedgerController.cs`, existing `IGeneralLedgerAppService`.
Second and last of Group B. Not to be confused with
`GeneralLedgerStatementController` (`api/accounts/statements/gl-account`,
`general-ledger-statement-api-spec.md`) — that's a read-only reporting view
over `IJournalEntryAppService`; this is the maker-checker-authorizer batch
that actually posts entries.

Each entry is a self-contained double-entry transfer: `chartOfAccountId`
(credit side) + `contraChartOfAccountId` (debit side), each optionally
paired with a `customerAccountId`/`contraCustomerAccountId` if that side is
a real customer account rather than a bare G/L account. Verified directly
against `AuthorizeGeneralLedger` (after the Voucher correction in §7/concepts
doc §5, the DTO shape alone isn't trusted for this module anymore) — this
one holds up as originally read, plus one thing the DTO wouldn't reveal:
**every entry posts as its own separate `Journal`**, not shared legs on one
journal the way Voucher works. `GeneralLedgerDTO` itself carries no
chart-of-account/customer-account fields at all — unlike Voucher, there is
no "primary" header account; the header is purely a container (branch,
posting period, `totalValue`, `remarks`) for entries that are each already
self-balancing.

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every ledger |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged ledger list. `status` is **optional**, same as Journal Voucher — omit it and supply `startDate`/`endDate` for a date-range search, or neither for a plain paged list |
| `/{id}` | GET | Single ledger |
| `/` | POST | Create ledger → `Pending` |
| `/{id}` | PUT | Update. Return message reflects whether entries now sum to *exactly* `totalValue` (same "balanced, not success" semantics as Refund/Journal Voucher) |
| `/{id}/audit` | POST | `{ option, remarks }` — `GeneralLedgerAuthOption`: `1`=Post (→ `Audited`), `2`=Reject. Only accepts `Pending`. Own enum, not the shared `BatchAuthOption` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; **posts synchronously, inline, in this same call** — one `Journal` per entry, no async broker dispatch), `2`=Reject. Refuses outright if the ledger isn't already `Audited`. **Unlike every sibling type**, an out-of-balance Post throws server-side instead of quietly returning `false` — this controller catches that and returns the normal `409` shape, so the client doesn't need to special-case it |
| `/{id}/entries?pageIndex=&pageSize=` | GET | Entries on one ledger (no `text` search param, same as Journal Voucher) |
| `/{id}/entries` | POST | Add a single entry |
| `/{id}/entries` | PUT | Full replace — every existing entry is deleted and the given list recreated in its place. Unlike Journal Voucher, there's only one bulk-replace method here, no duplicate to pick between |
| `/entries/remove` | POST | Batch-remove entries (`List<GeneralLedgerEntryDTO>`) |

No `PostEntry`, no queueable browse, no single-entry lookup — same as
Voucher/Refund, a ledger posts as one atomic unit on `Authorize`.

CSV import (`ParseGeneralLedgerImportEntries`) exists on this interface but
is deliberately not exposed, consistent with the rest of this module — no
controller here has a file-upload pattern yet.

---

## 9. Inter Account Transfer Batch — `api/accounts/interaccounttransferbatches`

Controller: `InterAccountTransferBatchController.cs`, existing
`IInterAccountTransferBatchAppService`. Ninth and last of this module. One
**source** customer account (the header's own `customerAccountId`)
transfers its balance out to however many **entries** you attach, each
targeting *either* a customer account or a raw G/L account — `apportionTo`
(`1`=CustomerAccount, `2`=GeneralLedgerAccount) is genuinely consulted
server-side (`AddNewInterAccountTransferBatchEntry` nulls out whichever of
`chartOfAccountId`/`customerAccountId` doesn't apply), unlike Voucher's
lookalike-but-dead per-entry fields. Each entry carries its own
`principal`+`interest` — the amount moved to it.

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list. `status` is **optional**, same four-overload dispatch as Journal Voucher/General Ledger |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending`. Only `branchId`/`customerAccountId`/`reference` are actually persisted — `availableBalance`, `startDate`/`endDate`, and the denormalized customer fields are display-only (see §9.2) |
| `/{id}` | PUT | Update `reference` only — `branchId`/`customerAccountId` are immutable post-creation |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; posts synchronously, one `Journal` per entry — see §9.3), `2`=Reject. Refuses outright if the batch isn't already `Audited` — see §9.1 for why this guard needed fixing |
| `/{id}/entries?text=&pageIndex=&pageSize=` | GET | Entries within one batch |
| `/{id}/entries` | POST | Add a single entry |
| `/{id}/entries` | PUT | Full replace — every existing entry is deleted and the given list recreated in its place |
| `/entries/remove` | POST | Batch-remove entries (`List<InterAccountTransferBatchEntryDTO>`) |
| `/{id}/dynamiccharges` | GET | Transfer-fee `DynamicCharge`s attached to this batch |
| `/{id}/dynamiccharges` | PUT | Full replace of the attached `DynamicCharge` set (id references only) — real and fed into posting, not decorative |

No `PostEntry`, no queueable browse, no single-entry lookup, no CSV import
(`ParseXImport` doesn't exist on this interface at all) — same shape as
Refund/Voucher/General Ledger.

`InterAccountTransferBatchDTO` has no `Priority` field at all, unlike every
other type in this module.

### 9.1 Fixed: a real control-bypass bug

`AuthorizeInterAccountTransferBatch` used to read:

```csharp
var persisted = _interAccountTransferBatchRepository.Get(interAccountTransferBatchDTO.Id, serviceHeader);
persisted.Status = (int)BatchStatus.Audited;
if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
    return false;
```

Two things wrong with this: `persisted.Status = ...` dereferences before
the null check, so an unknown id threw a `NullReferenceException` instead
of a clean `false`/`404`. Worse, it force-set the status to `Audited`
**before** checking it was already `Audited` — making that guard
tautologically true every time. A batch could be authorized (its journals
posted, real money moved) straight from `Pending`, skipping the Audit step
entirely, or even re-authorized after a prior `Rejected`. **Fixed** to
match every sibling type's pattern — check first, don't mutate before
checking:

```csharp
var persisted = _interAccountTransferBatchRepository.Get(interAccountTransferBatchDTO.Id, serviceHeader);

if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
    return false;
```

### 9.2 No control-total validation exists for this type

`availableBalance` on `InterAccountTransferBatchDTO` has **no backing
column** — in the reference app it was populated client-side only, by
looking up the source account's real balance at the moment the form
loaded, and never re-verified server-side. Unlike every other type in this
module (which all check entries' amount against some declared total before
allowing Audit/Authorize), nothing here stops an entry's `principal +
interest` from exceeding what the source account can actually cover, or
from taking it below its minimum balance. If that validation matters, it
has to happen client-side today — flagged, not fixed, since fixing it
means adding real balance-checking business logic, out of scope for a
controller-adaptation pass.

### 9.3 Posting mechanics

Same synchronous-on-Authorize shape as Refund/Voucher/General Ledger (no
async broker dispatch) but structurally different from both: each entry
gets its own call to `IJournalAppService.AddNewJournal` — the same
high-level posting entry point the rest of the front office uses, not
`_journalEntryPostingService.BulkSave` the way Voucher/General
Ledger/Refund batch theirs. Any `DynamicCharge`s attached via
`PUT /{id}/dynamiccharges` are looked up once per batch and passed into
every entry's `AddNewJournal` call as transfer-fee tariffs. Entries already
`Posted` from a prior partial run are silently skipped on a retry (checked
via `Status == Pending`), making a re-`Authorize` call reasonably safe to
retry after a partial failure.
