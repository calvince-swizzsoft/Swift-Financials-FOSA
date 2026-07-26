using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class StandingOrderBulkCopyDTO
    {
        public Guid Id { get; set; }

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

        public byte Trigger { get; set; }

        public decimal LoanAmount { get; set; }

        public decimal PaymentPerPeriod { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }

        public decimal CapitalizedInterest { get; set; }

        public string Remarks { get; set; }

        public bool ForceExecute { get; set; }

        public bool Chargeable { get; set; }

        public bool IsLocked { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
