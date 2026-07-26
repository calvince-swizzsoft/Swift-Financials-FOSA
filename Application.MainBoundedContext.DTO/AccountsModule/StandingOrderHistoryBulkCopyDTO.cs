using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class StandingOrderHistoryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid StandingOrderId { get; set; }

        public Guid PostingPeriodId { get; set; }

        public Guid BenefactorCustomerAccountId { get; set; }

        public Guid BeneficiaryCustomerAccountId { get; set; }

        public DateTime Duration_StartDate { get; set; }

        public DateTime Duration_EndDate { get; set; }

        public short Schedule_Frequency { get; set; }

        public DateTime Schedule_ExpectedRunDate { get; set; }

        public DateTime Schedule_ActualRunDate { get; set; }

        public byte Schedule_ExecuteAttemptCount { get; set; }

        public bool Schedule_IsExecuted { get; set; }

        public byte Charge_Type { get; set; }

        public double Charge_Percentage { get; set; }

        public decimal Charge_FixedAmount { get; set; }

        public byte Month { get; set; }

        public byte Trigger { get; set; }

        public decimal ExpectedPrincipal { get; set; }

        public decimal ExpectedInterest { get; set; }

        public decimal ActualPrincipal { get; set; }

        public decimal ActualInterest { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
