# Channels Canonical API — SwizzChannels Integration Spec

Audience: **not** the usual SwiftFinancialz frontend — this is the contract
SwiftFinancialz exposes so it can be registered as an institution on
[SwizzChannels](../../../SwizzChannels) (a separate solution/repo), the
platform that puts WhatsApp banking (and later USSD, Telegram, web) in front
of this system without SwizzChannels writing any SwiftFinancialz-specific
code. SwizzChannels calls these endpoints; SwiftFinancialz never calls out.

Source of truth:
- Controller: `WebApplication1/Areas/Channels/Controllers/CanonicalAccountsController.cs`
- Contract this implements: `canonical-financial-api-v0.2.md` §9 (Balance),
  §10 (Mini Statement), §18 (references), §19 (response envelope) in the
  SwizzChannels repo.
- Domain services it calls: `ICustomerAppService.FindCustomerBySerialNumber`
  (resolves `customerReference`), `ICustomerAccountAppService.FindCustomerAccountDTO`
  / `FetchCustomerAccountBalances` (resolves `accountReference` + balance),
  `IJournalEntryAppService.FindLastXGeneralLedgerTransactionsByCustomerAccountId`
  (mini statement lines) — the same calls `CustomerAccountStatementController`
  already uses (`docs/api/customer-account-statement-api-spec.md`).

## 1. This is a different envelope than every other controller in this API

Every other SwiftFinancialz endpoint returns
`{ success, message, data }`. These two endpoints instead return the
canonical envelope SwizzChannels' router expects — **do not** treat this as
a bug or normalize it client-side:

Success:
```json
{ "success": true, "data": { ... } }
```

Failure:
```json
{ "success": false, "error": { "code": "ACCOUNT_NOT_FOUND", "message": "..." } }
```

## 2. References

`customerReference` is the customer's `serialNumber` (as a string, e.g.
`"12345"` — not padded, not prefixed; `CustomerDTO.paddedSerialNumber`'s
`"CUS-"`-style formatting in the canonical spec's examples is illustrative
only). `accountReference` is the account's `fullAccountNumber`. Both are
resolved server-side and cross-checked — the resolved account must belong to
the resolved customer — before any data is returned; a mismatch reports
`ACCOUNT_NOT_FOUND`, not a more specific (and identity-leaking) error.

## 3. `POST /v1/accounts/balance`

Request:
```json
{ "customerReference": "12345", "accountReference": "0100012345678" }
```

Response:
```json
{
  "success": true,
  "data": {
    "accountReference": "0100012345678",
    "currency": "KES",
    "availableBalance": 125000.00,
    "ledgerBalance": 130000.00,
    "asOf": "2026-08-14T10:30:00+03:00"
  }
}
```

`availableBalance` / `ledgerBalance` map to
`CustomerAccountDTO.AvailableBalance` / `.BookBalance`. `currency` is
hardcoded `"KES"` — **no multi-currency concept exists anywhere in this
domain** (checked directly: no `Currency` field on `CustomerAccountDTO`, no
base-currency setting anywhere in the solution). If that ever changes, this
is the one place assuming KES.

## 4. `POST /v1/accounts/transactions` (mini statement)

Request:
```json
{ "customerReference": "12345", "accountReference": "0100012345678", "limit": 10 }
```

Response:
```json
{
  "success": true,
  "data": {
    "accountReference": "0100012345678",
    "transactions": [
      {
        "transactionReference": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "date": "2026-08-14T08:00:00+03:00",
        "type": "CREDIT",
        "description": "Salary",
        "amount": 85000.00,
        "currency": "KES",
        "balanceAfter": 125000.00
      }
    ]
  }
}
```

`limit` defaults to 10 if omitted or `<= 0`. Internally this calls
`FindLastXGeneralLedgerTransactionsByCustomerAccountId` with a fixed 90-day
lookback window (same default `CustomerAccountStatementController.GetMiniStatement`
uses) — a `limit` larger than what falls inside 90 days returns fewer rows
than asked for; there's no way to widen the window from this endpoint today.
`transactionReference` is the underlying `Journal`'s id (a GUID), not a
human-friendly reference — `JournalReference` exists on the source row but
is not always populated, so the guaranteed-unique id is used instead.
No `pagination` object is returned — this endpoint is deliberately
"last N", not cursor-paged; SwizzChannels' `MiniStatementResult.HasMore`
will always read `false` against this institution.

## 5. Known gaps — not yet built

- **No institution-side call-in authentication.** SwizzChannels'
  `OAuth2InstitutionAuthenticator` does a client-credentials grant against a
  token endpoint before calling in; SwiftFinancialz has no such endpoint yet
  (`AuthController` only does human username/password login). Until this
  exists, anyone who can reach `/v1/accounts/balance` or `/v1/accounts/transactions`
  can call them with any `customerReference`/`accountReference` pair they can
  guess — there's no secret in the request path today.
- **`CUSTOMER_VERIFICATION` is not implemented** (`POST /v1/customers/verify`
  + `/verify/confirm`) — SwizzChannels can't yet `LINK` a WhatsApp number to
  a SwiftFinancialz customer/account, only look up balance/statement once
  linked. Needs a new OTP/challenge mechanism — nothing in this domain
  currently generates or stores one; `MobileToBankRequestAuthOption.Verify`
  is an unrelated M-Pesa C2B reconciliation concept, not customer identity
  verification. SMS delivery would use `ITextAlertAppService.AddQuickTextAlert`
  against `CustomerDTO.PhoneNumber`.
- SwiftFinancialz is not yet registered with a running SwizzChannels
  instance (`POST /v1/institutions` on that platform) — these endpoints work
  today if called directly, but nothing calls them yet.
