using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.InventoryModule.Services
{
    public interface IUnitOfMeasurementAppService
    {
        Task<UnitOfMeasurementDTO> AddNewUnitOfMeasurementAsync(UnitOfMeasurementDTO unitOfMeasurementDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateUnitOfMeasurementAsync(UnitOfMeasurementDTO unitOfMeasurementDTO, ServiceHeader serviceHeader);

        Task<List<UnitOfMeasurementDTO>> FindUnitOfMeasurementsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<UnitOfMeasurementDTO>> FindUnitOfMeasurementsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<UnitOfMeasurementDTO>> FindUnitOfMeasurementsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<UnitOfMeasurementDTO> FindUnitOfMeasurementAsync(Guid unitOfMeasurementId, ServiceHeader serviceHeader);
    }
}
