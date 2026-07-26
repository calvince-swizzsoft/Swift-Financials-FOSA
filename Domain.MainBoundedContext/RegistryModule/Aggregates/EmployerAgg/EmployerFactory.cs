using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.EmployerAgg
{
    public static class EmployerFactory
    {
        public static Employer CreateEmployer(string description, Address address, int retirementAge, bool enforceRetirementAge)
        {
            var employer = new Employer()
            {
                Description = description,
                Address = address,
                RetirementAge = (byte)retirementAge,
                EnforceRetirementAge = enforceRetirementAge,
            };
            
            employer.GenerateNewIdentity();

            employer.CreatedDate = DateTime.Now;

            return employer;
        }
    }
}
