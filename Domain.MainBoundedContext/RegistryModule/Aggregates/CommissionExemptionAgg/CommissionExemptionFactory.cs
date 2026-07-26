using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CommissionExemptionAgg
{
    public static class CommissionExemptionFactory
    {
        public static CommissionExemption CreateCommissionExemption(Guid commissionId, string description)
        {
            var commissionExemption = new CommissionExemption();

            commissionExemption.GenerateNewIdentity();

            commissionExemption.CommissionId = commissionId;

            commissionExemption.Description = description;

            commissionExemption.CreatedDate = DateTime.Now;

            return commissionExemption;
        }
    }
}
