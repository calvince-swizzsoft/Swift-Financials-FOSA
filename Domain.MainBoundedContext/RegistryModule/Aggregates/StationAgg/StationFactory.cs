using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg
{
    public static class StationFactory
    {
        public static Station CreateStation(Guid zoneId, string description, Address address)
        {
            var station = new Station()
            {
                Description = description,
                Address = address
            };

            station.GenerateNewIdentity();

            station.ZoneId = zoneId;

            station.CreatedDate = DateTime.Now;

            return station;
        }
    }
}
