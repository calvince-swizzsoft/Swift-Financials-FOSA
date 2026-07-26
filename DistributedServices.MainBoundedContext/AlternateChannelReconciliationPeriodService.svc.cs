using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
//using System.Configuration;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AlternateChannelReconciliationPeriodService : IAlternateChannelReconciliationPeriodService
    {
        private readonly IAlternateChannelReconciliationPeriodAppService _alternateChannelReconciliationPeriodAppService;

        public AlternateChannelReconciliationPeriodService(
            IAlternateChannelReconciliationPeriodAppService alternateChannelReconciliationPeriodAppService)
        {
            Guard.ArgumentNotNull(alternateChannelReconciliationPeriodAppService, nameof(alternateChannelReconciliationPeriodAppService));

            _alternateChannelReconciliationPeriodAppService = alternateChannelReconciliationPeriodAppService;
        }

        #region AlternateChannel Reconciliation Period

        public AlternateChannelReconciliationPeriodDTO AddAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.AddNewAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodDTO, serviceHeader);
        }

        public bool UpdateAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.UpdateAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodDTO, serviceHeader);
        }

        public bool CloseAlternateChannelReconciliationPeriod(AlternateChannelReconciliationPeriodDTO alternateChannelReconciliationPeriodDTO, int alternateChannelReconciliationPeriodAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.CloseAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodDTO, alternateChannelReconciliationPeriodAuthOption, serviceHeader);
        }

        public List<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriods()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriods(serviceHeader);
        }

        public PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriodsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriods(pageIndex, pageSize, serviceHeader);
        }

        public AlternateChannelReconciliationPeriodDTO FindAlternateChannelReconciliationPeriod(Guid alternateChannelReconciliationPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriod(alternateChannelReconciliationPeriodId, serviceHeader);
        }

        public AlternateChannelReconciliationEntryDTO AddAlternateChannelReconciliationEntry(AlternateChannelReconciliationEntryDTO alternateChannelReconciliationEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.AddNewAlternateChannelReconciliationEntry(alternateChannelReconciliationEntryDTO, serviceHeader);
        }

        public bool RemoveAlternateChannelReconciliationEntries(List<AlternateChannelReconciliationEntryDTO> alternateChannelReconciliationEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.RemoveAlternateChannelReconciliationEntries(alternateChannelReconciliationEntryDTOs, serviceHeader);
        }

        public PageCollectionInfo<AlternateChannelReconciliationEntryDTO> FindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodIdAndFilterInPage(Guid alternateChannelReconciliationPeriodId, int status, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationEntriesByAlternateChannelReconciliationPeriodId(alternateChannelReconciliationPeriodId, status, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<AlternateChannelReconciliationPeriodDTO> FindAlternateChannelReconciliationPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelReconciliationPeriodAppService.FindAlternateChannelReconciliationPeriods(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public List<BatchImportEntryWrapper> ParseAlternateChannelReconciliationImport(Guid alternateChannelReconciliationPeriodId, string fileName)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _alternateChannelReconciliationPeriodAppService.ParseAlternateChannelReconciliationImport(alternateChannelReconciliationPeriodId, serviceBrokerSettingsElement.FileUploadDirectory, fileName, serviceHeader);
        }

        #endregion
    }
}
