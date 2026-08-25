using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.InventoryModule.Services
{
    public interface IPackageTypeAppService
    {
        Task<PackageTypeDTO> AddNewPackageTypeAsync(PackageTypeDTO packageTypeDTO, ServiceHeader serviceHeader);

        Task<bool> UpdatePackageTypeAsync(PackageTypeDTO packageTypeDTO, ServiceHeader serviceHeader);

        Task<List<PackageTypeDTO>> FindPackageTypesAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<PackageTypeDTO>> FindPackageTypesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<PackageTypeDTO>> FindPackageTypesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PackageTypeDTO> FindPackageTypeAsync(Guid packageTypeId, ServiceHeader serviceHeader);
    }
}
