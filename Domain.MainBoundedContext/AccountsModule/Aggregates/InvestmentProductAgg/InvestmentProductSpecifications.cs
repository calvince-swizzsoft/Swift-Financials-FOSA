using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductAgg
{
    public static class InvestmentProductSpecifications
    {
        public static Specification<InvestmentProduct> DefaultSpec()
        {
            Specification<InvestmentProduct> specification = new TrueSpecification<InvestmentProduct>();

            return specification;
        }

        public static Specification<InvestmentProduct> PooledSpec()
        {
            Specification<InvestmentProduct> specification = new DirectSpecification<InvestmentProduct>(x => x.IsPooled && x.PoolChartOfAccountId != null);

            return specification;
        }

        public static Specification<InvestmentProduct> InvestmentProductWithCode(int code)
        {
            Specification<InvestmentProduct> specification = new TrueSpecification<InvestmentProduct>();

            var codeSpec = new DirectSpecification<InvestmentProduct>(c => c.Code == code);

            specification &= codeSpec;

            return specification;
        }

        public static Specification<InvestmentProduct> InvestmentProductWithId(Guid loanProductId)
        {
            Specification<InvestmentProduct> specification = new TrueSpecification<InvestmentProduct>();

            var identitySpec = new DirectSpecification<InvestmentProduct>(c => c.Id == loanProductId);

            specification &= identitySpec;

            return specification;
        }

        public static Specification<InvestmentProduct> InvestmentProductFullText(string text)
        {
            Specification<InvestmentProduct> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<InvestmentProduct>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var codeSpec = new DirectSpecification<InvestmentProduct>(x => x.Code == number);

                    specification |= codeSpec;
                }
            }

            return specification;
        }

        public static Specification<InvestmentProduct> SuperSavingsProduct()
        {
            return new DirectSpecification<InvestmentProduct>(x => !x.IsLocked && x.IsSuperSaver);
        }
        public static Specification<InvestmentProduct> MandatoryInvestmentProducts(bool isMandatory)
        {
            Specification<InvestmentProduct> specification = new DirectSpecification<InvestmentProduct>(c => c.IsMandatory == isMandatory);

            return specification;
        }
    }
}
