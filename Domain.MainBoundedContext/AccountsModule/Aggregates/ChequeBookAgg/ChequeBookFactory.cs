using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeBookAgg
{
    public static class ChequeBookFactory
    {
        public static ChequeBook CreateChequeBook(Guid customerAccountId, int type, int numberOfVouchers, int initialVoucherNumber, string reference, string remarks)
        {
            var chequeBook = new ChequeBook()
            {
                CustomerAccountId = customerAccountId,
            };

            chequeBook.GenerateNewIdentity();

            chequeBook.Type = (byte)type;

            chequeBook.NumberOfVouchers = numberOfVouchers;

            chequeBook.InitialVoucherNumber = initialVoucherNumber;

            chequeBook.Reference = reference;

            chequeBook.Remarks = remarks;

            chequeBook.CreatedDate = DateTime.Now;

            return chequeBook;
        }
    }
}
