using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.SuperSaverPayableAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class SuperSaverPayableAppService : ISuperSaverPayableAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SuperSaverPayable> _superSaverPayableRepository;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;

        public SuperSaverPayableAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SuperSaverPayable> superSaverPayableRepository,
           IJournalEntryPostingService journalEntryPostingService,
           IPostingPeriodAppService postingPeriodAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           ICustomerAccountAppService customerAccountAppService,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (superSaverPayableRepository == null)
                throw new ArgumentNullException(nameof(superSaverPayableRepository));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _superSaverPayableRepository = superSaverPayableRepository;
            _journalEntryPostingService = journalEntryPostingService;
            _postingPeriodAppService = postingPeriodAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
        }

        public SuperSaverPayableDTO AddNewSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, ServiceHeader serviceHeader)
        {
            if (superSaverPayableDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var superSaverPayable = SuperSaverPayableFactory.CreateSuperSaverPayable(superSaverPayableDTO.CustomerAccountId, superSaverPayableDTO.BranchId, superSaverPayableDTO.BookBalance, superSaverPayableDTO.Amount, superSaverPayableDTO.WithholdingTaxAmount, superSaverPayableDTO.Status, superSaverPayableDTO.Remarks);

                    superSaverPayable.VoucherNumber = _superSaverPayableRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(VoucherNumber),0) + 1 AS Expr1 FROM {0}SuperSaverPayables", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                    superSaverPayable.CreatedBy = serviceHeader.ApplicationUserName;

                    _superSaverPayableRepository.Add(superSaverPayable, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return superSaverPayable.ProjectedAs<SuperSaverPayableDTO>();
                }
            }
            else return null;
        }

        public SuperSaverPayableDTO FindSuperSaverPayable(Guid superSaverPayableId, ServiceHeader serviceHeader)
        {
            if (superSaverPayableId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var superSaverPayable = _superSaverPayableRepository.Get(superSaverPayableId, serviceHeader);

                    if (superSaverPayable != null)
                    {
                        return superSaverPayable.ProjectedAs<SuperSaverPayableDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<SuperSaverPayableDTO> FindSuperSaverPayablesByStatus(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SuperSaverPayableSpecifications.SuperSaverPayableByStatusWithDateRange(status, text, startDate, endDate);

                ISpecification<SuperSaverPayable> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var superSaverPayablePagedCollection = _superSaverPayableRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (superSaverPayablePagedCollection != null)
                {
                    var pageCollection = superSaverPayablePagedCollection.PageCollection.ProjectedAsCollection<SuperSaverPayableDTO>();

                    var itemsCount = superSaverPayablePagedCollection.ItemsCount;

                    return new PageCollectionInfo<SuperSaverPayableDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public bool AuditSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuditOption, ServiceHeader serviceHeader)
        {
            if (superSaverPayableDTO == null || !Enum.IsDefined(typeof(SuperSaverPayableAuditOption), superSaverPayableAuditOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _superSaverPayableRepository.Get(superSaverPayableDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)ProcurementRecordStatus.Pending)
                    return false;

                switch ((SuperSaverPayableAuditOption)superSaverPayableAuditOption)
                {
                    case SuperSaverPayableAuditOption.Verified:

                        persisted.Status = (int)SuperSaverPayableAuditOption.Verified;
                        persisted.AuditRemarks = superSaverPayableDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;

                    case SuperSaverPayableAuditOption.Rejected:

                        persisted.Status = (int)SuperSaverPayableAuditOption.Rejected;
                        persisted.AuditRemarks = superSaverPayableDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        break;
                    default:
                        break;
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuthorizeSuperSaverPayable(SuperSaverPayableDTO superSaverPayableDTO, int superSaverPayableAuthOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (superSaverPayableDTO == null || !Enum.IsDefined(typeof(SuperSaverPayableAuthOption), superSaverPayableAuthOption))
                return result;

            var journals = new List<Journal>();

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _superSaverPayableRepository.Get(superSaverPayableDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)SuperSaverPayableStatus.Verified)
                    return result;

                switch ((SuperSaverPayableAuthOption)superSaverPayableAuthOption)
                {
                    case SuperSaverPayableAuthOption.Posted:

                        var customerSuperSaverInvestmentAccount = new CustomerAccountDTO();

                        var customerSavingsAccount = new CustomerAccountDTO();

                        var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                        var defaultSavingsProduct = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

                        if (postingPeriod != null && defaultSavingsProduct != null)
                        {
                            var superSaverProductDTO = _investmentProductAppService.FindSuperSaverInvestmentProduct(serviceHeader);

                            if (superSaverProductDTO != null)
                            {
                                customerSuperSaverInvestmentAccount = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(superSaverPayableDTO.CustomerAccountCustomerId, superSaverProductDTO.Id, serviceHeader).FirstOrDefault();
                            }
                            else throw new InvalidOperationException("Sorry, super deposit account for customer not found!");

                            var superSaverInterestChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.SuperSaverInterest, serviceHeader);

                            var withholdingTaxChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.SuperSaverWithholdingTax, serviceHeader);

                            if (withholdingTaxChartOfAccountId == Guid.Empty || superSaverInterestChartOfAccountId == Guid.Empty)
                                throw new InvalidOperationException("Sorry, Super Saver Withholding Tax/Super Saver Interest GL mapping not found!");

                            var customerSavingsAccounts = _customerAccountAppService.FindDefaultSavingsProductCustomerAccounts(superSaverPayableDTO.CustomerAccountCustomerId, serviceHeader);

                            if (customerSavingsAccounts == null)
                            {
                                var customerAccountDTO = new CustomerAccountDTO
                                {
                                    BranchId = superSaverPayableDTO.BranchId,
                                    CustomerId = superSaverPayableDTO.CustomerAccountCustomerId,
                                    CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                                    CustomerAccountTypeTargetProductId = defaultSavingsProduct.Id,
                                    CustomerAccountTypeTargetProductCode = defaultSavingsProduct.Code,
                                    Status = (int)CustomerAccountStatus.Normal,
                                    RecordStatus = (int)RecordStatus.Approved,
                                };

                                customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                if (customerAccountDTO != null)
                                    customerSavingsAccount = customerAccountDTO;
                            }
                            else
                                customerSavingsAccount = customerSavingsAccounts.FirstOrDefault();

                            var savingsProductDTO = _savingsProductAppService.FindSavingsProducts(customerSavingsAccount.CustomerAccountTypeProductCode, serviceHeader).FirstOrDefault();

                            var primaryDescription = "Super Saver Liquidation";

                            var secondaryDescription = superSaverPayableDTO.Remarks;

                            var reference = string.Format("{0}", superSaverPayableDTO.PaddedVoucherNumber);

                            var debitCustomerInvestmentAccount = JournalFactory.CreateJournal(null, postingPeriod.Id, superSaverPayableDTO.BranchId, null, superSaverPayableDTO.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.SuperSaverPayables, superSaverPayableDTO.CreatedDate, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(debitCustomerInvestmentAccount, savingsProductDTO.ChartOfAccountId, superSaverProductDTO.ChartOfAccountId, customerSavingsAccount, customerSuperSaverInvestmentAccount, serviceHeader);
                            journals.Add(debitCustomerInvestmentAccount);

                            var creditCustomerAccountJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, superSaverPayableDTO.BranchId, null, superSaverPayableDTO.Amount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.SuperSaverPayables, superSaverPayableDTO.CreatedDate, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(creditCustomerAccountJournal, savingsProductDTO.ChartOfAccountId, superSaverInterestChartOfAccountId, customerSavingsAccount, customerSavingsAccount, serviceHeader);
                            journals.Add(creditCustomerAccountJournal);

                            var debitCustomerAccountJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, superSaverPayableDTO.BranchId, null, superSaverPayableDTO.WithholdingTaxAmount, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.SuperSaverPayables, superSaverPayableDTO.CreatedDate, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(debitCustomerAccountJournal, withholdingTaxChartOfAccountId, savingsProductDTO.ChartOfAccountId, customerSavingsAccount, customerSavingsAccount, serviceHeader);
                            journals.Add(debitCustomerAccountJournal);

                            persisted.Status = (int)SuperSaverPayableStatus.Posted;

                            persisted.AuthorizationRemarks = superSaverPayableDTO.AuthorizationRemarks;

                            persisted.AuthorizedBy = serviceHeader.ApplicationUserName;

                            persisted.AuthorizedDate = DateTime.Now;

                        }
                        else throw new InvalidOperationException("Sorry, but requisite minimum requirements have not been satisfied viz. (posting period / default savings product)");

                        break;

                    case SuperSaverPayableAuthOption.Rejected:

                        persisted.Status = (int)SuperSaverPayableStatus.Rejected;
                        persisted.AuthorizationRemarks = superSaverPayableDTO.AuthorizationRemarks;
                        persisted.AuthorizedBy = serviceHeader.ApplicationUserName;
                        persisted.AuthorizedDate = DateTime.Now;

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
    }
}
