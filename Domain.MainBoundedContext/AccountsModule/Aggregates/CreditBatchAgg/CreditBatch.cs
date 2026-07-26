using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg
{
    public class CreditBatch : Entity
    {
        public Guid CreditTypeId { get; set; }

        public virtual CreditType CreditType { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }
        
        public int BatchNumber { get; set; }

        public int Type { get; set; }

        public decimal TotalValue { get; set; }

        public byte Month { get; set; }

        public DateTime? ValueDate { get; set; }

        public string Reference { get; set; }

        public virtual Charge Concession { get; set; }

        public bool RecoverIndefiniteCharges { get; set; }

        public bool RecoverArrearages { get; set; }

        public bool RecoverCarryForwards { get; set; }

        public bool PreserveAccountBalance { get; set; }

        public bool FuzzyMatching { get; set; }

        public bool EnforceMonthValueDate { get; set; }

        public byte Priority { get; set; }

        public byte Status { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }
    }
}
