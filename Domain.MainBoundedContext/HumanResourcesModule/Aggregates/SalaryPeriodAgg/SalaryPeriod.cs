using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryPeriodAgg
{
    public class SalaryPeriod : Entity
    {
        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public byte Month { get; set; }

        public byte EmployeeCategory { get; set; }
        
        public decimal TaxReliefAmount { get; set; }

        public decimal MaximumProvidentFundReliefAmount { get; set; }

        public decimal MaximumInsuranceReliefAmount { get; set; }

        public bool EnforceMonthValueDate { get; set; }

        public bool ExecutePayoutStandingOrders { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }
        
        HashSet<PaySlip> _paySlips;
        public virtual ICollection<PaySlip> PaySlips
        {
            get
            {
                if (_paySlips == null)
                {
                    _paySlips = new HashSet<PaySlip>();
                }
                return _paySlips;
            }
            private set
            {
                _paySlips = new HashSet<PaySlip>(value);
            }
        }
    }
}
