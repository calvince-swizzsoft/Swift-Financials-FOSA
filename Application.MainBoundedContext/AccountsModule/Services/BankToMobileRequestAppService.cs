using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankToMobileRequestAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class BankToMobileRequestAppService : IBankToMobileRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<BankToMobileRequest> _bankToMobileRequestRepository;
        private readonly IBrokerService _brokerService;

        public BankToMobileRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<BankToMobileRequest> bankToMobileRequestRepository,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (bankToMobileRequestRepository == null)
                throw new ArgumentNullException(nameof(bankToMobileRequestRepository));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _bankToMobileRequestRepository = bankToMobileRequestRepository;
            _brokerService = brokerService;
        }

        public BankToMobileRequestDTO AddNewBankToMobileRequest(BankToMobileRequestDTO bankToMobileRequestDTO, ServiceHeader serviceHeader)
        {
            if (bankToMobileRequestDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var bankToMobileRequest = BankToMobileRequestFactory.CreateBankToMobileRequest(bankToMobileRequestDTO.TransactionType, bankToMobileRequestDTO.AccountNumber, bankToMobileRequestDTO.TransactionCode, bankToMobileRequestDTO.UniqueTransactionIdentifier, bankToMobileRequestDTO.TransactionAmount, bankToMobileRequestDTO.CallbackPayload, bankToMobileRequestDTO.IncomingCipherTextPayload, bankToMobileRequestDTO.IncomingPlainTextPayload, bankToMobileRequestDTO.SystemTraceAuditNumber);
                    bankToMobileRequest.Status = (byte)BankToMobileRequestStatus.Pending;
                    bankToMobileRequest.IPNEnabled = bankToMobileRequestDTO.IPNEnabled;
                    bankToMobileRequest.CreatedBy = serviceHeader.ApplicationUserName;

                    _bankToMobileRequestRepository.Add(bankToMobileRequest, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return bankToMobileRequest.ProjectedAs<BankToMobileRequestDTO>();
                }
            }
            else return null;
        }

        public bool UpdateBankToMobileRequestResponse(Guid bankToMobileRequestId, string outgoingPlainTextPayload, string outgoingCipherTextPayload, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _bankToMobileRequestRepository.Get(bankToMobileRequestId, serviceHeader);

                if (persisted != null)
                {
                    persisted.OutgoingPlainTextPayload = outgoingPlainTextPayload;
                    persisted.OutgoingCipherTextPayload = outgoingCipherTextPayload;
                    persisted.Status = (byte)BankToMobileRequestStatus.Processed;
                }

                result = dbContextScope.SaveChanges(serviceHeader) > 0;

                if (result && persisted.IPNEnabled)
                {
                    _brokerService.ProcessBankToMobileRequests(DMLCommand.Update, serviceHeader, persisted.ProjectedAs<BankToMobileRequestDTO>());
                }
            }

            return result;
        }

        public bool UpdateBankToMobileRequestIPNStatus(Guid bankToMobileRequestId, int ipnStatus, string ipnResponse, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _bankToMobileRequestRepository.Get(bankToMobileRequestId, serviceHeader);

                if (persisted != null)
                {
                    persisted.IPNStatus = (byte)ipnStatus;
                    persisted.IPNResponse = ipnResponse;
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public bool ResetBankToMobileRequestsIPNStatus(Guid[] bankToMobileRequestIds, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var bankToMobileRequestDTOs = FindBankToMobileRequests(bankToMobileRequestIds, serviceHeader);

            if (bankToMobileRequestDTOs != null && bankToMobileRequestDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var item in bankToMobileRequestDTOs)
                    {
                        if (item.Status != (int)BankToMobileRequestStatus.Processed)
                            continue;

                        var persisted = _bankToMobileRequestRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            persisted.IPNStatus = (byte)InstantPaymentNotificationStatus.Pending;
                            persisted.IPNResponse = null;
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) > 0;

                    if (result)
                    {
                        _brokerService.ProcessBankToMobileRequests(DMLCommand.Update, serviceHeader, bankToMobileRequestDTOs.ToArray());
                    }
                }
            }

            return result;
        }

        public List<BankToMobileRequestDTO> FindBankToMobileRequests(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var bankToMobileRequests = _bankToMobileRequestRepository.GetAll(serviceHeader);

                if (bankToMobileRequests != null && bankToMobileRequests.Any())
                {
                    return bankToMobileRequests.ProjectedAsCollection<BankToMobileRequestDTO>();
                }
                else return null;
            }
        }

        public List<BankToMobileRequestDTO> FindBankToMobileRequests(Guid[] bankToMobileRequestIds, ServiceHeader serviceHeader)
        {
            if (bankToMobileRequestIds != null && bankToMobileRequestIds.Any())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = BankToMobileRequestSpecifications.BankToMobileRequestWithId(bankToMobileRequestIds);

                    ISpecification<BankToMobileRequest> spec = filter;

                    var bankToMobileRequests = _bankToMobileRequestRepository.AllMatching(spec, serviceHeader);

                    if (bankToMobileRequests != null && bankToMobileRequests.Any())
                    {
                        return bankToMobileRequests.ProjectedAsCollection<BankToMobileRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public BankToMobileRequestDTO FindBankToMobileRequest(Guid bankToMobileRequestId, ServiceHeader serviceHeader)
        {
            if (bankToMobileRequestId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var bankToMobileRequest = _bankToMobileRequestRepository.Get(bankToMobileRequestId, serviceHeader);

                    if (bankToMobileRequest != null)
                    {
                        return bankToMobileRequest.ProjectedAs<BankToMobileRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<BankToMobileRequestDTO> FindBankToMobileRequests(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BankToMobileRequestSpecifications.BankToMobileRequestWithDateRangeAndFilter(startDate, endDate, text);

                ISpecification<BankToMobileRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankToMobileRequestPagedCollection = _bankToMobileRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankToMobileRequestPagedCollection != null)
                {
                    var pageCollection = bankToMobileRequestPagedCollection.PageCollection.ProjectedAsCollection<BankToMobileRequestDTO>();

                    var itemsCount = bankToMobileRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BankToMobileRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<BankToMobileRequestDTO> FindThirdPartyNotifiableBankToMobileRequests(string text, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = BankToMobileRequestSpecifications.ThirdPartyNotifiableBankToMobileRequests(text, daysCap);

                ISpecification<BankToMobileRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var bankToMobileRequestPagedCollection = _bankToMobileRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (bankToMobileRequestPagedCollection != null)
                {
                    var pageCollection = bankToMobileRequestPagedCollection.PageCollection.ProjectedAsCollection<BankToMobileRequestDTO>();

                    var itemsCount = bankToMobileRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<BankToMobileRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }
    }
}
