

## Early subsequent notices
Second and third notices may be prepared and dispatched before the preceding notice’s response deadline. The deadline remains visible. Prior-stage dispatch, recipient, approval, arrears and days-overdue checks still apply. Recovery can become eligible immediately after all required third notices have been sent; the final response deadline does not block it. A notice’s own deadline must still be in the future when sent.


## Loan-level notice stages
The notice screen exposes First notice, Second notice, Third notice and Recovery only. Other/Saved notice tabs and recipient drawers are removed. GET `loan-notices/loans` groups before pagination and counts each loan once. POST `loan-notices/loans/action` performs prepare, approve, send, printed dispatch or reset for the stage’s required recipients. Email/SMS sending validates all contacts before writes and stages all new messages in one transaction. Existing queued/sent messages are not duplicated; explicit retries cover failures only. Existing per-recipient records remain for delivery tracking and audit. Print produces one bundled document. Current eligibility is rechecked at dispatch; recovery itself sends no messages.


## Deposit recovery from the Recovery tab

Each loan has a **Recover** action which opens a preview with a **Recover deposits from** checkbox list for the borrower and each guarantor. No accounts are selected initially. Selecting accounts recalculates the preview on the server; unselected accounts never contribute funds. It uses today's balances even if the list was viewed at a historical date. **Recover deposits** posts only after the operator confirms the displayed debit total. The existing Guarantor Attachment debt-transfer screen is unchanged.

- Target: overdue principal plus overdue interest already posted to interest receivable; no full-loan recall and no implicit interest accrual. Interest is settled before principal.
- Borrower deposits first, then capped proportional allocation among the loan's active guarantors. Capped amounts are redistributed within remaining guarantee capacity. The drawer shows uncovered shortfall.
- Sources: normal, approved accounts of approved/unlocked members, unlocked savings or investment/deposit products, and unlocked liability-classified control accounts. Equity/share capital is excluded by GL classification. The investment product’s Refundable flag does not control recovery eligibility. Investment credits must have matured; debits count immediately. Product/branch minimum balances are retained; accounts with running/pending fixed deposits are excluded. Only cleared balances on the product control account count, never absolute values of debit balances.
- Other active guarantees are conservatively reserved at their full recorded amount. This loan's own guarantee is not deducted twice. Remaining guarantee subtracts both previous deposit recoveries and legacy attachment-history amounts. The original guarantee contract and status are retained; this flow does not automatically release a guarantee or rewrite the original amount guaranteed. Prior recoveries/attachments remain conservatively deducted after a reversal until reviewed.
- An unresolved ageing report, shared loan account, duplicate active guarantors, invalid ledger mapping, or missing current posting period prevents posting. Notice response deadlines do not add a recovery waiting period.
- The server rebuilds the preview inside a serializable transaction; a changed preview returns 409. Journal debits/credits and the immutable recovery snapshot commit together. A persisted unique request ID makes retries idempotent.

Persistence adds the EF-mapped LoanRecoveries table through the existing additive automatic migration. Restart the API on the updated Debug build so the current EF model initializes; deployments using Utility must run its updated automatic migration before recovery. No recovery is posted by migration.

### Recovery endpoints

- GET /api/backoffice/loan-notices/loans/{loanCaseId}/recovery: current LoanRecoveryPreview, including borrower/guarantor source accounts and amounts, due principal and recoverable interest, totals, shortfall and basisHash. Read-only.
- POST /api/backoffice/loan-notices/loans/recovery: { loanCaseId, requestId, basisHash, selectedAccountIds }. Use a client-generated UUID for requestId and retain it when retrying an uncertain response. The selectedAccountIds list is required; the server validates every ID against the loan participants’ currently eligible accounts. No client-supplied recovery amounts are accepted. Returns { id, loanCaseId, total, postedAt } inside the standard success/data envelope.
- Expected errors: 400 malformed input, 404 unknown loan, 409 eligibility/balance conflict, 503 missing schema. API controllers delegate all calculation and posting to ILoanRecoveryAppService.

Validation: LoanRecoveryTests covers borrower priority, capped proportional allocation, cent rounding, negative balances, other commitments, posted interest limits, stale data, eligibility changes, retry identity, prior recovery/attachment capacity, balanced journal direction and commit failure. Real EF model tests include LoanRecoveries.

- POST /api/backoffice/loan-notices/loans/recovery/preview: { loanCaseId, selectedAccountIds }. Read-only server calculation using only selected accounts; an empty list returns zero recovery and all eligible account choices. GET recovery also starts with no selection. Account selections are bound to basisHash, revalidated on posting and retained in the recovery audit snapshot. No schema change is needed.
