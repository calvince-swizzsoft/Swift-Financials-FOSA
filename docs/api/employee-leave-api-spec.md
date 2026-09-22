# Employee leave API

Updated 22 September 2026. Existing routes return DTOs/page collections directly; these additions preserve that contract. Authentication is required. Expected validation errors return HTTP 400; controller permission failures return 403.

Base: `api/humanresource/leaveapplications`.

| Method / relative route | Contract |
| --- | --- |
| `GET preview` | Query: `employeeId`, `leaveTypeId`, `start`, `end` (ISO dates), optional `excludedId` for edit/approval review. Returns `LeavePreviewDTO`. Read access requires application, approval or recall permission. Preview is advisory; writes validate again. |
| `POST {id}/withdraw` | No body. Application permission and employee/submitter identity required. Pending only. Returns updated application with Status 16. |
| `POST {id}/retry-notification` | No body. Pending requires application permission; approved/rejected requires approval permission. Returns refreshed application; inspect NotificationPending for whether queuing remains outstanding. |
| `POST {id}/recall` | Body `{ "Remarks": "...", "EffectiveReturnDate": "2026-09-25" }`. Recall permission, Approved only. Return date must be today or later and within the original leave interval. Returns refreshed application. |

`LeavePreviewDTO`: `RequestedDays`, `CanSubmit`, `Error`, `Cycles`.
Each cycle contains `Start`, `End`, `Entitlement`, `Used`, `Reserved`, `Available`, `Requested`, `Remaining`. Dates are inclusive. Available is entitlement minus approved/taken days and pending reservations; remaining also deducts this request. Each non-accrued cycle must be sufficient. Accrued balance is cumulative completed service periods from EmploymentStartDate, with all reserved/taken days deducted. Its End is the .NET maximum-date sentinel, displayed as onward by the UI.

Application DTO additions: `ChargedDates` (server-owned comma-separated ISO dates), nullable `EffectiveReturnDate`, `NotificationPending`. Clients cannot set server-owned balances, status or snapshots through ordinary create/update. Status values: Pending 1, Approved 2, Rejected 4, Recalled 8, Withdrawn 16.

Create/update require application permission 22016. Decisions require approval 22017 and an actor different from both the submitting user and employee. Recall requires 22018. Writes serialize per employee. Pending requests reserve balance. Create refuses submission without another active employee-linked authorized approver. Saving succeeds even if notification queuing fails; show a retry action when NotificationPending is true.

`api/humanresource/leavetypes/capabilities` (GET) returns `{ "CanManage": true|false }`. Type create/update requires Leave Setup permission 22028; operational leave permissions may read types. Entitlement, cycle and accrued mode cannot change on a type already used by an application; create a new policy version/type. Weekend and holiday changes apply to newly calculated requests, not existing charged snapshots.

Employee create/update DTO accepts nullable ISO `EmploymentStartDate`. It is required to calculate accrued leave. Do not substitute record creation date. Current Annual Leave remains non-accrued with a yearly allowance of 30 days, excluding weekends/public holidays.

Migration prerequisite: `tools/sql/2026-09-22-leave-calendar.sql`. Backfill legacy snapshots before application use. Historic recalled records without an effective return date preserve prior full-cancellation treatment. Configure permission 22028 for authorized HR setup roles and populate Holiday records. No scheduled retry worker or delivery confirmation is implied by NotificationPending.

## Employee leave statistics

GET employee-statistics?employeeId={guid}&leaveTypeId={guid}&asAt=2026-09-22&pageIndex=0 requires an operational leave permission. It returns EmployeeLeaveStatisticsDTO: AsAt, Today, LeaveTypeDescription, IsAccrued, Balance (existing cycle DTO), BalanceError, TakenDays, UpcomingDays, HistoryCount and History. Balance uses current recorded statuses and reservations for the entitlement cycle containing asAt; it is not a historical audit snapshot. Accrued entitlement is calculated at asAt. Taken includes today; Upcoming is later than today. Recalled leave retains only charged dates before its effective return date.

History includes every status overlapping asAt's year, scoped to the selected employee/type, ordered by start date descending and paged at 10 rows. Each row has Id, Start, End, Status, ChargedDaysInYear, EffectiveReturnDate, Reason and AuthorizedBy. Pending days are reservations; rejected/withdrawn rows charge zero. Balance calculations use all matching records regardless of history page. Locked employees/types remain readable. Missing accrual commencement returns BalanceError while retaining history. This endpoint is read-only and does not send notifications. No schema migration is required.

