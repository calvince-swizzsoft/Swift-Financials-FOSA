using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRequestAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public class LoanRequestAppService : ILoanRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LoanRequest> _loanRequestRepository;
        private readonly IBrokerService _brokerService;

        public LoanRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LoanRequest> loanRequestRepository,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (loanRequestRepository == null)
                throw new ArgumentNullException(nameof(loanRequestRepository));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _loanRequestRepository = loanRequestRepository;
            _brokerService = brokerService;
        }

        public LoanRequestDTO AddNewLoanRequest(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader)
        {
            if (loanRequestDTO != null)
            {
                var existingLoanRequests = FindLoanRequests(loanRequestDTO.CustomerId, loanRequestDTO.LoanProductId, serviceHeader);

                if (existingLoanRequests != null && existingLoanRequests.Any(x => x.Status.In((int)LoanRequestStatus.New)))
                    throw new InvalidOperationException("Sorry, but selected customer has a pending loan request for the selected loan product!");

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var loanRequest = LoanRequestFactory.CreateLoanRequest(loanRequestDTO.CustomerId, loanRequestDTO.LoanProductId, loanRequestDTO.LoanPurposeId, loanRequestDTO.AmountApplied, loanRequestDTO.ReceivedDate, loanRequestDTO.Reference);

                    loanRequest.Status = (int)LoanRequestStatus.New;
                    loanRequest.CreatedBy = serviceHeader.ApplicationUserName;

                    _loanRequestRepository.Add(loanRequest, serviceHeader);

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        loanRequestDTO = loanRequest.ProjectedAs<LoanRequestDTO>();

                        #region Do we need to send alerts?

                        _brokerService.ProcessLoanRequestAccountAlerts(DMLCommand.None, serviceHeader, loanRequestDTO);

                        #endregion

                        return loanRequestDTO;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool CancelLoanRequest(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader)
        {
            if (loanRequestDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanRequestRepository.Get(loanRequestDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)LoanRequestStatus.New)
                    {
                        persisted.Status = (int)LoanRequestStatus.Rejected;

                        persisted.CancelledBy = serviceHeader.ApplicationUserName;
                        persisted.CancelledDate = DateTime.Now;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool RegisterLoanRequest(LoanRequestDTO loanRequestDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (loanRequestDTO == null)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanRequestRepository.Get(loanRequestDTO.Id, serviceHeader);

                if (persisted != null && persisted.Status == (int)LoanRequestStatus.New)
                {
                    persisted.Status = (int)LoanRequestStatus.Registered;

                    persisted.LoanCaseNumber = loanRequestDTO.LoanCaseNumber;
                    persisted.RegisteredBy = serviceHeader.ApplicationUserName;
                    persisted.RegisteredDate = DateTime.Now;

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }

            return result;
        }

        public bool RemoveLoanRequest(Guid loanRequestId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (loanRequestId != null && loanRequestId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanRequestRepository.Get(loanRequestId, serviceHeader);

                    if (persisted != null)
                    {
                        _loanRequestRepository.Remove(persisted, serviceHeader);

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
            }

            return result;
        }

        public LoanRequestDTO FindLoanRequest(Guid loanRequestId, ServiceHeader serviceHeader)
        {
            if (loanRequestId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanRequest = _loanRequestRepository.Get(loanRequestId, serviceHeader);

                    if (loanRequest != null)
                    {
                        return loanRequest.ProjectedAs<LoanRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanRequestDTO> FindLoanRequests(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var loanRequests = _loanRequestRepository.GetAll(serviceHeader);

                if (loanRequests != null && loanRequests.Any())
                {
                    return loanRequests.ProjectedAsCollection<LoanRequestDTO>();
                }
                else return null;
            }
        }

        public List<LoanRequestDTO> FindLoanRequests(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty && loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanRequestSpecifications.LoanRequestWithCustomerIdAndLoanProductId(customerId, loanProductId);

                    ISpecification<LoanRequest> spec = filter;

                    var loanRequests = _loanRequestRepository.AllMatching(spec, serviceHeader);

                    if (loanRequests != null && loanRequests.Any())
                    {
                        return loanRequests.ProjectedAsCollection<LoanRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanRequestDTO> FindLoanRequestsByCustomerIdInProcess(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanRequestSpecifications.LoanRequestWithCustomerIdInProcess(customerId);

                    ISpecification<LoanRequest> spec = filter;

                    var loanRequests = _loanRequestRepository.AllMatching(spec, serviceHeader);

                    if (loanRequests != null && loanRequests.Any())
                    {
                        return loanRequests.ProjectedAsCollection<LoanRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<LoanRequestDTO> FindLoanRequests(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanRequestSpecifications.DefaultSpec();

                ISpecification<LoanRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanRequestPagedCollection = _loanRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanRequestPagedCollection != null)
                {
                    var pageCollection = loanRequestPagedCollection.PageCollection.ProjectedAsCollection<LoanRequestDTO>();

                    var itemsCount = loanRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanRequestDTO> FindLoanRequests(string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanRequestSpecifications.LoanRequestFullText(text, loanRequestFilter);

                ISpecification<LoanRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanRequestPagedCollection = _loanRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanRequestPagedCollection != null)
                {
                    var pageCollection = loanRequestPagedCollection.PageCollection.ProjectedAsCollection<LoanRequestDTO>();

                    var itemsCount = loanRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanRequestDTO> FindLoanRequests(DateTime startDate, DateTime endDate, string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanRequestSpecifications.LoanRequestWithDateRangeAndFullText(startDate, endDate, text, loanRequestFilter);

                ISpecification<LoanRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanRequestPagedCollection = _loanRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanRequestPagedCollection != null)
                {
                    var pageCollection = loanRequestPagedCollection.PageCollection.ProjectedAsCollection<LoanRequestDTO>();

                    var itemsCount = loanRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanRequestDTO> FindLoanRequests(DateTime startDate, DateTime endDate, int status, string text, int loanRequestFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanRequestSpecifications.LoanRequestWithStatusAndFullText(startDate, endDate, status, text, loanRequestFilter);

                ISpecification<LoanRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanRequestPagedCollection = _loanRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanRequestPagedCollection != null)
                {
                    var pageCollection = loanRequestPagedCollection.PageCollection.ProjectedAsCollection<LoanRequestDTO>();

                    var itemsCount = loanRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

    }
}
