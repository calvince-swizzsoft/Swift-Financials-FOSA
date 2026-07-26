using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeCommissionAgg
{
    public class CreditTypeCommission : Domain.Seedwork.Entity
    {
        public Guid CreditTypeId { get; set; }

        public virtual CreditType CreditType { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        
    }
}
