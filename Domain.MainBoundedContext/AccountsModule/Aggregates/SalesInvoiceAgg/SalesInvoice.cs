using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceAgg
{
    public class SalesInvoice : Entity
    {  
        public string No { get; set; }
        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        //should allow date capture and validate the date capturd
        public DateTime DocumentDate { get; set; }

        public DateTime PostingDate { get; set; }

        public DateTime DueDate { get; set; }

        //Approval Status Should be Open, Pending Approval, Approved
        public string ApprovalStatus { get; set; }


        public Boolean Posted { get; set; }

        public Decimal TotalAmount { get; set; }

        public Decimal PaidAmount { get; set; }

        public Decimal RemainingAmount { get; set; }


        HashSet<SalesInvoiceLine> _salesInvoiceLines;
        public virtual ICollection<SalesInvoiceLine> SalesInvoiceLines
        {
            get
            {
                if (_salesInvoiceLines == null)
                {
                    _salesInvoiceLines = new HashSet<SalesInvoiceLine>();
                }
                return _salesInvoiceLines;
            }
            private set
            {
                _salesInvoiceLines = new HashSet<SalesInvoiceLine>(value);
            }
        }



        public void AddLine(int type, int salesInvoiceLnNo, string salesInvoiceLnDescription,  decimal salesInvoiceLnUnitCost, int salesInvoiceLnQuantity, decimal salesInvoiceLnAmount, Guid salesInvoiceLineCreditChartOfAccountId, ServiceHeader serviceHeader)
        {
            var salesInvoiceLine = SalesInvoiceLineFactory.CreateSalesInvoiceLine(this.Id, type, salesInvoiceLnNo, salesInvoiceLnDescription, salesInvoiceLnUnitCost, salesInvoiceLnQuantity, salesInvoiceLnAmount, salesInvoiceLineCreditChartOfAccountId);
            //CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, this.ValueDate, serviceHeader);

            this.SalesInvoiceLines.Add(salesInvoiceLine);
        }






    }
}