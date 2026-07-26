using Domain.Seedwork;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Collateral : ValueObject<Collateral>
    {
        public decimal Value { get; private set; }

        public double AdvanceRate { get; private set; }

        public byte Status { get; private set; }

        public Collateral(decimal value, double advanceRate, int status)
        {
            this.Value = value;
            this.AdvanceRate = advanceRate;
            this.Status = (byte)status;
        }

        private Collateral()
        { }
    }
}
