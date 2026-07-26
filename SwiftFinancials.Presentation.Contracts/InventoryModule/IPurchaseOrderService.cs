using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.InventoryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System.ServiceModel;


namespace SwiftFinancials.Presentation.Contracts.InventoryModule
{
    [ServiceContract(Name = "IPurchaseOrderService")]
    public interface IPurchaseOrderService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddPurchaseOrder(PurchaseOrderDTO purchaseOrderDTO, AsyncCallback callback, Object state);
        PurchaseOrderDTO EndAddPurchaseOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdatePurchaseOrder(PurchaseOrderDTO purchaseOrderDTO, AsyncCallback callback, Object state);
        bool EndUpdatePurchaseOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPurchaseOrderInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PurchaseOrderDTO> EndFindPurchaseOrderInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPurchaseOrdersByFilterInPage(int itemType, string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PurchaseOrderDTO> EndFindPurchaseOrdersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPurchaseOrder(Guid policyId, AsyncCallback callback, Object state);
        PurchaseOrderDTO EndFindPurchaseOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPurchaseOrderByCode(string code, AsyncCallback callback, Object state);
        List<PurchaseOrderDTO> EndFindPurchaseOrderByCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPolicies( AsyncCallback callback, Object state);
        List<PurchaseOrderDTO> EndFindPolicies(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddPurchaseOrderEntry(PurchaseOrderEntryDTO purchaseOrderEntryDTO, AsyncCallback callback, Object state);
        PurchaseOrderEntryDTO EndAddPurchaseOrderEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPurchaseOrderEntriesBypurchaseOrderId(Guid policyId, AsyncCallback callback, Object state);
        List<PurchaseOrderEntryDTO> EndFindPurchaseOrderEntriesBypurchaseOrderId(IAsyncResult result);
        /*
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPurchaseOrderLevies(Guid policyId, AsyncCallback callback, Object state);
        List<PurchaseOrderLevyDTO> EndFindPurchaseOrderLevies(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdatePurchaseOrderLevies(Guid policyId, List<LevyDTO> levies, AsyncCallback callback, Object state);
        bool EndUpdatePurchaseOrderLevies(IAsyncResult result);
        */
    }
}
