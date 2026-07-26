using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanCycleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAppraisalProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxiliaryConditionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxilliaryAppraisalFactorAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDeductibleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDynamicChargeAgg;
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

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class LoanProductAppService : ILoanProductAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LoanProduct> _loanProductRepository;
        private readonly IRepository<LoanProductDynamicCharge> _loanProductDynamicChargeRepository;
        private readonly IRepository<LoanProductAppraisalProduct> _loanProductAppraisalProductRepository;
        private readonly IRepository<LoanCycle> _loanCycleRepository;
        private readonly IRepository<LoanProductAuxiliaryCondition> _loanProductAuxiliaryConditionRepository;
        private readonly IRepository<LoanProductDeductible> _loanProductDeductibleRepository;
        private readonly IRepository<LoanProductAuxilliaryAppraisalFactor> _loanProductAuxiliaryAppraisalFactorRepository;
        private readonly IRepository<LoanProductCommission> _loanProductCommissionRepository;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IAppCache _appCache;

        public LoanProductAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LoanProduct> loanProductRepository,
           IRepository<LoanProductDynamicCharge> loanProductDynamicChargeRepository,
           IRepository<LoanProductAppraisalProduct> loanProductAppraisalProductRepository,
           IRepository<LoanCycle> loanCycleRepository,
           IRepository<LoanProductAuxiliaryCondition> loanProductAuxiliaryConditionRepository,
           IRepository<LoanProductDeductible> loanProductDeductibleRepository,
           IRepository<LoanProductAuxilliaryAppraisalFactor> loanProductAuxiliaryAppraisalFactorRepository,
           IRepository<LoanProductCommission> loanProductCommissionRepository,
           IInvestmentProductAppService investmentProductAppService,
           ISavingsProductAppService savingsProductAppService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (loanProductRepository == null)
                throw new ArgumentNullException(nameof(loanProductRepository));

            if (loanProductDynamicChargeRepository == null)
                throw new ArgumentNullException(nameof(loanProductDynamicChargeRepository));

            if (loanProductAppraisalProductRepository == null)
                throw new ArgumentNullException(nameof(loanProductAppraisalProductRepository));

            if (loanCycleRepository == null)
                throw new ArgumentNullException(nameof(loanCycleRepository));

            if (loanProductAuxiliaryConditionRepository == null)
                throw new ArgumentNullException(nameof(loanProductAuxiliaryConditionRepository));

            if (loanProductDeductibleRepository == null)
                throw new ArgumentNullException(nameof(loanProductDeductibleRepository));

            if (loanProductAuxiliaryAppraisalFactorRepository == null)
                throw new ArgumentNullException(nameof(loanProductAuxiliaryAppraisalFactorRepository));

            if (loanProductCommissionRepository == null)
                throw new ArgumentNullException(nameof(loanProductCommissionRepository));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _loanProductRepository = loanProductRepository;
            _loanProductDynamicChargeRepository = loanProductDynamicChargeRepository;
            _loanProductAppraisalProductRepository = loanProductAppraisalProductRepository;
            _loanCycleRepository = loanCycleRepository;
            _loanProductAuxiliaryConditionRepository = loanProductAuxiliaryConditionRepository;
            _loanProductDeductibleRepository = loanProductDeductibleRepository;
            _loanProductAuxiliaryAppraisalFactorRepository = loanProductAuxiliaryAppraisalFactorRepository;
            _loanProductCommissionRepository = loanProductCommissionRepository;
            _investmentProductAppService = investmentProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _appCache = appCache;
        }

        public LoanProductDTO AddNewLoanProduct(LoanProductDTO loanProductDTO, ServiceHeader serviceHeader)
        {
            if (loanProductDTO != null && loanProductDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var loanInterest = new LoanInterest(loanProductDTO.LoanInterestAnnualPercentageRate, loanProductDTO.LoanInterestChargeMode, loanProductDTO.LoanInterestRecoveryMode, loanProductDTO.LoanInterestCalculationMode);

                    var loanRegistration = new LoanRegistration(loanProductDTO.LoanRegistrationTermInMonths, loanProductDTO.LoanRegistrationMinimumAmount, loanProductDTO.LoanRegistrationMaximumAmount, loanProductDTO.LoanRegistrationMinimumInterestAmount, loanProductDTO.LoanRegistrationLoanProductSection, loanProductDTO.LoanRegistrationLoanProductCategory, loanProductDTO.LoanRegistrationConsecutiveIncome, loanProductDTO.LoanRegistrationInvestmentsMultiplier, loanProductDTO.LoanRegistrationMinimumGuarantors, loanProductDTO.LoanRegistrationMaximumGuarantees, loanProductDTO.LoanRegistrationRejectIfMemberHasBalance, loanProductDTO.LoanRegistrationSecurityRequired, loanProductDTO.LoanRegistrationAllowSelfGuarantee, loanProductDTO.LoanRegistrationGracePeriod, loanProductDTO.LoanRegistrationMinimumMembershipPeriod, loanProductDTO.LoanRegistrationPaymentFrequencyPerYear, loanProductDTO.LoanRegistrationPaymentDueDate, loanProductDTO.LoanRegistrationPayoutRecoveryMode, loanProductDTO.LoanRegistrationPayoutRecoveryPercentage, loanProductDTO.LoanRegistrationAggregateCheckOffRecoveryMode, loanProductDTO.LoanRegistrationChargeClearanceFee, loanProductDTO.LoanRegistrationMicrocredit, loanProductDTO.LoanRegistrationStandingOrderTrigger, loanProductDTO.LoanRegistrationTrackArrears, loanProductDTO.LoanRegistrationChargeArrearsFee, loanProductDTO.LoanRegistrationEnforceSystemAppraisalRecommendation, loanProductDTO.LoanRegistrationBypassAudit, loanProductDTO.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage, loanProductDTO.LoanRegistrationGuarantorSecurityMode, loanProductDTO.LoanRegistrationRoundingType, loanProductDTO.LoanRegistrationDisburseMicroLoanLessDeductions, loanProductDTO.LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement, loanProductDTO.LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal, loanProductDTO.LoanRegistrationThrottleScheduledArrearsRecovery, loanProductDTO.LoanRegistrationCreateStandingOrderOnLoanAudit);

                    var takeHome = new Charge(loanProductDTO.TakeHomeType, loanProductDTO.TakeHomePercentage, loanProductDTO.TakeHomeFixedAmount);

                    var loanProduct = LoanProductFactory.CreateLoanProduct(loanProductDTO.ChartOfAccountId, loanProductDTO.InterestReceivedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.Description, loanInterest, loanRegistration, takeHome, loanProductDTO.Priority);

                    loanProduct.Code = (short)_loanProductRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(Code),0) + 1 AS Expr1 FROM {0}LoanProducts", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                    if (loanProductDTO.IsLocked)
                        loanProduct.Lock();
                    else loanProduct.UnLock();

                    _loanProductRepository.Add(loanProduct, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return loanProduct.ProjectedAs<LoanProductDTO>();
                }
            }
            else return null;
        }

        public bool UpdateLoanProduct(LoanProductDTO loanProductDTO, ServiceHeader serviceHeader)
        {
            if (loanProductDTO == null || loanProductDTO.Id == Guid.Empty || loanProductDTO.ChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _loanProductRepository.Get(loanProductDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var loanInterest = new LoanInterest(loanProductDTO.LoanInterestAnnualPercentageRate, loanProductDTO.LoanInterestChargeMode, loanProductDTO.LoanInterestRecoveryMode, loanProductDTO.LoanInterestCalculationMode);

                    var loanRegistration = new LoanRegistration(loanProductDTO.LoanRegistrationTermInMonths, loanProductDTO.LoanRegistrationMinimumAmount, loanProductDTO.LoanRegistrationMaximumAmount, loanProductDTO.LoanRegistrationMinimumInterestAmount, loanProductDTO.LoanRegistrationLoanProductSection, loanProductDTO.LoanRegistrationLoanProductCategory, loanProductDTO.LoanRegistrationConsecutiveIncome, loanProductDTO.LoanRegistrationInvestmentsMultiplier, loanProductDTO.LoanRegistrationMinimumGuarantors, loanProductDTO.LoanRegistrationMaximumGuarantees, loanProductDTO.LoanRegistrationRejectIfMemberHasBalance, loanProductDTO.LoanRegistrationSecurityRequired, loanProductDTO.LoanRegistrationAllowSelfGuarantee, loanProductDTO.LoanRegistrationGracePeriod, loanProductDTO.LoanRegistrationMinimumMembershipPeriod, loanProductDTO.LoanRegistrationPaymentFrequencyPerYear, loanProductDTO.LoanRegistrationPaymentDueDate, loanProductDTO.LoanRegistrationPayoutRecoveryMode, loanProductDTO.LoanRegistrationPayoutRecoveryPercentage, loanProductDTO.LoanRegistrationAggregateCheckOffRecoveryMode, loanProductDTO.LoanRegistrationChargeClearanceFee, loanProductDTO.LoanRegistrationMicrocredit, loanProductDTO.LoanRegistrationStandingOrderTrigger, loanProductDTO.LoanRegistrationTrackArrears, loanProductDTO.LoanRegistrationChargeArrearsFee, loanProductDTO.LoanRegistrationEnforceSystemAppraisalRecommendation, loanProductDTO.LoanRegistrationBypassAudit, loanProductDTO.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage, loanProductDTO.LoanRegistrationGuarantorSecurityMode, loanProductDTO.LoanRegistrationRoundingType, loanProductDTO.LoanRegistrationDisburseMicroLoanLessDeductions, loanProductDTO.LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement, loanProductDTO.LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal, loanProductDTO.LoanRegistrationThrottleScheduledArrearsRecovery, loanProductDTO.LoanRegistrationCreateStandingOrderOnLoanAudit);

                    var takeHome = new Charge(loanProductDTO.TakeHomeType, loanProductDTO.TakeHomePercentage, loanProductDTO.TakeHomeFixedAmount);

                    var current = LoanProductFactory.CreateLoanProduct(loanProductDTO.ChartOfAccountId, loanProductDTO.InterestReceivedChartOfAccountId, loanProductDTO.InterestReceivableChartOfAccountId, loanProductDTO.InterestChargedChartOfAccountId, loanProductDTO.Description, loanInterest, loanRegistration, takeHome, loanProductDTO.Priority);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Code = persisted.Code;


                    if (loanProductDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _loanProductRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<LoanProductDTO> FindLoanProducts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<LoanProduct> spec = LoanProductSpecifications.DefaultSpec();

                var loanProducts = _loanProductRepository.AllMatching(spec, serviceHeader);

                if (loanProducts != null && loanProducts.Any())
                {
                    return loanProducts.ProjectedAsCollection<LoanProductDTO>();
                }
                else return null;
            }
        }

        public List<LoanProductDTO> FindCachedLoanProducts(ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<LoanProductDTO>>(string.Format("LoanProducts_{0}", serviceHeader.ApplicationDomainName), () =>
            {
                return FindLoanProducts(serviceHeader);
            });
        }

        public List<LoanProductDTO> FindLoanProducts(int code, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<LoanProduct> spec = LoanProductSpecifications.LoanProductWithCode(code);

                var loanProducts = _loanProductRepository.AllMatching(spec, serviceHeader);

                if (loanProducts != null && loanProducts.Any())
                {
                    return loanProducts.ProjectedAsCollection<LoanProductDTO>();
                }
                else return null;
            }
        }

        public List<LoanProductDTO> FindLoanProductsByInterestChargeMode(int interestChargeMode, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<LoanProduct> spec = LoanProductSpecifications.LoanProductWithInterestChargeMode(interestChargeMode);

                var loanProducts = _loanProductRepository.AllMatching(spec, serviceHeader);

                if (loanProducts != null && loanProducts.Any())
                {
                    return loanProducts.ProjectedAsCollection<LoanProductDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanProductDTO> FindLoanProducts(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanProductSpecifications.DefaultSpec();

                ISpecification<LoanProduct> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanProductCollection = _loanProductRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanProductCollection != null)
                {
                    var pageCollection = loanProductCollection.PageCollection.ProjectedAsCollection<LoanProductDTO>();

                    var itemsCount = loanProductCollection.ItemsCount;

                    return new PageCollectionInfo<LoanProductDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanProductDTO> FindLoanProducts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanProductSpecifications.LoanProductFullText(text);

                ISpecification<LoanProduct> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanProductCollection = _loanProductRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanProductCollection != null)
                {
                    var pageCollection = loanProductCollection.PageCollection.ProjectedAsCollection<LoanProductDTO>();

                    var itemsCount = loanProductCollection.ItemsCount;

                    return new PageCollectionInfo<LoanProductDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanProductDTO> FindLoanProducts(int loanProductSection, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanProductSpecifications.LoanProductFullText(loanProductSection, text);

                ISpecification<LoanProduct> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanProductCollection = _loanProductRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanProductCollection != null)
                {
                    var pageCollection = loanProductCollection.PageCollection.ProjectedAsCollection<LoanProductDTO>();

                    var itemsCount = loanProductCollection.ItemsCount;

                    return new PageCollectionInfo<LoanProductDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public LoanProductDTO FindLoanProduct(Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanProduct = _loanProductRepository.Get(loanProductId, serviceHeader);

                    if (loanProduct != null)
                    {
                        return loanProduct.ProjectedAs<LoanProductDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public LoanProductDTO FindLoanProductByName(string name, ServiceHeader serviceHeader)
        {
            if (name != null)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductSpecifications.LoanProductByName(name);

                    ISpecification<LoanProduct> spec = filter;

                    var loanProduct = _loanProductRepository.AllMatching(spec, serviceHeader).SingleOrDefault();

                    if (loanProduct != null)
                    {
                        return loanProduct.ProjectedAs<LoanProductDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public LoanProductDTO FindCachedLoanProduct(Guid loanProductId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<LoanProductDTO>(loanProductId.ToString("D"), () =>
            {
                return FindLoanProduct(loanProductId, serviceHeader);
            });
        }

        public List<DynamicChargeDTO> FindDynamicCharges(Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductDynamicChargeSpecifications.LoanProductDynamicChargeWithLoanProductId(loanProductId);

                    ISpecification<LoanProductDynamicCharge> spec = filter;

                    var loanProductDynamicCharges = _loanProductDynamicChargeRepository.AllMatching(spec, serviceHeader);

                    if (loanProductDynamicCharges != null)
                    {
                        var projection = loanProductDynamicCharges.ProjectedAsCollection<LoanProductDynamicChargeDTO>();

                        return (from p in projection select p.DynamicCharge).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DynamicChargeDTO> FindDynamicCharges(Guid loanProductId, int recoveryMode, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductDynamicChargeSpecifications.LoanProductDynamicChargeWithLoanProductIdAndRecoveryMode(loanProductId, recoveryMode);

                    ISpecification<LoanProductDynamicCharge> spec = filter;

                    var loanProductDynamicCharges = _loanProductDynamicChargeRepository.AllMatching(spec, serviceHeader);

                    if (loanProductDynamicCharges != null)
                    {
                        var projection = loanProductDynamicCharges.ProjectedAsCollection<LoanProductDynamicChargeDTO>();

                        return (from p in projection select p.DynamicCharge).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DynamicChargeDTO> FindCachedDynamicCharges(Guid loanProductId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<DynamicChargeDTO>>(string.Format("DynamicChargesByLoanProductId_{0}_{1}", serviceHeader.ApplicationDomainName, loanProductId.ToString("D")), () =>
            {
                return FindDynamicCharges(loanProductId, serviceHeader);
            });
        }

        public bool UpdateDynamicCharges(Guid loanProductId, List<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader)
        {
            if (loanProductId != null && dynamicCharges != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanProductRepository.Get(loanProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = LoanProductDynamicChargeSpecifications.LoanProductDynamicChargeWithLoanProductId(loanProductId);

                        ISpecification<LoanProductDynamicCharge> spec = filter;

                        var loanProductDynamicCharges = _loanProductDynamicChargeRepository.AllMatching(spec, serviceHeader);

                        if (loanProductDynamicCharges != null)
                        {
                            loanProductDynamicCharges.ToList().ForEach(x => _loanProductDynamicChargeRepository.Remove(x, serviceHeader));
                        }

                        if (dynamicCharges.Any())
                        {
                            foreach (var item in dynamicCharges)
                            {
                                var loanProductDynamicCharge = LoanProductDynamicChargeFactory.CreateLoanProductDynamicCharge(persisted.Id, item.Id);

                                _loanProductDynamicChargeRepository.Add(loanProductDynamicCharge, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public ProductCollectionInfo FindAppraisalProducts(Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductAppraisalProductSpecifications.LoanProductAppraisalProductWithLoanProductId(loanProductId);

                    ISpecification<LoanProductAppraisalProduct> spec = filter;

                    var loanProductAppraisalProducts = _loanProductAppraisalProductRepository.AllMatching(spec, serviceHeader);

                    if (loanProductAppraisalProducts != null && loanProductAppraisalProducts.Any())
                    {
                        var projection = loanProductAppraisalProducts.ProjectedAsCollection<LoanProductAppraisalProductDTO>();

                        var investmentsQualificationInvestmentProductDTOs = new List<InvestmentProductDTO>();
                        var loanRecoveryloanProductDTOs = new List<LoanProductDTO>();
                        var eligibleIncomeDeductionLoanProductDTOs = new List<LoanProductDTO>();
                        var eligibleIncomeDeductionInvestmentProductDTOs = new List<InvestmentProductDTO>();
                        var eligibleIncomeDeductionSavingsProductDTOs = new List<SavingsProductDTO>();

                        foreach (var item in projection)
                        {
                            switch ((ProductCode)item.ProductCode)
                            {
                                case ProductCode.Loan:

                                    var targetLoanProduct = FindLoanProduct(item.TargetProductId, serviceHeader);

                                    if (targetLoanProduct != null)
                                    {
                                        switch ((AppraisalProductPurpose)item.Purpose)
                                        {
                                            case AppraisalProductPurpose.LoanRecovery:
                                                loanRecoveryloanProductDTOs.Add(targetLoanProduct);
                                                break;
                                            case AppraisalProductPurpose.InvestmentsQualification:
                                                break;
                                            case AppraisalProductPurpose.EligibleIncomeDeduction:
                                                eligibleIncomeDeductionLoanProductDTOs.Add(targetLoanProduct);
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    break;
                                case ProductCode.Investment:

                                    var targetInvestmentProduct = _investmentProductAppService.FindInvestmentProduct(item.TargetProductId, serviceHeader);

                                    if (targetInvestmentProduct != null)
                                    {
                                        switch ((AppraisalProductPurpose)item.Purpose)
                                        {
                                            case AppraisalProductPurpose.LoanRecovery:
                                                break;
                                            case AppraisalProductPurpose.InvestmentsQualification:
                                                investmentsQualificationInvestmentProductDTOs.Add(targetInvestmentProduct);
                                                break;
                                            case AppraisalProductPurpose.EligibleIncomeDeduction:
                                                eligibleIncomeDeductionInvestmentProductDTOs.Add(targetInvestmentProduct);
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    break;
                                case ProductCode.Savings:

                                    var targetSavingsProduct = _savingsProductAppService.FindSavingsProduct(item.TargetProductId, Guid.Empty, serviceHeader);

                                    if (targetSavingsProduct != null)
                                    {
                                        switch ((AppraisalProductPurpose)item.Purpose)
                                        {
                                            case AppraisalProductPurpose.LoanRecovery:
                                                break;
                                            case AppraisalProductPurpose.InvestmentsQualification:
                                                break;
                                            case AppraisalProductPurpose.EligibleIncomeDeduction:
                                                eligibleIncomeDeductionSavingsProductDTOs.Add(targetSavingsProduct);
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

                        return new ProductCollectionInfo
                        {
                            InvestmentProductCollection = investmentsQualificationInvestmentProductDTOs,
                            LoanProductCollection = loanRecoveryloanProductDTOs,
                            EligibileIncomeDeductionLoanProductCollection = eligibleIncomeDeductionLoanProductDTOs,
                            EligibileIncomeDeductionInvestmentProductCollection = eligibleIncomeDeductionInvestmentProductDTOs,
                            EligibileIncomeDeductionSavingsProductCollection = eligibleIncomeDeductionSavingsProductDTOs,
                        };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateAppraisalProducts(Guid loanProductId, ProductCollectionInfo appraisalProductsTuple, ServiceHeader serviceHeader)
        {
            if (loanProductId != null && appraisalProductsTuple != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanProductRepository.Get(loanProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = LoanProductAppraisalProductSpecifications.LoanProductAppraisalProductWithLoanProductId(loanProductId);

                        ISpecification<LoanProductAppraisalProduct> spec = filter;

                        var loanProductAppraisalProducts = _loanProductAppraisalProductRepository.AllMatching(spec, serviceHeader);

                        if (loanProductAppraisalProducts != null)
                        {
                            loanProductAppraisalProducts.ToList().ForEach(x => _loanProductAppraisalProductRepository.Remove(x, serviceHeader));
                        }

                        if (appraisalProductsTuple.InvestmentProductCollection != null && appraisalProductsTuple.InvestmentProductCollection.Any())
                        {
                            foreach (var item in appraisalProductsTuple.InvestmentProductCollection)
                            {
                                var loanProductAppraisalProduct = LoanProductAppraisalProductFactory.CreateLoanAppraisalProduct(persisted.Id, (int)ProductCode.Investment, item.Id, (int)AppraisalProductPurpose.InvestmentsQualification);

                                _loanProductAppraisalProductRepository.Add(loanProductAppraisalProduct, serviceHeader);
                            }
                        }

                        if (appraisalProductsTuple.LoanProductCollection != null && appraisalProductsTuple.LoanProductCollection.Any())
                        {
                            foreach (var item in appraisalProductsTuple.LoanProductCollection)
                            {
                                var loanProductAppraisalProduct = LoanProductAppraisalProductFactory.CreateLoanAppraisalProduct(persisted.Id, (int)ProductCode.Loan, item.Id, (int)AppraisalProductPurpose.LoanRecovery);

                                _loanProductAppraisalProductRepository.Add(loanProductAppraisalProduct, serviceHeader);
                            }
                        }

                        if (appraisalProductsTuple.EligibileIncomeDeductionLoanProductCollection != null && appraisalProductsTuple.EligibileIncomeDeductionLoanProductCollection.Any())
                        {
                            foreach (var item in appraisalProductsTuple.EligibileIncomeDeductionLoanProductCollection)
                            {
                                var loanProductAppraisalProduct = LoanProductAppraisalProductFactory.CreateLoanAppraisalProduct(persisted.Id, (int)ProductCode.Loan, item.Id, (int)AppraisalProductPurpose.EligibleIncomeDeduction);

                                _loanProductAppraisalProductRepository.Add(loanProductAppraisalProduct, serviceHeader);
                            }
                        }

                        if (appraisalProductsTuple.EligibileIncomeDeductionInvestmentProductCollection != null && appraisalProductsTuple.EligibileIncomeDeductionInvestmentProductCollection.Any())
                        {
                            foreach (var item in appraisalProductsTuple.EligibileIncomeDeductionInvestmentProductCollection)
                            {
                                var loanProductAppraisalProduct = LoanProductAppraisalProductFactory.CreateLoanAppraisalProduct(persisted.Id, (int)ProductCode.Investment, item.Id, (int)AppraisalProductPurpose.EligibleIncomeDeduction);

                                _loanProductAppraisalProductRepository.Add(loanProductAppraisalProduct, serviceHeader);
                            }
                        }

                        if (appraisalProductsTuple.EligibileIncomeDeductionSavingsProductCollection != null && appraisalProductsTuple.EligibileIncomeDeductionSavingsProductCollection.Any())
                        {
                            foreach (var item in appraisalProductsTuple.EligibileIncomeDeductionSavingsProductCollection)
                            {
                                var loanProductAppraisalProduct = LoanProductAppraisalProductFactory.CreateLoanAppraisalProduct(persisted.Id, (int)ProductCode.Savings, item.Id, (int)AppraisalProductPurpose.EligibleIncomeDeduction);

                                _loanProductAppraisalProductRepository.Add(loanProductAppraisalProduct, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<LoanCycleDTO> FindLoanCycles(Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCycleSpecifications.LoanCycleWithLoanProductId(loanProductId);

                    ISpecification<LoanCycle> spec = filter;

                    var loanCycleCollection = _loanCycleRepository.AllMatching(spec, serviceHeader);

                    if (loanCycleCollection != null)
                    {
                        return loanCycleCollection.ProjectedAsCollection<LoanCycleDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateLoanCycles(Guid loanProductId, List<LoanCycleDTO> loanCycles, ServiceHeader serviceHeader)
        {
            if (loanProductId != null && loanCycles != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanProductRepository.Get(loanProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLoanCycles(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var loanCycle = _loanCycleRepository.Get(item.Id, serviceHeader);

                                if (loanCycle != null)
                                {
                                    _loanCycleRepository.Remove(loanCycle, serviceHeader);
                                }
                            }
                        }

                        if (loanCycles.Any())
                        {
                            foreach (var item in loanCycles)
                            {
                                var range = new Range(item.RangeLowerLimit, item.RangeUpperLimit);

                                var loanCycle = LoanCycleFactory.CreateLoanCycle(persisted.Id, range);

                                _loanCycleRepository.Add(loanCycle, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<LoanProductAuxiliaryConditionDTO> FindLoanProductAuxiliaryConditions(Guid baseLoanProductId, ServiceHeader serviceHeader)
        {
            if (baseLoanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductAuxiliaryConditionSpecifications.LoanProductAuxiliaryConditionWithBaseLoanProductId(baseLoanProductId);

                    ISpecification<LoanProductAuxiliaryCondition> spec = filter;

                    var loanProductAuxiliaryConditions = _loanProductAuxiliaryConditionRepository.AllMatching(spec, serviceHeader);

                    if (loanProductAuxiliaryConditions != null)
                    {
                        return loanProductAuxiliaryConditions.ProjectedAsCollection<LoanProductAuxiliaryConditionDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateLoanProductAuxiliaryConditions(Guid baseLoanProductId, List<LoanProductAuxiliaryConditionDTO> loanProductAuxiliaryConditions, ServiceHeader serviceHeader)
        {
            if (baseLoanProductId != null && loanProductAuxiliaryConditions != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanProductRepository.Get(baseLoanProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLoanProductAuxiliaryConditions(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var loanProductAuxiliaryCondition = _loanProductAuxiliaryConditionRepository.Get(item.Id, serviceHeader);

                                if (loanProductAuxiliaryCondition != null)
                                {
                                    _loanProductAuxiliaryConditionRepository.Remove(loanProductAuxiliaryCondition, serviceHeader);
                                }
                            }
                        }

                        if (loanProductAuxiliaryConditions.Any())
                        {
                            foreach (var item in loanProductAuxiliaryConditions)
                            {
                                var loanProductAuxiliaryCondition = LoanProductAuxiliaryConditionFactory.CreateLoanAppraisalProduct(persisted.Id, item.TargetLoanProductId, item.Condition, item.MaximumEligiblePercentage);

                                _loanProductAuxiliaryConditionRepository.Add(loanProductAuxiliaryCondition, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<LoanProductDeductibleDTO> FindLoanProductDeductibles(Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductDeductibleSpecifications.LoanProductDeductibleWithLoanProductId(loanProductId);

                    ISpecification<LoanProductDeductible> spec = filter;

                    var deductibleCollection = _loanProductDeductibleRepository.AllMatching(spec, serviceHeader);

                    if (deductibleCollection != null)
                    {
                        return deductibleCollection.ProjectedAsCollection<LoanProductDeductibleDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateLoanProductDeductibles(Guid loanProductId, List<LoanProductDeductibleDTO> loanProductDeductibles, ServiceHeader serviceHeader)
        {
            if (loanProductId != null && loanProductDeductibles != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanProductRepository.Get(loanProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLoanProductDeductibles(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var loanProductDeductible = _loanProductDeductibleRepository.Get(item.Id, serviceHeader);

                                if (loanProductDeductible != null)
                                {
                                    _loanProductDeductibleRepository.Remove(loanProductDeductible, serviceHeader);
                                }
                            }
                        }

                        if (loanProductDeductibles.Any())
                        {
                            foreach (var item in loanProductDeductibles)
                            {
                                var customerAccountType = new CustomerAccountType(item.CustomerAccountTypeProductCode, item.CustomerAccountTypeTargetProductId, item.CustomerAccountTypeTargetProductCode);

                                var charge = new Charge(item.ChargeType, item.ChargePercentage, item.ChargeFixedAmount);

                                var loanProductDeductible = LoanProductDeductibleFactory.CreateLoanProductDeductible(persisted.Id, item.Description, customerAccountType, charge, item.NetOffInvestmentBalance, item.ComputeChargeOnTopUp);

                                _loanProductDeductibleRepository.Add(loanProductDeductible, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public void FetchLoanProductDeductiblesProductDescription(List<LoanProductDeductibleDTO> loanProductDeductibles, ServiceHeader serviceHeader, bool useCache)
        {
            if (loanProductDeductibles != null && loanProductDeductibles.Any())
            {
                loanProductDeductibles.ForEach(item =>
                {
                    switch ((ProductCode)item.CustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                            var savingsProduct = useCache ? _savingsProductAppService.FindCachedSavingsProduct(item.CustomerAccountTypeTargetProductId, Guid.Empty, serviceHeader) : _savingsProductAppService.FindSavingsProduct(item.CustomerAccountTypeTargetProductId, Guid.Empty, serviceHeader);
                            if (savingsProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = savingsProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductIsDefault = savingsProduct.IsDefault;
                            }
                            break;
                        case ProductCode.Loan:
                            var loanProduct = useCache ? FindCachedLoanProduct(item.CustomerAccountTypeTargetProductId, serviceHeader) : FindLoanProduct(item.CustomerAccountTypeTargetProductId, serviceHeader);
                            if (loanProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = loanProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = loanProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = loanProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId = loanProduct.InterestReceivableChartOfAccountId;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountCode = loanProduct.InterestReceivableChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountName = loanProduct.InterestReceivableChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = loanProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductLoanProductSection = loanProduct.LoanRegistrationLoanProductSection;
                            }
                            break;
                        case ProductCode.Investment:
                            var investmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.CustomerAccountTypeTargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.CustomerAccountTypeTargetProductId, serviceHeader);
                            if (investmentProduct != null)
                            {
                                item.CustomerAccountTypeTargetProductChartOfAccountId = investmentProduct.ChartOfAccountId;
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = investmentProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = investmentProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = investmentProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductIsRefundable = investmentProduct.IsRefundable;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public List<LoanProductAuxilliaryAppraisalFactorDTO> FindLoanProductAuxilliaryAppraisalFactors(Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductAuxilliaryAppraisalFactorSpecifications.LoanProductAuxilliaryAppraisalFactorWithLoanProductId(loanProductId);

                    ISpecification<LoanProductAuxilliaryAppraisalFactor> spec = filter;

                    var auxilliaryAppraisalFactors = _loanProductAuxiliaryAppraisalFactorRepository.AllMatching(spec, serviceHeader);

                    if (auxilliaryAppraisalFactors != null)
                    {
                        return auxilliaryAppraisalFactors.ProjectedAsCollection<LoanProductAuxilliaryAppraisalFactorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateLoanProductAuxilliaryAppraisalFactors(Guid loanProductId, List<LoanProductAuxilliaryAppraisalFactorDTO> loanProductAuxilliaryAppraisalFactors, ServiceHeader serviceHeader)
        {
            if (loanProductId != null && loanProductAuxilliaryAppraisalFactors != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanProductRepository.Get(loanProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLoanProductAuxilliaryAppraisalFactors(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var loanProductAuxiliaryAppraisalFactor = _loanProductAuxiliaryAppraisalFactorRepository.Get(item.Id, serviceHeader);

                                if (loanProductAuxiliaryAppraisalFactor != null)
                                {
                                    _loanProductAuxiliaryAppraisalFactorRepository.Remove(loanProductAuxiliaryAppraisalFactor, serviceHeader);
                                }
                            }
                        }

                        if (loanProductAuxilliaryAppraisalFactors.Any())
                        {
                            foreach (var item in loanProductAuxilliaryAppraisalFactors)
                            {
                                var range = new Range(item.RangeLowerLimit, item.RangeUpperLimit);

                                var loanProductAuxiliaryAppraisalFactor = LoanProductAuxilliaryAppraisalFactorFactory.CreateLoanProductAuxilliaryAppraisalFactor(persisted.Id, range, item.LoaneeMultiplier, item.GuarantorMultiplier);

                                _loanProductAuxiliaryAppraisalFactorRepository.Add(loanProductAuxiliaryAppraisalFactor, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public double GetLoaneeAppraisalFactor(Guid loanProductId, decimal totalValue, ServiceHeader serviceHeader)
        {
            double result = default(double);

            var loanProduct = FindLoanProduct(loanProductId, serviceHeader);

            if (loanProduct != null)
            {
                var auxilliaryAppraisalFactors = FindLoanProductAuxilliaryAppraisalFactors(loanProductId, serviceHeader);

                if (auxilliaryAppraisalFactors != null && auxilliaryAppraisalFactors.Any())
                {
                    var targetAuxilliaryAppraisalFactor = auxilliaryAppraisalFactors.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                    if (targetAuxilliaryAppraisalFactor != null)
                    {
                        result = targetAuxilliaryAppraisalFactor.LoaneeMultiplier;
                    }
                    else result = loanProduct.LoanRegistrationInvestmentsMultiplier;
                }
                else result = loanProduct.LoanRegistrationInvestmentsMultiplier;
            }

            return result;
        }

        public double GetGuarantorAppraisalFactor(Guid loanProductId, decimal totalValue, ServiceHeader serviceHeader)
        {
            double result = default(double);

            var loanProduct = FindLoanProduct(loanProductId, serviceHeader);

            if (loanProduct != null)
            {
                var auxilliaryAppraisalFactors = FindLoanProductAuxilliaryAppraisalFactors(loanProductId, serviceHeader);

                if (auxilliaryAppraisalFactors != null && auxilliaryAppraisalFactors.Any())
                {
                    var targetAuxilliaryAppraisalFactor = auxilliaryAppraisalFactors.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                    if (targetAuxilliaryAppraisalFactor != null)
                    {
                        result = targetAuxilliaryAppraisalFactor.GuarantorMultiplier;
                    }
                }
            }

            return result == 0 ? 1 : result;
        }

        public List<CommissionDTO> FindCommissions(Guid loanProductId, int loanProductKnownChargeType, ServiceHeader serviceHeader)
        {
            if (loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanProductCommissionSpecifications.LoanProductCommission(loanProductId, loanProductKnownChargeType);

                    ISpecification<LoanProductCommission> spec = filter;

                    var loanProductCommissions = _loanProductCommissionRepository.AllMatching(spec, serviceHeader);

                    if (loanProductCommissions != null)
                    {
                        var loanProductCommissionDTOs = loanProductCommissions.ProjectedAsCollection<LoanProductCommissionDTO>();

                        var projection = (from p in loanProductCommissionDTOs
                                          select new
                                          {
                                              p.ChargeBasisValue,
                                              p.Commission
                                          });

                        foreach (var item in projection)
                            item.Commission.ChargeBasisValue = item.ChargeBasisValue; // map basis value

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionDTO> FindCachedCommissions(Guid loanProductId, int loanProductKnownChargeType, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByLoanProductIdAndLoanProductKnownChargeType_{0}_{1}_{2}", serviceHeader.ApplicationDomainName, loanProductId.ToString("D"), loanProductKnownChargeType), () =>
            {
                return FindCommissions(loanProductId, loanProductKnownChargeType, serviceHeader);
            });
        }

        public bool UpdateCommissions(Guid loanProductId, List<CommissionDTO> commissionDTOs, int loanProductKnownChargeType, int loanProductChargeBasisValue, ServiceHeader serviceHeader)
        {
            if (loanProductId != null && commissionDTOs != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanProductRepository.Get(loanProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = LoanProductCommissionSpecifications.LoanProductCommission(loanProductId, loanProductKnownChargeType);

                        ISpecification<LoanProductCommission> spec = filter;

                        var loanProductCommissions = _loanProductCommissionRepository.AllMatching(spec, serviceHeader);

                        if (loanProductCommissions != null)
                        {
                            loanProductCommissions.ToList().ForEach(x => _loanProductCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (commissionDTOs.Any())
                        {
                            foreach (var item in commissionDTOs)
                            {
                                var loanProductCommission = LoanProductCommissionFactory.CreateLoanProductCommission(persisted.Id, item.Id, loanProductKnownChargeType, loanProductChargeBasisValue);

                                _loanProductCommissionRepository.Add(loanProductCommission, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }
    }
}
