using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.TruncatedChequeAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ElectronicJournalAgg
{
    public class ElectronicJournal : Domain.Seedwork.Entity
    {
        public string FileName { get; set; }

        public virtual HeaderRecord HeaderRecord { get; set; }

        public virtual TrailerRecord TrailerRecord { get; set; }

        public int Status { get; set; }

        public string ClosedBy { get; set; }

        public DateTime? ClosedDate { get; set; }

        HashSet<TruncatedCheque> _truncatedCheques;
        public virtual ICollection<TruncatedCheque> TruncatedCheques
        {
            get
            {
                if (_truncatedCheques == null)
                {
                    _truncatedCheques = new HashSet<TruncatedCheque>();
                }
                return _truncatedCheques;
            }
            private set
            {
                _truncatedCheques = new HashSet<TruncatedCheque>(value);
            }
        }
    }
}
