using Domain.Seedwork;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg
{
    public class FixedDepositType : Entity
    {
        public string Description { get; set; }

        public int Months { get; set; }

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
