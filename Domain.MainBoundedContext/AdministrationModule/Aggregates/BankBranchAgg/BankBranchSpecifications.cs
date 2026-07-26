using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BankBranchAgg
{
    public static class BankBranchSpecifications
    {
        public static Specification<BankBranch> DefaultSpec()
        {
            Specification<BankBranch> specification = new TrueSpecification<BankBranch>();

            return specification;
        }

        public static ISpecification<BankBranch> BankBranchWithBankId(Guid bankId)
        {
            Specification<BankBranch> specification = new TrueSpecification<BankBranch>();

            if (bankId != null && bankId != Guid.Empty)
            {
                specification &= new DirectSpecification<BankBranch>(x => x.BankId == bankId);
            }

            return specification;
        }
    }
}
