using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoAgg
{
    public class SalesCreditMemo : Entity
    {

        //salescreditmemono
        public string No { get; set; }

        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime PostingDate { get; set; }

        public DateTime DueDate { get; set; }

        public string ApprovalStatus { get; set; }

        public Guid SalesInvoiceId { get; set; }

        public string AppliesToDocNo { get; set; }


        public Boolean Posted { get; set; }

        public Decimal TotalAmount { get; set; }



        HashSet<SalesCreditMemoLine> _salesCreditMemoLines;
        public virtual ICollection<SalesCreditMemoLine> SalesCreditMemoLines
        {
            get
            {
                if (_salesCreditMemoLines == null)
                {
                    _salesCreditMemoLines = new HashSet<SalesCreditMemoLine>();
                }
                return _salesCreditMemoLines;
            }
            private set
            {
                _salesCreditMemoLines = new HashSet<SalesCreditMemoLine>(value);
            }
        }





        public void AddLine(int type, int salesCreditMemoLnNo, string salesCreditMemoLnDescription, int salesCreditMemoLnQuantity, decimal salesCreditMemoLnTotalAmount, Guid salesCreditMemoLineDebitChartOfAccountId, decimal salesCreditMemoLineUnitCost, ServiceHeader serviceHeader)
        {
            var salesCreditMemoLine = SalesCreditMemoLineFactory.CreateSalesCreditMemoLine(this.Id, type, salesCreditMemoLnNo, salesCreditMemoLnDescription, salesCreditMemoLnQuantity, salesCreditMemoLnTotalAmount, salesCreditMemoLineDebitChartOfAccountId, salesCreditMemoLineUnitCost);
            //CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, this.ValueDate, serviceHeader);

            this.SalesCreditMemoLines.Add(salesCreditMemoLine);
        }
    }

}
