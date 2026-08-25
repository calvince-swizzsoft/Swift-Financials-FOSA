using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.InventoryModule.Services
{
    public interface ISupplierAppService
    {
        Task<SupplierDTO> AddNewSupplierAsync(SupplierDTO supplierDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateSupplierAsync(SupplierDTO supplierDTO, ServiceHeader serviceHeader);

        Task<List<SupplierDTO>> FindSuppliersAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<SupplierDTO>> FindSuppliersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<SupplierDTO>> FindSuppliersAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<SupplierDTO> FindSupplierAsync(Guid supplierId, ServiceHeader serviceHeader);
    }
}
