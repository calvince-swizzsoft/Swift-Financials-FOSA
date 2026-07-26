using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationVenueAgg
{
    public class EducationVenue : Domain.Seedwork.Entity
    {
        public string Description { get; set; }

        public bool EnforceInvestmentsBalance { get; set; }
        
        public bool IsLocked { get; private set; }

        

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
