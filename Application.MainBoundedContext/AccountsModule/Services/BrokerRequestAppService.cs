using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BrokerRequestAgg;
using Numero3.EntityFramework.Interfaces;
using Application.MainBoundedContext.Services;
using Domain.Seedwork;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using System.Threading.Tasks;
using System.Linq;
using Domain.Seedwork.Specification;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class BrokerRequestAppService : IBrokerRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<BrokerRequest> _brokerRequestRepository;
        private readonly IBrokerService _brokerService;

        public BrokerRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<BrokerRequest> brokerRequestRepository,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (brokerRequestRepository == null)
                throw new ArgumentNullException(nameof(brokerRequestRepository));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _brokerRequestRepository = brokerRequestRepository;
            _brokerService = brokerService;
        }
        public async Task<BrokerRequestDTO> AddNewBrokerRequestAsync(BrokerRequestDTO brokerRequestDTO, ServiceHeader serviceHeader)
        {
            var brokerRequestBindingModel = brokerRequestDTO.ProjectedAs<BrokerRequestBindingModel>();

            brokerRequestBindingModel.ValidateAll();

            if (brokerRequestDTO != null && !brokerRequestBindingModel.HasErrors)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var brokerRequest = BrokerRequestFactory.CreateBrokerRequest(brokerRequestDTO.TransactionType, brokerRequestDTO.TransactionCode, brokerRequestDTO.UniqueTransactionIdentifier, brokerRequestDTO.CallbackPayload, brokerRequestDTO.IncomingCipherTextPayload, brokerRequestDTO.IncomingPlainTextPayload, brokerRequestDTO.SystemTraceAuditNumber);
                    brokerRequest.Status = (byte)BrokerRequestStatus.Pending;
                    brokerRequest.IPNEnabled = brokerRequestDTO.IPNEnabled;
                    brokerRequest.CreatedBy = serviceHeader.ApplicationUserName;

                    _brokerRequestRepository.Add(brokerRequest, serviceHeader);

                    await dbContextScope.SaveChangesAsync(serviceHeader);

                    return brokerRequest.ProjectedAs<BrokerRequestDTO>();
                }
            }
            else return null;
        }

        public async Task<BrokerRequestDTO> FindBrokerRequestAsync(Guid brokerRequestId, ServiceHeader serviceHeader)
        {
            if (brokerRequestId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    return await _brokerRequestRepository.GetAsync<BrokerRequestDTO>(brokerRequestId, serviceHeader);
                }
            }
            else return null;
        }

        public async Task<List<BrokerRequestDTO>> FindBrokerRequestsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _brokerRequestRepository.GetAllAsync<BrokerRequestDTO>(serviceHeader);
            }
        }

        public async Task<bool> UpdateBrokerRequestResponse(Guid brokerRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _brokerRequestRepository.GetAsync(brokerRequestId, serviceHeader);

                if (persisted != null)
                {
                    persisted.OutgoingPlainTextPayload = outgoingPlainTextPayload;
                    persisted.OutgoingCipherTextPayload = outgoingCipherTextPayload;
                    persisted.Status = (byte)BrokerRequestStatus.Processed;
                }

                result = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;

                if (result && persisted.IPNEnabled)
                {
                    _brokerService.ProcessBrokerRequests(DMLCommand.Update, serviceHeader, persisted.ProjectedAs<BrokerRequestDTO>());
                }
            }

            return result;
        }

        public async Task<bool> UpdateBrokerRequestIPNStatusAsync(Guid brokerRequestId, int ipnStatus, string ipnResponse, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _brokerRequestRepository.Get(brokerRequestId, serviceHeader);

                if (persisted != null)
                {
                    persisted.IPNStatus = (byte)ipnStatus;
                    persisted.IPNResponse = ipnResponse;
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> ResetBrokerRequestsIPNStatusAsync(Guid[] brokerRequestIds, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var brokerRequestDTOs = await FindBrokerRequestsAsync(brokerRequestIds, serviceHeader);

            if (brokerRequestDTOs != null && brokerRequestDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var item in brokerRequestDTOs)
                    {
                        if (item.Status != (int)BrokerRequestStatus.Processed)
                            continue;

                        var persisted = await _brokerRequestRepository.GetAsync(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            persisted.IPNStatus = (byte)InstantPaymentNotificationStatus.Pending;
                            persisted.IPNResponse = null;
                        }
                    }

                    result = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;

                    if (result)
                    {
                        _brokerService.ProcessBrokerRequests(DMLCommand.Update, serviceHeader, brokerRequestDTOs.ToArray());
                    }
                }
            }

            return result;
        }

        public async Task<List<BrokerRequestDTO>> FindBrokerRequestsAsync(Guid[] brokerRequestIds, ServiceHeader serviceHeader)
        {
            if (brokerRequestIds != null && brokerRequestIds.Any())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = BrokerRequestSpecifications.BrokerRequestWithId(brokerRequestIds);

                    ISpecification<BrokerRequest> spec = filter;

                    return await _brokerRequestRepository.AllMatchingAsync<BrokerRequestDTO>(spec, serviceHeader);
                }
            }
            else return null;
        }

        public async Task<PageCollectionInfo<BrokerRequestDTO>> FindBrokerRequestsAsync(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BrokerRequestSpecifications.BrokerRequestWithDateRangeAndFilter(startDate, endDate, text);

                ISpecification<BrokerRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _brokerRequestRepository.AllMatchingPagedAsync<BrokerRequestDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<BrokerRequestDTO>> FindThirdPartyNotifiableBrokerRequestsAsync(string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BrokerRequestSpecifications.ThirdPartyNotifiableBrokerRequests(text, daysCap);

                ISpecification<BrokerRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _brokerRequestRepository.AllMatchingPagedAsync<BrokerRequestDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }
    }
}