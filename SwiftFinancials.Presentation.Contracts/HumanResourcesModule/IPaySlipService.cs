using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IPaySlipService")]
    public interface IPaySlipService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaySlips(AsyncCallback callback, Object state);
        List<PaySlipDTO> EndFindPaySlips(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaySlipsBySalaryPeriodId(Guid salaryPeriodId, AsyncCallback callback, Object state);
        List<PaySlipDTO> EndFindPaySlipsBySalaryPeriodId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaySlipsBySalaryPeriodIdInPage(Guid salaryPeriodId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PaySlipDTO> EndFindPaySlipsBySalaryPeriodIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaySlipsBySalaryPeriodIdAndFilterInPage(Guid salaryPeriodId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PaySlipDTO> EndFindPaySlipsBySalaryPeriodIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindQueablePaySlipsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PaySlipDTO> EndFindQueablePaySlipsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaySlip(Guid paySlipId, AsyncCallback callback, Object state);
        PaySlipDTO EndFindPaySlip(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaySlipEntry(Guid paySlipEntryId, AsyncCallback callback, Object state);
        PaySlipEntryDTO EndFindPaySlipEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPaySlipEntriesByPaySlipId(Guid paySlipId, AsyncCallback callback, Object state);
        List<PaySlipEntryDTO> EndFindPaySlipEntriesByPaySlipId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLoanAppraisalPaySlipsByCustomerId(Guid customerId, Guid loanProductId, AsyncCallback callback, Object state);
        List<PaySlipDTO> EndFindLoanAppraisalPaySlipsByCustomerId(IAsyncResult result);
    }
}
