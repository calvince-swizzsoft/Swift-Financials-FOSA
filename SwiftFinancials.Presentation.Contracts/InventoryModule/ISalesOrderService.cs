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
    [ServiceContract(Name = "ISalesOrderService")]
    public interface ISalesOrderService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSalesOrder(SalesOrderDTO salesOrderDTO, AsyncCallback callback, Object state);
        SalesOrderDTO EndAddSalesOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalesOrder(SalesOrderDTO salesOrderDTO, AsyncCallback callback, Object state);
        bool EndUpdateSalesOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesOrders(AsyncCallback callback, Object state);
        List<SalesOrderDTO> EndFindSalesOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesOrdersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SalesOrderDTO> EndFindSalesOrdersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesOrdersByFilterInPage(int itemType, string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SalesOrderDTO> EndFindSalesOrdersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesOrder(Guid salesOrderId, AsyncCallback callback, Object state);
        SalesOrderDTO EndFindSalesOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesOrderByCode(string code, AsyncCallback callback, Object state);
        List<SalesOrderDTO> EndFindSalesOrderByCode(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSalesOrderEntry(SalesOrderEntryDTO salesOrderEntryDTO, AsyncCallback callback, Object state);
        SalesOrderEntryDTO EndAddSalesOrderEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalesOrderEntriesBySalesOrderId(Guid salesOrderId, AsyncCallback callback, Object state);
        List<SalesOrderEntryDTO> EndFindSalesOrderEntriesBySalesOrderId(IAsyncResult result);
    }
}
