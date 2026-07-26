using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface IConditionalLendingAppService
    {
        Task<ConditionalLendingDTO> AddNewConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateConditionalLendingAsync(ConditionalLendingDTO conditionalLendingDTO, ServiceHeader serviceHeader);

        Task<ConditionalLendingEntryDTO> AddNewConditionalLendingEntryAsync(ConditionalLendingEntryDTO conditionalLendingEntryDTO, ServiceHeader serviceHeader);

        Task<bool> RemoveConditionalLendingEntriesAsync(List<ConditionalLendingEntryDTO> conditionalLendingEntryDTOs, ServiceHeader serviceHeader);

        Task<bool> UpdateConditionalLendingEntryCollectionAsync(Guid conditionalLendingId, List<ConditionalLendingEntryDTO> conditionalLendingEntryCollection, ServiceHeader serviceHeader);

        Task<List<ConditionalLendingDTO>> FindConditionalLendingsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<ConditionalLendingDTO>> FindConditionalLendingsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<ConditionalLendingDTO> FindConditionalLendingAsync(Guid conditionalLendingId, ServiceHeader serviceHeader);

        Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdAsync(Guid conditionalLendingId, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByConditionalLendingIdAsync(Guid conditionalLendingId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<List<ConditionalLendingEntryDTO>> FindConditionalLendingEntriesByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader);

        Task<bool> FetchCustomerConditionalLendingStatusAsync(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);
    }
}
