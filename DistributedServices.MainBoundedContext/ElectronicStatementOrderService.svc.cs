using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ElectronicStatementOrderService : IElectronicStatementOrderService
    {
        private readonly IElectronicStatementOrderAppService _electronicStatementOrderAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public ElectronicStatementOrderService(
            IElectronicStatementOrderAppService electronicStatementOrderAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(electronicStatementOrderAppService, nameof(electronicStatementOrderAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _electronicStatementOrderAppService = electronicStatementOrderAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Standing Order

        public List<ElectronicStatementOrderDTO> FindElectronicStatementOrders(bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var electronicStatementOrders = _electronicStatementOrderAppService.FindElectronicStatementOrders(serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(electronicStatementOrders, serviceHeader);

            return electronicStatementOrders;
        }

        public PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrdersInPage(int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var electronicStatementOrders = _electronicStatementOrderAppService.FindElectronicStatementOrders(pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(electronicStatementOrders.PageCollection, serviceHeader);

            return electronicStatementOrders;
        }

        public PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var electronicStatementOrders = _electronicStatementOrderAppService.FindElectronicStatementOrders(text, customerFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(electronicStatementOrders.PageCollection, serviceHeader);

            return electronicStatementOrders;
        }

        public PageCollectionInfo<ElectronicStatementOrderHistoryDTO> FindElectronicStatementOrderHistoryByElectronicStatementOrderIdInPage(Guid electronicStatementOrderId, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var electronicStatementOrderHistory = _electronicStatementOrderAppService.FindElectronicStatementOrderHistory(electronicStatementOrderId, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && electronicStatementOrderHistory != null && electronicStatementOrderHistory.PageCollection != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(electronicStatementOrderHistory.PageCollection, serviceHeader);

            return electronicStatementOrderHistory;
        }

        public ElectronicStatementOrderDTO FindElectronicStatementOrder(Guid electronicStatementOrderId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicStatementOrderAppService.FindElectronicStatementOrder(electronicStatementOrderId, serviceHeader);
        }

        public ElectronicStatementOrderHistoryDTO FindElectronicStatementOrderHistory(Guid electronicStatementOrderHistoryId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicStatementOrderAppService.FindElectronicStatementOrderHistory(electronicStatementOrderHistoryId, serviceHeader);
        }

        public ElectronicStatementOrderDTO AddElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicStatementOrderAppService.AddNewElectronicStatementOrder(electronicStatementOrderDTO, serviceHeader);
        }

        public bool UpdateElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicStatementOrderAppService.UpdateElectronicStatementOrder(electronicStatementOrderDTO, serviceHeader);
        }

        public bool FixSkippedElectronicStatementOrders(DateTime targetDate)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicStatementOrderAppService.FixSkippedElectronicStatementOrders(targetDate, serviceHeader);
        }

        public List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerAccountId(Guid customerAccountId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var electronicStatementOrders = _electronicStatementOrderAppService.FindElectronicStatementOrdersByCustomerAccountId(customerAccountId, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(electronicStatementOrders, serviceHeader);

            return electronicStatementOrders;
        }

        public List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerId(Guid customerId, int customerAccountTypeProductCode, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var electronicStatementOrders = _electronicStatementOrderAppService.FindElectronicStatementOrdersByCustomerId(customerId, customerAccountTypeProductCode, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(electronicStatementOrders, serviceHeader);

            return electronicStatementOrders;
        }

        #endregion
    }
}
