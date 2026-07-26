using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface ILocationAppService
    {
        LocationDTO AddNewLocation(LocationDTO locationDTO, ServiceHeader serviceHeader);

        bool UpdateLocation(LocationDTO locationDTO, ServiceHeader serviceHeader);

        List<LocationDTO> FindLocations(ServiceHeader serviceHeader);

        PageCollectionInfo<LocationDTO> FindLocations(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<LocationDTO> FindLocations(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LocationDTO FindLocation(Guid locationId, ServiceHeader serviceHeader);
    }
}
