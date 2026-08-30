# Loan Product API — Client Integration Spec

Audience: admin screens managing the Accounts-module `LoanProduct` catalogue
(product definition, dynamic charges, loan cycles, deductibles, appraisal
factors/products, and attached commissions), plus any screen that just needs
a picker of configured loan products — e.g. `ChequeTypeController`'s Create
form (`docs/api/cheque-type-api-spec.md` §5.4).

**This is a different `LoanProduct` concept from the separate legacy loan
API the `Loaning/LoanProducts.jsx` frontend screen talks to** — do not
assume they return the same data or the same ids. This controller is
strictly `Application.MainBoundedContext/AccountsModule/Services/ILoanProductAppService`
(the Accounts-module aggregate other Accounts-module screens, like
`ChequeTypeAttachedProduct`, reference), not the back-office loaning module.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/LoanProductController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/LoanProductAppService.cs`.
- DTOs: `Application.MainBoundedContext.DTO/AccountsModule/LoanProductDTO.cs` and
  siblings (`DynamicChargeDTO`, `LoanCycleDTO`, `LoanProductAuxiliaryConditionDTO`,
  `LoanProductDeductibleDTO`, `LoanProductAuxilliaryAppraisalFactorDTO`,
  `CommissionDTO`), plus `Application.MainBoundedContext.DTO/ProductCollectionInfo.cs`.
- Auth: same JWT bearer scheme as every other controller — `[Authorize]`.

## History note

`ILoanProductAppService` (create/update/find + several paged/filtered
overloads, plus CRUD-ish find/update pairs for every sub-collection below)
already existed at the app-service layer — no `WebApplication1` controller
called any of it beyond `FindLoanProducts(serviceHeader)`, added first as a
read-only picker endpoint. This is the full adaptation: everything the
reference MVC `LoanProductController`'s Create/Edit wizard did through
session-staged sub-resources is now direct CRUD.

**Not ported** (see the controller's own header comment for detail):
session-only wizard-staging actions (`StoreSelectedWellknownCharges`,
`StoreSelectedCharges`, `StoreSelectedProducts`, `ProcessSelectedLoans`,
`ProcessSelectedInvestmentProducts`, `ProcessSelectedSavingProducts`,
`SaveSelection`) — no persistence behavior of their own, superseded by the
sub-resource `PUT` endpoints below — and four near-duplicate single-record
lookups (`GetInvestmentProductDetails`, `GetSavingDetails`, `GetloanDetails`,
`GetLoanProductDetails`) used only to populate a description label in the
wizard, redundant with `GET /{id}` here (and `SavingsProductController`'s,
for the savings case).

## 1. List / read

### `GET /api/accounts/loanproducts`

Unpaged, every loan product — kept as the existing contract for pickers.
Do not add query params here; use `/paged` for the admin-screen listing.

```json
{ "success": true, "message": "", "data": LoanProductDTO[] }
```

`data` is `[]`, not `null`, if there are none.

### `GET /api/accounts/loanproducts/paged?text=&pageIndex=&pageSize=`

`text` empty/omitted returns the full unfiltered page; non-empty runs a
full-text filter. `pageIndex` default `0`, `pageSize` default `20`.

```json
{ "success": true, "message": "", "data": { "pageCollection": LoanProductDTO[], "itemsCount": number } }
```

### `GET /api/accounts/loanproducts/paged/section/{section}?text=&pageIndex=&pageSize=`

Same as above, additionally filtered by `LoanRegistrationLoanProductSection`
(`0` = FOSA, `1` = BOSA).

### `GET /api/accounts/loanproducts/{id}`

```json
{ "success": true, "message": "", "data": LoanProductDTO }
```

`404` if not found.

## 2. Create / update

### `POST /api/accounts/loanproducts`

Body:
```json
{
  "loanProduct": LoanProductDTO,
  "deductibles": LoanProductDeductibleDTO[] | null,
  "loanCycles": LoanCycleDTO[] | null,
  "auxiliaryConditions": LoanProductAuxiliaryConditionDTO[] | null,
  "auxiliaryAppraisalFactors": LoanProductAuxilliaryAppraisalFactorDTO[] | null,
  "dynamicCharges": DynamicChargeDTO[] | null,
  "appraisalProducts": ProductCollectionInfo | null,
  "commissions": CommissionDTO[] | null,
  "commissionKnownChargeType": number,
  "commissionChargeBasisValue": number
}
```

`loanProduct` is required and is validated (`Description` required,
`ChartOfAccountId`/`InterestReceivedChartOfAccountId`/
`InterestReceivableChartOfAccountId` must be real guids, term/payment
frequency consistency, etc.) — `400` with the real validation messages on
failure. Every other field is optional; omit a sub-collection to leave it
empty rather than attaching anything. `commissionKnownChargeType`/
`commissionChargeBasisValue` only matter when `commissions` is provided —
see §4 for what they mean.

Returns the freshly created (and re-fetched) `LoanProductDTO` on success:
```json
{ "success": true, "message": "Operation Success", "data": LoanProductDTO }
```

### `PUT /api/accounts/loanproducts/{id}`

Body: `LoanProductDTO`. Updates only the loan product's own fields — does
**not** touch any sub-collection, matching the reference app (only Create's
wizard staged sub-collections; Edit never did). Use the sub-resource
endpoints below for those. `400` on validation failure, `404` if `id`
doesn't exist.

## 3. Sub-resources — flat lists

Each of these follows the same `GET`/`PUT` shape: `GET` returns the current
list (`[]` if none), `PUT` does a **full replace** — send every item you
want kept, not just the delta — and returns the refreshed list.
`PUT` on a nonexistent `{id}` returns `404`.

| Route | Item DTO | Notes |
|---|---|---|
| `.../{id}/dynamic-charges` | `DynamicChargeDTO` | Join to existing `DynamicCharge` records — only `Id` is read on `PUT`. |
| `.../{id}/loan-cycles` | `LoanCycleDTO` | `RangeLowerLimit`/`RangeUpperLimit` define the cycle. |
| `.../{id}/auxiliary-conditions` | `LoanProductAuxiliaryConditionDTO` | `{id}` is the **base** loan product; `TargetLoanProductId` + `Condition` + `MaximumEligiblePercentage` per entry. |
| `.../{id}/deductibles` | `LoanProductDeductibleDTO` | Deductions taken against this loan product at disbursement. |
| `.../{id}/auxiliary-appraisal-factors` | `LoanProductAuxilliaryAppraisalFactorDTO` | Investments-range-banded `LoaneeMultiplier`/`GuarantorMultiplier`, overrides `LoanRegistrationInvestmentsMultiplier` when a matching band exists. |

### Runtime enforcement

- Locked loan products are rejected when registering a new loan case.
- All five interest calculation modes, including `FixedInterest`, are used by
  repayment schedules and periodic capitalization. Upfront products are
  charged at disbursement and are excluded from recurring capitalization.
- Fixed-interest schedules derive their number of instalments from term and
  payment frequency; they are not implicitly monthly.
- Microcredit applications must fall within a configured loan-cycle band.
- Auxiliary appraisal factors apply to both loanee entitlement and guarantor
  capacity. Auxiliary conditions enforce no-outstanding-balance and required
  Approved/Audited/Appraised target-loan states. Conditional-list and
  dividends-payable flags are rejected because those data sources are not
  available in the current origination service.
- Same-product balance rejection, maximum self-guarantee percentage, take-home
  limits, system-appraisal enforcement, and outstanding-balance entitlement
  behavior are enforced during registration/appraisal.

## 4. Sub-resource — appraisal products

`.../{id}/appraisal-products` (`GET`/`PUT`) round-trips a
`ProductCollectionInfo` (not a flat list) — the loan/investment/savings
products consulted during appraisal, split by purpose
(`InvestmentProductCollection` = investments qualification,
`LoanProductCollection` = loan recovery, `EligibileIncomeDeduction*` =
eligible income deduction). `PUT` is a full replace of the whole set across
all five sub-lists.

## 5. Sub-resource — commissions

Commissions attach to a loan product scoped by
`knownChargeType` (`Infrastructure.Crosscutting.Framework.Utils.LoanProductKnownChargeType`
— e.g. Loan Clearance Fee, Express/Normal Disbursement Fee) — there's no
"all commissions" view, so it's a required param, not defaulted.

### `GET /api/accounts/loanproducts/{id}/commissions?knownChargeType={n}`

```json
{ "success": true, "message": "", "data": CommissionDTO[] }
```

### `PUT /api/accounts/loanproducts/{id}/commissions`

Body:
```json
{ "knownChargeType": number, "chargeBasisValue": number, "commissions": CommissionDTO[] }
```

Full replace of which `Commission` records attach under that
`knownChargeType` — only each `CommissionDTO.Id` is read (join-table
pattern, same as `CommissionController`'s own `levies` sub-resource).
`chargeBasisValue`
(`Infrastructure.Crosscutting.Framework.Utils.LoanProductChargeBasisValue` —
`0` = Principal Balance, `1` = Book Balance) applies to the whole batch, not
per-commission. `404` if `id` doesn't exist.

## 6. `LoanProductDTO` — what matters for a picker

| Field | Notes |
|---|---|
| `Id` | What you send back (e.g. as one entry in `ChequeTypeController`'s `CreateChequeTypeRequest.AttachedProducts.LoanProductCollection`). |
| `Code` / `PaddedCode` | Numeric product code. |
| `Description` | Display label. |
