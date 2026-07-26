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
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerAppService _employerAppService;

        public EmployerService(
            IEmployerAppService employerAppService)
        {
            Guard.ArgumentNotNull(employerAppService, nameof(employerAppService));

            _employerAppService = employerAppService;
        }

        #region Employer

        public async Task<EmployerDTO> AddEmployerAsync(EmployerDTO employerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.AddNewEmployerAsync(employerDTO, serviceHeader);
        }

        public async Task<bool> UpdateEmployerAsync(EmployerDTO employerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.UpdateEmployerAsync(employerDTO, serviceHeader);
        }

        public async Task<List<EmployerDTO>> FindEmployersAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindEmployersAsync(serviceHeader);
        }

        public async Task<EmployerDTO> FindEmployerAsync(Guid employerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindEmployerAsync(employerId, serviceHeader);
        }

        public async Task<DivisionDTO> FindDivisionAsync(Guid divisionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindDivisionAsync(divisionId, serviceHeader);
        }

        public async Task<PageCollectionInfo<EmployerDTO>> FindEmployersInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindEmployersAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<EmployerDTO>> FindEmployersByFilterInPageAsync(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindEmployersAsync(text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<List<DivisionDTO>> FindDivisionsAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindDivisionsAsync(serviceHeader);
        }

        public async Task<List<DivisionDTO>> FindDivisionsByEmployerIdAsync(Guid employerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindDivisionsByEmployerIdAsync(employerId, serviceHeader);
        }

        public async Task<List<ZoneDTO>> FindZonesByEmployerIdAsync(Guid employerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.FindZonesAsync(employerId, serviceHeader);
        }

        public async Task<bool> UpdateDivisionsByEmployerIdAsync(Guid employerId, List<DivisionDTO> divisions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _employerAppService.UpdateDivisionsAsync(employerId, divisions, serviceHeader);
        }

        #endregion
    }
}
