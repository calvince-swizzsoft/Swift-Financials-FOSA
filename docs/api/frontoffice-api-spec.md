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

Query params: `type` (`2` = CashDeposit, `1` = CashWithdrawal — required to
get results), `status` (optional, defaults to `Pending` — pass explicitly
for `Authorized`/`Posted`/`Paid`/`Rejected`), `text`, `startDate`, `endDate`,
`pageIndex`, `pageSize`. Returns `PageCollectionInfo<CashDepositRequestDTO>`
or `PageCollectionInfo<CashWithdrawalRequestDTO>` under `data`, each row's
`CustomerName` populated server-side.

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
  (§7 below) — nothing further to call here until a checker approves it.
- **Blocked/failed**: `success: false`, `message` explains why (teller
  locked, account not approved, below minimum balance, teller range limit,
  validation errors), `data: null`.

### 4.3 Post an authorized request — `POST /post?id={requestId}`

Call once a checker has approved the request (via the generic workflow
endpoint, §7). Re-derives the transaction from the now-`Authorized` request
and posts the GL journal. `400` if the request isn't `Authorized` yet.
Response: `data` is the `JournalDTO` on success.

### 4.4 Mark posted without re-deriving — `POST /markposted?id={requestId}`

Flips an `Authorized` request straight to `Posted`/`Paid` without building a
new journal — only use this if the journal was already posted through
another path and this is purely a status correction.

There is **no** `POST /authorize` endpoint — see §7.

---

## 5. Treasury cash movement — `api/frontoffice/cashmanagement`

Controller: `CashManagementController.cs`.

### 5.1 Post a movement — `POST /`

Body: `FiscalCountDTO` — `TransactionType` (`TreasuryTransactionType`:
`BankToTreasury`/`TreasuryToBank`/`TreasuryToTeller`/`TreasuryToTreasury`),
denomination breakdown fields (`DenominationOneThousandValue` ... down to
50-cent), `TotalValue`. `400` if outgoing and the treasury's book balance
is insufficient.

---

## 6. Treasury master data — `api/frontoffice/treasurys`

Controller: `TreasurysController.cs`. `GET /` (list), `POST /` (create),
`PUT /{id}` (update) against `TreasuryDTO`.

---

## 7. Teller master data — `api/frontoffice/tellers`

Controller: `TellerController.cs`. `GET /` (list), `POST /` (create — GL
wiring auto-derived from `TellerType`), `PUT /{id}` (update),
`GET /{id}` (single), `GET /teller?employeeId={id}` (lookup by employee —
an admin/support lookup, not the "who am I" pattern in §3).

---

## 8. Cash transfers & cheque transfer batch — `api/frontoffice/transfers`

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

---

## 9. Cheque banking & clearance — `api/frontoffice/cheques`

Controller: `ChequesController.cs`.

- `GET /?text=&pageIndex=&pageSize=` — paged cheque list.
- `POST /bank` — body `{ selectedChequeIds: Guid[], bankLinkageDTO }`.
- `POST /clear` — body `{ selectedChequeIds: Guid[], clearingOption,
  actionType: "clear"|"unpay", unPayReasonDTO }` (`unPayReasonDTO` required
  when `actionType` is `"unpay"`).
- `GET /untransfered?teller={id}` — untransferred cheques for a teller.

---

## 10. End of Day close — `api/frontoffice/endofday`

Controller: `EndOfDayController.cs`.

### `POST /`

Body: `CashTransferRequestDTO` — `UntransferredChequesValue`,
`TellerCashBalanceStatusValue` (`TellerCashBalanceStatus`:
`Balanced`/`Shortage`/`Excess`), `ClosingBalance`, `BookBalance`. The
teller is always the caller's own (§3) — any `TellerId` in the body is
overwritten. Enforces, in order: cheques transferred (`400` if not),
EOD not already run today (`400` if it has), then posts the close journal
(+ suspense entry to `Teller.ShortageChartOfAccountId`/
`ExcessChartOfAccountId` if unbalanced). Success: `data` is the closing
`JournalDTO` — render/print the EOD receipt from this; there is no
server-side print endpoint (removed — see WORKFLOW.md §11).

---

## 11. Account closure — `api/frontoffice/accountclosures`

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

## 12. Fixed deposits — `api/frontoffice/fixeddeposits`

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

## 13. Expense payables — `api/frontoffice/expensepayables`

Controller: `ExpensePayableController.cs`. Sequence: Create (`Pending`) →
add entry lines → Verify (`Audited`/`Rejected`/`Deferred`) → **approval
happens through the generic workflow engine, not this controller** (§7 —
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

## 14. Sundry payments & customer receipts

Controllers: `SundryPaymentsController.cs`
(`api/frontoffice/sundrypayments`), `CustomerReceiptsController.cs`
(`api/frontoffice/customerreceipts`). Both post a **single-line** GL
journal against the caller's own teller cash account — no dedicated app
service backs either (same as the reference controllers, which posted
straight through the shared journal service).

### 14.1 `POST /` (both controllers)

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

## 15. In-house cheques — `api/frontoffice/inhousecheques`

Controller: `InHouseController.cs`.

| Route | Method | Purpose |
|---|---|---|
| `/?text=&startDate=&endDate=&pageIndex=&pageSize=` | GET | Paged list |
| `/{id}` | GET | Single cheque |
| `/unprinted?branchId={id}&text=&pageIndex=&pageSize=` | GET | Printing queue for a branch |
| `/` | POST | `{ cheques: InHouseChequeDTO[], moduleNavigationItemCode }` — batch-build entries |
| `/{id}/print` | POST | `{ printedNumber, bankLinkage: BankLinkageDTO, moduleNavigationItemCode }` — flips `IsPrinted`/`PrintedNumber` and posts the GL journal. The client renders/prints the cheque itself and reports back the printed number — this endpoint does no printing |

---

## 16. Automated (image-based) clearing — `api/frontoffice/automatedclearing`

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

## 17. Fiscal counts (standalone) — `api/frontoffice/fiscalcounts`

Controller: `FiscalCountController.cs`. A browse/manual-entry view over
denomination-count records — normal posting happens inline via treasury
cash movement (§5) or EOD close (§10), both of which build their own
`FiscalCountDTO`.

- `GET /?text=&startDate=&endDate=&pageIndex=&pageSize=` — paged list.
- `GET /{id}` — single record.
- `POST /` — manual entry (`FiscalCountDTO`).

---

## 18. Maker-checker — the generic workflow engine

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
