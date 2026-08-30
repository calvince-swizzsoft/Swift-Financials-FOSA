using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.AdministrationModule.Services;
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
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly IAppCache _appCache;

        public SavingsProductAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SavingsProduct> savingsProductRepository,
           IRepository<SavingsProductCommission> savingsProductCommissionRepository,
           IRepository<SavingsProductExemption> savingsProductExemptionRepository,
           IChartOfAccountAppService chartOfAccountAppService,
           IBranchAppService branchAppService,
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

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (branchAppService == null)
                throw new ArgumentNullException(nameof(branchAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _savingsProductRepository = savingsProductRepository;
            _savingsProductCommissionRepository = savingsProductCommissionRepository;
            _savingsProductExemptionRepository = savingsProductExemptionRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _branchAppService = branchAppService;
            _appCache = appCache;
        }

        public IDictionary<string, string[]> ValidateSavingsProduct(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader)
        {
            var errors = new Dictionary<string, List<string>>();
            Action<string, string> add = (field, message) =>
            {
                List<string> messages;
                if (!errors.TryGetValue(field, out messages))
                {
                    messages = new List<string>();
                    errors[field] = messages;
                }
                messages.Add(message);
            };

            if (savingsProductDTO == null)
            {
                add("SavingsProduct", "Savings product details are required.");
                return errors.ToDictionary(item => item.Key, item => item.Value.ToArray());
            }

            if (string.IsNullOrWhiteSpace(savingsProductDTO.Description))
                add("Description", "Description is required.");
            if (savingsProductDTO.ChartOfAccountId == Guid.Empty)
                add("ChartOfAccountId", "Chart of Account is required.");
            else if (_chartOfAccountAppService.FindChartOfAccount(savingsProductDTO.ChartOfAccountId, serviceHeader) == null)
                add("ChartOfAccountId", "The selected Chart of Account does not exist.");

            if (savingsProductDTO.MaximumAllowedWithdrawal <= 0m)
                add("MaximumAllowedWithdrawal", "Maximum allowed withdrawal must be greater than zero.");
            if (savingsProductDTO.MaximumAllowedDeposit <= 0m)
                add("MaximumAllowedDeposit", "Maximum allowed deposit must be greater than zero.");
            if (savingsProductDTO.MinimumBalance < 0m)
                add("MinimumBalance", "Minimum balance cannot be negative.");
            if (savingsProductDTO.OperatingBalance < 0m)
                add("OperatingBalance", "Operating balance cannot be negative.");
            else if (savingsProductDTO.OperatingBalance < savingsProductDTO.MinimumBalance)
                add("OperatingBalance", "Operating balance cannot be lower than the minimum balance.");
            if (savingsProductDTO.WithdrawalNoticeAmount < 0m)
                add("WithdrawalNoticeAmount", "Withdrawal notice amount cannot be negative.");
            else if (savingsProductDTO.WithdrawalNoticeAmount > savingsProductDTO.MaximumAllowedWithdrawal)
                add("WithdrawalNoticeAmount", "Withdrawal notice amount cannot exceed the maximum allowed withdrawal.");
            if (savingsProductDTO.WithdrawalNoticePeriod < 0 || savingsProductDTO.WithdrawalNoticePeriod > short.MaxValue)
                add("WithdrawalNoticePeriod", "Withdrawal notice period must be between 0 and 32,767 days.");
            else if (savingsProductDTO.WithdrawalNoticeAmount > 0m && savingsProductDTO.WithdrawalNoticePeriod == 0)
                add("WithdrawalNoticePeriod", "Withdrawal notice period must be greater than zero when a notice amount is configured.");
            if (savingsProductDTO.WithdrawalInterval < 0 || savingsProductDTO.WithdrawalInterval > short.MaxValue)
                add("WithdrawalInterval", "Withdrawal interval must be between 0 and 32,767 days.");
            if (double.IsNaN(savingsProductDTO.AnnualPercentageYield) || double.IsInfinity(savingsProductDTO.AnnualPercentageYield) ||
                savingsProductDTO.AnnualPercentageYield < 0d || savingsProductDTO.AnnualPercentageYield > 100d)
                add("AnnualPercentageYield", "Annual percentage yield must be between 0% and 100%.");
            if (savingsProductDTO.Priority < 0 || savingsProductDTO.Priority > 3)
                add("Priority", "Recovery priority must be between 0 and 3.");

            return errors.ToDictionary(item => item.Key, item => item.Value.Distinct().ToArray());
        }

        public SavingsProductDTO AddNewSavingsProduct(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader)
        {
            if (!ValidateSavingsProduct(savingsProductDTO, serviceHeader).Any())
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

                    if (savingsProductDTO.IsDefault)
                    {
                        savingsProduct.SetAsDefault();
                        foreach (var other in _savingsProductRepository.GetAll(serviceHeader))
                            other.ResetAsDefault();
                    }

                    _savingsProductRepository.Add(savingsProduct, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return savingsProduct.ProjectedAs<SavingsProductDTO>();
                }
            }
            else return null;
        }

        public bool UpdateSavingsProduct(SavingsProductDTO savingsProductDTO, ServiceHeader serviceHeader)
        {
            if (savingsProductDTO == null || savingsProductDTO.Id == Guid.Empty || ValidateSavingsProduct(savingsProductDTO, serviceHeader).Any())
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

                    if (savingsProductDTO.IsDefault)
                    {
                        current.SetAsDefault();
                        foreach (var other in _savingsProductRepository.GetAll(serviceHeader))
                            if (other.Id != current.Id) other.ResetAsDefault();
                    }
                    else current.ResetAsDefault();

                    _savingsProductRepository.Merge(persisted, current, serviceHeader);

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
                ISpecification<SavingsProduct> spec = SavingsProductSpecifications.MandatorySavingsProducts(isMandatory);

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
            if (!ValidateSavingsProductExemptions(savingsProductId, savingsProductExemptions, serviceHeader).Any())
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

        public IDictionary<string, string[]> ValidateSavingsProductExemptions(Guid savingsProductId, List<SavingsProductExemptionDTO> savingsProductExemptions, ServiceHeader serviceHeader)
        {
            var errors = new Dictionary<string, List<string>>();
            Action<string, string> add = (field, message) =>
            {
                List<string> messages;
                if (!errors.TryGetValue(field, out messages))
                {
                    messages = new List<string>();
                    errors[field] = messages;
                }
                messages.Add(message);
            };

            if (savingsProductId == Guid.Empty || _savingsProductRepository.Get(savingsProductId, serviceHeader) == null)
                add("SavingsProductId", "The savings product does not exist.");
            if (savingsProductExemptions == null)
            {
                add("Exemptions", "Savings product exemptions are required.");
                return errors.ToDictionary(item => item.Key, item => item.Value.ToArray());
            }

            var duplicateBranches = savingsProductExemptions.Where(item => item != null && item.BranchId != Guid.Empty)
                .GroupBy(item => item.BranchId).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
            if (duplicateBranches.Any()) add("BranchId", "A branch can only have one exemption for this savings product.");

            for (var index = 0; index < savingsProductExemptions.Count; index++)
            {
                var item = savingsProductExemptions[index];
                var prefix = string.Format("Exemptions[{0}]", index);
                if (item == null) { add(prefix, "Exemption details are required."); continue; }
                if (item.BranchId == Guid.Empty) add(prefix + ".BranchId", "Branch is required.");
                else if (_branchAppService.FindBranch(item.BranchId, serviceHeader) == null) add(prefix + ".BranchId", "The selected branch does not exist.");
                if (item.MaximumAllowedWithdrawal <= 0m) add(prefix + ".MaximumAllowedWithdrawal", "Maximum withdrawal must be greater than zero.");
                if (item.MaximumAllowedDeposit <= 0m) add(prefix + ".MaximumAllowedDeposit", "Maximum deposit must be greater than zero.");
                if (item.MinimumBalance < 0m) add(prefix + ".MinimumBalance", "Minimum balance cannot be negative.");
                if (item.OperatingBalance < item.MinimumBalance) add(prefix + ".OperatingBalance", "Operating balance cannot be lower than minimum balance.");
                if (item.WithdrawalNoticeAmount < 0m || item.WithdrawalNoticeAmount > item.MaximumAllowedWithdrawal)
                    add(prefix + ".WithdrawalNoticeAmount", "Withdrawal notice amount must be between zero and maximum withdrawal.");
                if (item.WithdrawalNoticePeriod < 0 || item.WithdrawalNoticePeriod > short.MaxValue)
                    add(prefix + ".WithdrawalNoticePeriod", "Withdrawal notice period must be between 0 and 32,767 days.");
                else if (item.WithdrawalNoticeAmount > 0m && item.WithdrawalNoticePeriod == 0)
                    add(prefix + ".WithdrawalNoticePeriod", "Withdrawal notice period must be greater than zero when a notice amount is configured.");
                if (item.WithdrawalInterval < 0 || item.WithdrawalInterval > short.MaxValue)
                    add(prefix + ".WithdrawalInterval", "Withdrawal interval must be between 0 and 32,767 days.");
                if (double.IsNaN(item.AnnualPercentageYield) || double.IsInfinity(item.AnnualPercentageYield) || item.AnnualPercentageYield < 0d || item.AnnualPercentageYield > 100d)
                    add(prefix + ".AnnualPercentageYield", "Annual percentage yield must be between 0% and 100%.");
            }

            return errors.ToDictionary(item => item.Key, item => item.Value.Distinct().ToArray());
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
