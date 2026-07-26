using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IDepartmentService")]
    public interface IDepartmentService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDepartment(DepartmentDTO departmentDTO, AsyncCallback callback, Object state);
        DepartmentDTO EndAddDepartment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDepartment(DepartmentDTO departmentDTO, AsyncCallback callback, Object state);
        bool EndUpdateDepartment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDepartments(AsyncCallback callback, Object state);
        List<DepartmentDTO> EndFindDepartments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDepartment(Guid departmentId, AsyncCallback callback, Object state);
        DepartmentDTO EndFindDepartment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDepartmentsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DepartmentDTO> EndFindDepartmentsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDepartmentsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DepartmentDTO> EndFindDepartmentsByFilterInPage(IAsyncResult result);
    }
}
