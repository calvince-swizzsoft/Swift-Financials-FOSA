using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.AccountClosureRequestAgg
{
    public class AccountClosureRequest : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public string Reason { get; set; }

        public byte Status { get; set; }

        public string ApprovedBy { get; set; }

        public string ApprovalRemarks { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }
        
        public string SettledBy { get; set; }

        public DateTime? SettledDate { get; set; }
    }
}
