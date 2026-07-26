using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeBookAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentLineAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentAgg
{
    public class Payment : Domain.Seedwork.Entity
    {

        //public Guid Id { get; set; }
        public string VoucherNumber { get; set; }
        public decimal TotalAmount { get; set; }

        public string Description { get; set; }
        public string Reference { get; set; }

        public byte Status { get; set; }

        public string PaidBy { get; set; }

        public DateTime? PaidDate { get; set; }

        public string PaymentMethod { get; set; }

        public Guid InvoiceId { get; set; }

        public string VendorName { get; set; }

        public string VendorAddress { get; set; }

        public string VendorNo { get; set; }

        public Guid VendorId { get; set; }

        public string InvoiceNo { get; set; }


        public Guid BankLinkageChartOfAccountId { get; set; }

        //public string DocumentNo { get; set; }


        public Boolean Posted { get; set; }


        HashSet<PaymentLine> _paymentLines;
        public virtual ICollection<PaymentLine> PaymentLines
        {
            get
            {
                if (_paymentLines == null)
                {
                    _paymentLines = new HashSet<PaymentLine>();
                }
                return _paymentLines;
            }
            private set
            {
                _paymentLines = new HashSet<PaymentLine>(value);
            }
        }

        public void AddLine(int type, int paymentLnNo, string paymentLnDescription, decimal paymentLnAmount, Guid paymentLineChartOfAccountId, int accountType, int documentType, ServiceHeader serviceHeader)
        {
            var paymentLine = PaymentLineFactory.CreatePaymentLine(this.Id, type, paymentLnNo, paymentLnDescription, paymentLnAmount, paymentLineChartOfAccountId, accountType, documentType);
            //CreateJournalEntry(this.Id, chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, this.ValueDate, serviceHeader);

            this.PaymentLines.Add(paymentLine);
        }

    }
}
