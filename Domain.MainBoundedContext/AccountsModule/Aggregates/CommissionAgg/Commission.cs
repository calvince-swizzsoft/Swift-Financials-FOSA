using Domain.Seedwork;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg
{
    public class Commission : Entity
    {
        public string Description { get; set; }

        public decimal MaximumCharge { get; set; }

        public byte RoundingType { get; set; }

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
