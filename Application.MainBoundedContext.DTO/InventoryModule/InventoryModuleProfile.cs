using AutoMapper;
using Domain.MainBoundedContext.InventoryModule.Aggregates.ReceiptAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.CategoryAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PurchaseOrderAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.SupplierAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.AssetTypeAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.PackageTypeAgg;
using Domain.MainBoundedContext.InventoryModule.Aggregates.UnitOfMeasurementAgg;
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

            //Supplier => SupplierDTO
            // ChartOfAccountAccountName is set explicitly by SupplierAppService (via
            // IChartOfAccountAppService.FindChartOfAccount), not flattened from the
            // ChartOfAccount navigation property here.
            CreateMap<Supplier, SupplierDTO>()
                .ForMember(dest => dest.ChartOfAccountAccountName, opt => opt.Ignore());

            //SupplierDTO => SupplierBindingModel
            CreateMap<SupplierDTO, SupplierBindingModel>();

            //AssetType => AssetTypeDTO
            CreateMap<AssetType, AssetTypeDTO>();

            //AssetTypeDTO => AssetTypeBindingModel
            CreateMap<AssetTypeDTO, AssetTypeBindingModel>();

            //PackageType => PackageTypeDTO
            CreateMap<PackageType, PackageTypeDTO>();

            //PackageTypeDTO => PackageTypeBindingModel
            CreateMap<PackageTypeDTO, PackageTypeBindingModel>();

            //UnitOfMeasurement => UnitOfMeasurementDTO
            // BaseUnitName is set explicitly by UnitOfMeasurementAppService (self-lookup
            // against its own repository), not flattened from the BaseUnit navigation here.
            CreateMap<UnitOfMeasurement, UnitOfMeasurementDTO>()
                .ForMember(dest => dest.BaseUnitName, opt => opt.Ignore());

            //UnitOfMeasurementDTO => UnitOfMeasurementBindingModel
            CreateMap<UnitOfMeasurementDTO, UnitOfMeasurementBindingModel>();

        }
    }
}