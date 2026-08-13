# WhatsApp Banking API — Integration Guide

**Status: built and compiling.** This replaces the earlier "proposed design"
draft — every endpoint below is real code in this repository. Read
`WebApplication1/Areas/WhatsAppBanking/WORKFLOW.md` for the functional
design and history of how this was built; this document is the practical
integration guide for the bot/conversation-orchestrator team.

**Before you integrate, three things must be configured by SACCO back
office** — none of these are code, but nothing here works without them. All
three are `<appSettings>` keys in `WebApplication1/Web.config`, read into
`DefaultSettings.Instance` on every access
(`Infrastructure.Crosscutting.Framework/Utils/DefaultSettings.cs`) — ops sets
them there directly, no controller, no rebuild:

| Setting | Web.config `<appSettings>` key | `DefaultSettings` property | What happens if unset |
|---|---|---|---|
| Digital Channel Branch | `DigitalChannelBranchId` (a real `Branch` row's GUID) | `Instance.DigitalChannelBranchId` | `POST /customer` returns `500` |
| Mobile money Paybill/shortcode | `MobileMoneyPaybillBusinessShortCode` | `Instance.MobileMoneyPaybillBusinessShortCode` | `GET /deposits/instructions` returns `500` |
| Inbound webhook shared secret | `MobileToBankWebhookSecret` (share this value with the mobile money provider) | `Instance.MobileToBankWebhookSecret` | `POST /webhooks/c2b-confirmation` returns `500` for everyone, always |

A blank or missing key is read as "unconfigured" — `DigitalChannelBranchId`
falls back to `Guid.Empty`, the string settings fall back to `null` — which
is exactly what the "what happens if unset" column above already assumed;
this wiring didn't change that contract, just made it configurable without
a code change.

Source of truth:
- Controllers: `WebApplication1/Areas/WhatsAppBanking/Controllers/`
  (`IdentityController.cs`, `RegistrationController.cs`,
  `TransactionsController.cs`, `DepositWebhookController.cs`) and the shared
  `WhatsAppBankingTokenStore.cs` in the same Area.
- Channel linking: `AlternateChannelDTO`, `IAlternateChannelAppService`
  (`Application.MainBoundedContext/AccountsModule/Services/`) — also see
  `docs/api/alternate-channel-api-spec.md`, the generic staff-facing
  linking/approval/fee controller this API builds on top of rather than
  duplicates.
- Withdrawal posting: `IBankToMobileRequestAppService.RequestPayout` — new,
  real GL posting, added specifically because the older
  `AddNewBankToMobileRequest` method does not debit or post anything (see
  §7.2 for the full story).
- Deposit matching: `IMobileToBankRequestAppService.AddNewMobileToBankRequest`
  — already existed and already does real GL posting; this API is the first
  thing that makes it reachable from outside this system.
- OTP delivery (SMS): `ITextAlertAppService.AddQuickTextAlert`.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/whatsappbanking` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | See §2 — **three** mechanisms, used at different points |

## 2. Authentication

1. **Channel auth** — `Authorization: Bearer <channel service JWT>` on
   every request to `IdentityController`/`RegistrationController`/
   `TransactionsController` (same JWT bearer scheme as every other
   controller in this system, issued once to the bot backend as a
   dedicated service account). This proves the caller is the legitimate
   bot/orchestrator, **not** the end customer.
2. **`phoneVerifiedToken`** — short-lived (10 minutes), returned by
   `POST /otp/verify` (§4.2). Proves momentary control of a phone number.
   Used only to complete **registration** (§5.1) and **linking**
   (§5.2)/**PIN reset** (§4.4) — it is **not** a general transacting
   session, and each token is single-use (consumed on the call that uses
   it, except `POST /customer` which deliberately doesn't consume it so
   `POST /link` can still use the same token right after).
3. **`sessionToken`** — returned by `POST /pin/authenticate` (§4.3) or
   `POST /pin/reset` (§4.4), sent as `X-WhatsApp-Session` on every
   fulfillment call (§6, §7). Requires an **`Approved`**, unlocked
   `AlternateChannel` link. 15 minutes, refreshed on every successful call.

**Important — read before relying on session/OTP state across a
deployment**: `phoneVerifiedToken`/`sessionToken`/OTP codes are held in an
in-process memory cache (`IAppCache`/`System.Runtime.Caching`), the same
mechanism this codebase already uses for read-model caching. This does
**not** survive an app-pool recycle and is **not** shared across
load-balanced instances. Fine for a single-instance pilot; a
multi-instance production deployment needs this moved to a shared store
(Redis, a DB table) — not built here, flagged for whoever takes this to
production scale.

`401` (`message: "Session expired or invalid"`) if `X-WhatsApp-Session` is
missing/expired/unknown. `403` (`"This channel link is not yet approved"` /
`"This channel link is locked"`) from `POST /pin/authenticate` if the
underlying `AlternateChannel` isn't `Approved` or is locked.

## 3. Response envelope

Same shape as every other controller in this system:

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

## 4. Identity — `IdentityController`

### 4.1 Request OTP — `POST /otp/request`

```json
{ "phoneNumber": "+254712345678" }
```

Starts either registration+linking (new number) or PIN reset
(already-linked number) — the bot decides which flow it's in from §4.2's
response; this endpoint's response is identical either way.

```json
{
  "success": true,
  "message": "OTP sent",
  "data": { "phoneNumber": "+254712345678", "expiresInSeconds": 300 }
}
```

`400` (`"Could not send OTP - check the phone number format..."`) if
`phoneNumber` isn't E.164-shaped (`+` prefix, 13+ characters) — the
underlying SMS gateway silently drops malformed recipients rather than
erroring, so this is checked and surfaced explicitly here.

### 4.2 Verify OTP — `POST /otp/verify`

```json
{ "phoneNumber": "+254712345678", "otp": "482913" }
```

`400` (`"Incorrect or expired OTP"`) if wrong/expired — same message
either way, doesn't reveal which.

```json
{
  "success": true,
  "message": "Verified",
  "data": {
    "phoneVerifiedToken": "9a1c4e2d8b3f4a6e9c2d5f7e1b0a3c44",
    "expiresInSeconds": 600,
    "isExistingCustomer": true,
    "hasApprovedLink": false,
    "customer": {
      "id": "3a2f9e7a-6b1a-4a2b-9a1a-0e6f5c8b7a10",
      "firstName": "Grace",
      "lastName": "Wanjiru",
      "accounts": [
        { "id": "7b8c9d0e-1111-2222-3333-444455556666", "accountNumber": "001-0000456-004-012" }
      ]
    }
  }
}
```

`isExistingCustomer` is resolved two ways, in order: first by checking
whether this phone number is already linked to **any** `AlternateChannel`
(a member already using Sacco Link/Sparrow/etc. is still an existing
customer, not just a WhatsApp-specific check), then by searching
`Customer.Address.MobileLine` for a member who's never linked any channel.
`hasApprovedLink` is specifically about a `WhatsAppBanking`-type link being
`Approved` and unlocked. `customer` is `null` when `isExistingCustomer` is
`false`.

**Bot routing**: `isExistingCustomer: false` → §5.1 (register) then §5.2
(link). `isExistingCustomer: true, hasApprovedLink: false` → straight to
§5.2 (link), skip registration. `hasApprovedLink: true` → this number is
already fully set up; route to §4.3 instead.

### 4.3 Authenticate with PIN — `POST /pin/authenticate`

Used at the start of every ordinary session — not OTP.

```json
{ "phoneNumber": "+254712345678", "pin": "4821" }
```

`404` if the number has no `WhatsAppBanking`-type link at all (route to
§5.2). `403` if not `Approved`/is locked (§2). `400`
(`"Incorrect PIN"`) on mismatch — **retry-lockout behavior is not
implemented** (`AlternateChannel.IsLocked` exists for this; nothing sets it
automatically after N failed attempts — still open, see §8).

```json
{ "success": true, "message": "", "data": { "sessionToken": "b6f2b6b06e0e4d0a9d3a1e9a7a9c2f11", "expiresInSeconds": 900 } }
```

### 4.4 Reset PIN — `POST /pin/reset`

Requires `phoneVerifiedToken` (a fresh OTP verify, not the old PIN — PIN
reset is deliberately step-up-authenticated).

```json
{ "phoneVerifiedToken": "9a1c4e2d8b3f4a6e9c2d5f7e1b0a3c44", "newPin": "7734" }
```

Internally: `IAlternateChannelAppService.SetMobilePIN` — hashed at rest via
the same `PasswordHash` (PBKDF2) utility this codebase already uses for
staff credentials, never stored or returned in plain text. Issues a fresh
session immediately, same shape as §4.3.

**Not implemented**: `AlternateChannelKnownChargeType.PINResetCharges` —
whether/how this fee applies is a pricing decision, not an engineering
default; no existing "charge this fee" one-liner exists elsewhere in this
codebase to safely reuse, so it's flagged rather than guessed at.

## 5. Registration and linking — `RegistrationController`

### 5.1 Register a new customer — `POST /customer`

Requires `phoneVerifiedToken`, only meaningful when §4.2 said
`isExistingCustomer: false`. Refuses (`409`) if the phone number turns out
to already resolve to a customer — enforced server-side, not just assumed
of a well-behaved bot.

```json
{
  "phoneVerifiedToken": "9a1c4e2d8b3f4a6e9c2d5f7e1b0a3c44",
  "firstName": "Peter",
  "lastName": "Otieno",
  "identityCardType": 1,
  "identityCardNumber": "23456789",
  "gender": 1,
  "birthDate": "1994-03-12"
}
```

`identityCardType`: `IdentityCardType` enum (`1`=National ID, `2`=Passport,
`3`=Alien ID, ...). `gender`: `Gender` enum (`1`=Male, `2`=Female).

Internally: creates an Individual `Customer` under
`DefaultSettings.Instance.DigitalChannelBranchId` (`AddressMobileLine` set
to the verified phone number — both for a correct contact record and so
later lookups by mobile line find this customer), then bulk-creates one
account per product mandatory-attached to that branch's company — no
product picker, matching how self-onboarding works elsewhere in this
system. `500` if the Digital Channel Branch isn't configured or no longer
exists — surface as "try again later," page a human; this is a setup
problem, not a customer error.

```json
{
  "success": true,
  "message": "Customer created",
  "data": {
    "id": "3a2f9e7a-6b1a-4a2b-9a1a-0e6f5c8b7a10",
    "firstName": "Peter",
    "lastName": "Otieno",
    "accounts": [{ "id": "7b8c9d0e-1111-2222-3333-444455556666", "accountNumber": "001-0000457-004-012" }]
  }
}
```

Does **not** link the channel yet — proceed to §5.2 with one of the
returned `accounts[].id`. The `phoneVerifiedToken` used here is still
valid for that next call.

### 5.2 Link this number for WhatsApp Banking — `POST /link`

```json
{
  "phoneVerifiedToken": "9a1c4e2d8b3f4a6e9c2d5f7e1b0a3c44",
  "accountId": "7b8c9d0e-1111-2222-3333-444455556666",
  "pin": "4821"
}
```

`409` if this number already has a `WhatsAppBanking`-type link. `403`
(`"This account does not belong to the verified customer"`) if `accountId`
doesn't belong to the customer this phone number resolves to — checked
explicitly; a `phoneVerifiedToken` only proves phone control, not which
account it's allowed to touch. `400` if the account can't be found or the
underlying `AlternateChannelDTO` fails validation.

Internally: creates an `AlternateChannel` — `Type: WhatsAppBanking` (`512`),
`CardNumber` = the verified phone number, `MobilePIN` = `pin` (hashed at
rest), `DailyLimit` =
`DefaultSettings.Instance.AlternateChannelsDefaultDailyLimit` (a
back-office-configured default, not client-supplied — currently
`40000`), `RecordStatus: New`.

```json
{
  "success": true,
  "message": "Submitted for approval - we'll confirm once your WhatsApp Banking link is active.",
  "data": { "id": "c4d5e6f7-2222-3333-4444-555566667777", "recordStatus": 0 }
}
```

`recordStatus: 0` is `New` (`RecordStatus`: `0`=New, `1`=Edited,
`2`=Approved, `3`=Rejected).

**Read this before assuming "approved" means what it sounds like**: back
office approves via `docs/api/alternate-channel-api-spec.md`'s
`POST /api/accounts/alternatechannels/{id}/approve` — real and callable
today. But it is **not a maker-checker gate**: nothing checks the record
is currently `New`, nothing checks the approver differs from whoever
created the link. It's an authorized-staff status flip, not independently
verified. There's also no push/poll mechanism here for the bot to learn
*when* approval happens — the bot's only way to find out is to try
`POST /pin/authenticate` (§4.3) periodically and see whether it still
returns `403`.

## 6. Accounts & balance — `TransactionsController`

Require `X-WhatsApp-Session` (§4.3/§4.4).

### 6.1 List accounts — `GET /accounts`

```json
{
  "success": true,
  "message": "",
  "data": [
    { "id": "7b8c9d0e-1111-2222-3333-444455556666", "accountNumber": "001-0000457-004-012", "productDescription": "Ordinary Savings" }
  ]
}
```

Scoped to the session's customer via `CustomerAccountsByCustomerId` — not
just the one account that was linked in §5.2.

### 6.2 Balance — `GET /accounts/{accountId}/balance`

`404` if `accountId` doesn't exist or doesn't belong to the session's
customer — checked explicitly (a session token alone does not imply
access to every account id someone might guess).

```json
{
  "success": true,
  "message": "",
  "data": {
    "accountId": "7b8c9d0e-1111-2222-3333-444455556666",
    "accountNumber": "001-0000457-004-012",
    "availableBalance": 12500.00,
    "feeApplicable": false
  }
}
```

`feeApplicable` reflects whether
`AlternateChannelKnownChargeType.BalanceInquiryCharges` is configured for
`WhatsAppBanking` — looked up, **not yet actually charged** (same
"flagged, not guessed at" reasoning as PIN reset charges above).

## 7. Deposits & withdrawals

### 7.1 Deposit — `GET /deposits/instructions`

Not a money-movement call — WhatsApp Banking doesn't pull funds itself.

```json
{
  "success": true,
  "message": "",
  "data": {
    "method": "MobileMoneyPaybill",
    "businessShortCode": "123456",
    "accountReference": "+254712345678",
    "note": "Use your linked WhatsApp Banking number as the account/reference."
  }
}
```

`500` if `DefaultSettings.Instance.MobileMoneyPaybillBusinessShortCode`
isn't configured.

**What actually happens after the customer pays**: the mobile money
provider must call
`POST /api/whatsappbanking/webhooks/c2b-confirmation` (§7.3, new, real,
built specifically to close this loop) with the payment confirmation. That
matches the payer's MSISDN against the `AlternateChannel` link from §5.2
and posts a real deposit journal via the existing, already-correct
`MobileToBankRequestAppService`. **This endpoint existing is necessary but
not sufficient** — the provider (Safaricom or whoever) must actually be
configured to call it. That's an external integration step, not something
this codebase can do for you.

### 7.2 Withdrawal — `POST /withdrawals`

```json
{ "accountId": "7b8c9d0e-1111-2222-3333-444455556666", "amount": 500.00 }
```

`404` if `accountId` doesn't belong to the session's customer. `400` if
`amount` is not positive, exceeds the linked channel's `DailyLimit`, or
exceeds the account's available balance.

**Corrected from the earlier draft of this spec**: that draft assumed
`IBankToMobileRequestAppService.AddNewBankToMobileRequest` "debits the
account, queues the payout." It does **neither** — read directly, it never
references a `CustomerAccountId` at all, it's purely an insert-only audit
row. This endpoint instead calls a **new** method,
`IBankToMobileRequestAppService.RequestPayout`, which does the real work:
balance-checks, then posts a real double-entry journal (Debit the
customer's product G/L, Credit
`SystemGeneralLedgerAccountCode.MobileWalletB2CSettlement` — the exact
reverse of how `MobileToBankRequestAppService` posts a C2B deposit), then
records a `BankToMobileRequest` row for a future outbound payout process
to pick up.

```json
{
  "success": true,
  "message": "Your account has been debited. Payout to your mobile money number is not yet automated - back office will process it manually.",
  "data": { "bankToMobileRequestId": "a1b2c3d4-5555-6666-7777-888899990000", "status": "Pending" }
}
```

**Read the message field, don't hardcode a happier one**: the debit and
journal are real — money genuinely leaves the customer's available
balance. The payout leg is not: `SwiftFinancials.BankToMobileHostInterface`
(the process that would actually pay the customer out over mobile money)
is still an empty, unimplemented stub. Until it's built, every withdrawal
needs a manual back-office process to actually disburse funds against the
`BankToMobileRequest` row this call creates — **do not tell the customer
"you'll receive it shortly"** in the bot's own copy; the message above is
deliberately honest about this and should be surfaced roughly as-is.

### 7.3 Inbound C2B webhook — `POST /webhooks/c2b-confirmation`

`DepositWebhookController` — **provider-facing, not bot-facing**. No
`Authorize`/bearer JWT (a payment provider's server callback can't
participate in this system's staff/service JWT scheme); authenticated via
an `X-Webhook-Secret` header checked against
`DefaultSettings.Instance.MobileToBankWebhookSecret`. `500` for every
request, unconditionally, if that secret isn't configured — fails closed,
not open.

```json
{
  "MSISDN": "+254712345678",
  "BusinessShortCode": "123456",
  "TransID": "QGH7XXTS92",
  "BillRefNumber": "+254712345678",
  "TransAmount": 500.00,
  "TransTime": "20260213142530",
  "OrgAccountBalance": 1500.00,
  "ThirdPartyTransID": "",
  "InvoiceNumber": "",
  "KYCInfo": "",
  "Remarks": ""
}
```

Internally always sets `MatchByMSISDN: true` before calling
`AddNewMobileToBankRequest` — this is what makes phone-number matching
(rather than an encoded `BillRefNumber` account reference) the deposit
story for WhatsApp and any other MSISDN-identified channel. Confirm this
header/body shape against whatever the actual provider integration sends —
the field names above mirror `MobileToBankRequestDTO` directly, not a
specific provider's API (Safaricom's Daraja C2B callback shape is close
but not guaranteed identical; map at the integration layer if needed).

```json
{
  "success": true,
  "message": "Matched and posted",
  "data": { "id": "e5f6a7b8-6666-7777-8888-999900001111", "status": "Auto-Matched" }
}
```

`status: "Unmatched"` (message: `"Recorded, unmatched - queued for manual
back-office reconciliation"`) if no `AlternateChannel` link (or other
matching strategy) resolves the payer — queued, not lost, but not credited
automatically either.

## 8. What's still genuinely open

1. **Retry-lockout policy** for `POST /pin/authenticate` — not
   implemented. `AlternateChannel.IsLocked` exists as a field; nothing
   sets it after N failed PIN attempts.
2. **`AlternateChannelKnownChargeType.PINResetCharges` /
   `BalanceInquiryCharges`** — looked up but not actually charged (§4.4,
   §6.2). No existing "charge this fee" primitive to safely reuse.
3. **Multi-instance session/OTP storage** (§2) — in-process cache only,
   fine for a pilot, needs a shared store before scaling out horizontally.
4. **Maker-checker for channel approval** (§5.2) — an authorized status
   flip today, not independently-verified approval. Whether that's
   acceptable for a self-service-linked financial channel is a
   product/security decision, not resolved here.
5. **Approval notification** (§5.2) — no push/poll mechanism; the bot
   learns approval happened only by retrying `POST /pin/authenticate`.
6. **Outbound B2C payout automation** (§7.2) — `SwiftFinancials.BankToMobileHostInterface`
   is still an empty stub. Every withdrawal needs manual back-office
   disbursement until it's built.
7. **Provider integration for the inbound webhook** (§7.3) — the endpoint
   is real; getting an actual mobile money provider to call it is a
   separate, external integration step.
8. **KYC document capture** at registration (§5.1) — not designed here.
