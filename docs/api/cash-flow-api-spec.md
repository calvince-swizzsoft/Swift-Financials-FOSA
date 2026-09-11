# Cash Flow Statement (direct method)

Base route: `api/accounts/financial-statements/cash-flow`. All routes require authentication and use `{ success, message, data }`. Queries run through `ICashFlowAppService` and the tenant-aware repository. Existing statements are unchanged.

## Installation and configuration

Cash-flow configuration is a domain aggregate (`CashFlowMapping : Entity`) with a factory, specifications, EF entity configuration and `IRepository<CashFlowMapping>` CRUD. `BoundedContextUnitOfWork` discovers its configuration through `AddFromAssembly`; the existing EF automatic-migration configuration manages the table along with the rest of the model. No standalone install script is required. The entity includes standard identity, sequential identity and creation audit fields, plus modification metadata. A unique index enforces one mapping per G/L account; the required account relationship does not cascade-delete mappings. Repository saves participate in the standard EF audit pipeline. No mappings are seeded.

Use the normal application schema-update process before running the updated backend. The former SQL-only installer has been removed and was not run by this implementation. If it was independently applied, preserve its mapping data and reconcile that legacy table with the EF schema/history before updating; do not drop configured mappings or blindly run automatic migrations over an independently created table.

`GET mappings` returns `{ ChartOfAccountId, AccountCode, AccountName, Section, Line }[]`.
`PUT mappings/{accountId}` takes `{ chartOfAccountId, section, line }`. The body ID must match the route. Each posting account has one mapping; the account must exist and have no child accounts. Cash mappings additionally require asset AccountType 1000. Sections: Cash, Operating, Investing, Financing, Exchange. Line is required, trimmed, max 120 characters. Same section/line combines accounts. Modified user/time are recorded. `DELETE mappings/{accountId}` removes the mapping.

The institution must explicitly identify all cash and qualifying cash-equivalent accounts and set classifications under its applicable accounting policy. Do not mark customer savings liabilities as cash. Cash-equivalent eligibility is not inferred from account names. Exchange is only for identified exchange-rate effects, not ordinary cash receipts/payments. Mapping changes apply to regenerated historical reports; this first version does not retain approved mapping versions or historical report snapshots.

## Generate

`GET ?startDate=2026-01-01&endDate=2026-09-11&branchId={optional-guid}`.
Both dates are required, inclusive calendar dates. Effective date is `COALESCE(JournalEntry.ValueDate, JournalEntry.CreatedDate)`, matching existing statements. Opening is before start; closing is before end + 1 day. Branch filters use Journal.BranchId, as the existing branch statement does. Omit branch for consolidated scope.

Returns StartDate, EndDate, BranchId, OpeningCash, ClosingCash, NetCashFlow (Operating + Investing + Financing), ExchangeEffects, UnclassifiedNet, ReconciliationDifference, IsComplete and Rows. Rows contain Section, Line, Receipts, Payments (positive magnitudes), Net (receipts less payments), DetailCount. Operating, Investing and Financing totals are derived from their rows. Review and Internal are generated sections, not configurable mappings.

Classification uses complete journal legs, never the contra field or transaction code. A balanced journal with all cash on one side and all noncash on the other allocates actual noncash amounts to mapped lines. Unmapped counterparts appear in Review. Balanced journals entirely within the configured cash set are Internal, excluded from flow totals. Mixed-sided, unbalanced or cross-period journals are Review using gross period cash debits/credits; nothing is proportionally guessed. A zero-net Review row still makes IsComplete false. Noncash-only journals do not enter the report. Reversed originals remain included together with reversal entries according to their respective effective dates; IsLocked is not a report exclusion.

Opening + NetCashFlow + ExchangeEffects + UnclassifiedNet = ClosingCash + ReconciliationDifference. IsComplete means all detected movements for configured cash accounts are classified and the difference is exactly zero. It does not prove that the configured cash account set or accounting policy is complete. Internal transfers routed through separate journals and clearing accounts require review/mapping; automatic cross-journal matching is not implemented. Branch reports are management reports based on journal ownership, not independent legal-entity cash-flow statements.

## Drill-down

`GET entries` takes the same dates/branch plus `section`, `line`, `pageIndex` (zero-based), `pageSize` (1–100, default 20). Returns at most pageSize rows with JournalId, ValueDate, Reference, Narration, Section, Line, Receipts, Payments, Net, TotalCount. Each row is the journal allocation to that report line. Multiple effective dates in one journal display the latest qualifying leg date. Rows sort by date then JournalId. Totals are aggregated in SQL; full journals are not downloaded to the browser. Details recompute from current data; regenerate the summary after postings or mapping changes.

Each generation reads mappings, balances and movements in a serializable read transaction for consistency. Existing indexes on journal-entry ChartOfAccountId/JournalId and journal BranchId should be reviewed against real execution plans and data volumes before rollout. This implementation is not a forecast, an indirect-method statement, a comparative-period report or a substitute for applicable disclosure requirements.

## Verification

Build `tools/tests/CashFlow.Tests/CashFlow.Tests.csproj` in Debug and run its executable for reconciliation/completeness checks. Run `tools/tests/CashFlow.Tests/Test-CashFlow.ps1` against LocalDB for real SQL fixtures (or supply `-Server`). It uses temporary tables in tempdb only, not an application database. Scenarios cover income, payments, opening balances, internal transfers, noncash journals, split allocations, unmapped and mixed journals, reversal originals, date boundaries/fallback, branch scope, exchange effects and server pagination.
