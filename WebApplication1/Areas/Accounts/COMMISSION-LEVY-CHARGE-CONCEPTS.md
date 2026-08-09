# Commission, Levy, Charge, and DynamicCharge — How They Actually Relate

Audience: anyone touching fee/tariff configuration or wondering whether
"commission," "charge," and "levy" are three names for the same thing in
this codebase. They aren't — this doc traces the real relationships in the
domain model, not just the naming.

Source of truth: `Domain.MainBoundedContext/AccountsModule/Aggregates/CommissionAgg/`,
`LevyAgg/`, `LevySplitAgg/`, `CommissionLevyAgg/`, `GraduatedScaleAgg/`,
`DynamicChargeAgg/`, `DynamicChargeCommissionAgg/`,
`Domain.MainBoundedContext/ValueObjects/Charge.cs`, and
`Application.MainBoundedContext/AccountsModule/Services/CommissionAppService.cs`
(the tariff-computation logic, e.g. `ComputeTariffsBySystemTransactionType`
around line 895-880, `ComputeTariffsByChequeType` per
`CHEQUE-TYPE-FUNCTIONAL-REQUIREMENTS.md`).

## 1. `Commission` — the primary tariff record

```csharp
// Domain.MainBoundedContext/AccountsModule/Aggregates/CommissionAgg/Commission.cs
public class Commission : Entity
{
    public string Description { get; set; }
    public decimal MaximumCharge { get; set; }
    public byte RoundingType { get; set; }
    public bool IsLocked { get; private set; }
}
```

The entity itself is small. Its actual rate structure lives in a separate
one-to-many aggregate, `GraduatedScale`:

```csharp
// Domain.MainBoundedContext/AccountsModule/Aggregates/GraduatedScaleAgg/GraduatedScale.cs
public class GraduatedScale : Entity
{
    public Guid CommissionId { get; set; }
    public virtual Range Range { get; set; }     // amount bracket this rate applies to
    public virtual Charge Charge { get; set; }   // how to compute the rate — see §3
}
```

A commission can have several `GraduatedScale` rows, each covering a
different transaction-amount bracket with its own rate (e.g. 0–1000 KES
flat fee, 1000–50000 a percentage, capped by `Commission.MaximumCharge`).

`Commission` is the universal fee building block almost every other module
attaches to via its own join aggregate: `ChequeTypeCommission`,
`SavingsProductCommission`, `LoanProductCommission`, `CreditTypeCommission`,
`DebitTypeCommission`, `AlternateChannelTypeCommission`,
`WireTransferTypeCommission`, `UnPayReasonCommission`,
`TextAlertCommission`, `SystemTransactionTypeInCommission`. Whatever the
join, the pattern is identical: `<X>Id` + `CommissionId`, no other data —
purely "this Commission applies to this X."

## 2. `Levy` — real, but subordinate to Commission, not a peer

```csharp
// Domain.MainBoundedContext/AccountsModule/Aggregates/LevyAgg/Levy.cs
public class Levy : Entity
{
    public string Description { get; set; }
    public virtual Charge Charge { get; set; }   // same value object Commission's scales use
    public bool IsLocked { get; private set; }
}
```

`Levy` is a genuinely separate aggregate — but it only ever attaches via
`CommissionLevy` (a `CommissionId` + `LevyId` join, same shape as every
other `<X>Commission` join above, just inverted), and critically, **a
levy's amount is computed as a percentage/fixed-amount of the commission's
own leviable charge — never of the raw transaction value directly**:

```csharp
// Application.MainBoundedContext/AccountsModule/Services/CommissionAppService.cs (~837-847)
switch ((ChargeType)levy.ChargeType)
{
    case ChargeType.Percentage:
        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
        break;
    case ChargeType.FixedAmount:
        levyCharges = levy.ChargeFixedAmount;
        break;
}
```

`leviableCommissionCharges` is the sum of whichever `CommissionSplit`
portions were flagged `Leviable` — so a levy is a charge-on-a-charge (think
VAT/excise duty on a fee), computed after the commission itself is
resolved, not an independent fee a caller can charge on its own. There is
no code path anywhere that computes a `Levy` amount without a `Commission`
already in scope.

Once computed, a levy amount can itself be split across multiple GL
accounts via `LevySplit` (`LevyId` + `ChartOfAccountId` + `Percentage` +
`Description`) — the same "split one computed amount across several
accounts" pattern `CommissionSplit` uses for the commission amount itself.

**Practical shape, top to bottom:**

```
Commission ──< GraduatedScale (rate by amount bracket)
     │
     ├──< CommissionSplit (how the computed commission amount is divided across GL accounts;
     │                      only the portions marked Leviable feed the levy calculation)
     │
     └──< CommissionLevy >── Levy ──< LevySplit (how the computed levy amount is divided across GL accounts)
```

## 3. `Charge` — not a third fee type; it's the shared rate-calculation shape

```csharp
// Domain.MainBoundedContext/ValueObjects/Charge.cs
public class Charge : ValueObject<Charge>
{
    public byte Type { get; private set; }       // ChargeType: Percentage or FixedAmount
    public double Percentage { get; private set; }
    public decimal FixedAmount { get; private set; }
}
```

There is no `Charge` *aggregate*. `Charge` is a plain value object — "how do
I compute this rate" — reused by both `GraduatedScale.Charge` (a
commission's rate in a given bracket) and `Levy.Charge` (a levy's rate). It
answers *how a number is computed*, not *what the fee is for*.

Separately, "charge" gets reused as informal vocabulary **for Commission
itself** throughout the codebase, which is the actual source of confusion:
`CommissionDTO.ChargeType`/`ChargeBenefactor`/`ChargeBasisValue`/
`KnownChargeType` are all classification fields *on a commission*, not a
different entity. The reference MVC `ChequeTypeController`'s
`Session["selectedCharges"]` variable is literally typed
`ObservableCollection<CommissionDTO>`. If you see "charge" in a variable
name or DTO field, check whether it's the value object, or just informal
naming for a `Commission` — it's usually the latter.

## 4. `DynamicCharge` — a genuinely different, fourth concept

Easy to conflate with `Commission` from the name alone, but the fields say
otherwise:

```csharp
// Domain.MainBoundedContext/AccountsModule/Aggregates/DynamicChargeAgg/DynamicCharge.cs
public class DynamicCharge : Entity
{
    public string Description { get; set; }
    public short RecoveryMode { get; set; }
    public short RecoverySource { get; set; }
    public short Installments { get; set; }
    public short InstallmentsBasisValue { get; set; }
    public bool FactorInLoanTerm { get; set; }
    public bool ComputeChargeOnTopUp { get; set; }
    public bool IsLocked { get; private set; }
}
```

`RecoveryMode`/`RecoverySource`/`Installments`/`FactorInLoanTerm` model a
charge that's **recovered progressively across a loan's repayment
installments** (e.g. an insurance premium or processing fee spread over the
loan term) — a fundamentally different charging mechanic from Commission's
typically-immediate, transaction-time charge. `Commission` has no
equivalent fields; this isn't "Commission plus extras."

It links to `Commission` via `DynamicChargeCommission` (`DynamicChargeId` +
`CommissionId`) and to `LoanProduct` via `LoanProductDynamicChargeAgg` — so
a `DynamicCharge` can reference a `Commission`'s rate/GL structure while
adding its own installment-recovery mechanics on top. It is not simply
another name for Commission, and not a peer of Levy either (Levy rides on a
commission's *amount*; DynamicCharge changes *when/how* a charge is
recovered).

## 5. Summary

| Concept | Real aggregate? | Relationship |
|---|---|---|
| `Commission` | Yes | The primary fee record everything else attaches to. Rate comes from `GraduatedScale` (amount-bracket + `Charge`). |
| `Levy` | Yes | Subordinate to Commission via `CommissionLevy`. Amount is always a percentage/fixed-amount of the *commission's* leviable charge — never independent. |
| `Charge` | **No** — value object | Shared "percentage or fixed amount" rate shape used by both `GraduatedScale` and `Levy`. Also used informally as a synonym for "commission" in naming (`CommissionDTO.ChargeType`, `selectedCharges`) — not a distinct fee type. |
| `DynamicCharge` | Yes | A fourth, separate concept: installment-recovered loan charges, linked to `Commission` via `DynamicChargeCommission` but with its own recovery mechanics `Commission` doesn't have. |
