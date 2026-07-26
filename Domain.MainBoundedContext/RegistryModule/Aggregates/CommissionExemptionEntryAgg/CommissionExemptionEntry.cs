using Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionEntryAgg
{
    public class CommissionExemptionEntry : Entity
    {
        public Guid CommissionExemptionId { get; set; }

        public virtual CommissionExemption CommissionExemption { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public string Remarks { get; set; }

        

        
    }
}
