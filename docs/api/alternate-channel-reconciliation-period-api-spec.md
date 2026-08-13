# Alternate Channel Reconciliation Period API — Client Integration Spec

Audience: back-office reconciliation screens — opening a reconciliation
period for a given `AlternateChannelType` and date range, uploading a
bank/processor statement file against it, reviewing matched/unmatched
entries, and closing (or suspending) the period.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/AlternateChannelReconciliationPeriodController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/AlternateChannelReconciliationPeriodAppService.cs`
  (`IAlternateChannelReconciliationPeriodAppService`) — real, already-built
  GL-aware reconciliation logic (matches `AlternateChannelLog` entries
  against an imported file, per `AlternateChannelType` and
  `SetDifferenceMode`), previously only reachable via the legacy
  `AlternateChannelReconciliationPeriodService.svc.cs` WCF passthrough — no
  controller existed before this one, same "fully built, WCF-only" gap
  ChequeBook/UnPayReason had.
- DTOs: `AlternateChannelReconciliationPeriodDTO`, `AlternateChannelReconciliationEntryDTO`,
  `BatchImportEntryWrapper` (`Application.MainBoundedContext.DTO`/`...AccountsModule`).
- Enums (`Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs`):
  `AlternateChannelReconciliationPeriodStatus` (`1`=Open, `2`=Closed,
  `4`=Suspended), `AlternateChannelReconciliationPeriodAuthOption` (`1`=Post,
  `2`=Reject), `AlternateChannelReconciliationEntryStatus` (`1`=Reconciled,
  `2`=Unreconciled), `SetDifferenceMode`, `AlternateChannelType`.
- Auth: same JWT bearer scheme as every other controller — `[Authorize]`.

## History note

Adapted from the reference MVC `AlternatePeriodsController`
(`Areas/Accounts`). Real issues found reading it, not ported:
- `Details(id)` calls `_channelService.FindAlternateChannelAsync(id, ...)` —
  loads an `AlternateChannel`, not a reconciliation period, despite being
  the reconciliation-period controller. Looks miswired/copy-pasted, same
  shape as `RegisterController`'s `Verify`/`Authorize`-bound-to-`DebitBatchDTO`
  mixup found earlier. `GET {id}` below actually fetches the period.
- `Processing(id)`'s GET also calls
  `FindAlternateChannelsByTypeAndFilterInPageAsync(64, 3, null, 2, 0, 10, true, ...)`
  with unexplained magic numbers, unrelated to the period being viewed —
  looks like leftover debug/copy-paste. Not reproduced.
- Large blocks of commented-out field mapping throughout `Index`/
  `Processing`/`Closing` (customer reference fields, cheque fields) — dead
  code, not reproduced.
- `Search` largely duplicates `Index`'s GET logic against a different DTO
  shape — not a distinct operation, not reproduced (same reasoning as
  `AlternateChannelController`'s `Create(id)`/`Linking(id)`/`History(id)`
  GET overloads being pure MVC view-staging).

**Flagged, not fixed**: `ParseAlternateChannelReconciliationImport` (the
app service method behind `POST {id}/import` below) gates on
`persisted.Status == (int)BatchStatus.Pending`, but `Status` is populated
from a different enum entirely (`AlternateChannelReconciliationPeriodStatus`
— Open/Closed/Suspended). It happens to work today only because
`BatchStatus.Pending` and `AlternateChannelReconciliationPeriodStatus.Open`
are both numerically `1` — a coincidence, not a correct type reference. Not
fixed here — it's existing app-service logic, out of scope for adapting a
controller on top of it.

## 1. Reconciliation periods

### `GET /api/accounts/alternatechannelreconciliationperiods`

Unpaged, every reconciliation period.

```json
{ "success": true, "message": "", "data": AlternateChannelReconciliationPeriodDTO[] }
```

### `GET /api/accounts/alternatechannelreconciliationperiods/paged?text=&pageIndex=&pageSize=`

`text` empty/omitted returns the full unfiltered page. `pageIndex` default
`0`, `pageSize` default `20`.

```json
{ "success": true, "message": "", "data": { "pageCollection": AlternateChannelReconciliationPeriodDTO[], "itemsCount": number } }
```

### `GET /api/accounts/alternatechannelreconciliationperiods/paged/status/{status}?startDate=&endDate=&text=&pageIndex=&pageSize=`

`status` (`AlternateChannelReconciliationPeriodStatus`) is a required route
segment — e.g. `status=1` lists every currently-Open period. `startDate`/
`endDate` default to a 30-days-before/30-days-after window (same default
the reference app's `Create`/`Clos` actions used) when omitted.

### `GET /api/accounts/alternatechannelreconciliationperiods/{id}`

```json
{ "success": true, "message": "", "data": AlternateChannelReconciliationPeriodDTO }
```

`404` if not found.

### `POST /api/accounts/alternatechannelreconciliationperiods`

Body: `AlternateChannelReconciliationPeriodDTO`. Required:
`AlternateChannelType`, `DurationStartDate`, `DurationEndDate` (must be
after `DurationStartDate`), `SetDifferenceMode`. Always starts
`Status: Open` (`1`) — the app service sets this, it can't be supplied.
`400` with real validation messages on failure.

```json
{ "success": true, "message": "Operation Success", "data": AlternateChannelReconciliationPeriodDTO }
```

### `PUT /api/accounts/alternatechannelreconciliationperiods/{id}`

Body: `AlternateChannelReconciliationPeriodDTO`. Rewrites
`AlternateChannelType`/duration/`SetDifferenceMode`/`Remarks` — `Status` and
audit fields (`CreatedBy`/`CreatedDate`) are carried over from the persisted
record regardless of what's in the body (the app service does this, not the
controller). `404` if `id` doesn't resolve.

### `POST /api/accounts/alternatechannelreconciliationperiods/{id}/post`

Body (optional): `{ "remarks": "..." }`. `AlternateChannelReconciliationPeriodAuthOption.Post`
— sets `Status: Closed`. Only succeeds while the period is currently
`Open`; otherwise `409`.

### `POST /api/accounts/alternatechannelreconciliationperiods/{id}/reject`

Same body shape and Open-only precondition as `post` above, but sets
`Status: Suspended` — **not** a rejection of the period's existence, it
still exists, just no longer `Open`. Naming matches the underlying
`AlternateChannelReconciliationPeriodAuthOption.Reject` value directly
rather than inventing new terminology for it.

## 2. Entries — `{periodId}/entries`

### `GET /api/accounts/alternatechannelreconciliationperiods/{id}/entries?status=&text=&pageIndex=&pageSize=`

`status` (`AlternateChannelReconciliationEntryStatus`) — `0` (unset)
matches every status; the app service's underlying specification treats it
as "no filter." `pageIndex` default `0`, `pageSize` default `20`.

```json
{ "success": true, "message": "", "data": { "pageCollection": AlternateChannelReconciliationEntryDTO[], "itemsCount": number } }
```

### `POST /api/accounts/alternatechannelreconciliationperiods/{id}/entries`

Body: `AlternateChannelReconciliationEntryDTO`. `AlternateChannelReconciliationPeriodId`
is set from the route, not read from the body. Required:
`PrimaryAccountNumber`/`SystemTraceAuditNumber`/`RetrievalReferenceNumber`/`Amount`
per the underlying factory — `400` with real validation messages on
failure.

```json
{ "success": true, "message": "Operation Success", "data": AlternateChannelReconciliationEntryDTO }
```

### `POST /api/accounts/alternatechannelreconciliationperiods/{id}/entries/remove`

Body: `AlternateChannelReconciliationEntryDTO[]` — only each entry's `.Id`
is read. `POST`, not `DELETE`, same reasoning as
`AlternateChannelController.Delink` (a body is required to identify which
entries to remove; `DELETE` request bodies are unreliable across clients).

```json
{ "success": true, "message": "Operation Success", "data": null }
```

## 3. Batch import — `POST /api/accounts/alternatechannelreconciliationperiods/{id}/import`

`multipart/form-data`, field name `file` — a bank/processor statement CSV
(4 columns minimum: reference number, primary account number, amount,
your-reference). Same upload shape as
`AutomatedClearingController.Upload` (`Areas/FrontOffice`) — saved to the
server-side file-upload directory from `serviceBrokerConfiguration`
(`ConfigurationHelper.GetServiceBrokerConfigurationSettings`), never a
client-supplied path.

Internally: matches each `AlternateChannelLog` entry for this period's
`AlternateChannelType`+date range against the uploaded file, using
`SetDifferenceMode` to decide the comparison direction (RRN/STAN/callback-
payload, file-vs-system or system-vs-file). Matched entries are saved
directly as `Reconciled` `AlternateChannelReconciliationEntry` rows (via
`AddNewAlternateChannelReconciliationEntry` internally) and the matching
source `AlternateChannelLog` rows are marked reconciled — **not** returned
in the response. Only the mismatched/unreconciled side comes back:

```json
{
  "success": true,
  "message": "Reconciliation file uploaded and parsed successfully.",
  "data": [
    { "Column1": "...", "Column2": "...", "Column3": "...", "Column4": "...", "Remarks": "RRN exists in File but not System" }
  ]
}
```

`400` if the period isn't currently `Open` (see the `BatchStatus.Pending`
flag above — same-numbered-coincidence, but functionally: not `Open` means
this fails today) or the file couldn't be parsed. `500` if the server-side
file-upload directory isn't configured.
