using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAttachedProductAgg;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyDebitTypeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class CompanyAppService : ICompanyAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyAttachedProduct> _companyAttachedProductRepository;
        private readonly IRepository<CompanyDebitType> _companyDebitTypeRepository;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IAppCache _appCache;

        public CompanyAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<Company> companyRepository,
            IRepository<CompanyAttachedProduct> companyAttachedProductRepository,
            IRepository<CompanyDebitType> companyDebitTypeRepository,
            IInvestmentProductAppService investmentProductAppService,
            ISavingsProductAppService savingsProductAppService,
            IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (companyRepository == null)
                throw new ArgumentNullException(nameof(companyRepository));

            if (companyAttachedProductRepository == null)
                throw new ArgumentNullException(nameof(companyAttachedProductRepository));

            if (companyDebitTypeRepository == null)
                throw new ArgumentNullException(nameof(companyDebitTypeRepository));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _companyRepository = companyRepository;
            _companyAttachedProductRepository = companyAttachedProductRepository;
            _companyDebitTypeRepository = companyDebitTypeRepository;
            _investmentProductAppService = investmentProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _appCache = appCache;
        }

        public CompanyDTO AddNewCompany(CompanyDTO companyDTO, ServiceHeader serviceHeader)
        {
            var companyBindingModel = companyDTO.ProjectedAs<CompanyBindingModel>();

            companyBindingModel.ValidateAll();

            if (companyBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, companyBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var address = new Address(companyDTO.AddressAddressLine1, companyDTO.AddressAddressLine2, companyDTO.AddressStreet, companyDTO.AddressPostalCode, companyDTO.AddressCity, companyDTO.AddressEmail, companyDTO.AddressLandLine, companyDTO.AddressMobileLine);

                var timeDuration = new TimeDuration(companyDTO.TimeDurationStartTime, companyDTO.TimeDurationEndTime);

                var company = CompanyFactory.CreateCompany(companyDTO.Description, companyDTO.Vision, companyDTO.Mission, companyDTO.Motto, companyDTO.RegistrationNumber, companyDTO.PersonalIdentificationNumber, companyDTO.ApplicationDisplayName, companyDTO.TransactionReceiptTopIndentation, companyDTO.TransactionReceiptLeftIndentation, companyDTO.TransactionReceiptFooter, companyDTO.FingerprintBiometricThreshold, companyDTO.ApplicationMembershipTextAlertsEnabled, companyDTO.EnforceCustomerAccountMakerChecker, companyDTO.BypassJournalVoucherAudit, companyDTO.BypassCreditBatchAudit, companyDTO.BypassDebitBatchAudit, companyDTO.BypassRefundBatchAudit, companyDTO.BypassWireTransferBatchAudit, companyDTO.BypassLoanDisbursementBatchAudit, companyDTO.BypassJournalReversalBatchAudit, companyDTO.BypassInterAccountTransferBatchAudit, companyDTO.BypassExpensePayableAudit, companyDTO.BypassGeneralLedgerAudit, companyDTO.ExcludeChargesInTransactionReceipt, companyDTO.MembershipTerminationNoticePeriod, companyDTO.ExcludeChequeMaturityDateInTransactionReceipt, companyDTO.TrackGuarantorCommittedInvestments, companyDTO.TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement, companyDTO.ReceiveLoanRequestBeforeLoanRegistration, address, timeDuration, companyDTO.RecoveryPriority, companyDTO.IsWithholdingTaxAgent, companyDTO.EnforceBudgetControl, companyDTO.LocalizeOnlineNotifications, companyDTO.ExcludeCustomerAccountBalanceInTransactionReceipt, companyDTO.EnforceFixedDepositBands, companyDTO.EnforceBiometricsForCashWithdrawal, companyDTO.EnforceTwoFactorAuthentication,companyDTO.RecoverArrearsOnCashDeposit, companyDTO.RecoverArrearsOnExternalChequeClearance, companyDTO.RecoverArrearsOnFixedDepositPayment, companyDTO.AllowDebitBatchToOverdrawAccount, companyDTO.EnforceTellerLimits, companyDTO.EnforceSystemLock, companyDTO.EnforceTellerCashTransferAcknowledgement, companyDTO.EnforceSingleUserSession, companyDTO.CustomerMembershipTextAlertsEnabled, companyDTO.EnforceInvestmentProductExemptions, companyDTO.EnforceMobileToBankReconciliationVerification);

                if (companyDTO.IsFileTrackingEnforced)
                    company.EnforceFileTracking();
                else company.ExemptFileTracking();

                _companyRepository.Add(company, serviceHeader);

                return dbContextScope.SaveChanges(serviceHeader) >= 0 ? company.ProjectedAs<CompanyDTO>() : null;
            }
        }

        public bool UpdateCompany(CompanyDTO companyDTO, ServiceHeader serviceHeader)
        {
            var companyBindingModel = companyDTO.ProjectedAs<CompanyBindingModel>();

            companyBindingModel.ValidateAll();

            if (companyBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, companyBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _companyRepository.Get(companyDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var address = new Address(companyDTO.AddressAddressLine1, companyDTO.AddressAddressLine2, companyDTO.AddressStreet, companyDTO.AddressPostalCode, companyDTO.AddressCity, companyDTO.AddressEmail, companyDTO.AddressLandLine, companyDTO.AddressMobileLine);

                    var timeDuration = new TimeDuration(companyDTO.TimeDurationStartTime, companyDTO.TimeDurationEndTime);

                    var current = CompanyFactory.CreateCompany(companyDTO.Description, companyDTO.Vision, companyDTO.Mission, companyDTO.Motto, companyDTO.RegistrationNumber, companyDTO.PersonalIdentificationNumber, companyDTO.ApplicationDisplayName, companyDTO.TransactionReceiptTopIndentation, companyDTO.TransactionReceiptLeftIndentation, companyDTO.TransactionReceiptFooter, companyDTO.FingerprintBiometricThreshold, companyDTO.ApplicationMembershipTextAlertsEnabled, companyDTO.EnforceCustomerAccountMakerChecker, companyDTO.BypassJournalVoucherAudit, companyDTO.BypassCreditBatchAudit, companyDTO.BypassDebitBatchAudit, companyDTO.BypassRefundBatchAudit, companyDTO.BypassWireTransferBatchAudit, companyDTO.BypassLoanDisbursementBatchAudit, companyDTO.BypassJournalReversalBatchAudit, companyDTO.BypassInterAccountTransferBatchAudit, companyDTO.BypassExpensePayableAudit, companyDTO.BypassGeneralLedgerAudit, companyDTO.ExcludeChargesInTransactionReceipt, companyDTO.MembershipTerminationNoticePeriod, companyDTO.ExcludeChequeMaturityDateInTransactionReceipt, companyDTO.TrackGuarantorCommittedInvestments, companyDTO.TransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement, companyDTO.ReceiveLoanRequestBeforeLoanRegistration, address, timeDuration, companyDTO.RecoveryPriority, companyDTO.IsWithholdingTaxAgent, companyDTO.EnforceBudgetControl, companyDTO.LocalizeOnlineNotifications, companyDTO.ExcludeCustomerAccountBalanceInTransactionReceipt, companyDTO.EnforceFixedDepositBands, companyDTO.EnforceBiometricsForCashWithdrawal, companyDTO.EnforceTwoFactorAuthentication,companyDTO.RecoverArrearsOnCashDeposit, companyDTO.RecoverArrearsOnExternalChequeClearance, companyDTO.RecoverArrearsOnFixedDepositPayment, companyDTO.AllowDebitBatchToOverdrawAccount, companyDTO.EnforceTellerLimits, companyDTO.EnforceSystemLock, companyDTO.EnforceTellerCashTransferAcknowledgement, companyDTO.EnforceSingleUserSession, companyDTO.CustomerMembershipTextAlertsEnabled, companyDTO.EnforceInvestmentProductExemptions, companyDTO.EnforceMobileToBankReconciliationVerification);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (companyDTO.IsFileTrackingEnforced)
                        current.EnforceFileTracking();
                    else current.ExemptFileTracking();

                    _companyRepository.Merge(persisted, current, serviceHeader);

                    // Lock?
                    if (companyDTO.IsLocked && !persisted.IsLocked)
                        LockCompany(persisted.Id, serviceHeader);
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<CompanyDTO> FindCompanies(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _companyRepository.GetAll<CompanyDTO>(serviceHeader);
            }
        }

        public PageCollectionInfo<CompanyDTO> FindCompanies(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CompanySpecifications.DefaultSpec();

                ISpecification<Company> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _companyRepository.AllMatchingPaged<CompanyDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public PageCollectionInfo<CompanyDTO> FindCompanies(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CompanySpecifications.CompanyFullText(text);

                ISpecification<Company> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return _companyRepository.AllMatchingPaged<CompanyDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public CompanyDTO FindCompany(Guid companyId, ServiceHeader serviceHeader)
        {
            if (companyId == Guid.Empty) return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return _companyRepository.Get<CompanyDTO>(companyId, serviceHeader);
            }
        }

        private bool LockCompany(Guid companyId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _companyRepository.Get(companyId, serviceHeader);

                if (persisted != null)
                {
                    persisted.Lock();
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public List<DebitTypeDTO> FindDebitTypes(Guid companyId, ServiceHeader serviceHeader)
        {
            if (companyId == Guid.Empty) return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CompanyDebitTypeSpecifications.CompanyDebitTypeWithCompanyId(companyId);

                ISpecification<CompanyDebitType> spec = filter;

                var companyDebitTypes = _companyDebitTypeRepository.AllMatching(spec, serviceHeader);

                if (companyDebitTypes != null && companyDebitTypes.Any())
                {
                    var projection = companyDebitTypes.ProjectedAsCollection<CompanyDebitTypeDTO>();

                    return (from p in projection select p.DebitType).ToList();
                }
                else return null;
            }
        }

        public List<DebitTypeDTO> FindCachedDebitTypes(Guid companyId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<DebitTypeDTO>>(string.Format("DebitTypesByCompanyId_{0}_{1}", serviceHeader.ApplicationDomainName, companyId.ToString("D")), () =>
            {
                return FindDebitTypes(companyId, serviceHeader);
            });
        }

        public bool UpdateDebitTypes(Guid companyId, List<DebitTypeDTO> debitTypeDTOs, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _companyRepository.Get(companyId, serviceHeader);

                if (persisted != null)
                {
                    var filter = CompanyDebitTypeSpecifications.CompanyDebitTypeWithCompanyId(companyId);

                    ISpecification<CompanyDebitType> spec = filter;

                    var companyDebitTypes = _companyDebitTypeRepository.AllMatching(spec, serviceHeader);

                    if (companyDebitTypes != null)
                    {
                        companyDebitTypes.ToList().ForEach(x => _companyDebitTypeRepository.Remove(x, serviceHeader));
                    }

                    if (debitTypeDTOs != null && debitTypeDTOs.Any())
                    {
                        foreach (var item in debitTypeDTOs)
                        {
                            var companyDebitType = CompanyDebitTypeFactory.CreateCompanyDebitType(persisted.Id, item.Id);
                            companyDebitType.CreatedBy = serviceHeader.ApplicationUserName;

                            _companyDebitTypeRepository.Add(companyDebitType, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public ProductCollectionInfo FindAttachedProducts(Guid companyId, ServiceHeader serviceHeader, bool useCache)
        {
            if (companyId == Guid.Empty) return null;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CompanyAttachedProductSpecifications.CompanyAttachedProductWithCompanyId(companyId);

                ISpecification<CompanyAttachedProduct> spec = filter;

                var companyAttachedProducts = _companyAttachedProductRepository.AllMatching(spec, serviceHeader);

                if (companyAttachedProducts != null && companyAttachedProducts.Any())
                {
                    var projection = companyAttachedProducts.ProjectedAsCollection<CompanyAttachedProductDTO>();

                    var investmentProductDTOs = new List<InvestmentProductDTO>();

                    var savingsProductDTOs = new List<SavingsProductDTO>();

                    foreach (var item in projection)
                    {
                        switch ((ProductCode)item.ProductCode)
                        {
                            case ProductCode.Investment:

                                var targetInvestmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.TargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.TargetProductId, serviceHeader);

                                if (targetInvestmentProduct != null)
                                {
                                    investmentProductDTOs.Add(targetInvestmentProduct);
                                }

                                break;
                            case ProductCode.Savings:

                                var targetSavingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.TargetProductId, Guid.Empty, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.TargetProductId, Guid.Empty, serviceHeader);

                                if (targetSavingsProduct != null)
                                {
                                    savingsProductDTOs.Add(targetSavingsProduct);
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    return new ProductCollectionInfo { InvestmentProductCollection = investmentProductDTOs, SavingsProductCollection = savingsProductDTOs };
                }
                else return null;
            }
        }

        public ProductCollectionInfo FindCachedAttachedProducts(Guid companyId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<ProductCollectionInfo>(string.Format("AttachedProductsByCompanyId_{0}_{1}", serviceHeader.ApplicationDomainName, companyId.ToString("D")), () =>
            {
                return FindAttachedProducts(companyId, serviceHeader, false);
            });
        }

        public bool UpdateAttachedProducts(Guid companyId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader)
        {
            if (companyId == Guid.Empty) return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _companyRepository.Get(companyId, serviceHeader);

                if (persisted != null)
                {
                    var filter = CompanyAttachedProductSpecifications.CompanyAttachedProductWithCompanyId(companyId);

                    ISpecification<CompanyAttachedProduct> spec = filter;

                    var companyAttachedProducts = _companyAttachedProductRepository.AllMatching(spec, serviceHeader);

                    if (companyAttachedProducts != null)
                    {
                        companyAttachedProducts.ToList().ForEach(x => _companyAttachedProductRepository.Remove(x, serviceHeader));
                    }

                    if (attachedProductsTuple != null && attachedProductsTuple.InvestmentProductCollection != null && attachedProductsTuple.InvestmentProductCollection.Any())
                    {
                        foreach (var item in attachedProductsTuple.InvestmentProductCollection)
                        {
                            var companyAttachedProduct = CompanyAttachedProductFactory.CreateCompanyAttachedProduct(persisted.Id, (int)ProductCode.Investment, item.Id);
                            companyAttachedProduct.CreatedBy = serviceHeader.ApplicationUserName;

                            _companyAttachedProductRepository.Add(companyAttachedProduct, serviceHeader);
                        }
                    }

                    if (attachedProductsTuple.SavingsProductCollection != null && attachedProductsTuple.SavingsProductCollection.Any())
                    {
                        foreach (var item in attachedProductsTuple.SavingsProductCollection)
                        {
                            var companyAttachedProduct = CompanyAttachedProductFactory.CreateCompanyAttachedProduct(persisted.Id, (int)ProductCode.Savings, item.Id);
                            companyAttachedProduct.CreatedBy = serviceHeader.ApplicationUserName;

                            _companyAttachedProductRepository.Add(companyAttachedProduct, serviceHeader);
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public async Task<int> GetCompaniesCountAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _companyRepository.CountAllAsync(serviceHeader);
            }
        }
    }
}
