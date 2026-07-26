using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationAttendeeAgg
{
    public static class EducationAttendeeFactory
    {
        public static EducationAttendee CreateEducationAttendee(Guid educationRegisterId, Guid customerId,  string remarks)
        {
            var educationAttendee = new EducationAttendee();

            educationAttendee.GenerateNewIdentity();

            educationAttendee.EducationRegisterId = educationRegisterId;

            educationAttendee.CustomerId = customerId;
            
            educationAttendee.Remarks = remarks;

            educationAttendee.CreatedDate = DateTime.Now;

            return educationAttendee;
        }
    }
}
