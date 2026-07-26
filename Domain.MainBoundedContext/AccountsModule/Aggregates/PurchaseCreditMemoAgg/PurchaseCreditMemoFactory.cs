using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoAgg
{
    public static class PurchaseCreditMemoFactory
    {

        public static PurchaseCreditMemo CreatePurchaseCreditMemo(string no, string vendorNo, string vendorName, string vendorAddress, DateTime documentDate, DateTime postingDate, DateTime dueDate, string approvalStatus, decimal totalAmount, Guid purchaseInvoiceId, string purchaseInvoiceNo, ServiceHeader serviceHeader)
        {
            var purchaseCreditMemo = new PurchaseCreditMemo();

            //purchaseCreditMemo.InvoiceNo = purchaseInvoiceNo;
            purchaseCreditMemo.PurchaseInvoiceId = purchaseInvoiceId;


            purchaseCreditMemo.No = no;
            
            purchaseCreditMemo.VendorNo = vendorNo;
            purchaseCreditMemo.VendorName = vendorName;
            purchaseCreditMemo.VendorAddress = vendorAddress;
            purchaseCreditMemo.DocumentDate = documentDate;
            purchaseCreditMemo.PostingDate = postingDate;
            purchaseCreditMemo.DueDate = dueDate;
            purchaseCreditMemo.ApprovalStatus = approvalStatus;
            //purchaseCreditMemo.PaidAmount = paidAmount;
            //purchaseCreditMemo.RemainingAmount = remainingAmount;
            purchaseCreditMemo.TotalAmount = totalAmount;
            purchaseCreditMemo.CreatedDate = DateTime.Now;

            purchaseCreditMemo.PurchaseInvoiceNo = purchaseInvoiceNo;

            purchaseCreditMemo.GenerateNewIdentity();

            return purchaseCreditMemo;

        }

    }
}
