# Loan Product API — Client Integration Spec

Audience: any screen that needs to let an admin pick from the set of
configured Accounts-module `LoanProduct` records — e.g.
`ChequeTypeController`'s Create form (`docs/api/cheque-type-api-spec.md`
§5.4), which needs at least one loan or investment product selected.

**This is a different `LoanProduct` concept from the separate legacy loan
API the `Loaning/LoanProducts.jsx` frontend screen talks to** — do not
assume they return the same data or the same ids. This controller is
strictly `Application.MainBoundedContext/AccountsModule/Services/ILoanProductAppService`
(the Accounts-module aggregate other Accounts-module screens, like
`ChequeTypeAttachedProduct`, reference), not the back-office loaning module.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/LoanProductController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/LoanProductAppService.cs`
  (`ILoanProductAppService.FindLoanProducts(serviceHeader)`).
- DTO: `Application.MainBoundedContext.DTO/AccountsModule/LoanProductDTO.cs`.
- Auth: same JWT bearer scheme as every other controller — `[Authorize]`.

## History note

`ILoanProductAppService.FindLoanProducts(serviceHeader)` already existed at
the app-service layer (plus several paged/filtered overloads) — no
`WebApplication1` controller called any of them. `ILoanProductAppService`
was only ever injected into the legacy `ValuesController.cs` monolith. This
controller is the first dedicated route for this aggregate — read-only,
matching the specific gap it closes (a picker needs a list, nothing more).

## 1. Endpoint — `GET /api/accounts/loanproducts`

No query params — returns every loan product, unpaged (same shape as
`InvestmentsProductController`'s `GET /`).

```json
{ "success": true, "message": "", "data": LoanProductDTO[] }
```

`data` is `[]`, not `null`, if there are none.

## 2. `LoanProductDTO` — what matters for a picker

| Field | Notes |
|---|---|
| `Id` | What you send back (e.g. as one entry in `ChequeTypeController`'s `CreateChequeTypeRequest.AttachedProducts.LoanProductCollection`). |
| `Code` / `PaddedCode` | Numeric product code. |
| `Description` | Display label. |

No create/update here — this controller is read-only by design. Full
loan-product CRUD (dynamic charges, loan cycles, appraisal factors, etc.)
isn't ported to this API yet; only the fields above were needed to close
the picker gap.
