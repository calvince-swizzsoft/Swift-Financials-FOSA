using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryPeriodAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg
{
    public class PaySlip : Entity
    {
        public Guid SalaryPeriodId { get; set; }

        public virtual SalaryPeriod SalaryPeriod { get; private set; }

        public Guid SalaryCardId { get; set; }

        public virtual SalaryCard SalaryCard { get; private set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }
        
        HashSet<PaySlipEntry> _paySlipEntries;
        public virtual ICollection<PaySlipEntry> PaySlipEntries
        {
            get
            {
                if (_paySlipEntries == null)
                {
                    _paySlipEntries = new HashSet<PaySlipEntry>();
                }
                return _paySlipEntries;
            }
            private set
            {
                _paySlipEntries = new HashSet<PaySlipEntry>(value);
            }
        }
    }
}
