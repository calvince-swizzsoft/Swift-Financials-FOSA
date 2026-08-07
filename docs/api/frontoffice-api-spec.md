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
for cheque deposits, `PaymentVoucher` for voucher withdrawals).

Response shape varies by outcome:
- **Posted directly** (within limits): `data` is the `JournalDTO` (id,
  sequential id, branch/posting-period/user descriptions, amount,
  reference, created date) — everything needed to render a receipt.
- **Authorization required** (above limit/below minimum/overdraft/voucher):
  `success: false`, `data: { dialog: true, isCashDepositRequest |
  isCashWithdrawalRequest: true, cashTransactionRequestId,
  selectedCustomerAccountId, transactionTotalValue, transactionReference,
  transactionCategory, ...paymentVoucher fields for withdrawals }`. The
  request is now `Pending` and enqueued into the generic workflow engine
  (§17 below) — nothing further to call here until a checker approves it.
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
`BankToTreasury`/`TreasuryToBank`/`TreasuryToTeller`/`TreasuryToTreasury`),
denomination breakdown fields (`DenominationOneThousandValue` ... down to
50-cent), `TotalValue`.

**Status codes, checked directly against every `return` in `Create`/
`DoSomething`**: only a missing posting period or treasury returns a real
`400` (`BadRequest`). Every other failure on this endpoint — insufficient
book balance on an outgoing transfer (`TreasuryToTeller`/`TreasuryToBank`/
`TreasuryToTreasury`; `BankToTreasury` has no such check), bank/treasury
not found, a denomination mismatch, an unhandled exception — returns HTTP
`200` with `success: false` in the body. **Check `success`, not HTTP
status, everywhere on this endpoint except that one case.**

The denomination fields and `TotalValue` **are cross-validated**: each
denomination field holds that denomination's own monetary subtotal (not a
raw note/coin piece count — e.g. `DenominationOneThousandValue` is "how
much was counted in 1000-notes"), and the eleven subtotals must sum to
exactly `TotalValue` (`Utils.SumDenominationValues`) or the call fails
(`success: false`, HTTP `200`) before anything is posted. The denomination
breakdown is then persisted as a separate physical-count audit record
(`FiscalCountAppService.AddNewFiscalCounts`) alongside the GL journal.

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
| `/cheques?TellerId={id}` | GET | Untransferred-cheques summary for a teller |
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

`CashTransferRequest` itself has no columns for a denomination breakdown —
on success, the counted denominations are written as a **companion**
`FiscalCount` record (`TransactionCode = TellerCashTransfer`,
`ChartOfAccountId` = the caller's own teller account), not onto the
request record. There's no endpoint to fetch that companion record back
via the transfer request itself; query it through
`GET /api/frontoffice/fiscalcounts` (§16) if needed.

---

## 8. Cheque banking & clearance — `api/frontoffice/cheques`

Controller: `ChequesController.cs`.

- `GET /?text=&pageIndex=&pageSize=` — paged cheque list.
- `POST /bank` — body `{ selectedChequeIds: Guid[], bankLinkageDTO }`.
- `POST /clear` — body `{ selectedChequeIds: Guid[], clearingOption,
  actionType: "clear"|"unpay", unPayReasonDTO }` (`unPayReasonDTO` required
  when `actionType` is `"unpay"`).
- `GET /untransfered?teller={id}` — untransferred cheques for a teller.

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

Enforces, in order: malformed input (`400`) → denomination reconciliation
(`400`) → caller has a linked teller record (`400`) → posting-model
validation → cheques transferred → EOD not already run today → writes a
`FiscalCount` record (`TransactionCode = TellerEndOfDay` — this is also
what `IsEndOfDayExecutedAsync` checks for, so a closed day actually stays
closed) → posts the close journal (+ suspense entry to
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
| `/{id}/settle` | POST | `{ option, remarks }` — `AccountClosureSettlementOption`: `1`=Settle, `2`=Defer. Only accepts `Audited`. Pays out remaining balance and closes the account |

Each transition returns `409` (not `400`) if the request isn't in the
right status for that action.

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
`2`=ChequeReceipt, `4`=CashPayment, `8`=CashPickup — direction of the
debit/credit against the teller account is derived from this). Response
`data` is the posted `JournalDTO`.

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
| `/` | POST | `{ cheques: InHouseChequeDTO[], moduleNavigationItemCode }` — batch-build entries |
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

Controller: `FiscalCountController.cs`. A browse/manual-entry view over
denomination-count records — normal posting happens inline via treasury
cash movement (§5), EOD close (§9), or a cash transfer request (§7), all of
which build/persist their own `FiscalCountDTO`.

- `GET /?text=&startDate=&endDate=&pageIndex=&pageSize=` — paged list.
- `GET /{id}` — single record.
- `POST /` — manual entry (`FiscalCountDTO`). Same reconciliation rule as
  every other entry point into `FiscalCount`: the eleven
  `Denomination*Value` subtotals must sum to `TotalValue`, `400` otherwise.

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
