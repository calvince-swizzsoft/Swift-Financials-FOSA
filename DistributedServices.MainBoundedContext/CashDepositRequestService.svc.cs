using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
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
    public class CashDepositRequestService : ICashDepositRequestService
    {
        private readonly ICashDepositRequestAppService _cashDepositRequestAppService;

        public CashDepositRequestService(
           ICashDepositRequestAppService cashDepositRequestAppService)
        {
            Guard.ArgumentNotNull(cashDepositRequestAppService, nameof(cashDepositRequestAppService));

            _cashDepositRequestAppService = cashDepositRequestAppService;
        }

        #region Customer Transaction Auth Request

        public CashDepositRequestDTO AddCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashDepositRequestAppService.AddNewCashDepositRequest(cashDepositRequestDTO, serviceHeader);
        }

        public bool AuthorizeCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, int customerTransactionAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashDepositRequestAppService.AuthorizeCashDepositRequest(cashDepositRequestDTO, customerTransactionAuthOption, serviceHeader);
        }

        public bool PostCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashDepositRequestAppService.PostCashDepositRequest(cashDepositRequestDTO, serviceHeader);
        }

        public List<CashDepositRequestDTO> FindCashDepositRequests()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashDepositRequestAppService.FindCashDepositRequests(serviceHeader);
        }

        public List<CashDepositRequestDTO> FindActionableCashDepositRequestsByCustomerAccount(CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashDepositRequestAppService.FindActionableCashDepositRequestsByCustomerAccount(customerAccountDTO, serviceHeader);
        }

        public PageCollectionInfo<CashDepositRequestDTO> FindCashDepositRequestsByFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashDepositRequestAppService.FindCashDepositRequests(startDate, endDate, status, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public CashDepositRequestDTO FindCashDepositRequest(Guid cashDepositRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashDepositRequestAppService.FindCashDepositRequest(cashDepositRequestId, serviceHeader);
        }

        #endregion
    }
}
