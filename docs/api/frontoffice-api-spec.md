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
`CashWithdrawalPaymentVoucher=4`), `CreditCustomerAccountId`, `BranchId`,
`TotalValue`, plus type-specific fields (`Drawer`/`DrawerBank`/`ChequeType`
for cheque deposits, `PaymentVoucher` for voucher withdrawals). `ChequeType`
is `Guid?` — omit it (or send `null`) if the teller doesn't select a cheque
type; the cheque then matures the same day it's deposited
(`ChequeType.MaturityPeriod` drives this when one is selected).

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
| `/cheques?TellerId={id}` | GET | Untransferred-cheques summary for a teller — previously threw an unhandled `ArgumentNullException` for any teller with zero pending cheques (the normal/clean state), since the underlying app-service call returns `null` rather than an empty list when there's nothing to find; fixed |
| `/cash` | GET | List cash transfer requests |
| `/cash` | POST | Raise a cash transfer request (`CashTransferRequestDTO`) |
| `/cheques` | POST | Batch-transfer selected cheques (`List<ExternalChequeDTO>`) — the EOD precondition, WORKFLOW.md §7 |
| `/cash/acknowledge?option={n}` | POST | Acknowledge a cash transfer request (`CashTransferRequestDTO`) |
| `/` | GET | All cash transfer requests |
| `/cash/utilize?request={id}` | POST | Mark a cash transfer request `Utilized` |

`POST /cash` requires a denomination breakdown that reconciles to `Amount`,
same as treasury cash movement (§5) — `CashTransferRequestDTO` carries the
same eleven `Denomination*Value` fields as `FiscalCountDTO`. As with §5,
this controller reports the mismatch — and every other business-rule
failure on `/cash` — as `success: false` with HTTP `200`, not `400`; check
`success` in the body. The only real `400` on `TransfersController` at all
is `POST /cash/utilize` with a missing `request` id.

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
`2`=ChequeReceipt, `4`=CashPayment, `8`=CashPickup, `32`=CashPaymentAccountClosure
— direction of the debit/credit against the teller account is derived from
this). Response `data` is the posted `JournalDTO`.

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

**Known gap**: `CustomerReceiptsController` posts one line only.
`IJournalAppService` has no apportioned-posting overload, so the reference
app's "split one receipt across multiple accounts" capability isn't
available here — see WORKFLOW.md §11 for why. `SundryPaymentsController`
likewise doesn't expose the credit-batch-entry pickup queue
(`CreditBatchType.Payout`/`CheckOff` browse) — only the single-transaction
post.

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
`WorkflowItem` audit row — do not call any front-office endpoint directly
to approve a request; there isn't one (the one that used to exist,
`CashDepositController`'s `POST /authorize`, was removed for bypassing
this engine — see WORKFLOW.md §11).
