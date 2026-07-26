using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.NextOfKinAgg
{
    public static class NextOfKinFactory
    {
        public static NextOfKin CreateNextOfKin(Guid customerId, int salutation, string firstName, string lastName, int identityCardType, string identityCardNumber, int gender, int relationship, Address address, double nominatedPercentage, string remarks)
        {
            var nextOfKin = new NextOfKin()
            {
                CustomerId = customerId,
                FirstName = firstName,
                LastName = lastName,
                IdentityCardType = (byte)identityCardType,
                IdentityCardNumber = identityCardNumber
            };

            nextOfKin.GenerateNewIdentity();

            nextOfKin.Salutation = (byte)salutation;

            nextOfKin.Gender = (byte)gender;

            nextOfKin.Relationship = (byte)relationship;

            nextOfKin.Address = address;

            nextOfKin.NominatedPercentage = nominatedPercentage;

            nextOfKin.Remarks = remarks;

            nextOfKin.CreatedDate = DateTime.Now;

            return nextOfKin;
        }
    }
}
