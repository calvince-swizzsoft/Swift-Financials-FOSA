using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.BackOfficeModule
{
    [ServiceContract(Name = "ILoanPurposeService")]
    public interface ILoanPurposeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLoanPurpose(LoanPurposeDTO loanPurposeDTO, AsyncCallback callback, Object state);
        LoanPurposeDTO EndAddLoanPurpose(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLoanPurpose(LoanPurposeDTO loanPurposeDTO, AsyncCallback callback, Object state);
        bool EndUpdateLoanPurpose(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanPurposes(AsyncCallback callback, Object state);
        List<LoanPurposeDTO> EndFindLoanPurposes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanPurpose(Guid loanPurposeId, AsyncCallback callback, Object state);
        LoanPurposeDTO EndFindLoanPurpose(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanPurposesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanPurposeDTO> EndFindLoanPurposesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanPurposesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LoanPurposeDTO> EndFindLoanPurposesByFilterInPage(IAsyncResult result);
    }
}
