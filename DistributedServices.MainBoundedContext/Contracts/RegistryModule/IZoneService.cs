using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IZoneService
    {
        #region Zone

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<ZoneDTO> AddZoneAsync(ZoneDTO zoneDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateZoneAsync(ZoneDTO zoneDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<ZoneDTO>> FindZonesAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<ZoneDTO>> FindZonesInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<ZoneDTO>> FindZonesByFilterInPageAsync(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<ZoneDTO> FindZoneAsync(Guid zoneId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<StationDTO> FindStationAsync(Guid stationId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<StationDTO>> FindStationsByZoneIdAsync(Guid zoneId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<StationDTO>> FindStationsByEmployerIdAsync(Guid employerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<StationDTO>> FindStationsByDivisionIdAsync(Guid divisionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateStationsByZoneIdAsync(Guid zoneId, List<StationDTO> stations);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<StationDTO>> FindStationsAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<StationDTO>> FindStationsByFilterInPageAsync(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> RemoveStationAsync(Guid stationId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> RemoveZoneAsync(Guid zoneId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> RemoveDivisionAsync(Guid divisionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> RemoveEmployerAsync(Guid employerId);

        #endregion
    }
}
