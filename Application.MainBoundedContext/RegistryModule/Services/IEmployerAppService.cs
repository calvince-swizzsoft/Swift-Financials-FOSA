using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using System;
using System.Collections.Generic;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IEmployerAppService
    {
        Task<EmployerDTO> AddNewEmployerAsync(EmployerDTO employerDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateEmployerAsync(EmployerDTO employerDTO, ServiceHeader serviceHeader);

        Task<List<EmployerDTO>> FindEmployersAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<EmployerDTO>> FindEmployersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<EmployerDTO>> FindEmployersAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<EmployerDTO> FindEmployerAsync(Guid employerId, ServiceHeader serviceHeader);

        Task<DivisionDTO> FindDivisionAsync(Guid divisionId, ServiceHeader serviceHeader);

        Task<List<DivisionDTO>> FindDivisionsAsync(ServiceHeader serviceHeader);

        Task<List<DivisionDTO>> FindDivisionsByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader);

        Task<List<ZoneDTO>> FindZonesAsync(Guid employerId, ServiceHeader serviceHeader);

        Task<bool> UpdateDivisionsAsync(Guid employerId, List<DivisionDTO> divisions, ServiceHeader serviceHeader);
    }
}
