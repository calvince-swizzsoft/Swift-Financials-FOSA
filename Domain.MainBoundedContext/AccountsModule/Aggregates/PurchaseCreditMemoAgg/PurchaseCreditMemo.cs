using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoAgg
{
    public class PurchaseCreditMemo : Entity
    {

        //public Guid Id {  get; set; }


        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        //public int No { get; set; }

        public string No { get; set; }
        public string PurchaseInvoiceNo { get; set;}

        public string VendorNo { get; set; }

        //public int VendorNo { get; set; }

        public string VendorName { get; set; }

        public string VendorAddress { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime PostingDate { get; set; }

        public DateTime DueDate { get; set; }

        public string ApprovalStatus { get; set; }

        //AppliesToDocNo
        public int InvoiceNo { get; set; }

        public Guid PurchaseInvoiceId { get; set; }

        public Boolean Posted { get; set;  }

        public Decimal TotalAmount { get; set; }

        public Decimal PaidAmount { get; set; }

        public Decimal RemainingAmount { get; set; }


        HashSet<PurchaseCreditMemoLine> _purchaseCreditMemoLines;
        public virtual ICollection<PurchaseCreditMemoLine> PurchaseCreditMemoLines
        {
            get
            {
                if (_purchaseCreditMemoLines == null)
                {
                    _purchaseCreditMemoLines = new HashSet<PurchaseCreditMemoLine>();
                }
                return _purchaseCreditMemoLines;
            }
            private set
            {
                _purchaseCreditMemoLines = new HashSet<PurchaseCreditMemoLine>(value);
            }
        }



        public void AddLine(int type, int purchaseCreditMemoLnNo, string purchaseCreditMemoLnDescription, int purchaseCreditMemoLnQuantity, int purchaseCreditMemoLnUnitCost, decimal purchaseCreditMemoLnAmount, Guid purchaseCreditMemoLineCreditChartOfAccountId, ServiceHeader serviceHeader)
        {
            var purchaseCreditMemoLine = PurchaseCreditMemoLineFactory.CreatePurchaseCreditMemoLine(this.Id, type, purchaseCreditMemoLnNo, purchaseCreditMemoLnDescription, purchaseCreditMemoLnQuantity, purchaseCreditMemoLnUnitCost, purchaseCreditMemoLnAmount, purchaseCreditMemoLineCreditChartOfAccountId);
            //CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, this.ValueDate, serviceHeader);

            this.PurchaseCreditMemoLines.Add(purchaseCreditMemoLine);
        }

    }
}
