using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SystemGeneralLedgerAccountMappingAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class SystemGeneralLedgerAccountMappingAppService : ISystemGeneralLedgerAccountMappingAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SystemGeneralLedgerAccountMapping> _systemGeneralLedgerAccountMappingRepository;

        public SystemGeneralLedgerAccountMappingAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SystemGeneralLedgerAccountMapping> systemGeneralLedgerAccountMappingRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (systemGeneralLedgerAccountMappingRepository == null)
                throw new ArgumentNullException(nameof(systemGeneralLedgerAccountMappingRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _systemGeneralLedgerAccountMappingRepository = systemGeneralLedgerAccountMappingRepository;
        }

        public SystemGeneralLedgerAccountMappingDTO AddNewSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, ServiceHeader serviceHeader)
        {
            if (systemGeneralLedgerAccountMappingDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    //get the specification
                    ISpecification<SystemGeneralLedgerAccountMapping> spec = SystemGeneralLedgerAccountMappingSpecifications.SystemGeneralLedgerAccountCode(systemGeneralLedgerAccountMappingDTO.SystemGeneralLedgerAccountCode);

                    //Query this criteria
                    var matchedsystemGeneralLedgerAccountMappings = _systemGeneralLedgerAccountMappingRepository.AllMatching(spec, serviceHeader);

                    if (matchedsystemGeneralLedgerAccountMappings != null && matchedsystemGeneralLedgerAccountMappings.Any())
                    {
                        systemGeneralLedgerAccountMappingDTO.ErrorMessageResult = string.Format("Sorry, but Account Code \"{0}\" already exists!", systemGeneralLedgerAccountMappingDTO.SystemGeneralLedgerAccountCodeDescription.ToUpper());

                        return systemGeneralLedgerAccountMappingDTO;
                    }
                    else
                    {

                        var systemGeneralLedgerAccountMapping = SystemGeneralLedgerAccountMappingFactory.CreateSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountMappingDTO.SystemGeneralLedgerAccountCode, systemGeneralLedgerAccountMappingDTO.ChartOfAccountId);

                        _systemGeneralLedgerAccountMappingRepository.Add(systemGeneralLedgerAccountMapping, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return systemGeneralLedgerAccountMapping.ProjectedAs<SystemGeneralLedgerAccountMappingDTO>();
                    }
                }
            }
            else return null;
        }

        public bool UpdateSystemGeneralLedgerAccountMapping(SystemGeneralLedgerAccountMappingDTO systemGeneralLedgerAccountMappingDTO, ServiceHeader serviceHeader)
        {
            if (systemGeneralLedgerAccountMappingDTO == null || systemGeneralLedgerAccountMappingDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _systemGeneralLedgerAccountMappingRepository.Get(systemGeneralLedgerAccountMappingDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = SystemGeneralLedgerAccountMappingFactory.CreateSystemGeneralLedgerAccountMapping(systemGeneralLedgerAccountMappingDTO.SystemGeneralLedgerAccountCode, systemGeneralLedgerAccountMappingDTO.ChartOfAccountId);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _systemGeneralLedgerAccountMappingRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var systemGeneralLedgerAccountMappings = _systemGeneralLedgerAccountMappingRepository.GetAll(serviceHeader);

                if (systemGeneralLedgerAccountMappings != null && systemGeneralLedgerAccountMappings.Any())
                {
                    return systemGeneralLedgerAccountMappings.ProjectedAsCollection<SystemGeneralLedgerAccountMappingDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SystemGeneralLedgerAccountMappingSpecifications.DefaultSpec();

                ISpecification<SystemGeneralLedgerAccountMapping> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var systemGeneralLedgerAccountMappingPagedCollection = _systemGeneralLedgerAccountMappingRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (systemGeneralLedgerAccountMappingPagedCollection != null)
                {
                    var pageCollection = systemGeneralLedgerAccountMappingPagedCollection.PageCollection.ProjectedAsCollection<SystemGeneralLedgerAccountMappingDTO>();

                    var itemsCount = systemGeneralLedgerAccountMappingPagedCollection.ItemsCount;

                    return new PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> FindSystemGeneralLedgerAccountMappings(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = SystemGeneralLedgerAccountMappingSpecifications.SystemGeneralLedgerAccountMappingFullText(text);

                ISpecification<SystemGeneralLedgerAccountMapping> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var systemGeneralLedgerAccountMappingCollection = _systemGeneralLedgerAccountMappingRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (systemGeneralLedgerAccountMappingCollection != null)
                {
                    var pageCollection = systemGeneralLedgerAccountMappingCollection.PageCollection.ProjectedAsCollection<SystemGeneralLedgerAccountMappingDTO>();

                    var itemsCount = systemGeneralLedgerAccountMappingCollection.ItemsCount;

                    return new PageCollectionInfo<SystemGeneralLedgerAccountMappingDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public SystemGeneralLedgerAccountMappingDTO FindSystemGeneralLedgerAccountMapping(Guid systemGeneralLedgerAccountMappingId, ServiceHeader serviceHeader)
        {
            if (systemGeneralLedgerAccountMappingId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var systemGeneralLedgerAccountMapping = _systemGeneralLedgerAccountMappingRepository.Get(systemGeneralLedgerAccountMappingId, serviceHeader);

                    if (systemGeneralLedgerAccountMapping != null)
                    {
                        return systemGeneralLedgerAccountMapping.ProjectedAs<SystemGeneralLedgerAccountMappingDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
