# End-to-End Demo / Test Script

A sequential walkthrough of the core SACCO lifecycle against the real
`WebApplication1` API — company/branch setup through loan origination,
repayment, and batch procedures. Every endpoint below is read directly
from the controller source (paths, verbs, request/response shapes), not
guessed — see the "Backend" line under each step for exactly where.

This is written as an **HTTP script** (method + route + body), runnable
via `curl`/Postman/PowerShell `Invoke-RestMethod` against a running
instance, with the matching **sidebar location** (nav `Code` +
`moduleRouteMap.js` path) noted for whoever wants to click through the
same flow in the `Swizzfinancial-FOSA` frontend instead. Run steps in
order — most later steps depend on IDs returned by earlier ones.

## 0. Prerequisites

- `WebApplication1` running (IIS Express: `iisexpress.exe
  /path:<repo>\WebApplication1 /port:58240`) against a real dev DB.
- A valid user to authenticate as. `POST /api/auth/login` with
  `{ "UserName": "...", "Password": "..." }` returns `{ token, UserName,
  roles }` — send `Authorization: Bearer <token>` on every call below.
- Every response follows `{ success, message, data }` unless noted.
- Replace every `<...Id>` placeholder with the real GUID returned by the
  step that created it.

---

## Phase 1 — Foundational setup

### 1.1 Create a company

- **Backend**: `Areas/Admin/Controllers/CompanyController.cs`, `POST
  api/administration/companies`
- **Sidebar**: nav Code `20003` → `/Administration/Company`
- **Body**:
  ```json
  {
    "Company": {
      "Description": "Demo SACCO Ltd",
      "AddressCity": "Nairobi",
      "AddressStreet": "Kangundo Rd",
      "AddressEmail": "demo@sacco.test",
      "AddressLandLine": "+254700000000"
    }
  }
  ```
- **Verify**: `201`/`200`, `data.Id` present — save as `<companyId>`.

### 1.2 Create a branch

- **Backend**: `Areas/Admin/Controllers/BranchController.cs`, `POST
  api/administration/branches`
- **Sidebar**: nav Code `20004` → `/Administration/Branches`
- **Body**: a plain `BranchDTO`, not wrapped:
  ```json
  {
    "Description": "Demo Main Branch",
    "CompanyId": "<companyId>",
    "AddressCity": "Nairobi",
    "AddressStreet": "Kangundo Rd",
    "AddressEmail": "branch@sacco.test",
    "AddressLandLine": "+254700000001"
  }
  ```
- **Verify**: `201`, `data.Id` present and `data.Code` assigned server-side
  — save as `<branchId>`.

### 1.3 Create a loan product

- **Backend**: `Areas/Accounts/Controllers/LoanProductController.cs`,
  `POST api/accounts/loanproducts`
- **Sidebar**: nav Code `23018` → `/Accounts/LoanProducts`
- **Body**: sub-collections (`Deductibles`/`LoanCycles`/etc.) are optional
  — the fields below are the ones the loan-origination guarantor checks
  in §4 actually read, not the full ~40-field set:
  ```json
  {
    "LoanProduct": {
      "Description": "Demo Development Loan",
      "LoanRegistrationTermInMonths": 12,
      "LoanRegistrationMinimumAmount": 5000,
      "LoanRegistrationMaximumAmount": 500000,
      "LoanRegistrationMinimumMembershipPeriod": 0,
      "LoanRegistrationMinimumGuarantors": 1,
      "LoanRegistrationMaximumGuarantees": 3,
      "LoanRegistrationSecurityRequired": true,
      "LoanRegistrationAllowSelfGuarantee": false,
      "LoanRegistrationGuarantorSecurityMode": 0,
      "LoanRegistrationMicrocredit": false,
      "LoanInterestAnnualPercentageRate": 12.0,
      "LoanInterestCalculationMode": 0,
      "LoanInterestChargeMode": 0,
      "LoanInterestRecoveryMode": 0,
      "LoanRegistrationPaymentFrequencyPerYear": 12
    }
  }
  ```
  `LoanRegistrationGuarantorSecurityMode: 0` = `Investments` — makes the
  "total amount guaranteed must cover amount applied" check in §4.3 real;
  set `LoanRegistrationMinimumMembershipPeriod: 0` so a freshly-created
  demo customer isn't blocked by the membership-period gate.
- **Verify**: `data.Id` present — save as `<loanProductId>`.

### 1.4 Create a fixed deposit type

- **Backend**: `Areas/Accounts/Controllers/FixedDepositTypeController.cs`,
  `POST api/accounts/fixeddeposittypes`
- **Sidebar**: nav Code `23029` → `/Accounts/FixedDepositTypes`
- **Body**:
  ```json
  {
    "FixedDepositType": { "Description": "12-Month Term Deposit", "Months": 12, "IsLocked": false },
    "EnforceFixedDepositBands": false
  }
  ```
- **Verify**: `data.Id` present — save as `<fixedDepositTypeId>`.
- **Prerequisite for §3.4 to actually post**: `SystemGeneralLedgerAccountCode.FixedDeposit`
  (48832) and `.FixedDepositInterest` (48833) must be mapped to a real
  Chart of Account — check
  `GET api/accounts/chartofaccounts/mappings` (`ChartOfAccountController`,
  nav Code `23006`) and add both if missing. Without this, §3.4's Verify
  step fails with "the requisite fixed deposit control account has not
  been set up" — a real, previously-hit gap, not hypothetical.

### 1.5 Confirm a cheque type exists (for §3.3)

- **Backend**: `Areas/Accounts/Controllers/ChequeTypeController.cs`,
  `GET api/accounts/chequetypes/all`
- **Sidebar**: nav Code `23023` → `/Accounts/ChequeTypes`
- If empty, `POST api/accounts/chequetypes` to create one — the cheque
  deposit in §3.3 can also be sent with no `ChequeType` at all (matures
  same-day), so this step is optional.

---

## Phase 2 — Customer onboarding

### 2.1 Create a customer

- **Backend**: `Areas/Registry/Controllers/CustomerController.cs`, `POST
  api/registry/customer`
- **Sidebar**: nav Code `21007` → `/Registry/Customers`
- **Body**:
  ```json
  {
    "Customer": {
      "Type": 0,
      "BranchId": "<branchId>",
      "IndividualSalutation": 0,
      "IndividualFirstName": "Demo",
      "IndividualLastName": "Member",
      "IndividualGender": 1,
      "IndividualMaritalStatus": 0,
      "IndividualIdentityCardType": 1,
      "IndividualIdentityCardNumber": "99999999",
      "IndividualBirthDate": "1990-01-01T00:00:00",
      "AddressCity": "Nairobi",
      "AddressStreet": "Kangundo Rd",
      "AddressEmail": "demo.member@sacco.test",
      "AddressMobileLine": "+254711111111"
    },
    "ModuleNavigationItemCode": 21007
  }
  ```
  `Type: 0` = `Individual`. For a non-individual customer use `Type: 1/2/3`
  (Partnership/Corporation/MicroCredit) and the `NonIndividual*` fields
  instead of `Individual*`.
- **Verify**: `201`, `data.Id` present — save as `<customerId>`. The
  customer record itself is created directly, but note (§4.1) that a loan
  can't be registered until the customer's `RecordStatus` is `Approved`
  (`2`) — check whether your build auto-approves on create or needs a
  separate approval step before proceeding to loan origination.

### 2.2 Add accounts for the customer

Two ways, both on `Areas/Accounts/Controllers/CustomerAccountsController.cs`:

**2.2a — Bulk-create one account per product attached to the branch's company** (simplest for a demo):
- `POST api/accounts/customer-accounts/customer/<customerId>/branch/<branchId>`
  (no body)
- **Verify**: `data` is the customer's full account list. Pick the savings
  account's `Id` — save as `<savingsAccountId>`. This is also the account
  §4.1's loan registration needs as `SavingsProductId` (the *product*,
  not the account — see §4.1).

**2.2b — Add one specific product's account** (if the company has no
products attached, or you want a specific one):
- `POST api/accounts/customer-accounts`
  ```json
  {
    "CustomerId": "<customerId>",
    "BranchId": "<branchId>",
    "CustomerAccountTypeTargetProductId": "<savingsProductId>"
  }
  ```
- **Sidebar** (view accounts either way): nav Code `23044` →
  `/Accounts/CustomerAccounts`

---

## Phase 3 — Front-office cash operations

All four of these share one controller,
`Areas/FrontOffice/Controllers/CashDepositController.cs`,
`POST api/frontoffice/requests`. You need:
- `<postingPeriodId>` — `GET api/accounts/postingperiods` (an active,
  non-closed period covering today).
- A user with the **Teller** role, whose JWT `EmployeeId` claim matches a
  real `swiftFin_Tellers` row (`GET api/frontoffice/tellers`) — a
  non-teller user gets `"Teller is missing"`.

**Response shape reminder (real bug fixed 2026-08-16, verify it's present
in your build)**: an over-limit deposit/withdrawal must come back as
`{ success: false, data: { dialog: true, ... } }`, never `{ success:
true, data: null }` — see `CashDepositController.cs`'s `ProcessCustomerTransactionAsync`.
If you see `success: true, data: null` for a large amount, that fix isn't
in your build.

### 3.1 Cash deposit

- **Sidebar**: nav Code `25006` → `/FrontOffice/SavingsReceiptsPayments`
- **Body**:
  ```json
  {
    "Type": 2,
    "CreditCustomerAccountId": "<savingsAccountId>",
    "BranchId": "<branchId>",
    "PostingPeriodId": "<postingPeriodId>",
    "TotalValue": 50000,
    "PrimaryDescription": "Demo cash deposit",
    "SecondaryDescription": "Funding for demo",
    "Reference": "DEMO-DEP-001"
  }
  ```
- **Verify**: `success: true`, `data` is a real `JournalDTO` with
  `TransactionCode: 2` ("Cash Deposit (Customer)"). Keep `TotalValue`
  under the savings product's `MaximumAllowedDeposit` (check
  `GET api/accounts/savingsproducts/<id>`) or you'll hit the
  authorization-required path instead of a direct post.

### 3.2 Cash withdrawal

- **Sidebar**: same screen as 3.1
- **Body**: `Type: 1`, `DebitCustomerAccountId` in place of
  `CreditCustomerAccountId`, everything else the same shape as 3.1.
- **Verify**: `TransactionCode: 1` ("Cash Withdrawal"), account balance
  reduced by `TotalValue`.

### 3.3 Cheque deposit

- **Sidebar**: same screen as 3.1
- **Body**:
  ```json
  {
    "Type": 3,
    "CreditCustomerAccountId": "<savingsAccountId>",
    "BranchId": "<branchId>",
    "PostingPeriodId": "<postingPeriodId>",
    "TotalValue": 20000,
    "PrimaryDescription": "Demo cheque deposit",
    "Reference": "000123",
    "Drawer": "Some Payer Ltd",
    "DrawerBank": "Demo Bank",
    "DrawerBankBranch": "Nairobi",
    "WriteDate": "2026-08-16T00:00:00",
    "ChequeType": null
  }
  ```
- **Verify** (real, documented behavior — not a bug): the deposited
  amount posts to the `ExternalChequesControl` suspense account, **not**
  the customer's spendable balance yet — `AvailableBalance` should be
  unchanged immediately after this call. It only becomes spendable once
  transferred/banked/cleared via `Areas/FrontOffice/Controllers/ChequesController.cs`
  (nav Code `25011` → `/FrontOffice/Cheques`, Catalogue → Bank → Clear
  tabs).

### 3.4 Fixed deposit — full lifecycle

- **Backend**: `Areas/FrontOffice/Controllers/FixedDepositController.cs`
- **Sidebar**: nav Code `25012` → `/FrontOffice/FixedDeposits`

**a) Originate** — `POST api/frontoffice/fixeddeposits`
```json
{
  "FixedDepositTypeId": "<fixedDepositTypeId>",
  "BranchId": "<branchId>",
  "CustomerAccountId": "<savingsAccountId>",
  "Category": 0,
  "MaturityAction": 0,
  "Value": 20000,
  "Term": 12,
  "Rate": 10.0,
  "Remarks": "Demo fixed deposit"
}
```
`Category: 0` = Term Deposit, `MaturityAction: 0` = Pay Principal &
Interest Due at maturity. Save `data.Id` as `<fixedDepositId>`. Status
should be `8` ("New").

**b) Verify/post** — `POST api/frontoffice/fixeddeposits/<fixedDepositId>/verify`
```json
{ "Approve": true, "ModuleNavigationItemCode": 25012 }
```
Requires §1.4's GL mappings. Status flips to `1` ("Running"); a real
journal posts (`Fdr Fixing`) debiting the customer, crediting the Fixed
Deposit control account. Confirm the customer's savings balance dropped
by `Value`.

**c) Early termination** (since maturity is a year out, this is the only
maturity-adjacent path testable without manipulating the DB clock) —
`POST api/frontoffice/fixeddeposits/terminate`
```json
{ "SelectedFixedDepositIds": ["<fixedDepositId>"], "ModuleNavigationItemCode": 25012 }
```
Status flips to `4` ("Revoked"), a reversing `Fdr Termination` journal
posts, principal returns to the customer's savings balance.

---

## Phase 4 — Loan origination pipeline

**Backend**: `Areas/BackOffice/Controllers/LoanCaseController.cs` for
everything in this phase.

### 4.1 (Optional) Look up a prospective guarantor before attaching

- `GET api/backoffice/loancases/guarantors/lookup?guarantorId=<id>&loanProductId=<loanProductId>`
- Returns `totalShares`/`committedShares`/`appraisalFactor`/`availableToGuarantee`
  computed the same way §4.2's real attach does — use this to sanity-check a
  candidate before submitting.

### 4.2 Register the loan (take a loan)

- **Sidebar**: nav Code `70007` → `/Loaning/LoanCases/registration`
- `POST api/backoffice/loancases`
  ```json
  {
    "LoanCase": {
      "CustomerId": "<customerId>",
      "LoanProductId": "<loanProductId>",
      "SavingsProductId": "<savingsProductId>",
      "BranchId": "<branchId>",
      "LoanPurposeId": "<loanPurposeId>",
      "RegistrationRemarkId": "<registrationRemarkId>",
      "AmountApplied": 100000
    },
    "Guarantors": [
      { "GuarantorId": "<guarantorCustomerId>", "AmountGuaranteed": 100000 }
    ],
    "CollateralDocumentIds": []
  }
  ```
  `SavingsProductId` is the **product** id (e.g. from
  `GET api/accounts/savingsproducts`), not the customer's own account id.
  `LoanPurposeId`/`RegistrationRemarkId` come from
  `GET api/backoffice/loanpurposes` / `.../loaningremarks` (both full CRUD
  catalogues, nav-mapped — check `docs/api/loan-backoffice-catalogues-api-spec.md`).
- **Guarantor checks actually enforced server-side** (`EnrichAndValidateGuarantors`,
  read directly from source — verify each on purpose during the demo):
  - Guarantor count must be within `[LoanRegistrationMinimumGuarantors,
    LoanRegistrationMaximumGuarantees]` — unless `LoanRegistrationMicrocredit`
    is set or `LoanRegistrationSecurityRequired` is false.
  - Every `GuarantorId` must resolve to a real customer.
  - Self-guarantee (`GuarantorId == CustomerId`) is rejected unless the
    product's `LoanRegistrationAllowSelfGuarantee` is true.
  - `TotalShares`/`CommittedShares`/`AppraisalFactor` are **recomputed
    server-side** from the guarantor's real savings+investment balances
    and existing pledges — values you send for these are ignored, don't
    bother sending them.
  - If `LoanRegistrationGuarantorSecurityMode == Investments` (0) and
    security is required: `sum(AmountGuaranteed)` across all guarantors
    must be `>= AmountApplied`, or the whole request is rejected with
    "does not fully secure the amount applied".
  - **To see a check actually fire**: try self-guaranteeing on a product
    with `LoanRegistrationAllowSelfGuarantee: false` (§1.3's example), or
    submit `AmountGuaranteed` below `AmountApplied` — expect a `400` with
    the specific message above, not a silent accept.
- **Verify**: status `Registered` (per `LoanCaseStatus`), save
  `data.Id` as `<loanCaseId>`.

### 4.3 Appraise

- **Sidebar**: nav Code `70008` → `/Loaning/LoanCases/appraisal`
- (Optional) `GET api/backoffice/loancases/<loanCaseId>/appraisal-worksheet`
  first — real computed figures (max via investments multiplier,
  outstanding balance, amortization) to base the appraisal on.
- `POST api/backoffice/loancases/<loanCaseId>/appraise`
  ```json
  {
    "Option": 1,
    "AppraisedAmount": 100000,
    "AppraisedAmountRemarks": "Within entitlement",
    "AppraisalRemarks": "Appraised for demo",
    "MonthlyPaybackAmount": 9500,
    "TotalPaybackAmount": 114000
  }
  ```
  `Option: 1` = Appraise, `2` = Reject.
- **Verify**: status `Appraised`.

### 4.4 Approve

- **Sidebar**: nav Code `70009` → `/Loaning/LoanCases/approval`
- `POST api/backoffice/loancases/<loanCaseId>/approve`
  ```json
  {
    "Option": 1,
    "ApprovedAmount": 100000,
    "ApprovedAmountRemarks": "Approved as applied",
    "ApprovedPrincipalPayment": 8333,
    "ApprovedInterestPayment": 1167,
    "MonthlyPaybackAmount": 9500,
    "TotalPaybackAmount": 114000,
    "ApprovalRemarks": "Approved for demo"
  }
  ```
  `Option: 1` = Approve, `2` = Reject, `4` = Defer.
- **Verify**: status `Approved` — **unless** the loan product has
  `LoanRegistrationBypassAudit` set, in which case this call auto-chains
  into audit and status may already be `Audited`; the response `message`
  says so explicitly if that happened.

### 4.5 Audit / verify (creates the repayment schedule)

- **Sidebar**: nav Code `70010` → `/Loaning/LoanCases/audit`
- `POST api/backoffice/loancases/<loanCaseId>/audit`
  ```json
  { "Option": 1, "AuditRemarks": "Verified for demo" }
  ```
  `Option: 1` = Audit ("Verify" in the UI), `2` = Reject, `4` = Defer.
- **This is the consequential step** — real domain logic, not just a
  status flip: creates the customer's loan/savings `CustomerAccount`s if
  missing, computes the repayment PV/PMT, recovers upfront dynamic
  charges, and **creates the repayment schedule as a `StandingOrder`**.
  Treat it as a black box, don't try to precompute its result.
- **Verify**: status `Audited`. `message` confirms the standing order was
  set up if `LoanRegistrationCreateStandingOrderOnLoanAudit` is true on
  the product. Look up the new standing order:
  `GET api/accounts/standingorders?customerAccountId=<loanAccountId>`
  (nav Code `23048` → `/Accounts/StandingOrders`) — **this is the
  "create a schedule for repayment" step**, done implicitly here, not a
  separate call.

### 4.6 Disburse

Loan disbursement in this repo is a **batch procedure**, not a single
loan-case action:

- **Backend**: `Areas/Accounts/Controllers/LoanDisbursementBatchController.cs`
- Create a batch (`POST api/accounts/loandisbursementbatches`), add
  `<loanCaseId>` as an entry (`POST .../{id}/entries`), `audit` then
  `authorize` the batch — `authorize` queues each entry for async
  posting (via `SwiftFinancials.LoanDisbursementPosting`, needs
  `SwiftFinancials.WindowsService` running to actually process the
  queue). Full detail: `docs/api/batch-procedures-api-spec.md` §6.
- **Verify**: once processed, the loan case status is `Disbursed`, a real
  disbursement journal posted, and money landed in the customer's loan
  account.

---

## Phase 5 — Repayment

### 5.1 Make a repayment / trigger the standing order

The repayment schedule created in §4.5 is a `StandingOrder` — repayments
are collected by **executing** it, the same engine that also handles
"trigger standing order" generically. One call covers both asks:

- **Backend**: `Areas/Accounts/Controllers/StandingOrderExecutionController.cs`,
  `POST api/accounts/standingorders/execution/execute`
- **Sidebar**: nav Code `23039` → `/Accounts/StandingOrders/Execution`
- **Body**:
  ```json
  { "TargetDate": "2026-08-16T00:00:00", "TargetDateOption": 0, "Priority": 0, "MaximumStandingOrderExecuteAttemptCount": 3, "PageSize": 100 }
  ```
  This executes **every** standing order due on `TargetDate`, not just
  the demo loan's — expected for a real batch-style engine. To confirm
  it fired for this specific loan, check the loan account's balance / the
  standing order's own execution history before and after.
- **Verify**: `success: true`, `"Standing orders executed successfully"`.
  Loan account balance reduced by the installment amount, a repayment
  journal posted.

---

## Phase 6 — Checkoff receipt & payout batch

Both are **`Areas/Accounts/Controllers/CreditBatchController.cs`**
batches, distinguished only by `CreditBatchDTO.Type`
(`CreditBatchType`: `Payout = 0xDADA`, `CheckOff = 0xDADA+1`).

### 6.1 Create the batch

`POST api/accounts/creditbatches`
```json
{
  "Type": 56027,
  "BranchId": "<branchId>",
  "TotalValue": 50000,
  "Reference": "Demo checkoff batch",
  "Priority": 0
}
```
`56027` = `0xDADA+1` = CheckOff. Use `56026` (`0xDADA`) for a Payout
batch instead — otherwise identical flow. Save `data.Id` as `<batchId>`.

### 6.2 Add an entry

`POST api/accounts/creditbatches/<batchId>/entries`
```json
{ "CustomerAccountId": "<savingsAccountId>", "Amount": 50000, "Description": "Demo checkoff" }
```

### 6.3 Audit then authorize

- `POST api/accounts/creditbatches/<batchId>/audit` — `{ "Option": 1 }`
  (moves `Pending` → `Audited`, only if entries total `<=` batch
  `TotalValue`)
- `POST api/accounts/creditbatches/<batchId>/authorize` — `{ "Option": 1,
  "ModuleNavigationItemCode": 0 }` (moves `Audited` → `Posted`; CheckOff
  and Payout entries get queued for async posting via
  `SwiftFinancials.CreditBatchPosting`)
- **Verify**: batch status `Posted`. Once the queue processes (needs
  `WindowsService` running), the entry posts a real journal and the
  member's loan/savings balance reflects the checkoff.

---

## Phase 7 — Verification / oversight screens

### 7.1 Audit trails

- **Backend**: `Areas/Admin/Controllers/AuditLogsController.cs`, `GET
  api/administration/auditlogs`
- **Sidebar**: nav Code `20008` → `/Administration/AuditLogs`
- **Verify**: every write action performed in Phases 1–6 above shows up
  here with the acting user, timestamp, and affected record.

### 7.2 Workflows (maker-checker queue)

- **Backend**: `Areas/Workflows/Controllers/WorkflowController.cs`
- **Sidebar**: nav Code `26015` → `/CommandHub/ApprovalRequests`
- `GET api/administration/workflows/items/mine` — items awaiting the
  current user's approval.
- `POST api/administration/workflows/items/approve` — approve/reject a
  pending item (e.g. an over-limit cash deposit from §3.1/3.2, if you
  deliberately exceeded the product's `MaximumAllowedDeposit` to trigger
  one).
- **Verify**: an approved item's underlying transaction actually posts
  (for a cash request, follow up with `POST api/frontoffice/requests/post?id=<requestId>`
  per §3's controller — approval alone doesn't post it).

### 7.3 Alternate channels — **known gap, verify it's still a gap**

- **Sidebar**: nav Codes `23053`/`23054`/`23014` →
  `/Accounts/AlternateChannels/Register` /
  `.../Management` / `.../Fees`
- **No backing `WebApplication1` controller exists for any of these
  routes** (confirmed by source search, 2026-08-16 — no
  `AlternateChannel*Controller.cs` anywhere under `WebApplication1/Areas`,
  despite the domain/app-service layer for it being fully built:
  `Application.MainBoundedContext/AccountsModule/Services/AlternateChannelAppService.cs`
  and friends). The `docs/api/alternate-channel-api-spec.md` referenced
  in the frontend's `moduleRouteMap.js` comments doesn't exist either.
- **What to actually check in the demo**: confirm these sidebar entries
  still 404 or show an empty/placeholder screen, not that they work —
  if someone has since built the controller, update this section and
  the "not built" framing above.

---

## Summary checklist

| # | Step | Endpoint | Nav Code |
|---|---|---|---|
| 1.1 | Create company | `POST api/administration/companies` | 20003 |
| 1.2 | Create branch | `POST api/administration/branches` | 20004 |
| 1.3 | Create loan product | `POST api/accounts/loanproducts` | 23018 |
| 1.4 | Create fixed deposit type | `POST api/accounts/fixeddeposittypes` | 23029 |
| 2.1 | Create customer | `POST api/registry/customer` | 21007 |
| 2.2 | Add customer account(s) | `POST api/accounts/customer-accounts/customer/{id}/branch/{id}` | 23044 |
| 3.1 | Cash deposit | `POST api/frontoffice/requests` (Type 2) | 25006 |
| 3.2 | Cash withdrawal | `POST api/frontoffice/requests` (Type 1) | 25006 |
| 3.3 | Cheque deposit | `POST api/frontoffice/requests` (Type 3) | 25006 |
| 3.4 | Fixed deposit lifecycle | `POST/.../verify/.../terminate api/frontoffice/fixeddeposits` | 25012 |
| 4.2 | Register loan + guarantors | `POST api/backoffice/loancases` | 70007 |
| 4.3 | Appraise loan | `POST api/backoffice/loancases/{id}/appraise` | 70008 |
| 4.4 | Approve loan | `POST api/backoffice/loancases/{id}/approve` | 70009 |
| 4.5 | Audit loan (creates repayment schedule) | `POST api/backoffice/loancases/{id}/audit` | 70010 |
| 4.6 | Disburse loan | `LoanDisbursementBatchController` batch flow | — |
| 5.1 | Repayment / trigger standing order | `POST api/accounts/standingorders/execution/execute` | 23039 |
| 6 | Checkoff receipt / payout batch | `CreditBatchController` batch flow | — |
| 7.1 | Audit trails | `GET api/administration/auditlogs` | 20008 |
| 7.2 | Workflows | `Areas/Workflows/Controllers/WorkflowController.cs` | 26015 |
| 7.3 | Alternate channels | *(no backend yet — verify still a gap)* | 23053/23054/23014 |
