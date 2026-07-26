using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoAgg
{
    public class SalesCreditMemoFactory
    {

        public static SalesCreditMemo CreateSalesCreditMemo (string salesCreditMemoNo, string customerNo, string customerName, string customerAddress, DateTime documentDate, DateTime postingDate, DateTime dueDate, Guid salesInvoiceId, string salesCreditMemoAppliesToDocNo, string approvalStatus, decimal totalAmount, ServiceHeader serviceHeader) 
        {


            var salesCreditMemo = new SalesCreditMemo();

            //purchaseInvoice.No = no;

            salesCreditMemo.TotalAmount = totalAmount;
            salesCreditMemo.No = salesCreditMemoNo;
            salesCreditMemo.CustomerNo = customerNo;
            salesCreditMemo.CustomerName = customerName;
            salesCreditMemo.CustomerAddress = customerAddress;
            salesCreditMemo.DocumentDate = documentDate;
            salesCreditMemo.PostingDate = postingDate;
            salesCreditMemo.DueDate = dueDate;
            salesCreditMemo.SalesInvoiceId = salesInvoiceId;
            salesCreditMemo.AppliesToDocNo = salesCreditMemoAppliesToDocNo;
            salesCreditMemo.ApprovalStatus = approvalStatus;
            salesCreditMemo.CreatedDate = DateTime.Now;

            salesCreditMemo.GenerateNewIdentity();

            return salesCreditMemo;

        }
    }
}
