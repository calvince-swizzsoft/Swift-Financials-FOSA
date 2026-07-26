using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IInterAccountTransferBatchService")]
    public interface IInterAccountTransferBatchService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, AsyncCallback callback, Object state);
        InterAccountTransferBatchDTO EndAddInterAccountTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, AsyncCallback callback, Object state);
        bool EndUpdateInterAccountTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddInterAccountTransferBatchEntry(InterAccountTransferBatchEntryDTO interAccountTransferBatchEntryDTO, AsyncCallback callback, Object state);
        InterAccountTransferBatchEntryDTO EndAddInterAccountTransferBatchEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveInterAccountTransferBatchEntries(List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveInterAccountTransferBatchEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, AsyncCallback callback, Object state);
        bool EndAuditInterAccountTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeInterAccountTransferBatch(InterAccountTransferBatchDTO interAccountTransferBatchDTO, int batchAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuthorizeInterAccountTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateInterAccountTransferBatchEntryCollection(Guid interAccountTransferBatchId, List<InterAccountTransferBatchEntryDTO> interAccountTransferBatchEntryCollection, AsyncCallback callback, Object state);
        bool EndUpdateInterAccountTransferBatchEntryCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDynamicChargesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, List<DynamicChargeDTO> dynamicCharges, AsyncCallback callback, Object state);
        bool EndUpdateDynamicChargesByInterAccountTransferBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInterAccountTransferBatches(AsyncCallback callback, Object state);
        List<InterAccountTransferBatchDTO> EndFindInterAccountTransferBatches(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInterAccountTransferBatchesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InterAccountTransferBatchDTO> EndFindInterAccountTransferBatchesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInterAccountTransferBatchesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InterAccountTransferBatchDTO> EndFindInterAccountTransferBatchesByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInterAccountTransferBatchesByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InterAccountTransferBatchDTO> EndFindInterAccountTransferBatchesByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInterAccountTransferBatch(Guid interAccountTransferBatchId, AsyncCallback callback, Object state);
        InterAccountTransferBatchDTO EndFindInterAccountTransferBatch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, AsyncCallback callback, Object state);
        List<InterAccountTransferBatchEntryDTO> EndFindInterAccountTransferBatchEntriesByInterAccountTransferBatchId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInterAccountTransferBatchEntriesByInterAccountTransferBatchIdInPage(Guid interAccountTransferBatchId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InterAccountTransferBatchEntryDTO> EndFindInterAccountTransferBatchEntriesByInterAccountTransferBatchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicChargesByInterAccountTransferBatchId(Guid interAccountTransferBatchId, AsyncCallback callback, Object state);
        List<DynamicChargeDTO> EndFindDynamicChargesByInterAccountTransferBatchId(IAsyncResult result);
    }
}
