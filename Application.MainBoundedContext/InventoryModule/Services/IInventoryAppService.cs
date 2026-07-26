using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.UnderwritingModule.Services
{
    public interface IInventoryAppService
    {
        Task<InventoryDTO> AddNewInventoryAsync(InventoryDTO inventoryDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateInventoryAsync(InventoryDTO inventoryDTO, ServiceHeader serviceHeader);

        Task<List<InventoryDTO>> FindInventoriesAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<InventoryDTO>> FindInventoriesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<InventoryDTO>> FindInventoriesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        InventoryDTO FindInventory(Guid inventoryId, ServiceHeader serviceHeader);
    }
}