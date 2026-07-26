using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IEmployeeAppraisalService")]
    public interface IEmployeeAppraisalService
    {
        #region Employee Appraisal

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewEmployeeAppraisal(List<EmployeeAppraisalDTO> employeeAppraisalDTOs, AsyncCallback callback, Object state);
        bool EndAddNewEmployeeAppraisal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployeeAppraisal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAppraiseEmployeeAppraisal(EmployeeAppraisalDTO employeeAppraisalDTO, AsyncCallback callback, Object state);
        bool EndAppraiseEmployeeAppraisal(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalsByPeriodInPage(Guid employeeId, Guid employeeAppraisalPeriodId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeAppraisalDTO> EndFindEmployeeAppraisalsByPeriodInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalsByPeriod(Guid employeeId, Guid employeeAppraisalPeriodId, AsyncCallback callback, Object state);
        List<EmployeeAppraisalDTO> EndFindEmployeeAppraisalsByPeriod(IAsyncResult result);

        #endregion
    }
}
