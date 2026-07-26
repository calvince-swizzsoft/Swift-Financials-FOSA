using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IElectronicStatementOrderService")]
    public interface IElectronicStatementOrderService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrders(bool includeProductDescription, AsyncCallback callback, Object state);
        List<ElectronicStatementOrderDTO> EndFindElectronicStatementOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrdersInPage(int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<ElectronicStatementOrderDTO> EndFindElectronicStatementOrdersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrdersByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<ElectronicStatementOrderDTO> EndFindElectronicStatementOrdersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrderHistoryByElectronicStatementOrderIdInPage(Guid electronicStatementOrderId, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<ElectronicStatementOrderHistoryDTO> EndFindElectronicStatementOrderHistoryByElectronicStatementOrderIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrder(Guid electronicStatementOrderId, AsyncCallback callback, Object state);
        ElectronicStatementOrderDTO EndFindElectronicStatementOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrderHistory(Guid electronicStatementOrderHistoryId, AsyncCallback callback, Object state);
        ElectronicStatementOrderHistoryDTO EndFindElectronicStatementOrderHistory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO, AsyncCallback callback, Object state);
        ElectronicStatementOrderDTO EndAddElectronicStatementOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO, AsyncCallback callback, Object state);
        bool EndUpdateElectronicStatementOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrdersByCustomerAccountId(Guid customerAccountId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<ElectronicStatementOrderDTO> EndFindElectronicStatementOrdersByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindElectronicStatementOrdersByCustomerId(Guid customerId, int customerAccountTypeProductCode, bool includeProductDescription, AsyncCallback callback, Object state);
        List<ElectronicStatementOrderDTO> EndFindElectronicStatementOrdersByCustomerId(IAsyncResult result);
        
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFixSkippedElectronicStatementOrders(DateTime targetDate, AsyncCallback callback, Object state);
        bool EndFixSkippedElectronicStatementOrders(IAsyncResult result);
    }
}
