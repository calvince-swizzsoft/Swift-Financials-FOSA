# Customer Account Verification — Client Integration Spec

Audience: back-office "checker" screens that approve or reject newly created
savings accounts before they're usable (e.g. before a cash deposit is
allowed on them).

**This is not a new controller.** Customer account verification runs
through the generic maker-checker workflow engine already exposed at
`WebApplication1/Areas/Workflows/Controllers/WorkflowController.cs`
(`api/administration/workflows`) — the only thing that changed is that the
engine now actually *does something* for this record type. If you've
already built anything against that controller for another approval flow,
this is the same API with a different `systemPermissionType` value.

Source of truth:
- Origination: `Application.MainBoundedContext/AccountsModule/Services/CustomerAccountAppService.cs`
  (`FinalizeSavingsAccountRecordStatus`, `OriginateCustomerAccountVerificationWorkflowIfRequired`)
- Approval execution: `Application.MainBoundedContext/AdministrationModule/Services/WorkflowProcessorAppService.cs`
  (`case SystemPermissionType.CustomerAccountVerification`)
- Generic workflow API: `docs/api` has no existing spec for `WorkflowController`
  itself yet — see `WebApplication1/Areas/Workflows/Controllers/WorkflowController.cs`
  directly for the full generic surface (list, get, queueable, items,
  approve, settings). This doc only covers how to use it for this specific
  record type.

## 1. When a savings account needs verification at all

Controlled per-company by `Company.EnforceCustomerAccountMakerChecker`
(existing flag, previously only consulted by account freeze/reactivate —
now also consulted at account creation):

- **Flag off** (this is the default for most companies today): a new
  savings account is auto-approved immediately —
  `RecordStatus = Approved`, no workflow created, cash deposits work right
  away. Loan/Investment accounts always behave this way regardless of the
  flag.
- **Flag on**: a new savings account is created with `RecordStatus = New`
  and a `Workflow` is originated for it
  (`SystemPermissionType.CustomerAccountVerification = 44857`, decimal —
  `0xAFC0 + 65`). It stays `New` — and cash deposits keep failing with
  `"Sorry, account is not approved yet"` — until every required approval
  step on that workflow is completed.

**Prerequisite**: if the flag is on but no role has been mapped to
`CustomerAccountVerification` (via the existing role-permission mapping in
`WebApplication1/Areas/Admin/Controllers/RolesController.cs`), the workflow
is never created at all — the account just stays `New` forever, same as
before this change. Map at least one role to that permission type first.

## 2. Checker inbox

```
GET /api/administration/workflows/items?systemPermissionType=44857&status=0&pageIndex=1&pageSize=20&text=&startDate=...&endDate=...
```

`status=0` = `WorkflowRecordStatus.Pending` — the items awaiting this
checker's action. The API filters by the caller's roles server-side (see
`WorkflowController.GetItems` → `IWorkflowAppService.FindWorkflowItems`) —
you only see items assigned to a role you actually hold.

**Building a unified "my approvals" inbox that spans every permission
type** (not just this one)? Don't loop this endpoint over every
`systemPermissionType` value client-side — use
`GET /api/administration/workflows/items/mine` instead (same query params,
minus `systemPermissionType`). It resolves scope purely from the caller's
roles server-side and returns items across every permission type those
roles can act on in one call.

Each returned `WorkflowItemDTO.WorkflowRecordId` is the `CustomerAccountId`
of the pending account — cross-reference with
`GET /api/accounts/customer-accounts/{id}` (or
`GET /api/accounts/statements/customer-account/{id}`, etc.) to show the
checker what they're approving.

## 3. Approve or reject

```
POST /api/administration/workflows/items/approve
Content-Type: application/json

{
  "workflowItem": {
    "id": "<workflowItemId — from the inbox item, NOT the customerAccountId>",
    "status": 2,
    "remarks": "string"
  },
  "usedBiometrics": false
}
```

`status`: `2` = Approve, `1` = Reject (`WorkflowApprovalOption` — note this
is a *different* enum from the `RecordAuthOption` used internally by
`AuthorizeCustomerAccount`; the API boundary here is `WorkflowApprovalOption`
values only).

Server-side checks you'll hit as `400`/`403`-equivalent `InvalidOperationException`
messages (all pre-existing, not new):
- The same user can't be both the item's creator and its approver
  ("Maker-checker failure...").
- The item must not already be locked (not yet unlocked in the approval
  chain — relevant if `CustomerAccountVerification` has more than one role
  mapped with different `ApprovalPriority`).
- The caller must hold the role the item is assigned to.

On success, this only *marks the workflow item approved* synchronously —
the actual `CustomerAccount.RecordStatus` flip happens **asynchronously**,
once `SwiftFinancials.WorkflowProcessorDispatcher` (a separate Windows
Service polling a message queue) picks up the queued job and calls
`WorkflowProcessorAppService.ProcessWorkflowQueueAsync`. There is no
webhook/callback — poll `GET /api/accounts/customer-accounts/{id}` and
check `recordStatus` to confirm it landed. **If that dispatcher service
isn't running, approvals will appear to succeed but the account will never
actually unlock** — this is infrastructure to confirm is deployed/running,
not something the API can detect or report on.

Also note: a `Workflow` only ever gets queued for the dispatcher at all once
every required approval step is done (`Workflow.Status` reaches
`Approved`/`Rejected`) — check `GET /by-record?recordId=...&systemPermissionType=44857`
first if nothing seems to be happening; a `Workflow` still sitting at
`Pending` with `currentApprovals < requiredApprovals` means the chain isn't
finished yet, not that anything is broken.

**Recovery for a workflow stuck at `matchedStatus: 0` (NotMatched)** despite
already being `Approved`/`Rejected` (dispatcher not running, queue message
lost, etc.): `POST /api/administration/workflows/{workflowId}/match` — an
admin-only manual trigger that runs the exact same processing the
dispatcher would have (`IWorkflowProcessorAppService.ProcessWorkflowQueueAsync`)
synchronously, bypassing the queue entirely. `404` if the workflow id
doesn't resolve; `400` if it hasn't reached a final Approved/Rejected status
yet; returns `{ success: true, message: "Workflow was already matched..." }`
harmlessly if it's already matched. Works for any permission type on the
generic engine, not just this one.

The same-user maker-checker guard is enforced. If the initiator or latest
approver attempts the next sequential approval, the API returns HTTP `409`
with error code `MAKER_CHECKER_VIOLATION` and a safe message directing the
user to have a different authorized user complete the step.

## 4. `RecordStatus` reference

| Value | Meaning |
|---|---|
| `0` | New — unverified, blocks cash deposits |
| `1` | Edited |
| `2` | Approved — unblocks cash deposits |
| `3` | Rejected |
