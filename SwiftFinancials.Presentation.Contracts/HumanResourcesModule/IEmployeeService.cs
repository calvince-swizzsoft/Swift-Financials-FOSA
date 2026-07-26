using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IEmployeeService")]
    public interface IEmployeeService
    {
        #region Employee

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmployee(EmployeeDTO employeeDTO, AsyncCallback callback, Object state);
        EmployeeDTO EndAddEmployee(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployee(EmployeeDTO employeeDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployee(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginCustomerIsEmployee(Guid customerId, AsyncCallback callback, Object state);
        bool EndCustomerIsEmployee(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployees(AsyncCallback callback, Object state);
        List<EmployeeDTO> EndFindEmployees(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDTO> EndFindEmployeesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDTO> EndFindEmployeesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeesByDepartmentIdAndFilterInPage(Guid departmentId, string text, int pageIndex, int pageSize, AsyncCallback callback, object state);
        PageCollectionInfo<EmployeeDTO> EndFindEmployeesByDepartmentIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployee(Guid employeeId, AsyncCallback callback, Object state);
        EmployeeDTO EndFindEmployee(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeesBySalaryGroupsBranchesAndDepartments(SalaryProcessingDTO salaryPeriodDTO, List<SalaryGroupDTO> salaryGroups, List<BranchDTO> branches, List<DepartmentDTO> departments, AsyncCallback callback, Object state);
        List<EmployeeDTO> EndFindEmployeesBySalaryGroupsBranchesAndDepartments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeBySerialNumber(int serialNumber, AsyncCallback callback, Object state);
        EmployeeDTO EndFindEmployeeBySerialNumber(IAsyncResult result);

        #endregion

        #region EmployeeDisciplinaryCases

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddNewEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, AsyncCallback callback, Object state);
        EmployeeDisciplinaryCaseDTO EndAddNewEmployeeDisciplinaryCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDisciplinaryCase(Guid employeeDisciplinaryCaseId, AsyncCallback callback, Object state);
        EmployeeDisciplinaryCaseDTO EndFindEmployeeDisciplinaryCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDisciplinaryCases(AsyncCallback callback, Object state);
        List<EmployeeDisciplinaryCaseDTO> EndFindEmployeeDisciplinaryCases(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDisciplinaryCasesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> EndFindEmployeeDisciplinaryCasesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDisciplinaryCasesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> EndFindEmployeeDisciplinaryCasesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployeeDisciplinaryCase(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeDisciplinaryCasesByEmployeeId(Guid employeeId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> EndFindEmployeeDisciplinaryCasesByEmployeeId(IAsyncResult result);

        #endregion
    }
}
