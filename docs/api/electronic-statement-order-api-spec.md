# Electronic Statement Order API — Client Integration Spec

Audience: any frontend that lists, inspects, creates, or edits e-statement
orders — a per-customer-account subscription to have a statement generated
and emailed on a recurring schedule — against `WebApplication1`'s
`ElectronicStatementOrderController`.

**Not to be confused with `docs/api/customer-account-statement-api-spec.md`**
(`CustomerAccountStatementController`) — that's on-demand statement content
(mini/full transaction history, PDF print), a completely different resource
with no overlap. This controller only manages the *subscription* (schedule,
duration, run history) — it never renders or emails a statement itself; see
§6 for what actually does.

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Accounts/Controllers/ElectronicStatementOrderController.cs`
- Domain service it calls: `Application.MainBoundedContext/AccountsModule/Services/IElectronicStatementOrderAppService.cs`
- Core DTOs: `Application.MainBoundedContext.DTO/AccountsModule/ElectronicStatementOrderDTO.cs`,
  `ElectronicStatementOrderHistoryDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

For triggering the actual generate-and-send batch run, see
`docs/api/electronic-statement-order-execution-api-spec.md` — that's a
separate controller, same split as Standing Orders
(`standing-order-api-spec.md` / `standing-order-execution-api-spec.md`) and
for the same reason: the batch-execution capability lives on a different
app service (`IRecurringBatchAppService`) than this CRUD one.

## History note

Adapted from the reference MVC `CoA_eStatementsController`
(`Areas/Accounts`) — the only reference controller that touches
`ElectronicStatementOrder` at all (two other reference controllers,
`CashDepositController`/`CustomerReceiptsController`, only make incidental
read-only calls to display a customer's existing subscriptions inline on a
teller screen — not CRUD, not ported as separate endpoints here since
`GET /by-customer-account/{id}` already covers that).

The reference app's own navigation entry for this screen
(`NavigationMenu.cs`, `Description = "E-Statements"`) declares
`ControllerName = "eStatements"` — missing the `"CoA_"` prefix every
sibling entry in the same nav group correctly includes for its own
controller (`CoA_Management`, `CoA_Signatories`, `CoA_ChequeBooks`, ...).
No `eStatementsController` class exists anywhere in the reference app; this
looks like a typo in that one nav entry, not a deliberate difference. Not
relevant to this API directly, but worth knowing if/when a nav item is
registered for this controller in the new system — don't reproduce the
missing prefix.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/electronicstatementorders` |
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
- `201 Created` — successful `POST /`.
- `400 Bad Request` — validation error, mismatched/missing id, or a
  business rule enforced inside the app service itself (see §5.9 — "start
  date must not be less than today" on `PUT`).
- `404 Not Found` — id doesn't resolve.
- `409 Conflict` — `POST /` for a `customerAccountId` that already has an
  e-statement order (see §5.8 — unlike most other controllers in this API,
  the underlying app service **throws** rather than setting an
  `ErrorMessageResult` field; `ElectronicStatementOrderDTO` has no such
  field at all). Surfaced here as a real `409`, not a generic `500`.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

## 3. `CustomerFilter` (the `customerFilter` query param used throughout)

Same enum as `docs/api/customer-api-spec.md` §7: `0`=Serial Number,
`1`=Personal Identification #, `2`=First Name, `3`=Last Name, plus address/
reference fields — see that doc for the full list.

## 4. `ElectronicStatementOrderDTO` — fields worth knowing

| Field | Notes |
|---|---|
| `id` | Server-assigned on create. |
| `customerAccountId` | `[ValidGuid]`, required. One order per account — see §5.8. |
| `durationStartDate` / `durationEndDate` | The subscription's active window. |
| `scheduleFrequency` | `ScheduleFrequency` enum (Daily/Weekly/BiWeekly/SemiMonthly/Monthly/BiMonthly/Quarterly/TriAnnual/SemiAnnual/Annual). |
| `scheduleExpectedRunDate` / `scheduleActualRunDate` | Nominal vs. holiday-adjusted next run date — server-computed on create/update, not client-set (see §5.9's date-recalculation logic). |
| `scheduleExecuteAttemptCount` | How many times the batch job has tried and failed this order since it was last reset. |
| `scheduleForceExecute` | Bypasses whatever gate normally skips an order (exact semantics live in the batch executor, not this controller). |
| `isLocked` | A locked order is excluded from due/skipped batch queries. |
| `remarks` | Free text. |

`ElectronicStatementOrderHistoryDTO` (read-only, §5.3) is a **run audit
log**, not statement content — `scheduleExpectedRunDate`/
`scheduleActualRunDate`/`scheduleExecuteAttemptCount`/`scheduleIsExecuted`/
`sender` per past run, plus the same customer/account descriptive fields
every other DTO in this API carries for display convenience.

## 5. Endpoints

All routes below are relative to `/api/accounts/electronicstatementorders`.

### 5.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `text`, `customerFilter` (int, §3). Omitting `text` returns the plain
paged listing. Returns `ApiEnvelope<PageCollectionInfo<ElectronicStatementOrderDTO>>`.

### 5.2 Get one — `GET /{id}`

`404` if not found.

### 5.3 Run history — `GET /{id}/history`

Paged (`pageIndex`, `pageSize`), `ApiEnvelope<PageCollectionInfo<ElectronicStatementOrderHistoryDTO>>`.
`404` if `id` doesn't resolve (checked against the order itself, not just an
empty history page).

### 5.4 By customer account — `GET /by-customer-account/{customerAccountId}`

Unpaged. Returns `ApiEnvelope<ElectronicStatementOrderDTO[]>` (`[]` if none
— never `null`).

### 5.5 By customer — `GET /by-customer/{customerId}?productCode=`

Unpaged, scoped further by `customerAccountTypeProductCode` (default `0` —
check what `0` resolves to before relying on it as "all products"; not
verified against the underlying specification here). Same `[]`-not-`null`
guarantee as §5.4.

### 5.6 Due — `GET /due?targetDate=&targetDateOption=&text=&customerFilter=`

`targetDate` default: today. `targetDateOption`: `0` (default) matches
`Schedule.ActualRunDate`, `1` matches `Schedule.ExpectedRunDate` — same
convention as `docs/api/standing-order-execution-api-spec.md` §3. Unpaged.

### 5.7 Skipped — `GET /skipped?targetDate=&text=&customerFilter=`

`targetDate` default: today. **Unpaged** — unlike Standing Order's
equivalent (`standing-order-api-spec.md`'s skipped listing, which pages),
`IElectronicStatementOrderAppService.FindSkippedElectronicStatementOrders`
returns a bare `List<T>`, verified directly against the interface rather
than assumed from the naming symmetry with Standing Orders.

### 5.8 Create — `POST /`

Body: `ElectronicStatementOrderDTO`. `400` if `ValidateAll()` fails. `409`
if `customerAccountId` already has an order — the app service throws
`InvalidOperationException("Sorry, but an e-statement order for the
selected account already exists!")` rather than returning an
`ErrorMessageResult` (the DTO doesn't have that field), so this controller
catches it explicitly to produce a real `409` instead of a `500`.

Success → `201`, `data` is the created `ElectronicStatementOrderDTO`.

### 5.9 Update — `PUT /{id}`

Body: full `ElectronicStatementOrderDTO`, `id` in the URL must match
`electronicStatementOrderDTO.id` (`400` otherwise). `404` if the order
doesn't exist.

**`400` (not `500`) if the new `durationStartDate` is in the past** — the
app service enforces this itself
(`"The start date must not be less than today!"`) and isn't caught by
`ValidateAll()`, so this controller catches the specific exception to
report it as a real validation error.

If the start date or the schedule's next-run date changed since last edit,
the app service recomputes `scheduleExpectedRunDate`/`scheduleActualRunDate`
from `scheduleFrequency`, adjusting for configured holidays — don't
pre-compute these client-side and send stale values expecting them to
stick; send the new `durationStartDate`/`scheduleFrequency` and let the
server derive the run dates.

Success: `data` is the freshly re-fetched `ElectronicStatementOrderDTO`.

## 6. What actually sends the statement — not this controller

Creating/editing an order here only manages the subscription record. The
actual "generate this account's statement and email it" work is a separate
batch process — see `docs/api/electronic-statement-order-execution-api-spec.md`.
An order with no batch run ever triggered against it will just sit there,
`scheduleActualRunDate` never advancing.
