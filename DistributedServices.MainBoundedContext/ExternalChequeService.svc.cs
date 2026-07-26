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
    public class ExternalChequeService : IExternalChequeService
    {
        private readonly IExternalChequeAppService _externalChequeAppService;

        public ExternalChequeService(
           IExternalChequeAppService externalChequeAppService)
        {
            Guard.ArgumentNotNull(externalChequeAppService, nameof(externalChequeAppService));

            _externalChequeAppService = externalChequeAppService;
        }

        #region External Cheque

        public ExternalChequeDTO AddExternalCheque(ExternalChequeDTO externalChequeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.AddNewExternalCheque(externalChequeDTO, serviceHeader);
        }

        public List<ExternalChequeDTO> FindUnClearedExternalChequesByCustomerAccountId(Guid customerAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindUnClearedExternalChequesByCustomerAccountId(customerAccountId, serviceHeader);
        }

        public bool MarkExternalChequeCleared(Guid externalChequeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.MarkExternalChequeCleared(externalChequeId, serviceHeader);
        }

        public List<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerIdAndFilter(Guid tellerId, string text)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(tellerId, text, serviceHeader);
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerIdAndFilterInPage(Guid tellerId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindUnTransferredExternalChequesByTellerId(tellerId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ExternalChequeDTO> FindExternalChequesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindExternalCheques(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ExternalChequeDTO> FindExternalChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindExternalCheques(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public bool TransferExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, TellerDTO tellerDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.TransferExternalCheques(externalChequeDTOs, tellerDTO, moduleNavigationItemCode, serviceHeader);
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalChequesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindUnClearedExternalCheques(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindUnClearedExternalCheques(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public bool ClearExternalCheque(ExternalChequeDTO externalChequeDTO, int clearingOption, int moduleNavigationItemCode, UnPayReasonDTO unPayReasonDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.ClearExternalCheque(externalChequeDTO, clearingOption, moduleNavigationItemCode, unPayReasonDTO, serviceHeader);
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnBankedExternalChequesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindUnBankedExternalCheques(text, pageIndex, pageSize, serviceHeader);
        }

        public bool BankExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.BankExternalCheques(externalChequeDTOs, bankLinkageDTO, moduleNavigationItemCode, serviceHeader);
        }

        public List<ExternalChequePayableDTO> FindExternalChequePayablesByExternalChequeId(Guid externalChequeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.FindExternalChequePayablesByExternalChequeId(externalChequeId, serviceHeader);
        }

        public bool UpdateExternalChequePayablesByExternalChequeId(Guid externalChequeId, List<ExternalChequePayableDTO> externalChequePayables)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _externalChequeAppService.UpdateExternalChequePayables(externalChequeId, externalChequePayables, serviceHeader);
        }

        #endregion
    }
}
