using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
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
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly IWorkflowAppService _workflowAppService;

        public CashWithdrawalRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CashWithdrawalRequest> cashWithdrawalRequestRepository,
           IHolidayAppService holidayAppService,
           ISavingsProductAppService savingsProductAppService,
           IChequeBookAppService chequeBookAppService,
           IAuthorizationAppService authorizationAppService,
           IWorkflowAppService workflowAppService)
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

            if (authorizationAppService == null)
                throw new ArgumentNullException(nameof(authorizationAppService));

            if (workflowAppService == null)
                throw new ArgumentNullException(nameof(workflowAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _cashWithdrawalRequestRepository = cashWithdrawalRequestRepository;
            _holidayAppService = holidayAppService;
            _savingsProductAppService = savingsProductAppService;
            _chequeBookAppService = chequeBookAppService;
            _authorizationAppService = authorizationAppService;
            _workflowAppService = workflowAppService;
        }

        private void EnrichRequestDetails(CashWithdrawalRequestDTO request, ServiceHeader serviceHeader)
        {
            if (request == null) return;

            // Some legacy requests stored an internal actor/teller identifier
            // in Remarks. It is not customer-facing commentary.
            Guid internalId;
            if (Guid.TryParse(request.Remarks, out internalId))
                request.Remarks = null;

            if (string.IsNullOrWhiteSpace(request.CustomerAccountCustomerAccountTypeTargetProductDescription)
                && request.CustomerAccountCustomerAccountTypeTargetProductId != Guid.Empty)
            {
                var product = _savingsProductAppService.FindSavingsProduct(
                    request.CustomerAccountCustomerAccountTypeTargetProductId,
                    request.CustomerAccountBranchId != Guid.Empty ? request.CustomerAccountBranchId : request.BranchId,
                    serviceHeader);

                request.CustomerAccountCustomerAccountTypeTargetProductDescription =
                    product != null ? product.Description : null;
            }

            if (request.PaymentVoucherId != Guid.Empty)
            {
                var voucher = _chequeBookAppService.FindPaymentVoucher(request.PaymentVoucherId, serviceHeader);
                if (voucher != null)
                {
                    request.PaymentVoucherNumber = voucher.PaddedVoucherNumber;
                    request.PaymentVoucherStatus = voucher.Status;
                }
            }
        }

        public CashWithdrawalRequestDTO AddNewCashWithdrawalRequestWithWorkflow(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, ServiceHeader serviceHeader)
        {
            var roles = _authorizationAppService.GetRolesListForSystemPermissionType(
                (int)SystemPermissionType.CashWithdrawalRequestAuthorization, serviceHeader);

            if (roles == null || !roles.Any() || roles.Sum(x => x.RequiredApprovers) < 1)
                throw new InvalidOperationException("Cash withdrawal request cannot be submitted because no approval role is configured for Cash Withdrawal Request Authorization.");

            var created = AddNewCashWithdrawalRequest(cashWithdrawalRequestDTO, serviceHeader);
            if (created == null)
                return null;

            var workflow = new WorkflowDTO
            {
                RecordId = created.Id,
                BranchId = created.BranchId,
                Status = (int)WorkflowRecordStatus.Pending,
                SystemPermissionType = (int)SystemPermissionType.CashWithdrawalRequestAuthorization,
                RequiredApprovals = roles.Sum(x => x.RequiredApprovers)
            };

            if (!_workflowAppService.AddNewWorkflow(workflow, roles, serviceHeader))
                throw new InvalidOperationException("The withdrawal request was stored, but its approval workflow could not be created. Contact an administrator before retrying.");

            return created;
        }

        public bool ResendCashWithdrawalApprovalRequest(Guid cashWithdrawalRequestId, ServiceHeader serviceHeader)
        {
            if (cashWithdrawalRequestId == Guid.Empty)
                throw new InvalidOperationException("A cash withdrawal request is required.");

            var request = FindCashWithdrawalRequest(cashWithdrawalRequestId, serviceHeader);
            if (request == null)
                throw new InvalidOperationException("The cash withdrawal request could not be found.");
            if (request.Status != (int)CashWithdrawalRequestAuthStatus.Pending)
                throw new InvalidOperationException("Only a pending cash withdrawal request can be resent for approval.");

            var permissionType = (int)SystemPermissionType.CashWithdrawalRequestAuthorization;
            if (_workflowAppService.IsWorkflowInProgress(request.Id, permissionType, serviceHeader))
                throw new InvalidOperationException("This cash withdrawal already has an unactioned approval request. Complete that approval before resending.");

            var roles = _authorizationAppService.GetRolesListForSystemPermissionType(permissionType, serviceHeader);
            if (roles == null || !roles.Any() || roles.Sum(x => x.RequiredApprovers) < 1)
                throw new InvalidOperationException("Cash withdrawal request cannot be resent because no approval role is configured for Cash Withdrawal Request Authorization.");

            return _workflowAppService.AddNewWorkflow(new WorkflowDTO
            {
                RecordId = request.Id,
                BranchId = request.BranchId,
                Status = (int)WorkflowRecordStatus.Pending,
                SystemPermissionType = permissionType,
                RequiredApprovals = roles.Sum(x => x.RequiredApprovers)
            }, roles, serviceHeader);
        }

        public CashWithdrawalRequestDTO AddNewCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, ServiceHeader serviceHeader)
        {
            return AddNewCashWithdrawalRequest(cashWithdrawalRequestDTO, false, serviceHeader);
        }

        public CashWithdrawalRequestDTO RecordPaidCashWithdrawal(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, ServiceHeader serviceHeader)
        {
            if (cashWithdrawalRequestDTO == null || cashWithdrawalRequestDTO.Category != (int)CashWithdrawalCategory.WithinLimits)
                throw new InvalidOperationException("Only a successfully posted within-limit withdrawal can be recorded as paid.");

            return AddNewCashWithdrawalRequest(cashWithdrawalRequestDTO, true, serviceHeader);
        }

        private CashWithdrawalRequestDTO AddNewCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, bool recordAsPaid, ServiceHeader serviceHeader)
        {
            if (cashWithdrawalRequestDTO != null && cashWithdrawalRequestDTO.BranchId != Guid.Empty)
            {
                cashWithdrawalRequestDTO.ValidateAll();
                if (cashWithdrawalRequestDTO.HasErrors)
                    throw new InvalidOperationException(string.Join("; ", cashWithdrawalRequestDTO.ErrorMessages));

                if (!cashWithdrawalRequestDTO.CustomerAccountId.HasValue || cashWithdrawalRequestDTO.CustomerAccountId == Guid.Empty)
                    throw new InvalidOperationException("A customer savings account is required.");

                if (!Enum.IsDefined(typeof(CashWithdrawalRequestType), cashWithdrawalRequestDTO.Type))
                    throw new InvalidOperationException("The withdrawal notice type is invalid.");

                if (!Enum.IsDefined(typeof(CashWithdrawalCategory), cashWithdrawalRequestDTO.Category))
                    throw new InvalidOperationException("The cash withdrawal category is invalid.");

                if (cashWithdrawalRequestDTO.TransactionType != (int)FrontOfficeTransactionType.CashWithdrawal &&
                    cashWithdrawalRequestDTO.TransactionType != (int)FrontOfficeTransactionType.CashWithdrawalPaymentVoucher)
                    throw new InvalidOperationException("The transaction type is not a cash withdrawal.");

                if (cashWithdrawalRequestDTO.TransactionType == (int)FrontOfficeTransactionType.CashWithdrawalPaymentVoucher)
                {
                    if (cashWithdrawalRequestDTO.PaymentVoucherId == Guid.Empty)
                        throw new InvalidOperationException("An active payment voucher is required.");
                    if (string.IsNullOrWhiteSpace(cashWithdrawalRequestDTO.PaymentVoucherPayee)
                        || string.IsNullOrWhiteSpace(cashWithdrawalRequestDTO.PaymentVoucherReference)
                        || !cashWithdrawalRequestDTO.PaymentVoucherWriteDate.HasValue)
                        throw new InvalidOperationException("Payment voucher payee, reference, and write date are required.");

                    var voucher = _chequeBookAppService.FindPaymentVoucher(cashWithdrawalRequestDTO.PaymentVoucherId, serviceHeader);
                    if (voucher == null || voucher.Status != (int)PaymentVoucherStatus.Active)
                        throw new InvalidOperationException("The selected payment voucher does not exist, has already been paid, or has been flagged.");
                    if (!cashWithdrawalRequestDTO.CustomerAccountId.HasValue
                        || voucher.ChequeBookCustomerAccountId != cashWithdrawalRequestDTO.CustomerAccountId.Value
                        || !voucher.ChequeBookIsActive || voucher.ChequeBookIsLocked)
                        throw new InvalidOperationException("The selected payment voucher is not from an active, unlocked cheque book for this customer account.");

                    voucher.Payee = cashWithdrawalRequestDTO.PaymentVoucherPayee.Trim();
                    voucher.Reference = cashWithdrawalRequestDTO.PaymentVoucherReference.Trim();
                    voucher.WriteDate = cashWithdrawalRequestDTO.PaymentVoucherWriteDate;
                    voucher.Amount = cashWithdrawalRequestDTO.Amount;
                    voucher.ValidateAll();
                    if (voucher.HasErrors)
                        throw new InvalidOperationException(string.Join("; ", voucher.ErrorMessages));
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var cashWithdrawalRequest = CashWithdrawalRequestFactory.CreateCashWithdrawalRequest(cashWithdrawalRequestDTO.BranchId, cashWithdrawalRequestDTO.CustomerAccountId, cashWithdrawalRequestDTO.ChartOfAccountId, cashWithdrawalRequestDTO.Type, cashWithdrawalRequestDTO.Category, cashWithdrawalRequestDTO.Amount, cashWithdrawalRequestDTO.Remarks, cashWithdrawalRequestDTO.PaymentVoucherId, cashWithdrawalRequestDTO.PaymentVoucherPayee, cashWithdrawalRequestDTO.PaymentVoucherReference, cashWithdrawalRequestDTO.PaymentVoucherWriteDate, cashWithdrawalRequestDTO.TransactionType);

                    switch ((CashWithdrawalRequestType)cashWithdrawalRequestDTO.Type)
                    {
                        case CashWithdrawalRequestType.ImmediateNotice:
                            cashWithdrawalRequest.MaturityDate = DateTime.Today;
                            break;
                        case CashWithdrawalRequestType.FutureNotice:
                            var savingsProduct = _savingsProductAppService.FindSavingsProduct(cashWithdrawalRequestDTO.CustomerAccountCustomerAccountTypeTargetProductId, cashWithdrawalRequestDTO.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct == null)
                                throw new InvalidOperationException("The savings product for this account could not be resolved.");
                            cashWithdrawalRequest.MaturityDate = _holidayAppService.FindBusinessDay(savingsProduct.WithdrawalNoticePeriod, true, serviceHeader) ?? DateTime.Today;
                            break;
                        default:
                            break;
                    }

                    cashWithdrawalRequest.Status = (byte)(recordAsPaid
                        ? CashWithdrawalRequestAuthStatus.Paid
                        : CashWithdrawalRequestAuthStatus.Pending);
                    cashWithdrawalRequest.CreatedBy = serviceHeader.ApplicationUserName;
                    if (recordAsPaid)
                    {
                        cashWithdrawalRequest.AuthorizedBy = serviceHeader.ApplicationUserName;
                        cashWithdrawalRequest.AuthorizedDate = DateTime.Now;
                        cashWithdrawalRequest.PaidBy = serviceHeader.ApplicationUserName;
                        cashWithdrawalRequest.PaidDate = DateTime.Now;
                    }

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
                        if (persisted.Status != (int)CashWithdrawalRequestAuthStatus.Pending)
                            return false;

                        if (string.IsNullOrWhiteSpace(cashWithdrawalRequestDTO.AuthorizationRemarks))
                            return false;

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
                        if (persisted.Status != (int)CashWithdrawalRequestAuthStatus.Authorized)
                            return false;

                        persisted.Status = (int)CashWithdrawalRequestAuthStatus.Paid;

                        persisted.PaidBy = serviceHeader.ApplicationUserName;
                        persisted.PaidDate = DateTime.Now;

                        if (paymentVoucherDTO != null && !_chequeBookAppService.PayVoucher(paymentVoucherDTO, serviceHeader))
                            throw new InvalidOperationException("The payment voucher could not be marked as paid. It may already be paid or flagged.");

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
                        var projection = cashWithdrawalRequest.ProjectedAs<CashWithdrawalRequestDTO>();
                        EnrichRequestDetails(projection, serviceHeader);
                        return projection;
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
