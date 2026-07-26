using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg
{
    public static class BranchSpecifications
    {
        public static Specification<Branch> DefaultSpec()
        {
            Specification<Branch> specification = new TrueSpecification<Branch>();

            return specification;
        }

        public static Specification<Branch> BranchFullText(string text)
        {
            Specification<Branch> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                var descriptionSpec = new DirectSpecification<Branch>(c => SqlFunctions.PatIndex(text, c.Description) > 0);

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static Specification<Branch> BranchCode(int code)
        {
            Specification<Branch> specification = new DirectSpecification<Branch>(x => x.Code == code);

            return specification;
        }
    }
}
