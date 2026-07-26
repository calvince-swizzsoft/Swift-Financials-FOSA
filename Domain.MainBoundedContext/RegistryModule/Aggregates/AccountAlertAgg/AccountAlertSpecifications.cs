using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.AccountAlertAgg
{
    public static class AccountAlertSpecifications
    {
        public static Specification<AccountAlert> DefaultSpec()
        {
            Specification<AccountAlert> specification = new TrueSpecification<AccountAlert>();

            return specification;
        }

        public static ISpecification<AccountAlert> AccountAlertWithCustomerId(Guid customerId)
        {
            Specification<AccountAlert> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<AccountAlert>(x => x.CustomerId == customerId);
            }

            return specification;
        }

        public static ISpecification<AccountAlert> AccountAlertWithCustomerIdAndType(Guid customerId, int type)
        {
            Specification<AccountAlert> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<AccountAlert>(x => x.CustomerId == customerId && x.Type == type);
            }

            return specification;
        }
    }
}
