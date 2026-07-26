using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderHistoryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCollateralAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class RecurringBatchAppService : IRecurringBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<RecurringBatch> _recurringBatchRepository;
        private readonly IRepository<RecurringBatchEntry> _recurringBatchEntryRepository;
        private readonly IRepository<StandingOrder> _standingOrderRepository;
        private readonly IRepository<StandingOrderHistory> _standingOrderHistoryRepository;
        private readonly IRepository<ElectronicStatementOrder> _electronicStatementOrderRepository;
        private readonly IRepository<ElectronicStatementOrderHistory> _electronicStatementOrderHistoryRepository;
        private readonly IRepository<LoanGuarantor> _loanGuarantorRepository;
        private readonly IRepository<LoanCollateral> _loanCollateralRepository;
        private readonly IRepository<CustomerDocument> _customerDocumentRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IHolidayAppService _holidayAppService;
        private readonly IMediaAppService _mediaAppService;

        private readonly IBrokerService _brokerService;

        private readonly NumberFormatInfo _nfi;

        public RecurringBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<RecurringBatch> recurringBatchRepository,
           IRepository<RecurringBatchEntry> recurringBatchEntryRepository,
           IRepository<StandingOrder> standingOrderRepository,
           IRepository<StandingOrderHistory> standingOrderHistoryRepository,
           IRepository<ElectronicStatementOrder> electronicStatementOrderRepository,
           IRepository<ElectronicStatementOrderHistory> electronicStatementOrderHistoryRepository,
           IRepository<LoanGuarantor> loanGuarantorRepository,
           IRepository<LoanCollateral> loanCollateralRepository,
           IRepository<CustomerDocument> customerDocumentRepository,
           IPostingPeriodAppService postingPeriodAppService,
           ILoanProductAppService loanProductAppService,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ISqlCommandAppService sqlCommandAppService,
           ICommissionAppService commissionAppService,
           ICustomerAccountAppService customerAccountAppService,
           IHolidayAppService holidayAppService,
           IMediaAppService mediaAppService,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (recurringBatchRepository == null)
                throw new ArgumentNullException(nameof(recurringBatchRepository));

            if (recurringBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(recurringBatchEntryRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (standingOrderRepository == null)
                throw new ArgumentNullException(nameof(standingOrderRepository));

            if (standingOrderHistoryRepository == null)
                throw new ArgumentNullException(nameof(standingOrderHistoryRepository));

            if (electronicStatementOrderRepository == null)
                throw new ArgumentNullException(nameof(electronicStatementOrderRepository));

            if (electronicStatementOrderHistoryRepository == null)
                throw new ArgumentNullException(nameof(electronicStatementOrderHistoryRepository));

            if (loanGuarantorRepository == null)
                throw new ArgumentNullException(nameof(loanGuarantorRepository));

            if (loanCollateralRepository == null)
                throw new ArgumentNullException(nameof(loanCollateralRepository));

            if (customerDocumentRepository == null)
                throw new ArgumentNullException(nameof(customerDocumentRepository));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            if (mediaAppService == null)
                throw new ArgumentNullException(nameof(mediaAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _recurringBatchRepository = recurringBatchRepository;
            _recurringBatchEntryRepository = recurringBatchEntryRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _loanProductAppService = loanProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _sqlCommandAppService = sqlCommandAppService;
            _commissionAppService = commissionAppService;
            _customerAccountAppService = customerAccountAppService;
            _standingOrderRepository = standingOrderRepository;
            _standingOrderHistoryRepository = standingOrderHistoryRepository;
            _electronicStatementOrderRepository = electronicStatementOrderRepository;
            _electronicStatementOrderHistoryRepository = electronicStatementOrderHistoryRepository;
            _loanGuarantorRepository = loanGuarantorRepository;
            _loanCollateralRepository = loanCollateralRepository;
            _customerDocumentRepository = customerDocumentRepository;
            _holidayAppService = holidayAppService;
            _mediaAppService = mediaAppService;
            _brokerService = brokerService;

            _nfi = new NumberFormatInfo();
            _nfi.CurrencySymbol = string.Empty;
        }

        public RecurringBatchDTO AddNewRecurringBatch(RecurringBatchDTO recurringBatchDTO, ServiceHeader serviceHeader)
        {
            if (recurringBatchDTO != null && recurringBatchDTO.BranchId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var recurringBatch = RecurringBatchFactory.CreateRecurringBatch(recurringBatchDTO.BranchId, recurringBatchDTO.PostingPeriodId, recurringBatchDTO.Type, recurringBatchDTO.Month, recurringBatchDTO.Reference, recurringBatchDTO.Priority);

                    recurringBatch.BatchNumber = _recurringBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}RecurringBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    recurringBatch.EnforceMonthValueDate = recurringBatchDTO.EnforceMonthValueDate;
                    recurringBatch.Status = (int)BatchStatus.Posted;
                    recurringBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    _recurringBatchRepository.Add(recurringBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return recurringBatch.ProjectedAs<RecurringBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateRecurringBatchEntries(Guid recurringBatchId, List<RecurringBatchEntryDTO> recurringBatchEntries, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (recurringBatchId != null && recurringBatchEntries != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _recurringBatchRepository.Get(recurringBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        if (recurringBatchEntries.Any())
                        {
                            List<RecurringBatchEntry> batchEntries = new List<RecurringBatchEntry>();

                            foreach (var item in recurringBatchEntries)
                            {
                                Duration electronicStatement = new Duration(item.ElectronicStatementStartDate, item.ElectronicStatementEndDate);

                                var recurringBatchEntry = RecurringBatchEntryFactory.CreateRecurringBatchEntry(persisted.Id, item.CustomerAccountId, item.SecondaryCustomerAccountId, item.StandingOrderId, item.ElectronicStatementOrderId, electronicStatement, item.Reference, item.Remarks);

                                recurringBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                recurringBatchEntry.EnforceCeiling = item.EnforceCeiling;
                                recurringBatchEntry.InterestCapitalizationMonths = (byte)item.InterestCapitalizationMonths;
                                recurringBatchEntry.ElectronicStatementSender = item.ElectronicStatementSender;
                                recurringBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                batchEntries.Add(recurringBatchEntry);
                            }

                            if (batchEntries.Any())
                            {
                                var bcpBatchEntries = new List<RecurringBatchEntryBulkCopyDTO>();

                                batchEntries.ForEach(c =>
                                {
                                    RecurringBatchEntryBulkCopyDTO bcpc =
                                        new RecurringBatchEntryBulkCopyDTO
                                        {
                                            Id = c.Id,
                                            RecurringBatchId = c.RecurringBatchId,
                                            CustomerAccountId = c.CustomerAccountId,
                                            SecondaryCustomerAccountId = c.SecondaryCustomerAccountId,
                                            StandingOrderId = c.StandingOrderId,
                                            ElectronicStatementOrderId = c.ElectronicStatementOrderId,
                                            ElectronicStatement_StartDate = c.ElectronicStatement.StartDate,
                                            ElectronicStatement_EndDate = c.ElectronicStatement.EndDate,
                                            ElectronicStatementSender = c.ElectronicStatementSender,
                                            Reference = c.Reference,
                                            Remarks = c.Remarks,
                                            InterestCapitalizationMonths = c.InterestCapitalizationMonths,
                                            EnforceCeiling = c.EnforceCeiling,
                                            Status = c.Status,
                                            CreatedBy = c.CreatedBy,
                                            CreatedDate = c.CreatedDate,
                                        };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _recurringBatchEntryRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public bool PostRecurringBatchEntry(Guid recurringBatchEntryId, int moduleNavigationItemCode, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (recurringBatchEntryId == null || recurringBatchEntryId == Guid.Empty)
                    return false;

                var recurringBatchEntry = _recurringBatchEntryRepository.Get(recurringBatchEntryId, serviceHeader);
                if (recurringBatchEntry == null || recurringBatchEntry.Status != (int)BatchEntryStatus.Pending)
                    return false;

                var recurringBatchEntryDTO = recurringBatchEntry.ProjectedAs<RecurringBatchEntryDTO>();

                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var tuple = new Tuple<bool, string>(false, string.Empty);

                switch ((RecurringBatchType)recurringBatchEntryDTO.RecurringBatchType)
                {
                    case RecurringBatchType.InterestCapitalization:
                        tuple = CapitalizeInterest(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.DynamicSavingsFees:
                        tuple = ChargeDynamicSavingsFees(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.IndefiniteLoanCharges:
                        tuple = ChargeIndefiniteLoanCharges(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.StandingOrder:
                        tuple = ExecuteStandingOrder(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.InvestmentBalancesAdjustment:
                        tuple = AdjustInvestmentBalance(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.InvestmentBalancesPooling:
                        tuple = PoolInvestmentBalance(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.GuarantorReleasing:
                        tuple = ReleaseLoanGuarantors(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.ElectronicStatementOrder:
                        tuple = ExecuteElectronicStatementOrder(recurringBatchEntryDTO, moduleNavigationItemCode, fileDirectory, blobDatabaseConnectionString, serviceHeader);
                        break;
                    case RecurringBatchType.ArrearsRecovery:
                        tuple = RecoverArrears(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    case RecurringBatchType.ArrearsRecoveryFromInvestmentProduct:
                        tuple = RecoverArrearsFromInvestmentProduct(recurringBatchEntryDTO, moduleNavigationItemCode, serviceHeader);
                        break;
                    default:
                        break;
                }

                recurringBatchEntry.Remarks = tuple.Item1 ? string.Format("Succeeded->{0}{1}", Environment.NewLine, tuple.Item2) : string.Format("Failed->{0}{1}", Environment.NewLine, tuple.Item2);

                recurringBatchEntry.Status = (int)BatchEntryStatus.Posted;

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<RecurringBatchDTO> FindRecurringBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var recurringBatches = _recurringBatchRepository.GetAll(serviceHeader);

                if (recurringBatches != null && recurringBatches.Any())
                {
                    return recurringBatches.ProjectedAsCollection<RecurringBatchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<RecurringBatchDTO> FindRecurringBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = RecurringBatchSpecifications.RecurringBatchesWithStatus(status, startDate, endDate, text);

                ISpecification<RecurringBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var recurringBatchPagedCollection = _recurringBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (recurringBatchPagedCollection != null)
                {
                    var pageCollection = recurringBatchPagedCollection.PageCollection.ProjectedAsCollection<RecurringBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _recurringBatchEntryRepository.AllMatchingCount(RecurringBatchEntrySpecifications.RecurringBatchEntryWithRecurringBatchId(item.Id, null), serviceHeader);

                            var postedItems = _recurringBatchEntryRepository.AllMatchingCount(RecurringBatchEntrySpecifications.PostedRecurringBatchEntryWithRecurringBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = recurringBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<RecurringBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public RecurringBatchDTO FindRecurringBatch(Guid recurringBatchId, ServiceHeader serviceHeader)
        {
            if (recurringBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var recurringBatch = _recurringBatchRepository.Get(recurringBatchId, serviceHeader);

                    if (recurringBatch != null)
                    {
                        return recurringBatch.ProjectedAs<RecurringBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<RecurringBatchEntryDTO> FindRecurringBatchEntryDTOsByRecurringBatchId(Guid recurringBatchId, ServiceHeader serviceHeader)
        {
            if (recurringBatchId != null && recurringBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = RecurringBatchEntrySpecifications.RecurringBatchEntryWithRecurringBatchId(recurringBatchId, null);

                    ISpecification<RecurringBatchEntry> spec = filter;

                    var recurringBatchEntries = _recurringBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (recurringBatchEntries != null && recurringBatchEntries.Any())
                    {
                        return recurringBatchEntries.ProjectedAsCollection<RecurringBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<RecurringBatchEntryDTO> FindRecurringBatchEntriesByRecurringBatchId(Guid recurringBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (recurringBatchId != null && recurringBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = RecurringBatchEntrySpecifications.RecurringBatchEntryWithRecurringBatchId(recurringBatchId, text);

                    ISpecification<RecurringBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var recurringBatchPagedCollection = _recurringBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (recurringBatchPagedCollection != null)
                    {
                        var persisted = _recurringBatchRepository.Get(recurringBatchId, serviceHeader);

                        var pageCollection = recurringBatchPagedCollection.PageCollection.ProjectedAsCollection<RecurringBatchEntryDTO>();

                        var itemsCount = recurringBatchPagedCollection.ItemsCount;

                        return new PageCollectionInfo<RecurringBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<RecurringBatchEntryDTO> FindQueableRecurringBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = RecurringBatchEntrySpecifications.QueableRecurringBatchEntries();

                ISpecification<RecurringBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var recurringBatchPagedCollection = _recurringBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (recurringBatchPagedCollection != null)
                {
                    var pageCollection = recurringBatchPagedCollection.PageCollection.ProjectedAsCollection<RecurringBatchEntryDTO>();

                    var itemsCount = recurringBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<RecurringBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool CapitalizeInterest(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var employerDTO in employerDTOs)
                {
                    if (employerDTO.IsLocked)
                        continue;

                    foreach (var loanProductDTO in loanProductDTOs)
                    {
                        var customerAccounts = _sqlCommandAppService.FindCustomerAccountsGivenEmployerAndLoanProduct(employerDTO.Id, loanProductDTO.Id, serviceHeader);

                        if (customerAccounts == null || !customerAccounts.Any())
                            continue;

                        foreach (var customerAccountDTO in customerAccounts)
                        {
                            switch ((CustomerAccountStatus)customerAccountDTO.Status)
                            {
                                case CustomerAccountStatus.Normal:

                                    var principalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, DateTime.Now, serviceHeader);

                                    if (principalBalance * -1 > 0m) // Check that we have a principal balance..
                                    {
                                        recurringBatchEntries.Add(
                                            new RecurringBatchEntryDTO
                                            {
                                                RecurringBatchId = recurringBatchDTO.Id,
                                                CustomerAccountId = customerAccountDTO.Id,
                                                Reference = string.Format("{0}~{1}", employerDTO.Description.Trim(), loanProductDTO.Description.Trim()),
                                            });
                                    }

                                    break;
                                case CustomerAccountStatus.Inactive:
                                    break;
                                case CustomerAccountStatus.Dormant:
                                    break;
                                case CustomerAccountStatus.Closed:
                                    break;
                                case CustomerAccountStatus.Remarked:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool CapitalizeInterest(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var interestCapitalizationMonths = recurringBatchDTO.InterestCapitalizationMonths;

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var customerDTO in customerDTOs)
                {
                    if (customerDTO.IsLocked)
                        continue;

                    foreach (var loanProductDTO in loanProductDTOs)
                    {
                        var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(loanProductDTO.Id, customerDTO.Id, serviceHeader);

                        if (customerAccounts == null || !customerAccounts.Any())
                            continue;

                        foreach (var customerAccountDTO in customerAccounts)
                        {
                            switch ((CustomerAccountStatus)customerAccountDTO.Status)
                            {
                                case CustomerAccountStatus.Normal:

                                    var principalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, DateTime.Now, serviceHeader);

                                    if (principalBalance * -1 > 0m) // Check that we have a principal balance..
                                    {
                                        recurringBatchEntries.Add(
                                            new RecurringBatchEntryDTO
                                            {
                                                RecurringBatchId = recurringBatchDTO.Id,
                                                CustomerAccountId = customerAccountDTO.Id,
                                                Reference = string.Format("{0}~{1}", customerDTO.FullName.Trim(), loanProductDTO.Description.Trim()),
                                                InterestCapitalizationMonths = interestCapitalizationMonths,
                                            });
                                    }

                                    break;
                                case CustomerAccountStatus.Inactive:
                                    break;
                                case CustomerAccountStatus.Dormant:
                                    break;
                                case CustomerAccountStatus.Closed:
                                    break;
                                case CustomerAccountStatus.Remarked:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool CapitalizeInterest(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var creditTypeDTO in creditTypeDTOs)
                {
                    if (creditTypeDTO.IsLocked)
                        continue;

                    foreach (var loanProductDTO in loanProductDTOs)
                    {
                        var customerAccounts = _sqlCommandAppService.FindCustomerAccountsGivenCreditTypeAndLoanProduct(creditTypeDTO.Id, loanProductDTO.Id, serviceHeader);

                        if (customerAccounts == null || !customerAccounts.Any())
                            continue;

                        foreach (var customerAccountDTO in customerAccounts)
                        {
                            switch ((CustomerAccountStatus)customerAccountDTO.Status)
                            {
                                case CustomerAccountStatus.Normal:

                                    var principalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, DateTime.Now, serviceHeader);

                                    if (principalBalance * -1 > 0m) // Check that we have a principal balance..
                                    {
                                        recurringBatchEntries.Add(
                                            new RecurringBatchEntryDTO
                                            {
                                                RecurringBatchId = recurringBatchDTO.Id,
                                                CustomerAccountId = customerAccountDTO.Id,
                                                Reference = string.Format("{0}~{1}", creditTypeDTO.Description.Trim(), loanProductDTO.Description.Trim()),
                                            });
                                    }

                                    break;
                                case CustomerAccountStatus.Inactive:
                                    break;
                                case CustomerAccountStatus.Dormant:
                                    break;
                                case CustomerAccountStatus.Closed:
                                    break;
                                case CustomerAccountStatus.Remarked:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool CapitalizeInterest(int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var loanProductDTOs = _loanProductAppService.FindLoanProducts(serviceHeader);

            if (loanProductDTOs != null && loanProductDTOs.Any())
            {
                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.InterestCapitalization,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("Interest Capitalization>Auto ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var loanProductDTO in loanProductDTOs)
                    {
                        var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(loanProductDTO.Id, serviceHeader);

                        if (customerAccounts == null || !customerAccounts.Any())
                            continue;

                        foreach (var customerAccountDTO in customerAccounts)
                        {
                            switch ((CustomerAccountStatus)customerAccountDTO.Status)
                            {
                                case CustomerAccountStatus.Normal:

                                    var principalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, DateTime.Now, serviceHeader);

                                    if (principalBalance * -1 > 0m) // Check that we have a principal balance..
                                    {
                                        recurringBatchEntries.Add(
                                            new RecurringBatchEntryDTO
                                            {
                                                RecurringBatchId = recurringBatchDTO.Id,
                                                CustomerAccountId = customerAccountDTO.Id,
                                                Reference = string.Format("{0}", loanProductDTO.Description.Trim()),
                                            });
                                    }

                                    break;
                                case CustomerAccountStatus.Inactive:
                                    break;
                                case CustomerAccountStatus.Dormant:
                                    break;
                                case CustomerAccountStatus.Closed:
                                    break;
                                case CustomerAccountStatus.Remarked:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<StandingOrderDTO> standingOrderDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var standingOrderDTO in standingOrderDTOs)
                {
                    if (standingOrderDTO.IsLocked)
                        continue;

                    recurringBatchEntries.Add(
                        new RecurringBatchEntryDTO
                        {
                            RecurringBatchId = recurringBatchDTO.Id,
                            StandingOrderId = standingOrderDTO.Id,
                            Reference = string.Format("{0}~{1}", standingOrderDTO.TriggerDescription, standingOrderDTO.Remarks),
                        });
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, int standingOrderTrigger, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var employerDTO in employerDTOs)
                {
                    if (employerDTO.IsLocked)
                        continue;

                    var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByEmployerAndTrigger(employerDTO.Id, standingOrderTrigger, serviceHeader);

                    if (standingOrderDTOs != null && standingOrderDTOs.Any())
                    {
                        foreach (var standingOrderDTO in standingOrderDTOs)
                        {
                            if (standingOrderDTO.IsLocked)
                                continue;

                            recurringBatchEntries.Add(
                                new RecurringBatchEntryDTO
                                {
                                    RecurringBatchId = recurringBatchDTO.Id,
                                    StandingOrderId = standingOrderDTO.Id,
                                    Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((StandingOrderTrigger)standingOrderTrigger), employerDTO.Description),
                                });
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<EmployerDTO> employerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var employerDTO in employerDTOs)
                {
                    if (employerDTO.IsLocked)
                        continue;

                    if (savingsProductDTOs != null && savingsProductDTOs.Any())
                    {
                        foreach (var savingsProductDTO in savingsProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByEmployerAndProductAndTrigger(employerDTO.Id, savingsProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((StandingOrderTrigger)standingOrderTrigger), employerDTO.Description),
                                        });
                                }
                            }
                        }
                    }

                    if (loanProductDTOs != null && loanProductDTOs.Any())
                    {
                        foreach (var loanProductDTO in loanProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByEmployerAndProductAndTrigger(employerDTO.Id, loanProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = employerDTO.Description.Trim(),
                                        });
                                }
                            }
                        }
                    }

                    if (investmentProductDTOs != null && investmentProductDTOs.Any())
                    {
                        foreach (var investmentProductDTO in investmentProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByEmployerAndProductAndTrigger(employerDTO.Id, investmentProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = employerDTO.Description.Trim(),
                                        });
                                }
                            }
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<CustomerDTO> customerDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var customerDTO in customerDTOs)
                {
                    if (customerDTO.IsLocked)
                        continue;

                    if (savingsProductDTOs != null && savingsProductDTOs.Any())
                    {
                        foreach (var savingsProductDTO in savingsProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByCustomerAndProductAndTrigger(customerDTO.Id, savingsProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((StandingOrderTrigger)standingOrderTrigger), standingOrderDTO.Remarks),
                                        });
                                }
                            }
                        }
                    }

                    if (loanProductDTOs != null && loanProductDTOs.Any())
                    {
                        foreach (var loanProductDTO in loanProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByCustomerAndProductAndTrigger(customerDTO.Id, loanProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = standingOrderDTO.Remarks,
                                        });
                                }
                            }
                        }
                    }

                    if (investmentProductDTOs != null && investmentProductDTOs.Any())
                    {
                        foreach (var investmentProductDTO in investmentProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByCustomerAndProductAndTrigger(customerDTO.Id, investmentProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = standingOrderDTO.Remarks,
                                        });
                                }
                            }
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool ExecuteStandingOrders(RecurringBatchDTO recurringBatchDTO, List<CreditTypeDTO> creditTypeDTOs, List<SavingsProductDTO> savingsProductDTOs, List<LoanProductDTO> loanProductDTOs, List<InvestmentProductDTO> investmentProductDTOs, int standingOrderTrigger, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var creditTypeDTO in creditTypeDTOs)
                {
                    if (creditTypeDTO.IsLocked)
                        continue;

                    if (savingsProductDTOs != null && savingsProductDTOs.Any())
                    {
                        foreach (var savingsProductDTO in savingsProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByCreditTypeAndProductAndTrigger(creditTypeDTO.Id, savingsProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((StandingOrderTrigger)standingOrderTrigger), creditTypeDTO.Description),
                                        });
                                }
                            }
                        }
                    }

                    if (loanProductDTOs != null && loanProductDTOs.Any())
                    {
                        foreach (var loanProductDTO in loanProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByCreditTypeAndProductAndTrigger(creditTypeDTO.Id, loanProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = creditTypeDTO.Description.Trim(),
                                        });
                                }
                            }
                        }
                    }

                    if (investmentProductDTOs != null && investmentProductDTOs.Any())
                    {
                        foreach (var investmentProductDTO in investmentProductDTOs)
                        {
                            var standingOrderDTOs = _sqlCommandAppService.FindStandingOrdersByCreditTypeAndProductAndTrigger(creditTypeDTO.Id, investmentProductDTO.Id, standingOrderTrigger, serviceHeader);

                            if (standingOrderDTOs != null && standingOrderDTOs.Any())
                            {
                                foreach (var standingOrderDTO in standingOrderDTOs)
                                {
                                    if (standingOrderDTO.IsLocked)
                                        continue;

                                    recurringBatchEntries.Add(
                                        new RecurringBatchEntryDTO
                                        {
                                            RecurringBatchId = recurringBatchDTO.Id,
                                            StandingOrderId = standingOrderDTO.Id,
                                            Reference = creditTypeDTO.Description.Trim(),
                                        });
                                }
                            }
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool ExecuteStandingOrders(DateTime targetDate, int targetDateOption, int priority, int maximumStandingOrderExecuteAttemptCount, int pageSize, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            var itemsCount = 0;

            var pageIndex = 0;

            var pageCollectionInfo = FindDueStandingOrders(targetDate, targetDateOption, pageIndex, pageSize, serviceHeader);

            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
            {
                itemsCount = pageCollectionInfo.ItemsCount;

                standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageCollectionInfo = FindDueStandingOrders(targetDate, targetDateOption, pageIndex, pageSize, serviceHeader);

                        if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                        {
                            standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                            ++pageIndex;
                        }
                        else break;
                    }
                }
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.StandingOrder,
                    Month = targetDate.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("Scheduled>Auto ~ {0}", EnumHelper.GetDescription((Month)targetDate.Month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var holidayDTOs = _holidayAppService.FindHolidaysInCurrentPostingPeriod(serviceHeader);

                    var holidays = new List<DateTime>();

                    var dateRanges = (from h in holidayDTOs ?? new List<HolidayDTO>() select new { h.DurationStartDate, h.DurationEndDate });

                    foreach (var item in dateRanges)
                        for (DateTime date = item.DurationStartDate; date <= item.DurationEndDate; date = date.AddDays(1))
                            holidays.Add(date);

                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    /*We need to calculate next run date for each*/
                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        standingOrderDTO.ScheduleExecuteAttemptCount += 1;

                        var canExecute = default(bool);

                        var benefactorCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BenefactorCustomerAccountId, serviceHeader);

                        if (benefactorCustomerAccountDTO != null && benefactorCustomerAccountDTO.Status != (int)CustomerAccountStatus.Closed)
                        {
                            var benefactorAccountAvailableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(benefactorCustomerAccountDTO, DateTime.Now, serviceHeader);

                            if (benefactorAccountAvailableBalance * -1 < 0m)
                            {
                                var expectedPrincipal = 0m;

                                var expectedInterest = 0m;

                                switch ((ProductCode)standingOrderDTO.BeneficiaryCustomerAccountCustomerAccountTypeProductCode)
                                {
                                    case ProductCode.Loan:

                                        expectedPrincipal = standingOrderDTO.Principal;
                                        expectedInterest = standingOrderDTO.Interest;

                                        break;
                                    case ProductCode.Savings:
                                    case ProductCode.Investment:

                                        switch ((ChargeType)standingOrderDTO.ChargeType)
                                        {
                                            case ChargeType.Percentage:
                                                expectedPrincipal = Convert.ToDecimal((standingOrderDTO.ChargePercentage * Convert.ToDouble(benefactorAccountAvailableBalance)) / 100);
                                                break;
                                            case ChargeType.FixedAmount:
                                                expectedPrincipal = standingOrderDTO.ChargeFixedAmount;
                                                break;
                                            default:
                                                break;
                                        }

                                        break;
                                    default:
                                        break;
                                }

                                canExecute = (expectedPrincipal + expectedInterest) <= benefactorAccountAvailableBalance;
                            }
                        }

                        var timestampNextRunDate = (!standingOrderDTO.ScheduleForceExecute && canExecute) ? true : (!standingOrderDTO.ScheduleForceExecute && standingOrderDTO.ScheduleExecuteAttemptCount >= maximumStandingOrderExecuteAttemptCount);

                        if (standingOrderDTO.ScheduleForceExecute)
                        {
                            standingOrderDTO.ScheduleForceExecute = false; // reset
                            standingOrderDTO.ScheduleExecuteAttemptCount = 0; //reset
                        }

                        if (timestampNextRunDate)
                        {
                            standingOrderDTO.ScheduleExecuteAttemptCount = 0; //reset

                            var siFrequency = (ScheduleFrequency)standingOrderDTO.ScheduleFrequency;

                            var startDate = standingOrderDTO.ScheduleExpectedRunDate;

                            switch ((ScheduleFrequency)standingOrderDTO.ScheduleFrequency)
                            {
                                case ScheduleFrequency.Annual:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddYears(1);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddYears(1).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.SemiAnnual:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(6);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(6).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.Quarterly:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(3);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(3).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.TriAnnual:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(4);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(4).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.BiMonthly:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(2);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(2).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.Monthly:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(1);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(1).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.SemiMonthly:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(15);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(15).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.BiWeekly:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(14);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(14).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.Weekly:

                                    standingOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(7);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(7).AddDays(-1), 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                case ScheduleFrequency.Daily:

                                    standingOrderDTO.ScheduleExpectedRunDate = DateTime.Today.AddDays(1);

                                    standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(DateTime.Today, 1, holidays);

                                    while (standingOrderDTO.ScheduleActualRunDate < standingOrderDTO.ScheduleExpectedRunDate)
                                    {
                                        standingOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(standingOrderDTO.ScheduleActualRunDate, 1, holidays);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        using (var dbContextScope = _dbContextScopeFactory.Create())
                        {
                            var persisted = _standingOrderRepository.Get(standingOrderDTO.Id, serviceHeader);

                            var duration = new Duration(standingOrderDTO.DurationStartDate, standingOrderDTO.DurationEndDate);

                            var schedule = new Schedule(standingOrderDTO.ScheduleFrequency, standingOrderDTO.ScheduleExpectedRunDate, standingOrderDTO.ScheduleActualRunDate, standingOrderDTO.ScheduleExecuteAttemptCount, standingOrderDTO.ScheduleForceExecute);

                            var charge = new Charge(standingOrderDTO.ChargeType, standingOrderDTO.ChargePercentage, standingOrderDTO.ChargeFixedAmount);

                            var current = StandingOrderFactory.CreateStandingOrder(standingOrderDTO.BenefactorCustomerAccountId, standingOrderDTO.BeneficiaryCustomerAccountId, duration, schedule, charge, standingOrderDTO.Trigger, standingOrderDTO.LoanAmount, standingOrderDTO.PaymentPerPeriod, standingOrderDTO.Principal, standingOrderDTO.Interest, standingOrderDTO.CapitalizedInterest, standingOrderDTO.Remarks, standingOrderDTO.Chargeable);

                            current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                            current.CreatedBy = persisted.CreatedBy;

                            _standingOrderRepository.Merge(persisted, current, serviceHeader);

                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                        }

                        if (result)
                        {
                            recurringBatchEntries.Add(
                                new RecurringBatchEntryDTO
                                {
                                    RecurringBatchId = recurringBatchDTO.Id,
                                    StandingOrderId = standingOrderDTO.Id,
                                    Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.Remarks),
                                });
                        }
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool ExecutePayoutStandingOrders(Guid benefactorCustomerAccountId, int month, int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccountId, (int)StandingOrderTrigger.Payout, serviceHeader);

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.StandingOrder,
                    Month = month,
                    Priority = (byte)priority,
                    Reference = string.Format("Payout ~ {0}", EnumHelper.GetDescription((Month)month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        recurringBatchEntries.Add(
                            new RecurringBatchEntryDTO
                            {
                                RecurringBatchId = recurringBatchDTO.Id,
                                StandingOrderId = standingOrderDTO.Id,
                                Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)month), standingOrderDTO.Remarks),
                            });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool ExecuteSweepingStandingOrders(int priority, int pageSize, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            var itemsCount = 0;

            var pageIndex = 0;

            var pageCollectionInfo = FindSweepStandingOrders(pageIndex, pageSize, serviceHeader);

            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
            {
                itemsCount = pageCollectionInfo.ItemsCount;

                standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageCollectionInfo = FindSweepStandingOrders(pageIndex, pageSize, serviceHeader);

                        if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                        {
                            standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                            ++pageIndex;
                        }
                        else break;
                    }
                }
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.StandingOrder,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("Sweeping ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        recurringBatchEntries.Add(
                            new RecurringBatchEntryDTO
                            {
                                RecurringBatchId = recurringBatchDTO.Id,
                                StandingOrderId = standingOrderDTO.Id,
                                Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.Remarks),
                            });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool ChargeDynamicFees(RecurringBatchDTO recurringBatchDTO, List<LoanProductDTO> loanProductDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var loanProductDTO in loanProductDTOs)
                {
                    var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(loanProductDTO.Id, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        foreach (var customerAccountDTO in customerAccounts)
                        {
                            if (customerAccountDTO.Status != (int)CustomerAccountStatus.Normal)
                                continue;

                            recurringBatchEntries.Add(
                                new RecurringBatchEntryDTO
                                {
                                    RecurringBatchId = recurringBatchDTO.Id,
                                    CustomerAccountId = customerAccountDTO.Id,
                                    Reference = loanProductDTO.Description.Trim(),
                                });
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool ChargeDynamicFees(RecurringBatchDTO recurringBatchDTO, List<SavingsProductDTO> savingsProductDTOs, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

            if (recurringBatchDTO != null)
            {
                var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                foreach (var savingsProductDTO in savingsProductDTOs)
                {
                    var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(savingsProductDTO.Id, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        foreach (var customerAccountDTO in customerAccounts)
                        {
                            if (customerAccountDTO.Status != (int)CustomerAccountStatus.Normal)
                                continue;

                            recurringBatchEntries.Add(
                                new RecurringBatchEntryDTO
                                {
                                    RecurringBatchId = recurringBatchDTO.Id,
                                    CustomerAccountId = customerAccountDTO.Id,
                                    Reference = savingsProductDTO.Description.Trim(),
                                });
                        }
                    }
                }

                result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                if (result)
                {
                    QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                }
            }

            return result;
        }

        public bool ProcessSavingsProductLedgerFees(int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var recurringBatchDTO = new RecurringBatchDTO
            {
                Type = (int)RecurringBatchType.DynamicSavingsFees,
                Month = DateTime.Today.Month,
                Priority = (byte)priority,
                Reference = string.Format("Indefinite Savings Charges~{0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
            };

            var savingsProductDTOs = _savingsProductAppService.FindSavingsProductsWithAutomatedLedgerFeeCalculation(serviceHeader);

            if (savingsProductDTOs != null && savingsProductDTOs.Any())
                result = ChargeDynamicFees(recurringBatchDTO, savingsProductDTOs, serviceHeader);

            return result;
        }

        public bool AdjustInvestmentBalances(string investmentNormalizationSets, int priority, bool enforceCeiling, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (!string.IsNullOrWhiteSpace(investmentNormalizationSets))
            {
                var investmentProducts = _investmentProductAppService.FindInvestmentProducts(serviceHeader);

                if (investmentProducts != null && investmentProducts.Any())
                {
                    var buffer = investmentNormalizationSets.Split(new char[] { ',' });

                    if (buffer != null && buffer.Length != 0)
                    {
                        serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                        Array.ForEach(buffer, (item) =>
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                var sets = item.Split(new char[] { ':' });

                                if (sets != null && sets.Length == 2)
                                {
                                    var sourceProductCode = -1;

                                    var destinationProductCode = -1;

                                    if (int.TryParse(sets[0], out sourceProductCode) && int.TryParse(sets[1], out destinationProductCode))
                                    {
                                        var sourceProduct = investmentProducts.SingleOrDefault(x => x.Code == sourceProductCode);

                                        var destinationProduct = investmentProducts.SingleOrDefault(x => x.Code == destinationProductCode);

                                        if (sourceProduct != null && destinationProduct != null)
                                        {
                                            var recurringBatchDTO = new RecurringBatchDTO
                                            {
                                                Type = (int)RecurringBatchType.InvestmentBalancesAdjustment,
                                                Month = DateTime.Today.Month,
                                                Priority = (byte)priority,
                                                Reference = string.Format("Investment Balances Adjustment ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                                            };

                                            recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                                            if (recurringBatchDTO != null)
                                            {
                                                var sourceProductAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(sourceProduct.Id, serviceHeader);

                                                if (sourceProductAccounts != null && sourceProductAccounts.Any())
                                                {
                                                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                                                    foreach (var sourceProductAccount in sourceProductAccounts)
                                                    {
                                                        if (sourceProductAccount.Status != (int)CustomerAccountStatus.Normal)
                                                            continue;

                                                        var destinationProductAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(destinationProduct.Id, sourceProductAccount.CustomerId, serviceHeader);

                                                        if (destinationProductAccounts != null && destinationProductAccounts.Any() && destinationProductAccounts.Count == 1) // ignore multiple A/Cs
                                                        {
                                                            foreach (var destinationProductAccount in destinationProductAccounts)
                                                            {
                                                                if (destinationProductAccount.Status != (int)CustomerAccountStatus.Normal)
                                                                    continue;

                                                                recurringBatchEntries.Add(
                                                                    new RecurringBatchEntryDTO
                                                                    {
                                                                        RecurringBatchId = recurringBatchDTO.Id,
                                                                        CustomerAccountId = sourceProductAccount.Id,
                                                                        SecondaryCustomerAccountId = destinationProductAccount.Id,
                                                                        Reference = string.Format("{0} vs {1}", sourceProduct.Description, destinationProduct.Description),
                                                                        EnforceCeiling = enforceCeiling,
                                                                    });
                                                            }
                                                        }
                                                    }

                                                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                                                    if (result)
                                                    {
                                                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }

            return result;
        }

        public bool PoolInvestmentBalances(int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var investmentProducts = _investmentProductAppService.FindPooledInvestmentProducts(serviceHeader);

            if (investmentProducts != null && investmentProducts.Any())
            {
                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.InvestmentBalancesPooling,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("Investment Balances Pooling ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var investmentProduct in investmentProducts)
                    {
                        var investmentProductAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(investmentProduct.Id, serviceHeader);

                        if (investmentProductAccounts != null && investmentProductAccounts.Any())
                        {
                            foreach (var investmentProductAccount in investmentProductAccounts)
                            {
                                recurringBatchEntries.Add(
                                    new RecurringBatchEntryDTO
                                    {
                                        RecurringBatchId = recurringBatchDTO.Id,
                                        CustomerAccountId = investmentProductAccount.Id,
                                        Reference = investmentProduct.Description,
                                    });
                            }
                        }
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool ReleaseLoanGuarantors(int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var loanProducts = _loanProductAppService.FindLoanProducts(serviceHeader);

            if (loanProducts != null && loanProducts.Any())
            {
                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.GuarantorReleasing,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("Guarantor Releasing ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var loanProduct in loanProducts)
                    {
                        var loanProductAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductId(loanProduct.Id, serviceHeader);

                        if (loanProductAccounts != null && loanProductAccounts.Any())
                        {
                            foreach (var loanProductAccount in loanProductAccounts)
                            {
                                if (loanProductAccount.Status != (int)CustomerAccountStatus.Normal)
                                    continue;

                                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { loanProductAccount }, serviceHeader, true);

                                if (loanProductAccount.BookBalance * -1 <= 0m) // IFF has no loan balance
                                {
                                    recurringBatchEntries.Add(new RecurringBatchEntryDTO
                                    {
                                        RecurringBatchId = recurringBatchDTO.Id,
                                        CustomerAccountId = loanProductAccount.Id,
                                        Reference = loanProduct.Description,
                                    });
                                }
                            }
                        }
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool ExecuteElectronicStatementOrders(DateTime targetDate, int targetDateOption, string sender, int priority, int pageSize, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var electronicStatementOrderDTOs = new List<ElectronicStatementOrderDTO>();

            var itemsCount = 0;

            var pageIndex = 0;

            var pageCollectionInfo = FindDueElectronicStatementOrders(targetDate, targetDateOption, pageIndex, pageSize, serviceHeader);

            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
            {
                itemsCount = pageCollectionInfo.ItemsCount;

                electronicStatementOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageCollectionInfo = FindDueElectronicStatementOrders(targetDate, targetDateOption, pageIndex, pageSize, serviceHeader);

                        if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                        {
                            electronicStatementOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                            ++pageIndex;
                        }
                        else break;
                    }
                }
            }

            if (electronicStatementOrderDTOs != null && electronicStatementOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ElectronicStatementOrder,
                    Month = targetDate.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("Scheduled>Auto ~ {0}", EnumHelper.GetDescription((Month)targetDate.Month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var holidayDTOs = _holidayAppService.FindHolidaysInCurrentPostingPeriod(serviceHeader);

                    var holidays = new List<DateTime>();

                    var dateRanges = (from h in holidayDTOs ?? new List<HolidayDTO>() select new { h.DurationStartDate, h.DurationEndDate });

                    foreach (var item in dateRanges)
                        for (DateTime date = item.DurationStartDate; date <= item.DurationEndDate; date = date.AddDays(1))
                            holidays.Add(date);

                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    /*We need to calculate next run date for each*/
                    foreach (var electronicStatementOrderDTO in electronicStatementOrderDTOs)
                    {
                        if (electronicStatementOrderDTO.IsLocked)
                            continue;

                        var siFrequency = (ScheduleFrequency)electronicStatementOrderDTO.ScheduleFrequency;

                        var startDate = electronicStatementOrderDTO.ScheduleExpectedRunDate;

                        var eStatementStartDate = electronicStatementOrderDTO.ScheduleActualRunDate;

                        var eStatementEndDate = electronicStatementOrderDTO.ScheduleActualRunDate;

                        switch ((ScheduleFrequency)electronicStatementOrderDTO.ScheduleFrequency)
                        {
                            case ScheduleFrequency.Annual:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddYears(1);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddYears(1).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddYears(-1).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.SemiAnnual:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(6);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(6).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddMonths(-6).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.Quarterly:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(3);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(3).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddMonths(-3).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.TriAnnual:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(4);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(4).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddMonths(-4).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.BiMonthly:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(2);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(2).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddMonths(-2).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.Monthly:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddMonths(1);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddMonths(1).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddMonths(-1).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.SemiMonthly:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(15);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(15).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddDays(-15).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.BiWeekly:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(14);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(14).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddDays(-14).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.Weekly:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = startDate.AddDays(7);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(startDate.AddDays(7).AddDays(-1), 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(startDate.AddDays(-7).AddDays(1), -1, holidays);

                                break;
                            case ScheduleFrequency.Daily:

                                electronicStatementOrderDTO.ScheduleExpectedRunDate = DateTime.Today.AddDays(1);

                                electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(DateTime.Today, 1, holidays);

                                while (electronicStatementOrderDTO.ScheduleActualRunDate < electronicStatementOrderDTO.ScheduleExpectedRunDate)
                                {
                                    electronicStatementOrderDTO.ScheduleActualRunDate = UberUtil.GetBusinessDay(electronicStatementOrderDTO.ScheduleActualRunDate, 1, holidays);
                                }

                                eStatementStartDate = UberUtil.GetBusinessDay(DateTime.Today, -1, holidays);

                                break;
                            default:
                                break;
                        }

                        using (var dbContextScope = _dbContextScopeFactory.Create())
                        {
                            var persisted = _electronicStatementOrderRepository.Get(electronicStatementOrderDTO.Id, serviceHeader);

                            var duration = new Duration(electronicStatementOrderDTO.DurationStartDate, electronicStatementOrderDTO.DurationEndDate);

                            var schedule = new Schedule(electronicStatementOrderDTO.ScheduleFrequency, electronicStatementOrderDTO.ScheduleExpectedRunDate, electronicStatementOrderDTO.ScheduleActualRunDate, electronicStatementOrderDTO.ScheduleExecuteAttemptCount, electronicStatementOrderDTO.ScheduleForceExecute);

                            var current = ElectronicStatementOrderFactory.CreateElectronicStatementOrder(electronicStatementOrderDTO.CustomerAccountId, duration, schedule, electronicStatementOrderDTO.Remarks);

                            current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                            current.CreatedBy = persisted.CreatedBy;

                            _electronicStatementOrderRepository.Merge(persisted, current, serviceHeader);

                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                        }

                        if (result)
                        {
                            recurringBatchEntries.Add(
                                new RecurringBatchEntryDTO
                                {
                                    RecurringBatchId = recurringBatchDTO.Id,
                                    ElectronicStatementOrderId = electronicStatementOrderDTO.Id,
                                    ElectronicStatementStartDate = eStatementStartDate,
                                    ElectronicStatementEndDate = eStatementEndDate,
                                    ElectronicStatementSender = sender,
                                    Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), electronicStatementOrderDTO.Remarks),
                                });
                        }

                        result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                        if (result)
                        {
                            QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                        }
                    }
                }
            }

            return result;
        }

        public bool RecoverArrears(int priority, int pageSize, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            var itemsCount = 0;

            var pageIndex = 0;

            var pageCollectionInfo = FindArrearsStandingOrders(pageIndex, pageSize, serviceHeader);

            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
            {
                itemsCount = pageCollectionInfo.ItemsCount;

                standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                if (itemsCount > pageSize)
                {
                    ++pageIndex;

                    while ((pageSize * pageIndex) <= itemsCount)
                    {
                        pageCollectionInfo = FindArrearsStandingOrders(pageIndex, pageSize, serviceHeader);

                        if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                        {
                            standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                            ++pageIndex;
                        }
                        else break;
                    }
                }
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ArrearsRecovery,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("Scheduled>Arrears Recovery ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                        if (beneficiaryCustomerAccountDTO == null)
                            continue;

                        _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { beneficiaryCustomerAccountDTO }, serviceHeader);

                        if (beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductThrottleScheduledArrearsRecovery)
                            continue;

                        recurringBatchEntries.Add(
                            new RecurringBatchEntryDTO
                            {
                                RecurringBatchId = recurringBatchDTO.Id,
                                StandingOrderId = standingOrderDTO.Id,
                                Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.TriggerDescription),
                            });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool RecoverArrearsFromInvestmentProduct(int priority, string targetProductCodes, int pageSize, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var kvpList = new List<KeyValuePair<InvestmentProductDTO, List<StandingOrderDTO>>>();

            if (!string.IsNullOrWhiteSpace(targetProductCodes))
            {
                var investmentProducts = _investmentProductAppService.FindInvestmentProducts(serviceHeader);

                if (investmentProducts != null && investmentProducts.Any())
                {
                    var buffer = targetProductCodes.Split(new char[] { ',' });

                    if (buffer != null && buffer.Length != 0)
                    {
                        Array.ForEach(buffer, (item) =>
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                int targetProductCode = 0;

                                int.TryParse(item[0].ToString(), out targetProductCode);

                                var investmentProductDTO = investmentProducts.SingleOrDefault(x => x.Code == targetProductCode);

                                if (investmentProductDTO != null)
                                {
                                    var standingOrderDTOs = new List<StandingOrderDTO>();

                                    var itemsCount = 0;

                                    var pageIndex = 0;

                                    var pageCollectionInfo = FindArrearsStandingOrders(pageIndex, pageSize, serviceHeader);

                                    if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                                    {
                                        itemsCount = pageCollectionInfo.ItemsCount;

                                        standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                                        if (itemsCount > pageSize)
                                        {
                                            ++pageIndex;

                                            while ((pageSize * pageIndex) <= itemsCount)
                                            {
                                                pageCollectionInfo = FindArrearsStandingOrders(pageIndex, pageSize, serviceHeader);

                                                if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                                                {
                                                    standingOrderDTOs.AddRange(pageCollectionInfo.PageCollection);

                                                    ++pageIndex;
                                                }
                                                else break;
                                            }
                                        }
                                    }

                                    kvpList.Add(new KeyValuePair<InvestmentProductDTO, List<StandingOrderDTO>>(investmentProductDTO, pageCollectionInfo.PageCollection));
                                }
                            }
                        });
                    }
                }
            }

            if (kvpList.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ArrearsRecoveryFromInvestmentProduct,
                    Month = DateTime.Today.Month - 1/*work with previous month > simba_chai*/,
                    Priority = (byte)priority,
                    Reference = string.Format("Scheduled>Arrears Recovery From Investment Product ~ {0}", EnumHelper.GetDescription((Month)(DateTime.Today.Month - 1))),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var kvp in kvpList)
                    {
                        foreach (var standingOrderDTO in kvp.Value)
                        {
                            if (standingOrderDTO.IsLocked)
                                continue;

                            var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                            if (beneficiaryCustomerAccountDTO == null)
                                continue;

                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { beneficiaryCustomerAccountDTO }, serviceHeader);

                            if (beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductThrottleScheduledArrearsRecovery)
                                continue;

                            if (beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductLoanProductSection == (int)LoanProductSection.FOSA)
                                continue;

                            var benefactorCustomerAccountDTO = new CustomerAccountDTO();

                            var benefactorCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(beneficiaryCustomerAccountDTO.CustomerId, kvp.Key.Id, serviceHeader);

                            if (benefactorCustomerAccountDTOs != null && benefactorCustomerAccountDTOs.Any())
                            {
                                benefactorCustomerAccountDTO = benefactorCustomerAccountDTOs.FirstOrDefault();

                                recurringBatchEntries.Add(
                                    new RecurringBatchEntryDTO
                                    {
                                        RecurringBatchId = recurringBatchDTO.Id,
                                        CustomerAccountId = benefactorCustomerAccountDTO.Id,
                                        StandingOrderId = standingOrderDTO.Id,
                                        Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)(DateTime.Today.Month - 1)), standingOrderDTO.TriggerDescription),
                                    });
                            }
                            else continue;
                        }
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool RecoverArrears(List<StandingOrderDTO> standingOrderDTOs, int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ArrearsRecovery,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("StandingOrder>Arrears Recovery ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        recurringBatchEntries.Add(
                             new RecurringBatchEntryDTO
                             {
                                 RecurringBatchId = recurringBatchDTO.Id,
                                 StandingOrderId = standingOrderDTO.Id,
                                 Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.TriggerDescription),
                             });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool RecoverArrears(ExternalChequeDTO externalChequeDTO, List<ExternalChequePayableDTO> externalChequePayables, int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            if (externalChequePayables != null && externalChequePayables.Any())
            {
                foreach (var item in externalChequePayables)
                {
                    if (item.CustomerAccountCustomerAccountTypeProductCode != (int)ProductCode.Loan)
                        continue;

                    var matchedStandingOrders = FindStandingOrdersByBeneficiaryCustomerAccountId(item.CustomerAccountId, serviceHeader);

                    if (matchedStandingOrders != null && matchedStandingOrders.Any())
                        standingOrderDTOs.AddRange(matchedStandingOrders);
                }
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ArrearsRecovery,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("ExternalCheque>{0}>Arrears Recovery ~ {1}", externalChequeDTO.Number, EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        recurringBatchEntries.Add(
                            new RecurringBatchEntryDTO
                            {
                                RecurringBatchId = recurringBatchDTO.Id,
                                StandingOrderId = standingOrderDTO.Id,
                                Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.TriggerDescription),
                            });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool RecoverArrears(FixedDepositDTO fixedDepositDTO, List<FixedDepositPayableDTO> fixedDepositPayables, int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            if (fixedDepositPayables != null && fixedDepositPayables.Any())
            {
                foreach (var item in fixedDepositPayables)
                {
                    if (item.CustomerAccountCustomerAccountTypeProductCode != (int)ProductCode.Loan)
                        continue;

                    var matchedStandingOrders = FindStandingOrdersByBeneficiaryCustomerAccountId(item.CustomerAccountId, serviceHeader);

                    if (matchedStandingOrders != null && matchedStandingOrders.Any())
                        standingOrderDTOs.AddRange(matchedStandingOrders);
                }
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ArrearsRecovery,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("FixedDeposit>{0}>Arrears Recovery ~ {1}", fixedDepositDTO.CustomerAccountCustomerFullName, EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        recurringBatchEntries.Add(
                            new RecurringBatchEntryDTO
                            {
                                RecurringBatchId = recurringBatchDTO.Id,
                                StandingOrderId = standingOrderDTO.Id,
                                Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.TriggerDescription),
                            });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool RecoverArrears(CreditBatchEntryDTO creditBatchEntryDTO, List<LoanProductDTO> loanProductCollection, int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            if (creditBatchEntryDTO != null && creditBatchEntryDTO.CustomerAccountId.HasValue && loanProductCollection != null && loanProductCollection.Any())
            {
                var matchedStandingOrders = FindStandingOrdersByBenefactorCustomerAccountId(creditBatchEntryDTO.CustomerAccountId.Value, serviceHeader);

                if (matchedStandingOrders != null && matchedStandingOrders.Any())
                {
                    foreach (var item in matchedStandingOrders)
                    {
                        if (loanProductCollection.Any(x => x.Id == item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId))
                            standingOrderDTOs.AddRange(matchedStandingOrders);
                    }
                }
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ArrearsRecovery,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("CreditBatchEntry>{0}-{1}>Arrears Recovery ~ {2}", creditBatchEntryDTO.Reference, creditBatchEntryDTO.PaddedCreditBatchBatchNumber, EnumHelper.GetDescription((Month)creditBatchEntryDTO.CreditBatchMonth)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        recurringBatchEntries.Add(
                            new RecurringBatchEntryDTO
                            {
                                RecurringBatchId = recurringBatchDTO.Id,
                                StandingOrderId = standingOrderDTO.Id,
                                Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.TriggerDescription),
                            });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool RecoverArrears(CustomerAccountDTO benefactorCustomerAccount, int priority, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var standingOrderDTOs = new List<StandingOrderDTO>();

            if (benefactorCustomerAccount != null)
            {
                var matchedStandingOrders = FindStandingOrdersByBenefactorCustomerAccountId(benefactorCustomerAccount.Id, serviceHeader);

                if (matchedStandingOrders != null && matchedStandingOrders.Any())
                    standingOrderDTOs.AddRange(matchedStandingOrders);
            }

            if (standingOrderDTOs != null && standingOrderDTOs.Any())
            {
                var recurringBatchDTO = new RecurringBatchDTO
                {
                    Type = (int)RecurringBatchType.ArrearsRecovery,
                    Month = DateTime.Today.Month,
                    Priority = (byte)priority,
                    Reference = string.Format("CustomerAccount>Arrears Recovery ~ {0}", EnumHelper.GetDescription((Month)DateTime.Today.Month)),
                };

                serviceHeader.ApplicationUserName = serviceHeader.ApplicationUserName ?? "Swift_ETL";

                recurringBatchDTO = AddNewRecurringBatch(recurringBatchDTO, serviceHeader);

                if (recurringBatchDTO != null)
                {
                    var recurringBatchEntries = new List<RecurringBatchEntryDTO>();

                    foreach (var standingOrderDTO in standingOrderDTOs)
                    {
                        if (standingOrderDTO.IsLocked)
                            continue;

                        recurringBatchEntries.Add(
                            new RecurringBatchEntryDTO
                            {
                                RecurringBatchId = recurringBatchDTO.Id,
                                StandingOrderId = standingOrderDTO.Id,
                                Reference = string.Format("{0}~{1}", EnumHelper.GetDescription((Month)DateTime.Today.Month), standingOrderDTO.TriggerDescription),
                            });
                    }

                    result = UpdateRecurringBatchEntries(recurringBatchDTO.Id, recurringBatchEntries, serviceHeader);

                    if (result)
                    {
                        QueueRecurringBatchEntries(recurringBatchDTO, serviceHeader);
                    }
                }
            }

            return result;
        }

        private Tuple<bool, string> CapitalizeInterest(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriodDTO = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriodDTO != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.CustomerAccount != null)
            {
                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var customerAccountDTO = recurringBatchEntryDTO.CustomerAccount;

                if (_sqlCommandAppService.CheckCapitalization(customerAccountDTO.Id, recurringBatchEntryDTO.RecurringBatchMonth, recurringBatchEntryDTO.InterestCapitalizationMonths, serviceHeader) == 0)
                {
                    var principalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 1, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                    if (principalBalance * -1 > 0m) // Check that we have a principal balance..
                    {
                        var interestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                        if ((interestBalance * -1) >= (principalBalance * -1))
                        {
                            builder.AppendLine("interest balance >= principal balance");
                        }
                        else
                        {
                            var journals = new List<Journal>();

                            var loanProductDTO = _loanProductAppService.FindCachedLoanProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                            var primaryDescription = recurringBatchEntryDTO.InterestCapitalizationMonths > 1 ?
                                string.Format("Interest Charged~{0}_Months", recurringBatchEntryDTO.InterestCapitalizationMonths) :
                                string.Format("Interest Charged~{0}", recurringBatchEntryDTO.RecurringBatchReference);

                            var secondaryDescription = recurringBatchEntryDTO.InterestCapitalizationMonths > 1 ?
                                string.Format("{0}~{1}_Months", loanProductDTO.Description, recurringBatchEntryDTO.InterestCapitalizationMonths) :
                                string.Format("{0}~{1}", loanProductDTO.Description, recurringBatchEntryDTO.RecurringBatchMonthDescription);

                            var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                            switch ((InterestCalculationMode)loanProductDTO.LoanInterestCalculationMode)
                            {
                                case InterestCalculationMode.ReducingBalance:
                                case InterestCalculationMode.DiminishingBalanceAmortization:

                                    // calculate rate
                                    double rb_Rate = (loanProductDTO.LoanInterestAnnualPercentageRate / 100) / loanProductDTO.LoanRegistrationPaymentFrequencyPerYear;

                                    // calculate interest
                                    var rb_InterestAmount = Math.Abs(principalBalance * Convert.ToDecimal(rb_Rate));

                                    // do we need to reset?
                                    rb_InterestAmount = Math.Max(rb_InterestAmount, loanProductDTO.LoanRegistrationMinimumInterestAmount);

                                    // do we need to multiply?
                                    if (recurringBatchEntryDTO.InterestCapitalizationMonths > 1)
                                        rb_InterestAmount = rb_InterestAmount * recurringBatchEntryDTO.InterestCapitalizationMonths;

                                    // do we need to round?
                                    switch ((RoundingType)loanProductDTO.LoanRegistrationRoundingType)
                                    {
                                        case RoundingType.ToEven:
                                            rb_InterestAmount = Math.Round(rb_InterestAmount, MidpointRounding.ToEven);
                                            break;
                                        case RoundingType.AwayFromZero:
                                            rb_InterestAmount = Math.Round(rb_InterestAmount, MidpointRounding.AwayFromZero);
                                            break;
                                        case RoundingType.Ceiling:
                                            rb_InterestAmount = Math.Ceiling(rb_InterestAmount);
                                            break;
                                        case RoundingType.Floor:
                                            rb_InterestAmount = Math.Floor(rb_InterestAmount);
                                            break;
                                        default:
                                            break;
                                    }

                                    // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                                    var rb_InterestJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerAccountDTO.BranchId, null, rb_InterestAmount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.InterestCapitalization, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(rb_InterestJournal, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                    journals.Add(rb_InterestJournal);

                                    var rb_InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                                    if (rb_InterestBalance * -1 > 0m) // Check that we have interest balance..
                                    {
                                        rb_InterestAmount += (rb_InterestBalance * -1);
                                    }

                                    // update S/Order interest
                                    _sqlCommandAppService.UpdateStandingOrderCapitalizedInterest(customerAccountDTO.Id, rb_InterestAmount, serviceHeader);

                                    break;
                                case InterestCalculationMode.StraightLine:
                                case InterestCalculationMode.StraightLineAmortization:

                                    var standingOrderDTOs = FindStandingOrdersByBeneficiaryCustomerAccountId(customerAccountDTO.Id, serviceHeader);

                                    if (standingOrderDTOs != null && standingOrderDTOs.Any())
                                    {
                                        foreach (var standingOrder in standingOrderDTOs)
                                        {
                                            if (standingOrder.IsLocked) continue;

                                            if (standingOrder.BeneficiaryCustomerAccountCustomerId == customerAccountDTO.CustomerId)
                                            {
                                                switch ((InterestChargeMode)loanProductDTO.LoanInterestChargeMode)
                                                {
                                                    case InterestChargeMode.Upfront:

                                                        if (UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate) > standingOrder.DurationEndDate)
                                                        {
                                                            // calculate rate
                                                            double upfront_sla_Rate = (loanProductDTO.LoanInterestAnnualPercentageRate / 100) / loanProductDTO.LoanRegistrationPaymentFrequencyPerYear;

                                                            // calculate interest
                                                            var upfront_sla_InterestAmount = Math.Abs(principalBalance * Convert.ToDecimal(upfront_sla_Rate));

                                                            // do we need to reset?
                                                            upfront_sla_InterestAmount = Math.Max(upfront_sla_InterestAmount, loanProductDTO.LoanRegistrationMinimumInterestAmount);

                                                            // do we need to multiply?
                                                            if (recurringBatchEntryDTO.InterestCapitalizationMonths > 1)
                                                                upfront_sla_InterestAmount = upfront_sla_InterestAmount * recurringBatchEntryDTO.InterestCapitalizationMonths;

                                                            // do we need to round?
                                                            switch ((RoundingType)loanProductDTO.LoanRegistrationRoundingType)
                                                            {
                                                                case RoundingType.ToEven:
                                                                    upfront_sla_InterestAmount = Math.Round(upfront_sla_InterestAmount, MidpointRounding.ToEven);
                                                                    break;
                                                                case RoundingType.AwayFromZero:
                                                                    upfront_sla_InterestAmount = Math.Round(upfront_sla_InterestAmount, MidpointRounding.AwayFromZero);
                                                                    break;
                                                                case RoundingType.Ceiling:
                                                                    upfront_sla_InterestAmount = Math.Ceiling(upfront_sla_InterestAmount);
                                                                    break;
                                                                case RoundingType.Floor:
                                                                    upfront_sla_InterestAmount = Math.Floor(upfront_sla_InterestAmount);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }

                                                            // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                                                            var upfront_sla_InterestJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerAccountDTO.BranchId, null, upfront_sla_InterestAmount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.InterestCapitalization, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                                            _journalEntryPostingService.PerformDoubleEntry(upfront_sla_InterestJournal, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                            journals.Add(upfront_sla_InterestJournal);

                                                            var upfront_sla_InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                                                            if (upfront_sla_InterestBalance * -1 > 0m) // Check that we have interest balance..
                                                            {
                                                                upfront_sla_InterestAmount += (upfront_sla_InterestBalance * -1);
                                                            }

                                                            // update S/Order interest
                                                            _sqlCommandAppService.UpdateStandingOrderCapitalizedInterest(customerAccountDTO.Id, upfront_sla_InterestAmount, serviceHeader);
                                                        }

                                                        break;
                                                    case InterestChargeMode.Periodic:

                                                        if (UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate) <= standingOrder.DurationEndDate)
                                                        {
                                                            if (standingOrder.CapitalizedInterest * -1 < 0m)
                                                            {
                                                                if (recurringBatchEntryDTO.InterestCapitalizationMonths > 1)
                                                                {
                                                                    // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                                                                    var interestJournal_0 = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerAccountDTO.BranchId, null, standingOrder.CapitalizedInterest * recurringBatchEntryDTO.InterestCapitalizationMonths, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.InterestCapitalization, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                                                    _journalEntryPostingService.PerformDoubleEntry(interestJournal_0, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                                    journals.Add(interestJournal_0);
                                                                }
                                                                else
                                                                {
                                                                    // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                                                                    var interestJournal_1 = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerAccountDTO.BranchId, null, standingOrder.CapitalizedInterest, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.InterestCapitalization, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                                                    _journalEntryPostingService.PerformDoubleEntry(interestJournal_1, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                                    journals.Add(interestJournal_1);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                builder.AppendLine("standing order capitalized interest <= 0");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // calculate rate
                                                            double periodic_sla_Rate = (loanProductDTO.LoanInterestAnnualPercentageRate / 100) / loanProductDTO.LoanRegistrationPaymentFrequencyPerYear;

                                                            // calculate interest
                                                            var periodic_sla_InterestAmount = Math.Abs(principalBalance * Convert.ToDecimal(periodic_sla_Rate));

                                                            // do we need to reset?
                                                            periodic_sla_InterestAmount = Math.Max(periodic_sla_InterestAmount, loanProductDTO.LoanRegistrationMinimumInterestAmount);

                                                            // do we need to multiply?
                                                            if (recurringBatchEntryDTO.InterestCapitalizationMonths > 1)
                                                                periodic_sla_InterestAmount = periodic_sla_InterestAmount * recurringBatchEntryDTO.InterestCapitalizationMonths;

                                                            // do we need to round?
                                                            switch ((RoundingType)loanProductDTO.LoanRegistrationRoundingType)
                                                            {
                                                                case RoundingType.ToEven:
                                                                    periodic_sla_InterestAmount = Math.Round(periodic_sla_InterestAmount, MidpointRounding.ToEven);
                                                                    break;
                                                                case RoundingType.AwayFromZero:
                                                                    periodic_sla_InterestAmount = Math.Round(periodic_sla_InterestAmount, MidpointRounding.AwayFromZero);
                                                                    break;
                                                                case RoundingType.Ceiling:
                                                                    periodic_sla_InterestAmount = Math.Ceiling(periodic_sla_InterestAmount);
                                                                    break;
                                                                case RoundingType.Floor:
                                                                    periodic_sla_InterestAmount = Math.Floor(periodic_sla_InterestAmount);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }

                                                            // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                                                            var periodic_sla_InterestJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerAccountDTO.BranchId, null, periodic_sla_InterestAmount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.InterestCapitalization, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                                            _journalEntryPostingService.PerformDoubleEntry(periodic_sla_InterestJournal, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                                            journals.Add(periodic_sla_InterestJournal);

                                                            var periodic_sla_InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccountDTO, 2, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                                                            if (periodic_sla_InterestBalance * -1 > 0m) // Check that we have interest balance..
                                                            {
                                                                periodic_sla_InterestAmount += (periodic_sla_InterestBalance * -1);
                                                            }

                                                            // update S/Order interest
                                                            _sqlCommandAppService.UpdateStandingOrderCapitalizedInterest(customerAccountDTO.Id, periodic_sla_InterestAmount, serviceHeader);
                                                        }

                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    break;

                                default:
                                    break;
                            }

                            #region Bulk-Insert journals && journal entries

                            if (journals.Any())
                            {
                                result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                            }
                            else builder.AppendLine("no transactions");

                            #endregion
                        }
                    }
                    else builder.AppendLine("no principal balance");
                }
                else builder.AppendLine("already capitalized");
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> ExecuteStandingOrder(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            bool result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriodDTO = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriodDTO != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.StandingOrder != null)
            {
                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var standingOrderDTO = recurringBatchEntryDTO.StandingOrder;

                var benefactorCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BenefactorCustomerAccountId, serviceHeader);

                var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                if (benefactorCustomerAccountDTO != null && benefactorCustomerAccountDTO.Status != (int)CustomerAccountStatus.Closed && beneficiaryCustomerAccountDTO != null && beneficiaryCustomerAccountDTO.Status != (int)CustomerAccountStatus.Closed)
                {
                    _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { benefactorCustomerAccountDTO, beneficiaryCustomerAccountDTO }, serviceHeader, true);

                    var benefactorAccountAvailableBalance = benefactorCustomerAccountDTO.AvailableBalance;

                    if (benefactorAccountAvailableBalance * -1 < 0m)
                    {
                        var expectedPrincipal = 0m;

                        var expectedInterest = 0m;

                        var actualPrincipal = 0m;

                        var actualInterest = 0m;

                        var principalArrears = 0m;

                        var interestArrears = 0m;

                        switch ((ProductCode)standingOrderDTO.BeneficiaryCustomerAccountCustomerAccountTypeProductCode)
                        {
                            case ProductCode.Loan:

                                expectedPrincipal = standingOrderDTO.Principal;
                                expectedInterest = standingOrderDTO.Interest;

                                break;
                            case ProductCode.Savings:

                                if (standingOrderDTO.Trigger == (int)StandingOrderTrigger.Sweep)
                                    expectedPrincipal = benefactorAccountAvailableBalance;
                                else
                                {
                                    switch ((ChargeType)standingOrderDTO.ChargeType)
                                    {
                                        case ChargeType.Percentage:
                                            expectedPrincipal = Math.Round(Convert.ToDecimal((standingOrderDTO.ChargePercentage * Convert.ToDouble(benefactorAccountAvailableBalance)) / 100), 4, MidpointRounding.AwayFromZero);
                                            break;
                                        case ChargeType.FixedAmount:
                                            expectedPrincipal = standingOrderDTO.ChargeFixedAmount;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                break;
                            case ProductCode.Investment:

                                switch ((ChargeType)standingOrderDTO.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        expectedPrincipal = Math.Round(Convert.ToDecimal((standingOrderDTO.ChargePercentage * Convert.ToDouble(benefactorAccountAvailableBalance)) / 100), 4, MidpointRounding.AwayFromZero);
                                        break;
                                    case ChargeType.FixedAmount:
                                        expectedPrincipal = standingOrderDTO.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            default:
                                break;
                        }

                        if ((expectedPrincipal + expectedInterest) > 0m)
                        {
                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { benefactorCustomerAccountDTO, beneficiaryCustomerAccountDTO }, serviceHeader);

                            var journals = new List<Journal>();

                            var standingOrderHistories = new List<StandingOrderHistory>();

                            var customerAccountArrearages = new List<CustomerAccountArrearage>();

                            var primaryDescription = string.Format("Standing Order~{0}", recurringBatchEntryDTO.RecurringBatchReference);

                            var secondaryDescription = string.Format("{0}~{1}~{2}", string.Format("{0}~{1}", beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductDescription, standingOrderDTO.BeneficiaryCustomerAccountCustomerReference1), string.Format("{0}~{1}", benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductDescription, standingOrderDTO.BenefactorCustomerAccountCustomerReference1), recurringBatchEntryDTO.RecurringBatchMonthDescription);

                            var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                            switch ((ProductCode)beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode)
                            {
                                case ProductCode.Savings:

                                    actualPrincipal = expectedPrincipal;

                                    actualInterest = expectedInterest;

                                    if (CheckRecovery(standingOrderDTO, postingPeriodDTO.Id, recurringBatchEntryDTO.RecurringBatchMonth, serviceHeader))
                                    {
                                        builder.AppendLine("schedule arrears recovery");

                                        actualPrincipal = 0m;
                                        actualInterest = 0m;
                                        expectedPrincipal = 0m;
                                        expectedInterest = 0m;
                                    }
                                    else builder.AppendLine("execute fresh recovery");

                                    if ((actualPrincipal + actualPrincipal) > 0m)
                                    {
                                        // Do we need to reset actual values?
                                        if (!(((benefactorAccountAvailableBalance) - (actualPrincipal + actualInterest)) >= 0m))
                                        {
                                            // reset actual interest & actual principal >> NB: interest has priority over principal!
                                            actualInterest = Math.Min(actualInterest, benefactorAccountAvailableBalance);
                                            actualPrincipal = benefactorAccountAvailableBalance - actualInterest;
                                        }

                                        var savingsJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, actualPrincipal + actualInterest, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(savingsJournal, beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(savingsJournal);

                                        var savingsHistory = StandingOrderHistoryFactory.CreateStandingOrderHistory(standingOrderDTO.Id, postingPeriodDTO.Id, standingOrderDTO.BenefactorCustomerAccountId, standingOrderDTO.BeneficiaryCustomerAccountId, new Duration(standingOrderDTO.DurationStartDate, standingOrderDTO.DurationEndDate), new Schedule(standingOrderDTO.ScheduleFrequency, standingOrderDTO.ScheduleExpectedRunDate, standingOrderDTO.ScheduleActualRunDate, standingOrderDTO.ScheduleExecuteAttemptCount, standingOrderDTO.ScheduleForceExecute), new Charge(standingOrderDTO.ChargeType, standingOrderDTO.ChargePercentage, standingOrderDTO.ChargeFixedAmount), recurringBatchEntryDTO.RecurringBatchMonth, standingOrderDTO.Trigger, standingOrderDTO.Principal, standingOrderDTO.Interest, expectedPrincipal, expectedInterest, standingOrderDTO.Remarks);
                                        savingsHistory.CreatedBy = serviceHeader.ApplicationUserName;
                                        standingOrderHistories.Add(savingsHistory);
                                    }

                                    break;
                                case ProductCode.Investment:

                                    var investmentAccountProduct = _investmentProductAppService.FindCachedInvestmentProduct(beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                                    actualPrincipal = expectedPrincipal;

                                    actualInterest = expectedInterest;

                                    if (investmentAccountProduct.TrackArrears)
                                    {
                                        principalArrears = beneficiaryCustomerAccountDTO.PrincipalArrearagesBalance * -1 < 0m ? beneficiaryCustomerAccountDTO.PrincipalArrearagesBalance : 0m;
                                        interestArrears = beneficiaryCustomerAccountDTO.InterestArrearagesBalance * -1 < 0m ? beneficiaryCustomerAccountDTO.InterestArrearagesBalance : 0m;
                                    }

                                    if (CheckRecovery(standingOrderDTO, postingPeriodDTO.Id, recurringBatchEntryDTO.RecurringBatchMonth, serviceHeader))
                                    {
                                        builder.AppendLine("schedule arrears recovery");

                                        actualPrincipal = 0m;
                                        actualInterest = 0m;
                                        expectedPrincipal = 0m;
                                        expectedInterest = 0m;
                                    }
                                    else builder.AppendLine("execute fresh recovery");

                                    if ((actualPrincipal + actualPrincipal) > 0m)
                                    {
                                        // Do we need to reset actual values?
                                        if (!(((benefactorAccountAvailableBalance) - (actualPrincipal + actualInterest)) >= 0m))
                                        {
                                            // reset actual interest & actual principal >> NB: interest has priority over principal!
                                            actualInterest = Math.Min(actualInterest, benefactorAccountAvailableBalance);
                                            actualPrincipal = benefactorAccountAvailableBalance - actualInterest;

                                            // track new arrears
                                            if (investmentAccountProduct.TrackArrears)
                                            {
                                                principalArrears += (expectedPrincipal - actualPrincipal);
                                                interestArrears += (expectedInterest - actualInterest);
                                            }
                                        }
                                        else
                                        {
                                            // reset old arrears
                                            if (investmentAccountProduct.TrackArrears)
                                            {
                                                interestArrears = interestArrears - actualInterest;
                                                interestArrears = interestArrears * -1 > 0m ? 0m : interestArrears;

                                                principalArrears = principalArrears - actualPrincipal;
                                                principalArrears = principalArrears * -1 > 0m ? 0m : principalArrears;
                                            }
                                        }

                                        var investmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, actualPrincipal + actualInterest, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(investmentJournal, beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(investmentJournal);

                                        var investmentHistory = StandingOrderHistoryFactory.CreateStandingOrderHistory(standingOrderDTO.Id, postingPeriodDTO.Id, standingOrderDTO.BenefactorCustomerAccountId, standingOrderDTO.BeneficiaryCustomerAccountId, new Duration(standingOrderDTO.DurationStartDate, standingOrderDTO.DurationEndDate), new Schedule(standingOrderDTO.ScheduleFrequency, standingOrderDTO.ScheduleExpectedRunDate, standingOrderDTO.ScheduleActualRunDate, standingOrderDTO.ScheduleExecuteAttemptCount, standingOrderDTO.ScheduleForceExecute), new Charge(standingOrderDTO.ChargeType, standingOrderDTO.ChargePercentage, standingOrderDTO.ChargeFixedAmount), recurringBatchEntryDTO.RecurringBatchMonth, standingOrderDTO.Trigger, standingOrderDTO.Principal, standingOrderDTO.Interest, expectedPrincipal, expectedInterest, standingOrderDTO.Remarks);
                                        investmentHistory.CreatedBy = serviceHeader.ApplicationUserName;
                                        standingOrderHistories.Add(investmentHistory);

                                        // Do we need to update arrearage history?
                                        var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Interest, interestArrears, reference);
                                        customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                        customerAccountArrearages.Add(customerAccountInterestArrearage);

                                        var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Principal, principalArrears, reference);
                                        customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                        customerAccountArrearages.Add(customerAccountPrincipalArrearage);
                                    }

                                    break;
                                case ProductCode.Loan:

                                    var loanAccountProduct = _loanProductAppService.FindCachedLoanProduct(beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                                    var principalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(beneficiaryCustomerAccountDTO, 1, DateTime.Now, serviceHeader);

                                    var interestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(beneficiaryCustomerAccountDTO, 2, DateTime.Now, serviceHeader);

                                    actualPrincipal = Math.Min(((principalBalance * -1 > 0m) ? (principalBalance * -1) : 0m), expectedPrincipal);

                                    actualInterest = Math.Min(((interestBalance * -1 > 0m) ? (interestBalance * -1) : 0m), expectedInterest);

                                    if (loanAccountProduct.LoanRegistrationTrackArrears)
                                    {
                                        principalArrears = Math.Min(((principalBalance * -1 > 0m) ? (principalBalance * -1) : 0m), beneficiaryCustomerAccountDTO.PrincipalArrearagesBalance * -1 < 0m ? beneficiaryCustomerAccountDTO.PrincipalArrearagesBalance : 0m);
                                        interestArrears = Math.Min(((interestBalance * -1 > 0m) ? (interestBalance * -1) : 0m), beneficiaryCustomerAccountDTO.InterestArrearagesBalance * -1 < 0m ? beneficiaryCustomerAccountDTO.InterestArrearagesBalance : 0m);
                                    }

                                    if (CheckRecovery(standingOrderDTO, postingPeriodDTO.Id, recurringBatchEntryDTO.RecurringBatchMonth, serviceHeader))
                                    {
                                        builder.AppendLine("schedule arrears recovery");

                                        actualPrincipal = 0m;
                                        actualInterest = 0m;
                                        expectedPrincipal = 0m;
                                        expectedInterest = 0m;
                                    }
                                    else builder.AppendLine("execute fresh recovery");

                                    if ((actualPrincipal + actualPrincipal) > 0m)
                                    {
                                        // Do we need to reset actual values?
                                        if (!(((benefactorAccountAvailableBalance) - (actualPrincipal + actualInterest)) >= 0m))
                                        {
                                            // reset actual interest & actual principal >> NB: interest has priority over principal!
                                            actualInterest = Math.Min(actualInterest, benefactorAccountAvailableBalance);
                                            actualPrincipal = benefactorAccountAvailableBalance - actualInterest;

                                            // track new arrears
                                            if (loanAccountProduct.LoanRegistrationTrackArrears)
                                            {
                                                principalArrears = (expectedPrincipal - actualPrincipal);
                                                interestArrears = (expectedInterest - actualInterest);
                                            }
                                        }
                                        else
                                        {
                                            // reset old arrears
                                            if (loanAccountProduct.LoanRegistrationTrackArrears)
                                            {
                                                interestArrears = interestArrears - actualInterest;
                                                interestArrears = interestArrears * -1 > 0m ? 0m : interestArrears;

                                                principalArrears = principalArrears - actualPrincipal;
                                                principalArrears = principalArrears * -1 > 0m ? 0m : principalArrears;
                                            }
                                        }

                                        // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit StandingOrderDTO.BenefactorProductChartOfAccountId
                                        var interestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, actualInterest, string.Format("{0} (Interest)", primaryDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(interestReceivableJournal, loanAccountProduct.InterestReceivableChartOfAccountId, benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(interestReceivableJournal);

                                        // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                                        var interestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, actualInterest, string.Format("{0} (Interest)", primaryDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(interestReceivedJournal, loanAccountProduct.InterestReceivedChartOfAccountId, loanAccountProduct.InterestChargedChartOfAccountId, beneficiaryCustomerAccountDTO, beneficiaryCustomerAccountDTO, serviceHeader);
                                        journals.Add(interestReceivedJournal);

                                        // Credit LoanProduct.ChartOfAccountId, Debit StandingOrderDTO.BenefactorProductChartOfAccountId
                                        var principalJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, actualPrincipal, string.Format("{0} (Principal)", primaryDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(principalJournal, loanAccountProduct.ChartOfAccountId, benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(principalJournal);

                                        var loanHistory = StandingOrderHistoryFactory.CreateStandingOrderHistory(standingOrderDTO.Id, postingPeriodDTO.Id, standingOrderDTO.BenefactorCustomerAccountId, standingOrderDTO.BeneficiaryCustomerAccountId, new Duration(standingOrderDTO.DurationStartDate, standingOrderDTO.DurationEndDate), new Schedule(standingOrderDTO.ScheduleFrequency, standingOrderDTO.ScheduleExpectedRunDate, standingOrderDTO.ScheduleActualRunDate, standingOrderDTO.ScheduleExecuteAttemptCount, standingOrderDTO.ScheduleForceExecute), new Charge(standingOrderDTO.ChargeType, standingOrderDTO.ChargePercentage, standingOrderDTO.ChargeFixedAmount), recurringBatchEntryDTO.RecurringBatchMonth, standingOrderDTO.Trigger, standingOrderDTO.Principal, standingOrderDTO.Interest, actualPrincipal, actualInterest, standingOrderDTO.Remarks);
                                        loanHistory.CreatedBy = serviceHeader.ApplicationUserName;
                                        standingOrderHistories.Add(loanHistory);

                                        // Do we need to update arrearage history?
                                        var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Interest, interestArrears, reference);
                                        customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                        customerAccountArrearages.Add(customerAccountInterestArrearage);

                                        var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Principal, principalArrears, reference);
                                        customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                        customerAccountArrearages.Add(customerAccountPrincipalArrearage);
                                    }
                                    break;
                                default:
                                    break;
                            }

                            if (standingOrderDTO.Chargeable && benefactorCustomerAccountDTO.CustomerAccountTypeProductCode == (int)ProductCode.Savings && ((actualPrincipal + actualInterest) > 0m))
                            {
                                var standingOrderTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductId, (int)SavingsProductKnownChargeType.StandingOrderFee, 0m, benefactorCustomerAccountDTO, serviceHeader);

                                if (standingOrderTariffs != null && standingOrderTariffs.Any())
                                {
                                    standingOrderTariffs.ForEach(tariff =>
                                    {
                                        var actualTariffAmount = tariff.Amount;

                                        // Do we need to reset expected values?
                                        if (!(((benefactorAccountAvailableBalance) - (actualPrincipal + actualInterest)) >= 0m))
                                        {
                                            // how much is available for recovery?
                                            var availableRecoveryAmount = (benefactorAccountAvailableBalance) - (actualPrincipal + actualInterest);

                                            // reset expected tariff amount
                                            actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);
                                        }

                                        var standingOrderTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(standingOrderTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(standingOrderTariffJournal);
                                    });
                                }
                            }

                            #region Bulk-Insert journals

                            if (journals.Any())
                            {
                                result = _journalEntryPostingService.BulkSave(serviceHeader, journals, null, standingOrderHistories, customerAccountArrearages);
                            }
                            else builder.AppendLine("no transactions");

                            #endregion
                        }
                        else builder.AppendLine(string.Format("expectedPrincipal + expectedInterest is {0}", string.Format(_nfi, "{0:C}", expectedPrincipal + expectedInterest)));
                    }
                    else builder.AppendLine(string.Format("benefactor account available balance is {0}", string.Format(_nfi, "{0:C}", benefactorAccountAvailableBalance)));
                }
                else builder.AppendLine(string.Format("validation of account(s) status has failed: benefactor={0}, beneficiary={1}", benefactorCustomerAccountDTO.StatusDescription, beneficiaryCustomerAccountDTO.StatusDescription));
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> ChargeDynamicSavingsFees(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriodDTO = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriodDTO != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.CustomerAccount != null)
            {
                var journals = new List<Journal>();

                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var customerAccountDTO = recurringBatchEntryDTO.CustomerAccount;

                var savingsProductDTO = _savingsProductAppService.FindCachedSavingsProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, customerAccountDTO.BranchId, serviceHeader);

                var secondaryDescription = string.Format("{0}~{1}", savingsProductDTO.Description, recurringBatchEntryDTO.RecurringBatchMonthDescription);

                var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                var dynamicFeesTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(savingsProductDTO.Id, (int)SavingsProductKnownChargeType.DynamicFee, 0m, customerAccountDTO, serviceHeader);

                if (dynamicFeesTariffs.Any())
                {
                    var totalRecoveryDeductions = 0m;

                    var availableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(customerAccountDTO, DateTime.Now, serviceHeader);

                    if ((availableBalance > 0m /*Will current account balance be positive?*/))
                    {
                        dynamicFeesTariffs.ForEach(tariff =>
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
                                var availableRecoveryAmount = (availableBalance) - totalRecoveryDeductions;
                                availableRecoveryAmount = (availableRecoveryAmount * -1 > 0m) ? 0m : availableRecoveryAmount;

                                // reset expected tariff amount
                                actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                // track deductions
                                totalRecoveryDeductions += actualTariffAmount;
                            }

                            var tariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerAccountDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.SavingsLedgerFee, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(tariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            journals.Add(tariffJournal);
                        });
                    }
                }
                else builder.AppendLine("no indefinite charges");

                #region Bulk-Insert journals && journal entries

                if (journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
                else builder.AppendLine("no transactions");

                #endregion
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> ChargeIndefiniteLoanCharges(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriodDTO = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            var defaultSavingsProduct = _savingsProductAppService.FindCachedDefaultSavingsProduct(serviceHeader);

            if (postingPeriodDTO != null && defaultSavingsProduct != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.CustomerAccount != null)
            {
                var journals = new List<Journal>();

                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var customerAccountDTO = recurringBatchEntryDTO.CustomerAccount;

                var loanProductDTO = _loanProductAppService.FindCachedLoanProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                var secondaryDescription = string.Format("{0}~{1}", loanProductDTO.Description, recurringBatchEntryDTO.RecurringBatchMonthDescription);

                var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true, false);

                if (customerAccountDTO.BookBalance * -1 > 0m) // IFF has loan balance
                {
                    var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerAccountDTO.CustomerId, defaultSavingsProduct.Id, serviceHeader);

                    if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                    {
                        var customerSavingsAccountDTO = customerSavingsAccounts.First();

                        var periodicDynamicFeesTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanProductDTO.Id, (int)DynamicChargeRecoverySource.SavingsAccount, (int)DynamicChargeRecoveryMode.Periodic, 0m, customerSavingsAccountDTO, serviceHeader);

                        if (periodicDynamicFeesTariffs.Any())
                        {
                            var totalRecoveryDeductions = 0m;

                            var availableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(customerSavingsAccountDTO, DateTime.Now, serviceHeader);

                            if (availableBalance > 0m /*Will current account balance be positive?*/)
                            {
                                periodicDynamicFeesTariffs.ForEach(tariff =>
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
                                        var availableRecoveryAmount = (availableBalance) - totalRecoveryDeductions;
                                        availableRecoveryAmount = (availableRecoveryAmount * -1 > 0m) ? 0m : availableRecoveryAmount;

                                        // reset expected tariff amount
                                        actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                        // track deductions
                                        totalRecoveryDeductions += actualTariffAmount;
                                    }

                                    var tariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerSavingsAccountDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanLedgerFee, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(tariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                    journals.Add(tariffJournal);
                                });
                            }
                        }
                        else builder.AppendLine("no indefinite charges");
                    }
                    else builder.AppendLine(string.Format("no default {0} account", defaultSavingsProduct.Description));
                }
                else builder.AppendLine(string.Format("book balance is {0}", string.Format(_nfi, "{0:C}", customerAccountDTO.BookBalance)));

                #region Bulk-Insert journals && journal entries

                if (journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
                else builder.AppendLine("no transactions");

                #endregion
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> AdjustInvestmentBalance(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriodDTO = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriodDTO != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.CustomerAccount != null && recurringBatchEntryDTO.SecondaryCustomerAccount != null)
            {
                var sourceProduct = _investmentProductAppService.FindCachedInvestmentProduct(recurringBatchEntryDTO.CustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                var destinationProduct = _investmentProductAppService.FindCachedInvestmentProduct(recurringBatchEntryDTO.SecondaryCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                var sourceProductAccount = recurringBatchEntryDTO.CustomerAccount;

                var destinationProductAccount = recurringBatchEntryDTO.SecondaryCustomerAccount;

                if (destinationProductAccount.BranchCompanyEnforceInvestmentProductExemptions && destinationProductAccount.CustomerType == (int)CustomerType.Individual && destinationProductAccount.CustomerIndividualClassification != (byte)CustomerClassification.Unknown)
                {
                    var investmentProductExemptionDTO = _investmentProductAppService.FindInvestmentProductExemption(destinationProduct.Id, destinationProductAccount.CustomerIndividualClassification, serviceHeader);

                    if (investmentProductExemptionDTO != null)
                        destinationProduct.MaximumBalance = investmentProductExemptionDTO.MaximumBalance;
                }

                var journals = new List<Journal>();

                var primaryDescription = string.Format("Normalization~{0}", recurringBatchEntryDTO.RecurringBatchReference);

                var secondaryDescription = string.Format("{0}~{1}", string.Format("{0}~{1}", sourceProduct.Description, destinationProduct.Description), recurringBatchEntryDTO.RecurringBatchMonthDescription);

                var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                sourceProductAccount.BookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(sourceProductAccount, 1, DateTime.Now, serviceHeader);

                if (sourceProductAccount.BookBalance * -1 < 0m)
                {
                    if (sourceProductAccount.BookBalance <= sourceProduct.MinimumBalance)
                    {
                        builder.AppendLine("source product account book balance less than or equal to source product minimum balance");
                    }
                    else
                    {
                        destinationProductAccount.BookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(destinationProductAccount, 1, DateTime.Now, serviceHeader);

                        if (destinationProductAccount.BookBalance < destinationProduct.MinimumBalance)
                        {
                            // debit source, credit destination
                            var transactionAmount = 0m;

                            if (destinationProductAccount.BookBalance * -1 <= 0m)
                                transactionAmount = Math.Min((destinationProduct.MinimumBalance - destinationProductAccount.BookBalance), (sourceProductAccount.BookBalance - sourceProduct.MinimumBalance));
                            else transactionAmount = Math.Min(destinationProduct.MinimumBalance, (sourceProductAccount.BookBalance - sourceProduct.MinimumBalance));

                            transactionAmount = Math.Max(0m, transactionAmount);

                            if (transactionAmount * -1 < 0m)
                            {
                                var normalizationJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, destinationProductAccount.BranchId, null, transactionAmount, primaryDescription, string.Format("CR_{0}/DR_{1}", string.Format("{0}", destinationProduct.PaddedCode).Trim(), string.Format("{0}", sourceProduct.PaddedCode)), reference, moduleNavigationItemCode, (int)SystemTransactionCode.InvestmentBalancesAdjustment, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                                _journalEntryPostingService.PerformDoubleEntry(normalizationJournal, destinationProduct.ChartOfAccountId, sourceProduct.ChartOfAccountId, destinationProductAccount, sourceProductAccount, serviceHeader);

                                journals.Add(normalizationJournal);
                            }
                        }
                        else if (destinationProductAccount.BookBalance > destinationProduct.MaximumBalance)
                        {
                            // debit destination, credit source
                            if (recurringBatchEntryDTO.EnforceCeiling)
                            {
                                var transactionAmount = (destinationProductAccount.BookBalance - destinationProduct.MaximumBalance);

                                transactionAmount = Math.Max(0m, transactionAmount);

                                if (transactionAmount * -1 < 0m)
                                {
                                    var normalizationJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, destinationProductAccount.BranchId, null, transactionAmount, primaryDescription, string.Format("CR_{0}/DR_{1}", string.Format("{0}", sourceProduct.PaddedCode).Trim(), string.Format("{0}", destinationProduct.PaddedCode)), reference, moduleNavigationItemCode, (int)SystemTransactionCode.InvestmentBalancesAdjustment, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                                    _journalEntryPostingService.PerformDoubleEntry(normalizationJournal, sourceProduct.ChartOfAccountId, destinationProduct.ChartOfAccountId, sourceProductAccount, destinationProductAccount, serviceHeader);

                                    journals.Add(normalizationJournal);
                                }
                            }
                        }
                        else
                        {
                            /* within range - adjust to destination max/* 
                             * / debit source, credit destination*/
                            if (recurringBatchEntryDTO.EnforceCeiling)
                            {
                                var transactionAmount = Math.Min((destinationProduct.MaximumBalance - destinationProductAccount.BookBalance), (sourceProductAccount.BookBalance - sourceProduct.MinimumBalance));

                                transactionAmount = Math.Max(0m, transactionAmount);

                                if (transactionAmount * -1 < 0m)
                                {
                                    var normalizationJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, destinationProductAccount.BranchId, null, transactionAmount, primaryDescription, string.Format("CR_{0}/DR_{1}", string.Format("{0}", destinationProduct.PaddedCode).Trim(), string.Format("{0}", sourceProduct.PaddedCode)), reference, moduleNavigationItemCode, (int)SystemTransactionCode.InvestmentBalancesAdjustment, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

                                    _journalEntryPostingService.PerformDoubleEntry(normalizationJournal, destinationProduct.ChartOfAccountId, sourceProduct.ChartOfAccountId, destinationProductAccount, sourceProductAccount, serviceHeader);

                                    journals.Add(normalizationJournal);
                                }
                            }
                        };
                    }
                }
                else builder.AppendLine(string.Format("source product account book balance is {0}", string.Format(_nfi, "{0:C}", sourceProductAccount.BookBalance)));

                #region Bulk-Insert journals && journal entries

                if (journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
                else builder.AppendLine("no transactions");

                #endregion
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> PoolInvestmentBalance(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriodDTO = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriodDTO != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.CustomerAccount != null)
            {
                var sourceProduct = _investmentProductAppService.FindCachedInvestmentProduct(recurringBatchEntryDTO.CustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                if (sourceProduct.IsPooled && sourceProduct.PoolChartOfAccountId != null && sourceProduct.PoolChartOfAccountId != Guid.Empty)
                {
                    var journals = new List<Journal>();

                    var primaryDescription = string.Format("Pooling~{0}", recurringBatchEntryDTO.RecurringBatchReference);

                    var secondaryDescription = string.Format("{0}~{1}", sourceProduct.Description, recurringBatchEntryDTO.RecurringBatchMonthDescription);

                    var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                    var sourceProductAccount = recurringBatchEntryDTO.CustomerAccount;

                    sourceProductAccount.BookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(sourceProductAccount, 1, DateTime.Now, serviceHeader);

                    if (sourceProductAccount.BookBalance * -1 < 0m)
                    {
                        // Credit InvestmentProduct.PoolChartOfAccountId, Debit InvestmentProduct.ChartOfAccountId
                        var poolInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, sourceProductAccount.BranchId, null, sourceProductAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.InvestmentBalancesPooling, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);
                        _journalEntryPostingService.PerformDoubleEntry(poolInvestmentJournal, sourceProduct.PoolChartOfAccountId.Value, sourceProduct.ChartOfAccountId, sourceProductAccount, sourceProductAccount, serviceHeader);
                        journals.Add(poolInvestmentJournal);
                    }
                    else builder.AppendLine(string.Format("source product account book balance is {0}", string.Format(_nfi, "{0:C}", sourceProductAccount.BookBalance)));

                    #region Bulk-Insert journals && journal entries

                    if (journals.Any())
                    {
                        result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                    }
                    else builder.AppendLine("no transactions");

                    #endregion
                }
                else builder.AppendLine(string.Format("source product is not pooled!"));
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> ReleaseLoanGuarantors(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var builder = new StringBuilder();

            if (recurringBatchEntryDTO != null && recurringBatchEntryDTO.CustomerAccount != null)
            {
                var loanGuarantors = FindAttachedLoanGuarantors(recurringBatchEntryDTO.CustomerAccount, serviceHeader);

                var loanCollaterals = FindAttachedLoanCollaterals(recurringBatchEntryDTO.CustomerAccount, serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    if (loanGuarantors != null && loanGuarantors.Any())
                    {
                        foreach (var item in loanGuarantors)
                        {
                            var persisted = _loanGuarantorRepository.Get(item.Id, serviceHeader);

                            if (persisted != null)
                            {
                                var releaseGuarantor = true;

                                if (persisted.LoanCase != null)
                                    releaseGuarantor = persisted.LoanCase.Status.In((int)LoanCaseStatus.Disbursed, (int)LoanCaseStatus.Rejected);

                                if (releaseGuarantor)
                                    persisted.Status = (int)LoanGuarantorStatus.Released;
                            }
                        }
                    }
                    else builder.AppendLine("no attached guarantors");

                    if (loanCollaterals != null && loanCollaterals.Any())
                    {
                        foreach (var item in loanCollaterals)
                        {
                            var persisted = _customerDocumentRepository.Get(item.Id, serviceHeader);

                            if (persisted != null)
                            {
                                var collateral = new Collateral(persisted.Collateral.Value, persisted.Collateral.AdvanceRate, (int)CollateralStatus.Released);

                                var current = CustomerDocumentFactory.CreateCustomerDocument(persisted.CustomerId, persisted.Type, collateral, persisted.FileName, persisted.FileTitle, persisted.FileDescription, persisted.FileMIMEType);

                                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                                current.CreatedBy = persisted.CreatedBy;

                                _customerDocumentRepository.Merge(persisted, current, serviceHeader);
                            }
                        }
                    }
                    else builder.AppendLine("no attached collaterals");

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> ExecuteElectronicStatementOrder(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            bool result = default(bool);

            var builder = new StringBuilder();

            if (recurringBatchEntryDTO != null && recurringBatchEntryDTO.ElectronicStatementOrder != null)
            {
                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var electronicStatementOrderDTO = recurringBatchEntryDTO.ElectronicStatementOrder;

                var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(electronicStatementOrderDTO.CustomerAccountId, serviceHeader);

                if (customerAccountDTO != null && customerAccountDTO.Status != (int)CustomerAccountStatus.Closed)
                {
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader);

                    var electronicStatementOrderHistories = new List<ElectronicStatementOrderHistory>();

                    var electronicStatementOrderHistory = ElectronicStatementOrderHistoryFactory.CreateElectronicStatementOrderHistory(electronicStatementOrderDTO.Id, electronicStatementOrderDTO.CustomerAccountId, new Duration(recurringBatchEntryDTO.ElectronicStatementStartDate, recurringBatchEntryDTO.ElectronicStatementEndDate), new Schedule(electronicStatementOrderDTO.ScheduleFrequency, electronicStatementOrderDTO.ScheduleExpectedRunDate, electronicStatementOrderDTO.ScheduleActualRunDate, electronicStatementOrderDTO.ScheduleExecuteAttemptCount, electronicStatementOrderDTO.ScheduleForceExecute), recurringBatchEntryDTO.ElectronicStatementSender, electronicStatementOrderDTO.Remarks);
                    electronicStatementOrderHistory.CreatedBy = serviceHeader.ApplicationUserName;
                    electronicStatementOrderHistories.Add(electronicStatementOrderHistory);

                    #region Bulk-Insert hisotries

                    if (electronicStatementOrderHistories.Any())
                    {
                        using (var dbContextScope = _dbContextScopeFactory.Create())
                        {
                            if (electronicStatementOrderHistories != null && electronicStatementOrderHistories.Any())
                            {
                                electronicStatementOrderHistories.ForEach(item =>
                                {
                                    _electronicStatementOrderHistoryRepository.Add(item, serviceHeader);
                                });
                            }

                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                        }

                        if (result)
                        {
                            var mediaDTO = _mediaAppService.PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(customerAccountDTO, recurringBatchEntryDTO.ElectronicStatementStartDate, recurringBatchEntryDTO.ElectronicStatementEndDate, false, false, moduleNavigationItemCode, blobDatabaseConnectionString, serviceHeader);

                            byte[] buffer = mediaDTO != null ? mediaDTO.Content : new byte[0] { };

                            if (buffer.Length != 0)
                            {
                                var fileName = string.Format("{0}_{1}_{2}.pdf", DateTime.Now.Ticks, electronicStatementOrderHistory.Id.ToString("D"), mediaDTO.FileRemarks);

                                var path = Path.Combine(fileDirectory, fileName);

                                using (var outputStream = System.IO.File.Create(path))
                                {
                                    outputStream.Write(buffer, 0, buffer.Length);
                                }

                                if (System.IO.File.Exists(path))
                                {
                                    mediaDTO.SKU = electronicStatementOrderHistory.Id;
                                    mediaDTO.FileName = path;
                                    mediaDTO.Content = null;

                                    if (_mediaAppService.PostFile(mediaDTO, fileDirectory, blobDatabaseConnectionString, serviceHeader))
                                    {
                                        builder.AppendLine(mediaDTO.FileRemarks);

                                        _brokerService.ProcessElectronicStatements(DMLCommand.None, serviceHeader, mediaDTO);
                                    }
                                    else builder.AppendLine("blob posting failed");
                                }
                                else builder.AppendLine("buffer writing failed");
                            }
                            else builder.AppendLine("empty buffer");
                        }
                        else builder.AppendLine("history persistence failed");
                    }
                    else builder.AppendLine("history generation failed");

                    #endregion
                }
                else builder.AppendLine(string.Format("validation of account status has failed: {0}", customerAccountDTO.StatusDescription));
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> RecoverArrears(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            bool result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriod = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriod = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriod = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriod != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.StandingOrder != null)
            {
                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var standingOrderDTO = recurringBatchEntryDTO.StandingOrder;

                var benefactorCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BenefactorCustomerAccountId, serviceHeader);

                var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                if (benefactorCustomerAccountDTO != null && benefactorCustomerAccountDTO.Status != (int)CustomerAccountStatus.Closed && beneficiaryCustomerAccountDTO != null && beneficiaryCustomerAccountDTO.Status != (int)CustomerAccountStatus.Closed)
                {
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { benefactorCustomerAccountDTO, beneficiaryCustomerAccountDTO }, serviceHeader);

                    var secondaryDescription = string.Format("{0}~{1}", beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductDescription, recurringBatchEntryDTO.RecurringBatchReference);

                    var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                    var carryForwardsArrearsRecoveredTuple = RecoverCarryFowardsArrears(postingPeriod.Id, benefactorCustomerAccountDTO, standingOrderDTO, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, serviceHeader);

                    builder.AppendLine(string.Format("{0} worth of carry forwards arrears recovered <> {1}", string.Format(_nfi, "{0:C}", carryForwardsArrearsRecoveredTuple.Item1), carryForwardsArrearsRecoveredTuple.Item2));

                    result = carryForwardsArrearsRecoveredTuple.Item1 > 0m;

                    var loanArrearsRecoveredTuple = RecoverArrearages(postingPeriod.Id, benefactorCustomerAccountDTO, standingOrderDTO, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, serviceHeader);

                    builder.AppendLine(string.Format("{0} worth of loan arrears recovered <> {1}", string.Format(_nfi, "{0:C}", loanArrearsRecoveredTuple.Item1), loanArrearsRecoveredTuple.Item2));

                    result = loanArrearsRecoveredTuple.Item1 > 0m;
                }
                else builder.AppendLine(string.Format("validation of account(s) status has failed: benefactor={0}, beneficiary={1}", benefactorCustomerAccountDTO.StatusDescription, beneficiaryCustomerAccountDTO.StatusDescription));
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private Tuple<bool, string> RecoverArrearsFromInvestmentProduct(RecurringBatchEntryDTO recurringBatchEntryDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            bool result = default(bool);

            var builder = new StringBuilder();

            PostingPeriodDTO postingPeriodDTO = null;

            if (recurringBatchEntryDTO.RecurringBatchPostingPeriodId.HasValue && recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value != Guid.Empty)
                postingPeriodDTO = _postingPeriodAppService.FindPostingPeriod(recurringBatchEntryDTO.RecurringBatchPostingPeriodId.Value, serviceHeader);
            else postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (postingPeriodDTO != null && recurringBatchEntryDTO != null && recurringBatchEntryDTO.StandingOrder != null)
            {
                serviceHeader.ApplicationUserName = recurringBatchEntryDTO.CreatedBy ?? serviceHeader.ApplicationUserName;

                var standingOrderDTO = recurringBatchEntryDTO.StandingOrder;

                var benefactorCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById((Guid)recurringBatchEntryDTO.CustomerAccountId, serviceHeader);

                var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                if (benefactorCustomerAccountDTO != null && benefactorCustomerAccountDTO.Status != (int)CustomerAccountStatus.Closed && beneficiaryCustomerAccountDTO != null && beneficiaryCustomerAccountDTO.Status != (int)CustomerAccountStatus.Closed)
                {
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { benefactorCustomerAccountDTO, beneficiaryCustomerAccountDTO }, serviceHeader);

                    var secondaryDescription = string.Format("{0}~{1}", beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductDescription, recurringBatchEntryDTO.RecurringBatchReference);

                    var reference = string.Format("{0}~{1}", recurringBatchEntryDTO.PaddedRecurringBatchBatchNumber, recurringBatchEntryDTO.Reference);

                    var loanArrearsRecoveredTuple = RecoverArrearagesByInvestmentProduct(postingPeriodDTO, benefactorCustomerAccountDTO, standingOrderDTO, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.StandingOrder, recurringBatchEntryDTO, serviceHeader);

                    builder.AppendLine(string.Format("{0} worth of loan arrears recovered <> {1}", string.Format(_nfi, "{0:C}", loanArrearsRecoveredTuple.Item1), loanArrearsRecoveredTuple.Item2));

                    result = loanArrearsRecoveredTuple.Item1 > 0m;
                }
                else builder.AppendLine(string.Format("validation of account(s) status has failed: benefactor={0}, beneficiary={1}", benefactorCustomerAccountDTO.StatusDescription, beneficiaryCustomerAccountDTO.StatusDescription));
            }

            return new Tuple<bool, string>(result, builder.ToString());
        }

        private bool CheckRecovery(StandingOrderDTO standingOrderDTO, Guid postingPeriodId, int month, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                switch ((StandingOrderTrigger)standingOrderDTO.Trigger)
                {
                    case StandingOrderTrigger.CheckOff:
                    case StandingOrderTrigger.Microloan:
                    case StandingOrderTrigger.Payout:

                        Specification<StandingOrderHistory> filter = StandingOrderHistorySpecifications.StandingOrderHistory(standingOrderDTO.Id, postingPeriodId, month);

                        ISpecification<StandingOrderHistory> spec = filter;

                        var standingOrderHistories = _standingOrderHistoryRepository.AllMatching(spec, serviceHeader);

                        if (standingOrderHistories != null)
                        {
                            result = standingOrderHistories.Any(x => x.BenefactorCustomerAccountId == standingOrderDTO.BenefactorCustomerAccountId);
                        }

                        break;

                    case StandingOrderTrigger.Schedule:
                    case StandingOrderTrigger.Sweep:

                        #region get start date after previous execution date 

                        var startDate = DateTime.Today;

                        switch ((ScheduleFrequency)standingOrderDTO.ScheduleFrequency)
                        {
                            case ScheduleFrequency.Annual:
                                startDate = startDate.AddYears(-1).AddDays(1);
                                break;

                            case ScheduleFrequency.SemiAnnual:
                                startDate = startDate.AddMonths(-6).AddDays(1);
                                break;

                            case ScheduleFrequency.TriAnnual:
                                startDate = startDate.AddMonths(-4).AddDays(1);
                                break;

                            case ScheduleFrequency.Quarterly:
                                startDate = startDate.AddMonths(-3).AddDays(1);
                                break;

                            case ScheduleFrequency.BiMonthly:
                                startDate = startDate.AddMonths(-2).AddDays(1);
                                break;

                            case ScheduleFrequency.Monthly:
                                startDate = startDate.AddMonths(-1).AddDays(1);
                                break;

                            case ScheduleFrequency.SemiMonthly:
                                startDate = startDate.AddDays(-15).AddDays(1);
                                break;

                            case ScheduleFrequency.BiWeekly:
                                startDate = startDate.AddDays(-14).AddDays(1);
                                break;

                            case ScheduleFrequency.Weekly:
                                startDate = startDate.AddDays(-7).AddDays(1);
                                break;

                            case ScheduleFrequency.Daily:
                                // :Today
                                break;

                            default:
                                break;
                        }

                        #endregion

                        var filter1 = StandingOrderHistorySpecifications.StandingOrderHistory(standingOrderDTO.Id, postingPeriodId, startDate);

                        ISpecification<StandingOrderHistory> spec1 = filter1;

                        var standingOrderHistories1 = _standingOrderHistoryRepository.AllMatching(spec1, serviceHeader);

                        if (standingOrderHistories1 != null)
                        {
                            result = standingOrderHistories1.Any(x => x.BenefactorCustomerAccountId == standingOrderDTO.BenefactorCustomerAccountId);
                        }

                        break;
                }
            }

            return result;
        }

        private PageCollectionInfo<StandingOrderDTO> FindDueStandingOrders(DateTime targetDate, int targetDateOption, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.DueStandingOrders(targetDate, targetDateOption, null, (int)StandingOrderCustomerAccountFilter.Beneficiary, (int)CustomerFilter.Reference1);

                ISpecification<StandingOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var standingOrderPagedCollection = _standingOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (standingOrderPagedCollection != null)
                {
                    var pageCollection = standingOrderPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderDTO>();

                    var itemsCount = standingOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<StandingOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        private List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.StandingOrderWithBeneficiaryCustomerAccountId(customerAccountId);

                ISpecification<StandingOrder> spec = filter;

                var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                if (standingOrders != null && standingOrders.Any())
                {
                    return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                }
                else return null;
            }
        }

        private List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerAccountId(customerAccountId);

                ISpecification<StandingOrder> spec = filter;

                var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                if (standingOrders != null && standingOrders.Any())
                {
                    return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                }
                else return null;
            }
        }

        private List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid customerAccountId, int trigger, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.StandingOrderWithBenefactorCustomerAccountIdAndTrigger(customerAccountId, trigger);

                ISpecification<StandingOrder> spec = filter;

                var standingOrders = _standingOrderRepository.AllMatching(spec, serviceHeader);

                if (standingOrders != null && standingOrders.Any())
                {
                    return standingOrders.ProjectedAsCollection<StandingOrderDTO>();
                }
                else return null;
            }
        }

        private PageCollectionInfo<StandingOrderDTO> FindSweepStandingOrders(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.StandingOrderFullText((int)StandingOrderTrigger.Sweep, null, (int)StandingOrderCustomerAccountFilter.Beneficiary, (int)CustomerFilter.Reference1);

                ISpecification<StandingOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var standingOrderPagedCollection = _standingOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (standingOrderPagedCollection != null)
                {
                    var pageCollection = standingOrderPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderDTO>();

                    var itemsCount = standingOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<StandingOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        private List<LoanGuarantorDTO> FindAttachedLoanGuarantors(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountDTO != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanGuarantorSpecifications.LoanGuarantorWithLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, (int)LoanGuarantorStatus.Attached);

                    ISpecification<LoanGuarantor> spec = filter;

                    var loanGuarantors = _loanGuarantorRepository.AllMatching(spec, serviceHeader);

                    if (loanGuarantors != null && loanGuarantors.Any())
                    {
                        return loanGuarantors.ProjectedAsCollection<LoanGuarantorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<LoanCollateralDTO> FindAttachedLoanCollaterals(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountDTO != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCollateralSpecifications.LoanCollateralWithCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, (int)CollateralStatus.Attached);

                    ISpecification<LoanCollateral> spec = filter;

                    var loanCollaterals = _loanCollateralRepository.AllMatching(spec, serviceHeader);

                    if (loanCollaterals != null && loanCollaterals.Any())
                    {
                        return loanCollaterals.ProjectedAsCollection<LoanCollateralDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private PageCollectionInfo<ElectronicStatementOrderDTO> FindDueElectronicStatementOrders(DateTime targetDate, int targetDateOption, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ElectronicStatementOrderSpecifications.DueElectronicStatementOrders(targetDate, targetDateOption, null, (int)CustomerFilter.Reference1);

                ISpecification<ElectronicStatementOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var electronicStatementOrderPagedCollection = _electronicStatementOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (electronicStatementOrderPagedCollection != null)
                {
                    var pageCollection = electronicStatementOrderPagedCollection.PageCollection.ProjectedAsCollection<ElectronicStatementOrderDTO>();

                    var itemsCount = electronicStatementOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<ElectronicStatementOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        private PageCollectionInfo<StandingOrderDTO> FindArrearsStandingOrders(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StandingOrderSpecifications.ArrearsStandingOrders((int)StandingOrderTrigger.Payout, (int)StandingOrderTrigger.Schedule);

                ISpecification<StandingOrder> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var standingOrderPagedCollection = _standingOrderRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (standingOrderPagedCollection != null)
                {
                    var pageCollection = standingOrderPagedCollection.PageCollection.ProjectedAsCollection<StandingOrderDTO>();

                    var itemsCount = standingOrderPagedCollection.ItemsCount;

                    return new PageCollectionInfo<StandingOrderDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        private void QueueRecurringBatchEntries(RecurringBatchDTO recurringBatchDTO, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var query = _recurringBatchRepository.DatabaseSqlQuery<Guid>(string.Format(
                @"SELECT Id
                            FROM  {0}RecurringBatchEntries
                            WHERE(RecurringBatchId = @RecurringBatchId)", DefaultSettings.Instance.TablePrefix), serviceHeader,
                  new SqlParameter("RecurringBatchId", recurringBatchDTO.Id));

                if (query != null)
                {
                    var data = from l in query
                               select new RecurringBatchEntryDTO
                               {
                                   Id = l,
                                   RecurringBatchPriority = recurringBatchDTO.Priority
                               };

                    _brokerService.ProcessRecurringBatchEntries(DMLCommand.Insert, serviceHeader, data.ToArray());
                }
            }
        }

        private Tuple<decimal, string> RecoverArrearages(Guid postingPeriodId, CustomerAccountDTO benefactorCustomerAccountDTO, StandingOrderDTO standingOrderDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, ServiceHeader serviceHeader)
        {
            var totalRecoveryDeductions = 0m;

            var builder = new StringBuilder();

            var targetArrearages = _customerAccountAppService.FindCustomerAccountArrearagesByCustomerAccountId(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

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
                    var journals = new List<Journal>();

                    var customerAccountArrearages = new List<CustomerAccountArrearage>();

                    var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                    var benefactorAccountAvailableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(benefactorCustomerAccountDTO, DateTime.Now, serviceHeader);

                    if (benefactorAccountAvailableBalance * -1 < 0m && beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan))
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
                                interestArrears = beneficiaryCustomerAccountDTO.InterestBalance * -1;

                            var targetLoanProduct = _loanProductAppService.FindCachedLoanProduct(beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                            var targetSavingsProduct = _savingsProductAppService.FindCachedSavingsProduct(benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductId, benefactorCustomerAccountDTO.BranchId, serviceHeader);

                            // do we need to charge for arrears
                            if (targetLoanProduct.LoanRegistrationChargeArrearsFee)
                            {
                                var loanArrearsFeeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(targetLoanProduct.Id, (int)LoanProductKnownChargeType.LoanArrearsFee, (interestArrears + principalArrears), (interestArrears + principalArrears), benefactorCustomerAccountDTO, serviceHeader);

                                if (loanArrearsFeeTariffs != null && loanArrearsFeeTariffs.Any())
                                {
                                    loanArrearsFeeTariffs.ForEach(tariff =>
                                    {
                                        var actualTariffAmount = tariff.Amount;

                                        // track deductions 
                                        totalRecoveryDeductions += actualTariffAmount;

                                        // Do we need to reset expected values?
                                        if (!((benefactorAccountAvailableBalance - totalRecoveryDeductions) >= 0m))
                                        {
                                            // reset deductions so far
                                            totalRecoveryDeductions = totalRecoveryDeductions - actualTariffAmount;

                                            // how much is available for recovery?
                                            var availableRecoveryAmount = (benefactorAccountAvailableBalance * -1 > 0m) ? 0m : benefactorAccountAvailableBalance;

                                            // reset expected tariff amount
                                            actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                            // track deductions
                                            totalRecoveryDeductions += actualTariffAmount;
                                        }

                                        var loanArrearsTariffJournal = JournalFactory.CreateJournal(null, postingPeriodId, benefactorCustomerAccountDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(loanArrearsTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, benefactorCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(loanArrearsTariffJournal);

                                        // reset available balance
                                        benefactorAccountAvailableBalance -= actualTariffAmount;
                                    });
                                }
                            }

                            // track deductions
                            totalRecoveryDeductions += principalArrears;
                            totalRecoveryDeductions += interestArrears;

                            // Do we need to reset expected values?
                            if (!((benefactorAccountAvailableBalance - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - (interestArrears + principalArrears);

                                // how much is available for recovery?
                                var availableRecoveryAmount = (benefactorAccountAvailableBalance * -1 > 0m) ? 0m : benefactorAccountAvailableBalance;

                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                interestArrears = Math.Min(interestArrears, availableRecoveryAmount);
                                principalArrears = Math.Min(principalArrears, (availableRecoveryAmount - interestArrears));

                                // track deductions
                                totalRecoveryDeductions += principalArrears;
                                totalRecoveryDeductions += interestArrears;
                            }

                            // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedLoanInterestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriodId, benefactorCustomerAccountDTO.BranchId, null, interestArrears, string.Format("Interest Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivableJournal, targetLoanProduct.InterestReceivableChartOfAccountId, targetSavingsProduct.ChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanInterestReceivableJournal);

                            // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                            var attachedLoanInterestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriodId, benefactorCustomerAccountDTO.BranchId, null, interestArrears, string.Format("Interest Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivedJournal, targetLoanProduct.InterestReceivedChartOfAccountId, targetLoanProduct.InterestChargedChartOfAccountId, beneficiaryCustomerAccountDTO, beneficiaryCustomerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanInterestReceivedJournal);

                            // Credit LoanProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedLoanPrincipalJournal = JournalFactory.CreateJournal(null, postingPeriodId, benefactorCustomerAccountDTO.BranchId, null, principalArrears, string.Format("Principal Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanPrincipalJournal, targetLoanProduct.ChartOfAccountId, targetSavingsProduct.ChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanPrincipalJournal);

                            // Update arrearage history
                            var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Interest, interestArrears * -1/*-ve cos is payment*/, reference);
                            customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(customerAccountInterestArrearage);

                            var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Principal, principalArrears * -1/*-ve cos is payment*/, reference);
                            customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(customerAccountPrincipalArrearage);
                        }
                    }

                    #region Bulk-Insert journals && journal entries

                    if (journals.Any())
                    {
                        _journalEntryPostingService.BulkSave(serviceHeader, journals, null, null, customerAccountArrearages);
                    }
                    else builder.AppendLine(string.Format("no transactions"));

                    #endregion
                }
                else builder.AppendLine(string.Format("principalArrears + interestArrears = {0}", string.Format(_nfi, "{0:C}", principalArrears + interestArrears)));
            }
            else builder.AppendLine(string.Format("no arrearages"));

            return new Tuple<decimal, string>(totalRecoveryDeductions, builder.ToString());
        }

        private Tuple<decimal, string> RecoverCarryFowardsArrears(Guid postingPeriodId, CustomerAccountDTO benefactorCustomerAccountDTO, StandingOrderDTO standingOrderDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, ServiceHeader serviceHeader)
        {
            var totalRecoveryDeductions = 0m;

            var builder = new StringBuilder();

            var availableBalance = benefactorCustomerAccountDTO.AvailableBalance;

            if ((availableBalance > 0m /*Will current account balance be positive?*/))
            {
                var carryForwards = _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                if (carryForwards != null && carryForwards.Any())
                {
                    var targetCarryForwards = from c in carryForwards
                                              where c.BenefactorCustomerAccountId == benefactorCustomerAccountDTO.Id
                                              select c;

                    if (targetCarryForwards != null && targetCarryForwards.Any())
                    {
                        var journals = new List<Journal>();

                        var customerAccountCarryForwards = new List<CustomerAccountCarryForward>();

                        _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { benefactorCustomerAccountDTO }, serviceHeader);

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

                                if ((principalArrears + interestArrears) > 0m)
                                {
                                    var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(item.BeneficiaryCustomerAccountId, serviceHeader);

                                    var targetSavingsProduct = _savingsProductAppService.FindCachedSavingsProduct(benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductId, benefactorCustomerAccountDTO.BranchId, serviceHeader);

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

                                        var primaryDescription = string.Format("Carry Forwards Paid~{0}", targetLoanProduct.Description);

                                        // Credit CarryForward.BeneficiaryChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                                        var carryFowardBeneficiaryJournal = JournalFactory.CreateJournal(null, postingPeriodId, benefactorCustomerAccountDTO.BranchId, null, principalArrears, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(carryFowardBeneficiaryJournal, item.BeneficiaryChartOfAccountId, targetSavingsProduct.ChartOfAccountId, benefactorCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(carryFowardBeneficiaryJournal);

                                        // reset available balance
                                        availableBalance -= principalArrears;

                                        // Do we need to update carry forward history?
                                        var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(benefactorCustomerAccountDTO.Id, item.BeneficiaryCustomerAccountId, item.BeneficiaryChartOfAccountId, principalArrears * -1/*-ve cos is payment*/, primaryDescription);
                                        customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                        customerAccountCarryForwards.Add(customerAccountCarryForward);
                                    }
                                }
                            }
                        }

                        #region Bulk-Insert journals && journal entries

                        if (journals.Any())
                        {
                            _journalEntryPostingService.BulkSave(serviceHeader, journals, customerAccountCarryForwards);
                        }
                        else builder.AppendLine(string.Format("no transactions"));

                        #endregion
                    }
                    else builder.AppendLine(string.Format("no target carry forwards"));
                }
                else builder.AppendLine(string.Format("no carry forwards"));
            }
            else builder.AppendLine(string.Format("available balance = {0}", string.Format(_nfi, "{0:C}", availableBalance)));

            return new Tuple<decimal, string>(totalRecoveryDeductions, builder.ToString());
        }

        private Tuple<decimal, string> RecoverArrearagesByInvestmentProduct(PostingPeriodDTO postingPeriodDTO, CustomerAccountDTO benefactorCustomerAccountDTO, StandingOrderDTO standingOrderDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, RecurringBatchEntryDTO recurringBatchEntryDTO, ServiceHeader serviceHeader)
        {
            var totalRecoveryDeductions = 0m;

            var builder = new StringBuilder();

            var targetArrearages = _customerAccountAppService.FindCustomerAccountArrearagesByCustomerAccountId(standingOrderDTO.BeneficiaryCustomerAccountId, UberUtil.GetLastDayOfMonth(recurringBatchEntryDTO.RecurringBatchMonth, postingPeriodDTO.DurationEndDate.Year, recurringBatchEntryDTO.RecurringBatchEnforceMonthValueDate), serviceHeader);

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

                if (totalPrincipalArrearages * -1 < 0m)
                {
                    interestArrears = totalInterestArrearages;
                }

                if ((principalArrears + interestArrears) > 0m)
                {
                    var journals = new List<Journal>();

                    var customerAccountArrearages = new List<CustomerAccountArrearage>();

                    var beneficiaryCustomerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(standingOrderDTO.BeneficiaryCustomerAccountId, serviceHeader);

                    var benefactorAccountBookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(benefactorCustomerAccountDTO, 1, DateTime.Now, serviceHeader);

                    if (benefactorAccountBookBalance * -1 < 0m && beneficiaryCustomerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan))
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
                                interestArrears = beneficiaryCustomerAccountDTO.InterestBalance * -1;

                            var targetLoanProduct = _loanProductAppService.FindCachedLoanProduct(beneficiaryCustomerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                            var targetInvestmentProduct = _investmentProductAppService.FindCachedInvestmentProduct(benefactorCustomerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                            // do we need to charge for arrears
                            if (targetLoanProduct.LoanRegistrationChargeArrearsFee)
                            {
                                var loanArrearsFeeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(targetLoanProduct.Id, (int)LoanProductKnownChargeType.LoanArrearsFee, (interestArrears + principalArrears), (interestArrears + principalArrears), benefactorCustomerAccountDTO, serviceHeader);

                                if (loanArrearsFeeTariffs != null && loanArrearsFeeTariffs.Any())
                                {
                                    loanArrearsFeeTariffs.ForEach(tariff =>
                                    {
                                        var actualTariffAmount = tariff.Amount;

                                        // track deductions 
                                        totalRecoveryDeductions += actualTariffAmount;

                                        // Do we need to reset expected values?
                                        if (!((benefactorAccountBookBalance - totalRecoveryDeductions) >= 0m))
                                        {
                                            // reset deductions so far
                                            totalRecoveryDeductions = totalRecoveryDeductions - actualTariffAmount;

                                            // how much is available for recovery?
                                            var availableRecoveryAmount = (benefactorAccountBookBalance * -1 > 0m) ? 0m : benefactorAccountBookBalance;

                                            // reset expected tariff amount
                                            actualTariffAmount = Math.Min(actualTariffAmount, availableRecoveryAmount);

                                            // track deductions
                                            totalRecoveryDeductions += actualTariffAmount;
                                        }

                                        var loanArrearsTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(loanArrearsTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, benefactorCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                                        journals.Add(loanArrearsTariffJournal);

                                        // reset available balance
                                        benefactorAccountBookBalance -= actualTariffAmount;
                                    });
                                }
                            }

                            // track deductions
                            totalRecoveryDeductions += principalArrears;
                            totalRecoveryDeductions += interestArrears;

                            // Do we need to reset expected values?
                            if (!((benefactorAccountBookBalance - totalRecoveryDeductions) >= 0m))
                            {
                                // reset deductions so far
                                totalRecoveryDeductions = totalRecoveryDeductions - (interestArrears + principalArrears);

                                // how much is available for recovery?
                                var availableRecoveryAmount = (benefactorAccountBookBalance * -1 > 0m) ? 0m : benefactorAccountBookBalance;

                                // reset expected interest & expected principal >> NB: interest has priority over principal!
                                interestArrears = Math.Min(interestArrears, availableRecoveryAmount);
                                principalArrears = Math.Min(principalArrears, (availableRecoveryAmount - interestArrears));

                                // track deductions
                                totalRecoveryDeductions += principalArrears;
                                totalRecoveryDeductions += interestArrears;
                            }

                            // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedLoanInterestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, interestArrears, string.Format("Interest Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivableJournal, targetLoanProduct.InterestReceivableChartOfAccountId, targetInvestmentProduct.ChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanInterestReceivableJournal);

                            // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                            var attachedLoanInterestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, interestArrears, string.Format("Interest Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivedJournal, targetLoanProduct.InterestReceivedChartOfAccountId, targetLoanProduct.InterestChargedChartOfAccountId, beneficiaryCustomerAccountDTO, beneficiaryCustomerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanInterestReceivedJournal);

                            // Credit LoanProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                            var attachedLoanPrincipalJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, benefactorCustomerAccountDTO.BranchId, null, principalArrears, string.Format("Principal Arrears Paid~{0}", targetLoanProduct.Description), secondaryDescription, reference, moduleNavigationItemCode, transactionCode, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanPrincipalJournal, targetLoanProduct.ChartOfAccountId, targetInvestmentProduct.ChartOfAccountId, beneficiaryCustomerAccountDTO, benefactorCustomerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanPrincipalJournal);

                            // Update arrearage history
                            var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Interest, interestArrears * -1/*-ve cos is payment*/, reference);
                            customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(customerAccountInterestArrearage);

                            var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(beneficiaryCustomerAccountDTO.Id, (int)ArrearageCategory.Principal, principalArrears * -1/*-ve cos is payment*/, reference);
                            customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                            customerAccountArrearages.Add(customerAccountPrincipalArrearage);
                        }
                    }

                    #region Bulk-Insert journals && journal entries

                    if (journals.Any())
                    {
                        _journalEntryPostingService.BulkSave(serviceHeader, journals, null, null, customerAccountArrearages);
                    }
                    else builder.AppendLine(string.Format("no transactions"));

                    #endregion
                }
                else builder.AppendLine(string.Format("principalArrears + interestArrears = {0}", string.Format(_nfi, "{0:C}", principalArrears + interestArrears)));
            }
            else builder.AppendLine(string.Format("no arrearages"));

            return new Tuple<decimal, string>(totalRecoveryDeductions, builder.ToString());
        }
    }
}
