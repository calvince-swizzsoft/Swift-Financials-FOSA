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
    public interface IInventoryService
    {
        #region Inventory

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<InventoryDTO> AddInventoryAsync(InventoryDTO inventoryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateInventoryAsync(InventoryDTO inventoryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<InventoryDTO>> FindInventoriesAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<InventoryDTO>> FindInventoriesInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<InventoryDTO>> FindInventoriesByFilterInPageAsync(string filter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        InventoryDTO FindInventory(Guid policyTypeId);

        #endregion
    }
}
