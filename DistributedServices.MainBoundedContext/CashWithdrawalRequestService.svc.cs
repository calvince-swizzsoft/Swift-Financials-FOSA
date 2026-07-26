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
    public class CashWithdrawalRequestService : ICashWithdrawalRequestService
    {
        private readonly ICashWithdrawalRequestAppService _cashWithdrawalRequestAppService;

        public CashWithdrawalRequestService(
           ICashWithdrawalRequestAppService cashWithdrawalRequestAppService)
        {
            Guard.ArgumentNotNull(cashWithdrawalRequestAppService, nameof(cashWithdrawalRequestAppService));

            _cashWithdrawalRequestAppService = cashWithdrawalRequestAppService;
        }

        #region Customer Transaction Auth Request

        public CashWithdrawalRequestDTO AddCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.AddNewCashWithdrawalRequest(cashWithdrawalRequestDTO, serviceHeader);
        }

        public bool AuthorizeCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, int customerTransactionAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.AuthorizeCashWithdrawalRequest(cashWithdrawalRequestDTO, customerTransactionAuthOption, serviceHeader);
        }

        public bool PayCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, PaymentVoucherDTO paymentVoucherDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.PayCashWithdrawalRequest(cashWithdrawalRequestDTO, paymentVoucherDTO, serviceHeader);
        }

        public List<CashWithdrawalRequestDTO> FindCashWithdrawalRequests()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.FindCashWithdrawalRequests(serviceHeader);
        }

        public List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByCustomerAccountId(CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.FindMatureCashWithdrawalRequestsByCustomerAccountId(customerAccountDTO, serviceHeader);
        }

        public List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByChartOfAccountId(Guid chartOfAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.FindMatureCashWithdrawalRequestsByChartOfAccountId(chartOfAccountId, serviceHeader);
        }

        public PageCollectionInfo<CashWithdrawalRequestDTO> FindCashWithdrawalRequestsByFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.FindCashWithdrawalRequests(startDate, endDate, status, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public CashWithdrawalRequestDTO FindCashWithdrawalRequest(Guid cashWithdrawalRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _cashWithdrawalRequestAppService.FindCashWithdrawalRequest(cashWithdrawalRequestId, serviceHeader);
        }

        #endregion
    }
}
