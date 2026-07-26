using Domain.Seedwork;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class LoanInterest : ValueObject<LoanInterest>
    {
        public double AnnualPercentageRate { get; private set; }

        public short ChargeMode { get; private set; }

        public short RecoveryMode { get; private set; }

        public short CalculationMode { get; private set; }

        public LoanInterest(double annualPercentageRate, int chargeMode, int recoveryMode, int calculationMode)
        {
            this.AnnualPercentageRate = annualPercentageRate;
            this.ChargeMode = (short)chargeMode;
            this.RecoveryMode = (short)recoveryMode;
            this.CalculationMode = (short)calculationMode;
        }

        private LoanInterest()
        {

        }
    }
}
