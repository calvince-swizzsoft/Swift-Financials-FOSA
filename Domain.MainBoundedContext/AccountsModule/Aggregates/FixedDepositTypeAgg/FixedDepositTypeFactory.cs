using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg
{
    public static class FixedDepositTypeFactory
    {
        public static FixedDepositType CreateFixedDepositType(string description, int months)
        {
            var fixedDepositType = new FixedDepositType();

            fixedDepositType.GenerateNewIdentity();

            fixedDepositType.Description = description;

            fixedDepositType.Months = months;

            fixedDepositType.CreatedDate = DateTime.Now;

            return fixedDepositType;
        }
    }
}
