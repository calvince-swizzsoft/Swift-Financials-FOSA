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
using Domain.MainBoundedContext.ValueObjects;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class CashTransferRequestAppService : ICashTransferRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CashTransferRequest> _cashTransferRequestRepository;
        private readonly IFiscalCountAppService _fiscalCountAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IAuthorizationAppService _authorizationAppService;
        private readonly ITellerAppService _tellerAppService;
        private readonly ITreasuryAppService _treasuryAppService;
        private readonly IJournalAppService _journalAppService;

        public CashTransferRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CashTransferRequest> cashTransferRequestRepository,
           IFiscalCountAppService fiscalCountAppService,
           IPostingPeriodAppService postingPeriodAppService,
           IAuthorizationAppService authorizationAppService,
           ITellerAppService tellerAppService,
           ITreasuryAppService treasuryAppService,
           IJournalAppService journalAppService)
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
            _tellerAppService = tellerAppService ?? throw new ArgumentNullException(nameof(tellerAppService));
            _treasuryAppService = treasuryAppService ?? throw new ArgumentNullException(nameof(treasuryAppService));
            _journalAppService = journalAppService ?? throw new ArgumentNullException(nameof(journalAppService));
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
            if (countedTotal <= 0m) throw new InvalidOperationException("A cash transfer requires a physical denomination count.");
            if (countedTotal != dto.Amount) throw new InvalidOperationException($"Counted denominations ({countedTotal}) do not match the transfer amount ({dto.Amount}).");

            dto.EmployeeId = teller.EmployeeId;
            dto.TotalCredits = teller.TotalCredits;
            dto.TotalDebits = teller.TotalDebits;
            dto.BookBalance = teller.BookBalance;
            dto.OpeningBalance = teller.OpeningBalance;
            dto.ClosingBalance = teller.ClosingBalance;
            dto.TellerCashBalanceStatusValue = dto.Amount == teller.BookBalance ? (int)TellerCashBalanceStatus.Balanced : dto.Amount < teller.BookBalance ? (int)TellerCashBalanceStatus.Shortage : (int)TellerCashBalanceStatus.Excess;

            return await AddNewCashTransferRequestAsync(dto, serviceHeader);
        }

        public async Task<CashTransferRequestDTO> AddNewCashTransferRequestAsync(CashTransferRequestDTO cashTransferRequestDTO, ServiceHeader serviceHeader)
        {
            var cashTransferRequestBindingModel = cashTransferRequestDTO.ProjectedAs<CashTransferRequestBindingModel>();

            cashTransferRequestBindingModel.ValidateAll();

            if (cashTransferRequestBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, cashTransferRequestBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var denomination = new Denomination(
                    cashTransferRequestDTO.DenominationOneThousandValue, cashTransferRequestDTO.DenominationFiveHundredValue,
                    cashTransferRequestDTO.DenominationTwoHundredValue, cashTransferRequestDTO.DenominationOneHundredValue,
                    cashTransferRequestDTO.DenominationFiftyValue, cashTransferRequestDTO.DenominationFourtyValue,
                    cashTransferRequestDTO.DenominationTwentyValue, cashTransferRequestDTO.DenominationTenValue,
                    cashTransferRequestDTO.DenominationFiveValue, cashTransferRequestDTO.DenominationOneValue,
                    cashTransferRequestDTO.DenominationFiftyCentValue);
                var cashTransferRequest = CashTransferRequestFactory.CreateCashTransferRequest(
                    cashTransferRequestDTO.EmployeeId.Value,
                    cashTransferRequestDTO.Amount,
                    cashTransferRequestDTO.Reference,
                    denomination);

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
                var requests = await _cashTransferRequestRepository.GetAllAsync<CashTransferRequestDTO>(serviceHeader);
                EnrichTellerDetails(requests, serviceHeader);
                return requests;
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
                var requests = await _cashTransferRequestRepository.AllMatchingAsync<CashTransferRequestDTO>(filter, serviceHeader);
                EnrichTellerDetails(requests, serviceHeader);
                return requests;
            }
        }

        private void EnrichTellerDetails(IEnumerable<CashTransferRequestDTO> requests, ServiceHeader serviceHeader)
        {
            if (requests == null) return;

            var tellersByEmployee = requests
                .Where(request => request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty)
                .Select(request => request.EmployeeId.Value)
                .Distinct()
                .Select(employeeId => _tellerAppService.FindTellerByEmployeeId(employeeId, serviceHeader))
                .Where(teller => teller != null && teller.EmployeeId.HasValue)
                .ToDictionary(teller => teller.EmployeeId.Value);

            foreach (var request in requests)
            {
                if (!request.EmployeeId.HasValue) continue;

                TellerDTO teller;
                if (!tellersByEmployee.TryGetValue(request.EmployeeId.Value, out teller)) continue;

                request.TellerId = teller.Id;
                request.TellerDescription = teller.Description;
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
            if (cashTransferRequestId == Guid.Empty)
                throw new InvalidOperationException("A cash transfer request is required.");
            if (serviceHeader == null || !serviceHeader.ApplicationUserEmployeeId.HasValue)
                throw new InvalidOperationException("The authenticated teller could not be resolved.");

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _cashTransferRequestRepository.GetAsync(cashTransferRequestId, serviceHeader);
                if (persisted == null)
                    throw new InvalidOperationException("The cash transfer request could not be found.");
                if (persisted.Utilized || persisted.Status == (int)CashTransferRequestStatus.Utilized)
                    throw new InvalidOperationException("The cash transfer request has already been utilized.");
                if (persisted.Status != (int)CashTransferRequestStatus.Acknowledged)
                    throw new InvalidOperationException("Only an acknowledged cash transfer request can be utilized.");
                if (!persisted.EmployeeId.HasValue || persisted.EmployeeId.Value != serviceHeader.ApplicationUserEmployeeId.Value)
                    throw new InvalidOperationException("Only the teller who created this cash transfer request can utilize it.");

                var teller = _tellerAppService.FindTellerByEmployeeId(persisted.EmployeeId.Value, serviceHeader);
                if (teller == null)
                    throw new InvalidOperationException("The originating teller could not be found.");
                if (!teller.ChartOfAccountId.HasValue || teller.ChartOfAccountId.Value == Guid.Empty)
                    throw new InvalidOperationException("The originating teller does not have a cash G/L account configured.");
                if (teller.EmployeeBranchId == Guid.Empty)
                    throw new InvalidOperationException("The originating teller does not have a branch configured.");

                var treasury = _treasuryAppService.FindTreasuryByBranchId(teller.EmployeeBranchId, serviceHeader);
                if (treasury == null || treasury.ChartOfAccountId == Guid.Empty)
                    throw new InvalidOperationException("No treasury cash G/L account is configured for the teller's branch.");

                var tellerLimitError = _tellerAppService.ValidateCashMovement(teller.Id, persisted.Amount, false, serviceHeader);
                if (!string.IsNullOrWhiteSpace(tellerLimitError))
                    throw new InvalidOperationException(tellerLimitError);

                var treasuryLimitError = _treasuryAppService.ValidateCashMovement(
                    treasury.Id,
                    null,
                    persisted.Amount,
                    (int)TreasuryTransactionType.TellerToTreasury,
                    serviceHeader);
                if (!string.IsNullOrWhiteSpace(treasuryLimitError))
                    throw new InvalidOperationException(treasuryLimitError);

                var denomination = persisted.Denomination;
                var countedTotal = denomination == null ? 0m :
                    denomination.OneThousandValue + denomination.FiveHundredValue + denomination.TwoHundredValue +
                    denomination.OneHundredValue + denomination.FiftyValue + denomination.FourtyValue +
                    denomination.TwentyValue + denomination.TenValue + denomination.FiveValue +
                    denomination.OneValue + denomination.FiftyCentValue;

                if (countedTotal > 0m)
                {
                    if (countedTotal != persisted.Amount)
                        throw new InvalidOperationException($"The persisted denomination count ({countedTotal}) does not match the transfer amount ({persisted.Amount}).");

                    var period = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
                    if (period == null)
                        throw new InvalidOperationException("The current posting period could not be resolved.");

                    var fiscalCount = _fiscalCountAppService.AddNewFiscalCount(new FiscalCountDTO
                    {
                        TransactionCode = (int)SystemTransactionCode.TellerCashTransfer,
                        TransactionType = (int)TreasuryTransactionType.TellerToTreasury,
                        PostingPeriodId = period.Id,
                        BranchId = teller.EmployeeBranchId,
                        ChartOfAccountId = teller.ChartOfAccountId.Value,
                        PrimaryDescription = "Teller to Treasury",
                        SecondaryDescription = teller.Description,
                        Reference = persisted.Reference,
                        DenominationOneThousandValue = denomination.OneThousandValue,
                        DenominationFiveHundredValue = denomination.FiveHundredValue,
                        DenominationTwoHundredValue = denomination.TwoHundredValue,
                        DenominationOneHundredValue = denomination.OneHundredValue,
                        DenominationFiftyValue = denomination.FiftyValue,
                        DenominationFourtyValue = denomination.FourtyValue,
                        DenominationTwentyValue = denomination.TwentyValue,
                        DenominationTenValue = denomination.TenValue,
                        DenominationFiveValue = denomination.FiveValue,
                        DenominationOneValue = denomination.OneValue,
                        DenominationFiftyCentValue = denomination.FiftyCentValue
                    }, serviceHeader);
                    if (fiscalCount == null)
                        throw new InvalidOperationException("The official fiscal count could not be recorded.");
                }
                else
                {
                    // Requests created before denomination persistence already wrote their
                    // fiscal count at request time. Reuse that historical count so deployment
                    // does not strand acknowledged requests or create a duplicate count.
                    var legacyCounts = _fiscalCountAppService.FindFiscalCounts(
                        (int)SystemTransactionCode.TellerCashTransfer,
                        persisted.Reference,
                        0,
                        1000,
                        serviceHeader);
                    var hasLegacyCount = legacyCounts?.PageCollection != null && legacyCounts.PageCollection.Any(item =>
                        string.Equals(item.Reference, persisted.Reference, StringComparison.OrdinalIgnoreCase));
                    if (!hasLegacyCount)
                        throw new InvalidOperationException("This legacy request has neither a persisted denomination count nor an existing fiscal count.");
                }

                var journal = _journalAppService.AddNewJournal(
                    null,
                    teller.EmployeeBranchId,
                    null,
                    persisted.Amount,
                    "Teller Cash Transfer",
                    teller.Description,
                    persisted.Reference,
                    0,
                    (int)SystemTransactionCode.TellerCashTransfer,
                    DateTime.Today,
                    teller.ChartOfAccountId.Value,
                    treasury.ChartOfAccountId,
                    serviceHeader,
                    true);
                if (journal == null)
                    throw new InvalidOperationException("The teller-to-treasury journal could not be posted.");

                persisted.Utilized = true;
                persisted.Status = (int)CashTransferRequestStatus.Utilized;

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }
    }
}
