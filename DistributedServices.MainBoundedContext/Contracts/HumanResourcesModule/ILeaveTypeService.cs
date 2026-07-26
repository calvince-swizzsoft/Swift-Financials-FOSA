using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ILeaveTypeService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LeaveTypeDTO AddNewLeaveType(LeaveTypeDTO leaveTypeDTO );

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateLeaveType(LeaveTypeDTO leaveTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<LeaveTypeDTO> FindLeaveTypes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<LeaveTypeDTO> FindLeaveTypesFilterInPage(string filterText, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        LeaveTypeDTO FindLeaveType(Guid leaveTypeId);
    }
}
