using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountHistoryAgg
{
    public static class CustomerAccountHistoryFactory
    {
        public static CustomerAccountHistory CreateCustomerAccountHistory(Guid customerAccountId, int managementAction, string remarks, string reference, string createdBy)
        {
            var customerAccountHistory = new CustomerAccountHistory();

            customerAccountHistory.GenerateNewIdentity();

            customerAccountHistory.CustomerAccountId = customerAccountId;

            customerAccountHistory.ManagementAction = managementAction;

            customerAccountHistory.Remarks = remarks;

            customerAccountHistory.Reference = reference;

            customerAccountHistory.CreatedBy = createdBy;

            customerAccountHistory.CreatedDate = DateTime.Now;

            return customerAccountHistory;
        }
    }
}
