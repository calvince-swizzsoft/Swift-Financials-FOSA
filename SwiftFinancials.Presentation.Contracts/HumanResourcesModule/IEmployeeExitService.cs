using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IEmployeeExitService")]
    public interface IEmployeeExitService
    {
        #region EmployeeExitDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewEmployeeExit(EmployeeExitDTO employeeExitDTO, AsyncCallback callback, object state);
        EmployeeExitDTO EndAddNewEmployeeExit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeExit(EmployeeExitDTO employeeExitDTO, AsyncCallback callback, object state);
        bool EndUpdateEmployeeExit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginVerifyEmployeeExit(EmployeeExitDTO employeeExitDTO, int auditOption, AsyncCallback callback, object state);
        bool EndVerifyEmployeeExit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginApproveEmployeeExit(EmployeeExitDTO employeeExitDTO, int authorizationOption, int moduleNavigationItemCode, AsyncCallback callback, object state);
        bool EndApproveEmployeeExit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeExitsByFilterAndDateInPage(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeExitDTO> EndFindEmployeeExitsByFilterAndDateInPage(IAsyncResult result);

        #endregion
    }
}
