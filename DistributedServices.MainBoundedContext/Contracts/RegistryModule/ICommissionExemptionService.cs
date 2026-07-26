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
    public interface ICommissionExemptionService
    {
        #region Commission Exemption

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CommissionExemptionDTO> AddCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CommissionExemptionEntryDTO> AddCommissionExemptionEntryAsync(CommissionExemptionEntryDTO commissionExemptionEntryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> RemoveCommissionExemptionEntriesAsync(List<CommissionExemptionEntryDTO> commissionExemptionEntryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCommissionExemptionEntryCollectionAsync(Guid commissionExemptionId, List<CommissionExemptionEntryDTO> commissionExemptionEntryCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CommissionExemptionDTO>> FindCommissionExemptionsAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CommissionExemptionDTO>> FindCommissionExemptionsByFilterInPageAsync(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CommissionExemptionDTO> FindCommissionExemptionAsync(Guid commissionExemptionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdAsync(Guid commissionExemptionId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdInPageAsync(Guid commissionExemptionId, string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCustomerIdAsync(Guid customerId);

        #endregion
    }
}
