using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg
{
    public static class PaymentVoucherFactory
    {
        public static PaymentVoucher CreatePaymentVoucher(Guid chequeBookId, int voucherNumber)
        {
            var paymentVoucher = new PaymentVoucher()
            {
                ChequeBookId = chequeBookId
            };

            paymentVoucher.GenerateNewIdentity();

            paymentVoucher.VoucherNumber = voucherNumber;

            paymentVoucher.CreatedDate = DateTime.Now;

            return paymentVoucher;
        }
    }
}
