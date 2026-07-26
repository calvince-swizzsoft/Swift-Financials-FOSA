using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentLineAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptLineAgg
{
    public static class ReceiptLineFactory
    {

        public static ReceiptLine CreateReceiptLine(Guid receiptId, Guid customerAccountId, string customerAccountNo, string description, decimal amount, Guid creditChartOfAccountId, int accounType, int documentType)
        {

            var receiptLine = new ReceiptLine();

            receiptLine.GenerateNewIdentity();

            receiptLine.ReceiptId = receiptId;
            receiptLine.CustomerAccountNo = customerAccountNo;

            receiptLine.CustomerAccountId = customerAccountId;

            receiptLine.Description = description;
            receiptLine.Amount = amount;
            receiptLine.CreditChartOfAccountId = creditChartOfAccountId;

            //paymentLine.DocumentNo = documentNO;

            receiptLine.AccountType = accounType;

            receiptLine.DocumentType = documentType;

            receiptLine.CreatedDate = DateTime.Now;

          
            return receiptLine;
        }
    }
}
