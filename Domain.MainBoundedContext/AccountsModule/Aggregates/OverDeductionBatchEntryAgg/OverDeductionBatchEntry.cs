using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchEntryAgg
{
    public class OverDeductionBatchEntry : Entity
    {
        public Guid OverDeductionBatchId { get; set; }

        public virtual OverDeductionBatch OverDeductionBatch { get; private set; }

        public Guid DebitCustomerAccountId { get; set; }

        public virtual CustomerAccount DebitCustomerAccount { get; private set; }

        public Guid CreditCustomerAccountId { get; set; }

        public virtual CustomerAccount CreditCustomerAccount { get; private set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public byte Status { get; set; }

        

        
    }
}
