using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IDirectorService
    {
        #region Director

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DirectorDTO AddDirector(DirectorDTO directorDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDirector(DirectorDTO directorDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DirectorDTO> FindDirectors();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DirectorDTO> FindDirectorsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DirectorDTO> FindDirectorsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DirectorDTO FindDirector(Guid directorId);

        #endregion
    }
}
