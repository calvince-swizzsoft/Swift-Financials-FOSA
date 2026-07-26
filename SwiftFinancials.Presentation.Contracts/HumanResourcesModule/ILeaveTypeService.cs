using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "ILeaveTypeService")]
    public interface ILeaveTypeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewLeaveType(LeaveTypeDTO leaveTypeDTO, AsyncCallback callback, object state);
        LeaveTypeDTO EndAddNewLeaveType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateLeaveType(LeaveTypeDTO leaveTypeDTO, AsyncCallback callback, object state);
        bool EndUpdateLeaveType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveTypes(AsyncCallback callback, object state);
        List<LeaveTypeDTO> EndFindLeaveTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveTypesFilterInPage(string filterText, int pageIndex, int pageSize, AsyncCallback callback, object state);
        PageCollectionInfo<LeaveTypeDTO> EndFindLeaveTypesFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindLeaveType(Guid leaveTypeId, AsyncCallback callback, object state);
        LeaveTypeDTO EndFindLeaveType(IAsyncResult result);
    }
}
