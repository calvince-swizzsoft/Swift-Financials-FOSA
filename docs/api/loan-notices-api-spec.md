# Defaulter notice workflow API

Base: `/api/backoffice/loan-notices`; authenticated API. Navigation code **70025**, under Back Office > Operations > Loaning > Defaulter Notices. The existing Utility navigation seed installs the item; role grants remain managed by existing navigation administration.

## Endpoints

- `GET /eligible?asAt=2026-09-16&pageIndex=0&pageSize=20`: complete per-loan ageing evaluated once, then matching notices paginated. Returns `total`, `items`, `reviewCount`, `disabledCount`, `issues`.
- `POST /generate`: `{loanCaseId, recipientCustomerId, noticeType, asAt, basisHash}` from an eligibility item. Recalculates eligibility and verifies the basis before saving. Returns the saved notice, or the existing duplicate.
- `GET /?pageIndex=0&pageSize=20`: saved history (newest first).
- `GET /{id}`: saved message, arrears, recipient, company, revision and audit metadata.
- `POST /{id}/approve`: only Draft; requires a different authenticated user from its creator. Expired response deadlines cannot be approved.
- `POST /{id}/cancel`: preserves the notice and records cancellation actor/time. Cancelled notices cannot be approved.
- `GET /{id}/print`: UTF-8 printable HTML attachment. Content is HTML-escaped; drafts and cancelled notices are labelled. Opening the file in a browser supports Print / Save as PDF. No download is recorded as delivery.

Success JSON uses `{success:true,data:...}`. Expected failures return the standard API error envelope: invalid inputs (400), maker self-approval (403), missing notice (404), stale basis/concurrent saves (409), missing migration (503). Unexpected errors use global error handling.

## Eligibility and persistence

Current company settings resolve through LoanCase.BranchId -> Branch.CompanyId. Each stage meeting its days and minimum arrears thresholds is offered for explicit user selection; there is no automatic escalation. Arrears are overdue principal plus overdue contractual interest. Unknown amounts/days or ageing issues exclude the loan, with a review reason. Zero arrears do not qualify. Dates cannot be in the future.

Borrowers come from the loan case. Guarantors are currently attached, case-linked guarantors (Status=Attached), deduplicated by customer. Released guarantors and unrelated product-level guarantees are excluded. Historical arrears dates do not reconstruct historical company settings or guarantor membership. Missing guarantors produce a review message; for Both, the valid borrower candidate is still available.

The immutable draft stores the full company policy, revision, recipient, rendered message and calculated arrears snapshot. The response deadline starts from the UTC draft preparation date, not the historical as-at date. This is a planned notice deadline, not evidence that delivery occurred. An outdated notice can be cancelled and prepared again for a later as-at date.

Draft -> Approved (independent user) when approval is required. Policies without approval create Ready notices. Draft/Ready/Approved can become Cancelled. Sending/delivery status is intentionally not implemented here; email/SMS notices are preview copies only. Approved/Ready print copies are printable, not proof of delivery.

A unique database key covers loan + recipient + notice type + as-at date, excluding policy revision/channel so changing either does not bypass duplicate protection. Existing active notices for the same loan/type/recipient are also reused across dates until cancelled. Cancellation preserves same-date idempotency; use a later as-at date for a replacement. Generation uses a serializable write scope and the unique index protects racing same-key inserts. The API asks the client to refresh after concurrent conflicts.

## Schema rollout and tests

`LoanNotice` is a Domain entity with standard EF configuration discovered by BoundedContextUnitOfWork. It maps to `swiftFin_LoanNotices`, with foreign keys to Company, LoanCase and recipient Customer, a unique duplicate key and concurrency token on Status. The normal **automatic migration** creates the table. No hand-written CREATE/DROP SQL or startup bootstrap is added.

Build/run the updated Utility migration project and restart the API. Refresh navigation after its seed; grant the new navigation item to the intended roles. Builds alone do not migrate a running database.

`LoanNoticeTests` covers threshold boundaries, unresolved ageing, recipient deduplication, stale snapshots, idempotency, active notices across dates, independent approval, cancellation, stored snapshot stability, rendering and HTML escaping. Existing loan-ageing/schedule/Form 4 tests also pass. No real notices or messages are sent by these tests.
