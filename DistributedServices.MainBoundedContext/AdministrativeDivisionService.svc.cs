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
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AdministrativeDivisionService : IAdministrativeDivisionService
    {
        private readonly IAdministrativeDivisionAppService _administrativeDivisionAppService;

        public AdministrativeDivisionService(IAdministrativeDivisionAppService administrativeDivisionAppService)
        {
            Guard.ArgumentNotNull(administrativeDivisionAppService, nameof(administrativeDivisionAppService));

            _administrativeDivisionAppService = administrativeDivisionAppService;
        }
        
        public async Task<AdministrativeDivisionDTO> AddAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _administrativeDivisionAppService.AddNewAdministrativeDivisionAsync(administrativeDivisionDTO, serviceHeader);
        }

        public async Task<bool> UpdateAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _administrativeDivisionAppService.UpdateAdministrativeDivisionAsync(administrativeDivisionDTO, serviceHeader);
        }

        public List<AdministrativeDivisionDTO> FindAdministrativeDivisions(bool updateDepth, bool traverseTree)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _administrativeDivisionAppService.FindAdministrativeDivisions(serviceHeader, updateDepth, traverseTree);
        }
         
        public async Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _administrativeDivisionAppService.FindAdministrativeDivisionsAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsByFilterInPageAsync(string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _administrativeDivisionAppService.FindAdministrativeDivisionsAsync(filter, pageIndex, pageSize, serviceHeader);
        }

        public AdministrativeDivisionDTO FindAdministrativeDivision(Guid administrativeDivisionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _administrativeDivisionAppService.FindAdministrativeDivision(administrativeDivisionId, serviceHeader);
        }
    }
}
