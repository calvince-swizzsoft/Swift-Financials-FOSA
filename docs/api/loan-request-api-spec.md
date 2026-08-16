# Loan Request API

Base path: `api/backoffice/loanrequests`. Controller:
`WebApplication1/Areas/BackOffice/Controllers/LoanRequestController.cs`.
Functional design: `WebApplication1/Areas/BackOffice/WORKFLOW.md` §5.

The optional pre-case intake stage before a real `LoanCase` is registered
— a member expressing interest in a loan product/purpose/amount, with no
guarantor or appraisal machinery involved yet. Fully independent of
`loan-case-api-spec.md`; nothing here is a prerequisite for registering a
loan case directly.

## Conventions

Standard envelope (`{ success, message, data }`), standard status codes —
see `docs/api/README.md`. All endpoints require a bearer JWT.
`LoanRequestStatus`: `0` = New, `1` = Registered, `2` = Rejected.

## 1. Search

`GET /?text=&loanRequestFilter=0&pageIndex=0&pageSize=20` →
`PageCollectionInfo<LoanRequestDTO>`.

## 2. Get one

`GET /{id}` → `LoanRequestDTO`. `404` if not found.

## 3. In-process requests for a customer

`GET /customers/{customerId}/in-process` → `LoanRequestDTO[]`.

## 4. Create

`POST /` — body: `LoanRequestDTO`. Required: `customerId`, `loanProductId`,
`loanPurposeId`, `amountApplied` (> 0).

`ValidateAll()`/`HasErrors` is checked (`400` with the real messages on
failure). `AddNewLoanRequest` sets `status`/`createdBy` itself regardless
of what's sent — always created as `New`. It also enforces a real
server-side rule: **a customer can't have two `New` (pending) requests for
the same loan product at once** — surfaced here as a clean `400` with the
app service's own message, not a generic `500` (the underlying method
throws `InvalidOperationException`, caught and translated).

## 5. Register

`POST /{id}/register`

```json
{ "loanCaseNumber": 0 }
```

Transitions `New → Registered`. `loanCaseNumber` is optional — send it
once a real `LoanCase` has actually been opened from this request, so the
two records stay linked (`RegisterLoanRequest` persists whatever
`loanCaseNumber` is on the DTO it's given). **The reference MVC app never
wires this link up at all** — no screen anywhere calls
`RegisterLoanRequest` — so there's no reference UI behavior to match here,
only the real app-service parameter. `409` if the request isn't currently
`New`.

## 6. Cancel

`POST /{id}/cancel` — no body. Transitions `New → Rejected`,
stamping `cancelledBy`/`cancelledDate`. `409` if the request isn't
currently `New`. Same "no reference screen, real app-service method"
situation as Register.

## 7. Delete

`DELETE /{id}` — hard removal via `RemoveLoanRequest`. `404` if not found.

## Not ported: Edit

The reference `LoanRequestController.Edit` action is a bare `GET` with no
`[HttpPost]` counterpart at all — dead/view-only in the reference app.
There's no update endpoint here for the same reason as
`loan-guarantor-api-spec.md`'s missing Update: reproducing it would expose
behavior the reference app itself never shipped.
