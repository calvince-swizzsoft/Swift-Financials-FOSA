using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestSurrenderLineAgg
{
    public class ImprestSurrenderLineFactory
    {

        public static ImprestSurrenderLine CreateImprestSurrenderLine(Guid surrenderId, DateTime expenseDate, string expenseCategory, string Description, decimal Amount, string ReceiptNo, Guid CreditChartOfAccountId)
        {
            var surrenderLine = new  ImprestSurrenderLine();


            surrenderLine.SurrenderId = surrenderId;

            surrenderLine.ExpenseDate = expenseDate;

            surrenderLine.ExpenseCategory = expenseCategory;

            surrenderLine.Description = Description;

            surrenderLine.Amount = Amount;

            surrenderLine.ReceiptNo = ReceiptNo;

            surrenderLine.CreditBankChardOfAccountId = CreditChartOfAccountId;


            surrenderLine.CreatedDate = DateTime.Now;


            surrenderLine.GenerateNewIdentity();

            return surrenderLine;


        }
    }
}
