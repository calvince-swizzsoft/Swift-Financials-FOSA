# Customer Editing and Verification

## Purpose

The customer-editing feature allows authorized registry users to amend a customer while preserving the company's maker-checker policy. The browser cannot authorize a customer or directly change `RecordStatus`.

## Permissions

- `CustomerEditing` is the maker permission. A user must belong to a role mapped to this permission before the API accepts an edit. The frontend checks `GET /api/registry/customer/edit-access` and only displays the edit action when access is granted.
- `CustomerEditVerification` is the checker permission. Map one or more approver roles to it and configure their approval priorities and required approver counts.

Do not assign both permissions to the same operational role when separation of duties is required.

## API flow

1. The UI loads the current record from `GET /api/registry/customer/{id}`.
2. It submits the complete amended DTO to `PUT /api/registry/customer/{id}`.
3. `CustomerController` checks `CustomerEditing`. IDs must match and the customer must exist.
4. `CustomerAppService.SubmitCustomerEditAsync` resolves the company from the persisted customer's branch. A client-supplied branch cannot be used to bypass company policy.
5. The application always preserves the persisted `RecordStatus`.

### Maker-checker disabled

The application validates duplicate identity, registration and payroll values, applies the edit immediately, and emits the existing customer-details alert when the mobile number changes.

### Maker-checker enabled

The proposed DTO is serialized into the workflow's nullable `Payload` column. The live customer remains unchanged. A workflow using `CustomerEditVerification` is raised for the configured checker roles. A second edit cannot be submitted while an unmatched edit workflow is pending.

After the final approval, `WorkflowProcessorAppService` deserializes and applies the staged DTO, then marks the workflow matched. Rejection marks the workflow matched without applying the payload. Workflow payloads are excluded from API JSON responses.

## Database migration

`Workflow.Payload` is an optional, unbounded string mapped by Entity Framework. This solution has EF6 automatic migrations enabled, so starting/updating the database adds the nullable column automatically. Ensure the application identity has schema-change permission when applying the migration.

## Frontend behavior

The Registry Customers index uses the canonical `/api/registry/customer` endpoint. It requests 20 customers per page, passes searches to the server through `text` and `customerFilter=0`, and uses the paging metadata returned by the API. Successful immediate edits and staged edits both refresh the current page.

## Test checklist

1. Run automatic migrations and confirm the workflows table has a nullable `Payload` column.
2. Map `CustomerEditing` to a maker role and `CustomerEditVerification` to at least one prioritized checker role.
3. With maker-checker disabled, edit a customer and confirm the live record changes immediately.
4. With maker-checker enabled, edit a customer and confirm the live record does not change before approval.
5. Confirm the checker receives a workflow task and approval applies the proposed values.
6. Submit another edit and reject it; confirm the live customer remains unchanged.
7. Attempt a PUT without `CustomerEditing`; confirm the API returns HTTP 403.
8. Include a changed `RecordStatus` in a PUT and confirm the persisted status is unchanged.
