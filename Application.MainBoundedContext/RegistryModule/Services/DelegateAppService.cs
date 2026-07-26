using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class DelegateAppService : IDelegateAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg.Delegate> _delegateRepository;

        public DelegateAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg.Delegate> delegateRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (delegateRepository == null)
                throw new ArgumentNullException(nameof(delegateRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _delegateRepository = delegateRepository;
        }

        public DelegateDTO AddNewDelegate(DelegateDTO delegateDTO, ServiceHeader serviceHeader)
        {
            if (delegateDTO == null)
                return null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // get the specification
                var filter = DelegateSpecifications.DelegateWithCustomerId(delegateDTO.CustomerId);

                ISpecification<Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg.Delegate> spec = filter;

                //Query this criteria
                var delegates = _delegateRepository.AllMatching(spec, serviceHeader);

                if (delegates != null && delegates.Any())
                    return null;
                else
                {
                    var @delegate = DelegateFactory.CreateDelegate(delegateDTO.ZoneId, delegateDTO.CustomerId, delegateDTO.Remarks);

                    if (delegateDTO.IsLocked)
                        @delegate.Lock();
                    else @delegate.UnLock();

                    _delegateRepository.Add(@delegate, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return @delegate.ProjectedAs<DelegateDTO>();
                }
            }
        }

        public bool UpdateDelegate(DelegateDTO delegateDTO, ServiceHeader serviceHeader)
        {
            if (delegateDTO == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _delegateRepository.Get(delegateDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DelegateFactory.CreateDelegate(delegateDTO.ZoneId, delegateDTO.CustomerId, delegateDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (delegateDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _delegateRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<DelegateDTO> FindDelegates(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg.Delegate> spec = DelegateSpecifications.DefaultSpec();

                var delegates = _delegateRepository.AllMatching(spec, serviceHeader);

                if (delegates != null && delegates.Any())
                {
                    return delegates.ProjectedAsCollection<DelegateDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DelegateDTO> FindDelegates(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DelegateSpecifications.DefaultSpec();

                ISpecification<Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg.Delegate> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var delegatePagedCollection = _delegateRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (delegatePagedCollection != null)
                {
                    var pageCollection = delegatePagedCollection.PageCollection.ProjectedAsCollection<DelegateDTO>();

                    var itemsCount = delegatePagedCollection.ItemsCount;

                    return new PageCollectionInfo<DelegateDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DelegateDTO> FindDelegates(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? DelegateSpecifications.DefaultSpec() : DelegateSpecifications.DelegateFullText(text);

                ISpecification<Domain.MainBoundedContext.RegistryModule.Aggregates.DelegateAgg.Delegate> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var delegatePagedCollection = _delegateRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (delegatePagedCollection != null)
                {
                    var pageCollection = delegatePagedCollection.PageCollection.ProjectedAsCollection<DelegateDTO>();

                    var itemsCount = delegatePagedCollection.ItemsCount;

                    return new PageCollectionInfo<DelegateDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DelegateDTO FindDelegate(Guid delegateId, ServiceHeader serviceHeader)
        {
            if (delegateId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var @delegate = _delegateRepository.Get(delegateId, serviceHeader);

                    if (@delegate != null)
                    {
                        return @delegate.ProjectedAs<DelegateDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
