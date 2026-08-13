using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankToMobileRequestAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
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
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;

        public BankToMobileRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<BankToMobileRequest> bankToMobileRequestRepository,
           IBrokerService brokerService,
           ICustomerAccountAppService customerAccountAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           IPostingPeriodAppService postingPeriodAppService,
           IJournalEntryPostingService journalEntryPostingService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (bankToMobileRequestRepository == null)
                throw new ArgumentNullException(nameof(bankToMobileRequestRepository));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _bankToMobileRequestRepository = bankToMobileRequestRepository;
            _brokerService = brokerService;
            _customerAccountAppService = customerAccountAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _postingPeriodAppService = postingPeriodAppService;
            _journalEntryPostingService = journalEntryPostingService;
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

        public BankToMobileRequestDTO RequestPayout(Guid customerAccountId, decimal amount, string accountNumber, int transactionType, ServiceHeader serviceHeader)
        {
            if (customerAccountId == Guid.Empty || amount <= 0m)
                return null;

            var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);

            if (customerAccountDTO == null)
                return null;

            // Withdrawal only makes sense against a Savings/Investment balance - a Loan account's
            // "balance" is what's owed, not available funds; withdrawing from one is a different
            // operation this method deliberately does not attempt.
            if ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode != ProductCode.Savings &&
                (ProductCode)customerAccountDTO.CustomerAccountTypeProductCode != ProductCode.Investment)
                return null;

            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true);
            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader);

            if (amount > customerAccountDTO.AvailableBalance)
                return null;

            var settlementAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.MobileWalletB2CSettlement, serviceHeader);

            var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

            if (settlementAccountId == Guid.Empty || postingPeriod == null)
                return null;

            // Same two-phase shape as MobileToBankRequestAppService.AddNewMobileToBankRequest:
            // the intent row is created/saved in its own scope first, journal posting happens
            // afterward via IJournalEntryPostingService's own scope - not a single atomic
            // transaction across both, matching this codebase's existing C2B posting pattern.
            bool result;
            BankToMobileRequestDTO bankToMobileRequestDTO;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var bankToMobileRequest = BankToMobileRequestFactory.CreateBankToMobileRequest(transactionType, accountNumber, EnumHelper.GetDescription(SystemTransactionCode.BankToMobile), Guid.NewGuid().ToString("N"), amount, null, null, null, null);
                bankToMobileRequest.Status = (byte)BankToMobileRequestStatus.Pending;
                bankToMobileRequest.CreatedBy = serviceHeader.ApplicationUserName;

                _bankToMobileRequestRepository.Add(bankToMobileRequest, serviceHeader);

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                bankToMobileRequestDTO = bankToMobileRequest.ProjectedAs<BankToMobileRequestDTO>();
            }

            if (result && bankToMobileRequestDTO != null)
            {
                // Debit the customer's product G/L (money leaving their balance), Credit the
                // B2C settlement account - the exact reverse direction of MobileToBankRequestAppService's
                // C2B posting, same PerformDoubleEntry primitive.
                var journal = JournalFactory.CreateJournal(null, postingPeriod.Id, customerAccountDTO.BranchId, null, amount, EnumHelper.GetDescription(SystemTransactionCode.BankToMobile), string.Format("{0}~{1}", bankToMobileRequestDTO.Id, accountNumber), accountNumber, 0x9999, (int)SystemTransactionCode.BankToMobile, null, serviceHeader);

                _journalEntryPostingService.PerformDoubleEntry(journal, settlementAccountId, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                result = _journalEntryPostingService.BulkSave(serviceHeader, new List<Journal> { journal });
            }

            return result ? bankToMobileRequestDTO : null;
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
