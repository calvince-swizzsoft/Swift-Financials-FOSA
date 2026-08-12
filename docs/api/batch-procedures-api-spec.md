# Batch Procedures API

Functional basis, the maker-checker-authorizer lifecycle every type here
shares, and why each type exists:
`WebApplication1/Areas/Accounts/BATCH-PROCEDURES-CONCEPTS.md`. Read that
first if you're building a screen against any of these — this doc is just
the route/field reference.

Progress across the nine types the reference app's "Batch Procedures" menu
covers:

| Type | Controller | Status |
|---|---|---|
| Credit | `CreditBatchController` (`api/accounts/creditbatches`) | Built — §1 |
| Debit | `DebitBatchController` (`api/accounts/debitbatches`) | Built — §2 |
| Wire Transfer | `WireTransferBatchController` (`api/accounts/wiretransferbatches`) | Built — §3 |
| Reversal | `JournalReversalBatchController` (`api/accounts/journalreversalbatches`) | Built — §4 |
| Refund | `OverDeductionBatchController` | Not started |
| Disbursement | `LoanDisbursementBatchController` | Not started |
| Voucher | `JournalVoucherController` | Not started |
| General Ledger | `GeneralLedgerController` | Not started |
| Inter Account Transfer | `InterAccountTransferBatchController` | Not started |

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
