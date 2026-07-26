using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountSignatoryAgg
{
    public static class CustomerAccountSignatoryFactory
    {
        public static CustomerAccountSignatory CreateCustomerAccountSignatory(Guid customerAccountId, int salutation, string firstName, string lastName, int identityCardType, string identityCardNumber, int gender, int relationship, Address address, string remarks)
        {
            var customerAccountSignatory = new CustomerAccountSignatory()
            {
                CustomerAccountId = customerAccountId,
                FirstName = firstName,
                LastName = lastName,
                IdentityCardNumber = identityCardNumber
            };

            customerAccountSignatory.GenerateNewIdentity();

            customerAccountSignatory.Salutation = salutation;

            customerAccountSignatory.Gender = gender;

            customerAccountSignatory.Relationship = relationship;

            customerAccountSignatory.IdentityCardType = identityCardType;

            customerAccountSignatory.Address = address;

            customerAccountSignatory.Remarks = remarks;

            customerAccountSignatory.CreatedDate = DateTime.Now;

            return customerAccountSignatory;
        }
    }
}
