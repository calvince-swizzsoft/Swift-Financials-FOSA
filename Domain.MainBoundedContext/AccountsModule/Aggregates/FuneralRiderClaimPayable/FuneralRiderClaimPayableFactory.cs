using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FuneralRiderClaimPayable
{
    public static class FuneralRiderClaimPayableFactory
    {
        public static FuneralRiderClaimPayable CreateFuneralRiderClaimPayable(Guid funeralRiderClaimId, Guid branchId, decimal amount, string remarks)
        {
            var funeralRiderClaimPayable = new FuneralRiderClaimPayable();

            funeralRiderClaimPayable.GenerateNewIdentity();

            funeralRiderClaimPayable.FuneralRiderClaimId = funeralRiderClaimId;

            funeralRiderClaimPayable.BranchId = branchId;

            funeralRiderClaimPayable.Amount = amount;

            funeralRiderClaimPayable.Remarks = remarks;

            funeralRiderClaimPayable.CreatedDate = DateTime.Now;

            return funeralRiderClaimPayable;
        }
    }
}
