using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeePasswordHistoryAppService
    {
        EmployeePasswordHistoryDTO AddNewEmployeePasswordHistory(EmployeePasswordHistoryDTO employeePasswordHistoryDTO, ServiceHeader serviceHeader);

        List<EmployeePasswordHistoryDTO> FindEmployeePasswordHistories(ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeePasswordHistoryDTO> FindEmployeePasswordHistories(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EmployeePasswordHistoryDTO FindEmployeePasswordHistory(Guid employeePasswordHistoryId, ServiceHeader serviceHeader);

        List<EmployeePasswordHistoryDTO> FindEmployeePasswordHistories(Guid employeeId, ServiceHeader serviceHeader);

        bool ValidatePasswordHistory(Guid employeeId, string proposedPassword, int passwordHistoryPolicy, ServiceHeader serviceHeader);
    }
}
