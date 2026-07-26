using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IPaySlipService
    {
        #region Pay Slip

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PaySlipDTO> FindPaySlips();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PaySlipDTO> FindPaySlipsBySalaryPeriodId(Guid salaryPeriodId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodIdInPage(Guid salaryPeriodId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PaySlipDTO> FindPaySlipsBySalaryPeriodIdAndFilterInPage(Guid salaryPeriodId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PaySlipDTO> FindQueablePaySlipsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PaySlipDTO FindPaySlip(Guid paySlipId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PaySlipEntryDTO FindPaySlipEntry(Guid paySlipEntryId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PaySlipEntryDTO> FindPaySlipEntriesByPaySlipId(Guid paySlipId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<PaySlipDTO> FindLoanAppraisalPaySlipsByCustomerId(Guid customerId, Guid loanProductId);

        #endregion
    }
}
