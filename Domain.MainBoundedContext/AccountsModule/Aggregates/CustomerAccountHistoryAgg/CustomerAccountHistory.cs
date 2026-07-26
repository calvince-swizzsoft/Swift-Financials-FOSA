using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountHistoryAgg
{
    public class CustomerAccountHistory : Domain.Seedwork.Entity
    {
        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        [Index("IX_CustomerAccountHistory_ManagementAction")]
        public int ManagementAction { get; set; }

        public string Remarks { get; set; }

        public string Reference { get; set; }
        public string ErrorMessageResult;




    }
}
