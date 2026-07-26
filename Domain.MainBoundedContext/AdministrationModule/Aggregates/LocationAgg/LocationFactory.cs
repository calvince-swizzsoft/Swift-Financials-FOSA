using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.LocationAgg
{
    public static class LocationFactory
    {
        public static Location CreateLocation(Guid branchId, string description)
        {
            var location = new Location();

            location.GenerateNewIdentity();

            location.BranchId = branchId;

            location.Description = description;

            return location;
        }
    }
}
