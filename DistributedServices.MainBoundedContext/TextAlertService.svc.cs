using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.Services;
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
    public class TextAlertService : ITextAlertService
    {
        private readonly ITextAlertAppService _textAlertAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public TextAlertService(ITextAlertAppService textAlertAppService, ISqlCommandAppService sqlCommandAppService)
        {
            Guard.ArgumentNotNull(textAlertAppService, nameof(textAlertAppService));
            Guard.ArgumentNotNull(sqlCommandAppService, nameof(sqlCommandAppService));

            _textAlertAppService = textAlertAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        #region TextAlert

        public bool AddBulkMessage(BulkMessageDTO bulkMessageDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.AddNewBulkMessage(bulkMessageDTO, serviceHeader);
        }

        public bool AddUSSDMessage(USSDMessageDTO ussdMessageDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.AddNewUSSDMessage(ussdMessageDTO, serviceHeader);
        }

        public bool AddTextAlerts(List<TextAlertDTO> textAlertDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.AddNewTextAlerts(textAlertDTOs, serviceHeader);
        }

        public bool AddTextAlertsWithHistory(GroupTextAlertDTO groupTextAlertDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.AddTextAlertsWithHistory(groupTextAlertDTO, serviceHeader);
        }

        public bool AddQuickTextAlert(QuickTextAlertDTO quickTextAlertDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.AddQuickTextAlert(quickTextAlertDTO, serviceHeader);
        }

        public bool UpdateTextAlert(TextAlertDTO textAlertDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.UpdateTextAlert(textAlertDTO, serviceHeader);
        }

        public List<TextAlertDTO> FindTextAlerts()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlerts(serviceHeader);
        }

        public List<TextAlertDTO> FindTextAlertsByMessageReference(string messageReference)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlertsByMessageReference(messageReference, serviceHeader);
        }

        public List<TextAlertDTO> FindTextAlertsByDLRStatus(int dlrStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlertsByDLRStatus(dlrStatus, serviceHeader);
        }

        public PageCollectionInfo<TextAlertDTO> FindTextAlertsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlerts(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<TextAlertDTO> FindTextAlertsByFilterInPage(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlerts(dlrStatus, text, pageIndex, pageSize, daysCap, serviceHeader);
        }

        public PageCollectionInfo<TextAlertDTO> FindTextAlertsByDateRangeAndFilterInPage(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlerts(dlrStatus, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public TextAlertDTO FindTextAlert(Guid textAlertId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlert(textAlertId, serviceHeader);
        }

        public List<TextAlertDTO> FindTextAlertsByDLRStatusAndOrigin(int dlrStatus, int origin)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindTextAlertsByDLRStatusAndOrigin(dlrStatus, origin, serviceHeader);
        }

        public bool ProcessInboundMessage(InboundMessageDTO inboundMessageDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.ProcessInboundMessage(inboundMessageDTO, serviceHeader);
        }

        public List<CommissionDTO> FindCommissionsBySystemTransactionCode(int systemTransactionCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.FindCommissions(systemTransactionCode, serviceHeader);
        }

        public bool UpdateCommissionsBySystemTransactionCode(int systemTransactionCode, CommissionDTO[] commissionDTOs, int chargeBenefactor)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _textAlertAppService.UpdateCommissions(systemTransactionCode, commissionDTOs, chargeBenefactor, serviceHeader);
        }

        public List<MonthlySummaryValuesDTO> FindTextAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _sqlCommandAppService.FindTextAlertsMonthlyStatatistics(companyId, startDate, endDate, serviceHeader);
        }

        #endregion
    }
}
