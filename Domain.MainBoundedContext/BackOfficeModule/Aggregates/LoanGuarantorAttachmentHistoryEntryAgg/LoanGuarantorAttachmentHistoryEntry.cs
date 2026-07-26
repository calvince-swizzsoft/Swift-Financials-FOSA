using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryEntryAgg
{
    public class LoanGuarantorAttachmentHistoryEntry : Entity
    {
        public Guid LoanGuarantorAttachmentHistoryId { get; set; }

        public virtual LoanGuarantorAttachmentHistory LoanGuarantorAttachmentHistory { get; private set; }

        public Guid LoanGuarantorId { get; set; }

        public virtual LoanGuarantor LoanGuarantor { get; private set; }

        public Guid DestinationCustomerAccountId { get; set; }

        public virtual CustomerAccount DestinationCustomerAccount { get; private set; }

        public decimal PrincipalAttached { get; set; }

        public decimal InterestAttached { get; set; }

        public string Reference { get; set; }
    }
}
