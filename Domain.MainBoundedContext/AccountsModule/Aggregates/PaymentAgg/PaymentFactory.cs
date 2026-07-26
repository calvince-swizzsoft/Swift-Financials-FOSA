using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentAgg
{
    public static class PaymentFactory
    {

        

        public static Payment CreatePayment(Guid invoiceId, Guid vendorId, string vendorNo, string vendorName, string vendorAddress, string description, string reference, decimal totalAmount, string paymentMethod, Guid bankLinkageChartOfAccountd, string voucherNumber)
        {
            
            var payment = new Payment()
            {
                InvoiceId = invoiceId
            };

            payment.GenerateNewIdentity();

            payment.VendorNo = vendorNo;

            payment.VendorName = vendorName;

            payment.VendorAddress = vendorAddress;

            //payment.VoucherNumber = voucherNumber;

            payment.Description = description;

            payment.Reference = reference;

            payment.PaymentMethod = paymentMethod;

            payment.VoucherNumber = voucherNumber;

            //payment.DocumentNo = documentNo;

            payment.VendorId = vendorId;

            payment.TotalAmount = totalAmount;

            payment.CreatedDate = DateTime.Now;

            payment.BankLinkageChartOfAccountId = bankLinkageChartOfAccountd;

            return payment;
        }
    }
}
