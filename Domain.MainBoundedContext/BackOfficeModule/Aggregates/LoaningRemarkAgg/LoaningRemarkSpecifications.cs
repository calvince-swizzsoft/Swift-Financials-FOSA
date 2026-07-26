using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoaningRemarkAgg
{
    public static class LoaningRemarkSpecifications
    {
        public static Specification<LoaningRemark> DefaultSpec()
        {
            Specification<LoaningRemark> specification = new TrueSpecification<LoaningRemark>();

            return specification;
        }

        public static Specification<LoaningRemark> LoaningRemarkFullText(string text)
        {
            Specification<LoaningRemark> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<LoaningRemark>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<LoaningRemark> LoaningRemarkDescription(string Description)
        {
            Specification<LoaningRemark> specification = new TrueSpecification<LoaningRemark>();

            specification &= new DirectSpecification<LoaningRemark>(c => c.Description == Description);

            return specification;
        }
    }
}
