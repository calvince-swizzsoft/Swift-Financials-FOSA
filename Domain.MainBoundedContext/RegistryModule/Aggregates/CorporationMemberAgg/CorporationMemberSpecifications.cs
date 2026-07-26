using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CorporationMemberAgg
{
    public static class CorporationMemberSpecifications
    {
        public static Specification<CorporationMember> DefaultSpec()
        {
            Specification<CorporationMember> specification = new TrueSpecification<CorporationMember>();

            return specification;
        }

        public static ISpecification<CorporationMember> CorporationMemberWithCorporationId(Guid corporationId)
        {
            Specification<CorporationMember> specification = new TrueSpecification<CorporationMember>();

            if (corporationId != null && corporationId != Guid.Empty)
            {
                specification &= new DirectSpecification<CorporationMember>(x => x.CorporationId == corporationId);
            }

            return specification;
        }
    }
}
