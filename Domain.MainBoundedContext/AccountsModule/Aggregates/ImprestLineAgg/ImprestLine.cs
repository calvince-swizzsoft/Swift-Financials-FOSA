using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates
{
    public class ImprestLine : Entity
    {

        public Guid ImprestId { get; set; }

        public string ExpenseCategory { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public Guid ImprestDebtorsChartOfAccountId { get; set; }

        public Guid ExpensChartOfAccountId { get; set; }

        public Guid BankChartOfAccountId { get; set;}

        public Decimal AmountSpent { get; set; }

        public Decimal AmountReturned { get; set; }

        public Decimal ReimbursementAmount { get; set; }

    }
}
