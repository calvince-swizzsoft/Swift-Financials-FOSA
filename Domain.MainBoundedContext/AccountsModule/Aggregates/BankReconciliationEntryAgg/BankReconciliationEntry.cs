using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationEntryAgg
{
    public class BankReconciliationEntry : Entity
    {
        public Guid BankReconciliationPeriodId { get; set; }

        public virtual BankReconciliationPeriod BankReconciliationPeriod { get; private set; }

        public Guid? ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public int AdjustmentType { get; set; }

        public decimal Value { get; set; }

        public string ChequeNumber { get; set; }

        public string ChequeDrawee { get; set; }

        public DateTime? ChequeDate { get; set; }

        public string Remarks { get; set; }

        

        
    }
}
