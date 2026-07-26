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
    public class GeneralLedgerService : IGeneralLedgerService
    {
        private readonly IGeneralLedgerAppService _generalLedgerAppService;

        public GeneralLedgerService(
           IGeneralLedgerAppService generalLedgerAppService)
        {
            Guard.ArgumentNotNull(generalLedgerAppService, nameof(generalLedgerAppService));

            _generalLedgerAppService = generalLedgerAppService;
        }

        #region General Ledger

        public GeneralLedgerDTO AddGeneralLedger(GeneralLedgerDTO generalLedgerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.AddNewGeneralLedger(generalLedgerDTO, serviceHeader);
        }

        public bool UpdateGeneralLedger(GeneralLedgerDTO generalLedgerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.UpdateGeneralLedger(generalLedgerDTO, serviceHeader);
        }

        public GeneralLedgerEntryDTO AddGeneralLedgerEntry(GeneralLedgerEntryDTO generalLedgerEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.AddNewGeneralLedgerEntry(generalLedgerEntryDTO, serviceHeader);
        }

        public bool RemoveGeneralLedgerEntries(List<GeneralLedgerEntryDTO> generalLedgerEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.RemoveGeneralLedgerEntries(generalLedgerEntryDTOs, serviceHeader);
        }

        public bool AuditGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.AuditGeneralLedger(generalLedgerDTO, generalLedgerAuthOption, serviceHeader);
        }

        public bool AuthorizeGeneralLedger(GeneralLedgerDTO generalLedgerDTO, int generalLedgerAuthOption, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.AuthorizeGeneralLedger(generalLedgerDTO, generalLedgerAuthOption, moduleNavigationItemCode, serviceHeader);
        }

        public bool UpdateGeneralLedgerEntryCollection(Guid generalLedgerId, List<GeneralLedgerEntryDTO> generalLedgerEntryCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.UpdateGeneralLedgerEntryCollection(generalLedgerId, generalLedgerEntryCollection, serviceHeader);
        }

        public List<GeneralLedgerDTO> FindGeneralLedgers()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.FindGeneralLedgers(serviceHeader);
        }

        public PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgersInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.FindGeneralLedgers(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgersByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.FindGeneralLedgers(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<GeneralLedgerDTO> FindGeneralLedgersByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.FindGeneralLedgers(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public GeneralLedgerDTO FindGeneralLedger(Guid generalLedgerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.FindGeneralLedger(generalLedgerId, serviceHeader);
        }

        public List<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerId(Guid generalLedgerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.FindGeneralLedgerEntriesByGeneralLedgerId(generalLedgerId, serviceHeader);
        }

        public PageCollectionInfo<GeneralLedgerEntryDTO> FindGeneralLedgerEntriesByGeneralLedgerIdInPage(Guid generalLedgerId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _generalLedgerAppService.FindGeneralLedgerEntriesByGeneralLedgerId(generalLedgerId, pageIndex, pageSize, serviceHeader);
        }

        public BatchImportParseInfo ParseGeneralLedgerImportEntries(GeneralLedgerEntryDTO generalLedgerEntryDTO, string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _generalLedgerAppService.ParseGeneralLedgerImportEntries( generalLedgerEntryDTO, serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);
        }

        #endregion

    }
}
