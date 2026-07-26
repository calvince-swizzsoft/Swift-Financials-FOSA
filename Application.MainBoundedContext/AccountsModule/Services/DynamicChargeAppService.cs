using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeCommissionAgg;
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

        public DynamicChargeAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<DynamicCharge> dynamicChargeRepository,
           IRepository<DynamicChargeCommission> dynamicChargeCommissionRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (dynamicChargeRepository == null)
                throw new ArgumentNullException(nameof(dynamicChargeRepository));

            if (dynamicChargeCommissionRepository == null)
                throw new ArgumentNullException(nameof(dynamicChargeCommissionRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _dynamicChargeRepository = dynamicChargeRepository;
            _dynamicChargeCommissionRepository = dynamicChargeCommissionRepository;
        }

        public DynamicChargeDTO AddNewDynamicCharge(DynamicChargeDTO dynamicChargeDTO, ServiceHeader serviceHeader)
        {
            if (dynamicChargeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var dynamicCharge = DynamicChargeFactory.CreateDynamicCharge(dynamicChargeDTO.Description, dynamicChargeDTO.RecoveryMode, dynamicChargeDTO.RecoverySource, dynamicChargeDTO.InstallmentsBasisValue, dynamicChargeDTO.Installments, dynamicChargeDTO.FactorInLoanTerm, dynamicChargeDTO.ComputeChargeOnTopUp);

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
            if (dynamicChargeDTO == null || dynamicChargeDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _dynamicChargeRepository.Get(dynamicChargeDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DynamicChargeFactory.CreateDynamicCharge(dynamicChargeDTO.Description, dynamicChargeDTO.RecoveryMode, dynamicChargeDTO.RecoverySource, dynamicChargeDTO.InstallmentsBasisValue, dynamicChargeDTO.Installments, dynamicChargeDTO.FactorInLoanTerm, dynamicChargeDTO.ComputeChargeOnTopUp);

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
