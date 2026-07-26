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
    public class StandingOrderService : IStandingOrderService
    {
        private readonly IStandingOrderAppService _standingOrderAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public StandingOrderService(
            IStandingOrderAppService standingOrderAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(standingOrderAppService, nameof(standingOrderAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _standingOrderAppService = standingOrderAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Standing Order

        public List<StandingOrderDTO> FindStandingOrders(bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrders = _standingOrderAppService.FindStandingOrders(serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrders, serviceHeader);

            return standingOrders;
        }

        public PageCollectionInfo<StandingOrderDTO> FindStandingOrdersInPage(int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrders = _standingOrderAppService.FindStandingOrders(pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrders.PageCollection, serviceHeader);

            return standingOrders;
        }

        public PageCollectionInfo<StandingOrderDTO> FindStandingOrdersByFilterInPage(string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrders = _standingOrderAppService.FindStandingOrders(text, customerAccountFilter, customerFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrders.PageCollection, serviceHeader);

            return standingOrders;
        }

        public PageCollectionInfo<StandingOrderDTO> FindStandingOrdersByTriggerAndFilterInPage(int trigger, string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrders = _standingOrderAppService.FindStandingOrders(trigger, text, customerAccountFilter, customerFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrders.PageCollection, serviceHeader);

            return standingOrders;
        }

        public PageCollectionInfo<StandingOrderHistoryDTO> FindStandingOrderHistoryByStandingOrderIdInPage(Guid standingOrderId, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrderHistory = _standingOrderAppService.FindStandingOrderHistory(standingOrderId, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && standingOrderHistory != null && standingOrderHistory.PageCollection != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrderHistory.PageCollection, serviceHeader);

            return standingOrderHistory;
        }

        public StandingOrderDTO FindStandingOrder(Guid standingOrderId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _standingOrderAppService.FindStandingOrder(standingOrderId, serviceHeader);
        }

        public StandingOrderDTO AddStandingOrder(StandingOrderDTO standingOrderDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _standingOrderAppService.AddNewStandingOrder(standingOrderDTO, serviceHeader);
        }

        public bool UpdateStandingOrder(StandingOrderDTO standingOrderDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _standingOrderAppService.UpdateStandingOrder(standingOrderDTO, serviceHeader);
        }

        public List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrders = _standingOrderAppService.FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccountId, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrders, serviceHeader);

            return standingOrders;
        }

        public List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrders = _standingOrderAppService.FindStandingOrdersByBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrders, serviceHeader);

            return standingOrders;
        }

        public List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerId(Guid benefactorCustomerId, int benefactorCustomerAccountTypeProductCode, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var standingOrders = _standingOrderAppService.FindStandingOrdersByBenefactorCustomerId(benefactorCustomerId, benefactorCustomerAccountTypeProductCode, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrders, serviceHeader);

            return standingOrders;
        }

        public bool AutoCreateStandindOrders(Guid benefactorProductId, int benefactorProductCode, Guid beneficiaryProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _standingOrderAppService.AutoCreateStandindOrders(benefactorProductId, benefactorProductCode, beneficiaryProductId, serviceHeader);
        }

        public bool FixSkippedStandingOrders(DateTime targetDate, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _standingOrderAppService.FixSkippedStandingOrders(targetDate, pageSize, serviceHeader);
        }

        #endregion
    }
}
