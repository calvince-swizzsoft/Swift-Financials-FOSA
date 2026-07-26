using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptAgg
{
    public static class ReceiptFactory
    {

        public static Receipt CreateReceipt(Guid invoiceId, Guid customerId, string description, string reference, decimal totalAmount, string paymentMethod, Guid bankLinkageChartOfAccountd, string receiptNumber, string salesInvoiceNo, string customerNo)
        {
            var receipt = new Receipt
            {
              
            };

            receipt.GenerateNewIdentity();
            receipt.ReceiptNo = receiptNumber;

            receipt.Description = description;

            receipt.Reference = reference;

            receipt.PaymentMethod = paymentMethod;

            receipt.CustomerId = customerId;

            receipt.TotalAmount = totalAmount;

            receipt.CreatedDate = DateTime.Now;

            receipt.BankLinkageChartOfAccountId = bankLinkageChartOfAccountd;

            receipt.SalesInvoiceNo = salesInvoiceNo;

            receipt.CustomerNo = customerNo;

            return receipt;
        }
    }
}
