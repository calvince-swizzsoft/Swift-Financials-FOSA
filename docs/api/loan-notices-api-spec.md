# Defaulter notice workflow API

Base: /api/backoffice/loan-notices; authenticated API; navigation module 70025.

## Endpoints

- GET /workflow?asAt=2026-09-21&stage=FirstNotice&pageIndex=0&pageSize=20: exclusive queue. Stage is FirstNotice, SecondNotice, FinalDemand or Recovery. Returns total, items, reviewCount, disabledCount and issues. Filtering precedes pagination.
- GET /eligible?asAt=2026-09-21&pageIndex=0&pageSize=20&otherOnly=true: eligible reminders and guarantor notices. Omit otherOnly for all currently eligible notices, with workflow ordering enforced.
- POST /generate: {loanCaseId, recipientCustomerId, noticeType, asAt, basisHash}. Revalidates eligibility and snapshot basis; reuses existing duplicates.
- GET /?pageIndex=0&pageSize=20: saved history, newest first.
- GET /{id}: saved message, arrears and audit metadata.
- POST /{id}/approve: Draft only; independent approver; response deadline must not have expired.
- POST /{id}/send: {destination, retry:false}. Email/SMS only. Uses the saved channel and message, revalidates current eligibility, and validates the current customer contact against the confirmed destination. Inserts a Pending EmailAlert/TextAlert and saves MessageAlertId plus Queued status in one transaction. Existing Queued/Sent records are idempotent. For DeliveryFailed, an explicit retry:true creates a new alert and archives the previous attempt. Returns the notice; no queue insertion is claimed as delivery.
- POST /{id}/record-sent: {dispatchReference}, required trimmed text of 1–500 characters. Print channel, Ready or Approved only; rechecks current eligibility, current stage and active notice identity. Deadline must be later than today. Records Sent status, sentBy, sentAtUtc and dispatchReference in one serializable transaction. Repeating for an already Sent notice is idempotent. This records external dispatch and does not call email/SMS gateways.
- POST /{id}/cancel: Draft, Ready or Approved only. Sent and Cancelled notices cannot be cancelled.
- GET /{id}/print: escaped UTF-8 HTML attachment with draft/cancelled/dispatch-recorded labels. Downloads never record dispatch.

Success: {success:true,data:...}. Standard API errors: invalid request 400, self-approval 403, missing notice 404, stale/conflicting state 409, missing schema migration 503.

## Progression

FirstNotice → SecondNotice → FinalDemand → Recovery. No missing stage is skipped. Each stage captures expectedRecipientIds in its immutable snapshot. All those recipients must have Sent records before the loan advances. Sent recipients disappear immediately from their old queue. Drafts and approvals do not advance it. Existing unsent historical stages cannot bypass the new sequence. Reminders and guarantor notice types remain separate.

Queue items add workflowStage, canGenerate, blockingReason and eligibleAfter (the previous response deadline). Recovery uses canGenerate to indicate eligibility for review only; it cannot generate a notice or execute recovery. Loans enter the next tab immediately but remain blocked through the previous response deadline and until company days/arrears thresholds are met. Disabled or missing stage settings produce waiting rows. Unresolved ageing produces review messages; zero arrears excludes a loan.

Stage placement uses current dispatch history. As-at controls ageing and response-period comparison, not historical reconstruction of policy or recipient membership. GET /{id} includes sendDestination for sendable electronic notices, plus messageAlertId, deliveryDestination, deliveryStatus, queuedBy, queuedAtUtc and deliveryAttemptsJson. DeliveryFailed permits explicit retry after review. Queued and failed electronic records remain active duplicate blockers. The first notice snapshot freezes the required recipients for a stage. Missing captured recipients require review. A missing guarantor blocks generation for a main stage that requires guarantors.

The saved message, arrears, policy revision and response deadline remain immutable. The deadline starts at draft preparation, not historical as-at or dispatch. Dispatch metadata records the time the operator confirms external dispatch. Nullable audit columns preserve existing data without assuming existing notices were sent.

Duplicate protection remains loan + recipient + type + as-at, with active records reused across dates. Cancelled same-date records remain idempotent; prepare replacements for a later date. Serializable operations and Status concurrency tokens prevent conflicting writes.

## Rollout and verification

Build the backend in Debug, run the normal Utility automatic migration to add SentBy (256), SentAtUtc (nullable timestamp) and DispatchReference (500) and StageRecipientIdsJson (nullable text), then restart the API. Do not substitute manual schema scripts. Builds alone do not migrate a database.

LoanNoticeTests covers approval, dispatch auditing/idempotency, partial recipient dispatch, each exclusive transition, threshold and response-period waiting, recovery expiry, settled/unresolved loans, paging and sent cancellation rejection. Tests send no real messages.

Older drafts without captured recipients preserve their original snapshot; dispatch captures the required recipient IDs in StageRecipientIdsJson. This prevents partial dispatch from skipping other required recipients.

## Electronic delivery integration

The existing Email/Text dispatcher queueing jobs discover Pending records after the notice transaction commits. Their existing UpdateEmailAlert/UpdateTextAlert AppService operations now update the linked notice in the same transaction as the provider result. SMTP Delivered maps to Sent with Accepted by mail server; SMS Submitted maps to Sent with Accepted by SMS provider; an actual SMS Delivered result is labelled Delivered. Failed submissions do not advance the loan. Old-attempt results cannot overwrite a newer retry because association uses the current message ID. A later handset-delivery failure remains visible without erasing the earlier submission audit.

Migrate nullable MessageAlertId, DeliveryDestination, DeliveryStatus, QueuedBy, QueuedAtUtc and DeliveryAttemptsJson fields, then deploy the updated shared assemblies to both API and workers. Preserve existing provider credentials and application-domain settings. The Messaging service must run its queueing jobs and receivers. At-least-once provider delivery remains possible if a provider accepts a message but its database acknowledgement fails; operator retries require checking the previous attempt.
