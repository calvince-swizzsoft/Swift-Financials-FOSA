using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class AlternateChannelLogAppService : IAlternateChannelLogAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<AlternateChannelLog> _alternateChannelLogRepository;
        private readonly IRepository<Journal> _journalRepository;

        public AlternateChannelLogAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<AlternateChannelLog> alternateChannelLogRepository,
           IRepository<Journal> journalRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (alternateChannelLogRepository == null)
                throw new ArgumentNullException(nameof(alternateChannelLogRepository));

            if (journalRepository == null)
                throw new ArgumentNullException(nameof(journalRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _alternateChannelLogRepository = alternateChannelLogRepository;
            _journalRepository = journalRepository;
        }

        public AlternateChannelLogDTO AddNewAlternateChannelLog(AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader)
        {
            var alternateChannelLogBindingModel = alternateChannelLogDTO.ProjectedAs<AlternateChannelLogBindingModel>();

            alternateChannelLogBindingModel.ValidateAll();

            if (alternateChannelLogBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, alternateChannelLogBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var ISO8583 = new ISO8583(alternateChannelLogDTO.ISO8583MessageTypeIdentification, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583SystemTraceAuditNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, alternateChannelLogDTO.ISO8583Message, alternateChannelLogDTO.ISO8583Amount);

                var SPARROW = new SPARROW(alternateChannelLogDTO.SPARROWMessageType, alternateChannelLogDTO.SPARROWSRCIMD, alternateChannelLogDTO.SPARROWDeviceId, alternateChannelLogDTO.SPARROWDate, alternateChannelLogDTO.SPARROWTime, alternateChannelLogDTO.SPARROWCardNumber, alternateChannelLogDTO.SPARROWMessage, alternateChannelLogDTO.SPARROWAmount);

                var WALLET = new WALLET(alternateChannelLogDTO.WALLETMessageTypeIdentification, alternateChannelLogDTO.WALLETPrimaryAccountNumber, alternateChannelLogDTO.WALLETSystemTraceAuditNumber, alternateChannelLogDTO.WALLETRetrievalReferenceNumber, alternateChannelLogDTO.WALLETMessage, alternateChannelLogDTO.WALLETCallbackPayload, alternateChannelLogDTO.WALLETAmount, alternateChannelLogDTO.WALLETRequestIdentifier);

                var alternateChannelLog = AlternateChannelLogFactory.CreateAlternateChannelLog(alternateChannelLogDTO.AlternateChannelType, ISO8583, SPARROW, WALLET, alternateChannelLogDTO.SystemTraceAuditNumber);

                _alternateChannelLogRepository.Add(alternateChannelLog, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? alternateChannelLog.ProjectedAs<AlternateChannelLogDTO>() : null;
            }
        }

        public async Task<AlternateChannelLogDTO> AddNewAlternateChannelLogAsync(AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader)
        {
            var alternateChannelLogBindingModel = alternateChannelLogDTO.ProjectedAs<AlternateChannelLogBindingModel>();

            alternateChannelLogBindingModel.ValidateAll();

            if (alternateChannelLogBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, alternateChannelLogBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var ISO8583 = new ISO8583(alternateChannelLogDTO.ISO8583MessageTypeIdentification, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583SystemTraceAuditNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, alternateChannelLogDTO.ISO8583Message, alternateChannelLogDTO.ISO8583Amount);

                var SPARROW = new SPARROW(alternateChannelLogDTO.SPARROWMessageType, alternateChannelLogDTO.SPARROWSRCIMD, alternateChannelLogDTO.SPARROWDeviceId, alternateChannelLogDTO.SPARROWDate, alternateChannelLogDTO.SPARROWTime, alternateChannelLogDTO.SPARROWCardNumber, alternateChannelLogDTO.SPARROWMessage, alternateChannelLogDTO.SPARROWAmount);

                var WALLET = new WALLET(alternateChannelLogDTO.WALLETMessageTypeIdentification, alternateChannelLogDTO.WALLETPrimaryAccountNumber, alternateChannelLogDTO.WALLETSystemTraceAuditNumber, alternateChannelLogDTO.WALLETRetrievalReferenceNumber, alternateChannelLogDTO.WALLETMessage, alternateChannelLogDTO.WALLETCallbackPayload, alternateChannelLogDTO.WALLETAmount, alternateChannelLogDTO.WALLETRequestIdentifier);

                var alternateChannelLog = AlternateChannelLogFactory.CreateAlternateChannelLog(alternateChannelLogDTO.AlternateChannelType, ISO8583, SPARROW, WALLET, alternateChannelLogDTO.SystemTraceAuditNumber);

                _alternateChannelLogRepository.Add(alternateChannelLog, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0 ? alternateChannelLog.ProjectedAs<AlternateChannelLogDTO>() : null;
            }
        }

        public async Task<ISO8583AlternateChannelLogDTO> AddNewISO8583AlternateChannelLogAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader)
        {
            var iso8583AlternateChannelLogBindingModel = alternateChannelLogDTO.ProjectedAs<ISO8583AlternateChannelLogBindingModel>();

            iso8583AlternateChannelLogBindingModel.ValidateAll();

            if (iso8583AlternateChannelLogBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, iso8583AlternateChannelLogBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var ISO8583 = new ISO8583(alternateChannelLogDTO.ISO8583MessageTypeIdentification, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583SystemTraceAuditNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, alternateChannelLogDTO.ISO8583Message, alternateChannelLogDTO.ISO8583Amount);

                var alternateChannelLog = AlternateChannelLogFactory.CreateAlternateChannelLog(alternateChannelLogDTO.AlternateChannelType, ISO8583, alternateChannelLogDTO.SystemTraceAuditNumber);

                _alternateChannelLogRepository.Add(alternateChannelLog, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0 ? alternateChannelLog.ProjectedAs<ISO8583AlternateChannelLogDTO>() : null;
            }
        }

        public async Task<SPARROWAlternateChannelLogDTO> AddNewSPARROWAlternateChannelLogAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader)
        {
            var sparrowAlternateChannelLogBindingModel = alternateChannelLogDTO.ProjectedAs<SPARROWAlternateChannelLogBindingModel>();

            sparrowAlternateChannelLogBindingModel.ValidateAll();

            if (sparrowAlternateChannelLogBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, sparrowAlternateChannelLogBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var SPARROW = new SPARROW(alternateChannelLogDTO.SPARROWMessageType, alternateChannelLogDTO.SPARROWSRCIMD, alternateChannelLogDTO.SPARROWDeviceId, alternateChannelLogDTO.SPARROWDate, alternateChannelLogDTO.SPARROWTime, alternateChannelLogDTO.SPARROWCardNumber, alternateChannelLogDTO.SPARROWMessage, alternateChannelLogDTO.SPARROWAmount);

                var alternateChannelLog = AlternateChannelLogFactory.CreateAlternateChannelLog(alternateChannelLogDTO.AlternateChannelType, SPARROW, alternateChannelLogDTO.SystemTraceAuditNumber);

                _alternateChannelLogRepository.Add(alternateChannelLog, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0 ? alternateChannelLog.ProjectedAs<SPARROWAlternateChannelLogDTO>() : null;
            }
        }

        public async Task<WALLETAlternateChannelLogDTO> AddNewWALLETAlternateChannelLogAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, ServiceHeader serviceHeader)
        {
            var walletAlternateChannelLogBindingModel = alternateChannelLogDTO.ProjectedAs<WALLETAlternateChannelLogBindingModel>();

            walletAlternateChannelLogBindingModel.ValidateAll();

            if (walletAlternateChannelLogBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, walletAlternateChannelLogBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var WALLET = new WALLET(alternateChannelLogDTO.WALLETMessageTypeIdentification, alternateChannelLogDTO.WALLETPrimaryAccountNumber, alternateChannelLogDTO.WALLETSystemTraceAuditNumber, alternateChannelLogDTO.WALLETRetrievalReferenceNumber, alternateChannelLogDTO.WALLETMessage, alternateChannelLogDTO.WALLETCallbackPayload, alternateChannelLogDTO.WALLETAmount, alternateChannelLogDTO.WALLETRequestIdentifier);

                var alternateChannelLog = AlternateChannelLogFactory.CreateAlternateChannelLog(alternateChannelLogDTO.AlternateChannelType, WALLET, alternateChannelLogDTO.SystemTraceAuditNumber);

                _alternateChannelLogRepository.Add(alternateChannelLog, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0 ? alternateChannelLog.ProjectedAs<WALLETAlternateChannelLogDTO>() : null;
            }
        }

        public bool UpdateAlternateChannelLogResponse(Guid alternateChannelLogId, string response, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelLogRepository.Get(alternateChannelLogId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Response = response;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public async Task<bool> UpdateAlternateChannelLogResponseAsync(Guid alternateChannelLogId, string payload, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _alternateChannelLogRepository.Get(alternateChannelLogId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Response = payload;
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
            }
        }

        public async Task<bool> UpdateWALLETAlternateChannelLogCallbackPayloadAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader)
        {
            var alternateChannelLogDTOs = await MatchWALLETAlternateChannelLogsAsync(alternateChannelLogDTO, false, daysCap, serviceHeader);

            if (alternateChannelLogDTOs != null && alternateChannelLogDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var dto in alternateChannelLogDTOs)
                    {
                        var persistedAlternateChannelLog = _alternateChannelLogRepository.Get(dto.Id, serviceHeader);

                        if (persistedAlternateChannelLog != null && string.IsNullOrWhiteSpace(persistedAlternateChannelLog.WALLET.CallbackPayload))
                        {
                            persistedAlternateChannelLog.WALLET = new WALLET(persistedAlternateChannelLog.WALLET.MessageTypeIdentification, persistedAlternateChannelLog.WALLET.PrimaryAccountNumber, persistedAlternateChannelLog.WALLET.SystemTraceAuditNumber, persistedAlternateChannelLog.WALLET.RetrievalReferenceNumber, persistedAlternateChannelLog.WALLET.Message, alternateChannelLogDTO.WALLETCallbackPayload, persistedAlternateChannelLog.WALLET.Amount, persistedAlternateChannelLog.WALLET.RequestIdentifier);

                            var filter = JournalSpecifications.JournalWithAlternateChannelLogId(persistedAlternateChannelLog.Id);

                            ISpecification<Journal> spec = filter;

                            var journals = await _journalRepository.AllMatchingAsync(spec, serviceHeader);

                            if (journals != null && journals.Any())
                            {
                                foreach (var item in journals)
                                {
                                    var persistedJournal = _journalRepository.Get(item.Id, serviceHeader);

                                    if (persistedJournal != null)
                                    {
                                        persistedJournal.Reference = string.Format("{0}~{1}", persistedJournal.Reference, alternateChannelLogDTO.WALLETCallbackPayload);
                                    }
                                }
                            }
                        }
                    }

                    return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public AlternateChannelLogDTO FindAlternateChannelLog(Guid alternateChannelLogId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _alternateChannelLogRepository.Get<AlternateChannelLogDTO>(alternateChannelLogId, serviceHeader);
            }
        }

        public async Task<AlternateChannelLogDTO> FindAlternateChannelLogAsync(Guid alternateChannelLogId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _alternateChannelLogRepository.GetAsync<AlternateChannelLogDTO>(alternateChannelLogId, serviceHeader);
            }
        }

        public List<AlternateChannelLogDTO> FindAlternateChannelLogs(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _alternateChannelLogRepository.GetAll<AlternateChannelLogDTO>(serviceHeader);
            }
        }

        public PageCollectionInfo<AlternateChannelLogDTO> FindAlternateChannelLogs(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelLogSpecifications.DefaultSpec();

                ISpecification<AlternateChannelLog> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _alternateChannelLogRepository.AllMatchingPaged<AlternateChannelLogDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public List<ISO8583AlternateChannelLogDTO> MatchISO8583AlternateChannelLogs(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = includeMessageTypeIdentification ?
                            AlternateChannelLogSpecifications.ISO8583(alternateChannelLogDTO.AlternateChannelType, alternateChannelLogDTO.ISO8583MessageTypeIdentification, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583SystemTraceAuditNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, daysCap) :
                            AlternateChannelLogSpecifications.ISO8583(alternateChannelLogDTO.AlternateChannelType, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583SystemTraceAuditNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return _alternateChannelLogRepository.AllMatching<ISO8583AlternateChannelLogDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = includeMessageTypeIdentification ?
                            AlternateChannelLogSpecifications.ISO8583(alternateChannelLogDTO.AlternateChannelType, alternateChannelLogDTO.ISO8583MessageTypeIdentification, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583SystemTraceAuditNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, daysCap) :
                            AlternateChannelLogSpecifications.ISO8583(alternateChannelLogDTO.AlternateChannelType, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583SystemTraceAuditNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return await _alternateChannelLogRepository.AllMatchingAsync<ISO8583AlternateChannelLogDTO>(spec, serviceHeader);
            }
        }

        public List<ISO8583AlternateChannelLogDTO> MatchISO8583AlternateChannelLogs(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelLogSpecifications.ISO8583(alternateChannelLogDTO.AlternateChannelType, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return _alternateChannelLogRepository.AllMatching<ISO8583AlternateChannelLogDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<ISO8583AlternateChannelLogDTO>> MatchISO8583AlternateChannelLogsAsync(ISO8583AlternateChannelLogDTO alternateChannelLogDTO, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AlternateChannelLogSpecifications.ISO8583(alternateChannelLogDTO.AlternateChannelType, alternateChannelLogDTO.ISO8583PrimaryAccountNumber, alternateChannelLogDTO.ISO8583RetrievalReferenceNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return await _alternateChannelLogRepository.AllMatchingAsync<ISO8583AlternateChannelLogDTO>(spec, serviceHeader);
            }
        }

        public List<SPARROWAlternateChannelLogDTO> MatchSPARROWAlternateChannelLogs(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageType, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = includeMessageType ?
                    AlternateChannelLogSpecifications.SPARROW(alternateChannelLogDTO.SPARROWMessageType, alternateChannelLogDTO.SPARROWSRCIMD, alternateChannelLogDTO.SPARROWDeviceId, alternateChannelLogDTO.SPARROWDate, alternateChannelLogDTO.SPARROWTime, alternateChannelLogDTO.SPARROWCardNumber, daysCap) :
                    AlternateChannelLogSpecifications.SPARROW(alternateChannelLogDTO.SPARROWSRCIMD, alternateChannelLogDTO.SPARROWDeviceId, alternateChannelLogDTO.SPARROWDate, alternateChannelLogDTO.SPARROWTime, alternateChannelLogDTO.SPARROWCardNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return _alternateChannelLogRepository.AllMatching<SPARROWAlternateChannelLogDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<SPARROWAlternateChannelLogDTO>> MatchSPARROWAlternateChannelLogsAsync(SPARROWAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageType, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = includeMessageType ?
                   AlternateChannelLogSpecifications.SPARROW(alternateChannelLogDTO.SPARROWMessageType, alternateChannelLogDTO.SPARROWSRCIMD, alternateChannelLogDTO.SPARROWDeviceId, alternateChannelLogDTO.SPARROWDate, alternateChannelLogDTO.SPARROWTime, alternateChannelLogDTO.SPARROWCardNumber, daysCap) :
                   AlternateChannelLogSpecifications.SPARROW(alternateChannelLogDTO.SPARROWSRCIMD, alternateChannelLogDTO.SPARROWDeviceId, alternateChannelLogDTO.SPARROWDate, alternateChannelLogDTO.SPARROWTime, alternateChannelLogDTO.SPARROWCardNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return await _alternateChannelLogRepository.AllMatchingAsync<SPARROWAlternateChannelLogDTO>(spec, serviceHeader);
            }
        }

        public List<WALLETAlternateChannelLogDTO> MatchWALLETAlternateChannelLogs(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = includeMessageTypeIdentification ?
                            AlternateChannelLogSpecifications.WALLET(alternateChannelLogDTO.WALLETMessageTypeIdentification, alternateChannelLogDTO.WALLETPrimaryAccountNumber, alternateChannelLogDTO.WALLETSystemTraceAuditNumber, alternateChannelLogDTO.WALLETRetrievalReferenceNumber, daysCap) :
                            AlternateChannelLogSpecifications.WALLET(alternateChannelLogDTO.WALLETPrimaryAccountNumber, alternateChannelLogDTO.WALLETSystemTraceAuditNumber, alternateChannelLogDTO.WALLETRetrievalReferenceNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return _alternateChannelLogRepository.AllMatching<WALLETAlternateChannelLogDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<WALLETAlternateChannelLogDTO>> MatchWALLETAlternateChannelLogsAsync(WALLETAlternateChannelLogDTO alternateChannelLogDTO, bool includeMessageTypeIdentification, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = includeMessageTypeIdentification ?
                               AlternateChannelLogSpecifications.WALLET(alternateChannelLogDTO.WALLETMessageTypeIdentification, alternateChannelLogDTO.WALLETPrimaryAccountNumber, alternateChannelLogDTO.WALLETSystemTraceAuditNumber, alternateChannelLogDTO.WALLETRetrievalReferenceNumber, daysCap) :
                               AlternateChannelLogSpecifications.WALLET(alternateChannelLogDTO.WALLETPrimaryAccountNumber, alternateChannelLogDTO.WALLETSystemTraceAuditNumber, alternateChannelLogDTO.WALLETRetrievalReferenceNumber, daysCap);

                ISpecification<AlternateChannelLog> spec = filter;

                return await _alternateChannelLogRepository.AllMatchingAsync<WALLETAlternateChannelLogDTO>(spec, serviceHeader);
            }
        }
    }
}
