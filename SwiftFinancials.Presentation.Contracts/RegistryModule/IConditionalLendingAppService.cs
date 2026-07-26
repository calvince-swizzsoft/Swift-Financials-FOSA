using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IConditionalLendingService")]
    public interface IConditionalLendingService
    {
        #region Commission Exemption

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddConditionalLending(ConditionalLendingDTO conditionalLendingDTO, AsyncCallback callback, Object state);
        ConditionalLendingDTO EndAddConditionalLending(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateConditionalLending(ConditionalLendingDTO conditionalLendingDTO, AsyncCallback callback, Object state);
        bool EndUpdateConditionalLending(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddConditionalLendingEntry(ConditionalLendingEntryDTO conditionalLendingEntryDTO, AsyncCallback callback, Object state);
        ConditionalLendingEntryDTO EndAddConditionalLendingEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveConditionalLendingEntries(List<ConditionalLendingEntryDTO> conditionalLendingEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveConditionalLendingEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateConditionalLendingEntryCollection(Guid conditionalLendingId, List<ConditionalLendingEntryDTO> conditionalLendingEntryCollection, AsyncCallback callback, Object state);
        bool EndUpdateConditionalLendingEntryCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindConditionalLendings(AsyncCallback callback, Object state);
        List<ConditionalLendingDTO> EndFindConditionalLendings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindConditionalLendingsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ConditionalLendingDTO> EndFindConditionalLendingsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindConditionalLending(Guid conditionalLendingId, AsyncCallback callback, Object state);
        ConditionalLendingDTO EndFindConditionalLending(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindConditionalLendingEntriesByConditionalLendingId(Guid conditionalLendingId, AsyncCallback callback, Object state);
        List<ConditionalLendingEntryDTO> EndFindConditionalLendingEntriesByConditionalLendingId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindConditionalLendingEntriesByConditionalLendingIdInPage(Guid conditionalLendingId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ConditionalLendingEntryDTO> EndFindConditionalLendingEntriesByConditionalLendingIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindConditionalLendingEntriesByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<ConditionalLendingEntryDTO> EndFindConditionalLendingEntriesByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFetchCustomerConditionalLendingStatus(Guid customerId, Guid loanProductId, AsyncCallback callback, Object state);
        bool EndFetchCustomerConditionalLendingStatus(IAsyncResult result);

        #endregion
    }
}
