using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptAgg
{
    public class Receipt : Entity
    {      
        public string ReceiptNo { get; set; }

        public string CustomerNo { get; set; }

        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public Decimal Amount { get; set; }

        public string SalesInvoiceNo { get; set; }

        public string ExternalDocumentNo { get; set; }

        public string PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; }

        public string Description { get; set; }
        public string Reference { get; set; }


        public Guid BankLinkageChartOfAccountId { get; set; }

        HashSet<ReceiptLine> _receiptLines;

        public Boolean Posted { get; set; }
        public virtual ICollection<ReceiptLine> ReceiptLines
        {
            get
            {
                if (_receiptLines == null)
                {
                    _receiptLines = new HashSet<ReceiptLine>();
                }
                return _receiptLines;
            }
            private set
            {
                _receiptLines = new HashSet<ReceiptLine>(value);
            }
        }

        public void AddLine(int type, Guid customerAccountId, string customerAccountNo, string receiptLnDescription, decimal receiptLnAmount, Guid receiptLineChartOfAccountId, int accountType, int documentType, ServiceHeader serviceHeader)
        {
            var receiptLine = ReceiptLineFactory.CreateReceiptLine(this.Id, customerAccountId, customerAccountNo, receiptLnDescription, receiptLnAmount, receiptLineChartOfAccountId, accountType, documentType);
            //CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, this.ValueDate, serviceHeader);

            this.ReceiptLines.Add(receiptLine);
        }

    }
}
