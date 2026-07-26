using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates
{
    public class ImprestSurrenderLine : Entity
    {


       public Guid SurrenderId { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string ExpenseCategory { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string ReceiptNo { get; set; }

        public Guid CreditBankChardOfAccountId { get; set; }
    }
}
