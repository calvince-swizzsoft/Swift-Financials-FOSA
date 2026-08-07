# Savings Receipts/Payments — Unified Form Layout

Frontend companion to `SAVINGS-RECEIPTS-PAYMENTS-FLOW.md` (read that first for
the classification/maker-checker behavior). This doc is about the *shape of
the form itself*: one screen, one `POST /api/frontoffice/requests` endpoint,
one `CustomerTransactionModel` payload, discriminated by `Type`
(`FrontOfficeTransactionType`) — and what that means for how the UI should be
laid out.

Source: `WebApplication1/Areas/FrontOffice/Controllers/CashDepositController.cs`,
`Create(CustomerTransactionModel)` and `ProcessCustomerTransactionAsync`.

## Why one screen, not four

The old reference MVC app (`CashDepositController.Create` in the sibling
`SwiftFinancials.Web` checkout) already did this as one Razor view with
conditional sections keyed off `FrontOfficeTransactionType`, not four
separate screens. The new API-side model (`CustomerTransactionModel`) mirrors
that same superset shape. So this isn't a new design — it's porting a pattern
that already worked, into a JSON-driven form with conditionally-rendered
sections instead of conditionally-rendered Razor partials.

## Field grouping

### Group 0 — Type selector (always visible, drives everything else)
| Field | Payload path |
|---|---|
| Transaction Type | `Type` (`FrontOfficeTransactionType`: `1` CashWithdrawal, `2` CashDeposit, `3` ChequeDeposit, `4` CashWithdrawalPaymentVoucher) |

### Group 1 — Shared core (all four types, no branching)
| Field | Payload path | Notes |
|---|---|---|
| Customer Account picker | `CreditCustomerAccountId` | Always this field, even for withdrawals — the server flips debit/credit internally based on `Type`. Do not build a separate debit-account picker. |
| Amount | `TotalValue` | |
| Branch | `BranchId` | Pull from session/JWT, not a visible input. |

### Group 2 — Cheque Deposit only (`Type == 3`)
| Field | Payload path |
|---|---|
| Cheque Number | `Reference` |
| Drawer | `Drawer` |
| Drawer's Bank | `DrawerBank` |
| Drawer's Bank Branch | `DrawerBankBranch` |
| Cheque Type | `ChequeType` (Guid — needs a lookup endpoint, not yet built) |
| Write Date | `WriteDate` |

No multi-payee apportionment picker needed — see "Cheque deposit is
single-payee" below.

### Group 3 — Withdrawal by Payment Voucher only (`Type == 4`)
| Field | Payload path |
|---|---|
| Payment Voucher picker (cheque book → voucher) | nested `PaymentVoucher: { Id, Payee, WriteDate, Reference, Amount }` |

Needs the cheque-book → payment-voucher lookup endpoint (not yet built —
tracked from the earlier controller-merge review, belongs on the owning
controller, not on `CashDepositController`).

### Groups 4 & 5 — Plain Cash Withdrawal / Plain Cash Deposit
Nothing beyond Group 1. These two are the simplest of the four — no extra
section at all.

## The request queue list — `GET /api/frontoffice/requests`

This is the read side of the same unified screen: the maker/checker/poster
work queue behind Create. It's feasible to render as one grid, more so than
Create was — `CashDepositRequestDTO` and `CashWithdrawalRequestDTO` share a
solid common core (`Id`, `BranchId`/`BranchDescription`,
`CustomerAccountId`/`CustomerName`, `Status`, `Amount`, `Remarks`,
`AuthorizedBy`/`AuthorizationRemarks`/`AuthorizedDate`,
`CreatedBy`/`CreatedDate`, `TransactionType`), with only a couple of
type-specific extras each: deposits add `Denomination`/`PostedBy`/
`PostedDate`; withdrawals add `Category` (WithinLimits/AboveMaximumAllowed/
BelowMinimumBalance/Overdraw/**PaymentVoucher**), `PaidBy`/`PaidDate`,
`PaymentVoucherId`/`PaymentVoucherPayee`.

Query params: `type`, `status`, `text`, `startDate`, `endDate`, `pageIndex`,
`pageSize`.

- **`type` omitted** → the server merges the deposit and withdrawal request
  queues into one page, sorted by `CreatedDate` descending, and pages the
  *combined* set (so `pageSize=20` means 20 merged rows, not 20-per-source).
  Each row is returned in its own native shape — a `CashDepositRequestDTO`
  or a `CashWithdrawalRequestDTO` — not flattened into a shared shape, so
  every row still carries its own `TransactionType`
  (`1` = CashWithdrawal, `2` = CashDeposit). Filter client-side on that
  field if you want a single-type view instead of the combined one — no
  extra API call needed to do that filtering, it's already in the payload.
- **`type=2` (CashDeposit) or `type=1` (CashWithdrawal)** → same as before,
  a single-source page scoped to that type.
- **`type=3` (ChequeDeposit) or `type=4` (CashWithdrawalPaymentVoucher)** →
  always empty. Cheque deposits post directly and never create a request
  row; payment-voucher withdrawals *do* create a request row, but it's
  stored with `TransactionType = CashWithdrawal` and
  `Category = PaymentVoucher` — so payment-voucher rows already surface
  inside the `type=1`/merged results, not under `type=4`. Don't build a
  "type 4" tab; filter on `Category === PaymentVoucher` within the
  withdrawal rows instead if a separate view is wanted.
- **`status` defaults to `Pending`** (this endpoint's primary purpose is a
  checker/teller work queue, not a full audit browse) — pass it explicitly
  for `Authorized`/`Rejected`/`Posted`/`Paid` views.

Previously, omitting `type` silently returned an empty page — fixed; it now
returns the merged queue described above.

## JSON examples

A casing note first, since it affects every example below: this project has
no `CamelCasePropertyNamesContractResolver` configured anywhere
(`WebApiConfig.cs`, `Global.asax.cs`) — checked directly, not assumed. So
DTOs (`CustomerTransactionModel`, `JournalDTO`, `CashDepositRequestDTO`,
`CashWithdrawalRequestDTO`, `PageCollectionInfo<T>`) serialize with their
exact C# property names, i.e. **PascalCase**, both ways. The envelope
(`success`/`message`/`data`) and the authorization-dialog payload
(`isCashDepositRequest`, `cashTransactionRequestId`, etc.) are different —
those are literal anonymous objects written directly in the controller, so
they're genuinely lowerCamelCase in the actual code and stay that way on the
wire. (`docs/api/customer-api-spec.md` shows camelCase for DTOs like
`CustomerDTO`/`PageCollectionInfo<T>` in a different area — that's stale
against this same default and worth a separate fix; not touched here since
it's a different controller.)

### POST / — Cash Deposit, within limits (posts directly)

Request:
```json
{
  "Type": 2,
  "CreditCustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "TotalValue": 5000.00,
  "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab"
}
```

Response `200`:
```json
{
  "success": true,
  "message": "Operation Success",
  "data": {
    "Id": "b2a1e0d3-4f5a-4a2a-9b1a-000000000001",
    "SequentialId": "c3b2f1e4-5a6b-4b3a-9c2b-000000000002",
    "BranchDescription": "Nairobi Main",
    "PrimaryDescription": "ok",
    "SecondaryDescription": "B001/T01/#00042",
    "PostingPeriodDescription": "August 2026",
    "ApplicationUserName": "jdoe",
    "CreatedDate": "2026-08-06T10:15:00",
    "TotalValue": 5000.00,
    "Reference": "CUST-REF-001"
  }
}
```
`Reference` here is the customer's own reference (`CustomerReference1`) —
server-derived, not what the client sent (see note 2 below).

### POST / — Cash Withdrawal, above limit (queued for approval)

Request:
```json
{
  "Type": 1,
  "CreditCustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "TotalValue": 500000.00,
  "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab"
}
```

Response `200`, business failure (this is the "show the approval-request
dialog" case, not an error state — check `data.dialog`, not just `success`):
```json
{
  "success": false,
  "message": "AboveMaximumAllowed.\nSuccessfully placed cash withdrawal authorization request",
  "data": {
    "isCashWithdrawalRequest": true,
    "dialog": true,
    "selectedCustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "transactionTotalValue": 500000.00,
    "transactionReference": null,
    "cashTransactionRequestId": "d4e5f6a7-8b9c-4d5e-9f6a-000000000003",
    "transactionCategory": 2,
    "paymentVoucherId": "00000000-0000-0000-0000-000000000000",
    "paymentVoucherPayee": null,
    "paymentVoucherChequeBookId": "00000000-0000-0000-0000-000000000000",
    "paymentVoucherWriteDate": null
  }
}
```
`transactionCategory: 2` = `CashWithdrawalCategory.AboveMaximumAllowed`
(the enum is `[Flags]`: `WithinLimits=1`, `AboveMaximumAllowed=2`,
`BelowMinimumBalance=4`, `Overdraw=8`, `PaymentVoucher=16`).

### POST / — Cheque Deposit (always posts directly, no maker-checker)

Request — `Reference` **is** the cheque number here, per note 2 below:
```json
{
  "Type": 3,
  "CreditCustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "TotalValue": 12000.00,
  "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
  "Reference": "000482",
  "Drawer": "Jane Wanjiru",
  "DrawerBank": "Equity Bank",
  "DrawerBankBranch": "Westlands",
  "ChequeType": "5e6f7a8b-9c0d-4e1f-8a2b-000000000004",
  "WriteDate": "2026-08-05T00:00:00"
}
```

Response `200` — same `JournalDTO` shape as the Cash Deposit example, but
note `PrimaryDescription` gets the cheque number appended server-side
(`"ok - 000482"`), and `Reference` echoes back exactly what was sent
(`"000482"`) since the overwrite is skipped for this type:
```json
{
  "success": true,
  "message": "Operation Success",
  "data": {
    "Id": "e5f6a7b8-9c0d-4e1f-8a2b-000000000005",
    "SequentialId": "f6a7b8c9-0d1e-4f2a-8b3c-000000000006",
    "BranchDescription": "Nairobi Main",
    "PrimaryDescription": "ok - 000482",
    "SecondaryDescription": "B001/T01/#00043",
    "PostingPeriodDescription": "August 2026",
    "ApplicationUserName": "jdoe",
    "CreatedDate": "2026-08-06T10:20:00",
    "TotalValue": 12000.00,
    "Reference": "000482"
  }
}
```

### POST / — Withdrawal by Payment Voucher (always queued, never direct)

Request:
```json
{
  "Type": 4,
  "CreditCustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "TotalValue": 25000.00,
  "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
  "PaymentVoucher": {
    "Id": "f1a2b3c4-d5e6-4f7a-8b9c-000000000007",
    "Payee": "ABC Suppliers Ltd",
    "WriteDate": "2026-08-06T00:00:00",
    "Reference": "PV-2026-0088",
    "Amount": 25000.00
  }
}
```

Response `200` — same dialog envelope as the withdrawal example, but with
the voucher fields populated and `transactionCategory: 16`
(`CashWithdrawalCategory.PaymentVoucher`):
```json
{
  "success": false,
  "message": "PaymentVoucher.\nSuccessfully placed cash withdrawal authorization request",
  "data": {
    "isCashWithdrawalRequest": true,
    "dialog": true,
    "selectedCustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "transactionTotalValue": 25000.00,
    "transactionReference": "PV-2026-0088",
    "cashTransactionRequestId": "a9b8c7d6-e5f4-4a3b-9c2d-000000000008",
    "transactionCategory": 16,
    "paymentVoucherId": "f1a2b3c4-d5e6-4f7a-8b9c-000000000007",
    "paymentVoucherPayee": "ABC Suppliers Ltd",
    "paymentVoucherChequeBookId": "00000000-0000-0000-0000-000000000000",
    "paymentVoucherWriteDate": "2026-08-06T00:00:00"
  }
}
```

### POST / — Blocked outright (e.g. overdraw)

No request created, nothing queued:
```json
{
  "success": false,
  "message": "Sorry, but the customer's account will be overdrawn!",
  "data": null
}
```

### GET /?type=2&status=1 — Cash deposit requests, Pending

```json
{
  "success": true,
  "message": "",
  "data": {
    "PageIndex": 0,
    "PageSize": 20,
    "ItemsCount": 1,
    "PageCollection": [
      {
        "Id": "e1f2a3b4-c5d6-4e7f-8a9b-000000000009",
        "BranchId": "8c9c1c2b-1b1a-4a5a-9c1a-1234567890ab",
        "BranchDescription": "Nairobi Main",
        "CustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "CustomerName": "Jane Wanjiru",
        "Status": 1,
        "Amount": 750000.00,
        "Remarks": null,
        "AuthorizedBy": null,
        "AuthorizationRemarks": null,
        "AuthorizedDate": null,
        "PostedBy": null,
        "PostedDate": null,
        "CreatedBy": "jdoe",
        "CreatedDate": "2026-08-06T09:00:00",
        "TransactionType": 2,
        "Posted": false
      }
    ]
  }
}
```

### GET / (no `type`) — merged deposit + withdrawal queue

Two rows, different native shapes, sorted by `CreatedDate` descending — note
the second row (a withdrawal) carries `Category`/`PaidBy`/`PaidDate`, which
the first row (a deposit) doesn't have at all:
```json
{
  "success": true,
  "message": "",
  "data": {
    "ItemsCount": 2,
    "PageCollection": [
      {
        "Id": "e1f2a3b4-c5d6-4e7f-8a9b-000000000009",
        "CustomerAccountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "CustomerName": "Jane Wanjiru",
        "Status": 1,
        "Amount": 750000.00,
        "TransactionType": 2,
        "CreatedDate": "2026-08-06T09:00:00"
      },
      {
        "Id": "d4e5f6a7-8b9c-4d5e-9f6a-000000000003",
        "CustomerAccountId": "7a8b9c0d-1e2f-4a3b-8c4d-00000000000a",
        "CustomerName": "John Otieno",
        "Status": 1,
        "Amount": 500000.00,
        "Category": 2,
        "TransactionType": 1,
        "CreatedDate": "2026-08-06T08:30:00"
      }
    ]
  }
}
```
Frontend filters this array on `TransactionType` (and `Category` where
present) to build single-type or single-category views — no second request
needed.

### GET /?type=3 or GET /?type=4 — always empty

```json
{ "success": true, "message": "", "data": { "PageCollection": [], "ItemsCount": 0 } }
```

## Notes for adjusting the frontend approach

1. **One account field, always.** `CreditCustomerAccountId` is read
   regardless of deposit or withdrawal; the server decides which side is
   debited/credited from `Type`. A separate debit/credit account picker
   would be pure overhead — don't build one.

2. **`Reference` is dual-purpose, and this used to be a real bug, now
   fixed.** For CashDeposit / CashWithdrawal / PaymentVoucher withdrawal,
   the server derives `Reference` itself from the customer's own reference
   — treat it as non-editable / don't collect it in those three sections.
   For ChequeDeposit, `Reference` **is** the cheque number
   (`NewExternalCheque.Number = transactionModel.Reference`), so it must be
   a real, required input in Group 2.

   Previously the server unconditionally overwrote `Reference` with the
   customer's reference *before* branching on `Type` — which silently
   clobbered whatever cheque number the frontend sent, so deposited cheques
   were recorded under the wrong `Number`. Fixed in `Create()`: the
   overwrite is now skipped for `ChequeDeposit`, so whatever the frontend
   sends as the cheque number is exactly what gets persisted. No frontend
   workaround needed — just make sure Group 2's "Cheque Number" field
   binds to `Reference` in the payload, not to a nested `ChequeDeposit.Number`
   (that nested DTO isn't read by `Create` at all).

3. **Cheque deposit is single-payee, like everything else.** The old app
   let one deposited cheque apportion its value across several customer
   accounts (`ChequePayableCustomerAccountIds`). The new backend doesn't —
   it always pays into the single selected `CreditCustomerAccountId`. Don't
   build a multi-account apportionment picker for Group 2; confirm with
   backend owners this simplification was intentional if it matters for a
   specific customer workflow.

4. **One shared "authorization required" dialog, not per-type ones.**
   Above-limit / below-minimum / overdraft responses all come back as
   `{ success:false, data:{ dialog:true, isCashDepositRequest |
   isCashWithdrawalRequest, ... } }` — same envelope for deposit and
   withdrawal categories alike. Build one dialog component keyed off
   whichever `is...Request` flag is present, rather than near-duplicate
   ones per type.

5. **Groups 2 and 3 are blocked on missing lookups; Groups 4 and 5 aren't.**
   Cheque Type list and the cheque-book/payment-voucher picker don't have
   backend endpoints yet (should live on their owning controllers, not on
   `CashDepositController` — see the controller-merge discussion this doc
   follows from). Sequence frontend work accordingly: plain deposit/
   withdrawal can ship now, cheque deposit and payment-voucher withdrawal
   need those lookups first.

6. **The request-queue screen is a separate view, fed by `GET /`.** `POST
   /api/frontoffice/requests/post` (poster step) and `.../markposted`
   (status-correction escape hatch) belong to that queue view, not this
   Create form — see "The request queue list" above for the list shape, and
   `SAVINGS-RECEIPTS-PAYMENTS-FLOW.md` for the maker/checker/poster
   lifecycle it drives.
