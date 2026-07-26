using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg
{
    public class CustomerAccount : Domain.Seedwork.Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public virtual CustomerAccountType CustomerAccountType { get; set; }

        public short ScoredLoanDisbursementProductCode { get; set; }

        public decimal ScoredLoanLimit { get; set; }

        public string ScoredLoanLimitRemarks { get; set; }

        public DateTime? ScoredLoanLimitDate { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public byte RecordStatus { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string SigningInstructions { get; set; }

        //public Guid ProductChartOfAccountId { get; set; }
    }
}
