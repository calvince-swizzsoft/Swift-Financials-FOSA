using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeGraduatedScaleAgg
{
    public static class FixedDepositTypeGraduatedScaleFactory
    {
        public static FixedDepositTypeGraduatedScale CreateFixedDepositTypeGraduatedScale(Guid fixedDepositTypeId, Range range, double percentage)
        {
            var fixedDepositTypeGraduatedScale = new FixedDepositTypeGraduatedScale();

            fixedDepositTypeGraduatedScale.GenerateNewIdentity();

            fixedDepositTypeGraduatedScale.FixedDepositTypeId = fixedDepositTypeId;

            fixedDepositTypeGraduatedScale.Range = range;

            fixedDepositTypeGraduatedScale.Percentage = percentage;

            fixedDepositTypeGraduatedScale.CreatedDate = DateTime.Now;

            return fixedDepositTypeGraduatedScale;
        }
    }
}
