using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionLevyAgg
{
    public class CommissionLevy : Domain.Seedwork.Entity
    {
        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        public Guid LevyId { get; set; }

        public virtual Levy Levy { get; private set; }

        
    }
}
