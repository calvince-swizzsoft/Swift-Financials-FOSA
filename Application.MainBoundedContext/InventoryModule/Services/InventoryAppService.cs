using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.UnderwritingModule.Services
{

    public class InventoryAppService : IInventoryAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Inventory> _inventoryRepository;

        public InventoryAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<Inventory> policyTypeRepository)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _inventoryRepository = policyTypeRepository ?? throw new ArgumentNullException(nameof(policyTypeRepository));
        }

        #region InventoryDTO

        public async Task<InventoryDTO> AddNewInventoryAsync(InventoryDTO inventoryDTO, ServiceHeader serviceHeader)
        {
            var inventoryBindingModel = inventoryDTO.ProjectedAs<InventoryBindingModel>();

            inventoryBindingModel.ValidateAll();

            if (inventoryBindingModel.HasErrors)
                throw new InvalidOperationException(string.Join(Environment.NewLine, inventoryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                inventoryDTO.Status = (int)InventoryRecordStatus.Active;

                var inventory = InventoryFactory.CreateInventory(
                    inventoryDTO.Code,
                    inventoryDTO.CompanyId,
                    inventoryDTO.CategoryId,
                    inventoryDTO.Description,
                    inventoryDTO.QuantityInStore,
                    inventoryDTO.Remarks,
                    inventoryDTO.UnitPrice,
                    inventoryDTO.UnitOfMeasure,
                    inventoryDTO.Status,
                    inventoryDTO.ExpiryDate,
                    inventoryDTO.Image
                );

                inventory.CreatedBy = serviceHeader.ApplicationUserName;

                if (inventoryDTO.IsLocked)
                    inventory.Lock();
                else
                    inventory.UnLock();

                _inventoryRepository.Add(inventory, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? inventory.ProjectedAs<InventoryDTO>() : null;
            }
        }



        public async Task<bool> UpdateInventoryAsync(InventoryDTO inventoryDTO, ServiceHeader serviceHeader)
        {
            var inventoryBindingModel = inventoryDTO.ProjectedAs<InventoryBindingModel>();

            inventoryBindingModel.ValidateAll();

            if (inventoryBindingModel.HasErrors)
                throw new InvalidOperationException(string.Join(Environment.NewLine, inventoryBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _inventoryRepository.GetAsync(inventoryDTO.Id, serviceHeader);

                if (persisted != null)
                {
                    if (inventoryDTO.QuantityInStore < 3)
                    {
                        inventoryDTO.Status = (int)InventoryRecordStatus.LowStock;
                    }

                    var current = InventoryFactory.CreateInventory(
                        inventoryDTO.Code,
                        inventoryDTO.CompanyId,
                        inventoryDTO.CategoryId,
                        inventoryDTO.Description,
                        inventoryDTO.QuantityInStore,
                        inventoryDTO.Remarks,
                        inventoryDTO.UnitPrice,
                        inventoryDTO.UnitOfMeasure,
                        inventoryDTO.Status,
                        inventoryDTO.ExpiryDate,
                        inventoryDTO.Image

                    );

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                    if (inventoryDTO.IsLocked)
                        current.Lock();
                    else
                        current.UnLock();

                    _inventoryRepository.Merge(persisted, current, serviceHeader);
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }


        public async Task<List<InventoryDTO>> FindInventoriesAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _inventoryRepository.GetAllAsync<InventoryDTO>(serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<InventoryDTO>> FindInventoriesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InventorySpecifications.DefaultSpec();

                ISpecification<Inventory> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _inventoryRepository.AllMatchingPagedAsync<InventoryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<InventoryDTO>> FindInventoriesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = InventorySpecifications.InventoryFullText(text);

                ISpecification<Inventory> spec = filter;

                var sortFields = new List<string> { "ClusterId" };

                return await _inventoryRepository.AllMatchingPagedAsync<InventoryDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public InventoryDTO FindInventory(Guid policyTypeId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    return _inventoryRepository.Get(policyTypeId, serviceHeader).ProjectedAs<InventoryDTO>();
                }
            }
        }

    
        Task<List<InventoryDTO>> IInventoryAppService.FindInventoriesAsync(ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        Task<PageCollectionInfo<InventoryDTO>> IInventoryAppService.FindInventoriesAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

     

      
        Task<PageCollectionInfo<InventoryDTO>> IInventoryAppService.FindInventoriesAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }

        InventoryDTO IInventoryAppService.FindInventory(Guid inventoryId, ServiceHeader serviceHeader)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
