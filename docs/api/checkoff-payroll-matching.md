# Check-Off payroll matching

Check-Off CSV Column 2 is the customer's `IndividualPayrollNumbers` identifier. `Reference3` is the separate Personal File Number and is not required for checkoff imports.

`CreditBatchAppService` uses the existing `FindCustomerAccountsByTargetProductIdAndPayrollNumber` application-service lookup for all normal loan/investment allocations and manual discrepancy matches. The existing lookup supports substring matching within the payroll field; rows matching multiple accounts remain discrepancies. Leading zeros are significant and must be retained in the CSV.

Fuzzy matching resolves a single customer by payroll number, then obtains that recipient's standing orders with the CheckOff trigger through the standing-order repository inside the application service. Ambiguous customer matches are not allocated across members. It no longer calls the Reference3 standing-order stored procedure (which is absent from the inspected development database).

Blank payroll identifiers become discrepancies during normal/fuzzy import and are rejected during manual matching. Customer reference fields and Payout behavior are unchanged.

The CSV layout remains Department, Payroll Number, Contribution, Product Balance, Beneficiary, Product Credit Code, Entry Type, Your Reference. The authenticated import route is unchanged.

## Verification

Build `WebApplication1/WebApplication1.csproj`, then run `tools/checkoff-payroll-tests/Run.ps1` with Windows PowerShell. The regression harness invokes the compiled parser with isolated service stubs and checks a payroll match with leading zeros, a blank identifier, and an ambiguous account match. It does not write to a database or post transactions.

## Investment posting restored

Check-Off posts loan repayments and investment contributions (including share capital and deposit contribution products). The investment branch uses ProductCode.Investment and IInvestmentProductAppService.FindCachedInvestmentProduct; it credits the investment product's ChartOfAccountId and debits the batch's CreditTypeChartOfAccountId. The temporary savings workaround was removed. Savings salary/payment credits remain in Payout. No savings Check-Off entry type was added. Payroll matching remains unchanged.
