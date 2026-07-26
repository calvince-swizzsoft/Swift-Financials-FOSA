using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmployeeService
    {
        #region Employee

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDTO AddEmployee(EmployeeDTO employeeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployee(EmployeeDTO employeeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool CustomerIsEmployee(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeDTO> FindEmployees();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDTO> FindEmployeesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDTO> FindEmployeesByFilterInPage(string text, int pageIndex, int pageSize);


        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDTO> FindEmployeesByDepartmentIdAndFilterInPage(Guid departmentId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDTO FindEmployee(Guid employeeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeDTO> FindEmployeesBySalaryGroupsBranchesAndDepartments(SalaryProcessingDTO salaryPeriodDTO, List<SalaryGroupDTO> salaryGroups, List<BranchDTO> branches, List<DepartmentDTO> departments);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDTO FindEmployeeBySerialNumber(int serialNumber);

        #endregion

        #region EmployeeDisciplinaryCases

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDisciplinaryCaseDTO AddNewEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeDisciplinaryCaseDTO FindEmployeeDisciplinaryCase(Guid employeeDisciplinaryCaseId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCases();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeDisciplinaryCase(EmployeeDisciplinaryCaseDTO employeeDisciplinaryCaseDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeDisciplinaryCaseDTO> FindEmployeeDisciplinaryCasesByEmployeeId(Guid employeeId, int pageIndex, int pageSize);

        #endregion
    }
}
