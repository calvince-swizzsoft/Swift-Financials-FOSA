using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg
{
    public static class CustomerAccountFactory
    {
        public static CustomerAccount CreateCustomerAccount(Guid customerId, Guid branchId, CustomerAccountType customerAccountType, Guid ProductChartOfAccount)
        {
            var customerAccount = new CustomerAccount();

            customerAccount.GenerateNewIdentity();

            customerAccount.CustomerId = customerId;

            customerAccount.BranchId = branchId;

            customerAccount.CustomerAccountType = customerAccountType;

            customerAccount.CreatedDate = DateTime.Now;

            //customerAccount.ProductChartOfAccountId = ProductChartOfAccount;

            return customerAccount;
        }
    }
}
