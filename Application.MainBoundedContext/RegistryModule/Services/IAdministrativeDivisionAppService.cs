using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IAdministrativeDivisionAppService
    {
        Task<AdministrativeDivisionDTO> AddNewAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO, ServiceHeader serviceHeader);
         
        Task<List<AdministrativeDivisionDTO>> FindAdministrativeDivisionsAsync(ServiceHeader serviceHeader);

        List<AdministrativeDivisionDTO> FindAdministrativeDivisions(ServiceHeader serviceHeader, bool updateDepth = false, bool traverseTree = true);
        
        Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        AdministrativeDivisionDTO FindAdministrativeDivision(Guid administrativeDivisionId, ServiceHeader serviceHeader);
    }
}
