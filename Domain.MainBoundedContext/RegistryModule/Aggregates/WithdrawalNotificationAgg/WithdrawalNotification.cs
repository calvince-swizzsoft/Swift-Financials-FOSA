using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalNotificationAgg
{
    public class WithdrawalNotification : Domain.Seedwork.Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public int Category { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public DateTime MaturityDate { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string ApprovalRemarks { get; set; }

        public string AuditedBy { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuditRemarks { get; set; }

        public string SettledBy { get; set; }

        public DateTime? SettledDate { get; set; }

        public string SettlementRemarks { get; set; }

        public byte SettlementType { get; set; }

        public bool IsLocked { get; private set; }
        
        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
