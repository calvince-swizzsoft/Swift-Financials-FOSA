using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.SuperSaverPayableAgg
{
    public class SuperSaverPayable : Entity
    {
        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public int VoucherNumber { get; set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public decimal BookBalance { get; set; }

        public decimal Amount { get; set; } 

        public decimal WithholdingTaxAmount { get; set; } 

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }
    }
}
