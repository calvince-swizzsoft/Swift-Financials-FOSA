using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashTransferRequestAgg;
using Domain.Seedwork;
using Numero3.EntityFramework.Interfaces;
using System;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Crosscutting.Framework.Adapter;
using Domain.Seedwork.Specification;
using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.AdministrationModule.Services;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class CashTransferRequestAppService : ICashTransferRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CashTransferRequest> _cashTransferRequestRepository;
        private readonly IFiscalCountAppService _fiscalCountAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IAuthorizationAppService _authorizationAppService;

        public CashTransferRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CashTransferRequest> cashTransferRequestRepository,
           IFiscalCountAppService fiscalCountAppService,
           IPostingPeriodAppService postingPeriodAppService,
           IAuthorizationAppService authorizationAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (cashTransferRequestRepository == null)
                throw new ArgumentNullException(nameof(cashTransferRequestRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _cashTransferRequestRepository = cashTransferRequestRepository;
            _fiscalCountAppService = fiscalCountAppService ?? throw new ArgumentNullException(nameof(fiscalCountAppService));
            _postingPeriodAppService = postingPeriodAppService ?? throw new ArgumentNullException(nameof(postingPeriodAppService));
            _authorizationAppService = authorizationAppService ?? throw new ArgumentNullException(nameof(authorizationAppService));
        }

        public async Task<CashTransferRequestDTO> CreateCashTransferAsync(CashTransferRequestDTO dto, TellerDTO teller, ServiceHeader serviceHeader)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (teller == null || !teller.EmployeeId.HasValue) throw new InvalidOperationException("The current teller could not be resolved.");
            if (string.IsNullOrWhiteSpace(dto.Reference)) throw new InvalidOperationException("A transfer reference is required.");

            var countedTotal = dto.DenominationOneThousandValue + dto.DenominationFiveHundredValue +
                dto.DenominationTwoHundredValue + dto.DenominationOneHundredValue + dto.DenominationFiftyValue +
                dto.DenominationFourtyValue + dto.DenominationTwentyValue + dto.DenominationTenValue +
                dto.DenominationFiveValue + dto.DenominationOneValue + dto.DenominationFiftyCentValue;
            if (!dto.TallyByTotal && countedTotal != dto.Amount) throw new InvalidOperationException($"Counted denominations ({countedTotal}) do not match the transfer amount ({dto.Amount}).");

            dto.EmployeeId = teller.EmployeeId;
            dto.TotalCredits = teller.TotalCredits;
            dto.TotalDebits = teller.TotalDebits;
            dto.BookBalance = teller.BookBalance;
            dto.OpeningBalance = teller.OpeningBalance;
            dto.ClosingBalance = teller.ClosingBalance;
            dto.TellerCashBalanceStatusValue = dto.Amount == teller.BookBalance ? (int)TellerCashBalanceStatus.Balanced : dto.Amount < teller.BookBalance ? (int)TellerCashBalanceStatus.Shortage : (int)TellerCashBalanceStatus.Excess;

            var request = await AddNewCashTransferRequestAsync(dto, serviceHeader);
            if (request == null) return null;
            var period = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
            if (period == null) throw new InvalidOperationException("The current posting period could not be resolved.");

            _fiscalCountAppService.AddNewFiscalCount(new FiscalCountDTO {
                TransactionCode = (int)SystemTransactionCode.TellerCashTransfer,
                TransactionType = (int)TreasuryTransactionType.TellerToTreasury,
                PostingPeriodId = period.Id, BranchId = teller.EmployeeBranchId,
                ChartOfAccountId = teller.ChartOfAccountId ?? Guid.Empty,
                PrimaryDescription = "Teller to Treasury", SecondaryDescription = teller.Description,
                Reference = dto.Reference, TotalValue = dto.Amount,
                DenominationOneThousandValue = dto.DenominationOneThousandValue, DenominationFiveHundredValue = dto.DenominationFiveHundredValue,
                DenominationTwoHundredValue = dto.DenominationTwoHundredValue, DenominationOneHundredValue = dto.DenominationOneHundredValue,
                DenominationFiftyValue = dto.DenominationFiftyValue, DenominationFourtyValue = dto.DenominationFourtyValue,
                DenominationTwentyValue = dto.DenominationTwentyValue, DenominationTenValue = dto.DenominationTenValue,
                DenominationFiveValue = dto.DenominationFiveValue, DenominationOneValue = dto.DenominationOneValue,
                DenominationFiftyCentValue = dto.DenominationFiftyCentValue
            }, serviceHeader);
            return request;
        }

        public async Task<CashTransferRequestDTO> AddNewCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, ServiceHeader serviceHeader)
        {
            var cashTransferRequestBindingModel = cashTransferRequestDTO.ProjectedAs<CashTransferRequestBindingModel>();

            cashTransferRequestBindingModel.ValidateAll();

            if (cashTransferRequestBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, cashTransferRequestBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var cashTransferRequest = CashTransferRequestFactory.CreateCashTransferRequest(cashTransferRequestDTO.EmployeeId.Value, cashTransferRequestDTO.Amount, cashTransferRequestDTO.Reference);

                cashTransferRequest.Status = (int)CashTransferRequestStatus.Pending;

                cashTransferRequest.CreatedBy = serviceHeader.ApplicationUserName;

                _cashTransferRequestRepository.Add(cashTransferRequest, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? cashTransferRequest.ProjectedAs<CashTransferRequestDTO>() : null;
            }
        }

        public async Task<bool> AcknowledgeCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, int cashTransferRequestAcknowledgeOption, ServiceHeader serviceHeader)
        {
            EnsureCashTransferAcknowledgementPermission(serviceHeader);
            if (cashTransferRequestDTO == null || cashTransferRequestDTO.Id == Guid.Empty)
                throw new InvalidOperationException("A cash transfer request is required.");
            if (!serviceHeader.ApplicationUserEmployeeId.HasValue || !serviceHeader.ApplicationUserBranchId.HasValue)
                throw new InvalidOperationException("The authenticated cash-management operator must be linked to an employee and branch.");

            var result = default(bool);

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _cashTransferRequestRepository.GetAsync(cashTransferRequestDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)CashTransferRequestStatus.Pending) return result;
                if (persisted.EmployeeId == serviceHeader.ApplicationUserEmployeeId.Value)
                    throw new InvalidOperationException("You cannot acknowledge or reject a cash transfer that you created.");
                if (persisted.Employee == null || persisted.Employee.BranchId != serviceHeader.ApplicationUserBranchId.Value)
                    throw new InvalidOperationException("This cash transfer does not belong to your branch.");

                switch ((CashTransferRequestAcknowledgeOption)cashTransferRequestAcknowledgeOption)
                {
                    case CashTransferRequestAcknowledgeOption.Acknowledge:

                        persisted.Status = (int)CashTransferRequestAcknowledgeOption.Acknowledge;
                        persisted.AcknowledgedDate = DateTime.Now;
                        persisted.Remarks = cashTransferRequestDTO.Remarks;
                        persisted.AcknowledgedBy = serviceHeader.ApplicationUserName;

                        break;

                    case CashTransferRequestAcknowledgeOption.Reject:

                        persisted.Status = (int)CashTransferRequestAcknowledgeOption.Reject;
                        persisted.AcknowledgedDate = DateTime.Now;
                        persisted.Remarks = cashTransferRequestDTO.Remarks;
                        persisted.AcknowledgedBy = serviceHeader.ApplicationUserName;

                        break;
                    default:
                        break;
                }

                result = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }

            return result;
        }

        public async Task<List<CashTransferRequestDTO>> FindCashTransferRequestsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _cashTransferRequestRepository.GetAllAsync<CashTransferRequestDTO>(serviceHeader);
            }
        }

        public async Task<List<CashTransferRequestDTO>> FindActionableCashTransferRequestsAsync(ServiceHeader serviceHeader)
        {
            EnsureCashTransferAcknowledgementPermission(serviceHeader);
            if (!serviceHeader.ApplicationUserEmployeeId.HasValue || !serviceHeader.ApplicationUserBranchId.HasValue)
                throw new InvalidOperationException("The authenticated cash-management operator must be linked to an employee and branch.");

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashTransferRequestSpecifications.PendingCashTransferRequestsForBranch(
                    serviceHeader.ApplicationUserBranchId.Value,
                    serviceHeader.ApplicationUserEmployeeId.Value);
                return await _cashTransferRequestRepository.AllMatchingAsync<CashTransferRequestDTO>(filter, serviceHeader);
            }
        }

        private void EnsureCashTransferAcknowledgementPermission(ServiceHeader serviceHeader)
        {
            if (serviceHeader == null) throw new InvalidOperationException("Authenticated caller context is required.");
            var callerRoles = serviceHeader.ApplicationUserRoles ?? new List<string>();
            var grantedRoles = _authorizationAppService.GetRolesForSystemPermissionType(
                (int)SystemPermissionType.TellerCashTransferAcknowledgement, serviceHeader) ?? new string[0];
            if (!callerRoles.Any(callerRole => grantedRoles.Any(grantedRole =>
                string.Equals(callerRole, grantedRole, StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException("Access denied: your role does not have Teller Cash Transfer Acknowledgement permission.");
        }

        public async Task<PageCollectionInfo<CashTransferRequestDTO>> FindCashTransferRequestsAsync(Guid employeeId, DateTime startDate, DateTime endDate, int status, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashTransferRequestSpecifications.CashTransferRequestWithEmployeeId(employeeId, startDate, endDate, status);

                ISpecification<CashTransferRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _cashTransferRequestRepository.AllMatchingPagedAsync<CashTransferRequestDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }


        public async Task<PageCollectionInfo<CashTransferRequestDTO>> FindAllCashTransferRequestsAsync(DateTime startDate, DateTime endDate, string text, int status, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashTransferRequestSpecifications.CashTransferRequestWithDateRangeStatusAndFullText(startDate, endDate, status, text, customerFilter);

                ISpecification<CashTransferRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _cashTransferRequestRepository.AllMatchingPagedAsync<CashTransferRequestDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<CashTransferRequestDTO> FindCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _cashTransferRequestRepository.GetAsync<CashTransferRequestDTO>(cashTransferRequestId, serviceHeader);
            }
        }

        public async Task<List<CashTransferRequestDTO>> FindMatureCashTransferRequestsAsync(Guid employeeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CashTransferRequestSpecifications.ActionableCashTransferRequestWithEmployeeId(employeeId);

                ISpecification<CashTransferRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _cashTransferRequestRepository.AllMatchingAsync<CashTransferRequestDTO>(spec, serviceHeader);
            }
        }

        public async Task<bool> UtilizeCashTransferRequestAsync(Guid cashTransferRequestId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (cashTransferRequestId != null && cashTransferRequestId != Guid.Empty)
                {
                    var persisted = await _cashTransferRequestRepository.GetAsync(cashTransferRequestId, serviceHeader);

                    if (persisted != null && persisted.Status != (int)CashTransferRequestStatus.Pending)
                    {
                        persisted.Utilized = true;
                        persisted.Status = (int)CashTransferRequestStatus.Utilized;
                    }
                }
                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }
    }
}
