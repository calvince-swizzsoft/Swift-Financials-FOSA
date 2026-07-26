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
    public interface IEmployerService
    {
        #region Employer

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<EmployerDTO> AddEmployerAsync(EmployerDTO employerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateEmployerAsync(EmployerDTO employerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<EmployerDTO>> FindEmployersAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<EmployerDTO> FindEmployerAsync(Guid employerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<DivisionDTO> FindDivisionAsync(Guid divisionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<EmployerDTO>> FindEmployersInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<EmployerDTO>> FindEmployersByFilterInPageAsync(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<DivisionDTO>> FindDivisionsAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<DivisionDTO>> FindDivisionsByEmployerIdAsync(Guid employerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<ZoneDTO>> FindZonesByEmployerIdAsync(Guid employerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateDivisionsByEmployerIdAsync(Guid employerId, List<DivisionDTO> divisions);

        #endregion
    }
}
