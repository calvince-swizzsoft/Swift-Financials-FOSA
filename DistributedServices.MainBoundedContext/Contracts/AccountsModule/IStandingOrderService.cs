using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IStandingOrderService
    {
        #region Standing Order

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<StandingOrderDTO> FindStandingOrders(bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<StandingOrderDTO> FindStandingOrdersInPage(int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<StandingOrderDTO> FindStandingOrdersByFilterInPage(string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<StandingOrderDTO> FindStandingOrdersByTriggerAndFilterInPage(int trigger, string text, int customerAccountFilter, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<StandingOrderHistoryDTO> FindStandingOrderHistoryByStandingOrderIdInPage(Guid standingOrderId, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        StandingOrderDTO FindStandingOrder(Guid standingOrderId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        StandingOrderDTO AddStandingOrder(StandingOrderDTO standingOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateStandingOrder(StandingOrderDTO standingOrderDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<StandingOrderDTO> FindStandingOrdersByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<StandingOrderDTO> FindStandingOrdersByBenefactorCustomerId(Guid benefactorCustomerId, int benefactorCustomerAccountTypeProductCode, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AutoCreateStandindOrders(Guid benefactorProductId, int benefactorProductCode, Guid beneficiaryProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool FixSkippedStandingOrders(DateTime targetDate, int pageSize);

        #endregion
    }
}
