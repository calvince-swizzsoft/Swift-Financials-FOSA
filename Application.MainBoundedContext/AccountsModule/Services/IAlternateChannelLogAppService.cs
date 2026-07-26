using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IAlternateChannelLogAppService
    {
        AlternateChannelLogDTO AddNewAlternateChannelLog(AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader);

        Task<AlternateChannelLogDTO> AddNewAlternateChannelLogAsync(AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader);

        Task<ISO8583AlternateChannelLogDTO> AddNewISO8583AlternateChannelLogAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader);

        Task<SPARROWAlternateChannelLogDTO> AddNewSPARROWAlternateChannelLogAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader);

        Task<WALLETAlternateChannelLogDTO> AddNewWALLETAlternateChannelLogAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader);

        bool UpdateAlternateChannelLogResponse(Guid alternateChannelLogId, string response, ServiceHeader serviceHeader);

        Task<bool> UpdateAlternateChannelLogResponseAsync(Guid alternateChannelLogId, string payload, ServiceHeader serviceHeader);

        Task<bool> UpdateWALLETAlternateChannelLogCallbackPayloadAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader);

        AlternateChannelLogDTO FindAlternateChannelLog(Guid alternateChannelLogId, ServiceHeader serviceHeader);

        Task<AlternateChannelLogDTO> FindAlternateChannelLogAsync(Guid alternateChannelLogId, ServiceHeader serviceHeader);

        List<AlternateChannelLogDTO> FindAlternateChannelLogs(ServiceHeader serviceHeader);

        PageCollectionInfo<AlternateChannelLogDTO> FindAlternateChannelLogs(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<ISO8583AlternateChannelLogDTO> MatchISO8583AlternateChannelLogs(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader);

        Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader);

        List<ISO8583AlternateChannelLogDTO> MatchISO8583AlternateChannelLogs(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader);

        Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader);

        List<SPARROWAlternateChannelLogDTO> MatchSPARROWAlternateChannelLogs(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageType, int daysCap, ServiceHeader serviceHeader);

        Task<List<SPARROWAlternateChannelLogDTO>> MatchSPARROWAlternateChannelLogsAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageType, int daysCap, ServiceHeader serviceHeader);

        List<WALLETAlternateChannelLogDTO> MatchWALLETAlternateChannelLogs(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader);

        Task<List<WALLETAlternateChannelLogDTO>> MatchWALLETAlternateChannelLogsAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader);
    }
}
