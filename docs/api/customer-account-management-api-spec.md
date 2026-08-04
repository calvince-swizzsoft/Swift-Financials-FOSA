# Customer Account Management API — Client Integration Spec

Audience: screens that change a customer account's lifecycle state —
activation, freezing, closure — plus attaching a plain remark, recording
signing-instruction changes, and viewing the resulting audit history.

Source of truth for everything below:
- Controller: `WebApplication1/Areas/Accounts/Controllers/CustomerAccountManagementController.cs`
- Domain service it calls: `Application.MainBoundedContext/AccountsModule/Services/ICustomerAccountAppService.cs`
  (`ManageCustomerAccount`, `FindCustomerAccountHistory`)
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

For adding/removing account signatories (a separate aggregate from account
*management*), see `docs/api/customer-account-signatory-api-spec.md`.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/customer-accounts` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | Bearer JWT on every request |

Shares its base path with the existing `CustomerAccountsController`
(`GET /`, `GET /{id}`, etc.) — these are additional sub-routes on the same
resource, not a competing controller.

## 2. Response envelope

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success, or a caught business error (`success: false`).
- `201 Created` — n/a here (all management actions are `200`, including the
  ones that change state — there's no "created" resource).
- `400 Bad Request` — missing request body.
- `404 Not Found` — `customerAccountId` doesn't resolve to an account.
- `409 Conflict` — a business-rule guard rejected the action (currently only
  `/activate` — see §4.1); `message` explains why.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

On the five action endpoints, `data` is always `null` and `success` mirrors
whatever `ManageCustomerAccount` returned — check `success`, not just the
`200` status, to know whether the action actually took effect.

## 3. How this maps to the domain

All five action endpoints are thin wrappers around one underlying method,
`ICustomerAccountAppService.ManageCustomerAccount(customerAccountId, managementAction, remarks, remarkType, serviceHeader)`
— the controller picks the `CustomerAccountManagementAction` code for you,
so the frontend never needs to know the raw enum values:

| Endpoint | `CustomerAccountManagementAction` | Notes |
|---|---|---|
| `POST /{id}/activate` | Activation | |
| `POST /{id}/freeze` | Deactivation | Also triggers frozen-account alerts (`IBrokerService.ProcessFrozenAccountAlerts`) on success — expect the member to be notified. |
| `POST /{id}/close` | Closure | |
| `POST /{id}/remark` | Remark | No state change — just appends a note to the account's history. This **is** the "account remark" feature; there's no separate remarks CRUD. |
| `POST /{id}/signing-instructions` | SigningInstructions | Distinct from the signatory list (see the signatory spec) — this logs a change to *how* the account should be signed against (e.g. mandate rules), not a signatory record itself. |

Every action requires the same body:

```ts
interface ManageCustomerAccountRequest {
  remarks: string;
  remarkType: number;  // CustomerAccountRemarkType enum
}
```

`CustomerAccountRemarkType`: `0` Actionable, `1` Informational.

## 4. Endpoints

All routes below are relative to `/api/accounts/customer-accounts`.

### 4.1 Activate — `POST /{customerAccountId}/activate`

**This is an "unfreeze," not a first-time activation.** New accounts are
created already `Active`/`Normal` — there's nothing to activate until an
account has at least one prior Activation/Deactivation/Remark history entry.
Calling this on an account that's never been frozen (or remarked, or
activated) returns `409 Conflict` with `"Sorry, but account freezing history
is missing!"`. If it's already active (last relevant history entry is
Activation), it returns `success: true` as a no-op — calling `/activate`
twice in a row is safe, calling it on a virgin account is not. The correct
sequence to test/demo this is freeze (§4.2) then activate.

### 4.2 Freeze — `POST /{customerAccountId}/freeze`
### 4.3 Close — `POST /{customerAccountId}/close`
### 4.4 Remark — `POST /{customerAccountId}/remark`
### 4.5 Signing instructions — `POST /{customerAccountId}/signing-instructions`

All five take the `ManageCustomerAccountRequest` body from §3 and return:
```json
{ "success": true, "message": "Customer account <activated|frozen|closed|...> successfully", "data": null }
```
`404` if `customerAccountId` doesn't exist.

### 4.6 History — `GET /{customerAccountId}/history`

Query: `managementAction` (optional int, `CustomerAccountManagementAction`
enum — omit for full history, supply to filter to one action type). `404`
if the account doesn't exist. Returns `ApiEnvelope<CustomerAccountHistoryDTO[]>`:

```ts
interface CustomerAccountHistoryDTO {
  id: string;
  customerAccountId: string;
  managementAction: number;   // CustomerAccountManagementAction enum
  remarks: string;
  reference: string;
  createdBy: string;
  createdDate: string;
}
```

Unpaged — this is a per-account audit trail, not expected to grow large
enough to need paging in normal use. `CustomerAccountManagementAction` raw
values, for reference (only needed if you're filtering `GET /history` by
action — the five POST endpoints above hide these from you):

| Action | Value |
|---|---|
| Activation | `48833` |
| Deactivation | `48834` |
| Remark | `48835` |
| Closure | `48838` |
| SigningInstructions | `48839` |
