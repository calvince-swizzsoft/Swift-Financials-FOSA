using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg
{
    public static class DynamicChargeSpecifications
    {
        public static Specification<DynamicCharge> DefaultSpec()
        {
            Specification<DynamicCharge> specification = new TrueSpecification<DynamicCharge>();

            return specification;
        }

        public static Specification<DynamicCharge> DynamicChargeFullText(string text)
        {
            Specification<DynamicCharge> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<DynamicCharge>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
