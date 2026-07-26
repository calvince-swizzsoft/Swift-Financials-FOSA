using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IAccountClosureRequestService
    {
        #region  Account Closure Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AccountClosureRequestDTO AddAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ApproveAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureApprovalOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuditAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureAuditOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool SettleAccountClosureRequest(AccountClosureRequestDTO accountClosureRequestDTO, int accountClosureSettlementOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AccountClosureRequestDTO FindAccountClosureRequest(Guid accountClosureRequestId, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AccountClosureRequestDTO> FindAccountClosureRequests(bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequestsInPage(int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequestsByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<AccountClosureRequestDTO> FindAccountClosureRequestsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, bool includeProductDescription);

        #endregion
    }
}
