using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class RecurringBatchEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid RecurringBatchId { get; set; }

        public Guid? CustomerAccountId { get; set; }

        public Guid? SecondaryCustomerAccountId { get; set; }

        public Guid? StandingOrderId { get; set; }

        public Guid? ElectronicStatementOrderId { get; set; }

        public DateTime ElectronicStatement_StartDate { get; set; }

        public DateTime ElectronicStatement_EndDate { get; set; }

        public string ElectronicStatementSender { get; set; }

        public string Reference { get; set; }

        public string Remarks { get; set; }

        public byte InterestCapitalizationMonths { get; set; }

        public bool EnforceCeiling { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
