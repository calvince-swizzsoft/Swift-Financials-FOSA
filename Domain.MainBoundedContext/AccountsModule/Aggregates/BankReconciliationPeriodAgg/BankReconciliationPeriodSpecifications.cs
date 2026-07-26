using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationPeriodAgg
{
    public static class BankReconciliationPeriodSpecifications
    {
        public static Specification<BankReconciliationPeriod> DefaultSpec()
        {
            Specification<BankReconciliationPeriod> specification = new TrueSpecification<BankReconciliationPeriod>();

            return specification;
        }
        
        public static Specification<BankReconciliationPeriod> BankReconciliationPeriodFullText(string text)
        {
            Specification<BankReconciliationPeriod> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var remarksSpec = new DirectSpecification<BankReconciliationPeriod>(c => c.Remarks.Contains(text));

                specification &= (remarksSpec);
            }

            return specification;
        }
    }
}
