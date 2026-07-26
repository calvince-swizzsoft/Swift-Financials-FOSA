using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SystemGeneralLedgerAccountMappingAgg
{
    public class SystemGeneralLedgerAccountMapping : Entity
    {
        [Index("IX_SystemGeneralLedgerAccountMapping_SystemGeneralLedgerAccountCode")]
        public int SystemGeneralLedgerAccountCode { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public string Description { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        
    }
}
