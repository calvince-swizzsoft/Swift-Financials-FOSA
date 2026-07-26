using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAttachedProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeCommissionAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class ChequeTypeAppService : IChequeTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ChequeType> _chequeTypeRepository;
        private readonly IRepository<ChequeTypeCommission> _chequeTypeCommissionRepository;
        private readonly IRepository<ChequeTypeAttachedProduct> _chequeTypeAttachedProductRepository;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly IAppCache _appCache;

        public ChequeTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ChequeType> chequeTypeRepository,
           IRepository<ChequeTypeCommission> chequeTypeCommissionRepository,
           IRepository<ChequeTypeAttachedProduct> chequeTypeAttachedProductRepository,
           ILoanProductAppService loanProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (chequeTypeRepository == null)
                throw new ArgumentNullException(nameof(chequeTypeRepository));

            if (chequeTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(chequeTypeCommissionRepository));

            if (chequeTypeAttachedProductRepository == null)
                throw new ArgumentNullException(nameof(chequeTypeAttachedProductRepository));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _chequeTypeRepository = chequeTypeRepository;
            _chequeTypeCommissionRepository = chequeTypeCommissionRepository;
            _chequeTypeAttachedProductRepository = chequeTypeAttachedProductRepository;
            _loanProductAppService = loanProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _appCache = appCache;
        }

        public ChequeTypeDTO AddNewChequeType(ChequeTypeDTO chequeTypeDTO, ServiceHeader serviceHeader)
        {
            if (chequeTypeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var chequeType = ChequeTypeFactory.CreateChequeType(chequeTypeDTO.Description, chequeTypeDTO.MaturityPeriod, chequeTypeDTO.ChargeRecoveryMode);

                    if (chequeTypeDTO.IsLocked)
                        chequeType.Lock();
                    else chequeType.UnLock();

                    _chequeTypeRepository.Add(chequeType, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return chequeType.ProjectedAs<ChequeTypeDTO>();
                }
            }
            else return null;
        }

        public bool UpdateChequeType(ChequeTypeDTO chequeTypeDTO, ServiceHeader serviceHeader)
        {
            if (chequeTypeDTO == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _chequeTypeRepository.Get(chequeTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ChequeTypeFactory.CreateChequeType(chequeTypeDTO.Description, chequeTypeDTO.MaturityPeriod, chequeTypeDTO.ChargeRecoveryMode);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (chequeTypeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _chequeTypeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<ChequeTypeDTO> FindChequeTypes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var chequeTypes = _chequeTypeRepository.GetAll(serviceHeader);

                if (chequeTypes != null && chequeTypes.Any())
                {
                    return chequeTypes.ProjectedAsCollection<ChequeTypeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<ChequeTypeDTO> FindChequeTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChequeTypeSpecifications.DefaultSpec();

                ISpecification<ChequeType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var chequeTypePagedCollection = _chequeTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (chequeTypePagedCollection != null)
                {
                    var pageCollection = chequeTypePagedCollection.PageCollection.ProjectedAsCollection<ChequeTypeDTO>();

                    var itemsCount = chequeTypePagedCollection.ItemsCount;

                    return new PageCollectionInfo<ChequeTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<ChequeTypeDTO> FindChequeTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ChequeTypeSpecifications.ChequeTypeFullText(text);

                ISpecification<ChequeType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var chequeTypeCollection = _chequeTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (chequeTypeCollection != null)
                {
                    var pageCollection = chequeTypeCollection.PageCollection.ProjectedAsCollection<ChequeTypeDTO>();

                    var itemsCount = chequeTypeCollection.ItemsCount;

                    return new PageCollectionInfo<ChequeTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public ChequeTypeDTO FindChequeType(Guid chequeTypeId, ServiceHeader serviceHeader)
        {
            if (chequeTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var chequeType = _chequeTypeRepository.Get(chequeTypeId, serviceHeader);

                    if (chequeType != null)
                    {
                        return chequeType.ProjectedAs<ChequeTypeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionDTO> FindCommissions(Guid chequeTypeId, ServiceHeader serviceHeader)
        {
            if (chequeTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ChequeTypeCommissionSpecifications.ChequeTypeCommissionWithChequeTypeId(chequeTypeId);

                    ISpecification<ChequeTypeCommission> spec = filter;

                    var chequeTypeCommissions = _chequeTypeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (chequeTypeCommissions != null)
                    {
                        var projection = chequeTypeCommissions.ProjectedAsCollection<ChequeTypeCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateCommissions(Guid chequeTypeId, List<CommissionDTO> commissions, ServiceHeader serviceHeader)
        {
            if (chequeTypeId != null && commissions != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _chequeTypeRepository.Get(chequeTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = ChequeTypeCommissionSpecifications.ChequeTypeCommissionWithChequeTypeId(chequeTypeId);

                        ISpecification<ChequeTypeCommission> spec = filter;

                        var chequeTypeCommissions = _chequeTypeCommissionRepository.AllMatching(spec, serviceHeader);

                        if (chequeTypeCommissions != null)
                        {
                            chequeTypeCommissions.ToList().ForEach(x => _chequeTypeCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (commissions.Any())
                        {
                            foreach (var item in commissions)
                            {
                                var chequeTypeCommission = ChequeTypeCommissionFactory.CreateChequeTypeCommission(persisted.Id, item.Id);

                                _chequeTypeCommissionRepository.Add(chequeTypeCommission, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public ProductCollectionInfo FindAttachedProducts(Guid chequeTypeId, ServiceHeader serviceHeader, bool useCache)
        {
            if (chequeTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = ChequeTypeAttachedProductSpecifications.ChequeTypeAttachedProductWithChequeTypeId(chequeTypeId);

                    ISpecification<ChequeTypeAttachedProduct> spec = filter;

                    var chequeTypeAttachedProducts = _chequeTypeAttachedProductRepository.AllMatching(spec, serviceHeader);

                    if (chequeTypeAttachedProducts != null && chequeTypeAttachedProducts.Any())
                    {
                        var projection = chequeTypeAttachedProducts.ProjectedAsCollection<ChequeTypeAttachedProductDTO>();

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
                                case ProductCode.Investment:

                                    var targetInvestmentProduct = useCache ? _investmentProductAppService.FindCachedInvestmentProduct(item.TargetProductId, serviceHeader) : _investmentProductAppService.FindInvestmentProduct(item.TargetProductId, serviceHeader);

                                    if (targetInvestmentProduct != null)
                                    {
                                        investmentProductDTOs.Add(targetInvestmentProduct);
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

        public ProductCollectionInfo FindCachedAttachedProducts(Guid chequeTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<ProductCollectionInfo>(string.Format("AttachedProductsByChequeTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, chequeTypeId.ToString("D")), () =>
            {
                return FindAttachedProducts(chequeTypeId, serviceHeader, true);
            });
        }

        public bool UpdateAttachedProducts(Guid chequeTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader)
        {
            if (chequeTypeId != null && attachedProductsTuple != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _chequeTypeRepository.Get(chequeTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = ChequeTypeAttachedProductSpecifications.ChequeTypeAttachedProductWithChequeTypeId(chequeTypeId);

                        ISpecification<ChequeTypeAttachedProduct> spec = filter;

                        var chequeTypeAttachedProducts = _chequeTypeAttachedProductRepository.AllMatching(spec, serviceHeader);

                        if (chequeTypeAttachedProducts != null)
                        {
                            chequeTypeAttachedProducts.ToList().ForEach(x => _chequeTypeAttachedProductRepository.Remove(x, serviceHeader));
                        }

                        if (attachedProductsTuple.InvestmentProductCollection != null && attachedProductsTuple.InvestmentProductCollection.Any())
                        {
                            foreach (var item in attachedProductsTuple.InvestmentProductCollection)
                            {
                                var chequeTypeAttachedProduct = ChequeTypeAttachedProductFactory.CreateChequeTypeAttachedProduct(persisted.Id, (int)ProductCode.Investment, item.Id);

                                _chequeTypeAttachedProductRepository.Add(chequeTypeAttachedProduct, serviceHeader);
                            }
                        }

                        if (attachedProductsTuple.LoanProductCollection != null && attachedProductsTuple.LoanProductCollection.Any())
                        {
                            foreach (var item in attachedProductsTuple.LoanProductCollection)
                            {
                                var chequeTypeAttachedProduct = ChequeTypeAttachedProductFactory.CreateChequeTypeAttachedProduct(persisted.Id, (int)ProductCode.Loan, item.Id);

                                _chequeTypeAttachedProductRepository.Add(chequeTypeAttachedProduct, serviceHeader);
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
