using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryAgg
{
    public class LoanGuarantorAttachmentHistory : Entity
    {
        public Guid SourceCustomerAccountId { get; set; }

        public virtual CustomerAccount SourceCustomerAccount { get; private set; }

        public decimal PrincipalBalance { get; set; }

        public decimal InterestBalance { get; set; }
        
        public byte Status { get; set; }
    }
}
