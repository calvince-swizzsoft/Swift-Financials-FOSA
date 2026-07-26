using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchEntryAgg
{
    public class InterAccountTransferBatchEntry : Entity
    {
        public Guid InterAccountTransferBatchId { get; set; }

        public virtual InterAccountTransferBatch InterAccountTransferBatch { get; private set; }

        public byte ApportionTo { get; set; }

        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid? ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public string PrimaryDescription { get; set; }

        public string SecondaryDescription { get; set; }

        public string Reference { get; set; }

        public byte Status { get; set; }

        

        
    }
}
