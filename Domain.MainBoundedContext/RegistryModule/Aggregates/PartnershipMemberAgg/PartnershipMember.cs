using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.PartnershipMemberAgg
{
    public class PartnershipMember : Domain.Seedwork.Entity
    {
        public Guid PartnershipId { get; set; }

        public virtual Customer Partnership { get; private set; }

        public int Salutation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte IdentityCardType { get; set; }

        public string IdentityCardNumber { get; set; }

        public string PayrollNumbers { get; set; }

        public int Gender { get; set; }

        public byte Relationship { get; set; }

        public virtual Address Address { get; set; }

        public string Remarks { get; set; }

        public bool Signatory { get; set; }
    }
}
