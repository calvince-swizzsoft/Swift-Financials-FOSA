using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeCommissionAgg
{
    public class DynamicChargeCommission : Domain.Seedwork.Entity
    {
        public Guid DynamicChargeId { get; set; }

        public virtual DynamicCharge DynamicCharge { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        
    }
}
