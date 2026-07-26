using AutoMapper;
using Domain.MainBoundedContext.InventoryModule.Aggregates.ReceiptAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg;
using Application.MainBoundedContext.DTO;
using Domain.MainBoundedContext.InventoryModule.Aggregates.SalesOrderEntryAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderEntryAgg;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class InventoryModuleProfile : Profile
    {
        public InventoryModuleProfile()
        {
            //Inventory => InventoryDTO
            CreateMap<Inventory, InventoryDTO>();

            //Customer => CustomerDTO

            //InventoryDTO => InventoryBindingModel
            CreateMap<InventoryDTO, InventoryBindingModel>();

            //Category => CategoryDTO
            CreateMap<Category, CategoryDTO>();

            //CategoryDTO => CategoryBindingModel
            CreateMap<CategoryDTO, CategoryBindingModel>();

            //PurchaseOrder => PurchaseOrderDTO
            CreateMap<PurchaseOrder, PurchaseOrderDTO>();

            //PurchaseOrderDTO => PurchaseOrderBindingModel
            CreateMap<PurchaseOrderDTO, PurchaseOrderBindingModel>();

            //SalesOrder => SalesOrderDTO
            CreateMap<SalesOrder, SalesOrderDTO>();

            //SalesOrderDTO => SalesOrderBindingModel
            CreateMap<SalesOrderDTO, SalesOrderBindingModel>();

            //SalesOrderEntryDTO => SalesOrderEntryBindingModel
            CreateMap<SalesOrderEntryDTO, SalesOrderEntryBindingModel>();

            //PurchaseOrderEntryDTO => PurchaseOrderEntryBindingModel
            CreateMap<PurchaseOrderEntryDTO, PurchaseOrderEntryBindingModel>();

            //SalesOrderEntry => SalesOrderEntryDTO
            CreateMap<SalesOrderEntry, SalesOrderEntryDTO>();

            //PurchaseOrderEntry => PurchaseOrderEntryDTO
            CreateMap<PurchaseOrderEntry, PurchaseOrderEntryDTO>();


        }
    }
}