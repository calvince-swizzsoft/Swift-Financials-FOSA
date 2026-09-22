# Mobile to Bank

Menu 23050 routes to `/Accounts/MobileToBank`. This is an incoming-payment reconciliation register, not a facility to initiate mobile payments.

The authenticated Web API controller `MobileToBankController` delegates to the existing `IMobileToBankRequestAppService` / `MobileToBankRequestAppService`, backed by the existing `MobileToBankRequest` domain aggregate. No schema or Unity registration changes are needed.

- GET `/api/accounts/mobile-to-bank?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD&status=0&pageIndex=0&pageSize=20&text=`. Status is optional (0 Unmatched, 1 AutoMatched, 2 ReconMatched). Dates are inclusive and filter CreatedDate. Page size is limited to 100.
- GET `/api/accounts/mobile-to-bank/{id}` fetches current payment details.
- PUT `/api/accounts/mobile-to-bank/{id}/reconcile` accepts only `{ "CustomerAccountId": "guid" }`. Amount, transaction references and audit metadata cannot be supplied by the client. The existing AppService resolves the account and permits only Unmatched/Pending records. A failed update returns 409.

The page uses the shared server-backed customer lookup and paged customer account picker. Selecting an account also selects its product. Saving changes matching status to ReconMatched with RecordStatus Pending. It does not post money. The existing `AuditMobileToBankRequestReconciliation` verification/posting workflow is not exposed by this controller or page. AutoMatched is not displayed as manual verification pending merely because its record status is zero.

Verification: `node scripts/test-mobile-to-bank.mjs`, frontend development build, backend Debug build. No live payments are changed by these checks.