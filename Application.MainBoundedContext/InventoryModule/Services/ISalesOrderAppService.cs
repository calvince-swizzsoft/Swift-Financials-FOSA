using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.UnderwritingModule.Services
{
    public interface ISalesOrderAppService
    {
        Task<SalesOrderDTO> AddNewSalesOrderAsync(SalesOrderDTO salesOrderDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateSalesOrderAsync(SalesOrderDTO salesOrderDTO, ServiceHeader serviceHeader);

        Task<List<SalesOrderDTO>> FindSalesOrdersAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<SalesOrderDTO>> FindSalesOrdersAsync(int itemType, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<SalesOrderDTO> FindSalesOrderAsync(Guid salesOrderId, ServiceHeader serviceHeader);

        Task<List<SalesOrderDTO>> FindSalesOrderAsync(string code, ServiceHeader serviceHeader);
        Task<List<SalesOrderEntryDTO>> FindSalesOrderEntriesBySalesOrderIdAsync(Guid salesOrderId, ServiceHeader serviceHeader);
        Task<SalesOrderEntryDTO> AddNewSalesOrderEntryAsync(SalesOrderEntryDTO salesOrderEntryDTO, ServiceHeader serviceHeader);
    }
}