# Cheque Book API — Client Integration Spec

Audience: front/back-office screens that issue chequebooks against a
customer's savings account, activate/lock them, and manage the per-leaf
payment vouchers a chequebook seeds (pay a leaf, flag/unflag it, match a
presented leaf back to its issuing chequebook).

Source of truth:
- Controller: `WebApplication1/Areas/Accounts/Controllers/ChequeBookController.cs`
- Domain service: `Application.MainBoundedContext/AccountsModule/Services/IChequeBookAppService.cs`
- Core DTOs: `Application.MainBoundedContext.DTO/AccountsModule/ChequeBookDTO.cs`,
  `PaymentVoucherDTO.cs`
- Auth: same JWT bearer scheme as every other controller — see
  `docs/api/customer-api-spec.md` §2.
- Related: `WebApplication1/Areas/FrontOffice/CHEQUE-PROCESSING-ANALYSIS.md`
  — full-stack trace of the whole cheque subsystem, including where
  `ChequeBook` sits relative to `ExternalCheque`/`InHouseCheque`/automated
  clearing and how it's used internally by `ElectronicJournalAppService` to
  auto-match KBACTS clearing-house records against payment vouchers.

## History note

`ChequeBookDTO`, its domain aggregate (`ChequeBookAgg`/`PaymentVoucherAgg`),
and `IChequeBookAppService` already existed in this codebase — fully built,
including serial numbering, per-leaf voucher creation, issuance-tariff
posting, and activate/pay/flag — but had **no API controller anywhere**.
It was only reachable through the legacy `ChequeBookService.svc.cs` WCF
passthrough, so the new Web API couldn't issue or manage chequebooks at all.
This controller closes that gap.

The reference MVC controller (`SwiftFinancials.Web` Accounts area,
`CoA_ChequeBooksController`) was **not** ported structurally — it's built
around session/`TempData`-staged multi-step forms this stateless API doesn't
have, and its `POST Edit` action has a copy-paste bug: it declares a
`ChequeBookDTO id, CustomerAccountDTO customerAccountDTO` signature and
actually validates/saves the `CustomerAccountDTO` via
`_channelService.UpdateCustomerAccountAsync` — meaning it never touched a
chequebook at all. `PUT /{id}` here correctly calls
`IChequeBookAppService.UpdateChequeBook`. The reference controller's
`GetSavingsProductsAsync`/`GetInvestmentProductsAsync` (Create-form product
pickers) aren't reproduced either — they duplicate already-documented
savings/investment product listing endpoints.

`IChequeBookAppService` lives under `AccountsModule`, and the reference
controller lived under `Areas/Accounts` — this controller follows that
placement, even though it's grouped thematically with the other
(`FrontOfficeModule`-owned) cheque controllers in
`CHEQUE-PROCESSING-ANALYSIS.md`.

## 1. Environment

| Concern | Value |
|---|---|
| Base path | `https://<host>/api/accounts/chequebooks` |
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
- `400 Bad Request` — missing/invalid body, a `ValidateAll()` failure
  (`message` is the joined validation error text), or `AddNewChequeBook`/
  `PayVoucher`/`FlagVoucher` returning a null/false result (see §5.4, §5.7,
  §5.8).
- `404 Not Found` — id doesn't resolve.
- `500 Internal Server Error` — unhandled exception, or `UpdateChequeBook`
  returning `false` for a chequebook that does exist.

## 3. Paging shape

`GET /` and `GET /{id}/vouchers` return `PageCollectionInfo<T>`:

```ts
interface PageCollectionInfo<T> {
  pageIndex: number;
  pageSize: number;
  pageCollection: T[];
  itemsCount: number;
}
```

## 4. `ChequeBookDTO` field reference

The DTO carries a lot of denormalized customer-account display fields (full
name, account number, salutation, etc.) copied onto the chequebook record at
read time — these are ignored on write. The fields that matter:

| Field | Notes |
|---|---|
| `id` | Server-assigned on create. |
| `customerAccountId` | `[ValidGuid]`. The savings account this chequebook is issued against. |
| `type` | `ChequeBookType`: `0`=`InHouse`, `1`=`External`. |
| `typeDescription` | Computed, read-only. |
| `serialNumber` | Server-assigned on create (`MAX(SerialNumber)+1` across all chequebooks). |
| `numberOfVouchers` | Must be `> 0` (regex-enforced). Number of leaves to seed. |
| `initialVoucherNumber` | Must be `> 0` (regex-enforced). First leaf number; leaves are numbered `initialVoucherNumber .. initialVoucherNumber + numberOfVouchers - 1`. |
| `reference` / `remarks` | Free text. |
| `isActive` | On `PUT`, flipping `false`→`true` activates this chequebook **and deactivates every other chequebook on the same customer account** — a customer has at most one active chequebook at a time. Flipping `true`→`false` does nothing (no deactivate path is called for that direction). |
| `isLocked` | On `PUT`, drives `Lock()`/`UnLock()` on the domain entity. |
| `createdBy` / `createdDate` | Server-assigned on create. |

## 5. `PaymentVoucherDTO` field reference

One row per cheque leaf, seeded automatically when a chequebook is created.

| Field | Notes |
|---|---|
| `id` | Server-assigned on create — this is the id used in the `vouchers/{id}/...` routes below. |
| `chequeBookId` | `[ValidGuid]`. Not re-read by `PayVoucher`/`FlagVoucher` (both resolve the voucher by `id` alone) — travels along from whatever fetch populated the DTO. |
| `chequeBookType`, `chequeBookSerialNumber`, `chequeBookCustomerAccountId`, `chequeBookIsActive`, `chequeBookIsLocked` | Denormalized from the parent chequebook, read-only in practice. |
| `voucherNumber` | The leaf number within the chequebook. |
| `payee` | `[Required]`. Read/persisted by `PayVoucher`. |
| `amount` | Regex-enforced `> 0`. Read/persisted by `PayVoucher`. |
| `writeDate` | Custom-validated: rejected if post-dated or more than ~6 periods stale (`ValidateWriteDate`). Read/persisted by `PayVoucher`; defaults to today if omitted. |
| `reference` | `[Required]`. Read/persisted by both `PayVoucher` and `FlagVoucher`. |
| `status` | `PaymentVoucherStatus`: `0`=`Active`, `1`=`Paid`, `2`=`Flagged` (server-assigned; not settable directly — use §5.8/§5.9). |
| `managementAction` | Write-only input to `FlagVoucher`: `PaymentVoucherManagementAction`, `0`=`Flag`, `1`=`Unflag`. Not persisted as its own column. |
| `paidBy` / `paidDate` | Server-assigned when paid or flagged. |
| `invoiceId` / `vendorId` | `[ValidGuid]` — legacy fields tied to expense-payable integration, not used by any endpoint in this controller. |

## 6. Endpoints

All routes below are relative to `/api/accounts/chequebooks`.

### 6.1 List / search — `GET /`

Query params (all optional): `text`, `type` (`ChequeBookType` int — when
present, results are filtered to that type), `pageIndex` (default `0`),
`pageSize` (default `20`).

### 6.2 List all (unpaged) — `GET /all`

Every chequebook in the system, no filter, no paging. Returns
`ApiEnvelope<ChequeBookDTO[]>` (empty array, not `null`, if there are none).

### 6.3 Get one — `GET /{id}`

`id` is a GUID. `404` if not found.

### 6.4 Create — `POST /`

Body:
```ts
interface CreateChequeBookRequest {
  chequeBook: ChequeBookDTO;
  moduleNavigationItemCode: number;
}
```

`400` (`data: null`) if `chequeBook` is missing/fails `ValidateAll()`, or if
`AddNewChequeBook` returns `null` — which happens when
`initialVoucherNumber + numberOfVouchers - 1 <= 0` (i.e. both are
effectively zero/negative; the DTO's own regex validators normally catch
this first, but the app service re-checks independently).

If a chequebook-issuance commission is configured on the customer's savings
product (`SavingsProductKnownChargeType.ChequeBookCharges`), it's computed
and posted as a journal automatically — not something the client requests.

### 6.5 Update — `PUT /{id}`

Body: `ChequeBookDTO` (`id` in the body is overwritten from the path
segment). `400` with the joined validation message on a `ValidateAll()`
failure, `404` if the id doesn't resolve. See §4 for the `isActive`/
`isLocked` side effects. Returns the re-fetched `ApiEnvelope<ChequeBookDTO>`
on success.

No delete endpoint — `IChequeBookAppService` doesn't expose one.

### 6.6 Vouchers for a chequebook — `GET /{id}/vouchers`

Query params (all optional): `text`, `pageIndex` (default `0`), `pageSize`
(default `20`). Returns `ApiEnvelope<PageCollectionInfo<PaymentVoucherDTO>>`.

### 6.7 Match a presented leaf — `GET /vouchers/match`

Query params: `chequeBookType` (required, `ChequeBookType` int),
`voucherNumber` (required, int), `chequeBookReference` (required, string —
the chequebook's `reference` field, not the voucher's). Returns
`ApiEnvelope<PaymentVoucherDTO[]>` (empty array if no match). This is the
same lookup `ElectronicJournalAppService` uses internally to auto-match
KBACTS clearing-house records against chequebook vouchers — exposed here as
a read-only diagnostic/manual-match tool, not otherwise wired into the
automated-clearing flow.

### 6.8 Pay a voucher — `POST /vouchers/{id}/pay`

Body: `PaymentVoucherDTO` — expected to be fetched from §6.6, edited, and
resubmitted whole (same contract as `ChequeTypeController.Update`). `id` in
the body is overwritten from the path segment, then `ValidateAll()` runs
(`payee`/`reference` required, `amount` and `writeDate` checked — see §5).
`400` on validation failure, or if `PayVoucher` returns `false` (voucher id
doesn't resolve, or its `status` is already `Paid`). No-op if already paid —
does not throw, just fails with `400`.

### 6.9 Flag / unflag a voucher — `POST /vouchers/{id}/flag`

Body: `PaymentVoucherDTO`, `managementAction` set to `0` (Flag) or `1`
(Unflag). Unlike §6.8, this does **not** run full `ValidateAll()` — `payee`/
`amount`/`writeDate` aren't read by `FlagVoucher`, only `reference` and
`managementAction` are, so requiring the rest would be unnecessary friction.
`400` if `FlagVoucher` returns `false` (voucher id doesn't resolve, or is
already `Paid` — a paid voucher can't be flagged either).
