using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.BackOfficeModule
{
    [ServiceContract(Name = "ILoanRequestService")]
    public interface ILoanRequestService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanRequest(LoanRequestDTO loanRequestDTO, AsyncCallback callback, Object state);
        LoanRequestDTO EndAddLoanRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveLoanRequest(Guid loanRequestId, AsyncCallback callback, Object state);
        bool EndRemoveLoanRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCancelLoanRequest(LoanRequestDTO loanRequestDTO, AsyncCallback callback, Object state);
        bool EndCancelLoanRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRegisterLoanRequest(LoanRequestDTO loanRequestDTO, AsyncCallback callback, Object state);
        bool EndRegisterLoanRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanRequest(Guid loanRequestId, AsyncCallback callback, Object state);
        LoanRequestDTO EndFindLoanRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanRequestsByFilterInPage(DateTime startDate, DateTime endDate, string text, int loanRequestFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanRequestDTO> EndFindLoanRequestsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanRequestsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int loanRequestFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanRequestDTO> EndFindLoanRequestsByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanRequestsByCustomerIdInProcess(Guid customerId, AsyncCallback callback, Object state);
        List<LoanRequestDTO> EndFindLoanRequestsByCustomerIdInProcess(IAsyncResult result);
    }
}
