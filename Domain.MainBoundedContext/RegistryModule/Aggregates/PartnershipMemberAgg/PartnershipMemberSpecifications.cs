using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.PartnershipMemberAgg
{
    public static class PartnershipMemberSpecifications
    {
        public static Specification<PartnershipMember> DefaultSpec()
        {
            Specification<PartnershipMember> specification = new TrueSpecification<PartnershipMember>();

            return specification;
        }

        public static ISpecification<PartnershipMember> PartnershipMemberWithPartnershipId(Guid partnershipId)
        {
            Specification<PartnershipMember> specification = new TrueSpecification<PartnershipMember>();

            if (partnershipId != null && partnershipId != Guid.Empty)
            {
                specification &= new DirectSpecification<PartnershipMember>(x => x.PartnershipId == partnershipId);
            }

            return specification;
        }
    }
}
