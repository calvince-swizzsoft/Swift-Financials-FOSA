using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequeAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequePayableAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class ExternalChequeAppService : IExternalChequeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ExternalCheque> _externalChequeRepository;
        private readonly IRepository<ExternalChequePayable> _externalChequePayableRepository;
        private readonly IJournalAppService _journalAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IRecurringBatchAppService _recurringBatchAppService;

        public ExternalChequeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ExternalCheque> externalChequeRepository,
           IRepository<ExternalChequePayable> externalChequePayableRepository,
           IJournalAppService journalAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           ICustomerAccountAppService customerAccountAppService,
           ICommissionAppService commissionAppService,
           ISqlCommandAppService sqlCommandAppService,
           IJournalEntryPostingService journalEntryPostingService,
           IPostingPeriodAppService postingPeriodAppService,
           IRecurringBatchAppService recurringBatchAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (externalChequeRepository == null)
                throw new ArgumentNullException(nameof(externalChequeRepository));

            if (externalChequePayableRepository == null)
                throw new ArgumentNullException(nameof(externalChequePayableRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (recurringBatchAppService == null)
                throw new ArgumentNullException(nameof(recurringBatchAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _externalChequeRepository = externalChequeRepository;
            _externalChequePayableRepository = externalChequePayableRepository;
            _journalAppService = journalAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
            _commissionAppService = commissionAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _postingPeriodAppService = postingPeriodAppService;
            _recurringBatchAppService = recurringBatchAppService;
        }

        public ExternalChequeDTO AddNewExternalCheque(ExternalChequeDTO externalChequeDTO, ServiceHeader serviceHeader)
        {
            if (externalChequeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var externalCheque = ExternalChequeFactory.CreateExternalCheque(externalChequeDTO.TellerId, externalChequeDTO.ChequeTypeId, externalChequeDTO.CustomerAccountId, externalChequeDTO.Number, externalChequeDTO.Amount, externalChequeDTO.Drawer, externalChequeDTO.DrawerBank, externalChequeDTO.DrawerBankBranch, externalChequeDTO.WriteDate, externalChequeDTO.MaturityDate);

                    externalCheque.CreatedBy = serviceHeader.ApplicationUserName;

                    _externalChequeRepository.Add(externalCheque, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return externalCheque.ProjectedAs<ExternalChequeDTO>();
                }
            }
            else return null;
        }

        public bool MarkExternalChequeCleared(Guid externalChequeId, ServiceHeader serviceHeader)
        {
            if (externalChequeId == null || externalChequeId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _externalChequeRepository.Get(externalChequeId, serviceHeader);

                if (persisted != null)
                {
                    persisted.IsCleared = true;
                    persisted.ClearedBy = serviceHeader.ApplicationUserName;
                    persisted.ClearedDate = DateTime.Now;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<ExternalChequeDTO> FindExternalCheques(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var externalCheques = _externalChequeRepository.GetAll(serviceHeader);

                if (externalCheques != null && externalCheques.Any())
                {
                    return externalCheques.ProjectedAsCollection<ExternalChequeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExternalChequeDTO> FindExternalCheques(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.DefaultSpec();

                ISpecification<ExternalCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var externalChequePagedCollection = _externalChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (externalChequePagedCollection != null)
                {
                    var pageCollection = externalChequePagedCollection.PageCollection.ProjectedAsCollection<ExternalChequeDTO>();

                    var itemsCount = externalChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExternalChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExternalChequeDTO> FindExternalCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.ExternalChequeFullText(text);

                ISpecification<ExternalCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var externalChequePagedCollection = _externalChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (externalChequePagedCollection != null)
                {
                    var pageCollection = externalChequePagedCollection.PageCollection.ProjectedAsCollection<ExternalChequeDTO>();

                    var itemsCount = externalChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExternalChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExternalChequeDTO> FindExternalCheques(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.ExternalChequeFullText(startDate, endDate, text);

                ISpecification<ExternalCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var externalChequePagedCollection = _externalChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (externalChequePagedCollection != null)
                {
                    var pageCollection = externalChequePagedCollection.PageCollection.ProjectedAsCollection<ExternalChequeDTO>();

                    var itemsCount = externalChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExternalChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public ExternalChequeDTO FindExternalCheque(Guid externalChequeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var externalCheque = _externalChequeRepository.Get(externalChequeId, serviceHeader);

                if (externalCheque != null)
                {
                    return externalCheque.ProjectedAs<ExternalChequeDTO>();
                }
                else return null;
            }
        }

        public List<ExternalChequeDTO> FindUnClearedExternalChequesByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.UnClearedExternalChequesWithCustomerAccountId(customerAccountId);

                ISpecification<ExternalCheque> spec = filter;

                var externalCheques = _externalChequeRepository.AllMatching(spec, serviceHeader);

                if (externalCheques != null && externalCheques.Any())
                {
                    return externalCheques.ProjectedAsCollection<ExternalChequeDTO>();
                }
                else return null;
            }
        }

        public List<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerId(Guid tellerId, string text, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.UnTransferredExternalChequesWithTellerId(tellerId, text);

                ISpecification<ExternalCheque> spec = filter;

                var externalCheques = _externalChequeRepository.AllMatching(spec, serviceHeader);

                if (externalCheques != null && externalCheques.Any())
                {
                    return externalCheques.ProjectedAsCollection<ExternalChequeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnTransferredExternalChequesByTellerId(Guid tellerId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.UnTransferredExternalChequesWithTellerId(tellerId, text);

                ISpecification<ExternalCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var externalChequePagedCollection = _externalChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (externalChequePagedCollection != null)
                {
                    var pageCollection = externalChequePagedCollection.PageCollection.ProjectedAsCollection<ExternalChequeDTO>();

                    var itemsCount = externalChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExternalChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.UnClearedExternalCheques(text);

                ISpecification<ExternalCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var externalChequePagedCollection = _externalChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (externalChequePagedCollection != null)
                {
                    var pageCollection = externalChequePagedCollection.PageCollection.ProjectedAsCollection<ExternalChequeDTO>();

                    var itemsCount = externalChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExternalChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnClearedExternalCheques(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (startDate != null && endDate != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ExternalChequeSpecifications.UnClearedExternalChequeWithMaturityDateRangeAndFullText(startDate, endDate, text);

                    ISpecification<ExternalCheque> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var externalChequePagedCollection = _externalChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (externalChequePagedCollection != null)
                    {
                        var pageCollection = externalChequePagedCollection.PageCollection.ProjectedAsCollection<ExternalChequeDTO>();

                        var itemsCount = externalChequePagedCollection.ItemsCount;

                        return new PageCollectionInfo<ExternalChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<ExternalChequeDTO> FindUnBankedExternalCheques(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ExternalChequeSpecifications.UnBankedExternalCheques(text);

                ISpecification<ExternalCheque> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var externalChequePagedCollection = _externalChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (externalChequePagedCollection != null)
                {
                    var pageCollection = externalChequePagedCollection.PageCollection.ProjectedAsCollection<ExternalChequeDTO>();

                    var itemsCount = externalChequePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ExternalChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool TransferExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, TellerDTO tellerDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (externalChequeDTOs != null && externalChequeDTOs.Any())
            {
                var chequesInHandChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesInHand, serviceHeader);

                if (tellerDTO != null && tellerDTO.ChartOfAccountId.HasValue && chequesInHandChartOfAccountId != Guid.Empty)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        externalChequeDTOs.ForEach(externalChequeDTO =>
                        {
                            var persisted = _externalChequeRepository.Get(externalChequeDTO.Id, serviceHeader);

                            if (persisted != null && !persisted.IsTransferred)
                            {
                                persisted.IsTransferred = true;
                                persisted.TransferredBy = serviceHeader.ApplicationUserName;
                                persisted.TransferredDate = DateTime.Now;

                                _journalAppService.AddNewJournal(tellerDTO.EmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Transfer~{0}", externalChequeDTO.Number), tellerDTO.Description, externalChequeDTO.Number, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeTransfer, null, tellerDTO.ChartOfAccountId.Value, chequesInHandChartOfAccountId, serviceHeader);
                            }
                        });

                        return dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
                else throw new InvalidOperationException("Sorry, but the requisite teller and/or external cheques in hand account has not been setup!");
            }
            else return false;
        }

        public bool ClearExternalCheque(ExternalChequeDTO externalChequeDTO, int clearingOption, int moduleNavigationItemCode, UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (externalChequeDTO != null)
            {
                var chequesSuspenseChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesControl, serviceHeader);

                if (chequesSuspenseChartOfAccountId == Guid.Empty)
                    return result;

                var postingPeriodDTO = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
                if (postingPeriodDTO == null)
                    return result;

                var journals = new List<Journal>();

                var externalChequePayables = new List<ExternalChequePayableDTO>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _externalChequeRepository.Get(externalChequeDTO.Id, serviceHeader);

                    if (persisted != null && !persisted.IsCleared && persisted.CustomerAccount != null)
                    {
                        var customerAccountDTO = persisted.CustomerAccount.ProjectedAs<CustomerAccountDTO>();

                        _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader);

                        switch ((ExternalChequeClearanceOption)clearingOption)
                        {
                            case ExternalChequeClearanceOption.Pay:

                                persisted.Remarks = "PAID";
                                persisted.IsCleared = true;
                                persisted.ClearedBy = serviceHeader.ApplicationUserName;
                                persisted.ClearedDate = DateTime.Now;

                                switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                                {
                                    case ProductCode.Savings:

                                        var secondaryDescription = customerAccountDTO.CustomerAccountTypeTargetProductDescription;

                                        var reference = string.Format("Cheque #{0}", externalChequeDTO.Number);

                                        var primaryJournal = _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} PAID", externalChequeDTO.Number), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, chequesSuspenseChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                        if (externalChequeDTO.ChequeTypeId != null && externalChequeDTO.ChequeTypeId != Guid.Empty && externalChequeDTO.ChequeTypeChargeRecoveryMode == (int)ChequeTypeChargeRecoveryMode.OnChequeClearance)
                                        {
                                            var chequeTypeTariffs = _commissionAppService.ComputeTariffsByChequeType(externalChequeDTO.ChequeTypeId.Value, externalChequeDTO.Amount, customerAccountDTO, serviceHeader);

                                            if (chequeTypeTariffs != null && chequeTypeTariffs.Any())
                                            {
                                                chequeTypeTariffs.ForEach(tariff =>
                                                {
                                                    _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                });
                                            }
                                        }

                                        externalChequePayables = FindExternalChequePayablesByExternalChequeId(externalChequeDTO.Id, serviceHeader);

                                        if (externalChequePayables != null && externalChequePayables.Any())
                                        {
                                            // track deductions
                                            var savingsAccountAvailableBalance = 0m;

                                            var totalRecoveryDeductions = 0m;

                                            // Recover dues for attached products
                                            if (((savingsAccountAvailableBalance + externalChequeDTO.Amount) > 0m /*Will current account balance + incoming batch amount be positive?*/))
                                            {
                                                if (!string.IsNullOrWhiteSpace(customerAccountDTO.BranchCompanyRecoveryPriority))
                                                {
                                                    var buffer = customerAccountDTO.BranchCompanyRecoveryPriority.Split(new char[] { ',' });

                                                    Array.ForEach(buffer, (item) =>
                                                    {
                                                        switch ((RecoveryPriority)Enum.Parse(typeof(RecoveryPriority), item))
                                                        {
                                                            case RecoveryPriority.Loans:
                                                                totalRecoveryDeductions = RecoverAttachedLoans(primaryJournal.Id, externalChequeDTO, postingPeriodDTO, externalChequePayables, journals, customerAccountDTO, savingsAccountAvailableBalance, totalRecoveryDeductions, secondaryDescription, reference, moduleNavigationItemCode, serviceHeader);
                                                                break;
                                                            case RecoveryPriority.Investments:
                                                                totalRecoveryDeductions = RecoverAttachedInvestments(primaryJournal.Id, externalChequeDTO, postingPeriodDTO, externalChequePayables, journals, customerAccountDTO, savingsAccountAvailableBalance, totalRecoveryDeductions, secondaryDescription, reference, moduleNavigationItemCode, serviceHeader);
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    });
                                                }
                                            }
                                        }

                                        break;
                                    case ProductCode.Loan:

                                        _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} PAID", externalChequeDTO.Number), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, chequesSuspenseChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                        break;
                                    case ProductCode.Investment:

                                        _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} PAID", externalChequeDTO.Number), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, chequesSuspenseChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                        break;
                                    default:
                                        break;
                                }

                                break;
                            case ExternalChequeClearanceOption.UnPay:

                                if (persisted.IsTransferred && persisted.IsBanked && persisted.BankLinkageChartOfAccountId != null && persisted.BankLinkageChartOfAccountId != Guid.Empty && unPayReasonDTO != null)
                                {
                                    persisted.Remarks = string.Format("UNPAID ({0})", unPayReasonDTO.Description);
                                    persisted.IsCleared = true;
                                    persisted.ClearedBy = serviceHeader.ApplicationUserName;
                                    persisted.ClearedDate = DateTime.Now;

                                    switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                                    {
                                        case ProductCode.Savings:

                                            _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} PAID", externalChequeDTO.Number), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, chequesSuspenseChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                            _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} UNPAID ({1})", externalChequeDTO.Number, unPayReasonDTO.Description), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, externalChequeDTO.BankLinkageChartOfAccountId.Value, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                            var unPaidTariffs_0 = _commissionAppService.ComputeTariffsByUnPayReason(unPayReasonDTO.Id, externalChequeDTO.Amount, customerAccountDTO, serviceHeader);

                                            if (unPaidTariffs_0 != null && unPaidTariffs_0.Any())
                                            {
                                                unPaidTariffs_0.ForEach(tariff =>
                                                {
                                                    _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, tariff.Amount, tariff.Description, string.Format("Cheque Clearance~{0} UNPAID ({1})", externalChequeDTO.Number, unPayReasonDTO.Description), customerAccountDTO.CustomerAccountTypeTargetProductDescription, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                });
                                            }

                                            break;
                                        case ProductCode.Loan:

                                            _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} PAID", externalChequeDTO.Number), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, chequesSuspenseChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                            _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} UNPAID ({1})", externalChequeDTO.Number, unPayReasonDTO.Description), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, externalChequeDTO.BankLinkageChartOfAccountId.Value, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                            var unPaidTariffs_1 = _commissionAppService.ComputeTariffsByUnPayReason(unPayReasonDTO.Id, externalChequeDTO.Amount, customerAccountDTO, serviceHeader);

                                            if (unPaidTariffs_1 != null && unPaidTariffs_1.Any())
                                            {
                                                unPaidTariffs_1.ForEach(tariff =>
                                                {
                                                    _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, tariff.Amount, tariff.Description, string.Format("Cheque Clearance~{0} UNPAID ({1})", externalChequeDTO.Number, unPayReasonDTO.Description), customerAccountDTO.CustomerAccountTypeTargetProductDescription, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                });
                                            }

                                            break;
                                        case ProductCode.Investment:

                                            _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} PAID", externalChequeDTO.Number), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, chequesSuspenseChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                            _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, externalChequeDTO.Amount, string.Format("Cheque Clearance~{0} UNPAID ({1})", externalChequeDTO.Number, unPayReasonDTO.Description), customerAccountDTO.CustomerAccountTypeTargetProductDescription, string.Format("Cheque #{0}", externalChequeDTO.Number), moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, externalChequeDTO.BankLinkageChartOfAccountId.Value, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                                            var unPaidTariffs_2 = _commissionAppService.ComputeTariffsByUnPayReason(unPayReasonDTO.Id, externalChequeDTO.Amount, customerAccountDTO, serviceHeader);

                                            if (unPaidTariffs_2 != null && unPaidTariffs_2.Any())
                                            {
                                                unPaidTariffs_2.ForEach(tariff =>
                                                {
                                                    _journalAppService.AddNewJournal(externalChequeDTO.TellerEmployeeBranchId, null, tariff.Amount, tariff.Description, string.Format("Cheque Clearance~{0} UNPAID ({1})", externalChequeDTO.Number, unPayReasonDTO.Description), customerAccountDTO.CustomerAccountTypeTargetProductDescription, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                });
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) > 0;
                }

                if (result && journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);

                    // trigger arrears recovery?
                    if (result && clearingOption == (int)ExternalChequeClearanceOption.Pay && externalChequeDTO.TellerEmployeeBranchRecoverArrearsOnExternalChequeClearance)
                    {
                        _recurringBatchAppService.RecoverArrears(externalChequeDTO, externalChequePayables, (int)QueuePriority.High, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool BankExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (externalChequeDTOs != null && externalChequeDTOs.Any())
            {
                var chequesInHandChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.ExternalChequesInHand, serviceHeader);

                if (bankLinkageDTO != null && chequesInHandChartOfAccountId != Guid.Empty)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        externalChequeDTOs.ForEach(externalChequeDTO =>
                        {
                            var persisted = _externalChequeRepository.Get(externalChequeDTO.Id, serviceHeader);

                            if (persisted != null && !persisted.IsBanked)
                            {
                                persisted.IsBanked = true;
                                persisted.BankLinkageChartOfAccountId = bankLinkageDTO.ChartOfAccountId;
                                persisted.BankedBy = serviceHeader.ApplicationUserName;
                                persisted.BankedDate = DateTime.Now;

                                _journalAppService.AddNewJournal(bankLinkageDTO.BranchId, null, externalChequeDTO.Amount, string.Format("Cheque Banking~{0}", externalChequeDTO.Number), bankLinkageDTO.BankBranchName, externalChequeDTO.Number, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeBanking, null, chequesInHandChartOfAccountId, bankLinkageDTO.ChartOfAccountId, serviceHeader);
                            }
                        });

                        return dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
                else throw new InvalidOperationException("Sorry, but the requisite bank-linkage and/or external cheques in hand account has not been setup!");
            }
            else return false;
        }

        public List<ExternalChequePayableDTO> FindExternalChequePayablesByExternalChequeId(Guid externalChequeId, ServiceHeader serviceHeader)
        {
            if (externalChequeId != null && externalChequeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ExternalChequePayableSpecifications.ExternalChequePayableWithExternalChequeId(externalChequeId);

                    ISpecification<ExternalChequePayable> spec = filter;

                    var externalChequePayables = _externalChequePayableRepository.AllMatching(spec, serviceHeader);

                    if (externalChequePayables != null && externalChequePayables.Any())
                    {
                        return externalChequePayables.ProjectedAsCollection<ExternalChequePayableDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateExternalChequePayables(Guid externalChequeId, List<ExternalChequePayableDTO> externalChequePayables, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingExternalChequePayables = FindExternalChequePayablesByExternalChequeId(externalChequeId, serviceHeader);

                if (existingExternalChequePayables != null && existingExternalChequePayables.Any())
                {
                    var oldSet = from c in existingExternalChequePayables ?? new List<ExternalChequePayableDTO> { } select c;

                    var newSet = from c in externalChequePayables ?? new List<ExternalChequePayableDTO> { } select c;

                    var commonSet = oldSet.Intersect(newSet, new ExternalChequePayableDTOEqualityComparer());

                    var insertSet = newSet.Except(commonSet, new ExternalChequePayableDTOEqualityComparer());

                    var deleteSet = oldSet.Except(commonSet, new ExternalChequePayableDTOEqualityComparer());

                    foreach (var item in insertSet)
                    {
                        var externalChequePayable = ExternalChequePayableFactory.CreateExternalChequePayable(externalChequeId, item.CustomerAccountId);

                        externalChequePayable.CreatedBy = serviceHeader.ApplicationUserName;

                        _externalChequePayableRepository.Add(externalChequePayable, serviceHeader);
                    }

                    foreach (var item in deleteSet)
                    {
                        var persisted = _externalChequePayableRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _externalChequePayableRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }
                else
                {
                    foreach (var item in externalChequePayables)
                    {
                        var externalChequePayable = ExternalChequePayableFactory.CreateExternalChequePayable(externalChequeId, item.CustomerAccountId);

                        externalChequePayable.CreatedBy = serviceHeader.ApplicationUserName;

                        _externalChequePayableRepository.Add(externalChequePayable, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        private decimal RecoverAttachedLoans(Guid? parentJournalId, ExternalChequeDTO externalChequeDTO, PostingPeriodDTO postingPeriod, List<ExternalChequePayableDTO> externalChequePayables, List<Journal> journals, CustomerAccountDTO externalChequeCustomerAccount, decimal savingsAccountAvailableBalance, decimal totalRecoveryDeductions, string secondaryDescription, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (externalChequePayables != null && externalChequePayables.Any())
            {
                var customerLoanAccounts = new List<CustomerAccountDTO>();

                foreach (var item in externalChequePayables)
                {
                    if (item.CustomerAccountCustomerAccountTypeProductCode != (int)ProductCode.Loan)
                        continue;

                    var result = _sqlCommandAppService.FindCustomerAccountById(item.CustomerAccountId, serviceHeader);

                    if (result != null)
                        customerLoanAccounts.Add(result);
                }

                if (customerLoanAccounts.Any())
                {
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerLoanAccounts, serviceHeader);

                    foreach (var loanAccount in customerLoanAccounts)
                    {
                        var principalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(loanAccount, 1, DateTime.Now, serviceHeader);

                        var interestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(loanAccount, 2, DateTime.Now, serviceHeader);

                        var actualLoanPrincipal = (principalBalance * -1 > 0m) ? principalBalance * -1 : 0m;

                        var actualLoanInterest = (interestBalance * -1 > 0m) ? interestBalance * -1 : 0m;

                        if ((actualLoanPrincipal + actualLoanInterest) > 0m)
                        {
                            // track deductions
                            totalRecoveryDeductions += actualLoanPrincipal;
                            totalRecoveryDeductions += actualLoanInterest;

                            // Do we need to reset expected values?
                            if (!(((savingsAccountAvailableBalance + externalChequeDTO.Amount) - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - (actualLoanInterest + actualLoanPrincipal);

                                // how much is available for recovery?
                                var availableRecoveryAmount = (savingsAccountAvailableBalance + externalChequeDTO.Amount) - totalRecoveryDeductions;

                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                actualLoanInterest = Math.Min(actualLoanInterest, availableRecoveryAmount);
                                actualLoanPrincipal = availableRecoveryAmount - actualLoanInterest;

                                // track deductions
                                totalRecoveryDeductions += actualLoanPrincipal;
                                totalRecoveryDeductions += actualLoanInterest;
                            }

                            // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var externalChequePayableInterestReceivableJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, externalChequeDTO.TellerEmployeeBranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}", loanAccount.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(externalChequePayableInterestReceivableJournal, loanAccount.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, externalChequeCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, loanAccount, externalChequeCustomerAccount, serviceHeader);
                            journals.Add(externalChequePayableInterestReceivableJournal);

                            // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                            var externalChequePayableInterestReceivedJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, externalChequeDTO.TellerEmployeeBranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}", loanAccount.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(externalChequePayableInterestReceivedJournal, loanAccount.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, loanAccount.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, loanAccount, loanAccount, serviceHeader);
                            journals.Add(externalChequePayableInterestReceivedJournal);

                            // Credit LoanProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var externalChequePayablePrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, externalChequeDTO.TellerEmployeeBranchId, null, actualLoanPrincipal, string.Format("Principal Paid~{0}", loanAccount.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(externalChequePayablePrincipalJournal, loanAccount.CustomerAccountTypeTargetProductChartOfAccountId, externalChequeCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, loanAccount, externalChequeCustomerAccount, serviceHeader);
                            journals.Add(externalChequePayablePrincipalJournal);
                        }
                    }
                }
            }

            return totalRecoveryDeductions;
        }

        private decimal RecoverAttachedInvestments(Guid? parentJournalId, ExternalChequeDTO externalChequeDTO, PostingPeriodDTO postingPeriod, List<ExternalChequePayableDTO> externalChequePayables, List<Journal> journals, CustomerAccountDTO externalChequeCustomerAccount, decimal savingsAccountAvailableBalance, decimal totalRecoveryDeductions, string secondaryDescription, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (externalChequePayables != null && externalChequePayables.Any())
            {
                var customerInvestmentAccounts = new List<CustomerAccountDTO>();

                foreach (var item in externalChequePayables)
                {
                    if (item.CustomerAccountCustomerAccountTypeProductCode != (int)ProductCode.Investment)
                        continue;

                    var result = _sqlCommandAppService.FindCustomerAccountById(item.CustomerAccountId, serviceHeader);

                    if (result != null)
                        customerInvestmentAccounts.Add(result);
                }

                if (customerInvestmentAccounts.Any())
                {
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerInvestmentAccounts, serviceHeader);

                    foreach (var investmentAccount in customerInvestmentAccounts)
                    {
                        var standingOrderDTO = _sqlCommandAppService.FindStandingOrder(externalChequeCustomerAccount.Id, investmentAccount.Id, (int)StandingOrderTrigger.Payout, serviceHeader);

                        if (standingOrderDTO != null && !standingOrderDTO.IsLocked)
                        {
                            var actualInvestmentPrincipal = 0m;

                            var actualInvestmentInterest = 0m;

                            switch ((ChargeType)standingOrderDTO.ChargeType)
                            {
                                case ChargeType.Percentage:
                                    actualInvestmentPrincipal = Convert.ToDecimal((standingOrderDTO.ChargePercentage * Convert.ToDouble(externalChequeDTO.Amount)) / 100);
                                    break;
                                case ChargeType.FixedAmount:
                                    actualInvestmentPrincipal = standingOrderDTO.ChargeFixedAmount;
                                    break;
                                default:
                                    break;
                            }

                            if ((actualInvestmentPrincipal + actualInvestmentInterest) > 0m)
                            {
                                // track deductions
                                totalRecoveryDeductions += actualInvestmentPrincipal;
                                totalRecoveryDeductions += actualInvestmentInterest;

                                // Do we need to reset expected values?
                                if (!(((savingsAccountAvailableBalance + externalChequeDTO.Amount) - totalRecoveryDeductions) >= 0m))
                                {
                                    // reset deductions so far
                                    totalRecoveryDeductions = totalRecoveryDeductions - (actualInvestmentInterest + actualInvestmentPrincipal);

                                    // how much is available for recovery?
                                    var availableRecoveryAmount = (savingsAccountAvailableBalance + externalChequeDTO.Amount) - totalRecoveryDeductions;

                                    // reset expected interest & expected principal >> NB: interest has priority over principal!
                                    actualInvestmentInterest = Math.Min(actualInvestmentInterest, availableRecoveryAmount);
                                    actualInvestmentPrincipal = availableRecoveryAmount - actualInvestmentInterest;

                                    // track deductions
                                    totalRecoveryDeductions += actualInvestmentPrincipal;
                                    totalRecoveryDeductions += actualInvestmentInterest;
                                }

                                // Credit Investment.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                                var attachedInvestmentPrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, externalChequeDTO.TellerEmployeeBranchId, null, actualInvestmentPrincipal + actualInvestmentInterest, string.Format("Investment~{0}", investmentAccount.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.ExternalChequeClearance, null, serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(attachedInvestmentPrincipalJournal, investmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, externalChequeCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, investmentAccount, externalChequeCustomerAccount, serviceHeader);
                                journals.Add(attachedInvestmentPrincipalJournal);
                            }
                        }
                    }
                }
            }

            return totalRecoveryDeductions;
        }
    }
}
