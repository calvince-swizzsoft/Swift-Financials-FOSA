using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Domain.MainBoundedContext.InventoryModule.Aggregates.AssetTypeAgg;
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
    public class AssetTypeAppService : IAssetTypeAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<AssetType> _assetTypeRepository;

        public AssetTypeAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<AssetType> assetTypeRepository)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _assetTypeRepository = assetTypeRepository ?? throw new ArgumentNullException(nameof(assetTypeRepository));
        }

        public async Task<AssetTypeDTO> AddNewAssetTypeAsync(AssetTypeDTO assetTypeDTO, ServiceHeader serviceHeader)
        {
            var assetTypeBindingModel = assetTypeDTO.ProjectedAs<AssetTypeBindingModel>();

            assetTypeBindingModel.ValidateAll();

            if (assetTypeBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, assetTypeBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var assetType = AssetTypeFactory.CreateAssetType(assetTypeDTO.Name, assetTypeDTO.DepreciationMethod, assetTypeDTO.UsefulLife, assetTypeDTO.IsTangible);

                _assetTypeRepository.Add(assetType, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? assetType.ProjectedAs<AssetTypeDTO>() : null;
            }
        }

        public async Task<bool> UpdateAssetTypeAsync(AssetTypeDTO assetTypeDTO, ServiceHeader serviceHeader)
        {
            var assetTypeBindingModel = assetTypeDTO.ProjectedAs<AssetTypeBindingModel>();

            assetTypeBindingModel.ValidateAll();

            if (assetTypeBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, assetTypeBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _assetTypeRepository.GetAsync(assetTypeDTO.Id, serviceHeader);

                if (persisted == null) return false;

                var current = AssetTypeFactory.CreateAssetType(assetTypeDTO.Name, assetTypeDTO.DepreciationMethod, assetTypeDTO.UsefulLife, assetTypeDTO.IsTangible);

                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                _assetTypeRepository.Merge(persisted, current, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
            }
        }

        public async Task<List<AssetTypeDTO>> FindAssetTypesAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _assetTypeRepository.GetAllAsync<AssetTypeDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<AssetTypeDTO>> FindAssetTypesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<AssetType> spec = AssetTypeSpecifications.DefaultSpec();

                var sortFields = new List<string> { "SequentialId" };

                return await _assetTypeRepository.AllMatchingPagedAsync<AssetTypeDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<AssetTypeDTO>> FindAssetTypesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<AssetType> spec = AssetTypeSpecifications.AssetTypeFullText(text);

                var sortFields = new List<string> { "SequentialId" };

                return await _assetTypeRepository.AllMatchingPagedAsync<AssetTypeDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<AssetTypeDTO> FindAssetTypeAsync(Guid assetTypeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _assetTypeRepository.GetAsync<AssetTypeDTO>(assetTypeId, serviceHeader);
            }
        }
    }
}
