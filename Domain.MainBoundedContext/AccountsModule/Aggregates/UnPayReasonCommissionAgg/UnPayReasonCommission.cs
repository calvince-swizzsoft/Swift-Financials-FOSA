using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonCommissionAgg
{
    public class UnPayReasonCommission : Domain.Seedwork.Entity
    {
        public Guid UnPayReasonId { get; set; }

        public virtual UnPayReason UnPayReason { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        
    }
}
