using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceLineAgg
{
    public class SalesInvoiceLineFactory
    {

        public static SalesInvoiceLine CreateSalesInvoiceLine(Guid salesInvoiceId, int type, int no, string description, decimal unitCost, int quantity, decimal totalAmount, Guid creditChartOfAccountId)
        {

            var salesInvoiceLine = new SalesInvoiceLine();

            salesInvoiceLine.GenerateNewIdentity();


            salesInvoiceLine.SalesInvoiceId = salesInvoiceId;

            //purchaseInvoiceLine.PurchaseInvoiceNo = purchaseInvoiceNo;

            salesInvoiceLine.Type = type;
            salesInvoiceLine.No = no;
            salesInvoiceLine.Description = description;
            salesInvoiceLine.UnitCost = unitCost;
            salesInvoiceLine.Quantity = quantity;
            salesInvoiceLine.Amount = totalAmount;

            salesInvoiceLine.CreditChartOfAccountId = creditChartOfAccountId;


            salesInvoiceLine.CreatedDate = DateTime.Now;

            return salesInvoiceLine;
        }

    }
}
