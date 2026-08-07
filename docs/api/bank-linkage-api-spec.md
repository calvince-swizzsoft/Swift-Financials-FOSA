# Bank Linkage API — Client Integration Spec

Audience: admin screens that map one of this SACCO's own branches to an
external `Bank` account (branch + G/L account + account number), used
downstream when moving cash between a teller/treasury and an external bank
(see `frontoffice-api-spec.md` §5 `CashManagementController`, which resolves
a `BankLinkage` by bank name to find the right G/L account to post
against).

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/BankLinkageController.cs`
- Domain service: `Application.MainBoundedContext/AccountsModule/Services/IBankLinkageAppService.cs`
- Core DTO: `Application.MainBoundedContext.DTO/AccountsModule/BankLinkageDTO.cs`
- Related: `bank-api-spec.md` (the external `Bank`/`BankBranch` master data a
  linkage points at), `chartofaccount-api-spec.md` (the G/L account a
  linkage posts to).
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.

## History note

`BankLinkageDTO`, its domain aggregate, and `IBankLinkageAppService` already
existed in this codebase before this doc — they just had no dedicated
controller. Two other things came out of writing this doc:

- **`BankDTO` was sharing its type with `BankLinkageDTO`.** A prior pass on
  `bank-api-spec.md` had `BankDTO` carrying a pasted-in copy of every
  linkage field (`bankName`, `branchId`, `chartOfAccountId`, ...) so that
  `BankDTO.ValidateAll()` would spuriously fail on real bank-only payloads
  (those fields are `[Required]` but never populated when just creating a
  bank). Those fields are now exclusively on `BankLinkageDTO`; `BankDTO` no
  longer has them. The one place in the codebase that read them off
  `BankDTO` (`ValuesController.AddBankWithLinkages`) now takes a
  `{ bank, bankLinkage }` request body instead of one overloaded `BankDTO`.
- **`CashManagementController` had a dead dependency.** Its
  `_bankLinkageAppService` field was declared but never assigned in the
  constructor — any code path that reached it (`BankToTreasury`/
  `TreasuryToBank` cash movement) would have thrown a
  `NullReferenceException`. Fixed by adding `IBankLinkageAppService` to the
  constructor; DI registration already existed
  (`UnityConfig.cs`/`Container.cs`), so no other wiring was needed.

The reference MVC controller (`SwiftFinancials.Web` Accounts area,
`BankLinkageController`) was **not** ported structurally — it's a
session-heavy multi-step wizard (`Create` → `branch` → `POST Create`, with
branch/G/L selections staged in `Session["bankName"]`/`Session["chartOfAccountId"]`
between requests) built for a WCF `_channelService` proxy this codebase
doesn't have. It also has a couple of live bugs worth knowing about if you
ever need to cross-reference it: `branch2` checks `Session["bankName2"]`
but reads `Session["bankName"]` (typo — always reads stale/wrong data), and
`Create`'s validation-failure path builds an `errorMessages` list that's
never surfaced to the view. This API is a plain stateless CRUD controller
instead — the client sends one complete `BankLinkageDTO` per request, no
session staging.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/banklinkages` |
| Transport | HTTPS only |
| Content type | `application/json` |
| Auth | Bearer JWT on every request |

## 2. Response envelope

```ts
interface ApiEnvelope<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

- `200 OK` — success.
- `400 Bad Request` — missing/invalid body, id mismatch on `PUT`, or a
  `BankLinkageDTO.ValidateAll()` failure (`message` is the joined
  validation error text).
- `404 Not Found` — id doesn't resolve.
- `500 Internal Server Error` — unhandled exception; `message` is the raw
  `ex.Message`.

Note `POST`/`PUT` return `200`, not `201`/`204` — matching
`IBankLinkageAppService.AddNewBankLinkage`/`UpdateBankLinkage`'s existing
shape (`AddNewBankLinkage` returns the created `BankLinkageDTO` directly,
there's no separate "created" status to signal).

## 3. Paging shape

`GET /` returns `PageCollectionInfo<BankLinkageDTO>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

## 4. `BankLinkageDTO` field reference

| Field | Notes |
|---|---|
| `id` | Server-assigned on create. |
| `bankId` | `[Required]`. FK to `Bank` — see `bank-api-spec.md`. |
| `bankName` | `[Required]`. Free-text display copy of the bank's name — **not** re-derived from `bankId`, the caller sets it directly (matches the reference app, which wrote it from the bank picker's selected label). |
| `bankBranchName` | `[Required]`. Free-text — the *external* bank's branch, not one of this SACCO's own branches. |
| `bankAccountNumber` | `[Required]`. The account number at the external bank. |
| `branchId` | `[ValidGuid]`. FK to this SACCO's own `Branch` (see `branch-api-spec.md`) — the branch this linkage belongs to. |
| `branchDescription` | Free-text display copy of the branch name, same caller-sets-it-directly pattern as `bankName`. |
| `chartOfAccountId` | `[ValidGuid]`. FK to the G/L account cash movements against this linkage post to. |
| `chartOfAccountAccountType`, `chartOfAccountAccountCode`, `chartOfAccountAccountName` | Free-text/numeric display copies of the linked G/L account. |
| `chartOfAccountName` | Computed, read-only: `"{type-first-digit}-{code} {name}"`. Ignored on write. |
| `chartOfAccountCostCenterId`, `chartOfAccountCostCenterDescription` | Optional cost-center tag on the G/L account, if used. |
| `remarks` | Optional free text. |
| `isLocked` | Plain bool field on the DTO — **not** wired to any lock/unlock endpoint here (`IBankLinkageAppService` has no lock operation, unlike `Branch`'s `PATCH /toggle-lock`). Settable via `PUT` like any other field. |
| `createdDate` | Server-assigned on create. |
| `bankLinkageBalance` | Not populated by this controller (always `0` from a plain `Create`/`Update`/`Get`) — it's a computed display field the existing `ValuesController.getBankWithLinkages` action fills in by cross-referencing `IChartOfAccountAppService.FindGeneralLedgerAccounts` balances. If you need the live G/L balance alongside the linkage, call that existing endpoint rather than this one. |
| `code`, `paddedCode`, `description`, `address`, `city`, `ibanNo`, `swiftCode`, `no` | Marked `//hacky` in the DTO source — display-only copies of the linked `Bank`'s own fields (see `bank-api-spec.md`). Not populated by this controller either, same reasoning as `bankLinkageBalance`. |

**Important:** unlike `bankId`/`branchId`/`chartOfAccountId` (real foreign
keys), `bankName`/`branchDescription`/`chartOfAccountAccountName`/etc. are
plain denormalized text fields the caller is responsible for keeping in
sync — this controller does not look them up or validate them against the
FK's actual current value. If the underlying bank/branch/G/L-account is
renamed later, these display copies go stale until the linkage itself is
`PUT` again with fresh values.

## 5. Endpoints

All routes below are relative to `/api/accounts/banklinkages`.

### 5.1 List / search — `GET /`

Query params (all optional): `text`, `pageIndex` (default `0`), `pageSize`
(default `20`). Omitting `text` returns the plain paged listing; supplying
it runs the underlying full-text spec on the linkage.

### 5.2 List all (unpaged) — `GET /all`

Every bank linkage in the system, no filter, no paging. Returns
`ApiEnvelope<BankLinkageDTO[]>` (empty array, not `null`, if there are
none). This is what `CashManagementController`'s bank-to-treasury/
treasury-to-bank cash movement resolves against internally (matched by
`bankName`) — fine for a dropdown here too.

### 5.3 Get one — `GET /{id}`

`id` is a GUID. `404` if not found. Returns `ApiEnvelope<BankLinkageDTO>`.

### 5.4 Get by bank account — `GET /by-bank-account/{bankAccountId}`

Looks up a linkage by its **linked customer bank account's id** (not the
external `bankId`, and not `bankAccountNumber`) — matches
`IBankLinkageAppService.FindBankLinkageByBankAccountId`, used elsewhere to
resolve an imprest's or bank-transfer's linkage from an account reference.
`404` if not found. Returns `ApiEnvelope<BankLinkageDTO>`.

### 5.5 Create — `POST /`

Body: `BankLinkageDTO`. `400` with the joined validation message if
`bankId`/`bankName`/`bankBranchName`/`bankAccountNumber` are missing or
`branchId`/`chartOfAccountId` fail `[ValidGuid]`.

Success:
```json
{ "success": true, "message": "Operation Success", "data": BankLinkageDTO }
```

### 5.6 Update — `PUT /{id}`

Body: full `BankLinkageDTO`, with `id` matching the path segment (`400` if
missing/mismatched, `404` if the id doesn't resolve). Same validation as
create. Returns the re-fetched `ApiEnvelope<BankLinkageDTO>` on success.

No delete endpoint — `IBankLinkageAppService` doesn't expose one.
