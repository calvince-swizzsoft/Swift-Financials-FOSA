using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductExemptionAgg;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class InvestmentProductAppService : IInvestmentProductAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<InvestmentProduct> _investmentProductRepository;
        private readonly IRepository<InvestmentProductExemption> _investmentProductExemptionRepository;
        private readonly IAppCache _appCache;

        public InvestmentProductAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<InvestmentProduct> investmentProductRepository,
           IRepository<InvestmentProductExemption> investmentProductExemptionRepository,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (investmentProductRepository == null)
                throw new ArgumentNullException(nameof(investmentProductRepository));

            if (investmentProductExemptionRepository == null)
                throw new ArgumentNullException(nameof(investmentProductExemptionRepository));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _investmentProductRepository = investmentProductRepository;
            _investmentProductExemptionRepository = investmentProductExemptionRepository;
            _appCache = appCache;
        }

        public InvestmentProductDTO AddNewInvestmentProduct(InvestmentProductDTO investmentProductDTO, ServiceHeader serviceHeader)
        {
            if (investmentProductDTO != null && investmentProductDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var investmentProduct = InvestmentProductFactory.CreateInvestmentProduct(investmentProductDTO.ParentId, investmentProductDTO.ChartOfAccountId, investmentProductDTO.PoolChartOfAccountId, investmentProductDTO.Description, investmentProductDTO.MinimumBalance, investmentProductDTO.MaximumBalance, investmentProductDTO.PoolAmount, investmentProductDTO.IsRefundable, investmentProductDTO.Priority, investmentProductDTO.MaturityPeriod, investmentProductDTO.IsPooled, investmentProductDTO.IsSuperSaver, investmentProductDTO.AnnualPercentageYield, investmentProductDTO.TransferBalanceToParentOnMembershipTermination, investmentProductDTO.TrackArrears, investmentProductDTO.ThrottleScheduledArrearsRecovery);

                    investmentProduct.Code = (short)_investmentProductRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(Code),0) + 1 AS Expr1 FROM {0}InvestmentProducts", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                    if (investmentProductDTO.IsLocked)
                        investmentProduct.Lock();
                    else investmentProduct.UnLock();

                    if (investmentProductDTO.IsMandatory)
                        investmentProduct.SetAsMandatory();
                    else investmentProduct.ResetAsMandatory();

                    _investmentProductRepository.Add(investmentProduct, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return investmentProduct.ProjectedAs<InvestmentProductDTO>();
                }
            }
            else return null;
        }

        public bool UpdateInvestmentProduct(InvestmentProductDTO investmentProductDTO, ServiceHeader serviceHeader)
        {
            if (investmentProductDTO == null || investmentProductDTO.Id == Guid.Empty || investmentProductDTO.ChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _investmentProductRepository.Get(investmentProductDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = InvestmentProductFactory.CreateInvestmentProduct(investmentProductDTO.ParentId, investmentProductDTO.ChartOfAccountId, investmentProductDTO.PoolChartOfAccountId, investmentProductDTO.Description, investmentProductDTO.MinimumBalance, investmentProductDTO.MaximumBalance, investmentProductDTO.PoolAmount, investmentProductDTO.IsRefundable, investmentProductDTO.Priority, investmentProductDTO.MaturityPeriod, investmentProductDTO.IsPooled, investmentProductDTO.IsSuperSaver, investmentProductDTO.AnnualPercentageYield, investmentProductDTO.TransferBalanceToParentOnMembershipTermination, investmentProductDTO.TrackArrears, investmentProductDTO.ThrottleScheduledArrearsRecovery);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Code = persisted.Code;

                    if (investmentProductDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    if (investmentProductDTO.IsMandatory)
                        current.SetAsMandatory();
                    else current.ResetAsMandatory();
                    _investmentProductRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<InvestmentProductDTO> FindMandatoryInvestmentProducts(bool isMandatory, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<InvestmentProduct> spec = InvestmentProductSpecifications.MandatoryInvestmentProducts(isMandatory);

                var investmentProducts = _investmentProductRepository.AllMatching(spec, serviceHeader);

                if (investmentProducts != null && investmentProducts.Any())
                {
                    return investmentProducts.ProjectedAsCollection<InvestmentProductDTO>();
                }
                else return null;
            }
        }
        public List<InvestmentProductDTO> FindInvestmentProducts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<InvestmentProduct> spec = InvestmentProductSpecifications.DefaultSpec();

                var investmentProducts = _investmentProductRepository.AllMatching(spec, serviceHeader);

                if (investmentProducts != null && investmentProducts.Any())
                {
                    return investmentProducts.ProjectedAsCollection<InvestmentProductDTO>();
                }
                else return null;
            }
        }

        public List<InvestmentProductDTO> FindCachedInvestmentProducts(ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<InvestmentProductDTO>>("InvestmentProducts", () =>
            {
                return FindInvestmentProducts(serviceHeader);
            });
        }

        public List<InvestmentProductDTO> FindInvestmentProducts(int code, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<InvestmentProduct> spec = InvestmentProductSpecifications.InvestmentProductWithCode(code);

                var investmentProducts = _investmentProductRepository.AllMatching(spec, serviceHeader);

                if (investmentProducts != null && investmentProducts.Any())
                {
                    return investmentProducts.ProjectedAsCollection<InvestmentProductDTO>();
                }
                else return null;
            }
        }

        public List<InvestmentProductDTO> FindPooledInvestmentProducts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<InvestmentProduct> spec = InvestmentProductSpecifications.PooledSpec();

                var investmentProducts = _investmentProductRepository.AllMatching(spec, serviceHeader);

                if (investmentProducts != null && investmentProducts.Any())
                {
                    return investmentProducts.ProjectedAsCollection<InvestmentProductDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<InvestmentProductDTO> FindInvestmentProducts(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InvestmentProductSpecifications.DefaultSpec();

                ISpecification<InvestmentProduct> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var investmentProductCollection = _investmentProductRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (investmentProductCollection != null)
                {
                    var pageCollection = investmentProductCollection.PageCollection.ProjectedAsCollection<InvestmentProductDTO>();

                    var itemsCount = investmentProductCollection.ItemsCount;

                    return new PageCollectionInfo<InvestmentProductDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<InvestmentProductDTO> FindInvestmentProducts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InvestmentProductSpecifications.InvestmentProductFullText(text);

                ISpecification<InvestmentProduct> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var investmentProductCollection = _investmentProductRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (investmentProductCollection != null)
                {
                    var pageCollection = investmentProductCollection.PageCollection.ProjectedAsCollection<InvestmentProductDTO>();

                    var itemsCount = investmentProductCollection.ItemsCount;

                    return new PageCollectionInfo<InvestmentProductDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public InvestmentProductDTO FindInvestmentProduct(Guid investmentProductId, ServiceHeader serviceHeader)
        {
            if (investmentProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var investmentProduct = _investmentProductRepository.Get(investmentProductId, serviceHeader);

                    if (investmentProduct != null)
                    {
                        return investmentProduct.ProjectedAs<InvestmentProductDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public InvestmentProductDTO FindCachedInvestmentProduct(Guid investmentProductId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<InvestmentProductDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, investmentProductId.ToString("D")), () =>
            {
                return FindInvestmentProduct(investmentProductId, serviceHeader);
            });
        }

        public InvestmentProductDTO FindSuperSaverInvestmentProduct(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InvestmentProductSpecifications.SuperSavingsProduct();

                ISpecification<InvestmentProduct> spec = filter;

                var investmentProducts = _investmentProductRepository.AllMatching(spec, serviceHeader);

                if (investmentProducts != null && investmentProducts.Any() && investmentProducts.Count() == 1)
                {
                    var investmentsProduct = investmentProducts.SingleOrDefault();

                    if (investmentsProduct != null)
                    {
                        return investmentsProduct.ProjectedAs<InvestmentProductDTO>();
                    }
                    else return null;
                }
                else return null;
            }
        }

        public List<InvestmentProductExemptionDTO> FindInvestmentProductExemptions(Guid investmentProductId, ServiceHeader serviceHeader)
        {
            if (investmentProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = InvestmentProductExemptionSpecifications.InvestmentProductExemptionWithInvestmentProductId(investmentProductId);

                    ISpecification<InvestmentProductExemption> spec = filter;

                    var auxilliaryAppraisalFactors = _investmentProductExemptionRepository.AllMatching(spec, serviceHeader);

                    if (auxilliaryAppraisalFactors != null)
                    {
                        return auxilliaryAppraisalFactors.ProjectedAsCollection<InvestmentProductExemptionDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public InvestmentProductExemptionDTO FindInvestmentProductExemption(Guid investmentProductId, int customerClassification, ServiceHeader serviceHeader)
        {
            if (investmentProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = InvestmentProductExemptionSpecifications.InvestmentProductExemptionWithInvestmentProductIdAndCustomerClassification(investmentProductId, customerClassification);

                    ISpecification<InvestmentProductExemption> spec = filter;

                    var investmentProductExemptions = _investmentProductExemptionRepository.AllMatching(spec, serviceHeader);

                    if (investmentProductExemptions != null)
                    {
                        var targetInvestmentProductExemption = investmentProductExemptions.FirstOrDefault();

                        if (targetInvestmentProductExemption != null)
                            return targetInvestmentProductExemption.ProjectedAs<InvestmentProductExemptionDTO>();
                        else return null;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateInvestmentProductExemptions(Guid investmentProductId, List<InvestmentProductExemptionDTO> investmentProductExemptions, ServiceHeader serviceHeader)
        {
            if (investmentProductId != null && investmentProductExemptions != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _investmentProductRepository.Get(investmentProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindInvestmentProductExemptions(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var investmentProductExemption = _investmentProductExemptionRepository.Get(item.Id, serviceHeader);

                                if (investmentProductExemption != null)
                                {
                                    _investmentProductExemptionRepository.Remove(investmentProductExemption, serviceHeader);
                                }
                            }
                        }

                        if (investmentProductExemptions.Any())
                        {
                            foreach (var item in investmentProductExemptions)
                            {
                                var investmentProductExemption = InvestmentProductExemptionFactory.CreateInvestmentProductExemption(persisted.Id, item.CustomerClassification, item.MaximumBalance, item.AppraisalMultiplier);

                                investmentProductExemption.CreatedBy = serviceHeader.ApplicationUserName;

                                _investmentProductExemptionRepository.Add(investmentProductExemption, serviceHeader);
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
