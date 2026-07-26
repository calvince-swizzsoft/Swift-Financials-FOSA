using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankLinkageAgg
{
    public class BankLinkage : Domain.Seedwork.Entity
    {
        public Guid BranchId { get; set; }

        public Guid BankId { get; set; }


        public virtual Branch Branch { get; private set; }

        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public string BankName { get; set; }

        public string BankBranchName { get; set; }

        public string BankAccountNumber { get; set; }

        public string Remarks { get; set; }

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
