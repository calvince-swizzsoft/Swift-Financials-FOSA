using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DirectDebitAgg
{
    public static class DirectDebitFactory
    {
        public static DirectDebit CreateDirectDebit(string description, CustomerAccountType customerAccountType, Charge charge)
        {
            var directDebit = new DirectDebit();

            directDebit.GenerateNewIdentity();

            directDebit.Description = description;

            directDebit.CustomerAccountType = customerAccountType;

            directDebit.Charge = charge;

            directDebit.CreatedDate = DateTime.Now;

            return directDebit;
        }
    }
}
