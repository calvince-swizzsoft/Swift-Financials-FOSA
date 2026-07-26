using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceAgg
{ 
    public static class SalesInvoiceFactory
    {

       public static SalesInvoice CreateSalesInvoice(string no, string customerNo, string customerName, string customerAddress, DateTime documentDate, DateTime postingDate, DateTime dueDate, string approvalStatus, Decimal paidAmount, Decimal remainingAmount, Decimal totalAmount, ServiceHeader serviceHeader)
        {
            var salesInvoice = new SalesInvoice();

            //purchaseInvoice.No = no
            salesInvoice.No = no;
            salesInvoice.CustomerNo = customerNo;
            salesInvoice.CustomerAddress = customerAddress;
            salesInvoice.CustomerName = customerName;
            salesInvoice.DueDate = dueDate;
            salesInvoice.DocumentDate = documentDate;
            salesInvoice.PostingDate = postingDate;
            salesInvoice.ApprovalStatus = approvalStatus;
            salesInvoice.PaidAmount = paidAmount;
            salesInvoice.RemainingAmount = remainingAmount;
            salesInvoice.TotalAmount = totalAmount;
            salesInvoice.CreatedDate = DateTime.Now;

            salesInvoice.GenerateNewIdentity();

            return salesInvoice;

        }

    }
}