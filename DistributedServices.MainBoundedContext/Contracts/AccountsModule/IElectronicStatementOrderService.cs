using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IElectronicStatementOrderService
    {
        #region Electronic Statement Order

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ElectronicStatementOrderDTO> FindElectronicStatementOrders(bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrdersInPage(int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription);
        
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ElectronicStatementOrderHistoryDTO> FindElectronicStatementOrderHistoryByElectronicStatementOrderIdInPage(Guid electronicStatementOrderId, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ElectronicStatementOrderDTO FindElectronicStatementOrder(Guid electronicStatementOrderId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ElectronicStatementOrderHistoryDTO FindElectronicStatementOrderHistory(Guid electronicStatementOrderHistoryId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ElectronicStatementOrderDTO AddElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateElectronicStatementOrder(ElectronicStatementOrderDTO electronicStatementOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool FixSkippedElectronicStatementOrders(DateTime targetDate);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerAccountId(Guid customerAccountId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ElectronicStatementOrderDTO> FindElectronicStatementOrdersByCustomerId(Guid customerId, int customerAccountTypeProductCode, bool includeProductDescription);
        
        #endregion
    }
}
