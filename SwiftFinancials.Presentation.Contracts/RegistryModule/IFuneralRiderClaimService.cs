using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IFuneralRiderClaimService")]
    public interface IFuneralRiderClaimService
    {
        #region FuneralRiderClaimDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO, AsyncCallback callback, object state);
        FuneralRiderClaimDTO EndAddNewFuneralRiderClaim(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateFuneralRiderClaim(FuneralRiderClaimDTO funeralRiderClaimDTO, AsyncCallback callback, object state);
        bool EndUpdateFuneralRiderClaim(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimsByFilterAndDateInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FuneralRiderClaimDTO> EndFindFuneralRiderClaimsByFilterAndDateInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimsByStatusAndFilterInPage(int status, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FuneralRiderClaimDTO> EndFindFuneralRiderClaimsByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaims(AsyncCallback callback, Object state);
        List<FuneralRiderClaimDTO> EndFindFuneralRiderClaims(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaim(Guid funeralRiderClaimId, AsyncCallback callback, Object state);
        FuneralRiderClaimDTO EndFindFuneralRiderClaim(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimsByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<FuneralRiderClaimDTO> EndFindFuneralRiderClaimsByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimsByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FuneralRiderClaimDTO> EndFindFuneralRiderClaimsByFilterInPage(IAsyncResult result);

        #endregion

        #region FuneralRiderClaimPayableDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, AsyncCallback callback, object state);
        FuneralRiderClaimPayableDTO EndAddNewFuneralRiderClaimPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, AsyncCallback callback, object state);
        bool EndUpdateFuneralRiderClaimPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int verificationOption, AsyncCallback callback, object state);
        bool EndAuditFuneralRiderClaimPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeFuneralRiderClaimPayable(FuneralRiderClaimPayableDTO funeralRiderClaimPayableDTO, int authorizationOption, int moduleNavigationItemCode, AsyncCallback callback, object state);
        bool EndAuthorizeFuneralRiderClaimPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostFuneralRiderClaimPayable(Guid funeralRiderClaimPayableId, AsyncCallback callback, object state);
        bool EndPostFuneralRiderClaimPayable(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimPayablesByRecordStatusFilterAndDateInPage(int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FuneralRiderClaimPayableDTO> EndFindFuneralRiderClaimPayablesByRecordStatusFilterAndDateInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimPayablesByRecordStatusPaymentStatusFilterAndDateInPage(int recordStatus, int paymentStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FuneralRiderClaimPayableDTO> EndFindFuneralRiderClaimPayablesByRecordStatusPaymentStatusFilterAndDateInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimPayablesByRecordStatusPaymentStatusAndFilterInPage(int recordStatus, int paymentStatus, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<FuneralRiderClaimPayableDTO> EndFindFuneralRiderClaimPayablesByRecordStatusPaymentStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFuneralRiderClaimPayable(Guid funeralRiderClaimPayableId, AsyncCallback callback, Object state);
        FuneralRiderClaimPayableDTO EndFindFuneralRiderClaimPayable(IAsyncResult result);

        #endregion
    }
}
