using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg
{
    public static class DivisionFactory
    {
        public static Division CreateDivision(Guid employerId, string description)
        {
            var division = new Division();

            division.GenerateNewIdentity();

            division.EmployerId = employerId;

            division.Description = description;

            division.CreatedDate = DateTime.Now;

            return division;
        }
    }
}
