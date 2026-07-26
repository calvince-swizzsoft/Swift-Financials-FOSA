using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SystemGeneralLedgerAccountMappingAgg
{
    public static class SystemGeneralLedgerAccountMappingFactory
    {
        public static SystemGeneralLedgerAccountMapping CreateSystemGeneralLedgerAccountMapping(int systemGeneralLedgerAccountCode, Guid chartOfAccountId)
        {
            var systemGeneralLedgerAccountMapping = new SystemGeneralLedgerAccountMapping();

            systemGeneralLedgerAccountMapping.GenerateNewIdentity();

            systemGeneralLedgerAccountMapping.SystemGeneralLedgerAccountCode = systemGeneralLedgerAccountCode;

            systemGeneralLedgerAccountMapping.ChartOfAccountId = chartOfAccountId;

            systemGeneralLedgerAccountMapping.CreatedDate = DateTime.Now;

            return systemGeneralLedgerAccountMapping;
        }
    }
}
