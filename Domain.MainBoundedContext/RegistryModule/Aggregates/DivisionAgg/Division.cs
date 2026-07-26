using Domain.MainBoundedContext.RegistryModule.Aggregates.EmployerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg
{
    public class Division : Domain.Seedwork.Entity
    {
        public Guid EmployerId { get; set; }

        public virtual Employer Employer { get; private set; }

        public string Description { get; set; }

        

        HashSet<Zone> _zones;
        public virtual ICollection<Zone> Zones
        {
            get
            {
                if (_zones == null)
                {
                    _zones = new HashSet<Zone>();
                }
                return _zones;
            }
            private set
            {
                _zones = new HashSet<Zone>(value);
            }
        }
    }
}
