using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.PartnershipMemberAgg
{
    public static class PartnershipMemberFactory
    {
        public static PartnershipMember CreatePartnershipMember(Guid partnershipId, int salutation, string firstName, string lastName, int identityCardType, string identityCardNumber, int gender, int relationship, Address address, string remarks, bool signatory)
        {
            var partnershipMember = new PartnershipMember();

            partnershipMember.GenerateNewIdentity();

            partnershipMember.PartnershipId = partnershipId;

            partnershipMember.Salutation = salutation;

            partnershipMember.FirstName = firstName;

            partnershipMember.LastName = lastName;

            partnershipMember.IdentityCardType = (byte)identityCardType;

            partnershipMember.IdentityCardNumber = identityCardNumber;

            partnershipMember.Gender = gender;

            partnershipMember.Relationship = (byte)relationship;

            partnershipMember.Address = address;

            partnershipMember.Remarks = remarks;

            partnershipMember.Signatory = signatory;

            partnershipMember.CreatedDate = DateTime.Now;

            return partnershipMember;
        }
    }
}
