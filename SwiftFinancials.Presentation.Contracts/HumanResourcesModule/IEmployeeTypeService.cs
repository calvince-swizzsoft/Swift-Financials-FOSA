using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IEmployeeTypeService")]
    public interface IEmployeeTypeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmployeeType(EmployeeTypeDTO employeeTypeDTO, AsyncCallback callback, Object state);
        EmployeeTypeDTO EndAddEmployeeType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployeeType(EmployeeTypeDTO employeeTypeDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployeeType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeTypes(AsyncCallback callback, Object state);
        List<EmployeeTypeDTO> EndFindEmployeeTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeType(Guid employeeTypeId, AsyncCallback callback, Object state);
        EmployeeTypeDTO EndFindEmployeeType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeTypesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeTypeDTO> EndFindEmployeeTypesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployeeTypesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployeeTypeDTO> EndFindEmployeeTypesByFilterInPage(IAsyncResult result);
    }
}
