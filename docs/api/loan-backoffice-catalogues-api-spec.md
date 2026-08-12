# Loan Back Office Catalogues API

Three small reference-data catalogues feeding the loan origination
pipeline's pickers — see `WebApplication1/Areas/BackOffice/WORKFLOW.md`
§13 and §15.2. All three are the same shape: `Description` (`[Required]`),
`IsLocked`, `CreatedDate`, `ErrorMessageResult` (set, not thrown, on a
duplicate `Description` at create time — same pattern as
`UnPayReasonController`/`CostCenterController`).

Standard envelope, standard status codes — see `docs/api/README.md`. All
endpoints require a bearer JWT.

## 1. Loan Purposes — `api/backoffice/loanpurposes`

Controller: `LoanPurposeController.cs`, existing `ILoanPurposeAppService`.
Backs the loan-case registration form's "loan purpose" picker
(`loanCase.loanPurposeId` — `loan-case-api-spec.md` §5).

| Route | Method | Purpose |
|---|---|---|
| `/` | GET | Unpaged list — use this for the picker |
| `/paged?text=&pageIndex=&pageSize=` | GET | Paged/searchable list — for an admin management screen |
| `/{id}` | GET | Single record |
| `/` | POST | Create. `409` with the duplicate message in `data.message` if `description` already exists |
| `/{id}` | PUT | Update |

## 2. Loaning Remarks — `api/backoffice/loaningremarks`

Controller: `LoaningRemarkController.cs`, existing `ILoaningRemarkAppService`.
Backs the loan-case registration form's "registration remark" picker
(`loanCase.registrationRemarkId` — `loan-case-api-spec.md` §5). Same route
shape as §1.

## 3. Income Adjustments — `api/backoffice/incomeadjustments`

Controller: `IncomeAdjustmentController.cs`, existing
`IIncomeAdjustmentAppService`. Backs the appraisal screen's income-adjustment
picker (`loan-case-api-spec.md` §8's `incomeAdjustments[].incomeAdjustmentId`).
Same route shape as §1, plus one extra field:

- `type` (`IncomeAdjustmentType`: `0`=Allowance, `1`=Deduction) — whether
  picking this adjustment adds to or subtracts from appraised income. Set
  at create/update time on the catalogue entry itself; the loan case's own
  `POST .../appraise` call resolves each submitted entry's `type`
  server-side from this catalogue, so don't bother sending it there.

## Not exposed on any of the three

Delete — none of the three app services have a remove method (matching the
reference app, which only ever offered Create/Edit for these). To retire an
entry, `PUT` it with `isLocked: true` instead; nothing currently reads
`isLocked` to hide it from a picker automatically, so a client-side filter
is needed until/unless that becomes a real requirement.
