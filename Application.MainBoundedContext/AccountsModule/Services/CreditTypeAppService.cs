using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAttachedProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeConcessionExemptProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeDirectDebitAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class CreditTypeAppService : ICreditTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CreditType> _creditTypeRepository;
        private readonly IRepository<CreditTypeCommission> _creditTypeCommissionRepository;
        private readonly IRepository<CreditTypeAttachedProduct> _creditTypeAttachedProductRepository;
        private readonly IRepository<CreditTypeDirectDebit> _creditTypeDirectDebitRepository;
        private readonly IRepository<CreditTypeConcessionExemptProduct> _creditTypeConcessionExemptProductRepository;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IAppCache _appCache;

        public CreditTypeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CreditType> creditTypeRepository,
           IRepository<CreditTypeCommission> creditTypeCommissionRepository,
           IRepository<CreditTypeAttachedProduct> creditTypeAttachedProductRepository,
           IRepository<CreditTypeDirectDebit> creditTypeDirectDebitRepository,
           IRepository<CreditTypeConcessionExemptProduct> creditTypeConcessionExemptProductRepository,
           IInvestmentProductAppService investmentProductAppService,
           ILoanProductAppService loanProductAppService,
           ISavingsProductAppService savingsProductAppService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (creditTypeRepository == null)
                throw new ArgumentNullException(nameof(creditTypeRepository));

            if (creditTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(creditTypeCommissionRepository));

            if (creditTypeAttachedProductRepository == null)
                throw new ArgumentNullException(nameof(creditTypeAttachedProductRepository));

            if (creditTypeDirectDebitRepository == null)
                throw new ArgumentNullException(nameof(creditTypeDirectDebitRepository));

            if (creditTypeConcessionExemptProductRepository == null)
                throw new ArgumentNullException(nameof(creditTypeConcessionExemptProductRepository));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _creditTypeRepository = creditTypeRepository;
            _creditTypeCommissionRepository = creditTypeCommissionRepository;
            _creditTypeAttachedProductRepository = creditTypeAttachedProductRepository;
            _creditTypeDirectDebitRepository = creditTypeDirectDebitRepository;
            _creditTypeConcessionExemptProductRepository = creditTypeConcessionExemptProductRepository;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
            _savingsProductAppService = savingsProductAppService;
            _appCache = appCache;
        }

        public CreditTypeDTO AddNewCreditType(CreditTypeDTO creditTypeDTO, ServiceHeader serviceHeader)
        {
            if (creditTypeDTO != null && creditTypeDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var creditType = CreditTypeFactory.CreateCreditType(creditTypeDTO.ChartOfAccountId, creditTypeDTO.Description, creditTypeDTO.TransactionOwnership);

                    if (creditTypeDTO.IsLocked)
                        creditType.Lock();
                    else creditType.UnLock();

                    _creditTypeRepository.Add(creditType, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return creditType.ProjectedAs<CreditTypeDTO>();
                }
            }
            else return null;
        }

        public bool UpdateCreditType(CreditTypeDTO creditTypeDTO, ServiceHeader serviceHeader)
        {
            if (creditTypeDTO == null || creditTypeDTO.Id == Guid.Empty || creditTypeDTO.ChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _creditTypeRepository.Get(creditTypeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = CreditTypeFactory.CreateCreditType(creditTypeDTO.ChartOfAccountId, creditTypeDTO.Description, creditTypeDTO.TransactionOwnership);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (creditTypeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _creditTypeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<CreditTypeDTO> FindCreditTypes(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<CreditType> spec = CreditTypeSpecifications.DefaultSpec();

                var creditTypes = _creditTypeRepository.AllMatching(spec, serviceHeader);

                if (creditTypes != null && creditTypes.Any())
                {
                    return creditTypes.ProjectedAsCollection<CreditTypeDTO>();
                }
                else return null;
            }
        }

        public List<CreditTypeDTO> FindCreditTypesByAttachedProductId(Guid attachedProductId, ServiceHeader serviceHeader)
        {
            if (attachedProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    ISpecification<CreditTypeAttachedProduct> spec = CreditTypeAttachedProductSpecifications.CreditTypeAttachedProductWithTargetProductId(attachedProductId);

                    var creditTypeAttachedProducts = _creditTypeAttachedProductRepository.AllMatching(spec, serviceHeader);

                    if (creditTypeAttachedProducts != null && creditTypeAttachedProducts.Any())
                    {
                        var projection = creditTypeAttachedProducts.ProjectedAsCollection<CreditTypeAttachedProductDTO>();

                        return (from p in projection select p.CreditType).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<CreditTypeDTO> FindCreditTypes(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditTypeSpecifications.DefaultSpec();

                ISpecification<CreditType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditTypeCollection = _creditTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditTypeCollection != null)
                {
                    var pageCollection = creditTypeCollection.PageCollection.ProjectedAsCollection<CreditTypeDTO>();

                    var itemsCount = creditTypeCollection.ItemsCount;

                    return new PageCollectionInfo<CreditTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CreditTypeDTO> FindCreditTypes(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CreditTypeSpecifications.CreditTypeFullText(text);

                ISpecification<CreditType> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var creditTypeCollection = _creditTypeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (creditTypeCollection != null)
                {
                    var pageCollection = creditTypeCollection.PageCollection.ProjectedAsCollection<CreditTypeDTO>();

                    var itemsCount = creditTypeCollection.ItemsCount;

                    return new PageCollectionInfo<CreditTypeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public CreditTypeDTO FindCreditType(Guid creditTypeId, ServiceHeader serviceHeader)
        {
            if (creditTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var creditType = _creditTypeRepository.Get(creditTypeId, serviceHeader);

                    if (creditType != null)
                    {
                        return creditType.ProjectedAs<CreditTypeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionDTO> FindCommissions(Guid creditTypeId, ServiceHeader serviceHeader)
        {
            if (creditTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditTypeCommissionSpecifications.CreditTypeCommissionWithCreditTypeId(creditTypeId);

                    ISpecification<CreditTypeCommission> spec = filter;

                    var creditTypeCommissions = _creditTypeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (creditTypeCommissions != null)
                    {
                        var projection = creditTypeCommissions.ProjectedAsCollection<CreditTypeCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateCommissions(Guid creditTypeId, List<CommissionDTO> commissionDTOs, ServiceHeader serviceHeader)
        {
            if (creditTypeId != null && commissionDTOs != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _creditTypeRepository.Get(creditTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = CreditTypeCommissionSpecifications.CreditTypeCommissionWithCreditTypeId(creditTypeId);

                        ISpecification<CreditTypeCommission> spec = filter;

                        var creditTypeCommissions = _creditTypeCommissionRepository.AllMatching(spec, serviceHeader);

                        if (creditTypeCommissions != null)
                        {
                            creditTypeCommissions.ToList().ForEach(x => _creditTypeCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (commissionDTOs.Any())
                        {
                            foreach (var item in commissionDTOs)
                            {
                                var creditTypeCommission = CreditTypeCommissionFactory.CreateCreditTypeCommission(persisted.Id, item.Id);
                                creditTypeCommission.CreatedBy = serviceHeader.ApplicationUserName;

                                _creditTypeCommissionRepository.Add(creditTypeCommission, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<DirectDebitDTO> FindDirectDebits(Guid creditTypeId, ServiceHeader serviceHeader)
        {
            if (creditTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditTypeDirectDebitSpecifications.CreditTypeDirectDebitWithCreditTypeId(creditTypeId);

                    ISpecification<CreditTypeDirectDebit> spec = filter;

                    var creditTypeDirectDebits = _creditTypeDirectDebitRepository.AllMatching(spec, serviceHeader);

                    if (creditTypeDirectDebits != null && creditTypeDirectDebits.Any())
                    {
                        var projection = creditTypeDirectDebits.ProjectedAsCollection<CreditTypeDirectDebitDTO>();

                        return (from p in projection select p.DirectDebit).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<DirectDebitDTO> FindCachedDirectDebits(Guid creditTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<DirectDebitDTO>>(string.Format("DirectDebitsByCreditTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, creditTypeId.ToString("D")), () =>
            {
                return FindDirectDebits(creditTypeId, serviceHeader);
            });
        }

        public bool UpdateDirectDebits(Guid creditTypeId, List<DirectDebitDTO> directDebitDTOs, ServiceHeader serviceHeader)
        {
            if (creditTypeId != null && directDebitDTOs != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _creditTypeRepository.Get(creditTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = CreditTypeDirectDebitSpecifications.CreditTypeDirectDebitWithCreditTypeId(creditTypeId);

                        ISpecification<CreditTypeDirectDebit> spec = filter;

                        var creditTypeDirectDebits = _creditTypeDirectDebitRepository.AllMatching(spec, serviceHeader);

                        if (creditTypeDirectDebits != null)
                        {
                            creditTypeDirectDebits.ToList().ForEach(x => _creditTypeDirectDebitRepository.Remove(x, serviceHeader));
                        }

                        if (directDebitDTOs.Any())
                        {
                            foreach (var item in directDebitDTOs)
                            {
                                var creditTypeDirectDebit = CreditTypeDirectDebitFactory.CreateCreditTypeDirectDebit(persisted.Id, item.Id);
                                creditTypeDirectDebit.CreatedBy = serviceHeader.ApplicationUserName;

                                _creditTypeDirectDebitRepository.Add(creditTypeDirectDebit, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public ProductCollectionInfo FindAttachedProducts(Guid creditTypeId, ServiceHeader serviceHeader, bool useCache)
        {
            if (creditTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditTypeAttachedProductSpecifications.CreditTypeAttachedProductWithCreditTypeId(creditTypeId);

                    ISpecification<CreditTypeAttachedProduct> spec = filter;

                    var creditTypeAttachedProducts = _creditTypeAttachedProductRepository.AllMatching(spec, serviceHeader);

                    if (creditTypeAttachedProducts != null && creditTypeAttachedProducts.Any())
                    {
                        var projection = creditTypeAttachedProducts.ProjectedAsCollection<CreditTypeAttachedProductDTO>();

                        var investmentProductDTOs = new List<InvestmentProductDTO>();

                        var loanProductDTOs = new List<LoanProductDTO>();

                        var savingsProductDTOs = new List<SavingsProductDTO>();

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

                        return new ProductCollectionInfo { InvestmentProductCollection = investmentProductDTOs, LoanProductCollection = loanProductDTOs, SavingsProductCollection = savingsProductDTOs };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public ProductCollectionInfo FindCachedAttachedProducts(Guid creditTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<ProductCollectionInfo>(string.Format("AttachedProductsByCreditTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, creditTypeId.ToString("D")), () =>
            {
                return FindAttachedProducts(creditTypeId, serviceHeader, false);
            });
        }

        public bool UpdateAttachedProducts(Guid creditTypeId, ProductCollectionInfo attachedProductsTuple, ServiceHeader serviceHeader)
        {
            if (creditTypeId != null && attachedProductsTuple != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _creditTypeRepository.Get(creditTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = CreditTypeAttachedProductSpecifications.CreditTypeAttachedProductWithCreditTypeId(creditTypeId);

                        ISpecification<CreditTypeAttachedProduct> spec = filter;

                        var creditTypeAttachedProducts = _creditTypeAttachedProductRepository.AllMatching(spec, serviceHeader);

                        if (creditTypeAttachedProducts != null)
                        {
                            creditTypeAttachedProducts.ToList().ForEach(x => _creditTypeAttachedProductRepository.Remove(x, serviceHeader));
                        }

                        if (attachedProductsTuple.InvestmentProductCollection != null && attachedProductsTuple.InvestmentProductCollection.Any())
                        {
                            foreach (var item in attachedProductsTuple.InvestmentProductCollection)
                            {
                                var creditTypeAttachedProduct = CreditTypeAttachedProductFactory.CreateCreditTypeAttachedProduct(persisted.Id, (int)ProductCode.Investment, item.Id);
                                creditTypeAttachedProduct.CreatedBy = serviceHeader.ApplicationUserName;

                                _creditTypeAttachedProductRepository.Add(creditTypeAttachedProduct, serviceHeader);
                            }
                        }

                        if (attachedProductsTuple.LoanProductCollection != null && attachedProductsTuple.LoanProductCollection.Any())
                        {
                            foreach (var item in attachedProductsTuple.LoanProductCollection)
                            {
                                var creditTypeAttachedProduct = CreditTypeAttachedProductFactory.CreateCreditTypeAttachedProduct(persisted.Id, (int)ProductCode.Loan, item.Id);
                                creditTypeAttachedProduct.CreatedBy = serviceHeader.ApplicationUserName;

                                _creditTypeAttachedProductRepository.Add(creditTypeAttachedProduct, serviceHeader);
                            }
                        }

                        if (attachedProductsTuple.SavingsProductCollection != null && attachedProductsTuple.SavingsProductCollection.Any())
                        {
                            foreach (var item in attachedProductsTuple.SavingsProductCollection)
                            {
                                var creditTypeAttachedProduct = CreditTypeAttachedProductFactory.CreateCreditTypeAttachedProduct(persisted.Id, (int)ProductCode.Savings, item.Id);
                                creditTypeAttachedProduct.CreatedBy = serviceHeader.ApplicationUserName;

                                _creditTypeAttachedProductRepository.Add(creditTypeAttachedProduct, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public ProductCollectionInfo FindConcessionExemptProducts(Guid creditTypeId, ServiceHeader serviceHeader, bool useCache)
        {
            if (creditTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CreditTypeConcessionExemptProductSpecifications.CreditTypeConcessionExemptProductWithCreditTypeId(creditTypeId);

                    ISpecification<CreditTypeConcessionExemptProduct> spec = filter;

                    var creditTypeConcessionExemptProducts = _creditTypeConcessionExemptProductRepository.AllMatching(spec, serviceHeader);

                    if (creditTypeConcessionExemptProducts != null && creditTypeConcessionExemptProducts.Any())
                    {
                        var projection = creditTypeConcessionExemptProducts.ProjectedAsCollection<CreditTypeConcessionExemptProductDTO>();

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

        public ProductCollectionInfo FindCachedConcessionExemptProducts(Guid creditTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<ProductCollectionInfo>(string.Format("ConcessionExemptProductsByCreditTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, creditTypeId.ToString("D")), () =>
            {
                return FindConcessionExemptProducts(creditTypeId, serviceHeader, false);
            });
        }

        public bool UpdateConcessionExemptProducts(Guid creditTypeId, ProductCollectionInfo concessionExemptProductsTuple, ServiceHeader serviceHeader)
        {
            if (creditTypeId != null && concessionExemptProductsTuple != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _creditTypeRepository.Get(creditTypeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = CreditTypeConcessionExemptProductSpecifications.CreditTypeConcessionExemptProductWithCreditTypeId(creditTypeId);

                        ISpecification<CreditTypeConcessionExemptProduct> spec = filter;

                        var creditTypeConcessionExemptProducts = _creditTypeConcessionExemptProductRepository.AllMatching(spec, serviceHeader);

                        if (creditTypeConcessionExemptProducts != null)
                        {
                            creditTypeConcessionExemptProducts.ToList().ForEach(x => _creditTypeConcessionExemptProductRepository.Remove(x, serviceHeader));
                        }

                        if (concessionExemptProductsTuple.InvestmentProductCollection != null && concessionExemptProductsTuple.InvestmentProductCollection.Any())
                        {
                            foreach (var item in concessionExemptProductsTuple.InvestmentProductCollection)
                            {
                                var creditTypeConcessionExemptProduct = CreditTypeConcessionExemptProductFactory.CreateCreditTypeConcessionExemptProduct(persisted.Id, (int)ProductCode.Investment, item.Id);
                                creditTypeConcessionExemptProduct.CreatedBy = serviceHeader.ApplicationUserName;

                                _creditTypeConcessionExemptProductRepository.Add(creditTypeConcessionExemptProduct, serviceHeader);
                            }
                        }

                        if (concessionExemptProductsTuple.LoanProductCollection != null && concessionExemptProductsTuple.LoanProductCollection.Any())
                        {
                            foreach (var item in concessionExemptProductsTuple.LoanProductCollection)
                            {
                                var creditTypeConcessionExemptProduct = CreditTypeConcessionExemptProductFactory.CreateCreditTypeConcessionExemptProduct(persisted.Id, (int)ProductCode.Loan, item.Id);
                                creditTypeConcessionExemptProduct.CreatedBy = serviceHeader.ApplicationUserName;

                                _creditTypeConcessionExemptProductRepository.Add(creditTypeConcessionExemptProduct, serviceHeader);
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
