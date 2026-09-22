# Kenya statutory salary heads

Implemented 21 September 2026 for monthly cash payroll from January 2025.

## Salary heads and accounts

Create one of each statutory head and attach it to the relevant salary groups and employee salary cards. Statutory calculations override fixed amounts and percentages on those cards.

| Head | Type code | Employee calculation | Employer contribution | Salary-head G/L |
|---|---:|---|---|---|
| NSSF | 61681 | 6% of gross cash wages, subject to the period's earnings ceiling | Equal to employee NSSF | NSSF payable |
| PAYE | 61683 | Monthly progressive bands, less eligible reliefs | None | PAYE payable |
| SHIF | 61693 | 2.75% of gross cash pay; KSh 300 minimum for positive pay | None | SHIF payable |
| Affordable Housing Levy | 61694 | 1.5% of regular cash earnings | Equal to employee levy | Housing Levy payable |

Map **Employer's Contribution (NSSF)** and **Employer's Contribution (Housing Levy)** to the appropriate expense G/L accounts under the chart-of-accounts system mappings. Employer contributions debit these expense accounts and credit the corresponding salary-head payable account. They do not reduce employee net pay. The linked product continues to use the existing salary-head product configuration.

NHIF keeps its original type code (61682) for historical records. It is hidden from new type selections and rejected in current payroll. Existing salary groups/cards must replace NHIF with SHIF; previously posted payslips are not rewritten. Singleton constraints also apply when changing a head's type on edit.

The development database inspected for this task contained no salary heads or identifiable statutory G/L accounts. This change supplies working statutory types and calculations; it does not invent institution-specific G/L accounts or seed salary heads with guessed mappings.

## Period and earnings rules

- The salary month and selected posting period's end year determine the contribution schedule, independent of the date payroll is processed or posted.
- NSSF upper earnings ceilings: January 2025 KSh 36,000; February 2025–January 2026 KSh 72,000; February 2026–January 2027 KSh 108,000. Both employee and employer monthly maxima are therefore KSh 2,160, 4,320 and 6,480 respectively. February 2027 onward requires a verified schedule update in `KenyaPayrollRules` before processing.
- Basic pay plus other earnings forms gross cash pay. Earnings marked **Is One-Off** are excluded from the Housing Levy base. Correctly classify bonuses and other irregular earnings; this flag also triggers the existing clearing of the salary-card amount after posting.
- Current rules run for full-time, part-time and contract employees. Cards must contain exactly one NSSF, PAYE, SHIF and Housing Levy head. Scheme-specific exemptions and contracted-out NSSF arrangements require separate support; omission is not treated as an exemption.
- PAYE bands: first KSh 24,000 at 10%; next 8,333 at 25%; next 467,667 at 30%; next 300,000 at 32.5%; remainder at 35%.
- Taxable pay deducts eligible registered pension contributions (including NSSF), limited to actual contributions, 30% of pensionable cash income and the period's configured ceiling (at most KSh 30,000), followed by SHIF, Housing Levy and a configured salary-card tax exemption. PAYE cannot fall below zero.
- New salary periods default to personal relief KSh 2,400, maximum pension deduction KSh 30,000 and maximum insurance relief KSh 5,000. These remain editable and existing periods are unchanged. Personal relief is for eligible residents; set zero where inapplicable. Insurance relief on the salary card is the already-calculated relief amount, not the premium.
- Monetary results round to two decimal places, midpoint away from zero. Statutory calculations do not use the salary-card rounding selector.
- Noncash benefits, mortgage-interest deductions, post-retirement medical-fund deductions, annual reconciliation and remittance/return submission are outside this change. Existing pre-2025 tariff processing is retained; it is not a newly validated historical statutory engine.

Expected setup failures return HTTP 409 with code `PAYROLL_SETUP_REQUIRED` and a safe, actionable message through the shared API error handler.

## Processing and posting safeguards

Payslip headers and entries are saved through the EF repository in the same scope as draft deletion. Do not use the standalone `SqlCommandAppService.BulkInsert` here: its separate connection waits on the outer serializable transaction and cannot roll back together with draft deletion.

All employee calculations complete before replacing draft payslips. Draft replacement uses one transaction and rechecks that the period is open and no payslips have been posted. Posting validates employer mappings before changing payslip status, and commits status, journals and one-off clearing together. Payout standing orders are dispatched only after that commit. Processing reads the stored period's relief settings rather than trusting values submitted by the client.

Regression coverage includes the real processing and posting AppService with mocked repositories/accounts, NSSF effective-date boundaries, PAYE bands and reliefs, SHIF minimums, irregular earnings, part-time staff, missing setup, journal failures, and employee/employer allocation. No live payroll or real account transactions were used for verification.

## Sources

- [Income Tax Act, section 22A](https://new.kenyalaw.org/akn/ke/act/1973/16/eng%402026-01-01/source)

- [KRA PAYE rates and deductions](https://www.kra.go.ke/individual/filing-paying/types-of-taxes/paye)
- [KRA simplified PAYE return guide](https://www.kra.go.ke/images/publications/STEP-BY-STEP-GUIDE-FOR-THE-SIMPLIFIED-PAYE-RETURN.pdf)
- [KRA Housing Levy notice](https://www.kra.go.ke/news-center/public-notices/2099-)
- [KRA guidance on regular earnings for Housing Levy](https://www.kra.go.ke/news-center/public-notices/1979-affordable-housing-levy-ahl)
- [Social Health Insurance Regulations, regulation 17](https://new.kenyalaw.org/akn/ke/act/ln/2024/49/eng%402024-09-20/source)
- [NSSF year 4 contribution notice](https://www.nssf.or.ke/notice-to-employers-year-4-2026-nssf-contribution-rates)
- [RBA discussion of 2024 NSSF limits](https://www.rba.go.ke/kenyas-pension-industry-soars-assets-under-management-reach-ksh2-25trillion/)
- [RBA discussion of 2025 NSSF limits](https://www.rba.go.ke/kenyan-pension-industry-surges-aum-hits-ksh-2-8-trillion-in-2025/)
