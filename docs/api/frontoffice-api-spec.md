# Front Office API — Client Integration Spec

Audience: teller/counter, treasury, and back-office screens (cash
transactions, treasury movement, cheques, end of day, account closure,
fixed deposits, expense payables, sundry payments, in-house cheques,
automated clearing, fiscal counts).

Source of truth:
- Functional design/workflow: `WebApplication1/Areas/FrontOffice/WORKFLOW.md`
  — read that first for *why* each endpoint exists and how the pieces fit
  together; this doc is only the request/response reference.
- Controllers: `WebApplication1/Areas/FrontOffice/Controllers/*.cs`.
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2. Every endpoint below requires
  `[Authorize]`.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/frontoffice/<area>` — see the table per controller below |
| Transport | HTTPS only |
| Content type | `application/json`, except the automated-clearing upload endpoint (`multipart/form-data`) |
| Auth | Bearer JWT on every request |

## 2. Response envelope & paging

`{ success: boolean, message: string, data: T | null }` on every endpoint —
same as `docs/api/README.md`. Paged list endpoints return
`PageCollectionInfo<T>` (`{ pageIndex, pageSize, pageCollection, itemsCount }`)
under `data`. `pageIndex` is 0-based, defaults to `0`; `pageSize` defaults
to `20` unless noted.

**Several controllers in this area report business-rule failures as
`success: false` inside a `200 OK`, not a `4xx` status** —
`CashManagementController` (§5), `TransfersController` (§7), and
`EndOfDayController` (§9) all do this for most of their failure paths,
reserving real `400`s for only a couple of early guard checks each. Check
`success` in the body on every call to these three; each section below
states exactly which of its failures are genuine `400`s.

## 3. Current-teller resolution

Any endpoint described as "resolves the current teller" reads the
`EmployeeId` claim off the caller's validated JWT and looks up the `Teller`
linked to that employee — there is no way to act as a different teller by
passing an id. If the caller has no linked teller, the endpoint returns
`400`.

---

## 4. Cash deposits / withdrawals — `api/frontoffice/requests`

Controller: `CashDepositController.cs`. Handles the core teller transaction
cycle (WORKFLOW.md §5) — deposit, withdrawal, cheque deposit, payment
voucher — plus the pending-request queue and posting an already-authorized
request.

### 4.0 Resolve teller context — `GET /context`

Returns the authenticated employee's teller and branch context for display:
`tellerId`, `tellerDescription`, `tellerCode`, `branchId`,
`branchDescription`, `isLocked`, and `bookBalance`. It accepts no employee,
teller, or branch identifier from the client. A missing teller linkage or
branch linkage returns `409` with a configuration-specific message.

### 4.1 List pending/authorized requests — `GET /`

Query params: `type` (optional — `2` = CashDeposit, `1` = CashWithdrawal),
`status` (optional, defaults to `Pending` — pass explicitly for
`Authorized`/`Posted`/`Paid`/`Rejected`), `text`, `startDate`, `endDate`,
`pageIndex`, `pageSize`.

- `type=2` or `type=1` → `data` is `PageCollectionInfo<CashDepositRequestDTO>`
  or `PageCollectionInfo<CashWithdrawalRequestDTO>` respectively, each row's
  `CustomerName` populated server-side.
- `type` omitted → `data` is `PageCollectionInfo<object>`: the deposit and
  withdrawal queues merged into one page, sorted by `CreatedDate` descending
  and paged as a combined set (`pageSize` applies to the merged total, not
  per source). Each row keeps its own native DTO shape — inspect
  `TransactionType` (`1`/`2`) client-side to tell which one you got, or to
  filter down to a single type without a second call.
- `type=3` (ChequeDeposit) or `type=4` (CashWithdrawalPaymentVoucher) →
  always empty; neither has its own request row (cheque deposits post
  directly; payment-voucher withdrawals are stored under `type=1` with
  `Category = PaymentVoucher`).

### 4.2 Post a transaction — `POST /`

Body: `CustomerTransactionModel` — `Type` (`FrontOfficeTransactionType`:
`CashDeposit=2`, `CashWithdrawal=1`, `ChequeDeposit=3`,
`CashWithdrawalPaymentVoucher=4`), `CreditCustomerAccountId`,
`TotalValue`, plus type-specific fields (`Drawer`/`DrawerBank`/`ChequeType`
for cheque deposits, `PaymentVoucher` for voucher withdrawals). `ChequeType`
is `Guid?` — omit it (or send `null`) if the teller doesn't select a cheque
type; the cheque then matures the same day it's deposited
(`ChequeType.MaturityPeriod` drives this when one is selected).

`BranchId` is not a client input. The API always replaces any supplied
value with the authenticated employee's teller branch before resolving the
branch, product, posting description, journal, or workflow. Clients should
omit it entirely and may use `GET /context` to show the inferred branch as
read-only operator context.

**`ChequeDeposit` does not credit the customer's spendable balance.** Unlike
`CashDeposit`, a cheque deposit posts its credit leg to the
`ExternalChequesControl` suspense account (still tagged to the customer for
statement visibility — it shows up on their
`CustomerAccountStatementType.ChequeDepositStatement` mini-statement — but
it is **not** part of `AvailableBalance`/`BookBalance` yet). The customer is
only actually credited once the cheque is transferred, banked, and
Pay-cleared (§7, §8) — that full cycle can take days. **Don't show a cheque
deposit's amount as available funds immediately the way you would for
`CashDeposit`** — surface it as "pending clearance" until the underlying
`ExternalCheque.IsCleared` becomes `true`. This was fixed mid-development
(previously `ChequeDeposit` incorrectly credited the customer immediately,
identically to a cash deposit, then credited them a second time on
clearance) — if your UI was built against the old behavior, it needs to
change.

`POST /` can additionally fail for a `ChequeDeposit` with `success: false`
and message `"Sorry, but the external cheques control account has not been
setup!"` if `SystemGeneralLedgerAccountCode.ExternalChequesControl` isn't
mapped to a chart of account yet — an admin/setup problem, not a per-request
one; surface it as such rather than retrying.

Response shape varies by outcome:
- **Posted directly** (within limits): `data` is the `JournalDTO` (id,
  sequential id, branch/posting-period/user descriptions, amount,
  reference, created date, `TransactionCode`/`TransactionCodeDescription` —
  render the transaction label from `TransactionCodeDescription`, not a
  client-side guess; it's `SystemTransactionCode.CashDeposit`/
  `CashWithdrawal`/`ChequeDeposit`/`CashWithdrawalPaymentVoucher` depending
  on `Type`) — everything needed to render a receipt.
- **Authorization required** (above limit/below minimum/overdraft/voucher):
  `success: false`, `data: { dialog: true, isCashDepositRequest |
  isCashWithdrawalRequest: true, cashTransactionRequestId,
  selectedCustomerAccountId, transactionTotalValue, transactionReference,
  transactionCategory, ...paymentVoucher fields for withdrawals }`. The
  request is now `Pending` and enqueued into the generic workflow engine
  (§17 below) — nothing further to call here until a checker approves it.
  Once a checker approves, prefer `POST /post?id={requestId}` (§4.3) to
  actually post it. `POST /` also has its own resubmit path for this — if
  you call it again with `CustomerTransactionModel.CashDepositRequestId`/
  `CashWithdrawalRequestId` set to the returned `cashTransactionRequestId`,
  it'll post against that specific now-`Authorized` request instead of
  creating a new one. **Set the id precisely** — for withdrawals this was
  previously unscoped and could post the current call's amount while
  marking a *different*, unrelated authorized request `Paid` if the
  customer had more than one pending; fixed to match deposits' existing
  correct behavior.
- **Blocked/failed**: `success: false`, `message` explains why (teller
  locked, account not approved, below minimum balance, teller range limit,
  validation errors), `data: null`.

### 4.3 Post an authorized request — `POST /post?id={requestId}`

Call once a checker has approved the request (via the generic workflow
endpoint, §17). Re-derives the transaction from the now-`Authorized` request
and posts the GL journal. `400` if the request isn't `Authorized` yet.
Response: `data` is the `JournalDTO` on success.

### 4.4 Mark posted without re-deriving — `POST /markposted?id={requestId}`

Flips an `Authorized` request straight to `Posted`/`Paid` without building a
new journal — only use this if the journal was already posted through
another path and this is purely a status correction.

There is **no** `POST /authorize` endpoint — see §17.

---

## 5. Treasury cash movement — `api/frontoffice/cashmanagement`

Controller: `CashManagementController.cs`.

### 5.1 Post a movement — `POST /`

Body: `FiscalCountDTO` — `TransactionType` (`TreasuryTransactionType`:
`BankToTreasury`/`TreasuryToBank`/`TreasuryToTeller`/`TreasuryToTreasury`
**only** — `TellerToTreasury` and `TellerCashTransfer` are real enum
members but belong to End of Day close (§9) and cash transfer requests
(§7) respectively, not this endpoint; sending either now returns
`success: false, message: "Operation Failed: Unsupported transaction type
for treasury cash movement."` instead of a fake success with nothing
posted, which is what it silently did before this fix), denomination
breakdown fields (`DenominationOneThousandValue` ... down to 50-cent),
`TotalValue`.

**Status codes, checked directly against every `return` in `Create`/
`DoSomething`**: only a missing posting period or treasury returns a real
`400` (`BadRequest`). Every other failure on this endpoint — insufficient
book balance on an outgoing transfer (`TreasuryToTeller`/`TreasuryToBank`/
`TreasuryToTreasury`; `BankToTreasury` has no such check), bank/treasury
not found, a denomination mismatch, an unhandled exception — returns HTTP
`200` with `success: false` in the body. **Check `success`, not HTTP
status, everywhere on this endpoint except that one case.**

The book-balance check reads the resolved treasury's real GL balance —
`Create` calls `ITreasuryAppService.FetchTreasuryBalances` right after
resolving `ActiveTreasury` (via `FindTreasuryByBranchId`, which doesn't
populate `BookBalance` on its own; `Treasury` has no balance column of its
own, only a `ChartOfAccountId`) to fill it in before any outgoing-transfer
check runs.

The denomination fields and `TotalValue` **are cross-validated**: each
denomination field holds that denomination's own monetary subtotal (not a
raw note/coin piece count — e.g. `DenominationOneThousandValue` is "how
much was counted in 1000-notes"), and the eleven subtotals must sum to
exactly `TotalValue` (`Utils.SumDenominationValues`) or the call fails
(`success: false`, HTTP `200`) before anything is posted. The denomination
breakdown is then persisted as a separate physical-count audit record
(`FiscalCountAppService.AddNewFiscalCounts`) alongside the GL journal —
the `TransactionType` you sent is persisted on that record too (§16.4),
including on the destination-side record `TreasuryToTreasury` writes.

`Id` is overloaded depending on `TransactionType` — it isn't the fiscal
count's own id on the way in:

| `TransactionType` | What `Id` must be |
|---|---|
| `BankToTreasury` / `TreasuryToBank` | The `Bank` id |
| `TreasuryToTreasury` | The **destination** `Treasury` id |
| `TreasuryToTeller` | Unused — send `TellerId` instead |

`DestinationBranchId` is only meaningful for `TreasuryToTreasury` — the
receiving branch. That call writes **two** `FiscalCount` records in one
request (source and destination sides), which is also why `TreasuryToTreasury`
is the one case where two chart-of-account ids matter: the source
treasury's (credited) and the destination treasury's (debited, resolved
from the `Id` you sent).

`BranchId` and `ChartOfAccountId` sent by the client are both overwritten
server-side regardless of value — `BranchId` is used once to resolve the
caller's own treasury (`FindTreasuryByBranchId`), then reset to that
treasury's own branch; `ChartOfAccountId` is always replaced with the
resolved treasury's chart of account. Don't bother populating either.

Treasury *master data* (creating/editing the `Treasury` vault record
itself) is no longer part of this area — it's pure admin CRUD, not
front-office cash-cycle behavior, and now lives at
`api/accounts/treasurys`; see `docs/api/treasury-api-spec.md`.

---

## 6. Teller master data — `api/frontoffice/tellers`

Controller: `TellerController.cs`. All responses use the standard
`{ success, message, data }` envelope.

- `GET /?tellerType=&text=&pageIndex=&pageSize=` — paged/filtered list
  against `TellerDTO` (`tellerType` optional `TellerType` filter, defaults
  to `0`/all; `pageIndex` 0-based, `pageSize` default `20`). `data` is
  `PageCollectionInfo<TellerDTO>`.
- `GET /{id}` — single teller. `404` if not found.
- `POST /` — create (`TellerDTO`, GL wiring auto-derived from `TellerType`).
  `400` with `data: null` and a semicolon-joined `message` on validation
  failure.
- `PUT /{id}` — update. Same route-`id`-is-authoritative behavior as
  Treasury above.
- `GET /teller?employeeId={id}` — lookup by employee (an admin/support
  lookup, not the "who am I" pattern in §3).

---

## 7. Cash transfers & cheque transfer batch — `api/frontoffice/transfers`

Controller: `TransfersController.cs`.

| Route | Method | Purpose |
|---|---|---|
| `/cheques` | GET | Current authenticated teller's transfer context: server-resolved `TellerToTreasury` direction, daily balances/totals, and pending-cheque value |
| `/cash` | GET | List cash transfer requests |
| `/cash` | POST | Raise a cash transfer request (`CashTransferRequestDTO`) |
| `/cheques` | POST | Batch-transfer selected cheques (`List<ExternalChequeDTO>`) — the EOD precondition, WORKFLOW.md §7 |
| `/cash/acknowledge?option={n}` | POST | Acknowledge a cash transfer request (`CashTransferRequestDTO`) |
| `/` | GET | All cash transfer requests |
| `/cash/utilize?request={id}` | POST | Mark a cash transfer request `Utilized` |

`POST /cash` is always classified server-side as `TellerToTreasury`; callers
cannot select or spoof the direction. `TallyByTotal: false` requires the eleven
`Denomination*Value` fields to reconcile to `Amount`. `TallyByTotal: true`
accepts the stated total without inventing a denomination breakdown. Creation,
daily teller-value resolution, reconciliation, status classification, and the
companion fiscal-count write are owned by `ICashTransferRequestAppService`.

`POST /cash` and `POST /cash/acknowledge` now actually run
`CashTransferRequestDTO.ValidateAll()` before checking `HasErrors` — the
call was previously missing, so `HasErrors` (which only reflects what
`ValidateAll()` populates) was always `false` and validation was silently
skipped entirely. The one real rule this enforces: `Amount` must be
greater than zero.

`CashTransferRequest` itself has no columns for a denomination breakdown —
on success, the counted denominations are written as a **companion**
`FiscalCount` record (`TransactionCode = TellerCashTransfer`,
`TransactionType = TreasuryTransactionType.TellerCashTransfer`,
`ChartOfAccountId` = the caller's own teller account), not onto the
request record. There's no endpoint to fetch that companion record back
via the transfer request itself; query it through
`GET /api/frontoffice/fiscalcounts` (§16) if needed.

---

## 8. Cheque banking & clearance — `api/frontoffice/cheques`

Controller: `ChequesController.cs`.

- `GET /?text=&pageIndex=&pageSize=` — paged cheque list. Each
  `ExternalChequeDTO` row carries `IsTransferred`/`IsBanked`/`IsCleared` —
  use these to decide which actions to offer per row (see below) rather
  than letting the user attempt an action the API will reject.
- `POST /bank` — body `{ selectedChequeIds: Guid[], bankLinkageDTO,
  moduleNavigationItemCode }`. Only cheques with `IsTransferred: true` are
  eligible — the server already filters its own candidate list to these, so
  an id for a not-yet-transferred cheque is simply ignored (excluded from
  `selectedCheques`), not an error.
- `POST /clear` — body `{ selectedChequeIds: Guid[], clearingOption,
  actionType: "clear"|"unpay", unPayReasonDTO, moduleNavigationItemCode }`
  (`unPayReasonDTO` required when `actionType` is `"unpay"`). **`clearingOption` must agree with
  `actionType`** (`Pay=1` with `"clear"`, `UnPay=2` with `"unpay"`) — the
  server does not derive one from the other, so a mismatched pair silently
  takes whichever branch `clearingOption` selects, not the one `actionType`
  implies (open issue, `CHEQUE-PROCESSING-ANALYSIS.md` Finding #5 — always
  send them in agreement). As of this doc, clearing (either `Pay` or
  `UnPay`) now requires `IsTransferred: true` **and** `IsBanked: true` on the
  cheque — attempting to clear a cheque that hasn't been banked yet fails
  with `success: false`, `message` containing "Failed to clear cheque" (or
  "Failed to unpay cheque"). The candidate list this endpoint sources from
  (`GET` uncleared cheques) is **not** filtered on `IsBanked` server-side, so
  don't rely on "the cheque showed up as clearable" — check `IsBanked`
  yourself before offering the Clear action, or expect this failure and
  surface it clearly.
- `GET /untransfered?teller={id}` — untransferred cheques for a teller.

**`moduleNavigationItemCode` on `bank`/`clear` is now required** — send the
`NavigationItem.Code` of the screen the user is on (the "Cheques" item,
`ControllerName=Cheques`/`ActionName=Index`) so the GL journal these
endpoints post carries an accurate reference back to it. Previously the
server hardcoded this to a placeholder value (`123` for `bank`, `1` for
`clear`) that didn't correspond to any real navigation item, silently
breaking that audit trail; fixed.

Note: this controller's failure responses (`bank`/`clear`) return
`{ success: false, message }` **without a `data` key at all**, unlike the
`{ success, message, data }` envelope every other controller in this API
follows — don't assume `data` is present (even as `null`) on a failed
`bank`/`clear` call.

---

## 9. End of Day close — `api/frontoffice/endofday`

Controller: `EndOfDayController.cs`.

### `POST /`

Body: `CashTransferRequestDTO` — `UntransferredChequesValue`,
`TellerCashBalanceStatusValue` (`TellerCashBalanceStatus`:
`Balanced`/`Shortage`/`Excess`), `ClosingBalance`, `BookBalance`, plus the
eleven `Denomination*Value` fields (same shape as `FiscalCountDTO`) — their
sum must equal `ClosingBalance` or the call returns a real `400` before
anything else runs. The teller is always the caller's own (§3) — any
`TellerId` in the body is overwritten.

**`UntransferredChequesValue` in the request body is no longer trusted for
the "transfer your cheques first" gate** — it's still accepted (and echoed
into the fiscal count record) but the actual gate now independently queries
`IExternalChequeAppService.FindUnTransferredExternalChequesByTellerId` for
the caller's own teller server-side. Previously a caller could send `0`
regardless of reality and bypass the precondition entirely; that's fixed.
No client change needed unless you were relying on the old bypass.

Enforces, in order: malformed input (`400`) → denomination reconciliation
(`400`) → caller has a linked teller record (`400`) → posting-model
validation → cheques transferred → EOD not already run today → writes a
`FiscalCount` record (`TransactionCode = TellerEndOfDay` — this is also
what `IsEndOfDayExecutedAsync` checks for, so a closed day actually stays
closed; `TransactionType = TreasuryTransactionType.TellerToTreasury`) →
posts the close journal (+ suspense entry to
`Teller.ShortageChartOfAccountId`/`ExcessChartOfAccountId` if unbalanced).

**Only the first three checks are real `400`s.** Everything from
posting-model validation onward — including "you need to transfer your
cheques first" and "you have already closed your day" — reports failure
as `success: false` with HTTP `200`, checked directly against every
`return` in `Create`. Check `success` in the body for those, not status
code.

Success: `data` is the closing `JournalDTO` — render/print the EOD receipt
from this; there is no server-side print endpoint (removed — see
WORKFLOW.md §11).

---

## 10. Account closure — `api/frontoffice/accountclosures`

Controller: `AccountClosureController.cs`. Real sequence:
**Create → Approve → Verify → Settle** (WORKFLOW.md §9 — this is the order
`AccountClosureRequestAppService` enforces, not the reference app's action
naming).

| Route | Method | Purpose |
|---|---|---|
| `/?status=&text=&startDate=&endDate=&pageIndex=&pageSize=` | GET | Paged list. Omit `status` for the default (all/unfiltered) paged view |
| `/{id}` | GET | Single request |
| `/customer-account/{customerAccountId}` | GET | All requests for a customer account (unpaged) |
| `/` | POST | Create (`AccountClosureRequestDTO`, `Reason` required) → `Registered`. `409` if the account already has one in progress (`errormassage` on the DTO surfaced as the error message) |
| `/{id}/approve` | POST | `{ option, remarks }` — `AccountClosureApprovalOption`: `1`=Approve, `2`=Defer. Only accepts `Registered`/`Deferred` |
| `/{id}/verify` | POST | `{ option, remarks }` — `AccountClosureAuditOption`: `1`=Audit(verify), `2`=Defer. Only accepts `Approved` |
| `/{id}/settle` | POST | `{ option, remarks }` — `AccountClosureSettlementOption`: `1`=Settle, `2`=Defer. Only accepts `Audited` |

Each transition returns `409` (not `400`) if the request isn't in the
right status for that action.

**`/settle` does not pay out the customer's remaining balance — it only
flips the request to `Settled`** (and closes the underlying customer
account, done earlier at `/verify` time). This matches the reference app,
where `Settle` was always just a status transition too. **The actual
payout is a separate, manual step**: `POST /api/frontoffice/sundrypayments`
with `transactionType: 32` (`CashPaymentAccountClosure`, §13) —
`chartOfAccountId` = the closed account's
`AccountClosureRequestDTO.CustomerAccountTypeTargetProductChartOfAccountId`,
`totalValue` = `AccountClosureRequestDTO.NetRefundable` (both available from
`GET /{id}` above). There's no ordering enforced between `/settle` and the
sundry-payment payout — build your UI flow to prompt for/perform the
payout as part of settling, even though they're two separate API calls.

Not reproduced from the reference controller: per-request loan
balance/investment balance/guarantor summary enrichment — compose that
client-side from the already-documented customer-accounts endpoints.

---

## 11. Fixed deposits — `api/frontoffice/fixeddeposits`

Controller: `FixedDepositController.cs`.

| Route | Method | Purpose |
|---|---|---|
| `/?text=&pageIndex=&pageSize=` | GET | Paged list |
| `/{id}` | GET | Single deposit |
| `/customer-account/{customerAccountId}` | GET | Deposits for a customer account |
| `/payable?startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Maturity payout queue |
| `/revocable?startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Early-termination queue |
| `/{id}/payables` | GET | Payable/payout lines for a deposit |
| `/{id}/payables` | PUT | Replace the payable lines (`List<FixedDepositPayableDTO>`) |
| `/` | POST | Origination (`FixedDepositDTO`, `Remarks` required) |
| `/{id}/verify` | POST | `{ approve: bool, moduleNavigationItemCode }` — Post or Reject |
| `/terminate` | POST | `{ selectedFixedDepositIds: Guid[], moduleNavigationItemCode }` — batch early termination |
| `/liquidate` | POST | `{ selectedFixedDepositIds: Guid[], moduleNavigationItemCode }` — batch maturity payout; `400` if any selected deposit hasn't reached `MaturityDate` |

---

## 12. Expense payables — `api/frontoffice/expensepayables`

Controller: `ExpensePayableController.cs`. Sequence: Create (`Pending`) →
add entry lines → Verify (`Audited`/`Rejected`/`Deferred`) → **approval
happens through the generic workflow engine, not this controller** (§17 —
Verify enqueues automatically when the option is `Post`).

| Route | Method | Purpose |
|---|---|---|
| `/?status=&text=&startDate=&endDate=&pageIndex=&pageSize=` | GET | Paged list |
| `/{id}` | GET | Single payable |
| `/{id}/entries?pageIndex=&pageSize=` | GET | Entry lines, with `data.totalApportioned`/`data.totalShortage` |
| `/` | POST | Header create (`ExpensePayableDTO`) → `Pending` |
| `/{id}/entries` | POST | Add one GL line (`ExpensePayableEntryDTO`) |
| `/entries/remove` | POST | Batch-remove lines (`List<ExpensePayableEntryDTO>`) |
| `/{id}/verify` | POST | `{ option, remarks }` — `ExpensePayableAuthOption`: `1`=Post, `2`=Reject, `4`=Defer. Only accepts `Pending`. On `Post` success, enqueues into the generic workflow (`SystemPermissionType.ExpensePayablesAuthorization`) |

---

## 13. Sundry payments & customer receipts

Controllers: `SundryPaymentsController.cs`
(`api/frontoffice/sundrypayments`), `CustomerReceiptsController.cs`
(`api/frontoffice/customerreceipts`). Both post a **single-line** GL
journal against the caller's own teller cash account — no dedicated app
service backs either (same as the reference controllers, which posted
straight through the shared journal service).

### 13.1 `POST /` (both controllers)

Body: `{ chartOfAccountId, totalValue, reference, primaryDescription,
moduleNavigationItemCode }`. Sundry payments additionally take
`transactionType` (`GeneralTransactionType`: `1`=CashReceipt,
`2`=ChequeReceipt, `4`=CashPayment, `8`=CashPickup, `16`=SundryPayment,
`32`=CashPaymentAccountClosure — direction of the debit/credit against the
teller account is derived from this), plus `creditBatchEntryId`, which is
**required when `transactionType: 8`** (ignored otherwise). Response `data`
is the posted `JournalDTO`.

For `transactionType: 32` (account closure payout — §10), resolve
`chartOfAccountId`/`totalValue` from the closure request first:
`GET /api/frontoffice/accountclosures/{id}` →
`chartOfAccountId = data.customerAccountTypeTargetProductChartOfAccountId`,
`totalValue = data.netRefundable`. This transaction type was missing
entirely until this doc's current revision — a batch create with
`transactionType: 32` previously returned `400 "Unsupported transaction
type"` with no other way to complete a closure's payout anywhere in this
API; fixed, now restores the reference app's original behavior (which also
treated account-closure payout as an ordinary sundry payment, not something
`/settle` did automatically).

For `transactionType: 8` (Cash Pickup), resolve `chartOfAccountId`/
`totalValue`/`creditBatchEntryId` from a picked credit-batch entry first —
full picker flow, field mapping, and the `entry.amount`-is-always-0 gotcha
are in §13.3. Omitting `creditBatchEntryId` on a `transactionType: 8`
request returns `400`. On success, this endpoint calls
`POST /api/accounts/creditbatches/entries/{creditBatchEntryId}/post`
itself — the client does not post the entry separately, it only has to pick
one and pass its id through.

**Known gap**: `CustomerReceiptsController` posts one line only.
`IJournalAppService` has no apportioned-posting overload, so the reference
app's "split one receipt across multiple accounts" capability isn't
available here — see WORKFLOW.md §11 for why.

### 13.2 Designing the unified "Sundry Receipts/Payments" screen

This is one client screen with a transaction-type selector (matching the
reference app's `NavigationMenu` entry "Sundry Receipts/Payments" →
`GeneralTransactionType`), but the six types split into two fundamentally
different input shapes. Business intent for each, from product:

| Type | Value | Purpose | What the teller does |
|---|---|---|---|
| Cash Payment | `4` | Miscellaneous payment not through a customer account (e.g. visitor entertainment, committee night-out expenses) | **Types everything**: picks a GL account, enters an amount |
| Cash Payment (Account Closure) | `32` | Cash payout when a customer closes a *child* account (e.g. a holiday account) | **Picks only**: selects an Audited closure request from a list; amount/account come from the request, not typed |
| Cash Pickup | `8` | Pays non-account holders (casual laborers); their pay is captured up front in Accounts under Credit Batch | **Picks only**: selects an entry from an already-captured list (§13.3); teller fills in *no* details at all |
| Cash Receipt | `1` | Receive income/cash not tied to a customer account (e.g. rent, penalties/fines) | **Types everything**: picks a GL account, enters an amount |
| Cheque Receipt | `2` | Receive a cheque into a GL account (e.g. a salary cheque) | **Types everything**: picks a GL account, enters an amount |
| Sundry Payment | `16` | Pay for goods/services rendered to the Sacco — payee may or may not be an account holder | Not yet implemented server-side — see gap below |

So there are really only two screen shapes, and the client should pick the
right one per `transactionType` rather than always rendering a generic
"chart of account + amount" form:

- **Manual entry** (Cash Payment `4`, Cash Receipt `1`, Cheque Receipt `2`):
  render a chart-of-account picker (typeahead against
  `GET /api/accounts/chartofaccounts`, `ChartOfAccountController.cs` — no
  standalone doc for it yet) + amount + reference + description. The teller
  free-types the GL account and amount; POST them straight through.
- **Pick-list only** (Cash Payment (Account Closure) `32`, Cash Pickup `8`):
  the teller never types an account or amount — they browse a queue of
  pre-existing records and the client reads `chartOfAccountId`/`totalValue`
  off whichever row was selected. See §13.1 above for the account-closure
  resolution, and §13.3 below for Cash Pickup.

**Cheque Receipt caveat**: today this only posts the GL journal from a
manually-typed chart of account + amount. It does **not** capture the
physical cheque (number, drawer, drawer's bank) anywhere —
`IExternalChequeAppService.AddNewExternalCheque` exists but nothing in this
project calls it for a sundry cheque receipt. If the business needs that
cheque detail retained for audit/lookup (as opposed to customer-account
cheque deposits via `CashDepositController`, which *do* create an
`ExternalCheque` — see `SAVINGS-RECEIPTS-PAYMENTS-FLOW.md`), that's a
separate backend change, not something the client can work around by itself.
For now, put cheque number/drawer/bank in the free-text `reference`/
`primaryDescription` fields if the teller needs to record them.

**Sundry Payment (`16`) — not implemented**: `SundryPaymentsController`'s
switch has no case for it; a request with `transactionType: 16` returns
`400 "Unsupported transaction type"`. This isn't a regression — the
reference MVC controller never handled it either, and its own Create view
had the "Sundry Payment" tab commented out. Per business, this type can pay
*either* an account holder or a non-account holder for goods/services
rendered to the Sacco, which means it doesn't cleanly fit either of the two
shapes above:
- If the payee is an account holder, the screen likely needs a **customer/
  customer-account picker** (existing: `Areas/Registry/Controllers/
  CustomerController.cs`, `CustomerAccountsController.cs`) with the GL
  account resolved from that account's product mapping — not a bare
  chart-of-account typeahead.
- If the payee is not an account holder, it's presumably the **manual
  entry** shape, same as Cash Payment, but posted under its own transaction
  code rather than `SystemTransactionCode.GeneralCashPayment` (candidates in
  `Enumerations.cs`: `CreditBatchSundryPayment = 43`, tagged "Sundry Payment
  Batch" — but that code is currently wired to the *separate*
  `CreditBatchType.SundryPayments` bulk-import feature in
  `CreditBatchAppService`, not to this single-line screen, so reusing it
  here needs confirming with whoever owns the GL chart of accounts/reports
  that key off transaction codes).

Don't build a Sundry Payment tab against this controller yet — the request
shape, the account-holder-vs-not branching, and the transaction code all
need a product/backend decision first. Everything else in this table is
safe to build against today.

### 13.3 Cash Pickup picker — `api/accounts/creditbatches`

Controller: `CreditBatchController.cs`. Exposes `ICreditBatchAppService`
(batch header CRUD/audit/authorize, plus entry CRUD/browse/post). Full list
below; the two rows that matter for the Cash Pickup tab are `entries/type/8`
(browse) and `entries/{entryId}/post` (consume).

| Route | Method | Purpose |
|---|---|---|
| `/all` | GET | Unpaged list of every batch |
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged batch list |
| `/{id}` | GET | Single batch |
| `/` | POST | Create batch → `Pending` |
| `/{id}` | PUT | Update batch's own fields (TotalValue, concession, recovery flags, reference, priority, value date) — does not touch entries |
| `/{id}/audit` | POST | `{ option, remarks }` — `BatchAuthOption`: `1`=Post (→ `Audited`, only if entries total ≤ batch `TotalValue`), `2`=Reject. Only accepts `Pending` |
| `/{id}/authorize` | POST | `{ option, remarks, moduleNavigationItemCode }` — `1`=Post (→ `Posted`; for `Payout`/`CheckOff` batches this also posts every entry's GL journal inline), `2`=Reject |
| `/{id}/entries?text=&filter=&pageIndex=&pageSize=` | GET | Entries within one batch |
| `/entries/type/{creditBatchType}?startDate=&endDate=&text=&filter=&pageIndex=&pageSize=` | GET | Entries across all batches of a `CreditBatchType` — **this is the Cash Pickup picker**: `creditBatchType=8` |
| `/entries/customer/{customerId}?creditBatchType=` | GET | Entries for one customer (used by `Payout`/`CheckOff`, not Cash Pickup — entries there aren't tied to a customer account) |
| `/entries/{entryId}` | GET | Single entry |
| `/{id}/entries` | POST | Add an entry to a batch |
| `/entries/{entryId}` | PUT | Update an entry (status is forward-only: `Pending → Posted/Rejected`) |
| `/entries/remove` | POST | Batch-remove entries (`List<CreditBatchEntryDTO>`) |
| `/entries/{entryId}/post` | POST | `{ moduleNavigationItemCode }` — marks one entry `Posted`. For `CashPickup`/`SundryPayments` batches this **only** flips status; it does not post a GL journal (see below) |

**Building the Cash Pickup tab:**
1. `GET /api/accounts/creditbatches/entries/type/8` (`8` = `CreditBatchType.CashPickup`). This is **not** filtered by entry status server-side — the underlying query only filters by date range/type/text — so filter the response client-side for `status === 1` (`BatchEntryStatus.Pending`) to show only entries not yet paid.
2. The teller picks a row. Read the payout details off it: `chartOfAccountId = entry.creditBatchCreditTypeChartOfAccountId`, `totalValue = entry.principal + entry.interest` (Interest is always 0 for Cash Pickup, so in practice this is just `principal`). **Do not use `entry.amount`** — `CreditBatchEntry` has no `Amount` column on the domain entity, so that field is never populated by the AutoMapper projection and is always `0`. This is a preexisting gap in the domain model (also present in the reference app), not something new.
3. `POST /api/frontoffice/sundrypayments` with `transactionType: 8`, the resolved `chartOfAccountId`/`totalValue`, and `creditBatchEntryId` set to the picked entry's `id`. `creditBatchEntryId` is required for this transaction type — the request is rejected otherwise.
4. On success, `SundryPaymentsController` itself calls `POST /api/accounts/creditbatches/entries/{entryId}/post` to flip the entry to `Posted`, so it drops out of step 1's list and can't be paid twice. The client doesn't need to call this separately — but note the call is best-effort: if it fails after the journal already posted, the entry stays `Pending` and could show up again. There's currently no automatic detection/repair for that case, so if it's a concern, cross-check `GET /entries/{entryId}` before re-showing an already-picked entry.

Entries for a batch only become eligible for pickup once the batch itself
reaches `Posted` (`/{id}/authorize` with `option: 1`) — a batch stuck at
`Pending`/`Audited` won't return anything useful from step 1 above.

---

## 14. In-house cheques — `api/frontoffice/inhousecheques`

Controller: `InHouseController.cs`.

| Route | Method | Purpose |
|---|---|---|
| `/?text=&startDate=&endDate=&pageIndex=&pageSize=` | GET | Paged list |
| `/{id}` | GET | Single cheque |
| `/unprinted?branchId={id}&text=&pageIndex=&pageSize=` | GET | Printing queue for a branch |
| `/` | POST | `{ cheques: InHouseChequeDTO[], moduleNavigationItemCode }` — batch-build entries. Each entry is validated (`branchId`, `debitChartOfAccountId` both required valid GUIDs; `chequeTypeId` optional but must be a valid GUID if present) — the first invalid entry in the batch fails the whole request with `success: false` and the joined validation message, `data: null`; nothing in the batch is saved. |
| `/{id}/print` | POST | `{ printedNumber, bankLinkage: BankLinkageDTO, moduleNavigationItemCode }` — flips `IsPrinted`/`PrintedNumber` and posts the GL journal. The client renders/prints the cheque itself and reports back the printed number — this endpoint does no printing |

---

## 15. Automated (image-based) clearing — `api/frontoffice/automatedclearing`

Controller: `AutomatedClearingController.cs`.

| Route | Method | Purpose |
|---|---|---|
| `/?status=&startDate=&endDate=&text=&pageIndex=&pageSize=` | GET | Paged electronic journal list |
| `/{id}` | GET | Single electronic journal |
| `/{id}/truncatedcheques?status=&text=&pageIndex=&pageSize=` | GET | Truncated cheques within a journal |
| `/upload` | POST | `multipart/form-data`, one file part. Server saves it to the configured upload directory and parses it into an `ElectronicJournalDTO` |
| `/{id}/close` | POST | Finalizes/exports the journal (PGP-encrypted, server-configured keys — nothing client-supplied) |
| `/truncatedcheques/{id}/clear` | POST | Clears a truncated cheque |
| `/truncatedcheques/{id}/match-voucher` | POST | Matches a truncated cheque to its payment voucher |

`/upload` and `/{id}/close` read file paths, a blob-store connection
string, and PGP key paths/passphrase entirely from server config
(`serviceBrokerConfiguration` section + the `BLOBStore` connection string)
— none of that is ever accepted from the request.

---

## 16. Fiscal counts (standalone) — `api/frontoffice/fiscalcounts`

Controller: `FiscalCountController.cs`. Nav: Front-Office → Treasury →
"Fiscal Counts", alongside "Cash Management" and "Authorizations" — a
*sibling* screen to those two, not a child of either.

**Frontend scope: this is a read-only catalogue, not a CRUD screen.**
Every `FiscalCount` row that matters is written implicitly by treasury cash
movement (§5, `CashManagementController` — `BankToTreasury`/
`TreasuryToBank`/`TreasuryToTeller`/`TreasuryToTreasury`), EOD close (§9,
`TellerEndOfDay`), or a cash transfer request (§7, `TellerCashTransfer`) —
each of those posts its own GL journal and, alongside it, a `FiscalCount`
denomination-audit row. The catalogue's job is to let a user pick one of
those transaction types and see every row it ever produced, with the
denomination breakdown, not to let them create or edit rows by hand. Build
list (§16.1) and detail (§16.2) only — skip create/update UI for this
screen; §16.3 exists (parity with every other `FiscalCount` entry point)
but isn't part of the intended flow.

### 16.1 List — `GET /?text=&startDate=&endDate=&transactionCode=&pageIndex=&pageSize=`

Paged list, `data: PageCollectionInfo<FiscalCountDTO>`
(`{ pageIndex, pageSize, pageCollection, itemsCount }`). All params
optional. `text` does a case-sensitive `Contains` against
`ChartOfAccount.AccountName`, `PrimaryDescription`, `SecondaryDescription`,
`Reference`, or `CreatedBy` (any one matches). Supplying either `startDate`
or `endDate` switches to the date-ranged query — the bound you didn't
supply defaults to `DateTime.MinValue`/`MaxValue`, so a lone `startDate`
means "from then through now" and a lone `endDate` means "everything up to
then". `pageIndex` is 0-based, `pageSize` defaults to `20`.

`transactionCode` is the "select a transaction type" filter — pass a
`SystemTransactionCode` int value to see only that type's rows; omit it (or
send `0`) for all types. This is the field to drive a type-selector
control (tabs/dropdown/chips) on the catalogue grid:

| Filter label | `transactionCode` value |
|---|---|
| Bank to Treasury | `SystemTransactionCode.BankToTreasury` |
| Treasury to Bank | `SystemTransactionCode.TreasuryToBank` |
| Treasury to Teller | `SystemTransactionCode.TreasuryToTeller` |
| Treasury to Treasury | `SystemTransactionCode.TreasuryToTreasury` |
| Teller End-of-Day | `SystemTransactionCode.TellerEndOfDay` |
| Teller Cash Transfer | `SystemTransactionCode.TellerCashTransfer` |

Filter by `TransactionCode` — this endpoint has no `TransactionType`
filter — see §16.4 for what `TransactionType` means on a read.

### 16.2 Get one — `GET /{id}`

Single `FiscalCountDTO`. `404` if not found.

### 16.3 Manual entry — `POST /` (not needed for the catalogue build)

Body: `FiscalCountDTO`. `400` (message = semicolon-joined validation
errors) on binding-model validation failure; `400` if the eleven
`Denomination*Value` subtotals don't sum to `TotalValue` — same
reconciliation rule as every other `FiscalCount` entry point (§5, §7, §9).
Unlike those three controllers, this endpoint has **no** `success:
false`/`200` failure path for business rules — validation and
reconciliation failures are real `400`s here. Exists for ad-hoc/manual
denomination records outside the normal flow; the catalogue screen itself
doesn't need a create form — every row it displays already came from §5/§7/
§9.

### 16.4 Which fields actually come back on a read

`FiscalCountDTO` is shared with the *write* side (§5/§7/§9 build one to
post a movement), so it carries fields the `FiscalCount` entity itself
doesn't have — `AutoMapper`'s `FiscalCount → FiscalCountDTO` map
(`FrontOfficeModuleProfile.cs`) only fills what the entity actually owns or
navigates to. On `GET`/list responses from this controller:

**Populated:** `Id`, `BranchId`/`BranchDescription`,
`PostingPeriodId`/`PostingPeriodDescription`, `ChartOfAccountId` and its
`ChartOfAccountAccountType`/`AccountCode`/`AccountName`/`ChartOfAccountName`
(pre-formatted `"type-code name"`) and, if set, `ChartOfAccountCostCenterId`/
`Description`, `PrimaryDescription`, `SecondaryDescription`, `Reference`,
`TotalValue` (server-computed sum of the denomination fields, not a stored
column), the eleven `Denomination*Value` fields, `TransactionCode` /
`TransactionCodeDescription` (`SystemTransactionCode` — what actually
persisted, e.g. `TreasuryToTeller`/`TellerEndOfDay`/`TellerCashTransfer`),
`TransactionType`/`TransactionTypeDescription` (`TreasuryTransactionType` —
the entity has a matching column and every fiscal-count-creating flow always
sets it: `BankToTreasury`/`TreasuryToBank`/`TreasuryToTeller`/
`TreasuryToTreasury` from §5's client-supplied value, `TellerToTreasury` from
§9 (End of Day), `TellerCashTransfer` from §7 (cash transfer request); §16.3's
manual-entry endpoint persists whatever the caller sends, including `0` if
omitted), `SystemTraceAuditNumber`, `CreatedBy`, `CreatedDate`.

**Always default/empty on read** — don't build grid columns for these:
`TellerId`/`TellerDescription`, `TreasuryId`/`TreasuryDescription`,
`DestinationBranchId`, `Description` (a generic field distinct from
`PrimaryDescription`/`SecondaryDescription`), `SavingsProduct`, `Teller`.
If you need to distinguish "which kind of movement wrote this row" for
filtering, use `TransactionCode` — this endpoint has no `TransactionType`
filter — but `TransactionType` itself is a reliable read field now, not
merely a write-side input.

---

## 17. Maker-checker — the generic workflow engine

Cash deposit/withdrawal requests and expense payables enqueue into the
**generic** maker-checker engine documented in
`docs/api/customer-verification-api-spec.md` §2 and
`docs/api/customer-account-verification-api-spec.md` §2 — not a
front-office-specific approval endpoint. Once a request is `Pending`:

- Checker inbox: `GET /api/administration/workflows/items/mine`.
- Approve/reject: `POST /api/administration/workflows/items/approve`.

That drives `WorkflowProcessorAppService`, which calls the underlying
`AuthorizeCashDepositRequest`/`AuthorizeCashWithdrawalRequest`/
`AuthorizeExpensePayable` app-service method itself and records a
`WorkflowItem` audit row. The dedicated cash-withdrawal screen in §18
still submits its decision through this workflow endpoint; it does not
bypass maker-checker. Do not call a front-office endpoint directly to
approve a request; there isn't one (the one that used to exist,
`CashDepositController`'s `POST /authorize`, was removed for bypassing
this engine — see WORKFLOW.md §11).

---

## 18. Cash withdrawal requests — `api/frontoffice/cash-withdrawal-requests`

This is the dedicated Treasury screen described by the supplied Cash
Withdrawal Request process guide. Its resource endpoints delegate to
`ICashWithdrawalRequestAppService`; authorization remains in the generic
workflow engine described in §17.

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/` | Paged browse by `status`, optional date range, text and `customerFilter` |
| `GET` | `/{id}` | Retrieve one request |
| `POST` | `/` | Lodge a manual withdrawal notice and create its approval workflow |

Create accepts the selected customer-account context, `Type` (`0`
Immediate Notice, `1` Future Notice), positive `Amount`, and required
`Remarks`. The API owns `Status`, `Category`, and `TransactionType`; a
client cannot forge those lifecycle fields. Future notice maturity is
calculated by the AppService from the savings product withdrawal-notice
period and holiday calendar.

The screen loads the caller-scoped `GET
/api/administration/workflows/items/mine` queue and only offers a decision
when the caller has an unlocked workflow item with permission type
`CashWithdrawalRequestAuthorization` (`44992`) whose `WorkflowRecordId`
matches the request. It submits the decision through `POST
/api/administration/workflows/items/approve`. Users without that workflow
authority may still see the request (subject to module access), but receive
an informational explanation instead of a non-functional action.

---

## 19. Navigation item codes (reference)

For menu/permission-registration purposes — deciding whether to show a
front-office menu entry and what to check against
`GET /api/administration/modules` (same caveat as `customer-api-spec.md`
§5.12 and `textalert-api-spec.md` §5) — the closest equivalents in the
reference app's seeded navigation
(`Infrastructure.Crosscutting.Framework/Utils/NavigationMenu.cs`, seeded
one-time by `SwiftFinancials.Utility/Program.cs`, not computed live) are
below. **Don't hardcode these hex-sum codes** — they belong to the old
app's own `NavigationMenu` seed list, not a table this API shares by
direct migration. Source the current code for each screen from
`GET /api/administration/modules` instead; use this table only to find
which seeded entry (if any) corresponds to which controller here.

### Front-Office module tree (legacy root `0x000061A8`)

| Menu path | Screen (this API) | Legacy code |
|---|---|---|
| Front-Office | *(root module)* | `0x000061A8` |
| Front-Office → Operations | *(submenu)* | `0x000061A8 + 1` |
| → Operations → Treasury | *(submenu)* | `0x000061A8 + 2` |
| → → Treasury → Cash Management | §5 `CashManagementController` | `0x000061A8 + 3` |
| → → Treasury → Cash Withdrawal Requests | §18 dedicated request screen; decisions still use the §17 workflow engine | `0x000061A8 + 4` |
| → Operations → Teller | *(submenu)* | `0x000061A8 + 5` |
| → → Teller → Savings Receipts/Payments | §4 `CashDepositController` (deposit/withdrawal) | `0x000061A8 + 6` |
| → → Teller → Sundry Receipts/Payments | §14 `SundryPaymentsController` | `0x000061A8 + 7` |
| → → Teller → Customer Receipts | §14 `CustomerReceiptsController` | `0x000061A8 + 8` |
| → → Teller → Cheques/Cash Transfer | §8 `TransfersController` | `0x000061A8 + 9` |
| → → Teller → End-Of-Day | §10 `EndOfDayController` | `0x000061A8 + 10` |
| → Operations → Cheques | §9 `ChequesController` | `0x000061A8 + 11` |
| → Operations → Fixed Deposits | §12 `FixedDepositController` | `0x000061A8 + 12` |
| → Operations → Expense Payables | §13 `ExpensePayableController` | `0x000061A8 + 13` |
| → Operations → Account Closure | §11 `AccountClosureController` | `0x000061A8 + 14` |

### Seeded elsewhere (under the Accounts → Setup tree, not the Front-Office tree)

Two front-office master-data screens are seeded under `Accounts → Setup`
in the reference app rather than under `Front-Office → Operations` —
same controllers, different parent menu:

| Menu path | Screen (this API) | Legacy code |
|---|---|---|
| Accounts → Setup → Tellers | §7 `TellerController` | `0x000059D8 + 21` |
| Accounts → Setup → Treasuries | §6 `TreasurysController` | `0x000059D8 + 20` |

### Not seeded in the reference app — no legacy code exists

These screens' reference-app menu entries were never finished — they're
commented out in `NavigationMenu.cs` with a placeholder
`ControllerName = "Controller"` that was never wired to a real controller,
so **no legacy code exists to map them to at all**:

- §16 `AutomatedClearingController` (planned nodes: Journals, Processing,
  Catalogue — commented out under a never-activated "Automated Clearing"
  submenu)
- §15 `InHouseController` (planned nodes: Writing, Printing, Catalogue —
  commented out under a never-activated "In-House" submenu)
- §17 `FiscalCountController` (no submenu was ever drafted, standalone or
  otherwise)
- The more granular sub-steps that were drafted but never activated for
  Fixed Deposits (Fixing/Verification/Termination/Liquidation/Catalogue),
  Expense Payables (Origination/Verification/Authorization), and Account
  Closure (Registration/Approval/Verification) — each of those *processes*
  has an active top-level code above (`+12`/`+13`/`+14`), just not the
  finer-grained per-action breakdown that was sketched and abandoned in
  the comments around it

If any of these need a real menu entry in the new system, register it
fresh via whatever seeds `GET /api/administration/modules` today — don't
resurrect the commented-out block, since its `ControllerName`/`ActionName`
values were never finished either.
