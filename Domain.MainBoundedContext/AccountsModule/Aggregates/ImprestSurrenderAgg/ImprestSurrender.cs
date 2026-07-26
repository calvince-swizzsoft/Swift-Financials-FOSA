using Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestSurrenderLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestSurrender
{
    public class ImprestSurrender : Entity
    {

        public string SurrenderNo { get; set; }

        public Guid ImprestId { get; set; }

        public DateTime SurrenderDate { get; set; }

        public decimal AmountIssued { get; set; }

        public decimal AmountSpent { get; set; }

        public decimal AmountReturned { get; set; }

        public decimal ReimbursableAmount { get; set; }

        public Guid ExpenseChartOfAccountId { get; set; }

        public String Status { get; set; }

        //public HashSet<ImprestSurrenderLine> Lines { get; set; } = new HashSet<ImprestSurrenderLine>();


        HashSet<ImprestSurrenderLine> _imprestSurrenderLines;

        public virtual ICollection<ImprestSurrenderLine> ImprestLines
        {
            get
            {
                if (_imprestSurrenderLines == null)
                {
                    _imprestSurrenderLines = new HashSet<ImprestSurrenderLine>();
                }
                return _imprestSurrenderLines;
            }
            private set
            {
                _imprestSurrenderLines = new HashSet<ImprestSurrenderLine>(value);
            }
        }

        public Boolean Posted { get; set; }

        public void AddLine(Guid ImprestId,DateTime surrenderDate, string expenseCategory, string description, decimal amount, string ReceiptNo, Guid expenseChartOfAccountId)
        {
            // var imprestLine = ImprestSurrenderLineFactory.CreateImprestLine(this.Id, expenseCategory, description, amount, expenseChartOfAccountId);
            // this.ImprestLines.Add(imprestLine);

            var imprestSurrenderLine = ImprestSurrenderLineFactory.CreateImprestSurrenderLine(ImprestId, SurrenderDate, expenseCategory, description, amount, ReceiptNo, expenseChartOfAccountId);
        }
    }
}
