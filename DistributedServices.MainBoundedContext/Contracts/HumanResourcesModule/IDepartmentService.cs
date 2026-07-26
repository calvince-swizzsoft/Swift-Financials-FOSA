using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IDepartmentService
    {
        #region Department

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DepartmentDTO AddDepartment(DepartmentDTO departmentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDepartment(DepartmentDTO departmentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DepartmentDTO> FindDepartments();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DepartmentDTO FindDepartment(Guid departmentId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DepartmentDTO> FindDepartmentsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DepartmentDTO> FindDepartmentsByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
