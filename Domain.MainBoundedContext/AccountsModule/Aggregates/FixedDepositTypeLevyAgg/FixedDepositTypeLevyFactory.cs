using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeLevyAgg
{
    public static class FixedDepositTypeLevyFactory
    {
        public static FixedDepositTypeLevy CreateFixedDepositTypeLevy(Guid fixedDepositTypeId, Guid levyId)
        {
            var fixedDepositTypeLevy = new FixedDepositTypeLevy();

            fixedDepositTypeLevy.GenerateNewIdentity();

            fixedDepositTypeLevy.FixedDepositTypeId = fixedDepositTypeId;

            fixedDepositTypeLevy.LevyId = levyId;

            fixedDepositTypeLevy.CreatedDate = DateTime.Now;

            return fixedDepositTypeLevy;
        }
    }
}
