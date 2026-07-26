using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeDirectDebitAgg
{
    public static class CreditTypeDirectDebitFactory
    {
        public static CreditTypeDirectDebit CreateCreditTypeDirectDebit(Guid creditTypeId, Guid directDebitId)
        {
            var creditTypeDirectDebit = new CreditTypeDirectDebit();

            creditTypeDirectDebit.GenerateNewIdentity();

            creditTypeDirectDebit.CreditTypeId = creditTypeId;

            creditTypeDirectDebit.DirectDebitId = directDebitId;

            creditTypeDirectDebit.CreatedDate = DateTime.Now;

            return creditTypeDirectDebit;
        }
    }
}
