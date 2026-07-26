using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.MessagingModule
{
    [ServiceContract(Name = "ITextAlertService")]
    public interface ITextAlertService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddBulkMessage(BulkMessageDTO bulkMessageDTO, AsyncCallback callback, Object state);
        bool EndAddBulkMessage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddUSSDMessage(USSDMessageDTO ussdMessageDTO, AsyncCallback callback, Object state);
        bool EndAddUSSDMessage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddTextAlerts(List<TextAlertDTO> textAlertDTOs, AsyncCallback callback, Object state);
        bool EndAddTextAlerts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateTextAlert(TextAlertDTO textAlertDTO, AsyncCallback callback, Object state);
        bool EndUpdateTextAlert(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlerts(AsyncCallback callback, Object state);
        List<TextAlertDTO> EndFindTextAlerts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlertsByMessageReference(string messageReference, AsyncCallback callback, Object state);
        List<TextAlertDTO> EndFindTextAlertsByMessageReference(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlertsByDLRStatus(int dlrStatus, AsyncCallback callback, Object state);
        List<TextAlertDTO> EndFindTextAlertsByDLRStatus(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlertsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<TextAlertDTO> EndFindTextAlertsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlertsByFilterInPage(int dlrStatus, string text, int pageIndex, int pageSize, int daysCap, AsyncCallback callback, Object state);
        PageCollectionInfo<TextAlertDTO> EndFindTextAlertsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlertsByDateRangeAndFilterInPage(int dlrStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<TextAlertDTO> EndFindTextAlertsByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlert(Guid textAlertId, AsyncCallback callback, Object state);
        TextAlertDTO EndFindTextAlert(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlertsByDLRStatusAndOrigin(int dlrStatus, int origin, AsyncCallback callback, Object state);
        List<TextAlertDTO> EndFindTextAlertsByDLRStatusAndOrigin(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginProcessInboundMessage(InboundMessageDTO inboundMessageDTO, AsyncCallback callback, Object state);
        bool EndProcessInboundMessage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddTextAlertsWithHistory(GroupTextAlertDTO groupTextAlertDTO, AsyncCallback callback, Object state);
        bool EndAddTextAlertsWithHistory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddQuickTextAlert(QuickTextAlertDTO quickTextAlertDTO, AsyncCallback callback, Object state);
        bool EndAddQuickTextAlert(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsBySystemTransactionCode(int systemTransactionCode, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsBySystemTransactionCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsBySystemTransactionCode(int systemTransactionCode, CommissionDTO[] commissionDTOs, int chargeBenefactor, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsBySystemTransactionCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTextAlertsMonthlyStatistics(Guid companyId, DateTime startDate, DateTime endDate, AsyncCallback callback, object state);
        List<MonthlySummaryValuesDTO> EndFindTextAlertsMonthlyStatistics(IAsyncResult result);
    }
}
