using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg
{
    public static class CustomerAccountCarryForwardSpecifications
    {
        public static Specification<CustomerAccountCarryForward> DefaultSpec()
        {
            Specification<CustomerAccountCarryForward> specification = new TrueSpecification<CustomerAccountCarryForward>();

            return specification;
        }

        public static ISpecification<CustomerAccountCarryForward> CustomerAccountCarryForwardWithBenefactorCustomerAccountId(Guid benefactorCustomerAccountId)
        {
            Specification<CustomerAccountCarryForward> specification = new DirectSpecification<CustomerAccountCarryForward>(x => x.BenefactorCustomerAccountId == benefactorCustomerAccountId);

            return specification;
        }

        public static ISpecification<CustomerAccountCarryForward> CustomerAccountCarryForwardWithBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId)
        {
            Specification<CustomerAccountCarryForward> specification = new DirectSpecification<CustomerAccountCarryForward>(x => x.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountId);

            return specification;
        }

        public static ISpecification<CustomerAccountCarryForward> CustomerAccountCarryForwardWithBeneficiaryCustomerAccountIdAndChartOfAccountId(Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId)
        {
            Specification<CustomerAccountCarryForward> specification = new DirectSpecification<CustomerAccountCarryForward>(x => x.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountId && x.BeneficiaryChartOfAccountId == beneficiaryChartOfAccountId);

            return specification;
        }

        public static ISpecification<CustomerAccountCarryForward> CustomerAccountCarryForwardWithBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountId(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId)
        {
            Specification<CustomerAccountCarryForward> specification = new DirectSpecification<CustomerAccountCarryForward>(x => x.BenefactorCustomerAccountId == benefactorCustomerAccountId && x.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountId);

            return specification;
        }
    }
}
