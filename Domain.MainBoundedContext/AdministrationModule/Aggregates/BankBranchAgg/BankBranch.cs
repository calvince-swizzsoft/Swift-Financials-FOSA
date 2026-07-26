using Domain.MainBoundedContext.AdministrationModule.Aggregates.BankAgg;
using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.BankBranchAgg
{
    public class BankBranch : Domain.Seedwork.Entity
    {
        public Guid BankId { get; set; }

        public virtual Bank Bank { get; private set; }

        public short Code { get; set; }

        public string Description { get; set; }

        public virtual Address Address { get; set; }

        
    }
}
