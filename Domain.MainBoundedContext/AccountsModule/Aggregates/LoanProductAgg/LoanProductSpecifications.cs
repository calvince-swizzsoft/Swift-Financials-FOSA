using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg
{
    public static class LoanProductSpecifications
    {
        public static Specification<LoanProduct> DefaultSpec()
        {
            Specification<LoanProduct> specification = new TrueSpecification<LoanProduct>();

            return specification;
        }

        public static Specification<LoanProduct> LoanProductWithCode(int code)
        {
            Specification<LoanProduct> specification = new TrueSpecification<LoanProduct>();

            var codeSpec = new DirectSpecification<LoanProduct>(c => c.Code == code);

            specification &= codeSpec;

            return specification;
        }

        public static Specification<LoanProduct> LoanProductByName(string name)
        {
            Specification<LoanProduct> specification = new TrueSpecification<LoanProduct>();

            var nameSpec = new DirectSpecification<LoanProduct>(c => c.Description == name);

            specification &= nameSpec;

            return specification;
        }

        public static Specification<LoanProduct> LoanProductWithId(Guid loanProductId)
        {
            Specification<LoanProduct> specification = new TrueSpecification<LoanProduct>();

            var identitySpec = new DirectSpecification<LoanProduct>(c => c.Id == loanProductId);

            specification &= identitySpec;

            return specification;
        }

        public static Specification<LoanProduct> LoanProductFullText(string text)
        {
            Specification<LoanProduct> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<LoanProduct>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var codeSpec = new DirectSpecification<LoanProduct>(x => x.Code == number);

                    specification |= codeSpec;
                }
            }

            return specification;
        }

        public static Specification<LoanProduct> LoanProductFullText(int loanProductSection, string text)
        {
            Specification<LoanProduct> specification = new DirectSpecification<LoanProduct>(x => x.LoanRegistration.LoanProductSection == loanProductSection);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<LoanProduct>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var codeSpec = new DirectSpecification<LoanProduct>(x => x.Code == number);

                    specification |= codeSpec;
                }
            }

            return specification;
        }

        public static Specification<LoanProduct> LoanProductWithInterestChargeMode(int interestChargeMode)
        {
            Specification<LoanProduct> specification = new TrueSpecification<LoanProduct>();

            var codeSpec = new DirectSpecification<LoanProduct>(c => c.LoanInterest.ChargeMode == interestChargeMode);

            specification &= codeSpec;

            return specification;
        }
    }
}
