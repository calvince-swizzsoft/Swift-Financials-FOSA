using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg
{
    public class CreditType : Domain.Seedwork.Entity
    {
        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public string Description { get; set; }

        public byte TransactionOwnership { get; set; }
        
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
