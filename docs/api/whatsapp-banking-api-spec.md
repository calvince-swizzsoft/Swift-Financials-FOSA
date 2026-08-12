# WhatsApp Banking API — Client Integration Spec

**Status: proposed design, not yet implemented.** Revision note: this spec
originally designed channel identity (OTP session) and money movement from
scratch. It's been revised to build on an existing **Alternate Channels**
framework found already living in this codebase (channel linking, per-
channel fees, C2B/B2C transaction logging) instead of duplicating it — see
`WORKFLOW.md` §3 for the full comparison. If you started integrating
against the previous version of this doc, the auth model in particular has
changed (§2).

Audience: the WhatsApp bot / conversation-orchestrator backend. Functional
design: `WebApplication1/Areas/WhatsAppBanking/WORKFLOW.md`.

Source of truth:
- Customer/account creation (reused as-is): `docs/api/customer-api-spec.md`,
  `docs/api/customer-accounts-api-spec.md`.
- Channel linking: `AlternateChannelDTO`
  (`Application.MainBoundedContext.DTO/AccountsModule/AlternateChannelDTO.cs`),
  `IAlternateChannelAppService`.
- Fees: `AlternateChannelKnownChargeType`, `AlternateChannelTypeCommission`.
- Deposit matching (target design, not live yet): `MobileToBankRequestDTO`,
  `IMobileToBankRequestAppService`.
- Withdrawal payout (target design, not live yet): `BankToMobileRequestDTO`,
  `IBankToMobileRequestAppService`.
- OTP delivery (SMS): `docs/api/textalert-api-spec.md`.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/whatsappbanking` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | See §2 — **three** mechanisms, used at different points |

## 2. Authentication

1. **Channel auth** — `Authorization: Bearer <channel service JWT>` on
   every request. Proves the caller is the legitimate bot/orchestrator.
   Same JWT bearer scheme as every other controller, issued once to the
   bot backend by back office as a dedicated service account.
2. **`phoneVerifiedToken`** — short-lived (proposed: 10 minutes), returned
   by `POST /otp/verify` (§4). Proves momentary control of a phone number.
   Used only to complete **onboarding** (§5) and **linking**/**PIN reset**
   (§4, §5.4) — it is **not** a general transacting session.
3. **`sessionToken`** — returned by `POST /pin/authenticate` (§4.3), sent
   as `X-WhatsApp-Session` on every fulfillment call (§6, §7). Requires an
   **`Approved`** `AlternateChannel` link — issued for the customer whose
   number is linked, not just OTP-verified. Short-lived (proposed: 15
   minutes, refreshed on each call).

This is the main change from the previous draft: OTP alone no longer grants
a transacting session. A customer authenticates ongoing WhatsApp Banking
use with the PIN they set during linking (§4), the same convention every
other channel in this system already uses for its own linked customers.

`401` (`message: "Session expired or invalid"`) if `X-WhatsApp-Session` is
missing/expired/unknown on an endpoint that requires it. `403`
(`message: "This channel link is not yet approved"` /
`"This channel link is locked"`) if the underlying `AlternateChannel` isn't
`Approved`, or is `IsLocked`, at the time `POST /pin/authenticate` is
called.

## 3. Response envelope

Same shape as every other controller:

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

`200`/`201`/`400`/`401`/`403`/`404`/`409`/`500` — see per-endpoint notes
below for which apply where.

## 4. Identity — OTP (phone verification) and PIN (session)

### 4.1 Request OTP — `POST /otp/request`

```json
{ "phoneNumber": "+254712345678" }
```

Used to start **either** onboarding+linking (new number) **or** PIN reset
(already-linked number) — the bot decides which flow it's in; this
endpoint's response is identical either way (existence isn't revealed
pre-verification, same as the previous draft).

```json
{
  "success": true,
  "message": "OTP sent",
  "data": { "phoneNumber": "+254712345678", "expiresInSeconds": 300 }
}
```

### 4.2 Verify OTP — `POST /otp/verify`

```json
{ "phoneNumber": "+254712345678", "otp": "482913" }
```

`400` if wrong/expired.

```json
{
  "success": true,
  "message": "Verified",
  "data": {
    "phoneVerifiedToken": "9a1c4e2d-8b3f-4a6e-9c2d-5f7e1b0a3c44",
    "expiresInSeconds": 600,
    "isExistingCustomer": true,
    "hasApprovedLink": false,
    "customer": {
      "id": "3a2f9e7a-6b1a-4a2b-9a1a-0e6f5c8b7a10",
      "firstName": "Grace",
      "lastName": "Wanjiru",
      "accounts": [
        {
          "id": "7b8c9d0e-1111-2222-3333-444455556666",
          "accountNumber": "001-0000456-004-012"
        }
      ]
    }
  }
}
```

`hasApprovedLink: false` with `isExistingCustomer: true` is the "existing
customer, first time on WhatsApp" case — bot should go straight to linking
(§5.4), skipping onboarding (§5.1–5.3). `isExistingCustomer: false` routes
to onboarding first. `customer: null` when `isExistingCustomer` is `false`.

### 4.3 Authenticate with PIN — `POST /pin/authenticate`

Used at the start of every ordinary WhatsApp Banking session, **not** OTP.

```json
{ "phoneNumber": "+254712345678", "pin": "4821" }
```

`404` if the number has no `AlternateChannel` link at all (bot should
route to §5.4). `403` per §2 if not `Approved`/is locked. `400`
(`message: "Incorrect PIN"`) on mismatch — **retry-lockout behavior is not
designed here** (`AlternateChannel.IsLocked` exists for this; how many
attempts trip it isn't decided — `WORKFLOW.md` §8.5).

```json
{
  "success": true,
  "message": "",
  "data": { "sessionToken": "b6f2b6b0-6e0e-4d0a-9d3a-1e9a7a9c2f11", "expiresInSeconds": 900 }
}
```

### 4.4 Reset PIN — `POST /pin/reset`

Requires `phoneVerifiedToken` (fresh OTP verify, not a session token — PIN
reset is deliberately step-up-authenticated with OTP again, not just the
old PIN).

```json
{ "phoneVerifiedToken": "9a1c4e2d-8b3f-4a6e-9c2d-5f7e1b0a3c44", "newPin": "7734" }
```

Internally: `AlternateChannelDTO.ResetMobilePIN = true` /
`NewMobilePIN = newPin` through the existing `UpdateAlternateChannel` —
these fields already exist on the DTO for exactly this. Should charge the
existing `PINResetCharges` fee (`AlternateChannelKnownChargeType`) — see
§8. `201`/`200` success shape same as §4.3's `data`.

## 5. Onboarding and linking

### 5.1 Onboard a new customer — `POST /customer`

Unchanged from the previous draft. Requires `phoneVerifiedToken` (§4.2),
only valid when `isExistingCustomer` was `false`.

```json
{
  "phoneVerifiedToken": "9a1c4e2d-8b3f-4a6e-9c2d-5f7e1b0a3c44",
  "firstName": "Peter",
  "lastName": "Otieno",
  "identityCardType": 1,
  "identityCardNumber": "23456789",
  "gender": 1,
  "birthDate": "1994-03-12"
}
```

Internally: `POST /api/registry/customer` (`Type: 0`/Individual,
`branchId` = Digital Channel Branch, `docs/api/customer-api-spec.md`
§5.12), then bulk-create via
`POST /api/accounts/customer-accounts/customer/{customerId}/branch/{branchId}`
(`docs/api/customer-accounts-api-spec.md` §4.5). Same "no product picker"
reasoning as before. `500` if the Digital Channel Branch isn't configured
(`WORKFLOW.md` §8.7) — surface as "try again later," route to a human.

Success → `201`, `data` shape matches §4.2's `customer` object. Does
**not** link the channel yet — proceed to §5.4.

### 5.4 Link this number for WhatsApp Banking — `POST /link`

Requires `phoneVerifiedToken` and a target `accountId` (from §4.2's or
§5.1's `accounts` list — the bot should prompt if the customer has more
than one).

```json
{
  "phoneVerifiedToken": "9a1c4e2d-8b3f-4a6e-9c2d-5f7e1b0a3c44",
  "accountId": "7b8c9d0e-1111-2222-3333-444455556666",
  "pin": "4821"
}
```

`400` if `pin` fails whatever format rule is decided (`WORKFLOW.md` §8.5 —
not fixed yet). `409` if this account already has a `WhatsAppBanking`-type
link.

Internally: creates an `AlternateChannelDTO` — `Type:
AlternateChannelType.WhatsAppBanking` (proposed new enum value, not yet
added — `WORKFLOW.md` §8.1), `CardNumber` = the verified `phoneNumber`,
`MobilePIN` = `pin`, `DailyLimit` = a back-office-configured default (not
client-supplied — `WORKFLOW.md` §8.6), `RecordStatus: New`. **Important
implementation detail, not just a formality**: `AlternateChannelDTO`'s own
`CheckAlternateChannelNumber` validation switches behavior by `Type`, and
its `default` case **blanks `CardNumber` to empty for any type it doesn't
recognize** — adding `WhatsAppBanking` to that switch (MSISDN-shaped,
same branch as `MCoopCash`/`SpotCash`/`PesaPepe`) is required, not
optional, or every linking request will fail validation.

Success → `201`:

```json
{
  "success": true,
  "message": "Submitted for approval — we'll confirm once your WhatsApp Banking link is active.",
  "data": { "id": "c4d5e6f7-2222-3333-4444-555566667777", "recordStatus": 0 }
}
```

`recordStatus: 0` is `New` (same `RecordStatus` enum as
`docs/api/customer-api-spec.md` §7: `0`=New, `1`=Edited, `2`=Approved,
`3`=Rejected). The bot should not offer balance/deposit/withdraw until a
later `POST /pin/authenticate` succeeds (§4.3), which only happens once
this reaches `Approved` — **how a customer finds out it's approved (poll?
push from back office?) is not designed here** — `WORKFLOW.md` §8.2.

## 6. Accounts & balance

Require `X-WhatsApp-Session` (§4.3).

### 6.1 List accounts — `GET /accounts`

Unchanged from the previous draft — thin passthrough over
`GET /api/accounts/customer-accounts/customer/{customerId}`, scoped to the
session's customer.

### 6.2 Balance — `GET /accounts/{accountId}/balance`

Unchanged response shape from the previous draft. **New**: should look up
and, if configured, charge `AlternateChannelKnownChargeType.BalanceInquiryCharges`
for `AlternateChannelType.WhatsAppBanking` via
`IAlternateChannelAppService.FindCachedCommissions(channelType, chargeType)`
— the same fee hook every other channel's balance inquiry already has a
place for. Whether this fee actually applies (some channels don't charge
for it) is a pricing decision, not an engineering one.

## 7. Deposits & withdrawals

Require `X-WhatsApp-Session`. **Both endpoints below describe the target
design; neither is fully buildable today without additional backend work
called out explicitly per endpoint — see `WORKFLOW.md` §3/§8 for why.**

### 7.1 Deposit — `GET /deposits/instructions`

Not a money-movement call — WhatsApp Banking doesn't pull funds itself.
Returns how to pay:

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

Once the customer pays, the provider's own confirmation is expected to
reach `MobileToBankRequestAppService.AddNewMobileToBankRequest`, which
already matches by `MSISDN` against the `AlternateChannel` link created in
§5.4 and posts the deposit journal, charging
`AlternateChannelKnownChargeType.DepositCharges` per the existing pattern.
**This requires a new inbound REST webhook that does not exist anywhere in
this codebase yet** (`AddNewMobileToBankRequest` is currently reachable
only via a legacy WCF passthrough, which no real payment provider posts
to) — building that webhook is a prerequisite for this endpoint's promise
to be real, not an implementation detail of this spec. Until it exists,
`GET /deposits/instructions` can ship, but a customer's payment will not
actually land in their account automatically.

### 7.2 Withdrawal — `POST /withdrawals`

```json
{
  "accountId": "7b8c9d0e-1111-2222-3333-444455556666",
  "amount": 500.00
}
```

`404` if `accountId` doesn't resolve/belong to the session's customer.
`400` if `amount` is not greater than zero or exceeds the linked channel's
`DailyLimit`. Should charge `AlternateChannelKnownChargeType.WithdrawalCharges`.

Internally, target design: creates a `BankToMobileRequestDTO` (debits the
account, queues the payout) via the existing
`IBankToMobileRequestAppService.AddNewBankToMobileRequest`. **The process
that would actually pay the customer out over mobile money —
`SwiftFinancials.BankToMobileHostInterface` — is an empty, unimplemented
project.** A request can be created and the account debited; nothing
disburses. This endpoint should not be considered shippable until either
that host is implemented, or an interim manual/agent-redemption fallback
is decided (`WORKFLOW.md` §6/§8.4) — flagged here so it isn't missed at
build time.

Response, once (if) fully wired:

```json
{
  "success": true,
  "message": "Withdrawal requested — you'll receive it on your linked mobile money number shortly.",
  "data": { "bankToMobileRequestId": "a1b2c3d4-5555-6666-7777-888899990000", "status": "Queued" }
}
```

## 8. Enum reference

**`RecordStatus`** (§5.4) — `0`=New, `1`=Edited, `2`=Approved, `3`=Rejected
(same as `docs/api/customer-api-spec.md` §7).

**`AlternateChannelType`** — existing values `1`=SaccoLink, `2`=Sparrow,
`4`=MCoopCash, `8`=SpotCash, `16`=Citius, `32`=AgencyBanking, `64`=PesaPepe,
`128`=AbcBank, `256`=Broker. **`WhatsAppBanking` does not exist yet** —
proposed as `512`, next in sequence. Must be added to
`Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs` *and* to
`AlternateChannelDTO.CheckAlternateChannelNumber`'s validation switch
(§5.4) before linking will work.

**`AlternateChannelKnownChargeType`** (§4.4, §6.2, §7) — the subset this
API touches: `Deposit` (`4`), `WithdrawalCharges` (`3`),
`BalanceInquiryCharges` (`6`), `PINResetCharges` (`8`). Full list in
`Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs`.

**`identityCardType`** / **`gender`** (§5.1) — unchanged, same as
`docs/api/customer-api-spec.md` §7.

## 9. Open items to confirm with backend before building against this

Full list: `WORKFLOW.md` §8. The ones most likely to block a bot
integration starting early, in priority order:

1. **`AlternateChannelType.WhatsAppBanking` doesn't exist yet** — needs
   adding to the enum and the `CardNumber` validation switch (§5.4) before
   `POST /link` can work at all.
2. **Inbound C2B webhook** (§7.1) and **B2C payout host** (§7.2) are both
   real, unbuilt backend work, not configuration — deposit and withdrawal
   cannot go live without them regardless of how this API is shaped.
3. **Linking approval turnaround** (§5.4) — no existing checker-inbox UX
   is designed for this yet; a customer submitting `POST /link` has no way
   to know when (or whether) they're approved without one.
4. **PIN policy** — format, retry/lockout behavior, `DailyLimit` sourcing
   — all open, all affect `POST /link`/`POST /pin/authenticate`'s exact
   validation rules.
