using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PackageTypeAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.InventoryModule.Services
{
    public class PackageTypeAppService : IPackageTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<PackageType> _packageTypeRepository;

        public PackageTypeAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<PackageType> packageTypeRepository)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _packageTypeRepository = packageTypeRepository ?? throw new ArgumentNullException(nameof(packageTypeRepository));
        }

        public async Task<PackageTypeDTO> AddNewPackageTypeAsync(PackageTypeDTO packageTypeDTO, ServiceHeader serviceHeader)
        {
            var packageTypeBindingModel = packageTypeDTO.ProjectedAs<PackageTypeBindingModel>();

            packageTypeBindingModel.ValidateAll();

            if (packageTypeBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, packageTypeBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var packageType = PackageTypeFactory.CreatePackageType(packageTypeDTO.Name, packageTypeDTO.Remarks);

                _packageTypeRepository.Add(packageType, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? packageType.ProjectedAs<PackageTypeDTO>() : null;
            }
        }

        public async Task<bool> UpdatePackageTypeAsync(PackageTypeDTO packageTypeDTO, ServiceHeader serviceHeader)
        {
            var packageTypeBindingModel = packageTypeDTO.ProjectedAs<PackageTypeBindingModel>();

            packageTypeBindingModel.ValidateAll();

            if (packageTypeBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, packageTypeBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _packageTypeRepository.GetAsync(packageTypeDTO.Id, serviceHeader);

                if (persisted == null) return false;

                var current = PackageTypeFactory.CreatePackageType(packageTypeDTO.Name, packageTypeDTO.Remarks);

                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                _packageTypeRepository.Merge(persisted, current, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
            }
        }

        public async Task<List<PackageTypeDTO>> FindPackageTypesAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _packageTypeRepository.GetAllAsync<PackageTypeDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<PackageTypeDTO>> FindPackageTypesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<PackageType> spec = PackageTypeSpecifications.DefaultSpec();

                var sortFields = new List<string> { "SequentialId" };

                return await _packageTypeRepository.AllMatchingPagedAsync<PackageTypeDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<PackageTypeDTO>> FindPackageTypesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<PackageType> spec = PackageTypeSpecifications.PackageTypeFullText(text);

                var sortFields = new List<string> { "SequentialId" };

                return await _packageTypeRepository.AllMatchingPagedAsync<PackageTypeDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PackageTypeDTO> FindPackageTypeAsync(Guid packageTypeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _packageTypeRepository.GetAsync<PackageTypeDTO>(packageTypeId, serviceHeader);
            }
        }
    }
}
