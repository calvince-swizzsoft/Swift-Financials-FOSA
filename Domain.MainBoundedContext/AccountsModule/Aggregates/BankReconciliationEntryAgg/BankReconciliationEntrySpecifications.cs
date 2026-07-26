using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationEntryAgg
{
    public static class BankReconciliationEntrySpecifications
    {
        public static Specification<BankReconciliationEntry> DefaultSpec()
        {
            Specification<BankReconciliationEntry> specification = new TrueSpecification<BankReconciliationEntry>();

            return specification;
        }

        public static Specification<BankReconciliationEntry> BankReconciliationEntryFullText(Guid bankReconciliationPeriodId, string text)
        {
            Specification<BankReconciliationEntry> specification = new DirectSpecification<BankReconciliationEntry>(x => x.BankReconciliationPeriodId == bankReconciliationPeriodId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var chequeNumberSpec = new DirectSpecification<BankReconciliationEntry>(c => c.ChequeNumber.Contains(text));

                var chequeDraweeSpec = new DirectSpecification<BankReconciliationEntry>(c => c.ChequeDrawee.Contains(text));

                var remarksSpec = new DirectSpecification<BankReconciliationEntry>(c => c.Remarks.Contains(text));

                var createdBySpec = new DirectSpecification<BankReconciliationEntry>(c => c.CreatedBy.Contains(text));

                specification &= (chequeNumberSpec | chequeDraweeSpec | remarksSpec | createdBySpec);
            }

            return specification;
        }
    }
}
