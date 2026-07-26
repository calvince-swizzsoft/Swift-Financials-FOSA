using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg
{
    public static class DebitTypeFactory
    {
        public static DebitType CreateDebitType(string description, CustomerAccountType customerAccountType)
        {
            var debitType = new DebitType();

            debitType.GenerateNewIdentity();

            debitType.Description = description;

            debitType.CustomerAccountType = customerAccountType;

            debitType.CreatedDate = DateTime.Now;

            return debitType;
        }
    }
}
