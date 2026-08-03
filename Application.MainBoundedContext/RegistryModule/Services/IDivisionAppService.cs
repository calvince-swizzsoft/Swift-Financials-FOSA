using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IDivisionAppService
    {
        Task<DivisionDTO> AddNewDivisionAsync(DivisionDTO divisionDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateDivisionAsync(DivisionDTO divisionDTO, ServiceHeader serviceHeader);

        Task<List<DivisionDTO>> FindDivisionsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<DivisionDTO>> FindDivisionsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<DivisionDTO>> FindDivisionsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<DivisionDTO> FindDivisionAsync(Guid divisionId, ServiceHeader serviceHeader);

        Task<List<DivisionDTO>> FindDivisionsByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader);

        Task<List<ZoneDTO>> FindZonesByDivisionIdAsync(Guid divisionId, ServiceHeader serviceHeader);
    }
}
