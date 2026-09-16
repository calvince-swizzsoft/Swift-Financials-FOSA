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

- `dlrStatus` defaults to `8` (`Sent`). Valid `DLRStatus` values are
  `1` Unknown, `2` Failed, `4` Pending, `8` Sent, `16` Not Applicable,
  and `32` Submitted (legacy). The numeric/shared enum value 8 remains unchanged;
  `MailMessageDLRStatusDescription` now returns `Sent` for email only. SMS labels are unchanged.
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

## Sending outcomes

The dispatcher processes Pending/Unknown records. Successful SMTP acceptance is
shown as Sent; this does not confirm inbox delivery or that the message was read.
Send/preparation exceptions record Failed and increment the attempt counter.
Once the failure is saved, the queue item is acknowledged and failed records are
not automatically resent. The exception is logged by the dispatcher, not exposed
as raw SMTP details in the UI. Submitted remains available for legacy records.

If saving the status fails, the error propagates so MSMQ retains the queue item.
A failure to save Sent after SMTP acceptance must not falsely record Failed;
SMTP and the database are not atomic, so that failure can still cause a retry.
Missing domain configuration or a missing record remains a processing error;
no safe record update is possible in those cases.

Apply the updated email-dispatcher binaries to the service host and restart that
host, as well as updating the API for the revised status description. Existing
Pending records are not retroactively changed to Failed without a send attempt.
No schema migration is required.
