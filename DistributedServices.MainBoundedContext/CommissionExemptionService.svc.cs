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
    public class CommissionExemptionService : ICommissionExemptionService
    {
        private readonly ICommissionExemptionAppService _commissionExemptionAppService;

        public CommissionExemptionService(
            ICommissionExemptionAppService commissionExemptionAppService)
        {
            Guard.ArgumentNotNull(commissionExemptionAppService, nameof(commissionExemptionAppService));

            _commissionExemptionAppService = commissionExemptionAppService;
        }

        #region Commission Exemption

        public async Task<CommissionExemptionDTO> AddCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.AddNewCommissionExemptionAsync(commissionExemptionDTO, serviceHeader);
        }

        public async Task<bool> UpdateCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.UpdateCommissionExemptionAsync(commissionExemptionDTO, serviceHeader);
        }

        public async Task<CommissionExemptionEntryDTO> AddCommissionExemptionEntryAsync(CommissionExemptionEntryDTO commissionExemptionEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.AddNewCommissionExemptionEntryAsync(commissionExemptionEntryDTO, serviceHeader);
        }

        public async Task<bool> RemoveCommissionExemptionEntriesAsync(List<CommissionExemptionEntryDTO> commissionExemptionEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.RemoveCommissionExemptionEntriesAsync(commissionExemptionEntryDTOs, serviceHeader);
        }

        public async Task<bool> UpdateCommissionExemptionEntryCollectionAsync(Guid commissionExemptionId, List<CommissionExemptionEntryDTO> commissionExemptionEntryCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.UpdateCommissionExemptionEntryCollectionAsync(commissionExemptionId, commissionExemptionEntryCollection, serviceHeader);
        }

        public async Task<List<CommissionExemptionDTO>> FindCommissionExemptionsAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.FindCommissionExemptionsAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<CommissionExemptionDTO>> FindCommissionExemptionsByFilterInPageAsync(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.FindCommissionExemptionsAsync(text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<CommissionExemptionDTO> FindCommissionExemptionAsync(Guid commissionExemptionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.FindCommissionExemptionAsync(commissionExemptionId, serviceHeader);
        }

        public async Task<List<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdAsync(Guid commissionExemptionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.FindCommissionExemptionEntriesByCommissionExemptionIdAsync(commissionExemptionId, serviceHeader);
        }

        public async Task<PageCollectionInfo<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdInPageAsync(Guid commissionExemptionId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.FindCommissionExemptionEntriesByCommissionExemptionIdAsync(commissionExemptionId, text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<List<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCustomerIdAsync(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _commissionExemptionAppService.FindCommissionExemptionEntriesByCustomerIdAsync(customerId, serviceHeader);
        }

        #endregion
    }
}
