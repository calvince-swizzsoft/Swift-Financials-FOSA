using Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg
{
    public class Zone : Domain.Seedwork.Entity
    {
        public Guid DivisionId { get; set; }

        public virtual Division Division { get; private set; }

        public string Description { get; set; }

        

        HashSet<Station> _stations;
        public virtual ICollection<Station> Stations
        {
            get
            {
                if (_stations == null)
                {
                    _stations = new HashSet<Station>();
                }
                return _stations;
            }
            private set
            {
                _stations = new HashSet<Station>(value);
            }
        }
    }
}
