using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDynamicChargeAgg
{
    public class LoanProductDynamicCharge : Domain.Seedwork.Entity
    {
        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        public Guid DynamicChargeId { get; set; }

        public virtual DynamicCharge DynamicCharge { get; private set; }

        
    }
}
