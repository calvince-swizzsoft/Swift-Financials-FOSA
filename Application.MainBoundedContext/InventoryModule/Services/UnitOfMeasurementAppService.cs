using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Domain.MainBoundedContext.InventoryModule.Aggregates.UnitOfMeasurementAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.InventoryModule.Services
{
    public class UnitOfMeasurementAppService : IUnitOfMeasurementAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<UnitOfMeasurement> _unitOfMeasurementRepository;

        public UnitOfMeasurementAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<UnitOfMeasurement> unitOfMeasurementRepository)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _unitOfMeasurementRepository = unitOfMeasurementRepository ?? throw new ArgumentNullException(nameof(unitOfMeasurementRepository));
        }

        public async Task<UnitOfMeasurementDTO> AddNewUnitOfMeasurementAsync(UnitOfMeasurementDTO unitOfMeasurementDTO, ServiceHeader serviceHeader)
        {
            var bindingModel = unitOfMeasurementDTO.ProjectedAs<UnitOfMeasurementBindingModel>();

            bindingModel.ValidateAll();

            if (bindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, bindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var unitOfMeasurement = UnitOfMeasurementFactory.CreateUnitOfMeasurement(unitOfMeasurementDTO.Name, unitOfMeasurementDTO.Contains, unitOfMeasurementDTO.BaseUnitId);

                _unitOfMeasurementRepository.Add(unitOfMeasurement, serviceHeader);

                if (await dbContextScope.SaveChangesAsync(serviceHeader) <= 0) return null;

                return await FindUnitOfMeasurementAsync(unitOfMeasurement.Id, serviceHeader);
            }
        }

        public async Task<bool> UpdateUnitOfMeasurementAsync(UnitOfMeasurementDTO unitOfMeasurementDTO, ServiceHeader serviceHeader)
        {
            var bindingModel = unitOfMeasurementDTO.ProjectedAs<UnitOfMeasurementBindingModel>();

            bindingModel.ValidateAll();

            if (bindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, bindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _unitOfMeasurementRepository.GetAsync(unitOfMeasurementDTO.Id, serviceHeader);

                if (persisted == null) return false;

                var current = UnitOfMeasurementFactory.CreateUnitOfMeasurement(unitOfMeasurementDTO.Name, unitOfMeasurementDTO.Contains, unitOfMeasurementDTO.BaseUnitId);

                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                _unitOfMeasurementRepository.Merge(persisted, current, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
            }
        }

        public async Task<List<UnitOfMeasurementDTO>> FindUnitOfMeasurementsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var units = await _unitOfMeasurementRepository.GetAllAsync<UnitOfMeasurementDTO>(serviceHeader);

                return ResolveBaseUnitNames(units, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<UnitOfMeasurementDTO>> FindUnitOfMeasurementsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<UnitOfMeasurement> spec = UnitOfMeasurementSpecifications.DefaultSpec();

                var sortFields = new List<string> { "SequentialId" };

                var page = await _unitOfMeasurementRepository.AllMatchingPagedAsync<UnitOfMeasurementDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (page != null) page.PageCollection = ResolveBaseUnitNames(page.PageCollection, serviceHeader);

                return page;
            }
        }

        public async Task<PageCollectionInfo<UnitOfMeasurementDTO>> FindUnitOfMeasurementsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<UnitOfMeasurement> spec = UnitOfMeasurementSpecifications.UnitOfMeasurementFullText(text);

                var sortFields = new List<string> { "SequentialId" };

                var page = await _unitOfMeasurementRepository.AllMatchingPagedAsync<UnitOfMeasurementDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (page != null) page.PageCollection = ResolveBaseUnitNames(page.PageCollection, serviceHeader);

                return page;
            }
        }

        public async Task<UnitOfMeasurementDTO> FindUnitOfMeasurementAsync(Guid unitOfMeasurementId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var unit = await _unitOfMeasurementRepository.GetAsync<UnitOfMeasurementDTO>(unitOfMeasurementId, serviceHeader);

                if (unit == null) return null;

                if (unit.BaseUnitId.HasValue)
                {
                    var baseUnit = await _unitOfMeasurementRepository.GetAsync(unit.BaseUnitId.Value, serviceHeader);

                    unit.BaseUnitName = baseUnit?.Name;
                }

                return unit;
            }
        }

        // Resolves each row's base-unit name individually — list sizes here are small
        // (unit-of-measure catalogues), and this avoids depending on EF navigation-property
        // eager-loading behavior in the generic repository's projection methods.
        private List<UnitOfMeasurementDTO> ResolveBaseUnitNames(List<UnitOfMeasurementDTO> units, ServiceHeader serviceHeader)
        {
            if (units == null || !units.Any()) return units;

            var namesById = new Dictionary<Guid, string>();

            foreach (var unit in units)
            {
                if (!unit.BaseUnitId.HasValue) continue;

                if (!namesById.TryGetValue(unit.BaseUnitId.Value, out var name))
                {
                    name = _unitOfMeasurementRepository.Get(unit.BaseUnitId.Value, serviceHeader)?.Name;

                    namesById[unit.BaseUnitId.Value] = name;
                }

                unit.BaseUnitName = name;
            }

            return units;
        }
    }
}
