using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAttachedProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeGraduatedScaleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeLevyAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class FixedDepositTypeAppService : IFixedDepositTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<FixedDepositType> _fixedDepositTypeRepository;
        private readonly IRepository<FixedDepositTypeGraduatedScale> _fixedDepositTypeGraduatedScaleRepository;
        private readonly IRepository<FixedDepositTypeLevy> _fixedDepositTypeLevyRepository;
        private readonly IRepository<FixedDepositTypeAttachedProduct> _fixedDepositTypeAttachedProductRepository;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ILevyAppService _levyAppService;
        private readonly IAppCache _appCache;

        public FixedDepositTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<FixedDepositType> fixedDepositTypeRepository,
           IRepository<FixedDepositTypeLevy> fixedDepositTypeLevyRepository,
           IRepository<FixedDepositTypeAttachedProduct> fixedDepositTypeAttachedProductRepository,
           IRepository<FixedDepositTypeGraduatedScale> fixedDepositTypeGraduatedScaleRepository,
           ILoanProductAppService loanProductAppService,
           ILevyAppService levyAppService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (fixedDepositTypeRepository == null)
                throw new ArgumentNullException(nameof(fixedDepositTypeRepository));

            if (fixedDepositTypeLevyRepository == null)
                throw new ArgumentNullException(nameof(fixedDepositTypeLevyRepository));

            if (fixedDepositTypeAttachedProductRepository == null)
                throw new ArgumentNullException(nameof(fixedDepositTypeAttachedProductRepository));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (levyAppService == null)
                throw new ArgumentNullException(nameof(levyAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            if (fixedDepositTypeGraduatedScaleRepository == null)
                throw new ArgumentNullException(nameof(fixedDepositTypeGraduatedScaleRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _fixedDepositTypeRepository = fixedDepositTypeRepository;
            _fixedDepositTypeLevyRepository = fixedDepositTypeLevyRepository;
            _fixedDepositTypeAttachedProductRepository = fixedDepositTypeAttachedProductRepository;
            _loanProductAppService = loanProductAppService;
            _levyAppService = levyAppService;
            _appCache = appCache;
            _fixedDepositTypeGraduatedScaleRepository = fixedDepositTypeGraduatedScaleRepository;
        }

        public FixedDepositTypeDTO AddNewFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeDTO != null)
            {
                if (enforceFixedDepositBands)
                {
                    var fixedDepositTypeDTOs = FindFixedDepositTypesByMonths(fixedDepositTypeDTO.Months, serviceHeader);

                    if (fixedDepositTypeDTOs != null && fixedDepositTypeDTOs.Count() > 0)
                        //throw new InvalidOperationException(string.Format("Months '{0}' already defined.", fixedDepositTypeDTO.Months));
                        fixedDepositTypeDTO.ErrorMessageResult = string.Format("Months '{0}' already defined.", fixedDepositTypeDTO.Months);
                    return fixedDepositTypeDTO;
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var fixedDepositType = FixedDepositTypeFactory.CreateFixedDepositType(fixedDepositTypeDTO.Description, fixedDepositTypeDTO.Months);

                    if (fixedDepositTypeDTO.IsLocked)
                        fixedDepositType.Lock();
                    else fixedDepositType.UnLock();

                    fixedDepositType.CreatedBy = serviceHeader.ApplicationUserName;

                    _fixedDepositTypeRepository.Add(fixedDepositType, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return fixedDepositType.ProjectedAs<FixedDepositTypeDTO>();
                }
            }
            else return null;
        }

        public bool UpdateFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeDTO == null) return false;

            if (enforceFixedDepositBands)
            {
                var fixedDepositTypeDTOs = FindFixedDepositTypes(serviceHeader).Where(x => x.Months == fixedDepositTypeDTO.Months && x.Id != fixedDepositTypeDTO.Id).ToList();

                if (fixedDepositTypeDTOs != null && fixedDepositTypeDTOs.Count() > 0) throw new InvalidOperationException(string.Format("Months '{0}' already defined.", fixedDepositTypeDTO.Months));
            }

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _fixedDepositTypeRepository.Get(fixedDepositTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = FixedDepositTypeFactory.CreateFixedDepositType(fixedDepositTypeDTO.Description, fixedDepositTypeDTO.Months);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (fixedDepositTypeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _fixedDepositTypeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<FixedDepositTypeDTO> FindFixedDepositTypes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var fixedDepositTypes = _fixedDepositTypeRepository.GetAll(serviceHeader);

                if (fixedDepositTypes != null && fixedDepositTypes.Any())
                {
                    return fixedDepositTypes.ProjectedAsCollection<FixedDepositTypeDTO>();
                }
                else return null;
            }
        }

        public List<FixedDepositTypeDTO> FindFixedDepositTypesByMonths(int months, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositTypeSpecifications.FixedDepositTypeByMonths(months);

                ISpecification<FixedDepositType> spec = filter;

                var fixedDepositTypes = _fixedDepositTypeRepository.AllMatching(spec, serviceHeader);

                if (fixedDepositTypes != null && fixedDepositTypes.Any())
                {
                    return fixedDepositTypes.ProjectedAsCollection<FixedDepositTypeDTO>();
                }
                else
                    return null;
            }
        }

        public PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositTypeSpecifications.DefaultSpec();

                ISpecification<FixedDepositType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositTypePagedCollection = _fixedDepositTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositTypePagedCollection != null)
                {
                    var pageCollection = fixedDepositTypePagedCollection.PageCollection.ProjectedAsCollection<FixedDepositTypeDTO>();

                    var itemsCount = fixedDepositTypePagedCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FixedDepositTypeSpecifications.FixedDepositTypeFullText(text);

                ISpecification<FixedDepositType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fixedDepositTypeCollection = _fixedDepositTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fixedDepositTypeCollection != null)
                {
                    var pageCollection = fixedDepositTypeCollection.PageCollection.ProjectedAsCollection<FixedDepositTypeDTO>();

                    var itemsCount = fixedDepositTypeCollection.ItemsCount;

                    return new PageCollectionInfo<FixedDepositTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public FixedDepositTypeDTO FindFixedDepositType(Guid fixedDepositTypeId, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var fixedDepositType = _fixedDepositTypeRepository.Get(fixedDepositTypeId, serviceHeader);

                    if (fixedDepositType != null)
                    {
                        return fixedDepositType.ProjectedAs<FixedDepositTypeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LevyDTO> FindLevies(Guid fixedDepositTypeId, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FixedDepositTypeLevySpecifications.FixedDepositTypeLevyWithFixedDepositTypeId(fixedDepositTypeId);

                    ISpecification<FixedDepositTypeLevy> spec = filter;

                    var fixedDepositTypeLevies = _fixedDepositTypeLevyRepository.AllMatching(spec, serviceHeader);

                    if (fixedDepositTypeLevies != null)
                    {
                        var projection = fixedDepositTypeLevies.ProjectedAsCollection<FixedDepositTypeLevyDTO>();

                        return (from p in projection select p.Levy).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateLevies(Guid fixedDepositTypeId, List<LevyDTO> commissions, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeId != null && commissions != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _fixedDepositTypeRepository.Get(fixedDepositTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = FixedDepositTypeLevySpecifications.FixedDepositTypeLevyWithFixedDepositTypeId(fixedDepositTypeId);

                        ISpecification<FixedDepositTypeLevy> spec = filter;

                        var fixedDepositTypeLevies = _fixedDepositTypeLevyRepository.AllMatching(spec, serviceHeader);

                        if (fixedDepositTypeLevies != null)
                        {
                            fixedDepositTypeLevies.ToList().ForEach(x => _fixedDepositTypeLevyRepository.Remove(x, serviceHeader));
                        }

                        if (commissions.Any())
                        {
                            foreach (var item in commissions)
                            {
                                var fixedDepositTypeLevy = FixedDepositTypeLevyFactory.CreateFixedDepositTypeLevy(persisted.Id, item.Id);

                                _fixedDepositTypeLevyRepository.Add(fixedDepositTypeLevy, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public ProductCollectionInfo FindAttachedProducts(Guid fixedDepositTypeId, ServiceHeader serviceHeader, bool useCache)
        {
            if (fixedDepositTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FixedDepositTypeAttachedProductSpecifications.FixedDepositTypeAttachedProductWithFixedDepositTypeId(fixedDepositTypeId);

                    ISpecification<FixedDepositTypeAttachedProduct> spec = filter;

                    var fixedDepositTypeAttachedProducts = _fixedDepositTypeAttachedProductRepository.AllMatching(spec, serviceHeader);

                    if (fixedDepositTypeAttachedProducts != null && fixedDepositTypeAttachedProducts.Any())
                    {
                        var projection = fixedDepositTypeAttachedProducts.ProjectedAsCollection<FixedDepositTypeAttachedProductDTO>();

                        var investmentProductDTOs = new List<InvestmentProductDTO>();
                        var loanProductDTOs = new List<LoanProductDTO>();

                        foreach (var item in projection)
                        {
                            switch ((ProductCode)item.ProductCode)
                            {
                                case ProductCode.Loan:

                                    var targetLoanProduct = useCache ? _loanProductAppService.FindCachedLoanProduct(item.TargetProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(item.TargetProductId, serviceHeader);

                                    if (targetLoanProduct != null)
                                    {
                                        loanProductDTOs.Add(targetLoanProduct);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        return new ProductCollectionInfo { InvestmentProductCollection = investmentProductDTOs, LoanProductCollection = loanProductDTOs };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public ProductCollectionInfo FindCachedAttachedProducts(Guid fixedDepositTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<ProductCollectionInfo>(string.Format("AttachedProductsByFixedDepositTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, fixedDepositTypeId.ToString("D")), () =>
            {
                return FindAttachedProducts(fixedDepositTypeId, serviceHeader, true);
            });
        }

        public bool UpdateAttachedProducts(Guid fixedDepositTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeId != null && attachedProductsTuple != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _fixedDepositTypeRepository.Get(fixedDepositTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = FixedDepositTypeAttachedProductSpecifications.FixedDepositTypeAttachedProductWithFixedDepositTypeId(fixedDepositTypeId);

                        ISpecification<FixedDepositTypeAttachedProduct> spec = filter;

                        var fixedDepositTypeAttachedProducts = _fixedDepositTypeAttachedProductRepository.AllMatching(spec, serviceHeader);

                        if (fixedDepositTypeAttachedProducts != null)
                        {
                            fixedDepositTypeAttachedProducts.ToList().ForEach(x => _fixedDepositTypeAttachedProductRepository.Remove(x, serviceHeader));
                        }

                        if (attachedProductsTuple.LoanProductCollection != null && attachedProductsTuple.LoanProductCollection.Any())
                        {
                            foreach (var item in attachedProductsTuple.LoanProductCollection)
                            {
                                var fixedDepositTypeAttachedProduct = FixedDepositTypeAttachedProductFactory.CreateFixedDepositTypeAttachedProduct(persisted.Id, (int)ProductCode.Loan, item.Id);

                                _fixedDepositTypeAttachedProductRepository.Add(fixedDepositTypeAttachedProduct, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<TariffWrapper> ComputeTariffs(Guid fixedDepositTypeId, decimal totalValue, Guid debitChartOfAccountId, int debitChartOfAccountCode, string debitChartOfAccountName, ServiceHeader serviceHeader)
        {
            var tariffs = new List<TariffWrapper>();

            var levyDTOs = FindLevies(fixedDepositTypeId, serviceHeader);

            if (levyDTOs != null && levyDTOs.Any())
            {
                foreach (var levyDTO in levyDTOs)
                {
                    if (levyDTO.IsLocked)
                        continue;

                    var levyCharges = 0m;

                    if (levyDTO.ChargeFixedAmount > 0m)
                        levyCharges = levyDTO.ChargeFixedAmount;
                    else levyCharges = Math.Round(Convert.ToDecimal((levyDTO.ChargePercentage * Convert.ToDouble(totalValue)) / 100), 0, MidpointRounding.AwayFromZero);

                    if (levyCharges == 0m) continue;

                    var levySplits = _levyAppService.FindLevySplits(levyDTO.Id, serviceHeader);

                    if (levySplits != null && levySplits.Any())
                    {
                        foreach (var levySplit in levySplits)
                        {
                            var levySplitValue = Math.Round(Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100), 4, MidpointRounding.AwayFromZero);

                            if (levySplitValue == 0m) continue;

                            tariffs.Add(new TariffWrapper
                            {
                                Amount = levySplitValue,
                                Description = levySplit.Description,
                                CreditGLAccountId = levySplit.ChartOfAccountId,
                                CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                DebitGLAccountId = debitChartOfAccountId,
                                DebitGLAccountCode = debitChartOfAccountCode,
                                DebitGLAccountName = debitChartOfAccountName,
                            });
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<FixedDepositTypeGraduatedScaleDTO> FindGraduatedScales(Guid fixedDepositTypeId, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FixedDepositTypeGraduatedScaleSpecifications.GraduatedScaleWithFixedDepositTypeId(fixedDepositTypeId);

                    ISpecification<FixedDepositTypeGraduatedScale> spec = filter;

                    return _fixedDepositTypeGraduatedScaleRepository.AllMatching<FixedDepositTypeGraduatedScaleDTO>(spec, serviceHeader);
                }
            }
            else return null;
        }

        public bool UpdateGraduatedScales(Guid fixedDepositTypeId, List<FixedDepositTypeGraduatedScaleDTO> graduatedScales, ServiceHeader serviceHeader)
        {
            if (fixedDepositTypeId != null && graduatedScales != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _fixedDepositTypeRepository.Get(fixedDepositTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindGraduatedScales(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var graduatedScale = _fixedDepositTypeGraduatedScaleRepository.Get(item.Id, serviceHeader);

                                if (graduatedScale != null)
                                {
                                    _fixedDepositTypeGraduatedScaleRepository.Remove(graduatedScale, serviceHeader);
                                }
                            }
                        }

                        if (graduatedScales.Any())
                        {
                            foreach (var item in graduatedScales)
                            {
                                var range = new Range(item.RangeLowerLimit, item.RangeUpperLimit);

                                var graduatedScale = FixedDepositTypeGraduatedScaleFactory.CreateFixedDepositTypeGraduatedScale(persisted.Id, range, item.Percentage);

                                _fixedDepositTypeGraduatedScaleRepository.Add(graduatedScale, serviceHeader);
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
