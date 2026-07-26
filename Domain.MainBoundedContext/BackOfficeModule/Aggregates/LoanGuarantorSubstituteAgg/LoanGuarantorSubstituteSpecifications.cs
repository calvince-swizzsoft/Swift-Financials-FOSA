using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorSubstituteAgg
{
    public static class LoanGuarantorSubstituteSpecifications
    {
        public static Specification<LoanGuarantorSubstitute> DefaultSpec()
        {
            Specification<LoanGuarantorSubstitute> specification = new TrueSpecification<LoanGuarantorSubstitute>();

            return specification;
        }
    }
}
