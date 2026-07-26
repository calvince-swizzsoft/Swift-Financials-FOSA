using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ElectronicJournalService : IElectronicJournalService
    {
        private readonly IElectronicJournalAppService _electronicJournalAppService;

        public ElectronicJournalService(
            IElectronicJournalAppService electronicJournalAppService)
        {
            Guard.ArgumentNotNull(electronicJournalAppService, nameof(electronicJournalAppService));

            _electronicJournalAppService = electronicJournalAppService;
        }

        public ElectronicJournalDTO ParseElectronicJournalImport(string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _electronicJournalAppService.ParseElectronicJournalImport(serviceBrokerSettingsElement.FileUploadDirectory, fileName, ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString, serviceHeader);
        }

        public bool CloseElectronicJournal(ElectronicJournalDTO electronicJournalDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _electronicJournalAppService.CloseElectronicJournal(electronicJournalDTO, serviceBrokerSettingsElement.EncryptionPublicKeyPath, serviceBrokerSettingsElement.EncryptionPrivateKeyPath, serviceBrokerSettingsElement.EncryptionPassPhrase, serviceBrokerSettingsElement.FileExportDirectory, serviceHeader);
        }

        public bool ClearTruncatedCheque(TruncatedChequeDTO truncatedChequeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.ClearTruncatedCheque(truncatedChequeDTO, serviceHeader);
        }

        public bool MatchTruncatedChequePaymentVoucher(TruncatedChequeDTO truncatedChequeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.MatchTruncatedChequePaymentVoucher(truncatedChequeDTO, serviceHeader);
        }

        public List<ElectronicJournalDTO> FindElectronicJournals()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.FindElectronicJournals(serviceHeader);
        }

        public PageCollectionInfo<ElectronicJournalDTO> FindElectronicJournalsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.FindElectronicJournals(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public ElectronicJournalDTO FindElectronicJournal(Guid electronicJournalId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.FindElectronicJournal(electronicJournalId, serviceHeader);
        }

        public PageCollectionInfo<TruncatedChequeDTO> FindTruncatedChequesByElectronicJournalIdAndFilterInPage(Guid electronicJournalId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.FindTruncatedCheques(electronicJournalId, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<TruncatedChequeDTO> FindTruncatedChequesByElectronicJournalIdAndStatusAndFilterInPage(Guid electronicJournalId, int status, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.FindTruncatedCheques(electronicJournalId, status, text, pageIndex, pageSize, serviceHeader);
        }

        public TruncatedChequeDTO FindTruncatedCheque(Guid truncatedChequeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _electronicJournalAppService.FindTruncatedCheque(truncatedChequeId, serviceHeader);
        }
    }
}
