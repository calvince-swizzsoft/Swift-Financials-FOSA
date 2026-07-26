using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FuneralRiderClaimAgg
{
    public class FuneralRiderClaim : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public virtual FuneralRiderClaimant FuneralRiderClaimant { get; set; }

        public virtual ImmediateSuperior ImmediateSuperior { get; set; }

        public virtual AreaChief AreaChief { get; set; }

        public virtual AreaDelegate AreaDelegate { get; set; }

        public virtual AreaBoardMember AreaBoardMember { get; set; }

        public byte Status { get; set; }

        public byte ClaimType { get; set; }

        public DateTime ClaimDate { get; set; }

        public DateTime DateOfDeath { get; set; }

        public string FileName { get; set; }

        public string FileTitle { get; set; }

        public string FileDescription { get; set; }

        public string FileMIMEType { get; set; }

        public virtual File File { get; set; }        

        public string Remarks { get; set; }
    }
}
