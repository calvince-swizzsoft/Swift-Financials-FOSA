using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Domain.MainBoundedContext.InventoryModule.Aggregates.SupplierAgg;
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
    public class SupplierAppService : ISupplierAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;

        public SupplierAppService(IDbContextScopeFactory dbContextScopeFactory, IRepository<Supplier> supplierRepository,
            IChartOfAccountAppService chartOfAccountAppService)
        {
            _dbContextScopeFactory = dbContextScopeFactory ?? throw new ArgumentNullException(nameof(dbContextScopeFactory));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            _chartOfAccountAppService = chartOfAccountAppService ?? throw new ArgumentNullException(nameof(chartOfAccountAppService));
        }

        public async Task<SupplierDTO> AddNewSupplierAsync(SupplierDTO supplierDTO, ServiceHeader serviceHeader)
        {
            var supplierBindingModel = supplierDTO.ProjectedAs<SupplierBindingModel>();

            supplierBindingModel.ValidateAll();

            if (supplierBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, supplierBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var supplier = SupplierFactory.CreateSupplier(supplierDTO.Name, supplierDTO.AddressLine1, supplierDTO.AddressLine2,
                    supplierDTO.Street, supplierDTO.PostalCode, supplierDTO.LandLine, supplierDTO.MobileLine, supplierDTO.Email,
                    supplierDTO.ChartOfAccountId);

                if (supplierDTO.IsLocked)
                    supplier.Lock();
                else supplier.UnLock();

                _supplierRepository.Add(supplier, serviceHeader);

                if (await dbContextScope.SaveChangesAsync(serviceHeader) <= 0) return null;

                return await FindSupplierAsync(supplier.Id, serviceHeader);
            }
        }

        public async Task<bool> UpdateSupplierAsync(SupplierDTO supplierDTO, ServiceHeader serviceHeader)
        {
            var supplierBindingModel = supplierDTO.ProjectedAs<SupplierBindingModel>();

            supplierBindingModel.ValidateAll();

            if (supplierBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, supplierBindingModel.ErrorMessages));

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _supplierRepository.GetAsync(supplierDTO.Id, serviceHeader);

                if (persisted == null) return false;

                var current = SupplierFactory.CreateSupplier(supplierDTO.Name, supplierDTO.AddressLine1, supplierDTO.AddressLine2,
                    supplierDTO.Street, supplierDTO.PostalCode, supplierDTO.LandLine, supplierDTO.MobileLine, supplierDTO.Email,
                    supplierDTO.ChartOfAccountId);

                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);

                if (supplierDTO.IsLocked)
                    current.Lock();
                else current.UnLock();

                _supplierRepository.Merge(persisted, current, serviceHeader);

                return await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
            }
        }

        public async Task<List<SupplierDTO>> FindSuppliersAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var suppliers = await _supplierRepository.GetAllAsync<SupplierDTO>(serviceHeader);

                return ResolveChartOfAccountNames(suppliers, serviceHeader);
            }
        }

        public async Task<PageCollectionInfo<SupplierDTO>> FindSuppliersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<Supplier> spec = SupplierSpecifications.DefaultSpec();

                var sortFields = new List<string> { "SequentialId" };

                var page = await _supplierRepository.AllMatchingPagedAsync<SupplierDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (page != null) page.PageCollection = ResolveChartOfAccountNames(page.PageCollection, serviceHeader);

                return page;
            }
        }

        public async Task<PageCollectionInfo<SupplierDTO>> FindSuppliersAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<Supplier> spec = SupplierSpecifications.SupplierFullText(text);

                var sortFields = new List<string> { "SequentialId" };

                var page = await _supplierRepository.AllMatchingPagedAsync<SupplierDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (page != null) page.PageCollection = ResolveChartOfAccountNames(page.PageCollection, serviceHeader);

                return page;
            }
        }

        public async Task<SupplierDTO> FindSupplierAsync(Guid supplierId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var supplier = await _supplierRepository.GetAsync<SupplierDTO>(supplierId, serviceHeader);

                if (supplier == null) return null;

                supplier.ChartOfAccountAccountName = _chartOfAccountAppService.FindChartOfAccount(supplier.ChartOfAccountId, serviceHeader)?.AccountName;

                return supplier;
            }
        }

        // Resolves each row's G/L account name individually rather than joining server-side —
        // list sizes here are small (supplier registers), and this keeps SupplierAppService
        // decoupled from ChartOfAccount's own EF mapping/AutoMapper flattening setup.
        private List<SupplierDTO> ResolveChartOfAccountNames(List<SupplierDTO> suppliers, ServiceHeader serviceHeader)
        {
            if (suppliers == null || !suppliers.Any()) return suppliers;

            var accountNamesById = new Dictionary<Guid, string>();

            foreach (var supplier in suppliers)
            {
                if (!accountNamesById.TryGetValue(supplier.ChartOfAccountId, out var accountName))
                {
                    accountName = _chartOfAccountAppService.FindChartOfAccount(supplier.ChartOfAccountId, serviceHeader)?.AccountName;

                    accountNamesById[supplier.ChartOfAccountId] = accountName;
                }

                supplier.ChartOfAccountAccountName = accountName;
            }

            return suppliers;
        }
    }
}
