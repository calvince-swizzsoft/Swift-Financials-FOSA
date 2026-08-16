# Loan Guarantor API (standalone CRUD/search)

Base path: `api/backoffice/loanguarantors`. Controller:
`WebApplication1/Areas/BackOffice/Controllers/LoanGuarantorController.cs`.
Functional design: `WebApplication1/Areas/BackOffice/WORKFLOW.md` §9.

This is **not** the same thing as the guarantor list embedded in
`loan-case-api-spec.md` §5 (Create) or §2 (`GET /{id}/guarantors` on a
specific case) — those are about attaching guarantors *to a case being
registered*. This controller is a standalone directory over
`LoanGuarantorDTO` records: search/lookup any guarantor record by id or by
the customer who's guaranteeing, and a bare create. Post-registration
attach/relieve/substitute is `loan-guarantor-attachment-api-spec.md`, a
different resource family (`LoanGuarantorAttachmentHistory`).

## Conventions

Standard envelope (`{ success, message, data }`), standard status codes —
see `docs/api/README.md`. All endpoints require a bearer JWT.

## 1. Search

`GET /?text=&pageIndex=0&pageSize=20` → `PageCollectionInfo<LoanGuarantorDTO>`.

## 2. Get one

`GET /{id}` → `LoanGuarantorDTO`. `404` if not found.

## 3. Guarantees by customer

`GET /customer/{customerId}` → `LoanGuarantorDTO[]`.

Every loan a customer is currently guaranteeing, across all loan cases —
the same underlying data `LoanCaseController`'s guarantor-eligibility
lookup (`loan-case-api-spec.md` §4) sums into `committedShares`, exposed
here as a real list of its own rather than just a total.

## 4. Create

`POST /` — body: `LoanGuarantorDTO`.

Runs `ValidateAll()`/`HasErrors` before persisting (`400` with the real
messages on failure), including `LoanGuarantorDTO`'s own
`ValidateAmountGuaranteed` rule (amount guaranteed must be > 0 and ≤
`(totalShares × appraisalFactor) − committedShares`). Unlike
`LoanCaseController.Create`'s guarantor handling, this endpoint does
**not** auto-compute `totalShares`/`committedShares`/`appraisalFactor` for
you — this is the bare app-service method
(`ILoanCaseAppService.AddNewLoanGuarantor`), not the case-registration
enrichment path, so send real values or the validator will reject a
zeroed-out record.

## Not built: Update

The reference MVC `LoanGuarantorController` has an `Edit` view but its
POST action is entirely commented out — no `UpdateLoanGuarantorAsync` call
exists anywhere in the reference app. Matching that, there's no `PUT`
here either; it would be exposing behavior the reference app itself never
actually shipped.
