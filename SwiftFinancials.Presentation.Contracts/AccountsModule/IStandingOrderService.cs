using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IStandingOrderService")]
    public interface IStandingOrderService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrders(bool includeProductDescription, AsyncCallback callback, Object state);
        List<StandingOrderDTO> EndFindStandingOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrdersInPage(int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<StandingOrderDTO> EndFindStandingOrdersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrdersByFilterInPage(string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<StandingOrderDTO> EndFindStandingOrdersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrdersByTriggerAndFilterInPage(int trigger, string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<StandingOrderDTO> EndFindStandingOrdersByTriggerAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrderHistoryByStandingOrderIdInPage(Guid standingOrderId, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<StandingOrderHistoryDTO> EndFindStandingOrderHistoryByStandingOrderIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrder(Guid standingOrderId, AsyncCallback callback, Object state);
        StandingOrderDTO EndFindStandingOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddStandingOrder(StandingOrderDTO standingOrderDTO, AsyncCallback callback, Object state);
        StandingOrderDTO EndAddStandingOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateStandingOrder(StandingOrderDTO standingOrderDTO, AsyncCallback callback, Object state);
        bool EndUpdateStandingOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<StandingOrderDTO> EndFindStandingOrdersByBenefactorCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<StandingOrderDTO> EndFindStandingOrdersByBeneficiaryCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindStandingOrdersByBenefactorCustomerId(Guid benefactorCustomerId, int benefactorCustomerAccountTypeProductCode, bool includeProductDescription, AsyncCallback callback, Object state);
        List<StandingOrderDTO> EndFindStandingOrdersByBenefactorCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAutoCreateStandindOrders(Guid benefactorProductId, int benefactorProductCode, Guid beneficiaryProductId, AsyncCallback callback, Object state);
        bool EndAutoCreateStandindOrders(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFixSkippedStandingOrders(DateTime targetDate, int pageSize, AsyncCallback callback, Object state);
        bool EndFixSkippedStandingOrders(IAsyncResult result);
    }
}
