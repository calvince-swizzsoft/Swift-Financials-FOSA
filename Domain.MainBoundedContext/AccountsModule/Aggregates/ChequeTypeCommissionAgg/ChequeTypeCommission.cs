using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeCommissionAgg
{
    public class ChequeTypeCommission : Domain.Seedwork.Entity
    {
        public Guid ChequeTypeId { get; set; }

        public virtual ChequeType ChequeType { get; private set; }

        public Guid CommissionId { get; set; }

        public virtual Commission Commission { get; private set; }

        
    }
}
