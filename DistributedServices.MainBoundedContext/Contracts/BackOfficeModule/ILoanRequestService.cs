using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILoanRequestService
    {
        #region Loan Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanRequestDTO AddLoanRequest(LoanRequestDTO loanRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveLoanRequest(Guid loanRequestId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CancelLoanRequest(LoanRequestDTO loanRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RegisterLoanRequest(LoanRequestDTO loanRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanRequestDTO FindLoanRequest(Guid loanRequestId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanRequestDTO> FindLoanRequestsByFilterInPage(DateTime startDate, DateTime endDate, string text, int loanRequestFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanRequestDTO> FindLoanRequestsByStatusAndFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int loanRequestFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanRequestDTO> FindLoanRequestsByCustomerIdInProcess(Guid customerId);

        #endregion
    }
}
