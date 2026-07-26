using Domain.MainBoundedContext.AccountsModule.Aggregates;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestAgg
{
    public class Imprest : Entity
    {
        public String No { get; set; }

        public string Amount { get; set; }

        public DateTime RequestDate { get; set; }

        public string EmployeeNo { get; set; }

        public string EmployeeName { get; set; }

        public decimal AmountRequested { get; set; }

        public string Purpose { get; set; }

        public bool Surrendered { get; set; }

        public DateTime SurrenderDate { get; set; }


        public Guid BankChartOfAccountId { get; set; }


       public String Status { get; set; }

        HashSet<ImprestLine> _imprestLines;

        public virtual ICollection<ImprestLine> ImprestLines
        {
            get
            {
                if (_imprestLines == null)
                {
                    _imprestLines = new HashSet<ImprestLine>();
                }
                return _imprestLines;
            }
            private set
            {
                _imprestLines = new HashSet<ImprestLine>(value);
            }
        }

        public Boolean Posted { get; set; }

        public void AddLine(int lineNo, string expenseCategory, string description, decimal amount, Guid imprestDebtorsChartOfAccountId, Guid bankChartOfAccountId, Guid expenseChartOfAccountId)
        {
            var imprestLine = ImprestLineFactory.CreateImprestLine(this.Id, expenseCategory, description, amount,imprestDebtorsChartOfAccountId, bankChartOfAccountId,  expenseChartOfAccountId);
            this.ImprestLines.Add(imprestLine);
        }


        public void UpdateLine(decimal requestedAmount, decimal amountSpent, decimal amountReturned, decimal reimbursementAmount, Guid expenseChartOfAccountId)
        {
            //var imprestLine = ImprestLineFactory.CreateImprestLine(this.Id, expenseCategory, description, amount, imprestDebtorsChartOfAccountId, bankChartOfAccountId, expenseChartOfAccountId);

            var imprestLine = ImprestLineFactory.UpdateImprestLine(amountSpent, amountReturned, reimbursementAmount, expenseChartOfAccountId, this.BankChartOfAccountId);

        }







    }
}
