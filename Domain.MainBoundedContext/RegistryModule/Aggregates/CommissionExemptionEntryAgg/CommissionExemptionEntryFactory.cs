using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionEntryAgg
{
    public static class CommissionExemptionEntryFactory
    {
        public static CommissionExemptionEntry CreateCommissionExemptionEntry(Guid commissionExemptionId, Guid customerId, string remarks)
        {
            var commissionExemptionEntry = new CommissionExemptionEntry();

            commissionExemptionEntry.GenerateNewIdentity();

            commissionExemptionEntry.CommissionExemptionId = commissionExemptionId;

            commissionExemptionEntry.CustomerId = customerId;
            
            commissionExemptionEntry.Remarks = remarks;

            commissionExemptionEntry.CreatedDate = DateTime.Now;

            return commissionExemptionEntry;
        }
    }
}
