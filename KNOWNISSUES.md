# Known Issues

Latent bugs and bug *patterns* worth knowing about — either already fixed
somewhere and worth watching for elsewhere, or deliberately left in a broken
state and tracked here so it isn't forgotten before shipping.

## `Enum.IsDefined` type mismatch on byte-backed enum properties

**Status:** all currently-known occurrences fixed. Documented here as a
pattern to avoid reintroducing, not as an open bug.

`Enum.IsDefined(Type, object)` requires the *boxed type* of the second
argument to exactly match the enum's underlying type (`int` by default,
unless the enum is explicitly declared `: byte`/`: short`/etc.). Passing a
boxed `byte` against an `int`-backed enum throws at runtime:

```
System.ArgumentException: Enum underlying type and the object must be same
type or object must be a String. Type passed in was 'System.Byte'; the enum
underlying type was 'System.Int32'.
```

This surfaced because several DTO description-getters guard a cast with
`Enum.IsDefined` (to avoid throwing on out-of-range stored values) but the
backing property is `byte` (stored compactly), while the enum itself is a
plain `int`-backed enum — so the guard itself threw on every call.

**Fixed:**
- `Application.MainBoundedContext.DTO/AccountsModule/CustomerAccountDTO.cs`
  — 8 properties (`CustomerType`, `CustomerIndividualSalutation`,
  `CustomerIndividualGender`, `CustomerIndividualMaritalStatus`,
  `CustomerIndividualIdentityCardType`, `CustomerIndividualNationality`,
  `CustomerIndividualType`, `CustomerIndividualClassification`).
- `Application.MainBoundedContext.DTO/AccountsModule/BrokerRequestDTO.cs`
  and `BrokerRequestBindingModel.cs` — `Status` property
  (`BrokerRequestStatus`).

Fix applied in each case: cast the property to `(int)` before the
`Enum.IsDefined` call, e.g.
`Enum.IsDefined(typeof(CustomerType), (int)CustomerType)`.

**Audit scope:** the bug pattern was isolated to
`Application.MainBoundedContext.DTO` — that's the one project where option/
status values are stored as `byte` for compactness. A full sweep of every
other project (`Application.MainBoundedContext`,
`Infrastructure.Data.MainBoundedContext`, `DistributedServices.MainBoundedContext`,
`WebApplication1` and its API mirrors, presentation/shared projects, SQLCLR)
found zero occurrences — everywhere else, option/status values are passed
around as plain `int`.

**If you add a new byte-backed enum property with a `Description`-style
getter guarded by `Enum.IsDefined`, cast to `(int)` before the call** — this
is the mistake to not repeat.

## Maker-checker approval guard disabled (`WorkflowAppService`)

**Status:** open — must be re-enabled before shipping or before any real
maker-checker enforcement is relied on.

`Application.MainBoundedContext/AdministrationModule/Services/WorkflowAppService.cs`,
`ApproveWorkflowItem` (marked `// TODO(maker-checker)`): the guard requiring
the workflow item's approver to be a different user from its initiator is
currently commented out. Right now the same user can create *and* approve
their own workflow item — the sequential maker-checker control is not
actually enforced.

This was disabled deliberately during hands-on testing (no separate
maker/checker test accounts were provisioned at the time). The check itself
was not deleted, just commented out — re-enabling is a one-line uncomment of
the `if (IsUserLatestApproverOfWorkflowItemEntry(...))` guard immediately
above the `TODO(maker-checker)` comment.
