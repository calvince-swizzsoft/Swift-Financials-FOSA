using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "IUnPayReasonService")]
    public interface IUnPayReasonService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddUnPayReason(UnPayReasonDTO unPayReasonDTO, AsyncCallback callback, Object state);
        UnPayReasonDTO EndAddUnPayReason(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateUnPayReason(UnPayReasonDTO unPayReasonDTO, AsyncCallback callback, Object state);
        bool EndUpdateUnPayReason(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnPayReasons(AsyncCallback callback, Object state);
        List<UnPayReasonDTO> EndFindUnPayReasons(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnPayReason(Guid unPayReasonId, AsyncCallback callback, Object state);
        UnPayReasonDTO EndFindUnPayReason(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnPayReasonsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<UnPayReasonDTO> EndFindUnPayReasonsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnPayReasonsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<UnPayReasonDTO> EndFindUnPayReasonsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCommissionsByUnPayReasonId(Guid unPayReasonId, AsyncCallback callback, Object state);
        List<CommissionDTO> EndFindCommissionsByUnPayReasonId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCommissionsByUnPayReasonId(Guid unPayReasonId, List<CommissionDTO> commissions, AsyncCallback callback, Object state);
        bool EndUpdateCommissionsByUnPayReasonId(IAsyncResult result);
    }
}
