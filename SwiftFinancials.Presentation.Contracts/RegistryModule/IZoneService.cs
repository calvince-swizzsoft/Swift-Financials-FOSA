using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IZoneService")]
    public interface IZoneService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddZone(ZoneDTO zoneDTO, AsyncCallback callback, Object state);
        ZoneDTO EndAddZone(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateZone(ZoneDTO zoneDTO, AsyncCallback callback, Object state);
        bool EndUpdateZone(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindZones(AsyncCallback callback, Object state);
        List<ZoneDTO> EndFindZones(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindZonesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ZoneDTO> EndFindZonesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindZonesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ZoneDTO> EndFindZonesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindZone(Guid zoneId, AsyncCallback callback, Object state);
        ZoneDTO EndFindZone(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStation(Guid stationId, AsyncCallback callback, Object state);
        StationDTO EndFindStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStationsByZoneId(Guid zoneId, AsyncCallback callback, Object state);
        List<StationDTO> EndFindStationsByZoneId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStationsByEmployerId(Guid employerId, AsyncCallback callback, Object state);
        List<StationDTO> EndFindStationsByEmployerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStationsByDivisionId(Guid divisionId, AsyncCallback callback, Object state);
        List<StationDTO> EndFindStationsByDivisionId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateStationsByZoneId(Guid zoneId, List<StationDTO> stations, AsyncCallback callback, Object state);
        bool EndUpdateStationsByZoneId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStations(AsyncCallback callback, Object state);
        List<StationDTO> EndFindStations(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStationsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<StationDTO> EndFindStationsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveStation(Guid stationId, AsyncCallback callback, Object state);
        bool EndRemoveStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveZone(Guid zoneId, AsyncCallback callback, Object state);
        bool EndRemoveZone(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveDivision(Guid divisionId, AsyncCallback callback, Object state);
        bool EndRemoveDivision(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveEmployer(Guid employerId, AsyncCallback callback, Object state);
        bool EndRemoveEmployer(IAsyncResult result);
    }
}
