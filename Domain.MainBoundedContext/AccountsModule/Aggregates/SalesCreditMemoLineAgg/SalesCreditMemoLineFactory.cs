using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoLineAgg
{
    public class SalesCreditMemoLineFactory
    {
        public static SalesCreditMemoLine CreateSalesCreditMemoLine(Guid salesCreditMemoId, int type, int no, string description, int quantity, decimal amount, Guid debitChartOfAccountId, decimal unitCost)
        {

            var salesCreditMemoLine = new SalesCreditMemoLine();
            
            salesCreditMemoLine.GenerateNewIdentity();


            salesCreditMemoLine.SalesCreditMemoId = salesCreditMemoId;

            //purchaseInvoiceLine.PurchaseInvoiceNo = purchaseInvoiceNo;

            salesCreditMemoLine.Type = type;
            salesCreditMemoLine.No = no;
            salesCreditMemoLine.Description = description;
            salesCreditMemoLine.Quantity = quantity;
            salesCreditMemoLine.Amount = amount;
            salesCreditMemoLine.UnitCost = unitCost;

            salesCreditMemoLine.DebitChartOfAccountId = debitChartOfAccountId;


            salesCreditMemoLine.CreatedDate = DateTime.Now;

            return salesCreditMemoLine; 
        }

    }
}
