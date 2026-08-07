# Denomination Capture — Frontend Adaptation Guide

Audience: frontend engineer adapting the treasury cash movement, End of Day
close, cash transfer request, and standalone fiscal count screens to
capture a real denomination breakdown, following the backend change that
now enforces it.

Companion docs: `docs/api/frontoffice-api-spec.md` §5/§7/§9/§16 (request/
response contracts), `WORKFLOW.md` §6 (why this exists functionally).

## 1. What changed, in one sentence

Four endpoints that used to accept (or silently ignore) a denomination
breakdown now **require** one that reconciles exactly to the transaction
total, and reject the request with `400` if it doesn't.

## 2. The shared field shape

Eleven fields, same names on every DTO that carries them
(`FiscalCountDTO`, and now `CashTransferRequestDTO` too):

| Field | Face value (KES) |
|---|---|
| `DenominationOneThousandValue` | 1000 |
| `DenominationFiveHundredValue` | 500 |
| `DenominationTwoHundredValue` | 200 |
| `DenominationOneHundredValue` | 100 |
| `DenominationFiftyValue` | 50 |
| `DenominationFourtyValue` | 40 |
| `DenominationTwentyValue` | 20 |
| `DenominationTenValue` | 10 |
| `DenominationFiveValue` | 5 |
| `DenominationOneValue` | 1 |
| `DenominationFiftyCentValue` | 0.5 |

**Important — each field is a monetary subtotal, not a note/coin count.**
`DenominationOneThousandValue: 5000` means "KES 5,000 counted in
1000-notes" (i.e. 5 physical notes), not "5". The backend reconciliation
is a plain sum of these eleven fields against the transaction total — it
does **not** multiply by face value, because it assumes you already did.

This has a direct UI consequence: if you want the counting screen to feel
natural for a teller (count physical notes: "3 of the 1000s, 7 of the
500s..."), build that as **piece-count inputs client-side**, and multiply
each by its face value before putting the result into the field the API
expects. Don't send raw piece counts directly — a teller who counts 3
`1000`-notes should submit `DenominationOneThousandValue: 3000`, not `3`.
The face-value table above is exactly what you need for that
multiplication, and it's also the reconciliation the server will perform
on the other end — get it right client-side and the `400` case shouldn't
come up in normal use.

## 3. Recommended shared component

Since the same eleven fields, the same face-value table, and the same
"live running total vs. target" UX apply to all four screens below, build
**one** denomination-entry component (piece-count inputs → computed
subtotal per row → summed total → live diff against the target amount)
and reuse it, parameterized by:
- the target amount to reconcile against (differs per screen, see §4), and
- the field name it should emit (`Denomination*Value` — identical across
  every screen).

Show the running total and the diff against the target *before* the user
submits — the server-side check is a hard `400` rejection, not a warning,
so catching the mismatch client-side avoids a failed round-trip.

## 4. Per-screen wiring

| Screen | Endpoint | Reconciles against | Notes |
|---|---|---|---|
| Treasury cash movement | `POST /api/frontoffice/cashmanagement` | `TotalValue` | Body is `FiscalCountDTO`. Denomination fields are top-level alongside `TotalValue`/`TransactionType`. |
| End of Day close | `POST /api/frontoffice/endofday` | `ClosingBalance` | Body is `CashTransferRequestDTO`. The denomination breakdown **is** the physical till count — `ClosingBalance` should already equal what you're about to enter here; don't let the UI treat them as two independently-typed numbers that happen to need to match. |
| Cash transfer request | `POST /api/frontoffice/transfers/cash` | `Amount` | Body is `CashTransferRequestDTO` (same fields as EOD). The count here represents what's physically handed over between tellers. |
| Standalone fiscal count | `POST /api/frontoffice/fiscalcounts` | `TotalValue` | Body is `FiscalCountDTO`. This is the manual/ad-hoc entry screen — same rule, no special-casing. |

## 5. What you get back / where the record ends up

- **Treasury movement, EOD, standalone fiscal count**: the denomination
  breakdown you sent is what gets persisted as the `FiscalCount` record's
  own fields directly.
- **Cash transfer request is different**: `CashTransferRequest` (the
  underlying entity) has no columns for a denomination breakdown at all.
  Your submitted count gets written as a **separate, companion**
  `FiscalCount` row (tagged `TransactionCode: TellerCashTransfer`), not
  onto the transfer request itself. There is no field on the transfer
  request response that echoes the count back — if you need to display it
  again, query `GET /api/frontoffice/fiscalcounts?...` and match on
  branch/reference/date, not by looking at the transfer request record.

## 6. JSON examples

### Treasury cash movement — success

```json
POST /api/frontoffice/cashmanagement
{
  "TransactionType": 3,
  "TotalValue": 15000.00,
  "DenominationOneThousandValue": 10000.00,
  "DenominationFiveHundredValue": 4000.00,
  "DenominationOneHundredValue": 1000.00,
  "DenominationTwoHundredValue": 0,
  "DenominationFiftyValue": 0,
  "DenominationFourtyValue": 0,
  "DenominationTwentyValue": 0,
  "DenominationTenValue": 0,
  "DenominationFiveValue": 0,
  "DenominationOneValue": 0,
  "DenominationFiftyCentValue": 0
}
```
(10 × 1000-notes + 8 × 500-notes + 10 × 100-notes = 15,000 — shown here as
already-multiplied subtotals: 10000 + 4000 + 1000 = 15000.)

### Treasury cash movement — reconciliation failure (`400`)

Same request but `DenominationOneHundredValue` sent as `500.00` instead of
`1000.00` (counted 500 short):

```json
{
  "success": false,
  "message": "Operation Failed: Counted denominations (14500.00) do not match the total value (15000.00)."
}
```
Show this inline against the running-total diff (§3) rather than as a
generic error toast — the message already tells you both numbers.

### End of Day close — success

```json
POST /api/frontoffice/endofday
{
  "TellerCashBalanceStatusValue": 1,
  "ClosingBalance": 8500.00,
  "BookBalance": 8500.00,
  "UntransferredChequesValue": 0,
  "DenominationOneThousandValue": 8000.00,
  "DenominationFiveHundredValue": 500.00,
  "DenominationTwoHundredValue": 0,
  "DenominationOneHundredValue": 0,
  "DenominationFiftyValue": 0,
  "DenominationFourtyValue": 0,
  "DenominationTwentyValue": 0,
  "DenominationTenValue": 0,
  "DenominationFiveValue": 0,
  "DenominationOneValue": 0,
  "DenominationFiftyCentValue": 0
}
```

### Cash transfer request — success

```json
POST /api/frontoffice/transfers/cash
{
  "Amount": 5000.00,
  "Reference": "Till top-up",
  "DenominationOneThousandValue": 5000.00,
  "DenominationFiveHundredValue": 0,
  "DenominationTwoHundredValue": 0,
  "DenominationOneHundredValue": 0,
  "DenominationFiftyValue": 0,
  "DenominationFourtyValue": 0,
  "DenominationTwentyValue": 0,
  "DenominationTenValue": 0,
  "DenominationFiveValue": 0,
  "DenominationOneValue": 0,
  "DenominationFiftyCentValue": 0
}
```

## 7. Casing

PascalCase on every field, matching the C# property names exactly (no
`camelCase` resolver configured anywhere in this project — same finding
documented in every other API spec in this set). `TellerCashBalanceStatusValue`,
not `tellerCashBalanceStatusValue`.
