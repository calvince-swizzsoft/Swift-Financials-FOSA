using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IAdministrativeDivisionService")]
    public interface IAdministrativeDivisionService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddAdministrativeDivision(AdministrativeDivisionDTO administrativeDivisionDTO, AsyncCallback callback, Object state);
        AdministrativeDivisionDTO EndAddAdministrativeDivision(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAdministrativeDivision(AdministrativeDivisionDTO administrativeDivisionDTO, AsyncCallback callback, Object state);
        bool EndUpdateAdministrativeDivision(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAdministrativeDivisionsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<AdministrativeDivisionDTO> EndFindAdministrativeDivisionsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAdministrativeDivisions(bool updateDepth, bool traverseTree, AsyncCallback callback, Object state);
        List<AdministrativeDivisionDTO> EndFindAdministrativeDivisions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAdministrativeDivisionsByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<AdministrativeDivisionDTO> EndFindAdministrativeDivisionsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAdministrativeDivision(Guid administrativeDivisionId, AsyncCallback callback, Object state);
        AdministrativeDivisionDTO EndFindAdministrativeDivision(IAsyncResult result);
    }
}
