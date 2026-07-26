using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountHistoryAgg
{
    public static class CustomerAccountHistorySpecifications
    {
        public static Specification<CustomerAccountHistory> DefaultSpec()
        {
            Specification<CustomerAccountHistory> specification = new TrueSpecification<CustomerAccountHistory>();

            return specification;
        }

        public static ISpecification<CustomerAccountHistory> CustomerAccountHistoryWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<CustomerAccountHistory> specification = new TrueSpecification<CustomerAccountHistory>();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerAccountHistory>(x => x.CustomerAccountId == customerAccountId);
            }

            return specification;
        }

        public static ISpecification<CustomerAccountHistory> CustomerAccountHistoryWithCustomerAccountIdAndManagementAction(Guid customerAccountId, int managementAction)
        {
            Specification<CustomerAccountHistory> specification = new TrueSpecification<CustomerAccountHistory>();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerAccountHistory>(x => x.CustomerAccountId == customerAccountId && x.ManagementAction == managementAction);
            }

            return specification;
        }
    }
}
