using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.MessagingModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmailAlertService
    {
        #region Email Alert

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmailAlertDTO AddEmailAlert(EmailAlertDTO emailAlertDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddEmailAlerts(List<EmailAlertDTO> emailAlertDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmailAlert(EmailAlertDTO emailAlertDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmailAlertDTO> FindEmailAlerts();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmailAlertDTO> FindEmailAlertsByDLRStatus(int dlrStatus);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmailAlertDTO> FindEmailAlertsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmailAlertDTO> FindEmailAlertsByFilterInPage(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmailAlertDTO> FindEmailAlertsByDateRangeAndFilterInPage(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmailAlertDTO> FindEmailAlertsByDLRStatusAndOrigin(int dlrStatus, int origin);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmailAlertDTO FindEmailAlert(Guid emailAlertId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddEmailAlertsWithHistory(GroupEmailAlertDTO groupEmailAlertDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddQuickEmailAlert(QuickEmailAlertDTO quickEmailAlertDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MonthlySummaryValuesDTO> FindEmailAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate);


        #endregion
    }
}
