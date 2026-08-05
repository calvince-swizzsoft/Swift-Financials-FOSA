# Text Alert API — Client Integration Spec

Audience: messaging/notification screens that list or manually create SMS
text alerts.

Source of truth:
- Controller: `WebApplication1/Areas/Messaging/Controllers/TextAlertController.cs`
- Domain service: `Application.MainBoundedContext/MessagingModule/Services/ITextAlertAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO/MessagingModule/TextAlertDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

## History note

Adapted from the reference MVC
`SwiftFinancials.Web/Areas/Messaging/Controllers/TextAlertController.cs`,
which routed through the monolithic `IChannelService`/`_channelService`
facade (`FindTextAlertsByFilterInPageAsync`, `AddTextAlertsAsync`) and
rendered a jQuery DataTables view. This controller routes directly through
the existing `ITextAlertAppService` instead — no new app service was needed.

**Behavior differences from the old controller, if you're porting logic
from it:**
- The old `Create()` action called `_channelService.AddTextAlertsAsync(...)`
  **even when validation failed** — a copy-paste bug that persisted the
  invalid alert anyway before showing the error. `POST /` here only
  persists after `TextAlertDTO.ValidateAll()` passes.
- The old list view hardcoded its DLR-status filter to `Delivered` (it was
  specifically a "delivered alerts" dashboard, not a general list). `GET /`
  here defaults to **unfiltered** paging; pass `dlrStatus=8` explicitly to
  reproduce the old "Delivered only" behavior.
- No update/delete endpoints — the reference controller never exposed them
  either (a text alert here is closer to an audit/message-log entry than a
  fully editable record), even though `ITextAlertAppService.UpdateTextAlert`
  exists on the interface. Ask if you need it exposed.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/messaging/textalert` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | Bearer JWT on every request |

## 2. Response envelope

Same `ApiEnvelope<T>` as every other controller — see
`docs/api/branch-api-spec.md` §2. Status codes used here:
- `200 OK` — success.
- `201 Created` — successful `POST`.
- `400 Bad Request` — missing body, or `TextAlertDTO.ValidateAll()` failure
  (`message` is the joined validation error text).
- `404 Not Found` — `id` doesn't resolve.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

## 3. Paging shape

`GET /` returns `PageCollectionInfo<TextAlertDTO>` — same shape as
`docs/api/branch-api-spec.md` §3.

## 4. Endpoints

All routes below are relative to `/api/messaging/textalert`.

### 4.1 List / search — `GET /`

Query params (all optional): `pageIndex` (default `0`), `pageSize` (default
`20`), `dlrStatus` (int — see §6), `text`.

- Omitting `dlrStatus` returns the plain, **unfiltered** paged listing —
  `text` is ignored in this case, since the unfiltered app-service overload
  takes no text parameter.
- Supplying `dlrStatus` runs the filtered lookup (`dlrStatus` + `text`
  together); `text` defaults to empty (no text filter) if omitted.

There's no date-range filter exposed yet, even though the app service has
an overload for one (`FindTextAlerts(dlrStatus, startDate, endDate, text,
...)`) — ask if you need it surfaced.

### 4.2 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<TextAlertDTO>`.

### 4.3 Create — `POST /`

Body: `TextAlertDTO` with `branchId`, `textMessageRecipient`, and
`textMessageBody` populated (`textMessageRecipient` and `textMessageBody`
are `[Required]`; `branchId` must be a non-empty GUID — `400` with the
specific validation message if `ValidateAll()` fails).

The following fields are **server-assigned** — any value sent for them is
overwritten before validation runs, matching the reference controller's
manual-alert defaults:

| Field | Forced value |
|---|---|
| `textMessageSecurityCritical` | `true` |
| `textMessagePriority` | `5` (`QueuePriority.High`) |
| `textMessageDLRStatus` | `4` (`DLRStatus.Pending`) |
| `textMessageOrigin` | `1` (`MessageOrigin.Within`) |
| `textMessageSendRetry` | `0` |

`messageCategory` is **not** overwritten — send it explicitly. If it's
`1` (`SMSAlert`), `textMessageRecipient` must additionally match
`^\+(?:[0-9]??){6,14}[0-9]$` (E.164-ish, leading `+`) or the app service
rejects the create outright (returns `null`, surfaced here as `500
Failed to create text alert` — this is an app-service short-circuit, not a
`ValidateAll()` failure, so it isn't caught by the `400` path).

If `appendSignature` is `true` and `branchId` resolves to a real branch, the
branch's description/company description are appended to the message body
server-side before sending — the `textMessageBody` you sent is not what
ultimately gets dispatched.

Success → `201`:
```json
{ "success": true, "message": "Text alert created successfully", "data": TextAlertDTO }
```

## 5. Navigation item code

Unlike the front-office transaction endpoints (cash deposit/withdrawal
authorization, etc.), `POST /` here does **not** take a
`moduleNavigationItemCode` parameter — `TextAlertDTO` has no such field, and
`TextAlertAppService.AddNewTextAlert` doesn't gate on one internally
(verified against the implementation: it only validates the recipient
format and optionally appends a branch signature — no permission/nav-code
check happens). Sending one in the request body is silently ignored, since
the DTO has no matching property to bind it to.

For menu/permission-registration purposes — deciding whether to show a
"Text Alerts" entry and what to check against `GET
/api/administration/modules` (same caveat as `customer-api-spec.md` §5.12)
— the closest equivalent in the reference app's seeded navigation
(`Infrastructure.Crosscutting.Framework/Utils/NavigationMenu.cs`) is:

| Area | Description | Legacy code (reference app only) |
|---|---|---|
| Dashboard → Messaging | Text Alerts | `0x00006590 + 8` |

That entry is a **read-only listing dashboard** tied to a different
reference controller (`Areas/Dashboard/Controllers/TextAlertsController.cs`,
plural) than the create-capable one this spec adapts
(`Areas/Messaging/Controllers/TextAlertController.cs`, singular) — there is
no seeded nav entry at all for the latter. Don't hardcode the legacy
hex-sum code above; it belongs to the old app's own `NavigationMenu` seed
list, not a table this API shares by direct migration. If/when this feature
gets a real nav entry registered in the new system, source its current code
from `GET /api/administration/modules` rather than a fixed constant.

## 6. Enum reference

`DLRStatus` (`textMessageDLRStatus`, and the `dlrStatus` query/filter param):

| Value | Name |
|---|---|
| 1 | UnKnown |
| 2 | Failed |
| 4 | Pending |
| 8 | Delivered |
| 16 | NotApplicable |
| 32 | Submitted |

`MessageOrigin` (`textMessageOrigin` — forced to `1` on create, listed for
completeness when reading existing records):

| Value | Name |
|---|---|
| 1 | Within |
| 2 | Without |
| 4 | Other |

`MessageCategory` (`messageCategory` — send explicitly, not defaulted):

| Value | Name |
|---|---|
| 1 | SMSAlert |
| 2 | USSDQuery |
| 4 | EmailAlert |
| 8 | PluginAlert |
| 16 | CreditBatchEntry |

(a few more exist beyond `CreditBatchEntry`; ask if you need the full list)

`QueuePriority` (`textMessagePriority` — forced to `5`/High on create):

| Value | Name |
|---|---|
| 0 | Lowest |
| 1 | VeryLow |
| 2 | Low |
| 3 | Normal |
| 4 | AboveNormal |
| 5 | High |
| 6 | VeryHigh |
| 7 | Highest |
