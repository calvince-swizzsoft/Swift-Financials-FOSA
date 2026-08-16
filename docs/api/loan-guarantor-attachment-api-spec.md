# Loan Guarantor Attachment API

Base path: `api/backoffice/loanguarantorattachments`. Controller:
`WebApplication1/Areas/BackOffice/Controllers/LoanGuarantorAttachmentController.cs`.
Functional design: `WebApplication1/Areas/BackOffice/WORKFLOW.md` §9.

Consolidates three reference MVC controllers
(`GuarantorAttachmentController`, `GuarantorRelievingController`,
`GuarantorSubstitutionController`) into one, because they all operate on
the same underlying resource: a `LoanGuarantorAttachmentHistory` record
and its entries. This is the **post-registration** guarantor flow — a
customer account's owner offering it as security on someone else's
already-open loan, separately from the guarantors submitted with the loan
case itself (`loan-case-api-spec.md` §5).

## Conventions

Standard envelope (`{ success, message, data }`), standard status codes —
see `docs/api/README.md`. All endpoints require a bearer JWT.
`moduleNavigationItemCode` fields follow this repo's usual convention
(see e.g. `batch-procedures-api-spec.md`) — pass whatever the frontend's
navigation-menu code is for the screen driving the action; `0` is
accepted but not meaningful for audit-trail purposes.

## 1. Attach guarantors

`POST /`

```json
{
  "sourceCustomerAccountId": "...",
  "destinationLoanProductId": "...",
  "loanGuarantors": [ { "customerId": "...", "amountGuaranteed": 50000.00, "...": "rest of LoanGuarantorDTO" } ],
  "moduleNavigationItemCode": 0
}
```

`sourceCustomerAccountId` is the guarantor's own customer account (the
account being pledged as security); `destinationLoanProductId` is the loan
product of the loan being guaranteed — despite the name, this is a
**loan product id, not a loan case id** (confirmed directly against
`ILoanCaseAppService.AttachLoanGuarantors`'s real parameter, not assumed
from the reference controller's variable naming). `409` if the attach
fails.

**Not reproduced from the reference `GuarantorAttachmentController`**: its
`CustomerAccountLookUp`/`AttachToLookUp` actions and hand-rolled ADO.NET
queries against `swiftFin_LoanGuarantors`/`swiftFin_Customers` directly —
the real equivalents are `CustomerAccountController`,
`LoanProductController`, and this repo's own
`loan-guarantor-api-spec.md` §2/§3.

## 2. Browse attachment history

`GET /?status=0&startDate=&endDate=&text=&pageIndex=0&pageSize=20`

`status`: `LoanGuarantorAttachmentHistoryStatus` — `0` = Attached
(default), `1` = Relieved. `startDate`/`endDate` default to the trailing
month if omitted. Returns `PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO>`.

## 3. Attachment entries

`GET /{id}/entries` → `LoanGuarantorAttachmentHistoryEntryDTO[]` for one
attachment history record.

`GET /entries/{entryId}` → a single entry. `404` if not found.

## 4. Relieve

`POST /{id}/relieve`

```json
{ "moduleNavigationItemCode": 0 }
```

`id` is the `LoanGuarantorAttachmentHistoryId`. Releases every guarantee
under that one attachment history record in a single call (matches the
reference `GuarantorRelievingController.Update` action exactly — it isn't
per-entry). `409` if the record doesn't exist or is already relieved.

## 5. Substitute

`POST /substitute`

```json
{
  "substituteGuarantorCustomerId": "...",
  "loanGuarantorIds": ["...", "..."],
  "moduleNavigationItemCode": 0
}
```

Replaces the guarantor on each of the given, already-existing
`LoanGuarantorDTO` records (picked by id — you don't resend their full
payload) with `substituteGuarantorCustomerId`. Each id in
`loanGuarantorIds` is resolved server-side first (`404`-equivalent `400`
if any doesn't exist) before calling
`ILoanCaseAppService.SubstituteLoanGuarantors` — same two-step shape the
reference `GuarantorSubstitutionController.Create` action uses
(`FindLoanGuarantorAsync` per id, then one `SubstituteLoanGuarantorsAsync`
call with the resolved list).

## Not built: `GuarantorManagementController`

Its `Add()` action accumulates guarantors in `Session` with real
validation (self-guarantee check, max-guarantees check, share-sufficiency
check) — but the `Create(LoanCaseDTO)` POST meant to actually commit that
session data does nothing at all
(`Session["LoanProductId"] = null; return View();`, no persistence call).
Dead/non-functional in the reference app itself, and everything real in
it duplicates `LoanCaseController.EnrichAndValidateGuarantors`
(`loan-case-api-spec.md` §5) — not ported.
