using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FiscalCountAgg
{
    public static class FiscalCountFactory
    {
        public static FiscalCount CreateFiscalCount(Guid postingPeriodId, Guid branchId, Guid chartOfAccountId, string primaryDescription, string secondaryDescription, string reference, Denomination denomination, int transactionCode)
        {
            var fiscalCount = new FiscalCount();

            fiscalCount.GenerateNewIdentity();

            fiscalCount.PostingPeriodId = postingPeriodId;

            fiscalCount.BranchId = branchId;

            fiscalCount.ChartOfAccountId = chartOfAccountId;

            fiscalCount.PrimaryDescription = primaryDescription;

            fiscalCount.SecondaryDescription = secondaryDescription;

            fiscalCount.Reference = reference;

            fiscalCount.Denomination = denomination;

            fiscalCount.TransactionCode = (short)transactionCode;
            
            fiscalCount.CreatedDate = DateTime.Now;

            fiscalCount.SystemTraceAuditNumber = fiscalCount.GenerateSystemTraceAuditNumber();

            return fiscalCount;
        }
    }
}
