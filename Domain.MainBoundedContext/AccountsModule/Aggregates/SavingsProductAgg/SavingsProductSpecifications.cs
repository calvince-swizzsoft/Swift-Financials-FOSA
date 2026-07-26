using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg
{
    public static class SavingsProductSpecifications
    {
        public static Specification<SavingsProduct> DefaultSpec()
        {
            Specification<SavingsProduct> specification = new TrueSpecification<SavingsProduct>();

            return specification;
        }

        public static Specification<SavingsProduct> SavingsProductWithCode(int code)
        {
            Specification<SavingsProduct> specification = new TrueSpecification<SavingsProduct>();

            var codeSpec = new DirectSpecification<SavingsProduct>(c => c.Code == code);

            specification &= codeSpec;

            return specification;
        }

        public static Specification<SavingsProduct> SavingsProductWithId(Guid loanProductId)
        {
            Specification<SavingsProduct> specification = new TrueSpecification<SavingsProduct>();

            var identitySpec = new DirectSpecification<SavingsProduct>(c => c.Id == loanProductId);

            specification &= identitySpec;

            return specification;
        }

        public static Specification<SavingsProduct> DefaultSavingsProduct()
        {
            return new DirectSpecification<SavingsProduct>(x => !x.IsLocked && x.IsDefault);
        }

        public static Specification<SavingsProduct> SavingsProductFullText(string text)
        {
            Specification<SavingsProduct> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<SavingsProduct>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var codeSpec = new DirectSpecification<SavingsProduct>(x => x.Code == number);

                    specification |= codeSpec;
                }
            }

            return specification;
        }

        public static Specification<SavingsProduct> SavingsProductsWithAutomatedLedgerFeeCalculation()
        {
            return new DirectSpecification<SavingsProduct>(x => !x.IsLocked && x.AutomateLedgerFeeCalculation);
        }

        public static Specification<SavingsProduct> MandatorySavingsProducts(bool isMandatory)
        {
            Specification<SavingsProduct> specification = new DirectSpecification<SavingsProduct>(c => c.IsMandatory == isMandatory);

            return specification;
        }
    }
}
