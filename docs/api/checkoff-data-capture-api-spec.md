# Checkoff Data Capture API

## Purpose and business flow

This API exposes the legacy payroll/checkoff data-attachment workflow as REST without changing its domain entities:

1. Open a `DataAttachmentPeriod` against an active posting period and calendar month.
2. Capture one or more `DataAttachmentEntry` records against customer product accounts.
3. Close the period with authorization remarks.
4. Browse the closed or open period through the read-only catalogue.

Captured entries are the upstream data later consumed by `CheckOff` credit-batch processing. Closing a period does not itself post accounting entries.

Base route: `/api/backoffice/checkoff-data-capture`.

## Permissions

| Permission | Capability |
| --- | --- |
| `BackOfficeCheckOffPeriodManagement` | List/create/edit data periods and list active posting periods |
| `BackOfficeCheckOffDataCapture` | Resolve the current period and add/remove entries while it is open |
| `BackOfficeCheckOffPeriodClosing` | Review entries and close an open period |
| `BackOfficeCheckOffCatalogueViewing` | Browse periods and captured entries read-only |

Authentication and at least one endpoint-appropriate role mapping are required. Read access to the shared period listing is allowed when the caller has any of the four permissions.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| GET | `/periods?text=&pageIndex=0&pageSize=20` | Search and page periods |
| GET | `/periods/current` | Current active/open period for capture |
| GET | `/periods/{id}` | Period details |
| GET | `/posting-periods` | Active posting periods for opening a period |
| POST | `/periods` | Open a period with `postingPeriodId`, `month`, `remarks`, and optional `isActive` |
| PUT | `/periods/{id}` | Change month/remarks while open; posting period is immutable |
| POST | `/periods/{id}/close` | Close with `{ remarks }` |
| GET | `/periods/{id}/entries?text=&pageIndex=0&pageSize=20` | Search/page captured entries |
| POST | `/periods/{id}/entries` | Capture an entry |
| DELETE | `/periods/{periodId}/entries/{entryId}` | Remove an entry while open |

The server owns the entry's period ID and sequence number. Sequence numbers increment independently per customer account and transaction type. Mutation of entries is rejected after the period closes, correcting a gap in the legacy MVC layer. Closing records the authenticated username, UTC/domain close date used by the existing service, and supplied authorization remarks.

`DataAttachmentTransactionType` values are: `1` Fresh Loan, `2` Adjust Balance, `3` Variation, `4` New Member, `5` Special Adjustments, `6` Stop Deduction, `7` Shares Deposit, `8` Risk Fund, and `9` Entrance Fee.

## Navigation and UI

The Back Office `Data Capture` folder maps as follows:

- `70019` Data Periods → `/Loaning/CheckOff/DataPeriods`
- `70020` Data Processing → `/Loaning/CheckOff/DataProcessing`
- `70021` Closing → `/Loaning/CheckOff/Closing`
- `70022` Catalogue → `/Loaning/CheckOff/Catalogue`

The processing UI first selects a searchable customer, then loads only that customer's accounts. Account options show customer, product, and full account number.
