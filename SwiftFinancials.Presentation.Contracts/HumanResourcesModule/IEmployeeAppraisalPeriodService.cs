using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IEmployeeAppraisalPeriodService")]
    public interface IEmployeeAppraisalPeriodService
    {
        #region EmployeeAppraisalPeriodDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, AsyncCallback callback, Object state);
        EmployeeAppraisalPeriodDTO EndAddEmployeeAppraisalPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeAppraisalPeriod(EmployeeAppraisalPeriodDTO employeeAppraisalPeriodDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployeeAppraisalPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalPeriods(AsyncCallback callback, Object state);
        List<EmployeeAppraisalPeriodDTO> EndFindEmployeeAppraisalPeriods(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalPeriodsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeAppraisalPeriodDTO> EndFindEmployeeAppraisalPeriodsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalPeriodsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeAppraisalPeriodDTO> EndFindEmployeeAppraisalPeriodsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalPeriod(Guid employeeAppraisalPeriodId, AsyncCallback callback, Object state);
        EmployeeAppraisalPeriodDTO EndFindEmployeeAppraisalPeriod(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCurrentEmployeeAppraisalPeriod(AsyncCallback callback, Object state);
        EmployeeAppraisalPeriodDTO EndFindCurrentEmployeeAppraisalPeriod(IAsyncResult result);

        #endregion

        #region EmployeeAppraisalPeriodRecommendationDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeAppraisalPeriodRecommendation(EmployeeAppraisalPeriodRecommendationDTO employeeAppraisalPeriodRecommendationDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployeeAppraisalPeriodRecommendation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeAppraisalPeriodRecommendation(Guid employeeId, Guid employeeAppraisalPeriodId, AsyncCallback callback, Object state);
        EmployeeAppraisalPeriodRecommendationDTO EndFindEmployeeAppraisalPeriodRecommendation(IAsyncResult result);

        #endregion
    }
}
