# Cheque Type — Functional Requirements & Implementation Notes

Audience: anyone configuring cheque types, building a cheque-deposit screen,
or wiring new behavior around cheque clearance/commissions. This is a
*functional* guide — what `ChequeType` is supposed to mean, and how much of
that meaning is enforced by the code. For the request/response contract, see
`docs/api/cheque-type-api-spec.md`.

Source of truth for everything below: `Domain.MainBoundedContext/AccountsModule/Aggregates/ChequeTypeAgg/`,
`ChequeTypeCommissionAgg/`, `ChequeTypeAttachedProductAgg/`,
`Application.MainBoundedContext/AccountsModule/Services/ChequeTypeAppService.cs`,
`Application.MainBoundedContext/FrontOfficeModule/Services/ExternalChequeAppService.cs`,
`Application.MainBoundedContext/FrontOfficeModule/Services/InHouseChequeAppService.cs`,
and `WebApplication1/Areas/FrontOffice/Controllers/CashDepositController.cs`.
Every claim here was checked against that code, including the reference MVC
app it was originally ported from — not assumed, and not taken from either
app's naming.

**Status:** this doc originally documented four gaps between what
`ChequeType`'s fields imply and what the code actually did. All four have
since been implemented (§3). §1–2 (what a cheque type is, why it exists)
are unchanged background; §3 now describes the real behavior plus the
deliberate scope limits chosen when closing each gap.

## 1. What a `ChequeType` actually is

A tiny admin-configurable master-data record — four fields, no invariants
beyond a lock flag:

```csharp
// Domain.MainBoundedContext/AccountsModule/Aggregates/ChequeTypeAgg/ChequeType.cs
public class ChequeType : Entity
{
    public string Description { get; set; }      // e.g. "Standard Cheque", "Bankers Cheque"
    public int MaturityPeriod { get; set; }       // days until clearance
    public int ChargeRecoveryMode { get; set; }   // when its commission is charged
    public bool IsLocked { get; private set; }
}
```

Think of it as a *classification tag* you put on an incoming (`ExternalCheque`,
deposited by a customer) or outgoing (`InHouseCheque`, written by the SACCO)
cheque — "this is a Bankers Cheque" vs. "this is a Standard Cheque" — plus
two admin-only "attachments" describing what that classification implies
(§2). Both `ExternalCheque` and `InHouseCheque` carry a nullable
`ChequeTypeId` FK; `TruncatedCheque` (the national image-clearing record)
does not reference `ChequeType` at all — it's a different concern
(clearing-house plumbing, not classification).

## 2. Why it's necessary at cheque-deposit time

The two things a cheque type drives:

1. **Which commission gets charged, and when.** A `ChequeTypeCommission`
   join row links a `ChequeType` to one or more `Commission` records.
   `ChargeRecoveryMode` (`ChequeTypeChargeRecoveryMode`: `0`=`OnChequeDeposit`,
   `1`=`OnChequeClearance`) says *when* that commission fires — immediately
   when the teller accepts the cheque, or later when it actually clears.
2. **Which products a cheque of this type is allowed to fund.** A
   `ChequeTypeAttachedProduct` join row links a `ChequeType` to specific loan
   and/or investment products (`ChequeTypeAttachedProductAgg/`) — e.g. "a
   Bankers Cheque can be used to pay down these particular loan products."

Both attachments are configured once, per cheque type, by an admin — via the
three-step wizard in the reference MVC screen, or the single
`POST /api/accounts/chequetypes` body in this codebase's
`ChequeTypeController` (see `docs/api/cheque-type-api-spec.md` §5.4). A
teller never sets either of these — they only ever *pick* a cheque type by
name at deposit time.

`MaturityPeriod` says how many days a cheque of this type typically takes to
clear (e.g. local cheques 3 days, out-of-town cheques 7 days), used to
derive the cheque's own `MaturityDate` when it's deposited (§3.1).

## 3. What's implemented, and the scope decisions behind each piece

### 3.1 `MaturityDate` — now always server-derived from `MaturityPeriod`

`ExternalChequeAppService.AddNewExternalCheque` now looks up the chosen
`ChequeType` (via `IChequeTypeAppService.FindChequeType`) before creating
the `ExternalCheque`, and computes:

```csharp
var maturityDate = DateTime.Today.AddDays(chequeType?.MaturityPeriod ?? 0);
```

This is passed to `ExternalChequeFactory.CreateExternalCheque(...)` in place
of whatever `ExternalChequeDTO.MaturityDate` the caller sent — **the value
is always server-computed, callers can't override it.** No cheque type
selected (or one with `MaturityPeriod = 0`) matures the same day it's
deposited. This also closes what was previously a live bug:
`CashDepositController.cs`'s cheque-deposit path never set
`NewExternalCheque.MaturityDate` at all, so every deposited cheque
persisted `MaturityDate = 0001-01-01`. That call site needed no change —
the fix lives entirely in the app service, so it applies to every caller
(the new API controller and the legacy WCF passthrough alike).

### 3.2 `ChargeRecoveryMode` — both values now have a real code path

`OnChequeClearance` already worked (narrowly — savings-account `Pay` only,
in `ClearExternalCheque`) before this round of changes and is untouched.

`OnChequeDeposit` is now implemented, mirroring that same pattern —
`ComputeTariffsByChequeType` + one journal per tariff — triggered instead
from `AddNewExternalCheque` right after the cheque is saved:

```csharp
if (chequeType != null && chequeType.ChargeRecoveryMode == (int)ChequeTypeChargeRecoveryMode.OnChequeDeposit)
{
    ChargeChequeTypeCommissionOnDeposit(chequeType, externalChequeDTO, serviceHeader);
}
```

Differences from the clearance branch, deliberate given deposit has neither
a Pay/UnPay nor a Savings/Loan/Investment distinction to key off:
- Applies regardless of the depositing account's product type (clearance's
  `OnChequeClearance` branch only fires for `ProductCode.Savings`).
- Journal uses a new transaction code, `SystemTransactionCode.ExternalChequeDepositCharge`
  (`= 92`), instead of reusing `ExternalChequeClearance` — so a deposit-time
  charge and a clearance-time charge are distinguishable in reports; they
  should never both apply to the same cheque (`OnChequeDeposit` and
  `OnChequeClearance` are mutually exclusive values of the same field).
- Branch for the journal: `externalChequeDTO.TellerEmployeeBranchId` if the
  caller populated it, else the payee's own `CustomerAccountDTO.BranchId` as
  a fallback (`CashDepositController`'s hand-built `ExternalChequeDTO` never
  sets `TellerEmployeeBranchId`, so this flow relies on the fallback today).
- If the cheque type has no attached commissions, or the customer is exempt
  from all of them, nothing posts — same "no tariffs, no journal" behavior
  as the clearance branch.

`InHouseCheque` is unaffected — it still charges unconditionally at write
time regardless of `ChargeRecoveryMode`, which was intentionally left as-is
(out of scope: nothing about deposit-side `ExternalCheque` commissions
implies in-house cheque behavior should change).

### 3.3 Attached products — now enforced, conservatively, on `ExternalChequePayable`

`ExternalChequeAppService.UpdateExternalChequePayables` — where a cheque's
payee accounts (which loan/investment products its proceeds should recover
against, per `RecoverAttachedLoans`/`RecoverAttachedInvestments` at
clearance) get set — now validates each payee against the cheque type's
configured `ChequeTypeAttachedProduct` list before saving:

```csharp
if (!ValidateChequePayablesAgainstAttachedProducts(externalCheque, externalChequePayables, serviceHeader))
    return false;
```

**Scope, chosen deliberately conservative:**
- **Opt-in only.** A cheque type with *no* attached products configured
  stays fully unrestricted — this can't retroactively break any existing
  cheque type nobody has configured this for. Restriction only kicks in
  once an admin has attached at least one loan or investment product to a
  cheque type.
- **The cheque's own deposit account is exempt.** `CashDepositController`'s
  cheque-deposit path always calls `UpdateExternalChequePayables` with a
  single payable pointing at the *same* account the cheque was deposited
  into (not a teller-chosen recovery target — see the code around
  `NewExternalCheque`/`ExternalChequePayables` in that controller). That
  entry is excluded from the check by design; it isn't a "which product
  should this cheque fund" selection, so it shouldn't be constrained by
  one. **In practice, this means the check currently never fires** — this
  codebase has no screen where a teller picks a *different* loan/investment
  payee for a cheque (the only other consumer is the legacy WCF passthrough,
  `ExternalChequeService.svc.cs`, which forwards whatever the old MVC app
  sent). It's real enforcement infrastructure, ready for whenever a genuine
  payee-picker gets built, not something with an active effect on today's
  callers.
- Validation failure returns `false` from `UpdateExternalChequePayables`
  (same contract as before — no new exception type, no message channel).
  `CashDepositController`'s one call site still doesn't check this return
  value, but per the point above it can never actually receive `false` from
  this new check under the current flow.

### 3.4 `IsLocked` — intentionally left as-is

Still never read outside `ChequeTypeAppService`. Out of scope for this
round — locking behavior wasn't part of the three items above, and there
was no clear signal for what "reject a locked cheque type" should look like
(hard block at deposit? at admin-selection time only?) without guessing at
a UX flow that doesn't exist yet.

## 4. Summary table

| Field | Intent | Status |
|---|---|---|
| `Description` | Human label, picked by teller at deposit | Fully wired (drives the typeahead/dropdown) — unchanged. |
| `MaturityPeriod` | Days until clearance; derives the cheque's `MaturityDate` | **Implemented** — `AddNewExternalCheque` always computes `MaturityDate` from it server-side (§3.1). |
| `ChargeRecoveryMode` | Choose whether the cheque type's commission is charged at deposit or at clearance | **Implemented** — both `OnChequeDeposit` (new) and `OnChequeClearance` (pre-existing, savings/Pay-only) now have real code paths (§3.2). `InHouseCheque` still ignores this flag by design (always charges at write time). |
| Attached commissions (`ChequeTypeCommission`) | Which `Commission`(s) apply to this cheque type | Read by `ComputeTariffsByChequeType`, called from both deposit and clearance now. |
| Attached products (`ChequeTypeAttachedProduct`) | Which loan/investment products a cheque of this type may fund | **Implemented, opt-in** — enforced in `UpdateExternalChequePayables` when a cheque type has products attached; unrestricted otherwise. No current caller triggers it (§3.3). |
| `IsLocked` | Should presumably block use of a locked cheque type | Not implemented — out of scope this round (§3.4). |

## 5. If you're picking this up next

- **`IsLocked` enforcement** (§3.4) is the one remaining gap from the
  original analysis. Needs a UX decision first: block cheque-type selection
  entirely once locked, or just block new deposits/writes against an
  already-selected locked type?
- **A real payee-picker for `ExternalChequePayable`** would make §3.3's
  enforcement actually observable — right now it's inert infrastructure
  because the only caller always self-selects the deposit account.
- If the business ever wants `OnChequeDeposit` and `OnChequeClearance` to
  **both** apply to different portions of a cheque's commission (rather
  than being mutually exclusive per cheque type), that's a bigger change to
  `ChequeTypeChargeRecoveryMode` itself, not a follow-up to this pass.
