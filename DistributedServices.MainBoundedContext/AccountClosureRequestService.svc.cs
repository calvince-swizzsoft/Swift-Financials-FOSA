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
    public class AccountClosureRequestService : IAccountClosureRequestService
    {
        private readonly IAccountClosureRequestAppService _accountClosureRequestAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public AccountClosureRequestService(
            IAccountClosureRequestAppService accountClosureRequestAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(accountClosureRequestAppService, nameof(accountClosureRequestAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _accountClosureRequestAppService = accountClosureRequestAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Account Closure Request

        public AccountClosureRequestDTO AddAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _accountClosureRequestAppService.AddNewAccountClosureRequest(accountClosureRequestDTO, serviceHeader);
        }

        public bool ApproveAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureApprovalOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _accountClosureRequestAppService.ApproveAccountClosureRequest(accountClosureRequestDTO, accountClosureApprovalOption, serviceHeader);
        }

        public bool AuditAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureAuditOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _accountClosureRequestAppService.AuditAccountClosureRequest(accountClosureRequestDTO, accountClosureAuditOption, serviceHeader);
        }

        public bool SettleAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureSettlementOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _accountClosureRequestAppService.SettleAccountClosureRequest(accountClosureRequestDTO, accountClosureSettlementOption, serviceHeader);
        }

        public AccountClosureRequestDTO FindAccountClosureRequest(Guid accountClosureRequestId, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var accountClosureRequest = _accountClosureRequestAppService.FindAccountClosureRequest(accountClosureRequestId, serviceHeader);

            if (includeProductDescription && accountClosureRequest != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<AccountClosureRequestDTO> { accountClosureRequest }, serviceHeader);

            return accountClosureRequest;
        }

        public List<AccountClosureRequestDTO> FindAccountClosureRequests(bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var accountClosureRequests = _accountClosureRequestAppService.FindAccountClosureRequests(serviceHeader);

            if (includeProductDescription && accountClosureRequests != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(accountClosureRequests, serviceHeader);

            return accountClosureRequests;
        }

        public PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequestsInPage(int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var accountClosureRequests = _accountClosureRequestAppService.FindAccountClosureRequests(pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && accountClosureRequests != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(accountClosureRequests.PageCollection, serviceHeader);

            return accountClosureRequests;
        }

        public PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequestsByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var accountClosureRequests = _accountClosureRequestAppService.FindAccountClosureRequests(text, customerFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && accountClosureRequests != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(accountClosureRequests.PageCollection, serviceHeader);

            return accountClosureRequests;
        }

        public PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequestsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var accountClosureRequests = _accountClosureRequestAppService.FindAccountClosureRequests(startDate, endDate, status, text, customerFilter, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && accountClosureRequests != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(accountClosureRequests.PageCollection, serviceHeader);

            return accountClosureRequests;
        }

        #endregion
    }
}
