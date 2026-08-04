# Customer Account Statement API — Client Integration Spec

Audience: front-office / member-facing screens that show a single customer
account's transaction history — mini-statement, full statement for a date
range, and a printable PDF.

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Accounts/Controllers/CustomerAccountStatementController.cs`
- Domain services it calls:
  `Application.MainBoundedContext/AccountsModule/Services/IJournalEntryAppService.cs` (transaction listing),
  `Application.MainBoundedContext/Services/IMediaAppService.cs` (PDF rendering),
  `Application.MainBoundedContext/AccountsModule/Services/ICustomerAccountAppService.cs` (`FindCustomerAccountDTO`, to resolve the account before either call)
- Line-item shape: `Application.MainBoundedContext.DTO/GeneralLedgerTransaction.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

For the back-office / chart-of-accounts ledger view, see
`docs/api/general-ledger-statement-api-spec.md` — that's a separate
controller, scoped to a G/L account rather than a customer account.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/statements/customer-account` |
| Transport | HTTPS only |
| Content type | `application/json`, except `/print` which returns `application/pdf` |
| Auth | Bearer JWT on every request |

## 2. Response envelope

The two JSON endpoints (`/mini`, plain `GET /{customerAccountId}`) use the
standard envelope:

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success, or a caught business error (`success: false`).
- `400 Bad Request` — `startDate` after `endDate`.
- `404 Not Found` — `customerAccountId` doesn't resolve to an account.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

`/print` does **not** use the envelope — on success it streams a raw PDF
(`Content-Type: application/pdf`, `Content-Disposition: attachment`); on
failure it returns a plain `HttpError` body at the relevant status code
(`404`/`400`/`500`), not the `{ success, message, data }` shape. Check the
`Content-Type` header before trying to parse the response as JSON.

## 3. The `GeneralLedgerTransaction` line shape

Both JSON endpoints return pages/lists of this shape (trimmed to the fields
you'll actually use — see the source file for the rest, mostly audit
metadata):

```ts
interface GeneralLedgerTransaction {
  id: string;
  glAccountName: string;              // "<type digit>-<code> <description>"
  customerFullName: string;
  customerAccountNumber: string;      // full formatted account number
  journalPrimaryDescription: string;
  journalSecondaryDescription: string;
  journalReference: string;
  debit: number;
  credit: number;
  bookBalance: number;
  availableBalance: number;
  runningBalance: number;             // pre-computed — don't recompute client-side
  journalTransactionCode: number;     // SystemTransactionCode enum
  journalTransactionCodeDescription: string;
  journalValueDate: string | null;
  journalCreatedDate: string;
  journalIsLocked: boolean;           // true when the posting was reversed
}
```

`runningBalance` is computed server-side per line — do not re-derive it from
`debit`/`credit` client-side, and don't assume it resets to 0 at the start of
a page (it's a running balance over the *whole account*, not just the
current page).

## 4. Paging shape

`GET /{customerAccountId}` returns `PageCollectionInfo<GeneralLedgerTransaction>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

`GET /{customerAccountId}/mini` also returns a `PageCollectionInfo`
(the app service always pages, even for "last N") — don't assume it's a bare
array.

## 5. Endpoints

All routes below are relative to `/api/accounts/statements/customer-account`.

### 5.1 Mini statement — `GET /{customerAccountId}/mini`

Query: `lastXDays` (default `90`), `lastXItems` (default `20`),
`tallyDebitsCredits` (default `true` — whether the service tallies
debit/credit totals into the result; leave `true` unless you have a reason
not to). Returns the most recent transactions within the last `lastXDays`
days, capped at `lastXItems`.

### 5.2 Full statement — `GET /{customerAccountId}`

Query: `startDate`, `endDate` (ISO dates, default: last calendar month to
today), `pageIndex` (default `0`), `pageSize` (default `20`), `text`
(optional filter string), `journalEntryFilter` (int — which field `text`
matches against; see `JournalEntryFilter` enum, e.g. `4` = Reference — the
old MVC statement screen defaulted to filtering by reference), `tallyDebitsCredits`
(default `true`). `400` if `startDate` is after `endDate`.

### 5.3 Print PDF — `GET /{customerAccountId}/print`

Query: `startDate`, `endDate` (same defaults as §5.2), `chargeForPrinting`
(default `false` — **when `true`, this posts a real statement-printing fee
to the account** via the configured savings-product tariff; don't default
this to `true` in a UI without the user explicitly asking for a charged
printout), `includeInterestStatement` (default `false` — loan accounts only;
adds a second "Interest Statement" section to the PDF), `moduleNavigationItemCode`
(gates a module-nav permission check — source from
`GET /api/administration/modules` rather than hardcoding, same caveat as the
Customer API's `Create` endpoint).

Response is the raw PDF bytes (`application/pdf`, `Content-Disposition: attachment`).
Trigger a browser download or open in a PDF viewer — do not attempt to parse
the body as JSON.
