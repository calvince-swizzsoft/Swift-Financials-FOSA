using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorSubstituteAgg
{
    public class LoanGuarantorSubstitute : Domain.Seedwork.Entity
    {
        public Guid LoanGuarantorId { get; set; }

        public virtual LoanGuarantor LoanGuarantor { get; private set; }

        public Guid PreviousCustomerId { get; set; }

        public virtual Customer PreviousCustomer { get; private set; }

        public Guid CurrentCustomerId { get; set; }

        public virtual Customer CurrentCustomer { get; private set; }

        

        
    }
}
