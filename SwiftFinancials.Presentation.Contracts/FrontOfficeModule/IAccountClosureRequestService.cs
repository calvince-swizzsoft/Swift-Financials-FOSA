using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "IAccountClosureRequestService")]
    public interface IAccountClosureRequestService
    {
        #region  Account Closure Request

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, AsyncCallback callback, Object state);
        AccountClosureRequestDTO EndAddAccountClosureRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginApproveAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureApprovalOption, AsyncCallback callback, Object state);
        bool EndApproveAccountClosureRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureAuditOption, AsyncCallback callback, Object state);
        bool EndAuditAccountClosureRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginSettleAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureSettlementOption, AsyncCallback callback, Object state);
        bool EndSettleAccountClosureRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAccountClosureRequest(Guid accountClosureRequestId, bool includeProductDescription, AsyncCallback callback, Object state);
        AccountClosureRequestDTO EndFindAccountClosureRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAccountClosureRequests(bool includeProductDescription, AsyncCallback callback, Object state);
        List<AccountClosureRequestDTO> EndFindAccountClosureRequests(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAccountClosureRequestsInPage(int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<AccountClosureRequestDTO> EndFindAccountClosureRequestsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAccountClosureRequestsByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<AccountClosureRequestDTO> EndFindAccountClosureRequestsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAccountClosureRequestsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<AccountClosureRequestDTO> EndFindAccountClosureRequestsByStatusAndFilterInPage(IAsyncResult result);




        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAccountclosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, AsyncCallback callback, Object state);
        bool EndUpdateAccountclosureRequest(IAsyncResult result);
        #endregion
    }
}
