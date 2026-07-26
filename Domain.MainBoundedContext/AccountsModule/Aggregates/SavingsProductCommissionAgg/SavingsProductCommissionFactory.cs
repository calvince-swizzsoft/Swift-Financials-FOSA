using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductCommissionAgg
{
    public static class SavingsProductCommissionFactory
    {
        public static SavingsProductCommission CreateSavingsProductCommission(Guid savingsProductId, Guid commissionId, int savingsProductKnownChargeType, int chargeBenefactor)
        {
            var savingsProductCommission = new SavingsProductCommission();

            savingsProductCommission.GenerateNewIdentity();

            savingsProductCommission.SavingsProductId = savingsProductId;

            savingsProductCommission.CommissionId = commissionId;

            savingsProductCommission.KnownChargeType = savingsProductKnownChargeType;

            savingsProductCommission.ChargeBenefactor = (byte)chargeBenefactor;

            savingsProductCommission.CreatedDate = DateTime.Now;

            return savingsProductCommission;
        }
    }
}
