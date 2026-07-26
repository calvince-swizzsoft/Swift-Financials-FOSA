using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "IFixedDepositService")]
    public interface IFixedDepositService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginInvokeFixedDeposit(FixedDepositDTO fixedDepositDTO, AsyncCallback callback, Object state);
        FixedDepositDTO EndInvokeFixedDeposit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuditFixedDeposit(FixedDepositDTO fixedDepositDTO, int fixedDepositAuthOption, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAuditFixedDeposit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRevokeFixedDeposits(List<FixedDepositDTO> fixedDepositDTOs, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndRevokeFixedDeposits(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPayFixedDeposit(FixedDepositDTO fixedDepositDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPayFixedDeposit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDeposits(bool includeProductDescription, AsyncCallback callback, Object state);
        List<FixedDepositDTO> EndFindFixedDeposits(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositsByCustomerAccountId(Guid customerAccountId, bool includeProductDescription, AsyncCallback callback, Object state);
        List<FixedDepositDTO> EndFindFixedDepositsByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositsByBranchIdInPage(Guid branchId, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<FixedDepositDTO> EndFindFixedDepositsByBranchIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPayableFixedDepositsByFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<FixedDepositDTO> EndFindPayableFixedDepositsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindRevocableFixedDepositsByFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<FixedDepositDTO> EndFindRevocableFixedDepositsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositsByFilterInPage(string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<FixedDepositDTO> EndFindFixedDepositsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositsByStatusAndFilterInPage(int status, string text, int pageIndex, int pageSize, bool includeProductDescription, AsyncCallback callback, Object state);
        PageCollectionInfo<FixedDepositDTO> EndFindFixedDepositsByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDeposit(Guid fixedDepositId, AsyncCallback callback, Object state);
        FixedDepositDTO EndFindFixedDeposit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindFixedDepositPayablesByFixedDepositId(Guid fixedDepositId, AsyncCallback callback, Object state);
        List<FixedDepositPayableDTO> EndFindFixedDepositPayablesByFixedDepositId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateFixedDepositPayablesByFixedDepositId(Guid fixedDepositId, List<FixedDepositPayableDTO> fixedDepositPayables, AsyncCallback callback, Object state);
        bool EndUpdateFixedDepositPayablesByFixedDepositId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginExecutePayableFixedDeposits(DateTime targetDate, int pageSize, AsyncCallback callback, Object state);
        bool EndExecutePayableFixedDeposits(IAsyncResult result);
    }
}
