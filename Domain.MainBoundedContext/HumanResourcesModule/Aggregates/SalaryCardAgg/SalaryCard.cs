using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardEntryAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryCardAgg
{
    public class SalaryCard : Domain.Seedwork.Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public Guid SalaryGroupId { get; set; }

        public virtual SalaryGroup SalaryGroup { get; private set; }

        [Index("IX_SalaryCard_CardNumber")]
        public int CardNumber { get; set; }

        public bool IsTaxExempt { get; set; }

        public decimal TaxExemption { get; set; }

        public decimal InsuranceReliefAmount { get; set; }

        public string Remarks { get; set; }

        public bool IsLocked { get; private set; }

        

        HashSet<SalaryCardEntry> _salaryCardEntries;
        public virtual ICollection<SalaryCardEntry> SalaryCardEntries
        {
            get
            {
                if (_salaryCardEntries == null)
                {
                    _salaryCardEntries = new HashSet<SalaryCardEntry>();
                }
                return _salaryCardEntries;
            }
            private set
            {
                _salaryCardEntries = new HashSet<SalaryCardEntry>(value);
            }
        }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
