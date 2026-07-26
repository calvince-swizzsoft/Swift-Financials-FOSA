using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.Seedwork;
using Domain.MainBoundedContext.AdministrationModule.Aggregates.LocationAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public class LocationAppService : ILocationAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Location> _locationRepository;

        public LocationAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<Location> locationRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (locationRepository == null)
                throw new ArgumentNullException(nameof(locationRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _locationRepository = locationRepository;
        }

        public LocationDTO AddNewLocation(LocationDTO locationDTO, ServiceHeader serviceHeader)
        {
            if (locationDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var location = LocationFactory.CreateLocation(locationDTO.BranchId, locationDTO.Description);

                    location.CreatedBy = serviceHeader.ApplicationUserName;

                    location.CreatedDate = DateTime.Now;

                    if (locationDTO.IsLocked)
                        location.Lock();
                    else location.UnLock();

                    _locationRepository.Add(location, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return location.ProjectedAs<LocationDTO>();
                }
            }
            else return null;
        }

        public bool UpdateLocation(LocationDTO locationDTO, ServiceHeader serviceHeader)
        {
            if (locationDTO == null || locationDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _locationRepository.Get(locationDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = LocationFactory.CreateLocation(locationDTO.BranchId, locationDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (locationDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _locationRepository.Merge(persisted, current, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return true;
                }
                else return false;
            }
        }

        public List<LocationDTO> FindLocations(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var locations = _locationRepository.GetAll(serviceHeader);

                if (locations != null && locations.Any())
                {
                    return locations.ProjectedAsCollection<LocationDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<LocationDTO> FindLocations(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LocationSpecifications.DefaultSpec();

                ISpecification<Location> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var locationPagedCollection = _locationRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (locationPagedCollection != null)
                {
                    var pageCollection = locationPagedCollection.PageCollection.ProjectedAsCollection<LocationDTO>();

                    var itemsCount = locationPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LocationDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LocationDTO> FindLocations(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LocationSpecifications.LocationFullText(text);

                ISpecification<Location> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var locationCollection = _locationRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (locationCollection != null)
                {
                    var pageCollection = locationCollection.PageCollection.ProjectedAsCollection<LocationDTO>();

                    var itemsCount = locationCollection.ItemsCount;

                    return new PageCollectionInfo<LocationDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public LocationDTO FindLocation(Guid locationId, ServiceHeader serviceHeader)
        {
            if (locationId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var location = _locationRepository.Get(locationId, serviceHeader);

                    if (location != null)
                    {
                        return location.ProjectedAs<LocationDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
