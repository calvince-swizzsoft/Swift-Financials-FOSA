# General Ledger Statement API — Client Integration Spec

Audience: back-office / accounting screens — ledger statements for a
specific chart-of-accounts (G/L) account (e.g. a suspense account, an income
account, a G/L clearing account), and an unscoped audit browse across every
G/L posting in a date range. Not tied to any single customer.

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Accounts/Controllers/GeneralLedgerStatementController.cs`
- Domain service it calls: `Application.MainBoundedContext/AccountsModule/Services/IJournalEntryAppService.cs`
- Line-item shape: `Application.MainBoundedContext.DTO/GeneralLedgerTransaction.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

For a single member's account statement (savings/loan/investment), see
`docs/api/customer-account-statement-api-spec.md` — that's a separate
controller, scoped to a customer account rather than a G/L account.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/statements/gl-account` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | Bearer JWT on every request |

## 2. Response envelope

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success, or a caught business error (`success: false`).
- `400 Bad Request` — `startDate` after `endDate`.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`. Note: unlike the customer-account statement controller,
  these endpoints don't validate `chartOfAccountId` against a lookup first —
  an unknown id just comes back as an empty page (`itemsCount: 0`), not a
  `404`.

## 3. Line shape and paging

Same `GeneralLedgerTransaction` shape and `PageCollectionInfo<T>` wrapper as
the customer-account statement API — see
`docs/api/customer-account-statement-api-spec.md` §3–4. For G/L statements,
`glAccountName`/`glAccountCode` on each line describe the account itself
(useful mainly on the unscoped §3.3 browse, where lines span multiple G/L
accounts); `customerAccountNumber`/`customerFullName` are populated when the
posting happens to be tied to a customer account, blank otherwise (e.g. pure
inter-G/L postings).

## 4. Endpoints

All routes below are relative to `/api/accounts/statements/gl-account`.

### 4.1 Ledger statement — `GET /{chartOfAccountId}`

Query: `startDate`, `endDate` (ISO dates, default: last calendar month to
today), `pageIndex` (default `0`), `pageSize` (default `20`), `text`
(optional filter string), `journalEntryFilter` (int, `JournalEntryFilter`
enum — which field `text` matches), `transactionDateFilter` (int, default
`2` = `CreatedDate`; `1` = `ValueDate` — which date field `startDate`/`endDate`
are compared against), `tallyDebitsCredits` (default `true`).

### 4.2 Ledger statement by transaction code — `GET /{chartOfAccountId}/by-transaction-code`

Same as §4.1 but replaces the free-text `text`/`journalEntryFilter` filter
with two specific fields: `transactionCode` (required, int —
`SystemTransactionCode` enum, e.g. `2` = Cash Deposit, `1` = Cash Withdrawal)
and `reference` (optional string, exact-ish match on `journalReference`).
Use this when you already know the transaction type and just need to trace
a specific reference through the ledger — narrower and faster than the
free-text search in §4.1.

### 4.3 Unscoped browse — `GET /`

No `chartOfAccountId` — every G/L posting across every account in the date
range. Query: `startDate`, `endDate` (same defaults as §4.1), `pageIndex`,
`pageSize`, `text`, `journalEntryFilter`. This is the back-office "all
transactions" audit view (mirrors what the old MVC app's
`TransactionJournalsController` browse screen did) — expect this to be a lot
of rows on a live system; always page it, and steer users toward §4.1/§4.2
once they know which account they care about.
