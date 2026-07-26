using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonCommissionAgg;
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
    public class UnPayReasonAppService : IUnPayReasonAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<UnPayReason> _unPayReasonRepository;
        private readonly IRepository<UnPayReasonCommission> _unPayReasonCommissionRepository;
        private readonly IAppCache _appCache;

        public UnPayReasonAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<UnPayReason> unPayReasonRepository,
           IRepository<UnPayReasonCommission> unPayReasonCommissionRepository,
           IAppCache appCache)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (unPayReasonRepository == null)
                throw new ArgumentNullException(nameof(unPayReasonRepository));

            if (unPayReasonCommissionRepository == null)
                throw new ArgumentNullException(nameof(unPayReasonCommissionRepository));

            if (appCache == null)
                throw new ArgumentNullException(nameof(appCache));

            _dbContextScopeFactory = dbContextScopeFactory;
            _unPayReasonRepository = unPayReasonRepository;
            _unPayReasonCommissionRepository = unPayReasonCommissionRepository;
            _appCache = appCache;
        }

        public UnPayReasonDTO AddNewUnPayReason(UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader)
        {
            if (unPayReasonDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    ISpecification<UnPayReason> spec = UnPayReasonSpecifications.UnapayReasonDescription(unPayReasonDTO.Description);

                    var matchedunPayReason = _unPayReasonRepository.AllMatching(spec, serviceHeader);

                    if (matchedunPayReason != null && matchedunPayReason.Any())
                    {
                        //throw new InvalidOperationException(string.Format("Sorry, but Account Code {0} already exists!", chartOfAccountDTO.AccountCode));
                        unPayReasonDTO.ErrorMessageResult = string.Format("Sorry, but Unpay Reason {0} already exists!", unPayReasonDTO.Description.ToUpper());
                        return unPayReasonDTO;
                    }
                    else
                    {
                        var unPayReason = UnPayReasonFactory.CreateUnPayReason(unPayReasonDTO.Code, unPayReasonDTO.Description);

                        if (unPayReasonDTO.IsLocked)
                            unPayReason.Lock();
                        else unPayReason.UnLock();

                        _unPayReasonRepository.Add(unPayReason, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return unPayReason.ProjectedAs<UnPayReasonDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateUnPayReason(UnPayReasonDTO unPayReasonDTO, ServiceHeader serviceHeader)
        {
            if (unPayReasonDTO == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _unPayReasonRepository.Get(unPayReasonDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = UnPayReasonFactory.CreateUnPayReason(unPayReasonDTO.Code, unPayReasonDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (unPayReasonDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _unPayReasonRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<UnPayReasonDTO> FindUnPayReasons(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var unPayReasons = _unPayReasonRepository.GetAll(serviceHeader);

                if (unPayReasons != null && unPayReasons.Any())
                {
                    return unPayReasons.ProjectedAsCollection<UnPayReasonDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<UnPayReasonDTO> FindUnPayReasons(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = UnPayReasonSpecifications.DefaultSpec();

                ISpecification<UnPayReason> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var unPayReasonPagedCollection = _unPayReasonRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (unPayReasonPagedCollection != null)
                {
                    var pageCollection = unPayReasonPagedCollection.PageCollection.ProjectedAsCollection<UnPayReasonDTO>();

                    var itemsCount = unPayReasonPagedCollection.ItemsCount;

                    return new PageCollectionInfo<UnPayReasonDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<UnPayReasonDTO> FindUnPayReasons(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = UnPayReasonSpecifications.UnPayReasonFullText(text);

                ISpecification<UnPayReason> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var unPayReasonCollection = _unPayReasonRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (unPayReasonCollection != null)
                {
                    var pageCollection = unPayReasonCollection.PageCollection.ProjectedAsCollection<UnPayReasonDTO>();

                    var itemsCount = unPayReasonCollection.ItemsCount;

                    return new PageCollectionInfo<UnPayReasonDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public UnPayReasonDTO FindUnPayReason(Guid unPayReasonId, ServiceHeader serviceHeader)
        {
            if (unPayReasonId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var unPayReason = _unPayReasonRepository.Get(unPayReasonId, serviceHeader);

                    if (unPayReason != null)
                    {
                        return unPayReason.ProjectedAs<UnPayReasonDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public UnPayReasonDTO FindCachedUnPayReason(Guid unPayReasonId, ServiceHeader serviceHeader)
        {
            return _appCache.GetOrAdd<UnPayReasonDTO>(string.Format("{0}_{1}", serviceHeader.ApplicationDomainName, unPayReasonId.ToString("D")), () =>
            {
                return FindUnPayReason(unPayReasonId, serviceHeader);
            });
        }

        public List<CommissionDTO> FindCommissions(Guid unPayReasonId, ServiceHeader serviceHeader)
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

        public bool UpdateCommissions(Guid unPayReasonId, List<CommissionDTO> commissions, ServiceHeader serviceHeader)
        {
            if (unPayReasonId != null && commissions != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _unPayReasonRepository.Get(unPayReasonId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = UnPayReasonCommissionSpecifications.UnPayReasonCommissionWithUnPayReasonId(unPayReasonId);

                        ISpecification<UnPayReasonCommission> spec = filter;

                        var unPayReasonCommissions = _unPayReasonCommissionRepository.AllMatching(spec, serviceHeader);

                        if (unPayReasonCommissions != null)
                        {
                            unPayReasonCommissions.ToList().ForEach(x => _unPayReasonCommissionRepository.Remove(x, serviceHeader));
                        }

                        if (commissions.Any())
                        {
                            foreach (var item in commissions)
                            {
                                var unPayReasonCommission = UnPayReasonCommissionFactory.CreateUnPayReasonCommission(persisted.Id, item.Id);

                                _unPayReasonCommissionRepository.Add(unPayReasonCommission, serviceHeader);
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
