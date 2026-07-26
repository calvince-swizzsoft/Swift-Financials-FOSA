using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardInstallmentAgg
{
    public static class CustomerAccountCarryForwardInstallmentFactory
    {
        public static CustomerAccountCarryForwardInstallment CreateCustomerAccountCarryForwardInstallment(Guid customerAccountId,  Guid chartOfAccountId, decimal amount, string reference)
        {
            var customerAccountCarryForwardInstallment = new CustomerAccountCarryForwardInstallment();

            customerAccountCarryForwardInstallment.GenerateNewIdentity();

            customerAccountCarryForwardInstallment.CustomerAccountId = customerAccountId;

            customerAccountCarryForwardInstallment.ChartOfAccountId = chartOfAccountId;

            customerAccountCarryForwardInstallment.Amount = amount;

            customerAccountCarryForwardInstallment.Reference = reference;

            customerAccountCarryForwardInstallment.CreatedDate = DateTime.Now;

            return customerAccountCarryForwardInstallment;
        }
    }
}
