using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CashFlowMappingAgg
{
    public static class CashFlowMappingSpecifications
    {
        public static ISpecification<CashFlowMapping> All()
        { return new TrueSpecification<CashFlowMapping>(); }

        public static ISpecification<CashFlowMapping> WithAccount(Guid accountId)
        { return new DirectSpecification<CashFlowMapping>(mapping => mapping.ChartOfAccountId == accountId); }

        public static ISpecification<CashFlowMapping> CashAccounts()
        { return new DirectSpecification<CashFlowMapping>(mapping => mapping.Section == "Cash"); }
    }
}
