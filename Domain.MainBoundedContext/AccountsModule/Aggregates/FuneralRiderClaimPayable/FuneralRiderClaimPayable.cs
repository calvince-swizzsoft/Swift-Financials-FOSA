using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FuneralRiderClaimAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FuneralRiderClaimPayable
{
    public class FuneralRiderClaimPayable : Entity
    {
        public Guid FuneralRiderClaimId { get; set; }

        public virtual FuneralRiderClaim FuneralRiderClaim { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public int ReferenceNumber { get; set; }

        public decimal Amount { get; set; }

        public byte RecordStatus { get; set; }

        public byte PaymentStatus { get; set; }

        public string Remarks { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

    }
}
