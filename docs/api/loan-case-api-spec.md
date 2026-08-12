# Loan Case API

Base path: `api/backoffice/loancases`. Controller:
`WebApplication1/Areas/BackOffice/Controllers/LoanCaseController.cs`.
Functional design: `WebApplication1/Areas/BackOffice/WORKFLOW.md` §5 (loan
request intake, upstream of this), §14.1 (registration, what was built and
why), and §14.2 (appraisal).

This covers the first two stages of the loan origination pipeline: opening
a loan case with its guarantors and collateral, and appraising it.
Approval and audit/verification (`LoanCaseStatus.Appraised → Approved →
Audited`) are separate, not-yet-built stages — see `WORKFLOW.md` §7-8.
Disbursement (`LoanCaseStatus.Approved`/`Audited → Disbursed`) is already
documented separately: `batch-procedures-api-spec.md` §6.

## Conventions

Standard envelope (`{ success, message, data }`), standard paging shape,
standard status codes — see `docs/api/README.md`. All endpoints require a
bearer JWT.

## 1. List loan cases

`GET /?status={LoanCaseStatus}&text=&loanCaseFilter=0&pageIndex=0&pageSize=20&includeBatchStatus=true`

`status` defaults to `0` (`Registered`) — the "just opened, not yet
appraised" queue, matching the reference app's default Index screen.
`loanCaseFilter` selects what `text` searches against
(`LoanCaseFilter`: `CaseNumber`/`Reference`/`Branch`/`LoanPurpose`/...).
Returns `PageCollectionInfo<LoanCaseDTO>`.

## 2. Get one loan case

`GET /{id}` → `{ loanCase: LoanCaseDTO, guarantors: LoanGuarantorDTO[], collaterals: LoanCollateralDTO[] }`.
`404` if not found. `GET /{id}/guarantors` and `GET /{id}/collaterals` are
also available standalone.

## 3. In-process applications for a customer

`GET /customers/{customerId}/in-process` → `LoanCaseDTO[]`.

Same check `AddNewLoanCase` itself enforces server-side (a customer can't
have two concurrent applications for the same product) — call this before
submitting a new case to warn the loan officer early, rather than
discovering it from a `409` on `POST /`.

## 4. Guarantor eligibility lookup

`GET /guarantors/lookup?guarantorId={id}&loanProductId={id}`

Resolves a prospective guarantor's real share balance and what they've
already committed elsewhere, computed the same way `Create` computes it
(not decorative):

```json
{
  "guarantorId": "...",
  "serialNumber": 1042,
  "fullName": "Mr. John Kamau",
  "employerDescription": "...",
  "stationDescription": "...",
  "identificationNumber": "...",
  "payrollNumber": "...",
  "totalShares": 150000.00,
  "committedShares": 40000.00,
  "appraisalFactor": 3.0,
  "availableToGuarantee": 410000.00
}
```

`totalShares` = sum of the guarantor's savings + investment account book
balances. `committedShares` = sum of `AmountGuaranteed` across every other
loan they currently guarantee. `appraisalFactor` comes from
`ILoanProductAppService.GetGuarantorAppraisalFactor`. `availableToGuarantee`
= `(totalShares × appraisalFactor) − committedShares` — the real ceiling
`LoanGuarantorDTO`'s own validator enforces on `Create` (see §5).

`404` if the guarantor customer doesn't exist; `400` if the loan product
doesn't.

## 5. Register a loan case

`POST /`

```json
{
  "loanCase": { "...": "LoanCaseDTO fields — see below" },
  "guarantors": [ { "guarantorId": "...", "amountGuaranteed": 50000.00 } ],
  "collateralDocumentIds": ["..."]
}
```

Required on `loanCase`: `customerId`, `loanProductId`, `savingsProductId`,
`loanPurposeId`, `registrationRemarkId`, `amountApplied`, `branchId`. The
loan officer's branch isn't resolved server-side (no Web API controller in
this repo reads "current user's branch" today, unlike the reference MVC
app's `ApplicationUserManager` lookup) — send it explicitly.

What happens server-side, in order (all real, all enforced — see
`WORKFLOW.md` §14.1 for why this all had to live in the controller rather
than the app service):

1. Customer must exist and be `RecordStatus.Approved`.
2. Loan product, savings product, loan purpose, and registration remark
   (loaning-remark) must each exist.
3. Membership-period gate: customer's account age (months since
   `CreatedDate`) must be ≥ the loan product's
   `LoanRegistrationMinimumMembershipPeriod`.
4. Guarantor count checked against the loan product's
   `LoanRegistrationMinimumGuarantors`/`MaximumGuarantees` (skipped
   entirely for microcredit products or when `LoanRegistrationSecurityRequired`
   is false). Self-guarantee rejected unless
   `LoanRegistrationAllowSelfGuarantee` is set. Each guarantor's
   `totalShares`/`committedShares`/`appraisalFactor` are computed
   server-side (see §4) — **don't bother sending them, they're
   overwritten**. If the product's guarantor security mode is
   `Investments`, the sum of `amountGuaranteed` across all guarantors must
   cover `amountApplied`.
5. Collateral document ids are resolved to real `CustomerDocumentDTO`
   records; unknown ids are silently dropped (not an error — matches how
   the reference screen treated a stale picker selection).
6. The loan product's ~40 registration-time fields (interest rate/mode,
   term, take-home, standing-order trigger, rounding type, etc.) are
   snapshotted onto the loan case — this is what makes an already-open
   case's terms independent of later edits to the product.
7. Full `LoanCaseDTO.ValidateAll()` runs and is actually checked — amount
   applied range, retirement-age restriction, and the security-sufficiency
   rule (§4 above, restated declaratively on the DTO) all gate the create.
   **The reference MVC controller calls `ValidateAll()` too but never
   checks the result — this fixes that, so a request that would have
   silently "succeeded" invalid there gets a real `400` here.**
8. `AddNewLoanCase` persists the case (`Status: Registered`,
   `CaseNumber` server-generated) after its own duplicate-in-process check
   (§3). Guarantors and collateral are then attached via
   `UpdateLoanGuarantors`/`UpdateLoanCollaterals`.

Returns the freshly-fetched `LoanCaseDTO` in `data` on success. `409` if
`AddNewLoanCase`'s own duplicate-application guard fires (message from
`ErrorMessageResult`); `400` for every validation failure above.

Branch budget-balance validation
(`LoanCaseDTO.BranchBudgetBalance`/`BranchCompanyEnforceBudgetControl`) is
intentionally not wired — same as the reference `Create` action. Real
budget-balance computation is `IBudgetAppService`'s job, out of scope here;
those fields stay at their defaults (`false`/`0`) unless a future pass adds
it.

## 6. Update a loan case

`PUT /{id}` — body: `LoanCaseDTO`. Only the plain registration fields
(product/purpose/savings product, amount applied, remarks, interest/term
settings, etc.) are replaced; `status`, `caseNumber`, `createdBy`, and
`createdDate` are always preserved server-side regardless of what's sent.
`404` if the case doesn't exist. There's no dedicated Edit screen in the
reference app either — guarantors/collaterals/appraisal factors each have
their own update entry points, unchanged by this endpoint.

**Real bug fixed in `LoanCaseAppService.UpdateLoanCaseAsync` itself, not
just documented**: two lines used to re-stamp `persisted.CreatedDate` to
`DateTime.UtcNow` immediately after the method had already restored it to
its original value two lines above, and unconditionally set `CancelledBy`
on every plain update, not just cancellations. Both directly contradicted
the method's own preceding comment ("Restore original values that were
overwritten") and were removed — if you were previously seeing a loan
case's `createdDate` drift on every edit, that's why, and it's fixed now.

## 7. Appraisal worksheet

`GET /{id}/appraisal-worksheet`

Read-only. System-computed figures for a `Registered`/`Deferred` case, plus
the case itself and its current guarantors/collaterals/appraisal factors —
gives the appraiser numbers to work from before deciding. `404` if the case
doesn't exist.

```json
{
  "loanCase": { "...": "LoanCaseDTO" },
  "totalShares": 190000.00,
  "investmentsBalance": 40000.00,
  "savingsBalance": 150000.00,
  "maximumLoan": 120000.00,
  "outstandingLoansBalance": 15000.00,
  "maximumEntitled": 105000.00,
  "loanPart": 100000.00,
  "interestPart": 12000.00,
  "loanPlusInterest": 112000.00,
  "paymentPerPeriod": 9333.33,
  "appraisalFactors": [ "...LoanAppraisalFactorDTO[]" ],
  "guarantors": [ "...LoanGuarantorDTO[]" ],
  "collaterals": [ "...LoanCollateralDTO[]" ]
}
```

`maximumLoan` = `investmentsBalance × LoanRegistrationInvestmentsMultiplier`
(the loan product's own multiplier). `outstandingLoansBalance` = book +
carry-forward balance on the customer's existing account for this specific
loan product. `maximumEntitled` = `maximumLoan − outstandingLoansBalance`.
`interestPart` is a simple-interest estimate
(`loanPart × (APR/100) × (termInMonths/12)`), not the amortized figure.
`paymentPerPeriod` is a standard amortization `PMT` off the loan product's
monthly rate and term — `0` if the term or rate is `0`.

`GET /{id}/appraisal-factors` returns just the `appraisalFactors` array
above, standalone.

## 8. Appraise a loan case

`POST /{id}/appraise`

```json
{
  "option": 1,
  "moduleNavigationItemCode": 1234,
  "appraisedNetIncome": 45000.00,
  "appraisedAbility": 15000.00,
  "systemAppraisedAmount": 100000.00,
  "systemAppraisalRemarks": "...",
  "appraisedAmount": 100000.00,
  "appraisedAmountRemarks": "...",
  "appraisalRemarks": "...",
  "monthlyPaybackAmount": 9333.33,
  "totalPaybackAmount": 112000.00,
  "loanProductLatestIncome": 45000.00,
  "totalLoansBalance": 15000.00,
  "incomeAdjustments": [
    { "incomeAdjustmentId": "...", "customerAccountId": null, "amount": 5000.00, "isEnabled": true }
  ]
}
```

`option`: `LoanAppraisalOption` — `1` = Appraise (→ `Appraised`), `2` =
Reject (→ `Rejected`, releases the case's guarantors). Requires the case to
currently be `Registered` or `Deferred` — `409` otherwise. `incomeAdjustments`
is only applied when `option` is Appraise (a rejection has no appraisal
figures worth keeping); each entry's `incomeAdjustmentId` must resolve to a
real `IncomeAdjustmentDTO` (its `description`/`type` are filled in
server-side, don't bother sending them) and the same id can't appear twice
in one request. Returns the freshly-fetched `LoanCaseDTO` on success.

**Real bug fixed in `LoanCaseAppService.AppraiseLoanCase`/`Async`
themselves, not just the controller**: the guard clause used to force-set
`persisted.Status` to `Registered` *before even null-checking* the fetched
entity — appraising a nonexistent loan case id used to throw a
`NullReferenceException` (now a clean `404`), and the "must be Registered
or Deferred" precondition was tautologically always true for any case that
did exist (now actually enforced). The identical bug shape is still present,
unfixed, in `ApproveLoanCase`/`AuditLoanCase`/`MarkLoanCaseDisbursed` — not
this endpoint's concern, but worth knowing before building against those.

## Not built yet

Approval, audit/verification, cancellation, restructuring, and
every guarantor sub-flow beyond initial attach (substitute/relieve/release)
— see `WORKFLOW.md` §6-10 for the design and current status of each.
