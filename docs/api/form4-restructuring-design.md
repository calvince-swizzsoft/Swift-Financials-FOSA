# Loan restructuring and SASRA Form 4

## Access

- **Reports → Loan Reports → Loan ageing → Repayment schedules**: original and restructured loan cases. Open a restructured case to review its replacement terms, effective posting boundary, opening principal/interest and retained risk category. Saved corrections append revisions.
- **SASRA Reports → Form 4 · Risk Classification of Assets and Provisioning**: preview account classifications, complete dated credit-quality reviews and download the official Excel workbook when all checks pass. Form 4 is derived from loan-account schedules and postings; it does not require mapping an entire G/L into a risk category.

## Restructuring persistence and calculation

The existing `LoanRepaymentPlan` domain aggregate now stores `IsRestructuring`, `EffectiveAt`, `OpeningInterest`, `PriorRiskCategory`, `PriorPlanIds` and `OpeningLedgerHash`. Previous schedule IDs are an immutable snapshot reference list; the original schedule revisions remain intact. The fields are added through normal EF automatic migration only.

The loan restructuring AppService previously committed its case halfway through a scope that permits one commit. It now stages the case, two clearing journals, standing-order changes and draft replacement schedule before one outer Serializable commit. A capture failure prevents that commit. This does not change the existing requirement to settle interest before restructuring, and it does not turn generated repayment terms into confirmed contractual terms.

The opening balance is the actual principal balance at the positive restructuring journal's effective timestamp. The two restructuring principal movements must net to zero; the positive amount must match the opening balance. The snapshot references all supported previous loan-case schedules for the account and retains at least their calculated risk category and the latest prior credit-review category. Future reports allocate only post-boundary repayments to the replacement schedule. Earlier reports use the old schedules. A later original advance remains a separate schedule.

Any changed pre-boundary posting or prior schedule revision requires opening re-review. Loading the replacement schedule refreshes proposed references and marks a changed opening unconfirmed; saving appends a correction. Clearing-journal identity, amount and net balance are checked separately from the opening-posting hash. Dates and risk categories cannot be used to hide prior arrears or upgrade the retained category.

The workflow supports one restructuring per account lifecycle and zero opening interest, consistent with the existing posting path. Multiple restructurings and interest carry-forward/capitalization require a separate supported workflow. Reclassification following sustained performance is not automatic: the retained category remains a minimum. This is conservative retention, not an implementation of an upgrade approval workflow.

## Form 4 classification and provision

The implementation uses the DT SACCO rules in regulations 40–44: principal or interest delinquency, five categories, and rates of 1%, 5%, 25%, 50% and 100%. The classification uses the worse of days overdue and the missed-instalment criterion. On a shared account, the missed-instalment comparison uses the maximum count for an individual loan case, rather than summing unrelated loans' missed payments. The report counts customer loan accounts once, keeps ordinary/restructured sections separate and excludes confirmed settled accounts.

Source: [Kenya Law — Deposit-taking SACCO Business Regulations, 2010](https://new.kenyalaw.org/akn/ke/act/ln/2010/95), regulations 40–44 and Form 4. The workbook is downloaded from [SASRA's official Form 4 page](https://www.sasra.go.ke/download/form-4-risk-classification-of-assets-and-provisioning/), SHA-256 `5e8db04fc69d36d8d39a05be674f8c64d5634007575666dc9d817f0c8d9bad75`, version `WORKBOOK-5E8DB04FC69D`.

Age alone does not establish documentation quality or recoverability. Each outstanding account therefore needs a dated review, with a risk category no better than the calculated/retained minimum, a supported provisioning-basis adjustment (zero must be explicit), and evidence covering credit quality and accrued-interest/interest-in-suspense treatment. Adjustments are report inputs, not journal postings or automatic deductions inferred from account names. The principal G/L control remains separate from these adjustments. This reporting feature does not implement interest suspension, write-off, IFRS 9 measurement or provision-posting workflows.

`LoanRiskReview` is a domain entity mapped to `swiftFin_LoanRiskReviews`, with a unique account/date/revision index, account FK, evidence, author, creation time and a hash of the reviewed ageing result. Reviews persist in the database as append-only revisions. A changed ageing basis invalidates a review. Reviews apply to an exact reporting date, not indefinitely.

The official `.xls` layout, classifications, rates, subtotals and grand total are preserved. The implementation populates counts and exposure and evaluates provision formulas before export. A supporting review sheet records the institution, cases, principal, adjustments, categories, evidence and review revisions. No signature or filing is automated. Institution fields are prefilled from an existing profile when available; a profile is not required to open the report.

## Validation and testing

Missing schedules, unresolved interest, invalid openings, stale reviews, ledger differences and missing institution details block Excel export. Preview still shows issues and clearly labels classified totals incomplete. Schedule/detail/review grids use pages of 20 rows. Client validation covers required values; AppServices enforce dates, amounts, account identity, retained categories and revision conflicts.

40 synthetic assertions cover opening allocations, old/new reporting dates, repayments/refunds, changed history, repeat restructuring, regulatory boundaries, all ten Form 4 rows, rates, official workbook formulas, adjustments and export blocking. A test of the actual restructuring AppService verifies complete staging before one commit and no commit after capture failure. Existing principal/interest/SASRA regression tests pass.

No real loans were restructured and no existing customer schedules or risk reviews were invented during implementation. Existing missing contractual schedules must be confirmed before a complete live Form 4 can be produced.

Live verification: normal EF automatic migration completed; principal KSh 261,000.02 and interest receivable KSh 0.00 both reconcile with zero difference. Form 4 finds four accounts, all requiring contractual schedule confirmation, and blocks Excel export. No real schedules, risk reviews or financial postings were created.
