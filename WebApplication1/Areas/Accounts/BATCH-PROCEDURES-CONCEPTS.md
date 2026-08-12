# Batch Procedures — Functional Basis

Audience: anyone building the remaining `Areas/Accounts/Controllers/*BatchController.cs`
adaptations (Debit, Refund, Wire Transfer, Disbursement, Reversal, Voucher,
General Ledger, Inter Account Transfer), or wondering why `NavigationMenu.cs`
splits every batch type into three separate "Origination / Verification /
Authorization" menu groups.

Source: reference `SwiftFinancials.Web/Areas/Accounts/Controllers/BatchOrigination_*`,
`BatchVerification_*`, `BatchAuthorization_*`, `CreditBatchController.cs`
(reference) / `CreditBatchController.cs` (this repo, already built),
`CreditBatchAppService.cs`, `Infrastructure.Crosscutting.Framework/Utils/NavigationMenu.cs`.

## 1. The pattern: batch = bulk GL posting under three-way segregation of duties

Every one of the nine types is the same shape underneath: a **header**
(who/what/how much in total) plus **line entries** (the individual
postings), moved through three gates before the GL is touched. This is
standard segregation-of-duties control for anything that moves money across
many beneficiaries at once — no single person can originate *and* approve
their own batch.

```mermaid
flowchart TD
    Start["Maker stages a batch: header + line entries"] --> Origination

    subgraph Origination["Stage 1 — Origination (maker)"]
        O1["Create header (type, total value, reference, month/period)"]
        O2["Add entries one at a time, or import a CSV\n(running total checked against header TotalValue as you go)"]
        O1 --> O2
    end

    Origination -->|"Status: Pending"| Verification

    subgraph Verification["Stage 2 — Verification (checker)"]
        V1{"Entries total == header TotalValue?\nData looks right?"}
    end

    Verification -- "Audit: Post" --> Audited["Status: Audited\n(still nothing posted to the GL)"]
    Verification -- "Audit: Reject" --> RejectedV["Status: Rejected\nDiscrepancies cleared, flow ends"]

    Audited --> Authorization

    subgraph Authorization["Stage 3 — Authorization (approver)"]
        A1{"Final approval"}
    end

    Authorization -- "Authorize: Post" --> Posted["Status: Posted\n**GL journals created here** —\nthe only stage where money actually moves"]
    Authorization -- "Authorize: Reject" --> RejectedA["Status: Rejected\nDiscrepancies cleared, flow ends"]

    Posted --> Pickable["Entries individually payable/pickable\n(where the type supports it — PostXEntry)"]
```

Three real roles, not a two-step approval:

1. **Maker** (Origination) — stages the batch, has zero authority to post.
2. **Checker** (Verification) — audits correctness, still can't post.
3. **Approver** (Authorization) — the only one whose action creates GL
   journals. For types with a `PostXEntry` method (Credit, Debit, Wire
   Transfer, Reversal, Disbursement — see §3), individual entries only
   become payable *after* the batch itself reaches `Posted` here; for types
   without one (Refund, Voucher, General Ledger, Inter Account Transfer —
   confirm per type), Authorization posts every line at once.

This is the exact same maker-checker principle already enforced elsewhere
in this codebase (teller cash-deposit requests above a limit, the generic
`Workflow` engine) — Batch Procedures is that same control applied to
*bulk* postings instead of one transaction at a time. `NavigationMenu.cs`'s
three menu groups are a role-routing artifact of that (different people see
different stages) — not a reason to build three separate backend
controllers. `CreditBatchController` already proved one REST controller
with `/{id}/audit` and `/{id}/authorize` actions covers all three stages;
the plan is to keep doing that for the other eight.

## 2. What each type is actually for

```mermaid
flowchart LR
    subgraph Credit-side["Crediting many accounts"]
        Credit["Credit\nPayout, CheckOff,\nCashPickup, SundryPayments"]
    end

    subgraph Debit-side["Debiting many accounts"]
        Debit["Debit\nbulk collections,\nstanding-order deductions"]
        Refund["Refund\n(OverDeductionBatch)\nrefunds an over-collection\nfrom a prior Credit/Debit/CheckOff run"]
    end

    subgraph External["Money leaving the institution"]
        Wire["Wire Transfer\nbatch RTGS/bank wires"]
        Disb["Disbursement\napproved loan principal\nreleased in bulk"]
    end

    subgraph Correction["Fixing what's already posted"]
        Rev["Reversal\nbatch-reverses previously\nposted GL journals"]
    end

    subgraph Manual["Manual posting — two different mechanics, see §5"]
        Voucher["Voucher\nN single-leg lines,\neach Dr/Cr-tagged,\nmust balance as a set"]
        GL["General Ledger\neach line is a pre-paired\naccount A -> account B\ntransfer, self-balancing"]
    end

    subgraph Internal["Moving money between the institution's own GL accounts"]
        IAT["Inter Account Transfer\nGL-to-GL, with a\nDynamicCharges sub-resource"]
    end

    Refund -.corrects.-> Credit
    Refund -.corrects.-> Debit
    Rev -.corrects.-> Manual
```

| Type | Functional purpose | Backing DTO / service |
|---|---|---|
| **Credit** | Crediting many member accounts from one source in one run. `Payout` = savings/dividend interest payout runs. `CheckOff` = an employer remits one lump sum of payroll deductions; the batch allocates it across many members' loan repayments, share contributions, welfare/insurance premiums, etc. — confirmed in `CreditBatchAppService`, which has explicit `CheckOffEntryType` handling per product line (`sLoan`, `sInterest`, `sShare`, `wCont`, `sInvest`, `sRisk`, `wLoan`, `sLoanInterest`). `CashPickup`/`SundryPayments` pay non-members. | `CreditBatchDTO` / `ICreditBatchAppService` — **built** (`CreditBatchController.cs`) |
| **Debit** | The mirror of Credit — bulk debits off many member accounts in one run (standing-order collections, bulk fee/charge deductions). No sub-type enum, unlike Credit. | `DebitBatchDTO` / `IDebitBatchAppService` |
| **Refund** | Correcting a Credit/Debit/CheckOff run that over-collected — refunds the excess back to affected members in bulk. Smaller interface surface: no `PostXEntry`/queueable-entries method, so entries likely post wholesale on Authorization rather than being individually pickable — confirm when building. | `OverDeductionBatchDTO` / `IOverDeductionBatchAppService` |
| **Wire Transfer** | Batch of outgoing external transfers (bank wires) — money leaving the institution to external accounts. | `WireTransferBatchDTO` / `IWireTransferBatchAppService` |
| **Disbursement** | Releasing approved loan principal to members in bulk, after a loan case has cleared appraisal/approval upstream. Interface also exposes `DisburseMicroLoan` and a transaction-threshold validator that look like a separate alternate-channel (USSD/API) disbursement path, not part of this batch UI — confirm with product before deciding whether those belong on the same controller. | `LoanDisbursementBatchDTO` / `ILoanDisbursementBatchAppService` (lives in `BackOfficeModule`, but the reference app still routes its screens under Areas/Accounts) |
| **Reversal** | Batch-reversing previously posted GL journals — corrections to postings already authorized elsewhere. | `JournalReversalBatchDTO` / `IJournalReversalBatchAppService` |
| **Inter Account Transfer** | Bulk GL-to-GL transfers between chart-of-accounts (branch/cost-center reallocation), with a `DynamicCharges` sub-resource for transfer fees. | `InterAccountTransferBatchDTO` / `IInterAccountTransferBatchAppService` |
| **Voucher** | The classic **N-line general journal**: any number of single-leg lines, each independently tagged via `JournalVoucherType` — Debit/Credit × G/L-account/Customer-account (4 combinations) — and the whole collection must balance to the header's `TotalValue`. General-purpose adjusting entries, accruals, cost allocations. Own status/auth-option enums (`JournalVoucherStatus`/`JournalVoucherAuthOption`) rather than the shared `BatchStatus`/`BatchAuthOption` the rest of this table uses. | `JournalVoucherDTO` / `IJournalVoucherAppService` |
| **General Ledger** | A batch of **pre-paired account-to-account transfers** — each single entry row carries *both* a credit-side account (`ChartOfAccountId`/`CustomerAccountId`) *and* a debit/contra-side account (`ContraChartOfAccountId`/`ContraCustomerAccountId`) at once, each side independently resolvable to a raw G/L account or a customer account. Every row is self-balancing by construction (no matching offset line needed, unlike Voucher) — this is a bulk "move money from specific account A to specific account B" correction/transfer tool, not a free-form journal. Own parallel enums (`GeneralLedgerStatus`/`GeneralLedgerAuthOption`). **Settled, not redundant with Voucher — see §5.** | `GeneralLedgerDTO` / `IGeneralLedgerAppService` |

## 3. Which types let you pay out entry-by-entry vs. all at once

Not every type behaves like Credit's `CashPickup` (browse individually
payable entries after the batch posts). Interface surface as of this
writing:

| Type | Has `PostXEntry` (per-entry payout)? | Has `FindQueableXEntries`? | Has CSV import (`ParseXImport`)? |
|---|---|---|---|
| Credit | Yes | Yes (Payout/CheckOff only) | Yes |
| Debit | Yes | Yes | Yes |
| Wire Transfer | Yes | Yes | Yes |
| Reversal | Yes | Yes | No |
| Disbursement | Yes | Yes | No |
| Refund | No | No | Yes |
| Voucher | No | No | No |
| General Ledger | No | No | Yes (`ParseGeneralLedgerImportEntries`) |
| Inter Account Transfer | No | No | No |

CSV import is out of scope for all of them in the near term regardless —
none of this project's controllers have a file-upload pattern yet, so it's
one shared initiative later rather than five ad hoc ones now.

## 4. Comparison to Microsoft Dynamics 365

The closest analogue is Dynamics' **Journals + Journal Batches + batch
approval workflows**:

| This system | Dynamics 365 (Business Central / F&O) |
|---|---|
| Voucher / General Ledger | **General Journal** — a journal batch/template posting arbitrary debit/credit lines to the G/L |
| Credit / Debit | **Payment Journal** (Credit — paying/crediting many accounts in one batch) or a **Direct Debit collection journal** (Debit — collecting from many accounts) |
| Wire Transfer | Payment Journal + **Export Payment File** (bank-file generation for EFT/wire batches) |
| Disbursement | Closest to a Payment Journal releasing funds against an approved upstream commitment — Dynamics has no native "loan disbursement," that's a lending-vertical concept here too |
| Reversal | BC's native **Reverse Transaction / Correction** on posted G/L entries |
| Inter Account Transfer | **Bank Account Transfer** journal / intercompany posting, with charges attached |
| Origination → Verification → Authorization | Dynamics' **journal batch approval workflows** (submit → review → approve; posting only happens on final approval) — same three-gate control, configured via workflow rules instead of hardcoded stages |

Nothing here is exotic — it's a fairly textbook core-banking
batch-posting-with-approval-workflow design.

## 5. Settled: Voucher vs. General Ledger are genuinely different tools

From the app-service interface alone these looked like the same concept
twice (same CRUD/audit/authorize shape, same "replace the whole entry
collection" update method, only the enum names differ) — this codebase has
precedent for exactly that kind of duplication being real (Commission had a
redundant, buggier `ChargesController` — see
`COMMISSION-LEVY-CHARGE-CONCEPTS.md`). Reading the actual entry DTOs
(`JournalVoucherEntryDTO.cs`, `GeneralLedgerEntryDTO.cs`) and both reference
controllers (`BatchOrigination_VoucherController.cs`,
`AddGeneralLedgerController.cs`) settles it: they are not redundant. The
difference is in what one *entry row* represents:

```mermaid
flowchart TD
    subgraph VoucherShape["Voucher — N single-leg lines"]
        direction LR
        VL1["Line 1: Dr, G/L Rent Expense, 5,000"]
        VL2["Line 2: Dr, Customer #4021 loan a/c, 2,000"]
        VL3["Line 3: Cr, G/L Cash, 7,000"]
        VNote["Each line picks ONE account\n(JournalVoucherType: Debit/Credit x GLAccount/CustomerAccount)\nand the whole set of lines must sum to the header TotalValue.\nBalancing is the preparer's job across however many lines it takes."]
    end

    subgraph GLShape["General Ledger — pre-paired transfers"]
        direction LR
        GL1["Row 1: Credit side = Customer #1050 savings a/c\nDebit/Contra side = Customer #2210 savings a/c\nAmount: 3,000"]
        GL2["Row 2: Credit side = G/L Suspense\nDebit/Contra side = Customer #3390 loan a/c\nAmount: 1,200"]
        GLNote["Each ROW already specifies both sides of its own\ntransfer (ChartOfAccountId + ContraChartOfAccountId,\neach independently a G/L or customer account).\nEvery row is self-balancing by construction —\nno matching offset line required."]
    end
```

| | Voucher | General Ledger |
|---|---|---|
| Unit of an entry | One account (Dr or Cr, G/L or customer) | A pair of accounts (credit side + contra/debit side) in one row |
| How a batch balances | Across the whole collection of lines summing to `TotalValue` | Automatically — each row is a complete transfer by itself |
| Typical use | General-purpose adjusting/accrual entries — the textbook "General Journal" | Bulk account-to-account corrections/transfers — moving or fixing money between two *specific* accounts, often two member accounts |
| Entry-side account resolution | `ChartOfAccountId` (single) | `ChartOfAccountId` (credit) **and** `ContraChartOfAccountId` (debit), each resolvable via `CreditCustomerAccountLookUp`/`DebitCustomerAccountLookup` against a real customer account |

Both are worth building as designed — Group B is unblocked.
