using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "IDirectorService")]
    public interface IDirectorService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDirector(DirectorDTO directorDTO, AsyncCallback callback, Object state);
        DirectorDTO EndAddDirector(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDirector(DirectorDTO directorDTO, AsyncCallback callback, Object state);
        bool EndUpdateDirector(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectors(AsyncCallback callback, Object state);
        List<DirectorDTO> EndFindDirectors(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectorsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DirectorDTO> EndFindDirectorsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirectorsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DirectorDTO> EndFindDirectorsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDirector(Guid directorId, AsyncCallback callback, Object state);
        DirectorDTO EndFindDirector(IAsyncResult result);
    }
}
