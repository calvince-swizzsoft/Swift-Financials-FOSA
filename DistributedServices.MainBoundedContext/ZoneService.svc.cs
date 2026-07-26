using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ZoneService : IZoneService
    {
        private readonly IZoneAppService _zoneAppService;

        public ZoneService(
           IZoneAppService zoneAppService)
        {
            Guard.ArgumentNotNull(zoneAppService, nameof(zoneAppService));

            _zoneAppService = zoneAppService;
        }

        #region Zone

        public async Task<ZoneDTO> AddZoneAsync(ZoneDTO zoneDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.AddNewZoneAsync(zoneDTO, serviceHeader);
        }

        public async Task<bool> UpdateZoneAsync(ZoneDTO zoneDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.UpdateZoneAsync(zoneDTO, serviceHeader);
        }

        public async Task<List<ZoneDTO>> FindZonesAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindZonesAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<ZoneDTO>> FindZonesInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindZonesAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<ZoneDTO>> FindZonesByFilterInPageAsync(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindZonesAsync(text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<ZoneDTO> FindZoneAsync(Guid zoneId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindZoneAsync(zoneId, serviceHeader);
        }

        public async Task<StationDTO> FindStationAsync(Guid stationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindStationAsync(stationId, serviceHeader);
        }

        public async Task<List<StationDTO>> FindStationsByZoneIdAsync(Guid zoneId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindStationsByZoneIdAsync(zoneId, serviceHeader);
        }

        public async Task<List<StationDTO>> FindStationsByEmployerIdAsync(Guid employerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindStationsByEmployerIdAsync(employerId, serviceHeader);
        }

        public async Task<List<StationDTO>> FindStationsByDivisionIdAsync(Guid divisionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindStationsByDivisionIdAsync(divisionId, serviceHeader);
        }

        public async Task<bool> UpdateStationsByZoneIdAsync(Guid zoneId, List<StationDTO> stations)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.UpdateStationsAsync(zoneId, stations, serviceHeader);
        }

        public async Task<List<StationDTO>> FindStationsAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindStationsAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<StationDTO>> FindStationsByFilterInPageAsync(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.FindStationsAsync(text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<bool> RemoveStationAsync(Guid stationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.RemoveStationAsync(stationId, serviceHeader);
        }

        public async Task<bool> RemoveZoneAsync(Guid zoneId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.RemoveZoneAsync(zoneId, serviceHeader);
        }

        public async Task<bool> RemoveDivisionAsync(Guid divisionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.RemoveDivisionAsync(divisionId, serviceHeader);
        }

        public async Task<bool> RemoveEmployerAsync(Guid employerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _zoneAppService.RemoveEmployerAsync(employerId, serviceHeader);
        }

        #endregion
    }
}
