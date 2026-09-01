# Standing Order API — Client Integration Spec

Audience: any frontend that lists, inspects, creates, or edits standing
orders (recurring transfer instructions between a benefactor and beneficiary
customer account) against `WebApplication1`'s `StandingOrderController`.

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Accounts/Controllers/StandingOrderController.cs`
- Domain service it calls: `Application.MainBoundedContext/AccountsModule/Services/IStandingOrderAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO/AccountsModule/StandingOrderDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2 for the login flow and header shape.

If the controller changes, regenerate this doc from source rather than
editing it out of sync.

For the batch-execution endpoints (running due orders, fixing skipped ones,
sweeping, payouts), see `docs/api/standing-order-execution-api-spec.md` —
that's a separate controller.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/standingorders` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | Bearer JWT on every request |

## 2. Response envelope

Every endpoint returns the standard envelope used across the API:

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success, or a caught business error (`success: false`).
- `201 Created` — successful `POST /` (standing order created).
- `400 Bad Request` — validation error, or a mismatched/missing id.
- `404 Not Found` — id doesn't resolve to a standing order.
- `409 Conflict` — `POST /` succeeded at the DTO level but the app service
  attached a business-rule error to the created record (see §5.9).
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

## 3. Paging shape

List endpoints that page return `PageCollectionInfo<StandingOrderDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

`pageIndex` is 0-based.

## 4. Key enums

| Enum | Values |
|---|---|
| `StandingOrderTrigger` (`trigger`) | `0` Payout, `1` Check-Off, `2` Schedule, `3` Sweep, `4` Microloan |
| `ScheduleFrequency` | `1` Annual, `2` SemiAnnual, `3` Quarterly, `4` TriAnnual, `6` BiMonthly, `12` Monthly, `24` SemiMonthly, `26` BiWeekly, `52` Weekly, `365` Daily |
| `ChargeType` | `1` Percentage, `2` FixedAmount |
| `RoundingType` (`BeneficiaryProductRoundingType`, loan beneficiaries only) | `0` NoRounding, `1` ToEven, `2` AwayFromZero, `3` Ceiling, `4` Floor |
| `CustomerFilter` (`customerFilter`) | `0` SerialNumber, `1` PersonalIdentificationNumber, `2` FirstName, `3` LastName, `4` IdentityCardNumber, `5` PayrollNumbers, ... (see `Enumerations.cs`) |
| `StandingOrderCustomerAccountFilter` (`customerAccountFilter`) | `0` Beneficiary, `1` Benefactor |
| `ProductCode` (`productCode` in §5.6) | `1` Savings, `2` Loan, `3` Investment |

`targetDateOption` (§5.7 `GET /due`, and the execution controller's
`POST /execute`) selects which schedule field `targetDate` is compared
against (`StandingOrderSpecifications.DueStandingOrders`):
- `0` (default) — match against `Schedule.ActualRunDate` (the
  holiday-adjusted date the order is actually slated to run).
- `1` — match against `Schedule.ExpectedRunDate` (the nominal date before
  holiday adjustment).

Any other value falls back to `0`'s behavior (`switch` default case).

## 5. Endpoints

All routes below are relative to `/api/accounts/standingorders`.

### 5.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `text`, `customerAccountFilter` (int, default `0`), `customerFilter`
(int, default `0`), `trigger` (int, `StandingOrderTrigger`).

- `trigger` supplied → filtered-and-paged-by-trigger search.
- Else, `text` supplied → filtered-and-paged text search.
- Else → plain paged listing.

Returns `ApiEnvelope<PageCollectionInfo<StandingOrderDTO>>`.

### 5.2 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<StandingOrderDTO>`.

### 5.3 History — `GET /{id}/history`

Query: `pageIndex` (default `0`), `pageSize` (default `20`). Returns
`ApiEnvelope<PageCollectionInfo<StandingOrderHistoryDTO>>`. `404` if no
history page could be built (e.g. `id` doesn't resolve).

### 5.4 By benefactor account — `GET /by-benefactor-account/{benefactorCustomerAccountId}`

Query: `trigger` (optional int). Returns `ApiEnvelope<StandingOrderDTO[]>` —
all standing orders where this account is the *benefactor* (the paying
side), optionally narrowed to one trigger type.

### 5.5 By beneficiary account — `GET /by-beneficiary-account/{beneficiaryCustomerAccountId}`

Same shape as §5.4, for the *beneficiary* (receiving) side.

### 5.6 By benefactor customer — `GET /by-benefactor-customer/{benefactorCustomerId}`

Query: `productCode` (int, `ProductCode` enum — which of the customer's
accounts to match as benefactor). Returns `ApiEnvelope<StandingOrderDTO[]>`.

### 5.7 Due — `GET /due`

Query: `targetDate` (ISO date, default today), `targetDateOption` (int —
controls how `targetDate` is interpreted by the underlying specification,
e.g. exact vs. up-to), `text`, `customerAccountFilter`, `customerFilter`.
Returns `ApiEnvelope<StandingOrderDTO[]>` — standing orders due to run on or
around `targetDate`. Unpaged; intended for operational review, not UI
listing of large result sets.

### 5.8 Skipped — `GET /skipped`

Query: `targetDate` (ISO date, default today), `text`,
`customerAccountFilter`, `customerFilter`, `pageIndex`, `pageSize`. Returns
`ApiEnvelope<PageCollectionInfo<StandingOrderDTO>>` — standing orders that
were due on or before `targetDate` but didn't execute.

### 5.9 Create — `POST /`

Body: full `StandingOrderDTO`. The server runs `ValidateAll()` first (`400`
with the collected `ErrorMessages` joined by `; ` if invalid), then calls
`AddNewStandingOrder`.

The application service also validates the account relationship and business
rules against persisted data. Both accounts must exist, must be different,
and must not be closed; dates and enum values must be valid; monetary values
cannot be negative; non-sweep savings/investment orders require a positive
fixed amount or a percentage in the range `(0, 100]`; and loan beneficiaries
require a positive principal and/or interest recovery. These failures return
`400 Bad Request` with the specific validation message.

A standing order with the same benefactor/beneficiary/trigger combination
is rejected before persistence. The app service returns the submitted DTO
with `ErrorMessageResult` describing the conflict, and the controller
surfaces it as `409 Conflict`. No duplicate record is created.

Success → `201`:
```json
{ "success": true, "message": "Standing order created successfully", "data": StandingOrderDTO }
```

### 5.10 Update — `PUT /{id}`

Body: full `StandingOrderDTO`. `id` in the URL must equal `data.id` in the
body (`400` otherwise). `404` if the standing order doesn't exist. Runs
`ValidateAll()` the same way as Create. On success, re-fetches and returns
the updated `StandingOrderDTO`.

Note: editing the schedule's start date or frequency after skipped runs have
accrued causes the service to recompute `ScheduleExpectedRunDate` /
`ScheduleActualRunDate` server-side (walking forward against the holiday
calendar) — don't attempt to compute or override these fields client-side.

### 5.11 Auto-create — `POST /auto-create`

```ts
interface AutoCreateStandingOrdersRequest {
  benefactorProductId: string;   // GUID
  benefactorProductCode: number; // ProductCode enum
  beneficiaryProductId: string;  // GUID
}
```

Bulk-provisions a monthly Payout standing order (₨50 fixed charge, 5-year
duration) for every customer who holds both the benefactor and beneficiary
product and doesn't already have one. Intended for admin/back-office
tooling, not a per-customer create flow. Returns `ApiEnvelope<boolean>` —
`false` just means nothing new was created (e.g. no matching accounts),
not an error.
