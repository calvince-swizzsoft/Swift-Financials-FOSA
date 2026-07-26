using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IZoneAppService
    {
        Task<ZoneDTO> AddNewZoneAsync(ZoneDTO zoneDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateZoneAsync(ZoneDTO zoneDTO, ServiceHeader serviceHeader);

        Task<List<ZoneDTO>> FindZonesAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<ZoneDTO>> FindZonesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<ZoneDTO>> FindZonesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<ZoneDTO> FindZoneAsync(Guid zoneId, ServiceHeader serviceHeader);

        Task<StationDTO> FindStationAsync(Guid stationId, ServiceHeader serviceHeader);

        Task<List<StationDTO>> FindStationsByZoneIdAsync(Guid zoneId, ServiceHeader serviceHeader);

        Task<List<StationDTO>> FindStationsByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader);

        Task<List<StationDTO>> FindStationsByDivisionIdAsync(Guid divisionId, ServiceHeader serviceHeader);

        Task<bool> UpdateStationsAsync(Guid zoneId, List<StationDTO> stations, ServiceHeader serviceHeader);

        Task<List<StationDTO>> FindStationsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<StationDTO>> FindStationsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        bool RemoveStation(Guid stationId, ServiceHeader serviceHeader);

        Task<bool> RemoveStationAsync(Guid stationId, ServiceHeader serviceHeader);

        Task<bool> RemoveZoneAsync(Guid zoneId, ServiceHeader serviceHeader);

        Task<bool> RemoveDivisionAsync(Guid divisionId, ServiceHeader serviceHeader);

        Task<bool> RemoveEmployerAsync(Guid employerId, ServiceHeader serviceHeader);
    }
}
