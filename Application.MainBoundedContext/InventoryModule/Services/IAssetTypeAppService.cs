using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.InventoryModule.Services
{
    public interface IAssetTypeAppService
    {
        Task<AssetTypeDTO> AddNewAssetTypeAsync(AssetTypeDTO assetTypeDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateAssetTypeAsync(AssetTypeDTO assetTypeDTO, ServiceHeader serviceHeader);

        Task<List<AssetTypeDTO>> FindAssetTypesAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<AssetTypeDTO>> FindAssetTypesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<AssetTypeDTO>> FindAssetTypesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<AssetTypeDTO> FindAssetTypeAsync(Guid assetTypeId, ServiceHeader serviceHeader);
    }
}
