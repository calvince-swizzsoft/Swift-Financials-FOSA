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
    public class BankToMobileRequestService : IBankToMobileRequestService
    {
        private readonly IBankToMobileRequestAppService _bankToMobileRequestAppService;

        public BankToMobileRequestService(
            IBankToMobileRequestAppService bankToMobileRequestAppService)
        {
            Guard.ArgumentNotNull(bankToMobileRequestAppService, nameof(bankToMobileRequestAppService));

            _bankToMobileRequestAppService = bankToMobileRequestAppService;
        }

        #region Bank To Mobile Request

        public BankToMobileRequestDTO AddBankToMobileRequest(BankToMobileRequestDTO bankToMobileRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.AddNewBankToMobileRequest(bankToMobileRequestDTO, serviceHeader);
        }

        public bool UpdateBankToMobileRequestResponse(Guid bankToMobileRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.UpdateBankToMobileRequestResponse(bankToMobileRequestId, outgoingPlainTextPayload, outgoingCipherTextPayload, serviceHeader);
        }

        public bool UpdateBankToMobileRequestIPNStatus(Guid bankToMobileRequestId, int ipnStatus, string ipnResponse)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.UpdateBankToMobileRequestIPNStatus(bankToMobileRequestId, ipnStatus, ipnResponse, serviceHeader);
        }

        public bool ResetBankToMobileRequestsIPNStatus(Guid[] bankToMobileRequestIds)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.ResetBankToMobileRequestsIPNStatus(bankToMobileRequestIds, serviceHeader);
        }

        public List<BankToMobileRequestDTO> FindBankToMobileRequests()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.FindBankToMobileRequests(serviceHeader);
        }

        public BankToMobileRequestDTO FindBankToMobileRequest(Guid bankToMobileRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.FindBankToMobileRequest(bankToMobileRequestId, serviceHeader);
        }

        public PageCollectionInfo<BankToMobileRequestDTO> FindBankToMobileRequestsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.FindBankToMobileRequests(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<BankToMobileRequestDTO> FindThirdPartyNotifiableBankToMobileRequestsByFilterInPage(string text, int pageIndex, int pageSize, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankToMobileRequestAppService.FindThirdPartyNotifiableBankToMobileRequests(text, pageIndex, pageSize, daysCap, serviceHeader);
        }

        #endregion
    }
}
