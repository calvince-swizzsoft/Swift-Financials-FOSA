using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class LocationService : ILocationService
    {
        private readonly ILocationAppService _locationAppService;

        public LocationService(
            ILocationAppService locationAppService)
        {
            Guard.ArgumentNotNull(locationAppService, nameof(locationAppService));

            _locationAppService = locationAppService;
        }

        public LocationDTO AddLocation(LocationDTO locationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _locationAppService.AddNewLocation(locationDTO, serviceHeader);
        }

        public bool UpdateLocation(LocationDTO locationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _locationAppService.UpdateLocation(locationDTO, serviceHeader);
        }

        public List<LocationDTO> FindLocations()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _locationAppService.FindLocations(serviceHeader);
        }

        public PageCollectionInfo<LocationDTO> FindLocationsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _locationAppService.FindLocations(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<LocationDTO> FindLocationsByFilterInPage(string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _locationAppService.FindLocations(filter, pageIndex, pageSize, serviceHeader);
        }

        public LocationDTO FindLocation(Guid locationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _locationAppService.FindLocation(locationId, serviceHeader);
        }
    }
}
