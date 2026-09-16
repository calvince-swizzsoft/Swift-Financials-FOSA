# Audit history API

Authenticated GET `/api/administration/auditlogs` and `/entries` return
`{ success: true, data: { PageCollection: [...], ItemsCount: n } }`.
These endpoints now return a page rather than an unbounded array.

Parameters: `pageIndex` (zero based, default 0), `pageSize` (1–100, default 20),
`text` (optional, maximum 256 characters), `startDate` and `endDate` (optional
independently, calendar dates). Omit both dates for all history. Results use
descending SequentialId, newest recorded entries first, with a stable unique
ordering. The end date includes the entire day, using an exclusive next-day
boundary. Invalid input returns a standard 400 `audit.invalid_request` error.

Filtering, ordering, count and pagination run in SQL through the AuditLog
AppService. Only the requested page is projected to DTOs. Logs search event,
table, record ID, narration, user and environment fields. Entries search event,
activity, user, designation and environment fields, plus an exact customer GUID.
Searching names through the customer table is not supported.

The UI uses 20/50/100 rows, debounced server search, date filters, refresh/retry,
and request cancellation when filters or tabs change. No schema migration.
