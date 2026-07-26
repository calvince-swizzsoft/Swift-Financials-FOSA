using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IDivisionService")]
    public interface IDivisionService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDivision(DivisionDTO DivisionDTO, AsyncCallback callback, Object state);
        DivisionDTO EndAddDivision(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDivision(DivisionDTO DivisionDTO, AsyncCallback callback, Object state);
        bool EndUpdateDivision(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDivisionsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DivisionDTO> EndFindDivisionsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDivisions(bool updateDepth, bool traverseTree, AsyncCallback callback, Object state);
        List<DivisionDTO> EndFindDivisions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDivisionsByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DivisionDTO> EndFindDivisionsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDivision(Guid DivisionId, AsyncCallback callback, Object state);
        DivisionDTO EndFindDivision(IAsyncResult result);
    }
}
