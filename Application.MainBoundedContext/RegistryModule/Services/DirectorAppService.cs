using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DirectorAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class DirectorAppService : IDirectorAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Director> _directorRepository;

        public DirectorAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Director> directorRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (directorRepository == null)
                throw new ArgumentNullException(nameof(directorRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _directorRepository = directorRepository;
        }

        public DirectorDTO AddNewDirector(DirectorDTO directorDTO, ServiceHeader serviceHeader)
        {
            if (directorDTO == null)
                return null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // get the specification
                var filter = DirectorSpecifications.DirectorWithCustomerId(directorDTO.CustomerId);

                ISpecification<Director> spec = filter;

                //Query this criteria
                var directors = _directorRepository.AllMatching(spec, serviceHeader);

                if (directors != null && directors.Any())
                    return null;
                else
                {
                    var director = DirectorFactory.CreateDirector(directorDTO.DivisionId, directorDTO.CustomerId, directorDTO.Remarks);

                    if (directorDTO.IsLocked)
                        director.Lock();
                    else director.UnLock();

                    _directorRepository.Add(director, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return director.ProjectedAs<DirectorDTO>();
                }
            }
        }

        public bool UpdateDirector(DirectorDTO directorDTO, ServiceHeader serviceHeader)
        {
            if (directorDTO == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _directorRepository.Get(directorDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DirectorFactory.CreateDirector(directorDTO.DivisionId, directorDTO.CustomerId, directorDTO.Remarks);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (directorDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _directorRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<DirectorDTO> FindDirectors(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<Director> spec = DirectorSpecifications.DefaultSpec();

                var directors = _directorRepository.AllMatching(spec, serviceHeader);

                if (directors != null && directors.Any())
                {
                    return directors.ProjectedAsCollection<DirectorDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<DirectorDTO> FindDirectors(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DirectorSpecifications.DefaultSpec();

                ISpecification<Director> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var directorPagedCollection = _directorRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (directorPagedCollection != null)
                {
                    var pageCollection = directorPagedCollection.PageCollection.ProjectedAsCollection<DirectorDTO>();

                    var itemsCount = directorPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DirectorDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<DirectorDTO> FindDirectors(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? DirectorSpecifications.DefaultSpec() : DirectorSpecifications.DirectorFullText(text);

                ISpecification<Director> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var directorPagedCollection = _directorRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (directorPagedCollection != null)
                {
                    var pageCollection = directorPagedCollection.PageCollection.ProjectedAsCollection<DirectorDTO>();

                    var itemsCount = directorPagedCollection.ItemsCount;

                    return new PageCollectionInfo<DirectorDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public DirectorDTO FindDirector(Guid directorId, ServiceHeader serviceHeader)
        {
            if (directorId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var director = _directorRepository.Get(directorId, serviceHeader);

                    if (director != null)
                    {
                        return director.ProjectedAs<DirectorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
