using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositAgg
{
    public class FixedDeposit : Entity
    {
        public Guid? FixedDepositTypeId { get; set; }

        public virtual FixedDepositType FixedDepositType { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public byte Category { get; set; }

        public byte MaturityAction { get; set; }

        public decimal Value { get; set; }

        public int Term { get; set; }

        public double Rate { get; set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        public DateTime MaturityDate { get; set; }

        public decimal ExpectedInterest { get; set; }

        public decimal TotalExpected { get; set; }

        public string PaidBy { get; set; }

        public DateTime? PaidDate { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }
    }
}
