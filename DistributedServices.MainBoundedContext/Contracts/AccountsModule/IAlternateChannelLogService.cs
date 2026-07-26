using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IAlternateChannelLogService
    {
        #region Alternate Channel Log

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<AlternateChannelLogDTO> AddAlternateChannelLogAsync(AlternateChannelLogDTO alternateChannelLogDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<ISO8583AlternateChannelLogDTO> AddISO8583AlternateChannelLogAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<SPARROWAlternateChannelLogDTO> AddSPARROWAlternateChannelLogAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<WALLETAlternateChannelLogDTO> AddWALLETAlternateChannelLogAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateAlternateChannelLogResponseAsync(Guid alternateChannelLogId, string payload);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<AlternateChannelLogDTO> FindAlternateChannelLogAsync(Guid alternateChannelLogId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AlternateChannelLogDTO> FindAlternateChannelLogs();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AlternateChannelLogDTO> FindAlternateChannelLogsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsByRetrievalReferenceNumberAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<SPARROWAlternateChannelLogDTO>> MatchSPARROWAlternateChannelLogsAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageType, int daysCap);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<WALLETAlternateChannelLogDTO>> MatchWALLETAlternateChannelLogsAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateWALLETAlternateChannelLogCallbackPayloadAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, int daysCap);

        #endregion
    }
}
