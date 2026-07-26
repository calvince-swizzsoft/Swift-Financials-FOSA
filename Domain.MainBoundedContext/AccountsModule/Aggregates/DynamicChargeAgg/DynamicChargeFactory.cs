using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg
{
    public static class DynamicChargeFactory
    {
        public static DynamicCharge CreateDynamicCharge(string description, int recoveryMode, int recoverySource, int installmentsBasisValue, int installments, bool factorInLoanTerm, bool computeChargeOnTopUp)
        {
            var dynamicCharge = new DynamicCharge();

            dynamicCharge.GenerateNewIdentity();

            dynamicCharge.Description = description;

            dynamicCharge.RecoveryMode = (short)recoveryMode;

            dynamicCharge.RecoverySource = (short)recoverySource;

            dynamicCharge.InstallmentsBasisValue = (short)installmentsBasisValue;

            dynamicCharge.Installments = (short)installments;

            dynamicCharge.FactorInLoanTerm = factorInLoanTerm;

            dynamicCharge.ComputeChargeOnTopUp = computeChargeOnTopUp;

            dynamicCharge.CreatedDate = DateTime.Now;

            return dynamicCharge;
        }
    }
}
