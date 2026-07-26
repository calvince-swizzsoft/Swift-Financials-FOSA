using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class JournalAppService : IJournalAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Journal> _journalRepository;
        private readonly IRepository<JournalEntry> _journalEntryRepository;
        private readonly IRepository<AlternateChannelLog> _alternateChannelLogRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public JournalAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<Journal> journalRepository,
            IRepository<JournalEntry> journalEntryRepository,
            IRepository<AlternateChannelLog> alternateChannelLogRepository,
            IPostingPeriodAppService postingPeriodAppService,
            IJournalEntryPostingService journalEntryPostingService,
            IInvestmentProductAppService investmentProductAppService,
            ILoanProductAppService loanProductAppService,
            ISavingsProductAppService savingsProductAppService,
            ICommissionAppService commissionAppService,
            ICustomerAccountAppService customerAccountAppService,
            ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (journalRepository == null)
                throw new ArgumentNullException(nameof(journalRepository));

            if (journalEntryRepository == null)
                throw new ArgumentNullException(nameof(journalEntryRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (alternateChannelLogRepository == null)
                throw new ArgumentNullException(nameof(alternateChannelLogRepository));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _journalRepository = journalRepository;
            _journalEntryRepository = journalEntryRepository;
            _alternateChannelLogRepository = alternateChannelLogRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _commissionAppService = commissionAppService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public JournalDTO AddNewJournal(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, ServiceHeader serviceHeader, bool useCache)
        {
            var postingPeriod = useCache ? _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader) : _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

            if (postingPeriod != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var journal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, serviceHeader);

                    _journalEntryPostingService.PerformDoubleEntry(journal, creditChartOfAccountId, debitChartOfAccountId, serviceHeader);

                    _journalRepository.Add(journal, serviceHeader);

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return journal.ProjectedAs<JournalDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }


        public JournalDTO AddNewJournalSingleEntry(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid chartOfAccountId, Guid contraChartOfAccountId, int journalType, ServiceHeader serviceHeader, bool useCache)
        {
            var postingPeriod = useCache ? _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader) : _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

            if (postingPeriod != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var journal = JournalFactory.CreateJournal(branchId, postingPeriod.Id, branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, serviceHeader);


                    switch ((JournalVoucherType)journalType)
                    {
                        case JournalVoucherType.CreditGLAccount:
                            _journalEntryPostingService.PerformSingleEntry(journal, chartOfAccountId, contraChartOfAccountId, totalValue * -1, serviceHeader);
                            break;
                        case JournalVoucherType.DebitGLAccount:
                            _journalEntryPostingService.PerformSingleEntry(journal, chartOfAccountId, contraChartOfAccountId, totalValue, serviceHeader);
                            break;
                        //case JournalVoucherType.DebitCustomerAccount:
                        //    _journalEntryPostingService.PerformSingleEntry(journal, chartOfAccountId, contraChartOfAccountId, journalVoucherDTO.CustomerAccountId.Value, totalValue * -1, serviceHeader);
                        //    break;
                        //case JournalVoucherType.CreditCustomerAccount:
                        //    _journalEntryPostingService.PerformSingleEntry(journal, chartOfAccountId, chartOfAccountId, journalVoucherDTO.CustomerAccountId.Value, journalVoucherDTO.TotalValue, serviceHeader);
                        //    break;           
                        default:
                            break;
                    }
                    //_journalEntryPostingService.PerformSingleEntry(journal, chartOfAccountId, contraChartOfAccountId, totalValue, serviceHeader);

                    _journalRepository.Add(journal, serviceHeader);

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return journal.ProjectedAs<JournalDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public JournalDTO AddNewJournal(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var postingPeriod = useCache ? _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader) : _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

            if (postingPeriod != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var journal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, serviceHeader);

                    _journalEntryPostingService.PerformDoubleEntry(journal, creditChartOfAccountId, debitChartOfAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader);

                    _journalRepository.Add(journal, serviceHeader);

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return journal.ProjectedAs<JournalDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, ServiceHeader serviceHeader, bool useCache)
        {
            return AddNewJournal(null, branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, serviceHeader, useCache);
        }

        public JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<ApportionmentWrapper> apportionments, List<TariffWrapper> tariffs, List<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader, bool useCache)
        {
            JournalDTO journal = null;

            if (creditCustomerAccountDTO != null && debitCustomerAccountDTO != null)
            {
                if (tariffs != null && tariffs.Any())
                {
                    tariffs.ForEach(item =>
                    {
                        journal = AddNewJournal(null, branchId, alternateChannelLogId, item.Amount, item.Description, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, item.CreditGLAccountId, item.DebitGLAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader, useCache);
                    });
                }
            }

            if (apportionments != null && apportionments.Any())
            {
                apportionments.ForEach(item =>
                {
                    switch ((ApportionTo)item.ApportionTo)
                    {
                        case ApportionTo.CustomerAccount:

                            #region apportionment

                            switch ((ProductCode)item.CreditCustomerAccount.CustomerAccountTypeProductCode)
                            {
                                case ProductCode.Loan:

                                    if (item.RecoverCarryForwards)
                                    {
                                        var postingPeriod = useCache ? _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader) : _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                                        if (postingPeriod != null)
                                        {
                                            RecoverSundryCarryFowards(null, branchId, alternateChannelLogId, postingPeriod.Id, item.DebitCustomerAccount, item.CreditCustomerAccount, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, serviceHeader, useCache);

                                            journal = new JournalDTO { };/*hack 13-12-2016 izmoto*/
                                        }
                                    }

                                    var loanProductDTO = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CreditCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CreditCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                                    if (item.Interest * -1 < 0m)
                                    {
                                        // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit 'debitChartOfAccountId'
                                        journal = AddNewJournal(branchId, alternateChannelLogId, item.Interest, item.PrimaryDescription, item.SecondaryDescription, string.Format("{0}>IntReceivable", item.Reference), moduleNavigationItemCode, transactionCode, valueDate, loanProductDTO.InterestReceivableChartOfAccountId, debitChartOfAccountId, item.CreditCustomerAccount, item.DebitCustomerAccount, serviceHeader, useCache);

                                        // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                                        journal = AddNewJournal(branchId, alternateChannelLogId, item.Interest, item.PrimaryDescription, item.SecondaryDescription, string.Format("{0}>IntReceived", item.Reference), moduleNavigationItemCode, transactionCode, valueDate, loanProductDTO.InterestReceivedChartOfAccountId, loanProductDTO.InterestChargedChartOfAccountId, item.DebitCustomerAccount, item.DebitCustomerAccount, serviceHeader, useCache);
                                    }

                                    // Credit LoanProduct.ChartOfAccountId, Debit 'debitChartOfAccountId'
                                    if (item.Principal * -1 < 0m)
                                        journal = AddNewJournal(branchId, alternateChannelLogId, item.Principal, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, loanProductDTO.ChartOfAccountId, debitChartOfAccountId, item.CreditCustomerAccount, item.DebitCustomerAccount, serviceHeader, useCache);

                                    break;
                                case ProductCode.Investment:

                                    var investmentProductDTO = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CreditCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CreditCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                                    // Credit InvestmentProduct.ChartOfAccountId, Debit 'debitChartOfAccountId'
                                    if (item.Principal * -1 < 0m)
                                        journal = AddNewJournal(branchId, alternateChannelLogId, item.Principal, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, investmentProductDTO.ChartOfAccountId, debitChartOfAccountId, item.CreditCustomerAccount, item.DebitCustomerAccount, serviceHeader, useCache);

                                    // Credit InvestmentProduct.ChartOfAccountId, Debit 'debitChartOfAccountId'
                                    if (item.Interest * -1 < 0m)
                                        journal = AddNewJournal(branchId, alternateChannelLogId, item.Interest, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, investmentProductDTO.ChartOfAccountId, debitChartOfAccountId, item.CreditCustomerAccount, item.DebitCustomerAccount, serviceHeader, useCache);

                                    break;

                                case ProductCode.Savings:

                                    var savingsProductDTO = _savingsProductAppService.FindSavingsProduct(item.CreditCustomerAccount.CustomerAccountTypeTargetProductId, item.CreditCustomerAccount.BranchId, serviceHeader);

                                    // Credit SavingsProduct.ChartOfAccountId, Debit 'debitChartOfAccountId'
                                    if (item.Principal * -1 < 0m)
                                        journal = AddNewJournal(branchId, alternateChannelLogId, item.Principal, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, savingsProductDTO.ChartOfAccountId, debitChartOfAccountId, item.CreditCustomerAccount, item.DebitCustomerAccount, serviceHeader, useCache);

                                    // Credit SavingsProduct.ChartOfAccountId, Debit 'debitChartOfAccountId'
                                    if (item.Interest * -1 < 0m)
                                        journal = AddNewJournal(branchId, alternateChannelLogId, item.Interest, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, savingsProductDTO.ChartOfAccountId, debitChartOfAccountId, item.CreditCustomerAccount, item.DebitCustomerAccount, serviceHeader, useCache);

                                    break;
                                default:
                                    break;
                            }

                            #endregion

                            #region dynamic charges

                            List<TariffWrapper> dynamicChargeTariffs = null;

                            CustomerAccountDTO dynamicChargeTariffCustomerAccountDTO = null;

                            switch ((ProductCode)item.DebitCustomerAccount.CustomerAccountTypeProductCode)
                            {
                                case ProductCode.Savings:

                                    dynamicChargeTariffs = _commissionAppService.ComputeTariffsByDynamicCharges(dynamicCharges, (item.Principal + item.Interest), item.DebitCustomerAccount, serviceHeader);

                                    dynamicChargeTariffCustomerAccountDTO = item.DebitCustomerAccount;

                                    break;
                                case ProductCode.Loan:
                                case ProductCode.Investment:

                                    var defaultSavingsProduct = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

                                    if (defaultSavingsProduct != null)
                                    {
                                        var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(defaultSavingsProduct.Id, item.DebitCustomerAccount.CustomerId, serviceHeader);

                                        if (customerAccounts != null && customerAccounts.Any())
                                        {
                                            foreach (var customerAccount in customerAccounts)
                                            {
                                                dynamicChargeTariffs = _commissionAppService.ComputeTariffsByDynamicCharges(dynamicCharges, (item.Principal + item.Interest), customerAccount, serviceHeader);

                                                dynamicChargeTariffCustomerAccountDTO = customerAccount;

                                                break;
                                            }
                                        }
                                    }

                                    break;
                                default:
                                    break;
                            }

                            if (dynamicChargeTariffs != null && dynamicChargeTariffs.Any())
                            {
                                dynamicChargeTariffs.ForEach(tariff =>
                                {
                                    journal = AddNewJournal(null, branchId, alternateChannelLogId, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, tariff.CreditGLAccountId, tariff.DebitGLAccountId, dynamicChargeTariffCustomerAccountDTO, dynamicChargeTariffCustomerAccountDTO, serviceHeader, useCache);
                                });
                            }

                            #endregion

                            break;
                        case ApportionTo.GeneralLedgerAccount:

                            // Credit item.CreditChartOfAccountId, Debit 'debitChartOfAccountId'
                            if (item.Principal * -1 < 0m)
                            {
                                if (debitCustomerAccountDTO != null)
                                    journal = AddNewJournal(branchId, alternateChannelLogId, item.Principal, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, item.CreditChartOfAccountId, debitChartOfAccountId, debitCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader, useCache);
                                else journal = AddNewJournal(branchId, alternateChannelLogId, item.Principal, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, item.CreditChartOfAccountId, debitChartOfAccountId, serviceHeader, useCache);
                            }

                            // Credit item.CreditChartOfAccountId, Debit 'debitChartOfAccountId'
                            if (item.Interest * -1 < 0m)
                            {
                                if (debitCustomerAccountDTO != null)
                                    journal = AddNewJournal(branchId, alternateChannelLogId, item.Interest, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, item.CreditChartOfAccountId, debitChartOfAccountId, debitCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader, useCache);
                                else journal = AddNewJournal(branchId, alternateChannelLogId, item.Interest, item.PrimaryDescription, item.SecondaryDescription, item.Reference, moduleNavigationItemCode, transactionCode, valueDate, item.CreditChartOfAccountId, debitChartOfAccountId, serviceHeader, useCache);
                            }

                            break;
                        default:
                            break;
                    }
                });
            }

            return journal;
        }

        public JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            return AddNewJournal(null, branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader, useCache);
        }

        public JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs, ServiceHeader serviceHeader, bool useCache)
        {
            var journal = AddNewJournal(null, branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader, useCache);

            if (journal != null)
            {
                if (tariffs != null && tariffs.Any())
                {
                    tariffs.ForEach(item =>
                    {
                        AddNewJournal(journal.Id, branchId, alternateChannelLogId, item.Amount, item.Description, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, item.CreditGLAccountId, item.DebitGLAccountId, item.CreditCustomerAccount ?? creditCustomerAccountDTO, item.DebitCustomerAccount ?? debitCustomerAccountDTO, serviceHeader, useCache);
                    });
                }
            }

            return journal;
        }

        public JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, List<TariffWrapper> tariffs, ServiceHeader serviceHeader, bool useCache)
        {
            var journal = AddNewJournal(null, branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, serviceHeader, useCache);

            if (tariffs != null && tariffs.Any())
            {
                tariffs.ForEach(item =>
                {
                    AddNewJournal(journal.Id, branchId, alternateChannelLogId, item.Amount, item.Description, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, item.CreditGLAccountId, item.DebitGLAccountId, serviceHeader, useCache);
                });
            }

            return journal;
        }

        public bool AddNewJournals(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs, ServiceHeader serviceHeader, bool useCache)
        {
            var result = default(bool);

            if (tariffs != null && tariffs.Any())
            {
                var postingPeriod = useCache ? _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader) : _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                if (postingPeriod != null)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        tariffs.ForEach(item =>
                        {
                            var journal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, branchId, alternateChannelLogId, item.Amount, item.Description, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, serviceHeader, true);

                            _journalEntryPostingService.PerformDoubleEntry(journal, item.CreditGLAccountId, item.DebitGLAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader);

                            _journalRepository.Add(journal, serviceHeader);
                        });

                        result = (dbContextScope.SaveChanges(serviceHeader) >= 0);
                    }
                }
            }

            return result;
        }

        public bool ReverseAlternateChannelJournals(Guid[] alternateChannelLogIds, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var alternateChannelLogDTOs = FindAlternateChannelLogs(alternateChannelLogIds, serviceHeader);

            if (alternateChannelLogDTOs != null && alternateChannelLogDTOs.Any())
            {
                var kvp = new Dictionary<Guid, IEnumerable<JournalDTO>>();

                foreach (var alternateChannelLogDTO in alternateChannelLogDTOs)
                {
                    if (alternateChannelLogDTO.IsReversed) continue;

                    var journalDTOs = FindJournals(new Guid[] { alternateChannelLogDTO.Id }, serviceHeader);

                    if (journalDTOs != null && journalDTOs.Any())
                        kvp.Add(alternateChannelLogDTO.Id, journalDTOs.Where(x => !x.IsLocked));
                }

                if (kvp.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in kvp)
                        {
                            var persisted = _alternateChannelLogRepository.Get(item.Key, serviceHeader);

                            if (persisted != null)
                            {
                                persisted.MarkReversed();
                            }
                        }

                        dbContextScope.SaveChanges(serviceHeader);
                    }

                    var targetJournals = new List<JournalDTO>();

                    foreach (var item in kvp)
                    {
                        targetJournals.AddRange(item.Value);
                    }

                    result = ReverseJournals(targetJournals, null, moduleNavigationItemCode, serviceHeader);
                }
            }

            return result;
        }

        public bool ReverseJournals(List<JournalDTO> journalDTOs, string description, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (journalDTOs != null && journalDTOs.Any())
            {
                var journals = new List<Journal>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var journalDTO in journalDTOs)
                    {
                        if (journalDTO.IsLocked) continue;

                        var primaryDescription = string.Format("{0}~{1}", description ?? "Reversal", journalDTO.PrimaryDescription);
                        var secondaryDescription = journalDTO.SecondaryDescription;
                        var reference = journalDTO.Reference;

                        var reversalJournal = JournalFactory.CreateJournal(null, journalDTO.PostingPeriodId, journalDTO.BranchId, journalDTO.AlternateChannelLogId, journalDTO.TotalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, journalDTO.TransactionCode, null, serviceHeader);

                        #region  mark reversal journal locked

                        reversalJournal.Lock();

                        #endregion

                        var journalEntries = FindJournalEntries(serviceHeader, journalDTO.Id);

                        if (journalEntries != null && journalEntries.Any())
                        {
                            foreach (var journalEntry in journalEntries)
                            {
                                if (journalEntry.Amount * -1 > 0m)
                                {
                                    #region DR

                                    var reverseDebitJournalEntry = JournalEntryFactory.CreateJournalEntry(reversalJournal.Id, journalEntry.ChartOfAccountId, journalEntry.ContraChartOfAccountId, journalEntry.CustomerAccountId, journalEntry.Amount * -1, null, serviceHeader);

                                    reversalJournal.JournalEntries.Add(reverseDebitJournalEntry);

                                    #endregion
                                }
                                else
                                {
                                    #region CR

                                    var reverseCreditJournalEntry = JournalEntryFactory.CreateJournalEntry(reversalJournal.Id, journalEntry.ChartOfAccountId, journalEntry.ContraChartOfAccountId, journalEntry.CustomerAccountId, journalEntry.Amount * -1, null, serviceHeader);

                                    reversalJournal.JournalEntries.Add(reverseCreditJournalEntry);

                                    #endregion
                                }
                            }
                        }

                        journals.Add(reversalJournal);

                        #region mark original journal locked

                        var persisted = _journalRepository.Get(journalDTO.Id, serviceHeader);
                        persisted.Lock();

                        #endregion
                    }

                    #region commit lock flags

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    #endregion
                }

                if (result && journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
            }

            return result;
        }


        public JournalDTO FindJournal(Guid journalId, ServiceHeader serviceHeader)
        {
            if (journalId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var journal = _journalRepository.Get(journalId, serviceHeader);

                    if (journal != null)
                    {
                        return journal.ProjectedAs<JournalDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<JournalDTO> FindJournals(Guid[] alternateChannelLogIds, ServiceHeader serviceHeader)
        {
            if (alternateChannelLogIds != null && alternateChannelLogIds.Any())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalSpecifications.JournalWithAlternateChannelLogId(alternateChannelLogIds);

                    ISpecification<Journal> spec = filter;

                    var journals = _journalRepository.AllMatching(spec, serviceHeader);

                    if (journals != null && journals.Any())
                    {
                        return journals.ProjectedAsCollection<JournalDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<JournalDTO> FindJournals(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalFilter, ServiceHeader serviceHeader)
        {
            if (startDate != null && endDate != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalSpecifications.JournalWithDateRangeAndFullText(startDate, endDate, text, journalFilter);

                    ISpecification<Journal> spec = filter;

                    var sortFields = new List<string> { "CreatedDate" };

                    var journalPagedCollection = _journalRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (journalPagedCollection != null)
                    {
                        var pageCollection = journalPagedCollection.PageCollection.ProjectedAsCollection<JournalDTO>();

                        var itemsCount = journalPagedCollection.ItemsCount;

                        return new PageCollectionInfo<JournalDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<JournalDTO> FindReversibleJournals(int pageIndex, int pageSize, int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalFilter, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = JournalSpecifications.ReversibleJournalWithDateRangeAndFullText(systemTransactionCode, startDate, endDate, text, journalFilter, serviceHeader);

                ISpecification<Journal> spec = filter;

                var sortFields = new List<string> { "CreatedDate" };

                var journalPagedCollection = _journalRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (journalPagedCollection != null)
                {
                    var pageCollection = journalPagedCollection.PageCollection.ProjectedAsCollection<JournalDTO>();

                    var itemsCount = journalPagedCollection.ItemsCount;

                    return new PageCollectionInfo<JournalDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<JournalEntryDTO> FindJournalEntries(ServiceHeader serviceHeader, params Guid[] journalIds)
        {
            if (journalIds != null && journalIds.Any())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalEntrySpecifications.JournalEntriesWithJournalIds(journalIds);

                    ISpecification<JournalEntry> spec = filter;

                    var journalEntries = _journalEntryRepository.AllMatching(spec, serviceHeader);

                    if (journalEntries != null)
                    {
                        return journalEntries.ProjectedAsCollection<JournalEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<JournalEntryDTO> FindJournalEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader, params Guid[] journalIds)
        {
            if (journalIds != null && journalIds.Any())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = JournalEntrySpecifications.JournalEntriesWithJournalIds(journalIds);

                    ISpecification<JournalEntry> spec = filter;

                    var sortFields = new List<string> { "CreatedDate" };

                    var journalEntryPagedCollection = _journalEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (journalEntryPagedCollection != null)
                    {
                        var pageCollection = journalEntryPagedCollection.PageCollection.ProjectedAsCollection<JournalEntryDTO>();

                        var itemsCount = journalEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<JournalEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<AlternateChannelLogDTO> FindAlternateChannelLogs(Guid[] alternateChannelLogIds, ServiceHeader serviceHeader)
        {
            if (alternateChannelLogIds != null && alternateChannelLogIds.Any())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = AlternateChannelLogSpecifications.AlternateChannelLogWithId(alternateChannelLogIds);

                    ISpecification<AlternateChannelLog> spec = filter;

                    return _alternateChannelLogRepository.AllMatching<AlternateChannelLogDTO>(spec, serviceHeader);
                }
            }
            else return null;
        }

        private decimal RecoverSundryCarryFowards(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, Guid postingPeriodId, CustomerAccountDTO benefactorCustomerAccountDTO, CustomerAccountDTO beneficiaryCustomerAccountDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, ServiceHeader serviceHeader, bool useCache)
        {
            var totalRecoveryDeductions = 0m;

            var carryForwards = _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(beneficiaryCustomerAccountDTO.Id, serviceHeader);

            if (carryForwards != null && carryForwards.Any())
            {
                var journals = new List<Journal>();

                var customerAccountCarryForwards = new List<CustomerAccountCarryForward>();

                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { benefactorCustomerAccountDTO }, serviceHeader);
                _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { benefactorCustomerAccountDTO, beneficiaryCustomerAccountDTO }, serviceHeader);

                var availableBalance = 0m;

                if (benefactorCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Savings))
                    availableBalance = benefactorCustomerAccountDTO.AvailableBalance;
                else if (benefactorCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Investment))
                    availableBalance = benefactorCustomerAccountDTO.BookBalance;

                if ((availableBalance > 0m /*Will current account balance be positive?*/))
                {
                    var grouping = from p in carryForwards
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

                            if ((principalArrears + interestArrears) > 0m)
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

                                var primaryDescription = string.Format("Carry Forwards Paid~{0}", beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductDescription);

                                // Credit CarryForward.BeneficiaryChartOfAccountId, Debit Benefactor.ChartOfAccountId
                                var carryFowardBeneficiaryJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriodId, branchId, alternateChannelLogId, principalArrears, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(carryFowardBeneficiaryJournal, item.BeneficiaryChartOfAccountId, benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, benefactorCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                journals.Add(carryFowardBeneficiaryJournal);

                                // reset available balance
                                availableBalance -= principalArrears;

                                // Do we need to update carry forward history?
                                var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(benefactorCustomerAccountDTO.Id, beneficiaryCustomerAccountDTO.Id, item.BeneficiaryChartOfAccountId, principalArrears * -1/*-ve cos is payment*/, primaryDescription);
                                customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                customerAccountCarryForwards.Add(customerAccountCarryForward);
                            }
                        }
                    }
                }

                #region Bulk-Insert journals && journal entries

                _journalEntryPostingService.BulkSave(serviceHeader, journals, customerAccountCarryForwards);

                #endregion
            }

            return totalRecoveryDeductions;
        }
    }
}