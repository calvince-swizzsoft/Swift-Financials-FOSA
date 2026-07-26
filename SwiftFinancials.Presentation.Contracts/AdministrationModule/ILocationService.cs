using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AdministrationModule
{
    [ServiceContract(Name = "ILocationService")]
    public interface ILocationService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLocation(LocationDTO locationDTO, AsyncCallback callback, Object state);
        LocationDTO EndAddLocation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLocation(LocationDTO locationDTO, AsyncCallback callback, Object state);
        bool EndUpdateLocation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLocations(AsyncCallback callback, Object state);
        List<LocationDTO> EndFindLocations(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLocationsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LocationDTO> EndFindLocationsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLocationsByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LocationDTO> EndFindLocationsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLocation(Guid locationId, AsyncCallback callback, Object state);
        LocationDTO EndFindLocation(IAsyncResult result);
    }
}
