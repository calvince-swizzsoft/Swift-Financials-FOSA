using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.TellerAgg
{
    public class TellerSpecifications
    {
        public static Specification<Teller> DefaultSpec()
        {
            Specification<Teller> specification = new TrueSpecification<Teller>();

            return specification;
        }

        public static ISpecification<Teller> TellerWithEmployeeId(Guid employeeId)
        {
            Specification<Teller> specification = new DirectSpecification<Teller>(x => x.EmployeeId == employeeId);

            return specification;
        }

        public static ISpecification<Teller> TellerWithTellerType(int tellerType, string reference)
        {
            Specification<Teller> specification = new DirectSpecification<Teller>(x => x.Type == tellerType && x.Reference == reference);

            return specification;
        }

        public static ISpecification<Teller> TellerWithReference(string reference)
        {
            Specification<Teller> specification = new DirectSpecification<Teller>(x => x.Reference == reference);

            return specification;
        }

        public static Specification<Teller> TellerFullText(int tellerType, string text)
        {
            Specification<Teller> specification = new DirectSpecification<Teller>(x => x.Type == tellerType);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Teller>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
