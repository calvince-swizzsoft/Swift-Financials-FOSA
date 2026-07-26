using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IConditionalLendingService
    {
        #region Conditional Lending

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<ConditionalLendingDTO> AddConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<ConditionalLendingEntryDTO> AddConditionalLendingEntryAsync(ConditionalLendingEntryDTO conditionalLendingEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> RemoveConditionalLendingEntriesAsync(List<ConditionalLendingEntryDTO> conditionalLendingEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateConditionalLendingEntryCollectionAsync(Guid conditionalLendingId, List<ConditionalLendingEntryDTO> conditionalLendingEntryCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<ConditionalLendingDTO>> FindConditionalLendingsAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<ConditionalLendingDTO>> FindConditionalLendingsByFilterInPageAsync(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<ConditionalLendingDTO> FindConditionalLendingAsync(Guid conditionalLendingId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdAsync(Guid conditionalLendingId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdInPageAsync(Guid conditionalLendingId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByCustomerIdAsync(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> FetchCustomerConditionalLendingStatusAsync(Guid customerId, Guid loanProductId);

        #endregion
    }
}
