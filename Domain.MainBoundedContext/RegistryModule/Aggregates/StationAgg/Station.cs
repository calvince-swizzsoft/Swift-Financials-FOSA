using Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg;
using Domain.MainBoundedContext.ValueObjects;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg
{
    public class Station : Domain.Seedwork.Entity
    {
        public Guid ZoneId { get; set; }

        public virtual Zone Zone { get; private set; }

        public string Description { get; set; }

        public virtual Address Address { get; set; }

        
    }
}
