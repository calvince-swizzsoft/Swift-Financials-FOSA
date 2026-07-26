using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILeaveApplicationService
    {
        #region Leave Application

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LeaveApplicationDTO AddLeaveApplication(LeaveApplicationDTO leaveApplicationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLeaveApplication(LeaveApplicationDTO leaveApplicationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeLeaveApplication(LeaveApplicationDTO leaveApplicationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RecallLeaveApplication(LeaveApplicationDTO leaveApplicationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LeaveApplicationDTO> FindLeaveApplications();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplicationsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        decimal FindEmployeeLeaveBalances(Guid employeeId, Guid leaveTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplicationsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LeaveApplicationDTO> FindLeaveApplicationsByStatusAndFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LeaveApplicationDTO FindLeaveApplication(Guid leaveApplicationId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeId(Guid employeeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LeaveApplicationDTO> FindLeaveApplicationsByEmployeeIdAndLeaveTypeId(Guid employeeId, Guid leaveTypeId);

        #endregion
    }
}
