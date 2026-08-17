# Email Alerts API

Base path: `api/messaging/emailalerts`. All endpoints require a bearer JWT.
The controller is a REST adaptation of the useful behavior split across the
reference MVC `Messaging/EmailAlertController` (manual composition) and
`Dashboard/EmailAlertsController` (history/status monitoring).

Creating an alert does not perform SMTP delivery inside the HTTP request.
It persists the alert and places its ID on the email MSMQ queue; see
[`../EMAIL-DELIVERY.md`](../EMAIL-DELIVERY.md) for the dispatcher flow.

## List and filter

`GET /?dlrStatus=8&text=&startDate=&endDate=&pageIndex=0&pageSize=20`

- `dlrStatus` defaults to `8` (`Delivered`). Valid `DLRStatus` values are
  `1` Unknown, `2` Failed, `4` Pending, `8` Delivered, `16` Not Applicable,
  and `32` Submitted.
- `text` searches recipient, subject, and body through the existing app
  service specification.
- `startDate` and `endDate` are optional, but must be supplied together.
- `pageIndex` is zero-based; `pageSize` must be between 1 and 1000.

Returns the standard envelope containing `PageCollectionInfo<EmailAlertDTO>`.

## Retrieve one alert

`GET /{id}` returns the stored email alert or `404`.

## Compose and queue

`POST /`

```json
{
  "branchId": "optional-guid",
  "mailMessageFrom": "branch@example.org",
  "mailMessageTo": "recipient@example.org",
  "mailMessageCC": "optional@example.org",
  "mailMessageSubject": "Subject",
  "mailMessageBody": "<p>Message body</p>",
  "mailMessageIsBodyHtml": true,
  "mailMessagePriority": 3,
  "mailMessageSecurityCritical": false
}
```

The caller supplies composition fields. The controller always overwrites
transport lifecycle fields before validation/persistence:

- `mailMessageDLRStatus = 4` (`Pending`)
- `mailMessageOrigin = 1` (`Within`)
- `mailMessageSendRetry = 0`
- `mailMessageAttachments = ""`
- identity/audit fields (`id`, `createdBy`, `createdDate`)

The dispatcher sends using its configured SMTP identity, replacing the
stored `mailMessageFrom` after successful SMTP acceptance. The public
manual-composition endpoint does not accept attachments; dispatcher-created
internal alerts retain their existing staged-attachment mechanism.

Returns `201` when the alert was persisted and queued, `400` for invalid
input, or `409` when the application service cannot queue it. A successful
response means queued—not delivered to an inbox.

There are intentionally no public update/delete endpoints. Updates are an
internal dispatcher concern used to record delivery state.
