using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IAlternateChannelLogService")]
    public interface IAlternateChannelLogService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAlternateChannelLog(AlternateChannelLogDTO alternateChannelLogDTO, AsyncCallback callback, Object state);
        AlternateChannelLogDTO EndAddAlternateChannelLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddISO8583AlternateChannelLog(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, AsyncCallback callback, Object state);
        ISO8583AlternateChannelLogDTO EndAddISO8583AlternateChannelLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSPARROWAlternateChannelLog(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, AsyncCallback callback, Object state);
        SPARROWAlternateChannelLogDTO EndAddSPARROWAlternateChannelLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddWALLETAlternateChannelLog(WALLETAlternateChannelLogDTO alternateChannelLogDTO, AsyncCallback callback, Object state);
        WALLETAlternateChannelLogDTO EndAddWALLETAlternateChannelLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAlternateChannelLogResponse(Guid alternateChannelLogId, string payload, AsyncCallback callback, Object state);
        bool EndUpdateAlternateChannelLogResponse(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelLog(Guid alternateChannelLogId, AsyncCallback callback, Object state);
        AlternateChannelLogDTO EndFindAlternateChannelLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelLogs(AsyncCallback callback, Object state);
        List<AlternateChannelLogDTO> EndFindAlternateChannelLogs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAlternateChannelLogsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<AlternateChannelLogDTO> EndFindAlternateChannelLogsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchISO8583AlternateChannelLogs(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, AsyncCallback callback, Object state);
        List<ISO8583AlternateChannelLogDTO> EndMatchISO8583AlternateChannelLogs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchISO8583AlternateChannelLogsByRetrievalReferenceNumber(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap, AsyncCallback callback, Object state);
        List<ISO8583AlternateChannelLogDTO> EndMatchISO8583AlternateChannelLogsByRetrievalReferenceNumber(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchSPARROWAlternateChannelLogs(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageType, int daysCap, AsyncCallback callback, Object state);
        List<SPARROWAlternateChannelLogDTO> EndMatchSPARROWAlternateChannelLogs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMatchWALLETAlternateChannelLogs(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, AsyncCallback callback, Object state);
        List<WALLETAlternateChannelLogDTO> EndMatchWALLETAlternateChannelLogs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateWALLETAlternateChannelLogCallbackPayload(WALLETAlternateChannelLogDTO alternateChannelLogDTO, int daysCap, AsyncCallback callback, Object state);
        bool EndUpdateWALLETAlternateChannelLogCallbackPayload(IAsyncResult result);
    }
}
