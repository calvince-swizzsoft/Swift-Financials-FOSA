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
| Refund | `OverDeductionBatchController` | Not started |
| Disbursement | `LoanDisbursementBatchController` | Not started |
| Reversal | `JournalReversalBatchController` | Not started |
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
