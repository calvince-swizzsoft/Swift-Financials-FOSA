using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupEntryAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupAgg
{
    public class SalaryGroup : Domain.Seedwork.Entity
    {
        public string Description { get; set; }

        

        HashSet<SalaryGroupEntry> _salaryGroupEntries;
        public virtual ICollection<SalaryGroupEntry> SalaryGroupEntries
        {
            get
            {
                if (_salaryGroupEntries == null)
                {
                    _salaryGroupEntries = new HashSet<SalaryGroupEntry>();
                }
                return _salaryGroupEntries;
            }
            private set
            {
                _salaryGroupEntries = new HashSet<SalaryGroupEntry>(value);
            }
        }
    }
}
