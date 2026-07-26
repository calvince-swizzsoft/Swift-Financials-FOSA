using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestLineAgg
{
    public class ImprestLineFactory
    {

        public Guid ImprestId { get; set; }

        public string ExpenseCategory { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public static ImprestLine CreateImprestLine(Guid imprestId, string expenseCategory, string Description, decimal Amount, Guid ImprestDebtorsChartOfAccountId, Guid BankChartOfAccountId, Guid ExpenseChartOfAccountId)
        {

            var imprestLine = new ImprestLine();

            imprestLine.GenerateNewIdentity();


            imprestLine.ImprestId = imprestId;

            imprestLine.ExpenseCategory = expenseCategory;
            imprestLine.Description = Description;
            imprestLine.Amount = Amount;

            imprestLine.ImprestDebtorsChartOfAccountId = ImprestDebtorsChartOfAccountId;

            imprestLine.ExpensChartOfAccountId = ExpenseChartOfAccountId;
            imprestLine.CreatedDate = DateTime.Now;

            

            return imprestLine;
        }


        public static ImprestLine UpdateImprestLine(decimal amountSpent, decimal amountReturned, decimal reimbursementAmount, Guid BankChartOfAccountId, Guid ExpenseChartOfAccountId)
        {

            var imprestLine = new ImprestLine();

            imprestLine.AmountSpent = amountSpent;

            imprestLine.AmountReturned = amountReturned;

            imprestLine.ReimbursementAmount = reimbursementAmount;
            imprestLine.BankChartOfAccountId = BankChartOfAccountId;
            imprestLine.ExpensChartOfAccountId = ExpenseChartOfAccountId;
            imprestLine.CreatedDate = DateTime.Now;

            return imprestLine;
        }

    }
}
