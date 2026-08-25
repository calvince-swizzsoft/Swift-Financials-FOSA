# API Error Handling Strategy

**Scope:** `WebApplication1` (ASP.NET Web API 2 on .NET Framework 4.7.2)

## Goal

Every API failure should:

- return the correct HTTP status;
- use the same JSON structure;
- include a stable error code that clients can act on;
- hide exception and infrastructure details;
- include a correlation ID for support; and
- be logged once at the appropriate severity.

## Error response

All standard API endpoints should return errors in this format:

```json
{
  "success": false,
  "message": "The batch must be audited before it can be authorized.",
  "error": {
    "code": "BATCH_NOT_AUDITED",
    "validationErrors": null
  },
  "correlationId": "f018f72e7a5d4c8ba404dc715e8d7d5a"
}
```

For validation failures:

```json
{
  "success": false,
  "message": "One or more fields are invalid.",
  "error": {
    "code": "VALIDATION_FAILED",
    "validationErrors": {
      "amount": ["Amount must be greater than zero."],
      "valueDate": ["Value date is required."]
    }
  },
  "correlationId": "f018f72e7a5d4c8ba404dc715e8d7d5a"
}
```

Rules:

- `message` is safe to show to a user.
- `error.code` is stable and intended for client logic.
- Clients must not parse `message` to determine what happened.
- Stack traces, exception types, SQL errors, paths, and credentials must never
  appear in the response.
- The correlation ID must also be returned in the `X-Correlation-ID` header.

Successful endpoints can retain the existing response:

```json
{
  "success": true,
  "message": "Batch authorized successfully.",
  "data": {}
}
```

## Error classes

| Situation | HTTP status | Default code | Example |
|---|---:|---|---|
| Invalid request or DTO | 400 | `VALIDATION_FAILED` | Required field is missing |
| Missing or expired login | 401 | `AUTHENTICATION_REQUIRED` | JWT is invalid |
| User lacks permission | 403 | `ACCESS_DENIED` | Teller cannot authorize batch |
| Resource does not exist | 404 | `RESOURCE_NOT_FOUND` | Loan case ID is unknown |
| Business/state conflict | 409 | Domain-specific code | `BATCH_NOT_AUDITED` |
| Duplicate/concurrent update | 409 | `RESOURCE_CONFLICT` | Record changed by another user |
| Upload is too large | 413 | `PAYLOAD_TOO_LARGE` | RDL exceeds limit |
| Too many requests | 429 | `RATE_LIMIT_EXCEEDED` | Login attempts exceeded |
| Required dependency unavailable | 503 | `DEPENDENCY_UNAVAILABLE` | Database or gateway unavailable |
| Unexpected software failure | 500 | `INTERNAL_ERROR` | Unhandled exception |

Use `400` for invalid input. Use `409` when the input is valid but cannot be
applied because of the resource's current state or a business rule.

Error codes use uppercase `SNAKE_CASE`. General errors use shared codes;
business errors use clear domain codes such as:

- `LOAN_CASE_INVALID_STATE`
- `BATCH_NOT_AUDITED`
- `INSUFFICIENT_AVAILABLE_BALANCE`
- `OPERATION_ALREADY_COMPLETED`

Once a code is used by a released client, its meaning must not change.

## Implementation approach

### 1. Add shared error models

Create shared models in `WebApplication1/Infrastructure/Errors`:

```text
ApiErrorResponse.cs
ApiError.cs
ApiException.cs
ErrorCodes.cs
```

`ApiException` should contain the HTTP status, stable error code, safe client
message, and optional field validation errors.

Known application failures can use a few subclasses:

```text
ValidationException       -> 400
AuthenticationException   -> 401
AccessDeniedException     -> 403
ResourceNotFoundException -> 404
BusinessConflictException -> 409
DependencyException       -> 503
```

Do not create a class for every business rule. For example, use
`BusinessConflictException` with the code `BATCH_NOT_AUDITED`.

### 2. Handle exceptions centrally

Register Web API 2 components in `WebApiConfig.Register`:

- a `DelegatingHandler` to create or accept a valid correlation ID;
- an `IExceptionHandler` to translate exceptions into the standard response;
  and
- an `IExceptionLogger` to record unexpected exceptions.

The existing MVC `HandleErrorAttribute` is not sufficient for Web API
controllers.

The global exception handler should behave as follows:

```text
Known ApiException
    -> use its status, code, and safe message

Unknown Exception
    -> log the exception
    -> return 500 / INTERNAL_ERROR
    -> return "An unexpected error occurred."
```

Do not return `InternalServerError(ex)` because it can produce a different
response format and expose implementation details.

### 3. Keep responsibilities clear

Controllers should validate input, call application services, return success
results, and avoid broad `catch (Exception)` blocks.

Application/domain services should enforce business rules, report known
failures with a typed result or classified exception, and remain independent
of HTTP response types.

The global API pipeline should convert failures to HTTP responses, attach the
correlation ID, and log unexpected failures once.

A failure must not be represented inconsistently through a boolean return,
`ErrorMessageResult`, and an exception. During migration, adapt these legacy
results at the controller boundary to the standard error response.

## Logging

Logging is part of centralized error handling, but not every error is an
error-level event.

| Failure | Log level |
|---|---|
| Expected validation or ordinary 404 | None or Information |
| Business conflict | Information |
| Access denied on a sensitive operation | Warning |
| Repeated authentication failures | Warning |
| Dependency failure | Error |
| Unexpected exception | Error |
| Data-integrity or system-wide failure | Critical |

Log structured fields such as the correlation ID, error code, HTTP status,
HTTP method, route template, authenticated user ID, permitted company/branch
IDs, request duration, and dependency name.

Log an exception once, at the API boundary where request context is available.
Do not log it in a service and then log it again in the controller.

Never log passwords, tokens, authorization headers, connection strings, full
request bodies, full account numbers, national IDs, uploaded documents, or
other sensitive customer and financial data.

## Financial operations

Posting, transfers, authorization, disbursement, and reversals must distinguish:

- operation rejected before processing;
- operation failed before submission;
- operation completed;
- operation already completed; and
- outcome unknown because a dependency timed out.

Do not tell a client to automatically retry a financial command when the
outcome is unknown. Such commands should use an operation reference or
idempotency key before automatic retries are introduced.

Business audit records and diagnostic logs are different. The audit trail
records who performed a financial action and what changed. Diagnostic logs
explain why the software failed. Both are required for money-moving operations.

## Special external contracts

The Channels canonical API can retain its published `{ success, error }`
format. It should adapt the internal classification to that format while using
correlation IDs, safe messages, and centralized logging. It must not return
`ex.Message` for unexpected exceptions.

## Migration plan

1. Add the shared response models and error codes.
2. Add correlation-ID handling.
3. Add the global exception handler and structured logger.
4. Add tests for the central pipeline.
5. Migrate one small controller as a proof of the pattern.
6. Migrate loan and batch workflows, defining their business error codes.
7. Migrate remaining controllers feature by feature.
8. Update API specifications and the frontend client as each area moves.

Do not rewrite every controller at once. The central handler immediately makes
unexpected exceptions safe; expected business errors can then be standardized
incrementally.

## Required tests

Automated tests should verify:

- 400 validation response with field errors;
- 401 authentication failure;
- 403 authorization failure;
- 404 missing resource;
- 409 business conflict;
- 503 dependency failure;
- safe 500 response for an unexpected exception;
- matching correlation ID in the header, body, and log;
- only one log entry for an unexpected exception; and
- no sensitive data or exception detail in responses or captured logs.

## Definition of done

The API has suitable error handling when:

- all endpoints use documented HTTP statuses and stable error codes;
- standard endpoints return the same error structure;
- all responses include a correlation ID;
- unexpected exceptions are handled and logged centrally;
- clients never receive raw exception details;
- known business conflicts are not returned as generic 500 errors;
- financial commands have safe retry/idempotency rules; and
- the frontend uses status and error code instead of parsing messages.
