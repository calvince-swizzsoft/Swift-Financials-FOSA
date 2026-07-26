using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg
{
    public class PurchaseInvoice : Entity
    {

        public string No { get; set; }

        public string VendorNo { get; set; }

        public string VendorName { get; set; }

        public string VendorAddress { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime PostingDate { get; set; }

        public DateTime DueDate { get; set; }

        public string ApprovalStatus { get; set; }

        public Boolean Posted { get; set; }

        public Decimal TotalAmount {get; set;}

        public Decimal PaidAmount { get; set; }

        public Decimal RemainingAmount { get; set; }


        HashSet<PurchaseInvoiceLine> _purchaseInvoiceLines;
        public virtual ICollection<PurchaseInvoiceLine> PurchaseInvoiceLines
        {
            get
            {
                if (_purchaseInvoiceLines == null)
                {
                    _purchaseInvoiceLines = new HashSet<PurchaseInvoiceLine>();
                }
                return _purchaseInvoiceLines;
            }
            private set
            {
                _purchaseInvoiceLines = new HashSet<PurchaseInvoiceLine>(value);
            }
        }



        public void AddLine(int type, int purchaseInvoiceLnNo, string purchaseInvoiceLnDescription, decimal purchaseInvoiceLnUnitCost, int purchaseInvoiceLnQuantity, decimal purchaseInvoiceLnTotalAmount, Guid purchaseInvoiceLineDebitChartOfAccountId, ServiceHeader serviceHeader)
        {
            var purchaseInvoiceLine = PurchaseInvoiceLineFactory.CreatePurchaseInvoiceLine(this.Id, type, purchaseInvoiceLnNo, purchaseInvoiceLnDescription, purchaseInvoiceLnUnitCost, purchaseInvoiceLnQuantity, purchaseInvoiceLnTotalAmount, purchaseInvoiceLineDebitChartOfAccountId);
                //CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, this.ValueDate, serviceHeader);

            this.PurchaseInvoiceLines.Add(purchaseInvoiceLine);
        }



    }
}
