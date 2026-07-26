using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanAppraisalFactorAgg
{
    public class LoanAppraisalFactor : Domain.Seedwork.Entity
    {
        public Guid LoanCaseId { get; set; }

        public virtual LoanCase LoanCase { get; private set; }

        public string Description { get; set; }

        public int Type { get; set; }

        public decimal Amount { get; set; }

        
    }
}
