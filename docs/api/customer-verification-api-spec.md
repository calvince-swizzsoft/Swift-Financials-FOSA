# Customer Verification — Client Integration Spec

Audience: back-office "checker" screens that approve or reject newly
registered customers before their record is considered authoritative.

**This is not a new controller.** Customer verification runs through the
same generic maker-checker workflow engine as
[Customer Account Verification](customer-account-verification-api-spec.md),
exposed at `WebApplication1/Areas/Workflows/Controllers/WorkflowController.cs`
(`api/administration/workflows`) — only the `systemPermissionType` value
differs. If you've already integrated the checker inbox for customer
accounts, this is the same API surface.

Source of truth:
- Origination: `Application.MainBoundedContext/RegistryModule/Services/CustomerAppService.cs`
  (`FinalizeCustomerRecordStatus`, `OriginateCustomerVerificationWorkflowIfRequired`)
- Approval execution: `Application.MainBoundedContext/AdministrationModule/Services/WorkflowProcessorAppService.cs`
  (`case SystemPermissionType.CustomerVerification`)
- Generic workflow API: see `WebApplication1/Areas/Workflows/Controllers/WorkflowController.cs`
  directly for the full generic surface (list, get, queueable, items,
  approve, settings). This doc only covers how to use it for this specific
  record type.

## 1. When a new customer needs verification at all

Controlled per-company by `Company.EnforceCustomerMakerChecker` (a new
flag, independent of `Company.EnforceCustomerAccountMakerChecker` — a
company can require maker-checker on one, both, or neither):

- **Flag off** (the default): a new customer is auto-approved immediately —
  `RecordStatus = Approved`, no workflow created.
- **Flag on**: a new customer is created with `RecordStatus = New` and a
  `Workflow` is originated for it
  (`SystemPermissionType.CustomerVerification = 44858`, decimal —
  `0xAFC0 + 66`). It stays `New` until every required approval step on that
  workflow is completed.

**Prerequisite**: if the flag is on but no role has been mapped to
`CustomerVerification` (via the existing role-permission mapping in
`WebApplication1/Areas/Admin/Controllers/RolesController.cs`), the workflow
is never created at all — the customer just stays `New` forever. Map at
least one role to that permission type first.

Note this is entirely separate from `CustomerAccountVerification`
(`44857`) — approving a customer does not approve their savings accounts,
and vice versa. `RecordStatus` is tracked independently on `Customer` and
on each `CustomerAccount`.

## 2. Checker inbox

```
GET /api/administration/workflows/items?systemPermissionType=44858&status=0&pageIndex=1&pageSize=20&text=&startDate=...&endDate=...
```

`status=0` = `WorkflowRecordStatus.Pending` — the items awaiting this
checker's action. The API filters by the caller's roles server-side (see
`WorkflowController.GetItems` → `IWorkflowAppService.FindWorkflowItems`) —
you only see items assigned to a role you actually hold.

Each returned `WorkflowItemDTO.WorkflowRecordId` is the `CustomerId` of the
pending customer — cross-reference with
`GET /api/registry/customer/{id}` to show the checker what they're
approving.

## 3. Approve or reject

```
POST /api/administration/workflows/items/approve
Content-Type: application/json

{
  "workflowItem": {
    "id": "<workflowItemId — from the inbox item, NOT the customerId>",
    "status": 2,
    "remarks": "string"
  },
  "usedBiometrics": false
}
```

`status`: `2` = Approve, `1` = Reject (`WorkflowApprovalOption` — a
*different* enum from the `RecordAuthOption` used internally by
`AuthorizeCustomer`; the API boundary here is `WorkflowApprovalOption`
values only).

Server-side checks you'll hit as `400`/`403`-equivalent `InvalidOperationException`
messages (all pre-existing, not new):
- The same user can't be both the item's creator and its approver
  ("Maker-checker failure...").
- The item must not already be locked (not yet unlocked in the approval
  chain — relevant if `CustomerVerification` has more than one role mapped
  with different `ApprovalPriority`).
- The caller must hold the role the item is assigned to.

On success, this only *marks the workflow item approved* synchronously —
the actual `Customer.RecordStatus` flip happens **asynchronously**, once
`SwiftFinancials.WorkflowProcessorDispatcher` (a separate Windows Service
polling a message queue) picks up the queued job and calls
`WorkflowProcessorAppService.ProcessWorkflowQueueAsync`. There is no
webhook/callback — poll `GET /api/registry/customer/{id}` and check
`recordStatus` to confirm it landed. **If that dispatcher service isn't
running, approvals will appear to succeed but the customer will never
actually unlock** — this is infrastructure to confirm is deployed/running,
not something the API can detect or report on.

## 4. `RecordStatus` reference

| Value | Meaning |
|---|---|
| `0` | New — unverified |
| `1` | Edited |
| `2` | Approved |
| `3` | Rejected |
