using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IAdministrativeDivisionService
    {
        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<AdministrativeDivisionDTO> AddAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO);

        [OperationContract()] 
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateAdministrativeDivisionAsync(AdministrativeDivisionDTO administrativeDivisionDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))] 
        List<AdministrativeDivisionDTO> FindAdministrativeDivisions(bool updateDepth, bool traverseTree);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<AdministrativeDivisionDTO>> FindAdministrativeDivisionsByFilterInPageAsync(string filter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        AdministrativeDivisionDTO FindAdministrativeDivision(Guid administrativeDivisionId);
    }
}
