using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IEmployeeAppraisalPeriodAppService
    {
        #region EmployeeAppraisalPeriodDTO
        EmployeeAppraisalPeriodDTO AddNewEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, ServiceHeader serviceHeader);

        bool UpdateEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, ServiceHeader serviceHeader);

        List<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods(ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<EmployeeAppraisalPeriodDTO> FindEmployeeAppraisalPeriods(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        EmployeeAppraisalPeriodDTO FindEmployeeAppraisalPeriod(Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader);

        EmployeeAppraisalPeriodDTO FindCurrentEmployeeAppraisalPeriod(ServiceHeader serviceHeader);

        #endregion

        #region EmployeeAppraisalPeriodRecommendationDTO

        bool UpdateEmployeeAppraisalPeriodRecommendation(EmployeeAppraisalPeriodRecommendationDTO employeeAppraisalPeriodRecommendationDTO, ServiceHeader serviceHeader);

        EmployeeAppraisalPeriodRecommendationDTO FindEmployeeAppraisalPeriodRecommendation(Guid employeeId, Guid employeeAppraisalPeriodId, ServiceHeader serviceHeader);

        #endregion
    }
}
