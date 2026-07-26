using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface ILeaveTypeAppService
    {
        LeaveTypeDTO AddNewLeaveType(LeaveTypeDTO leaveTypeDTO, ServiceHeader serviceHeader);

        bool UpdateLeaveType(LeaveTypeDTO leaveTypeDTO, ServiceHeader serviceHeader);

        List<LeaveTypeDTO> FindLeaveTypes(ServiceHeader serviceHeader);

        PageCollectionInfo<LeaveTypeDTO> FindLeaveTypes(string filterText, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        LeaveTypeDTO FindLeaveType(Guid leaveTypeId, ServiceHeader serviceHeader);
    }
}
