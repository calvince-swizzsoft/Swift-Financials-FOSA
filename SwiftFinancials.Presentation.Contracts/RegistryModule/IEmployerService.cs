using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IEmployerService")]
    public interface IEmployerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddEmployer(EmployerDTO employerDTO, AsyncCallback callback, Object state);
        EmployerDTO EndAddEmployer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateEmployer(EmployerDTO employerDTO, AsyncCallback callback, Object state);
        bool EndUpdateEmployer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployers(AsyncCallback callback, Object state);
        List<EmployerDTO> EndFindEmployers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployer(Guid employerId, AsyncCallback callback, Object state);
        EmployerDTO EndFindEmployer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDivision(Guid divisionId, AsyncCallback callback, Object state);
        DivisionDTO EndFindDivision(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployerDTO> EndFindEmployersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindEmployersByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<EmployerDTO> EndFindEmployersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDivisions(AsyncCallback callback, Object state);
        List<DivisionDTO> EndFindDivisions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDivisionsByEmployerId(Guid employerId, AsyncCallback callback, Object state);
        List<DivisionDTO> EndFindDivisionsByEmployerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindZonesByEmployerId(Guid employerId, AsyncCallback callback, Object state);
        List<ZoneDTO> EndFindZonesByEmployerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDivisionsByEmployerId(Guid employerId, List<DivisionDTO> divisions, AsyncCallback callback, Object state);
        bool EndUpdateDivisionsByEmployerId(IAsyncResult result);
    }
}
