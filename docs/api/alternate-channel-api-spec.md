# Alternate Channel API — Client Integration Spec

Audience: back-office staff screens managing channel linking (Sacco Link,
Sparrow, MCo-op Cash, SpotCash, Citius, Agency Banking, PesaPepe, ABC Bank,
Broker, WhatsApp Banking) and per-channel-type fee configuration. This is
**Piece A** from
`WebApplication1/Areas/WhatsAppBanking/WORKFLOW.md` §4: a generic,
channel-type-agnostic controller, not scoped to any one channel. WhatsApp
Banking's bot-facing API (`Areas/WhatsAppBanking`, proposed, not yet built)
will call the same underlying app service for linking/status rather than
duplicating it.

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/AlternateChannelController.cs`.
- App service: `Application.MainBoundedContext/AccountsModule/Services/AlternateChannelAppService.cs`
  (`IAlternateChannelAppService`).
- DTO: `Application.MainBoundedContext.DTO/AccountsModule/AlternateChannelDTO.cs`.
- Enums (`Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs`):
  `AlternateChannelType`, `AlternateChannelFilter`, `AlternateChannelKnownChargeType`,
  `ChargeBenefactor`, `RecordStatus`.
- Auth: same JWT bearer scheme as every other controller — `[Authorize]`.

## History note

`IAlternateChannelAppService` (linking/update/replace/renew/delink/stop,
several find/paged/filtered overloads, and channel-type fee CRUD) already
existed — no `WebApplication1` controller called any of it. The only
existing entry point was `AlternateChannelService.svc.cs` (legacy WCF). This
is the first REST controller for this aggregate.

**Real findings from reading the reference MVC `RegisterController`, not
reproduced:**
- `Verify`/`Authorize` actions are bound to `DebitBatchDTO` and call
  `AuditDebitBatchAsync`/`AuthorizeDebitBatchAsync` — copy-pasted from a
  DebitBatch controller, nothing to do with `AlternateChannel`.
  `GetDebitBatchesAsync` is the same kind of leftover.
- `History`'s POST action is byte-for-byte identical to `Linking`'s POST
  (calls `AddAlternateChannelAsync` again) — it does not fetch history,
  despite the name.
- `Create(Guid id, ...)`/`Search`/`Linking(Guid id)`/`History(Guid id)` (the
  GET overloads) only pre-fill a create form from an existing
  `CustomerAccount` lookup — pure MVC view-staging. A JSON API client can
  call the existing CustomerAccount endpoints itself before submitting
  `POST /`; not reproduced here.

**Important correction to `WORKFLOW.md`'s working assumption**: there is
**no real maker-checker gate anywhere in this codebase** for
`AlternateChannel` linking. `AddNewAlternateChannel` never sets
`RecordStatus` (defaults to `0` = `New`); `UpdateAlternateChannel` copies
whatever `RecordStatus` the caller supplies straight onto the persisted
record — no check that it's currently `New`/`Edited`, no distinct
maker-vs-checker identity check, unlike e.g. the Batch Procedures module's
real `Audited`/`Authorized` guard clauses. `POST {id}/approve` and
`POST {id}/reject` below are a thin convenience over that same ungated
`Update` call, not new enforcement this controller invents. If real
maker-checker enforcement is wanted here, it doesn't exist at any layer yet
— a product/security decision, not something fixable by adding a route.

**Also flagged, not fixed**: `AlternateChannelDTO.CheckAlternateChannelNumber`
(the `CardNumber` validator) unconditionally blanks `CardNumber` for
`AlternateChannelType.AgencyBanking` and `.Citius` — `POST`/`PUT` can never
succeed for those two channel types today. No spec anywhere states the
intended `CardNumber` format for either, so this is flagged rather than
guessed at.

**Since fixed**: `AlternateChannelType.WhatsAppBanking = 512` is added,
with a real `CheckAlternateChannelNumber` validation case — E.164-shaped,
the same regex `MCoopCash`/`SpotCash`/`PesaPepe` already use — and the
matching `MaskedCardNumber` masking-style case (grouped with the other
phone-number-shaped channels), so it does not inherit the
`AgencyBanking`/`Citius` gap above. No changes were needed in this
controller for the new type — confirms the "generic across every
`AlternateChannelType`" design actually holds.

## 1. List / read

### `GET /api/accounts/alternatechannels`

Unpaged, every alternate channel link.

```json
{ "success": true, "message": "", "data": AlternateChannelDTO[] }
```

### `GET /api/accounts/alternatechannels/paged?text=&filter=&pageIndex=&pageSize=`

`text` empty/omitted returns the full unfiltered page. `filter`
(`AlternateChannelFilter` — default `0` = `PrimaryAccountNumber`) picks
which field `text` searches against (serial number, ID number, name,
address fields, references, ...). `pageIndex` default `0`, `pageSize`
default `20`.

```json
{ "success": true, "message": "", "data": { "pageCollection": AlternateChannelDTO[], "itemsCount": number } }
```

### `GET /api/accounts/alternatechannels/paged/type/{type}/status/{recordStatus}?text=&filter=&pageIndex=&pageSize=`

Exact-match filter on both `type` (`AlternateChannelType`) and
`recordStatus` (`RecordStatus`) — **both required**, no "any" sentinel
exists at the specification layer. This is the checker-inbox query: e.g.
`type=1&status=0` (Sacco Link, `New`) lists every Sacco Link link waiting
on `POST {id}/approve` or `POST {id}/reject`.

### `GET /api/accounts/alternatechannels/third-party-notifiable/type/{type}?text=&filter=&pageIndex=&pageSize=`

Approved, unlocked, not-yet-`IsThirdPartyNotified` links of `type`. Real,
already-built filtering logic; no consumer of this exists yet in this
codebase (see `WORKFLOW.md` §3/§9 — the outbound B2C host is unimplemented).

### `GET /api/accounts/alternatechannels/{id}`

```json
{ "success": true, "message": "", "data": AlternateChannelDTO }
```

`404` if not found.

### `GET /api/accounts/alternatechannels/by-customer-account/{customerAccountId}`

Every channel linked to that `CustomerAccount` (a customer can have more
than one channel of different types, or, for channel types not gated to
one-per-account, potentially more than one of the same type — see
`AddNewAlternateChannel`'s type-specific duplicate check below).

### `GET /api/accounts/alternatechannels/by-customer/{customerId}`

Every channel across all of a customer's accounts.

### `GET /api/accounts/alternatechannels/by-card-number?cardNumber=&type=`

`cardNumber` required (query param, not a route segment — MSISDN-shaped
values like `+254712345678` don't round-trip cleanly through URL path
segments). `type` optional — omit for "any type with this card number",
supply it to also match `AlternateChannelType`. This is the exact lookup
`MobileToBankRequestAppService` uses internally to match a C2B deposit
confirmation's MSISDN to a linked account.

## 2. Link / update / lifecycle

### `POST /api/accounts/alternatechannels`

Body: `AlternateChannelDTO`. Required: `CustomerAccountId`, `Type`,
`CardNumber` (format validated per `Type` — see the flagged
`AgencyBanking`/`Citius` gap above). `400` with real validation messages on
failure.

Duplicate detection, per `AddNewAlternateChannel`:
- Any type: a `409` if `CardNumber`+`Type` is already assigned to another
  account.
- `SaccoLink`/`Sparrow`/`AgencyBanking`/`AbcBank` additionally: `409` if the
  target `CustomerAccountId` already has a link of that same `Type` (one
  per account for these types). `MCoopCash`/`Citius`/`SpotCash`/`PesaPepe`
  have no such per-account cap.

```json
{ "success": true, "message": "Operation Success", "data": AlternateChannelDTO }
```

New records always start `RecordStatus: New` (`0`).

### `PUT /api/accounts/alternatechannels/{id}`

Body: `AlternateChannelDTO`. Generic field update — `CardNumber`/`Remarks`/
`DailyLimit`/`IsThirdPartyNotified`(+`ThirdPartyResponse`)/`IsLocked`/
`RecordStatus`. `CustomerAccountId` in the body must match the persisted
record's, or the call is a no-op — indistinguishable from "not found" at
the app-service layer, so both map to `404` here. `409` (with the real
message) on a `CardNumber`+`Type` collision with another record. This is
also, today, the only way `RecordStatus` moves at all — see the maker-
checker correction above. Prefer `POST {id}/approve`/`{id}/reject` (§3) for
that specific case.

### `POST /api/accounts/alternatechannels/{id}/replace`

Body: `AlternateChannelDTO` — card/number replacement (lost/stolen SIM, new
card, ...). Sets `RecordStatus` back to `Edited` and logs a **Channel
Replacement** entry in the customer's account history, unlike plain
`PUT {id}`. Same `CustomerAccountId`-must-match / `409`-on-collision rules
as `PUT`.

### `POST /api/accounts/alternatechannels/{id}/renew`

Same shape and preconditions as `replace` — logs **Channel Renewal**
instead.

### `POST /api/accounts/alternatechannels/{id}/stop`

Body: `AlternateChannelDTO` (only `CustomerAccountId` and `Remarks`
matter). Locks the channel (`IsLocked: true`, transacting suspended) and
logs **Channel Stoppage**. Distinct from `delink` below, which removes the
link entirely rather than suspending it. `404` if `id` doesn't exist.

### `POST /api/accounts/alternatechannels/{id}/delink`

Body: `AlternateChannelDTO` (only `CustomerAccountId` and `Remarks`
matter). **Hard-deletes** the `AlternateChannel` row after logging a
**Channel Delinking** history entry. `POST`, not `DELETE` — the body's
`CustomerAccountId`/`Remarks` drive the history log entry and aren't
recoverable from the route `id` alone. `404` if `id` doesn't exist.

```json
{ "success": true, "message": "Operation Success", "data": null }
```

## 3. Approve / reject

Convenience wrappers over `PUT {id}` — fetch the current record, flip
`RecordStatus`, save. **Not a maker-checker gate** (see the correction
above): nothing checks the record is currently `New`/`Edited`, nothing
checks the approver differs from whoever created or last edited it.

### `POST /api/accounts/alternatechannels/{id}/approve`

Body (optional): `{ "remarks": "..." }`. Sets `RecordStatus: Approved`.

### `POST /api/accounts/alternatechannels/{id}/reject`

Same body shape, `RecordStatus: Rejected`.

Both: `404` if `id` doesn't exist, `409` (with the real message) on the
same `CardNumber`+`Type` collision check `PUT` runs (the round-tripped
`CardNumber` is unchanged, so this only fires if another record was
created/edited to collide in between).

## 4. Fees (per channel type, not per link)

Fees are scoped by channel **type**, not by individual link — every link
of a given `Type` shares the same commission for a given
`AlternateChannelKnownChargeType` (`Linking`, `Replacement`, `Renewal`,
`WithdrawalCharges`, `DepositCharges`, `MiniStatementCharges`,
`BalanceInquiryCharges`, `AirtimeCharges`, `PINResetCharges`, ...).

### `GET /api/accounts/alternatechannels/types/{type}/commissions?knownChargeType={n}`

`knownChargeType` required, no "all" default (same reasoning as
`LoanProductController`'s commissions sub-resource).

```json
{ "success": true, "message": "", "data": CommissionDTO[] }
```

### `PUT /api/accounts/alternatechannels/types/{type}/commissions`

Body:
```json
{ "knownChargeType": number, "chargeBenefactor": number, "commissions": CommissionDTO[] }
```

Full replace of which `Commission` records apply for this
`type`+`knownChargeType` — only each `CommissionDTO.Id` is read (join-table
pattern, same as `LoanProductController`'s own commissions sub-resource).
`chargeBenefactor` (`ChargeBenefactor` — `0` = Customer, `1` = Institution)
applies to the whole batch, not per-commission.

## 5. What this controller deliberately does not cover

- **`MobilePIN`/`NewMobilePIN`/`ResetMobilePIN`** — **update, since the
  WhatsApp Banking build (see `WORKFLOW.md` §5): this gap is closed.**
  `IAlternateChannelAppService.SetMobilePIN`/`VerifyMobilePIN` now exist,
  hashed at rest via the same PBKDF2 utility used for staff credentials, and
  `AddNewAlternateChannel` accepts and hashes a `MobilePIN` at link time.
  **This controller itself still doesn't expose a PIN route** — staff don't
  set/reset a customer's channel PIN through `AlternateChannelController`;
  that only happens self-service, through `Areas/WhatsAppBanking`'s
  `RegistrationController.POST /link` (initial PIN) and
  `IdentityController.POST /pin/reset` (reset), both OTP-gated. If a staff-
  facing PIN reset is ever needed, it'd be a new route here calling the same
  `SetMobilePIN`, not new app-service work.
- **C2B/B2C money movement** (`MobileToBankRequest`/`BankToMobileRequest`)
  — separate aggregates/app services, out of scope for this controller.
  See `WORKFLOW.md` §3/§7/§9.
- ~~`AlternateChannelType.WhatsAppBanking` does not exist yet~~ — **done**:
  the enum value (`512`) and the matching `CheckAlternateChannelNumber`
  validation case are both added. This controller needed no changes for
  it, confirming the design.
