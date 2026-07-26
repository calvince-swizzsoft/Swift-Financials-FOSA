using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.UnderwritingModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryAppService _inventoryAppService;

        public InventoryService(IInventoryAppService inventoryAppService)
        {
            Guard.ArgumentNotNull(inventoryAppService, nameof(inventoryAppService));

            _inventoryAppService = inventoryAppService;
        }

        #region Inventory

        public async Task<InventoryDTO> AddInventoryAsync(InventoryDTO inventoryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _inventoryAppService.AddNewInventoryAsync(inventoryDTO, serviceHeader);
        }

        public async Task<bool> UpdateInventoryAsync(InventoryDTO inventoryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _inventoryAppService.UpdateInventoryAsync(inventoryDTO, serviceHeader);
        }

        public async Task<List<InventoryDTO>> FindInventoriesAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _inventoryAppService.FindInventoriesAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<InventoryDTO>> FindInventoriesInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _inventoryAppService.FindInventoriesAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<InventoryDTO>> FindInventoriesByFilterInPageAsync(string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _inventoryAppService.FindInventoriesAsync(filter, pageIndex, pageSize, serviceHeader);
        }

        public InventoryDTO FindInventory(Guid policyTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inventoryAppService.FindInventory(policyTypeId, serviceHeader);
        }

     

        Task<List<InventoryDTO>> IInventoryService.FindInventoriesAsync()
        {
            throw new NotImplementedException();
        }

        Task<PageCollectionInfo<InventoryDTO>> IInventoryService.FindInventoriesInPageAsync(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        Task<PageCollectionInfo<InventoryDTO>> IInventoryService.FindInventoriesByFilterInPageAsync(string filter, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        InventoryDTO IInventoryService.FindInventory(Guid policyTypeId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
