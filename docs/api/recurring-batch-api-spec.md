# Recurring Batch Inspection API

Read-only operational inspection of asynchronous recurring-procedure batches.
All routes require bearer authentication and return `{ success, message, data }`.

Base path: `api/accounts/recurringbatches`

## Endpoints

- `GET /?type=&pageIndex=0&pageSize=20` — newest recurring batches, paged;
  optional `type` is a `RecurringBatchType` value (`0` through `9`). Each
  `RecurringBatchDTO` includes `PostedEntries` as `posted/total`.
- `GET /{id}` — one batch; `404` when absent.
- `GET /{id}/entries?text=&pageIndex=0&pageSize=20` — the batch's entries,
  including `StatusDescription`, `Remarks`, linked `StandingOrder`, customer
  accounts, reference, creator, and creation time.
- `GET /queueable?pageIndex=0&pageSize=20` — pending entries belonging to a
  posted batch. This is an operational backlog view, not proof that the worker
  is healthy.

`pageIndex` is zero-based. `pageSize` must be between 1 and 200.

The API is deliberately read-only: posting remains the responsibility of the
configured recurring-batch worker and its MSMQ consumer.
