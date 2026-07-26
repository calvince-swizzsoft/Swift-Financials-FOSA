using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentLineAgg
{
    public class PaymentLineFactory
    {

        public static PaymentLine CreatePaymentLine(Guid paymentId, int type, int no, string description, decimal amount, Guid chartOfAccountId, int accounType,int documentType)
        {

            var paymentLine = new PaymentLine();

            paymentLine.GenerateNewIdentity();

            paymentLine.PaymentId = paymentId;
            //purchaseInvoiceLine.PurchaseInvoiceNo = purchaseInvoiceNo;
            paymentLine.AccountType = type;
            paymentLine.No = no;
            paymentLine.Description = description;
            paymentLine.Amount = amount;

            paymentLine.ChartOfAccountId = chartOfAccountId;

            //paymentLine.DocumentNo = documentNO;

            paymentLine.AccountType = accounType;

            paymentLine.DocumentType = documentType;

            paymentLine.CreatedDate = DateTime.Now;

            return paymentLine;
        }
    }
}
