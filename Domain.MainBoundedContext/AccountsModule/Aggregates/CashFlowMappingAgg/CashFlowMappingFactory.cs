using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CashFlowMappingAgg
{
    public static class CashFlowMappingFactory
    {
        public static CashFlowMapping CreateCashFlowMapping(Guid accountId, string section, string line, string user)
        {
            var mapping = new CashFlowMapping(accountId, section, line, user);
            mapping.GenerateNewIdentity();
            mapping.CreatedBy = user;
            mapping.CreatedDate = DateTime.Now;
            return mapping;
        }
    }
}
