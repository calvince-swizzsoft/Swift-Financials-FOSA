using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg
{
    public static class CustomerAccountArrearageSpecifications
    {
        public static Specification<CustomerAccountArrearage> DefaultSpec()
        {
            Specification<CustomerAccountArrearage> specification = new TrueSpecification<CustomerAccountArrearage>();

            return specification;
        }

        public static ISpecification<CustomerAccountArrearage> CustomerAccountArrearageWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<CustomerAccountArrearage> specification = new DirectSpecification<CustomerAccountArrearage>(x => x.CustomerAccountId == customerAccountId);

            return specification;
        }

        public static ISpecification<CustomerAccountArrearage> CustomerAccountArrearageWithCustomerAccountIdAndCategory(Guid customerAccountId, int category)
        {
            Specification<CustomerAccountArrearage> specification = new DirectSpecification<CustomerAccountArrearage>(x => x.CustomerAccountId == customerAccountId && x.Category == category);

            return specification;
        }

        public static ISpecification<CustomerAccountArrearage> CustomerAccountArrearageWithCustomerAccountIdAndEndDate(Guid customerAccountId, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CustomerAccountArrearage> specification = new DirectSpecification<CustomerAccountArrearage>(x => x.CustomerAccountId == customerAccountId && x.CreatedDate <= endDate);

            return specification;
        }
    }
}
