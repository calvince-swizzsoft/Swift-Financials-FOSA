using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class EducationVenueService : IEducationVenueService
    {
        private readonly IEducationVenueAppService _educationVenueAppService;

        public EducationVenueService(
            IEducationVenueAppService educationVenueAppService)
        {
            Guard.ArgumentNotNull(educationVenueAppService, nameof(educationVenueAppService));

            _educationVenueAppService = educationVenueAppService;
        }

        #region EducationVenue

        public EducationVenueDTO AddEducationVenue(EducationVenueDTO educationVenueDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationVenueAppService.AddNewEducationVenue(educationVenueDTO, serviceHeader);
        }

        public bool UpdateEducationVenue(EducationVenueDTO educationVenueDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationVenueAppService.UpdateEducationVenue(educationVenueDTO, serviceHeader);
        }

        public List<EducationVenueDTO> FindEducationVenues()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationVenueAppService.FindEducationVenues(serviceHeader);
        }

        public PageCollectionInfo<EducationVenueDTO> FindEducationVenuesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationVenueAppService.FindEducationVenues(pageIndex, pageSize, serviceHeader);
        }

        public EducationVenueDTO FindEducationVenue(Guid educationVenueId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationVenueAppService.FindEducationVenue(educationVenueId, serviceHeader);
        }

        public PageCollectionInfo<EducationVenueDTO> FindEducationVenuesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _educationVenueAppService.FindEducationVenues(text, pageIndex, pageSize, serviceHeader);
        }

        #endregion
    }
}
