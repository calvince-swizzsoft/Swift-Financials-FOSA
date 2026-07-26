using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationRegisterAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EducationAttendeeAgg
{
    public class EducationAttendee : Entity
    {
        public Guid EducationRegisterId { get; set; }

        public virtual EducationRegister EducationRegister { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public string Remarks { get; set; }

        

        
    }
}
