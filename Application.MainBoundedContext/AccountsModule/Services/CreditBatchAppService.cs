using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchDiscrepancyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using KBCsv;
using LazyCache;
using NPOI.SS.Formula.Functions;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class CreditBatchAppService : ICreditBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CreditBatch> _creditBatchRepository;
        private readonly IRepository<CreditBatchEntry> _creditBatchEntryRepository;
        private readonly IRepository<CreditBatchDiscrepancy> _creditBatchDiscrepancyRepository;
        private readonly IRepository<StandingOrder> _standingOrderRepository;
        private readonly IRepository<LoanCase> _loanCaserepository;

        private readonly IRepository<StandingOrderHistory> _standingOrderHistoryRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ICreditTypeAppService _creditTypeAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IRecurringBatchAppService _recurringBatchAppService;
        private readonly IBrokerService _brokerService;
        private readonly IAppCache _appCache;

        public CreditBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CreditBatch> creditBatchRepository,
           IRepository<LoanCase> loancaseRepository,
           IRepository<CreditBatchEntry> creditBatchEntryRepository,
           IRepository<CreditBatchDiscrepancy> creditBatchDiscrepancyRepository,
           IRepository<StandingOrder> standingOrderRepository,
           IRepository<StandingOrderHistory> standingOrderHistoryRepository,
           IPostingPeriodAppService postingPeriodAppService,
           ICreditTypeAppService creditTypeAppService,
           ILoanProductAppService loanProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ISavingsProductAppService savingsProductAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ISqlCommandAppService sqlCommandAppService,
           ICommissionAppService commissionAppService,
           IBranchAppService branchAppService,
           IJournalAppService journalAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           ICustomerAccountAppService customerAccountAppService,
           IRecurringBatchAppService recurringBatchAppService,
           IBrokerService brokerService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (creditBatchRepository == null)
                throw new ArgumentNullException(nameof(creditBatchRepository));

            if (creditBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(creditBatchEntryRepository));

            if (loancaseRepository == null)
                throw new ArgumentNullException(nameof(loancaseRepository));


            if (creditBatchDiscrepancyRepository == null)
                throw new ArgumentNullException(nameof(creditBatchDiscrepancyRepository));

            if (standingOrderRepository == null)
                throw new ArgumentNullException(nameof(standingOrderRepository));

            if (standingOrderHistoryRepository == null)
                throw new ArgumentNullException(nameof(standingOrderHistoryRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (creditTypeAppService == null)
                throw new ArgumentNullException(nameof(creditTypeAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (branchAppService == null)
                throw new ArgumentNullException(nameof(branchAppService));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (recurringBatchAppService == null)
                throw new ArgumentNullException(nameof(recurringBatchAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _creditBatchRepository = creditBatchRepository;
            _creditBatchEntryRepository = creditBatchEntryRepository;
            _creditBatchDiscrepancyRepository = creditBatchDiscrepancyRepository;
            _standingOrderRepository = standingOrderRepository;
            _standingOrderHistoryRepository = standingOrderHistoryRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _creditTypeAppService = creditTypeAppService;
            _loanProductAppService = loanProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _sqlCommandAppService = sqlCommandAppService;
            _commissionAppService = commissionAppService;
            _branchAppService = branchAppService;
            _journalAppService = journalAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
            _recurringBatchAppService = recurringBatchAppService;
            _brokerService = brokerService;
            _appCache = appCache;
            _loanCaserepository = loancaseRepository;
        }

        public CreditBatchDTO AddNewCreditBatch(CreditBatchDTO creditBatchDTO, ServiceHeader serviceHeader)
        {
            if (creditBatchDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var concession = new Charge(creditBatchDTO.ConcessionType, creditBatchDTO.ConcessionPercentage, creditBatchDTO.ConcessionFixedAmount);

                    var creditBatch = CreditBatchFactory.CreateCreditBatch(creditBatchDTO.CreditTypeId, creditBatchDTO.BranchId, creditBatchDTO.PostingPeriodId, creditBatchDTO.TotalValue, creditBatchDTO.Type, creditBatchDTO.Reference, creditBatchDTO.Month, concession, creditBatchDTO.Priority);

                    creditBatch.BatchNumber = _creditBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}CreditBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    creditBatch.ValueDate = creditBatchDTO.ValueDate;
                    creditBatch.EnforceMonthValueDate = creditBatchDTO.EnforceMonthValueDate;
                    creditBatch.Status = (int)BatchStatus.Pending;
                    creditBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    switch ((CreditBatchType)creditBatchDTO.Type)
                    {
                        case CreditBatchType.Payout:
                            creditBatch.RecoverIndefiniteCharges = creditBatchDTO.RecoverIndefiniteCharges;
                            creditBatch.RecoverArrearages = creditBatchDTO.RecoverArrearages;
                            creditBatch.RecoverCarryForwards = creditBatchDTO.RecoverCarryForwards;
                            creditBatch.PreserveAccountBalance = creditBatchDTO.PreserveAccountBalance;
                            break;
                        case CreditBatchType.CheckOff:
                            creditBatch.FuzzyMatching = creditBatchDTO.FuzzyMatching;
                            break;
                        default:
                            break;
                    }

                    _creditBatchRepository.Add(creditBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return creditBatch.ProjectedAs<CreditBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateCreditBatch(CreditBatchDTO creditBatchDTO, ServiceHeader serviceHeader)
        {
            if (creditBatchDTO == null || creditBatchDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _creditBatchRepository.Get(creditBatchDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var concession = new Charge(creditBatchDTO.ConcessionType, creditBatchDTO.ConcessionPercentage, creditBatchDTO.ConcessionFixedAmount);

                    persisted.TotalValue = creditBatchDTO.TotalValue;
                    persisted.Concession = concession;
                    persisted.RecoverIndefiniteCharges = creditBatchDTO.RecoverIndefiniteCharges;
                    persisted.RecoverCarryForwards = creditBatchDTO.RecoverCarryForwards;
                    persisted.RecoverArrearages = creditBatchDTO.RecoverArrearages;
                    persisted.PreserveAccountBalance = creditBatchDTO.PreserveAccountBalance;
                    persisted.FuzzyMatching = creditBatchDTO.FuzzyMatching;
                    persisted.Reference = creditBatchDTO.Reference;
                    persisted.Priority = (byte)creditBatchDTO.Priority;
                    persisted.EnforceMonthValueDate = creditBatchDTO.EnforceMonthValueDate;
                    persisted.ValueDate = creditBatchDTO.ValueDate;

                    dbContextScope.SaveChanges(serviceHeader);

                    var persistedEntriesTotal = _sqlCommandAppService.FindCreditBatchEntriesTotal(persisted.Id, serviceHeader);

                    return persisted.TotalValue >= persistedEntriesTotal;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public bool AuditCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, ServiceHeader serviceHeader)
        {
            if (creditBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _creditBatchRepository.Get(creditBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        var entriesTotal = _sqlCommandAppService.FindCreditBatchEntriesTotal(persisted.Id, serviceHeader);

                        if (persisted.TotalValue >= entriesTotal)
                        {
                            persisted.Status = (int)BatchStatus.Audited;
                            persisted.AuditRemarks = creditBatchDTO.AuditRemarks;
                            persisted.AuditedBy = serviceHeader.ApplicationUserName;
                            persisted.AuditedDate = DateTime.Now;
                        }

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuditRemarks = creditBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        _sqlCommandAppService.DeleteCreditBatchDiscrepancies(persisted.Id, serviceHeader);

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeCreditBatch(CreditBatchDTO creditBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (creditBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _creditBatchRepository.Get(creditBatchDTO.Id, serviceHeader);

                //if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
                //    return result;

                if (persisted == null)
                    return result;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        var entriesTotal = _sqlCommandAppService.FindCreditBatchEntriesTotal(persisted.Id, serviceHeader);

                        if (persisted.TotalValue >= entriesTotal)
                        {
                            persisted.Status = (int)BatchStatus.Posted;
                            persisted.AuthorizationRemarks = creditBatchDTO.AuthorizationRemarks;
                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;
                        }

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuthorizationRemarks = creditBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        _sqlCommandAppService.DeleteCreditBatchDiscrepancies(persisted.Id, serviceHeader);

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                if (result && batchAuthOption == (int)BatchAuthOption.Post && creditBatchDTO.Type.In((int)CreditBatchType.Payout, (int)CreditBatchType.CheckOff))
                {
                    var query = _creditBatchRepository.DatabaseSqlQuery<Guid>(string.Format(
                          @"SELECT Id
                    FROM  {0}CreditBatchEntries
                    WHERE(CreditBatchId = @CreditBatchId)", DefaultSettings.Instance.TablePrefix), serviceHeader,
                            new SqlParameter("CreditBatchId", creditBatchDTO.Id));

                    if (query != null)
                    {
                        var data = from l in query
                                   select new CreditBatchEntryDTO
                                   {
                                       Id = l,
                                       CreditBatchPriority = creditBatchDTO.Priority
                                   };

                        _brokerService.ProcessCreditBatchEntries(DMLCommand.None, serviceHeader, data.ToArray());
                    }
                }
            }

            return result;
        }
        public bool MatchCreditBatchDiscrepancy(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, Guid chartOfAccountId, int moduleNavigationItemCode, int discrepancyAuthOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (chartOfAccountId != null && chartOfAccountId != Guid.Empty)
            {
                var journals = new List<Journal>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _creditBatchDiscrepancyRepository.Get(creditBatchDiscrepancyDTO.Id, serviceHeader);

                    if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                        return false;

                    switch ((CreditBatchDiscrepancyAuthOption)discrepancyAuthOption)
                    {
                        case CreditBatchDiscrepancyAuthOption.Match:

                            #region CreditBatchDiscrepancyAuthOption.Match

                            if (creditBatchDiscrepancyDTO.CreditBatchType.In((int)CreditBatchType.Payout, (int)CreditBatchType.CheckOff))
                            {
                                var amount = default(decimal);

                                if (decimal.TryParse(creditBatchDiscrepancyDTO.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                                {
                                    var creditBatchEntriesTotal = _sqlCommandAppService.FindCreditBatchEntriesTotal(persisted.CreditBatchId, serviceHeader);

                                    if (persisted.CreditBatch.TotalValue >= (creditBatchEntriesTotal + amount))
                                    {
                                        persisted.Status = (int)BatchEntryStatus.Posted;
                                        persisted.PostedBy = serviceHeader.ApplicationUserName;
                                        persisted.PostedDate = DateTime.Now;

                                        var creditBatchEntry = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.CreditBatchId, null, chartOfAccountId, amount, 0m, 0m, creditBatchDiscrepancyDTO.Column2, creditBatchDiscrepancyDTO.Column1);

                                        creditBatchEntry.Status = (int)BatchEntryStatus.Posted;
                                        creditBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                        _creditBatchEntryRepository.Add(creditBatchEntry, serviceHeader);

                                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                                        if (result)
                                        {
                                            var projection = creditBatchEntry.ProjectedAs<CreditBatchEntryDTO>();

                                            var primaryDescription = projection.CreditBatchReference;

                                            var secondaryDescription = string.Format("{0}~{1}", projection.CreditBatchTypeDescription, projection.CreditBatchMonthDescription);

                                            var reference = string.Format("{0}~{1}", projection.PaddedCreditBatchBatchNumber, projection.Reference);

                                            var transactionCode = 0;

                                            switch ((CreditBatchType)projection.CreditBatchType)
                                            {
                                                case CreditBatchType.Payout:
                                                    transactionCode = (int)SystemTransactionCode.CreditBatchPayout;
                                                    break;
                                                case CreditBatchType.CheckOff:
                                                    transactionCode = (int)SystemTransactionCode.CreditBatchCheckOff;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            // Credit CreditBatchEntryDTO.ChartOfAccountId, Debit CreditBatchDTO.CreditTypeChartOfAccountId
                                            var discrepancyJournal = JournalFactory.CreateJournal(null, projection.CreditBatchPostingPeriodId.Value, projection.CreditBatchBranchId, null, amount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, UberUtil.GetLastDayOfMonth(creditBatchDiscrepancyDTO.CreditBatchMonth, creditBatchDiscrepancyDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchDiscrepancyDTO.CreditBatchEnforceMonthValueDate, creditBatchDiscrepancyDTO.CreditBatchValueDate), serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(discrepancyJournal, projection.ChartOfAccountId.Value, projection.CreditBatchCreditTypeChartOfAccountId, serviceHeader);
                                            journals.Add(discrepancyJournal);
                                        }
                                    }
                                    else throw new InvalidOperationException(string.Format("matching {0} would exceed batch total", creditBatchDiscrepancyDTO.Column3));
                                }
                                else throw new InvalidOperationException(string.Format("unable to parse amount {0}.", creditBatchDiscrepancyDTO.Column3));
                            }
                            else throw new InvalidOperationException(string.Format("invalid batch type {0}.", creditBatchDiscrepancyDTO.CreditBatchTypeDescription));

                            #endregion

                            break;
                        case CreditBatchDiscrepancyAuthOption.Reject:

                            #region CreditBatchDiscrepancyAuthOption.Reject

                            persisted.Status = (int)BatchEntryStatus.Rejected;
                            persisted.PostedBy = serviceHeader.ApplicationUserName;
                            persisted.PostedDate = DateTime.Now;

                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                            #endregion

                            break;
                        default:
                            break;
                    }
                }

                if (result && journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
            }

            return result;
        }

        public bool MatchCreditBatchDiscrepancy(CreditBatchDiscrepancyDTO creditBatchDiscrepancyDTO, CustomerAccountDTO customerAccountDTO, int discrepancyAuthOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (creditBatchDiscrepancyDTO != null && customerAccountDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _creditBatchDiscrepancyRepository.Get(creditBatchDiscrepancyDTO.Id, serviceHeader);

                    if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                        return false;

                    switch ((CreditBatchDiscrepancyAuthOption)discrepancyAuthOption)
                    {
                        case CreditBatchDiscrepancyAuthOption.Match:

                            #region CreditBatchDiscrepancyAuthOption.Match

                            if (creditBatchDiscrepancyDTO.CreditBatchType.In((int)CreditBatchType.Payout, (int)CreditBatchType.CheckOff))
                            {
                                switch ((CreditBatchType)creditBatchDiscrepancyDTO.CreditBatchType)
                                {
                                    case CreditBatchType.Payout:

                                        var amount = default(decimal);

                                        if (decimal.TryParse(creditBatchDiscrepancyDTO.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                                        {
                                            var customerSavingsAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndPayrollNumber(customerAccountDTO.CustomerAccountTypeTargetProductId, creditBatchDiscrepancyDTO.Column1, serviceHeader);

                                            if (customerSavingsAccounts.Any())
                                            {
                                                if (customerSavingsAccounts.Count == 1)
                                                {
                                                    if (customerSavingsAccounts[0].Id == customerAccountDTO.Id)
                                                    {
                                                        var creditBatchEntriesTotal = _sqlCommandAppService.FindCreditBatchEntriesTotal(persisted.CreditBatchId, serviceHeader);

                                                        if (persisted.CreditBatch.TotalValue >= (creditBatchEntriesTotal + amount))
                                                        {
                                                            persisted.Status = (int)BatchEntryStatus.Posted;
                                                            persisted.PostedBy = serviceHeader.ApplicationUserName;
                                                            persisted.PostedDate = DateTime.Now;

                                                            var creditBatchEntry = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.CreditBatchId, customerAccountDTO.Id, null, amount, 0m, 0m, creditBatchDiscrepancyDTO.Column2, creditBatchDiscrepancyDTO.Column1);

                                                            creditBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                                            creditBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                                            _creditBatchEntryRepository.Add(creditBatchEntry, serviceHeader);

                                                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                                                            if (result)
                                                            {
                                                                var projection = creditBatchEntry.ProjectedAs<CreditBatchEntryDTO>();

                                                                _brokerService.ProcessCreditBatchEntries(DMLCommand.None, serviceHeader, projection);
                                                            }
                                                        }
                                                        else throw new InvalidOperationException(string.Format("matching {0} would exceed batch total", creditBatchDiscrepancyDTO.Column3));
                                                    }
                                                    else throw new InvalidOperationException("mismatch between expected account and proposed account");
                                                }
                                                else throw new InvalidOperationException(string.Format("found {0} customer account matches.", customerSavingsAccounts.Count()));
                                            }
                                            else throw new InvalidOperationException(string.Format("no match for savings product customer account."));
                                        }
                                        else throw new InvalidOperationException(string.Format("unable to parse amount {0}.", creditBatchDiscrepancyDTO.Column3));

                                        break;
                                    case CreditBatchType.CheckOff:

                                        var contributionAmount = default(decimal);

                                        var productBalance = default(decimal);

                                        if (decimal.TryParse(creditBatchDiscrepancyDTO.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out contributionAmount) && decimal.TryParse(creditBatchDiscrepancyDTO.Column4, NumberStyles.Any, CultureInfo.InvariantCulture, out productBalance))
                                        {
                                            var creditBatchEntriesTotal = _sqlCommandAppService.FindCreditBatchEntriesTotal(persisted.CreditBatchId, serviceHeader);

                                            if (persisted.CreditBatch.TotalValue >= (creditBatchEntriesTotal + contributionAmount))
                                            {
                                                switch ((CheckOffEntryType)Enum.Parse(typeof(CheckOffEntryType), creditBatchDiscrepancyDTO.Column7))
                                                {
                                                    case CheckOffEntryType.sLoan:

                                                        #region sLoan

                                                        var customerLoanPrincipalAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(customerAccountDTO.CustomerAccountTypeTargetProductId, creditBatchDiscrepancyDTO.Column2, serviceHeader);

                                                        if (customerLoanPrincipalAccounts.Any())
                                                        {
                                                            if (customerLoanPrincipalAccounts.Count == 1)
                                                            {
                                                                if (customerLoanPrincipalAccounts[0].Id == customerAccountDTO.Id)
                                                                {
                                                                    persisted.Status = (int)BatchEntryStatus.Posted;
                                                                    persisted.PostedBy = serviceHeader.ApplicationUserName;
                                                                    persisted.PostedDate = DateTime.Now;

                                                                    var sLoanCreditBatchEntry = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.CreditBatchId, customerAccountDTO.Id, null, contributionAmount, 0m, productBalance, creditBatchDiscrepancyDTO.Column5, creditBatchDiscrepancyDTO.Column2);

                                                                    sLoanCreditBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                                                    sLoanCreditBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                                                    _creditBatchEntryRepository.Add(sLoanCreditBatchEntry, serviceHeader);

                                                                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                                                                    if (result)
                                                                    {
                                                                        var projection = sLoanCreditBatchEntry.ProjectedAs<CreditBatchEntryDTO>();

                                                                        _brokerService.ProcessCreditBatchEntries(DMLCommand.None, serviceHeader, projection);
                                                                    }
                                                                }
                                                                else throw new InvalidOperationException("mismatch between expected account and proposed account");
                                                            }
                                                            else throw new InvalidOperationException(string.Format("found {0} customer account matches.", customerLoanPrincipalAccounts.Count()));
                                                        }
                                                        else throw new InvalidOperationException(string.Format("no match for loan product customer account."));

                                                        #endregion

                                                        break;

                                                    case CheckOffEntryType.sInterest:

                                                        #region sInterest

                                                        var customerLoanInterestAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(customerAccountDTO.CustomerAccountTypeTargetProductId, creditBatchDiscrepancyDTO.Column2, serviceHeader);

                                                        if (customerLoanInterestAccounts.Any())
                                                        {
                                                            if (customerLoanInterestAccounts.Count == 1)
                                                            {
                                                                if (customerLoanInterestAccounts[0].Id == customerAccountDTO.Id)
                                                                {
                                                                    persisted.Status = (int)BatchEntryStatus.Posted;
                                                                    persisted.PostedBy = serviceHeader.ApplicationUserName;
                                                                    persisted.PostedDate = DateTime.Now;

                                                                    var sInterestCreditBatchEntry = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.CreditBatchId, customerAccountDTO.Id, null, 0m, contributionAmount, productBalance, creditBatchDiscrepancyDTO.Column5, creditBatchDiscrepancyDTO.Column2);

                                                                    sInterestCreditBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                                                    sInterestCreditBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                                                    _creditBatchEntryRepository.Add(sInterestCreditBatchEntry, serviceHeader);

                                                                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                                                                    if (result)
                                                                    {
                                                                        var projection = sInterestCreditBatchEntry.ProjectedAs<CreditBatchEntryDTO>();

                                                                        _brokerService.ProcessCreditBatchEntries(DMLCommand.None, serviceHeader, projection);
                                                                    }
                                                                }
                                                                else throw new InvalidOperationException("mismatch between expected account and proposed account");
                                                            }
                                                            else throw new InvalidOperationException(string.Format("found {0} customer account matches.", customerLoanInterestAccounts.Count()));
                                                        }
                                                        else throw new InvalidOperationException(string.Format("no match for loan product customer account."));

                                                        #endregion

                                                        break;

                                                    case CheckOffEntryType.sShare:
                                                    case CheckOffEntryType.wCont:
                                                    case CheckOffEntryType.sInvest:
                                                    case CheckOffEntryType.sRisk:
                                                    case CheckOffEntryType.wLoan:

                                                        #region sShare/wCont/sInvest/sRisk/wLoan

                                                        var customerInvestmentAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(customerAccountDTO.CustomerAccountTypeTargetProductId, creditBatchDiscrepancyDTO.Column2, serviceHeader);

                                                        if (customerInvestmentAccounts.Any())
                                                        {
                                                            if (customerInvestmentAccounts.Count == 1)
                                                            {
                                                                if (customerInvestmentAccounts[0].Id == customerAccountDTO.Id)
                                                                {
                                                                    persisted.Status = (int)BatchEntryStatus.Posted;
                                                                    persisted.PostedBy = serviceHeader.ApplicationUserName;
                                                                    persisted.PostedDate = DateTime.Now;

                                                                    var sShareCreditBatchEntry = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.CreditBatchId, customerAccountDTO.Id, null, contributionAmount, 0m, productBalance, creditBatchDiscrepancyDTO.Column5, creditBatchDiscrepancyDTO.Column2);

                                                                    sShareCreditBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                                                    sShareCreditBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                                                    _creditBatchEntryRepository.Add(sShareCreditBatchEntry, serviceHeader);

                                                                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                                                                    if (result)
                                                                    {
                                                                        var projection = sShareCreditBatchEntry.ProjectedAs<CreditBatchEntryDTO>();

                                                                        _brokerService.ProcessCreditBatchEntries(DMLCommand.None, serviceHeader, projection);
                                                                    }
                                                                }
                                                                else throw new InvalidOperationException("mismatch between expected account and proposed account");
                                                            }
                                                            else throw new InvalidOperationException(string.Format("found {0} customer account matches.", customerInvestmentAccounts.Count()));
                                                        }
                                                        else throw new InvalidOperationException(string.Format("no match for investment product customer account."));

                                                        #endregion

                                                        break;

                                                    case CheckOffEntryType.sLoanInterest:

                                                        #region sLoanInterest

                                                        var customerLoanAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(customerAccountDTO.CustomerAccountTypeTargetProductId, creditBatchDiscrepancyDTO.Column2, serviceHeader);

                                                        if (customerLoanAccounts.Any())
                                                        {
                                                            if (customerLoanAccounts.Count == 1)
                                                            {
                                                                if (customerLoanAccounts[0].Id == customerAccountDTO.Id)
                                                                {
                                                                    var targetLoanProduct = _loanProductAppService.FindLoanProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                                                                    customerAccountDTO.PrincipalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, DateTime.Now, serviceHeader);

                                                                    customerAccountDTO.InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, DateTime.Now, serviceHeader);

                                                                    customerAccountDTO.BookBalance = customerAccountDTO.PrincipalBalance + customerAccountDTO.InterestBalance;

                                                                    switch ((AggregateCheckOffRecoveryMode)targetLoanProduct.LoanRegistrationAggregateCheckOffRecoveryMode)
                                                                    {
                                                                        case AggregateCheckOffRecoveryMode.OutstandingBalance:

                                                                            // reset expected interest & expected principal >> NB: interest has priority over principal!
                                                                            var actualLoanInterestOutstandingBalance = Math.Min(Math.Abs(customerAccountDTO.InterestBalance), contributionAmount);

                                                                            var actualLoanPrincipalOutstandingBalance = contributionAmount - actualLoanInterestOutstandingBalance;

                                                                            persisted.Status = (int)BatchEntryStatus.Posted;
                                                                            persisted.PostedBy = serviceHeader.ApplicationUserName;
                                                                            persisted.PostedDate = DateTime.Now;

                                                                            var sLoanInterest_OutstandingBalance = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.CreditBatchId, customerAccountDTO.Id, null, actualLoanPrincipalOutstandingBalance, actualLoanInterestOutstandingBalance, customerAccountDTO.BookBalance, creditBatchDiscrepancyDTO.Column5, creditBatchDiscrepancyDTO.Column2);

                                                                            sLoanInterest_OutstandingBalance.Status = (int)BatchEntryStatus.Pending;
                                                                            sLoanInterest_OutstandingBalance.CreatedBy = serviceHeader.ApplicationUserName;

                                                                            _creditBatchEntryRepository.Add(sLoanInterest_OutstandingBalance, serviceHeader);

                                                                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                                                                            if (result)
                                                                            {
                                                                                var projection = sLoanInterest_OutstandingBalance.ProjectedAs<CreditBatchEntryDTO>();

                                                                                _brokerService.ProcessCreditBatchEntries(DMLCommand.None, serviceHeader, projection);
                                                                            }

                                                                            break;
                                                                        case AggregateCheckOffRecoveryMode.StandingOrder:

                                                                            var standingOrders = FindStandingOrdersByBeneficiaryCustomerAccountId(customerAccountDTO.Id, (int)StandingOrderTrigger.CheckOff, serviceHeader);

                                                                            if (standingOrders != null && standingOrders.Any(x => !x.IsLocked))
                                                                            {
                                                                                if (standingOrders.Count == 1)
                                                                                {
                                                                                    var targetStandingOrder = standingOrders[0];

                                                                                    // reset expected interest & expected principal >> NB: interest has priority over principal!
                                                                                    var actualLoanInterestStandingOrder = Math.Min(Math.Abs(targetStandingOrder.Interest), contributionAmount);

                                                                                    var actualLoanPrincipalStandingOrder = contributionAmount - actualLoanInterestStandingOrder;

                                                                                    persisted.Status = (int)BatchEntryStatus.Posted;
                                                                                    persisted.PostedBy = serviceHeader.ApplicationUserName;
                                                                                    persisted.PostedDate = DateTime.Now;

                                                                                    var sLoanInterest_StandingOrder = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.CreditBatchId, customerAccountDTO.Id, null, actualLoanPrincipalStandingOrder, actualLoanInterestStandingOrder, customerAccountDTO.BookBalance, creditBatchDiscrepancyDTO.Column5, creditBatchDiscrepancyDTO.Column2);

                                                                                    sLoanInterest_StandingOrder.Status = (int)BatchEntryStatus.Pending;
                                                                                    sLoanInterest_StandingOrder.CreatedBy = serviceHeader.ApplicationUserName;

                                                                                    _creditBatchEntryRepository.Add(sLoanInterest_StandingOrder, serviceHeader);

                                                                                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                                                                                    if (result)
                                                                                    {
                                                                                        var projection = sLoanInterest_StandingOrder.ProjectedAs<CreditBatchEntryDTO>();

                                                                                        _brokerService.ProcessCreditBatchEntries(DMLCommand.None, serviceHeader, projection);
                                                                                    }
                                                                                }
                                                                                else throw new InvalidOperationException(string.Format("found {0} beneficiary product standing order matches.", standingOrders.Count()));
                                                                            }
                                                                            else throw new InvalidOperationException(string.Format("no match for beneficiary product standing order."));

                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                                else throw new InvalidOperationException("mismatch between expected account and proposed account");
                                                            }
                                                            else throw new InvalidOperationException(string.Format("found {0} customer account matches.", customerLoanAccounts.Count()));
                                                        }
                                                        else throw new InvalidOperationException(string.Format("no match for loan product customer account."));

                                                        #endregion

                                                        break;

                                                    default:
                                                        break;
                                                }
                                            }
                                            else throw new InvalidOperationException(string.Format("matching {0} would exceed batch total", creditBatchDiscrepancyDTO.Column3));
                                        }
                                        else throw new InvalidOperationException(string.Format("unable to parse amount(s) {0}/{1}.", creditBatchDiscrepancyDTO.Column3, creditBatchDiscrepancyDTO.Column4));

                                        break;
                                    default:
                                        break;
                                }
                            }
                            else throw new InvalidOperationException(string.Format("invalid batch type {0}.", creditBatchDiscrepancyDTO.CreditBatchTypeDescription));

                            #endregion

                            break;
                        case CreditBatchDiscrepancyAuthOption.Reject:

                            #region CreditBatchDiscrepancyAuthOption.Reject

                            persisted.Status = (int)BatchEntryStatus.Rejected;
                            persisted.PostedBy = serviceHeader.ApplicationUserName;
                            persisted.PostedDate = DateTime.Now;

                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                            #endregion

                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        public bool MatchCreditBatchDiscrepancies(List<CreditBatchDiscrepancyDTO> creditBatchDiscrepancyDTOs, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (creditBatchDiscrepancyDTOs != null && creditBatchDiscrepancyDTOs.Any() && customerAccountDTO != null)
            {
                creditBatchDiscrepancyDTOs.ForEach(creditBatchDiscrepancyDTO =>
                {
                    result = MatchCreditBatchDiscrepancy(creditBatchDiscrepancyDTO, customerAccountDTO, (int)CreditBatchDiscrepancyAuthOption.Match, serviceHeader);
                });
            }

            return result;
        }

        public CreditBatchEntryDTO AddNewCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (creditBatchEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var creditBatchEntry = CreditBatchEntryFactory.CreateCreditBatchEntry(creditBatchEntryDTO.CreditBatchId, creditBatchEntryDTO.CustomerAccountId, creditBatchEntryDTO.ChartOfAccountId, creditBatchEntryDTO.Principal, creditBatchEntryDTO.Interest, creditBatchEntryDTO.Balance, creditBatchEntryDTO.Beneficiary, creditBatchEntryDTO.Reference);

                    creditBatchEntry.Status = (int)BatchEntryStatus.Pending;
                    creditBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _creditBatchEntryRepository.Add(creditBatchEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return creditBatchEntry.ProjectedAs<CreditBatchEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveCreditBatchEntries(List<CreditBatchEntryDTO> creditBatchEntryDTOs, ServiceHeader serviceHeader)
        {
            if (creditBatchEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in creditBatchEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _creditBatchEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _creditBatchEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool UpdateCreditBatchEntry(CreditBatchEntryDTO creditBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (creditBatchEntryDTO == null || creditBatchEntryDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _creditBatchEntryRepository.Get(creditBatchEntryDTO.Id, serviceHeader);

                if (persisted != null && persisted.Status < creditBatchEntryDTO.Status/*status flags can only go up?*/)
                {
                    persisted.Status = (byte)creditBatchEntryDTO.Status;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool PostCreditBatchEntry(Guid creditBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (MarkCreditBatchEntryPosted(creditBatchEntryId, serviceHeader))
            {
                var creditBatchEntryDTO = FindCreditBatchEntry(creditBatchEntryId, serviceHeader);
                if (creditBatchEntryDTO == null || creditBatchEntryDTO.Status != (int)BatchEntryStatus.Posted)
                    return result;

                var creditBatchDTO = FindCachedCreditBatch(creditBatchEntryDTO.CreditBatchId, serviceHeader);
                if (creditBatchDTO == null)
                    return result;

                var postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(creditBatchEntryDTO.CreditBatchPostingPeriodId ?? Guid.Empty, serviceHeader) ?? _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);
                if (postingPeriodDTO == null)
                    return result;

                serviceHeader.ApplicationUserName = creditBatchDTO.AuthorizedBy ?? serviceHeader.ApplicationUserName;

                var primaryDescription = creditBatchDTO.Reference;

                var secondaryDescription = string.Format("{0}~{1}", creditBatchDTO.TypeDescription, creditBatchDTO.MonthDescription);

                var reference = creditBatchEntryDTO.Reference;

                var journals = new List<Journal>();

                var standingOrderHistories = new List<StandingOrderHistory>();

                var customerAccountCarryForwards = new List<CustomerAccountCarryForward>();

                var customerAccountArrearages = new List<CustomerAccountArrearage>();

                var transactionOwnershipBranchId = Guid.Empty;

                switch ((CreditBatchType)creditBatchDTO.Type)
                {
                    case CreditBatchType.Payout:

                        #region Savings Payout (e.g salary)

                        var payoutCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(creditBatchEntryDTO.CustomerAccountId ?? Guid.Empty, serviceHeader);

                        switch ((TransactionOwnership)creditBatchDTO.CreditTypeTransactionOwnership)
                        {
                            case TransactionOwnership.InitiatingBranch:
                                transactionOwnershipBranchId = creditBatchDTO.BranchId;
                                break;
                            case TransactionOwnership.BeneficiaryBranch:
                                transactionOwnershipBranchId = payoutCustomerAccount.BranchId;
                                break;
                            default:
                                break;
                        }

                        var targetSavingsProduct = _savingsProductAppService.FindCachedSavingsProduct(payoutCustomerAccount.CustomerAccountTypeTargetProductId, payoutCustomerAccount.BranchId, serviceHeader);

                        var payoutTariffs = _commissionAppService.ComputeTariffsByPayoutCreditType(creditBatchDTO.CreditTypeId, (creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest), payoutCustomerAccount, serviceHeader);

                        var primaryJournal = _journalAppService.AddNewJournal(transactionOwnershipBranchId, null, (creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest), primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), targetSavingsProduct.ChartOfAccountId, creditBatchDTO.CreditTypeChartOfAccountId, payoutCustomerAccount, payoutCustomerAccount, payoutTariffs, serviceHeader);

                        if (primaryJournal != null)
                        {
                            var availableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(payoutCustomerAccount, DateTime.Now, serviceHeader);

                            if (creditBatchDTO.PreserveAccountBalance) /*/ to mean we are only using the credit batch entry amount /*/
                            {
                                var minimumBalance = availableBalance > targetSavingsProduct.MinimumBalance ? 0m : targetSavingsProduct.MinimumBalance;

                                // reset available balance
                                availableBalance = (creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest) - (payoutTariffs.Sum(x => x.Amount) + minimumBalance);
                            }

                            // Recover dues for attached products
                            if ((availableBalance > 0m /*/ Will current account balance be positive ?/*/))
                            {
                                // track deductions
                                var totalRecoveryDeductions = 0m;

                                // recover concession-exempt loans
                                var concessionExemptTuple = RecoverAttachedConcessionExemptLoans(transactionOwnershipBranchId, primaryJournal.Id, postingPeriodDTO.Id, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, standingOrderHistories, customerAccountCarryForwards, customerAccountArrearages, payoutCustomerAccount, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                                // reset deductions
                                totalRecoveryDeductions = concessionExemptTuple.Item1;

                                // reset available balance
                                availableBalance = concessionExemptTuple.Item2;

                                if (creditBatchDTO.PreserveAccountBalance) /*/ to mean we are only using the credit batch entry amount /*/
                                {
                                    // work out concession
                                    /* this is the maximum amount to be used for recovery - 23.06.2017
                                     */

                                    var maximumAmountAvailabeForRecovery = 0m;

                                    switch ((ChargeType)creditBatchDTO.ConcessionType)
                                    {
                                        case ChargeType.Percentage:
                                            maximumAmountAvailabeForRecovery = Math.Round(Convert.ToDecimal(((100d - creditBatchDTO.ConcessionPercentage) * Convert.ToDouble(creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest)) / 100), 4, MidpointRounding.AwayFromZero);
                                            break;
                                        case ChargeType.FixedAmount:
                                            maximumAmountAvailabeForRecovery = (creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest) - creditBatchDTO.ConcessionFixedAmount;
                                            break;
                                        default:
                                            break;
                                    }

                                    // reset available balance
                                    availableBalance = Math.Min(maximumAmountAvailabeForRecovery, availableBalance);
                                }

                                if ((availableBalance > 0m /*/ Will current account balance be positive ?/*/))
                                {
                                    var branchDTO = _branchAppService.FindCachedBranch(payoutCustomerAccount.BranchId, serviceHeader);

                                    if (branchDTO != null && !string.IsNullOrWhiteSpace(branchDTO.CompanyRecoveryPriority))
                                    {
                                        var buffer = branchDTO.CompanyRecoveryPriority.Split(new char[] { ',' });

                                        Array.ForEach(buffer, (item) =>
                                        {
                                            switch ((RecoveryPriority)Enum.Parse(typeof(RecoveryPriority), item))
                                            {
                                                case RecoveryPriority.Loans:

                                                    // do loans recovery
                                                    var loansTuple = RecoverAttachedLoans(transactionOwnershipBranchId, primaryJournal.Id, postingPeriodDTO.Id, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, standingOrderHistories, customerAccountCarryForwards, customerAccountArrearages, payoutCustomerAccount, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                                                    // reset deductions
                                                    totalRecoveryDeductions = loansTuple.Item1;

                                                    // reset available balance
                                                    availableBalance = loansTuple.Item2;

                                                    break;
                                                case RecoveryPriority.Investments:

                                                    // do investments recovery
                                                    var investmentsTuple = RecoverAttachedInvestments(transactionOwnershipBranchId, primaryJournal.Id, postingPeriodDTO.Id, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, customerAccountArrearages, standingOrderHistories, payoutCustomerAccount, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                                                    // reset deductions
                                                    totalRecoveryDeductions = investmentsTuple.Item1;

                                                    // reset available balance
                                                    availableBalance = investmentsTuple.Item2;

                                                    break;
                                                case RecoveryPriority.Savings:

                                                    // do savings recovery
                                                    var savingsTuple = RecoverAttachedSavings(transactionOwnershipBranchId, primaryJournal.Id, postingPeriodDTO.Id, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, standingOrderHistories, payoutCustomerAccount, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                                                    // reset deductions
                                                    totalRecoveryDeductions = savingsTuple.Item1;

                                                    // reset available balance
                                                    availableBalance = savingsTuple.Item2;

                                                    break;
                                                case RecoveryPriority.DirectDebits:

                                                    // do direct debits recovery
                                                    var directDebitsTuple = RecoverAttachedDirectDebits(transactionOwnershipBranchId, primaryJournal.Id, postingPeriodDTO.Id, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, payoutCustomerAccount, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                                                    // reset deductions
                                                    totalRecoveryDeductions = directDebitsTuple.Item1;

                                                    // reset available balance
                                                    availableBalance = directDebitsTuple.Item2;

                                                    break;
                                                default:
                                                    break;
                                            }
                                        });
                                    }
                                }
                            }
                        }

                        #endregion

                        break;
                    case CreditBatchType.CheckOff:

                        #region Loan/Investment

                        var checkOffCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(creditBatchEntryDTO.CustomerAccountId ?? Guid.Empty, serviceHeader);

                        _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { checkOffCustomerAccount }, serviceHeader, true);

                        switch ((TransactionOwnership)creditBatchDTO.CreditTypeTransactionOwnership)
                        {
                            case TransactionOwnership.InitiatingBranch:
                                transactionOwnershipBranchId = creditBatchDTO.BranchId;
                                break;
                            case TransactionOwnership.BeneficiaryBranch:
                                transactionOwnershipBranchId = checkOffCustomerAccount.BranchId;
                                break;
                            default:
                                break;
                        }

                        if (creditBatchEntryDTO.ChartOfAccountId != null && creditBatchEntryDTO.ChartOfAccountId != Guid.Empty)
                        {
                            // Credit CreditBatchEntryDTO.ChartOfAccountId, Debit CreditBatchDTO.CreditTypeChartOfAccountId
                            var checkOffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, transactionOwnershipBranchId, null, creditBatchEntryDTO.Principal, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchCheckOff, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(checkOffJournal, creditBatchEntryDTO.ChartOfAccountId.Value, creditBatchDTO.CreditTypeChartOfAccountId, checkOffCustomerAccount, checkOffCustomerAccount, serviceHeader);
                            journals.Add(checkOffJournal);
                        }
                        else
                        {
                            switch ((ProductCode)checkOffCustomerAccount.CustomerAccountTypeProductCode)
                            {
                                case ProductCode.Loan:

                                    #region Loan Check-Off

                                    var targetLoanProduct = _loanProductAppService.FindCachedLoanProduct(checkOffCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);


                                    var PrimaryDescriptionintrest = "Loan Interest Payment";
                                    var secoSecondaryDescriptionintrest = "Loan Repayment CheckOff Interest";

                                    var principledescription = " Loan Principal Repayment";
                                    var PrincipleSecondarydescription = "Check Off Principal Repayment";



                                    // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit CreditBatchDTO.CreditTypeChartOfAccountId
                                    var interestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, transactionOwnershipBranchId, null, creditBatchEntryDTO.Interest, PrimaryDescriptionintrest, secoSecondaryDescriptionintrest, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchCheckOff, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(interestReceivableJournal, targetLoanProduct.InterestReceivableChartOfAccountId, creditBatchDTO.CreditTypeChartOfAccountId, checkOffCustomerAccount, checkOffCustomerAccount, serviceHeader);
                                    journals.Add(interestReceivableJournal);

                                    //// Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                                    //var interestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, transactionOwnershipBranchId, null, creditBatchEntryDTO.Interest, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchCheckOff, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    //_journalEntryPostingService.PerformDoubleEntry(interestReceivedJournal, targetLoanProduct.InterestReceivedChartOfAccountId, targetLoanProduct.InterestChargedChartOfAccountId, checkOffCustomerAccount, checkOffCustomerAccount, serviceHeader);
                                    //journals.Add(interestReceivedJournal);

                                    // Credit LoanProduct.ChartOfAccountId, Debit CreditBatchDTO.CreditTypeChartOfAccountId
                                    var principalJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, transactionOwnershipBranchId, null, creditBatchEntryDTO.Principal, principledescription, PrincipleSecondarydescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchCheckOff, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(principalJournal, targetLoanProduct.ChartOfAccountId, creditBatchDTO.CreditTypeChartOfAccountId, checkOffCustomerAccount, checkOffCustomerAccount, serviceHeader);
                                    journals.Add(principalJournal);

                                    var loanStandingOrders = FindStandingOrdersByBeneficiaryCustomerAccountId(checkOffCustomerAccount.Id, (int)StandingOrderTrigger.CheckOff, serviceHeader);

                                    if (loanStandingOrders != null && loanStandingOrders.Any(x => !x.IsLocked))
                                    {
                                        if (loanStandingOrders.Count == 1)
                                        {
                                            var targetLoanStandingOrder = loanStandingOrders[0];

                                            var principalArrears = 0m;

                                            var interestArrears = 0m;

                                            if (targetLoanProduct.LoanRegistrationTrackArrears)
                                            {
                                                var principalBalance = checkOffCustomerAccount.PrincipalBalance + creditBatchEntryDTO.Principal;

                                                var interestBalance = checkOffCustomerAccount.InterestBalance + creditBatchEntryDTO.Interest;

                                                principalArrears = Math.Min(((principalBalance * -1 > 0m) ? (principalBalance * -1) : 0m), checkOffCustomerAccount.PrincipalArrearagesBalance * -1 < 0m ? checkOffCustomerAccount.PrincipalArrearagesBalance : 0m);

                                                interestArrears = Math.Min(((interestBalance * -1 > 0m) ? (interestBalance * -1) : 0m), checkOffCustomerAccount.InterestArrearagesBalance * -1 < 0m ? checkOffCustomerAccount.InterestArrearagesBalance : 0m);

                                                if ((principalArrears + interestArrears) > 0m)
                                                {
                                                    // cumulate
                                                    interestArrears += (creditBatchEntryDTO.Interest - interestArrears);
                                                    principalArrears += (creditBatchEntryDTO.Principal - principalArrears);
                                                }
                                                else
                                                {
                                                    // track new
                                                    interestArrears = targetLoanStandingOrder.Interest - creditBatchEntryDTO.Interest;
                                                    principalArrears = targetLoanStandingOrder.Principal - creditBatchEntryDTO.Principal;
                                                }

                                                interestArrears = interestArrears * -1 > 0m ? 0m : interestArrears;
                                                principalArrears = principalArrears * -1 > 0m ? 0m : principalArrears;
                                            }

                                            var history = StandingOrderHistoryFactory.CreateStandingOrderHistory(targetLoanStandingOrder.Id, postingPeriodDTO.Id, targetLoanStandingOrder.BenefactorCustomerAccountId, targetLoanStandingOrder.BeneficiaryCustomerAccountId, new Duration(targetLoanStandingOrder.DurationStartDate, targetLoanStandingOrder.DurationEndDate), new Schedule(targetLoanStandingOrder.ScheduleFrequency, targetLoanStandingOrder.ScheduleExpectedRunDate, targetLoanStandingOrder.ScheduleActualRunDate, targetLoanStandingOrder.ScheduleExecuteAttemptCount, targetLoanStandingOrder.ScheduleForceExecute), new Charge(targetLoanStandingOrder.ChargeType, targetLoanStandingOrder.ChargePercentage, targetLoanStandingOrder.ChargeFixedAmount), creditBatchEntryDTO.CreditBatchMonth, targetLoanStandingOrder.Trigger, targetLoanStandingOrder.Principal, targetLoanStandingOrder.Interest, creditBatchEntryDTO.Principal, creditBatchEntryDTO.Interest, targetLoanStandingOrder.Remarks);
                                            history.CreatedBy = serviceHeader.ApplicationUserName;
                                            standingOrderHistories.Add(history);

                                            // Do we need to update arrearage history?
                                            var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(checkOffCustomerAccount.Id, (int)ArrearageCategory.Interest, interestArrears, reference);
                                            customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                            customerAccountArrearages.Add(customerAccountInterestArrearage);

                                            var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(checkOffCustomerAccount.Id, (int)ArrearageCategory.Principal, principalArrears, reference);
                                            customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                            customerAccountArrearages.Add(customerAccountPrincipalArrearage);
                                        }
                                    }

                                    #endregion

                                    break;
                                //we will use savings not investments
                                case ProductCode.Savings:

                                    #region Investment Check-Off

                                    //var targetInvestmentProduct = _investmentProductAppService.FindCachedInvestmentProduct(checkOffCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);
                                    var targetInvestmentProduct = _savingsProductAppService.FindCachedSavingsProduct(checkOffCustomerAccount.CustomerAccountTypeTargetProductId, checkOffCustomerAccount.BranchId, serviceHeader);
                                    secondaryDescription = string.Format("{0} ({1})", secondaryDescription, targetInvestmentProduct.Description);

                                    // Investment Check-Off Journal: Credit InvestmentProduct.ChartOfAccountId, Debit SystemGeneralLedgerAccountCode.CommonControl
                                    var investmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, transactionOwnershipBranchId, null, creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchCheckOff, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(investmentJournal, targetInvestmentProduct.ChartOfAccountId, creditBatchDTO.CreditTypeChartOfAccountId, checkOffCustomerAccount, checkOffCustomerAccount, serviceHeader);
                                    journals.Add(investmentJournal);

                                    var investmentStandingOrders = FindStandingOrdersByBeneficiaryCustomerAccountId(checkOffCustomerAccount.Id, (int)StandingOrderTrigger.CheckOff, serviceHeader);

                                    if (investmentStandingOrders != null && investmentStandingOrders.Any(x => !x.IsLocked))
                                    {
                                        if (investmentStandingOrders.Count == 1)
                                        {
                                            var targetInvestmentStandingOrder = investmentStandingOrders[0];

                                            var principalArrears = 0m;

                                            var interestArrears = 0m;

                                            //if (targetInvestmentProduct.TrackArrears)
                                            //{
                                            //    principalArrears = checkOffCustomerAccount.PrincipalArrearagesBalance * -1 < 0m ? checkOffCustomerAccount.PrincipalArrearagesBalance : 0m;

                                            //    interestArrears = checkOffCustomerAccount.InterestArrearagesBalance * -1 < 0m ? checkOffCustomerAccount.InterestArrearagesBalance : 0m;

                                            //    if ((principalArrears + interestArrears) > 0m)
                                            //    {
                                            //        // cumulate
                                            //        interestArrears += (creditBatchEntryDTO.Interest - interestArrears);
                                            //        principalArrears += (creditBatchEntryDTO.Principal - principalArrears);
                                            //    }
                                            //    else
                                            //    {
                                            //        // track new
                                            //        interestArrears = targetInvestmentStandingOrder.Interest - creditBatchEntryDTO.Interest;
                                            //        principalArrears = targetInvestmentStandingOrder.Principal - creditBatchEntryDTO.Principal;
                                            //    }

                                            //    interestArrears = interestArrears * -1 > 0m ? 0m : interestArrears;
                                            //    principalArrears = principalArrears * -1 > 0m ? 0m : principalArrears;
                                            //}

                                            var history = StandingOrderHistoryFactory.CreateStandingOrderHistory(targetInvestmentStandingOrder.Id, postingPeriodDTO.Id, targetInvestmentStandingOrder.BenefactorCustomerAccountId, targetInvestmentStandingOrder.BeneficiaryCustomerAccountId, new Duration(targetInvestmentStandingOrder.DurationStartDate, targetInvestmentStandingOrder.DurationEndDate), new Schedule(targetInvestmentStandingOrder.ScheduleFrequency, targetInvestmentStandingOrder.ScheduleExpectedRunDate, targetInvestmentStandingOrder.ScheduleActualRunDate, targetInvestmentStandingOrder.ScheduleExecuteAttemptCount, targetInvestmentStandingOrder.ScheduleForceExecute), new Charge(targetInvestmentStandingOrder.ChargeType, targetInvestmentStandingOrder.ChargePercentage, targetInvestmentStandingOrder.ChargeFixedAmount), creditBatchEntryDTO.CreditBatchMonth, targetInvestmentStandingOrder.Trigger, targetInvestmentStandingOrder.Principal, targetInvestmentStandingOrder.Interest, creditBatchEntryDTO.Principal, creditBatchEntryDTO.Interest, targetInvestmentStandingOrder.Remarks);
                                            history.CreatedBy = serviceHeader.ApplicationUserName;
                                            standingOrderHistories.Add(history);

                                            // Do we need to update arrearage history?
                                            var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(checkOffCustomerAccount.Id, (int)ArrearageCategory.Interest, interestArrears, reference);
                                            customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                            customerAccountArrearages.Add(customerAccountInterestArrearage);

                                            var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(checkOffCustomerAccount.Id, (int)ArrearageCategory.Principal, principalArrears, reference);
                                            customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                            customerAccountArrearages.Add(customerAccountPrincipalArrearage);
                                        }
                                    }

                                    #endregion

                                    break;
                                default:
                                    break;
                            }
                        }

                        #endregion

                        break;
                    case CreditBatchType.CashPickup:
                    case CreditBatchType.SundryPayments:

                        #region Flag True

                        result = true;

                        #endregion

                        break;
                    default:
                        break;
                }

                #region Bulk-Insert journals 

                result = _journalEntryPostingService.BulkSave(serviceHeader, journals, customerAccountCarryForwards, standingOrderHistories, customerAccountArrearages);

                #endregion

                #region  trigger arrears recovery?

                if (result && creditBatchEntryDTO.CreditBatchType.In((int)CreditBatchType.Payout) && creditBatchEntryDTO.CreditBatchRecoverArrearages)
                {
                    var creditTypeAttachedProducts = _creditTypeAppService.FindCachedAttachedProducts(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

                    if (creditTypeAttachedProducts != null)
                    {
                        _recurringBatchAppService.RecoverArrears(creditBatchEntryDTO, creditTypeAttachedProducts.LoanProductCollection, (int)QueuePriority.High, serviceHeader);
                    }
                }

                #endregion
            }

            return result;
        }
        public List<CreditBatchDTO> FindCreditBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var creditBatches = _creditBatchRepository.GetAll(serviceHeader);

                if (creditBatches != null && creditBatches.Any())
                {
                    return creditBatches.ProjectedAsCollection<CreditBatchDTO>();
                }
                else return null;
            }
        }


        public PageCollectionInfo<CreditBatchDTO> FindCreditBatches(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditBatchSpecifications.CreditBatchFullText(text);

                ISpecification<CreditBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditBatchCollection = _creditBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditBatchCollection != null)
                {
                    var pageCollection = creditBatchCollection.PageCollection.ProjectedAsCollection<CreditBatchDTO>();

                    var itemsCount = creditBatchCollection.ItemsCount;

                    return new PageCollectionInfo<CreditBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }


        public PageCollectionInfo<CreditBatchDTO> FindCreditBatches(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditBatchSpecifications.DefaultSpec();

                ISpecification<CreditBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditBatchPagedCollection = _creditBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditBatchPagedCollection != null)
                {
                    var pageCollection = creditBatchPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchDTO>();

                    var itemsCount = creditBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CreditBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }



        public PageCollectionInfo<CreditBatchDTO> FindCreditBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditBatchSpecifications.CreditBatchesWithStatus(status, startDate, endDate, text);

                ISpecification<CreditBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditBatchPagedCollection = _creditBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditBatchPagedCollection != null)
                {
                    var pageCollection = creditBatchPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _creditBatchEntryRepository.AllMatchingCount(CreditBatchEntrySpecifications.CreditBatchEntryWithCreditBatchId(item.Id, null), serviceHeader);

                            var postedItems = _creditBatchEntryRepository.AllMatchingCount(CreditBatchEntrySpecifications.PostedCreditBatchEntryWithCreditBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = creditBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CreditBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }
        public CreditBatchDTO FindCreditBatch(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            if (creditBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var creditBatch = _creditBatchRepository.Get(creditBatchId, serviceHeader);

                    if (creditBatch != null)
                    {
                        return creditBatch.ProjectedAs<CreditBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public CreditBatchEntryDTO FindCreditBatchEntry(Guid creditBatchEntryId, ServiceHeader serviceHeader)
        {
            if (creditBatchEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var creditBatchEntry = _creditBatchEntryRepository.Get(creditBatchEntryId, serviceHeader);

                    if (creditBatchEntry != null)
                    {
                        return creditBatchEntry.ProjectedAs<CreditBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public CreditBatchDTO FindCachedCreditBatch(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<CreditBatchDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, creditBatchId.ToString("D")), () =>
            {
                return FindCreditBatch(creditBatchId, serviceHeader);
            });
        }

        public List<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchId(Guid creditBatchId, ServiceHeader serviceHeader)
        {
            if (creditBatchId != null && creditBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditBatchEntrySpecifications.CreditBatchEntryWithCreditBatchId(creditBatchId, null, (int)CreditBatchEntryFilter.Reference);

                    ISpecification<CreditBatchEntry> spec = filter;

                    var creditBatchEntries = _creditBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (creditBatchEntries != null && creditBatchEntries.Any())
                    {
                        return creditBatchEntries.ProjectedAsCollection<CreditBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchId(Guid creditBatchId, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (creditBatchId != null && creditBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditBatchEntrySpecifications.CreditBatchEntryWithCreditBatchId(creditBatchId, text, creditBatchEntryFilter);

                    ISpecification<CreditBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var creditBatchEntryPagedCollection = _creditBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (creditBatchEntryPagedCollection != null)
                    {
                        var persisted = _creditBatchRepository.Get(creditBatchId, serviceHeader);

                        var persistedEntriesTotal = _sqlCommandAppService.FindCreditBatchEntriesTotal(persisted.Id, serviceHeader);

                        var pageCollection = creditBatchEntryPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchEntryDTO>();

                        var itemsCount = creditBatchEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<CreditBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount, TotalApportioned = persistedEntriesTotal, TotalShortage = persisted.TotalValue - persistedEntriesTotal };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CreditBatchEntryDTO> FindCreditBatchEntriesByCreditBatchType(int creditBatchType, DateTime startDate, DateTime endDate, string text, int creditBatchEntryFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditBatchEntrySpecifications.CreditBatchEntryWithDateRangeAndCreditBatchType(startDate, endDate, creditBatchType, text, creditBatchEntryFilter);

                ISpecification<CreditBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditBatchEntryPagedCollection = _creditBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditBatchEntryPagedCollection != null)
                {
                    var pageCollection = creditBatchEntryPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchEntryDTO>();

                    var itemsCount = creditBatchEntryPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CreditBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepanciesByCreditBatchId(Guid creditBatchId, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (creditBatchId != null && creditBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditBatchDiscrepancySpecifications.CreditBatchDiscrepancyWithCreditBatchId(creditBatchId, text, creditBatchDiscrepancyFilter);

                    ISpecification<CreditBatchDiscrepancy> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var creditBatchDiscrepancyPagedCollection = _creditBatchDiscrepancyRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (creditBatchDiscrepancyPagedCollection != null)
                    {
                        var pageCollection = creditBatchDiscrepancyPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchDiscrepancyDTO>();

                        var itemsCount = creditBatchDiscrepancyPagedCollection.ItemsCount;

                        return new PageCollectionInfo<CreditBatchDiscrepancyDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepancies(int status, DateTime startDate, DateTime endDate, string text, int creditBatchDiscrepancyFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditBatchDiscrepancySpecifications.CreditBatchDiscrepancyWithDateRange(status, startDate, endDate, text, creditBatchDiscrepancyFilter);

                ISpecification<CreditBatchDiscrepancy> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditBatchDiscrepancyPagedCollection = _creditBatchDiscrepancyRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditBatchDiscrepancyPagedCollection != null)
                {
                    var pageCollection = creditBatchDiscrepancyPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchDiscrepancyDTO>();

                    var itemsCount = creditBatchDiscrepancyPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CreditBatchDiscrepancyDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CreditBatchDiscrepancyDTO> FindCreditBatchDiscrepancies(int creditBatchType, int status, int productCode, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditBatchDiscrepancySpecifications.CreditBatchDiscrepancyWithCreditBatchTypeAndStatusAndProductCode(creditBatchType, status, productCode, text);

                ISpecification<CreditBatchDiscrepancy> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditBatchDiscrepancyPagedCollection = _creditBatchDiscrepancyRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditBatchDiscrepancyPagedCollection != null)
                {
                    var pageCollection = creditBatchDiscrepancyPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchDiscrepancyDTO>();

                    var itemsCount = creditBatchDiscrepancyPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CreditBatchDiscrepancyDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }


        public List<CreditBatchEntryDTO> FindCreditBatchEntriesByCustomerId(int creditBatchType, Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditBatchEntrySpecifications.CreditBatchEntryWithCreditBatchTypeAndCustomerId(creditBatchType, customerId);

                    ISpecification<CreditBatchEntry> spec = filter;

                    var creditBatchEntries = _creditBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (creditBatchEntries != null && creditBatchEntries.Any())
                    {
                        return creditBatchEntries.ProjectedAsCollection<CreditBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CreditBatchEntryDTO> FindLoanAppraisalCreditBatchEntriesByCustomerId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            var loanProduct = _loanProductAppService.FindLoanProduct(loanProductId, serviceHeader);

            if (loanProduct != null)
            {
                var creditTypes = _creditTypeAppService.FindCreditTypesByAttachedProductId(loanProductId, serviceHeader);

                if (creditTypes != null && creditTypes.Any())
                {
                    var creditBatchEntryDTOsList = new List<CreditBatchEntryDTO>();

                    var endDate = UberUtil.GetLastDayOfMonth(DateTime.Today);

                    using (_dbContextScopeFactory.CreateReadOnly())
                    {
                        var startDate = endDate.AddMonths((int)loanProduct.LoanRegistrationConsecutiveIncome/*consecutive months*/ * -1);

                        startDate = startDate.AddMonths(-1);

                        startDate = UberUtil.GetFirstDayOfMonth(startDate);

                        var creditTypeIds = (from c in creditTypes
                                             select c.Id).ToArray();

                        var filter = CreditBatchEntrySpecifications.CreditBatchEntryWithDateRangeAndCreditTypeIdAndCustomerId((int)CreditBatchType.Payout, customerId, (int)BatchEntryStatus.Posted, startDate, endDate, creditTypeIds);

                        ISpecification<CreditBatchEntry> spec = filter;

                        var creditBatchEntries = _creditBatchEntryRepository.AllMatching(spec, serviceHeader);

                        if (creditBatchEntries != null && creditBatchEntries.Any())
                        {
                            creditBatchEntryDTOsList.AddRange(creditBatchEntries.ProjectedAsCollection<CreditBatchEntryDTO>());
                        }
                    }

                    return creditBatchEntryDTOsList;
                }
                else return null;
            }
            else return null;
        }

        public PageCollectionInfo<CreditBatchEntryDTO> FindQueableCreditBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditBatchEntrySpecifications.QueableCreditBatchEntries((int)CreditBatchType.Payout, (int)CreditBatchType.CheckOff);

                ISpecification<CreditBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditBatchPagedCollection = _creditBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditBatchPagedCollection != null)
                {
                    var pageCollection = creditBatchPagedCollection.PageCollection.ProjectedAsCollection<CreditBatchEntryDTO>();

                    var itemsCount = creditBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CreditBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<BatchImportEntryWrapper> ParseCreditBatchImport(Guid creditBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var persisted = _creditBatchRepository.Get(creditBatchId, serviceHeader);

                if (persisted != null && persisted.Status == (int)BatchStatus.Pending && !string.IsNullOrWhiteSpace(fileUploadDirectory) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var path = Path.Combine(fileUploadDirectory, fileName);

                    if (System.IO.File.Exists(path))
                    {
                        var importEntries = new List<BatchImportEntryWrapper> { };

                        using (var streamReader = new StreamReader(path))
                        using (var reader = new CsvReader(streamReader))
                        {
                            // the CSV file has a header record, so we read that first
                            reader.ReadHeaderRecord();

                            while (reader.HasMoreRecords)
                            {
                                var dataRecord = reader.ReadDataRecord();

                                switch ((CreditBatchType)persisted.Type)
                                {
                                    case CreditBatchType.Payout:

                                        if (dataRecord.Count == 5)
                                        {
                                            var payoutEntry = new BatchImportEntryWrapper
                                            {
                                                Column1 = dataRecord[0], //Payroll Number
                                                Column2 = dataRecord[1], //Customer Name
                                                Column3 = dataRecord[2], //Amount
                                                Column4 = dataRecord[3], //Savings Product Credit Code
                                                Column5 = dataRecord[4], //YourReference
                                            };

                                            importEntries.Add(payoutEntry);
                                        }

                                        break;
                                    case CreditBatchType.CheckOff:


                                        //if (dataRecord.Count == 8)
                                        if (dataRecord.Count == 6)
                                        {
                                            var checkOffEntry = new BatchImportEntryWrapper
                                            {
                                                Column1 = dataRecord[0], //Ministry Department -- Payroll
                                                Column2 = dataRecord[1], //Payroll Number -- Beneficiary 
                                                Column3 = dataRecord[2], //Contribution -- Contribution
                                                Column4 = dataRecord[3], //Product Balance -- Loan/Investment Product Credit Code  
                                                Column5 = dataRecord[4], //Beneficiary -- Type (sInterest, sLoan, sShare, wCont)
                                                Column6 = dataRecord[5], //Loan/Investment Product Credit Code --Reference
                                                                         //Column7 = dataRecord[6], //Type (sInterest,sLoan,sShare,wCont)
                                                                         //Column8 = dataRecord[7], //YourReference
                                            };

                                            importEntries.Add(checkOffEntry);
                                        }

                                        break;
                                    case CreditBatchType.CashPickup:

                                        if (dataRecord.Count == 4)
                                        {
                                            var payoutEntry = new BatchImportEntryWrapper
                                            {
                                                Column1 = dataRecord[0], //Beneficiary
                                                Column2 = dataRecord[1], //Reference
                                                Column3 = dataRecord[2], //Amount
                                                Column4 = dataRecord[3], //YourReference
                                            };

                                            importEntries.Add(payoutEntry);
                                        }

                                        break;
                                    case CreditBatchType.SundryPayments:

                                        if (dataRecord.Count == 4)
                                        {
                                            var payoutEntry = new BatchImportEntryWrapper
                                            {
                                                Column1 = dataRecord[0], //G/L Account Code
                                                Column2 = dataRecord[1], //Reference
                                                Column3 = dataRecord[2], //Amount
                                                Column4 = dataRecord[3], //YourReference
                                            };

                                            importEntries.Add(payoutEntry);
                                        }

                                        break;

                                    default:
                                        break;
                                }
                            }
                        }

                        if (importEntries.Any())
                        {
                            BatchImportParseInfo parseInfo = null;

                            switch ((CreditBatchType)persisted.Type)
                            {
                                case CreditBatchType.Payout:
                                    parseInfo = ParsePayout(importEntries, serviceHeader);
                                    break;
                                case CreditBatchType.CheckOff:
                                    parseInfo = persisted.FuzzyMatching ? ParseCheckOff_Fuzzy(persisted, importEntries, serviceHeader) : ParseCheckOff(persisted, importEntries, serviceHeader);
                                    break;
                                case CreditBatchType.CashPickup:
                                    parseInfo = ParseCashPickup(importEntries);
                                    break;
                                case CreditBatchType.SundryPayments:
                                    parseInfo = ParseSundryPayments(importEntries, serviceHeader);
                                    break;
                                default:
                                    break;
                            }

                            if (parseInfo != null)
                            {
                                UpdateCreditBatchEntries(creditBatchId, parseInfo.MatchedCollection1, serviceHeader);

                                if (persisted.Type.In((int)CreditBatchType.Payout, (int)CreditBatchType.CheckOff))
                                {
                                    var discrepancies = new List<CreditBatchDiscrepancyDTO>();

                                    foreach (var item in parseInfo.MismatchedCollection)
                                    {
                                        discrepancies.Add(new CreditBatchDiscrepancyDTO
                                        {
                                            Column1 = item.Column1,
                                            Column2 = item.Column2,
                                            Column3 = item.Column3,
                                            Column4 = item.Column4,
                                            Column5 = item.Column5,
                                            Column6 = item.Column6,
                                            Column7 = item.Column7,
                                            Column8 = item.Column8,
                                            Remarks = item.Remarks,
                                        });
                                    }

                                    UpdateCreditBatchDiscrepancies(creditBatchId, discrepancies, serviceHeader);
                                }

                                return parseInfo.MismatchedCollection;
                            }
                            else return null;
                        }
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
        }
        private BatchImportParseInfo ParsePayout(List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection1 = new List<CreditBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var savingsProducts = _savingsProductAppService.FindSavingsProducts(serviceHeader);

            var count = 0;

            importEntries.ForEach(item =>
            {
                var amount = default(decimal);

                if (decimal.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                {
                    var productCode = default(int);

                    if (int.TryParse(item.Column4, out productCode))
                    {
                        var matchedSavingsProducts = savingsProducts.Where(x => x.Code == productCode);

                        if (matchedSavingsProducts != null && matchedSavingsProducts.Any() && matchedSavingsProducts.Count() == 1)
                        {
                            var targetSavingsProduct = matchedSavingsProducts.First();

                            var customerSavingsAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndPayrollNumber(targetSavingsProduct.Id, item.Column1, serviceHeader);

                            if (customerSavingsAccounts.Any())
                            {
                                if (customerSavingsAccounts.Count == 1)
                                {
                                    var targetCustomerAccount = customerSavingsAccounts[0];

                                    CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                                    creditBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                                    creditBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                    creditBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                    creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                    creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                    creditBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                    creditBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                    creditBatchEntry.Beneficiary = item.Column2;
                                    creditBatchEntry.ProductDescription = targetSavingsProduct.Description;
                                    creditBatchEntry.Principal = amount;
                                    creditBatchEntry.Reference = item.Column1;

                                    result.MatchedCollection1.Add(creditBatchEntry);
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches by payroll number {2}", count, customerSavingsAccounts.Count(), item.Column1);

                                    result.MismatchedCollection.Add(item);
                                }
                            }
                            else
                            {
                                item.Remarks = string.Format("Record #{0} ~ no match for savings product customer account by payroll number {1}", count, item.Column1);

                                result.MismatchedCollection.Add(item);
                            }
                        }
                        else
                        {
                            item.Remarks = string.Format("Record #{0} ~ no match for savings product code {1}", count, item.Column4);

                            result.MismatchedCollection.Add(item);
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ unable to parse product code {1}", count, item.Column4);

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse amount {1}", count, item.Column3);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }
        private BatchImportParseInfo ParseCheckOff(CreditBatch creditBatch, List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection1 = new List<CreditBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader);

            var investmentProducts = _investmentProductAppService.FindInvestmentProducts(serviceHeader);

            var savingProducts = _savingsProductAppService.FindSavingsProducts(serviceHeader);

            var count = 0;

            importEntries.ForEach(async item =>
            {
                var contributionAmount = default(decimal);

                var productBalance = default(decimal);

                //if (decimal.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out contributionAmount) && decimal.TryParse(item.Column4, NumberStyles.Any, CultureInfo.InvariantCulture, out productBalance))
                if (decimal.TryParse(item.Column2, NumberStyles.Any, CultureInfo.InvariantCulture, out contributionAmount))
                {
                    var productCode = default(int);

                    if (int.TryParse(item.Column4, out productCode))
                    {
                        switch ((CheckOffEntryType)Enum.Parse(typeof(CheckOffEntryType), item.Column5))
                        {
                            case CheckOffEntryType.sLoan:

                                #region sLoan

                                var sLoan_MatchedLoanProducts = loanProducts.Where(x => x.Code == productCode);

                                if (sLoan_MatchedLoanProducts != null && sLoan_MatchedLoanProducts.Any() && sLoan_MatchedLoanProducts.Count() == 1)
                                {
                                    var targetLoanPrincipalProduct = sLoan_MatchedLoanProducts.First();

                                    var customerLoanPrincipalAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(targetLoanPrincipalProduct.Id, item.Column1, serviceHeader);

                                    if (customerLoanPrincipalAccounts.Any())
                                    {
                                        if (customerLoanPrincipalAccounts.Count == 1)
                                        {
                                            var targetCustomerAccount = customerLoanPrincipalAccounts[0];

                                            var existingEntries = from b in result.MatchedCollection1
                                                                  where b.CustomerAccountId == targetCustomerAccount.Id
                                                                  select b;

                                            if (existingEntries != null && existingEntries.Any())
                                            {
                                                if (existingEntries.Count() == 1)
                                                {
                                                    existingEntries.Single().Principal += contributionAmount;
                                                    existingEntries.Single().Balance += productBalance;
                                                }
                                                else
                                                {
                                                    item.Remarks = string.Format("Record #{0} ~ (sLoan) >> Existing entry couldn't be matched!", count);

                                                    result.MismatchedCollection.Add(item);
                                                }
                                            }
                                            else
                                            {
                                                // Get the first loan case for the customer (without await)
                                                var loanCasesList = _loanCaserepository
                                                    .GetAllAsync(serviceHeader)
                                                    .Result;

                                                var now = DateTime.Now; // or DateTime.UtcNow if you prefer UTC

                                                var loanCase = loanCasesList
                                                    .Where(l => l.CustomerId == targetCustomerAccount.CustomerId && l.DisbursedDate.HasValue)
                                                    .OrderBy(l => Math.Abs((l.DisbursedDate.Value - now).TotalSeconds))
                                                    .FirstOrDefault();




                                                if (loanCase != null && targetLoanPrincipalProduct != null)
                                                {

                                                    // ================== OPENING BALANCE ==================
                                                    decimal principalBalance = loanCase.TotalLoansBalance;

                                                    // ================== MONTHLY INTEREST RATE ==================
                                                    decimal monthlyRate = (decimal)targetLoanPrincipalProduct.LoanInterestAnnualPercentageRate / 100m / 12m;

                                                    // ================== INTEREST FOR CONTRIBUTION ==================
                                                    decimal interestAmount = Math.Round(contributionAmount * monthlyRate, 2, MidpointRounding.AwayFromZero);

                                                    // ================== PRINCIPAL PORTION ==================
                                                    decimal principalPortion = contributionAmount - interestAmount;

                                                    // ================== NEW BALANCE ==================
                                                    decimal newBalance = principalBalance - principalPortion;

                                                    // ================== CHECK FIRST MONTH / INTEREST ALREADY CHARGED ==================
                                                    bool isFirstMonth = loanCase.DisbursedDate.HasValue &&
                                                                        loanCase.DisbursedDate.Value.Year == now.Year &&
                                                                        loanCase.DisbursedDate.Value.Month == now.Month;

                                                    if (isFirstMonth)
                                                    {
                                                        // Skip interest in first month, entire payment goes to principal
                                                        principalPortion = contributionAmount;
                                                        interestAmount = 0;
                                                        newBalance = principalBalance - principalPortion;
                                                    }

                                                    // ================== POPULATE DTO ==================
                                                    CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO
                                                    {
                                                        CustomerAccountId = targetCustomerAccount.Id,
                                                        CustomerAccountBranchId = targetCustomerAccount.BranchId,
                                                        CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode,
                                                        CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId,
                                                        CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode,
                                                        CustomerAccountCustomerId = targetCustomerAccount.CustomerId,
                                                        CustomerAccountCustomerIndividualPayrollNumbers = item.Column1,
                                                        Beneficiary = item.Column3,
                                                        ProductDescription = targetLoanPrincipalProduct.Description,
                                                        Principal = principalPortion,
                                                        Interest = interestAmount,
                                                        Balance = newBalance,
                                                        Reference = loanCase.CaseNumber.ToString(),
                                                    };

                                                    result.MatchedCollection1.Add(creditBatchEntry);

                                                }
                                            }


                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches by personal file number {2}", count, customerLoanPrincipalAccounts.Count(), item.Column1);

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ no match for loan product customer account by personal file number {1}", count, item.Column1);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for loan product", count);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sInterest:

                                #region sInterest

                                var sInterest_MatchedLoanProducts = loanProducts.Where(x => x.Code == productCode);

                                if (sInterest_MatchedLoanProducts != null && sInterest_MatchedLoanProducts.Any() && sInterest_MatchedLoanProducts.Count() == 1)
                                {
                                    var targetLoanInterestProduct = sInterest_MatchedLoanProducts.First();

                                    var customerLoanInterestAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(targetLoanInterestProduct.Id, item.Column1, serviceHeader);

                                    if (customerLoanInterestAccounts.Any())
                                    {
                                        if (customerLoanInterestAccounts.Count == 1)
                                        {
                                            var targetCustomerAccount = customerLoanInterestAccounts[0];

                                            var existingEntries = from b in result.MatchedCollection1
                                                                  where b.CustomerAccountId == targetCustomerAccount.Id
                                                                  select b;

                                            if (existingEntries != null && existingEntries.Any())
                                            {
                                                if (existingEntries.Count() == 1)
                                                {
                                                    existingEntries.Single().Interest += contributionAmount;
                                                    existingEntries.Single().Balance += productBalance;
                                                }
                                                else
                                                {
                                                    item.Remarks = string.Format("Record #{0} ~ (sInterest) >> Existing entry couldn't be matched!", count);

                                                    result.MismatchedCollection.Add(item);
                                                }
                                            }
                                            else
                                            {

                                                CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                                                creditBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                                                creditBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                creditBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                creditBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                creditBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                                creditBatchEntry.Beneficiary = item.Column3;
                                                creditBatchEntry.ProductDescription = targetLoanInterestProduct.Description;
                                                creditBatchEntry.Interest = contributionAmount;
                                                creditBatchEntry.Balance = productBalance;
                                                creditBatchEntry.Reference = item.Column1;

                                                result.MatchedCollection1.Add(creditBatchEntry);
                                            }
                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches by personal file number {2}", count, customerLoanInterestAccounts.Count(), item.Column1);

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ no match for loan product customer account by personal file number {1}", count, item.Column1);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for loan product", count);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sShare:
                            case CheckOffEntryType.wCont:
                            case CheckOffEntryType.sInvest:
                            case CheckOffEntryType.sRisk:
                            case CheckOffEntryType.wLoan:

                                #region sShare/wCont/sInvest/sRisk/wLoan

                                //fix to use savingProducts not investmentProducts
                                //var matchedInvestmentProducts = investmentProducts.Where(x => x.Code == productCode);


                                var matchedInvestmentProducts = savingProducts.Where(x => x.Code == productCode);



                                if (matchedInvestmentProducts != null && matchedInvestmentProducts.Any() && matchedInvestmentProducts.Count() == 1)
                                {
                                    var targetInvestmentProduct = matchedInvestmentProducts.First();

                                    var customerInvestmentAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(targetInvestmentProduct.Id, item.Column1, serviceHeader);

                                    if (customerInvestmentAccounts.Any())
                                    {
                                        if (customerInvestmentAccounts.Count == 1)
                                        {
                                            var targetCustomerAccount = customerInvestmentAccounts[0];

                                            CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                                            creditBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                                            creditBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                            creditBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                            creditBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                            creditBatchEntry.Beneficiary = item.Column3;
                                            creditBatchEntry.ProductDescription = targetInvestmentProduct.Description;
                                            creditBatchEntry.Principal = contributionAmount;
                                            creditBatchEntry.Balance = productBalance;
                                            creditBatchEntry.Reference = item.Column1;

                                            result.MatchedCollection1.Add(creditBatchEntry);
                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches by personal file number {2}", count, customerInvestmentAccounts.Count(), item.Column1);

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ no match for investment product customer account by personal file number {1}", count, item.Column1);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for investment product", count);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sLoanInterest:

                                #region sLoanInterest

                                var matchedLoanProducts = loanProducts.Where(x => x.Code == productCode);

                                if (matchedLoanProducts != null && matchedLoanProducts.Any() && matchedLoanProducts.Count() == 1)
                                {
                                    var targetLoanProduct = matchedLoanProducts.First();

                                    var customerLoanAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndReference3(targetLoanProduct.Id, item.Column1, serviceHeader);

                                    if (customerLoanAccounts.Any())
                                    {
                                        if (customerLoanAccounts.Count == 1)
                                        {
                                            var targetCustomerAccount = customerLoanAccounts[0];

                                            targetCustomerAccount.PrincipalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(targetCustomerAccount, 1, DateTime.Now, serviceHeader);
                                            targetCustomerAccount.PrincipalBalance = targetCustomerAccount.PrincipalBalance * -1 > 0m ? targetCustomerAccount.PrincipalBalance : 0m;

                                            targetCustomerAccount.InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(targetCustomerAccount, 2, DateTime.Now, serviceHeader);
                                            targetCustomerAccount.InterestBalance = targetCustomerAccount.InterestBalance * -1 > 0m ? targetCustomerAccount.InterestBalance : 0m;

                                            targetCustomerAccount.BookBalance = targetCustomerAccount.PrincipalBalance + targetCustomerAccount.InterestBalance;

                                            var checkOffTariffs = _commissionAppService.ComputeTariffsByCheckOffCreditType(creditBatch.CreditTypeId, contributionAmount, creditBatch.CreditType.ChartOfAccountId, serviceHeader);

                                            contributionAmount -= checkOffTariffs.Sum(x => x.Amount);

                                            switch ((AggregateCheckOffRecoveryMode)targetLoanProduct.LoanRegistrationAggregateCheckOffRecoveryMode)
                                            {
                                                case AggregateCheckOffRecoveryMode.OutstandingBalance:

                                                    if (checkOffTariffs != null && checkOffTariffs.Any())
                                                    {
                                                        checkOffTariffs.ForEach(tariff =>
                                                        {
                                                            CreditBatchEntryDTO creditBatchEntryTariff_OutstandingBalance = new CreditBatchEntryDTO();

                                                            creditBatchEntryTariff_OutstandingBalance.ChartOfAccountId = tariff.CreditGLAccountId;
                                                            creditBatchEntryTariff_OutstandingBalance.CustomerAccountId = targetCustomerAccount.Id;
                                                            creditBatchEntryTariff_OutstandingBalance.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                            creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                            creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                            creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                            creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                            creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                                            creditBatchEntryTariff_OutstandingBalance.Beneficiary = item.Column3;
                                                            creditBatchEntryTariff_OutstandingBalance.ProductDescription = targetLoanProduct.Description;
                                                            creditBatchEntryTariff_OutstandingBalance.Principal = tariff.Amount;
                                                            creditBatchEntryTariff_OutstandingBalance.Balance = targetCustomerAccount.BookBalance;
                                                            creditBatchEntryTariff_OutstandingBalance.Reference = string.Format("{0}~{1}", item.Column1, tariff.Description);

                                                            result.MatchedCollection1.Add(creditBatchEntryTariff_OutstandingBalance);
                                                        });
                                                    }

                                                    // reset expected interest & expected principal >> NB: interest has priority over principal!
                                                    var actualLoanInterestOutstandingBalance = Math.Min(Math.Abs(targetCustomerAccount.InterestBalance), contributionAmount);

                                                    var actualLoanPrincipalOutstandingBalance = contributionAmount - actualLoanInterestOutstandingBalance;

                                                    CreditBatchEntryDTO creditBatchEntryOutstandingBalance = new CreditBatchEntryDTO();

                                                    creditBatchEntryOutstandingBalance.CustomerAccountId = targetCustomerAccount.Id;
                                                    creditBatchEntryOutstandingBalance.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                    creditBatchEntryOutstandingBalance.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                    creditBatchEntryOutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                    creditBatchEntryOutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                    creditBatchEntryOutstandingBalance.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                    creditBatchEntryOutstandingBalance.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                                    creditBatchEntryOutstandingBalance.Beneficiary = item.Column3;
                                                    creditBatchEntryOutstandingBalance.ProductDescription = targetLoanProduct.Description;
                                                    creditBatchEntryOutstandingBalance.Interest = actualLoanInterestOutstandingBalance;
                                                    creditBatchEntryOutstandingBalance.Principal = actualLoanPrincipalOutstandingBalance;
                                                    creditBatchEntryOutstandingBalance.Balance = targetCustomerAccount.BookBalance;
                                                    creditBatchEntryOutstandingBalance.Reference = item.Column1;

                                                    result.MatchedCollection1.Add(creditBatchEntryOutstandingBalance);

                                                    break;
                                                case AggregateCheckOffRecoveryMode.StandingOrder:

                                                    var standingOrders = FindStandingOrdersByBeneficiaryCustomerAccountId(targetCustomerAccount.Id, (int)StandingOrderTrigger.CheckOff, serviceHeader);

                                                    if (standingOrders != null && standingOrders.Any(x => !x.IsLocked))
                                                    {
                                                        if (standingOrders.Count == 1)
                                                        {
                                                            if (checkOffTariffs != null && checkOffTariffs.Any())
                                                            {
                                                                checkOffTariffs.ForEach(tariff =>
                                                                {
                                                                    CreditBatchEntryDTO creditBatchEntryTariff_StandingOrder = new CreditBatchEntryDTO();

                                                                    creditBatchEntryTariff_StandingOrder.ChartOfAccountId = tariff.CreditGLAccountId;
                                                                    creditBatchEntryTariff_StandingOrder.CustomerAccountId = targetCustomerAccount.Id;
                                                                    creditBatchEntryTariff_StandingOrder.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                                    creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                                    creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                                    creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                                    creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                                    creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                                                    creditBatchEntryTariff_StandingOrder.Beneficiary = item.Column3;
                                                                    creditBatchEntryTariff_StandingOrder.ProductDescription = targetLoanProduct.Description;
                                                                    creditBatchEntryTariff_StandingOrder.Principal = tariff.Amount;
                                                                    creditBatchEntryTariff_StandingOrder.Balance = targetCustomerAccount.BookBalance;
                                                                    creditBatchEntryTariff_StandingOrder.Reference = string.Format("{0}~{1}", item.Column1, tariff.Description);

                                                                    result.MatchedCollection1.Add(creditBatchEntryTariff_StandingOrder);
                                                                });
                                                            }

                                                            var targetStandingOrder = standingOrders[0];

                                                            // reset expected interest & expected principal >> NB: interest has priority over principal!
                                                            var actualLoanInterestStandingOrder = Math.Min(Math.Min(Math.Abs(targetCustomerAccount.InterestBalance), targetStandingOrder.Interest), contributionAmount);

                                                            var actualLoanPrincipalStandingOrder = contributionAmount - actualLoanInterestStandingOrder;

                                                            CreditBatchEntryDTO creditBatchEntryStandingOrder = new CreditBatchEntryDTO();

                                                            creditBatchEntryStandingOrder.CustomerAccountId = targetCustomerAccount.Id;
                                                            creditBatchEntryStandingOrder.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                            creditBatchEntryStandingOrder.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                            creditBatchEntryStandingOrder.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                            creditBatchEntryStandingOrder.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                            creditBatchEntryStandingOrder.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                            creditBatchEntryStandingOrder.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                                                            creditBatchEntryStandingOrder.Beneficiary = item.Column3;
                                                            creditBatchEntryStandingOrder.ProductDescription = targetLoanProduct.Description;
                                                            creditBatchEntryStandingOrder.Interest = actualLoanInterestStandingOrder;
                                                            creditBatchEntryStandingOrder.Principal = actualLoanPrincipalStandingOrder;
                                                            creditBatchEntryStandingOrder.Balance = targetCustomerAccount.BookBalance;
                                                            creditBatchEntryStandingOrder.Reference = item.Column1;

                                                            result.MatchedCollection1.Add(creditBatchEntryStandingOrder);
                                                        }
                                                        else
                                                        {
                                                            item.Remarks = string.Format("Record #{0} ~ found {1} standing order matches", count, standingOrders.Count);

                                                            result.MismatchedCollection.Add(item);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        item.Remarks = string.Format("Record #{0} ~ no match for {1} standing order", count, targetLoanProduct.Description);

                                                        result.MismatchedCollection.Add(item);
                                                    }

                                                    break;
                                                default:

                                                    item.Remarks = string.Format("Record #{0} ~ {1} has undefined check-off recovery mode (Agg.)", count, targetLoanProduct.Description);

                                                    result.MismatchedCollection.Add(item);

                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches by personal file number {2}", count, customerLoanAccounts.Count(), item.Column1);

                                            result.MismatchedCollection.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ no match for loan product customer account by personal file number {1}", count, item.Column1);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ no match for loan product", count);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            default:

                                item.Remarks = string.Format("Record #{0} ~ unable to parse check-off type {1}", count, item.Column5);

                                result.MismatchedCollection.Add(item);

                                break;
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ unable to parse product code {1}", count, item.Column4);

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse amount(s) {1}/{2}", count, item.Column3, item.Column2);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private BatchImportParseInfo ParseCheckOff_Fuzzy(CreditBatch creditBatch, List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection1 = new List<CreditBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var count = 0;

            importEntries.ForEach(item =>
            {
                var contributionAmount = default(decimal);

                var productBalance = default(decimal);

                if (decimal.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out contributionAmount) && decimal.TryParse(item.Column4, NumberStyles.Any, CultureInfo.InvariantCulture, out productBalance))
                {
                    var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByCustomerRerence3AndTrigger(item.Column2, (int)StandingOrderTrigger.CheckOff, serviceHeader);

                    if (standingOrderDTOs != null && standingOrderDTOs.Any())
                    {
                        standingOrderDTOs.ForEach(standingOrderDTO =>
                        {
                            var benefactorCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BenefactorCustomerAccountId, serviceHeader);

                            if (benefactorCustomerAccount != null)
                            {
                                standingOrderDTO.BenefactorCustomerAccountCustomerAccountTypeProductCode = benefactorCustomerAccount.CustomerAccountTypeProductCode;
                                standingOrderDTO.BenefactorCustomerAccountCustomerAccountTypeTargetProductId = benefactorCustomerAccount.CustomerAccountTypeTargetProductId;
                                standingOrderDTO.BenefactorCustomerAccountCustomerAccountTypeTargetProductCode = benefactorCustomerAccount.CustomerAccountTypeTargetProductCode;
                            }

                            var beneficiaryCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                            if (beneficiaryCustomerAccount != null)
                            {
                                standingOrderDTO.BeneficiaryCustomerAccountCustomerAccountTypeProductCode = beneficiaryCustomerAccount.CustomerAccountTypeProductCode;
                                standingOrderDTO.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId = beneficiaryCustomerAccount.CustomerAccountTypeTargetProductId;
                                standingOrderDTO.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductCode = beneficiaryCustomerAccount.CustomerAccountTypeTargetProductCode;
                            }
                        });

                        _customerAccountAppService.FetchCustomerAccountsProductDescription(standingOrderDTOs, serviceHeader);

                        switch ((CheckOffEntryType)Enum.Parse(typeof(CheckOffEntryType), item.Column7))
                        {
                            case CheckOffEntryType.sLoan:

                                #region sLoan

                                var sLoan_MatchedStandingOrders = standingOrderDTOs.Where(x => x.BeneficiaryCustomerAccountCustomerAccountTypeProductCode == (int)ProductCode.Loan && x.Principal == contributionAmount);

                                if (sLoan_MatchedStandingOrders != null && sLoan_MatchedStandingOrders.Any())
                                {
                                    if (sLoan_MatchedStandingOrders.Count() == 1)
                                    {
                                        var targetLoanPrincipalStandingOrder = sLoan_MatchedStandingOrders.First();

                                        var targetCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(targetLoanPrincipalStandingOrder.BeneficiaryCustomerAccountId, serviceHeader);

                                        var existingEntries = from b in result.MatchedCollection1
                                                              where b.CustomerAccountId == targetCustomerAccount.Id
                                                              select b;

                                        if (existingEntries != null && existingEntries.Any())
                                        {
                                            if (existingEntries.Count() == 1)
                                            {
                                                existingEntries.Single().Principal += contributionAmount;
                                                existingEntries.Single().Balance += productBalance;
                                            }
                                            else
                                            {
                                                item.Remarks = string.Format("Record #{0} ~ (sLoan) >> Existing entry couldn't be matched!", count);

                                                result.MismatchedCollection.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                                            creditBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                                            creditBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                            creditBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                            creditBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column2;
                                            creditBatchEntry.Beneficiary = item.Column5;
                                            creditBatchEntry.ProductDescription = targetLoanPrincipalStandingOrder.BeneficiaryProductDescription;
                                            creditBatchEntry.Principal = contributionAmount;
                                            creditBatchEntry.Balance = productBalance;
                                            creditBatchEntry.Reference = item.Column2;

                                            result.MatchedCollection1.Add(creditBatchEntry);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ (sLoan) found {1} matches for standing order having Principal as {2}", count, sLoan_MatchedStandingOrders.Count(), contributionAmount);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ (sLoan) no match for standing order having Principal as {1}", count, contributionAmount);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sInterest:

                                #region sInterest

                                var sInterest_MatchedStandingOrders = standingOrderDTOs.Where(x => x.BeneficiaryCustomerAccountCustomerAccountTypeProductCode == (int)ProductCode.Loan && x.Interest == contributionAmount);

                                if (sInterest_MatchedStandingOrders != null && sInterest_MatchedStandingOrders.Any())
                                {
                                    if (sInterest_MatchedStandingOrders.Count() == 1)
                                    {
                                        var targetLoanInterestStandingOrder = sInterest_MatchedStandingOrders.First();

                                        var targetCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(targetLoanInterestStandingOrder.BeneficiaryCustomerAccountId, serviceHeader);

                                        var existingEntries = from b in result.MatchedCollection1
                                                              where b.CustomerAccountId == targetCustomerAccount.Id
                                                              select b;

                                        if (existingEntries != null && existingEntries.Any())
                                        {
                                            if (existingEntries.Count() == 1)
                                            {
                                                existingEntries.Single().Interest += contributionAmount;
                                                existingEntries.Single().Balance += productBalance;
                                            }
                                            else
                                            {
                                                item.Remarks = string.Format("Record #{0} ~ (sInterest) >> Existing entry couldn't be matched!", count);

                                                result.MismatchedCollection.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                                            creditBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                                            creditBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                            creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                            creditBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                            creditBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column2;
                                            creditBatchEntry.Beneficiary = item.Column5;
                                            creditBatchEntry.ProductDescription = targetLoanInterestStandingOrder.BeneficiaryProductDescription;
                                            creditBatchEntry.Interest = contributionAmount;
                                            creditBatchEntry.Balance = productBalance;
                                            creditBatchEntry.Reference = item.Column2;

                                            result.MatchedCollection1.Add(creditBatchEntry);
                                        }
                                    }
                                    else
                                    {
                                        item.Remarks = string.Format("Record #{0} ~ (sInterest) found {1} matches for standing order having Interest as {2}", count, sInterest_MatchedStandingOrders.Count(), contributionAmount);

                                        result.MismatchedCollection.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ (sInterest) no match for standing order having Interest as {1}", count, contributionAmount);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sShare:
                            case CheckOffEntryType.wCont:
                            case CheckOffEntryType.sInvest:
                            case CheckOffEntryType.sRisk:
                            case CheckOffEntryType.wLoan:

                                #region sShare/wCont/sInvest/sRisk/wLoan

                                var sShare_MatchedStandingOrders = standingOrderDTOs.Where(x => x.BeneficiaryCustomerAccountCustomerAccountTypeProductCode == (int)ProductCode.Investment && x.ChargeFixedAmount == contributionAmount);

                                if (sShare_MatchedStandingOrders != null && sShare_MatchedStandingOrders.Any())
                                {
                                    foreach (var targetInvestmentStandingOrder in sShare_MatchedStandingOrders)
                                    {
                                        var targetCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(targetInvestmentStandingOrder.BeneficiaryCustomerAccountId, serviceHeader);

                                        CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                                        creditBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                                        creditBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                        creditBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                        creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                        creditBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                        creditBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                        creditBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column2;
                                        creditBatchEntry.Beneficiary = item.Column5;
                                        creditBatchEntry.ProductDescription = targetInvestmentStandingOrder.BeneficiaryProductDescription;
                                        creditBatchEntry.Principal = contributionAmount;
                                        creditBatchEntry.Balance = productBalance;
                                        creditBatchEntry.Reference = item.Column2;

                                        result.MatchedCollection1.Add(creditBatchEntry);
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ (sShare/wCont/sInvest/sRisk/wLoan) no match for standing order having ChargeFixedAmount as {1}", count, contributionAmount);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            case CheckOffEntryType.sLoanInterest:

                                #region sLoanInterest

                                var sLoanInterest_MatchedStandingOrders = standingOrderDTOs.Where(x => x.BeneficiaryCustomerAccountCustomerAccountTypeProductCode == (int)ProductCode.Loan && x.PaymentPerPeriod == contributionAmount);

                                if (sLoanInterest_MatchedStandingOrders != null && sLoanInterest_MatchedStandingOrders.Any())
                                {
                                    foreach (var target_sLoanInterestStandingOrder in sLoanInterest_MatchedStandingOrders)
                                    {
                                        var targetCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(target_sLoanInterestStandingOrder.BeneficiaryCustomerAccountId, serviceHeader);

                                        var targetLoanProduct = _loanProductAppService.FindCachedLoanProduct(targetCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                                        targetCustomerAccount.PrincipalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(targetCustomerAccount, 1, DateTime.Now, serviceHeader);
                                        targetCustomerAccount.PrincipalBalance = targetCustomerAccount.PrincipalBalance * -1 > 0m ? targetCustomerAccount.PrincipalBalance : 0m;

                                        targetCustomerAccount.InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(targetCustomerAccount, 2, DateTime.Now, serviceHeader);
                                        targetCustomerAccount.InterestBalance = targetCustomerAccount.InterestBalance * -1 > 0m ? targetCustomerAccount.InterestBalance : 0m;

                                        targetCustomerAccount.BookBalance = targetCustomerAccount.PrincipalBalance + targetCustomerAccount.InterestBalance;

                                        var checkOffTariffs = _commissionAppService.ComputeTariffsByCheckOffCreditType(creditBatch.CreditTypeId, contributionAmount, creditBatch.CreditType.ChartOfAccountId, serviceHeader);

                                        contributionAmount -= checkOffTariffs.Sum(x => x.Amount);

                                        switch ((AggregateCheckOffRecoveryMode)targetLoanProduct.LoanRegistrationAggregateCheckOffRecoveryMode)
                                        {
                                            case AggregateCheckOffRecoveryMode.OutstandingBalance:

                                                if (checkOffTariffs != null && checkOffTariffs.Any())
                                                {
                                                    checkOffTariffs.ForEach(tariff =>
                                                    {
                                                        CreditBatchEntryDTO creditBatchEntryTariff_OutstandingBalance = new CreditBatchEntryDTO();

                                                        creditBatchEntryTariff_OutstandingBalance.ChartOfAccountId = tariff.CreditGLAccountId;
                                                        creditBatchEntryTariff_OutstandingBalance.CustomerAccountId = targetCustomerAccount.Id;
                                                        creditBatchEntryTariff_OutstandingBalance.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                        creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                        creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                        creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                        creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                        creditBatchEntryTariff_OutstandingBalance.CustomerAccountCustomerIndividualPayrollNumbers = item.Column2;
                                                        creditBatchEntryTariff_OutstandingBalance.Beneficiary = item.Column5;
                                                        creditBatchEntryTariff_OutstandingBalance.ProductDescription = target_sLoanInterestStandingOrder.BeneficiaryProductDescription;
                                                        creditBatchEntryTariff_OutstandingBalance.Principal = tariff.Amount;
                                                        creditBatchEntryTariff_OutstandingBalance.Balance = targetCustomerAccount.BookBalance;
                                                        creditBatchEntryTariff_OutstandingBalance.Reference = string.Format("{0}~{1}", item.Column2, tariff.Description);

                                                        result.MatchedCollection1.Add(creditBatchEntryTariff_OutstandingBalance);
                                                    });
                                                }

                                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                                var actualLoanInterestOutstandingBalance = Math.Min(Math.Abs(targetCustomerAccount.InterestBalance), contributionAmount);

                                                var actualLoanPrincipalOutstandingBalance = contributionAmount - actualLoanInterestOutstandingBalance;

                                                CreditBatchEntryDTO creditBatchEntryOutstandingBalance = new CreditBatchEntryDTO();

                                                creditBatchEntryOutstandingBalance.CustomerAccountId = targetCustomerAccount.Id;
                                                creditBatchEntryOutstandingBalance.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                creditBatchEntryOutstandingBalance.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                creditBatchEntryOutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                creditBatchEntryOutstandingBalance.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                creditBatchEntryOutstandingBalance.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                creditBatchEntryOutstandingBalance.CustomerAccountCustomerIndividualPayrollNumbers = item.Column2;
                                                creditBatchEntryOutstandingBalance.Beneficiary = item.Column5;
                                                creditBatchEntryOutstandingBalance.ProductDescription = target_sLoanInterestStandingOrder.BeneficiaryProductDescription;
                                                creditBatchEntryOutstandingBalance.Interest = actualLoanInterestOutstandingBalance;
                                                creditBatchEntryOutstandingBalance.Principal = actualLoanPrincipalOutstandingBalance;
                                                creditBatchEntryOutstandingBalance.Balance = targetCustomerAccount.BookBalance;
                                                creditBatchEntryOutstandingBalance.Reference = item.Column2;

                                                result.MatchedCollection1.Add(creditBatchEntryOutstandingBalance);

                                                break;
                                            case AggregateCheckOffRecoveryMode.StandingOrder:

                                                if (checkOffTariffs != null && checkOffTariffs.Any())
                                                {
                                                    checkOffTariffs.ForEach(tariff =>
                                                    {
                                                        CreditBatchEntryDTO creditBatchEntryTariff_StandingOrder = new CreditBatchEntryDTO();

                                                        creditBatchEntryTariff_StandingOrder.ChartOfAccountId = tariff.CreditGLAccountId;
                                                        creditBatchEntryTariff_StandingOrder.CustomerAccountId = targetCustomerAccount.Id;
                                                        creditBatchEntryTariff_StandingOrder.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                        creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                        creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                        creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                        creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                        creditBatchEntryTariff_StandingOrder.CustomerAccountCustomerIndividualPayrollNumbers = item.Column2;
                                                        creditBatchEntryTariff_StandingOrder.Beneficiary = item.Column5;
                                                        creditBatchEntryTariff_StandingOrder.ProductDescription = target_sLoanInterestStandingOrder.BeneficiaryProductDescription;
                                                        creditBatchEntryTariff_StandingOrder.Principal = tariff.Amount;
                                                        creditBatchEntryTariff_StandingOrder.Balance = targetCustomerAccount.BookBalance;
                                                        creditBatchEntryTariff_StandingOrder.Reference = string.Format("{0}~{1}", item.Column2, tariff.Description);

                                                        result.MatchedCollection1.Add(creditBatchEntryTariff_StandingOrder);
                                                    });
                                                }

                                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                                var actualLoanInterestStandingOrder = Math.Min(Math.Min(Math.Abs(targetCustomerAccount.InterestBalance), target_sLoanInterestStandingOrder.Interest), contributionAmount);

                                                var actualLoanPrincipalStandingOrder = contributionAmount - actualLoanInterestStandingOrder;

                                                CreditBatchEntryDTO creditBatchEntryStandingOrder = new CreditBatchEntryDTO();

                                                creditBatchEntryStandingOrder.CustomerAccountId = targetCustomerAccount.Id;
                                                creditBatchEntryStandingOrder.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                                                creditBatchEntryStandingOrder.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                                                creditBatchEntryStandingOrder.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                                                creditBatchEntryStandingOrder.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                                                creditBatchEntryStandingOrder.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                                                creditBatchEntryStandingOrder.CustomerAccountCustomerIndividualPayrollNumbers = item.Column2;
                                                creditBatchEntryStandingOrder.Beneficiary = item.Column5;
                                                creditBatchEntryStandingOrder.ProductDescription = target_sLoanInterestStandingOrder.BeneficiaryProductDescription;
                                                creditBatchEntryStandingOrder.Interest = actualLoanInterestStandingOrder;
                                                creditBatchEntryStandingOrder.Principal = actualLoanPrincipalStandingOrder;
                                                creditBatchEntryStandingOrder.Balance = targetCustomerAccount.BookBalance;
                                                creditBatchEntryStandingOrder.Reference = item.Column2;

                                                result.MatchedCollection1.Add(creditBatchEntryStandingOrder);

                                                break;
                                            default:

                                                item.Remarks = string.Format("Record #{0} ~ (sLoanInterest) {1} has undefined check-off recovery mode (Agg.)", count, targetLoanProduct.Description);

                                                result.MismatchedCollection.Add(item);

                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~ (sLoanInterest) no match for standing order having PaymentPerPeriod as {1}", count, contributionAmount);

                                    result.MismatchedCollection.Add(item);
                                }

                                #endregion

                                break;

                            default:

                                item.Remarks = string.Format("Record #{0} ~ unable to parse check-off type {1}.", count, item.Column7);

                                result.MismatchedCollection.Add(item);

                                break;
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ unable to find check-off standing orders for {1}.", count, item.Column2);

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse amount(s) {1}/{2}.", count, item.Column3, item.Column4);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private BatchImportParseInfo ParseCashPickup(List<BatchImportEntryWrapper> importEntries)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection1 = new List<CreditBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var count = 0;

            importEntries.ForEach(item =>
            {
                var amount = default(decimal);

                if (decimal.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                {
                    CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                    creditBatchEntry.Beneficiary = item.Column1;
                    creditBatchEntry.Reference = item.Column2;
                    creditBatchEntry.Principal = amount;

                    result.MatchedCollection1.Add(creditBatchEntry);
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse amount.", count);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private BatchImportParseInfo ParseSundryPayments(List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection1 = new List<CreditBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var count = 0;

            importEntries.ForEach(item =>
            {
                var amount = default(decimal);

                if (decimal.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                {
                    var chartOfAccountCode = default(int);

                    if (int.TryParse(item.Column1, out chartOfAccountCode))
                    {
                        var matchedChartOfAccounts = _chartOfAccountAppService.FindChartOfAccounts(chartOfAccountCode, serviceHeader);

                        if (matchedChartOfAccounts != null && matchedChartOfAccounts.Any())
                        {
                            if (matchedChartOfAccounts.Count == 1)
                            {
                                var targetChartOfAccount = matchedChartOfAccounts[0];

                                if (targetChartOfAccount.AccountCategory == (int)ChartOfAccountCategory.DetailAccount)
                                {
                                    CreditBatchEntryDTO creditBatchEntry = new CreditBatchEntryDTO();

                                    creditBatchEntry.ChartOfAccountId = targetChartOfAccount.Id;
                                    creditBatchEntry.Principal = amount;
                                    creditBatchEntry.Reference = item.Column2;

                                    result.MatchedCollection1.Add(creditBatchEntry);
                                }
                                else
                                {
                                    item.Remarks = string.Format("Record #{0} ~  G/L account category is Header Account (Non-Postable).", count);

                                    result.MismatchedCollection.Add(item);
                                }
                            }
                            else
                            {
                                item.Remarks = string.Format("Record #{0} ~ found {1} G/L account matches.", count, matchedChartOfAccounts.Count());

                                result.MismatchedCollection.Add(item);
                            }
                        }
                        else
                        {
                            item.Remarks = string.Format("Record #{0} ~ no match for G/L account code {1}", count, item.Column1);

                            result.MismatchedCollection.Add(item);
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ unable to parse G/L account code {1}.", count, item.Column1);

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse amount {1}.", count, item.Column3);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private bool UpdateCreditBatchEntries(Guid creditBatchId, List<CreditBatchEntryDTO> creditBatchEntries, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (creditBatchId != null && creditBatchEntries != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _creditBatchRepository.Get(creditBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        _sqlCommandAppService.DeleteCreditBatchEntries(persisted.Id, serviceHeader);

                        if (creditBatchEntries.Any())
                        {
                            List<CreditBatchEntry> batchEntries = new List<CreditBatchEntry>();

                            foreach (var item in creditBatchEntries)
                            {
                                var creditBatchEntry = CreditBatchEntryFactory.CreateCreditBatchEntry(persisted.Id, item.CustomerAccountId, item.ChartOfAccountId, item.Principal, item.Interest, item.Balance, item.Beneficiary, item.Reference);

                                creditBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                creditBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                batchEntries.Add(creditBatchEntry);
                            }

                            if (batchEntries.Any())
                            {
                                var bcpBatchEntries = new List<CreditBatchEntryBulkCopyDTO>();

                                batchEntries.ForEach(c =>
                                {
                                    CreditBatchEntryBulkCopyDTO bcpc =
                                        new CreditBatchEntryBulkCopyDTO
                                        {
                                            Id = c.Id,
                                            CreditBatchId = c.CreditBatchId,
                                            CustomerAccountId = c.CustomerAccountId,
                                            ChartOfAccountId = c.ChartOfAccountId,
                                            Principal = c.Principal,
                                            Interest = c.Interest,
                                            Balance = c.Balance,
                                            Beneficiary = c.Beneficiary,
                                            Reference = c.Reference,
                                            Status = c.Status,
                                            CreatedBy = c.CreatedBy,
                                            CreatedDate = c.CreatedDate,
                                        };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _creditBatchEntryRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool UpdateCreditBatchDiscrepancies(Guid creditBatchId, List<CreditBatchDiscrepancyDTO> creditBatchDiscrepancies, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (creditBatchId != null && creditBatchDiscrepancies != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _creditBatchRepository.Get(creditBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        _sqlCommandAppService.DeleteCreditBatchDiscrepancies(persisted.Id, serviceHeader);

                        if (creditBatchDiscrepancies.Any())
                        {
                            List<CreditBatchDiscrepancy> batchDiscrepancies = new List<CreditBatchDiscrepancy>();

                            foreach (var item in creditBatchDiscrepancies)
                            {
                                var creditBatchDiscrepancy = CreditBatchDiscrepancyFactory.CreateCreditBatchDiscrepancy(persisted.Id, item.Column1, item.Column2, item.Column3, item.Column4, item.Column5, item.Column6, item.Column7, item.Column8, item.Remarks);

                                creditBatchDiscrepancy.Status = (int)BatchEntryStatus.Pending;
                                creditBatchDiscrepancy.CreatedBy = serviceHeader.ApplicationUserName;

                                batchDiscrepancies.Add(creditBatchDiscrepancy);
                            }

                            if (batchDiscrepancies.Any())
                            {
                                var bcpBatchEntries = new List<CreditBatchDiscrepancyBulkCopyDTO>();

                                batchDiscrepancies.ForEach(c =>
                                {
                                    CreditBatchDiscrepancyBulkCopyDTO bcpc =
                                        new CreditBatchDiscrepancyBulkCopyDTO
                                        {
                                            Id = c.Id,
                                            CreditBatchId = c.CreditBatchId,
                                            Column1 = c.Column1,
                                            Column2 = c.Column2,
                                            Column3 = c.Column3,
                                            Column4 = c.Column4,
                                            Column5 = c.Column5,
                                            Column6 = c.Column6,
                                            Column7 = c.Column7,
                                            Column8 = c.Column8,
                                            Remarks = c.Remarks,
                                            Status = c.Status,
                                            PostedBy = c.PostedBy,
                                            PostedDate = c.PostedDate,
                                            CreatedBy = c.CreatedBy,
                                            CreatedDate = c.CreatedDate,
                                        };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _creditBatchDiscrepancyRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private Tuple<decimal, decimal> RecoverAttachedDirectDebits(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, CustomerAccountDTO batchEntryCustomerAccount, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader)
        {
            var creditTypeDirectDebits = _creditTypeAppService.FindCachedDirectDebits(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

            if (creditTypeDirectDebits != null && creditTypeDirectDebits.Any())
            {
                foreach (var directDebitDTO in creditTypeDirectDebits)
                {
                    if (directDebitDTO.IsLocked) continue;

                    CustomerAccountDTO directDebitCustomerAccountDTO = null;

                    #region Find direct-debit beneficiary account

                    var directDebitCustomerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(directDebitDTO.CustomerAccountTypeTargetProductId, batchEntryCustomerAccount.CustomerId, serviceHeader);

                    if (directDebitCustomerAccounts != null && directDebitCustomerAccounts.Any())
                        directDebitCustomerAccountDTO = directDebitCustomerAccounts.First();

                    #endregion

                    if (directDebitCustomerAccountDTO != null)
                    {
                        var directDebitAmount = 0m;

                        switch ((ChargeType)directDebitDTO.ChargeType)
                        {
                            case ChargeType.Percentage:
                                directDebitAmount = Math.Round(Convert.ToDecimal((directDebitDTO.ChargePercentage * Convert.ToDouble(creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest)) / 100), 4, MidpointRounding.AwayFromZero);
                                break;
                            case ChargeType.FixedAmount:
                                directDebitAmount = directDebitDTO.ChargeFixedAmount;
                                break;
                            default:
                                break;
                        }

                        if (directDebitDTO.CustomerAccountTypeProductCode == (int)ProductCode.Investment)
                        {
                            var targetDirectDebitInvestmentProduct = _investmentProductAppService.FindCachedInvestmentProduct(directDebitDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { directDebitCustomerAccountDTO }, serviceHeader);

                            if (directDebitCustomerAccountDTO.BookBalance < targetDirectDebitInvestmentProduct.MaximumBalance)
                            {
                                directDebitAmount = Math.Min(directDebitAmount, (targetDirectDebitInvestmentProduct.MaximumBalance - directDebitCustomerAccountDTO.BookBalance));
                            }
                            else directDebitAmount = 0m;
                        }

                        if (directDebitAmount > 0m)
                        {
                            // track deductions
                            totalRecoveryDeductions += directDebitAmount;

                            // Do we need to reset expected values?
                            if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - directDebitAmount;

                                // how much is available for recovery?
                                var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                // reset expected direct debit
                                directDebitAmount = Math.Min(directDebitAmount, availableRecoveryAmount);

                                // track deductions
                                totalRecoveryDeductions += directDebitAmount;
                            }

                            switch ((ProductCode)directDebitDTO.CustomerAccountTypeProductCode)
                            {
                                case ProductCode.Savings:

                                    var targetDirectDebitSavingsProduct = _savingsProductAppService.FindCachedSavingsProduct(directDebitDTO.CustomerAccountTypeTargetProductId, transactionOwnershipBranchId, serviceHeader);

                                    var savingsDirectDebitJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, directDebitAmount, directDebitDTO.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(savingsDirectDebitJournal, targetDirectDebitSavingsProduct.ChartOfAccountId, targetSavingsProduct.ChartOfAccountId, directDebitCustomerAccountDTO, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(savingsDirectDebitJournal);

                                    // reset available balance
                                    availableBalance -= directDebitAmount;

                                    break;
                                case ProductCode.Investment:

                                    var targetDirectDebitInvestmentProduct = _investmentProductAppService.FindCachedInvestmentProduct(directDebitDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                                    var investmentDirectDebitJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, directDebitAmount, directDebitDTO.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(investmentDirectDebitJournal, targetDirectDebitInvestmentProduct.ChartOfAccountId, targetSavingsProduct.ChartOfAccountId, directDebitCustomerAccountDTO, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(investmentDirectDebitJournal);

                                    // reset available balance
                                    availableBalance -= directDebitAmount;

                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            return new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);
        }

        private Tuple<decimal, decimal> RecoverAttachedConcessionExemptLoans(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, List<StandingOrderHistory> standingOrderHistories, List<CustomerAccountCarryForward> customerAccountCarryForwards, List<CustomerAccountArrearage> customerAccountArrearages, CustomerAccountDTO benefactorCustomerAccount, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader)
        {
            var tuple = new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);

            var creditTypeAttachedProducts = _creditTypeAppService.FindCachedAttachedProducts(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

            if (creditTypeAttachedProducts != null && creditTypeAttachedProducts.LoanProductCollection != null && creditTypeAttachedProducts.LoanProductCollection.Any())
            {
                var creditTypeConcessionExemptProducts = _creditTypeAppService.FindCachedConcessionExemptProducts(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

                if (creditTypeConcessionExemptProducts != null && creditTypeConcessionExemptProducts.LoanProductCollection != null && creditTypeConcessionExemptProducts.LoanProductCollection.Any())
                {
                    var attachedLoansKVPList = new List<KeyValuePair<CustomerAccountDTO, StandingOrderDTO>>();

                    var payoutStandingOrders = FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccount.Id, (int)StandingOrderTrigger.Payout, serviceHeader);

                    if (payoutStandingOrders != null && payoutStandingOrders.Any())
                    {
                        foreach (var payoutStandingOrderDTO in payoutStandingOrders)
                        {
                            if (payoutStandingOrderDTO.IsLocked)
                                continue;

                            var beneficiaryCustomerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(payoutStandingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                            if (beneficiaryCustomerAccountDTO.Status.In((int)CustomerAccountStatus.Closed))
                                continue;

                            if (attachedLoansKVPList.Any(x => x.Key.Id == beneficiaryCustomerAccountDTO.Id && x.Value.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountDTO.Id))
                                continue; /*Standing order beneficiary MUST NOT exist in KVP list - safety check for duplicate active standing orders*/

                            if (!creditTypeAttachedProducts.LoanProductCollection.Any(x => x.Id == beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId))
                                continue;/*MUST exist in attached products list*/

                            if (!beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan))
                                continue;
                            else if (creditTypeConcessionExemptProducts.LoanProductCollection.Any(x => x.Id == beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId))
                            {
                                _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { beneficiaryCustomerAccountDTO }, serviceHeader);

                                attachedLoansKVPList.Add(new KeyValuePair<CustomerAccountDTO, StandingOrderDTO>(beneficiaryCustomerAccountDTO, payoutStandingOrderDTO));
                            }
                        }
                    }

                    if (attachedLoansKVPList.Any())
                    {
                        var orderedAttachedLoansKVPList = attachedLoansKVPList.OrderBy(x => x.Key.CustomerAccountTypeTargetProductRecoveryPriority);

                        tuple = DoLoanRecovery(transactionOwnershipBranchId, parentJournalId, postingPeriodId, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, standingOrderHistories, customerAccountCarryForwards, customerAccountArrearages, benefactorCustomerAccount, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader, orderedAttachedLoansKVPList);
                    }
                }
            }

            return tuple;
        }

        private Tuple<decimal, decimal> RecoverAttachedLoans(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, List<StandingOrderHistory> standingOrderHistories, List<CustomerAccountCarryForward> customerAccountCarryForwards, List<CustomerAccountArrearage> customerAccountArrearages, CustomerAccountDTO benefactorCustomerAccount, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader)
        {
            var tuple = new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);

            var creditTypeAttachedProducts = _creditTypeAppService.FindCachedAttachedProducts(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

            if (creditTypeAttachedProducts != null && creditTypeAttachedProducts.LoanProductCollection != null && creditTypeAttachedProducts.LoanProductCollection.Any())
            {
                var concessionExemptLoanProducts = new List<LoanProductDTO>();

                var creditTypeConcessionExemptProducts = _creditTypeAppService.FindCachedConcessionExemptProducts(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

                if (creditTypeConcessionExemptProducts != null && creditTypeConcessionExemptProducts.LoanProductCollection != null && creditTypeConcessionExemptProducts.LoanProductCollection.Any())
                {
                    concessionExemptLoanProducts = creditTypeConcessionExemptProducts.LoanProductCollection;
                }

                var attachedLoansKVPList = new List<KeyValuePair<CustomerAccountDTO, StandingOrderDTO>>();

                var payoutStandingOrders = FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccount.Id, (int)StandingOrderTrigger.Payout, serviceHeader);

                if (payoutStandingOrders != null && payoutStandingOrders.Any())
                {
                    foreach (var payoutStandingOrderDTO in payoutStandingOrders)
                    {
                        if (payoutStandingOrderDTO.IsLocked)
                            continue;

                        var beneficiaryCustomerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(payoutStandingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                        if (beneficiaryCustomerAccountDTO.Status.In((int)CustomerAccountStatus.Closed))
                            continue;

                        if (attachedLoansKVPList.Any(x => x.Key.Id == beneficiaryCustomerAccountDTO.Id && x.Value.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountDTO.Id))
                            continue; /*Standing order beneficiary MUST NOT exist in KVP list - safety check for duplicate active standing orders*/

                        if (concessionExemptLoanProducts.Any(x => x.Id == beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId))
                            continue;/*MUST not exist in concession exempt products list*/

                        if (!beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan))
                            continue;
                        else if (creditTypeAttachedProducts.LoanProductCollection.Any(x => x.Id == beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId))
                        {
                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { beneficiaryCustomerAccountDTO }, serviceHeader);

                            attachedLoansKVPList.Add(new KeyValuePair<CustomerAccountDTO, StandingOrderDTO>(beneficiaryCustomerAccountDTO, payoutStandingOrderDTO));
                        }
                    }
                }

                if (attachedLoansKVPList.Any())
                {
                    var orderedAttachedLoansKVPList = attachedLoansKVPList.OrderBy(x => x.Key.CustomerAccountTypeTargetProductRecoveryPriority);

                    tuple = DoLoanRecovery(transactionOwnershipBranchId, parentJournalId, postingPeriodId, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, standingOrderHistories, customerAccountCarryForwards, customerAccountArrearages, benefactorCustomerAccount, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader, orderedAttachedLoansKVPList);
                }
            }

            return tuple;
        }

        private Tuple<decimal, decimal> DoLoanRecovery(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, List<StandingOrderHistory> standingOrderHistories, List<CustomerAccountCarryForward> customerAccountCarryForwards, List<CustomerAccountArrearage> customerAccountArrearages, CustomerAccountDTO benefactorCustomerAccount, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader, IOrderedEnumerable<KeyValuePair<CustomerAccountDTO, StandingOrderDTO>> orderedAttachedLoansKVPList)
        {
            foreach (var attachedLoansKVP in orderedAttachedLoansKVPList)
            {
                if (creditBatchEntryDTO.CreditBatchRecoverCarryForwards)
                {
                    // do carry forwards recovery
                    var carryForwardsTuple = RecoverCarryForwards(transactionOwnershipBranchId, parentJournalId, postingPeriodId, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, customerAccountCarryForwards, benefactorCustomerAccount, attachedLoansKVP.Key.Id, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                    // reset deductions
                    totalRecoveryDeductions = carryForwardsTuple.Item1;

                    // reset available balance
                    availableBalance = carryForwardsTuple.Item2;
                }

                var targetLoanCustomerAccount = attachedLoansKVP.Key;

                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { targetLoanCustomerAccount }, serviceHeader, true);

                var loanAccountProduct = _loanProductAppService.FindCachedLoanProduct(targetLoanCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                var principalBalance = targetLoanCustomerAccount.PrincipalBalance;

                var interestBalance = targetLoanCustomerAccount.InterestBalance;

                var expectedPrincipal = 0m;

                var expectedInterest = 0m;

                var actualLoanPrincipal = 0m;

                var actualLoanInterest = 0m;

                var principalArrears = 0m;

                var interestArrears = 0m;

                if (DateTime.Today >= attachedLoansKVP.Value.DurationStartDate)
                {
                    expectedPrincipal = attachedLoansKVP.Value.Principal * -1 < 0m ? attachedLoansKVP.Value.Principal : 0m;
                    expectedInterest = attachedLoansKVP.Value.Interest * -1 < 0m ? attachedLoansKVP.Value.Interest : 0m;

                    actualLoanPrincipal = Math.Min(((principalBalance * -1 > 0m) ? (principalBalance * -1) : 0m), expectedPrincipal);
                    expectedPrincipal = actualLoanPrincipal;

                    actualLoanInterest = Math.Min(((interestBalance * -1 > 0m) ? (interestBalance * -1) : 0m), expectedInterest);
                    expectedInterest = actualLoanInterest;

                    /* ExpectedInterest
                     * if interest calculation mode is straightline && interest charge mode is periodic >> pick interest balance
                     * if interest calculation mode is straightline && interest charge mode is upfront >> pick s/o value
                     */
                    if (loanAccountProduct.LoanInterestCalculationMode == (int)InterestCalculationMode.StraightLine)
                    {
                        switch ((InterestChargeMode)loanAccountProduct.LoanInterestChargeMode)
                        {
                            case InterestChargeMode.Upfront:
                                expectedInterest = actualLoanInterest;
                                break;
                            case InterestChargeMode.Periodic:
                                expectedInterest = (interestBalance * -1 > 0m) ? (interestBalance * -1) : 0m;
                                break;
                            default:
                                break;
                        }
                    }

                    if (loanAccountProduct.LoanRegistrationTrackArrears)
                    {
                        principalArrears = Math.Min(((principalBalance * -1 > 0m) ? (principalBalance * -1) : 0m), targetLoanCustomerAccount.PrincipalArrearagesBalance * -1 < 0m ? targetLoanCustomerAccount.PrincipalArrearagesBalance : 0m);

                        switch ((InterestChargeMode)loanAccountProduct.LoanInterestChargeMode)
                        {
                            case InterestChargeMode.Periodic:
                                interestArrears = Math.Min(((interestBalance * -1 > 0m) ? (interestBalance * -1) : 0m), targetLoanCustomerAccount.InterestArrearagesBalance * -1 < 0m ? targetLoanCustomerAccount.InterestArrearagesBalance : 0m);
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (DateTime.Today >= attachedLoansKVP.Value.DurationStartDate)
                {
                    switch ((PayoutRecoveryMode)loanAccountProduct.LoanRegistrationPayoutRecoveryMode)
                    {
                        case PayoutRecoveryMode.StandingOrder:

                            if (CheckRecovery(attachedLoansKVP.Value.Id, postingPeriodId, benefactorCustomerAccount.Id, creditBatchEntryDTO.CreditBatchMonth, serviceHeader))
                            {
                                // recover arrearages
                                var arrearagesTuple = RecoverArrearages(transactionOwnershipBranchId, parentJournalId, postingPeriodId, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, customerAccountArrearages, benefactorCustomerAccount, targetLoanCustomerAccount.Id, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                                // reset deductions
                                totalRecoveryDeductions = arrearagesTuple.Item1;

                                // reset available balance
                                availableBalance = arrearagesTuple.Item2;

                                // reset actual/expected
                                actualLoanPrincipal = 0m;
                                actualLoanInterest = 0m;
                                expectedPrincipal = 0m;
                                expectedInterest = 0m;
                            }

                            break;
                        case PayoutRecoveryMode.Percentage:

                            // reset actual/expected
                            actualLoanPrincipal = Convert.ToDecimal((loanAccountProduct.LoanRegistrationPayoutRecoveryPercentage * Convert.ToDouble(principalBalance * -1 > 0m ? principalBalance * -1 : 0m)) / 100);
                            expectedPrincipal = actualLoanPrincipal;

                            actualLoanInterest = interestBalance * -1 > 0m ? interestBalance * -1 : 0m;
                            expectedInterest = actualLoanInterest;

                            break;
                        default:
                            break;
                    }
                }

                if ((actualLoanPrincipal + actualLoanInterest) > 0m)
                {
                    // track deductions
                    totalRecoveryDeductions += actualLoanPrincipal;
                    totalRecoveryDeductions += actualLoanInterest;

                    // Do we need to reset expected values?
                    if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                    {
                        // reset deductions so far
                        totalRecoveryDeductions = totalRecoveryDeductions - (actualLoanInterest + actualLoanPrincipal);

                        // how much is available for recovery?
                        var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                        // reset expected interest & expected principal >> NB: interest has priority over principal!
                        actualLoanInterest = Math.Min(actualLoanInterest, availableRecoveryAmount);
                        actualLoanPrincipal = Math.Min(actualLoanPrincipal, (availableRecoveryAmount - actualLoanInterest));

                        // track deductions
                        totalRecoveryDeductions += actualLoanPrincipal;
                        totalRecoveryDeductions += actualLoanInterest;

                        // track new arrears
                        if (loanAccountProduct.LoanRegistrationTrackArrears)
                        {
                            principalArrears += (expectedPrincipal - actualLoanPrincipal);
                            interestArrears += (expectedInterest - actualLoanInterest);
                        }
                    }
                    else
                    {
                        // reset old arrears
                        if (loanAccountProduct.LoanRegistrationTrackArrears)
                        {
                            interestArrears = interestArrears - actualLoanInterest;
                            interestArrears = interestArrears * -1 > 0m ? 0m : interestArrears;

                            principalArrears = principalArrears - actualLoanPrincipal;
                            principalArrears = principalArrears * -1 > 0m ? 0m : principalArrears;
                        }
                    }

                    // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                    var attachedLoanInterestReceivableJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}", loanAccountProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivableJournal, loanAccountProduct.InterestReceivableChartOfAccountId, targetSavingsProduct.ChartOfAccountId, attachedLoansKVP.Key, benefactorCustomerAccount, serviceHeader);
                    journals.Add(attachedLoanInterestReceivableJournal);

                    // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                    var attachedLoanInterestReceivedJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}", loanAccountProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivedJournal, loanAccountProduct.InterestReceivedChartOfAccountId, loanAccountProduct.InterestChargedChartOfAccountId, attachedLoansKVP.Key, attachedLoansKVP.Key, serviceHeader);
                    journals.Add(attachedLoanInterestReceivedJournal);

                    // Credit LoanProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                    var attachedLoanPrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualLoanPrincipal, string.Format("Principal Paid~{0}", loanAccountProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(attachedLoanPrincipalJournal, loanAccountProduct.ChartOfAccountId, targetSavingsProduct.ChartOfAccountId, attachedLoansKVP.Key, benefactorCustomerAccount, serviceHeader);
                    journals.Add(attachedLoanPrincipalJournal);

                    // reset available balance
                    availableBalance -= actualLoanInterest;
                    availableBalance -= actualLoanPrincipal;

                    // Do we need to recover period dynamic charges from savings?
                    if (creditBatchEntryDTO.CreditBatchRecoverIndefiniteCharges)
                    {
                        var periodicDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(attachedLoansKVP.Key.CustomerAccountTypeTargetProductId, (int)DynamicChargeRecoverySource.SavingsAccount, (int)DynamicChargeRecoveryMode.Periodic, (principalBalance * -1 > 0m ? principalBalance * -1 : 0m), benefactorCustomerAccount, serviceHeader, -1);

                        if (periodicDynamicChargeTariffs != null && periodicDynamicChargeTariffs.Any())
                        {
                            periodicDynamicChargeTariffs.ForEach(tariff =>
                            {
                                var actualTariffAmount = tariff.Amount;

                                // track deductions
                                totalRecoveryDeductions += actualTariffAmount;

                                // Do we need to reset expected values?
                                if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                                {
                                    // reset deductions so far
                                    totalRecoveryDeductions = totalRecoveryDeductions - actualTariffAmount;

                                    // how much is available for recovery?
                                    var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                    // reset expected tariff amount
                                    actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                    // track deductions
                                    totalRecoveryDeductions += actualTariffAmount;
                                }

                                var dynamicChargeTariffJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(dynamicChargeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, benefactorCustomerAccount, benefactorCustomerAccount, serviceHeader);
                                journals.Add(dynamicChargeTariffJournal);

                                // reset available balance
                                availableBalance -= actualTariffAmount;
                            });
                        }
                    }

                    // Do we need to update SI history?
                    if (DateTime.Today >= attachedLoansKVP.Value.DurationStartDate)
                    {
                        var history = StandingOrderHistoryFactory.CreateStandingOrderHistory(attachedLoansKVP.Value.Id, postingPeriodId, attachedLoansKVP.Value.BenefactorCustomerAccountId, attachedLoansKVP.Value.BeneficiaryCustomerAccountId, new Duration(attachedLoansKVP.Value.DurationStartDate, attachedLoansKVP.Value.DurationEndDate), new Schedule(attachedLoansKVP.Value.ScheduleFrequency, attachedLoansKVP.Value.ScheduleExpectedRunDate, attachedLoansKVP.Value.ScheduleActualRunDate, attachedLoansKVP.Value.ScheduleExecuteAttemptCount, attachedLoansKVP.Value.ScheduleForceExecute), new Charge(attachedLoansKVP.Value.ChargeType, attachedLoansKVP.Value.ChargePercentage, attachedLoansKVP.Value.ChargeFixedAmount), creditBatchEntryDTO.CreditBatchMonth, attachedLoansKVP.Value.Trigger, attachedLoansKVP.Value.Principal, attachedLoansKVP.Value.Interest, actualLoanPrincipal, actualLoanInterest, attachedLoansKVP.Value.Remarks);
                        history.CreatedBy = serviceHeader.ApplicationUserName;
                        standingOrderHistories.Add(history);

                        // Do we need to update arrearage history?
                        var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(targetLoanCustomerAccount.Id, (int)ArrearageCategory.Interest, interestArrears, reference);
                        customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                        customerAccountArrearages.Add(customerAccountInterestArrearage);

                        var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(targetLoanCustomerAccount.Id, (int)ArrearageCategory.Principal, principalArrears, reference);
                        customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                        customerAccountArrearages.Add(customerAccountPrincipalArrearage);

                        // Do we need to effect standing order charges?
                        if (attachedLoansKVP.Value.Chargeable)
                        {
                            var standingOrderTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(targetSavingsProduct.Id, (int)SavingsProductKnownChargeType.StandingOrderFee, 0m, benefactorCustomerAccount, serviceHeader);

                            if (standingOrderTariffs != null && standingOrderTariffs.Any())
                            {
                                standingOrderTariffs.ForEach(tariff =>
                                {
                                    var actualTariffAmount = tariff.Amount;

                                    // track deductions
                                    totalRecoveryDeductions += actualTariffAmount;

                                    // Do we need to reset expected values?
                                    if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                                    {
                                        // reset deductions so far
                                        totalRecoveryDeductions = totalRecoveryDeductions - actualTariffAmount;

                                        // how much is available for recovery?
                                        var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                        // reset expected tariff amount
                                        actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                        // track deductions
                                        totalRecoveryDeductions += actualTariffAmount;
                                    }

                                    var standingOrderTariffJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(standingOrderTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, benefactorCustomerAccount, benefactorCustomerAccount, serviceHeader);
                                    journals.Add(standingOrderTariffJournal);

                                    // reset available balance
                                    availableBalance -= actualTariffAmount;
                                });
                            }
                        }
                    }
                }
            }

            return new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);
        }

        private Tuple<decimal, decimal> RecoverCarryForwards(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, List<CustomerAccountCarryForward> customerAccountCarryForwards, CustomerAccountDTO benefactorCustomerAccount, Guid beneficiaryCustomerAccountId, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader)
        {
            var carryForwards = _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId, serviceHeader);

            if (carryForwards != null && carryForwards.Any())
            {
                var targetCarryForwards = from c in carryForwards
                                          where c.BenefactorCustomerAccountId == benefactorCustomerAccount.Id
                                          select c;

                if (targetCarryForwards != null && targetCarryForwards.Any())
                {
                    var grouping = from p in targetCarryForwards
                                   group p.Amount by new
                                   {
                                       p.BeneficiaryCustomerAccountId,
                                       p.BeneficiaryChartOfAccountId
                                   } into g
                                   select new
                                   {
                                       BeneficiaryCustomerAccountId = g.Key.BeneficiaryCustomerAccountId,
                                       BeneficiaryChartOfAccountId = g.Key.BeneficiaryChartOfAccountId,
                                       Payments = g.ToList()
                                   };

                    foreach (var item in grouping)
                    {
                        var totalPayments = item.Payments.Sum();

                        if (totalPayments * -1 < 0m)
                        {
                            var principalArrears = totalPayments;

                            var interestArrears = 0m;

                            #region do we need to repay in installments?

                            var carryForwardInstallments = _customerAccountAppService.FindCustomerAccountCarryForwardInstallments(item.BeneficiaryCustomerAccountId, item.BeneficiaryChartOfAccountId, serviceHeader);

                            if (carryForwardInstallments != null && carryForwardInstallments.Any() && carryForwardInstallments.Count == 1)
                            {
                                var targetInstallment = carryForwardInstallments.FirstOrDefault();

                                if (targetInstallment != null && targetInstallment.Amount * -1 < 0m && !targetInstallment.IsLocked)
                                    principalArrears = Math.Min(targetInstallment.Amount, principalArrears);
                            }

                            #endregion

                            if ((principalArrears + interestArrears) > 0m)
                            {
                                var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(item.BeneficiaryCustomerAccountId, serviceHeader);

                                var targetLoanProduct = _loanProductAppService.FindCachedLoanProduct(beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                                if (targetSavingsProduct != null && targetLoanProduct != null)
                                {
                                    // track deductions
                                    totalRecoveryDeductions += principalArrears;
                                    totalRecoveryDeductions += interestArrears;

                                    // Do we need to reset expected values?
                                    if (!((availableBalance - totalRecoveryDeductions) >= 0m))
                                    {
                                        // reset deductions so far
                                        totalRecoveryDeductions = totalRecoveryDeductions - (interestArrears + principalArrears);

                                        // how much is available for recovery?
                                        var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                        // reset expected interest & expected principal >> NB: interest has priority over principal!
                                        interestArrears = Math.Min(interestArrears, availableRecoveryAmount);
                                        principalArrears = Math.Min(principalArrears, (availableRecoveryAmount - interestArrears));

                                        // track deductions
                                        totalRecoveryDeductions += principalArrears;
                                        totalRecoveryDeductions += interestArrears;
                                    }

                                    // Credit CarryForward.BeneficiaryChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                                    var carryFowardBeneficiaryJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, principalArrears, string.Format("Carry Forwards Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(carryFowardBeneficiaryJournal, item.BeneficiaryChartOfAccountId, targetSavingsProduct.ChartOfAccountId, benefactorCustomerAccount, benefactorCustomerAccount, serviceHeader);
                                    journals.Add(carryFowardBeneficiaryJournal);

                                    // Do we need to update carry forward history?
                                    var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(benefactorCustomerAccount.Id, item.BeneficiaryCustomerAccountId, item.BeneficiaryChartOfAccountId, principalArrears * -1/*-ve cos is payment*/, reference);
                                    customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                    customerAccountCarryForwards.Add(customerAccountCarryForward);

                                    // reset available balance
                                    availableBalance -= principalArrears;
                                }
                            }
                        }
                    }
                }
            }

            return new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);
        }

        private Tuple<decimal, decimal> RecoverArrearages(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, List<CustomerAccountArrearage> customerAccountArrearages, CustomerAccountDTO benefactorCustomerAccount, Guid beneficiaryCustomerAccountId, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader)
        {
            var targetArrearages = _customerAccountAppService.FindCustomerAccountArrearagesByCustomerAccountId(beneficiaryCustomerAccountId, serviceHeader);

            if (targetArrearages != null && targetArrearages.Any())
            {
                var principalArrears = 0m;

                var interestArrears = 0m;

                var totalPrincipalArrearages = targetArrearages.Where(x => x.Category == (int)ArrearageCategory.Principal).Sum(x => x.Amount);

                if (totalPrincipalArrearages * -1 < 0m)
                {
                    principalArrears = totalPrincipalArrearages;
                }

                var totalInterestArrearages = targetArrearages.Where(x => x.Category == (int)ArrearageCategory.Interest).Sum(x => x.Amount);

                if (totalInterestArrearages * -1 < 0m)
                {
                    interestArrears = totalInterestArrearages;
                }

                if ((principalArrears + interestArrears) > 0m)
                {
                    var beneficiaryCustomerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(beneficiaryCustomerAccountId, serviceHeader);

                    if (beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan))
                    {
                        beneficiaryCustomerAccountDTO.PrincipalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(beneficiaryCustomerAccountDTO, 1, DateTime.Now, serviceHeader);

                        beneficiaryCustomerAccountDTO.InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(beneficiaryCustomerAccountDTO, 2, DateTime.Now, serviceHeader);

                        beneficiaryCustomerAccountDTO.BookBalance = beneficiaryCustomerAccountDTO.PrincipalBalance + beneficiaryCustomerAccountDTO.InterestBalance;

                        if (beneficiaryCustomerAccountDTO.BookBalance * -1 <= 0m) // IFF has no loan balance, zeroize arrears
                        {
                            var zeroizeCustomerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Interest, interestArrears * -1/*-ve cos is payment*/, reference);
                            zeroizeCustomerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(zeroizeCustomerAccountInterestArrearage);

                            var zeroizeCustomerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Principal, principalArrears * -1/*-ve cos is payment*/, reference);
                            zeroizeCustomerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(zeroizeCustomerAccountPrincipalArrearage);
                        }
                        else
                        {
                            if (beneficiaryCustomerAccountDTO.PrincipalBalance * -1 < principalArrears) // IFF principal balance is less than principal arrears
                                principalArrears = beneficiaryCustomerAccountDTO.PrincipalBalance * -1;

                            if (beneficiaryCustomerAccountDTO.InterestBalance * -1 < interestArrears)  // IFF interest balance is less than interest arrears
                                interestArrears = beneficiaryCustomerAccountDTO.InterestBalance * -1; ;

                            var targetLoanProduct = _loanProductAppService.FindCachedLoanProduct(beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                            // do we need to charge for arrears
                            if (targetLoanProduct.LoanRegistrationChargeArrearsFee)
                            {
                                var loanArrearsFeeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(targetLoanProduct.Id, (int)LoanProductKnownChargeType.LoanArrearsFee, (interestArrears + principalArrears), (interestArrears + principalArrears), benefactorCustomerAccount, serviceHeader);

                                if (loanArrearsFeeTariffs != null && loanArrearsFeeTariffs.Any())
                                {
                                    loanArrearsFeeTariffs.ForEach(tariff =>
                                    {
                                        var actualTariffAmount = tariff.Amount;

                                        // track deductions 
                                        totalRecoveryDeductions += actualTariffAmount;

                                        // Do we need to reset expected values?
                                        if (!((availableBalance - totalRecoveryDeductions) >= 0m))
                                        {
                                            // reset deductions so far
                                            totalRecoveryDeductions = totalRecoveryDeductions - actualTariffAmount;

                                            // how much is available for recovery?
                                            var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                            // reset expected tariff amount
                                            actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                            // track deductions
                                            totalRecoveryDeductions += actualTariffAmount;
                                        }

                                        var loanArrearsTariffJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(loanArrearsTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, benefactorCustomerAccount, benefactorCustomerAccount, serviceHeader);
                                        journals.Add(loanArrearsTariffJournal);

                                        // reset available balance
                                        availableBalance -= actualTariffAmount;
                                    });
                                }
                            }

                            // track deductions
                            totalRecoveryDeductions += principalArrears;
                            totalRecoveryDeductions += interestArrears;

                            // Do we need to reset expected values?
                            if (!((availableBalance - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - (interestArrears + principalArrears);

                                // how much is available for recovery?
                                var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                interestArrears = Math.Min(interestArrears, availableRecoveryAmount);
                                principalArrears = Math.Min(principalArrears, (availableRecoveryAmount - interestArrears));

                                // track deductions
                                totalRecoveryDeductions += principalArrears;
                                totalRecoveryDeductions += interestArrears;
                            }

                            // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedLoanInterestReceivableJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, interestArrears, string.Format("Interest Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivableJournal, targetLoanProduct.InterestReceivableChartOfAccountId, targetSavingsProduct.ChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccount, serviceHeader);
                            journals.Add(attachedLoanInterestReceivableJournal);

                            // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                            var attachedLoanInterestReceivedJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, interestArrears, string.Format("Interest Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivedJournal, targetLoanProduct.InterestReceivedChartOfAccountId, targetLoanProduct.InterestChargedChartOfAccountId, beneficiaryCustomerAccountDTO, beneficiaryCustomerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanInterestReceivedJournal);

                            // Credit LoanProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedLoanPrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, principalArrears, string.Format("Principal Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanPrincipalJournal, targetLoanProduct.ChartOfAccountId, targetSavingsProduct.ChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccount, serviceHeader);
                            journals.Add(attachedLoanPrincipalJournal);

                            // Do we need to update arrearage history?
                            var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountId, (int)ArrearageCategory.Interest, interestArrears * -1/*-ve cos is payment*/, reference);
                            customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(customerAccountInterestArrearage);

                            var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountId, (int)ArrearageCategory.Principal, principalArrears * -1/*-ve cos is payment*/, reference);
                            customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(customerAccountPrincipalArrearage);

                            // reset available balance
                            availableBalance -= principalArrears;
                            availableBalance -= interestArrears;
                        }
                    }
                }
            }

            return new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);
        }

        private Tuple<decimal, decimal> RecoverAttachedInvestments(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, List<CustomerAccountArrearage> customerAccountArrearages, List<StandingOrderHistory> standingOrderHistories, CustomerAccountDTO benefactorCustomerAccount, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader)
        {
            var creditTypeAttachedProducts = _creditTypeAppService.FindCachedAttachedProducts(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

            if (creditTypeAttachedProducts != null && creditTypeAttachedProducts.InvestmentProductCollection != null && creditTypeAttachedProducts.InvestmentProductCollection.Any())
            {
                var attachedInvestmentsKVPList = new List<KeyValuePair<CustomerAccountDTO, StandingOrderDTO>>();

                var payoutStandingOrders = FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccount.Id, (int)StandingOrderTrigger.Payout, serviceHeader);

                if (payoutStandingOrders != null && payoutStandingOrders.Any())
                {
                    foreach (var payoutStandingOrderDTO in payoutStandingOrders)
                    {
                        if (payoutStandingOrderDTO.IsLocked)
                            continue;

                        var beneficiaryCustomerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(payoutStandingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                        if (beneficiaryCustomerAccountDTO.Status.In((int)CustomerAccountStatus.Closed))
                            continue;

                        if (attachedInvestmentsKVPList.Any(x => x.Key.Id == beneficiaryCustomerAccountDTO.Id && x.Value.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountDTO.Id))
                            continue; /*Standing order beneficiary MUST NOT exist in KVP list - safety check for duplicate active standing orders*/

                        if (!beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Investment))
                            continue;
                        else if (creditTypeAttachedProducts.InvestmentProductCollection.Any(x => x.Id == beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId))
                        {
                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { beneficiaryCustomerAccountDTO }, serviceHeader);

                            attachedInvestmentsKVPList.Add(new KeyValuePair<CustomerAccountDTO, StandingOrderDTO>(beneficiaryCustomerAccountDTO, payoutStandingOrderDTO));
                        }
                    }
                }

                if (attachedInvestmentsKVPList.Any())
                {
                    var orderedAttachedInvestmentsKVPList = attachedInvestmentsKVPList.OrderBy(x => x.Key.BranchCompanyRecoveryPriority);

                    foreach (var attachedInvestmentsKVP in orderedAttachedInvestmentsKVPList)
                    {
                        var targetInvestmentCustomerAccount = attachedInvestmentsKVP.Key;

                        _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { targetInvestmentCustomerAccount }, serviceHeader);

                        var investmentAccountProduct = _investmentProductAppService.FindCachedInvestmentProduct(targetInvestmentCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                        var expectedPrincipal = 0m;

                        var expectedInterest = 0m;

                        var actualInvestmentPrincipal = 0m;

                        var actualInvestmentInterest = 0m;

                        var principalArrears = 0m;

                        var interestArrears = 0m;

                        switch ((ChargeType)attachedInvestmentsKVP.Value.ChargeType)
                        {
                            case ChargeType.Percentage:
                                expectedPrincipal = Convert.ToDecimal((attachedInvestmentsKVP.Value.ChargePercentage * Convert.ToDouble(creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest)) / 100);
                                actualInvestmentPrincipal = expectedPrincipal;
                                break;
                            case ChargeType.FixedAmount:
                                expectedPrincipal = attachedInvestmentsKVP.Value.ChargeFixedAmount;
                                actualInvestmentPrincipal = expectedPrincipal;
                                break;
                            default:
                                break;
                        }

                        if (investmentAccountProduct.TrackArrears)
                        {
                            principalArrears = targetInvestmentCustomerAccount.PrincipalArrearagesBalance * -1 < 0m ? targetInvestmentCustomerAccount.PrincipalArrearagesBalance : 0m;
                        }

                        if (DateTime.Today >= attachedInvestmentsKVP.Value.DurationStartDate && attachedInvestmentsKVP.Value.ChargeType.In((int)ChargeType.FixedAmount))
                        {
                            if (CheckRecovery(attachedInvestmentsKVP.Value.Id, postingPeriodId, benefactorCustomerAccount.Id, creditBatchEntryDTO.CreditBatchMonth, serviceHeader))
                            {
                                // recover arrearages
                                var arrearagesTuple = RecoverArrearages(transactionOwnershipBranchId, parentJournalId, postingPeriodId, creditBatchEntryDTO, secondaryDescription, reference, moduleNavigationItemCode, journals, customerAccountArrearages, benefactorCustomerAccount, targetInvestmentCustomerAccount.Id, availableBalance, targetSavingsProduct, totalRecoveryDeductions, serviceHeader);

                                // reset deductions
                                totalRecoveryDeductions = arrearagesTuple.Item1;

                                // reset available balance
                                availableBalance = arrearagesTuple.Item2;

                                // reset actual/expected
                                actualInvestmentPrincipal = 0m;
                                actualInvestmentInterest = 0m;
                                expectedPrincipal = 0m;
                                expectedInterest = 0m;
                            }
                        }

                        if ((actualInvestmentPrincipal + actualInvestmentInterest) > 0m)
                        {
                            // track deductions
                            totalRecoveryDeductions += actualInvestmentPrincipal;
                            totalRecoveryDeductions += actualInvestmentInterest;

                            // Do we need to reset expected values?
                            if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - (actualInvestmentInterest + actualInvestmentPrincipal);

                                // how much is available for recovery?
                                var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                actualInvestmentInterest = Math.Min(actualInvestmentInterest, availableRecoveryAmount);
                                actualInvestmentPrincipal = Math.Min(actualInvestmentPrincipal, (availableRecoveryAmount - actualInvestmentInterest));

                                // track deductions
                                totalRecoveryDeductions += actualInvestmentPrincipal;
                                totalRecoveryDeductions += actualInvestmentInterest;

                                // track new arrears
                                if (investmentAccountProduct.TrackArrears)
                                {
                                    principalArrears += (expectedPrincipal - actualInvestmentPrincipal);
                                    interestArrears += (expectedInterest - actualInvestmentInterest);
                                }
                            }
                            else
                            {
                                // reset old arrears
                                if (investmentAccountProduct.TrackArrears)
                                {
                                    interestArrears = interestArrears - actualInvestmentInterest;
                                    interestArrears = interestArrears * -1 > 0m ? 0m : interestArrears;

                                    principalArrears = principalArrears - actualInvestmentPrincipal;
                                    principalArrears = principalArrears * -1 > 0m ? 0m : principalArrears;
                                }
                            }

                            // Credit Investment.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedInvestmentPrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualInvestmentPrincipal + actualInvestmentInterest, string.Format("Investment~{0}", investmentAccountProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedInvestmentPrincipalJournal, investmentAccountProduct.ChartOfAccountId, targetSavingsProduct.ChartOfAccountId, attachedInvestmentsKVP.Key, benefactorCustomerAccount, serviceHeader);
                            journals.Add(attachedInvestmentPrincipalJournal);

                            // reset available balance
                            availableBalance -= actualInvestmentPrincipal;
                            availableBalance -= actualInvestmentInterest;

                            if (DateTime.Today >= attachedInvestmentsKVP.Value.DurationStartDate)
                            {
                                var history = StandingOrderHistoryFactory.CreateStandingOrderHistory(attachedInvestmentsKVP.Value.Id, postingPeriodId, attachedInvestmentsKVP.Value.BenefactorCustomerAccountId, attachedInvestmentsKVP.Value.BeneficiaryCustomerAccountId, new Duration(attachedInvestmentsKVP.Value.DurationStartDate, attachedInvestmentsKVP.Value.DurationEndDate), new Schedule(attachedInvestmentsKVP.Value.ScheduleFrequency, attachedInvestmentsKVP.Value.ScheduleExpectedRunDate, attachedInvestmentsKVP.Value.ScheduleActualRunDate, attachedInvestmentsKVP.Value.ScheduleExecuteAttemptCount, attachedInvestmentsKVP.Value.ScheduleForceExecute), new Charge(attachedInvestmentsKVP.Value.ChargeType, attachedInvestmentsKVP.Value.ChargePercentage, attachedInvestmentsKVP.Value.ChargeFixedAmount), creditBatchEntryDTO.CreditBatchMonth, attachedInvestmentsKVP.Value.Trigger, expectedPrincipal, expectedInterest, actualInvestmentPrincipal, actualInvestmentInterest, attachedInvestmentsKVP.Value.Remarks);
                                history.CreatedBy = serviceHeader.ApplicationUserName;
                                standingOrderHistories.Add(history);

                                // Do we need to update arrearage history?
                                var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(targetInvestmentCustomerAccount.Id, (int)ArrearageCategory.Interest, interestArrears, reference);
                                customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                customerAccountArrearages.Add(customerAccountInterestArrearage);

                                var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(targetInvestmentCustomerAccount.Id, (int)ArrearageCategory.Principal, principalArrears, reference);
                                customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                customerAccountArrearages.Add(customerAccountPrincipalArrearage);

                                // Do we need to effect standing order charges?
                                if (attachedInvestmentsKVP.Value.Chargeable && ((actualInvestmentPrincipal + actualInvestmentInterest) > 0m))
                                {
                                    var standingOrderTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(targetSavingsProduct.Id, (int)SavingsProductKnownChargeType.StandingOrderFee, 0m, benefactorCustomerAccount, serviceHeader);

                                    if (standingOrderTariffs != null && standingOrderTariffs.Any())
                                    {
                                        standingOrderTariffs.ForEach(tariff =>
                                        {
                                            var actualTariffAmount = tariff.Amount;

                                            // track deductions
                                            totalRecoveryDeductions += actualTariffAmount;

                                            // Do we need to reset expected values?
                                            if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                                            {
                                                // reset deductions so far
                                                totalRecoveryDeductions = totalRecoveryDeductions - actualTariffAmount;

                                                // how much is available for recovery?
                                                var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                                // reset expected tariff amount
                                                actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                                // track deductions
                                                totalRecoveryDeductions += actualTariffAmount;
                                            }

                                            var standingOrderTariffJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(standingOrderTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, benefactorCustomerAccount, benefactorCustomerAccount, serviceHeader);
                                            journals.Add(standingOrderTariffJournal);

                                            // reset available balance
                                            availableBalance -= actualTariffAmount;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);
        }

        private Tuple<decimal, decimal> RecoverAttachedSavings(Guid transactionOwnershipBranchId, Guid? parentJournalId, Guid postingPeriodId, CreditBatchEntryDTO creditBatchEntryDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, List<Journal> journals, List<StandingOrderHistory> standingOrderHistories, CustomerAccountDTO benefactorCustomerAccount, decimal availableBalance, SavingsProductDTO targetSavingsProduct, decimal totalRecoveryDeductions, ServiceHeader serviceHeader)
        {
            var creditTypeAttachedProducts = _creditTypeAppService.FindCachedAttachedProducts(creditBatchEntryDTO.CreditBatchCreditTypeId, serviceHeader);

            if (creditTypeAttachedProducts != null && creditTypeAttachedProducts.SavingsProductCollection != null && creditTypeAttachedProducts.SavingsProductCollection.Any())
            {
                var attachedSavingsKVPList = new List<KeyValuePair<CustomerAccountDTO, StandingOrderDTO>>();

                var payoutStandingOrders = FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccount.Id, (int)StandingOrderTrigger.Payout, serviceHeader);

                if (payoutStandingOrders != null && payoutStandingOrders.Any())
                {
                    foreach (var payoutStandingOrderDTO in payoutStandingOrders)
                    {
                        if (payoutStandingOrderDTO.IsLocked)
                            continue;

                        var beneficiaryCustomerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(payoutStandingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                        if (beneficiaryCustomerAccountDTO.Status.In((int)CustomerAccountStatus.Closed))
                            continue;

                        if (attachedSavingsKVPList.Any(x => x.Key.Id == beneficiaryCustomerAccountDTO.Id && x.Value.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountDTO.Id))
                            continue; /*Standing order beneficiary MUST NOT exist in KVP list - safety check for duplicate active standing orders*/

                        if (!beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Savings))
                            continue;
                        else if (creditTypeAttachedProducts.SavingsProductCollection.Any(x => x.Id == beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId) && beneficiaryCustomerAccountDTO.Id != creditBatchEntryDTO.CustomerAccountCustomerId)
                        {
                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { beneficiaryCustomerAccountDTO }, serviceHeader);

                            attachedSavingsKVPList.Add(new KeyValuePair<CustomerAccountDTO, StandingOrderDTO>(beneficiaryCustomerAccountDTO, payoutStandingOrderDTO));
                        }
                    }
                }

                if (attachedSavingsKVPList.Any())
                {
                    var orderedAttachedSavingsKVP = attachedSavingsKVPList.OrderBy(x => x.Key.CustomerAccountTypeTargetProductRecoveryPriority);

                    foreach (var attachedSavingsKVP in orderedAttachedSavingsKVP)
                    {
                        var targetSavingsCustomerAccount = attachedSavingsKVP.Key;

                        _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { targetSavingsCustomerAccount }, serviceHeader);

                        var savingsAccountProduct = _savingsProductAppService.FindCachedSavingsProduct(targetSavingsCustomerAccount.CustomerAccountTypeTargetProductId, Guid.Empty, serviceHeader);

                        var expectedSavingsPrincipal = 0m;

                        var expectedSavingsInterest = 0m;

                        var actualSavingsPrincipal = 0m;

                        var actualSavingsInterest = 0m;

                        switch ((ChargeType)attachedSavingsKVP.Value.ChargeType)
                        {
                            case ChargeType.Percentage:
                                expectedSavingsPrincipal = Math.Round(Convert.ToDecimal((attachedSavingsKVP.Value.ChargePercentage * Convert.ToDouble(creditBatchEntryDTO.Principal + creditBatchEntryDTO.Interest)) / 100), 4, MidpointRounding.AwayFromZero);
                                actualSavingsPrincipal = expectedSavingsPrincipal;
                                break;
                            case ChargeType.FixedAmount:
                                expectedSavingsPrincipal = attachedSavingsKVP.Value.ChargeFixedAmount;
                                actualSavingsPrincipal = expectedSavingsPrincipal;
                                break;
                            default:
                                break;
                        }

                        if (DateTime.Today >= attachedSavingsKVP.Value.DurationStartDate)
                        {
                            if (CheckRecovery(attachedSavingsKVP.Value.Id, postingPeriodId, benefactorCustomerAccount.Id, creditBatchEntryDTO.CreditBatchMonth, serviceHeader))
                            {
                                // arrears
                                actualSavingsPrincipal = Math.Min(targetSavingsCustomerAccount.PrincipalArrearagesBalance, actualSavingsPrincipal);
                                actualSavingsInterest = Math.Min(targetSavingsCustomerAccount.InterestArrearagesBalance, actualSavingsInterest);
                            }
                        }

                        if ((actualSavingsPrincipal + actualSavingsInterest) > 0m)
                        {
                            // track deductions
                            totalRecoveryDeductions += actualSavingsPrincipal;
                            totalRecoveryDeductions += actualSavingsInterest;

                            // Do we need to reset expected values?
                            if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - (actualSavingsInterest + actualSavingsPrincipal);

                                // how much is available for recovery?
                                var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                actualSavingsInterest = Math.Min(actualSavingsInterest, availableRecoveryAmount);
                                actualSavingsPrincipal = Math.Min(actualSavingsPrincipal, (availableRecoveryAmount - actualSavingsInterest));

                                // track deductions
                                totalRecoveryDeductions += actualSavingsPrincipal;
                                totalRecoveryDeductions += actualSavingsInterest;
                            }

                            // Credit SavingsProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedSavingsPrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualSavingsPrincipal + actualSavingsInterest, string.Format("Savings~{0}", savingsAccountProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedSavingsPrincipalJournal, savingsAccountProduct.ChartOfAccountId, targetSavingsProduct.ChartOfAccountId, targetSavingsCustomerAccount, benefactorCustomerAccount, serviceHeader);
                            journals.Add(attachedSavingsPrincipalJournal);

                            // reset available balance
                            availableBalance -= actualSavingsPrincipal;
                            availableBalance -= actualSavingsInterest;

                            if (DateTime.Today >= attachedSavingsKVP.Value.DurationStartDate)
                            {
                                var history = StandingOrderHistoryFactory.CreateStandingOrderHistory(attachedSavingsKVP.Value.Id, postingPeriodId, attachedSavingsKVP.Value.BenefactorCustomerAccountId, attachedSavingsKVP.Value.BeneficiaryCustomerAccountId, new Duration(attachedSavingsKVP.Value.DurationStartDate, attachedSavingsKVP.Value.DurationEndDate), new Schedule(attachedSavingsKVP.Value.ScheduleFrequency, attachedSavingsKVP.Value.ScheduleExpectedRunDate, attachedSavingsKVP.Value.ScheduleActualRunDate, attachedSavingsKVP.Value.ScheduleExecuteAttemptCount, attachedSavingsKVP.Value.ScheduleForceExecute), new Charge(attachedSavingsKVP.Value.ChargeType, attachedSavingsKVP.Value.ChargePercentage, attachedSavingsKVP.Value.ChargeFixedAmount), creditBatchEntryDTO.CreditBatchMonth, attachedSavingsKVP.Value.Trigger, expectedSavingsPrincipal, expectedSavingsInterest, actualSavingsPrincipal, actualSavingsInterest, attachedSavingsKVP.Value.Remarks);
                                history.CreatedBy = serviceHeader.ApplicationUserName;
                                standingOrderHistories.Add(history);

                                // Do we need to effect standing order charges?
                                if (attachedSavingsKVP.Value.Chargeable && ((actualSavingsPrincipal + actualSavingsInterest) > 0m))
                                {
                                    var standingOrderTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(targetSavingsProduct.Id, (int)SavingsProductKnownChargeType.StandingOrderFee, 0m, benefactorCustomerAccount, serviceHeader);

                                    if (standingOrderTariffs != null && standingOrderTariffs.Any())
                                    {
                                        standingOrderTariffs.ForEach(tariff =>
                                        {
                                            var actualTariffAmount = tariff.Amount;

                                            // track deductions
                                            totalRecoveryDeductions += actualTariffAmount;

                                            // Do we need to reset expected values?
                                            if (!(((availableBalance) - totalRecoveryDeductions) >= 0m))
                                            {
                                                // reset deductions so far
                                                totalRecoveryDeductions = totalRecoveryDeductions - actualTariffAmount;

                                                // how much is available for recovery?
                                                var availableRecoveryAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                                // reset expected tariff amount
                                                actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                                // track deductions
                                                totalRecoveryDeductions += actualTariffAmount;
                                            }

                                            var standingOrderTariffJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, transactionOwnershipBranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.CreditBatchPayout, UberUtil.GetLastDayOfMonth(creditBatchEntryDTO.CreditBatchMonth, creditBatchEntryDTO.CreditBatchPostingPeriodDurationEndDate.Year, creditBatchEntryDTO.CreditBatchEnforceMonthValueDate, creditBatchEntryDTO.CreditBatchValueDate), serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(standingOrderTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, benefactorCustomerAccount, benefactorCustomerAccount, serviceHeader);
                                            journals.Add(standingOrderTariffJournal);

                                            // reset available balance
                                            availableBalance -= actualTariffAmount;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new Tuple<decimal, decimal>(totalRecoveryDeductions, availableBalance);
        }

        private List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            if (beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBeneficiaryCustomerAccountIdAndTrigger(beneficiaryCustomerAccountId, trigger);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerAccountIdAndTrigger(benefactorCustomerAccountId, trigger);

                    ISpecification<StandingOrder> spec = filter;

                    var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                    if (standingOrders != null && standingOrders.Any())
                    {
                        return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private bool MarkCreditBatchEntryPosted(Guid creditBatchEntryId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (creditBatchEntryId == null || creditBatchEntryId == Guid.Empty)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _creditBatchEntryRepository.Get(creditBatchEntryId, serviceHeader);

                if (persisted != null)
                {
                    switch ((BatchEntryStatus)persisted.Status)
                    {
                        case BatchEntryStatus.Pending:
                            persisted.Status = (int)BatchEntryStatus.Posted;
                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        private bool CheckRecovery(Guid standingOrderId, Guid postingPeriodId, Guid benefactorCustomerAccountId, int month, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderHistorySpecifications.StandingOrderHistory(standingOrderId, postingPeriodId, month);

                ISpecification<StandingOrderHistory> spec = filter;

                var standingOrderHistories = _standingOrderHistoryRepository.AllMatching(spec, serviceHeader);

                if (standingOrderHistories != null)
                {
                    result = standingOrderHistories.Any(x => x.BenefactorCustomerAccountId == benefactorCustomerAccountId);
                }
            }

            return result;
        }
    }
}