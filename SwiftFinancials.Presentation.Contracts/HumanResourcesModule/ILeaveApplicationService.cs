using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "ILeaveApplicationService")]
    public interface ILeaveApplicationService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, AsyncCallback callback, Object state);
        LeaveApplicationDTO EndAddLeaveApplication(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, AsyncCallback callback, Object state);
        bool EndUpdateLeaveApplication(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, AsyncCallback callback, Object state);
        bool EndAuthorizeLeaveApplication(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRecallLeaveApplication(LeaveApplicationDTO leaveApplicationDTO, AsyncCallback callback, Object state);
        bool EndRecallLeaveApplication(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveApplications(AsyncCallback callback, Object state);
        List<LeaveApplicationDTO> EndFindLeaveApplications(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveApplicationsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LeaveApplicationDTO> EndFindLeaveApplicationsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveApplicationsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LeaveApplicationDTO> EndFindLeaveApplicationsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveApplicationsByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<LeaveApplicationDTO> EndFindLeaveApplicationsByStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveApplication(Guid leaveApplicationId, AsyncCallback callback, Object state);
        LeaveApplicationDTO EndFindLeaveApplication(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveApplicationsByEmployeeId(Guid employeeId, AsyncCallback callback, Object state);
        List<LeaveApplicationDTO> EndFindLeaveApplicationsByEmployeeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveApplicationsByEmployeeIdAndLeaveTypeId(Guid employeeId, Guid leaveTypeId, AsyncCallback callback, Object state);
        List<LeaveApplicationDTO> EndFindLeaveApplicationsByEmployeeIdAndLeaveTypeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeLeaveBalances(Guid employeeId, Guid leaveTypeId, AsyncCallback callback, Object state);
        decimal EndFindEmployeeLeaveBalances(IAsyncResult result);
    }
}
