using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositPayableAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class FixedDepositAppService : IFixedDepositAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<FixedDeposit> _fixedDepositRepository;
        private readonly IRepository<FixedDepositPayable> _fixedDepositPayableRepository;
        private readonly IJournalAppService _journalAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IFixedDepositTypeAppService _fixedDepositTypeAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IRecurringBatchAppService _recurringBatchAppService;

        public FixedDepositAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<FixedDeposit> fixedDepositRepository,
           IRepository<FixedDepositPayable> fixedDepositPayableRepository,
           IJournalAppService journalAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           ICustomerAccountAppService customerAccountAppService,
           IFixedDepositTypeAppService fixedDepositTypeAppService,
           ISqlCommandAppService sqlCommandAppService,
           IJournalEntryPostingService journalEntryPostingService,
           IPostingPeriodAppService postingPeriodAppService,
           IRecurringBatchAppService recurringBatchAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (fixedDepositRepository == null)
                throw new ArgumentNullException(nameof(fixedDepositRepository));

            if (fixedDepositPayableRepository == null)
                throw new ArgumentNullException(nameof(fixedDepositPayableRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (fixedDepositTypeAppService == null)
                throw new ArgumentNullException(nameof(fixedDepositTypeAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (recurringBatchAppService == null)
                throw new ArgumentNullException(nameof(recurringBatchAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _fixedDepositRepository = fixedDepositRepository;
            _fixedDepositPayableRepository = fixedDepositPayableRepository;
            _journalAppService = journalAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
            _fixedDepositTypeAppService = fixedDepositTypeAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _postingPeriodAppService = postingPeriodAppService;
            _recurringBatchAppService = recurringBatchAppService;
        }

        public FixedDepositDTO InvokeFixedDeposit(FixedDepositDTO fixedDepositDTO, ServiceHeader serviceHeader)
        {
            if (fixedDepositDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var fixedDeposit = FixedDepositFactory.CreateFixedDeposit(fixedDepositDTO.FixedDepositTypeId, fixedDepositDTO.BranchId, fixedDepositDTO.CustomerAccountId, fixedDepositDTO.Category, fixedDepositDTO.MaturityAction, fixedDepositDTO.Value, fixedDepositDTO.Term, fixedDepositDTO.Rate, fixedDepositDTO.Remarks);

                    fixedDeposit.Status = (int)FixedDepositStatus.New;

                    switch ((FixedDepositCategory)fixedDeposit.Category)
                    {
                        case FixedDepositCategory.TermDeposit:
                            fixedDeposit.MaturityDate = fixedDeposit.CreatedDate.AddMonths(fixedDeposit.Term).Date;
                            fixedDeposit.ExpectedInterest = (decimal)Math.Round((Convert.ToDouble(fixedDeposit.Value) * (fixedDeposit.Rate / 12) * fixedDeposit.Term) / 100, 4, MidpointRounding.AwayFromZero);
                            fixedDeposit.TotalExpected = fixedDeposit.Value + (decimal)Math.Round((Convert.ToDouble(fixedDeposit.Value) * (fixedDeposit.Rate / 12) * fixedDeposit.Term) / 100, 4, MidpointRounding.AwayFromZero);
                            break;
                        case FixedDepositCategory.CallDeposit:
                            fixedDeposit.MaturityDate = fixedDeposit.CreatedDate;
                            break;
                        default:
                            break;
                    }

                    fixedDeposit.CreatedBy = serviceHeader.ApplicationUserName;

                    _fixedDepositRepository.Add(fixedDeposit, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return fixedDeposit.ProjectedAs<FixedDepositDTO>();
                }
            }
            else return null;
        }

        public bool AuditFixedDeposit(FixedDepositDTO fixedDepositDTO, int fixedDepositAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader, bool suppressBalanceCheck = false)
        {
            var result = default(bool);

            if (fixedDepositDTO == null || !Enum.IsDefined(typeof(FixedDepositAuthOption), fixedDepositAuthOption))
                return result;

            var fixedDepositChartOfAccountId = Guid.Empty;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _fixedDepositRepository.Get(fixedDepositDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)FixedDepositStatus.New)
                    return result;

                switch ((FixedDepositAuthOption)fixedDepositAuthOption)
                {
                    case FixedDepositAuthOption.Post:

                        fixedDepositChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.FixedDeposit, serviceHeader);


                        if (fixedDepositChartOfAccountId == Guid.Empty)
                        {
                            fixedDepositDTO.errormassage = string.Format(("Sorry, but the requisite fixed deposit control account has not been set up!"));
                            return result;
                        }

                        persisted.Status = (int)FixedDepositStatus.Running;
                        persisted.AuditRemarks = fixedDepositDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case FixedDepositAuthOption.Reject:

                        persisted.Status = (int)FixedDepositStatus.Rejected;
                        persisted.AuditRemarks = fixedDepositDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
            }

            if (result && fixedDepositAuthOption == (int)FixedDepositAuthOption.Post)
            {
                var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(fixedDepositDTO.CustomerAccountId, serviceHeader);

                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerAccount }, serviceHeader);

                
                if (!suppressBalanceCheck && fixedDepositDTO.Value > customerAccount.AvailableBalance)
                {
                    fixedDepositDTO.errormassage = string.Format(("Sorry, but the customer account has insufficient cash!"));
                    return result;
                }

                _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccount }, serviceHeader);

                var primaryDescription = "FDR Fixing";

                var secondaryDescription = fixedDepositDTO.CategoryDescription;

                var reference = fixedDepositDTO.Remarks;

                _journalAppService.AddNewJournal(fixedDepositDTO.BranchId, null, fixedDepositDTO.Value, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FixedDeposit, null, fixedDepositChartOfAccountId, customerAccount.CustomerAccountTypeTargetProductChartOfAccountId, customerAccount, customerAccount, serviceHeader);
            }

            return result;
        }

        public bool RevokeFixedDeposits(List<FixedDepositDTO> fixedDepositDTOs, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (fixedDepositDTOs != null && fixedDepositDTOs.Any())
            {
                var fixedDepositChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.FixedDeposit, serviceHeader);

               
                if (fixedDepositChartOfAccountId == Guid.Empty)
                {
                    fixedDepositDTOs.ForEach(dto => dto.errormassage=string.Format("Sorry, but the requisite fixed deposit control account has not been set up!"));
                    return false;
                }


                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    fixedDepositDTOs.ForEach(fixedDepositDTO =>
                    {
                        var persisted = _fixedDepositRepository.Get(fixedDepositDTO.Id, serviceHeader);

                        if (persisted != null && persisted.Status == (int)FixedDepositStatus.Running)
                        {
                            persisted.Status = (int)FixedDepositStatus.Revoked;

                            persisted.PaidBy = serviceHeader.ApplicationUserName;

                            persisted.PaidDate = DateTime.Now;

                            var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(fixedDepositDTO.CustomerAccountId, serviceHeader);

                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccount }, serviceHeader);

                            var primaryDescription = "FDR Termination";

                            var secondaryDescription = fixedDepositDTO.CategoryDescription;

                            var reference = fixedDepositDTO.Remarks;

                            _journalAppService.AddNewJournal(fixedDepositDTO.BranchId, null, fixedDepositDTO.Value, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FixedDeposit, null, customerAccount.CustomerAccountTypeTargetProductChartOfAccountId, fixedDepositChartOfAccountId, customerAccount, customerAccount, serviceHeader);
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool PayFixedDeposit(FixedDepositDTO fixedDepositDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (fixedDepositDTO != null)
            {
                var fixedDepositChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.FixedDeposit, serviceHeader);

                var fixedDepositInterestChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.FixedDepositInterest, serviceHeader);

                
                if (fixedDepositChartOfAccountId == Guid.Empty || fixedDepositInterestChartOfAccountId == Guid.Empty)
                {
                    fixedDepositDTO.errormassage = string.Format("Sorry, but the requisite fixed deposit control and/or fixed deposit interest control account has not been setup!");
                    return result;
                }


                var postingPeriodDTO = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
                
                if (postingPeriodDTO == null)
                {
                    fixedDepositDTO.errormassage = string.Format("Sorry, but the current posting period could not be determined!");
                    return result;
                }


                    var journals = new List<Journal>();

                var fixedDepositPayables = new List<FixedDepositPayableDTO>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _fixedDepositRepository.Get(fixedDepositDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)FixedDepositStatus.Running && fixedDepositDTO.MaturityDate <= DateTime.Now)
                    {
                        switch ((FixedDepositCategory)persisted.Category)
                        {
                            case FixedDepositCategory.TermDeposit:
                                break;
                            case FixedDepositCategory.CallDeposit:
                                persisted.Term = UberUtil.GetPeriod(DateTime.Now, persisted.CreatedDate);
                                persisted.MaturityDate = DateTime.Today;
                                persisted.ExpectedInterest = (decimal)Math.Round((Convert.ToDouble(persisted.Value) * (persisted.Rate / 12) * persisted.Term) / 100, 4, MidpointRounding.AwayFromZero);
                                persisted.TotalExpected = persisted.Value + (decimal)Math.Round((Convert.ToDouble(persisted.Value) * (persisted.Rate / 12) * persisted.Term) / 100, 4, MidpointRounding.AwayFromZero);
                                break;
                            default:
                                break;
                        }

                        persisted.Status = (int)FixedDepositStatus.Paid;

                        persisted.PaidBy = serviceHeader.ApplicationUserName;

                        persisted.PaidDate = DateTime.Now;

                        var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(fixedDepositDTO.CustomerAccountId, serviceHeader);

                        _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader);

                        var primaryDescription = "FDR Liquidation";

                        var secondaryDescription = fixedDepositDTO.CategoryDescription;

                        var reference = fixedDepositDTO.Remarks;

                        var primaryJournal = _journalAppService.AddNewJournal(fixedDepositDTO.BranchId, null, fixedDepositDTO.Value, string.Format("{0} (Principal)", primaryDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FixedDeposit, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, fixedDepositChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);

                        var tariffs = _fixedDepositTypeAppService.ComputeTariffs(fixedDepositDTO.FixedDepositTypeId ?? Guid.Empty, persisted.ExpectedInterest, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName, serviceHeader);

                        _journalAppService.AddNewJournal(fixedDepositDTO.BranchId, null, fixedDepositDTO.ExpectedInterest, string.Format("{0} (Interest)", primaryDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FixedDeposit, null, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, fixedDepositInterestChartOfAccountId, customerAccountDTO, customerAccountDTO, tariffs, serviceHeader);

                        if (fixedDepositDTO.Category == (int)FixedDepositCategory.TermDeposit)
                        {
                            switch ((FixedDepositMaturityAction)fixedDepositDTO.MaturityAction)
                            {
                                case FixedDepositMaturityAction.PayPrincipalAndInterestDue:

                                    fixedDepositPayables = FindFixedDepositPayablesByFixedDepositId(fixedDepositDTO.Id, serviceHeader);

                                    if (fixedDepositPayables != null && fixedDepositPayables.Any())
                                    {
                                        // track deductions
                                        var savingsAccountAvailableBalance = 0m;

                                        var totalRecoveryDeductions = 0m;

                                        // Recover dues for attached products
                                        if (((savingsAccountAvailableBalance + (fixedDepositDTO.Value + fixedDepositDTO.ExpectedInterest)) > 0m /*Will current account balance + incoming batch amount be positive?*/))
                                        {
                                            if (!string.IsNullOrWhiteSpace(customerAccountDTO.BranchCompanyRecoveryPriority))
                                            {
                                                var buffer = customerAccountDTO.BranchCompanyRecoveryPriority.Split(new char[] { ',' });

                                                Array.ForEach(buffer, (item) =>
                                                {
                                                    switch ((RecoveryPriority)Enum.Parse(typeof(RecoveryPriority), item))
                                                    {
                                                        case RecoveryPriority.Loans:
                                                            totalRecoveryDeductions = RecoverAttachedLoans(primaryJournal.Id, fixedDepositDTO, postingPeriodDTO, fixedDepositPayables, journals, customerAccountDTO, savingsAccountAvailableBalance, totalRecoveryDeductions, secondaryDescription, reference, moduleNavigationItemCode, serviceHeader);
                                                            break;
                                                        case RecoveryPriority.Investments:
                                                            break;
                                                        case RecoveryPriority.Savings:
                                                            break;
                                                        case RecoveryPriority.DirectDebits:
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                });
                                            }
                                        }
                                    }

                                    break;
                                case FixedDepositMaturityAction.PayInterestDueAndRollOverPrincipal:

                                    var payInterestDueAndRollOverPrincipal = new FixedDepositDTO
                                    {
                                        FixedDepositTypeId = fixedDepositDTO.FixedDepositTypeId,
                                        BranchId = fixedDepositDTO.BranchId,
                                        CustomerAccountId = fixedDepositDTO.CustomerAccountId,
                                        Category = fixedDepositDTO.Category,
                                        MaturityAction = fixedDepositDTO.MaturityAction,
                                        Value = fixedDepositDTO.Value,
                                        Term = fixedDepositDTO.Term,
                                        Rate = fixedDepositDTO.Rate,
                                        Remarks = fixedDepositDTO.Remarks,
                                        Status = (int)FixedDepositStatus.Running,
                                    };

                                    payInterestDueAndRollOverPrincipal = InvokeFixedDeposit(payInterestDueAndRollOverPrincipal, serviceHeader);

                                    if (payInterestDueAndRollOverPrincipal != null)
                                        AuditFixedDeposit(payInterestDueAndRollOverPrincipal, (int)FixedDepositAuthOption.Post, moduleNavigationItemCode, serviceHeader, true);

                                    break;
                                case FixedDepositMaturityAction.RollOverPrincipalAndInterestDue:

                                    var rollOverPrincipalAndInterestDue = new FixedDepositDTO
                                    {
                                        FixedDepositTypeId = fixedDepositDTO.FixedDepositTypeId,
                                        BranchId = fixedDepositDTO.BranchId,
                                        CustomerAccountId = fixedDepositDTO.CustomerAccountId,
                                        Category = fixedDepositDTO.Category,
                                        MaturityAction = fixedDepositDTO.MaturityAction,
                                        Value = (fixedDepositDTO.Value + fixedDepositDTO.ExpectedInterest),
                                        Term = fixedDepositDTO.Term,
                                        Rate = fixedDepositDTO.Rate,
                                        Remarks = fixedDepositDTO.Remarks,
                                        Status = (int)FixedDepositStatus.Running,
                                    };

                                    rollOverPrincipalAndInterestDue = InvokeFixedDeposit(rollOverPrincipalAndInterestDue, serviceHeader);
                                    if (rollOverPrincipalAndInterestDue != null)
                                        AuditFixedDeposit(rollOverPrincipalAndInterestDue, (int)FixedDepositAuthOption.Post, moduleNavigationItemCode, serviceHeader, true);

                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }

                if (result && journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);

                    // trigger arrears recovery?
                    if (result && fixedDepositDTO.BranchCompanyRecoverArrearsOnFixedDepositPayment)
                    {
                        _recurringBatchAppService.RecoverArrears(fixedDepositDTO, fixedDepositPayables, (int)QueuePriority.High, serviceHeader);
                    }
                }
            }

            return result;
        }

        public List<FixedDepositPayableDTO> FindFixedDepositPayablesByFixedDepositId(Guid fixedDepositId, ServiceHeader serviceHeader)
        {
            if (fixedDepositId != null && fixedDepositId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FixedDepositPayableSpecifications.FixedDepositPayableWithFixedDepositId(fixedDepositId);

                    ISpecification<FixedDepositPayable> spec = filter;

                    var fixedDepositPayables = _fixedDepositPayableRepository.AllMatching(spec, serviceHeader);

                    if (fixedDepositPayables != null && fixedDepositPayables.Any())
                    {
                        return fixedDepositPayables.ProjectedAsCollection<FixedDepositPayableDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateFixedDepositPayables(Guid fixedDepositId, List<FixedDepositPayableDTO> fixedDepositPayables, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingFixedDepositPayables = FindFixedDepositPayablesByFixedDepositId(fixedDepositId, serviceHeader);

                if (existingFixedDepositPayables != null && existingFixedDepositPayables.Any())
                {
                    var oldSet = from c in existingFixedDepositPayables ?? new List<FixedDepositPayableDTO> { } select c;

                    var newSet = from c in fixedDepositPayables ?? new List<FixedDepositPayableDTO> { } select c;

                    var commonSet = oldSet.Intersect(newSet, new FixedDepositPayableDTOEqualityComparer());

                    var insertSet = newSet.Except(commonSet, new FixedDepositPayableDTOEqualityComparer());

                    var deleteSet = oldSet.Except(commonSet, new FixedDepositPayableDTOEqualityComparer());

                    foreach (var item in insertSet)
                    {
                        var fixedDepositPayable = FixedDepositPayableFactory.CreateFixedDepositPayable(fixedDepositId, item.CustomerAccountId);

                        fixedDepositPayable.CreatedBy = serviceHeader.ApplicationUserName;

                        _fixedDepositPayableRepository.Add(fixedDepositPayable, serviceHeader);
                    }

                    foreach (var item in deleteSet)
                    {
                        var persisted = _fixedDepositPayableRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _fixedDepositPayableRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }
                else
                {
                    foreach (var item in fixedDepositPayables)
                    {
                        var fixedDepositPayable = FixedDepositPayableFactory.CreateFixedDepositPayable(fixedDepositId, item.CustomerAccountId);

                        fixedDepositPayable.CreatedBy = serviceHeader.ApplicationUserName;

                        _fixedDepositPayableRepository.Add(fixedDepositPayable, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public List<FixedDepositDTO> FindFixedDeposits(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var fixedDeposits = _fixedDepositRepository.GetAll(serviceHeader);

                if (fixedDeposits != null && fixedDeposits.Any())
                {
                    return fixedDeposits.ProjectedAsCollection<FixedDepositDTO>();
                }
                else return null;
            }
        }

        public List<FixedDepositDTO> FindFixedDepositsByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositSpecifications.FixedDepositsWithCustomerAccountId(customerAccountId);

                ISpecification<FixedDeposit> spec = filter;

                var fixedDeposits = _fixedDepositRepository.AllMatching(spec, serviceHeader);

                if (fixedDeposits != null && fixedDeposits.Any())
                {
                    return fixedDeposits.ProjectedAsCollection<FixedDepositDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<FixedDepositDTO> FindFixedDeposits(Guid branchId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositSpecifications.FixedDepositsWithBranchId(branchId);

                ISpecification<FixedDeposit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositPagedCollection = _fixedDepositRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositPagedCollection != null)
                {
                    var pageCollection = fixedDepositPagedCollection.PageCollection.ProjectedAsCollection<FixedDepositDTO>();

                    var itemsCount = fixedDepositPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FixedDepositDTO> FindPayableFixedDeposits(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositSpecifications.PayableFixedDepositsWithMaturityDateRangeAndFullText(startDate, endDate, text);

                ISpecification<FixedDeposit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositPagedCollection = _fixedDepositRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositPagedCollection != null)
                {
                    var pageCollection = fixedDepositPagedCollection.PageCollection.ProjectedAsCollection<FixedDepositDTO>();

                    var itemsCount = fixedDepositPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FixedDepositDTO> FindDueFixedDeposits(DateTime targetDate, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositSpecifications.DueFixedDeposits(targetDate);

                ISpecification<FixedDeposit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositPagedCollection = _fixedDepositRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositPagedCollection != null)
                {
                    var pageCollection = fixedDepositPagedCollection.PageCollection.ProjectedAsCollection<FixedDepositDTO>();

                    var itemsCount = fixedDepositPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FixedDepositDTO> FindRevocableFixedDeposits(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositSpecifications.RevocableFixedDepositsWithMaturityDateRangeAndFullText(startDate, endDate, text);

                ISpecification<FixedDeposit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositPagedCollection = _fixedDepositRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositPagedCollection != null)
                {
                    var pageCollection = fixedDepositPagedCollection.PageCollection.ProjectedAsCollection<FixedDepositDTO>();

                    var itemsCount = fixedDepositPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FixedDepositDTO> FindFixedDeposits(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositSpecifications.FixedDepositFullText(text);

                ISpecification<FixedDeposit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositPagedCollection = _fixedDepositRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositPagedCollection != null)
                {
                    var pageCollection = fixedDepositPagedCollection.PageCollection.ProjectedAsCollection<FixedDepositDTO>();

                    var itemsCount = fixedDepositPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FixedDepositDTO> FindFixedDepositsByStatus(int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositSpecifications.FixedDepositWithStatusAndFullText(status, text);

                ISpecification<FixedDeposit> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositPagedCollection = _fixedDepositRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositPagedCollection != null)
                {
                    var pageCollection = fixedDepositPagedCollection.PageCollection.ProjectedAsCollection<FixedDepositDTO>();

                    var itemsCount = fixedDepositPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public FixedDepositDTO FindFixedDeposit(Guid fixedDepositId, ServiceHeader serviceHeader)
        {
            if (fixedDepositId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var fixedDeposit = _fixedDepositRepository.Get(fixedDepositId, serviceHeader);

                    if (fixedDeposit != null)
                    {
                        return fixedDeposit.ProjectedAs<FixedDepositDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private decimal RecoverAttachedLoans(Guid? parentJournalId, FixedDepositDTO fixedDepositDTO, PostingPeriodDTO postingPeriod, List<FixedDepositPayableDTO> fixedDepositPayables, List<Journal> journals, CustomerAccountDTO fixedDepositCustomerAccount, decimal savingsAccountAvailableBalance, decimal totalRecoveryDeductions, string secondaryDescription, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (fixedDepositPayables != null && fixedDepositPayables.Any())
            {
                var customerLoanAccounts = new List<CustomerAccountDTO>();

                foreach (var item in fixedDepositPayables)
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
                            if (!(((savingsAccountAvailableBalance + (fixedDepositDTO.Value + fixedDepositDTO.ExpectedInterest)) - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - (actualLoanInterest + actualLoanPrincipal);

                                // how much is available for recovery?
                                var availableRecoveryAmount = (savingsAccountAvailableBalance + (fixedDepositDTO.Value + fixedDepositDTO.ExpectedInterest)) - totalRecoveryDeductions;

                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                actualLoanInterest = Math.Min(actualLoanInterest, availableRecoveryAmount);
                                actualLoanPrincipal = availableRecoveryAmount - actualLoanInterest;

                                // track deductions
                                totalRecoveryDeductions += actualLoanPrincipal;
                                totalRecoveryDeductions += actualLoanInterest;
                            }

                            // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var fixedDepositPayableInterestReceivableJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, fixedDepositDTO.BranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}", loanAccount.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FixedDeposit, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(fixedDepositPayableInterestReceivableJournal, loanAccount.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, fixedDepositCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, loanAccount, fixedDepositCustomerAccount, serviceHeader);
                            journals.Add(fixedDepositPayableInterestReceivableJournal);

                            // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                            var fixedDepositPayableInterestReceivedJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, fixedDepositDTO.BranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}", loanAccount.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FixedDeposit, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(fixedDepositPayableInterestReceivedJournal, loanAccount.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, loanAccount.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, loanAccount, loanAccount, serviceHeader);
                            journals.Add(fixedDepositPayableInterestReceivedJournal);

                            // Credit LoanProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var fixedDepositPayablePrincipalJournal = JournalFactory.CreateJournal(parentJournalId, postingPeriod.Id, fixedDepositDTO.BranchId, null, actualLoanPrincipal, string.Format("Principal Paid~{0}", loanAccount.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.FixedDeposit, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(fixedDepositPayablePrincipalJournal, loanAccount.CustomerAccountTypeTargetProductChartOfAccountId, fixedDepositCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, loanAccount, fixedDepositCustomerAccount, serviceHeader);
                            journals.Add(fixedDepositPayablePrincipalJournal);
                        }
                    }
                }
            }

            return totalRecoveryDeductions;
        }

        public bool ExecutePayableFixedDeposits(DateTime targetDate, int pageSize, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var fixedDepositDTOs = new List<FixedDepositDTO>();

            var itemsCount = 0;

            var pageIndex = 0;

            var pageCollectionInfo = FindDueFixedDeposits(targetDate, pageIndex, pageSize, serviceHeader);

            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
            {
                itemsCount = pageCollectionInfo.ItemsCount;

                fixedDepositDTOs.AddRange(pageCollectionInfo.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageCollectionInfo = FindDueFixedDeposits(targetDate, pageIndex, pageSize, serviceHeader);

                        if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                        {
                            fixedDepositDTOs.AddRange(pageCollectionInfo.PageCollection);

                            ++pageIndex;
                        }
                        else break;
                    }
                }
            }

            if (fixedDepositDTOs != null && fixedDepositDTOs.Any())
            {
                foreach (var item in fixedDepositDTOs)
                {
                    result = PayFixedDeposit(item, 0x8888, serviceHeader);
                }
            }

            return result;
        }
    }
}
