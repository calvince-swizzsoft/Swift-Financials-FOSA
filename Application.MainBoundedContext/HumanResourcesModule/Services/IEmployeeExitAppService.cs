using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeExitAppService
    {
        #region EmployeeExitDTO

        EmployeeExitDTO AddNewEmployeeExit(EmployeeExitDTO employeeExitDTO, ServiceHeader serviceHeader);

        bool UpdateEmployeeExit(EmployeeExitDTO employeeExitDTO, ServiceHeader serviceHeader);

        bool VerifyEmployeeExit(EmployeeExitDTO employeeExitDTO, int auditOption, ServiceHeader serviceHeader);

        bool ApproveEmployeeExit(EmployeeExitDTO employeeExitDTO, int authorizationOption, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeExitDTO> FindEmployeeExits(int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        #endregion
    }
}
