using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IPurchaseOrderService
    {
        #region PurchaseOrder

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PurchaseOrderDTO> AddPurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdatePurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<PurchaseOrderDTO>> FindPurchaseOrdersAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrderInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrdersByFilterInPageAsync(int itemType, string filter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PurchaseOrderDTO> FindPurchaseOrderAsync(Guid policyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<PurchaseOrderDTO>> FindPurchaseOrderByCodeAsync(string code);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PurchaseOrderEntryDTO> AddPurchaseOrderEntryAsync(PurchaseOrderEntryDTO purchaseOrderEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<PurchaseOrderEntryDTO>> FindPurchaseOrderEntriesByPurchaseOrderIdAsync(Guid policyId);

        /*
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<PurchaseOrderLevyDTO>> FindPurchaseOrderLeviesAsync(Guid policyId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdatePurchaseOrderLeviesAsync(Guid policyId, List<LevyDTO> levies);
        */
        #endregion
    }
}