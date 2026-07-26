using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg
{
    public static class FixedDepositTypeSpecifications
    {
        public static Specification<FixedDepositType> DefaultSpec()
        {
            Specification<FixedDepositType> specification = new TrueSpecification<FixedDepositType>();

            return specification;
        }

        public static Specification<FixedDepositType> FixedDepositTypeFullText(string text)
        {
            Specification<FixedDepositType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<FixedDepositType>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static Specification<FixedDepositType> FixedDepositTypeByMonths(int months)
        {
            var specification = new DirectSpecification<FixedDepositType>(c => c.Months == months);

            return specification;
        }
    }
}
