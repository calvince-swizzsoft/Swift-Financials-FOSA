using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerCreditTypeAgg
{
    public static class CustomerCreditTypeFactory
    {
        public static CustomerCreditType CreateCustomerCreditType(Guid customerId, Guid creditTypeId)
        {
            var customerCreditType = new CustomerCreditType();

            customerCreditType.GenerateNewIdentity();

            customerCreditType.CustomerId = customerId;

            customerCreditType.CreditTypeId = creditTypeId;

            customerCreditType.CreatedDate = DateTime.Now;

            return customerCreditType;
        }
    }
}
