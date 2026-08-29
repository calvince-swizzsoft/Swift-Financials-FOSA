using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashDepositRequestAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class CashDepositRequestAppService : ICashDepositRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CashDepositRequest> _cashDepositRequestRepository;
        private readonly IHolidayAppService _holidayAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IChequeBookAppService _chequeBookAppService;
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly IWorkflowAppService _workflowAppService;

        public CashDepositRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CashDepositRequest> cashDepositRequestRepository,
           IHolidayAppService holidayAppService,
           ISavingsProductAppService savingsProductAppService,
           IChequeBookAppService chequeBookAppService,
           IAuthorizationAppService authorizationAppService,
           IWorkflowAppService workflowAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (cashDepositRequestRepository == null)
                throw new ArgumentNullException(nameof(cashDepositRequestRepository));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (chequeBookAppService == null)
                throw new ArgumentNullException(nameof(chequeBookAppService));

            if (authorizationAppService == null)
                throw new ArgumentNullException(nameof(authorizationAppService));

            if (workflowAppService == null)
                throw new ArgumentNullException(nameof(workflowAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _cashDepositRequestRepository = cashDepositRequestRepository;
            _holidayAppService = holidayAppService;
            _savingsProductAppService = savingsProductAppService;
            _chequeBookAppService = chequeBookAppService;
            _authorizationAppService = authorizationAppService;
            _workflowAppService = workflowAppService;
        }

        public CashDepositRequestDTO AddNewCashDepositRequestWithWorkflow(CashDepositRequestDTO cashDepositRequestDTO, ServiceHeader serviceHeader)
        {
            var roles = _authorizationAppService.GetRolesListForSystemPermissionType(
                (int)SystemPermissionType.CashDepositRequestAuthorization, serviceHeader);

            if (roles == null || !roles.Any() || roles.Sum(x => x.RequiredApprovers) < 1)
                throw new InvalidOperationException("Cash deposit request cannot be submitted because no approval role is configured for Cash Deposit Request Authorization.");

            var created = AddNewCashDepositRequest(cashDepositRequestDTO, serviceHeader);
            if (created == null)
                return null;

            var workflow = new WorkflowDTO
            {
                RecordId = created.Id,
                BranchId = created.BranchId,
                Status = (int)WorkflowRecordStatus.Pending,
                SystemPermissionType = (int)SystemPermissionType.CashDepositRequestAuthorization,
                RequiredApprovals = roles.Sum(x => x.RequiredApprovers)
            };

            if (!_workflowAppService.AddNewWorkflow(workflow, roles, serviceHeader))
                throw new InvalidOperationException("The cash deposit request was stored, but its approval workflow could not be created. Contact an administrator before retrying.");

            return created;
        }

        public CashDepositRequestDTO AddNewCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, ServiceHeader serviceHeader)
        {
            if (cashDepositRequestDTO != null && cashDepositRequestDTO.BranchId != Guid.Empty)
            {
                cashDepositRequestDTO.ValidateAll();
                if (cashDepositRequestDTO.HasErrors)
                    throw new InvalidOperationException(string.Join("; ", cashDepositRequestDTO.ErrorMessages));

                if (cashDepositRequestDTO.CustomerAccountId == Guid.Empty)
                    throw new InvalidOperationException("A customer savings account is required.");

                if (cashDepositRequestDTO.Amount <= 0m)
                    throw new InvalidOperationException("The cash deposit request amount must be greater than zero.");

                if (!Enum.IsDefined(typeof(FrontOfficeTransactionType), cashDepositRequestDTO.TransactionType))
                    throw new InvalidOperationException("The transaction type is invalid.");

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var cashDepositRequest = CashDepositRequestFactory.CreateCashDepositRequest(cashDepositRequestDTO.BranchId, cashDepositRequestDTO.CustomerAccountId, cashDepositRequestDTO.Amount, cashDepositRequestDTO.Remarks, cashDepositRequestDTO.TransactionType);

                    //cashDepositRequest.Status = (int)CashDepositRequestAuthStatus.Pending;

                  
                    cashDepositRequest.Status = (byte)cashDepositRequestDTO.Status;
                    cashDepositRequest.CreatedBy = serviceHeader.ApplicationUserName;

                    _cashDepositRequestRepository.Add(cashDepositRequest, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return cashDepositRequest.ProjectedAs<CashDepositRequestDTO>();
                }
            }
            else return null;
        }

        public bool AuthorizeCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, int customerTransactionAuthOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (cashDepositRequestDTO != null && Enum.IsDefined(typeof(CustomerTransactionAuthOption), customerTransactionAuthOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _cashDepositRequestRepository.Get(cashDepositRequestDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        switch ((CustomerTransactionAuthOption)customerTransactionAuthOption)
                        {
                            case CustomerTransactionAuthOption.Authorize:
                                persisted.Status = (int)CashDepositRequestAuthStatus.Authorized;
                                break;
                            case CustomerTransactionAuthOption.Reject:
                                persisted.Status = (int)CashDepositRequestAuthStatus.Rejected;
                                break;
                            default:
                                break;
                        }

                        persisted.AuthorizationRemarks = cashDepositRequestDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
            }

            return result;
        }

        public bool PostCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (cashDepositRequestDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _cashDepositRequestRepository.Get(cashDepositRequestDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        persisted.Status = (int)CashDepositRequestAuthStatus.Posted;

                        persisted.PostedBy = serviceHeader.ApplicationUserName;
                        persisted.PostedDate = DateTime.Now;

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
            }

            return result;
        }

        public List<CashDepositRequestDTO> FindCashDepositRequests(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var cashDepositRequests = _cashDepositRequestRepository.GetAll(serviceHeader);

                if (cashDepositRequests != null && cashDepositRequests.Any())
                {
                    return cashDepositRequests.ProjectedAsCollection<CashDepositRequestDTO>();
                }
                else return null;
            }
        }

        public List<CashDepositRequestDTO> FindActionableCashDepositRequestsByCustomerAccount(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountDTO != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var startDate = DateTime.Today;

                    var endDate = DateTime.Now;

                    var filter = CashDepositRequestSpecifications.ActionableCashDepositRequestWithCustomerAccountId(customerAccountDTO.Id, serviceHeader.ApplicationUserName, startDate, endDate);

                    ISpecification<CashDepositRequest> spec = filter;

                    var cashDepositRequests = _cashDepositRequestRepository.AllMatching(spec, serviceHeader);

                    if (cashDepositRequests != null && cashDepositRequests.Any())
                    {
                        return cashDepositRequests.ProjectedAsCollection<CashDepositRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CashDepositRequestDTO> FindCashDepositRequests(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashDepositRequestSpecifications.CashDepositRequestWithDateRangeAndFullText(startDate, endDate, status, text, customerFilter);

                ISpecification<CashDepositRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var cashDepositRequestPagedCollection = _cashDepositRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (cashDepositRequestPagedCollection != null)
                {
                    var pageCollection = cashDepositRequestPagedCollection.PageCollection.ProjectedAsCollection<CashDepositRequestDTO>();

                    var itemsCount = cashDepositRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CashDepositRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public CashDepositRequestDTO FindCashDepositRequest(Guid cashDepositRequestId, ServiceHeader serviceHeader)
        {
            if (cashDepositRequestId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var cashDepositRequest = _cashDepositRequestRepository.Get(cashDepositRequestId, serviceHeader);

                    if (cashDepositRequest != null)
                    {
                        return cashDepositRequest.ProjectedAs<CashDepositRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
