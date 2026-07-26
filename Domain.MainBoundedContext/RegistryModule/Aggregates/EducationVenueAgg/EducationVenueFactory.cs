using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationVenueAgg
{
    public static class EducationVenueFactory
    {
        public static EducationVenue CreateEducationVenue(string description, bool enforceInvestmentsBalance)
        {
            var educationVenue = new EducationVenue();

            educationVenue.GenerateNewIdentity();

            educationVenue.Description = description;

            educationVenue.EnforceInvestmentsBalance = enforceInvestmentsBalance;

            educationVenue.CreatedDate = DateTime.Now;

            return educationVenue;
        }
    }
}
