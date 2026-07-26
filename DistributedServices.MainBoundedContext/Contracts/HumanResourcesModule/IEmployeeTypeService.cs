using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IEmployeeTypeService
    {
        #region EmployeeType

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeTypeDTO AddEmployeeType(EmployeeTypeDTO employeeTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateEmployeeType(EmployeeTypeDTO employeeTypeDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<EmployeeTypeDTO> FindEmployeeTypes();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        EmployeeTypeDTO FindEmployeeType(Guid employeeTypeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypesInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<EmployeeTypeDTO> FindEmployeeTypesByFilterInPage(string text, int pageIndex, int pageSize);

        #endregion
    }
}
