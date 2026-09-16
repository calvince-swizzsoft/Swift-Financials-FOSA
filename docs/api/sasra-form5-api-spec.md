# SASRA Form 5 API

Base: `/api/accounts/sasra/setup`

- `GET /form5/definition`: latest saved DT workbook revision, or the pinned official definition with conservative SOFP starter mappings. Requires no institution-profile creation just to read it.
- `POST /versions`: existing version endpoint saves Form 5 account mappings. Only asset posting accounts are allowed; canonical cells, signs, descriptions and sources cannot change. Duplicate mappings and stale revisions use existing validation.
- `POST /form5/preview`: body `{versionId, form1VersionId, form6VersionId, yearStart, asAt, reportPurpose}`. All three revisions must exist. Dates must fall within one financial year. Current DT institution details are required for generation.

Response is the normal `{success,data}` envelope. Result extends the common Form 6 result with `form1VersionId`, `form1Revision`, `form6VersionId`, `form6Revision`, `reportPurpose`, and `warnings`. `rows` carries C8:C28 input and ratio lines, with null `amount` for unavailable ratios. `unmappedAccounts` lists relevant non-zero SOFP investment/property/equipment accounts not yet classified. `workbookBase64` is present only when blocking `issues` are empty. Ratios exceeding limits are warnings, not data failures.

The service reads closing balances in a serializable reporting scope, validates mapped asset posting accounts, calls the existing mapping-based Form 1 calculation using the selected SOFP revision, and reuses D22 (core capital), D34 (SOFP assets) and D44 (SOFP deposits), all in KSh. Only C11/C12/C13 accept Form 5 G/L mappings.

The official source workbook is embedded as `Sasra.Form5.xls` and verified by SHA-256 `c1a7ced517a55824467f615d4be666db4e20129f17d09e453e3a404c2f9d4bc5`. Content version `WORKBOOK-C1A7CED517A5`. Source: https://www.sasra.go.ke/download/form-5-investment-return/ . No new tables or migration scripts are needed.

The workbook's missing C15 ratio and C16 maximum and fixed C17 excess are corrected to land/buildings divided by total assets, 5%, and ratio minus maximum. This 5% ceiling matches regulation 48(1), https://new.kenyalaw.org/akn/ke/act/ln/2010/95 . Other limits retain the source's 40%, 5% and 10%. Non-positive denominators yield the visible text n.a. and dependent excess cells do the same. The source C11 label excludes land/buildings; preview guidance explicitly preserves that template distinction and asks users to review regulatory treatment using supporting schedules.

C8:C13 are editable in the exported workbook. Additional adjustments, exclusions and waivers remain Excel-only. The source calculation does not ingest edits made in previously downloaded workbooks. The official source specifies quarterly returns; `reportPurpose` defaults to `Quarterly` and enforces calendar quarter-end dates in the AppService. `Interim` permits any otherwise valid date and labels the workbook title and print header as an interim working copy. Other purpose values are rejected.

See Form5Tests for canonical-definition protection, source-unit conversion, ratios, input edits, zero/negative core capital, invalid mappings, missing accounts, and upstream issue propagation. Native Excel verified no formula errors and recalculation after representative edits. No database mappings were changed during verification.
