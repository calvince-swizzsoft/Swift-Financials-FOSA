using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.UnderwritingModule.Services
{
    public interface IPurchaseOrderAppService
    {
        #region PurchaseOrderDTO
        Task<PurchaseOrderDTO> AddNewPurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO, ServiceHeader serviceHeader);

        Task<bool> UpdatePurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO, ServiceHeader serviceHeader);

        Task<List<PurchaseOrderDTO>> FindPurchaseOrdersAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrderAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrdersAsync(int itemType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PurchaseOrderDTO> FindPurchaseOrderAsync(Guid policyId, ServiceHeader serviceHeader);

        Task<List<PurchaseOrderDTO>> FindPurchaseOrderAsync(string code, ServiceHeader serviceHeader);

        #endregion

        #region PurchaseOrderEntryDTO
        Task <PurchaseOrderEntryDTO> AddNewPurchaseOrderEntryAsync(PurchaseOrderEntryDTO purchaseOrderEntryDTO, ServiceHeader serviceHeader);

        Task<List<PurchaseOrderEntryDTO>> FindPurchaseOrderEntriesByPurchaseOrderIdAsync(Guid policyId, ServiceHeader serviceHeader);







        #endregion

    }
}
