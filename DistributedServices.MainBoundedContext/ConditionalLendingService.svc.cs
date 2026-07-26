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
    public class ConditionalLendingService : IConditionalLendingService
    {
        private readonly IConditionalLendingAppService _conditionalLendingAppService;

        public ConditionalLendingService(
            IConditionalLendingAppService conditionalLendingAppService)
        {
            Guard.ArgumentNotNull(conditionalLendingAppService, nameof(conditionalLendingAppService));

            _conditionalLendingAppService = conditionalLendingAppService;
        }

        #region Conditional Lending

        public async Task<ConditionalLendingDTO> AddConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.AddNewConditionalLendingAsync(conditionalLendingDTO, serviceHeader);
        }

        public async Task<bool> UpdateConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.UpdateConditionalLendingAsync(conditionalLendingDTO, serviceHeader);
        }

        public async Task<ConditionalLendingEntryDTO> AddConditionalLendingEntryAsync(ConditionalLendingEntryDTO conditionalLendingEntryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.AddNewConditionalLendingEntryAsync(conditionalLendingEntryDTO, serviceHeader);
        }

        public async Task<bool> RemoveConditionalLendingEntriesAsync(List<ConditionalLendingEntryDTO> conditionalLendingEntryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.RemoveConditionalLendingEntriesAsync(conditionalLendingEntryDTOs, serviceHeader);
        }

        public async Task<bool> UpdateConditionalLendingEntryCollectionAsync(Guid conditionalLendingId, List<ConditionalLendingEntryDTO> conditionalLendingEntryCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.UpdateConditionalLendingEntryCollectionAsync(conditionalLendingId, conditionalLendingEntryCollection, serviceHeader);
        }

        public async Task<List<ConditionalLendingDTO>> FindConditionalLendingsAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.FindConditionalLendingsAsync(serviceHeader);
        }

        public async Task<PageCollectionInfo<ConditionalLendingDTO>> FindConditionalLendingsByFilterInPageAsync(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.FindConditionalLendingsAsync(text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<ConditionalLendingDTO> FindConditionalLendingAsync(Guid conditionalLendingId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.FindConditionalLendingAsync(conditionalLendingId, serviceHeader);
        }

        public async Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdAsync(Guid conditionalLendingId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.FindConditionalLendingEntriesByConditionalLendingIdAsync(conditionalLendingId, serviceHeader);
        }

        public async Task<PageCollectionInfo<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdInPageAsync(Guid conditionalLendingId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.FindConditionalLendingEntriesByConditionalLendingIdAsync(conditionalLendingId, text, pageIndex, pageSize, serviceHeader);
        }

        public async Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByCustomerIdAsync(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.FindConditionalLendingEntriesByCustomerIdAsync(customerId, serviceHeader);
        }

        public async Task<bool> FetchCustomerConditionalLendingStatusAsync(Guid customerId, Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _conditionalLendingAppService.FetchCustomerConditionalLendingStatusAsync(customerId, loanProductId, serviceHeader);
        }

        #endregion
    }
}
