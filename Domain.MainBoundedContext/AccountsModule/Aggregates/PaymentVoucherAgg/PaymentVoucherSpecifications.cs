using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg
{
    public static class PaymentVoucherSpecifications
    {
        public static Specification<PaymentVoucher> DefaultSpec()
        {
            Specification<PaymentVoucher> specification = new TrueSpecification<PaymentVoucher>();

            return specification;
        }

        public static ISpecification<PaymentVoucher> PaymentVoucherWithChequeBookId(Guid chequeBookId, string text)
        {
            Specification<PaymentVoucher> specification = new TrueSpecification<PaymentVoucher>();

            if (chequeBookId != null && chequeBookId != Guid.Empty)
            {
                specification &= new DirectSpecification<PaymentVoucher>(x => x.ChequeBookId == chequeBookId);

                if (!String.IsNullOrWhiteSpace(text))
                {
                    int number = default(int);
                    if (int.TryParse(text.StripPunctuation(), out number))
                    {
                        var voucherNumberSpec = new DirectSpecification<PaymentVoucher>(c => c.VoucherNumber == number);
                        var amountSpec = new DirectSpecification<PaymentVoucher>(c => c.Amount == number);

                        specification &= (voucherNumberSpec | amountSpec);
                    }
                    else
                    {
                        var payeeSpec = new DirectSpecification<PaymentVoucher>(c => c.Payee.Contains(text));
                        var remarksSpec = new DirectSpecification<PaymentVoucher>(c => c.Reference.Contains(text));

                        specification &= (payeeSpec | remarksSpec);
                    }
                }
            }

            return specification;
        }

        public static ISpecification<PaymentVoucher> PaymentVoucherWithVoucherNumberAndChequeBookReference(int chequeBookType, int voucherNumber, string chequeBookReference)
        {
            Specification<PaymentVoucher> specification = new DirectSpecification<PaymentVoucher>(x => x.ChequeBook.Type == chequeBookType && x.VoucherNumber == voucherNumber);

            if (!string.IsNullOrWhiteSpace(chequeBookReference))
            {
                specification &= new DirectSpecification<PaymentVoucher>(x => x.ChequeBook.Reference == chequeBookReference);
            }

            return specification;
        }
    }
}
