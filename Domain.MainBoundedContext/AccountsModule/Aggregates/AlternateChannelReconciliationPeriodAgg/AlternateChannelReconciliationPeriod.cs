using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationEntryAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationPeriodAgg
{
    public class AlternateChannelReconciliationPeriod : Entity
    {
        public short AlternateChannelType { get; set; }

        public virtual Duration Duration { get; set; }

        public byte SetDifferenceMode { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        

        

        HashSet<AlternateChannelReconciliationEntry> _alternateChannelReconciliationEntries;
        public virtual ICollection<AlternateChannelReconciliationEntry> AlternateChannelReconciliationEntries
        {
            get
            {
                if (_alternateChannelReconciliationEntries == null)
                {
                    _alternateChannelReconciliationEntries = new HashSet<AlternateChannelReconciliationEntry>();
                }
                return _alternateChannelReconciliationEntries;
            }
            private set
            {
                _alternateChannelReconciliationEntries = new HashSet<AlternateChannelReconciliationEntry>(value);
            }
        }
    }
}
