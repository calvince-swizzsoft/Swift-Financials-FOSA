using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IDynamicChargeService")]
    public interface IDynamicChargeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDynamicCharge(DynamicChargeDTO dynamicChargeDTO, AsyncCallback callback, Object state);
        DynamicChargeDTO EndAddDynamicCharge(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDynamicCharge(DynamicChargeDTO dynamicChargeDTO, AsyncCallback callback, Object state);
        bool EndUpdateDynamicCharge(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicCharges(AsyncCallback callback, Object state);
        List<DynamicChargeDTO> EndFindDynamicCharges(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicCharge(Guid dynamicChargeId, AsyncCallback callback, Object state);
        DynamicChargeDTO EndFindDynamicCharge(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicChargesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DynamicChargeDTO> EndFindDynamicChargesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByDynamicChargeId(Guid dynamicChargeId, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByDynamicChargeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByDynamicChargeId(Guid dynamicChargeId, List<CommissionDTO> commissions, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByDynamicChargeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDynamicChargesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DynamicChargeDTO> EndFindDynamicChargesByFilterInPage(IAsyncResult result);
    }
}
