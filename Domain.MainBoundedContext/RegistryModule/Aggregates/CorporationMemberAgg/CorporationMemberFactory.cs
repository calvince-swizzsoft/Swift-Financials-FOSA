using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CorporationMemberAgg
{
    public static class CorporationMemberFactory
    {
        public static CorporationMember CreateCorporationMember(Guid corporationId, Guid customerId, string remarks, bool signatory)
        {
            var corporationMember = new CorporationMember(); ;

            corporationMember.GenerateNewIdentity();

            corporationMember.CorporationId = corporationId;

            corporationMember.CustomerId = customerId;

            corporationMember.Remarks = remarks;

            corporationMember.Signatory = signatory;

            corporationMember.CreatedDate = DateTime.Now;

            return corporationMember;
        }
    }
}
