using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using KBCsv;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class DebitBatchAppService : IDebitBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<DebitBatch> _debitBatchRepository;
        private readonly IRepository<DebitBatchEntry> _debitBatchEntryRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IDebitTypeAppService _debitTypeAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IBrokerService _brokerService;
        private readonly IAppCache _appCache;

        public DebitBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<DebitBatch> debitBatchRepository,
           IRepository<DebitBatchEntry> debitBatchEntryRepository,
           IPostingPeriodAppService postingPeriodAppService,
           IDebitTypeAppService debitTypeAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ICommissionAppService commissionAppService,
           ISqlCommandAppService sqlCommandAppService,
           IBrokerService brokerService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (debitBatchRepository == null)
                throw new ArgumentNullException(nameof(debitBatchRepository));

            if (debitBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(debitBatchEntryRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (debitTypeAppService == null)
                throw new ArgumentNullException(nameof(debitTypeAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _debitBatchRepository = debitBatchRepository;
            _debitBatchEntryRepository = debitBatchEntryRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _debitTypeAppService = debitTypeAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _commissionAppService = commissionAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _brokerService = brokerService;
            _appCache = appCache;
        }

        public DebitBatchDTO AddNewDebitBatch(DebitBatchDTO debitBatchDTO, ServiceHeader serviceHeader)
        {
            if (debitBatchDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var debitBatch = DebitBatchFactory.CreateDebitBatch(debitBatchDTO.DebitTypeId, debitBatchDTO.BranchId, debitBatchDTO.Reference, debitBatchDTO.Priority);

                    debitBatch.BatchNumber = _debitBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}DebitBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    debitBatch.Status = (int)BatchStatus.Pending;
                    debitBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    _debitBatchRepository.Add(debitBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return debitBatch.ProjectedAs<DebitBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateDebitBatch(DebitBatchDTO debitBatchDTO, ServiceHeader serviceHeader)
        {
            if (debitBatchDTO == null || debitBatchDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _debitBatchRepository.Get(debitBatchDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DebitBatchFactory.CreateDebitBatch(debitBatchDTO.DebitTypeId, debitBatchDTO.BranchId, debitBatchDTO.Reference, debitBatchDTO.Priority);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.BatchNumber = persisted.BatchNumber;
                    current.Status = persisted.Status;
                    current.CreatedBy = persisted.CreatedBy;


                    _debitBatchRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public bool AuditDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, ServiceHeader serviceHeader)
        {
            if (debitBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _debitBatchRepository.Get(debitBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Audited;
                        persisted.AuditRemarks = debitBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuditRemarks = debitBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeDebitBatch(DebitBatchDTO debitBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (debitBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _debitBatchRepository.Get(debitBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
                    return result;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Posted;
                        persisted.AuthorizationRemarks = debitBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuthorizationRemarks = debitBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                if (result && batchAuthOption == (int)BatchAuthOption.Post)
                {
                    var query = _debitBatchRepository.DatabaseSqlQuery<Guid>(string.Format(
                          @"SELECT Id
                            FROM  {0}DebitBatchEntries
                            WHERE(DebitBatchId = @DebitBatchId)", DefaultSettings.Instance.TablePrefix), serviceHeader,
                            new SqlParameter("DebitBatchId", debitBatchDTO.Id));

                    if (query != null)
                    {
                        var data = from l in query
                                   select new DebitBatchEntryDTO
                                   {
                                       Id = l,
                                       DebitBatchPriority = debitBatchDTO.Priority
                                   };

                        _brokerService.ProcessDebitBatchEntries(DMLCommand.None, serviceHeader, data.ToArray());
                    }
                }
            }

            return result;
        }

        public bool PostDebitBatchEntry(Guid debitBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (MarkDebitBatchEntryPosted(debitBatchEntryId, serviceHeader))
            {
                var debitBatchEntryDTO = FindDebitBatchEntry(debitBatchEntryId, serviceHeader);
                if (debitBatchEntryDTO == null || debitBatchEntryDTO.Status != (int)BatchEntryStatus.Posted)
                    return result;

                var debitBatchDTO = FindCachedDebitBatch(debitBatchEntryDTO.DebitBatchId, serviceHeader);
                if (debitBatchDTO == null)
                    return result;

                var postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);
                if (postingPeriodDTO == null)
                    return result;

                serviceHeader.ApplicationUserName = debitBatchDTO.AuthorizedBy ?? serviceHeader.ApplicationUserName;

                var secondaryDescription = string.Format("{0}~{1}", debitBatchDTO.Reference, debitBatchDTO.DebitTypeDescription);

                var reference = string.Format("{0}~{1}", debitBatchDTO.PaddedBatchNumber, debitBatchEntryDTO.Reference);

                var journals = new List<Journal>();

                CustomerAccountDTO batchEntryCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(debitBatchEntryDTO.CustomerAccountId, serviceHeader);

                if (batchEntryCustomerAccount != null)
                {
                    var availableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(batchEntryCustomerAccount, DateTime.Now, serviceHeader);

                    var actualTariffAmount = 0m;

                    var totalDeductions = 0m;

                    switch ((ProductCode)batchEntryCustomerAccount.CustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:

                            var sp_DebitTypeTariffs = _commissionAppService.ComputeTariffsByDebitType(debitBatchDTO.DebitTypeId, debitBatchEntryDTO.BasisValue, debitBatchEntryDTO.Multiplier, batchEntryCustomerAccount, serviceHeader);

                            sp_DebitTypeTariffs.ForEach(tariff =>
                            {
                                if (debitBatchDTO.BranchCompanyAllowDebitBatchToOverdrawAccount)
                                {
                                    var sp_debitTypeTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, debitBatchDTO.BranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.DebitBatch, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(sp_debitTypeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, batchEntryCustomerAccount, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(sp_debitTypeTariffJournal);
                                }
                                else
                                {
                                    actualTariffAmount = tariff.Amount;

                                    totalDeductions += actualTariffAmount;

                                    if (!((availableBalance - totalDeductions) >= 0m))
                                    {
                                        //reset deductions
                                        totalDeductions = totalDeductions - actualTariffAmount;

                                        // how much is available for deduction?
                                        var availableDeductableAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                        // reset expected tariff amount
                                        actualTariffAmount = Math.Min(actualTariffAmount, availableDeductableAmount);

                                        // track tariffs
                                        totalDeductions += actualTariffAmount;
                                    }

                                    var sp_debitTypeTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, debitBatchDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.DebitBatch, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(sp_debitTypeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, batchEntryCustomerAccount, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(sp_debitTypeTariffJournal);

                                    // reset available balance
                                    availableBalance -= actualTariffAmount;
                                }
                            });

                            break;
                        case ProductCode.Loan:

                            var lp_DebitTypeTariffs = _commissionAppService.ComputeTariffsByDebitType(debitBatchDTO.DebitTypeId, debitBatchEntryDTO.BasisValue, debitBatchEntryDTO.Multiplier, batchEntryCustomerAccount, serviceHeader);

                            lp_DebitTypeTariffs.ForEach(tariff =>
                            {
                                if (debitBatchDTO.BranchCompanyAllowDebitBatchToOverdrawAccount)
                                {
                                    var lp_debitTypeTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, debitBatchDTO.BranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.DebitBatch, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(lp_debitTypeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, batchEntryCustomerAccount, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(lp_debitTypeTariffJournal);
                                }
                                else
                                {
                                    actualTariffAmount = tariff.Amount;

                                    totalDeductions += actualTariffAmount;

                                    if (!((availableBalance - totalDeductions) >= 0m))
                                    {
                                        //reset deductions
                                        totalDeductions = totalDeductions - actualTariffAmount;

                                        // how much is available for deduction?
                                        var availableDeductableAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                        // reset expected tariff amount
                                        actualTariffAmount = Math.Min(actualTariffAmount, availableDeductableAmount);

                                        // track tariffs
                                        totalDeductions += actualTariffAmount;
                                    }

                                    var lp_debitTypeTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, debitBatchDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.DebitBatch, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(lp_debitTypeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, batchEntryCustomerAccount, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(lp_debitTypeTariffJournal);

                                    // reset available balance
                                    availableBalance -= actualTariffAmount;
                                }
                            });

                            break;
                        case ProductCode.Investment:

                            var ip_DebitTypeTariffs = _commissionAppService.ComputeTariffsByDebitType(debitBatchDTO.DebitTypeId, debitBatchEntryDTO.BasisValue, debitBatchEntryDTO.Multiplier, batchEntryCustomerAccount, serviceHeader);

                            ip_DebitTypeTariffs.ForEach(tariff =>
                            {
                                if (debitBatchDTO.BranchCompanyAllowDebitBatchToOverdrawAccount)
                                {
                                    var ip_debitTypeTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, debitBatchDTO.BranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.DebitBatch, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(ip_debitTypeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, batchEntryCustomerAccount, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(ip_debitTypeTariffJournal);
                                }
                                else
                                {
                                    actualTariffAmount = tariff.Amount;

                                    totalDeductions += actualTariffAmount;

                                    if (!((availableBalance - totalDeductions) >= 0m))
                                    {
                                        //reset deductions
                                        totalDeductions = totalDeductions - actualTariffAmount;

                                        // how much is available for deduction?
                                        var availableDeductableAmount = (availableBalance * -1 > 0m) ? 0m : availableBalance;

                                        // reset expected tariff amount
                                        actualTariffAmount = Math.Min(actualTariffAmount, availableDeductableAmount);

                                        // track tariffs
                                        totalDeductions += actualTariffAmount;
                                    }

                                    var ip_debitTypeTariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, debitBatchDTO.BranchId, null, actualTariffAmount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.DebitBatch, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(ip_debitTypeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, batchEntryCustomerAccount, batchEntryCustomerAccount, serviceHeader);
                                    journals.Add(ip_debitTypeTariffJournal);

                                    // reset available balance
                                    availableBalance -= actualTariffAmount;
                                }
                            });

                            break;
                        default:
                            break;
                    }
                }

                #region Bulk-Insert journals && journal entries, text alerts & message histories

                if (journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }

                #endregion
            }

            return result;
        }

        public DebitBatchEntryDTO AddNewDebitBatchEntry(DebitBatchEntryDTO debitBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (debitBatchEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var debitBatchEntry = DebitBatchEntryFactory.CreateDebitBatchEntry(debitBatchEntryDTO.DebitBatchId, debitBatchEntryDTO.CustomerAccountId, debitBatchEntryDTO.Multiplier, debitBatchEntryDTO.BasisValue, debitBatchEntryDTO.Reference);

                    debitBatchEntry.Status = (int)BatchEntryStatus.Pending;
                    debitBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _debitBatchEntryRepository.Add(debitBatchEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return debitBatchEntry.ProjectedAs<DebitBatchEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveDebitBatchEntries(List<DebitBatchEntryDTO> debitBatchEntryDTOs, ServiceHeader serviceHeader)
        {
            if (debitBatchEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in debitBatchEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _debitBatchEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _debitBatchEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<DebitBatchDTO> FindDebitBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var debitBatches = _debitBatchRepository.GetAll(serviceHeader);

                if (debitBatches != null && debitBatches.Any())
                {
                    return debitBatches.ProjectedAsCollection<DebitBatchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DebitBatchDTO> FindDebitBatches(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DebitBatchSpecifications.DebitBatchFullText(text);

                ISpecification<DebitBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var debitBatchCollection = _debitBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (debitBatchCollection != null)
                {
                    var pageCollection = debitBatchCollection.PageCollection.ProjectedAsCollection<DebitBatchDTO>();

                    var itemsCount = debitBatchCollection.ItemsCount;

                    return new PageCollectionInfo<DebitBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DebitBatchDTO> FindDebitBatches(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DebitBatchSpecifications.DefaultSpec();

                ISpecification<DebitBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var debitBatchPagedCollection = _debitBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (debitBatchPagedCollection != null)
                {
                    var pageCollection = debitBatchPagedCollection.PageCollection.ProjectedAsCollection<DebitBatchDTO>();

                    var itemsCount = debitBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DebitBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }





        public PageCollectionInfo<DebitBatchDTO> FindDebitBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DebitBatchSpecifications.DebitBatchesWithStatus(status, startDate, endDate, text);

                ISpecification<DebitBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var debitBatchPagedCollection = _debitBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (debitBatchPagedCollection != null)
                {
                    var pageCollection = debitBatchPagedCollection.PageCollection.ProjectedAsCollection<DebitBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _debitBatchEntryRepository.AllMatchingCount(DebitBatchEntrySpecifications.DebitBatchEntryWithDebitBatchId(item.Id, null), serviceHeader);

                            var postedItems = _debitBatchEntryRepository.AllMatchingCount(DebitBatchEntrySpecifications.PostedDebitBatchEntryWithDebitBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = debitBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DebitBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DebitBatchDTO FindDebitBatch(Guid debitBatchId, ServiceHeader serviceHeader)
        {
            if (debitBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var debitBatch = _debitBatchRepository.Get(debitBatchId, serviceHeader);

                    if (debitBatch != null)
                    {
                        return debitBatch.ProjectedAs<DebitBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchId(Guid debitBatchId, ServiceHeader serviceHeader)
        {
            if (debitBatchId != null && debitBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DebitBatchEntrySpecifications.DebitBatchEntryWithDebitBatchId(debitBatchId, null);

                    ISpecification<DebitBatchEntry> spec = filter;

                    var debitBatchEntries = _debitBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (debitBatchEntries != null && debitBatchEntries.Any())
                    {
                        return debitBatchEntries.ProjectedAsCollection<DebitBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<DebitBatchEntryDTO> FindDebitBatchEntriesByDebitBatchId(Guid debitBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (debitBatchId != null && debitBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DebitBatchEntrySpecifications.DebitBatchEntryWithDebitBatchId(debitBatchId, text);

                    ISpecification<DebitBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var debitBatchPagedCollection = _debitBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (debitBatchPagedCollection != null)
                    {
                        var pageCollection = debitBatchPagedCollection.PageCollection.ProjectedAsCollection<DebitBatchEntryDTO>();

                        var itemsCount = debitBatchPagedCollection.ItemsCount;

                        return new PageCollectionInfo<DebitBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DebitBatchEntryDTO> FindDebitBatchEntriesByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DebitBatchEntrySpecifications.DebitBatchEntryWithCustomerId(customerId);

                    ISpecification<DebitBatchEntry> spec = filter;

                    var debitBatchEntries = _debitBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (debitBatchEntries != null && debitBatchEntries.Any())
                    {
                        return debitBatchEntries.ProjectedAsCollection<DebitBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<DebitBatchEntryDTO> FindQueableDebitBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DebitBatchEntrySpecifications.QueableDebitBatchEntries();

                ISpecification<DebitBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var debitBatchPagedCollection = _debitBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (debitBatchPagedCollection != null)
                {
                    var pageCollection = debitBatchPagedCollection.PageCollection.ProjectedAsCollection<DebitBatchEntryDTO>();

                    var itemsCount = debitBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DebitBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<BatchImportEntryWrapper> ParseDebitBatchImport(Guid debitBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var persisted = _debitBatchRepository.Get(debitBatchId, serviceHeader);

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

                                if (dataRecord.Count == 4)
                                {
                                    var payoutEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], //Payroll Number
                                        Column2 = dataRecord[1], //Customer Name
                                        Column3 = dataRecord[2], //Multiplier
                                        Column4 = dataRecord[3], //BasisValue
                                    };

                                    importEntries.Add(payoutEntry);
                                }
                            }
                        }

                        if (importEntries.Any())
                        {
                            BatchImportParseInfo parseInfo = parseInfo = DoParse(persisted.DebitTypeId, importEntries, serviceHeader);

                            if (parseInfo != null)
                            {
                                UpdateDebitBatchEntries(debitBatchId, parseInfo.MatchedCollection2, serviceHeader);

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

        public DebitBatchEntryDTO FindDebitBatchEntry(Guid debitBatchEntryId, ServiceHeader serviceHeader)
        {
            if (debitBatchEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var debitBatchEntry = _debitBatchEntryRepository.Get(debitBatchEntryId, serviceHeader);

                    if (debitBatchEntry != null)
                    {
                        return debitBatchEntry.ProjectedAs<DebitBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public DebitBatchDTO FindCachedDebitBatch(Guid debitBatchId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<DebitBatchDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, debitBatchId.ToString("D")), () =>
            {
                return FindDebitBatch(debitBatchId, serviceHeader);
            });
        }

        private bool UpdateDebitBatchEntries(Guid debitBatchId, List<DebitBatchEntryDTO> debitBatchEntries, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (debitBatchId != null && debitBatchEntries != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _debitBatchRepository.Get(debitBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        int recordsAffected = _sqlCommandAppService.DeleteDebitBatchEntries(persisted.Id, serviceHeader);

                        if (debitBatchEntries.Any())
                        {
                            List<DebitBatchEntry> batchEntries = new List<DebitBatchEntry>();

                            var totalItems = debitBatchEntries.Count;

                            foreach (var item in debitBatchEntries)
                            {
                                var debitBatchEntry = DebitBatchEntryFactory.CreateDebitBatchEntry(persisted.Id, item.CustomerAccountId, item.Multiplier, item.BasisValue, item.Reference);

                                debitBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                debitBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                batchEntries.Add(debitBatchEntry);
                            }

                            #region Bulk-Insert entries

                            if (batchEntries.Any())
                            {
                                var bcpBatchEntries = new List<DebitBatchEntryBulkCopyDTO>();

                                batchEntries.ForEach(c =>
                                {
                                    DebitBatchEntryBulkCopyDTO bcpc =
                                        new DebitBatchEntryBulkCopyDTO
                                        {
                                            Id = c.Id,
                                            DebitBatchId = c.DebitBatchId,
                                            CustomerAccountId = c.CustomerAccountId,
                                            Multiplier = c.Multiplier,
                                            BasisValue = c.BasisValue,
                                            Reference = c.Reference,
                                            Status = c.Status,
                                            CreatedBy = c.CreatedBy,
                                            CreatedDate = c.CreatedDate,
                                        };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _debitBatchEntryRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }

                            #endregion
                        }
                    }
                }
            }

            return result;
        }

        private BatchImportParseInfo DoParse(Guid debitTypeId, List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection2 = new List<DebitBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var debitTypeDTO = _debitTypeAppService.FindDebitType(debitTypeId, serviceHeader);

            _debitTypeAppService.FetchDebitTypesProductDescription(new List<DebitTypeDTO> { debitTypeDTO }, serviceHeader);

            var count = 0;

            importEntries.ForEach(item =>
            {
                var multiplier = default(double);

                var basisValue = default(decimal);

                if (double.TryParse(item.Column3, out multiplier) && decimal.TryParse(item.Column4, out basisValue))
                {
                    var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndPayrollNumber(debitTypeDTO.CustomerAccountTypeTargetProductId, item.Column1, serviceHeader);

                    if (customerAccounts.Any())
                    {
                        if (customerAccounts.Count == 1)
                        {
                            var targetCustomerAccount = customerAccounts[0];

                            DebitBatchEntryDTO debitBatchEntry = new DebitBatchEntryDTO();

                            debitBatchEntry.CustomerAccountId = targetCustomerAccount.Id;
                            debitBatchEntry.CustomerAccountBranchId = targetCustomerAccount.BranchId;
                            debitBatchEntry.CustomerAccountCustomerAccountTypeProductCode = targetCustomerAccount.CustomerAccountTypeProductCode;
                            debitBatchEntry.CustomerAccountCustomerAccountTypeTargetProductId = targetCustomerAccount.CustomerAccountTypeTargetProductId;
                            debitBatchEntry.CustomerAccountCustomerAccountTypeTargetProductCode = targetCustomerAccount.CustomerAccountTypeTargetProductCode;
                            debitBatchEntry.CustomerAccountCustomerId = targetCustomerAccount.CustomerId;
                            debitBatchEntry.CustomerAccountCustomerIndividualPayrollNumbers = item.Column1;
                            debitBatchEntry.CustomerAccountCustomerIndividualFirstName = item.Column2;
                            debitBatchEntry.ProductDescription = debitTypeDTO.CustomerAccountTypeTargetProductDescription;
                            debitBatchEntry.Multiplier = multiplier;
                            debitBatchEntry.BasisValue = basisValue;
                            debitBatchEntry.Reference = item.Column1;

                            result.MatchedCollection2.Add(debitBatchEntry);
                        }
                        else
                        {
                            item.Remarks = string.Format("Record #{0} ~ found {1} customer account matches.", count, customerAccounts.Count());

                            result.MismatchedCollection.Add(item);
                        }
                    }
                    else
                    {
                        item.Remarks = string.Format("Record #{0} ~ no match for {1} product customer account.", count, debitTypeDTO.CustomerAccountTypeTargetProductDescription);

                        result.MismatchedCollection.Add(item);
                    }
                }
                else
                {
                    item.Remarks = string.Format("Record #{0} ~ unable to parse multiplier.", count);

                    result.MismatchedCollection.Add(item);
                }

                // tally
                count += 1;
            });

            return result;
        }

        private bool MarkDebitBatchEntryPosted(Guid debitBatchEntryId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (debitBatchEntryId == null || debitBatchEntryId == Guid.Empty)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _debitBatchEntryRepository.Get(debitBatchEntryId, serviceHeader);

                if (persisted != null)
                {
                    switch ((BatchEntryStatus)persisted.Status)
                    {
                        case BatchEntryStatus.Pending:
                            persisted.Status = (int)BatchEntryStatus.Posted;
                            result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                            break;
                        case BatchEntryStatus.Posted:
                            result = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }
    }
}
