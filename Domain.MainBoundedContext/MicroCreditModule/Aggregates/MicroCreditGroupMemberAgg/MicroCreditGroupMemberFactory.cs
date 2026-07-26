using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupMemberAgg
{
    public static class MicroCreditGroupMemberFactory
    {
        public static MicroCreditGroupMember CreateMicroCreditGroupMember(Guid microCreditGroupId, Guid customerId, int designation, int loanCycle, string remarks)
        {
            var microCreditGroupMember = new MicroCreditGroupMember();

            microCreditGroupMember.GenerateNewIdentity();

            microCreditGroupMember.MicroCreditGroupId = microCreditGroupId;

            microCreditGroupMember.CustomerId = customerId;

            microCreditGroupMember.Designation = (byte)designation;

            microCreditGroupMember.LoanCycle = (short)loanCycle;

            microCreditGroupMember.Remarks = remarks;

            microCreditGroupMember.CreatedDate = DateTime.Now;

            return microCreditGroupMember;
        }
    }
}
