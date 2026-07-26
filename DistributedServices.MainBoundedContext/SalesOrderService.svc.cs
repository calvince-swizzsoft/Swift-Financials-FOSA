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
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderAppService _salesOrderAppService;
        private readonly ISalesOrderAppService _salesOrderEntryRepository;

        public SalesOrderService(ISalesOrderAppService salesOrderAppService, ISalesOrderAppService salesOrderEntryRepository)
        {
            Guard.ArgumentNotNull(salesOrderAppService, nameof(salesOrderAppService));

            _salesOrderAppService = salesOrderAppService;
            _salesOrderEntryRepository = salesOrderEntryRepository;
        }

        #region SalesOrder

        public async Task<SalesOrderDTO> AddSalesOrderAsync(SalesOrderDTO salesOrderDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderAppService.AddNewSalesOrderAsync(salesOrderDTO, serviceHeader);
        }

        public async Task<SalesOrderDTO> FindSalesOrderAsync(Guid salesOrderId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderAppService.FindSalesOrderAsync(salesOrderId, serviceHeader);
        }

        public async Task<List<SalesOrderDTO>> FindSalesOrderByCodeAsync(string code)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderAppService.FindSalesOrderAsync(code, serviceHeader);
        }

        public async Task<List<SalesOrderDTO>> FindSalesOrdersAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderAppService.FindSalesOrdersAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersByFilterInPageAsync(int itemType, string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderAppService.FindSalesOrdersAsync(itemType, filter, pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderAppService.FindSalesOrdersAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<bool> UpdateSalesOrderAsync(SalesOrderDTO salesOrderDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderAppService.UpdateSalesOrderAsync(salesOrderDTO, serviceHeader);
        }

        #endregion

        #region salesOrderEntry

        public async Task<SalesOrderEntryDTO> AddSalesOrderEntryAsync(SalesOrderEntryDTO salesOrderEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderEntryRepository.AddNewSalesOrderEntryAsync(salesOrderEntryDTO, serviceHeader);
        }

        public async Task<List<SalesOrderEntryDTO>> FindSalesOrderEntriesBySalesOrderIdAsync(Guid salesOrderId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _salesOrderEntryRepository.FindSalesOrderEntriesBySalesOrderIdAsync(salesOrderId, serviceHeader);
        }

        #endregion
    }
}
