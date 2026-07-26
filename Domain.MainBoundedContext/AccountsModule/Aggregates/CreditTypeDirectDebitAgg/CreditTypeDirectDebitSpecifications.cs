using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeDirectDebitAgg
{
    public static class CreditTypeDirectDebitSpecifications
    {
        public static Specification<CreditTypeDirectDebit> DefaultSpec()
        {
            Specification<CreditTypeDirectDebit> specification = new TrueSpecification<CreditTypeDirectDebit>();

            return specification;
        }

        public static ISpecification<CreditTypeDirectDebit> CreditTypeDirectDebitWithCreditTypeId(Guid creditTypeId)
        {
            Specification<CreditTypeDirectDebit> specification = new TrueSpecification<CreditTypeDirectDebit>();

            if (creditTypeId != null && creditTypeId != Guid.Empty)
            {
                specification &= new DirectSpecification<CreditTypeDirectDebit>(x => x.CreditTypeId == creditTypeId);
            }

            return specification;
        }
    }
}
