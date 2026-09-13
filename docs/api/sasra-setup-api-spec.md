# SASRA reporting setup

Implemented foundation: authenticated institution settings and immutable draft template revisions. This is not report generation, official workbook verification or portal submission.

## Scope

Base route: `api/accounts/sasra/setup`. Uses the normal authenticated service header and server-configured ApplicationDomainName database. One institution profile per configured database (unique SingletonKey=1). No client-supplied connection, company or tenant identifier. Deployments containing multiple independent institutions in one database require additional institution isolation before using this module; branches of one SACCO share its profile.

## Routes

| Method | Route | Behaviour |
|---|---|---|
| GET | profile | Returns current profile; Revision=0 and unset fields before initial setup |
| PUT | profile | Save Profile, InstitutionName, RegistrationNumber, Revision; revision must match the current value |
| GET | versions?pageIndex=0&pageSize=20 | Returns Items and Total; newest first; pageSize 1-100 |
| GET | versions/{id} | Returns the immutable saved definition and its Lines, AccountIds and AccountNames |
| POST | versions | Creates a new immutable revision and its complete template/account tree in one serializable transaction |

Success envelope: `{ success: true, message: "", data: ... }`. Standard error envelope contains message, correlationId and error.validationErrors. Stale revisions return 409, unknown versions 404, invalid fields 400. Missing schema returns 503; SQL uniqueness/deadlock conflicts return 409. Unexpected failures use the global sanitized handler.

## Profile

Profile accepts DT, NWDT or NotApplicable. InstitutionName is required, maximum 256; RegistrationNumber is optional, maximum 80. Revision starts at 0 in a create request; every successful save increments it. Changes to the profile do not rewrite historical template revisions.

## Template request

Required: Profile (DT/NWDT and must match saved settings), ReportCode (40), Title (256), Version (40), Revision, Lines (1-500).

Optional provenance: SourceUrl (HTTPS on sasra.go.ke, maximum 1000), WorkbookSha256 (64 hexadecimal characters), EffectiveFrom (nullable date). Provenance is recorded, not verified. A missing effective date is not invented from the website publication date.

ReportCode and Version are trimmed and normalized to uppercase. Revisions are independent per Profile + ReportCode + Version; first save expects Revision=0. To revise, GET the latest saved version and POST its edited definition with the returned Revision. An older revision cannot overwrite or append over a newer one. Changing code/version starts a new sequence; the UI resets the expected revision to 0. No update/delete routes exist for saved definitions.

Each line contains Code (unique, maximum 40), Description (256), Source, Sheet, Cell, Sign and AccountIds. Array order determines Position.

- Header: no output cell, worksheet or account mappings.
- GlBalance: future calculation source is cumulative G/L closing balance.
- GlMovement: future calculation source is G/L period movement.
- Sign: 1 preserves ledger sign; -1 reverses it. No absolute-value conversion.
- Sheet: valid worksheet name, maximum 31 characters. Cell: A1 notation within Excel worksheet limits. No two lines may write the same cell.
- Accounts: existing leaf posting accounts only, maximum 500 per line and 2000 per definition. Duplicate account use across these direct source lines is rejected. Formula/derived totals are deferred and must not be represented by duplicate direct mappings.
- Empty account mappings are permitted because definitions are drafts. They do not establish report readiness.

AccountNames in responses is a display dictionary; submitted names are not used for validation or persistence. Save creates a root and child ReportTemplate tree plus existing ReportTemplateEntry account links. All owned templates are marked IsVersioned and locked; legacy ReportTemplateAppService updates refuse versioned templates. The separate SasraLineDefinition stores source, sign, explicit order and sheet metadata.

## Database / migration

Schema is implemented in the domain and registered EF configurations, using the repository's existing automatic-migration workflow. New tables: swiftFin_SasraInstitutionProfiles, swiftFin_SasraTemplateVersions, swiftFin_SasraLineDefinitions. Existing swiftFin_ReportTemplates gains IsVersioned (false for existing templates). Unique indices protect singleton settings and version/revision identities; relationships do not cascade-delete existing mappings.

Apply the EF model migration through the deployment's normal migration process before enabling setup. This change does not introduce a separate SQL table-creation script. No migration was applied to the configured database during development. Review the full pending EF migration first: the existing repository configuration enables automatic data-loss migrations, so a blanket migration can include unrelated model changes. Do not run a destructive migration merely to add these tables.

## UI and verification

Route: /Reports/GenerateSasraForm/Setup, linked from SASRA Reports as Reporting setup. Operational layout, info popovers, server-backed account search, horizontal report lines, 20-row paging, loading/error/empty states. Save failures retain the editor. Historical revisions can be opened and used as the basis of a new revision.

Regression tests: tools/tests/Sasra.Tests (validation, revision conflict, one commit per successful definition, no commit after simulated failure, actual EF model discovery without DB initialization). Frontend: scripts/test-sasra-setup.mjs covers client validation, worksheet bounds, duplicates and 500-line inputs. Builds: Debug API and frontend development. No live browser verification or live database migration was performed.

## Sidebar integration

NavigationMenu.cs defines SASRA Reports as code 26016 under Command Hub > Operations > Utilities (26011), alongside Financial Position and User-Defined Reports. The frontend moduleRouteMap maps it to /Reports/GenerateSasraForm; this landing route opens reporting setup and covers /Setup through the normal prefix-based module permissions. The former external-server PDF launcher is no longer the landing page.

Existing deployments can apply docs/database/add-sasra-navigation.sql as an idempotent data migration. It adds only this navigation item and the existing Administrator role grant, matching the utility's default convention. Other roles must be assigned through Administration > Modules. The ordinary full utility seed also picks up this new canonical menu definition on future runs. Navigation does not apply the separate SASRA setup schema migration. Reload the frontend navigation after installation.

## Standard definitions catalogue (12 September 2026)

GET standard-definitions?profile=DT (or NWDT) returns profile-specific standard draft metadata. POST standard-definitions initializes missing definitions for the saved institution profile; its integer result is the number added. This runs in an outer serializable transaction and preserves all existing definitions/mappings. Repeat initialization adds zero records. A definition is considered present by profile + report code, irrespective of version.

DT catalogue: Forms 1, 2, 2B, 3, 4, 4B, 5, 6, 7, 9, 11 and 12. NW-DT: Forms 2A through 2H, 11 and 12. NW-DT 2B-2H retain the published generic form names until workbook headings can be inspected. Catalogue sources: https://www.sasra.go.ke/dts-regulatory-return-forms/, https://www.sasra.go.ke/download-category/regulatory-reporting-forms/page/2/, https://www.sasra.go.ke/nw-dts-regulatory-returns-forms/ and https://www.sasra.go.ke/download-category/nwdts-resources/page/2/.

Codes, published titles and source links are supplied by the application. CATALOGUE-2026-09-12 is an application catalogue revision, not a regulatory effective version. Drafts have one header and no output cells or account mappings: these are catalogue starters, not verified complete reporting definitions. No checksum or effective date is fabricated. The UI keeps technical metadata and custom definition editing behind an advanced section. Choosing the institution category remains an institution setting; never infer it from FOSA use.

The 12 DT catalogue drafts were installed in the configured database through AddStandardDefinitions on 12 September 2026. A repeated invocation added zero. The existing saved DT profile was retained. Installation required no schema migration. For an explicitly authorized repeat installation outside the API, build tools/tests/Sasra.Tests in Debug, then run its Install-StandardCatalogue.ps1 -ApiConfig pointing to the API Web.config. That wrapper supplies the application audit/logging settings temporarily, explicitly disables EF schema initialization, and restores the test-runner configuration afterward. Normal regression runs do not touch the database.


## Supplied Form 6 (DT Statement of Financial Position)

The uploaded `form6-statement-of-financial-position.xls` is embedded in the application assembly with SHA-256 `cd81fc5daad85edd657b8a2f3a9ab7f634774ea7f35d54eb39bcdf98f64a6dea`. Its application version is `WORKBOOK-CD81FC5DAAD8`. This does not claim a regulatory effective date. No schema change is introduced; mappings use the existing domain entities and automatic-migration tables.

- `GET form6/definition`: latest saved revision for this workbook, or a new populated definition. Requires saved DT institution profile. Does not write data.
- `POST versions`: saves its mappings as an immutable revision. Original row positions, descriptions, sources, signs and checksum are fixed. Only posting accounts of the appropriate asset/liability/equity category may be mapped. Duplicate mappings are rejected. Formula and current-surplus lines cannot have mappings.
- `POST form6/preview`: `{versionId, yearStart, asAt}`. Both dates are required, and asAt must lie within the year beginning at yearStart. Returns rows, formula strings, signed amounts in KSh thousands, unmapped accounts (their Balance is in KSh), Difference and LedgerDifference (in thousands), Issues, GeneratedAtUtc, and WorkbookBase64 when validation succeeds. The UI downloads these exact preview bytes as `.xls`; there is no second query between preview and download.

The query covers all branches in the institution database, groups signed journal-entry amounts, and uses `COALESCE(j.ValueDate,j.CreatedDate) < asAt + 1 day`. SOFP balances are cumulative. Current-year surplus is the negative net income/expense balance arising since yearStart. Prior-year unclosed net income/expenses are added to mapped retained earnings. Already posted closing transfers remain in the mapped equity ledger and are not added a second time. Consequently, after closing, transferred surplus is classified by the equity account to which it was posted; review the retained-earnings split when producing a post-closing return.

Assets retain debit-positive signs, the loan-loss allowance is reversed then deducted by the workbook, and liabilities/equity use credit-positive presentation. Map depreciation/other contra-assets with the asset they reduce. No absolute values or balancing plugs are applied. Income/expense accounts are automatic and must not be manually mapped.

Downloads require registration number, no unmapped non-zero balance-sheet accounts, a balanced ledger and total assets equal total liabilities plus equity within KSh 0.01. Preview remains available with useful issues. Empty line mappings mean no ledger accounts assigned to that line; the financial classification still requires the institution's review. This implementation does not submit returns, sign authorization blocks, verify the institution's regulatory status, or persist generated return runs. Earlier catalogue drafts are preserved.

Excel retains the original sheet, formulas, labels, styles, merges, print settings and authorization area. C3 receives registration number, C4 financial year, C5/C6 typed start/end dates. Input values are divided by 1,000 once, formula caches are recalculated with NPOI, and the original `.xls` format is retained.


## Form 7 — Statement of Comprehensive Income

Source: https://www.sasra.go.ke/download/form-7-statement-of-comprehensive-income/ (downloaded 12 September 2026).
The original `.xls` is embedded as `Sasra.Form7.xls`. SHA-256: `a4d3f3d375ee0e03b5db865e05103f9086bfceb202e31c2c7bce91c7cb7116ca`; application version `WORKBOOK-A4D3F3D375EE`, not an official regulatory version. Existing domain entities hold immutable revisions and G/L mappings; no schema migration is required.

- `GET form7/definition`: populated workbook definition or latest saved revision, for the saved DT profile. Read-only.
- `POST versions`: save mappings against the fixed Form 7 definition. Only mapped account IDs/names are editable; canonical codes, descriptions, cells, sources, signs and checksum are validated. Account existence, leaf status, duplicate use and compatible income/expense category are checked.
- `POST form7/preview`: `{versionId,yearStart,asAt}`. Requires a saved Form 7 revision and dates within one financial year. Returns Rows, UnmappedAccounts, Issues, Difference, LedgerDifference, LedgerNetIncome, UnclosedSurplus, ClosingAdjustment, dates, institution name, generation timestamp and WorkbookBase64 when all checks pass.

The 23 input rows use `GlMovement`; 12 totals use the embedded workbook's formulas. Activity covers all branches from yearStart inclusive through asAt inclusive, with an exclusive next-day cutoff and `COALESCE(j.ValueDate,j.CreatedDate)`. Income/expense types are 4000/5000. Fiscal-period closing journals (transaction code 15), including their reversals which retain the code, are separated from operating activity. Other reversals retain their signed effect. Manually posted closing transfers without that code cannot be distinguished automatically.

Income is credit-positive and expenses debit-positive. Recovery row C34 permits income or contra-expense accounts and reverses the ledger sign, so the workbook subtracts recoveries from provision expense. Donation row C54 also permits income/expense accounts and presents their net contribution; C56 adds it to post-tax income. Balance-sheet accounts, such as accumulated depreciation and loan principal, are not valid mappings. Use depreciation expense and income/provision recovery accounts respectively.

Amounts in report rows and headline controls are KSh thousands. Unmapped account Balance and ClosingBalance are KSh. Difference is C56 less independently aggregated net income excluding closing journals. LedgerDifference independently sums all journal entries in the selected period. UnclosedSurplus includes closing transfers and matches the date/type basis of Form 6 C62; ClosingAdjustment + UnclosedSurplus = LedgerNetIncome. This is a ledger-based bridge, not a comparison against a separately saved Form 6 report run.

Download is blocked for unmapped non-zero income/expense activity or closing balances, missing registration number, ledger imbalance or net-income difference above KSh 0.01. Zero activity is supported. No balancing plug is added. Preview and download use the same generated bytes. Original labels, formulas, layout, merges, print settings and authorization blocks are retained, with registration/year and typed start/end dates populated in C3:C6. The file remains `.xls`. There is no website monitoring, regulatory submission, electronic signing or persisted report-run history.

Validation: Form7Tests checks formulas, scale, signs, donations, recoveries, closing bridge, duplicate/type rules, export blocking and original workbook preservation. `scripts/test-sasra-form7-query.ps1` in the frontend repository executes the actual query against read-only SQL fixtures to verify date boundaries, created-date fallback, reversals and closing separation.

Form 7 export increases only rows 18 and 19 to a minimum of 29 points: the supplied labels already wrap, but their original single-line heights clip them once amounts are populated. Other row heights and all workbook formulas remain unchanged.


## Form 1 — Capital Adequacy

Source: https://www.sasra.go.ke/download/form-1-capital-adequacy/ (downloaded 12 September 2026). Embedded resource `Sasra.Form1.xls`; SHA-256 `65571f0718af4de497d6c003214af6e6aefea7d52521466997368538dca79c13`; application version `WORKBOOK-65571F0718AF`. This DT workbook uses whole KSh in column D of `Capital Adequacy`. Existing SASRA domain entities hold mappings/revisions; no schema change.

- `GET form1/definition`: latest saved revision, or canonical definition with compatible mappings suggested from the latest saved Form 6. General/other reserve eligibility is not assumed. No writes.
- `POST versions`: save the fixed definition using existing immutable-revision validation. Thirteen GlBalance lines accept eligible equity (capital) or asset accounts (asset categories). Formula, Manual, Derived and Constant sources are fixed and cannot map accounts. Leaf/existence/type/duplicate checks apply.
- `POST form1/preview`: `{versionId,form6VersionId,yearStart,asAt,surplusAdjustment,investmentDeduction,otherDeductions,offBalanceSheetAssets,capitalEligibilityReviewed,reviewNotes}`. IDs must select saved Form 1 and Form 6 revisions. Four decimal amounts are required, including explicit zero; deductions/exposures must be non-negative, each absolute amount at most 10^15. Notes max 1,000 characters and required for non-zero manual amounts. Dates must lie within one financial year.

Returns Form 6-shaped row/issue/export metadata plus `form6VersionId`, `form6Revision`, `rawCurrentSurplus`, `adjustedSurplus`, `eligibleCurrentSurplus`, `inputs` and `warnings`. Row `isRatio` distinguishes decimal ratios from KSh amounts. All monetary rows, unmapped balances and controls are KSh. Inputs and review notes are returned but not persisted; this is not saved report-run history.

Closing balances include the whole as-at day, with value-date/created-date fallback and all branches. Current-year fiscal closing transfers (code 15) are separated: core component mappings exclude those transfers; eligible surplus includes 50% of adjusted positive pre-closing P&L or the full loss. Prior unclosed P&L augments retained earnings. Manually coded closing transfers cannot be distinguished. D19/D20 are manual capital deductions and D37 is supported off-balance-sheet exposure. Form 6 C38 and C45, converted from thousands once, supply D34 and D44. Form 1 D33 supplies D41. Original formulas calculate the 10%/8%/8% ratios and gaps.

Preview reads under a serializable scope and pins both revisions. It propagates Form 6 issues, detects non-zero unmapped asset/equity balances (excluding Form 6 revaluation/proposed-dividend accounts), requires registration and eligibility review, checks ledger balance and Form 1-to-Form 6 assets within KSh 0.01, and rejects unavailable/non-positive ratio denominators. A non-zero asset difference cannot yet be supported by an attached reconciliation. Data issues suppress WorkbookBase64; a deficient ratio alone is a warning and does not suppress export.

Original `.xls` formulas, formatting and layout are preserved, with D3:D6 registration/year/dates populated. Extra Year 3/Year 4 columns are preserved but not populated. Download uses preview's exact bytes; no regulatory submission or signing. Shared API model-validation errors include actionable field details and correlation IDs.

Validation: 544 Form 1 assertions plus 559 existing SASRA assertions; frontend client-validation checks and development build; API Debug build; actual SQL run against read-only boundary/reversal/closing fixtures. Synthetic workbook was reopened and visually checked via rendered representation. Current database lacks a saved Form 6 workbook revision, so live preview could not complete; ten Form 1 mappings were saved/read back in revision 2.

## Form 2 — Liquidity Statement

`GET /api/accounts/sasra/setup/form2/definition` returns the latest compatible DT workbook revision, or a canonical definition with suggested mappings from the current Form 6. Read access has no institution-profile prerequisite. Save mappings through the existing `POST /versions` endpoint; all normal revision, account and domain persistence rules apply.

`POST /api/accounts/sasra/setup/form2/preview` accepts:

```json
{
  "versionId": "<saved Form 2 revision UUID>",
  "form6VersionId": "<saved Form 6 revision UUID>",
  "yearStart": "2026-01-01",
  "asAt": "2026-09-12",
  "manualAmounts": { "D15": 0, "D16": 0, "D24": 0, "D37": 0, "D38": 0, "D39": 0, "D44": 0, "D45": 0 },
  "exclusions": { "D9": 0, "D10": 0, "D13": 0, "D19": 0, "D20": 0, "D22": 0, "D23": 0, "D27": 0, "D28": 0, "D33": 0, "D34": 0 },
  "liquidityReviewed": false,
  "reviewNotes": "Identify the supporting bank, deposit, maturity and restriction schedules."
}
```

Zero amounts here illustrate the shape, not an assertion about the institution. Both dictionaries require exactly the specified uppercase cell keys and explicit non-negative KSh amounts, up to 10^15. Notes are required, at most 1,000 characters. Dates must fall within the selected financial year and SQL-supported range. Review may be false for a diagnostic preview but blocks Excel.

Manual D16 is **additional** bank obligations; credit balances from bank accounts mapped to D13 enter D16 automatically. D15 is the portion of bank balances over 90 days, not an extra asset. Exclusions reduce mapped balances; never exclude the same amount twice. D37–D39 must be subsets of reported deposits. D44–D45 require a liability maturity schedule and must not duplicate deposits or obligations already deducted from liquid assets.

Response `data` extends the Form 6 preview result with `form6VersionId`, `form6Revision`, `bankOverdrafts`, `inputs` and `warnings`. `difference` is gross deposit mappings minus Form 6 deposit mappings, before exclusions, in KSh. `ledgerDifference` is the sum of the closing ledger. `rows[].isRatio` distinguishes ratio values (0.15 = 15%). Only an issue-free result includes `workbookBase64`; the client downloads those bytes as `.xls`. A deficient but correctly computed liquidity ratio is a warning, not a blocker. Manual inputs and notes are not persisted as a report run.

The original source is https://www.sasra.go.ke/download/form-2-liquidity-statement/, checksum `b71442b70b5eed0867078026911763ec6742fc24c2e46581b1167bee98c0a7b1`, version `WORKBOOK-B71442B70B5E`, sheet `Liquidity`, column D, whole KSh. D43's source double count is corrected to `SUM(D44:D45)`; all other formulas are retained, including `D50 = D35 + D46` (gross deposits plus other liabilities), and the 15% constant. Workbook versus older completion-note differences at the 90/91-day boundaries are explicitly disclosed in the UI; maturity inputs require a confirmed reporting policy.

Expected errors use the existing sanitized API error envelope with field messages and support reference. Invalid request fields return 400, missing saved revisions 404, and stale/concurrent saves 409. Preview financial issues return a successful diagnostic result with `issues` and no Excel bytes. No new tables, startup seeding or custom SQL migration are introduced.


### Form 1 mapping-based generation (13 September 2026)

The Capital Adequacy UI now sends `mappingBased: true` with `versionId`, `form6VersionId`, `yearStart` and `asAt` to the existing Form 1 preview endpoint. No manual adjustment amounts, review notes or eligibility confirmation are required in this mode. Nonzero manual adjustments supplied with this mode return 400 rather than being silently ignored. Older callers without this flag retain the existing adjustment/review contract.

The mapping-based report uses ledger surplus, applies the existing positive-profit 50% / full-loss calculation, and initializes additional deductions and exposures to zero. A warning identifies it as generated without additional regulatory adjustments. Complete those in the editable Excel file: D13 is the eligible surplus contribution, D19 investment deductions, D20 other deductions and D37 off-balance-sheet exposures. Preserve total and ratio formulas. D41 now uses SUM(D26:D32), so edits to asset components propagate to the ratio denominator. Excel edits are not imported or persisted in the application.

Mapping, ledger, Form 6 reconciliation and denominator validations still apply. The mode removes the manual-input/review gate only; it does not label an unchecked report as regulator-ready. Existing stored mappings and schema are unchanged.


### Form 2 mapping-based generation (13 September 2026)

The Form 2 UI now sends `mappingBased: true` with versionId, form6VersionId, yearStart and asAt to the existing preview endpoint. ManualAmounts, Exclusions, ReviewNotes and LiquidityReviewed are not required in this mode. Nonzero adjustments or unknown schedule keys return 400 rather than being silently ignored. Existing callers without this flag retain the original schedule-validation contract.

Mapped closing balances, account signs and automatic per-account bank overdraft splitting remain unchanged. Additional schedule amounts and exclusions start at zero, with a warning that eligibility/maturity adjustments must be completed in Excel. D16 already contains mapped bank overdrafts: users must add only additional obligations. The editable Liquidity sheet retains the total/ratio formulas, including the existing corrected D43 subtotal. No schedule confirmation is required to download in mapping mode; ledger, mapping, gross-deposit reconciliation and denominator checks still apply. Excel edits are not imported or saved back into mapping revisions. No schema or saved-mapping changes are made.

## Form 3 — Statement of Deposit Return

`GET /api/accounts/sasra/setup/form3/definition` returns the latest compatible DT revision, or the canonical workbook definition with proposed mappings from Form 6 C44 → E9 (non-withdrawable), C42 → E10 (savings), C43 → E11 (term). These three category mappings apply across five bands. Read access needs no institution setup; persistence uses existing `POST /versions` with normal revision conflict checks and the existing DT profile. Liability posting accounts only; no duplicates across categories.

`POST /api/accounts/sasra/setup/form3/preview` accepts:

```json
{
  "versionId": "<saved Form 3 revision UUID>",
  "form6VersionId": "<saved Form 6 revision UUID>",
  "yearStart": "2026-01-01",
  "asAt": "2026-09-13"
}
```

Dates must be SQL-supported and within one financial year. Closing balances include all prior postings and the entire as-at day, using the same journal ValueDate/CreatedDate fallback as the existing SASRA G/L calculation. Full ledger, saved mappings and customer-linked balances are read in one Serializable read-only scope. One grouped SQL query covers all customer accounts, with no API page truncation or per-customer queries.

`data` extends the Form 6 result with `depositRows` (15 items with range, depositType, category, band, cell, countCell, accountCount and amount), `totalAccounts`, `totalDeposits`, `mappedLedgerDeposits`, `form6VersionId`, `form6Revision`, `form6Difference`, allocation diagnostics and `warnings`. Only `depositRows[].amount` is in KSh thousands; all reconciliation and diagnostic amounts are whole KSh. Inherited `rows` is unused. `difference` is positive banded deposits minus mapped G/L deposits. `ledgerDifference` is the full closing ledger sum. `form6Difference` compares mapped deposits with Form 6 C42–C44. Nonzero unmapped Form 6 deposit accounts appear in `unmappedAccounts` with their signed G/L balances.

Combine principal and posted interest by customer-account ID within each category before banding. Count positive accounts per category, not unique members or fixed-deposit contracts; exclude zero balances. Missing/null/orphan customer links and debit deposit balances block Excel even if their amounts offset. Differences exceeding KSh 0.01 and missing registration also block Excel. Diagnostic financial issues return 200 with `issues` and no workbook bytes; malformed fields use existing 400 field messages, missing revisions 404, stale saves 409 and sanitized support references.

Band policy: `<50,000`; `50,000..100,000`; `>100,000..300,000`; `>300,000..1,000,000`; `>1,000,000`. Apply before dividing by 1,000. The source labels share endpoints, so the chosen non-overlapping policy is explicitly disclosed. Unposted accrued interest requires supporting schedules and Excel adjustment, not inference from ordinary G/L balances.

Source: https://www.sasra.go.ke/download/form3-statement-of-deposit-return/; SHA-256 `25b1834496bce1d3f88956fb6e32f86bdb7c2a5383f05b47ccbc18c599071eea`; version `WORKBOOK-25B1834496BC`; embedded resource `Sasra.Form3.xls`. Output sheet `Deposits`: D counts, E KSh thousands to five decimals; original D26/E26 sum formulas retained and evaluated. Heading D8 wrapped; sheet editable. The client downloads `workbookBase64` as `.xls`. Excel edits are not imported into saved mappings. No new schema or startup seeding.
