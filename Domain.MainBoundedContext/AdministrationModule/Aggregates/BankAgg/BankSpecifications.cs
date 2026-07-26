using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BankAgg
{
    public static class BankSpecifications
    {
        public static Specification<Bank> DefaultSpec()
        {
            Specification<Bank> specification = new TrueSpecification<Bank>();

            return specification;
        }

        public static Specification<Bank> BankFullText(string text)
        {
            Specification<Bank> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                int number = default(int);

                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var codeSpec = new DirectSpecification<Bank>(c => c.Code == number);

                    specification &= codeSpec;
                }
                else
                {
                    var descriptionSpec = new DirectSpecification<Bank>(c => c.Description.Contains(text));

                    specification &= descriptionSpec;
                }
            }

            return specification;
        }
    }
}
