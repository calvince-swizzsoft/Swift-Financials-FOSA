using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.SalaryHeadAgg
{
    public class SalaryHead : Domain.Seedwork.Entity
    {
        public Guid ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public byte Category { get; set; }

        public bool IsOneOff { get; set; }

        public virtual CustomerAccountType CustomerAccountType { get; set; }

        
    }
}
