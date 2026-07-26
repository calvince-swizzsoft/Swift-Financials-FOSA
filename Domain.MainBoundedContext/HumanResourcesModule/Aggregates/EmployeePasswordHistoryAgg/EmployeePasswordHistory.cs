using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeePasswordHistoryAgg
{
    public class EmployeePasswordHistory : Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public string PasswordHash { get; set; }

        

        
    }
}
