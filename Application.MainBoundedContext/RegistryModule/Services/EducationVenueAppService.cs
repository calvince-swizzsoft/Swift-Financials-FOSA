using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EducationVenueAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class EducationVenueAppService : IEducationVenueAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EducationVenue> _educationVenueRepository;

        public EducationVenueAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EducationVenue> educationVenueRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (educationVenueRepository == null)
                throw new ArgumentNullException(nameof(educationVenueRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _educationVenueRepository = educationVenueRepository;
        }

        public EducationVenueDTO AddNewEducationVenue(EducationVenueDTO educationVenueDTO, ServiceHeader serviceHeader)
        {
            if (educationVenueDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var educationVenue = EducationVenueFactory.CreateEducationVenue(educationVenueDTO.Description, educationVenueDTO.EnforceInvestmentsBalance);

                    if (educationVenueDTO.IsLocked)
                        educationVenue.Lock();
                    else educationVenue.UnLock();

                    _educationVenueRepository.Add(educationVenue, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return educationVenue.ProjectedAs<EducationVenueDTO>();
                }
            }
            else return null;
        }

        public bool UpdateEducationVenue(EducationVenueDTO educationVenueDTO, ServiceHeader serviceHeader)
        {
            if (educationVenueDTO == null || educationVenueDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _educationVenueRepository.Get(educationVenueDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = EducationVenueFactory.CreateEducationVenue(educationVenueDTO.Description, educationVenueDTO.EnforceInvestmentsBalance);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    

                    if (educationVenueDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _educationVenueRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }

        public List<EducationVenueDTO> FindEducationVenues(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var educationVenues = _educationVenueRepository.GetAll(serviceHeader);

                if (educationVenues != null && educationVenues.Any())
                {
                    return educationVenues.ProjectedAsCollection<EducationVenueDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EducationVenueDTO> FindEducationVenues(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EducationVenueSpecifications.DefaultSpec();

                ISpecification<EducationVenue> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var educationVenuePagedCollection = _educationVenueRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (educationVenuePagedCollection != null)
                {
                    var pageCollection = educationVenuePagedCollection.PageCollection.ProjectedAsCollection<EducationVenueDTO>();

                    var itemsCount = educationVenuePagedCollection.ItemsCount;

                    return new PageCollectionInfo<EducationVenueDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EducationVenueDTO> FindEducationVenues(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EducationVenueSpecifications.EducationVenueFullText(text);

                ISpecification<EducationVenue> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var educationVenueCollection = _educationVenueRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (educationVenueCollection != null)
                {
                    var pageCollection = educationVenueCollection.PageCollection.ProjectedAsCollection<EducationVenueDTO>();

                    var itemsCount = educationVenueCollection.ItemsCount;

                    return new PageCollectionInfo<EducationVenueDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EducationVenueDTO FindEducationVenue(Guid educationVenueId, ServiceHeader serviceHeader)
        {
            if (educationVenueId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var educationVenue = _educationVenueRepository.Get(educationVenueId, serviceHeader);

                    if (educationVenue != null)
                    {
                        return educationVenue.ProjectedAs<EducationVenueDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
