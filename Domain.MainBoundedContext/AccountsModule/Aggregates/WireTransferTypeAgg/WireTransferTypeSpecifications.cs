using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeAgg
{
    public static class WireTransferTypeSpecifications
    {
        public static Specification<WireTransferType> DefaultSpec()
        {
            Specification<WireTransferType> specification = new TrueSpecification<WireTransferType>();

            return specification;
        }

        public static Specification<WireTransferType> WireTransferTypeFullText(string text)
        {
            Specification<WireTransferType> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<WireTransferType>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<WireTransferType> WireTransferTypeDescription(string Description)
        {
            Specification<WireTransferType> specification = new TrueSpecification<WireTransferType>();

            specification &= new DirectSpecification<WireTransferType>(c => c.Description == Description);

            return specification;
        }
    }
}
