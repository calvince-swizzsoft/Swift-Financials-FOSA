using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
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
    public class FixedDepositService : IFixedDepositService
    {
        private readonly IFixedDepositAppService _fixedDepositAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public FixedDepositService(
            IFixedDepositAppService fixedDepositAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(fixedDepositAppService, nameof(fixedDepositAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _fixedDepositAppService = fixedDepositAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Fixed Deposit

        public FixedDepositDTO InvokeFixedDeposit(FixedDepositDTO fixedDepositDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.InvokeFixedDeposit(fixedDepositDTO, serviceHeader);
        }

        public bool AuditFixedDeposit(FixedDepositDTO fixedDepositDTO, int fixedDepositAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.AuditFixedDeposit(fixedDepositDTO, fixedDepositAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool RevokeFixedDeposits(List<FixedDepositDTO> fixedDepositDTOs, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.RevokeFixedDeposits(fixedDepositDTOs, moduleNavigationItemCode, serviceHeader);
        }

        public bool PayFixedDeposit(FixedDepositDTO fixedDepositDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.PayFixedDeposit(fixedDepositDTO, moduleNavigationItemCode, serviceHeader);
        }

        public List<FixedDepositDTO> FindFixedDeposits(bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var fixedDeposits = _fixedDepositAppService.FindFixedDeposits(serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(fixedDeposits, serviceHeader);

            return fixedDeposits;
        }

        public List<FixedDepositDTO> FindFixedDepositsByCustomerAccountId(Guid customerAccountId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var fixedDeposits = _fixedDepositAppService.FindFixedDepositsByCustomerAccountId(customerAccountId, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(fixedDeposits, serviceHeader);

            return fixedDeposits;
        }

        public PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByBranchIdInPage(Guid branchId, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var fixedDeposits = _fixedDepositAppService.FindFixedDeposits(branchId, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(fixedDeposits.PageCollection, serviceHeader);

            return fixedDeposits;
        }

        public PageCollectionInfo<FixedDepositDTO> FindPayableFixedDepositsByFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var fixedDeposits = _fixedDepositAppService.FindPayableFixedDeposits(startDate, endDate, text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(fixedDeposits.PageCollection, serviceHeader);

            return fixedDeposits;
        }

        public PageCollectionInfo<FixedDepositDTO> FindRevocableFixedDepositsByFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var fixedDeposits = _fixedDepositAppService.FindRevocableFixedDeposits(startDate, endDate, text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(fixedDeposits.PageCollection, serviceHeader);

            return fixedDeposits;
        }

        public PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByFilterInPage(string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var fixedDeposits = _fixedDepositAppService.FindFixedDeposits(text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(fixedDeposits.PageCollection, serviceHeader);

            return fixedDeposits;
        }

        public PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByStatusAndFilterInPage(int status, string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var fixedDeposits = _fixedDepositAppService.FindFixedDepositsByStatus(status, text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(fixedDeposits.PageCollection, serviceHeader);

            return fixedDeposits;
        }

        public FixedDepositDTO FindFixedDeposit(Guid fixedDepositId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.FindFixedDeposit(fixedDepositId, serviceHeader);
        }

        public List<FixedDepositPayableDTO> FindFixedDepositPayablesByFixedDepositId(Guid fixedDepositId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.FindFixedDepositPayablesByFixedDepositId(fixedDepositId, serviceHeader);
        }

        public bool UpdateFixedDepositPayablesByFixedDepositId(Guid fixedDepositId, List<FixedDepositPayableDTO> fixedDepositPayables)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.UpdateFixedDepositPayables(fixedDepositId, fixedDepositPayables, serviceHeader);
        }

        public bool ExecutePayableFixedDeposits(DateTime targetDate, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositAppService.ExecutePayableFixedDeposits(targetDate, pageSize, serviceHeader);
        }

        #endregion
    }
}
