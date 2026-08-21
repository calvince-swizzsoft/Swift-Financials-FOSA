# User Onboarding and First-Login Password Change

This document describes the REST application's employee/user onboarding
flow: identity creation, delivery of initial credentials through the central
email-alert pipeline, and the mandatory password change performed before a
new user receives an application JWT.

> **Bootstrapping the very first account.** Every administrator-created user
> below requires an existing administrator to create them — a brand-new
> deployment has none. `SwiftFinancials.Utility.exe` breaks that cycle by
> seeding one bootstrap `admin` account (in a full-access `Administrator`
> role) the first time it runs against an empty database, printing its
> one-time password to the console. It goes through this exact first-login
> flow like any other user. See
> [`DEPLOYMENT.md`](DEPLOYMENT.md#the-database-starts-with-zero-users--this-is-expected-and-self-healing)
> for details.

## Legacy behavior retained

The implementation is adapted from the reference application's
`DistributedServices.MainBoundedContext/MembershipManagerService.svc.cs`
and `SwiftFinancials.Web/Controllers/AccountController.cs`.

The reference application:

1. created a user with an initial password;
2. submitted an HTML login-details message to `IEmailAlertAppService`;
3. included the username, initial password, and configured client URL;
4. treated a null `LastPasswordChangedDate` as a first login; and
5. required `ChangePasswordAsync` with the existing password before allowing
   the user into the application.

The REST implementation preserves those business rules while replacing the
legacy MVC redirects, `TempData`, cookies, and WCF facade with JSON endpoints
and JWT authentication.

## End-to-end flow

```mermaid
sequenceDiagram
    actor Administrator
    participant API as Users API
    participant Identity as ASP.NET Identity
    participant Outbox as EmailAlertAppService
    participant Dispatcher as Email Dispatcher
    actor User
    participant Auth as Auth API
    participant UI as Web Client

    Administrator->>API: POST /api/administration/users
    API->>Identity: Create(user, temporary password)
    Identity-->>API: User created; LastPasswordChangedDate = null
    API->>Outbox: AddNewEmailAlert(login details)
    Outbox-->>Dispatcher: Persist and enqueue alert
    Dispatcher-->>User: Credentials email
    User->>UI: Open configured login link
    UI->>Auth: POST /api/auth/login
    Auth->>Identity: Verify username and temporary password
    Auth-->>UI: requiresPasswordChange = true; no JWT
    UI->>Auth: POST /api/auth/change-initial-password
    Auth->>Identity: ChangePassword(current, new)
    Auth->>Identity: Set LastPasswordChangedDate
    Auth-->>UI: JWT, username, and roles
```

The important security boundary is that the login endpoint does **not** issue
a normal bearer token while `LastPasswordChangedDate` is null. A new user
therefore cannot enter authenticated application routes before changing the
temporary password.

## User creation

### Endpoint

```http
POST /api/administration/users
Content-Type: application/json
Authorization: Bearer <administrator-token>
```

The existing `UserDTO` is accepted. Fields relevant to onboarding include:

```json
{
  "userName": "jane.doe",
  "email": "jane.doe@example.org",
  "firstName": "Jane",
  "otherNames": "Doe",
  "password": "Temporary#123",
  "branchId": "00000000-0000-0000-0000-000000000000",
  "employeeId": "00000000-0000-0000-0000-000000000000"
}
```

`UserManagerService.CreateUser` creates the `ApplicationUser` using ASP.NET
Identity. It deliberately leaves `LastPasswordChangedDate` null. After a
successful identity creation, it creates a security-critical, highest-priority
`EmailAlertDTO` through `IEmailAlertAppService.AddNewEmailAlert`.

The queued email contains:

- the user's display name;
- username;
- email address;
- temporary password;
- an instruction to change the password on first sign-in; and
- the configured web-client login link.

The message is HTML encoded before user-controlled values are inserted. It is
not sent directly with `SmtpClient`; persistence, MSMQ delivery, SMTP dispatch,
and status tracking follow the central process documented in
[`EMAIL-DELIVERY.md`](EMAIL-DELIVERY.md).

Relevant implementation:

- `WebApplication1/Areas/Identity/Controllers/UserController.cs`
- `WebApplication1/Areas/Identity/Services/UserManagerService.cs`
- `Application.MainBoundedContext/MessagingModule/Services/EmailAlertAppService.cs`

### Delivery semantics

A successful user-creation response means the identity record was created. It
does not mean the email reached the inbox. The email alert is asynchronous and
must be monitored in the Email Alerts screen/database and dispatcher services.
In this system, an email status of `Delivered` means that SMTP accepted the
message, not that the recipient opened or received it in their inbox.

## First login

### Login request

```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "jane.doe",
  "password": "Temporary#123"
}
```

If the credentials are valid and `LastPasswordChangedDate` is null, the API
returns HTTP 200 with:

```json
{
  "requiresPasswordChange": true,
  "userName": "jane.doe"
}
```

No `token` property is returned. The web client detects this response and
replaces the login form with the mandatory password-change form.

For users who have already changed their password, the existing response is
unchanged:

```json
{
  "token": "<jwt>",
  "userName": "jane.doe",
  "roles": ["Employee"]
}
```

## Initial password change

### Endpoint

```http
POST /api/auth/change-initial-password
Content-Type: application/json
```

```json
{
  "userName": "jane.doe",
  "currentPassword": "Temporary#123",
  "newPassword": "A-New-Strong#456",
  "confirmPassword": "A-New-Strong#456"
}
```

The endpoint:

1. requires every field and matching new-password confirmation;
2. confirms that the user exists and still has a null
   `LastPasswordChangedDate`;
3. calls ASP.NET Identity `ChangePasswordAsync`, which verifies the current
   password and applies the configured password policy;
4. sets `LastPasswordChangedDate` to the current UTC time; and
5. returns the normal JWT, username, and roles so the client can establish
   the authenticated session.

The endpoint is intentionally usable without a bearer token because the login
flow does not issue one to a first-time user. Possession of the valid current
password is required. After `LastPasswordChangedDate` is populated, this
endpoint rejects subsequent attempts; later password-management operations
should use an authenticated change-password endpoint.

Relevant implementation:

- `WebApplication1/Areas/Auth/Controllers/AuthController.cs`
- `WebApplication1/Areas/Identity/ApplicationUser.cs`
- frontend: `src/pages/Auth/Login.jsx` in `Swizzfinancial-FOSA`

## Configuration

The URL placed in the welcome email is controlled by `WebApplication1/Web.config`:

```xml
<add key="Frontend:LoginUrl" value="http://localhost:5173/login" />
```

Set this per deployment environment. Production must use the externally
reachable HTTPS login URL. If the setting is absent or blank, the current code
falls back to `http://localhost:5173/login`, which is suitable only for local
development.

SMTP credentials and dispatcher settings are separate from this URL. See
[`EMAIL-DELIVERY.md`](EMAIL-DELIVERY.md) for the MSMQ, Windows Service, and
SMTP configuration required to deliver the queued message.

## Password policy

The new password is validated by `ApplicationUserManager` using these
`Web.config` application settings:

- `RequiredPasswordLength`
- `PasswordRequireNonLetterOrDigit`
- `PasswordRequireDigit`
- `PasswordRequireLowercase`
- `PasswordRequireUppercase`

Client-side checks improve usability, but the server-side ASP.NET Identity
validator is authoritative.

## Security considerations

The credential email mirrors the reference application's behavior and the
current business requirement, but sending a reusable password by email has
inherent risk. Treat these alerts as security-critical and restrict access to
their stored bodies. Email administrators and anyone with Email Alerts detail
permission may otherwise be able to view temporary credentials.

Recommended future hardening:

- replace emailed passwords with a short-lived, single-use activation token;
- store only a masked/redacted body in general email-history views;
- rate-limit login and initial-password-change attempts;
- record failed onboarding delivery and provide an administrator resend flow;
- expire temporary credentials after a configurable interval; and
- ensure all production login links use HTTPS.

Never log the submitted current, temporary, or new password. Password values
must also be excluded from audit-trail activity strings and API error bodies.

## Deployment and verification checklist

1. Set `Frontend:LoginUrl` to the correct HTTPS address.
2. Confirm the email dispatcher prerequisites in `EMAIL-DELIVERY.md`.
3. Create a test user with a real test mailbox and a policy-compliant temporary
   password.
4. Confirm an Email Alert record is created with `Pending` status.
5. Confirm the dispatcher advances it to `Delivered`/SMTP accepted.
6. Open the link and sign in with the emailed username and temporary password.
7. Confirm the API returns `requiresPasswordChange: true` without a JWT.
8. Confirm a wrong current password is rejected.
9. Confirm a weak or mismatched new password is rejected.
10. Change to a valid new password and confirm a JWT is returned.
11. Confirm the temporary password no longer works.
12. Confirm subsequent login with the new password proceeds normally without
    the first-login prompt.

## Current scope

This implementation enforces the first-login condition represented by a null
`LastPasswordChangedDate`. The legacy reference also checked configured
password age for periodic expiry. Periodic password-expiry enforcement is not
part of this onboarding change and should be documented and implemented as a
separate authentication policy if it is required by the deployment.
