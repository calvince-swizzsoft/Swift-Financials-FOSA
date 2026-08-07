# Chart of Accounts — Design & Usage Guidelines

Audience: anyone setting up or maintaining the chart of accounts (back-office
admin, or a developer wiring a new feature that needs a G/L account to post
to). This is a *functional* guide — what the structure means and how to use
it correctly. For the request/response contract, see
`docs/api/chartofaccount-api-spec.md`.

Source of truth for everything below:
`Domain.MainBoundedContext/AccountsModule/Aggregates/ChartOfAccountAgg/`,
`Application.MainBoundedContext/AccountsModule/Services/ChartOfAccountAppService.cs`.
Every claim here was checked against that code, not assumed — including the
ones that say a field *doesn't* do what its name implies.

## 1. Why this is confusing at first glance

The chart of accounts looks like it should be a strict, self-enforcing tree
with postable/non-postable rules, account-type-scoped numbering, and
behavioral flags that gate what can happen to an account. **It isn't.** The
`ChartOfAccount` entity is a plain data bag — `Domain.MainBoundedContext/.../ChartOfAccount.cs`
has no invariants beyond "you can set a parent" and "you can lock/unlock
it." Almost everything that looks like a rule is actually a **convention**
the people entering data have to follow themselves; the system won't stop
you from breaking it. That's the actual source of confusion, and it's the
main thing this doc exists to make explicit.

## 2. The hierarchy

Every account optionally has a `ParentId`, forming a tree. Two things about
this tree are real, enforced behavior — not convention:

- **Root accounts** (`ParentId` empty) choose their own `AccountType`
  (Asset/Liability/Equity/Income/Expense — see §3).
- **Child accounts inherit `AccountType` from their parent, always** —
  whatever `AccountType` you send when creating a child is silently
  discarded (`ChartOfAccountFactory.CreateChartOfAccount(ChartOfAccount
  parent, ...)` sets `AccountType = parent.AccountType`, full stop). You
  cannot mix types within one branch of the tree.

`Depth` (0 = root, 1 = child of a root, 2 = grandchild, ...) is **not**
stored or maintained on write. It only exists as a computed value inside
`GET /tree` (`FindGeneralLedgerAccounts`), which walks the parent chain at
read time. The flat list/get-by-id endpoints always return `Depth: 0` and
empty `Children` — don't build tree UI off them.

## 3. Account Type and the numbering convention

Five types, fixed by the enum (`Infrastructure.Crosscutting.Framework/Utils/Enumerations.cs`):

| Type | Value |
|---|---|
| Asset | 1000 |
| Liability | 2000 |
| Equity/Capital | 3000 |
| Income/Revenue | 4000 |
| Expense | 5000 |

The display name every screen builds (`"{Type}-{Code} {Name}"`, e.g.
`1000-1001 Vault Cash`) only reads sensibly if `AccountCode`'s leading
digit matches the type it belongs under (1xxx under Asset, 2xxx under
Liability, ...). **This is a numbering convention for humans, not a
server-side rule** — nothing checks that a code starting with `2` isn't
attached to an Asset-type branch. Pick a code range per type up front (e.g.
1000–1999 Asset, 2000–2999 Liability, ...) and stick to it, because nothing
else will.

The one thing that *is* enforced: **`AccountCode` must be globally unique**
across the entire chart of accounts, regardless of type or branch. Two
different behaviors depending on operation, both surfaced by the API as
`409` (see the spec for exact shapes) — worth knowing if you're debugging
directly against the app service: create reports it via
`ChartOfAccountDTO.ErrorMessageResult`, update throws
`InvalidOperationException`.

## 4. Header vs. Detail — "postable" is mostly aspirational

`AccountCategory` has exactly two values:

| Category | Value | Intent |
|---|---|---|
| `HeaderAccount` | 4096 | Non-postable — a grouping/summary node |
| `DetailAccount` | 4097 | Postable — a real leaf account transactions hit |

The names strongly suggest the system blocks postings to a Header account.
**It mostly doesn't.** Grepping every place `AccountCategory` is actually
read outside the Chart of Account CRUD path itself turns up exactly one
consumer in the whole codebase: `CreditBatchAppService`, for one specific
credit-batch matching flow. The core journal posting path
(`JournalAppService`/the `Journal` aggregate) never checks
`AccountCategory` at all — nothing stops a teller transaction, a general
ledger voucher, or a system process from posting straight to a Header
account if its `ChartOfAccountId` gets wired there by mistake.

**Practical rule to self-impose:** only ever point `ChartOfAccountId`
fields elsewhere in the system (Teller, Treasury, Products, System G/L
mappings — see §6) at `DetailAccount` nodes. Header accounts exist purely
to organize the tree for display/reporting.

## 5. The three behavior flags — mostly inert

`IsControlAccount`, `IsReconciliationAccount`, `PostAutomaticallyOnly` all
sound like they gate real behavior. Checked directly: **none of them are
read anywhere outside the Chart of Account aggregate/factory/app-service
itself.** No posting path checks `PostAutomaticallyOnly` to block manual
journal entries; nothing reads `IsReconciliationAccount` to feed a
reconciliation workflow. As of this codebase, they're metadata you can set
and display, not switches that change what the system will let you do.

The one exception — `IsControlAccount` has exactly one real, enforced
effect: **if it's `true`, `CostCenterId` is silently forced to `null`** on
both create and update, no matter what you send
(`ChartOfAccountFactory`: `costCenterId = (!isControlAccount && ...) ?
costCenterId : null`). Control accounts can't carry a cost center. That's
the only one of these three flags with teeth.

Don't build UI copy or user-facing help text that implies these flags
enforce anything beyond that — they don't, today. If the business actually
needs `PostAutomaticallyOnly` to block manual posting, that's new
application-layer work, not a checkbox that already does something.

## 6. Cost centers

A cost center (`docs/api/costcenter-api-spec.md`) is a simple, separate,
flat lookup — no hierarchy, no behavior flags. Attach one via
`ChartOfAccountId.CostCenterId`, with the one caveat from §5: it's ignored
whenever `IsControlAccount` is `true`. There's no other validation tying
cost centers to specific account types or categories — any Detail account
can optionally carry any cost center.

## 7. How the rest of the system points back at a chart of account

This is the part that actually matters operationally: a chart of account
by itself does nothing. It becomes meaningful once other aggregates are
wired to post against it. Set up the accounts *before* the entities that
reference them, or those entities will have nowhere valid to point:

| Entity | Field(s) | Purpose |
|---|---|---|
| `Teller` | `ChartOfAccountId`, `ShortageChartOfAccountId`, `ExcessChartOfAccountId` | The till's cash account, plus the two suspense accounts EOD variances post to automatically |
| `Treasury` | `ChartOfAccountId` | The vault's cash account |
| Savings/Investment/Loan products | `ChartOfAccountId` on the product itself (e.g. `SavingsProductDTO.ChartOfAccountId`); denormalized onto `CustomerAccountDTO` as `CustomerAccountTypeTargetProductChartOfAccountId` | Where customer balances for that product post |
| System transaction types (Payables Control, Fixed Deposit, External Cheques Control, ...) | `SystemGeneralLedgerAccountMapping` (`api/accounts/chartofaccounts/systemgeneralledgermappings`) | The default account a given system process posts to when there's no natural product/teller to hang the posting off — see §3.6/3.7 of `chartofaccount-api-spec.md` |

Recommended order for standing up a new branch or product line:
1. Create the branch's own sub-tree of chart of accounts (at minimum: a
   cash/vault Detail account, a suspense/shortage-excess pair if it'll have
   a teller).
2. Create the Treasury and Teller records, pointing at those accounts.
3. Create/attach the product's target chart of account.
4. Only then touch `SystemGeneralLedgerAccountMapping` entries that are
   branch-agnostic system defaults (most of these are set up once per
   deployment, not per branch).

## 8. `GET /tree` — what it gives you and what it doesn't (yet)

`GET /tree` is the only place `Depth`/`Children`-equivalent hierarchy comes
back correctly (as a flat list of `GeneralLedgerAccount` with a computed
`Depth` and a pre-formatted `IndentedName` you can drop straight into a
tab-indented `<select>`). It does **not** currently populate `Balance` —
that field exists on `GeneralLedgerAccount` but only gets filled in by a
separate call, `FetchGeneralLedgerAccountBalances(...)`, which this
controller doesn't invoke (it needs a cut-off date and, for per-branch
figures, a branch scope — deliberately left out of the initial pass, see
`docs/api/README.md` changelog). If a screen needs live balances alongside
the tree, that's a follow-up, not something already wired.

## 9. Common mistakes this doc exists to prevent

- Assuming a Header account can't be posted to — it can, nothing stops it.
- Assuming `PostAutomaticallyOnly`/`IsReconciliationAccount` do something —
  they're display metadata only, right now.
- Sending a different `AccountType` for a child account and expecting it to
  stick — it's silently overwritten by the parent's type.
- Trusting `Depth`/`Children` off `GET /{id}` or the flat list — always use
  `GET /tree` for hierarchy.
- Reusing an `AccountCode` — it's globally unique, not scoped per branch or
  per type, and the failure mode differs between create (`409` via
  `ErrorMessageResult`) and update (`409` via a thrown exception, caught and
  normalized by the controller).
- Setting `CostCenterId` on a control account and expecting it to save —
  it's discarded silently.
