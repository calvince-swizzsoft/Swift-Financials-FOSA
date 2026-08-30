using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class DynamicChargeAppService : IDynamicChargeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<DynamicCharge> _dynamicChargeRepository;
        private readonly IRepository<DynamicChargeCommission> _dynamicChargeCommissionRepository;
        private readonly IRepository<Commission> _commissionRepository;

        public DynamicChargeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<DynamicCharge> dynamicChargeRepository,
           IRepository<DynamicChargeCommission> dynamicChargeCommissionRepository,
           IRepository<Commission> commissionRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (dynamicChargeRepository == null)
                throw new ArgumentNullException(nameof(dynamicChargeRepository));

            if (dynamicChargeCommissionRepository == null)
                throw new ArgumentNullException(nameof(dynamicChargeCommissionRepository));

            if (commissionRepository == null)
                throw new ArgumentNullException(nameof(commissionRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _dynamicChargeRepository = dynamicChargeRepository;
            _dynamicChargeCommissionRepository = dynamicChargeCommissionRepository;
            _commissionRepository = commissionRepository;
        }

        public DynamicChargeDTO AddNewDynamicChargeConfiguration(DynamicChargeDTO dynamicChargeDTO, List<Guid> commissionIds, ServiceHeader serviceHeader)
        {
            ValidateDynamicChargeConfiguration(dynamicChargeDTO, commissionIds);
            using (var scope = _dbContextScopeFactory.Create())
            {
                var duplicate = _dynamicChargeRepository.AllMatching(DynamicChargeSpecifications.DynamicChargeWithDescription(dynamicChargeDTO.Description.Trim()), serviceHeader);
                if (duplicate != null && duplicate.Any()) throw new InvalidOperationException(string.Format("An indefinite charge named \"{0}\" already exists.", dynamicChargeDTO.Description.Trim()));
                var created = AddNewDynamicCharge(dynamicChargeDTO, serviceHeader);
                if (created == null) return null;
                var commissions = ResolveCommissions(commissionIds, serviceHeader);
                if (!UpdateCommissions(created.Id, commissions, serviceHeader)) throw new InvalidOperationException("Applicable charges could not be saved.");
                scope.SaveChanges(serviceHeader);
                return FindDynamicCharge(created.Id, serviceHeader);
            }
        }

        public DynamicChargeDTO UpdateDynamicChargeConfiguration(DynamicChargeDTO dynamicChargeDTO, List<Guid> commissionIds, ServiceHeader serviceHeader)
        {
            ValidateDynamicChargeConfiguration(dynamicChargeDTO, commissionIds);
            if (dynamicChargeDTO.Id == Guid.Empty) throw new InvalidOperationException("Indefinite charge ID is required.");
            using (var scope = _dbContextScopeFactory.Create())
            {
                var duplicate = _dynamicChargeRepository.AllMatching(DynamicChargeSpecifications.DynamicChargeWithDescription(dynamicChargeDTO.Description.Trim()), serviceHeader);
                if (duplicate != null && duplicate.Any(item => item.Id != dynamicChargeDTO.Id)) throw new InvalidOperationException(string.Format("An indefinite charge named \"{0}\" already exists.", dynamicChargeDTO.Description.Trim()));
                if (!UpdateDynamicCharge(dynamicChargeDTO, serviceHeader)) return null;
                if (!UpdateCommissions(dynamicChargeDTO.Id, ResolveCommissions(commissionIds, serviceHeader), serviceHeader)) throw new InvalidOperationException("Applicable charges could not be saved.");
                scope.SaveChanges(serviceHeader);
                return FindDynamicCharge(dynamicChargeDTO.Id, serviceHeader);
            }
        }

        public void ValidateDynamicChargeConfiguration(DynamicChargeDTO dynamicChargeDTO, List<Guid> commissionIds)
        {
            if (dynamicChargeDTO == null) throw new InvalidOperationException("Indefinite charge data is required.");
            if (string.IsNullOrWhiteSpace(dynamicChargeDTO.Description)) throw new InvalidOperationException("Indefinite charge name is required.");
            if (!Enum.IsDefined(typeof(DynamicChargeRecoveryMode), dynamicChargeDTO.RecoveryMode)) throw new InvalidOperationException("Select a valid recovery mode.");
            if (!Enum.IsDefined(typeof(DynamicChargeRecoverySource), dynamicChargeDTO.RecoverySource)) throw new InvalidOperationException("Select a valid recovery source.");
            if (dynamicChargeDTO.RecoverySource == (int)DynamicChargeRecoverySource.LoanAccount && dynamicChargeDTO.RecoveryMode != (int)DynamicChargeRecoveryMode.Upfront)
                throw new InvalidOperationException("Loan-account recovery is supported only for Upfront indefinite charges. Use Savings Account for Periodic or Carry Forward recovery.");
            if (commissionIds == null || !commissionIds.Any()) throw new InvalidOperationException("Select at least one applicable charge.");
            if (commissionIds.Any(id => id == Guid.Empty) || commissionIds.Distinct().Count() != commissionIds.Count) throw new InvalidOperationException("Applicable charge IDs must be valid and unique.");
        }

        private List<CommissionDTO> ResolveCommissions(IEnumerable<Guid> commissionIds, ServiceHeader serviceHeader)
        {
            return commissionIds.Select(id =>
            {
                var commission = _commissionRepository.Get(id, serviceHeader);
                if (commission == null) throw new InvalidOperationException(string.Format("Selected charge {0} does not exist.", id));
                if (commission.IsLocked) throw new InvalidOperationException(string.Format("Charge \"{0}\" is locked and cannot be attached.", commission.Description));
                return commission.ProjectedAs<CommissionDTO>();
            }).ToList();
        }

        public DynamicChargeDTO AddNewDynamicCharge(DynamicChargeDTO dynamicChargeDTO, ServiceHeader serviceHeader)
        {
            if (dynamicChargeDTO == null || string.IsNullOrWhiteSpace(dynamicChargeDTO.Description)) throw new InvalidOperationException("Indefinite charge name is required.");
            if (dynamicChargeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var dynamicCharge = DynamicChargeFactory.CreateDynamicCharge(dynamicChargeDTO.Description.Trim(), dynamicChargeDTO.RecoveryMode, dynamicChargeDTO.RecoverySource, dynamicChargeDTO.InstallmentsBasisValue, dynamicChargeDTO.Installments, dynamicChargeDTO.FactorInLoanTerm, dynamicChargeDTO.ComputeChargeOnTopUp);

                    if (dynamicChargeDTO.IsLocked)
                        dynamicCharge.Lock();
                    else dynamicCharge.UnLock();

                    _dynamicChargeRepository.Add(dynamicCharge, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return dynamicCharge.ProjectedAs<DynamicChargeDTO>();
                }
            }
            else return null;
        }

        public bool UpdateDynamicCharge(DynamicChargeDTO dynamicChargeDTO, ServiceHeader serviceHeader)
        {
            if (dynamicChargeDTO == null || string.IsNullOrWhiteSpace(dynamicChargeDTO.Description)) throw new InvalidOperationException("Indefinite charge name is required.");
            if (dynamicChargeDTO == null || dynamicChargeDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _dynamicChargeRepository.Get(dynamicChargeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DynamicChargeFactory.CreateDynamicCharge(dynamicChargeDTO.Description.Trim(), dynamicChargeDTO.RecoveryMode, dynamicChargeDTO.RecoverySource, dynamicChargeDTO.InstallmentsBasisValue, dynamicChargeDTO.Installments, dynamicChargeDTO.FactorInLoanTerm, dynamicChargeDTO.ComputeChargeOnTopUp);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (dynamicChargeDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _dynamicChargeRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<DynamicChargeDTO> FindDynamicCharges(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var dynamicCharges = _dynamicChargeRepository.GetAll(serviceHeader);

                if (dynamicCharges != null && dynamicCharges.Any())
                {
                    return dynamicCharges.ProjectedAsCollection<DynamicChargeDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DynamicChargeDTO> FindDynamicCharges(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DynamicChargeSpecifications.DefaultSpec();

                ISpecification<DynamicCharge> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var dynamicChargePagedCollection = _dynamicChargeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (dynamicChargePagedCollection != null)
                {
                    var pageCollection = dynamicChargePagedCollection.PageCollection.ProjectedAsCollection<DynamicChargeDTO>();

                    var itemsCount = dynamicChargePagedCollection.ItemsCount;

                    return new PageCollectionInfo<DynamicChargeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DynamicChargeDTO> FindDynamicCharges(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DynamicChargeSpecifications.DynamicChargeFullText(text);

                ISpecification<DynamicCharge> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var dynamicChargeCollection = _dynamicChargeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (dynamicChargeCollection != null)
                {
                    var pageCollection = dynamicChargeCollection.PageCollection.ProjectedAsCollection<DynamicChargeDTO>();

                    var itemsCount = dynamicChargeCollection.ItemsCount;

                    return new PageCollectionInfo<DynamicChargeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DynamicChargeDTO FindDynamicCharge(Guid dynamicChargeId, ServiceHeader serviceHeader)
        {
            if (dynamicChargeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var dynamicCharge = _dynamicChargeRepository.Get(dynamicChargeId, serviceHeader);

                    if (dynamicCharge != null)
                    {
                        return dynamicCharge.ProjectedAs<DynamicChargeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<CommissionDTO> FindCommissions(Guid dynamicChargeId, ServiceHeader serviceHeader)
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

        public bool UpdateCommissions(Guid dynamicChargeId, List<CommissionDTO> commissions, ServiceHeader serviceHeader)
        {
            if (dynamicChargeId != null && commissions != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _dynamicChargeRepository.Get(dynamicChargeId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = DynamicChargeCommissionSpecifications.DynamicChargeCommissionWithDynamicChargeId(dynamicChargeId);

                        ISpecification<DynamicChargeCommission> spec = filter;

                        var dynamicChargeCommissions = _dynamicChargeCommissionRepository.AllMatching(spec, serviceHeader);

                        if (dynamicChargeCommissions != null)
                        {
                            dynamicChargeCommissions.ToList().ForEach(x => _dynamicChargeCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (commissions.Any())
                        {
                            foreach (var item in commissions)
                            {
                                var dynamicChargeCommission = DynamicChargeCommissionFactory.CreateDynamicChargeCommission(persisted.Id, item.Id);

                                _dynamicChargeCommissionRepository.Add(dynamicChargeCommission, serviceHeader);
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
