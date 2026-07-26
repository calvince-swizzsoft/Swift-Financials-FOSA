using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILoanPurposeService
    {
        #region Loan Purpose

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanPurposeDTO AddLoanPurpose(LoanPurposeDTO loanPurposeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoanPurpose(LoanPurposeDTO loanPurposeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoanPurposeDTO> FindLoanPurposes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoanPurposeDTO FindLoanPurpose(Guid loanPurposeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanPurposeDTO> FindLoanPurposesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoanPurposeDTO> FindLoanPurposesByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
