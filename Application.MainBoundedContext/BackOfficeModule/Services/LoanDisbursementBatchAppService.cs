using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.MicroCreditModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public class LoanDisbursementBatchAppService : ILoanDisbursementBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LoanDisbursementBatch> _loanDisbursementBatchRepository;
        private readonly IRepository<LoanDisbursementBatchEntry> _loanDisbursementBatchEntryRepository;
        private readonly IRepository<LoanCase> _loanCaseRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IFinancialsService _financialsService;
        private readonly IStandingOrderAppService _standingOrderAppService;
        private readonly IMicroCreditGroupAppService _microCreditGroupAppService;
        private readonly ILoanCaseAppService _loanCaseAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IDesignationAppService _designationAppService;
        private readonly IBrokerService _brokerService;
        private readonly IAppCache _appCache;

        public LoanDisbursementBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LoanDisbursementBatch> loanDisbursementBatchRepository,
           IRepository<LoanDisbursementBatchEntry> loanDisbursementBatchEntryRepository,
           IRepository<LoanCase> loanCaseRepository,
           IPostingPeriodAppService postingPeriodAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ISqlCommandAppService sqlCommandAppService,
           ICommissionAppService commissionAppService,
           ICustomerAccountAppService customerAccountAppService,
           IFinancialsService financialsService,
           IStandingOrderAppService standingOrderAppService,
           IMicroCreditGroupAppService microCreditGroupAppService,
           ILoanCaseAppService loanCaseAppService,
           ILoanProductAppService loanProductAppService,
           ISavingsProductAppService savingsProductAppService,
           IDesignationAppService designationAppService,
           IBrokerService brokerService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (loanDisbursementBatchRepository == null)
                throw new ArgumentNullException(nameof(loanDisbursementBatchRepository));

            if (loanDisbursementBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(loanDisbursementBatchEntryRepository));

            if (loanCaseRepository == null)
                throw new ArgumentNullException(nameof(loanCaseRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (financialsService == null)
                throw new ArgumentNullException(nameof(financialsService));

            if (standingOrderAppService == null)
                throw new ArgumentNullException(nameof(standingOrderAppService));

            if (microCreditGroupAppService == null)
                throw new ArgumentNullException(nameof(microCreditGroupAppService));

            if (loanCaseAppService == null)
                throw new ArgumentNullException(nameof(loanCaseAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (designationAppService == null)
                throw new ArgumentNullException(nameof(designationAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _loanDisbursementBatchRepository = loanDisbursementBatchRepository;
            _loanDisbursementBatchEntryRepository = loanDisbursementBatchEntryRepository;
            _loanCaseRepository = loanCaseRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _sqlCommandAppService = sqlCommandAppService;
            _commissionAppService = commissionAppService;
            _customerAccountAppService = customerAccountAppService;
            _financialsService = financialsService;
            _standingOrderAppService = standingOrderAppService;
            _microCreditGroupAppService = microCreditGroupAppService;
            _loanCaseAppService = loanCaseAppService;
            _loanProductAppService = loanProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _designationAppService = designationAppService;
            _brokerService = brokerService;
            _appCache = appCache;
        }

        public LoanDisbursementBatchDTO AddNewLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var loanDisbursementBatch = LoanDisbursementBatchFactory.CreateLoanDisbursementBatch(loanDisbursementBatchDTO.BranchId, loanDisbursementBatchDTO.DataAttachmentPeriodId, loanDisbursementBatchDTO.Type, loanDisbursementBatchDTO.LoanProductCategory, loanDisbursementBatchDTO.Reference, loanDisbursementBatchDTO.Priority);

                    loanDisbursementBatch.BatchNumber = _loanDisbursementBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}LoanDisbursementBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    loanDisbursementBatch.Status = (int)BatchStatus.Pending;
                    loanDisbursementBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    _loanDisbursementBatchRepository.Add(loanDisbursementBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return loanDisbursementBatch.ProjectedAs<LoanDisbursementBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchDTO == null || loanDisbursementBatchDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanDisbursementBatchRepository.Get(loanDisbursementBatchDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.Reference = loanDisbursementBatchDTO.Reference;
                    persisted.Priority = (byte)loanDisbursementBatchDTO.Priority;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public bool AuditLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanDisbursementBatchRepository.Get(loanDisbursementBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Audited;
                        persisted.AuditRemarks = loanDisbursementBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuditRemarks = loanDisbursementBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeLoanDisbursementBatch(LoanDisbursementBatchDTO loanDisbursementBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (loanDisbursementBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanDisbursementBatchRepository.Get(loanDisbursementBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
                    return result;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Posted;
                        persisted.AuthorizationRemarks = loanDisbursementBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuthorizationRemarks = loanDisbursementBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                if (result && batchAuthOption == (int)BatchAuthOption.Post)
                {
                    var query = _loanDisbursementBatchRepository.DatabaseSqlQuery<Guid>(string.Format(
                  @"SELECT Id
                    FROM  {0}LoanDisbursementBatchEntries
                    WHERE(LoanDisbursementBatchId = @LoanDisbursementBatchId)", DefaultSettings.Instance.TablePrefix),
                  serviceHeader,
                  new SqlParameter("LoanDisbursementBatchId", loanDisbursementBatchDTO.Id))
                  .ToList(); // Executes the query


                    if (query != null)
                    {
                        var data = from l in query
                                   select new LoanDisbursementBatchEntryDTO
                                   {
                                       Id = l,
                                       LoanDisbursementBatchPriority = loanDisbursementBatchDTO.Priority
                                   };

                        _brokerService.ProcessLoanDisbursementBatchEntries(DMLCommand.None, serviceHeader, data.ToArray());
                    }
                }
            }

            return result;
        }

        public LoanDisbursementBatchEntryDTO AddNewLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanDisbursementBatchEntryDTO.LoanCaseId, serviceHeader);

                    if (persisted != null)
                    {
                        if (_loanDisbursementBatchEntryRepository.AllMatchingCount(LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanCaseId(persisted.Id), serviceHeader) != 0)
                            throw new InvalidOperationException("Sorry, but the selected loan number has already been batched!");
                        else
                        {
                            persisted.IsBatched = true;
                            persisted.BatchNumber = loanDisbursementBatchEntryDTO.LoanDisbursementBatchBatchNumber;
                            persisted.BatchedBy = serviceHeader.ApplicationUserName;

                            var loanDisbursementBatchEntry = LoanDisbursementBatchEntryFactory.CreateLoanDisbursementBatchEntry(loanDisbursementBatchEntryDTO.LoanDisbursementBatchId, loanDisbursementBatchEntryDTO.LoanCaseId, loanDisbursementBatchEntryDTO.Reference);

                            loanDisbursementBatchEntry.Status = (int)BatchEntryStatus.Pending;
                            loanDisbursementBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                            _loanDisbursementBatchEntryRepository.Add(loanDisbursementBatchEntry, serviceHeader);

                            dbContextScope.SaveChanges(serviceHeader);

                            return loanDisbursementBatchEntry.ProjectedAs<LoanDisbursementBatchEntryDTO>();
                        }
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool RemoveLoanDisbursementBatchEntries(List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntryDTOs, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in loanDisbursementBatchEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _loanDisbursementBatchEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _loanDisbursementBatchEntryRepository.Remove(persisted, serviceHeader);

                            var persistedLoanCase = _loanCaseRepository.Get(persisted.LoanCaseId, serviceHeader);

                            if (persistedLoanCase != null)
                            {
                                persistedLoanCase.IsBatched = false;
                                persistedLoanCase.BatchNumber = 0;
                                persistedLoanCase.BatchedBy = null;
                            }
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool UpdateLoanDisbursementBatchEntry(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchEntryDTO == null || loanDisbursementBatchEntryDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanDisbursementBatchEntryRepository.Get(loanDisbursementBatchEntryDTO.Id, serviceHeader);

                if (persisted != null && persisted.Status < loanDisbursementBatchEntryDTO.Status/*status flags can only go up?*/)
                {
                    persisted.Status = (byte)loanDisbursementBatchEntryDTO.Status;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public bool UpdateLoanDisbursementBatchEntries(Guid loanDisbursementBatchId, List<LoanDisbursementBatchEntryDTO> loanDisbursementBatchEntries, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (loanDisbursementBatchId != null && loanDisbursementBatchId != Guid.Empty)
            {
                List<LoanDisbursementBatchEntry> batchEntries = new List<LoanDisbursementBatchEntry>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persistedLoanDisbursementBatch = _loanDisbursementBatchRepository.Get(loanDisbursementBatchId, serviceHeader);

                    if (persistedLoanDisbursementBatch != null)
                    {
                        var existingLoanDisbursementBatchEntries = FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(loanDisbursementBatchId, serviceHeader);

                        if (existingLoanDisbursementBatchEntries != null && existingLoanDisbursementBatchEntries.Any())
                        {
                            var oldSet = from c in existingLoanDisbursementBatchEntries ?? new List<LoanDisbursementBatchEntryDTO> { } select c;

                            var newSet = from c in loanDisbursementBatchEntries ?? new List<LoanDisbursementBatchEntryDTO> { } select c;

                            var commonSet = oldSet.Intersect(newSet, new LoanDisbursementBatchEntryDTOEqualityComparer());

                            var insertSet = newSet.Except(commonSet, new LoanDisbursementBatchEntryDTOEqualityComparer());

                            var deleteSet = oldSet.Except(commonSet, new LoanDisbursementBatchEntryDTOEqualityComparer());

                            if (insertSet != null && insertSet.Any())
                            {
                                List<LoanDisbursementBatchEntry> insertSetBatchEntries = new List<LoanDisbursementBatchEntry>();

                                foreach (var item in insertSet)
                                {
                                    if (_loanDisbursementBatchEntryRepository.AllMatchingCount(LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanCaseId(item.LoanCaseId), serviceHeader) == 0)
                                    {
                                        if (!insertSetBatchEntries.Any(x => x.LoanCaseId == item.LoanCaseId) && (persistedLoanDisbursementBatch.LoanProductCategory == item.LoanCaseLoanRegistrationLoanProductCategory))
                                        {
                                            var loanDisbursementBatchEntry = LoanDisbursementBatchEntryFactory.CreateLoanDisbursementBatchEntry(loanDisbursementBatchId, item.LoanCaseId, item.Reference);

                                            loanDisbursementBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                            loanDisbursementBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                            insertSetBatchEntries.Add(loanDisbursementBatchEntry);
                                        }
                                    }
                                }

                                if (insertSetBatchEntries.Any())
                                {
                                    foreach (var item in insertSetBatchEntries)
                                    {
                                        var persistedLoanCase = _loanCaseRepository.Get(item.LoanCaseId, serviceHeader);

                                        if (persistedLoanCase != null)
                                        {
                                            persistedLoanCase.IsBatched = true;
                                            persistedLoanCase.BatchNumber = persistedLoanDisbursementBatch.BatchNumber;
                                            persistedLoanCase.BatchedBy = serviceHeader.ApplicationUserName;
                                        }
                                    }

                                    batchEntries.AddRange(insertSetBatchEntries);
                                }
                            }
                        }
                        else
                        {
                            List<LoanDisbursementBatchEntry> freshBatchEntries = new List<LoanDisbursementBatchEntry>();

                            foreach (var item in loanDisbursementBatchEntries)
                            {
                                if (_loanDisbursementBatchEntryRepository.AllMatchingCount(LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanCaseId(item.LoanCaseId), serviceHeader) == 0)
                                {
                                    if (!freshBatchEntries.Any(x => x.LoanCaseId == item.LoanCaseId))
                                    {
                                        var loanDisbursementBatchEntry = LoanDisbursementBatchEntryFactory.CreateLoanDisbursementBatchEntry(loanDisbursementBatchId, item.LoanCaseId, item.Reference);

                                        loanDisbursementBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                        loanDisbursementBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                        freshBatchEntries.Add(loanDisbursementBatchEntry);
                                    }
                                }
                            }

                            if (freshBatchEntries.Any())
                            {
                                foreach (var item in freshBatchEntries)
                                {
                                    var persistedLoanCase = _loanCaseRepository.Get(item.LoanCaseId, serviceHeader);

                                    if (persistedLoanCase != null)
                                    {
                                        persistedLoanCase.IsBatched = true;
                                        persistedLoanCase.BatchNumber = persistedLoanDisbursementBatch.BatchNumber;
                                        persistedLoanCase.BatchedBy = serviceHeader.ApplicationUserName;
                                    }
                                }

                                batchEntries.AddRange(freshBatchEntries);
                            }
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                }

                if (result)
                {
                    var bcpBatchEntries = new List<LoanDisbursementBatchEntryBulkCopyDTO>();

                    batchEntries.ForEach(c =>
                    {
                        LoanDisbursementBatchEntryBulkCopyDTO bcpc =
                            new LoanDisbursementBatchEntryBulkCopyDTO
                            {
                                Id = c.Id,
                                LoanDisbursementBatchId = c.LoanDisbursementBatchId,
                                LoanCaseId = c.LoanCaseId,
                                Reference = c.Reference,
                                Status = c.Status,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate,
                            };

                        bcpBatchEntries.Add(bcpc);
                    });

                    result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _loanDisbursementBatchEntryRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                }
            }

            return result;
        }

        public bool PostLoanDisbursementBatchEntry(Guid loanDisbursementBatchEntryId, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (MarkLoanDisbursementBatchEntryPosted(loanDisbursementBatchEntryId, serviceHeader))
            {
                var loanDisbursementBatchEntryDTO = FindLoanDisbursementBatchEntry(loanDisbursementBatchEntryId, serviceHeader);
                if (loanDisbursementBatchEntryDTO == null || loanDisbursementBatchEntryDTO.Status != (int)BatchEntryStatus.Posted)
                    return result;

                var loanDisbursementBatchDTO = FindCachedLoanDisbursementBatch(loanDisbursementBatchEntryDTO.LoanDisbursementBatchId, serviceHeader);
                if (loanDisbursementBatchDTO == null)
                    return result;

                var postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);
                if (postingPeriodDTO == null)
                    return result;

                serviceHeader.ApplicationUserName = loanDisbursementBatchDTO.AuthorizedBy ?? serviceHeader.ApplicationUserName;

                CustomerAccountDTO customerLoanAccountDTO = null;

                CustomerAccountDTO customerSavingsAccountDTO = null;

                #region find loan account

                var customerLoanAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(loanDisbursementBatchEntryDTO.LoanCaseCustomerId, loanDisbursementBatchEntryDTO.LoanCaseLoanProductId, serviceHeader);

                if (customerLoanAccounts != null && customerLoanAccounts.Any())
                    customerLoanAccountDTO = customerLoanAccounts.First();
                else
                {
                    var customerAccountDTO = new CustomerAccountDTO
                    {
                        BranchId = loanDisbursementBatchEntryDTO.LoanCaseBranchId,
                        CustomerId = loanDisbursementBatchEntryDTO.LoanCaseCustomerId,
                        CustomerAccountTypeProductCode = (int)ProductCode.Loan,
                        CustomerAccountTypeTargetProductId = loanDisbursementBatchEntryDTO.LoanCaseLoanProductId,
                        CustomerAccountTypeTargetProductCode = loanDisbursementBatchEntryDTO.LoanCaseLoanProductCode,
                        Status = (int)CustomerAccountStatus.Normal,
                        RecordStatus = (int)RecordStatus.Approved,
                    };

                    customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                    if (customerAccountDTO != null)
                        customerLoanAccountDTO = customerAccountDTO;
                }

                #endregion

                #region find savings account

                var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(loanDisbursementBatchEntryDTO.LoanCaseCustomerId, loanDisbursementBatchEntryDTO.LoanCaseSavingsProductId, serviceHeader);

                if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                    customerSavingsAccountDTO = customerSavingsAccounts.First();
                else
                {
                    var customerAccountDTO = new CustomerAccountDTO
                    {
                        BranchId = loanDisbursementBatchEntryDTO.LoanCaseBranchId,
                        CustomerId = loanDisbursementBatchEntryDTO.LoanCaseCustomerId,
                        CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                        CustomerAccountTypeTargetProductId = loanDisbursementBatchEntryDTO.LoanCaseSavingsProductId,
                        CustomerAccountTypeTargetProductCode = loanDisbursementBatchEntryDTO.LoanCaseSavingsProductCode,
                        Status = (int)CustomerAccountStatus.Normal,
                    };

                    customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                    if (customerAccountDTO != null)
                        customerSavingsAccountDTO = customerAccountDTO;
                }

                #endregion

                if (customerLoanAccountDTO != null && customerSavingsAccountDTO != null)
                {
                    var primaryDescription = string.Format("Disbursement~{0} {1}", loanDisbursementBatchEntryDTO.LoanCaseLoanProductDescription, loanDisbursementBatchEntryDTO.LoanCaseReference);

                    var secondaryDescription = string.Format("{0}~{1}", loanDisbursementBatchDTO.TypeDescription, loanDisbursementBatchDTO.Reference);

                    var reference = string.Format("B#{0}~L#{1}", loanDisbursementBatchDTO.PaddedBatchNumber, loanDisbursementBatchEntryDTO.LoanCasePaddedCaseNumber);

                    var journals = new List<Journal>();

                    var customerAccountCarryForwards = new List<CustomerAccountCarryForward>();

                    var customerAccountArrearages = new List<CustomerAccountArrearage>();

                    #region 1. Disburse to customer savings & loan accounts

                    var disbursementJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(disbursementJournal, loanDisbursementBatchEntryDTO.LoanCaseSavingsProductChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseLoanProductChartOfAccountId, customerSavingsAccountDTO, customerLoanAccountDTO, serviceHeader);
                    journals.Add(disbursementJournal);

                    #endregion

                    #region 2. Do we need to recover upfront dynamic charges from loan?

                    var loanAccountDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanDisbursementBatchEntryDTO.LoanCaseLoanProductId, (int)DynamicChargeRecoverySource.LoanAccount, (int)DynamicChargeRecoveryMode.Upfront, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount, customerLoanAccountDTO, serviceHeader, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths);

                    if (loanAccountDynamicChargeTariffs != null && loanAccountDynamicChargeTariffs.Any())
                    {
                        // recover interest charges
                        loanAccountDynamicChargeTariffs.ForEach(tariff =>
                        {
                            var dynamicChargeTariffJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(dynamicChargeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerLoanAccountDTO, customerLoanAccountDTO, serviceHeader);
                            journals.Add(dynamicChargeTariffJournal);
                        });

                        // bookmark top-up amount
                        loanDisbursementBatchEntryDTO.LoanCaseAuditTopUpAmount = loanAccountDynamicChargeTariffs.Sum(x => x.Amount);
                    }

                    #endregion

                    if (_loanCaseAppService.MarkLoanCaseDisbursed(loanDisbursementBatchEntryDTO, serviceHeader)) // mark disbursed at this point to take care of persisting audit top up charges above
                    {
                        #region 3. Do we need to auto-create a standing order?

                        decimal PV = (loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount + loanDisbursementBatchEntryDTO.LoanCaseAuditTopUpAmount);

                        decimal Pmt = (decimal)_financialsService.Pmt(loanDisbursementBatchEntryDTO.LoanCaseLoanInterestCalculationMode, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationPaymentFrequencyPerYear, loanDisbursementBatchEntryDTO.LoanCaseLoanInterestAnnualPercentageRate, -(double)PV, 0d, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationPaymentDueDate);

                        var repaymentSchedule = _financialsService.RepaymentSchedule(loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationPaymentFrequencyPerYear, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationGracePeriod, loanDisbursementBatchEntryDTO.LoanCaseLoanInterestCalculationMode, loanDisbursementBatchEntryDTO.LoanCaseLoanInterestAnnualPercentageRate, -(double)PV, 0d, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationPaymentDueDate);

                        // do we need to reset?
                        var chargeableFirstInterestValue = Math.Max(repaymentSchedule.First().InterestPayment, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationMinimumInterestAmount);

                        var existingStandingOrders = _standingOrderAppService.FindStandingOrders(customerSavingsAccountDTO.Id, customerLoanAccountDTO.Id, serviceHeader);

                        if (existingStandingOrders != null && existingStandingOrders.Any())
                        {
                            var targetStandingOrder = existingStandingOrders.FirstOrDefault();

                            if (targetStandingOrder != null)
                            {
                                targetStandingOrder.ChargeType = (int)ChargeType.FixedAmount;
                                targetStandingOrder.Trigger = loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationStandingOrderTrigger;
                                targetStandingOrder.LoanAmount = PV;
                                targetStandingOrder.PaymentPerPeriod = Pmt;
                                targetStandingOrder.Principal = loanDisbursementBatchEntryDTO.LoanCaseApprovedPrincipalPayment != 0m ? loanDisbursementBatchEntryDTO.LoanCaseApprovedPrincipalPayment : repaymentSchedule.First().PrincipalPayment;
                                targetStandingOrder.Interest = Math.Max(chargeableFirstInterestValue, loanDisbursementBatchEntryDTO.LoanCaseApprovedInterestPayment != 0m ? loanDisbursementBatchEntryDTO.LoanCaseApprovedInterestPayment : repaymentSchedule.First().InterestPayment);
                                targetStandingOrder.DurationStartDate = repaymentSchedule.First().DueDate;
                                targetStandingOrder.DurationEndDate = repaymentSchedule.Last().DueDate;
                                targetStandingOrder.ScheduleFrequency = loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationPaymentFrequencyPerYear;
                                targetStandingOrder.IsLocked = false;
                                targetStandingOrder.Remarks = string.Empty;
                                targetStandingOrder.BeneficiaryProductProductCode = (int)ProductCode.Loan;
                                targetStandingOrder.BeneficiaryProductRoundingType = loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationRoundingType;

                                loanDisbursementBatchEntryDTO.LoanCaseMonthlyPaybackAmount = (targetStandingOrder.Principal + targetStandingOrder.Interest);

                                if (targetStandingOrder.DurationStartDate == targetStandingOrder.DurationEndDate) // happens for 1-month loans
                                    targetStandingOrder.DurationEndDate = UberUtil.GetLastDayOfMonth(targetStandingOrder.DurationEndDate);

                                switch ((InterestCalculationMode)loanDisbursementBatchEntryDTO.LoanCaseLoanInterestCalculationMode)
                                {
                                    case InterestCalculationMode.StraightLineAmortization:
                                    case InterestCalculationMode.DiminishingBalanceAmortization:
                                        targetStandingOrder.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths;
                                        targetStandingOrder.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths;
                                        targetStandingOrder.PaymentPerPeriod = (targetStandingOrder.Principal + targetStandingOrder.Interest);
                                        loanDisbursementBatchEntryDTO.LoanCaseMonthlyPaybackAmount = targetStandingOrder.PaymentPerPeriod;
                                        break;
                                    default:
                                        break;
                                }

                                targetStandingOrder.CapitalizedInterest = targetStandingOrder.Interest;
                                _standingOrderAppService.UpdateStandingOrder(targetStandingOrder, serviceHeader);
                            }
                        }
                        else
                        {
                            var newStandingOrderDTO =
                                new StandingOrderDTO
                                {
                                    ChargeType = (int)ChargeType.FixedAmount,
                                    Trigger = loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationStandingOrderTrigger,
                                    BenefactorCustomerAccountId = customerSavingsAccountDTO.Id,
                                    BeneficiaryCustomerAccountId = customerLoanAccountDTO.Id,
                                    BeneficiaryProductProductCode = (int)ProductCode.Loan,
                                    BeneficiaryProductRoundingType = loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationRoundingType,
                                    PaymentPerPeriod = Pmt,
                                    LoanAmount = PV,
                                    Principal = loanDisbursementBatchEntryDTO.LoanCaseApprovedPrincipalPayment != 0m ? loanDisbursementBatchEntryDTO.LoanCaseApprovedPrincipalPayment : repaymentSchedule.First().PrincipalPayment,
                                    Interest = Math.Max(chargeableFirstInterestValue, loanDisbursementBatchEntryDTO.LoanCaseApprovedInterestPayment != 0m ? loanDisbursementBatchEntryDTO.LoanCaseApprovedInterestPayment : repaymentSchedule.First().InterestPayment),
                                    DurationStartDate = repaymentSchedule.First().DueDate,
                                    DurationEndDate = repaymentSchedule.Last().DueDate,
                                    ScheduleFrequency = loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationPaymentFrequencyPerYear,
                                    Chargeable = true
                                };

                            loanDisbursementBatchEntryDTO.LoanCaseMonthlyPaybackAmount = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);

                            if (newStandingOrderDTO.DurationStartDate == newStandingOrderDTO.DurationEndDate) // happens for 1-month loans
                                newStandingOrderDTO.DurationEndDate = UberUtil.GetLastDayOfMonth(newStandingOrderDTO.DurationEndDate);

                            switch ((InterestCalculationMode)loanDisbursementBatchEntryDTO.LoanCaseLoanInterestCalculationMode)
                            {
                                case InterestCalculationMode.StraightLineAmortization:
                                case InterestCalculationMode.DiminishingBalanceAmortization:
                                    newStandingOrderDTO.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths;
                                    newStandingOrderDTO.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths;
                                    newStandingOrderDTO.PaymentPerPeriod = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);
                                    loanDisbursementBatchEntryDTO.LoanCaseMonthlyPaybackAmount = newStandingOrderDTO.PaymentPerPeriod;
                                    break;
                                default:
                                    break;
                            }

                            newStandingOrderDTO.CapitalizedInterest = newStandingOrderDTO.Interest;
                            _standingOrderAppService.AddNewStandingOrder(newStandingOrderDTO, serviceHeader);
                        }

                        #endregion

                        #region 4. Do we need to recover disbursement charges?

                        switch ((DisbursementType)loanDisbursementBatchDTO.Type)
                        {
                            case DisbursementType.Normal:

                                var normalTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanDisbursementBatchEntryDTO.LoanCaseLoanProductId, (int)LoanProductKnownChargeType.NormalLoanDisbursementFee, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount, customerSavingsAccountDTO, serviceHeader);

                                normalTariffs.ForEach(tariff =>
                                {
                                    var normalTariffJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(normalTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                    journals.Add(normalTariffJournal);
                                });

                                break;
                            case DisbursementType.Express:

                                var expressTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanDisbursementBatchEntryDTO.LoanCaseLoanProductId, (int)LoanProductKnownChargeType.ExpressLoanDisbursementFee, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount, customerSavingsAccountDTO, serviceHeader);

                                expressTariffs.ForEach(tariff =>
                                {
                                    var expressTariffJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(expressTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                    journals.Add(expressTariffJournal);
                                });

                                break;
                            case DisbursementType.Waiver:
                            default:
                                break;
                        }

                        #endregion

                        var attachedLoans = _loanCaseAppService.FindAttachedLoansByLoanCaseId(loanDisbursementBatchEntryDTO.LoanCaseId, serviceHeader);

                        #region 5. Do we need to recover upfront dynamic charges from savings?

                        var savingsAccountUpfrontDynamicChargeAmount = loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount;

                        var savingsAccountUpfrontDynamicChargeTopUpAmount = 0m;

                        if (attachedLoans != null && attachedLoans.Any())
                        {
                            foreach (var attachedLoanDTO in attachedLoans)
                            {
                                if (attachedLoanDTO.CustomerAccountCustomerId == loanDisbursementBatchEntryDTO.LoanCaseCustomerId) // safety check
                                {
                                    if (attachedLoanDTO.CustomerAccountId == customerLoanAccountDTO.Id) // this is a top-up
                                    {
                                        _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerLoanAccountDTO }, serviceHeader, true);

                                        var actualPrincipalBalance = 0m;
                                        if (customerLoanAccountDTO.PrincipalBalance * -1 > 0m)
                                        {
                                            if (attachedLoanDTO.PrincipalBalance * -1 > 0m)
                                                actualPrincipalBalance = Math.Min((customerLoanAccountDTO.PrincipalBalance * -1), (attachedLoanDTO.PrincipalBalance * -1));
                                            else actualPrincipalBalance = customerLoanAccountDTO.PrincipalBalance * -1;
                                        }

                                        savingsAccountUpfrontDynamicChargeTopUpAmount = (loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount - actualPrincipalBalance);
                                    }
                                }
                            }
                        }

                        var savingsAccountUpfrontDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanDisbursementBatchEntryDTO.LoanCaseLoanProductId, (int)DynamicChargeRecoverySource.SavingsAccount, (int)DynamicChargeRecoveryMode.Upfront, savingsAccountUpfrontDynamicChargeAmount, customerSavingsAccountDTO, serviceHeader, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths, savingsAccountUpfrontDynamicChargeTopUpAmount);

                        if (savingsAccountUpfrontDynamicChargeTariffs != null && savingsAccountUpfrontDynamicChargeTariffs.Any())
                        {
                            savingsAccountUpfrontDynamicChargeTariffs.ForEach(tariff =>
                            {
                                var dynamicChargeTariffJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(dynamicChargeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                journals.Add(dynamicChargeTariffJournal);
                            });
                        }

                        #endregion

                        #region 6. Do we need to charge interest upfront?

                        if (loanDisbursementBatchEntryDTO.LoanCaseLoanInterestChargeMode == (int)InterestChargeMode.Upfront)
                        {
                            var repaymentScheduleTotalInterest = repaymentSchedule.Sum(x => x.InterestPayment);

                            var minimumTotalInterest = loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationMinimumInterestAmount * loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths;

                            // do we need to reset?
                            var chargeableTotalInterestValue = Math.Max(repaymentScheduleTotalInterest, minimumTotalInterest);

                            if (chargeableTotalInterestValue != 0m)
                            {
                                // do we need to round?
                                switch ((RoundingType)loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationRoundingType)
                                {
                                    case RoundingType.ToEven:
                                        chargeableTotalInterestValue = Math.Round(chargeableTotalInterestValue, MidpointRounding.ToEven);
                                        break;
                                    case RoundingType.AwayFromZero:
                                        chargeableTotalInterestValue = Math.Round(chargeableTotalInterestValue, MidpointRounding.AwayFromZero);
                                        break;
                                    case RoundingType.Ceiling:
                                        chargeableTotalInterestValue = Math.Ceiling(chargeableTotalInterestValue);
                                        break;
                                    case RoundingType.Floor:
                                        chargeableTotalInterestValue = Math.Floor(chargeableTotalInterestValue);
                                        break;
                                    default:
                                        break;
                                }

                                // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                                var chargeInterestJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, chargeableTotalInterestValue, "Interest Charged Up-front", secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                                _journalEntryPostingService.PerformDoubleEntry(chargeInterestJournal, loanDisbursementBatchEntryDTO.LoanCaseLoanProductInterestChargedChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseLoanProductInterestReceivableChartOfAccountId, customerLoanAccountDTO, customerLoanAccountDTO, serviceHeader);
                                journals.Add(chargeInterestJournal);

                                #region  Do we need to recover interest upfront?

                                if (loanDisbursementBatchEntryDTO.LoanCaseLoanInterestRecoveryMode == (int)InterestRecoveryMode.Upfront)
                                {
                                    // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                                    var interestReceivableJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, chargeableTotalInterestValue, "Interest Paid Up-front", secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(interestReceivableJournal, loanDisbursementBatchEntryDTO.LoanCaseLoanProductInterestReceivableChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseSavingsProductChartOfAccountId, customerLoanAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                    journals.Add(interestReceivableJournal);

                                    // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                                    var interestReceivedJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, chargeableTotalInterestValue, "Interest Paid Up-front", secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(interestReceivedJournal, loanDisbursementBatchEntryDTO.LoanCaseLoanProductInterestReceivedChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseLoanProductInterestChargedChartOfAccountId, customerLoanAccountDTO, customerLoanAccountDTO, serviceHeader);
                                    journals.Add(interestReceivedJournal);
                                }

                                #endregion
                            }
                        }

                        #endregion

                        #region 7. Do we need to recover attached loans?

                        var attachedLoansAmount = 0m;

                        if (attachedLoans != null && attachedLoans.Any())
                        {
                            attachedLoans.ForEach(attachedLoanDTO =>
                            {
                                if (attachedLoanDTO.CustomerAccountCustomerId == loanDisbursementBatchEntryDTO.LoanCaseCustomerId) // safety check
                                {
                                    if (attachedLoanDTO.CustomerAccountId != customerLoanAccountDTO.Id) /*release current guarantors*/
                                        _loanCaseAppService.ReleaseLoanGuarantors(attachedLoanDTO.CustomerAccountId, serviceHeader);
                                    else if (attachedLoanDTO.CustomerAccountId == customerLoanAccountDTO.Id) /*this is a refinance loan, release pre-existing guarantors*/
                                        _loanCaseAppService.ReleaseRefinancedLoanGuarantors(attachedLoanDTO.CustomerAccountId, loanDisbursementBatchEntryDTO.LoanCaseCreatedDate, serviceHeader);

                                    var attachedLoanAccount = _customerAccountAppService.FindCustomerAccountDTO(attachedLoanDTO.CustomerAccountId, serviceHeader);

                                    _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { attachedLoanAccount }, serviceHeader, true);

                                    var actualPrincipalBalance = 0m;
                                    if (attachedLoanAccount.PrincipalBalance * -1 > 0m)
                                    {
                                        if (attachedLoanDTO.PrincipalBalance * -1 > 0m)
                                            actualPrincipalBalance = Math.Min((attachedLoanAccount.PrincipalBalance * -1), (attachedLoanDTO.PrincipalBalance * -1));
                                        else actualPrincipalBalance = attachedLoanAccount.PrincipalBalance * -1;
                                    }

                                    var actualInterestBalance = 0m;
                                    if (attachedLoanAccount.InterestBalance * -1 > 0m)
                                    {
                                        if (attachedLoanDTO.InterestBalance * -1 > 0m)
                                            actualInterestBalance = Math.Min((attachedLoanAccount.InterestBalance * -1), (attachedLoanDTO.InterestBalance * -1));
                                        else actualInterestBalance = attachedLoanAccount.InterestBalance * -1;
                                    }

                                    var actualCarryForwardsBalance = 0m;
                                    if (attachedLoanAccount.CarryForwardsBalance * -1 > 0m)
                                    {
                                        if (attachedLoanDTO.CarryForwardsBalance * -1 > 0m)
                                            actualCarryForwardsBalance = Math.Min((attachedLoanAccount.CarryForwardsBalance * -1), (attachedLoanDTO.CarryForwardsBalance * -1));
                                        else actualCarryForwardsBalance = attachedLoanAccount.CarryForwardsBalance * -1;
                                    }

                                    #region recover attached interest + principal

                                    // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                                    var interestReceivableClearanceJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, actualInterestBalance, string.Format("Attached Interest Clearance~{0}", attachedLoanDTO.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanOffsetting, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(interestReceivableClearanceJournal, attachedLoanDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseSavingsProductChartOfAccountId, attachedLoanAccount, customerSavingsAccountDTO, serviceHeader);
                                    journals.Add(interestReceivableClearanceJournal);

                                    // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                                    var interestReceivedClearanceJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, actualInterestBalance, string.Format("Attached Interest Clearance~{0}", attachedLoanDTO.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanOffsetting, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(interestReceivedClearanceJournal, attachedLoanDTO.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, attachedLoanDTO.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, attachedLoanAccount, attachedLoanAccount, serviceHeader);
                                    journals.Add(interestReceivedClearanceJournal);

                                    // Credit LoanProduct.ChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                                    var principalClearanceJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, actualPrincipalBalance, string.Format("Attached Principal Clearance~{0}", attachedLoanDTO.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanOffsetting, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(principalClearanceJournal, attachedLoanDTO.CustomerAccountTypeTargetProductChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseSavingsProductChartOfAccountId, attachedLoanAccount, customerSavingsAccountDTO, serviceHeader);
                                    journals.Add(principalClearanceJournal);

                                    #endregion

                                    #region recover attached carryforwards

                                    var currentCarryForwards = _customerAccountAppService.FindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountId(customerSavingsAccountDTO.Id, attachedLoanDTO.CustomerAccountId, serviceHeader);

                                    if (currentCarryForwards != null && currentCarryForwards.Any())
                                    {
                                        var grouping = from p in currentCarryForwards
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
                                                var carryForwardsAmount = Math.Min((totalPayments), (attachedLoanDTO.CarryForwardsBalance * -1));

                                                // Credit CarryForward.BeneficiaryChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                                                var carryFowardBeneficiaryJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, carryForwardsAmount, string.Format("Carry Forwards Offsetting~{0}", attachedLoanDTO.CustomerAccountTypeTargetProductDescription), secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanOffsetting, null, serviceHeader);
                                                _journalEntryPostingService.PerformDoubleEntry(carryFowardBeneficiaryJournal, item.BeneficiaryChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseSavingsProductChartOfAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                                journals.Add(carryFowardBeneficiaryJournal);

                                                // Do we need to update carry forward history?
                                                var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(customerSavingsAccountDTO.Id, item.BeneficiaryCustomerAccountId, item.BeneficiaryChartOfAccountId, carryForwardsAmount * -1/*-ve cos is payment*/, string.Format("Carry Forwards Offsetting~{0}", attachedLoanDTO.CustomerAccountTypeTargetProductDescription));
                                                customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                                customerAccountCarryForwards.Add(customerAccountCarryForward);
                                            }
                                        }
                                    }

                                    #endregion

                                    #region offset arrearages

                                    var actualPrincipalArrearagesBalance = attachedLoanAccount.PrincipalArrearagesBalance * -1 > 0m ? attachedLoanAccount.PrincipalArrearagesBalance * -1 : 0m;

                                    var actualInterestArrearagesBalance = attachedLoanAccount.InterestArrearagesBalance * -1 > 0m ? attachedLoanAccount.InterestArrearagesBalance * -1 : 0m;

                                    // Update arrearage history
                                    var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(attachedLoanAccount.Id, (int)ArrearageCategory.Interest, actualInterestArrearagesBalance * -1/*-ve cos is payment*/, reference);
                                    customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                    customerAccountArrearages.Add(customerAccountInterestArrearage);

                                    var customerAccountPrincipalArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(attachedLoanAccount.Id, (int)ArrearageCategory.Principal, actualPrincipalArrearagesBalance * -1/*-ve cos is payment*/, reference);
                                    customerAccountPrincipalArrearage.CreatedBy = serviceHeader.ApplicationUserName;
                                    customerAccountArrearages.Add(customerAccountPrincipalArrearage);

                                    #endregion

                                    //  levy clearance charges?
                                    if (attachedLoanDTO.CustomerAccountTypeTargetProductChargeClearanceFee)
                                    {
                                        var clearanceTariffs = _commissionAppService.ComputeTariffsByLoanProduct(attachedLoanDTO.CustomerAccountCustomerAccountTypeTargetProductId, (int)LoanProductKnownChargeType.LoanClearanceCharges, (actualPrincipalBalance + actualInterestBalance + actualCarryForwardsBalance), actualPrincipalBalance, customerSavingsAccountDTO, serviceHeader);

                                        clearanceTariffs.ForEach(tariff =>
                                        {
                                            var clearanceTariffJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanOffsetting, null, serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(clearanceTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                            journals.Add(clearanceTariffJournal);
                                        });
                                    }

                                    // tally recovered attached loans amount
                                    attachedLoansAmount += (actualPrincipalBalance + actualInterestBalance + actualCarryForwardsBalance);
                                }
                            });
                        }

                        #endregion

                        #region 8. Do we need to recover deductibles

                        var loanProductDeductibles = _loanProductAppService.FindLoanProductDeductibles(loanDisbursementBatchEntryDTO.LoanCaseLoanProductId, serviceHeader);

                        if (loanProductDeductibles != null && loanProductDeductibles.Any())
                        {
                            _loanProductAppService.FetchLoanProductDeductiblesProductDescription(loanProductDeductibles, serviceHeader);

                            foreach (var deductibleDTO in loanProductDeductibles)
                            {
                                if (deductibleDTO.CustomerAccountTypeTargetProductId == loanDisbursementBatchEntryDTO.LoanCaseSavingsProductId)
                                    continue;

                                CustomerAccountDTO deductibleCustomerAccountDTO = null;

                                #region Find deductible beneficiary account

                                var deductibleCustomerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(deductibleDTO.CustomerAccountTypeTargetProductId, loanDisbursementBatchEntryDTO.LoanCaseCustomerId, serviceHeader);

                                if (deductibleCustomerAccounts != null && deductibleCustomerAccounts.Any())
                                {
                                    _customerAccountAppService.FetchCustomerAccountsProductDescription(deductibleCustomerAccounts, serviceHeader);

                                    deductibleCustomerAccountDTO = deductibleCustomerAccounts.First();
                                }
                                else
                                {
                                    var customerAccountDTO = new CustomerAccountDTO
                                    {
                                        BranchId = loanDisbursementBatchEntryDTO.LoanCaseBranchId,
                                        CustomerId = loanDisbursementBatchEntryDTO.LoanCaseCustomerId,
                                        CustomerAccountTypeProductCode = deductibleDTO.CustomerAccountTypeProductCode,
                                        CustomerAccountTypeTargetProductId = deductibleDTO.CustomerAccountTypeTargetProductId,
                                        CustomerAccountTypeTargetProductCode = deductibleDTO.CustomerAccountTypeTargetProductCode,
                                        Status = (int)CustomerAccountStatus.Normal,
                                    };

                                    customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                    if (customerAccountDTO != null)
                                    {
                                        deductibleCustomerAccountDTO = customerAccountDTO;

                                        _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { deductibleCustomerAccountDTO }, serviceHeader);
                                    }
                                }

                                #endregion

                                if (deductibleCustomerAccountDTO != null)
                                {
                                    var deductibleAmount = 0m;

                                    switch ((ChargeType)deductibleDTO.ChargeType)
                                    {
                                        case ChargeType.Percentage:
                                            if (deductibleDTO.ComputeChargeOnTopUp && savingsAccountUpfrontDynamicChargeTopUpAmount > 0m)
                                            {
                                                deductibleAmount = Math.Round(Convert.ToDecimal((deductibleDTO.ChargePercentage * Convert.ToDouble(savingsAccountUpfrontDynamicChargeTopUpAmount)) / 100), 4, MidpointRounding.AwayFromZero);
                                            }
                                            else
                                            {
                                                deductibleAmount = Math.Round(Convert.ToDecimal((deductibleDTO.ChargePercentage * Convert.ToDouble(loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount)) / 100), 4, MidpointRounding.AwayFromZero);
                                            }
                                            break;
                                        case ChargeType.FixedAmount:
                                            if (deductibleDTO.ComputeChargeOnTopUp && savingsAccountUpfrontDynamicChargeTopUpAmount > 0m)
                                            {
                                                deductibleAmount = Math.Min(savingsAccountUpfrontDynamicChargeTopUpAmount, deductibleDTO.ChargeFixedAmount);
                                            }
                                            else
                                            {
                                                deductibleAmount = Math.Min(deductibleDTO.ChargeFixedAmount, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount);
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    if (deductibleAmount * -1 < 0m)
                                    {
                                        if (deductibleDTO.NetOffInvestmentBalance && deductibleDTO.CustomerAccountTypeProductCode == (int)ProductCode.Investment)
                                        {
                                            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { deductibleCustomerAccountDTO }, serviceHeader);

                                            if (deductibleCustomerAccountDTO.BookBalance * -1 < 0m && deductibleCustomerAccountDTO.BookBalance < deductibleAmount)
                                                deductibleAmount -= deductibleCustomerAccountDTO.BookBalance;
                                        }

                                        if (deductibleAmount * -1 < 0m)
                                        {
                                            var deductibleJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, loanDisbursementBatchEntryDTO.LoanCaseBranchId, null, deductibleAmount, deductibleDTO.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanDisbursement, null, serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(deductibleJournal, deductibleCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, loanDisbursementBatchEntryDTO.LoanCaseSavingsProductChartOfAccountId, deductibleCustomerAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                            journals.Add(deductibleJournal);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region 9. Bulk-Insert journals

                        if (journals.Any())
                        {
                            result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                        }

                        #endregion

                        #region 10. Do we need to increment loan cycle? >> microcredit loan product

                        if (loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationMicrocredit)
                        {
                            var microCreditGroupMember = _microCreditGroupAppService.FindMicroCreditGroupMemberByCustomerId(loanDisbursementBatchEntryDTO.LoanCaseCustomerId, serviceHeader);

                            if (microCreditGroupMember != null)
                            {
                                microCreditGroupMember.LoanCycle = ++microCreditGroupMember.LoanCycle;

                                _microCreditGroupAppService.UpdateMicroCreditGroupMember(microCreditGroupMember, serviceHeader);
                            }
                        }

                        #endregion

                        #region 11. Do we need to auto-attach to parent loan?

                        var parentLoanCase = _loanCaseAppService.FindLoanCase(loanDisbursementBatchEntryDTO.LoanCaseParentId ?? Guid.Empty, serviceHeader);

                        if (parentLoanCase != null)
                        {
                            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerLoanAccountDTO }, serviceHeader, true, false);

                            var newAttachedLoansList = new List<AttachedLoanDTO>();

                            var parentLoanCaseAttachedLoans = _loanCaseAppService.FindAttachedLoansByLoanCaseId(parentLoanCase.Id, serviceHeader);

                            if (parentLoanCaseAttachedLoans != null && parentLoanCaseAttachedLoans.Any())
                                newAttachedLoansList.AddRange(parentLoanCaseAttachedLoans);

                            var alreadyAttached = newAttachedLoansList.Any(x => x.CustomerAccountId == customerLoanAccountDTO.Id);

                            if (alreadyAttached)
                            {
                                foreach (var item in newAttachedLoansList)
                                {
                                    if (item.CustomerAccountId == customerLoanAccountDTO.Id)/*reset attached balances*/
                                    {
                                        item.PrincipalBalance = customerLoanAccountDTO.PrincipalBalance;
                                        item.InterestBalance = customerLoanAccountDTO.InterestBalance;
                                    }
                                }
                            }
                            else
                            {
                                var newAttachedLoanDTO = new AttachedLoanDTO
                                {
                                    LoanCaseId = parentLoanCase.Id,
                                    CustomerAccountId = customerLoanAccountDTO.Id,
                                    PrincipalBalance = customerLoanAccountDTO.PrincipalBalance,
                                    InterestBalance = customerLoanAccountDTO.InterestBalance,
                                };

                                newAttachedLoansList.Add(newAttachedLoanDTO);
                            }

                            _loanCaseAppService.UpdateAttachedLoans(parentLoanCase.Id, newAttachedLoansList, serviceHeader);
                        }

                        #endregion

                        #region 12. Do we need to track carry forward dynamic charges from savings?

                        var savingsAccountCarryForwardDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanDisbursementBatchEntryDTO.LoanCaseLoanProductId, (int)DynamicChargeRecoverySource.SavingsAccount, (int)DynamicChargeRecoveryMode.CarryForward, loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount, customerSavingsAccountDTO, serviceHeader, loanDisbursementBatchEntryDTO.LoanCaseLoanRegistrationTermInMonths, 0m, attachedLoansAmount);

                        if (savingsAccountCarryForwardDynamicChargeTariffs != null && savingsAccountCarryForwardDynamicChargeTariffs.Any())
                        {
                            var customerAccountCarryForwardDTOs = new List<CustomerAccountCarryForwardDTO>();

                            savingsAccountCarryForwardDynamicChargeTariffs.ForEach(tariff =>
                            {
                                var customerAccountCarryForwardDTO = new CustomerAccountCarryForwardDTO
                                {
                                    BenefactorCustomerAccountId = customerSavingsAccountDTO.Id,
                                    BeneficiaryCustomerAccountId = customerLoanAccountDTO.Id,
                                    BeneficiaryChartOfAccountId = tariff.CreditGLAccountId,
                                    Amount = tariff.Amount,
                                    Reference = string.Format("{0}~{1}", tariff.Description, reference),
                                };

                                if (tariff.DynamicCharge != null)
                                    customerAccountCarryForwardDTO.DynamicChargeInstallments = tariff.DynamicCharge.Installments > 1 ? tariff.DynamicCharge.Installments : 1;

                                customerAccountCarryForwardDTOs.Add(customerAccountCarryForwardDTO);
                            });

                            var grouping = from p in customerAccountCarryForwardDTOs
                                           group p.Amount by new
                                           {
                                               p.BeneficiaryCustomerAccountId,
                                               p.BeneficiaryChartOfAccountId,
                                               p.DynamicChargeInstallments
                                           } into g
                                           select new
                                           {
                                               BeneficiaryCustomerAccountId = g.Key.BeneficiaryCustomerAccountId,
                                               BeneficiaryChartOfAccountId = g.Key.BeneficiaryChartOfAccountId,
                                               DynamicChargeInstallments = g.Key.DynamicChargeInstallments,
                                               Payments = g.ToList()
                                           };

                            var customerAccountCarryForwardInstallmentDTOs = new List<CustomerAccountCarryForwardInstallmentDTO>();

                            foreach (var item in grouping)
                            {
                                var totalPayments = item.Payments.Sum();

                                var installment = totalPayments / item.DynamicChargeInstallments;

                                var customerAccountCarryForwardInstallmentDTO = new CustomerAccountCarryForwardInstallmentDTO
                                {
                                    CustomerAccountId = item.BeneficiaryCustomerAccountId,
                                    ChartOfAccountId = item.BeneficiaryChartOfAccountId,
                                    Amount = installment,
                                    Reference = reference
                                };

                                customerAccountCarryForwardInstallmentDTOs.Add(customerAccountCarryForwardInstallmentDTO);
                            }

                            customerAccountCarryForwardDTOs.ForEach(carryForwardDTO =>
                            {
                                var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(carryForwardDTO.BenefactorCustomerAccountId, carryForwardDTO.BeneficiaryCustomerAccountId, carryForwardDTO.BeneficiaryChartOfAccountId, carryForwardDTO.Amount, carryForwardDTO.Reference);
                                customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                customerAccountCarryForwards.Add(customerAccountCarryForward);
                            });

                            #region update carry-forward installments

                            _customerAccountAppService.UpdateCustomerAccountCarryForwardInstallments(customerAccountCarryForwardInstallmentDTOs, serviceHeader);

                            #endregion
                        }

                        #endregion

                        #region 13. Bulk-Insert carry forwards + arrearages

                        _journalEntryPostingService.BulkSave(serviceHeader, null, customerAccountCarryForwards, null, customerAccountArrearages);

                        #endregion
                    }
                }
            }

            return result;
        }

        public decimal DisburseMicroLoan(Guid alternateChannelLogId, Guid settlementChartOfAccountId, CustomerAccountDTO customerLoanAccountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var settlementAmount = 0m;

            if (customerLoanAccountDTO == null || alternateChannelLogId == Guid.Empty || settlementChartOfAccountId == Guid.Empty)
                return settlementAmount;

            var loanProductDTO = _loanProductAppService.FindLoanProduct(customerLoanAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

            SavingsProductDTO savingsProductDTO = null;

            CustomerAccountDTO customerSavingsAccountDTO = null;

            #region find savings account

            var savingsProducts = _savingsProductAppService.FindSavingsProducts(customerLoanAccountDTO.ScoredLoanDisbursementProductCode, serviceHeader);

            if (savingsProducts != null && savingsProducts.Any())
                savingsProductDTO = savingsProducts.First();

            if (savingsProductDTO != null)
            {
                var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerLoanAccountDTO.CustomerId, savingsProductDTO.Id, serviceHeader);

                if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                    customerSavingsAccountDTO = customerSavingsAccounts.First();
                else
                {
                    var customerAccountDTO = new CustomerAccountDTO
                    {
                        BranchId = customerLoanAccountDTO.BranchId,
                        CustomerId = customerLoanAccountDTO.CustomerId,
                        CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                        CustomerAccountTypeTargetProductId = savingsProductDTO.Id,
                        CustomerAccountTypeTargetProductCode = savingsProductDTO.Code,
                        Status = (int)CustomerAccountStatus.Normal,
                    };

                    customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                    if (customerAccountDTO != null)
                        customerSavingsAccountDTO = customerAccountDTO;
                }
            }

            #endregion

            if (customerSavingsAccountDTO == null)
                return settlementAmount;

            var postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);
            if (postingPeriodDTO == null)
                return settlementAmount;

            var journals = new List<Journal>();

            var customerAccountCarryForwards = new List<CustomerAccountCarryForward>();

            var totalLoanDeductions = 0m;

            #region 1. Disburse to customer savings & loan accounts

            var disbursementJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
            _journalEntryPostingService.PerformDoubleEntry(disbursementJournal, savingsProductDTO.ChartOfAccountId, loanProductDTO.ChartOfAccountId, customerSavingsAccountDTO, customerLoanAccountDTO, serviceHeader);
            journals.Add(disbursementJournal);

            #endregion

            #region 2. Do we need to recover upfront dynamic charges from loan?

            var loanAccountDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanProductDTO.Id, (int)DynamicChargeRecoverySource.LoanAccount, (int)DynamicChargeRecoveryMode.Upfront, totalValue, customerLoanAccountDTO, serviceHeader, loanProductDTO.LoanRegistrationTermInMonths);

            if (loanAccountDynamicChargeTariffs != null && loanAccountDynamicChargeTariffs.Any())
            {
                // recover interest charges
                loanAccountDynamicChargeTariffs.ForEach(tariff =>
                {
                    var dynamicChargeTariffJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(dynamicChargeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerLoanAccountDTO, customerLoanAccountDTO, serviceHeader);
                    journals.Add(dynamicChargeTariffJournal);
                });
            }

            #endregion

            #region 3. Do we need to auto-create a standing order?

            decimal PV = totalValue;

            decimal Pmt = (decimal)_financialsService.Pmt(loanProductDTO.LoanInterestCalculationMode, loanProductDTO.LoanRegistrationTermInMonths, loanProductDTO.LoanRegistrationPaymentFrequencyPerYear, loanProductDTO.LoanInterestAnnualPercentageRate, -(double)PV, 0d, loanProductDTO.LoanRegistrationPaymentDueDate);

            var repaymentSchedule = _financialsService.RepaymentSchedule(loanProductDTO.LoanRegistrationTermInMonths, loanProductDTO.LoanRegistrationPaymentFrequencyPerYear, loanProductDTO.LoanRegistrationGracePeriod, loanProductDTO.LoanInterestCalculationMode, loanProductDTO.LoanInterestAnnualPercentageRate, -(double)PV, 0d, loanProductDTO.LoanRegistrationPaymentDueDate);

            // do we need to reset?
            var chargeableFirstInterestValue = Math.Max(repaymentSchedule.First().InterestPayment, loanProductDTO.LoanRegistrationMinimumInterestAmount);

            var existingStandingOrders = _standingOrderAppService.FindStandingOrders(customerSavingsAccountDTO.Id, customerLoanAccountDTO.Id, serviceHeader);

            if (existingStandingOrders != null && existingStandingOrders.Any())
            {
                var targetStandingOrder = existingStandingOrders.FirstOrDefault();

                if (targetStandingOrder != null)
                {
                    targetStandingOrder.ChargeType = (int)ChargeType.FixedAmount;
                    targetStandingOrder.Trigger = loanProductDTO.LoanRegistrationStandingOrderTrigger;
                    targetStandingOrder.LoanAmount = PV;
                    targetStandingOrder.PaymentPerPeriod = Pmt;
                    targetStandingOrder.Principal = repaymentSchedule.First().PrincipalPayment;
                    targetStandingOrder.Interest = Math.Max(chargeableFirstInterestValue, repaymentSchedule.First().InterestPayment);
                    targetStandingOrder.DurationStartDate = repaymentSchedule.First().DueDate;
                    targetStandingOrder.DurationEndDate = repaymentSchedule.Last().DueDate;
                    targetStandingOrder.ScheduleFrequency = loanProductDTO.LoanRegistrationPaymentFrequencyPerYear;
                    targetStandingOrder.IsLocked = false;
                    targetStandingOrder.Remarks = string.Empty;
                    targetStandingOrder.BeneficiaryProductProductCode = (int)ProductCode.Loan;
                    targetStandingOrder.BeneficiaryProductRoundingType = loanProductDTO.LoanRegistrationRoundingType;

                    if (targetStandingOrder.DurationStartDate == targetStandingOrder.DurationEndDate) // happens for 1-month loans
                        targetStandingOrder.DurationEndDate = UberUtil.GetLastDayOfMonth(targetStandingOrder.DurationEndDate);

                    switch ((InterestCalculationMode)loanProductDTO.LoanInterestCalculationMode)
                    {
                        case InterestCalculationMode.StraightLineAmortization:
                        case InterestCalculationMode.DiminishingBalanceAmortization:
                            targetStandingOrder.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                            targetStandingOrder.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                            targetStandingOrder.PaymentPerPeriod = (targetStandingOrder.Principal + targetStandingOrder.Interest);
                            break;
                        default:
                            break;
                    }

                    targetStandingOrder.CapitalizedInterest = targetStandingOrder.Interest;
                    _standingOrderAppService.UpdateStandingOrder(targetStandingOrder, serviceHeader);
                }
            }
            else
            {
                var newStandingOrderDTO =
                    new StandingOrderDTO
                    {
                        ChargeType = (int)ChargeType.FixedAmount,
                        Trigger = loanProductDTO.LoanRegistrationStandingOrderTrigger,
                        BenefactorCustomerAccountId = customerSavingsAccountDTO.Id,
                        BeneficiaryCustomerAccountId = customerLoanAccountDTO.Id,
                        BeneficiaryProductProductCode = (int)ProductCode.Loan,
                        BeneficiaryProductRoundingType = loanProductDTO.LoanRegistrationRoundingType,
                        PaymentPerPeriod = Pmt,
                        LoanAmount = PV,
                        Principal = repaymentSchedule.First().PrincipalPayment,
                        Interest = Math.Max(chargeableFirstInterestValue, repaymentSchedule.First().InterestPayment),
                        DurationStartDate = repaymentSchedule.First().DueDate,
                        DurationEndDate = repaymentSchedule.Last().DueDate,
                        ScheduleFrequency = loanProductDTO.LoanRegistrationPaymentFrequencyPerYear,
                        Chargeable = true
                    };

                if (newStandingOrderDTO.DurationStartDate == newStandingOrderDTO.DurationEndDate) // happens for 1-month loans
                    newStandingOrderDTO.DurationEndDate = UberUtil.GetLastDayOfMonth(newStandingOrderDTO.DurationEndDate);

                switch ((InterestCalculationMode)loanProductDTO.LoanInterestCalculationMode)
                {
                    case InterestCalculationMode.StraightLineAmortization:
                    case InterestCalculationMode.DiminishingBalanceAmortization:
                        newStandingOrderDTO.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                        newStandingOrderDTO.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                        newStandingOrderDTO.PaymentPerPeriod = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);
                        break;
                    default:
                        break;
                }

                newStandingOrderDTO.CapitalizedInterest = newStandingOrderDTO.Interest;
                _standingOrderAppService.AddNewStandingOrder(newStandingOrderDTO, serviceHeader);
            }

            #endregion

            #region 4. Do we need to recover upfront dynamic charges from savings?

            var savingsAccountUpfrontDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanProductDTO.Id, (int)DynamicChargeRecoverySource.SavingsAccount, (int)DynamicChargeRecoveryMode.Upfront, totalValue, customerSavingsAccountDTO, serviceHeader, loanProductDTO.LoanRegistrationTermInMonths);

            if (savingsAccountUpfrontDynamicChargeTariffs != null && savingsAccountUpfrontDynamicChargeTariffs.Any())
            {
                savingsAccountUpfrontDynamicChargeTariffs.ForEach(tariff =>
                {
                    var dynamicChargeTariffJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(dynamicChargeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                    journals.Add(dynamicChargeTariffJournal);
                });

                // track deductions
                totalLoanDeductions += savingsAccountUpfrontDynamicChargeTariffs.Sum(x => x.Amount);
            }

            #endregion

            #region 5. Do we need to charge interest upfront?

            if (loanProductDTO.LoanInterestChargeMode == (int)InterestChargeMode.Upfront)
            {
                var repaymentScheduleTotalInterest = repaymentSchedule.Sum(x => x.InterestPayment);

                var minimumTotalInterest = loanProductDTO.LoanRegistrationMinimumInterestAmount * loanProductDTO.LoanRegistrationTermInMonths;

                // do we need to reset?
                var chargeableTotalInterestValue = Math.Max(repaymentScheduleTotalInterest, minimumTotalInterest);

                if (chargeableTotalInterestValue != 0m)
                {
                    // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                    var chargeInterestJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, chargeableTotalInterestValue, "Interest Charged Up-front", secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
                    _journalEntryPostingService.PerformDoubleEntry(chargeInterestJournal, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, customerLoanAccountDTO, customerLoanAccountDTO, serviceHeader);
                    journals.Add(chargeInterestJournal);

                    #region  Do we need to recover interest upfront?

                    if (loanProductDTO.LoanInterestRecoveryMode == (int)InterestRecoveryMode.Upfront)
                    {
                        // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit SavingsProduct.ChartOfAccountId
                        var interestReceivableJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, chargeableTotalInterestValue, "Interest Paid Up-front", secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
                        _journalEntryPostingService.PerformDoubleEntry(interestReceivableJournal, loanProductDTO.InterestReceivableChartOfAccountId, savingsProductDTO.ChartOfAccountId, customerLoanAccountDTO, customerSavingsAccountDTO, serviceHeader);
                        journals.Add(interestReceivableJournal);

                        // Credit LoanProduct.InterestReceivedChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                        var interestReceivedJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, chargeableTotalInterestValue, "Interest Paid Up-front", secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
                        _journalEntryPostingService.PerformDoubleEntry(interestReceivedJournal, loanProductDTO.InterestReceivedChartOfAccountId, loanProductDTO.InterestChargedChartOfAccountId, customerLoanAccountDTO, customerLoanAccountDTO, serviceHeader);
                        journals.Add(interestReceivedJournal);

                        // track deductions
                        totalLoanDeductions += chargeableTotalInterestValue;
                    }

                    #endregion
                }
            }

            #endregion

            #region 6. Do we need to recover deductibles

            var loanProductDeductibles = _loanProductAppService.FindLoanProductDeductibles(loanProductDTO.Id, serviceHeader);

            if (loanProductDeductibles != null && loanProductDeductibles.Any())
            {
                _loanProductAppService.FetchLoanProductDeductiblesProductDescription(loanProductDeductibles, serviceHeader);

                foreach (var deductibleDTO in loanProductDeductibles)
                {
                    if (deductibleDTO.CustomerAccountTypeTargetProductId == savingsProductDTO.Id)
                        continue;

                    CustomerAccountDTO deductibleCustomerAccountDTO = null;

                    #region Find deductible beneficiary account

                    var deductibleCustomerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(deductibleDTO.CustomerAccountTypeTargetProductId, customerLoanAccountDTO.CustomerId, serviceHeader);

                    if (deductibleCustomerAccounts != null && deductibleCustomerAccounts.Any())
                    {
                        _customerAccountAppService.FetchCustomerAccountsProductDescription(deductibleCustomerAccounts, serviceHeader);

                        deductibleCustomerAccountDTO = deductibleCustomerAccounts.First();
                    }
                    else
                    {
                        var customerAccountDTO = new CustomerAccountDTO
                        {
                            BranchId = customerLoanAccountDTO.BranchId,
                            CustomerId = customerLoanAccountDTO.CustomerId,
                            CustomerAccountTypeProductCode = deductibleDTO.CustomerAccountTypeProductCode,
                            CustomerAccountTypeTargetProductId = deductibleDTO.CustomerAccountTypeTargetProductId,
                            CustomerAccountTypeTargetProductCode = deductibleDTO.CustomerAccountTypeTargetProductCode,
                            Status = (int)CustomerAccountStatus.Normal,
                        };

                        customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                        if (customerAccountDTO != null)
                        {
                            deductibleCustomerAccountDTO = customerAccountDTO;

                            _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { deductibleCustomerAccountDTO }, serviceHeader);
                        }
                    }

                    #endregion

                    if (deductibleCustomerAccountDTO != null)
                    {
                        var deductibleAmount = 0m;

                        switch ((ChargeType)deductibleDTO.ChargeType)
                        {
                            case ChargeType.Percentage:
                                deductibleAmount = Math.Round(Convert.ToDecimal((deductibleDTO.ChargePercentage * Convert.ToDouble(totalValue)) / 100), 4, MidpointRounding.AwayFromZero);
                                break;
                            case ChargeType.FixedAmount:
                                deductibleAmount = deductibleDTO.ChargeFixedAmount;
                                break;
                            default:
                                break;
                        }

                        if (deductibleDTO.NetOffInvestmentBalance && deductibleDTO.CustomerAccountTypeProductCode == (int)ProductCode.Investment)
                        {
                            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { deductibleCustomerAccountDTO }, serviceHeader);

                            if (deductibleCustomerAccountDTO.BookBalance * -1 < 0m && deductibleCustomerAccountDTO.BookBalance < deductibleAmount)
                                deductibleAmount -= deductibleCustomerAccountDTO.BookBalance;
                        }

                        if (deductibleAmount * -1 < 0m)
                        {
                            var deductibleJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, deductibleAmount, deductibleDTO.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(deductibleJournal, deductibleCustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, savingsProductDTO.ChartOfAccountId, deductibleCustomerAccountDTO, customerSavingsAccountDTO, serviceHeader);
                            journals.Add(deductibleJournal);

                            // track deductions
                            totalLoanDeductions += deductibleAmount;
                        }
                    }
                }
            }

            #endregion

            settlementAmount = loanProductDTO.LoanRegistrationDisburseMicroLoanLessDeductions ? (totalValue - totalLoanDeductions) : totalValue;

            if ((settlementAmount > 0m /*Is the settlement amount positive?*/))
            {
                #region 7. Post microloan to settlement account

                var settlementJournal = JournalFactory.CreateJournal(disbursementJournal.Id, postingPeriodDTO.Id, customerLoanAccountDTO.BranchId, alternateChannelLogId, settlementAmount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.PesaPepeMicroloan, null, serviceHeader);
                _journalEntryPostingService.PerformDoubleEntry(settlementJournal, settlementChartOfAccountId, savingsProductDTO.ChartOfAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                journals.Add(settlementJournal);

                #endregion

                #region 8. Bulk-Insert journals

                _journalEntryPostingService.BulkSave(serviceHeader, journals);

                #endregion

                #region 9. Do we need to increment loan cycle? >> microcredit loan product

                if (loanProductDTO.LoanRegistrationMicrocredit)
                {
                    var microCreditGroupMember = _microCreditGroupAppService.FindMicroCreditGroupMemberByCustomerId(customerLoanAccountDTO.CustomerId, serviceHeader);

                    if (microCreditGroupMember != null)
                    {
                        microCreditGroupMember.LoanCycle = ++microCreditGroupMember.LoanCycle;

                        _microCreditGroupAppService.UpdateMicroCreditGroupMember(microCreditGroupMember, serviceHeader);
                    }
                }

                #endregion

                #region 10. Do we need to track carry forward dynamic charges from savings?

                var savingsAccountCarryForwardDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(loanProductDTO.Id, (int)DynamicChargeRecoverySource.SavingsAccount, (int)DynamicChargeRecoveryMode.CarryForward, totalValue, customerSavingsAccountDTO, serviceHeader, loanProductDTO.LoanRegistrationTermInMonths);

                if (savingsAccountCarryForwardDynamicChargeTariffs != null && savingsAccountCarryForwardDynamicChargeTariffs.Any())
                {
                    savingsAccountCarryForwardDynamicChargeTariffs.ForEach(tariff =>
                    {
                        var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(customerSavingsAccountDTO.Id, customerLoanAccountDTO.Id, tariff.CreditGLAccountId, tariff.Amount, string.Format("{0}~{1}", tariff.Description, reference));
                        customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                        customerAccountCarryForwards.Add(customerAccountCarryForward);
                    });
                }

                #endregion

                #region 10. Bulk-Insert carry forwards

                _journalEntryPostingService.BulkSave(serviceHeader, null, customerAccountCarryForwards);

                #endregion
            }

            return settlementAmount;
        }

        public List<LoanDisbursementBatchDTO> FindLoanDisbursementBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var loanDisbursementBatches = _loanDisbursementBatchRepository.GetAll(serviceHeader);

                if (loanDisbursementBatches != null && loanDisbursementBatches.Any())
                {
                    return loanDisbursementBatches.ProjectedAsCollection<LoanDisbursementBatchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanDisbursementBatchDTO> FindLoanDisbursementBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanDisbursementBatchSpecifications.LoanDisbursementBatchesWithDateRangeAndStatus(startDate, endDate, status, text);

                ISpecification<LoanDisbursementBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanDisbursementBatchPagedCollection = _loanDisbursementBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanDisbursementBatchPagedCollection != null)
                {
                    var pageCollection = loanDisbursementBatchPagedCollection.PageCollection.ProjectedAsCollection<LoanDisbursementBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _loanDisbursementBatchEntryRepository.AllMatchingCount(LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanDisbursementBatchId(item.Id, null), serviceHeader);

                            var postedItems = _loanDisbursementBatchEntryRepository.AllMatchingCount(LoanDisbursementBatchEntrySpecifications.PostedLoanDisbursementBatchEntryWithLoanDisbursementBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = loanDisbursementBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanDisbursementBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public LoanDisbursementBatchDTO FindLoanDisbursementBatch(Guid loanDisbursementBatchId, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanDisbursementBatch = _loanDisbursementBatchRepository.Get(loanDisbursementBatchId, serviceHeader);

                    if (loanDisbursementBatch != null)
                    {
                        return loanDisbursementBatch.ProjectedAs<LoanDisbursementBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public LoanDisbursementBatchEntryDTO FindLoanDisbursementBatchEntry(Guid loanDisbursementBatchEntryId, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanDisbursementBatchEntry = _loanDisbursementBatchEntryRepository.Get(loanDisbursementBatchEntryId, serviceHeader);

                    if (loanDisbursementBatchEntry != null)
                    {
                        return loanDisbursementBatchEntry.ProjectedAs<LoanDisbursementBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public LoanDisbursementBatchDTO FindCachedLoanDisbursementBatch(Guid loanDisbursementBatchId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<LoanDisbursementBatchDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, loanDisbursementBatchId.ToString("D")), () =>
            {
                return FindLoanDisbursementBatch(loanDisbursementBatchId, serviceHeader);
            });
        }

        public List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(Guid loanDisbursementBatchId, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchId != null && loanDisbursementBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanDisbursementBatchId(loanDisbursementBatchId, null);

                    ISpecification<LoanDisbursementBatchEntry> spec = filter;

                    var loanDisbursementBatchEntries = _loanDisbursementBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (loanDisbursementBatchEntries != null && loanDisbursementBatchEntries.Any())
                    {
                        return loanDisbursementBatchEntries.ProjectedAsCollection<LoanDisbursementBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchId(Guid loanDisbursementBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (loanDisbursementBatchId != null && loanDisbursementBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanDisbursementBatchId(loanDisbursementBatchId, text);

                    ISpecification<LoanDisbursementBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var loanDisbursementBatchPagedCollection = _loanDisbursementBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (loanDisbursementBatchPagedCollection != null)
                    {
                        var pageCollection = loanDisbursementBatchPagedCollection.PageCollection.ProjectedAsCollection<LoanDisbursementBatchEntryDTO>();

                        var itemsCount = loanDisbursementBatchPagedCollection.ItemsCount;

                        return new PageCollectionInfo<LoanDisbursementBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanDisbursementBatchType(int loanDisbursementBatchType, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithDateRangeAndLoanDisbursementBatchType(startDate, endDate, loanDisbursementBatchType, text);

                ISpecification<LoanDisbursementBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanDisbursementBatchPagedCollection = _loanDisbursementBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanDisbursementBatchPagedCollection != null)
                {
                    var pageCollection = loanDisbursementBatchPagedCollection.PageCollection.ProjectedAsCollection<LoanDisbursementBatchEntryDTO>();

                    var itemsCount = loanDisbursementBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanDisbursementBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByCustomerId(int loanDisbursementBatchType, Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanDisbursementBatchTypeAndCustomerId(loanDisbursementBatchType, customerId);

                    ISpecification<LoanDisbursementBatchEntry> spec = filter;

                    var loanDisbursementBatchEntries = _loanDisbursementBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (loanDisbursementBatchEntries != null && loanDisbursementBatchEntries.Any())
                    {
                        return loanDisbursementBatchEntries.ProjectedAsCollection<LoanDisbursementBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<LoanDisbursementBatchEntryDTO> FindQueableLoanDisbursementBatchEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanDisbursementBatchEntrySpecifications.QueableLoanDisbursementBatchEntries();

                ISpecification<LoanDisbursementBatchEntry> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanDisbursementBatchPagedCollection = _loanDisbursementBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanDisbursementBatchPagedCollection != null)
                {
                    var pageCollection = loanDisbursementBatchPagedCollection.PageCollection.ProjectedAsCollection<LoanDisbursementBatchEntryDTO>();

                    var itemsCount = loanDisbursementBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanDisbursementBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool ValidateLoanDisbursementBatchEntriesExceedTransactionThreshold(Guid loanDisbursementBatchId, Guid designationId, int transactionThresholdType, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var transactionThresholdDTOs = _designationAppService.FindTransactionThresholdCollection(designationId, transactionThresholdType, serviceHeader);

            if (transactionThresholdDTOs != null && transactionThresholdDTOs.Any())
            {
                var threshold = transactionThresholdDTOs.First().Threshold;

                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryExceedingThreshold(loanDisbursementBatchId, threshold);

                    ISpecification<LoanDisbursementBatchEntry> spec = filter;

                    var tally = _loanDisbursementBatchEntryRepository.AllMatchingCount(spec, serviceHeader);

                    result = tally > 0;
                }
            }

            return result;
        }

        private bool MarkLoanDisbursementBatchEntryPosted(Guid loanDisbursementBatchEntryId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (loanDisbursementBatchEntryId == null || loanDisbursementBatchEntryId == Guid.Empty)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanDisbursementBatchEntryRepository.Get(loanDisbursementBatchEntryId, serviceHeader);

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
    }
}
