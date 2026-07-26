using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductExemptionAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using LazyCache;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class SavingsProductAppService : ISavingsProductAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SavingsProduct> _savingsProductRepository;
        private readonly IRepository<SavingsProductCommission> _savingsProductCommissionRepository;
        private readonly IRepository<SavingsProductExemption> _savingsProductExemptionRepository;
        private readonly IAppCache _appCache;

        public SavingsProductAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SavingsProduct> savingsProductRepository,
           IRepository<SavingsProductCommission> savingsProductCommissionRepository,
           IRepository<SavingsProductExemption> savingsProductExemptionRepository,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (savingsProductRepository == null)
                throw new ArgumentNullException(nameof(savingsProductRepository));

            if (savingsProductCommissionRepository == null)
                throw new ArgumentNullException(nameof(savingsProductCommissionRepository));

            if (savingsProductExemptionRepository == null)
                throw new ArgumentNullException(nameof(savingsProductExemptionRepository));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _savingsProductRepository = savingsProductRepository;
            _savingsProductCommissionRepository = savingsProductCommissionRepository;
            _savingsProductExemptionRepository = savingsProductExemptionRepository;
            _appCache = appCache;
        }

        public SavingsProductDTO AddNewSavingsProduct(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader)
        {
            if (savingsProductDTO != null && savingsProductDTO.ChartOfAccountId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var savingsProduct = SavingsProductFactory.CreateSavingsProduct(savingsProductDTO.ChartOfAccountId, savingsProductDTO.Description, savingsProductDTO.MaximumAllowedWithdrawal, savingsProductDTO.MaximumAllowedDeposit, savingsProductDTO.MinimumBalance, savingsProductDTO.OperatingBalance, savingsProductDTO.WithdrawalNoticeAmount, savingsProductDTO.WithdrawalNoticePeriod, savingsProductDTO.WithdrawalInterval, savingsProductDTO.AnnualPercentageYield, savingsProductDTO.Priority, savingsProductDTO.AutomateLedgerFeeCalculation, savingsProductDTO.ThrottleOverTheCounterWithdrawals);

                    savingsProduct.Code = (short)_savingsProductRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(Code),0) + 1 AS Expr1 FROM {0}SavingsProducts", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();

                    if (savingsProductDTO.IsLocked)
                        savingsProduct.Lock();
                    else savingsProduct.UnLock();

                    if (savingsProductDTO.IsMandatory)
                        savingsProduct.SetAsMandatory();
                    else savingsProduct.ResetAsMandatory();

                    _savingsProductRepository.Add(savingsProduct, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return savingsProduct.ProjectedAs<SavingsProductDTO>();
                }
            }
            else return null;
        }

        public bool UpdateSavingsProduct(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader)
        {
            if (savingsProductDTO == null || savingsProductDTO.Id == Guid.Empty || savingsProductDTO.ChartOfAccountId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _savingsProductRepository.Get(savingsProductDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = SavingsProductFactory.CreateSavingsProduct(savingsProductDTO.ChartOfAccountId, savingsProductDTO.Description, savingsProductDTO.MaximumAllowedWithdrawal, savingsProductDTO.MaximumAllowedDeposit, savingsProductDTO.MinimumBalance, savingsProductDTO.OperatingBalance, savingsProductDTO.WithdrawalNoticeAmount, savingsProductDTO.WithdrawalNoticePeriod, savingsProductDTO.WithdrawalInterval, savingsProductDTO.AnnualPercentageYield, savingsProductDTO.Priority, savingsProductDTO.AutomateLedgerFeeCalculation, savingsProductDTO.ThrottleOverTheCounterWithdrawals);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.Code = persisted.Code;

                    if (savingsProductDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    if (savingsProductDTO.IsMandatory)
                        current.SetAsMandatory();
                    else current.ResetAsMandatory();

                    _savingsProductRepository.Merge(persisted, current, serviceHeader);

                    // Set as default?
                    if (savingsProductDTO.IsDefault && !persisted.IsDefault)
                        SetSavingsProductAsDefault(persisted.Id, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<SavingsProductDTO> FindSavingsProducts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SavingsProduct> spec = SavingsProductSpecifications.DefaultSpec();

                var savingsProducts = _savingsProductRepository.AllMatching(spec, serviceHeader);

                if (savingsProducts != null && savingsProducts.Any())
                {
                    return savingsProducts.ProjectedAsCollection<SavingsProductDTO>();
                }
                else return null;
            }
        }

        public List<SavingsProductDTO> FindMandatorySavingsProducts(bool isMandatory, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SavingsProduct> spec = SavingsProductSpecifications.DefaultSpec();

                var savingsProducts = _savingsProductRepository.AllMatching(spec, serviceHeader);

                if (savingsProducts != null && savingsProducts.Any())
                {
                    return savingsProducts.ProjectedAsCollection<SavingsProductDTO>();
                }
                else return null;
            }
        }

        public List<SavingsProductDTO> FindCachedSavingsProducts(ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<SavingsProductDTO>>(string.Format("SavingsProducts_{0}", serviceHeader.ApplicationDomainName), () =>
            {
                return FindSavingsProducts(serviceHeader);
            });
        }

        public List<SavingsProductDTO> FindSavingsProducts(int code, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SavingsProduct> spec = SavingsProductSpecifications.SavingsProductWithCode(code);

                var savingsProducts = _savingsProductRepository.AllMatching(spec, serviceHeader);

                if (savingsProducts != null && savingsProducts.Any())
                {
                    return savingsProducts.ProjectedAsCollection<SavingsProductDTO>();
                }
                else return null;
            }
        }
        public List<SavingsProductDTO> FindSavingsProductsWithAutomatedLedgerFeeCalculation(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SavingsProduct> spec = SavingsProductSpecifications.SavingsProductsWithAutomatedLedgerFeeCalculation();

                var savingsProducts = _savingsProductRepository.AllMatching(spec, serviceHeader);

                if (savingsProducts != null && savingsProducts.Any())
                {
                    return savingsProducts.ProjectedAsCollection<SavingsProductDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<SavingsProductDTO> FindSavingsProducts(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SavingsProductSpecifications.DefaultSpec();

                ISpecification<SavingsProduct> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var savingsProductCollection = _savingsProductRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (savingsProductCollection != null)
                {
                    var pageCollection = savingsProductCollection.PageCollection.ProjectedAsCollection<SavingsProductDTO>();

                    var itemsCount = savingsProductCollection.ItemsCount;

                    return new PageCollectionInfo<SavingsProductDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<SavingsProductDTO> FindSavingsProducts(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SavingsProductSpecifications.SavingsProductFullText(text);

                ISpecification<SavingsProduct> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var savingsProductCollection = _savingsProductRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (savingsProductCollection != null)
                {
                    var pageCollection = savingsProductCollection.PageCollection.ProjectedAsCollection<SavingsProductDTO>();

                    var itemsCount = savingsProductCollection.ItemsCount;

                    return new PageCollectionInfo<SavingsProductDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public SavingsProductDTO FindSavingsProduct(Guid savingsProductId, Guid exemptionsBranchId, ServiceHeader serviceHeader)
        {
            if (savingsProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var savingsProduct = _savingsProductRepository.Get(savingsProductId, serviceHeader);

                    if (savingsProduct != null)
                    {
                        var projection = savingsProduct.ProjectedAs<SavingsProductDTO>();

                        if (exemptionsBranchId != Guid.Empty)
                        {
                            var exemptions = FindSavingsProductExemptions(savingsProductId, serviceHeader);

                            if (exemptions != null && exemptions.Any())
                            {
                                var targetExemption = exemptions.Where(x => x.BranchId == exemptionsBranchId).FirstOrDefault();

                                if (targetExemption != null)
                                {
                                    projection.MaximumAllowedWithdrawal = targetExemption.MaximumAllowedWithdrawal;
                                    projection.MaximumAllowedDeposit = targetExemption.MaximumAllowedDeposit;
                                    projection.MinimumBalance = targetExemption.MinimumBalance;
                                    projection.OperatingBalance = targetExemption.OperatingBalance;
                                    projection.WithdrawalNoticeAmount = targetExemption.WithdrawalNoticeAmount;
                                    projection.WithdrawalNoticePeriod = targetExemption.WithdrawalNoticePeriod;
                                    projection.WithdrawalInterval = targetExemption.WithdrawalInterval;
                                    projection.AnnualPercentageYield = targetExemption.AnnualPercentageYield;
                                }
                            }
                        }

                        return projection;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public SavingsProductDTO FindCachedSavingsProduct(Guid savingsProductId, Guid exemptionsBranchId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<SavingsProductDTO>(string.Format("{0}_{1}_{2}", serviceHeader.ApplicationDomainName, savingsProductId.ToString("D"), exemptionsBranchId.ToString("D")), () =>
            {
                return FindSavingsProduct(savingsProductId, exemptionsBranchId, serviceHeader);
            });
        }

        public SavingsProductDTO FindDefaultSavingsProduct(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SavingsProductSpecifications.DefaultSavingsProduct();

                ISpecification<SavingsProduct> spec = filter;

                var savingsProducts = _savingsProductRepository.AllMatching(spec, serviceHeader);

                if (savingsProducts != null && savingsProducts.Any() && savingsProducts.Count() == 1)
                {
                    var savingsProduct = savingsProducts.SingleOrDefault();

                    if (savingsProduct != null)
                    {
                        return savingsProduct.ProjectedAs<SavingsProductDTO>();
                    }
                    else return null;
                }
                else return null;
            }
        }

        public SavingsProductDTO FindCachedDefaultSavingsProduct(ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<SavingsProductDTO>(string.Format("DefaultSavingsProduct_{0}", serviceHeader.ApplicationDomainName), () =>
            {
                return FindDefaultSavingsProduct(serviceHeader);
            });
        }

        public List<CommissionDTO> FindCommissions(Guid savingsProductId, int savingsProductKnownChargeType, ServiceHeader serviceHeader)
        {
            if (savingsProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SavingsProductCommissionSpecifications.SavingsProductCommission(savingsProductId, savingsProductKnownChargeType);

                    ISpecification<SavingsProductCommission> spec = filter;

                    var savingsProductCommissions = _savingsProductCommissionRepository.AllMatching(spec, serviceHeader);

                    if (savingsProductCommissions != null)
                    {
                        var savingsProductCommissionDTOs = savingsProductCommissions.ProjectedAsCollection<SavingsProductCommissionDTO>();

                        var projection = (from p in savingsProductCommissionDTOs
                                          select new
                                          {
                                              p.ChargeBenefactor,
                                              p.Commission
                                          });

                        foreach (var item in projection)
                            item.Commission.ChargeBenefactor = item.ChargeBenefactor; // map benefactor

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionDTO> FindCachedCommissions(Guid savingsProductId, int savingsProductKnownChargeType, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsBySavingsProductIdAndSavingsProductKnownChargeType_{0}_{1}_{2}", serviceHeader.ApplicationDomainName, savingsProductId.ToString("D"), savingsProductKnownChargeType), () =>
            {
                return FindCommissions(savingsProductId, savingsProductKnownChargeType, serviceHeader);
            });
        }

        public bool UpdateCommissions(Guid savingsProductId, List<CommissionDTO> commissionDTOs, int savingsProductKnownChargeType, int savingsProductChargeBenefactor, ServiceHeader serviceHeader)
        {
            if (savingsProductId != null && commissionDTOs != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _savingsProductRepository.Get(savingsProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = SavingsProductCommissionSpecifications.SavingsProductCommission(savingsProductId, savingsProductKnownChargeType);

                        ISpecification<SavingsProductCommission> spec = filter;

                        var savingsProductCommissions = _savingsProductCommissionRepository.AllMatching(spec, serviceHeader);

                        if (savingsProductCommissions != null)
                        {
                            savingsProductCommissions.ToList().ForEach(x => _savingsProductCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (commissionDTOs.Any())
                        {
                            foreach (var item in commissionDTOs)
                            {
                                var SavingsProductCommission = SavingsProductCommissionFactory.CreateSavingsProductCommission(persisted.Id, item.Id, savingsProductKnownChargeType, savingsProductChargeBenefactor);

                                _savingsProductCommissionRepository.Add(SavingsProductCommission, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<SavingsProductExemptionDTO> FindSavingsProductExemptions(Guid savingsProductId, ServiceHeader serviceHeader)
        {
            if (savingsProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SavingsProductExemptionSpecifications.SavingsProductExemptionWithSavingsProductId(savingsProductId);

                    ISpecification<SavingsProductExemption> spec = filter;

                    var auxilliaryAppraisalFactors = _savingsProductExemptionRepository.AllMatching(spec, serviceHeader);

                    if (auxilliaryAppraisalFactors != null)
                    {
                        return auxilliaryAppraisalFactors.ProjectedAsCollection<SavingsProductExemptionDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool UpdateSavingsProductExemptions(Guid savingsProductId, List<SavingsProductExemptionDTO> savingsProductExemptions, ServiceHeader serviceHeader)
        {
            if (savingsProductId != null && savingsProductExemptions != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _savingsProductRepository.Get(savingsProductId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindSavingsProductExemptions(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var savingsProductExemption = _savingsProductExemptionRepository.Get(item.Id, serviceHeader);

                                if (savingsProductExemption != null)
                                {
                                    _savingsProductExemptionRepository.Remove(savingsProductExemption, serviceHeader);
                                }
                            }
                        }

                        if (savingsProductExemptions.Any())
                        {
                            foreach (var item in savingsProductExemptions)
                            {
                                var savingsProductExemption = SavingsProductExemptionFactory.CreateSavingsProductExemption(persisted.Id, item.BranchId, item.MaximumAllowedWithdrawal, item.MaximumAllowedDeposit, item.MinimumBalance, item.OperatingBalance, item.WithdrawalNoticeAmount, item.WithdrawalNoticePeriod, item.WithdrawalInterval, item.AnnualPercentageYield);

                                savingsProductExemption.CreatedBy = serviceHeader.ApplicationUserName;

                                _savingsProductExemptionRepository.Add(savingsProductExemption, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        private bool SetSavingsProductAsDefault(Guid savingsProductId, ServiceHeader serviceHeader)
        {
            if (savingsProductId == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _savingsProductRepository.Get(savingsProductId, serviceHeader);

                if (persisted != null)
                {
                    persisted.SetAsDefault();

                    var otherSavingsProducts = _savingsProductRepository.GetAll(serviceHeader);

                    foreach (var item in otherSavingsProducts)
                    {
                        if (item.Id != persisted.Id)
                        {
                            var savingsProduct = _savingsProductRepository.Get(item.Id, serviceHeader);

                            savingsProduct.ResetAsDefault();
                        }
                    }

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }
    }
}
