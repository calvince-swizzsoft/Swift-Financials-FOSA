using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IEmployeeAppraisalTargetService")]
    public interface IEmployeeAppraisalTargetService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, AsyncCallback callback, Object state);
        EmployeeAppraisalTargetDTO EndAddEmployeeAppraisalTarget(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeAppraisalTarget(EmployeeAppraisalTargetDTO employeeAppraisalTargetDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployeeAppraisalTarget(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalTargetsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeAppraisalTargetDTO> EndFindEmployeeAppraisalTargetsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChildEmployeeAppraisalTargetsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeAppraisalTargetDTO> EndFindChildEmployeeAppraisalTargetsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalTargets(bool updateDepth, bool traverseTree, AsyncCallback callback, Object state);
        List<EmployeeAppraisalTargetDTO> EndFindEmployeeAppraisalTargets(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalTargetsByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeAppraisalTargetDTO> EndFindEmployeeAppraisalTargetsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalTarget(Guid employeeAppraisalTargetId, AsyncCallback callback, Object state);
        EmployeeAppraisalTargetDTO EndFindEmployeeAppraisalTarget(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindChildEmployeeAppraisalTargets(AsyncCallback callback, Object state);
        List<EmployeeAppraisalTargetDTO> EndFindChildEmployeeAppraisalTargets(IAsyncResult result);
    }
}
