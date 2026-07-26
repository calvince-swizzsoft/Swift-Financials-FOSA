using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationRegisterAgg
{
    public static class EducationRegisterFactory
    {
        public static EducationRegister CreateEducationRegister(Guid postingPeriodId, Guid educationVenueId, string description, Duration duration, string remarks)
        {
            var educationRegister = new EducationRegister();

            educationRegister.GenerateNewIdentity();

            educationRegister.PostingPeriodId = postingPeriodId;

            educationRegister.EducationVenueId = educationVenueId;

            educationRegister.Description = description;

            educationRegister.Duration = duration;

            educationRegister.Remarks = remarks;

            educationRegister.CreatedDate = DateTime.Now;

            return educationRegister;
        }
    }
}
