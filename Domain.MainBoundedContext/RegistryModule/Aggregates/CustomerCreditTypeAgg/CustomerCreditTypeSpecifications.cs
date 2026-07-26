using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerCreditTypeAgg
{
    public static class CustomerCreditTypeSpecifications
    {
        public static Specification<CustomerCreditType> DefaultSpec()
        {
            Specification<CustomerCreditType> specification = new TrueSpecification<CustomerCreditType>();

            return specification;
        }

        public static ISpecification<CustomerCreditType> CustomerCreditTypeWithCustomerId(Guid customerId)
        {
            Specification<CustomerCreditType> specification = new TrueSpecification<CustomerCreditType>();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerCreditType>(x => x.CustomerId == customerId);
            }

            return specification;
        }
    }
}
