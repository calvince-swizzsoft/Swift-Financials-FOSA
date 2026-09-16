# Loan ageing per loan

GET `/api/backoffice/loan-ageing/loans?asAt=2026-09-16&pageIndex=0&pageSize=20` (authenticated; optional branchId).

The standard response data contains `loans`, `totalLoans`, `asAt` and portfolio reconciliation issues. Rows contain loanCaseId, customerAccountId, caseNumber, loaneeName, product, outstandingPrincipal, overduePrincipal, overdueInterest, daysPastDue, status and issues. Monetary fields/days are nullable when unresolved. Pagination counts loans, not accounts. The existing report endpoint and SASRA calculations remain account-based.

Rows project the existing oldest-due-first principal and interest allocations by case, before account details are cleared. Repayments are not counted anew against each loan. Missing schedules on shared accounts leave their allocation unknown. A single-case account can still show its known ledger principal. Valid restructuring transfers exposure into its replacement case; original cases carry no duplicated balance. Ambiguous account links produce one unresolved row per case. Unlinked account postings remain portfolio reconciliation exceptions.
