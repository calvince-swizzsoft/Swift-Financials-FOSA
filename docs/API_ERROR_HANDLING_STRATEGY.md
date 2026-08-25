# API Error Handling and Logging Strategy

**Status:** Proposed  
**Scope:** `SwiftFinancialz2/WebApplication1` ASP.NET Web API 2 API  
**Consumers:** `Swizzfinancial-FOSA2` and other API clients  
**Compatibility target:** .NET Framework 4.7.2, ASP.NET Web API 2

## 1. Purpose

This document defines the error contract and operational handling required for
the API. Its goal is to make every failure:

- predictable for clients;
- safe to expose outside the server;
- diagnosable through a correlation ID and structured server logs;
- mapped to the correct HTTP status;
- testable independently of controller implementation details; and
- incrementally adoptable without rewriting all existing controllers at once.

This is the canonical strategy for new endpoints. Existing endpoints should be
migrated by feature area according to the rollout plan in section 16.

## 2. Design principles

1. **HTTP status describes the broad outcome.** A stable application error code
   describes the precise failure.
2. **Expected failures are not exceptions.** Validation, missing resources,
   access denial, and known business-rule conflicts should be returned or
   raised as classified application errors.
3. **Unexpected exceptions are handled once.** Controllers must not repeatedly
   catch `Exception` merely to return `InternalServerError(ex)`.
4. **Clients never receive exception objects, stack traces, SQL messages, file
   paths, or internal implementation details.**
5. **Every error response has a correlation ID.** The same ID appears in server
   logs and the `X-Correlation-ID` response header.
6. **Logging is structured and proportional.** Expected client mistakes do not
   generate error-level noise; unexpected and dependency failures do.
7. **Sensitive financial and identity data is denied by default in logs.**
8. **The response envelope is consistent across all standard API endpoints.**
   Explicitly versioned external contracts may keep their own documented shape
   but must use the same internal classification, correlation, and logging
   pipeline.

## 3. Standard response contract

### 3.1 Successful response

Existing successful responses remain compatible:

```json
{
  "success": true,
  "message": "Loan case created successfully.",
  "data": {
    "id": "4a2fffa4-466f-4d45-b82a-726a04d5fb29"
  }
}
```

### 3.2 Error response

All standard API errors use this envelope:

```json
{
  "success": false,
  "message": "The request could not be completed.",
  "error": {
    "code": "LOAN_CASE_INVALID_STATE",
    "details": null,
    "validationErrors": null
  },
  "correlationId": "01J6YB8C4K4FJ8J4Y3W8N6A2QX"
}
```

Fields:

| Field | Required | Meaning |
|---|---:|---|
| `success` | Yes | Always `false` for an error. |
| `message` | Yes | Safe, user-facing summary. Must not contain diagnostics. |
| `error.code` | Yes | Stable, documented, machine-readable code. |
| `error.details` | No | Safe contextual values needed by the client. Never diagnostic internals. |
| `error.validationErrors` | No | Field-level validation failures. |
| `correlationId` | Yes | Identifier used to find the corresponding server logs. |

`data` is omitted from errors. During migration, accepting `data: null` is
permitted for backward compatibility, but new endpoints must not rely on it.

### 3.3 Validation response

```json
{
  "success": false,
  "message": "One or more fields are invalid.",
  "error": {
    "code": "VALIDATION_FAILED",
    "details": null,
    "validationErrors": {
      "amount": ["Amount must be greater than zero."],
      "valueDate": ["Value date cannot be before the current posting period."]
    }
  },
  "correlationId": "01J6YB8C4K4FJ8J4Y3W8N6A2QX"
}
```

Field keys use the API's JSON property naming convention. Multiple messages
may be returned for one field. Cross-field errors use the key `request`.

### 3.4 Headers

Every response must include:

```http
X-Correlation-ID: 01J6YB8C4K4FJ8J4Y3W8N6A2QX
```

Retryable responses may additionally include:

```http
Retry-After: 30
```

The API accepts a caller-provided `X-Correlation-ID` only when it matches the
allowed format and length. Otherwise it creates a new ID. The value is a
diagnostic identifier, not a security credential.

## 4. Error taxonomy and HTTP mapping

| Class | HTTP | Default code | Client action | Server log level |
|---|---:|---|---|---|
| Malformed request | 400 | `INVALID_REQUEST` | Correct the request. | Information |
| Validation failure | 400 | `VALIDATION_FAILED` | Display field errors. | Normally none/Information |
| Authentication missing/invalid | 401 | `AUTHENTICATION_REQUIRED` | Re-authenticate once. | Information; Warning for suspicious repetition |
| Authenticated but forbidden | 403 | `ACCESS_DENIED` | Do not retry unchanged. | Warning for sensitive operations; otherwise Information |
| Resource missing | 404 | `RESOURCE_NOT_FOUND` | Refresh/navigate away. | Normally none/Information |
| Method/content negotiation error | 405/406/415 | framework-specific stable code | Correct client integration. | Information |
| Business rule/state conflict | 409 | domain-specific code | Refresh data or correct workflow. | Information/Warning |
| Duplicate/concurrency conflict | 409 | `RESOURCE_CONFLICT` or `CONCURRENCY_CONFLICT` | Refresh and retry deliberately. | Warning |
| Payload too large | 413 | `PAYLOAD_TOO_LARGE` | Reduce upload size. | Information |
| Unsupported business value | 422 | `UNPROCESSABLE_REQUEST` | Correct a semantically invalid value. | Information |
| Rate limited | 429 | `RATE_LIMIT_EXCEEDED` | Retry after delay. | Warning/metric |
| Dependency timeout | 503 | `DEPENDENCY_TIMEOUT` | Retry according to policy. | Error |
| Dependency unavailable | 503 | `DEPENDENCY_UNAVAILABLE` | Retry later. | Error |
| Unexpected server failure | 500 | `INTERNAL_ERROR` | Report correlation ID; retry cautiously. | Error/Critical |

### 4.1 Choosing 400, 409, and 422

- Use **400** when the request is structurally invalid or fails DTO validation.
- Use **409** when the request is valid but conflicts with current persisted
  state, such as authorizing a batch that has not been audited.
- Use **422** when the request is structurally valid but contains a semantic
  value that cannot be processed and the problem does not depend on current
  resource state. Existing clients may initially treat 422 like 400.

### 4.2 Error-code naming

Codes are uppercase `SNAKE_CASE` and never change meaning. Use:

- shared codes for generic conditions, such as `VALIDATION_FAILED`;
- domain codes for actionable business failures, such as
  `BATCH_NOT_AUDITED`, `LOAN_CASE_INVALID_STATE`, or
  `INSUFFICIENT_AVAILABLE_BALANCE`.

Do not encode HTTP status numbers, exception type names, UI wording, or route
names in a code. Once released, a code is part of the API contract.

An error-code catalogue must be maintained in the API documentation with:
code, HTTP status, meaning, applicable endpoints, and safe client guidance.

## 5. Exception hierarchy

Introduce a small API/application exception hierarchy for failures that must
cross layers:

```text
ApiException
|-- RequestValidationException          -> 400
|-- AuthenticationRequiredException     -> 401
|-- AccessDeniedException               -> 403
|-- ResourceNotFoundException           -> 404
|-- BusinessConflictException           -> 409
|-- ConcurrencyConflictException        -> 409
|-- UnprocessableRequestException       -> 422
|-- DependencyTimeoutException          -> 503
`-- DependencyUnavailableException      -> 503
```

The base exception carries:

- stable `ErrorCode`;
- safe client `Message`;
- optional safe `Details`;
- optional field validation errors;
- HTTP status mapping; and
- optional underlying exception for server logging only.

Do not introduce a separate exception subclass for every individual business
rule. Use `BusinessConflictException` with a stable domain error code.

Domain and application services may alternatively return a typed result for
expected, high-frequency business outcomes. The boundary adapter converts the
result to the same error envelope. A method must not signal the same kind of
failure through booleans, `ErrorMessageResult`, and exceptions simultaneously.

## 6. Responsibilities by layer

### Domain/application services

- Enforce business invariants and authorization rules that depend on domain
  state.
- Return typed failures or throw a classified application exception.
- Never depend on HTTP concepts such as `IHttpActionResult`.
- Preserve the underlying exception as `InnerException` when translating a
  dependency failure.
- Do not format user-interface messages or write routine controller logs.

### Controllers

- Validate route/body presence and invoke model/DTO validation.
- Return success responses and explicit expected errors when the condition is
  immediately known.
- Do not catch `Exception` solely to call `InternalServerError(ex)`.
- Catch only when the controller can recover, add meaningful classification,
  or perform necessary cleanup that cannot be expressed with `using`/`finally`.
- Never inspect exception-message text to determine an HTTP status.

### Global API pipeline

- Establish and return the correlation ID.
- Convert classified exceptions into the standard envelope.
- Convert unclassified exceptions into safe `500 INTERNAL_ERROR` responses.
- Log once at the boundary with request and execution context.
- Normalize framework-generated errors where Web API allows it, including
  model binding, unsupported media type, and route/method failures.

## 7. Web API 2 implementation architecture

Implement the following shared components in `WebApplication1`:

```text
Infrastructure/Errors/
|-- ApiError.cs
|-- ApiErrorResponse.cs
|-- ApiException.cs
|-- ErrorCodes.cs
|-- ApiExceptionHandler.cs
|-- ApiExceptionLogger.cs
|-- CorrelationIdHandler.cs
`-- ModelStateExtensions.cs
```

Because this is ASP.NET Web API 2, register:

- a `DelegatingHandler` for correlation IDs;
- one `IExceptionHandler` for the final exception-to-response mapping;
- one `IExceptionLogger` for exceptions observed by Web API; and
- a global action filter only for response/model-state normalization that
  cannot be handled more appropriately by the components above.

Register these through `WebApiConfig.Register`. The MVC
`HandleErrorAttribute` in `FilterConfig` is not the API exception strategy and
must not be relied on for API controllers.

The exception handler and exception logger must cooperate to avoid duplicate
logs. Use a request property flag or a shared logging policy so the same
failure is recorded once.

## 8. Correlation and request context

For every request, capture these structured properties when available:

- `CorrelationId`
- `RequestMethod`
- normalized `RouteTemplate` rather than a raw URL
- `ResponseStatusCode`
- `ErrorCode`
- `DurationMs`
- authenticated `UserId` or username
- `TenantId`, company, and branch identifiers where applicable
- deployment environment and application version
- dependency name and duration for outbound calls

Do not place correlation state in a static mutable variable. Store it in the
request context and logging scope so asynchronous execution remains correct.

Return the correlation ID on success as well as failure. This lets support
trace a request that produced an incorrect but technically successful result.

## 9. Logging policy

### 9.1 Structured events

Use structured properties rather than concatenated strings. Conceptually:

```text
API request failed
CorrelationId={CorrelationId}
ErrorCode={ErrorCode}
StatusCode={StatusCode}
RouteTemplate={RouteTemplate}
UserId={UserId}
DurationMs={DurationMs}
```

The implementation should use the already-referenced logging abstraction so
the storage provider can be configured independently. Production needs a
durable provider/sink; debugger output or local text alone is insufficient.

### 9.2 Severity rules

- **Trace/Debug:** local diagnostic detail; disabled or sampled in production.
- **Information:** expected lifecycle and client-caused outcomes where an audit
  trail is useful.
- **Warning:** suspicious authentication activity, forbidden sensitive actions,
  concurrency conflicts, repeated failures, and degraded recoverable behavior.
- **Error:** unexpected exceptions and failed dependencies.
- **Critical:** process-level failure, widespread data-integrity risk, or an
  unavailable critical subsystem requiring immediate intervention.

Do not log every 400 or 404 at Error level. Track their volumes using metrics
or sampled informational events.

### 9.3 Log once

An exception is logged at the outermost boundary that has complete request
context. Inner layers may add context by wrapping/rethrowing, but must not log
and rethrow the same error unless they own a separate operational event.

Never use `throw ex;`; use `throw;` to preserve the original stack trace.

## 10. Data protection and redaction

Never log:

- passwords, PINs, OTPs, tokens, cookies, or `Authorization` headers;
- full request/response bodies by default;
- full bank/customer account numbers, card data, or payment credentials;
- national identity numbers, tax identifiers, phone numbers, or email
  addresses unless explicitly masked and approved;
- uploaded document contents;
- connection strings or database credentials; or
- raw SQL parameter values containing customer or financial data.

Use identifiers needed for diagnosis, preferably internal GUIDs. If a business
identifier is essential, mask it consistently—for example, retain only the
last four characters. Redaction must happen before an event reaches the log
provider.

Exception messages may contain sensitive values. Store the exception object
only in restricted server logs and never copy `ex.Message` into a client
response for unexpected failures.

Define access control, retention, deletion, and export rules for production
logs with the organization responsible for compliance. Logging implementation
is not complete until these operational rules exist.

## 11. Authentication and authorization failures

- Missing, invalid, or expired credentials return `401` with
  `AUTHENTICATION_REQUIRED`.
- Valid credentials without permission return `403` with `ACCESS_DENIED`.
- Do not disclose whether a protected resource exists when doing so would leak
  information; return the security-approved response consistently.
- Authentication logs must not contain the token or supplied password.
- Repeated failures should feed a metric/security alert and any rate-limiting
  controls.
- The API response must not instruct the frontend to redirect. The frontend
  decides its navigation behavior from the status/code.

## 12. Dependency and transient failures

Database, message broker, file storage, SSRS, and external gateway failures
must be translated deliberately:

- timeout or temporary unavailability: `503 DEPENDENCY_TIMEOUT` or
  `DEPENDENCY_UNAVAILABLE`;
- invalid response from a dependency: `502 DEPENDENCY_INVALID_RESPONSE` when
  the API is acting as a gateway, otherwise a classified `503`;
- permanent business rejection from a dependency: map to an appropriate safe
  `409`/`422` domain code, not `500`;
- API implementation defects remain `500 INTERNAL_ERROR`.

Retries belong at the dependency boundary and only for operations proven to be
idempotent or protected by an idempotency key. Never automatically retry a
financial posting merely because it timed out; the result may be unknown.

Log dependency name, duration, outcome, and provider correlation/reference ID,
but not full payloads. Mark whether the transaction outcome is known,
rejected, or indeterminate.

## 13. Financial-operation requirements

For posting, authorization, transfer, disbursement, reversal, and other
money-moving commands:

- distinguish **rejected**, **failed before submission**, and **outcome
  unknown**;
- support or plan an idempotency key for commands that clients may retry;
- return `409 OPERATION_ALREADY_COMPLETED` for a confirmed duplicate outcome;
- return a specific retry-safe or outcome-unknown code when confirmation is
  unavailable;
- never tell a client to blindly retry an indeterminate posting;
- include the correlation ID and safe operation/reference ID in audit records;
  and
- keep compliance audit events separate from diagnostic application logs.

An audit trail answers who performed a business action and what changed.
Diagnostic logs explain why software failed. One does not replace the other.

## 14. External/versioned contract exception

The Channels canonical API currently uses `{ success, data }` and
`{ success, error }`. It may preserve that published wire format. Internally it
must still:

- classify failures with the same taxonomy;
- use safe messages and stable external error codes;
- include/return a correlation ID;
- use the centralized logger and redaction rules; and
- avoid returning `ex.Message` for unexpected exceptions.

An adapter at that API boundary should translate the internal standard error
to the canonical external envelope.

## 15. Testing strategy

### Unit tests

- every exception type maps to its documented status and code;
- unexpected exceptions become `500 INTERNAL_ERROR` with no internal message;
- validation errors preserve field mappings;
- correlation IDs are accepted, generated, validated, and returned correctly;
- redaction removes all prohibited fields;
- severity selection follows the logging policy.

### Integration tests

For representative endpoints, verify:

- 400 invalid body and field errors;
- 401 missing/expired authentication;
- 403 insufficient permission;
- 404 missing resource;
- 409 invalid workflow state and duplicate/concurrency conflict;
- 413 oversized upload where applicable;
- 503 simulated dependency failure;
- 500 simulated unexpected failure;
- exact content type and envelope shape;
- correlation header/body equality; and
- one—not zero or two—corresponding structured log events for a 500.

### Contract tests

Maintain serialized response fixtures so field removal, renaming, casing, or
code/status drift fails CI. Add the standard errors and endpoint-specific domain
codes to each API integration specification.

### Security tests

Assert that responses and captured test logs do not contain exception stacks,
SQL text, credentials, tokens, account numbers, or supplied sensitive values.

## 16. Incremental implementation plan

### Phase 1: contract and infrastructure

1. Approve this document and the initial error-code catalogue.
2. Add response/error models and shared error constants.
3. Add correlation-ID handling to all API responses.
4. Add the global exception handler and exception logger.
5. Configure a durable structured-log provider and environment-specific sinks.
6. Add unit and integration tests for the shared pipeline.

This phase must not require all controllers to be migrated. Unhandled
exceptions immediately become safe and traceable.

### Phase 2: one vertical slice

Choose one actively used, bounded area—preferably a non-posting catalogue
controller first. Replace local `catch (Exception)` blocks, introduce domain
codes for known conflicts, update its API spec, and update the frontend client
for the new envelope. Verify production-like logs in a non-production
environment.

### Phase 3: business-critical workflows

Migrate loan lifecycle and batch/posting controllers. Define explicit codes for
every state transition and address idempotency/outcome-unknown behavior before
enabling automatic frontend retries.

### Phase 4: remaining controllers

Migrate feature-by-feature. Remove repeated private response helpers only when
the shared approach fully replaces them. Do not perform a risky repository-wide
mechanical rewrite.

### Phase 5: enforcement and operations

- Add contract/error tests to CI.
- Add dashboards for request rate, status, error code, dependency latency, and
  unhandled exceptions.
- Alert on sustained 5xx/503 rates, critical security events, and financial
  outcome-unknown events—not on isolated validation failures.
- Document log access, retention, redaction, and incident-response procedures.
- Prohibit new `InternalServerError(ex)` and raw exception-message responses
  through code review and, where practical, static checks.

## 17. Backward compatibility

The rollout should preserve the existing top-level `success` and `message`
fields so current clients continue to display a useful message. Adding
`error` and `correlationId` is additive.

During migration:

- keep documented existing status codes unless they are unsafe or plainly
  incorrect;
- publish changes where a former `400` becomes `409`/`422`;
- never change the meaning of an existing error code;
- let the frontend fall back to `message` and then a generic local message;
  and
- do not expose internal details merely to retain an accidental old behavior.

If a breaking cleanup becomes necessary, release it as an explicitly versioned
API contract.

## 18. Definition of done

The API error-handling initiative is complete when:

- every API failure returns a documented status and safe standard envelope;
- every response contains a valid correlation ID;
- unexpected exceptions are logged once and never exposed to clients;
- validation, authentication, authorization, not-found, conflict, dependency,
  and unexpected failures are distinguishable by stable codes;
- controllers no longer contain catch-all blocks that return raw exceptions;
- financial commands have documented retry/idempotency semantics;
- structured production logs are durable, searchable, access-controlled, and
  redacted;
- contract, integration, and security tests cover the error matrix; and
- the frontend consumes status/code rather than parsing message text.

## 19. Initial decisions requiring approval

Before implementation begins, confirm:

1. whether `422` will be adopted immediately or semantic validation will remain
   `400` during the first compatibility phase;
2. the durable structured-log provider and destination;
3. correlation-ID format and maximum accepted length;
4. production log retention/access rules;
5. which user, tenant, company, and branch identifiers are permitted in logs;
6. the first controller selected for the vertical-slice migration; and
7. the idempotency policy for money-moving commands.
