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

## 4. `RecordStatus` reference

| Value | Meaning |
|---|---|
| `0` | New — unverified, blocks cash deposits |
| `1` | Edited |
| `2` | Approved — unblocks cash deposits |
| `3` | Rejected |
