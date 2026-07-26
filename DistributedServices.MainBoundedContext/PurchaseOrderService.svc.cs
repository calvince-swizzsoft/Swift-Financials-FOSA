using Application.MainBoundedContext.DTO;
//using Application.MainBoundedContext.DTO.CustomerModule;
using Application.MainBoundedContext.DTO.InventoryModule;
using Application.MainBoundedContext.CustomerModule.Services;
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
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderAppService _policyAppService;

        public PurchaseOrderService(IPurchaseOrderAppService policyAppService)
        {
            Guard.ArgumentNotNull(policyAppService, nameof(policyAppService));

            _policyAppService = policyAppService;
        }

        #region PurchaseOrder 

        public async Task<PurchaseOrderDTO> AddPurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.AddNewPurchaseOrderAsync(purchaseOrderDTO, serviceHeader);
        }

        public async Task<PurchaseOrderDTO> FindPurchaseOrderAsync(Guid policyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.FindPurchaseOrderAsync(policyId, serviceHeader);
        }

        public async Task<List<PurchaseOrderDTO>> FindPurchaseOrderByCodeAsync(string code)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.FindPurchaseOrderAsync(code, serviceHeader);
        }

        public async Task<List<PurchaseOrderDTO>> FindPurchaseOrdersAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.FindPurchaseOrdersAsync(serviceHeader);
        }


        public async Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrdersByFilterInPageAsync(int itemType, string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.FindPurchaseOrdersAsync(itemType, filter, pageIndex, pageSize, serviceHeader);
        }


        public async Task<PageCollectionInfo<PurchaseOrderDTO>> FindPurchaseOrderInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.FindPurchaseOrderAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<bool> UpdatePurchaseOrderAsync(PurchaseOrderDTO itemRegisterDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.UpdatePurchaseOrderAsync(itemRegisterDTO, serviceHeader);
        }


        public async Task<PurchaseOrderEntryDTO> AddPurchaseOrderEntryAsync(PurchaseOrderEntryDTO purchaseOrderEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.AddNewPurchaseOrderEntryAsync(purchaseOrderEntryDTO, serviceHeader);
        }
        public async Task<List<PurchaseOrderEntryDTO>> FindPurchaseOrderEntriesByPurchaseOrderIdAsync(Guid policyId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _policyAppService.FindPurchaseOrderEntriesByPurchaseOrderIdAsync(policyId, serviceHeader);
        }

        //public bool RemovePurchaseOrderEntries(List<PurchaseOrderEntryDTO> purchaseOrderEntryDTOs)
        //{
        //    var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

        //    return _policyAppService.RemovePurchaseOrderEntries(purchaseOrderEntryDTOs, serviceHeader);
        //}

        //public async Task<PurchaseOrderEntryDTO> FindPurchaseOrderEntryById(Guid id)
        //{
        //    var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

        //    return await _policyAppService.FindPurchaseOrderEntryById(id, serviceHeader);
        //}

        //public List<PurchaseOrderEntryDTO> FindPurchaseOrderEntriesByPurchaseOrderId(Guid purchaseOrderId, string description)
        //{
        //    var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

        //    var purchaseOrderEntries = _policyAppService.FindPurchaseOrderEntriesByPurchaseOrderId(purchaseOrderId, serviceHeader);

        //    if (includeDescription && purchaseOrderEntries != null)
        //        _.FetchCustomerAccountsProductDescription(purchaseOrderEntries, serviceHeader);

        //    return purchaseOrderEntries;
        //}

        #endregion
    }
}
