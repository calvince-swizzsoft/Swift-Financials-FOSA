namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg
{
    public class DynamicCharge : Domain.Seedwork.Entity
    {
        public string Description { get; set; }

        public short RecoveryMode { get; set; }

        public short RecoverySource { get; set; }
        
        public short Installments { get; set; }

        public short InstallmentsBasisValue { get; set; }

        public bool FactorInLoanTerm { get; set; }

        public bool ComputeChargeOnTopUp { get; set; }

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
