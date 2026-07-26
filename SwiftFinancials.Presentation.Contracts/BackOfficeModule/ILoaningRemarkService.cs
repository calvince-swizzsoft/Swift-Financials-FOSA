using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.BackOfficeModule
{
    [ServiceContract(Name = "ILoaningRemarkService")]
    public interface ILoaningRemarkService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoaningRemark(LoaningRemarkDTO loaningRemarkDTO, AsyncCallback callback, Object state);
        LoaningRemarkDTO EndAddLoaningRemark(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoaningRemark(LoaningRemarkDTO loaningRemarkDTO, AsyncCallback callback, Object state);
        bool EndUpdateLoaningRemark(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoaningRemarks(AsyncCallback callback, Object state);
        List<LoaningRemarkDTO> EndFindLoaningRemarks(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoaningRemarksInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoaningRemarkDTO> EndFindLoaningRemarksInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoaningRemark(Guid loaningRemarkId, AsyncCallback callback, Object state);
        LoaningRemarkDTO EndFindLoaningRemark(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoaningRemarksByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoaningRemarkDTO> EndFindLoaningRemarksByFilterInPage(IAsyncResult result);
    }
}
