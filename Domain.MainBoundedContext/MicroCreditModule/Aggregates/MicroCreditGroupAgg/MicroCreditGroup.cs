using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupMemberAgg;
using Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditOfficerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupAgg
{
    public class MicroCreditGroup : Domain.Seedwork.Entity
    {
        public Guid? ParentId { get; set; }

        public virtual MicroCreditGroup Parent { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid MicroCreditOfficerId { get; set; }

        public virtual MicroCreditOfficer MicroCreditOfficer { get; private set; }

        public int Type { get; set; }

        public string Purpose { get; set; }

        public string Activities { get; set; }

        public int MeetingFrequency { get; set; }

        public int MeetingDayOfWeek { get; set; }

        public string MeetingPlace { get; set; }

        public int MinimumMembers { get; set; }

        public int MaximumMembers { get; set; }

        public string Remarks { get; set; }

        

        HashSet<MicroCreditGroupMember> _microCreditGroupMembers;
        public virtual ICollection<MicroCreditGroupMember> MicroCreditGroupMembers
        {
            get
            {
                if (_microCreditGroupMembers == null)
                {
                    _microCreditGroupMembers = new HashSet<MicroCreditGroupMember>();
                }
                return _microCreditGroupMembers;
            }
            private set
            {
                _microCreditGroupMembers = new HashSet<MicroCreditGroupMember>(value);
            }
        }
    }
}
