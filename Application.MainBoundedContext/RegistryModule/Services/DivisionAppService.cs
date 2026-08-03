using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Domain.MainBoundedContext.RegistryModule.Aggregates.DivisionAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.ZoneAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class DivisionAppService : IDivisionAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Division> _divisionRepository;
        private readonly IRepository<Zone> _zoneRepository;

        public DivisionAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<Division> divisionRepository,
           IRepository<Zone> zoneRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (divisionRepository == null)
                throw new ArgumentNullException(nameof(divisionRepository));

            if (zoneRepository == null)
                throw new ArgumentNullException(nameof(zoneRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _divisionRepository = divisionRepository;
            _zoneRepository = zoneRepository;
        }

        public async Task<DivisionDTO> AddNewDivisionAsync(DivisionDTO divisionDTO, ServiceHeader serviceHeader)
        {
            var divisionBindingModel = divisionDTO.ProjectedAs<DivisionBindingModel>();

            divisionBindingModel.ValidateAll();

            if (divisionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, divisionBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var division = DivisionFactory.CreateDivision(divisionDTO.EmployerId, divisionDTO.Description);

                _divisionRepository.Add(division, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? division.ProjectedAs<DivisionDTO>() : null;
            }
        }

        public async Task<bool> UpdateDivisionAsync(DivisionDTO divisionDTO, ServiceHeader serviceHeader)
        {
            var divisionBindingModel = divisionDTO.ProjectedAs<DivisionBindingModel>();

            divisionBindingModel.ValidateAll();

            if (divisionBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, divisionBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _divisionRepository.GetAsync(divisionDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    var current = DivisionFactory.CreateDivision(divisionDTO.EmployerId, divisionDTO.Description);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    _divisionRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public async Task<List<DivisionDTO>> FindDivisionsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _divisionRepository.GetAllAsync<DivisionDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<DivisionDTO>> FindDivisionsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = DivisionSpecifications.DefaultSpec();

                ISpecification<Division> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _divisionRepository.AllMatchingPagedAsync<DivisionDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<DivisionDTO>> FindDivisionsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? DivisionSpecifications.DefaultSpec() : DivisionSpecifications.DivisionFullText(text);

                ISpecification<Division> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _divisionRepository.AllMatchingPagedAsync<DivisionDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<DivisionDTO> FindDivisionAsync(Guid divisionId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _divisionRepository.GetAsync<DivisionDTO>(divisionId, serviceHeader);
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

        public async Task<List<ZoneDTO>> FindZonesByDivisionIdAsync(Guid divisionId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ZoneSpecifications.ZoneWithDivisionId(divisionId);

                ISpecification<Zone> spec = filter;

                return await _zoneRepository.AllMatchingAsync<ZoneDTO>(spec, serviceHeader);
            }
        }
    }
}
