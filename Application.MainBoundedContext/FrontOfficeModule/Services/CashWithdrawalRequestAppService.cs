using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashWithdrawalRequestAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class CashWithdrawalRequestAppService : ICashWithdrawalRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CashWithdrawalRequest> _cashWithdrawalRequestRepository;
        private readonly IHolidayAppService _holidayAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IChequeBookAppService _chequeBookAppService;

        public CashWithdrawalRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CashWithdrawalRequest> cashWithdrawalRequestRepository,
           IHolidayAppService holidayAppService,
           ISavingsProductAppService savingsProductAppService,
           IChequeBookAppService chequeBookAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (cashWithdrawalRequestRepository == null)
                throw new ArgumentNullException(nameof(cashWithdrawalRequestRepository));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (chequeBookAppService == null)
                throw new ArgumentNullException(nameof(chequeBookAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _cashWithdrawalRequestRepository = cashWithdrawalRequestRepository;
            _holidayAppService = holidayAppService;
            _savingsProductAppService = savingsProductAppService;
            _chequeBookAppService = chequeBookAppService;
        }

        public CashWithdrawalRequestDTO AddNewCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, ServiceHeader serviceHeader)
        {
            if (cashWithdrawalRequestDTO != null && cashWithdrawalRequestDTO.BranchId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var cashWithdrawalRequest = CashWithdrawalRequestFactory.CreateCashWithdrawalRequest(cashWithdrawalRequestDTO.BranchId, cashWithdrawalRequestDTO.CustomerAccountId, cashWithdrawalRequestDTO.ChartOfAccountId, cashWithdrawalRequestDTO.Type, cashWithdrawalRequestDTO.Category, cashWithdrawalRequestDTO.Amount, cashWithdrawalRequestDTO.Remarks, cashWithdrawalRequestDTO.PaymentVoucherId, cashWithdrawalRequestDTO.PaymentVoucherPayee);

                    switch ((CashWithdrawalRequestType)cashWithdrawalRequestDTO.Type)
                    {
                        case CashWithdrawalRequestType.ImmediateNotice:
                            cashWithdrawalRequest.MaturityDate = DateTime.Today;
                            break;
                        case CashWithdrawalRequestType.FutureNotice:
                            var savingsProduct = _savingsProductAppService.FindSavingsProduct(cashWithdrawalRequestDTO.CustomerAccountCustomerAccountTypeTargetProductId, cashWithdrawalRequestDTO.CustomerAccountBranchId, serviceHeader);
                            cashWithdrawalRequest.MaturityDate = _holidayAppService.FindBusinessDay(savingsProduct.WithdrawalNoticePeriod, true, serviceHeader) ?? DateTime.Today;
                            break;
                        default:
                            break;
                    }

                    cashWithdrawalRequest.Status = (int)CashWithdrawalRequestAuthStatus.Pending;
                    cashWithdrawalRequest.CreatedBy = serviceHeader.ApplicationUserName;

                    _cashWithdrawalRequestRepository.Add(cashWithdrawalRequest, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return cashWithdrawalRequest.ProjectedAs<CashWithdrawalRequestDTO>();
                }
            }
            else return null;
        }

        public bool AuthorizeCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, int customerTransactionAuthOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (cashWithdrawalRequestDTO != null && Enum.IsDefined(typeof(CustomerTransactionAuthOption), customerTransactionAuthOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _cashWithdrawalRequestRepository.Get(cashWithdrawalRequestDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var proceed = false;

                        switch ((CashWithdrawalRequestType)persisted.Type)
                        {
                            case CashWithdrawalRequestType.ImmediateNotice:
                                proceed = persisted.MaturityDate.Year == DateTime.Today.Year && persisted.MaturityDate.Month == DateTime.Today.Month && persisted.MaturityDate.Day == DateTime.Today.Day;
                                break;
                            case CashWithdrawalRequestType.FutureNotice:
                                proceed = persisted.MaturityDate <= DateTime.Today;
                                break;
                            default:
                                break;
                        }

                        if (proceed)
                        {
                            switch ((CustomerTransactionAuthOption)customerTransactionAuthOption)
                            {
                                case CustomerTransactionAuthOption.Authorize:
                                    persisted.Status = (int)CashWithdrawalRequestAuthStatus.Authorized;
                                    break;
                                case CustomerTransactionAuthOption.Reject:
                                    persisted.Status = (int)CashWithdrawalRequestAuthStatus.Rejected;
                                    break;
                                default:
                                    break;
                            }

                            persisted.AuthorizationRemarks = cashWithdrawalRequestDTO.AuthorizationRemarks;
                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;

                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                        }
                    }
                }
            }

            return result;
        }

        public bool PayCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, PaymentVoucherDTO paymentVoucherDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (cashWithdrawalRequestDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _cashWithdrawalRequestRepository.Get(cashWithdrawalRequestDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        persisted.Status = (int)CashWithdrawalRequestAuthStatus.Paid;

                        persisted.PaidBy = serviceHeader.ApplicationUserName;
                        persisted.PaidDate = DateTime.Now;

                        if (paymentVoucherDTO != null)
                            _chequeBookAppService.PayVoucher(paymentVoucherDTO, serviceHeader);

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
            }

            return result;
        }

        public List<CashWithdrawalRequestDTO> FindCashWithdrawalRequests(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var cashWithdrawalRequests = _cashWithdrawalRequestRepository.GetAll(serviceHeader);

                if (cashWithdrawalRequests != null && cashWithdrawalRequests.Any())
                {
                    return cashWithdrawalRequests.ProjectedAsCollection<CashWithdrawalRequestDTO>();
                }
                else return null;
            }
        }

        public List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByCustomerAccountId(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            var actionableCashWithdrawalRequests = FindActionableCashWithdrawalRequestsByCustomerAccount(customerAccountDTO, serviceHeader);

            if (actionableCashWithdrawalRequests != null && actionableCashWithdrawalRequests.Any())
            {
                var targetCashWithdrawalRequests = new List<CashWithdrawalRequestDTO>();

                foreach (var item in actionableCashWithdrawalRequests)
                {
                    switch ((CashWithdrawalRequestType)item.Type)
                    {
                        case CashWithdrawalRequestType.ImmediateNotice:
                            if (item.MaturityDate == DateTime.Today)
                                targetCashWithdrawalRequests.Add(item);
                            break;
                        case CashWithdrawalRequestType.FutureNotice:
                            if (item.MaturityDate >= item.CreatedDate)
                                targetCashWithdrawalRequests.Add(item);
                            break;
                        default:
                            break;
                    }
                }

                return targetCashWithdrawalRequests.OrderByDescending(x => x.CreatedDate).ToList();
            }
            else return null;
        }

        public List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByChartOfAccountId(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            var actionableCashWithdrawalRequests = FindActionableCashWithdrawalRequestsByChartOfAccountId(chartOfAccountId, serviceHeader);

            if (actionableCashWithdrawalRequests != null && actionableCashWithdrawalRequests.Any())
            {
                var targetCashWithdrawalRequests = new List<CashWithdrawalRequestDTO>();

                foreach (var item in actionableCashWithdrawalRequests)
                {
                    switch ((CashWithdrawalRequestType)item.Type)
                    {
                        case CashWithdrawalRequestType.ImmediateNotice:
                            if (item.MaturityDate == DateTime.Today)
                                targetCashWithdrawalRequests.Add(item);
                            break;
                        case CashWithdrawalRequestType.FutureNotice:
                            if (item.MaturityDate >= item.CreatedDate)
                                targetCashWithdrawalRequests.Add(item);
                            break;
                        default:
                            break;
                    }
                }

                return targetCashWithdrawalRequests.OrderByDescending(x => x.CreatedDate).ToList();
            }
            else return null;
        }

        public PageCollectionInfo<CashWithdrawalRequestDTO> FindCashWithdrawalRequests(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashWithdrawalRequestSpecifications.CashWithdrawalRequestWithDateRangeAndFullText(startDate, endDate, status, text, customerFilter);

                ISpecification<CashWithdrawalRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var cashWithdrawalRequestPagedCollection = _cashWithdrawalRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (cashWithdrawalRequestPagedCollection != null)
                {
                    var pageCollection = cashWithdrawalRequestPagedCollection.PageCollection.ProjectedAsCollection<CashWithdrawalRequestDTO>();

                    var itemsCount = cashWithdrawalRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CashWithdrawalRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public CashWithdrawalRequestDTO FindCashWithdrawalRequest(Guid cashWithdrawalRequestId, ServiceHeader serviceHeader)
        {
            if (cashWithdrawalRequestId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var cashWithdrawalRequest = _cashWithdrawalRequestRepository.Get(cashWithdrawalRequestId, serviceHeader);

                    if (cashWithdrawalRequest != null)
                    {
                        return cashWithdrawalRequest.ProjectedAs<CashWithdrawalRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<CashWithdrawalRequestDTO> FindActionableCashWithdrawalRequestsByCustomerAccount(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountDTO != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var startDate = DateTime.Today.AddDays(customerAccountDTO.CustomerAccountTypeTargetProductWithdrawalNoticePeriod * -2);

                    var endDate = DateTime.Now;

                    var filter = CashWithdrawalRequestSpecifications.ActionableCashWithdrawalRequestWithCustomerAccountId(customerAccountDTO.Id, serviceHeader.ApplicationUserName, startDate, endDate);

                    ISpecification<CashWithdrawalRequest> spec = filter;

                    var cashWithdrawalRequests = _cashWithdrawalRequestRepository.AllMatching(spec, serviceHeader);

                    if (cashWithdrawalRequests != null && cashWithdrawalRequests.Any())
                    {
                        return cashWithdrawalRequests.ProjectedAsCollection<CashWithdrawalRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<CashWithdrawalRequestDTO> FindActionableCashWithdrawalRequestsByChartOfAccountId(Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            if (chartOfAccountId != null && chartOfAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var startDate = DateTime.Today;

                    var endDate = DateTime.Now;

                    var filter = CashWithdrawalRequestSpecifications.ActionableCashWithdrawalRequestWithChartOfAccountId(chartOfAccountId, serviceHeader.ApplicationUserName, startDate, endDate);

                    ISpecification<CashWithdrawalRequest> spec = filter;

                    var cashWithdrawalRequests = _cashWithdrawalRequestRepository.AllMatching(spec, serviceHeader);

                    if (cashWithdrawalRequests != null && cashWithdrawalRequests.Any())
                    {
                        return cashWithdrawalRequests.ProjectedAsCollection<CashWithdrawalRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
