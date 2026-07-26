using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILocationService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LocationDTO AddLocation(LocationDTO locationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLocation(LocationDTO locationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LocationDTO> FindLocations();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LocationDTO> FindLocationsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LocationDTO> FindLocationsByFilterInPage(string filter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LocationDTO FindLocation(Guid locationId);
    }
}
