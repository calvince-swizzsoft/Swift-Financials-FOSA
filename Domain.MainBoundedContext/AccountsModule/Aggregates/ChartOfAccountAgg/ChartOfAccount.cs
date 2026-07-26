using Domain.MainBoundedContext.AccountsModule.Aggregates.CostCenterAgg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg
{
    public class ChartOfAccount : Domain.Seedwork.Entity
    {
        public Guid? ParentId { get; set; }

        public virtual ChartOfAccount Parent { get; private set; }

        public Guid? CostCenterId { get; set; }

        public virtual CostCenter CostCenter { get; private set; }

        public short AccountType { get; set; }

        public short AccountCategory { get; set; }

        public int AccountCode { get; set; }

        public string AccountName { get; set; }

        public int Depth { get; set; }

        public bool IsControlAccount { get; set; }

        public bool IsReconciliationAccount { get; set; }

        public bool PostAutomaticallyOnly { get; set; }

        public bool IsLocked { get; private set; }

        

        HashSet<ChartOfAccount> _children;
        public virtual ICollection<ChartOfAccount> Children
        {
            get
            {
                if (_children == null)
                    _children = new HashSet<ChartOfAccount>();
                return _children;
            }
            private set
            {
                _children = new HashSet<ChartOfAccount>(value);
            }
        }

        public void SetParentChartOfAccount(ChartOfAccount chartOfAccount)
        {
            if (chartOfAccount == null || chartOfAccount.IsTransient())
                throw new ArgumentNullException("chartOfAccount");

            this.ParentId = chartOfAccount.Id;
            this.Parent = chartOfAccount;
        }

        public void SetCostCenter(CostCenter costCenter)
        {
            if (costCenter == null || costCenter.IsTransient())
            {
                throw new ArgumentNullException("costCenter");
            }

            // Fix id and set reference
            this.CostCenterId = costCenter.Id;
            this.CostCenter = costCenter;
        }

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
