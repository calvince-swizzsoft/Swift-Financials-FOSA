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
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AlternateChannelLogService : IAlternateChannelLogService
    {
        private readonly IAlternateChannelLogAppService _alternateChannelLogAppService;

        public AlternateChannelLogService(IAlternateChannelLogAppService alternateChannelLogAppService)
        {
            Guard.ArgumentNotNull(alternateChannelLogAppService, nameof(alternateChannelLogAppService));

            _alternateChannelLogAppService = alternateChannelLogAppService;
        }

        #region Alternate Channel Log

        public async Task<AlternateChannelLogDTO> AddAlternateChannelLogAsync(AlternateChannelLogDTO alternateChannelLogDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.AddNewAlternateChannelLogAsync(alternateChannelLogDTO, serviceHeader);
        }

        public async Task<ISO8583AlternateChannelLogDTO> AddISO8583AlternateChannelLogAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.AddNewISO8583AlternateChannelLogAsync(alternateChannelLogDTO, serviceHeader);
        }

        public async Task<SPARROWAlternateChannelLogDTO> AddSPARROWAlternateChannelLogAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.AddNewSPARROWAlternateChannelLogAsync(alternateChannelLogDTO, serviceHeader);
        }

        public async Task<WALLETAlternateChannelLogDTO> AddWALLETAlternateChannelLogAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.AddNewWALLETAlternateChannelLogAsync(alternateChannelLogDTO, serviceHeader);
        }

        public async Task<bool> UpdateAlternateChannelLogResponseAsync(Guid alternateChannelLogId, string payload)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.UpdateAlternateChannelLogResponseAsync(alternateChannelLogId, payload, serviceHeader);
        }

        public async Task<AlternateChannelLogDTO> FindAlternateChannelLogAsync(Guid alternateChannelLogId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.FindAlternateChannelLogAsync(alternateChannelLogId, serviceHeader);
        }

        public List<AlternateChannelLogDTO> FindAlternateChannelLogs()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelLogAppService.FindAlternateChannelLogs(serviceHeader);
        }

        public PageCollectionInfo<AlternateChannelLogDTO> FindAlternateChannelLogsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _alternateChannelLogAppService.FindAlternateChannelLogs(pageIndex, pageSize, serviceHeader);
        }

        public async Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsByRetrievalReferenceNumberAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.MatchISO8583AlternateChannelLogsAsync(alternateChannelLogDTO, daysCap, serviceHeader);
        }

        public async Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.MatchISO8583AlternateChannelLogsAsync(alternateChannelLogDTO, includeMessageTypeIdentification, daysCap, serviceHeader);
        }

        public async Task<List<SPARROWAlternateChannelLogDTO>> MatchSPARROWAlternateChannelLogsAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageType, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.MatchSPARROWAlternateChannelLogsAsync(alternateChannelLogDTO, includeMessageType, daysCap, serviceHeader);
        }

        public async Task<List<WALLETAlternateChannelLogDTO>> MatchWALLETAlternateChannelLogsAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.MatchWALLETAlternateChannelLogsAsync(alternateChannelLogDTO, includeMessageTypeIdentification, daysCap, serviceHeader);
        }

        public async Task<bool> UpdateWALLETAlternateChannelLogCallbackPayloadAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _alternateChannelLogAppService.UpdateWALLETAlternateChannelLogCallbackPayloadAsync(alternateChannelLogDTO, daysCap, serviceHeader);
        }

        #endregion
    }
}
