using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountSignatoryAgg
{
    public static class CustomerAccountSignatorySpecifications
    {
        public static Specification<CustomerAccountSignatory> DefaultSpec()
        {
            Specification<CustomerAccountSignatory> specification = new TrueSpecification<CustomerAccountSignatory>();

            return specification;
        }

        public static ISpecification<CustomerAccountSignatory> CustomerAccountSignatoryWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<CustomerAccountSignatory> specification = new TrueSpecification<CustomerAccountSignatory>();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerAccountSignatory>(x => x.CustomerAccountId == customerAccountId);
            }

            return specification;
        }
    }
}
