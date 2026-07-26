using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FuneralRiderClaimAgg
{
    public static class FuneralRiderClaimFactory
    {
        public static FuneralRiderClaim CreateFuneralRiderClaim(Guid customerId, Guid branchId, FuneralRiderClaimant funeralRiderClaimant, ImmediateSuperior immediateSuperior, AreaChief areaChief, AreaDelegate areaDelegate, AreaBoardMember areaBoardMember, int status, int claimType, DateTime claimDate, DateTime dateOfDeath, string fileName, string title, string description, string mimeType, string remarks)
        {
            var funeralRiderClaim = new FuneralRiderClaim();

            funeralRiderClaim.GenerateNewIdentity();

            funeralRiderClaim.CustomerId = customerId;

            funeralRiderClaim.BranchId = branchId;

            funeralRiderClaim.FuneralRiderClaimant = funeralRiderClaimant;

            funeralRiderClaim.ImmediateSuperior = immediateSuperior;

            funeralRiderClaim.AreaChief = areaChief;

            funeralRiderClaim.AreaDelegate = areaDelegate;

            funeralRiderClaim.AreaBoardMember = areaBoardMember;

            funeralRiderClaim.Status = (byte)status;

            funeralRiderClaim.ClaimType = (byte)claimType;

            funeralRiderClaim.ClaimDate = claimDate;

            funeralRiderClaim.DateOfDeath = dateOfDeath;

            funeralRiderClaim.FileName = fileName;

            funeralRiderClaim.FileTitle = title;

            funeralRiderClaim.FileDescription = description;

            funeralRiderClaim.FileMIMEType = mimeType;

            funeralRiderClaim.Remarks = remarks;

            funeralRiderClaim.CreatedDate = DateTime.Now;

            return funeralRiderClaim;
        }
    }
}
