using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg
{
    public static class CustomerAccountArrearageFactory
    {
        public static CustomerAccountArrearage CreateCustomerAccountArrearage(Guid customerAccountId, int category, decimal amount, string reference)
        {
            var customerAccountCarryForward = new CustomerAccountArrearage();

            customerAccountCarryForward.GenerateNewIdentity();

            customerAccountCarryForward.CustomerAccountId = customerAccountId;

            customerAccountCarryForward.Category = (byte)category;

            customerAccountCarryForward.Amount = amount;

            customerAccountCarryForward.Reference = reference;

            customerAccountCarryForward.CreatedDate = DateTime.Now;

            return customerAccountCarryForward;
        }
    }
}
