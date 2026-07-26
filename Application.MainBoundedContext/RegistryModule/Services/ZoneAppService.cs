using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EmployerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.StationAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class ZoneAppService : IZoneAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<Division> _divisionRepository;
        private readonly IRepository<Zone> _zoneRepository;
        private readonly IRepository<Station> _stationRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public ZoneAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Employer> employerRepository,
           IRepository<Division> divisionRepository,
           IRepository<Zone> zoneRepository,
           IRepository<Station> stationRepository,
           IRepository<Customer> customerRepository,
           ISqlCommandAppService sqlCommandAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employerRepository == null)
                throw new ArgumentNullException(nameof(employerRepository));

            if (divisionRepository == null)
                throw new ArgumentNullException(nameof(divisionRepository));

            if (zoneRepository == null)
                throw new ArgumentNullException(nameof(zoneRepository));

            if (stationRepository == null)
                throw new ArgumentNullException(nameof(stationRepository));

            if (customerRepository == null)
                throw new ArgumentNullException(nameof(customerRepository));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employerRepository = employerRepository;
            _divisionRepository = divisionRepository;
            _zoneRepository = zoneRepository;
            _stationRepository = stationRepository;
            _customerRepository = customerRepository;
            _sqlCommandAppService = sqlCommandAppService;
        }

        public async Task<ZoneDTO> AddNewZoneAsync(ZoneDTO zoneDTO, ServiceHeader serviceHeader)
        {
            var zoneBindingModel = zoneDTO.ProjectedAs<ZoneBindingModel>();

            zoneBindingModel.ValidateAll();

            if (zoneBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, zoneBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var zone = ZoneFactory.CreateZone(zoneDTO.DivisionId, zoneDTO.Description);

                _zoneRepository.Add(zone, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? zone.ProjectedAs<ZoneDTO>() : null;
            }
        }

        public async Task<bool> UpdateZoneAsync(ZoneDTO zoneDTO, ServiceHeader serviceHeader)
        {
            var zoneBindingModel = zoneDTO.ProjectedAs<ZoneBindingModel>();

            zoneBindingModel.ValidateAll();

            if (zoneBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, zoneBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _zoneRepository.GetAsync(zoneDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = ZoneFactory.CreateZone(zoneDTO.DivisionId, zoneDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _zoneRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<List<ZoneDTO>> FindZonesAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _zoneRepository.GetAllAsync<ZoneDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<ZoneDTO>> FindZonesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ZoneSpecifications.DefaultSpec();

                ISpecification<Zone> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _zoneRepository.AllMatchingPagedAsync<ZoneDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<ZoneDTO>> FindZonesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? ZoneSpecifications.DefaultSpec() : ZoneSpecifications.ZoneFullText(text);

                ISpecification<Zone> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _zoneRepository.AllMatchingPagedAsync<ZoneDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<ZoneDTO> FindZoneAsync(Guid zoneId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _zoneRepository.GetAsync<ZoneDTO>(zoneId, serviceHeader);
            }
        }

        public async Task<StationDTO> FindStationAsync(Guid stationId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _stationRepository.GetAsync<StationDTO>(stationId, serviceHeader);
            }
        }

        public async Task<List<StationDTO>> FindStationsByZoneIdAsync(Guid zoneId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StationSpecifications.StationWithZoneId(zoneId);

                ISpecification<Station> spec = filter;

                return await _stationRepository.AllMatchingAsync<StationDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<StationDTO>> FindStationsByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StationSpecifications.StationWithEmployerId(employerId);

                ISpecification<Station> spec = filter;

                return await _stationRepository.AllMatchingAsync<StationDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<StationDTO>> FindStationsByDivisionIdAsync(Guid divisionId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StationSpecifications.StationWithDivisionId(divisionId);

                ISpecification<Station> spec = filter;

                return (await _stationRepository.AllMatchingAsync<StationDTO>(spec, serviceHeader)).ToList();
            }
        }

        public async Task<List<StationDTO>> FindStationsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StationSpecifications.DefaultSpec();

                ISpecification<Station> spec = filter;

                return await _stationRepository.AllMatchingAsync<StationDTO>(spec, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<StationDTO>> FindStationsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = StationSpecifications.StationFullText(text);

                ISpecification<Station> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _stationRepository.AllMatchingPagedAsync<StationDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<bool> UpdateStationsAsync(Guid zoneId, List<StationDTO> stations, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _zoneRepository.GetAsync(zoneId, serviceHeader);

                if (persisted != null)
                {
                    if (persisted.Stations != null && persisted.Stations.Any())
                    {
                        foreach (var station in persisted.Stations.ToList())
                        {
                            var filter = CustomerSpecifications.CustomerWithStationId(station.Id, string.Empty, (int)CustomerFilter.Reference1);

                            ISpecification<Customer> spec = filter;

                            var customers = await _customerRepository.AllMatchingAsync(spec, serviceHeader);

                            if (!(customers != null && customers.Any()))
                            {
                                _stationRepository.Remove(station, serviceHeader);
                            }
                        }
                    }

                    if (stations.Any())
                    {
                        foreach (var item in stations)
                        {
                            if (item.Id == Guid.Empty)
                            {
                                var address = new Address(item.AddressAddressLine1, item.AddressAddressLine2, item.AddressStreet, item.AddressPostalCode, item.AddressCity, item.AddressEmail, item.AddressLandLine, item.AddressMobileLine);

                                var station = StationFactory.CreateStation(persisted.Id, item.Description, address);

                                _stationRepository.Add(station, serviceHeader);
                            }
                            else
                            {
                                var filter = CustomerSpecifications.CustomerWithStationId(item.Id, string.Empty, (int)CustomerFilter.Reference1);

                                ISpecification<Customer> spec = filter;

                                var customers = await _customerRepository.AllMatchingAsync(spec, serviceHeader);

                                if (!(customers != null && customers.Any()))
                                {
                                    var address = new Address(item.AddressAddressLine1, item.AddressAddressLine2, item.AddressStreet, item.AddressPostalCode, item.AddressCity, item.AddressEmail, item.AddressLandLine, item.AddressMobileLine);

                                    var station = StationFactory.CreateStation(persisted.Id, item.Description, address);

                                    _stationRepository.Add(station, serviceHeader);
                                }
                            }
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public bool RemoveStation(Guid stationId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (stationId != null && stationId != Guid.Empty)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _stationRepository.Get(stationId, serviceHeader);

                    if (persisted != null)
                    {
                        var filter = CustomerSpecifications.CustomerWithStationId(stationId, string.Empty, (int)CustomerFilter.Reference1);

                        ISpecification<Customer> spec = filter;

                        var customers = _customerRepository.AllMatching(spec, serviceHeader);

                        if ((customers != null && customers.Any()))
                        {
                            foreach (var customer in customers)
                            {
                                var persistedCustomer = _customerRepository.Get(customer.Id, serviceHeader);

                                if (persistedCustomer != null)
                                {
                                    persistedCustomer.StationId = null;
                                }
                            }
                        }
                        _stationRepository.Remove(persisted, serviceHeader);

                        result = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
            }

            return result;
        }

        public async Task<bool> RemoveStationAsync(Guid stationId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _stationRepository.GetAsync(stationId, serviceHeader);

                if (persisted != null)
                {
                    var recordsAffected = await _sqlCommandAppService.DelinkStationAsync(persisted.Id, serviceHeader);

                    _stationRepository.Remove(persisted, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> RemoveZoneAsync(Guid zoneId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _zoneRepository.GetAsync(zoneId, serviceHeader);

                if (persisted != null)
                {
                    var stations = await FindStationsByZoneIdAsync(zoneId, serviceHeader);

                    if (stations != null && stations.Any())
                    {
                        foreach (var station in stations)
                        {
                            await RemoveStationAsync(station.Id, serviceHeader);
                        }
                    }
                    _zoneRepository.Remove(persisted, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }
        public async Task<bool> RemoveDivisionAsync(Guid divisionId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _divisionRepository.GetAsync(divisionId, serviceHeader);

                if (persisted != null)
                {
                    var stations = await FindStationsByDivisionIdAsync(divisionId, serviceHeader);

                    if (stations != null && stations.Any())
                    {
                        foreach (var station in stations)
                        {
                            await RemoveZoneAsync(station.ZoneId, serviceHeader);
                        }
                    }
                    _divisionRepository.Remove(persisted, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<bool> RemoveEmployerAsync(Guid employerId, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _employerRepository.GetAsync(employerId, serviceHeader);

                if (persisted != null)
                {
                    var stations = await FindStationsByEmployerIdAsync(employerId, serviceHeader);

                    if (stations != null && stations.Any())
                    {
                        foreach (var station in stations)
                        {
                            await RemoveDivisionAsync(station.ZoneDivisionId, serviceHeader);
                        }
                    }
                    _employerRepository.Remove(persisted, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }
    }
}
