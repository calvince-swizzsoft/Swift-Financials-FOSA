using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryHeadAgg;
using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryGroupEntryAgg
{
    public class SalaryGroupEntry : Domain.Seedwork.Entity
    {
        public Guid SalaryGroupId { get; set; }

        public virtual SalaryGroup SalaryGroup { get; private set; }

        public Guid SalaryHeadId { get; set; }

        public virtual SalaryHead SalaryHead { get; private set; }

        public virtual Charge Charge { get; set; }

        public decimal MinimumValue { get; set; }

        public byte RoundingType { get; set; }

        
    }
}
