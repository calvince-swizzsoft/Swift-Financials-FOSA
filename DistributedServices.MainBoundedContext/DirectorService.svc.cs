using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorAppService _directorAppService;

        public DirectorService(
            IDirectorAppService directorAppService)
        {
            Guard.ArgumentNotNull(directorAppService, nameof(directorAppService));

            _directorAppService = directorAppService;
        }

        public DirectorDTO AddDirector(DirectorDTO directorDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directorAppService.AddNewDirector(directorDTO, serviceHeader);
        }

        public bool UpdateDirector(DirectorDTO directorDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directorAppService.UpdateDirector(directorDTO, serviceHeader);
        }

        public List<DirectorDTO> FindDirectors()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directorAppService.FindDirectors(serviceHeader);
        }

        public PageCollectionInfo<DirectorDTO> FindDirectorsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directorAppService.FindDirectors(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<DirectorDTO> FindDirectorsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directorAppService.FindDirectors(text, pageIndex, pageSize, serviceHeader);
        }

        public DirectorDTO FindDirector(Guid directorId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _directorAppService.FindDirector(directorId, serviceHeader);
        }
    }
}
