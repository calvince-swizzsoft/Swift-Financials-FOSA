using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Charge : ValueObject<Charge>
    {
        public byte Type { get; private set; }

        public double Percentage { get; private set; }

        public decimal FixedAmount { get; private set; }

        public Charge(int type, double percentage, decimal fixedAmount)
        {
            this.Type = (byte)type;
            this.Percentage = percentage;
            this.FixedAmount = fixedAmount;

            switch ((ChargeType)this.Type)
            {
                case ChargeType.Percentage:
                    this.FixedAmount = 0m;
                    break;
                case ChargeType.FixedAmount:
                    this.Percentage = 0d;
                    break;
                default:
                    break;
            }
        }

        private Charge()
        { }
    }
}
