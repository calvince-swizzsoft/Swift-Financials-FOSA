using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MessagingModule
{
    [ServiceContract(Name = "IEmailAlertService")]
    public interface IEmailAlertService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmailAlert(EmailAlertDTO emailAlertDTO, AsyncCallback callback, Object state);
        EmailAlertDTO EndAddEmailAlert(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmailAlerts(List<EmailAlertDTO> emailAlertDTOs, AsyncCallback callback, Object state);
        bool EndAddEmailAlerts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmailAlert(EmailAlertDTO emailAlertDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmailAlert(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlerts(AsyncCallback callback, Object state);
        List<EmailAlertDTO> EndFindEmailAlerts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlertsByDLRStatus(int dlrStatus, AsyncCallback callback, Object state);
        List<EmailAlertDTO> EndFindEmailAlertsByDLRStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlertsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmailAlertDTO> EndFindEmailAlertsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlertsByFilterInPage(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, AsyncCallback callback, Object state);
        PageCollectionInfo<EmailAlertDTO> EndFindEmailAlertsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlertsByDateRangeAndFilterInPage(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmailAlertDTO> EndFindEmailAlertsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlertsByDLRStatusAndOrigin(int dlrStatus, int origin, AsyncCallback callback, Object state);
        List<EmailAlertDTO> EndFindEmailAlertsByDLRStatusAndOrigin(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlert(Guid emailAlertId, AsyncCallback callback, Object state);
        EmailAlertDTO EndFindEmailAlert(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmailAlertsWithHistory(GroupEmailAlertDTO groupEmailAlertDTO, AsyncCallback callback, Object state);
        bool EndAddEmailAlertsWithHistory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddQuickEmailAlert(QuickEmailAlertDTO quickEmailAlertDTO, AsyncCallback callback, Object state);
        bool EndAddQuickEmailAlert(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmailAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate, AsyncCallback callback, Object state);
        List<MonthlySummaryValuesDTO> EndFindEmailAlertsMonthlyStatistics(IAsyncResult result);

    }
}
