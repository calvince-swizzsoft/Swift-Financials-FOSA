using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ChartOfAccountSummaryDTO
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? CostCenterId { get; set; }

        public short AccountType { get; set; }

        public short AccountCategory { get; set; }

        public int AccountCode { get; set; }

        public string AccountName { get; set; }

        public int Depth { get; set; }

        public bool IsControlAccount { get; set; }

        public bool IsReconciliationAccount { get; set; }

        public bool PostAutomaticallyOnly { get; set; }

        public bool IsLocked { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
