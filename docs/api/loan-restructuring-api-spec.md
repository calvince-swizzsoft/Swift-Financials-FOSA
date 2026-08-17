# Loan Restructuring API

Base path: `api/backoffice/loanrestructuring`. Controller:
`WebApplication1/Areas/BackOffice/Controllers/LoanRestructuringController.cs`.
Functional design: `WebApplication1/Areas/BackOffice/WORKFLOW.md` §10.

Gives an existing, disbursed loan a new term and payment. **Keyed by the
loan's `CustomerAccountId` — not a `LoanCaseId`** — unlike every other
lifecycle action in the Back Office module. The controller also owns the
loan-only customer-account lookup used by its picker, because the generic
customer-accounts API does not expose its domain service's product-code
filter.

## Conventions

Standard envelope (`{ success, message, data }`), standard status codes —
see `docs/api/README.md`. All endpoints require a bearer JWT.

## Loan-account picker

`GET /accounts?text=&customerFilter=0&pageIndex=0&pageSize=20`

Returns a standard paged `CustomerAccountDTO` envelope containing only
accounts whose `customerAccountTypeProductCode` is `2` (`ProductCode.Loan`).
Use this endpoint for the restructuring account picker; the generic
`api/accounts/customer-accounts` list has no product-code query parameter.

## Restructure

`POST /`

```json
{
  "branchId": "...",
  "customerAccountId": "...",
  "nPer": 24,
  "pmt": 5000.00,
  "reference": "Restructure — extended term after job change",
  "moduleNavigationItemCode": 0
}
```

`customerAccountId` is the loan account being restructured. `nPer`
(new number of periods) and `pmt` (new payment per period) are the actual
inputs — the reference screen has no server-side amortization preview for
these, the loan officer enters them directly. `reference` is required
(free text, stored on the restructuring journal's description).

The controller rejects a missing account with `404`, and rejects savings
or investment accounts with `400`; callers cannot bypass the loan-only
picker by submitting another customer-account id directly.

Authorization follows the selected loan account's product section:
FOSA accounts use `FrontOfficeLoanRestructuring` (45016), while BOSA
accounts use `BackOfficeLoanRestructuring` (45017). If the applicable
permission has role mappings, the caller must hold one of those roles.

`ILoanCaseAppService.RestructureLoan` (real, verified server logic, not
guessed from the DTO) only succeeds when:

- The account resolves to a real, currently-outstanding loan
  (`principalBalance > 0`).
- The account has **no outstanding interest balance** — a loan with
  unpaid interest can't be restructured until that's cleared.
- The customer has no *other* loan case for the same loan product
  currently mid-pipeline (`Registered`/`Appraised`/`Deferred`/`Approved`/
  `Audited`) — restructuring itself opens a new `LoanCaseDTO` under the
  hood, and that same duplicate-in-process guard
  (`loan-case-api-spec.md` §5, point 8) applies here too.

`409` if any of the above isn't satisfied — check the customer's loan
account balances and in-process applications
(`loan-case-api-spec.md` §3) before offering this action in the UI, same
as any other guarded transition in this module.
