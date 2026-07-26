using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequePayableAgg
{
    public static class ExternalChequePayableSpecifications
    {
        public static Specification<ExternalChequePayable> DefaultSpec()
        {
            Specification<ExternalChequePayable> specification = new TrueSpecification<ExternalChequePayable>();

            return specification;
        }

        public static Specification<ExternalChequePayable> ExternalChequePayableWithExternalChequeId(Guid externalChequeId)
        {
            Specification<ExternalChequePayable> specification = DefaultSpec();

            if (externalChequeId != null && externalChequeId != Guid.Empty)
            {
                var externalChequeIdSpec = new DirectSpecification<ExternalChequePayable>(c => c.ExternalChequeId == externalChequeId);

                specification &= externalChequeIdSpec;
            }

            return specification;
        }
    }
}
