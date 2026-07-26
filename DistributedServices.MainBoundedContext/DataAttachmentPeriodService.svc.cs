using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
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
    public class DataAttachmentPeriodService : IDataAttachmentPeriodService
    {
        private readonly IDataAttachmentPeriodAppService _dataAttachmentPeriodAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public DataAttachmentPeriodService(
            IDataAttachmentPeriodAppService dataAttachmentPeriodAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(dataAttachmentPeriodAppService, nameof(dataAttachmentPeriodAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _dataAttachmentPeriodAppService = dataAttachmentPeriodAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Data Attachment Period

        public DataAttachmentPeriodDTO AddDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.AddNewDataAttachmentPeriod(dataAttachmentPeriodDTO, serviceHeader);
        }

        public bool UpdateDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.UpdateDataAttachmentPeriod(dataAttachmentPeriodDTO, serviceHeader);
        }

        public bool CloseDataAttachmentPeriod(DataAttachmentPeriodDTO dataAttachmentPeriodDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.CloseDataAttachmentPeriod(dataAttachmentPeriodDTO, serviceHeader);
        }

        public List<DataAttachmentPeriodDTO> FindDataAttachmentPeriods()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.FindDataAttachmentPeriods(serviceHeader);
        }

        public PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriodsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.FindDataAttachmentPeriods(pageIndex, pageSize, serviceHeader);
        }

        public DataAttachmentPeriodDTO FindDataAttachmentPeriod(Guid dataAttachmentPeriodId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.FindDataAttachmentPeriod(dataAttachmentPeriodId, serviceHeader);
        }

        public DataAttachmentEntryDTO AddDataAttachmentEntry(DataAttachmentEntryDTO dataAttachmentEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.AddNewDataAttachmentEntry(dataAttachmentEntryDTO, serviceHeader);
        }

        public bool RemoveDataAttachmentEntries(List<DataAttachmentEntryDTO> dataAttachmentEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.RemoveDataAttachmentEntries(dataAttachmentEntryDTOs, serviceHeader);
        }

        public PageCollectionInfo<DataAttachmentEntryDTO> FindDataAttachmentEntriesByDataAttachmentPeriodIdAndFilterInPage(Guid dataAttachmentPeriodId, string text, int pageIndex, int pageSize, bool includeProductDescription)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var dataAttachmentEntries = _dataAttachmentPeriodAppService.FindDataAttachmentEntriesByDataAttachmentPeriodId(dataAttachmentPeriodId, text, pageIndex, pageSize, serviceHeader);

            if (includeProductDescription && dataAttachmentEntries != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(dataAttachmentEntries.PageCollection, serviceHeader);

            return dataAttachmentEntries;
        }

        public PageCollectionInfo<DataAttachmentPeriodDTO> FindDataAttachmentPeriodsByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.FindDataAttachmentPeriods(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public DataAttachmentPeriodDTO FindCurrentDataAttachmentPeriod()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _dataAttachmentPeriodAppService.FindCurrentDataAttachmentPeriod(serviceHeader);
        }

        #endregion
    }
}
