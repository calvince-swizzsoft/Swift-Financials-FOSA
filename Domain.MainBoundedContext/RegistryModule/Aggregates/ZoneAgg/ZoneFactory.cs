using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg
{
    public static class ZoneFactory
    {
        public static Zone CreateZone(Guid divisionId, string description)
        {
            var zone = new Zone();

            zone.GenerateNewIdentity();

            zone.DivisionId = divisionId;

            zone.Description = description;

            zone.CreatedDate = DateTime.Now;

            return zone;
        }
    }
}
