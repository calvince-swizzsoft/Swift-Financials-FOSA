# Loan ageing API

Authenticated prefix: `/api/backoffice/loan-ageing`. Controllers delegate to `ILoanAgeingAppService`; domain schedules persist through the existing EF repository and automatic-migration architecture.

## Endpoints

- `GET cases?text=&pageIndex=0&pageSize=20`: posted original/restructured loan cases, product description, disbursement amount/date and latest schedule revision/confirmation state. Search is case number or product description; page size 1–100. This is a case list, not an outstanding-account count.
- `GET cases/{id}/plan`: latest immutable schedule revision. If none exists, returns revision 0 with original account, principal G/L, unambiguous posted source disbursement and its principal total, but **no guessed instalments**. Missing/ambiguous source links require review.
- `GET cases/{id}/history`: latest 100 schedule revisions including instalment detail, author, creation timestamp and evidence.
- `POST plans`: append a confirmed original-schedule revision, using the DTO obtained above and its expected revision number.
- `GET report?asAt=2026-09-13&pageIndex=0&pageSize=20&branchId=<optional UUID>&accountId=<optional UUID>`: portfolio principal ageing, control totals and paged accounts. `accountId` returns that account's instalment allocation detail. `branchId` follows account ownership and includes cross-branch postings for those accounts; it is not a branch journal trial-balance filter.

## Schedule confirmation

```json
{
  "loanCaseId": "<case UUID>",
  "customerAccountId": "<resolved account UUID>",
  "principalChartOfAccountId": "<original principal G/L UUID>",
  "sourceJournalId": "<original disbursement UUID>",
  "revision": 0,
  "isConfirmed": true,
  "disbursementDate": "2026-09-01",
  "principal": 1000.00,
  "allocationPolicy": "PRINCIPAL-FIFO-V1",
  "evidence": "Contractual dates checked against the approved agreement.",
  "instalments": [
    { "number": 1, "dueDate": "2026-10-01", "principal": 500.00 },
    { "number": 2, "dueDate": "2026-11-01", "principal": 500.00 }
  ]
}
```

The example is a request shape, not a schedule for an existing customer. Amounts are KSh with at most two decimals. Require 1–1,200 sequential instalments, strictly increasing dates on/after original disbursement, positive amounts and exact principal total. Evidence is required, at most 2,000 characters. Original loan/account/source/GL/principal cannot be changed through correction. Confirmation is explicit, server validated. Stale revision returns 409. No update/delete endpoint exists for saved schedules. New original batch disbursements validate the final schedule against the verified loan terms and effective date, then save revision 1 with both isConfirmed and interestTermsConfirmed true alongside the funding journals. Principal totals, interest minimums/rounding, and upfront timing are validated. Conflicting charge/recovery modes, nonstandard approved payment overrides, or ambiguous interest mappings retain an unconfirmed revision with review reasons in Evidence. Historical imports/captured schedules remain unchanged; manual confirmation appends a new revision.

## Report semantics

`data` contains asAt, generatedAtUtc, policy, totalAccounts, accountsRequiringReview, ledgerPrincipal, accountPrincipal, difference, knownOverduePrincipal, issues, warnings and accounts. Each account includes caseNumbers, outstandingPrincipal, nullable overduePrincipal/daysPastDue/oldestUnpaidDueDate, bucket and issues. Detail adds instalments with planId, caseNumber, number, dueDate, principal, allocatedPrincipal and remainingPrincipal.

Unknown ageing is null, never zero. The known-overdue total excludes unresolved accounts and must not be presented as complete portfolio arrears when accountsRequiringReview is nonzero. Full all-branch G/L reconciliation includes unlinked and non-loan-account principal postings. Settled principal accounts have zero principal and bucket Settled; Disbursed case status alone does not imply an outstanding balance.

The cutoff is the entire selected journal value-date day, falling back to journal creation date. Age begins the day after the oldest unpaid principal instalment's due date. Confirmed original schedules are combined by customer account and allocated using net principal reductions, oldest due first, with case/instalment tie-breakers. Prepayments allocate to future principal. Refunds and linked repayment reversals undo latest settlements by reducing the payment pool. This allocation is calculated on read; it neither creates journals nor changes repayment records.

SASRA risk classes/provisions and restructured schedule replacement are **not assessed**. Principal and contractual interest are assessed independently when their terms and postings are supported. Restructured accounts, unconfirmed schedules, unmatched funding/increases, ambiguous account links and credit balances require review. Ordinary corrections use latest revisions for historical reports; previous revisions are available, but immutable reporting runs are not stored.

## Errors and persistence

Uses `{success,message,data}` and the existing sanitized error envelope with field details/support reference. Invalid input: 400; missing case/account: 404; stale/concurrent schedule: 409; missing automatic-migration schema: 503. Financial/data issues return a diagnostic report with issues and null ageing for affected accounts.

Tables: `swiftFin_LoanRepaymentPlans`, `swiftFin_LoanRepaymentInstalments`, added only through normal EF automatic migration. Foreign keys pin case, customer account, original journal and principal G/L. Unique case/revision and plan/instalment indexes enforce version identity. Snapshot author, evidence and creation date are retained. API and legacy service containers both register the AppService. Standard batch disbursement captures its draft in the same journal-save unit of work; existing pre-journal status updates in the older disbursement workflow are outside this change.


## Contractual interest extension — 13 September 2026

Existing routes are extended, not replaced. `LoanPlanDTO` adds `interestTermsConfirmed` (default false), `interestReceivableChartOfAccountId` and `interestChargedChartOfAccountId` (nullable UUIDs). Each `instalments` row adds nullable `interest` and `interestDueDate`. A positive confirmed interest amount requires its own due date on/after disbursement; zero interest must be explicit. Dates may differ from principal dates or coincide across interest rows. Upfront interest can be entered once at disbursement with zero in other rows. Optional draft values must still be valid non-negative cents and supported dates.

The plan endpoint proposes interest G/Ls from the product when onboarding an old principal-only revision. Saving pins them and rejects client-supplied substitutions. Confirmed interest requires distinct principal, interest-receivable and interest-charged accounts. Supporting evidence and overall schedule confirmation remain required. Appending a revision does not post charges or payments.

Account results add `interest`: `receivable`, nullable `scheduledInterest`, `netSettlements`, `overdueInterest`, `unaccruedDueInterest`, `daysPastDue`, `bucket`, `issues`, and detailed `instalments` (`planId`, `caseNumber`, `number`, `dueDate`, `interest`, `allocatedInterest`, `remainingInterest`). List responses omit both component detail collections; the existing accountId query returns the detail. `combinedDaysPastDue` is the maximum resolved principal/interest age; it and `combinedBucket` remain unknown/Needs review if either component is unresolved.

Portfolio results add `accountInterestReceivable`, `ledgerInterestReceivable`, `interestDifference`, `knownOverdueInterest` and `interestAccountsRequiringReview`. Existing principal totals and principal review counts retain their original meaning. Unknown interest is excluded from the known interest total, not treated as zero. Branch semantics match principal account ownership.

Interest-receivable debits against the pinned interest-charged G/L are charges. Other credits are ledger settlements (not necessarily cash receipts); supported refunds and linked settlement reversals reduce those settlements. Charge credits/waivers require review and are never counted as payments. The net settlement pool allocates oldest interest due first across cases sharing an account, including future interest for prepayments. Principal and interest pools never mix. A zero receivable without confirmed terms and sufficient cumulative accruals does not establish settlement. Uncharged due interest, credit balances, excess charges/settlements, ambiguous postings and restructuring block a complete interest result. Interest accrual generation, waiver approval and variable-rate recalculation are separate workflows.

Persistence extends the existing domain entities and EF mappings only: nullable G/L foreign keys and interest fields, plus a false-default confirmation flag. Old principal schedules remain readable and require explicit interest confirmation. New standard batch disbursements capture generated interest as an unconfirmed proposal; product minimums/rounding/upfront recovery must be checked against approved terms. No manual SQL schema scripts or startup seeding are used.


## Restructuring and Form 4 extension

POST `api/backoffice/loan-ageing/form4`: body `{yearStart,asAt,institutionName,registrationNumber}`. Returns the pinned workbook version, principal G/L controls, ordinary/restructured category rows, detailed account diagnostics, latest dated reviews, exposure/provision totals, unresolved count and `workbookBase64` only when all checks pass. This DT return uses all loan accounts and server-derived ageing, not client-submitted balances.

POST `api/backoffice/loan-ageing/risk-reviews`: `{customerAccountId,asAt,revision,riskCategory,provisioningAdjustment,evidence,basisHash}`. Risk categories 0–4 represent Performing through Loss. Adjustment is required, including explicit zero. Evidence is required (max 2,000 characters). The server recalculates the ageing basis and rejects stale hashes, better-than-minimum categories, negative total exposure and stale review revisions. Saving appends a domain review revision.

Plan DTOs add `isRestructuring`, `effectiveAt`, `openingInterest`, `priorRiskCategory`, `priorPlanIds` and `openingLedgerHash`. Original loan plans cannot submit restructure metadata. The source case must have a posted restructuring; confirmed replacements require confirmed interest and prior schedules. Opening amounts/identifiers are server-derived. Reports before the effective boundary retain original terms. Reopening a replacement proposes current opening references; saving a correction appends a revision.

New domain table: `swiftFin_LoanRiskReviews`. Repayment-plan fields are additive EF changes; normal automatic migration only. All three Debug migration hosts must use the updated domain/infrastructure assemblies.


## Historical schedule proposals and Excel-only Form 4 adjustments

`GET cases/{id}/schedule-proposal` returns a read-only plan, source terms, warnings, canConfirm and proposalHash. Dates come from the original disbursement, saved payment timing and grace. Original case terms are used, including posted upfront interest once. Missing or contradictory terms block bulk confirmation.

`POST generated-schedules/confirm` accepts 1–100 distinct `{loanCaseId, revision, proposalHash}` records. It rechecks every proposal and saves immutable confirmed revisions in one serializable transaction. Stale proposals return 409. No financial postings or new schema are involved.

`POST form4` now exports a system-derived working copy, using calculated categories and booked principal without manual UI review adjustments. Institution details remain required. Unresolved accounts and G/L differences are included in the workbook as exceptions. `isWorkingCopy`, `unclassifiedPrincipal`, account `isSettled` and `interestReceivable` describe this basis. Unclassified accounts retain blank Excel categories; they are not silently classified as performing.

The Loan detail worksheet exposes blue category, adjustment, inclusion and explanation cells. These feed the original Form 4 count/exposure/provision formulas. Excel changes do not persist to the database. The legacy risk-review endpoint remains for compatibility but is no longer used by this Form 4 workflow.


### Reviewing existing unconfirmed schedules

Operations > Loaning > Repayment Schedules > View schedule exposes **Review and confirm schedule** for a captured plan whose principal or interest terms remain unconfirmed. This requests a fresh schedule proposal; viewing does not save. Review the proposed amounts/dates and use **Save schedule** to explicitly confirm a new revision. Proposals with unresolved exceptions cannot be saved. Already-confirmed schedules do not show this action.
