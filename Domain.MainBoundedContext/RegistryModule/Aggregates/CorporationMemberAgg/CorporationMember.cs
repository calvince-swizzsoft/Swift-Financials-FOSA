using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CorporationMemberAgg
{
    public class CorporationMember : Domain.Seedwork.Entity
    {
        public Guid CorporationId { get; set; }

        public virtual Customer Corporation { get; private set; }

        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public string Remarks { get; set; }

        public bool Signatory { get; set; }

        

        
    }
}
