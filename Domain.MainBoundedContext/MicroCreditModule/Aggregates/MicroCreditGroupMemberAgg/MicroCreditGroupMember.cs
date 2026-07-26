using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupMemberAgg
{
    public class MicroCreditGroupMember : Domain.Seedwork.Entity
    {
        public Guid MicroCreditGroupId { get; set; }

        public virtual MicroCreditGroup MicroCreditGroup { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        [Index("IX_MicroCreditGroupMember_Designation")]
        public byte Designation { get; set; }

        public short LoanCycle { get; set; }

        public string Remarks { get; set; }

        

        
    }
}
