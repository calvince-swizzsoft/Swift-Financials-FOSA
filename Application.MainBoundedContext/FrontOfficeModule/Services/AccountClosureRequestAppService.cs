using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.AccountClosureRequestAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class AccountClosureRequestAppService : IAccountClosureRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<AccountClosureRequest> _accountClosureRequestRepository;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IBrokerService _brokerService;

        public AccountClosureRequestAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<AccountClosureRequest> accountClosureRequestRepository,
            ICustomerAccountAppService customerAccountAppService,
            IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (accountClosureRequestRepository == null)
                throw new ArgumentNullException(nameof(accountClosureRequestRepository));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _accountClosureRequestRepository = accountClosureRequestRepository;
            _customerAccountAppService = customerAccountAppService;
            _brokerService = brokerService;
        }

        public AccountClosureRequestDTO AddNewAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, ServiceHeader serviceHeader)
        {
            if (accountClosureRequestDTO == null)
                return null;

            var existingRequests = FindAccountClosureRequestsByCustomerAccountId(accountClosureRequestDTO.CustomerAccountId, serviceHeader);

            if (existingRequests != null && existingRequests.Any(x => x.Status.In((int)AccountClosureRequestStatus.Registered, (int)AccountClosureRequestStatus.Audited, (int)AccountClosureRequestStatus.Approved, (int)AccountClosureRequestStatus.Deferred)))
            {
                accountClosureRequestDTO.errormassage = string.Format(("Sorry, but a closure request for the selected customer account is currently undergoing processing!"));
                return accountClosureRequestDTO;
            }
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var accountClosureRequest = AccountClosureRequestFactory.CreateAccountClosureRequest(accountClosureRequestDTO.CustomerAccountId, accountClosureRequestDTO.BranchId, accountClosureRequestDTO.Reason, serviceHeader);

                accountClosureRequest.Status = (int)AccountClosureRequestStatus.Registered;

                _accountClosureRequestRepository.Add(accountClosureRequest, serviceHeader);

                dbContextScope.SaveChanges(serviceHeader);

                return accountClosureRequest.ProjectedAs<AccountClosureRequestDTO>();
            }
        }

        public bool ApproveAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureApprovalOption, ServiceHeader serviceHeader)
        {
            if (accountClosureRequestDTO != null && Enum.IsDefined(typeof(AccountClosureApprovalOption), accountClosureApprovalOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _accountClosureRequestRepository.Get(accountClosureRequestDTO.Id, serviceHeader);

                    if (persisted != null && (persisted.Status == (int)AccountClosureRequestStatus.Registered || persisted.Status == (int)AccountClosureRequestStatus.Deferred))
                    {
                        switch ((AccountClosureApprovalOption)accountClosureApprovalOption)
                        {
                            case AccountClosureApprovalOption.Approve:
                                persisted.Status = (int)AccountClosureRequestStatus.Approved;
                                break;
                            case AccountClosureApprovalOption.Defer:
                                persisted.Status = (int)AccountClosureRequestStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        persisted.ApprovalRemarks = accountClosureRequestDTO.ApprovalRemarks;
                        persisted.ApprovedBy = serviceHeader.ApplicationUserName;
                        persisted.ApprovedDate = DateTime.Now;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool AuditAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureAuditOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (accountClosureRequestDTO != null && Enum.IsDefined(typeof(AccountClosureAuditOption), accountClosureAuditOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _accountClosureRequestRepository.Get(accountClosureRequestDTO.Id, serviceHeader);

                    if (persisted != null && (persisted.Status == (int)AccountClosureRequestStatus.Approved))
                    {
                        switch ((AccountClosureAuditOption)accountClosureAuditOption)
                        {
                            case AccountClosureAuditOption.Audit:
                                persisted.Status = (int)AccountClosureRequestStatus.Audited;
                                break;
                            case AccountClosureAuditOption.Defer:
                                persisted.Status = (int)AccountClosureRequestStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        persisted.AuditRemarks = accountClosureRequestDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }

                if (result && accountClosureAuditOption == (int)AccountClosureAuditOption.Audit)
                {
                    _customerAccountAppService.ManageCustomerAccount(accountClosureRequestDTO.CustomerAccountId, (int)CustomerAccountManagementAction.Closure, accountClosureRequestDTO.Reason, (int)CustomerAccountRemarkType.Informational, serviceHeader);
                }
            }

            return result;
        }

        public bool SettleAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureSettlementOption, ServiceHeader serviceHeader)
        {
            if (accountClosureRequestDTO != null && Enum.IsDefined(typeof(AccountClosureSettlementOption), accountClosureSettlementOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _accountClosureRequestRepository.Get(accountClosureRequestDTO.Id, serviceHeader);

                    if (persisted != null && (persisted.Status == (int)AccountClosureRequestStatus.Audited))
                    {
                        switch ((AccountClosureSettlementOption)accountClosureSettlementOption)
                        {
                            case AccountClosureSettlementOption.Settle:
                                persisted.Status = (int)AccountClosureRequestStatus.Settled;

                                #region Do we need to send alerts?

                                _brokerService.ProcessAccountClosureRequestAlerts(DMLCommand.None, serviceHeader, accountClosureRequestDTO);

                                #endregion
                                break;
                            case AccountClosureSettlementOption.Defer:
                                persisted.Status = (int)AccountClosureRequestStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        persisted.SettledBy = serviceHeader.ApplicationUserName;
                        persisted.SettledDate = DateTime.Now;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public AccountClosureRequestDTO FindAccountClosureRequest(Guid accountClosureRequestId, ServiceHeader serviceHeader)
        {
            if (accountClosureRequestId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var accountClosureRequest = _accountClosureRequestRepository.Get(accountClosureRequestId, serviceHeader);

                    if (accountClosureRequest != null)
                    {
                        return accountClosureRequest.ProjectedAs<AccountClosureRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<AccountClosureRequestDTO> FindAccountClosureRequests(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<AccountClosureRequest> spec = AccountClosureRequestSpecifications.DefaultSpec();

                var accountClosureRequests = _accountClosureRequestRepository.AllMatching(spec, serviceHeader);

                if (accountClosureRequests != null && accountClosureRequests.Any())
                {
                    return accountClosureRequests.ProjectedAsCollection<AccountClosureRequestDTO>();
                }
                else return null;
            }
        }

        public List<AccountClosureRequestDTO> FindAccountClosureRequestsByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = AccountClosureRequestSpecifications.AccountClosureRequestWithCustomerAccountId(customerAccountId);

                    ISpecification<AccountClosureRequest> spec = filter;

                    var accountClosureRequests = _accountClosureRequestRepository.AllMatching(spec, serviceHeader);

                    if (accountClosureRequests != null && accountClosureRequests.Any())
                    {
                        return accountClosureRequests.ProjectedAsCollection<AccountClosureRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequests(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AccountClosureRequestSpecifications.DefaultSpec();

                ISpecification<AccountClosureRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var accountClosureRequestPagedCollection = _accountClosureRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (accountClosureRequestPagedCollection != null)
                {
                    var pageCollection = accountClosureRequestPagedCollection.PageCollection.ProjectedAsCollection<AccountClosureRequestDTO>();

                    var itemsCount = accountClosureRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AccountClosureRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequests(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AccountClosureRequestSpecifications.AccountClosureRequestFullText(text, customerFilter);

                ISpecification<AccountClosureRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var accountClosureRequestPagedCollection = _accountClosureRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (accountClosureRequestPagedCollection != null)
                {
                    var pageCollection = accountClosureRequestPagedCollection.PageCollection.ProjectedAsCollection<AccountClosureRequestDTO>();

                    var itemsCount = accountClosureRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AccountClosureRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequests(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = AccountClosureRequestSpecifications.AccountClosureRequestWithDateRangeAndFullText(startDate, endDate, status, text, customerFilter);

                ISpecification<AccountClosureRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var accountClosureRequestPagedCollection = _accountClosureRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (accountClosureRequestPagedCollection != null)
                {
                    var pageCollection = accountClosureRequestPagedCollection.PageCollection.ProjectedAsCollection<AccountClosureRequestDTO>();

                    var itemsCount = accountClosureRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<AccountClosureRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }
    }
}
