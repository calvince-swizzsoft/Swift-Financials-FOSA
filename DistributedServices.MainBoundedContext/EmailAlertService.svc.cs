using Application.MainBoundedContext.DTO;
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
    public class EmailAlertService : IEmailAlertService
    {
        private readonly IEmailAlertAppService _emailAlertAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public EmailAlertService(IEmailAlertAppService emailAlertAppService, ISqlCommandAppService sqlCommandAppService)
        {
            Guard.ArgumentNotNull(emailAlertAppService, nameof(emailAlertAppService));
            Guard.ArgumentNotNull(sqlCommandAppService, nameof(sqlCommandAppService));

            _emailAlertAppService = emailAlertAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        #region Email Alert

        public EmailAlertDTO AddEmailAlert(EmailAlertDTO emailAlertDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.AddNewEmailAlert(emailAlertDTO, serviceHeader);
        }

        public bool AddEmailAlerts(List<EmailAlertDTO> emailAlertDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.AddNewEmailAlerts(emailAlertDTOs, serviceHeader);
        }

        public bool AddEmailAlertsWithHistory(GroupEmailAlertDTO groupEmailAlertDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.AddEmailAlertsWithHistory(groupEmailAlertDTO, serviceHeader);
        }

        public bool AddQuickEmailAlert(QuickEmailAlertDTO quickEmailAlertDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.AddQuickEmailAlert(quickEmailAlertDTO, serviceHeader);
        }

        public bool UpdateEmailAlert(EmailAlertDTO emailAlertDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.UpdateEmailAlert(emailAlertDTO, serviceHeader);
        }

        public List<EmailAlertDTO> FindEmailAlerts()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.FindEmailAlerts(serviceHeader);
        }

        public List<EmailAlertDTO> FindEmailAlertsByDLRStatus(int dlrStatus)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.FindEmailAlertsByDLRStatus(dlrStatus, serviceHeader);
        }

        public PageCollectionInfo<EmailAlertDTO> FindEmailAlertsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.FindEmailAlerts(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<EmailAlertDTO> FindEmailAlertsByFilterInPage(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.FindEmailAlerts(dlrStatus, text, pageIndex, pageSize, daysCap, serviceHeader);
        }

        public PageCollectionInfo<EmailAlertDTO> FindEmailAlertsByDateRangeAndFilterInPage(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.FindEmailAlerts(dlrStatus, startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public List<EmailAlertDTO> FindEmailAlertsByDLRStatusAndOrigin(int dlrStatus, int origin)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.FindEmailAlertsByDLRStatusAndOrigin(dlrStatus, origin, serviceHeader);
        }

        public EmailAlertDTO FindEmailAlert(Guid emailAlertId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _emailAlertAppService.FindEmailAlert(emailAlertId, serviceHeader);
        }

        public List<MonthlySummaryValuesDTO> FindEmailAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _sqlCommandAppService.FindEmailAlertsMonthlyStatistics(companyId, startDate, endDate, serviceHeader);
        }

        #endregion
    }
}
