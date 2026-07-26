using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg
{
    public static class CustomerAccountCarryForwardFactory
    {
        public static CustomerAccountCarryForward CreateCustomerAccountCarryForward(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId, decimal amount, string reference)
        {
            var customerAccountCarryForward = new CustomerAccountCarryForward();

            customerAccountCarryForward.GenerateNewIdentity();

            customerAccountCarryForward.BenefactorCustomerAccountId = benefactorCustomerAccountId;

            customerAccountCarryForward.BeneficiaryCustomerAccountId = beneficiaryCustomerAccountId;

            customerAccountCarryForward.BeneficiaryChartOfAccountId = beneficiaryChartOfAccountId;

            customerAccountCarryForward.Amount = amount;

            customerAccountCarryForward.Reference = reference;

            customerAccountCarryForward.CreatedDate = DateTime.Now;

            return customerAccountCarryForward;
        }
    }
}
