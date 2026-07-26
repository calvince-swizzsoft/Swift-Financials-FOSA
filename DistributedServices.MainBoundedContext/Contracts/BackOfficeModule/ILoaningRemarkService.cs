using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILoaningRemarkService
    {
        #region Loaning Remark

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoaningRemarkDTO AddLoaningRemark(LoaningRemarkDTO loaningRemarkDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLoaningRemark(LoaningRemarkDTO loaningRemarkDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LoaningRemarkDTO> FindLoaningRemarks();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarksInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LoaningRemarkDTO FindLoaningRemark(Guid loaningRemarkId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LoaningRemarkDTO> FindLoaningRemarksByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
