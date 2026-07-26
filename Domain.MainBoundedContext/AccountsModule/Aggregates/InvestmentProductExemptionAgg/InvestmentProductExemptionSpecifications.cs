using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductExemptionAgg
{
    public static class InvestmentProductExemptionSpecifications
    {
        public static Specification<InvestmentProductExemption> DefaultSpec()
        {
            Specification<InvestmentProductExemption> specification = new TrueSpecification<InvestmentProductExemption>();

            return specification;
        }

        public static ISpecification<InvestmentProductExemption> InvestmentProductExemptionWithInvestmentProductId(Guid investmentProductId)
        {
            Specification<InvestmentProductExemption> specification = new TrueSpecification<InvestmentProductExemption>();

            if (investmentProductId != null && investmentProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<InvestmentProductExemption>(x => x.InvestmentProductId == investmentProductId);
            }

            return specification;
        }

        public static ISpecification<InvestmentProductExemption> InvestmentProductExemptionWithInvestmentProductIdAndCustomerClassification(Guid investmentProductId, int customerClassification)
        {
            Specification<InvestmentProductExemption> specification = new TrueSpecification<InvestmentProductExemption>();

            if (investmentProductId != null && investmentProductId != Guid.Empty)
            {
                specification &= new DirectSpecification<InvestmentProductExemption>(x => x.InvestmentProductId == investmentProductId && x.CustomerClassification == customerClassification);
            }

            return specification;
        }
    }
}
