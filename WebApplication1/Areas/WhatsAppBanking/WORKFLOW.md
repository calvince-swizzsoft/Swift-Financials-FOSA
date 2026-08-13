# WhatsApp Banking — Functional Workflow (Front Office Phase)

**Status: proposed design, nothing in `Areas/WhatsAppBanking` exists in code
yet.** Revision note: an earlier draft of this document designed channel
identity and money movement from scratch. Before that shipped, an existing
**Alternate Channels** module was found already living in this codebase
(`AlternateChannel`/`AlternateChannelLog`/`MobileToBankRequest`/
`BankToMobileRequest` — see §3) — the same problem this document solves,
already partially built for other channels (Sacco Link, Sparrow, M-Co-op
Cash, SpotCash, ...). This revision designs WhatsApp Banking as a new
channel on top of that framework instead of a parallel one. See §3 for what
that changes and §9 for what it doesn't solve on its own. **This latest
revision splits the backend work into two separate, sequenced pieces** — a
generic, channel-type-agnostic `AlternateChannelController` (no reference-app
precedent for building this per-channel) and the WhatsApp-specific bot-facing
API — see §4. **Piece A is now built** (`WebApplication1/Areas/Accounts/Controllers/AlternateChannelController.cs`,
`docs/api/alternate-channel-api-spec.md`) — building it surfaced a harder
truth than §4 originally assumed: there is no real maker-checker gate for
`AlternateChannel` linking anywhere in this codebase, at any layer. §4 and
§5 are updated accordingly.

Audience: anyone building or reviewing the `Areas/WhatsAppBanking` API
surface, or building the WhatsApp bot/conversation layer that calls it.

Source of truth:
- Customer/account creation (reused as-is):
  `docs/api/customer-api-spec.md`, `docs/api/customer-accounts-api-spec.md`.
- Deposits/withdrawals (existing in-branch path, referenced for comparison):
  `docs/api/frontoffice-api-spec.md` §4.
- OTP delivery: `docs/api/textalert-api-spec.md`.
- **Alternate Channels framework (this revision's foundation)**:
  - Linking: `Domain.MainBoundedContext/AccountsModule/Aggregates/AlternateChannelAgg/AlternateChannel.cs`,
    `Application.MainBoundedContext/AccountsModule/Services/IAlternateChannelAppService.cs`.
  - Channel types/fees: `AlternateChannelType`, `AlternateChannelKnownChargeType`
    in `Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs`.
  - Transaction log + reconciliation: `AlternateChannelLogAgg`,
    `AlternateChannelReconciliationPeriodAgg`/`...EntryAgg`.
  - C2B deposit matching: `Domain.MainBoundedContext/AccountsModule/Aggregates/MobileToBankRequestAgg/`,
    `Application.MainBoundedContext/AccountsModule/Services/MobileToBankRequestAppService.cs`.
  - B2C withdrawal queuing: `BankToMobileRequestAgg`,
    `BankToMobileRequestAppService.cs`, `SwiftFinancials.BankToMobileHostInterface`
    (the outbound host — **currently an empty, unimplemented project**).
  - Reference staff UI **Piece A (§4) adapts from** — all three are generic,
    keyed off `AlternateChannelDTO.Type`, and cover every existing channel
    type already, not just one: reference app
    `Areas/Accounts/Controllers/RegisterController.cs` (`Linking`, `Create`,
    `Edit`, `Verify`, `Authorize`, `History` actions — the staff linking
    lifecycle), `AlternateChannelsController.cs` and
    `AlternatechannelManagementController.cs` (per-channel-type fee/
    commission configuration).

## 1. What "WhatsApp Banking" means here, this phase

A self-service channel over WhatsApp. A member (or prospective member) chats
with the SACCO's WhatsApp Business number and, without visiting a branch or
talking to a teller, can:

1. Register as a customer, if they aren't one yet, and get a savings
   account opened.
2. Link their phone number as a **WhatsApp Banking alternate channel**
   against that account (§5) — a one-time step, subject to back-office
   approval, same as every other channel this system already supports.
3. Check balance / a mini-statement.
4. Deposit into an account.
5. Withdraw from an account.

**Explicitly out of scope this phase** (§8): loan application, anything
back-office/underwriting.

## 2. Actors & systems

```mermaid
flowchart LR
    Customer["End customer\n(WhatsApp app)"]
    Meta["WhatsApp Business Platform\n(Meta Cloud API)"]
    Bot["Bot / Conversation Orchestrator\n(new, separate service — not this repo)"]
    API["WhatsApp Banking API\nAreas/WhatsAppBanking (proposed, Piece B)"]
    AltChanCtrl["AlternateChannelController\n(BUILT, Piece A — generic,\nall channel types)"]
    AltChan["Alternate Channels framework\n(existing, unexposed)"]
    Core["Existing core banking API\nCustomer / CustomerAccounts / FrontOffice"]

    Customer <-->|WhatsApp messages| Meta
    Meta <-->|webhook + send API| Bot
    Bot -->|"fulfillment calls\n(this doc's companion spec)"| API
    API -->|"linking, fees, tx log"| AltChan
    API -->|"customer/account creation"| Core
    AltChanCtrl -->|"staff linking/approval,\nfee config, all channel types"| AltChan
```

- **End customer** — chats via WhatsApp, no app install.
- **WhatsApp Business Platform** — Meta's Cloud API. Not part of this
  codebase.
- **Bot / Conversation Orchestrator** — a new, separate service. Owns the
  conversation state machine, renders WhatsApp messages, calls the
  WhatsApp Banking API. Treated as **the frontend** in the companion spec.
- **WhatsApp Banking API** (`Areas/WhatsAppBanking`, proposed, **Piece B** —
  §4) — thin glue. Registers WhatsApp as a new `AlternateChannelType` and
  calls the existing `IAlternateChannelAppService`/
  `IMobileToBankRequestAppService`/`IBankToMobileRequestAppService` for
  everything channel-shaped; calls the existing Customer/CustomerAccounts
  controllers for onboarding.
- **`AlternateChannelController`** (**built, Piece A** — §4 —
  `WebApplication1/Areas/Accounts/Controllers/AlternateChannelController.cs`,
  `docs/api/alternate-channel-api-spec.md`) — the staff-facing linking/
  approval/fee-configuration surface, generic across every
  `AlternateChannelType`, not WhatsApp-specific. Sits between the Alternate
  Channels framework and back-office staff, the same role
  `RegisterController`/`AlternateChannelsController` play in the reference
  app. `Areas/WhatsAppBanking` depends on this, it does not reimplement it.
- **Alternate Channels framework** (existing, this repo, currently
  unexposed by any controller) — owns channel linking, per-channel fees,
  and (for the money-movement pieces) C2B/B2C request logging. Detailed
  in §3.

## 3. Why this is a new `AlternateChannelType`, not a bespoke design

**What already exists, and is reused as-is:**

- **Linking** (`AlternateChannel` aggregate) — a per-`CustomerAccount`
  record: `Type` (channel), `CardNumber` (MSISDN for a mobile-style
  channel), `MobilePIN`, `DailyLimit`, `IsThirdPartyNotified`/
  `ThirdPartyResponse`, `IsLocked`, `RecordStatus`. Every existing channel
  (Sacco Link, Sparrow, M-Co-op Cash, ...) requires a customer to be
  **linked** before they can transact on it — a real record with a real
  `RecordStatus` field, not implicit. **Correction, found building Piece A
  (§4): the *approval step* itself has no enforcement anywhere in the
  code** — `RecordStatus` is just a field anyone with `PUT`/Update access
  can set to anything, including `Approved`, in one call. Treat "approval"
  as a UI/process convention this phase can build on top of (§4), not a
  system-enforced gate that already exists.
- **Fees** (`AlternateChannelKnownChargeType`) — Linking, Replacement,
  Renewal, **Withdrawal, Deposit, Mini Statement, Balance Inquiry**,
  Airtime, **PIN Reset**, resolved per channel type via
  `IAlternateChannelAppService.FindCommissions(channelType, chargeType)`.
  Every fulfillment action below has a natural fee hook already built —
  no bespoke pricing model needed.
- **C2B deposit matching** (`MobileToBankRequest`) — real, working GL
  logic: given an M-Pesa-shaped Paybill confirmation (`MSISDN`,
  `BusinessShortCode`, `TransID`, `BillRefNumber`, `TransAmount`, ...), it
  matches to a `CustomerAccount` either by parsing an encoded account
  reference or **by `MSISDN` against the `AlternateChannel` link**
  (`FindAlternateChannelsByCardNumber` — this is exactly the WhatsApp
  linking record from above), then posts a real journal
  (Credit customer's product GL / Debit `MobileWalletC2BSettlement`).
  Unmatched requests queue for manual back-office reconciliation
  (`ReconcileMobileToBankRequest`/`AuditMobileToBankRequestReconciliation`).
- **B2C withdrawal queuing** (`BankToMobileRequest`) — a record shape +
  `IBrokerService.ProcessBankToMobileRequests` dispatch exists, intended
  to hand off to an outbound host process for the actual telco payout call.

**What's genuinely missing, confirmed by reading the code, not assumed:**

- No `WebApplication1` controller exists for **any** of this — same
  "fully built, WCF-only" shape as ChequeBook/UnPayReason before those got
  controllers. `AlternateChannelService.svc.cs`,
  `MobileToBankRequestService.svc.cs`, `BankToMobileRequestService.svc.cs`
  are the only current entry points. **See §4 — this splits into two
  separate, sequenced pieces of work, not one controller.**
- **No inbound REST webhook exists** for a provider (Safaricom or anyone)
  to actually call `AddNewMobileToBankRequest` with a live C2B
  confirmation. The matching/posting logic is real; nothing external can
  trigger it today.
- **`SwiftFinancials.BankToMobileHostInterface`** — the project that would
  make an outbound B2C call to actually pay a customer out — is an empty,
  unimplemented stub (checked directly: default `Class1.cs`, nothing
  else). `BankToMobileRequest` rows can be created and queued; nothing
  consumes that queue.
- ~~No existing `AlternateChannelType` covers WhatsApp~~ — **done**:
  `AlternateChannelType.WhatsAppBanking = 512` is added, with a matching
  `CheckAlternateChannelNumber` validation case (§4).

**Net effect on this design**: linking, fees, and deposit-matching reuse
real, existing logic — genuine leverage, not just a nicer diagram. The
inbound C2B webhook and the outbound B2C host are real, scoped backend
work this phase still needs to do; they are not solved by "just call the
existing service," because nothing external can reach it yet.

## 4. Sequencing — two controllers, not one

Prompted by a fair review question: is a dedicated `WhatsAppBankingController`
even the right shape, given the reference app never builds one controller
per channel type? For half of this work, it isn't — and that's why this is
two separate, sequenced pieces of work, not one.

**Piece A — generic `AlternateChannelController` (staff-facing, all channel
types, not WhatsApp-specific). Built.** The reference app never has a
per-channel-type controller for linking/approval/fee management — every
existing channel (Sacco Link, Sparrow, MCo-op Cash, SpotCash, Citius, Agency
Banking, PesaPepe, ABC Bank, Broker) flows through the *same* generic
controllers, keyed off `AlternateChannelDTO.Type`:
- `RegisterController` (`Linking`/`Create`/`Edit`/`Verify`/`Authorize`/
  `History`) — the staff-facing linking lifecycle.
- `AlternateChannelsController` / `AlternatechannelManagementController` —
  per-channel-type fee/commission configuration.

`WebApplication1/Areas/Accounts/Controllers/AlternateChannelController.cs`
(`docs/api/alternate-channel-api-spec.md`) adapts these generically, one
controller for every `AlternateChannelType` — link/update/replace/renew/
stop/delink, paged/filtered listing (including a checker-inbox-shaped
type+status query), and the type-scoped commissions sub-resource. WhatsApp
Banking is just one more `Type` value — `AlternateChannelType.
WhatsAppBanking = 512` and a matching `CheckAlternateChannelNumber`
validation case (E.164-shaped, same regex as `MCoopCash`/`SpotCash`/
`PesaPepe`) are **now both added** — no changes needed to this controller
for that, confirming the point above.

**Real findings from building it, not assumptions:**
- `RegisterController.Verify`/`Authorize` are bound to `DebitBatchDTO` and
  call `AuditDebitBatchAsync`/`AuthorizeDebitBatchAsync` — copy-pasted from
  a DebitBatch controller, nothing to do with `AlternateChannel` at all.
  There was never a real maker-checker action to adapt from these two
  reference actions specifically.
- **There is no real maker-checker gate for `AlternateChannel` linking
  anywhere in this codebase, at any layer** — not in the reference app
  (see above), not in `IAlternateChannelAppService`.
  `UpdateAlternateChannel` copies whatever `RecordStatus` the caller
  supplies straight onto the persisted record: no check that it's
  currently `New`/`Edited`, no maker-vs-checker identity check, nothing —
  unlike the Batch Procedures module's real `Audited`/`Authorized` guard
  clauses elsewhere in this same codebase. `AlternateChannelController`
  exposes `POST {id}/approve`/`{id}/reject` as a convenience over that same
  ungated `Update` call — it is explicitly **not** new enforcement; it
  cannot be, since the domain layer underneath has nothing to enforce
  against. If real maker-checker enforcement is wanted for channel linking
  (a fair thing to want for a financial channel), it needs new domain work,
  not just a route — this is now a confirmed, scoped gap, not a hedge.
- `AlternateChannelDTO.CheckAlternateChannelNumber` (the `CardNumber`
  validator) unconditionally blanks `CardNumber` for
  `AlternateChannelType.AgencyBanking`/`.Citius` — linking can never
  succeed for either type today, a pre-existing bug independent of this
  work. Still flagged, not fixed (no spec states the intended format for
  either) — unrelated to WhatsApp Banking, left alone. **Open question 1
  (§9) is done**: `AlternateChannelType.WhatsAppBanking = 512` is added,
  and — unlike `AgencyBanking`/`Citius` — it has a real validation case
  (the same E.164-shaped regex `MCoopCash`/`SpotCash`/`PesaPepe` already
  use, plus the matching `MaskedCardNumber` masking-style case), so it
  doesn't inherit their bug.

**Piece B — `Areas/WhatsAppBanking` (customer/bot-facing, WhatsApp-specific,
no reference-app precedent). Not started.** OTP request/verify, PIN
link/authenticate/reset, and the fulfillment endpoints (§5, §7). This is
genuinely new surface: every existing channel in the reference app is
staff-linked at a branch, so there's no self-service/bot/session concept
anywhere to adapt from — nothing to port, unlike Piece A. It calls Piece
A's underlying app service (`IAlternateChannelAppService`) rather than
duplicating linking/approval logic — it does not reimplement
`RecordStatus`/`IsLocked` handling, it consumes it, including consuming the
fact that "approval" is currently just a status write.

**Dependency direction, revised now that Piece A is real**: Piece A no
longer blocks Piece B on "does a checker screen exist" — it does now. It
blocks Piece B on a sharper question: **is an ungated status flip an
acceptable approval mechanism for a financial channel a customer links to
themselves over WhatsApp?** §5's sequence diagram's "back office approves"
step is real and callable today, but "approves" currently means "any staff
account with API access called `POST {id}/approve`," not "a second,
different staff member verified a first staff member's linking decision."
Whether that's fine as a v1 or needs real maker-checker domain work first
is a product/security call (open question 2, §9), not an engineering
default either way.

## 5. Identity — phone verification (OTP) + channel PIN (linking)

Two different mechanisms, used for two different things — this is the main
change from the previous draft, which used OTP for *every* session:

- **OTP** proves momentary control of a phone number. Used only at
  **linking time** (once) and for **PIN reset** (rare) — not on every
  chat session. Delivered by SMS via the existing `ITextAlertAppService`
  (`docs/api/textalert-api-spec.md`), same as before.
- **`MobilePIN`** (set once, during linking) is what authenticates
  **every subsequent transacting session** — the same convention every
  other `AlternateChannel` in this system already uses (this is precisely
  what the field is for). The bot prompts for it at the start of a
  balance/deposit/withdraw flow, not a fresh OTP each time.
  **Correction, found building Piece A (§4)**: `MobilePIN`/`NewMobilePIN`/
  `ResetMobilePIN` exist on `AlternateChannelDTO`, but **no method on
  `IAlternateChannelAppService` reads or persists any of them** —
  `AddNewAlternateChannel` doesn't accept a PIN at all, and there is no
  PIN-set/PIN-reset call anywhere in the app service. The fields being on
  the DTO is not evidence this works — same "a DTO's fields aren't proof
  of behavior" lesson `JournalVoucherController` ran into. This is real,
  scoped Piece B backend work (a PIN-set call has to be added to
  `IAlternateChannelAppService`/`AlternateChannelAppService`, likely
  alongside `AddNewAlternateChannel`), not something Piece B can just call.

```mermaid
sequenceDiagram
    participant C as Customer (WhatsApp)
    participant B as Bot
    participant A as WhatsApp Banking API

    Note over C,A: One-time linking
    C->>B: "I want to link this number"
    B->>A: POST /otp/request { phoneNumber }
    A-->>C: SMS OTP
    C->>B: OTP code
    B->>A: POST /otp/verify { phoneNumber, otp }
    A-->>B: { phoneVerifiedToken, isExistingCustomer, accounts? }
    B-->>C: "Choose a 4-digit PIN for WhatsApp Banking"
    C->>B: PIN
    B->>A: POST /link { phoneVerifiedToken, accountId, pin }
    A-->>B: AlternateChannel created, RecordStatus: New (pending approval)
    B-->>C: "Submitted — we'll confirm once approved"
    Note over A: Back office approves via Piece A's POST {id}/approve (§4 — built, but an ungated status flip, not a real maker-checker gate)

    Note over C,A: Every later session
    C->>B: Any message
    B-->>C: "Enter your WhatsApp Banking PIN"
    C->>B: PIN
    B->>A: POST /pin/authenticate { phoneNumber, pin }
    A-->>B: { sessionToken } (only if AlternateChannel.RecordStatus == Approved and not IsLocked)
    B-->>C: Main menu (§7)
```

- Linking is **self-service-initiated but not self-service-approved** —
  consistent with every existing channel, none of which let a customer
  activate themselves. This adds latency (§9) that in-branch linking
  doesn't have; that's a deliberate trade for not requiring a branch visit,
  not an oversight.
- `isExistingCustomer: false` at OTP-verify time routes into onboarding
  (§6) before linking is even possible — `AlternateChannel` is per-account,
  so a brand-new WhatsApp user needs an account to link *to* first.
- PIN storage/hashing, retry-lockout behavior (`AlternateChannel.IsLocked`
  already exists for this — how it gets set isn't designed here), and
  `DailyLimit` sourcing (customer-chosen vs. back-office-configured
  default) are open items — §9.

## 6. Flow: new customer — onboarding, then linking

```mermaid
flowchart TD
    A["OTP verified, isExistingCustomer: false"] --> B["Bot collects minimal KYC:\nfirst name, last name,\nID type + number, DOB, gender"]
    B --> C["POST /customer\n(WhatsApp Banking API)"]
    C --> D["Creates CustomerDTO (Individual),\nbranchId = Digital Channel Branch,\nvia existing Customer API"]
    D --> E["Bulk-creates mandatory savings\naccount(s) — existing bulk-create\nendpoint, customer-accounts-api-spec.md §4.5"]
    E --> F["Bot: 'Now let's link this\nnumber for WhatsApp Banking'"]
    F --> G["Linking flow (§5)"]
    G --> H["RecordStatus: New — pending approval"]
```

Unchanged from the previous draft except it now flows straight into §5's
linking step rather than ending at "you're set up" — a new customer isn't
actually able to transact over WhatsApp until linking is approved, same as
an existing customer.

**Digital Channel Branch is still needed** — `AlternateChannel` doesn't
solve *where* a self-onboarded customer's `CustomerDTO.branchId` comes
from; that's a separate, still-open back-office setup dependency (§9),
unrelated to the Alternate Channels framework.

**KYC document capture** is still not designed here — unchanged open item.

## 7. Flow: returning, linked customer — balance, deposit, withdrawal

```mermaid
flowchart TD
    A["PIN authenticated,\nsessionToken issued"] --> B{"Main menu"}
    B -->|"1. Balance"| C["GET /accounts/{id}/balance\n(charges BalanceInquiryCharges fee, if configured)"]
    B -->|"2. Deposit"| D["Bot: 'Pay via [Paybill number],\naccount = your linked phone number'"]
    B -->|"3. Withdraw"| E["Bot asks: which account, amount"]
    D --> F["Customer pays via mobile money\n(outside this system)"]
    F --> G["Provider sends C2B confirmation\nto a NEW inbound webhook (§3, §9 — not built yet)"]
    G --> H["MobileToBankRequestAppService matches\nby MSISDN → AlternateChannel → CustomerAccount,\nposts journal, charges DepositCharges fee"]
    E --> I["POST /withdrawals\n{ accountId, amount }"]
    I --> J["Creates BankToMobileRequest\n(existing shape) + debits the account"]
    J --> K["Outbound payout — host interface\nis unimplemented (§3, §9) — NOT live"]
```

- **Fees**: balance, deposit, and withdrawal each look up
  `IAlternateChannelAppService.FindCachedCommissions(AlternateChannelType.WhatsAppBanking,
  chargeType)` (`BalanceInquiryCharges`/`DepositCharges`/
  `WithdrawalCharges`) and apply the resulting `Commission` the same way
  every other channel's transactions already do — not a new pricing
  concept, just a new lookup key.
- **Deposit is customer-funds-first, confirmation-driven** — the bot does
  not itself move money; it tells the customer how to pay (Paybill number
  + their linked phone as the account reference) and the *existing*
  `MobileToBankRequestAppService` matching/posting logic takes over once a
  confirmation arrives. **The inbound webhook that would receive that
  confirmation does not exist yet** — real, scoped work, not a design gap
  this document can resolve by itself. Until it's built, deposit
  can't go beyond "tell the customer how to pay" for a live provider.
- **Withdrawal creates a real `BankToMobileRequest`**, which is the
  correct domain shape, but **the process that actually pays the customer
  out doesn't exist** (`SwiftFinancials.BankToMobileHostInterface` is an
  empty stub). Until it's built, a withdrawal can debit the account and
  queue the request, but nothing disburses — this needs either that host
  built, or an interim manual/agent-redemption fallback, before this flow
  can go live. Flagged prominently, not glossed over.
- Debit posting for a withdrawal (the account-side leg,
  before the payout leg) can reuse `POST /api/frontoffice/requests`
  (`CashWithdrawal`) the same way the earlier draft proposed, if a faster
  interim "ledger moves, payout is manual" path is wanted while the real
  B2C host is built — a phasing decision, not designed further here.

## 8. Roadmap — Phase 2 (not designed yet)

Unchanged from the previous draft: loan application over this same channel
is the natural next step once customer/account/linking/deposit/withdrawal
is live and stable, but needs its own audit of the loan/underwriting domain
first — not part of this document.

## 9. Open design questions — confirm before implementation

1. ~~`AlternateChannelType.WhatsAppBanking` needs to actually be added~~ —
   **done** (§4): the enum value (`512`) and a real
   `CheckAlternateChannelNumber` validation case (E.164-shaped, same regex
   as `MCoopCash`/`SpotCash`/`PesaPepe`) are both in. Confirmed Piece A's
   `AlternateChannelController` needed zero changes for the new type — it
   was already generic, as designed.
2. **Self-service linking approval mechanism, not just latency** — Piece A
   (§4) is built and callable today, so the question isn't "does an
   approval screen exist" anymore, it's whether an **ungated** status flip
   (any staff account with API access can call `POST {id}/approve`, with
   no check the record is currently `New`, no maker-vs-checker identity
   check) is acceptable for a channel a customer links to themselves over
   WhatsApp — or whether real maker-checker domain work is a prerequisite.
   Separately, whatever the answer: what's an acceptable turnaround time
   for a customer waiting on WhatsApp? Both need a product/security answer.
3. **Inbound C2B webhook** — building a REST endpoint that a mobile money
   provider can call with a live payment confirmation, mapped to
   `MobileToBankRequestAppService.AddNewMobileToBankRequest`. This is the
   single highest-leverage piece of real backend work identified here —
   it's the deposit story not just for WhatsApp but for every channel that
   wants C2B deposits.
4. **Outbound B2C host** — `SwiftFinancials.BankToMobileHostInterface`
   needs an actual implementation (or a decision to route B2C payouts some
   other way) before self-service withdrawal payout works at all.
5. **PIN vs. OTP threat model** — is a 4-ish digit `MobilePIN`,
   authenticated over a bot conversation, an acceptable step-down from
   OTP-per-session for a financial channel? What's the lockout policy
   behind `AlternateChannel.IsLocked`, and is there a PIN-reset flow
   (`PINResetCharges` fee already exists as a concept) designed anywhere
   yet? Not resolved here — needs security/compliance input. **Sharper now
   (§4/§5): `MobilePIN`/`NewMobilePIN`/`ResetMobilePIN` exist on the DTO
   but nothing in `IAlternateChannelAppService` sets or reads any of them
   — PIN storage/hashing/lockout has to be designed and built from scratch,
   it isn't "wire up the existing field."**
6. **`DailyLimit` sourcing** — customer-requested (up to a back-office
   ceiling) or fixed by back office entirely? Existing channels don't
   answer this generically; check how Sacco Link/Sparrow linking currently
   sets it, or decide fresh for this channel.
7. **Digital Channel Branch** (customer/account creation, still separate
   from the Alternate Channels framework) — unchanged open item from the
   previous draft.
8. **KYC document capture** — unchanged open item.
9. **Compliance/AML** — unchanged open item, now sharper: this system's
   only precedent for channel access is staff-vetted, in-branch linking.
   A self-service-initiated (even if approval-gated) linking flow is a
   real departure from that precedent and needs sign-off, not an
   engineering default.
