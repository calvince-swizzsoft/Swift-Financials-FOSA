using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IEducationVenueService")]
    public interface IEducationVenueService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEducationVenue(EducationVenueDTO educationVenueDTO, AsyncCallback callback, Object state);
        EducationVenueDTO EndAddEducationVenue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEducationVenue(EducationVenueDTO educationVenueDTO, AsyncCallback callback, Object state);
        bool EndUpdateEducationVenue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationVenues(AsyncCallback callback, Object state);
        List<EducationVenueDTO> EndFindEducationVenues(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationVenuesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EducationVenueDTO> EndFindEducationVenuesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationVenue(Guid educationVenueId, AsyncCallback callback, Object state);
        EducationVenueDTO EndFindEducationVenue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEducationVenuesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EducationVenueDTO> EndFindEducationVenuesByFilterInPage(IAsyncResult result);
    }
}
