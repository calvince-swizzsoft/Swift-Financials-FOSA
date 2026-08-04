# Standing Order Execution API — Client Integration Spec

Audience: internal admin/ops tooling that needs to manually trigger standing
order batch runs — the same runs `SwiftFinancials.StandingOrderInvoker`
(a separate Quartz-scheduled Windows Service) fires on a cron. This
controller exposes those runs over REST so they can be re-triggered on
demand (e.g. after a scheduled run failed, or for testing) without waiting
for the next cron tick.

This is **not** a customer-facing controller — every endpoint here runs a
potentially large batch operation across many accounts. Gate access to
whatever admin role/screen calls it.

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Accounts/Controllers/StandingOrderExecutionController.cs`
- Domain services it calls:
  `Application.MainBoundedContext/AccountsModule/Services/IRecurringBatchAppService.cs`,
  `Application.MainBoundedContext/AccountsModule/Services/IStandingOrderAppService.cs`
- Reference for what each endpoint mirrors:
  `SwiftFinancials.StandingOrderInvoker/Configuration/StandingOrderJob.cs`,
  `SkippedStandingOrderJob.cs`, `SweepingStandingOrderJob.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

For listing/inspecting/creating individual standing orders, see
`docs/api/standing-order-api-spec.md` — that's a separate controller.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/standingorders/execution` |
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

- `200 OK` with `data: boolean` — the underlying app service returning
  `false` means "ran, but nothing matched" (e.g. no due orders), **not** a
  failure. Check the `message` for which case it was.
- `400 Bad Request` — missing request body.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`. Because these are batch operations, a `500` partway through
  may mean some orders in the batch executed and others didn't — check
  `GET /api/accounts/standingorders/skipped` afterward rather than assuming
  all-or-nothing.

## 3. Key enums

| Enum | Values |
|---|---|
| `QueuePriority` (`priority` on every endpoint here) | `0` Lowest, `1` VeryLow, `2` Low, `3` Normal, `4` AboveNormal, `5` High, `6` VeryHigh, `7` Highest |

`targetDateOption` (§3.1) selects which schedule field `targetDate` is
compared against (`StandingOrderSpecifications.DueStandingOrders`):
- `0` (default) — match against `Schedule.ActualRunDate` (holiday-adjusted
  actual run date).
- `1` — match against `Schedule.ExpectedRunDate` (nominal date, pre-holiday
  adjustment).

`StandingOrderInvoker`'s scheduled `StandingOrderJob` reads this per-entry
from config (`StandingOrderInvokerSettingsElement.TargetDateOption`), so
there's no single "correct" default — confirm with ops which one this
environment's scheduled job actually uses before assuming `0`.

## 4. Endpoints

All routes below are relative to `/api/accounts/standingorders/execution`.
All are `POST` with a JSON body — none take query params.

### 4.1 Execute due standing orders — `POST /execute`

Mirrors `StandingOrderJob` (the Dispatcher). Runs
`IRecurringBatchAppService.ExecuteStandingOrders(targetDate, targetDateOption, priority, maximumStandingOrderExecuteAttemptCount, pageSize)`
— finds and processes standing orders due as of `targetDate`, retrying up to
`maximumStandingOrderExecuteAttemptCount` times per order before it's
considered skipped.

```ts
interface ExecuteStandingOrdersRequest {
  targetDate?: string;                            // ISO date, default: today
  targetDateOption: number;                        // how targetDate is interpreted (see StandingOrderSpecifications)
  priority: number;                                 // QueuePriority enum
  maximumStandingOrderExecuteAttemptCount: number;  // retry ceiling before an order counts as skipped
  pageSize?: number;                                // default 100
}
```

### 4.2 Fix skipped standing orders — `POST /fix-skipped`

Mirrors `SkippedStandingOrderJob` (the Fixer). Runs
`IStandingOrderAppService.FixSkippedStandingOrders(targetDate, pageSize)` —
resets `ScheduleExecuteAttemptCount` to `0` for standing orders that were
skipped on or before `targetDate`, so the next `/execute` run retries them.

```ts
interface FixSkippedStandingOrdersRequest {
  targetDate?: string;  // ISO date, default: yesterday (matches the scheduled job's convention)
  pageSize?: number;    // default 100
}
```

### 4.3 Sweep — `POST /sweep`

Mirrors `SweepingStandingOrderJob` (the Sweeper). Runs
`IRecurringBatchAppService.ExecuteSweepingStandingOrders(priority, pageSize)`
— processes all standing orders with `Trigger = Sweep` (moving an account's
full balance, not a fixed amount).

```ts
interface SweepStandingOrdersRequest {
  priority: number;    // QueuePriority enum
  pageSize?: number;   // default 100
}
```

### 4.4 Payout — `POST /payout`

No scheduled job triggers this one today — it's exposed here purely as an
on-demand action (e.g. "run this member's dividend payout now"). Runs
`IRecurringBatchAppService.ExecutePayoutStandingOrders(benefactorCustomerAccountId, month, priority)`
for a single benefactor account rather than the whole due set.

```ts
interface PayoutStandingOrdersRequest {
  benefactorCustomerAccountId: string;  // GUID
  month: number;                        // 1-12
  priority: number;                     // QueuePriority enum
}
```
