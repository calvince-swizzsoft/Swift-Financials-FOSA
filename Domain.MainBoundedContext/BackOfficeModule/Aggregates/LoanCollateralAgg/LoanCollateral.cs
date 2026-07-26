using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCollateralAgg
{
    public class LoanCollateral : Entity
    {
        public Guid LoanCaseId { get; set; }

        public virtual LoanCase LoanCase { get; private set; }

        public Guid CustomerDocumentId { get; set; }

        public virtual CustomerDocument CustomerDocument { get; private set; }

        public decimal Value { get; set; }

        

        
    }
}
