using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.EmployerAgg;
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
    public class EmployerAppService : IEmployerAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<Division> _divisionRepository;
        private readonly IRepository<Zone> _zoneRepository;

        public EmployerAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Employer> employerRepository,
           IRepository<Division> divisionRepository,
           IRepository<Zone> zoneRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employerRepository == null)
                throw new ArgumentNullException(nameof(employerRepository));

            if (divisionRepository == null)
                throw new ArgumentNullException(nameof(divisionRepository));

            if (zoneRepository == null)
                throw new ArgumentNullException(nameof(zoneRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employerRepository = employerRepository;
            _divisionRepository = divisionRepository;
            _zoneRepository = zoneRepository;
        }

        public async Task<EmployerDTO> AddNewEmployerAsync(EmployerDTO employerDTO, ServiceHeader serviceHeader)
        {
            var employerBindingModel = employerDTO.ProjectedAs<EmployerBindingModel>();

            employerBindingModel.ValidateAll();

            if (employerBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, employerBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var address = new Address(employerDTO.AddressAddressLine1, employerDTO.AddressAddressLine2, employerDTO.AddressStreet, employerDTO.AddressPostalCode, employerDTO.AddressCity, employerDTO.AddressEmail, employerDTO.AddressLandLine, employerDTO.AddressMobileLine);

                var employer = EmployerFactory.CreateEmployer(employerDTO.Description, address, employerDTO.RetirementAge, employerDTO.EnforceRetirementAge);

                if (employerDTO.IsLocked)
                    employer.Lock();
                else employer.UnLock();

                _employerRepository.Add(employer, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? employer.ProjectedAs<EmployerDTO>() : null;
            }
        }

        public async Task<bool> UpdateEmployerAsync(EmployerDTO employerDTO, ServiceHeader serviceHeader)
        {
            var employerBindingModel = employerDTO.ProjectedAs<EmployerBindingModel>();

            employerBindingModel.ValidateAll();

            if (employerBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, employerBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _employerRepository.Get(employerDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var address = new Address(employerDTO.AddressAddressLine1, employerDTO.AddressAddressLine2, employerDTO.AddressStreet, employerDTO.AddressPostalCode, employerDTO.AddressCity, employerDTO.AddressEmail, employerDTO.AddressLandLine, employerDTO.AddressMobileLine);

                    var current = EmployerFactory.CreateEmployer(employerDTO.Description, address, employerDTO.RetirementAge, employerDTO.EnforceRetirementAge);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (employerDTO.IsLocked)
                        current.Lock();
                    else current.UnLock();

                    _employerRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<List<EmployerDTO>> FindEmployersAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _employerRepository.GetAllAsync<EmployerDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<EmployerDTO>> FindEmployersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployerSpecifications.DefaultSpec();

                ISpecification<Employer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _employerRepository.AllMatchingPagedAsync<EmployerDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<EmployerDTO>> FindEmployersAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? EmployerSpecifications.DefaultSpec() : EmployerSpecifications.EmployerFullText(text);

                ISpecification<Employer> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _employerRepository.AllMatchingPagedAsync<EmployerDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<EmployerDTO> FindEmployerAsync(Guid employerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _employerRepository.GetAsync<EmployerDTO>(employerId, serviceHeader);
            }
        }

        public async Task<DivisionDTO> FindDivisionAsync(Guid divisionId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _divisionRepository.GetAsync<DivisionDTO>(divisionId, serviceHeader);
            }
        }

        public async Task<List<DivisionDTO>> FindDivisionsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _divisionRepository.GetAllAsync<DivisionDTO>(serviceHeader);
            }
        }

        public async Task<List<DivisionDTO>> FindDivisionsByEmployerIdAsync(Guid employerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DivisionSpecifications.DivisionWithEmployerId(employerId);

                ISpecification<Division> spec = filter;

                return await _divisionRepository.AllMatchingAsync<DivisionDTO>(spec, serviceHeader);
            }
        }

        public async Task<List<ZoneDTO>> FindZonesAsync(Guid employerId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ZoneSpecifications.ZoneWithEmployerId(employerId);

                ISpecification<Zone> spec = filter;

                return await _zoneRepository.AllMatchingAsync<ZoneDTO>(spec, serviceHeader);
            }
        }

        public async Task<bool> UpdateDivisionsAsync(Guid employerId, List<DivisionDTO> divisions, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _employerRepository.GetAsync(employerId, serviceHeader);

                if (persisted != null)
                {
                    if (persisted.Divisions != null && persisted.Divisions.Any())
                    {
                        foreach (var division in persisted.Divisions.ToList())
                        {
                            var filter = ZoneSpecifications.ZoneWithDivisionId(division.Id);

                            ISpecification<Zone> spec = filter;

                            var zones = await _zoneRepository.AllMatchingAsync(spec, serviceHeader);

                            if (!(zones != null && zones.Any()))
                            {
                                _divisionRepository.Remove(division, serviceHeader);
                            }
                        }
                    }

                    if (divisions.Any())
                    {
                        foreach (var item in divisions)
                        {
                            if (item.Id == Guid.Empty)
                            {
                                var division = DivisionFactory.CreateDivision(persisted.Id, item.Description);

                                _divisionRepository.Add(division, serviceHeader);
                            }
                            else
                            {
                                var filter = ZoneSpecifications.ZoneWithDivisionId(item.Id);

                                ISpecification<Zone> spec = filter;

                                var zones = await _zoneRepository.AllMatchingAsync(spec, serviceHeader);

                                if (!(zones != null && zones.Any()))
                                {
                                    var division = DivisionFactory.CreateDivision(persisted.Id, item.Description);

                                    _divisionRepository.Add(division, serviceHeader);
                                }
                            }
                        }
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }
    }
}
