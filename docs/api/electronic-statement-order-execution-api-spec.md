# Electronic Statement Order Execution API — Client Integration Spec

Audience: internal admin/ops tooling that needs to manually trigger
e-statement batch runs — generating and emailing statements for every
subscription due as of a given date. Same shape and purpose as
`docs/api/standing-order-execution-api-spec.md`, split out of the CRUD
controller for the same reason: the actual batch-execution capability lives
on a different app service (`IRecurringBatchAppService`) than order
CRUD/listing (`IElectronicStatementOrderAppService`).

**Not customer-facing.** Both endpoints here run a potentially large batch
operation across many accounts. Gate access to an admin role/screen.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/ElectronicStatementOrderExecutionController.cs`
- Domain services it calls:
  `Application.MainBoundedContext/AccountsModule/Services/IRecurringBatchAppService.cs`
  (`ExecuteElectronicStatementOrders`),
  `Application.MainBoundedContext/AccountsModule/Services/IElectronicStatementOrderAppService.cs`
  (`FixSkippedElectronicStatementOrders`)
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

For listing/inspecting/creating individual e-statement orders, see
`docs/api/electronic-statement-order-api-spec.md`.

Unlike Standing Orders, **no separate scheduled Windows Service was found**
for this batch (`SwiftFinancials.StandingOrderInvoker` has no e-statement
equivalent in this codebase) — confirm with ops whether/how this runs on a
schedule today before assuming `POST /execute` is purely a manual
supplement to an existing cron job, the way its Standing Order counterpart
is.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/electronicstatementorders/execution` |
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

- `200 OK` with `data: boolean` — `false` means "ran, but nothing matched"
  (e.g. no due orders), **not** a failure. Check `message`.
- `400 Bad Request` — missing request body.
- `500 Internal Server Error` — unhandled exception. Because these are
  batch operations, a `500` partway through may mean some orders in the
  batch executed and others didn't — check
  `GET /api/accounts/electronicstatementorders/skipped` afterward rather
  than assuming all-or-nothing.

## 3. Endpoints

Both `POST` with a JSON body, no query params.

### 3.1 Execute due orders — `POST /execute`

Runs `IRecurringBatchAppService.ExecuteElectronicStatementOrders(targetDate,
targetDateOption, sender, priority, pageSize)` — finds every e-statement
order due as of `targetDate` (paged internally, not capped by the
`pageSize` you send being a hard ceiling on total processed), creates a
`RecurringBatch`/`RecurringBatchEntry` audit record per run (`Type:
ElectronicStatementOrder`), and stamps `sender` onto each entry as the
outgoing email's from-address/display name.

```ts
interface ExecuteElectronicStatementOrdersRequest {
  targetDate?: string;    // ISO date, default: today
  targetDateOption: number; // 0 = match Schedule.ActualRunDate, 1 = match Schedule.ExpectedRunDate — see electronic-statement-order-api-spec.md §5.6
  sender: string;          // From-address/display name stamped on each RecurringBatchEntry for this run
  priority: number;        // QueuePriority enum, same as standing-order-execution-api-spec.md §3
  pageSize?: number;       // default 100
}
```

### 3.2 Fix skipped orders — `POST /fix-skipped`

Runs `IElectronicStatementOrderAppService.FixSkippedElectronicStatementOrders(targetDate)`
— resets `scheduleExecuteAttemptCount` to `0` for every order skipped on or
before `targetDate`, so the next `/execute` run retries them.

```ts
interface FixSkippedElectronicStatementOrdersRequest {
  targetDate?: string;  // ISO date, default: yesterday
}
```

Unlike its Standing Order counterpart (`standing-order-execution-api-spec.md`
§4.2), this one takes **no `pageSize`** —
`FixSkippedElectronicStatementOrders` doesn't accept one; verified directly
against the interface, not assumed from the naming symmetry.
