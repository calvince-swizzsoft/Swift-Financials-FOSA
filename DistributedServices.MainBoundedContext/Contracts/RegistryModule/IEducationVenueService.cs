using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEducationVenueService
    {
        #region Education Venue

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EducationVenueDTO AddEducationVenue(EducationVenueDTO educationVenueDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEducationVenue(EducationVenueDTO educationVenueDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EducationVenueDTO> FindEducationVenues();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EducationVenueDTO> FindEducationVenuesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EducationVenueDTO FindEducationVenue(Guid educationVenueId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EducationVenueDTO> FindEducationVenuesByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
