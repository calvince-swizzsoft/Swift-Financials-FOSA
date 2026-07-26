using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchAgg
{
    public class DebitBatch : Entity
    {
        public Guid DebitTypeId { get; set; }

        public virtual DebitType DebitType { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public int BatchNumber { get; set; }

        public string Reference { get; set; }

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
