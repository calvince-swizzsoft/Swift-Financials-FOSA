using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchDiscrepancyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchEntryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using KBCsv;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class OverDeductionBatchAppService : IOverDeductionBatchAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<OverDeductionBatch> _overDeductionBatchRepository;
        private readonly IRepository<OverDeductionBatchEntry> _overDeductionBatchEntryRepository;
        private readonly IRepository<OverDeductionBatchDiscrepancy> _overDeductionBatchDiscrepancyRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ICustomerAppService _customerAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;

        public OverDeductionBatchAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<OverDeductionBatch> overDeductionBatchRepository,
           IRepository<OverDeductionBatchEntry> overDeductionBatchEntryRepository,
           IRepository<OverDeductionBatchDiscrepancy> overDeductionBatchDiscrepancyRepository,
           IPostingPeriodAppService postingPeriodAppService,
           ILoanProductAppService loanProductAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ICustomerAccountAppService customerAccountAppService,
           ISqlCommandAppService sqlCommandAppService,
           ICustomerAppService customerAppService,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (overDeductionBatchRepository == null)
                throw new ArgumentNullException(nameof(overDeductionBatchRepository));

            if (overDeductionBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(overDeductionBatchEntryRepository));

            if (overDeductionBatchDiscrepancyRepository == null)
                throw new ArgumentNullException(nameof(overDeductionBatchDiscrepancyRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (customerAppService == null)
                throw new ArgumentNullException(nameof(customerAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _overDeductionBatchRepository = overDeductionBatchRepository;
            _overDeductionBatchEntryRepository = overDeductionBatchEntryRepository;
            _overDeductionBatchDiscrepancyRepository = overDeductionBatchDiscrepancyRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _loanProductAppService = loanProductAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _customerAppService = customerAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
        }

        public OverDeductionBatchDTO AddNewOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var overDeductionBatch = OverDeductionBatchFactory.CreateOverDeductionBatch(overDeductionBatchDTO.BranchId, overDeductionBatchDTO.TotalValue, overDeductionBatchDTO.Reference);

                    overDeductionBatch.BatchNumber = _overDeductionBatchRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(BatchNumber),0) + 1 AS Expr1 FROM {0}OverDeductionBatches", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    overDeductionBatch.Status = (int)BatchStatus.Pending;
                    overDeductionBatch.CreatedBy = serviceHeader.ApplicationUserName;

                    _overDeductionBatchRepository.Add(overDeductionBatch, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return overDeductionBatch.ProjectedAs<OverDeductionBatchDTO>();
                }
            }
            else return null;
        }

        public bool UpdateOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchDTO == null || overDeductionBatchDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _overDeductionBatchRepository.Get(overDeductionBatchDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.TotalValue = overDeductionBatchDTO.TotalValue;

                    dbContextScope.SaveChanges(serviceHeader);

                    return persisted.TotalValue == persisted.OverDeductionBatchEntries.Sum(x => x.Principal + x.Interest);
                }
                else throw new InvalidOperationException("Sorry, but the persisted entity could not be identified!");
            }
        }

        public OverDeductionBatchEntryDTO AddNewOverDeductionBatchEntry(OverDeductionBatchEntryDTO overDeductionBatchEntryDTO, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchEntryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var overDeductionBatchEntry = OverDeductionBatchEntryFactory.CreateOverDeductionBatchEntry(overDeductionBatchEntryDTO.OverDeductionBatchId, overDeductionBatchEntryDTO.DebitCustomerAccountId, overDeductionBatchEntryDTO.CreditCustomerAccountId, overDeductionBatchEntryDTO.Principal, overDeductionBatchEntryDTO.Interest);

                    overDeductionBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                    _overDeductionBatchEntryRepository.Add(overDeductionBatchEntry, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return overDeductionBatchEntry.ProjectedAs<OverDeductionBatchEntryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveOverDeductionBatchEntries(List<OverDeductionBatchEntryDTO> overDeductionBatchEntryDTOs, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchEntryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in overDeductionBatchEntryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _overDeductionBatchEntryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _overDeductionBatchEntryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuditOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _overDeductionBatchRepository.Get(overDeductionBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Pending)
                    return false;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        persisted.Status = (int)BatchStatus.Audited;
                        persisted.AuditRemarks = overDeductionBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuditRemarks = overDeductionBatchDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        _sqlCommandAppService.DeleteOverDeductionBatchDiscrepancies(persisted.Id, serviceHeader);

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeOverDeductionBatch(OverDeductionBatchDTO overDeductionBatchDTO, int batchAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (overDeductionBatchDTO == null || !Enum.IsDefined(typeof(BatchAuthOption), batchAuthOption))
                return result;

            var journals = new List<Journal>();

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _overDeductionBatchRepository.Get(overDeductionBatchDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)BatchStatus.Audited)
                    return result;

                switch ((BatchAuthOption)batchAuthOption)
                {
                    case BatchAuthOption.Post:

                        var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                        if (postingPeriod != null)
                        {
                            overDeductionBatchDTO = FindOverDeductionBatch(persisted.Id, serviceHeader);

                            var overDeductionBatchEntries = FindOverDeductionBatchEntriesByOverDeductionBatchId(overDeductionBatchDTO.Id, serviceHeader);

                            var primaryDescription = "Refund";

                            var secondaryDescription = overDeductionBatchDTO.Reference;

                            var reference = overDeductionBatchDTO.PaddedBatchNumber;

                            overDeductionBatchEntries.ForEach(batchEntry =>
                            {
                                var debitCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(batchEntry.DebitCustomerAccountId, serviceHeader);

                                var creditCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(batchEntry.CreditCustomerAccountId, serviceHeader);

                                _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { debitCustomerAccount, creditCustomerAccount }, serviceHeader);

                                switch ((ProductCode)debitCustomerAccount.CustomerAccountTypeProductCode)
                                {
                                    case ProductCode.Savings:
                                    case ProductCode.Investment:

                                        var batchEntryAmount = batchEntry.Principal + batchEntry.Interest;

                                        // Refund Journal: OverDeduction OverDeductionCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, Debit DebitCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId
                                        var refundJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, overDeductionBatchDTO.BranchId, null, batchEntryAmount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.OverDeductionBatch, null, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(refundJournal, creditCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, debitCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, creditCustomerAccount, debitCustomerAccount, serviceHeader);
                                        journals.Add(refundJournal);

                                        break;
                                    case ProductCode.Loan:

                                        var loanProduct = _loanProductAppService.FindCachedLoanProduct(debitCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                                        // Credit OverDeductionCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                                        var interestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, overDeductionBatchDTO.BranchId, null, batchEntry.Interest, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.OverDeductionBatch, null, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(interestReceivableJournal, creditCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, loanProduct.InterestReceivableChartOfAccountId, creditCustomerAccount, debitCustomerAccount, serviceHeader);
                                        journals.Add(interestReceivableJournal);

                                        // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivedChartOfAccountId
                                        var interestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, overDeductionBatchDTO.BranchId, null, batchEntry.Interest, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.OverDeductionBatch, null, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(interestReceivedJournal, loanProduct.InterestChargedChartOfAccountId, loanProduct.InterestReceivedChartOfAccountId, debitCustomerAccount, debitCustomerAccount, serviceHeader);
                                        journals.Add(interestReceivedJournal);

                                        // Credit OverDeductionCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, Debit LoanProduct.ChartOfAccountId
                                        var principalJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, overDeductionBatchDTO.BranchId, null, batchEntry.Principal, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.OverDeductionBatch, null, serviceHeader);
                                        _journalEntryPostingService.PerformDoubleEntry(principalJournal, creditCustomerAccount.CustomerAccountTypeTargetProductChartOfAccountId, loanProduct.ChartOfAccountId, creditCustomerAccount, debitCustomerAccount, serviceHeader);
                                        journals.Add(principalJournal);

                                        break;
                                    default:
                                        break;
                                }

                                var persistedBatchEntry = _overDeductionBatchEntryRepository.Get(batchEntry.Id, serviceHeader);

                                if (persistedBatchEntry != null)
                                {
                                    persistedBatchEntry.Status = (int)BatchEntryStatus.Posted;
                                }
                            });

                            persisted.Status = (int)BatchStatus.Posted;
                            persisted.AuthorizationRemarks = overDeductionBatchDTO.AuthorizationRemarks;
                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                            persisted.AuthorizedDate = DateTime.Now;
                        }
                        else throw new InvalidOperationException("Sorry, but requisite minimum requirements have not been satisfied viz. (posting period)");

                        break;
                    case BatchAuthOption.Reject:

                        persisted.Status = (int)BatchStatus.Rejected;
                        persisted.AuthorizationRemarks = overDeductionBatchDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

                        _sqlCommandAppService.DeleteOverDeductionBatchDiscrepancies(persisted.Id, serviceHeader);

                        break;
                    default:
                        break;
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
            }

            if (result && journals.Any())
            {
                result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
            }

            return result;
        }

        public List<BatchImportEntryWrapper> ParseOverDeductionBatchImport(Guid overDeductionBatchId, string fileUploadDirectory, string fileName, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var persisted = _overDeductionBatchRepository.Get(overDeductionBatchId, serviceHeader);

                if (persisted != null && persisted.Status == (int)BatchStatus.Pending && !string.IsNullOrWhiteSpace(fileUploadDirectory) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var path = Path.Combine(fileUploadDirectory, fileName);

                    if (File.Exists(path))
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

                                if (dataRecord.Count == 12)
                                {
                                    var refundEntry = new BatchImportEntryWrapper
                                    {
                                        Column1 = dataRecord[0], // Debit Customer Serial Number
                                        Column2 = dataRecord[1], // Debit Customer Full Name
                                        Column3 = dataRecord[2], // Debit Customer Account Type Product Code >> Investment/Loan 
                                        Column4 = dataRecord[3], // Debit Customer Account Type Target Product Code
                                        Column5 = dataRecord[4], // Debit Customer Account Type Target Product Name
                                        Column6 = dataRecord[5], // Credit Customer Serial Number
                                        Column7 = dataRecord[6], // Credit Customer Full Name
                                        Column8 = dataRecord[7], // Credit Customer Account Type Product Code >> Savings/Investment/Loan 
                                        Column9 = dataRecord[8], // Credit Customer Account Type Target Product Code                                      
                                        Column10 = dataRecord[9], // Credit Customer Account Type Target Product Name                                        
                                        Column11 = dataRecord[10], // Amount
                                        Column12 = dataRecord[11], //Interest
                                    };

                                    importEntries.Add(refundEntry);
                                }
                            }
                        }

                        if (importEntries.Any())
                        {
                            BatchImportParseInfo parseInfo = null;

                            parseInfo = ParseRefunds(importEntries, serviceHeader);

                            if (parseInfo != null)
                            {
                                UpdateOverDeductionBatchEntries(overDeductionBatchId, parseInfo.MatchedCollection8, serviceHeader);

                                var discrepancies = new List<OverDeductionBatchDiscrepancyDTO>();

                                foreach (var item in parseInfo.MismatchedCollection)
                                {
                                    discrepancies.Add(new OverDeductionBatchDiscrepancyDTO
                                    {
                                        Column1 = item.Column1,
                                        Column2 = item.Column2,
                                        Column3 = item.Column3,
                                        Column4 = item.Column4,
                                        Column5 = item.Column5,
                                        Column6 = item.Column6,
                                        Column7 = item.Column7,
                                        Column8 = item.Column8,
                                        Column9 = item.Column9,
                                        Column10 = item.Column10,
                                        Column11 = item.Column11,
                                        Column12 = item.Column12,
                                        Remarks = item.Remarks,
                                    });
                                }

                                UpdateOverDeductionBatchDiscrepancies(overDeductionBatchId, discrepancies, serviceHeader);

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

        private BatchImportParseInfo ParseRefunds(List<BatchImportEntryWrapper> importEntries, ServiceHeader serviceHeader)
        {
            var result = new BatchImportParseInfo
            {
                MatchedCollection8 = new List<OverDeductionBatchEntryDTO> { },
                MismatchedCollection = new List<BatchImportEntryWrapper> { }
            };

            var count = 0;

            importEntries.ForEach(item =>
           {
               var debitCustomerSerialNumber = default(int);
               var debitCustomerAccountTypeProductCode = default(int);
               var debitCustomerAccountTypeTargetProductCode = default(int);
               var creditCustomerSerialNumber = default(int);
               var creditCustomerAccountTypeProductCode = default(int);
               var creditCustomerAccountTypeTargetProductCode = default(int);
               var amount = default(decimal);
               var interest = default(decimal);

               if ((int.TryParse(item.Column1, NumberStyles.Any, CultureInfo.InvariantCulture, out debitCustomerSerialNumber))
               && (int.TryParse(item.Column3, NumberStyles.Any, CultureInfo.InvariantCulture, out debitCustomerAccountTypeProductCode))
              && (int.TryParse(item.Column4, NumberStyles.Any, CultureInfo.InvariantCulture, out debitCustomerAccountTypeTargetProductCode))
              && (int.TryParse(item.Column6, NumberStyles.Any, CultureInfo.InvariantCulture, out creditCustomerSerialNumber))
              && (int.TryParse(item.Column8, NumberStyles.Any, CultureInfo.InvariantCulture, out creditCustomerAccountTypeProductCode))
              && (int.TryParse(item.Column9, NumberStyles.Any, CultureInfo.InvariantCulture, out creditCustomerAccountTypeTargetProductCode))
              && (decimal.TryParse(item.Column11, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
              && (decimal.TryParse(item.Column12, NumberStyles.Any, CultureInfo.InvariantCulture, out interest)))
               {
                   if ((amount + interest) > 0m && amount >= 0m && interest >= 0m)
                   {
                       var debitCustomerDTO = _customerAppService.FindCustomerBySerialNumber(debitCustomerSerialNumber, serviceHeader).FirstOrDefault();

                       var creditCustomerDTO = _customerAppService.FindCustomerBySerialNumber(creditCustomerSerialNumber, serviceHeader).FirstOrDefault();

                       if (debitCustomerDTO != null && creditCustomerDTO != null)
                       {
                           var loanProductDTO = new LoanProductDTO();
                           var investmentProductDTO = new InvestmentProductDTO();
                           var debitCustomerAccountDTOs = new List<CustomerAccountDTO>();
                           var creditCustomerAccountDTOs = new List<CustomerAccountDTO>();

                           #region debit customer account

                           switch ((ProductCode)debitCustomerAccountTypeProductCode)
                           {
                               case ProductCode.Loan:

                                   loanProductDTO = _loanProductAppService.FindLoanProducts(debitCustomerAccountTypeTargetProductCode, serviceHeader).SingleOrDefault();

                                   if (loanProductDTO != null)
                                   {
                                       debitCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(debitCustomerDTO.Id, loanProductDTO.Id, serviceHeader);
                                   }
                                   else
                                   {
                                       item.Remarks = string.Format("Record #{0} ~ unable to parse, either of the debit target product could not be found.", count);

                                       result.MismatchedCollection.Add(item);
                                   }
                                   break;

                               case ProductCode.Investment:

                                   investmentProductDTO = _investmentProductAppService.FindInvestmentProducts(debitCustomerAccountTypeTargetProductCode, serviceHeader).SingleOrDefault();

                                   if (investmentProductDTO != null)
                                   {
                                       debitCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(debitCustomerDTO.Id, investmentProductDTO.Id, serviceHeader);
                                   }
                                   else
                                   {
                                       item.Remarks = string.Format("Record #{0} ~ unable to parse, either of the debit target product could not be found.", count);

                                       result.MismatchedCollection.Add(item);
                                   }
                                   break;

                               default:

                                   item.Remarks = string.Format("Record #{0} ~ unable to parse, debit customer account type target product code is invalid.", count);

                                   result.MismatchedCollection.Add(item);

                                   break;
                           }

                           #endregion

                           #region credit customer account

                           switch ((ProductCode)creditCustomerAccountTypeProductCode)
                           {
                               case ProductCode.Savings:

                                   var creditSavingsProductDTO = _savingsProductAppService.FindSavingsProducts(creditCustomerAccountTypeTargetProductCode, serviceHeader).SingleOrDefault();

                                   if (creditSavingsProductDTO != null)
                                   {
                                       creditCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(creditCustomerDTO.Id, creditSavingsProductDTO.Id, serviceHeader);
                                   }
                                   else
                                   {
                                       item.Remarks = string.Format("Record #{0} ~ unable to parse, either of the credit target product could not be found.", count);

                                       result.MismatchedCollection.Add(item);
                                   }
                                   break;

                               case ProductCode.Investment:

                                   var creditInvestmentProductDTO = _investmentProductAppService.FindInvestmentProducts(creditCustomerAccountTypeTargetProductCode, serviceHeader).SingleOrDefault();

                                   if (creditInvestmentProductDTO != null)
                                   {
                                       creditCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(creditCustomerDTO.Id, creditInvestmentProductDTO.Id, serviceHeader);
                                   }
                                   else
                                   {
                                       item.Remarks = string.Format("Record #{0} ~ unable to parse, either of the credit target product could not be found.", count);

                                       result.MismatchedCollection.Add(item);
                                   }
                                   break;

                               case ProductCode.Loan:

                                   var creditLoanProductDTO = _loanProductAppService.FindLoanProducts(creditCustomerAccountTypeTargetProductCode, serviceHeader).SingleOrDefault();

                                   if (creditLoanProductDTO != null)
                                   {
                                       creditCustomerAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(creditCustomerDTO.Id, creditLoanProductDTO.Id, serviceHeader);
                                   }
                                   else
                                   {
                                       item.Remarks = string.Format("Record #{0} ~ unable to parse, either of the credit target product could not be found.", count);

                                       result.MismatchedCollection.Add(item);
                                   }
                                   break;

                               default:

                                   item.Remarks = string.Format("Record #{0} ~ unable to parse, credit customer account type target product code is invalid.", count);

                                   result.MismatchedCollection.Add(item);

                                   break;
                           }
                           #endregion

                           if (creditCustomerAccountDTOs != null && debitCustomerAccountDTOs != null)
                           {
                               var creditCustomerAccount = creditCustomerAccountDTOs[0];

                               var debitCustomerAccount = debitCustomerAccountDTOs[0];

                               var overDeductionBatchEntry = new OverDeductionBatchEntryDTO()
                               {
                                   CreditCustomerAccountId = creditCustomerAccount.Id,
                                   DebitCustomerAccountId = debitCustomerAccount.Id,
                                   Principal = amount,
                                   Interest = interest,
                                   Status = (int)BatchStatus.Pending,
                               };

                               result.MatchedCollection8.Add(overDeductionBatchEntry);
                           }
                           else
                           {
                               item.Remarks = string.Format("Record #{0} ~ unable to parse, either of the debit/ credit customer account could not be found.", count);

                               result.MismatchedCollection.Add(item);
                           }

                       }
                       else
                       {
                           item.Remarks = string.Format("Record #{0} ~ unable to parse, there is no customer matches by serial number {1}", count, debitCustomerSerialNumber);

                           result.MismatchedCollection.Add(item);
                       }
                   }
                   else
                   {
                       item.Remarks = string.Format("Record #{0} ~ unable to parse, either sum of amount and interest is less or equal to zero.", count);

                       result.MismatchedCollection.Add(item);
                   }
               }
               else
               {
                   item.Remarks = string.Format("Record #{0} ~ unable to parse serial number/code/amount/interest.", count);

                   result.MismatchedCollection.Add(item);
               }
               // tally
               count += 1;
           });

            return result;
        }

        private bool UpdateOverDeductionBatchDiscrepancies(Guid overDeductionBatchId, List<OverDeductionBatchDiscrepancyDTO> overDeductionBatchDiscrepancies, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (overDeductionBatchId != null && overDeductionBatchDiscrepancies != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _overDeductionBatchRepository.Get(overDeductionBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        _sqlCommandAppService.DeleteOverDeductionBatchDiscrepancies(persisted.Id, serviceHeader);

                        if (overDeductionBatchDiscrepancies.Any())
                        {
                            List<OverDeductionBatchDiscrepancy> batchDiscrepancies = new List<OverDeductionBatchDiscrepancy>();

                            foreach (var item in overDeductionBatchDiscrepancies)
                            {
                                var overDeductionBatchDiscrepancy = OverDeductionBatchDiscrepancyFactory.CreateOverDeductionBatchDiscrepancy(persisted.Id, item.Column1, item.Column2, item.Column3, item.Column4, item.Column5, item.Column6, item.Column7, item.Column8, item.Column9, item.Column10, item.Column11, item.Column12, item.Remarks);

                                overDeductionBatchDiscrepancy.Status = (int)BatchEntryStatus.Pending;

                                overDeductionBatchDiscrepancy.CreatedBy = serviceHeader.ApplicationUserName;

                                batchDiscrepancies.Add(overDeductionBatchDiscrepancy);
                            }

                            if (batchDiscrepancies.Any())
                            {
                                var bcpBatchEntries = new List<OverDeductionBatchDiscrepancyBulkCopyDTO>();

                                batchDiscrepancies.ForEach(c =>
                                {
                                    OverDeductionBatchDiscrepancyBulkCopyDTO bcpc = new OverDeductionBatchDiscrepancyBulkCopyDTO
                                    {
                                        Id = c.Id,
                                        OverDeductionBatchId = c.OverDeductionBatchId,
                                        Column1 = c.Column1,
                                        Column2 = c.Column2,
                                        Column3 = c.Column3,
                                        Column4 = c.Column4,
                                        Column5 = c.Column5,
                                        Column6 = c.Column6,
                                        Column7 = c.Column7,
                                        Column8 = c.Column8,
                                        Column9 = c.Column9,
                                        Column10 = c.Column10,
                                        Remarks = c.Remarks,
                                        Status = c.Status,
                                        PostedBy = c.PostedBy,
                                        PostedDate = c.PostedDate,
                                        CreatedBy = c.CreatedBy,
                                        CreatedDate = c.CreatedDate,
                                    };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _overDeductionBatchDiscrepancyRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private bool UpdateOverDeductionBatchEntries(Guid overDeductionBatchId, List<OverDeductionBatchEntryDTO> overDeductionBatchEntries, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (overDeductionBatchId != null && overDeductionBatchEntries != null && overDeductionBatchEntries.Any())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var persisted = _overDeductionBatchRepository.Get(overDeductionBatchId, serviceHeader);

                    if (persisted != null)
                    {
                        _sqlCommandAppService.DeleteOverDeductionBatchEntries(persisted.Id, serviceHeader);

                        if (overDeductionBatchEntries.Any())
                        {
                            List<OverDeductionBatchEntry> batchEntries = new List<OverDeductionBatchEntry>();

                            foreach (var item in overDeductionBatchEntries)
                            {
                                var overDeductionBatchEntry = OverDeductionBatchEntryFactory.CreateOverDeductionBatchEntry(overDeductionBatchId, item.DebitCustomerAccountId, item.CreditCustomerAccountId, item.Principal, item.Interest);

                                overDeductionBatchEntry.Status = (int)BatchEntryStatus.Pending;
                                overDeductionBatchEntry.CreatedBy = serviceHeader.ApplicationUserName;

                                batchEntries.Add(overDeductionBatchEntry);
                            }

                            if (batchEntries.Any())
                            {
                                var bcpBatchEntries = new List<OverDeductionBatchEntryBulkCopyDTO>();

                                batchEntries.ForEach(c =>
                                {
                                    OverDeductionBatchEntryBulkCopyDTO bcpc =
                                        new OverDeductionBatchEntryBulkCopyDTO
                                        {
                                            Id = c.Id,
                                            OverDeductionBatchId = c.OverDeductionBatchId,
                                            CreditCustomerAccountId = c.CreditCustomerAccountId,
                                            DebitCustomerAccountId = c.DebitCustomerAccountId,
                                            Principal = c.Principal,
                                            Interest = c.Interest,
                                            Status = c.Status,
                                            CreatedBy = c.CreatedBy,
                                            CreatedDate = c.CreatedDate,
                                        };

                                    bcpBatchEntries.Add(bcpc);
                                });

                                result = _sqlCommandAppService.BulkInsert(string.Format("{0}{1}", DefaultSettings.Instance.TablePrefix, _overDeductionBatchEntryRepository.Pluralize()), bcpBatchEntries, serviceHeader);
                            }
                        }
                    }
                }
            }

            return result;
        }


        public List<OverDeductionBatchDTO> FindOverDeductionBatches(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var overDeductionBatchs = _overDeductionBatchRepository.GetAll(serviceHeader);

                if (overDeductionBatchs != null && overDeductionBatchs.Any())
                {
                    return overDeductionBatchs.ProjectedAsCollection<OverDeductionBatchDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<OverDeductionBatchDTO> FindOverDeductionBatches(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = OverDeductionBatchSpecifications.OverDeductionBatchWithStatus(status, startDate, endDate, text);

                ISpecification<OverDeductionBatch> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var overDeductionBatchPagedCollection = _overDeductionBatchRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (overDeductionBatchPagedCollection != null)
                {
                    var pageCollection = overDeductionBatchPagedCollection.PageCollection.ProjectedAsCollection<OverDeductionBatchDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            var totalItems = _overDeductionBatchEntryRepository.AllMatchingCount(OverDeductionBatchEntrySpecifications.OverDeductionBatchEntryWithOverDeductionBatchId(item.Id, null), serviceHeader);

                            var postedItems = _overDeductionBatchEntryRepository.AllMatchingCount(OverDeductionBatchEntrySpecifications.PostedOverDeductionBatchEntryWithOverDeductionBatchId(item.Id), serviceHeader);

                            item.PostedEntries = string.Format("{0}/{1}", postedItems, totalItems);
                        }
                    }

                    var itemsCount = overDeductionBatchPagedCollection.ItemsCount;

                    return new PageCollectionInfo<OverDeductionBatchDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public OverDeductionBatchDTO FindOverDeductionBatch(Guid overDeductionBatchId, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var overDeductionBatch = _overDeductionBatchRepository.Get(overDeductionBatchId, serviceHeader);

                    if (overDeductionBatch != null)
                    {
                        return overDeductionBatch.ProjectedAs<OverDeductionBatchDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchId(Guid overDeductionBatchId, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchId != null && overDeductionBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = OverDeductionBatchEntrySpecifications.OverDeductionBatchEntryWithOverDeductionBatchId(overDeductionBatchId, null);

                    ISpecification<OverDeductionBatchEntry> spec = filter;

                    var overDeductionBatchEntries = _overDeductionBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (overDeductionBatchEntries != null && overDeductionBatchEntries.Any())
                    {
                        return overDeductionBatchEntries.ProjectedAsCollection<OverDeductionBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<OverDeductionBatchEntryDTO> FindOverDeductionBatchEntriesByOverDeductionBatchId(Guid overDeductionBatchId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (overDeductionBatchId != null && overDeductionBatchId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = OverDeductionBatchEntrySpecifications.OverDeductionBatchEntryWithOverDeductionBatchId(overDeductionBatchId, text);

                    ISpecification<OverDeductionBatchEntry> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var overDeductionBatchEntryPagedCollection = _overDeductionBatchEntryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (overDeductionBatchEntryPagedCollection != null)
                    {
                        var persisted = _overDeductionBatchRepository.Get(overDeductionBatchId, serviceHeader);

                        var persistedEntriesTotal = 0m;

                        var overDeductionBatchEntries = _overDeductionBatchEntryRepository.AllMatching(OverDeductionBatchEntrySpecifications.OverDeductionBatchEntryWithOverDeductionBatchId(persisted.Id, null), serviceHeader);

                        if (overDeductionBatchEntries != null && overDeductionBatchEntries.Any())
                            persistedEntriesTotal = overDeductionBatchEntries.Sum(x => x.Principal + x.Interest);

                        var pageCollection = overDeductionBatchEntryPagedCollection.PageCollection.ProjectedAsCollection<OverDeductionBatchEntryDTO>();

                        var itemsCount = overDeductionBatchEntryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<OverDeductionBatchEntryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount, TotalApportioned = persistedEntriesTotal, TotalShortage = persisted.TotalValue - persistedEntriesTotal };
                    }
                    else return null;
                }
            }
            else return null;
        }

    }
}
