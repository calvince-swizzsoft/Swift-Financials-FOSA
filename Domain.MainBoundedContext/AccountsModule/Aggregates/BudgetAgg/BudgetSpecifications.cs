using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetAgg
{
    public static class BudgetSpecifications
    {
        public static Specification<Budget> DefaultSpec()
        {
            Specification<Budget> specification = new TrueSpecification<Budget>();

            return specification;
        }

        public static Specification<Budget> BudgetFullText(string text)
        {
            Specification<Budget> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<Budget>(c => c.Description.Contains(text));

                specification &= descriptionSpec;
            }

            return specification;
        }

        public static ISpecification<Budget> BudgetWithPostingPeriodIdAndBranchId(Guid postingPeriodId, Guid branchId)
        {
            Specification<Budget> specification = new TrueSpecification<Budget>();

            if (postingPeriodId != null && postingPeriodId != Guid.Empty && branchId != null && branchId != Guid.Empty)
            {
                specification &= new DirectSpecification<Budget>(x => x.PostingPeriodId == postingPeriodId && x.BranchId == branchId);
            }

            return specification;
        }
    }
}
