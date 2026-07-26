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
    public class MobileToBankRequestService : IMobileToBankRequestService
    {
        private readonly IMobileToBankRequestAppService _mobileToBankRequestAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public MobileToBankRequestService(
            IMobileToBankRequestAppService mobileToBankRequestAppService,
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(mobileToBankRequestAppService, nameof(mobileToBankRequestAppService));
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _mobileToBankRequestAppService = mobileToBankRequestAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        #region Mobile To Bank Request

        public MobileToBankRequestDTO AddMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _mobileToBankRequestAppService.AddNewMobileToBankRequest(mobileToBankRequestDTO, serviceHeader);
        }

        public bool ReconcileMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _mobileToBankRequestAppService.ReconcileMobileToBankRequest(mobileToBankRequestDTO, serviceHeader);
        }

        public bool AuditMobileToBankRequestReconciliation(MobileToBankRequestDTO mobileToBankRequestDTO, int requestAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _mobileToBankRequestAppService.AuditMobileToBankRequestReconciliation(mobileToBankRequestDTO, requestAuthOption, serviceHeader);
        }

        public List<MobileToBankRequestDTO> FindMobileToBankRequests()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _mobileToBankRequestAppService.FindMobileToBankRequests(serviceHeader);
        }

        public PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequestsByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var mobileToBankRequests = _mobileToBankRequestAppService.FindMobileToBankRequests(status, startDate, endDate, text, pageIndex, pageSize, serviceHeader);

            if (mobileToBankRequests != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(mobileToBankRequests.PageCollection, serviceHeader);

            return mobileToBankRequests;
        }

        public PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequestsByStatusRecordStatusAndFilterInPage(int status, int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var mobileToBankRequests = _mobileToBankRequestAppService.FindMobileToBankRequests(status, recordStatus, startDate, endDate, text, pageIndex, pageSize, serviceHeader);

            if (mobileToBankRequests != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(mobileToBankRequests.PageCollection, serviceHeader);

            return mobileToBankRequests;
        }

        public MobileToBankRequestDTO FindMobileToBankRequest(Guid mobileToBankRequestId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _mobileToBankRequestAppService.FindMobileToBankRequest(mobileToBankRequestId, serviceHeader);
        }

        public PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequestsByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var mobileToBankRequests = _mobileToBankRequestAppService.FindMobileToBankRequests(startDate, endDate, text, pageIndex, pageSize, serviceHeader);

            if (mobileToBankRequests != null)
                _customerAccountAppService.FetchCustomerAccountsProductDescription(mobileToBankRequests.PageCollection, serviceHeader);

            return mobileToBankRequests;
        }

        #endregion
    }
}