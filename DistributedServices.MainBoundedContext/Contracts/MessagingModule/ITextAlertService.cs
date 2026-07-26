using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ITextAlertService
    {
        #region Text Alert

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddBulkMessage(BulkMessageDTO bulkMessageDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddUSSDMessage(USSDMessageDTO ussdMessageDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddTextAlerts(List<TextAlertDTO> textAlertDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateTextAlert(TextAlertDTO textAlertDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TextAlertDTO> FindTextAlerts();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TextAlertDTO> FindTextAlertsByMessageReference(string messageReference);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TextAlertDTO> FindTextAlertsByDLRStatus(int dlrStatus);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TextAlertDTO> FindTextAlertsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TextAlertDTO> FindTextAlertsByFilterInPage(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<TextAlertDTO> FindTextAlertsByDateRangeAndFilterInPage(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        TextAlertDTO FindTextAlert(Guid textAlertId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TextAlertDTO> FindTextAlertsByDLRStatusAndOrigin(int dlrStatus, int origin);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ProcessInboundMessage(InboundMessageDTO inboundMessageDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddTextAlertsWithHistory(GroupTextAlertDTO groupTextAlertDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddQuickTextAlert(QuickTextAlertDTO quickTextAlertDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CommissionDTO> FindCommissionsBySystemTransactionCode(int systemTransactionCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCommissionsBySystemTransactionCode(int systemTransactionCode, CommissionDTO[] commissionDTOs, int chargeBenefactor);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<MonthlySummaryValuesDTO> FindTextAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate);

        #endregion
    }
}
