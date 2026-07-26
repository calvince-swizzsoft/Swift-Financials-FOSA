using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoLineAgg
{
    public class PurchaseCreditMemoLineFactory
    {
        public static PurchaseCreditMemoLine CreatePurchaseCreditMemoLine(Guid purchaseCreditMemoId, int type, int no, string description, int quantity, int unitCost, decimal totalAmount, Guid creditChartOfAccountId)
        {

            var purchaseCreditMemoLine = new PurchaseCreditMemoLine();

            purchaseCreditMemoLine.GenerateNewIdentity();


            purchaseCreditMemoLine.PurchaseCreditMemoId = purchaseCreditMemoId;

            //purchaseCreditMemoLine.PurchaseCreditMemoNo = purchaseCreditMemoNo;

            purchaseCreditMemoLine.Type = type;
            purchaseCreditMemoLine.No = no;
            purchaseCreditMemoLine.Description = description;
            purchaseCreditMemoLine.Quantity = quantity;
            purchaseCreditMemoLine.UnitCost = unitCost;
            purchaseCreditMemoLine.Amount = totalAmount;

            purchaseCreditMemoLine.CreditChartOfAccountId = creditChartOfAccountId;


            purchaseCreditMemoLine.CreatedDate = DateTime.Now;

            return purchaseCreditMemoLine;
        }

    }
}
