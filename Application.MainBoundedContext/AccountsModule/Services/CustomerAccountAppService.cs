using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardInstallmentAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountHistoryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountSignatoryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class CustomerAccountAppService : ICustomerAccountAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CustomerAccount> _customerAccountRepository;
        private readonly IRepository<CustomerAccountHistory> _customerAccountHistoryRepository;
        private readonly IRepository<CustomerAccountSignatory> _customerAccountSignatoryRepository;
        private readonly IRepository<CustomerAccountCarryForward> _customerAccountCarryForwardRepository;
        private readonly IRepository<CustomerAccountCarryForwardInstallment> _customerAccountCarryForwardInstallmentRepository;
        private readonly IRepository<CustomerAccountArrearage> _customerAccountArrearageRepository;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IAppCache _appCache;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IBrokerService _brokerService;

        public CustomerAccountAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<CustomerAccount> customerAccountRepository,
            IRepository<CustomerAccountHistory> customerAccountHistoryRepository,
            IRepository<CustomerAccountSignatory> customerAccountSignatoryRepository,
            IRepository<CustomerAccountCarryForward> customerAccountCarryForwardRepository,
            IRepository<CustomerAccountCarryForwardInstallment> customerAccountCarryForwardInstallmentRepository,
            IRepository<CustomerAccountArrearage> customerAccountArrearageRepository,
            ISavingsProductAppService savingsProductAppService,
            IInvestmentProductAppService investmentProductAppService,
            ILoanProductAppService loanProductAppService,
            ISqlCommandAppService sqlCommandAppService,
            IAppCache appCache,
            IPostingPeriodAppService postingPeriodAppService,
            IJournalEntryPostingService journalEntryPostingService,
            IChartOfAccountAppService chartOfAccountAppService,
            ICommissionAppService commissionAppService,
            IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (customerAccountRepository == null)
                throw new ArgumentNullException(nameof(customerAccountRepository));

            if (customerAccountHistoryRepository == null)
                throw new ArgumentNullException(nameof(customerAccountHistoryRepository));

            if (customerAccountSignatoryRepository == null)
                throw new ArgumentNullException(nameof(customerAccountSignatoryRepository));

            if (customerAccountCarryForwardRepository == null)
                throw new ArgumentNullException(nameof(customerAccountCarryForwardRepository));

            if (customerAccountCarryForwardInstallmentRepository == null)
                throw new ArgumentNullException(nameof(customerAccountCarryForwardInstallmentRepository));

            if (customerAccountArrearageRepository == null)
                throw new ArgumentNullException(nameof(customerAccountArrearageRepository));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _customerAccountRepository = customerAccountRepository;
            _customerAccountHistoryRepository = customerAccountHistoryRepository;
            _customerAccountSignatoryRepository = customerAccountSignatoryRepository;
            _customerAccountCarryForwardRepository = customerAccountCarryForwardRepository;
            _customerAccountCarryForwardInstallmentRepository = customerAccountCarryForwardInstallmentRepository;
            _customerAccountArrearageRepository = customerAccountArrearageRepository;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _appCache = appCache;
            _postingPeriodAppService = postingPeriodAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _commissionAppService = commissionAppService;
            _brokerService = brokerService;
        }

        public CustomerAccountDTO AddNewCustomerAccount(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountDTO != null && customerAccountDTO.CustomerId != Guid.Empty && customerAccountDTO.BranchId != Guid.Empty)
            {
                var matchedCustomerAccounts = FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (matchedCustomerAccounts != null && matchedCustomerAccounts.Any())
                {
                    customerAccountDTO.ErrorMessageResult = string.Format("Sorry, but customer already has an account for the selected product please choose another product!"+"  "+customerAccountDTO.CustomerAccountTypeTargetProductDescription);
                    return customerAccountDTO;
                }
                else
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create(DbContextScopeOption.ForceCreateNew))
                    {
                        var customerAccountType = new CustomerAccountType(customerAccountDTO.CustomerAccountTypeProductCode, customerAccountDTO.CustomerAccountTypeTargetProductId, customerAccountDTO.CustomerAccountTypeTargetProductCode);

                        var customerAccount = CustomerAccountFactory.CreateCustomerAccount(customerAccountDTO.CustomerId, customerAccountDTO.BranchId, customerAccountType);

                        customerAccount.Status = (byte)customerAccountDTO.Status;

                        switch ((ProductCode)customerAccountType.ProductCode)
                        {
                            case ProductCode.Loan:
                            case ProductCode.Investment:
                                customerAccount.RecordStatus = (int)RecordStatus.Approved;
                                break;
                            case ProductCode.Savings:
                            default:
                                customerAccount.RecordStatus = (byte)customerAccountDTO.RecordStatus;
                                break;
                        }

                        customerAccount.CreatedBy = serviceHeader.ApplicationUserName;

                        _customerAccountRepository.Add(customerAccount, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return customerAccount.ProjectedAs<CustomerAccountDTO>();
                    }
                }
            }
            else return null;
        }

        public bool AddNewCustomerAccounts(CustomerDTO customerDTO, List<SavingsProductDTO> savingsProducts, List<InvestmentProductDTO> investmentProducts, List<LoanProductDTO> loanProducts, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (customerDTO != null && customerDTO.BranchId != Guid.Empty)
            {
                var customerAccoountDTOs = new List<CustomerAccountDTO>();

                #region savings

                if (savingsProducts != null && savingsProducts.Any())
                {
                    foreach (var item in savingsProducts)
                    {
                        var customerAccountDTO = new CustomerAccountDTO
                        {
                            CustomerId = customerDTO.Id,
                            BranchId = customerDTO.BranchId,
                            CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                            CustomerAccountTypeTargetProductId = item.Id,
                            CustomerAccountTypeTargetProductCode = item.Code,
                            Status = (int)CustomerAccountStatus.Normal,
                        };

                        customerAccoountDTOs.Add(customerAccountDTO);
                    }
                }

                #endregion

                #region investments

                if (investmentProducts != null && investmentProducts.Any())
                {
                    foreach (var item in investmentProducts)
                    {
                        var customerAccountDTO = new CustomerAccountDTO
                        {
                            CustomerId = customerDTO.Id,
                            BranchId = customerDTO.BranchId,
                            CustomerAccountTypeProductCode = (int)ProductCode.Investment,
                            CustomerAccountTypeTargetProductId = item.Id,
                            CustomerAccountTypeTargetProductCode = item.Code,
                            Status = (int)CustomerAccountStatus.Normal,
                        };

                        customerAccoountDTOs.Add(customerAccountDTO);
                    }
                }

                #endregion

                #region loans

                if (loanProducts != null && loanProducts.Any())
                {
                    foreach (var item in loanProducts)
                    {
                        var customerAccountDTO = new CustomerAccountDTO
                        {
                            CustomerId = customerDTO.Id,
                            BranchId = customerDTO.BranchId,
                            CustomerAccountTypeProductCode = (int)ProductCode.Loan,
                            CustomerAccountTypeTargetProductId = item.Id,
                            CustomerAccountTypeTargetProductCode = item.Code,
                            Status = (int)CustomerAccountStatus.Normal,
                        };

                        customerAccoountDTOs.Add(customerAccountDTO);
                    }
                }

                #endregion

                if (customerAccoountDTOs.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in customerAccoountDTOs)
                        {
                            var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerIdAndCustomerAccountTypeTargetProductId(item.CustomerId, item.CustomerAccountTypeTargetProductId);

                            ISpecification<CustomerAccount> spec = filter;

                            if (_customerAccountRepository.AllMatchingCount(spec, serviceHeader) == 0)
                            {
                                var customerAccountType = new CustomerAccountType(item.CustomerAccountTypeProductCode, item.CustomerAccountTypeTargetProductId, item.CustomerAccountTypeTargetProductCode);

                                var customerAccount = CustomerAccountFactory.CreateCustomerAccount(item.CustomerId, item.BranchId, customerAccountType);

                                customerAccount.Status = (byte)item.Status;

                                switch ((ProductCode)customerAccountType.ProductCode)
                                {
                                    case ProductCode.Loan:
                                    case ProductCode.Investment:
                                        customerAccount.RecordStatus = (int)RecordStatus.Approved;
                                        break;
                                    case ProductCode.Savings:
                                    default:
                                        customerAccount.RecordStatus = (byte)item.RecordStatus;
                                        break;
                                }

                                customerAccount.CreatedBy = serviceHeader.ApplicationUserName;

                                _customerAccountRepository.Add(customerAccount, serviceHeader);
                            }
                        }

                        result = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
            }

            return result;
        }

        public bool UpdateCustomerAccount(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountDTO == null || customerAccountDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _customerAccountRepository.Get(customerAccountDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.Remarks = customerAccountDTO.Remarks;
                    persisted.RecordStatus = (byte)customerAccountDTO.RecordStatus;
                    persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                    persisted.ModifiedDate = DateTime.Now;

                    return dbContextScope.SaveChanges(serviceHeader) > 0;
                }
                else return false;
            }
        }

        public bool ManageCustomerAccount(Guid customerAccountId, int managementAction, string remarks, int remarkType, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            switch ((CustomerAccountManagementAction)managementAction)
            {
                case CustomerAccountManagementAction.Activation:
                    result = Activate(customerAccountId, remarks, remarkType, serviceHeader);
                    break;
                case CustomerAccountManagementAction.Deactivation:
                    result = Deactivate(customerAccountId, remarks, remarkType, serviceHeader);

                    if (result)
                    {
                        #region Do we need to send alerts?

                        var customerAccountDTO = FindCustomerAccountDTO(customerAccountId, serviceHeader);

                        _brokerService.ProcessFrozenAccountAlerts(DMLCommand.None, serviceHeader, customerAccountDTO);

                        #endregion
                    }
                    break;
                case CustomerAccountManagementAction.Remark:
                    result = Remark(customerAccountId, remarks, remarkType, serviceHeader);
                    break;
                case CustomerAccountManagementAction.Closure:
                    result = Close(customerAccountId, remarks, remarkType, serviceHeader);
                    break;
                case CustomerAccountManagementAction.SigningInstructions:
                    result = SigningInstructions(customerAccountId, remarks, remarkType, serviceHeader);
                    break;
                default:
                    break;
            }

            return result;
        }

        public bool UpdateCustomerAccountsBranch(List<CustomerAccountDTO> customerAccountDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                customerAccountDTOs.ForEach(item =>
                {
                    var persisted = _customerAccountRepository.Get(item.Id, serviceHeader);

                    if (persisted != null)
                    {
                        persisted.BranchId = item.BranchId;
                    }
                });

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public List<CustomerAccountDTO> FindCustomerAccounts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountSpecifications.DefaultSpec();

                ISpecification<CustomerAccount> spec = filter;

                var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                if (customerAccounts != null && customerAccounts.Any())
                {
                    return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>();
                }
                else return null;
            }
        }

        public List<CustomerAccountSummaryDTO> FindCustomerAccounts(Guid[] customerAccountIds, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountSpecifications.CustomerAccountWithId(customerAccountIds);

                ISpecification<CustomerAccount> spec = filter;

                return _customerAccountRepository.AllMatching<CustomerAccountSummaryDTO>(spec, serviceHeader);
            }
        }


        public CustomerAccountDTO FindCustomerAccounts(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _customerAccountRepository.Get<CustomerAccountDTO>(customerAccountId, serviceHeader);
            }
        }

        public async Task<List<CustomerAccountSummaryDTO>> FindCustomerAccountsAsync(Guid[] customerAccountIds, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountSpecifications.CustomerAccountWithId(customerAccountIds);

                ISpecification<CustomerAccount> spec = filter;

                return await _customerAccountRepository.AllMatchingAsync<CustomerAccountSummaryDTO>(spec, serviceHeader);
            }
        }

        public List<CustomerAccountDTO> FindDefaultSavingsProductCustomerAccounts(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                var defaultSavingsProduct = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

                if (defaultSavingsProduct != null)
                {
                    return FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerId, defaultSavingsProduct.Id, serviceHeader);
                }
                else return null;
            }
            else return null;
        }

        public CustomerAccountDTO FindCustomerAccountDTO(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var customerAccount = _customerAccountRepository.Get(customerAccountId, serviceHeader);

                    if (customerAccount != null)
                    {
                        return customerAccount.ProjectedAs<CustomerAccountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public CustomerAccountDTO FindCustomerAccountDTO(string fullAccountNumber, ServiceHeader serviceHeader)
        {
            if (!string.IsNullOrWhiteSpace(fullAccountNumber))
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountFullAccountNumber(fullAccountNumber);

                    ISpecification<CustomerAccount> spec = filter;

                    var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>()[0];
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccounts(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountSpecifications.DefaultSpec();

                ISpecification<CustomerAccount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var customerAccountPagedCollection = _customerAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (customerAccountPagedCollection != null)
                {
                    var pageCollection = customerAccountPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountDTO>();

                    var itemsCount = customerAccountPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CustomerAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccounts(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountSpecifications.CustomerAccountFullText(text, customerFilter);

                ISpecification<CustomerAccount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var customerAccountPagedCollection = _customerAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (customerAccountPagedCollection != null)
                {
                    var pageCollection = customerAccountPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountDTO>();

                    var itemsCount = customerAccountPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CustomerAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCode(int productCode, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerAccountTypeProductCodeAndFullText(productCode, text, customerFilter);

                ISpecification<CustomerAccount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var customerAccountPagedCollection = _customerAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (customerAccountPagedCollection != null)
                {
                    var pageCollection = customerAccountPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountDTO>();

                    var itemsCount = customerAccountPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CustomerAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCodeAndRecordStatus(int productCode, int recordStatus, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerAccountTypeProductCodeAndRecordStatusAndFullText(productCode, recordStatus, text, customerFilter);

                ISpecification<CustomerAccount> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var customerAccountPagedCollection = _customerAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (customerAccountPagedCollection != null)
                {
                    var pageCollection = customerAccountPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountDTO>();

                    var itemsCount = customerAccountPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CustomerAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerId(Guid customerId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerId(customerId);

                    ISpecification<CustomerAccount> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountPagedCollection = _customerAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (customerAccountPagedCollection != null)
                    {
                        var pageCollection = customerAccountPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountDTO>();

                        var itemsCount = customerAccountPagedCollection.ItemsCount;

                        return new PageCollectionInfo<CustomerAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerAccountTypeTargetProductId(Guid targetProductId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerAccountTypeTargetProductId(targetProductId);

                    ISpecification<CustomerAccount> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountPagedCollection = _customerAccountRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (customerAccountPagedCollection != null)
                    {
                        var pageCollection = customerAccountPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountDTO>();

                        var itemsCount = customerAccountPagedCollection.ItemsCount;

                        return new PageCollectionInfo<CustomerAccountDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByCustomerAccountTypeTargetProductIdAndCustomerEmployerId(Guid targetProductId, Guid customerEmployerId, ServiceHeader serviceHeader)
        {
            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerAccountTypeTargetProductIdAndCustomerEmployerId(targetProductId, customerEmployerId);

                    ISpecification<CustomerAccount> spec = filter;

                    var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountDTO> FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(Guid customerId, Guid targetProductId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty && targetProductId != null && targetProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerIdAndCustomerAccountTypeTargetProductId(customerId, targetProductId);

                    ISpecification<CustomerAccount> spec = filter;

                    var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountDTO> FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductIds(Guid customerId, Guid[] targetProductIds, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty && targetProductIds != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerIdAndCustomerAccountTypeTargetProductId(customerId, targetProductIds);

                    ISpecification<CustomerAccount> spec = filter;

                    var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerId(customerId);

                    ISpecification<CustomerAccount> spec = filter;

                    var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCode(Guid customerId, int productCode, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerIdAndCustomerAccountTypeTargetProductCode(customerId, productCode);

                    ISpecification<CustomerAccount> spec = filter;

                    var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCodes(Guid customerId, int[] targetProductCodes, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSpecifications.CustomerAccountWithCustomerIdAndCustomerAccountTypeTargetProductCode(customerId, targetProductCodes);

                    ISpecification<CustomerAccount> spec = filter;

                    var customerAccounts = _customerAccountRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountHistoryDTO> FindCustomerAccountHistory(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountHistorySpecifications.CustomerAccountHistoryWithCustomerAccountId(customerAccountId);

                    ISpecification<CustomerAccountHistory> spec = filter;

                    var customerAccountHistories = _customerAccountHistoryRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountHistories != null && customerAccountHistories.Any())
                    {
                        return customerAccountHistories.ProjectedAsCollection<CustomerAccountHistoryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountHistoryDTO> FindCustomerAccountHistory(Guid customerAccountId, int managementAction, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountHistorySpecifications.CustomerAccountHistoryWithCustomerAccountIdAndManagementAction(customerAccountId, managementAction);

                    ISpecification<CustomerAccountHistory> spec = filter;

                    var customerAccountHistories = _customerAccountHistoryRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountHistories != null && customerAccountHistories.Any())
                    {
                        return customerAccountHistories.ProjectedAsCollection<CustomerAccountHistoryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public void FetchCustomerAccountsProductDescription(List<CustomerAccountDTO> customerAccounts, ServiceHeader serviceHeader, bool useCache)
        {
            if (customerAccounts != null && customerAccounts.Any())
            {
                customerAccounts.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountTypeTargetProductId, item.BranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountTypeTargetProductId, item.BranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountType = savingsProduct.ChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductIsDefault = savingsProduct.IsDefault;
                                item.CustomerAccountTypeTargetProductWithdrawalNoticeAmount = savingsProduct.WithdrawalNoticeAmount;
                                item.CustomerAccountTypeTargetProductWithdrawalNoticePeriod = savingsProduct.WithdrawalNoticePeriod;
                                item.CustomerAccountTypeTargetProductWithdrawalInterval = savingsProduct.WithdrawalInterval;
                                item.CustomerAccountTypeTargetProductMaximumAllowedWithdrawal = savingsProduct.MaximumAllowedWithdrawal;
                                item.CustomerAccountTypeTargetProductMaximumAllowedDeposit = savingsProduct.MaximumAllowedDeposit;
                                item.CustomerAccountTypeTargetProductMinimumBalance = savingsProduct.MinimumBalance;
                                item.CustomerAccountTypeTargetProductRecoveryPriority = savingsProduct.Priority;
                                item.CustomerAccountTypeTargetProductThrottleOverTheCounterWithdrawals = savingsProduct.ThrottleOverTheCounterWithdrawals;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountType = loanProduct.ChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId = loanProduct.InterestReceivableChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountType = loanProduct.InterestReceivableChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountCode = loanProduct.InterestReceivableChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountName = loanProduct.InterestReceivableChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId = loanProduct.InterestReceivedChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountType = loanProduct.InterestReceivedChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountCode = loanProduct.InterestReceivedChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountName = loanProduct.InterestReceivedChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId = loanProduct.InterestChargedChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountType = loanProduct.InterestChargedChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountCode = loanProduct.InterestChargedChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountName = loanProduct.InterestChargedChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductLoanProductSection = loanProduct.LoanRegistrationLoanProductSection;
                                item.CustomerAccountTypeTargetProductIsMicrocredit = loanProduct.LoanRegistrationMicrocredit;
                                item.CustomerAccountTypeTargetProductChargeClearanceFee = loanProduct.LoanRegistrationChargeClearanceFee;
                                item.CustomerAccountTypeTargetProductThrottleScheduledArrearsRecovery = loanProduct.LoanRegistrationThrottleScheduledArrearsRecovery;
                                item.CustomerAccountTypeTargetProductRoundingType = loanProduct.LoanRegistrationRoundingType;
                                item.CustomerAccountTypeTargetProductRecoveryPriority = loanProduct.Priority;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountType = investmentProduct.ChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductIsRefundable = investmentProduct.IsRefundable;
                                item.CustomerAccountTypeTargetProductMaturityPeriod = investmentProduct.MaturityPeriod;
                                item.CustomerAccountTypeTargetProductTransferBalanceToParentOnMembershipTermination = investmentProduct.TransferBalanceToParentOnMembershipTermination;
                                item.CustomerAccountTypeTargetProductParentId = investmentProduct.ParentId;
                                item.CustomerAccountTypeTargetProductThrottleScheduledArrearsRecovery = investmentProduct.ThrottleScheduledArrearsRecovery;
                                item.CustomerAccountTypeTargetProductRecoveryPriority = investmentProduct.Priority;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<StandingOrderDTO> standingOrders, ServiceHeader serviceHeader, bool useCache)
        {
            if (standingOrders != null && standingOrders.Any())
            {
                standingOrders.ForEach(item =>
                {
                    switch ((ProductCode)item.BeneficiaryCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, item.BeneficiaryCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, item.BeneficiaryCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.BeneficiaryProductDescription = savingsProduct.Description.Trim();
                                item.BeneficiaryProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.BeneficiaryProductProductCode = (int)ProductCode.Savings;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.BeneficiaryProductDescription = loanProduct.Description.Trim();
                                item.BeneficiaryProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.BeneficiaryProductProductCode = (int)ProductCode.Loan;
                                item.BeneficiaryProductLoanProductSection = loanProduct.LoanRegistrationLoanProductSection;
                                item.BeneficiaryProductRoundingType = loanProduct.LoanRegistrationRoundingType;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.BeneficiaryProductDescription = investmentProduct.Description.Trim();
                                item.BeneficiaryProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.BeneficiaryProductProductCode = (int)ProductCode.Investment;
                            }
                            break;
                        default:
                            break;
                    }

                    switch ((ProductCode)item.BenefactorCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, item.BenefactorCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, item.BenefactorCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.BenefactorProductDescription = savingsProduct.Description.Trim();
                                item.BenefactorProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.BenefactorProductProductCode = (int)ProductCode.Savings;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.BenefactorProductDescription = loanProduct.Description.Trim();
                                item.BenefactorProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.BenefactorProductProductCode = (int)ProductCode.Loan;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.BenefactorProductDescription = investmentProduct.Description.Trim();
                                item.BenefactorProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.BenefactorProductProductCode = (int)ProductCode.Investment;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<StandingOrderHistoryDTO> standingOrderHistoryDTOs, ServiceHeader serviceHeader, bool useCache)
        {
            if (standingOrderHistoryDTOs != null && standingOrderHistoryDTOs.Any())
            {
                standingOrderHistoryDTOs.ForEach(item =>
                {
                    switch ((ProductCode)item.BeneficiaryCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, item.BeneficiaryCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, item.BeneficiaryCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.BeneficiaryProductDescription = savingsProduct.Description.Trim();
                                item.BeneficiaryProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.BeneficiaryProductProductCode = (int)ProductCode.Savings;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.BeneficiaryProductDescription = loanProduct.Description.Trim();
                                item.BeneficiaryProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.BeneficiaryProductProductCode = (int)ProductCode.Loan;
                                item.BeneficiaryProductLoanProductSection = loanProduct.LoanRegistrationLoanProductSection;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.BeneficiaryCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.BeneficiaryProductDescription = investmentProduct.Description.Trim();
                                item.BeneficiaryProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.BeneficiaryProductProductCode = (int)ProductCode.Investment;
                            }
                            break;
                        default:
                            break;
                    }

                    switch ((ProductCode)item.BenefactorCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, item.BenefactorCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, item.BenefactorCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.BenefactorProductDescription = savingsProduct.Description.Trim();
                                item.BenefactorProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.BenefactorProductProductCode = (int)ProductCode.Savings;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.BenefactorProductDescription = loanProduct.Description.Trim();
                                item.BenefactorProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.BenefactorProductProductCode = (int)ProductCode.Loan;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.BenefactorCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.BenefactorProductDescription = investmentProduct.Description.Trim();
                                item.BenefactorProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.BenefactorProductProductCode = (int)ProductCode.Investment;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<ElectronicStatementOrderDTO> electronicStatementOrders, ServiceHeader serviceHeader, bool useCache)
        {
            if (electronicStatementOrders != null && electronicStatementOrders.Any())
            {
                electronicStatementOrders.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductDescription = savingsProduct.Description.Trim();
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductProductCode = (int)ProductCode.Savings;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductDescription = loanProduct.Description.Trim();
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductProductCode = (int)ProductCode.Loan;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductDescription = investmentProduct.Description.Trim();
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductProductCode = (int)ProductCode.Investment;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<ElectronicStatementOrderHistoryDTO> electronicStatementOrderHistoryDTOs, ServiceHeader serviceHeader, bool useCache)
        {
            if (electronicStatementOrderHistoryDTOs != null && electronicStatementOrderHistoryDTOs.Any())
            {
                electronicStatementOrderHistoryDTOs.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductDescription = savingsProduct.Description.Trim();
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductProductCode = (int)ProductCode.Savings;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductDescription = loanProduct.Description.Trim();
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductProductCode = (int)ProductCode.Loan;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductDescription = investmentProduct.Description.Trim();
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductProductCode = (int)ProductCode.Investment;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<CreditBatchEntryDTO> creditBatchEntries, ServiceHeader serviceHeader, bool useCache)
        {
            if (creditBatchEntries != null && creditBatchEntries.Any())
            {
                creditBatchEntries.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<DebitBatchEntryDTO> debitBatchEntries, ServiceHeader serviceHeader, bool useCache)
        {
            if (debitBatchEntries != null && debitBatchEntries.Any())
            {
                debitBatchEntries.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<OverDeductionBatchEntryDTO> overDeductionBatchEntries, ServiceHeader serviceHeader, bool useCache)
        {
            if (overDeductionBatchEntries != null && overDeductionBatchEntries.Any())
            {
                overDeductionBatchEntries.ForEach(item =>
                {
                    switch ((ProductCode)item.DebitCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.DebitCustomerAccountCustomerAccountTypeTargetProductId, item.DebitCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.DebitCustomerAccountCustomerAccountTypeTargetProductId, item.DebitCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.DebitProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.DebitProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.DebitProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.DebitProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.DebitCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.DebitCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.DebitProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.DebitProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.DebitProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.DebitProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.DebitCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.DebitCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.DebitProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.DebitProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.DebitProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.DebitProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }

                    switch ((ProductCode)item.CreditCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CreditCustomerAccountCustomerAccountTypeTargetProductId, item.CreditCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CreditCustomerAccountCustomerAccountTypeTargetProductId, item.CreditCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.CreditProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CreditProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CreditProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CreditProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CreditCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CreditCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.CreditProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CreditProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CreditProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CreditProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CreditCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CreditCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.CreditProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CreditProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CreditProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CreditProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<FixedDepositDTO> fixedDeposits, ServiceHeader serviceHeader, bool useCache)
        {
            if (fixedDeposits != null && fixedDeposits.Any())
            {
                fixedDeposits.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<AlternateChannelDTO> alternateChannels, ServiceHeader serviceHeader, bool useCache)
        {
            if (alternateChannels != null && alternateChannels.Any())
            {
                alternateChannels.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<SalaryHeadDTO> salaryHeads, ServiceHeader serviceHeader, bool useCache)
        {
            if (salaryHeads != null && salaryHeads.Any())
            {
                salaryHeads.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountTypeTargetProductId, Guid.Empty, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountTypeTargetProductId, Guid.Empty, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<AttachedLoanDTO> attachedLoans, ServiceHeader serviceHeader, bool useCache)
        {
            if (attachedLoans != null && attachedLoans.Any())
            {
                attachedLoans.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountType = loanProduct.ChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId = loanProduct.InterestReceivableChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountType = loanProduct.InterestReceivableChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountCode = loanProduct.InterestReceivableChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountName = loanProduct.InterestReceivableChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId = loanProduct.InterestReceivedChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountType = loanProduct.InterestReceivedChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountCode = loanProduct.InterestReceivedChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountName = loanProduct.InterestReceivedChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId = loanProduct.InterestChargedChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountType = loanProduct.InterestChargedChartOfAccountAccountType;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountCode = loanProduct.InterestChargedChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestChargedChartOfAccountName = loanProduct.InterestChargedChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductChargeClearanceFee = loanProduct.LoanRegistrationChargeClearanceFee;
                                item.CustomerAccountTypeTargetProductProductSection = loanProduct.LoanRegistrationLoanProductSection;
                                item.CustomerAccountTypeTargetProductIsMicrocredit = loanProduct.LoanRegistrationMicrocredit;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<ExternalChequePayableDTO> externalChequePayables, ServiceHeader serviceHeader, bool useCache)
        {
            if (externalChequePayables != null && externalChequePayables.Any())
            {
                externalChequePayables.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductChargeClearanceFee = loanProduct.LoanRegistrationChargeClearanceFee;
                                item.CustomerAccountTypeTargetProductProductSection = loanProduct.LoanRegistrationLoanProductSection;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId = loanProduct.InterestReceivableChartOfAccountId;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<MobileToBankRequestDTO> mobileToBankRequests, ServiceHeader serviceHeader, bool useCache)
        {
            if (mobileToBankRequests != null && mobileToBankRequests.Any())
            {
                mobileToBankRequests.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductChargeClearanceFee = loanProduct.LoanRegistrationChargeClearanceFee;
                                item.CustomerAccountTypeTargetProductProductSection = loanProduct.LoanRegistrationLoanProductSection;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId = loanProduct.InterestReceivableChartOfAccountId;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<DataAttachmentEntryDTO> dataAttachmentEntries, ServiceHeader serviceHeader, bool useCache)
        {
            if (dataAttachmentEntries != null && dataAttachmentEntries.Any())
            {
                dataAttachmentEntries.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<WithdrawalSettlementDTO> withdrawalSettlements, ServiceHeader serviceHeader, bool useCache)
        {
            if (withdrawalSettlements != null && withdrawalSettlements.Any())
            {
                withdrawalSettlements.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<WireTransferBatchEntryDTO> eftBatchEntries, ServiceHeader serviceHeader, bool useCache)
        {
            if (eftBatchEntries != null && eftBatchEntries.Any())
            {
                eftBatchEntries.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.ProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<AccountClosureRequestDTO> accountClosureRequests, ServiceHeader serviceHeader, bool useCache)
        {
            if (accountClosureRequests != null && accountClosureRequests.Any())
            {
                accountClosureRequests.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<CustomerAccountCarryForwardInstallmentDTO> customerAccountCarryForwardInstallments, ServiceHeader serviceHeader, bool useCache)
        {
            if (customerAccountCarryForwardInstallments != null && customerAccountCarryForwardInstallments.Any())
            {
                customerAccountCarryForwardInstallments.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, item.CustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.ProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.ProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.ProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.ProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.ProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.ProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<LoanGuarantorAttachmentHistoryDTO> loanGuarantorAttachmentHistoryDTOs, ServiceHeader serviceHeader, bool useCache)
        {
            if (loanGuarantorAttachmentHistoryDTOs != null && loanGuarantorAttachmentHistoryDTOs.Any())
            {
                loanGuarantorAttachmentHistoryDTOs.ForEach(item =>
                {
                    switch ((ProductCode)item.SourceCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.SourceCustomerAccountCustomerAccountTypeTargetProductId, item.SourceCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.SourceCustomerAccountCustomerAccountTypeTargetProductId, item.SourceCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.SourceCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.SourceCustomerAccountCustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.SourceCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.SourceCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.SourceCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.SourceCustomerAccountCustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.SourceCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.SourceCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.SourceCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.SourceCustomerAccountCustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void FetchCustomerAccountsProductDescription(List<LoanGuarantorAttachmentHistoryEntryDTO> loanGuarantorAttachmentHistoryEntries, ServiceHeader serviceHeader, bool useCache)
        {
            if (loanGuarantorAttachmentHistoryEntries != null && loanGuarantorAttachmentHistoryEntries.Any())
            {
                loanGuarantorAttachmentHistoryEntries.ForEach(item =>
                {
                    switch ((ProductCode)item.DestinationCustomerAccountCustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.DestinationCustomerAccountCustomerAccountTypeTargetProductId, item.DestinationCustomerAccountBranchId, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.DestinationCustomerAccountCustomerAccountTypeTargetProductId, item.DestinationCustomerAccountBranchId, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.DestinationCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.DestinationCustomerAccountCustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.DestinationCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.DestinationCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.DestinationCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.DestinationCustomerAccountCustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.DestinationCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.DestinationCustomerAccountCustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.DestinationCustomerAccountCustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.DestinationCustomerAccountCustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public decimal ComputeEligibleLoanAppraisalInvestmentsBalance(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            var result = 0m;

            var productCollectionInfo = _loanProductAppService.FindAppraisalProducts(loanProductId, serviceHeader);

            if (productCollectionInfo != null && productCollectionInfo.InvestmentProductCollection != null)
            {
                var productIds = (from p in productCollectionInfo.InvestmentProductCollection
                                  select p.Id).ToArray();

                var customerAccountDTOs = FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductIds(customerId, productIds, serviceHeader);

                if (customerAccountDTOs != null && customerAccountDTOs.Any())
                {
                    FetchCustomerAccountBalances(customerAccountDTOs, serviceHeader, false, true);

                    result = customerAccountDTOs.Sum(x => x.BookBalance);
                }
            }

            return result;
        }

        public double GetLoaneeAppraisalFactorByCustomerClassification(int customerClassification, Guid loanProductId, ServiceHeader serviceHeader)
        {
            var result = 0d;

            var productCollectionInfo = _loanProductAppService.FindAppraisalProducts(loanProductId, serviceHeader);

            if (productCollectionInfo != null && productCollectionInfo.InvestmentProductCollection != null)
            {
                foreach (var item in productCollectionInfo.InvestmentProductCollection)
                {
                    var investmentProductExemptions = _investmentProductAppService.FindInvestmentProductExemptions(item.Id, serviceHeader);

                    if (investmentProductExemptions != null && investmentProductExemptions.Any())
                    {
                        var targetInvestmentProductExemption = investmentProductExemptions.Where(x => x.CustomerClassification == customerClassification).FirstOrDefault(); /*we expect only one entry per classification*/

                        if (targetInvestmentProductExemption != null)
                        {
                            result = targetInvestmentProductExemption.AppraisalMultiplier;
                        }
                    }

                    break; /*only consider first selection? is there a better way to handle this?*/
                }
            }
            else
            {
                var loanProduct = _loanProductAppService.FindLoanProduct(loanProductId, serviceHeader);

                if (loanProduct != null)
                {
                    result = loanProduct.LoanRegistrationInvestmentsMultiplier;
                }
            }

            return result;
        }

        public List<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSignatorySpecifications.CustomerAccountSignatoryWithCustomerAccountId(customerAccountId);

                    ISpecification<CustomerAccountSignatory> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccounts = _customerAccountSignatoryRepository.AllMatching(spec, serviceHeader);

                    if (customerAccounts != null && customerAccounts.Any())
                    {
                        return customerAccounts.ProjectedAsCollection<CustomerAccountSignatoryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountId(Guid customerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountSignatorySpecifications.CustomerAccountSignatoryWithCustomerAccountId(customerAccountId);

                    ISpecification<CustomerAccountSignatory> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountSignatoryPagedCollection = _customerAccountSignatoryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (customerAccountSignatoryPagedCollection != null)
                    {
                        var pageCollection = customerAccountSignatoryPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountSignatoryDTO>();

                        var itemsCount = customerAccountSignatoryPagedCollection.ItemsCount;

                        return new PageCollectionInfo<CustomerAccountSignatoryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountCarryForwardSpecifications.CustomerAccountCarryForwardWithBenefactorCustomerAccountId(benefactorCustomerAccountId);

                    ISpecification<CustomerAccountCarryForward> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwards = _customerAccountCarryForwardRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountCarryForwards != null && customerAccountCarryForwards.Any())
                    {
                        return customerAccountCarryForwards.ProjectedAsCollection<CustomerAccountCarryForwardDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountCarryForwardSpecifications.CustomerAccountCarryForwardWithBenefactorCustomerAccountId(benefactorCustomerAccountId);

                    ISpecification<CustomerAccountCarryForward> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwardPagedCollection = _customerAccountCarryForwardRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (customerAccountCarryForwardPagedCollection != null)
                    {
                        var pageCollection = customerAccountCarryForwardPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountCarryForwardDTO>();

                        var itemsCount = customerAccountCarryForwardPagedCollection.ItemsCount;

                        return new PageCollectionInfo<CustomerAccountCarryForwardDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader)
        {
            if (beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountCarryForwardSpecifications.CustomerAccountCarryForwardWithBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId);

                    ISpecification<CustomerAccountCarryForward> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwards = _customerAccountCarryForwardRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountCarryForwards != null && customerAccountCarryForwards.Any())
                    {
                        var projection = customerAccountCarryForwards.ProjectedAsCollection<CustomerAccountCarryForwardDTO>().OrderBy(x => x.CreatedDate).ToArray();

                        var cumulativeBalance = 0m;

                        Array.ForEach(projection, (item) =>
                        {
                            cumulativeBalance += item.Amount;

                            item.RunningBalance = cumulativeBalance;
                        });

                        return projection.ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountId(Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId, ServiceHeader serviceHeader)
        {
            if (beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty && beneficiaryChartOfAccountId != null && beneficiaryChartOfAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountCarryForwardSpecifications.CustomerAccountCarryForwardWithBeneficiaryCustomerAccountIdAndChartOfAccountId(beneficiaryCustomerAccountId, beneficiaryChartOfAccountId);

                    ISpecification<CustomerAccountCarryForward> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwards = _customerAccountCarryForwardRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountCarryForwards != null && customerAccountCarryForwards.Any())
                    {
                        var projection = customerAccountCarryForwards.ProjectedAsCollection<CustomerAccountCarryForwardDTO>().OrderBy(x => x.CreatedDate).ToArray();

                        var cumulativeBalance = 0m;

                        Array.ForEach(projection, (item) =>
                        {
                            cumulativeBalance += item.Amount;

                            item.RunningBalance = cumulativeBalance;
                        });

                        return projection.ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountId(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader)
        {
            if (beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountCarryForwardSpecifications.CustomerAccountCarryForwardWithBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountId(benefactorCustomerAccountId, beneficiaryCustomerAccountId);

                    ISpecification<CustomerAccountCarryForward> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwards = _customerAccountCarryForwardRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountCarryForwards != null && customerAccountCarryForwards.Any())
                    {
                        return customerAccountCarryForwards.ProjectedAsCollection<CustomerAccountCarryForwardDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountArrearageSpecifications.CustomerAccountArrearageWithCustomerAccountId(customerAccountId);

                    ISpecification<CustomerAccountArrearage> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountArrearages = _customerAccountArrearageRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountArrearages != null && customerAccountArrearages.Any())
                    {
                        return customerAccountArrearages.ProjectedAsCollection<CustomerAccountArrearageDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, DateTime endDate, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountArrearageSpecifications.CustomerAccountArrearageWithCustomerAccountIdAndEndDate(customerAccountId, endDate);

                    ISpecification<CustomerAccountArrearage> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountArrearages = _customerAccountArrearageRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountArrearages != null && customerAccountArrearages.Any())
                    {
                        return customerAccountArrearages.ProjectedAsCollection<CustomerAccountArrearageDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, int category, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountArrearageSpecifications.CustomerAccountArrearageWithCustomerAccountIdAndCategory(customerAccountId, category);

                    ISpecification<CustomerAccountArrearage> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountArrearages = _customerAccountArrearageRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountArrearages != null && customerAccountArrearages.Any())
                    {
                        var projection = customerAccountArrearages.ProjectedAsCollection<CustomerAccountArrearageDTO>().OrderBy(x => x.CreatedDate).ToArray();

                        var cumulativeBalance = 0m;

                        Array.ForEach(projection, (item) =>
                        {
                            cumulativeBalance += item.Amount;

                            item.RunningBalance = cumulativeBalance;
                        });

                        return projection.ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountArrearageSpecifications.CustomerAccountArrearageWithCustomerAccountId(customerAccountId);

                    ISpecification<CustomerAccountArrearage> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountArrearagePagedCollection = _customerAccountArrearageRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (customerAccountArrearagePagedCollection != null)
                    {
                        var pageCollection = customerAccountArrearagePagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountArrearageDTO>();

                        var itemsCount = customerAccountArrearagePagedCollection.ItemsCount;

                        return new PageCollectionInfo<CustomerAccountArrearageDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountCarryForwardSpecifications.CustomerAccountCarryForwardWithBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId);

                    ISpecification<CustomerAccountCarryForward> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwardPagedCollection = _customerAccountCarryForwardRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (customerAccountCarryForwardPagedCollection != null)
                    {
                        var pageCollection = customerAccountCarryForwardPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountCarryForwardDTO>();

                        var itemsCount = customerAccountCarryForwardPagedCollection.ItemsCount;

                        return new PageCollectionInfo<CustomerAccountCarryForwardDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public CustomerAccountSignatoryDTO AddNewCustomerAccountSignatory(CustomerAccountSignatoryDTO customerAccountSignatoryDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountSignatoryDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var address = new Address(customerAccountSignatoryDTO.AddressAddressLine1, customerAccountSignatoryDTO.AddressAddressLine2, customerAccountSignatoryDTO.AddressStreet, customerAccountSignatoryDTO.AddressPostalCode, customerAccountSignatoryDTO.AddressCity, customerAccountSignatoryDTO.AddressEmail, customerAccountSignatoryDTO.AddressLandLine, customerAccountSignatoryDTO.AddressMobileLine);

                    var customerAccountSignatory = CustomerAccountSignatoryFactory.CreateCustomerAccountSignatory(customerAccountSignatoryDTO.CustomerAccountId, customerAccountSignatoryDTO.Salutation, customerAccountSignatoryDTO.FirstName, customerAccountSignatoryDTO.LastName, customerAccountSignatoryDTO.IdentityCardType, customerAccountSignatoryDTO.IdentityCardNumber, customerAccountSignatoryDTO.Gender, customerAccountSignatoryDTO.Relationship, address, customerAccountSignatoryDTO.Remarks);

                    customerAccountSignatory.CreatedBy = serviceHeader.ApplicationUserName;

                    _customerAccountSignatoryRepository.Add(customerAccountSignatory, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return customerAccountSignatory.ProjectedAs<CustomerAccountSignatoryDTO>();
                }
            }
            else return null;
        }

        public bool RemoveCustomerAccountSignatories(List<CustomerAccountSignatoryDTO> customerAccountSignatoryDTOs, ServiceHeader serviceHeader)
        {
            if (customerAccountSignatoryDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in customerAccountSignatoryDTOs)
                {
                    if (item.Id != null && item.Id != Guid.Empty)
                    {
                        var persisted = _customerAccountSignatoryRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _customerAccountSignatoryRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public void FetchCustomerAccountBalances(List<CustomerAccountDTO> customerAccounts, ServiceHeader serviceHeader, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            if (customerAccounts != null && customerAccounts.Any())
            {
                customerAccounts.ForEach(customerAccount =>
                {
                    switch ((ProductCode)customerAccount.CustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:

                            customerAccount.AvailableBalance = _sqlCommandAppService.FindCustomerAccountAvailableBalance(customerAccount, DateTime.Now, serviceHeader);

                            customerAccount.BookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccount, 1, DateTime.Now, serviceHeader);

                            break;
                        case ProductCode.Loan:

                            customerAccount.PrincipalBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccount, 1, DateTime.Now, serviceHeader);

                            if (includeInterestBalanceForLoanAccounts)
                            {
                                customerAccount.InterestBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccount, 2, DateTime.Now, serviceHeader);

                                var carryForwards = FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(customerAccount.Id, serviceHeader);

                                if (carryForwards != null && carryForwards.Any())
                                {
                                    customerAccount.CarryForwardsBalance = carryForwards.Sum(x => x.Amount) * -1;
                                }

                                var arrearages = FindCustomerAccountArrearagesByCustomerAccountId(customerAccount.Id, serviceHeader);

                                if (arrearages != null && arrearages.Any())
                                {
                                    customerAccount.PrincipalArrearagesBalance = arrearages.Where(x => x.Category == (int)ArrearageCategory.Principal).Sum(x => x.Amount) * -1;
                                    customerAccount.InterestArrearagesBalance = arrearages.Where(x => x.Category == (int)ArrearageCategory.Interest).Sum(x => x.Amount) * -1;
                                }
                            }

                            customerAccount.BookBalance = customerAccount.PrincipalBalance + customerAccount.InterestBalance;

                            break;
                        case ProductCode.Investment:

                            customerAccount.BookBalance = _sqlCommandAppService.FindCustomerAccountBookBalance(customerAccount, 1, DateTime.Now, serviceHeader, considerMaturityPeriodForInvestmentAccounts);

                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public bool ChargeAccountActivationFee(Guid customerAccountId, string remarks, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var postingPeriodDTO = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

            var defaultSavingsProductDTO = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

            var targetCustomerAccountDTO = FindCustomerAccountDTO(customerAccountId, serviceHeader);

            if (postingPeriodDTO != null && defaultSavingsProductDTO != null && targetCustomerAccountDTO != null)
            {
                CustomerAccountDTO customerSavingsAccountDTO = null;

                if (targetCustomerAccountDTO.CustomerAccountTypeTargetProductId == defaultSavingsProductDTO.Id)
                    customerSavingsAccountDTO = targetCustomerAccountDTO;
                else
                {
                    var customerSavingsAccounts = FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(targetCustomerAccountDTO.CustomerId, defaultSavingsProductDTO.Id, serviceHeader);

                    if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                        customerSavingsAccountDTO = customerSavingsAccounts.First();
                    else
                    {
                        var customerAccountDTO = new CustomerAccountDTO
                        {
                            BranchId = targetCustomerAccountDTO.BranchId,
                            CustomerId = targetCustomerAccountDTO.CustomerId,
                            CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                            CustomerAccountTypeTargetProductId = defaultSavingsProductDTO.Id,
                            CustomerAccountTypeTargetProductCode = defaultSavingsProductDTO.Code,
                            Status = (int)CustomerAccountStatus.Normal,
                        };

                        customerAccountDTO = AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                        if (customerAccountDTO != null)
                            customerSavingsAccountDTO = customerAccountDTO;
                    }
                }

                var journals = new List<Journal>();

                var activationFeeTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.CustomerAccountActivationFee, 0m, customerSavingsAccountDTO, serviceHeader);

                if (activationFeeTariffs.Any())
                {
                    activationFeeTariffs.ForEach(tariff =>
                    {
                        var tariffJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, customerSavingsAccountDTO.BranchId, null, tariff.Amount, tariff.Description, remarks, targetCustomerAccountDTO.FullAccountNumber, moduleNavigationItemCode, (int)SystemTransactionCode.AccountActivation, null, serviceHeader);
                        _journalEntryPostingService.PerformDoubleEntry(tariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                        journals.Add(tariffJournal);
                    });
                }

                if (journals.Any())
                {
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
            }

            return result;
        }


        public CustomerAccountArrearageDTO AddNewCustomerAccountArrearage(CustomerAccountArrearageDTO customerAccountArrearageDTO, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                switch ((ArrearageAdjustmentType)customerAccountArrearageDTO.AdjustmentType)
                {
                    case ArrearageAdjustmentType.Addition:
                        customerAccountArrearageDTO.Amount = customerAccountArrearageDTO.Amount * 1;
                        break;
                    case ArrearageAdjustmentType.Deduction:
                        customerAccountArrearageDTO.Amount = customerAccountArrearageDTO.Amount * -1;
                        break;
                    default:
                        break;
                }

                var customerAccountInterestArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(customerAccountArrearageDTO.CustomerAccountId, customerAccountArrearageDTO.Category, customerAccountArrearageDTO.Amount, customerAccountArrearageDTO.Reference);
                customerAccountInterestArrearage.CreatedBy = serviceHeader.ApplicationUserName;

                _customerAccountArrearageRepository.Add(customerAccountInterestArrearage, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) > 0 ? customerAccountInterestArrearage.ProjectedAs<CustomerAccountArrearageDTO>() : null;
            }
        }

        public async Task<bool> UpdateCustomerAccountArrearagesAsync(List<CustomerAccountArrearageDTO> customerAccountArrearages, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                customerAccountArrearages.ForEach(item =>
                {
                    if (item.Id == Guid.Empty)
                    {
                        switch ((ArrearageAdjustmentType)item.AdjustmentType)
                        {
                            case ArrearageAdjustmentType.Addition:
                                break;
                            case ArrearageAdjustmentType.Deduction:
                                item.Amount = item.Amount * -1;
                                break;
                            default:
                                break;
                        }

                        var customerAccountArrearage = CustomerAccountArrearageFactory.CreateCustomerAccountArrearage(item.CustomerAccountId, item.Category, item.Amount, item.Reference);

                        customerAccountArrearage.CreatedBy = serviceHeader.ApplicationUserName;

                        _customerAccountArrearageRepository.Add(customerAccountArrearage, serviceHeader);
                    }
                });

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public CustomerAccountCarryForwardDTO AddNewCustomerAccountCarryForward(CustomerAccountCarryForwardDTO customerAccountCarryForwardDTO, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                switch ((CarryForwardAdjustmentType)customerAccountCarryForwardDTO.AdjustmentType)
                {
                    case CarryForwardAdjustmentType.Addition:
                        break;
                    case CarryForwardAdjustmentType.Deduction:
                        customerAccountCarryForwardDTO.Amount = customerAccountCarryForwardDTO.Amount * -1;
                        break;
                    default:
                        break;
                }

                var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(customerAccountCarryForwardDTO.BenefactorCustomerAccountId, customerAccountCarryForwardDTO.BeneficiaryCustomerAccountId, customerAccountCarryForwardDTO.BeneficiaryChartOfAccountId, customerAccountCarryForwardDTO.Amount, customerAccountCarryForwardDTO.Reference);

                customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;

                _customerAccountCarryForwardRepository.Add(customerAccountCarryForward, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) > 0 ? customerAccountCarryForward.ProjectedAs<CustomerAccountCarryForwardDTO>() : null;
            }
        }

        public bool UpdateCustomerAccountCarryForwardInstallment(CustomerAccountCarryForwardInstallmentDTO customerAccountCarryForwardInstallmentDTO, ServiceHeader serviceHeader)
        {
            if (customerAccountCarryForwardInstallmentDTO == null || customerAccountCarryForwardInstallmentDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _customerAccountCarryForwardInstallmentRepository.Get(customerAccountCarryForwardInstallmentDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = CustomerAccountCarryForwardInstallmentFactory.CreateCustomerAccountCarryForwardInstallment(customerAccountCarryForwardInstallmentDTO.CustomerAccountId, customerAccountCarryForwardInstallmentDTO.ChartOfAccountId, customerAccountCarryForwardInstallmentDTO.Amount, customerAccountCarryForwardInstallmentDTO.Reference);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    if (customerAccountCarryForwardInstallmentDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _customerAccountCarryForwardInstallmentRepository.Merge(persisted, current, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool UpdateCustomerAccountCarryForwardInstallments(List<CustomerAccountCarryForwardInstallmentDTO> customerAccountCarryForwardInstallmentDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                customerAccountCarryForwardInstallmentDTOs.ForEach(item =>
                {
                    var filter = CustomerAccountCarryForwardInstallmentSpecifications.CustomerAccountCarryForwardInstallmentWithCustomerAccountIdAndChartOfAccountId(item.CustomerAccountId, item.ChartOfAccountId);

                    ISpecification<CustomerAccountCarryForwardInstallment> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwardInstallments = _customerAccountCarryForwardInstallmentRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountCarryForwardInstallments != null && customerAccountCarryForwardInstallments.Any())
                    {
                        foreach (var carryForwardInstallment in customerAccountCarryForwardInstallments)
                        {
                            var persisted = _customerAccountCarryForwardInstallmentRepository.Get(carryForwardInstallment.Id, serviceHeader);

                            if (persisted != null)
                            {
                                _customerAccountCarryForwardInstallmentRepository.Remove(persisted, serviceHeader);
                            }
                        }
                    }

                    var customerAccountCarryForwardInstallment = CustomerAccountCarryForwardInstallmentFactory.CreateCustomerAccountCarryForwardInstallment(item.CustomerAccountId, item.ChartOfAccountId, item.Amount, item.Reference);

                    if (item.IsLocked)
                        customerAccountCarryForwardInstallment.Lock();
                    else customerAccountCarryForwardInstallment.UnLock();

                    customerAccountCarryForwardInstallment.CreatedBy = serviceHeader.ApplicationUserName;

                    _customerAccountCarryForwardInstallmentRepository.Add(customerAccountCarryForwardInstallment, serviceHeader);
                });

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public PageCollectionInfo<CustomerAccountCarryForwardInstallmentDTO> FindCustomerAccountCarryForwardInstallments(DateTime startDate, DateTime endDate, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerAccountCarryForwardInstallmentSpecifications.CustomerAccountCarryForwardInstallmentWithDateRangeAndFullText(startDate, endDate, text, customerFilter);

                ISpecification<CustomerAccountCarryForwardInstallment> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var customerAccountCarryForwardInstallmentPagedCollection = _customerAccountCarryForwardInstallmentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (customerAccountCarryForwardInstallmentPagedCollection != null)
                {
                    var pageCollection = customerAccountCarryForwardInstallmentPagedCollection.PageCollection.ProjectedAsCollection<CustomerAccountCarryForwardInstallmentDTO>();

                    FetchCustomerAccountsProductDescription(pageCollection, serviceHeader, true);

                    var itemsCount = customerAccountCarryForwardInstallmentPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CustomerAccountCarryForwardInstallmentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<CustomerAccountCarryForwardInstallmentDTO> FindCustomerAccountCarryForwardInstallments(Guid customerAccountId, Guid chartOfAccountId, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty && chartOfAccountId != null && chartOfAccountId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CustomerAccountCarryForwardInstallmentSpecifications.CustomerAccountCarryForwardInstallmentWithCustomerAccountIdAndChartOfAccountId(customerAccountId, chartOfAccountId);

                    ISpecification<CustomerAccountCarryForwardInstallment> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var customerAccountCarryForwardInstallments = _customerAccountCarryForwardInstallmentRepository.AllMatching(spec, serviceHeader);

                    if (customerAccountCarryForwardInstallments != null && customerAccountCarryForwardInstallments.Any())
                    {
                        var pageCollection = customerAccountCarryForwardInstallments.ProjectedAsCollection<CustomerAccountCarryForwardInstallmentDTO>();

                        FetchCustomerAccountsProductDescription(pageCollection, serviceHeader, true);

                        return pageCollection;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool AdjustCustomerAccountLoanInterest(LoanInterestAdjustmentDTO loanInterestAdjustmentDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (loanInterestAdjustmentDTO != null)
            {
                var customerAccountDTO = FindCustomerAccountDTO(loanInterestAdjustmentDTO.CustomerAccountId, serviceHeader);

                if (customerAccountDTO != null && customerAccountDTO.CustomerAccountTypeProductCode == (int)ProductCode.Loan)
                {
                    var journals = new List<Journal>();

                    FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true);

                    switch ((LoanInterestAdjustmentType)loanInterestAdjustmentDTO.AdjustmentType)
                    {
                        case LoanInterestAdjustmentType.Increase:

                            // Credit LoanProduct.InterestChargedChartOfAccountId, Debit LoanProduct.InterestReceivableChartOfAccountId
                            var increaseInterestJournal = JournalFactory.CreateJournal(null, loanInterestAdjustmentDTO.PostingPeriodId, loanInterestAdjustmentDTO.BranchId, null, loanInterestAdjustmentDTO.Amount, loanInterestAdjustmentDTO.Reference, string.Format("{0} > {1}", EnumHelper.GetDescription(SystemTransactionCode.InterestAdjustment), customerAccountDTO.CustomerAccountTypeTargetProductDescription), string.Format("Adjustment Type > {0}", loanInterestAdjustmentDTO.AdjustmentTypeDescription), loanInterestAdjustmentDTO.ModuleNavigationItemCode, (int)SystemTransactionCode.InterestAdjustment, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(increaseInterestJournal, customerAccountDTO.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            journals.Add(increaseInterestJournal);

                            break;
                        case LoanInterestAdjustmentType.Decrease:

                            // Credit LoanProduct.InterestReceivableChartOfAccountId, Debit LoanProduct.InterestChargedChartOfAccountId
                            var decreaseInterestJournal = JournalFactory.CreateJournal(null, loanInterestAdjustmentDTO.PostingPeriodId, loanInterestAdjustmentDTO.BranchId, null, loanInterestAdjustmentDTO.Amount, loanInterestAdjustmentDTO.Reference, string.Format("{0} > {1}", EnumHelper.GetDescription(SystemTransactionCode.InterestAdjustment), customerAccountDTO.CustomerAccountTypeTargetProductDescription), string.Format("Adjustment Type > {0}", loanInterestAdjustmentDTO.AdjustmentTypeDescription), loanInterestAdjustmentDTO.ModuleNavigationItemCode, (int)SystemTransactionCode.InterestAdjustment, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(decreaseInterestJournal, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, customerAccountDTO.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            journals.Add(decreaseInterestJournal);

                            break;
                        default:
                            break;
                    }

                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
            }

            return result;
        }

        private bool Deactivate(Guid customerAccountId, string remarks, int remarkType, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty && !string.IsNullOrEmpty(remarks))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<CustomerAccountHistory> spec = CustomerAccountHistorySpecifications.CustomerAccountHistoryWithCustomerAccountId(customerAccountId);

                    var matchedCustomerAccountHistories = _customerAccountHistoryRepository.AllMatching(spec, serviceHeader);

                    if (matchedCustomerAccountHistories == null || !matchedCustomerAccountHistories.Any())
                    {
                        var persisted0 = _customerAccountRepository.Get(customerAccountId, serviceHeader);

                        persisted0.Status = (int)CustomerAccountStatus.Inactive;
                        persisted0.Remarks = remarks;

                        persisted0.RecordStatus = (int)RecordStatus.Edited;
                        persisted0.ModifiedBy = serviceHeader.ApplicationUserName;
                        persisted0.ModifiedDate = DateTime.Now;

                        var customerAccountHistory0 = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(customerAccountId, (int)CustomerAccountManagementAction.Deactivation, remarks, EnumHelper.GetDescription((CustomerAccountRemarkType)remarkType), serviceHeader.ApplicationUserName);

                        _customerAccountHistoryRepository.Add(customerAccountHistory0, serviceHeader);
                    }
                    else
                    {
                        var filtered = matchedCustomerAccountHistories.Where(x => x.ManagementAction == (int)CustomerAccountManagementAction.Activation || x.ManagementAction == (int)CustomerAccountManagementAction.Deactivation).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                        if (filtered != null && filtered.ManagementAction == (int)CustomerAccountManagementAction.Deactivation)
                        { }
                        else
                        {
                            var persisted1 = _customerAccountRepository.Get(customerAccountId, serviceHeader);

                            persisted1.Status = (int)CustomerAccountStatus.Inactive;
                            persisted1.Remarks = remarks;

                            persisted1.RecordStatus = (int)RecordStatus.Edited;
                            persisted1.ModifiedBy = serviceHeader.ApplicationUserName;
                            persisted1.ModifiedDate = DateTime.Now;

                            var customerAccountHistory1 = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(customerAccountId, (int)CustomerAccountManagementAction.Deactivation, remarks, EnumHelper.GetDescription((CustomerAccountRemarkType)remarkType), serviceHeader.ApplicationUserName);

                            _customerAccountHistoryRepository.Add(customerAccountHistory1, serviceHeader);
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }
            else return false;
        }

        private bool Activate(Guid customerAccountId, string remarks, int remarkType, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty && !string.IsNullOrEmpty(remarks))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<CustomerAccountHistory> spec = CustomerAccountHistorySpecifications.CustomerAccountHistoryWithCustomerAccountId(customerAccountId);

                    var matchedCustomerAccountHistories = _customerAccountHistoryRepository.AllMatching(spec, serviceHeader);

                    if (matchedCustomerAccountHistories != null && matchedCustomerAccountHistories.Any(x => x.ManagementAction == (int)CustomerAccountManagementAction.Activation || x.ManagementAction == (int)CustomerAccountManagementAction.Deactivation || x.ManagementAction == (int)CustomerAccountManagementAction.Remark))
                    {
                        var filtered = matchedCustomerAccountHistories.Where(x => x.ManagementAction == (int)CustomerAccountManagementAction.Activation || x.ManagementAction == (int)CustomerAccountManagementAction.Deactivation || x.ManagementAction == (int)CustomerAccountManagementAction.Remark).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                        if (filtered != null && filtered.ManagementAction == (int)CustomerAccountManagementAction.Activation)
                            return true;
                        else
                        {
                            var persisted = _customerAccountRepository.Get(customerAccountId, serviceHeader);

                            persisted.Status = (byte)CustomerAccountStatus.Normal;
                            persisted.Remarks = remarks;

                            persisted.RecordStatus = persisted.Branch.Company.EnforceCustomerAccountMakerChecker ? (byte)RecordStatus.Edited : (byte)RecordStatus.Approved;
                            persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                            persisted.ModifiedDate = DateTime.Now;

                            var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(customerAccountId, (int)CustomerAccountManagementAction.Activation, remarks, EnumHelper.GetDescription((CustomerAccountRemarkType)remarkType), serviceHeader.ApplicationUserName);

                            _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                            return dbContextScope.SaveChanges(serviceHeader) > 0;
                        }
                    }
                    else throw new InvalidOperationException("Sorry, but account freezing history is missing!");
                }
            }
            else return false;
        }

        private bool Remark(Guid customerAccountId, string remarks, int remarkType, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty && !string.IsNullOrEmpty(remarks))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _customerAccountRepository.Get(customerAccountId, serviceHeader);

                    persisted.Remarks = remarks;

                    switch ((CustomerAccountRemarkType)remarkType)
                    {
                        case CustomerAccountRemarkType.Actionable:
                            persisted.Status = (int)CustomerAccountStatus.Remarked;
                            persisted.RecordStatus = (int)RecordStatus.Edited;

                            persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                            persisted.ModifiedDate = DateTime.Now;
                            break;
                        case CustomerAccountRemarkType.Informational:
                            break;
                        default:
                            break;
                    }

                    var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(customerAccountId, (int)CustomerAccountManagementAction.Remark, remarks, EnumHelper.GetDescription((CustomerAccountRemarkType)remarkType), serviceHeader.ApplicationUserName);

                    _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }
            else return false;
        }

        private bool SigningInstructions(Guid customerAccountId, string remarks, int remarkType, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty && !string.IsNullOrEmpty(remarks))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _customerAccountRepository.Get(customerAccountId, serviceHeader);

                    persisted.SigningInstructions = remarks;

                    remarkType = (int)CustomerAccountRemarkType.Informational;

                    var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(customerAccountId, (int)CustomerAccountManagementAction.SigningInstructions, remarks, EnumHelper.GetDescription((CustomerAccountRemarkType)remarkType), serviceHeader.ApplicationUserName);

                    _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }
            else return false;
        }

        private bool Close(Guid customerAccountId, string remarks, int remarkType, ServiceHeader serviceHeader)
        {
            if (customerAccountId != null && customerAccountId != Guid.Empty && !string.IsNullOrEmpty(remarks))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _customerAccountRepository.Get(customerAccountId, serviceHeader);

                    persisted.Status = (int)CustomerAccountStatus.Closed;
                    persisted.Remarks = remarks;

                    persisted.RecordStatus = (int)RecordStatus.Edited;
                    persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                    persisted.ModifiedDate = DateTime.Now;

                    var customerAccountHistory = CustomerAccountHistoryFactory.CreateCustomerAccountHistory(customerAccountId, (int)CustomerAccountManagementAction.Closure, remarks, EnumHelper.GetDescription((CustomerAccountRemarkType)remarkType), serviceHeader.ApplicationUserName);

                    _customerAccountHistoryRepository.Add(customerAccountHistory, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }
            else return false;
        }
    }
}
