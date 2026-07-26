using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System.ServiceModel;


namespace SwiftFinancials.Presentation.Contracts.InventoryModule
{
    [ServiceContract(Name = "IInventoryService")]
    public interface IInventoryService
    {
        #region Inventory

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddInventory(InventoryDTO inventoryDTO, AsyncCallback callback, Object state);
        InventoryDTO EndAddInventory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateInventory(InventoryDTO inventoryDTO, AsyncCallback callback, Object state);
        bool EndUpdateInventory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInventoriesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InventoryDTO> EndFindInventoriesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInventories(AsyncCallback callback, Object state);
        List<InventoryDTO> EndFindInventories(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInventoriesByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InventoryDTO> EndFindInventoriesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInventory(Guid policyTypeId, AsyncCallback callback, Object state);
        InventoryDTO EndFindInventory(IAsyncResult result);

        #endregion
    }
}
