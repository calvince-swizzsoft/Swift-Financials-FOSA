using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryEntryAgg
{
    public static class LoanGuarantorAttachmentHistoryEntrySpecifications
    {
        public static Specification<LoanGuarantorAttachmentHistoryEntry> DefaultSpec()
        {
            Specification<LoanGuarantorAttachmentHistoryEntry> specification = new TrueSpecification<LoanGuarantorAttachmentHistoryEntry>();

            return specification;
        }

        public static Specification<LoanGuarantorAttachmentHistoryEntry> LoanGuarantorWithLoanGuarantorAttachmentHistoryId(Guid loanGuarantorAttachmentHistoryId)
        {
            Specification<LoanGuarantorAttachmentHistoryEntry> specification = DefaultSpec();

            if (loanGuarantorAttachmentHistoryId != null && loanGuarantorAttachmentHistoryId != Guid.Empty)
            {
                var loanGuarantorAttachmentHistoryIdSpec = new DirectSpecification<LoanGuarantorAttachmentHistoryEntry>(c => c.LoanGuarantorAttachmentHistoryId == loanGuarantorAttachmentHistoryId);

                specification &= loanGuarantorAttachmentHistoryIdSpec;
            }

            return specification;
        }
    }
}
