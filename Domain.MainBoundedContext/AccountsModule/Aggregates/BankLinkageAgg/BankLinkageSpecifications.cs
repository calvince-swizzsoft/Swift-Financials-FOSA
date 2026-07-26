using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankLinkageAgg
{
    public static class BankLinkageSpecifications
    {
        public static Specification<BankLinkage> DefaultSpec()
        {
            Specification<BankLinkage> specification = new TrueSpecification<BankLinkage>();

            return specification;
        }


        public static ISpecification<BankLinkage> BankLinkageWithBankAccountId(Guid BankAccountId)
        {
            Specification<BankLinkage> specification = new DirectSpecification<BankLinkage>(x => x.BankId == BankAccountId);

            return specification;
        }

        public static Specification<BankLinkage> BankLinkageFullText(string text)
        {
            Specification<BankLinkage> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var bankNameSpec = new DirectSpecification<BankLinkage>(c => c.BankName.Contains(text));

                var bankBranchNameSpec = new DirectSpecification<BankLinkage>(c => c.BankBranchName.Contains(text));

                var bankAccountNumberSpec = new DirectSpecification<BankLinkage>(c => c.BankAccountNumber.Contains(text));

                var remarksSpec = new DirectSpecification<BankLinkage>(c => c.Remarks.Contains(text));

                specification &= (bankNameSpec | bankBranchNameSpec | bankAccountNumberSpec | remarksSpec);
            }

            return specification;
        }
    }
}
