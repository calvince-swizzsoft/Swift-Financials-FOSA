using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg
{
    public class PurchaseInvoiceLineFactory
    {
        public static PurchaseInvoiceLine CreatePurchaseInvoiceLine(Guid purchaseInvoiceId, int type, int no, string description, decimal unitCost, int quantity, decimal totalAmount, Guid debitChartOfAccountId)
        {

            var purchaseInvoiceLine = new PurchaseInvoiceLine();

            purchaseInvoiceLine.GenerateNewIdentity();


            purchaseInvoiceLine.PurchaseInvoiceId = purchaseInvoiceId;

            //purchaseInvoiceLine.PurchaseInvoiceNo = purchaseInvoiceNo;

            purchaseInvoiceLine.Type = type;
            purchaseInvoiceLine.No = no;
            purchaseInvoiceLine.Description = description;
            purchaseInvoiceLine.UnitCost = unitCost;
            purchaseInvoiceLine.Quantity = quantity;
            purchaseInvoiceLine.Amount = totalAmount;
            
            purchaseInvoiceLine.DebitChartOfAccountId = debitChartOfAccountId;


            purchaseInvoiceLine.CreatedDate = DateTime.Now;

            return purchaseInvoiceLine; 
        }

    }
}
