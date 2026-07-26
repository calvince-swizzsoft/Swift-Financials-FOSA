using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionLevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionSplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GraduatedScaleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SystemTransactionTypeInCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeCommissionAgg;
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
    public class CommissionAppService : ICommissionAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Commission> _commissionRepository;
        private readonly IRepository<GraduatedScale> _graduatedScaleRepository;
        private readonly IRepository<CommissionLevy> _commissionLevyRepository;
        private readonly IRepository<CommissionSplit> _commissionSplitRepository;
        private readonly IRepository<SystemTransactionTypeInCommission> _systemTransactionTypeInCommissionRepository;
        private readonly IRepository<LevySplit> _levySplitRepository;
        private readonly IRepository<DynamicChargeCommission> _dynamicChargeCommissionRepository;
        private readonly IRepository<CreditTypeCommission> _creditTypeCommissionRepository;
        private readonly IRepository<ChequeTypeCommission> _chequeTypeCommissionRepository;
        private readonly IRepository<DebitTypeCommission> _debitTypeCommissionRepository;
        private readonly IRepository<UnPayReasonCommission> _unPayReasonCommissionRepository;
        private readonly IRepository<WireTransferTypeCommission> _wireTransferTypeCommissionRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ICommissionExemptionAppService _commissionExemptionAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IAlternateChannelAppService _alternateChannelAppService;
        private readonly ITextAlertAppService _textAlertAppService;
        private readonly IAppCache _appCache;

        public CommissionAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Commission> commissionRepository,
           IRepository<GraduatedScale> graduatedScaleRepository,
           IRepository<CommissionLevy> commissionLevyRepository,
           IRepository<CommissionSplit> commissionSplitRepository,
           IRepository<SystemTransactionTypeInCommission> systemTransactionTypeInCommissionRepository,
           IRepository<LevySplit> levySplitRepository,
           IRepository<DynamicChargeCommission> dynamicChargeCommissionRepository,
           IRepository<CreditTypeCommission> creditTypeCommissionRepository,
           IRepository<ChequeTypeCommission> chequeTypeCommissionRepository,
           IRepository<DebitTypeCommission> debitTypeCommissionRepository,
           IRepository<UnPayReasonCommission> unPayReasonCommissionRepository,
           IRepository<WireTransferTypeCommission> wireTransferTypeCommissionRepository,
           ISqlCommandAppService sqlCommandAppService,
           ICommissionExemptionAppService commissionExemptionAppService,
           ISavingsProductAppService savingsProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           ILoanProductAppService loanProductAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           IAlternateChannelAppService alternateChannelAppService,
           ITextAlertAppService textAlertAppService,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (commissionRepository == null)
                throw new ArgumentNullException(nameof(commissionRepository));

            if (graduatedScaleRepository == null)
                throw new ArgumentNullException(nameof(graduatedScaleRepository));

            if (commissionLevyRepository == null)
                throw new ArgumentNullException(nameof(commissionLevyRepository));

            if (commissionSplitRepository == null)
                throw new ArgumentNullException(nameof(commissionSplitRepository));

            if (systemTransactionTypeInCommissionRepository == null)
                throw new ArgumentNullException(nameof(systemTransactionTypeInCommissionRepository));

            if (levySplitRepository == null)
                throw new ArgumentNullException(nameof(levySplitRepository));

            if (dynamicChargeCommissionRepository == null)
                throw new ArgumentNullException(nameof(dynamicChargeCommissionRepository));

            if (creditTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(creditTypeCommissionRepository));

            if (chequeTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(chequeTypeCommissionRepository));

            if (debitTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(debitTypeCommissionRepository));

            if (unPayReasonCommissionRepository == null)
                throw new ArgumentNullException(nameof(unPayReasonCommissionRepository));

            if (wireTransferTypeCommissionRepository == null)
                throw new ArgumentNullException(nameof(wireTransferTypeCommissionRepository));

            if (commissionExemptionAppService == null)
                throw new ArgumentNullException(nameof(commissionExemptionAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (alternateChannelAppService == null)
                throw new ArgumentNullException(nameof(alternateChannelAppService));

            if (textAlertAppService == null)
                throw new ArgumentNullException(nameof(textAlertAppService));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _commissionRepository = commissionRepository;
            _graduatedScaleRepository = graduatedScaleRepository;
            _commissionLevyRepository = commissionLevyRepository;
            _commissionSplitRepository = commissionSplitRepository;
            _systemTransactionTypeInCommissionRepository = systemTransactionTypeInCommissionRepository;
            _levySplitRepository = levySplitRepository;
            _dynamicChargeCommissionRepository = dynamicChargeCommissionRepository;
            _creditTypeCommissionRepository = creditTypeCommissionRepository;
            _chequeTypeCommissionRepository = chequeTypeCommissionRepository;
            _debitTypeCommissionRepository = debitTypeCommissionRepository;
            _unPayReasonCommissionRepository = unPayReasonCommissionRepository;
            _wireTransferTypeCommissionRepository = wireTransferTypeCommissionRepository;
            _sqlCommandAppService = sqlCommandAppService;
            _commissionExemptionAppService = commissionExemptionAppService;
            _savingsProductAppService = savingsProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _loanProductAppService = loanProductAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _alternateChannelAppService = alternateChannelAppService;
            _textAlertAppService = textAlertAppService;
            _appCache = appCache;
        }

        public CommissionDTO AddNewCommission(CommissionDTO commissionDTO, ServiceHeader serviceHeader)
        {
            if (commissionDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<Commission> spec = CommissionSpecifications.CommissionWithCommission(commissionDTO.Description);

                    var matchedCommission = _commissionRepository.AllMatching(spec, serviceHeader);

                    if (matchedCommission != null && matchedCommission.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        commissionDTO.ErrorMessageResult = string.Format("Sorry, but Commission \"{0}\" already exists!", commissionDTO.Description.ToUpper());
                        return commissionDTO;
                    }
                    else
                    {
                        var commission = CommissionFactory.CreateCommission(commissionDTO.Description, commissionDTO.MaximumCharge, commissionDTO.RoundingType);

                        if (commissionDTO.IsLocked)
                            commission.Lock();
                        else commission.UnLock();

                        _commissionRepository.Add(commission, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return commission.ProjectedAs<CommissionDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateCommission(CommissionDTO commissionDTO, ServiceHeader serviceHeader)
        {
            if (commissionDTO == null || commissionDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _commissionRepository.Get(commissionDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = CommissionFactory.CreateCommission(commissionDTO.Description, commissionDTO.MaximumCharge, commissionDTO.RoundingType);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (commissionDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _commissionRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<CommissionDTO> FindCommissions(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var commissions = _commissionRepository.GetAll(serviceHeader);

                if (commissions != null && commissions.Any())
                {
                    return commissions.ProjectedAsCollection<CommissionDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<CommissionDTO> FindCommissions(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CommissionSpecifications.DefaultSpec();

                ISpecification<Commission> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var commissionPagedCollection = _commissionRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (commissionPagedCollection != null)
                {
                    var pageCollection = commissionPagedCollection.PageCollection.ProjectedAsCollection<CommissionDTO>();

                    var itemsCount = commissionPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CommissionDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<CommissionDTO> FindCommissions(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CommissionSpecifications.CommissionFullText(text);

                ISpecification<Commission> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var commissionPagedCollection = _commissionRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (commissionPagedCollection != null)
                {
                    var pageCollection = commissionPagedCollection.PageCollection.ProjectedAsCollection<CommissionDTO>();

                    var itemsCount = commissionPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CommissionDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public CommissionDTO FindCommission(Guid commissionId, ServiceHeader serviceHeader)
        {
            if (commissionId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var commission = _commissionRepository.Get(commissionId, serviceHeader);

                    if (commission != null)
                    {
                        return commission.ProjectedAs<CommissionDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<GraduatedScaleDTO> FindGraduatedScales(Guid commissionId, ServiceHeader serviceHeader)
        {
            if (commissionId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = GraduatedScaleSpecifications.GraduatedScaleWithCommissionId(commissionId);

                    ISpecification<GraduatedScale> spec = filter;

                    var graduatedScaleCollection = _graduatedScaleRepository.AllMatching(spec, serviceHeader);

                    if (graduatedScaleCollection != null && graduatedScaleCollection.Any())
                    {
                        return graduatedScaleCollection.ProjectedAsCollection<GraduatedScaleDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<GraduatedScaleDTO> FindCachedGraduatedScales(Guid commissionId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<GraduatedScaleDTO>>(string.Format("GraduatedScalesByCommissionId_{0}_{1}", serviceHeader.ApplicationDomainName, commissionId.ToString("D")), () =>
            {
                return FindGraduatedScales(commissionId, serviceHeader);
            });
        }

        public bool UpdateGraduatedScales(Guid commissionId, List<GraduatedScaleDTO> graduatedScales, ServiceHeader serviceHeader)
        {
            if (commissionId != null && graduatedScales != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _commissionRepository.Get(commissionId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindGraduatedScales(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var graduatedScale = _graduatedScaleRepository.Get(item.Id, serviceHeader);

                                if (graduatedScale != null)
                                {
                                    _graduatedScaleRepository.Remove(graduatedScale, serviceHeader);
                                }
                            }
                        }

                        if (graduatedScales.Any())
                        {
                            foreach (var item in graduatedScales)
                            {
                                var range = new Range(item.RangeLowerLimit, item.RangeUpperLimit);

                                var charge = new Charge(item.ChargeType, item.ChargePercentage, item.ChargeFixedAmount);

                                var graduatedScale = GraduatedScaleFactory.CreateGraduatedScale(persisted.Id, range, charge);

                                _graduatedScaleRepository.Add(graduatedScale, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<LevyDTO> FindLevies(Guid commissionId, ServiceHeader serviceHeader)
        {
            if (commissionId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CommissionLevySpecifications.CommissionLevyWithCommissionId(commissionId);

                    ISpecification<CommissionLevy> spec = filter;

                    var commissionLevies = _commissionLevyRepository.AllMatching(spec, serviceHeader);

                    if (commissionLevies != null && commissionLevies.Any())
                    {
                        var projection = commissionLevies.ProjectedAsCollection<CommissionLevyDTO>();

                        return (from p in projection select p.Levy).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LevyDTO> FindCachedLevies(Guid commissionId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<LevyDTO>>(string.Format("LeviesByCommissionId_{0}_{1}", serviceHeader.ApplicationDomainName, commissionId.ToString("D")), () =>
            {
                return FindLevies(commissionId, serviceHeader);
            });
        }

        public bool UpdateLevies(Guid commissionId, List<LevyDTO> levies, ServiceHeader serviceHeader)
        {
            if (commissionId != null && levies != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _commissionRepository.Get(commissionId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = CommissionLevySpecifications.CommissionLevyWithCommissionId(commissionId);

                        ISpecification<CommissionLevy> spec = filter;

                        var commissionLevies = _commissionLevyRepository.AllMatching(spec, serviceHeader);

                        if (commissionLevies != null)
                        {
                            commissionLevies.ToList().ForEach(x => _commissionLevyRepository.Remove(x, serviceHeader));
                        }

                        if (levies.Any())
                        {
                            foreach (var item in levies)
                            {
                                var commissionLevy = CommissionLevyFactory.CreateCommissionLevy(persisted.Id, item.Id);

                                _commissionLevyRepository.Add(commissionLevy, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<CommissionSplitDTO> FindCommissionSplits(Guid commissionId, ServiceHeader serviceHeader)
        {
            if (commissionId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = CommissionSplitSpecifications.CommissionSplitWithCommissionId(commissionId);

                    ISpecification<CommissionSplit> spec = filter;

                    var commissionSplits = _commissionSplitRepository.AllMatching(spec, serviceHeader);

                    if (commissionSplits != null && commissionSplits.Any())
                    {
                        return commissionSplits.ProjectedAsCollection<CommissionSplitDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionSplitDTO> FindCachedCommissionSplits(Guid commissionId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionSplitDTO>>(string.Format("CommissionSplitsByCommissionId_{0}_{1}", serviceHeader.ApplicationDomainName, commissionId.ToString("D")), () =>
            {
                return FindCommissionSplits(commissionId, serviceHeader);
            });
        }

        public bool UpdateCommissionSplits(Guid commissionId, List<CommissionSplitDTO> commissionSplits, ServiceHeader serviceHeader)
        {
            if (commissionId != null && commissionSplits != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _commissionRepository.Get(commissionId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindCommissionSplits(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var commissionSplit = _commissionSplitRepository.Get(item.Id, serviceHeader);

                                if (commissionSplit != null)
                                {
                                    _commissionSplitRepository.Remove(commissionSplit, serviceHeader);
                                }
                            }
                        }

                        if (commissionSplits.Any())
                        {
                            foreach (var item in commissionSplits)
                            {
                                var commissionSplit = CommissionSplitFactory.CreateCommissionSplit(persisted.Id, item.ChartOfAccountId, item.Description, item.Percentage, item.Leviable);

                                _commissionSplitRepository.Add(commissionSplit, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public List<CommissionDTO> GetCommissionsForSystemTransactionType(int systemTransactionType, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<SystemTransactionTypeInCommission> spec = SystemTransactionTypeInCommissionSpecifications.SystemTransactionType(systemTransactionType);

                var systemTransactionTypesInCommissions = _systemTransactionTypeInCommissionRepository.AllMatching(spec, serviceHeader);

                if (systemTransactionTypesInCommissions != null && systemTransactionTypesInCommissions.Any())
                {
                    var systemTransactionTypesInCommissionDTOs = systemTransactionTypesInCommissions.ProjectedAsCollection<SystemTransactionTypeInCommissionDTO>();

                    var projection = (from p in systemTransactionTypesInCommissionDTOs
                                      select new
                                      {
                                          p.ComplementType,
                                          p.ComplementPercentage,
                                          p.ComplementFixedAmount,
                                          p.Commission
                                      });

                    foreach (var item in projection)
                    {
                        // map complement
                        item.Commission.SystemTransactionType = systemTransactionType;
                        item.Commission.ComplementType = item.ComplementType;
                        item.Commission.ComplementPercentage = item.ComplementPercentage;
                        item.Commission.ComplementFixedAmount = item.ComplementFixedAmount;
                    }

                    return (from p in projection select p.Commission).ToList();
                }
                else return null;
            }
        }

        public bool AddSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO, ServiceHeader serviceHeader)
        {
            if (commissions != null && commissions.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _systemTransactionTypeInCommissionRepository.GetAll(serviceHeader);

                    Array.ForEach(commissions, (item) =>
                    {
                        var matches = allAuthTypes.Where(x => x.SystemTransactionType == systemTransactionType && x.CommissionId == item.Id);

                        if (matches.Any()) return;

                        var complement = new Charge(chargeDTO.Type, chargeDTO.Percentage, chargeDTO.FixedAmount);

                        var systemTransactionTypeInCommission = SystemTransactionTypeInCommissionFactory.CreateSystemTransactionTypeInCommission(systemTransactionType, item.Id, complement);

                        _systemTransactionTypeInCommissionRepository.Add(systemTransactionTypeInCommission, serviceHeader);
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool RemoveSystemTransactionTypeFromCommissions(int systemTransactionType, CommissionDTO[] commissions, ServiceHeader serviceHeader)
        {
            if (commissions != null && commissions.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var allAuthTypes = _systemTransactionTypeInCommissionRepository.GetAll(serviceHeader);

                    Array.ForEach(commissions, (item) =>
                    {
                        var matches = allAuthTypes.Where(x => x.SystemTransactionType == systemTransactionType && x.CommissionId == item.Id);

                        if (matches.Any())
                        {
                            matches.ToList().ForEach(x => _systemTransactionTypeInCommissionRepository.Remove(x, serviceHeader));
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) > -0;
                }
            }
            else return false;
        }

        public bool IsSystemTransactionTypeInCommission(int systemTransactionType, Guid commissionId, ServiceHeader serviceHeader)
        {
            if (commissionId == null || commissionId == Guid.Empty)
                return false;
            else
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = SystemTransactionTypeInCommissionSpecifications.SystemTransactionTypeAndCommissionId(systemTransactionType, commissionId);

                    // get the specification
                    ISpecification<SystemTransactionTypeInCommission> spec = filter;

                    // Query this criteria
                    var matches = _systemTransactionTypeInCommissionRepository.AllMatching(spec, serviceHeader);

                    return matches != null && matches.Any();
                }
            }
        }

        public bool MapSystemTransactionTypeToCommissions(int systemTransactionType, CommissionDTO[] commissions, ChargeDTO chargeDTO, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingCommissions = GetCommissionsForSystemTransactionType(systemTransactionType, serviceHeader);

                if (existingCommissions != null && existingCommissions.Any())
                {
                    var oldSet = from c in existingCommissions ?? new List<CommissionDTO> { } select c.Id;

                    var newSet = from c in commissions ?? new CommissionDTO[] { } select c.Id;

                    var commonSet = oldSet.Intersect(newSet);

                    var insertSet = newSet.Except(commonSet);

                    var deleteSet = oldSet.Except(commonSet);

                    foreach (var commissionId in insertSet)
                    {
                        var complement = new Charge(chargeDTO.Type, chargeDTO.Percentage, chargeDTO.FixedAmount);

                        var systemTransactionTypeInCommission = SystemTransactionTypeInCommissionFactory.CreateSystemTransactionTypeInCommission(systemTransactionType, commissionId, complement);

                        _systemTransactionTypeInCommissionRepository.Add(systemTransactionTypeInCommission, serviceHeader);
                    }

                    foreach (var commissionId in deleteSet)
                    {
                        var filter = SystemTransactionTypeInCommissionSpecifications.SystemTransactionTypeAndCommissionId(systemTransactionType, commissionId);

                        ISpecification<SystemTransactionTypeInCommission> spec = filter;

                        var matches = _systemTransactionTypeInCommissionRepository.AllMatching(spec, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persisted = _systemTransactionTypeInCommissionRepository.Get(mapping.Id, serviceHeader);

                                _systemTransactionTypeInCommissionRepository.Remove(persisted, serviceHeader);
                            }
                        }
                    }

                    foreach (var commissionId in commonSet)
                    {
                        var filter = SystemTransactionTypeInCommissionSpecifications.SystemTransactionTypeAndCommissionId(systemTransactionType, commissionId);

                        ISpecification<SystemTransactionTypeInCommission> spec = filter;

                        var matches = _systemTransactionTypeInCommissionRepository.AllMatching(spec, serviceHeader);

                        if (matches != null && matches.Any())
                        {
                            foreach (var mapping in matches)
                            {
                                var persisted = _systemTransactionTypeInCommissionRepository.Get(mapping.Id, serviceHeader);

                                var complement = new Charge(chargeDTO.Type, chargeDTO.Percentage, chargeDTO.FixedAmount);

                                persisted.Complement = complement;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in commissions)
                    {
                        var complement = new Charge(chargeDTO.Type, chargeDTO.Percentage, chargeDTO.FixedAmount);

                        var systemTransactionTypeInCommission = SystemTransactionTypeInCommissionFactory.CreateSystemTransactionTypeInCommission(systemTransactionType, item.Id, complement);

                        _systemTransactionTypeInCommissionRepository.Add(systemTransactionTypeInCommission, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool UpdateCommissionSplit(CommissionSplitDTO commissionSplitDTO, ServiceHeader serviceHeader)
        {
            if (commissionSplitDTO == null || commissionSplitDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _commissionSplitRepository.Get(commissionSplitDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    persisted.Leviable = commissionSplitDTO.Leviable;

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<TariffWrapper> ComputeTariffsBySystemTransactionType(int systemTransactionType, decimal totalValue, Guid debitChartOfAccountId, int debitChartOfAccountCode, string debitChartOfAccountName, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = GetCommissionsForSystemTransactionType(systemTransactionType, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = debitChartOfAccountId,
                                    DebitGLAccountCode = debitChartOfAccountCode,
                                    DebitGLAccountName = debitChartOfAccountName,
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

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
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsBySystemTransactionType(int systemTransactionType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = GetCommissionsForSystemTransactionType(systemTransactionType, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = levySplit.Description,
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByLoanProduct(Guid loanProductId, int dynamicChargeRecoverySource, int dynamicChargeRecoveryMode, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, int loanCaseLoanRegistrationTermInMonths, decimal topUpValue, decimal attachedLoansAmount, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var dynamicCharges = useCache ? _loanProductAppService.FindCachedDynamicCharges(loanProductId, serviceHeader) : _loanProductAppService.FindDynamicCharges(loanProductId, serviceHeader);

            if (dynamicCharges != null && dynamicCharges.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                dynamicCharges.ForEach(dynamicChargeDTO =>
                {
                    if (dynamicChargeDTO.RecoverySource == dynamicChargeRecoverySource && dynamicChargeDTO.RecoveryMode == dynamicChargeRecoveryMode)
                    {
                        var commissions = useCache ? FindCachedCommissionsByDynamicChargeId(dynamicChargeDTO.Id, serviceHeader) : FindCommissionsByDynamicChargeId(dynamicChargeDTO.Id, serviceHeader);

                        if (commissions != null && commissions.Any())
                        {
                            foreach (var commission in commissions)
                            {
                                if (commission.IsLocked) continue;

                                var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                                if (commissionExempted) continue;

                                var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                                if (graduatedScales != null && graduatedScales.Any())
                                {
                                    var workingAmount = (dynamicChargeDTO.ComputeChargeOnTopUp && topUpValue > 0m) ? topUpValue 
                                    : (dynamicChargeDTO.InstallmentsBasisValue == (int)DynamicChargeInstallmentsBasisValue.AttachedLoansAmount) ? attachedLoansAmount 
                                    : totalValue;

                                    var targetGraduatedScale = graduatedScales.Where(x => (workingAmount >= x.RangeLowerLimit && workingAmount <= x.RangeUpperLimit)).SingleOrDefault();

                                    if (targetGraduatedScale == null) continue;

                                    var commissionCharges = 0m;

                                    var leviableCommissionCharges = 0m;

                                    switch ((ChargeType)targetGraduatedScale.ChargeType)
                                    {
                                        case ChargeType.Percentage:
                                            commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(workingAmount)) / 100);
                                            commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                            break;
                                        case ChargeType.FixedAmount:
                                            commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                            commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                            break;
                                        default:
                                            break;
                                    }

                                    if (commissionCharges == 0m) continue;

                                    switch ((RoundingType)commission.RoundingType)
                                    {
                                        case RoundingType.ToEven:
                                            commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                            break;
                                        case RoundingType.AwayFromZero:
                                            commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                            break;
                                        case RoundingType.Ceiling:
                                            commissionCharges = Math.Ceiling(commissionCharges);
                                            break;
                                        case RoundingType.Floor:
                                            commissionCharges = Math.Floor(commissionCharges);
                                            break;
                                        default:
                                            break;
                                    }

                                    if (dynamicChargeDTO.FactorInLoanTerm && dynamicChargeRecoveryMode == (int)DynamicChargeRecoveryMode.Upfront)
                                    {
                                        var loanProductDTO = useCache ? _loanProductAppService.FindCachedLoanProduct(loanProductId, serviceHeader) : _loanProductAppService.FindLoanProduct(loanProductId, serviceHeader);

                                        if (loanProductDTO != null)
                                        {
                                            double NPer = (loanCaseLoanRegistrationTermInMonths != -1 ? loanCaseLoanRegistrationTermInMonths / 12d : loanProductDTO.LoanRegistrationTermInMonths / 12d) * loanProductDTO.LoanRegistrationPaymentFrequencyPerYear;

                                            commissionCharges = commissionCharges * (decimal)NPer;
                                        }
                                    }

                                    var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                                    if (commissionSplits != null && commissionSplits.Any())
                                    {
                                        foreach (var commissionSplit in commissionSplits)
                                        {
                                            var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                            if (commissionSplitValue == 0m) continue;

                                            tariffs.Add(new TariffWrapper
                                            {
                                                Amount = commissionSplitValue,
                                                Description = commissionSplit.Description,
                                                CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                                CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                                CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                                DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                                DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                                DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                                DebitCustomerAccount = customerAccountDTO,
                                                CreditCustomerAccount = customerAccountDTO,
                                                DynamicCharge = dynamicChargeDTO,
                                            });

                                            if (commissionSplit.Leviable)
                                                leviableCommissionCharges += commissionSplitValue;
                                        }
                                    }

                                    if (leviableCommissionCharges == 0m) continue;

                                    var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                                    if (levies != null && levies.Any())
                                    {
                                        foreach (var levy in levies)
                                        {
                                            if (levy.IsLocked) continue;

                                            var levyCharges = 0m;

                                            switch ((ChargeType)levy.ChargeType)
                                            {
                                                case ChargeType.Percentage:
                                                    levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                                    break;
                                                case ChargeType.FixedAmount:
                                                    levyCharges = levy.ChargeFixedAmount;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            if (levyCharges == 0m) continue;

                                            var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                            if (levySplits != null && levySplits.Any())
                                            {
                                                foreach (var levySplit in levySplits)
                                                {
                                                    var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                                    if (levySplitValue == 0m) continue;

                                                    tariffs.Add(new TariffWrapper
                                                    {
                                                        Amount = levySplitValue,
                                                        Description = levySplit.Description,
                                                        CreditGLAccountId = levySplit.ChartOfAccountId,
                                                        CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                                        CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                                        DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                                        DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                                        DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                                        DebitCustomerAccount = customerAccountDTO,
                                                        CreditCustomerAccount = customerAccountDTO,
                                                        DynamicCharge = dynamicChargeDTO,
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByPayoutCreditType(Guid creditTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? FindCachedCommissionsByCreditTypeId(creditTypeId, serviceHeader) : FindCommissionsByCreditTypeId(creditTypeId, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = levySplit.Description,
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByCheckOffCreditType(Guid creditTypeId, decimal totalValue, Guid debitChartOfAccountId, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? FindCachedCommissionsByCreditTypeId(creditTypeId, serviceHeader) : FindCommissionsByCreditTypeId(creditTypeId, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = debitChartOfAccountId,
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = levySplit.Description,
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = debitChartOfAccountId,
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByCreditBatchEntry(CreditBatchEntryDTO creditBatchEntry, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            if (creditBatchEntry != null)
            {
                var commissions = useCache ? FindCachedCommissionsByCreditTypeId(creditBatchEntry.CreditBatchCreditTypeId, serviceHeader) : FindCommissionsByCreditTypeId(creditBatchEntry.CreditBatchCreditTypeId, serviceHeader);

                if (commissions != null && commissions.Any())
                {
                    var totalValue = (creditBatchEntry.Principal + creditBatchEntry.Interest);

                    foreach (var commission in commissions)
                    {
                        if (commission.IsLocked) continue;

                        var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                        if (graduatedScales != null && graduatedScales.Any())
                        {
                            var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                            if (targetGraduatedScale == null) continue;

                            var commissionCharges = 0m;

                            var leviableCommissionCharges = 0m;

                            switch ((ChargeType)targetGraduatedScale.ChargeType)
                            {
                                case ChargeType.Percentage:
                                    commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                    commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                    break;
                                case ChargeType.FixedAmount:
                                    commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                    commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                    break;
                                default:
                                    break;
                            }

                            if (commissionCharges == 0m) continue;

                            switch ((RoundingType)commission.RoundingType)
                            {
                                case RoundingType.ToEven:
                                    commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                    break;
                                case RoundingType.AwayFromZero:
                                    commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                    break;
                                case RoundingType.Ceiling:
                                    commissionCharges = Math.Ceiling(commissionCharges);
                                    break;
                                case RoundingType.Floor:
                                    commissionCharges = Math.Floor(commissionCharges);
                                    break;
                                default:
                                    break;
                            }

                            var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                            if (commissionSplits != null && commissionSplits.Any())
                            {
                                foreach (var commissionSplit in commissionSplits)
                                {
                                    var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                    if (commissionSplitValue == 0m) continue;

                                    tariffs.Add(new TariffWrapper
                                    {
                                        Amount = commissionSplitValue,
                                        Description = commissionSplit.Description,
                                        CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                        CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                        CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                        DebitGLAccountId = creditBatchEntry.CreditBatchCreditTypeChartOfAccountId,
                                        DebitGLAccountCode = creditBatchEntry.CreditBatchCreditTypeChartOfAccountAccountCode,
                                        DebitGLAccountName = creditBatchEntry.CreditBatchCreditTypeChartOfAccountAccountName
                                    });

                                    if (commissionSplit.Leviable)
                                        leviableCommissionCharges += commissionSplitValue;
                                }
                            }

                            if (leviableCommissionCharges == 0m) continue;

                            var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                            if (levies != null && levies.Any())
                            {
                                foreach (var levy in levies)
                                {
                                    if (levy.IsLocked) continue;

                                    var levyCharges = 0m;

                                    switch ((ChargeType)levy.ChargeType)
                                    {
                                        case ChargeType.Percentage:
                                            levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                            break;
                                        case ChargeType.FixedAmount:
                                            levyCharges = levy.ChargeFixedAmount;
                                            break;
                                        default:
                                            break;
                                    }

                                    if (levyCharges == 0m) continue;

                                    var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                    if (levySplits != null && levySplits.Any())
                                    {
                                        foreach (var levySplit in levySplits)
                                        {
                                            var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                            if (levySplitValue == 0m) continue;

                                            tariffs.Add(new TariffWrapper
                                            {
                                                Amount = levySplitValue,
                                                Description = levySplit.Description,
                                                CreditGLAccountId = levySplit.ChartOfAccountId,
                                                CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                                CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                                DebitGLAccountId = creditBatchEntry.CreditBatchCreditTypeChartOfAccountId,
                                                DebitGLAccountCode = creditBatchEntry.CreditBatchCreditTypeChartOfAccountAccountCode,
                                                DebitGLAccountName = creditBatchEntry.CreditBatchCreditTypeChartOfAccountAccountName
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByChequeType(Guid chequeTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? FindCachedCommissionsByChequeTypeId(chequeTypeId, serviceHeader) : FindCommissionsByChequeTypeId(chequeTypeId, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = levySplit.Description,
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByChequeType(Guid chequeTypeId, decimal totalValue, Guid debitChartOfAccountId, int debitChartOfAccountCode, string debitChartOfAccountName, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? FindCachedCommissionsByChequeTypeId(chequeTypeId, serviceHeader) : FindCommissionsByChequeTypeId(chequeTypeId, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = debitChartOfAccountId,
                                    DebitGLAccountCode = debitChartOfAccountCode,
                                    DebitGLAccountName = debitChartOfAccountName,
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

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
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByDebitType(Guid debitTypeId, decimal totalValue, double multiplier, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? FindCachedCommissionsByDebitTypeId(debitTypeId, serviceHeader) : FindCommissionsByDebitTypeId(debitTypeId, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = commissionCharges * (decimal)multiplier;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = commissionCharges * (decimal)multiplier;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = levySplit.Description,
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByUnPayReason(Guid unPayReasonId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? FindCachedCommissionsByUnPayReasonId(unPayReasonId, serviceHeader) : FindCommissionsByUnPayReasonId(unPayReasonId, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = levySplit.Description,
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsBySavingsProduct(Guid savingsProductId, int savingsProductKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, double multiplier, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? _savingsProductAppService.FindCachedCommissions(savingsProductId, savingsProductKnownChargeType, serviceHeader) : _savingsProductAppService.FindCommissions(savingsProductId, savingsProductKnownChargeType, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                var institutionSettlementAccountId = useCache ? _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InstitutionSettlement, serviceHeader) : _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InstitutionSettlement, serviceHeader);

                var institutionSettlementAccountDTO = useCache ? _chartOfAccountAppService.FindCachedChartOfAccountSummary(institutionSettlementAccountId, serviceHeader) : _chartOfAccountAppService.FindChartOfAccountSummary(institutionSettlementAccountId, serviceHeader);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = commissionCharges * (decimal)multiplier;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = commissionCharges * (decimal)multiplier;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    ChargeBenefactor = commission.ChargeBenefactor,
                                    Amount = commissionSplitValue,
                                    Description = string.Format("{0}~{1}", commissionSplit.Description, EnumHelper.GetDescription((SavingsProductKnownChargeType)savingsProductKnownChargeType)),
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.Id : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId),
                                    DebitGLAccountCode = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountCode : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode),
                                    DebitGLAccountName = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountName : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName),
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            ChargeBenefactor = commission.ChargeBenefactor,
                                            Amount = levySplitValue,
                                            Description = string.Format("{0}~{1}", levySplit.Description, EnumHelper.GetDescription((SavingsProductKnownChargeType)savingsProductKnownChargeType)),
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.Id : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId),
                                            DebitGLAccountCode = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountCode : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode),
                                            DebitGLAccountName = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountName : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName),
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByLoanProduct(Guid loanProductId, int loanProductKnownChargeType, decimal bookBalance, decimal principalBalance, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? _loanProductAppService.FindCachedCommissions(loanProductId, loanProductKnownChargeType, serviceHeader) : _loanProductAppService.FindCommissions(loanProductId, loanProductKnownChargeType, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var workingAmount = 0m;

                        switch ((LoanProductChargeBasisValue)commission.ChargeBasisValue)
                        {
                            case LoanProductChargeBasisValue.PrincipalBalance:
                                workingAmount = principalBalance;
                                break;
                            case LoanProductChargeBasisValue.BookBalance:
                                workingAmount = bookBalance;
                                break;
                            default:
                                break;
                        }

                        var targetGraduatedScale = graduatedScales.Where(x => (workingAmount >= x.RangeLowerLimit && workingAmount <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(workingAmount)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = string.Format("{0}~{1}", commissionSplit.Description, EnumHelper.GetDescription((LoanProductKnownChargeType)loanProductKnownChargeType)),
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = string.Format("{0}~{1}", levySplit.Description, EnumHelper.GetDescription((LoanProductKnownChargeType)loanProductKnownChargeType)),
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByDynamicCharges(List<DynamicChargeDTO> dynamicCharges, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            if (dynamicCharges != null && dynamicCharges.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                dynamicCharges.ForEach(dynamicChargeDTO =>
                {
                    var commissions = useCache ? FindCachedCommissionsByDynamicChargeId(dynamicChargeDTO.Id, serviceHeader) : FindCommissionsByDynamicChargeId(dynamicChargeDTO.Id, serviceHeader);

                    if (commissions != null && commissions.Any())
                    {
                        foreach (var commission in commissions)
                        {
                            if (commission.IsLocked) continue;

                            var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                            if (commissionExempted) continue;

                            var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                            if (graduatedScales != null && graduatedScales.Any())
                            {
                                var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                                if (targetGraduatedScale == null) continue;

                                var commissionCharges = 0m;

                                var leviableCommissionCharges = 0m;

                                switch ((ChargeType)targetGraduatedScale.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                        commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                        break;
                                    case ChargeType.FixedAmount:
                                        commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                        commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                        break;
                                    default:
                                        break;
                                }

                                if (commissionCharges == 0m) continue;

                                switch ((RoundingType)commission.RoundingType)
                                {
                                    case RoundingType.ToEven:
                                        commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                        break;
                                    case RoundingType.AwayFromZero:
                                        commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                        break;
                                    case RoundingType.Ceiling:
                                        commissionCharges = Math.Ceiling(commissionCharges);
                                        break;
                                    case RoundingType.Floor:
                                        commissionCharges = Math.Floor(commissionCharges);
                                        break;
                                    default:
                                        break;
                                }

                                var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                                if (commissionSplits != null && commissionSplits.Any())
                                {
                                    foreach (var commissionSplit in commissionSplits)
                                    {
                                        var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                        if (commissionSplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = commissionSplitValue,
                                            Description = string.Format("{0}~{1}", commissionSplit.Description, dynamicChargeDTO.Description),
                                            CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                            CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });

                                        if (commissionSplit.Leviable)
                                            leviableCommissionCharges += commissionSplitValue;
                                    }
                                }

                                if (leviableCommissionCharges == 0m) continue;

                                var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                                if (levies != null && levies.Any())
                                {
                                    foreach (var levy in levies)
                                    {
                                        if (levy.IsLocked) continue;

                                        var levyCharges = 0m;

                                        switch ((ChargeType)levy.ChargeType)
                                        {
                                            case ChargeType.Percentage:
                                                levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                                break;
                                            case ChargeType.FixedAmount:
                                                levyCharges = levy.ChargeFixedAmount;
                                                break;
                                            default:
                                                break;
                                        }

                                        if (levyCharges == 0m) continue;

                                        var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                        if (levySplits != null && levySplits.Any())
                                        {
                                            foreach (var levySplit in levySplits)
                                            {
                                                var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                                if (levySplitValue == 0m) continue;

                                                tariffs.Add(new TariffWrapper
                                                {
                                                    Amount = levySplitValue,
                                                    Description = string.Format("{0}~{1}", levySplit.Description, dynamicChargeDTO.Description),
                                                    CreditGLAccountId = levySplit.ChartOfAccountId,
                                                    CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                                    CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                                    DebitCustomerAccount = customerAccountDTO,
                                                    CreditCustomerAccount = customerAccountDTO
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                });
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByAlternateChannelType(int alternateChannelType, int alternateChannelTypeKnownChargeType, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? _alternateChannelAppService.FindCachedCommissions(alternateChannelType, alternateChannelTypeKnownChargeType, serviceHeader) : _alternateChannelAppService.FindCommissions(alternateChannelType, alternateChannelTypeKnownChargeType, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                var institutionSettlementAccountId = useCache ? _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InstitutionSettlement, serviceHeader) : _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InstitutionSettlement, serviceHeader);

                var institutionSettlementAccountDTO = useCache ? _chartOfAccountAppService.FindCachedChartOfAccountSummary(institutionSettlementAccountId, serviceHeader) : _chartOfAccountAppService.FindChartOfAccountSummary(institutionSettlementAccountId, serviceHeader);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    ChargeBenefactor = commission.ChargeBenefactor,
                                    Amount = commissionSplitValue,
                                    Description = string.Format("{0}~{1}", commissionSplit.Description, EnumHelper.GetDescription((AlternateChannelKnownChargeType)alternateChannelTypeKnownChargeType)),
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.Id : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId),
                                    DebitGLAccountCode = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountCode : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode),
                                    DebitGLAccountName = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountName : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName),
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            ChargeBenefactor = commission.ChargeBenefactor,
                                            Amount = levySplitValue,
                                            Description = string.Format("{0}~{1}", levySplit.Description, EnumHelper.GetDescription((AlternateChannelKnownChargeType)alternateChannelTypeKnownChargeType)),
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.Id : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId),
                                            DebitGLAccountCode = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountCode : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode),
                                            DebitGLAccountName = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountName : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName),
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByTextAlert(int systemTransactionCode, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? _textAlertAppService.FindCachedCommissions(systemTransactionCode, serviceHeader) : _textAlertAppService.FindCommissions(systemTransactionCode, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                var institutionSettlementAccountId = useCache ? _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InstitutionSettlement, serviceHeader) : _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.InstitutionSettlement, serviceHeader);

                var institutionSettlementAccountDTO = useCache ? _chartOfAccountAppService.FindCachedChartOfAccountSummary(institutionSettlementAccountId, serviceHeader) : _chartOfAccountAppService.FindChartOfAccountSummary(institutionSettlementAccountId, serviceHeader);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    ChargeBenefactor = commission.ChargeBenefactor,
                                    Amount = commissionSplitValue,
                                    Description = string.Format("{0}~{1}", commissionSplit.Description, EnumHelper.GetDescription((SystemTransactionCode)systemTransactionCode)),
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.Id : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId),
                                    DebitGLAccountCode = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountCode : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode),
                                    DebitGLAccountName = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountName : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName),
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            ChargeBenefactor = commission.ChargeBenefactor,
                                            Amount = levySplitValue,
                                            Description = string.Format("{0}~{1}", levySplit.Description, EnumHelper.GetDescription((SystemTransactionCode)systemTransactionCode)),
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.Id : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId),
                                            DebitGLAccountCode = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountCode : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode),
                                            DebitGLAccountName = commission.ChargeBenefactor == (int)ChargeBenefactor.Customer ? customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName : (institutionSettlementAccountDTO != null ? institutionSettlementAccountDTO.AccountName : customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName),
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        public List<TariffWrapper> ComputeTariffsByWireTransferType(Guid wireTransferTypeId, decimal totalValue, CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader, bool useCache)
        {
            var tariffs = new List<TariffWrapper>();

            var commissions = useCache ? FindCachedCommissionsByWireTransferTypeId(wireTransferTypeId, serviceHeader) : FindCommissionsByWireTransferTypeId(wireTransferTypeId, serviceHeader);

            if (commissions != null && commissions.Any())
            {
                if (customerAccountDTO == null) return tariffs;

                FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, useCache);

                foreach (var commission in commissions)
                {
                    if (commission.IsLocked) continue;

                    var commissionExempted = useCache ? _commissionExemptionAppService.FetchCachedCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader) : _commissionExemptionAppService.FetchCustomerCommissionExemptionStatus(customerAccountDTO, commission.Id, serviceHeader);

                    if (commissionExempted) continue;

                    var graduatedScales = useCache ? FindCachedGraduatedScales(commission.Id, serviceHeader) : FindGraduatedScales(commission.Id, serviceHeader);

                    if (graduatedScales != null && graduatedScales.Any())
                    {
                        var targetGraduatedScale = graduatedScales.Where(x => (totalValue >= x.RangeLowerLimit && totalValue <= x.RangeUpperLimit)).SingleOrDefault();

                        if (targetGraduatedScale == null) continue;

                        var commissionCharges = 0m;

                        var leviableCommissionCharges = 0m;

                        switch ((ChargeType)targetGraduatedScale.ChargeType)
                        {
                            case ChargeType.Percentage:
                                commissionCharges = Convert.ToDecimal((targetGraduatedScale.ChargePercentage * Convert.ToDouble(totalValue)) / 100);
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            case ChargeType.FixedAmount:
                                commissionCharges = targetGraduatedScale.ChargeFixedAmount;
                                commissionCharges = Math.Min(commissionCharges, commission.MaximumCharge);
                                break;
                            default:
                                break;
                        }

                        if (commissionCharges == 0m) continue;

                        switch ((RoundingType)commission.RoundingType)
                        {
                            case RoundingType.ToEven:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.ToEven);
                                break;
                            case RoundingType.AwayFromZero:
                                commissionCharges = Math.Round(commissionCharges, MidpointRounding.AwayFromZero);
                                break;
                            case RoundingType.Ceiling:
                                commissionCharges = Math.Ceiling(commissionCharges);
                                break;
                            case RoundingType.Floor:
                                commissionCharges = Math.Floor(commissionCharges);
                                break;
                            default:
                                break;
                        }

                        var commissionSplits = useCache ? FindCachedCommissionSplits(commission.Id, serviceHeader) : FindCommissionSplits(commission.Id, serviceHeader);

                        if (commissionSplits != null && commissionSplits.Any())
                        {
                            foreach (var commissionSplit in commissionSplits)
                            {
                                var commissionSplitValue = Convert.ToDecimal((commissionSplit.Percentage * Convert.ToDouble(commissionCharges)) / 100);

                                if (commissionSplitValue == 0m) continue;

                                tariffs.Add(new TariffWrapper
                                {
                                    Amount = commissionSplitValue,
                                    Description = commissionSplit.Description,
                                    CreditGLAccountId = commissionSplit.ChartOfAccountId,
                                    CreditGLAccountCode = commissionSplit.ChartOfAccountAccountCode,
                                    CreditGLAccountName = commissionSplit.ChartOfAccountAccountName,
                                    DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                    DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                    DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                    DebitCustomerAccount = customerAccountDTO,
                                    CreditCustomerAccount = customerAccountDTO
                                });

                                if (commissionSplit.Leviable)
                                    leviableCommissionCharges += commissionSplitValue;
                            }
                        }

                        if (leviableCommissionCharges == 0m) continue;

                        var levies = useCache ? FindCachedLevies(commission.Id, serviceHeader) : FindLevies(commission.Id, serviceHeader);

                        if (levies != null && levies.Any())
                        {
                            foreach (var levy in levies)
                            {
                                if (levy.IsLocked) continue;

                                var levyCharges = 0m;

                                switch ((ChargeType)levy.ChargeType)
                                {
                                    case ChargeType.Percentage:
                                        levyCharges = Convert.ToDecimal((levy.ChargePercentage * Convert.ToDouble(leviableCommissionCharges)) / 100);
                                        break;
                                    case ChargeType.FixedAmount:
                                        levyCharges = levy.ChargeFixedAmount;
                                        break;
                                    default:
                                        break;
                                }

                                if (levyCharges == 0m) continue;

                                var levySplits = useCache ? FindCachedLevySplits(levy.Id, serviceHeader) : FindLevySplits(levy.Id, serviceHeader);

                                if (levySplits != null && levySplits.Any())
                                {
                                    foreach (var levySplit in levySplits)
                                    {
                                        var levySplitValue = Convert.ToDecimal((levySplit.Percentage * Convert.ToDouble(levyCharges)) / 100);

                                        if (levySplitValue == 0m) continue;

                                        tariffs.Add(new TariffWrapper
                                        {
                                            Amount = levySplitValue,
                                            Description = levySplit.Description,
                                            CreditGLAccountId = levySplit.ChartOfAccountId,
                                            CreditGLAccountCode = levySplit.ChartOfAccountAccountCode,
                                            CreditGLAccountName = levySplit.ChartOfAccountAccountName,
                                            DebitGLAccountId = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId,
                                            DebitGLAccountCode = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountCode,
                                            DebitGLAccountName = customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountName,
                                            DebitCustomerAccount = customerAccountDTO,
                                            CreditCustomerAccount = customerAccountDTO
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tariffs;
        }

        private void FetchCustomerAccountsProductDescription(List<CustomerAccountDTO> customerAccounts, ServiceHeader serviceHeader, bool useCache = true)
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
                                item.CustomerAccountTypeTargetProductChartOfAccountCode = savingsProduct.ChartOfAccountAccountCode;
                                item.CustomerAccountTypeTargetProductChartOfAccountName = savingsProduct.ChartOfAccountAccountName;
                                item.CustomerAccountTypeTargetProductDescription = savingsProduct.Description.Trim();
                                item.CustomerAccountTypeTargetProductIsDefault = savingsProduct.IsDefault;
                                item.CustomerAccountTypeTargetProductWithdrawalNoticeAmount = savingsProduct.WithdrawalNoticeAmount;
                                item.CustomerAccountTypeTargetProductWithdrawalNoticePeriod = savingsProduct.WithdrawalNoticePeriod;
                                item.CustomerAccountTypeTargetProductWithdrawalInterval = savingsProduct.WithdrawalInterval;
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
                                item.CustomerAccountTypeTargetProductMaturityPeriod = investmentProduct.MaturityPeriod;
                                item.CustomerAccountTypeTargetProductTransferBalanceToParentOnMembershipTermination = investmentProduct.TransferBalanceToParentOnMembershipTermination;
                                item.CustomerAccountTypeTargetProductParentId = investmentProduct.ParentId;
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        private List<LevySplitDTO> FindLevySplits(Guid levyId, ServiceHeader serviceHeader)
        {
            if (levyId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LevySplitSpecifications.LevySplitWithLevyId(levyId);

                    ISpecification<LevySplit> spec = filter;

                    var levySplits = _levySplitRepository.AllMatching(spec, serviceHeader);

                    if (levySplits != null && levySplits.Any())
                    {
                        return levySplits.ProjectedAsCollection<LevySplitDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<LevySplitDTO> FindCachedLevySplits(Guid levyId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<LevySplitDTO>>(string.Format("LevySplitsByLevyId_{0}_{1}", serviceHeader.ApplicationDomainName, levyId.ToString("D")), () =>
            {
                return FindLevySplits(levyId, serviceHeader);
            });
        }

        private List<CommissionDTO> FindCommissionsByDynamicChargeId(Guid dynamicChargeId, ServiceHeader serviceHeader)
        {
            if (dynamicChargeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DynamicChargeCommissionSpecifications.DynamicChargeCommissionWithDynamicChargeId(dynamicChargeId);

                    ISpecification<DynamicChargeCommission> spec = filter;

                    var dynamicChargeCommissions = _dynamicChargeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (dynamicChargeCommissions != null)
                    {
                        var projection = dynamicChargeCommissions.ProjectedAsCollection<DynamicChargeCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<CommissionDTO> FindCachedCommissionsByDynamicChargeId(Guid dynamicChargeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByDynamicChargeId_{0}_{1}", serviceHeader.ApplicationDomainName, dynamicChargeId.ToString("D")), () =>
            {
                return FindCommissionsByDynamicChargeId(dynamicChargeId, serviceHeader);
            });
        }

        private List<CommissionDTO> FindCommissionsByCreditTypeId(Guid creditTypeId, ServiceHeader serviceHeader)
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

        private List<CommissionDTO> FindCachedCommissionsByCreditTypeId(Guid creditTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByCreditTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, creditTypeId.ToString("D")), () =>
            {
                return FindCommissionsByCreditTypeId(creditTypeId, serviceHeader);
            });
        }

        private List<CommissionDTO> FindCommissionsByChequeTypeId(Guid chequeTypeId, ServiceHeader serviceHeader)
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

        private List<CommissionDTO> FindCachedCommissionsByChequeTypeId(Guid chequeTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByChequeTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, chequeTypeId.ToString("D")), () =>
            {
                return FindCommissionsByChequeTypeId(chequeTypeId, serviceHeader);
            });
        }

        private List<CommissionDTO> FindCommissionsByDebitTypeId(Guid debitTypeId, ServiceHeader serviceHeader)
        {
            if (debitTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = DebitTypeCommissionSpecifications.DebitTypeCommissionWithDebitTypeId(debitTypeId);

                    ISpecification<DebitTypeCommission> spec = filter;

                    var debitTypeCommissions = _debitTypeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (debitTypeCommissions != null)
                    {
                        var projection = debitTypeCommissions.ProjectedAsCollection<DebitTypeCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<CommissionDTO> FindCachedCommissionsByDebitTypeId(Guid debitTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByDebitTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, debitTypeId.ToString("D")), () =>
            {
                return FindCommissionsByDebitTypeId(debitTypeId, serviceHeader);
            });
        }

        private List<CommissionDTO> FindCommissionsByUnPayReasonId(Guid unPayReasonId, ServiceHeader serviceHeader)
        {
            if (unPayReasonId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = UnPayReasonCommissionSpecifications.UnPayReasonCommissionWithUnPayReasonId(unPayReasonId);

                    ISpecification<UnPayReasonCommission> spec = filter;

                    var unPayReasonCommissions = _unPayReasonCommissionRepository.AllMatching(spec, serviceHeader);

                    if (unPayReasonCommissions != null)
                    {
                        var projection = unPayReasonCommissions.ProjectedAsCollection<UnPayReasonCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<CommissionDTO> FindCachedCommissionsByUnPayReasonId(Guid unPayReasonId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByUnPayReasonId_{0}_{1}", serviceHeader.ApplicationDomainName, unPayReasonId.ToString("D")), () =>
            {
                return FindCommissionsByUnPayReasonId(unPayReasonId, serviceHeader);
            });
        }

        private List<CommissionDTO> FindCommissionsByWireTransferTypeId(Guid wireTransferTypeId, ServiceHeader serviceHeader)
        {
            if (wireTransferTypeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WireTransferTypeCommissionSpecifications.WireTransferTypeCommissionWithWireTransferTypeId(wireTransferTypeId);

                    ISpecification<WireTransferTypeCommission> spec = filter;

                    var wireTransferTypeCommissions = _wireTransferTypeCommissionRepository.AllMatching(spec, serviceHeader);

                    if (wireTransferTypeCommissions != null)
                    {
                        var projection = wireTransferTypeCommissions.ProjectedAsCollection<WireTransferTypeCommissionDTO>();

                        return (from p in projection select p.Commission).ToList();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private List<CommissionDTO> FindCachedCommissionsByWireTransferTypeId(Guid wireTransferTypeId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<List<CommissionDTO>>(string.Format("CommissionsByWireTransferTypeId_{0}_{1}", serviceHeader.ApplicationDomainName, wireTransferTypeId.ToString("D")), () =>
            {
                return FindCommissionsByWireTransferTypeId(wireTransferTypeId, serviceHeader);
            });
        }
    }
}
