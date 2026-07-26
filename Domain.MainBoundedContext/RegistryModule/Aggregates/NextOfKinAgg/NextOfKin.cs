using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.NextOfKinAgg
{
    public class NextOfKin : Domain.Seedwork.Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public byte Salutation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string IdentityCardNumber { get; set; }

        public byte Gender { get; set; }

        public byte Relationship { get; set; }

        public byte IdentityCardType { get; set; }

        public virtual Address Address { get; set; }

        public double NominatedPercentage { get; set; }

        public string Remarks { get; set; }
    }
}
