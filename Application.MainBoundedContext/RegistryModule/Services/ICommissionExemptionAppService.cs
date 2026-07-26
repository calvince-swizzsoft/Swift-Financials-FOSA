using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using System;
using System.Collections.Generic;
using Infrastructure.Crosscutting.Framework.Utils;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface ICommissionExemptionAppService
    {
        Task<CommissionExemptionDTO> AddNewCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateCommissionExemptionAsync(CommissionExemptionDTO commissionExemptionDTO, ServiceHeader serviceHeader);

        Task<CommissionExemptionEntryDTO> AddNewCommissionExemptionEntryAsync(CommissionExemptionEntryDTO commissionExemptionEntryDTO, ServiceHeader serviceHeader);

        Task<bool> RemoveCommissionExemptionEntriesAsync(List<CommissionExemptionEntryDTO> commissionExemptionEntryDTOs, ServiceHeader serviceHeader);

        Task<bool> UpdateCommissionExemptionEntryCollectionAsync(Guid commissionExemptionId, List<CommissionExemptionEntryDTO> commissionExemptionEntryCollection, ServiceHeader serviceHeader);

        Task<List<CommissionExemptionDTO>> FindCommissionExemptionsAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CommissionExemptionDTO>> FindCommissionExemptionsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<CommissionExemptionDTO> FindCommissionExemptionAsync(Guid commissionExemptionId, ServiceHeader serviceHeader);
            
        Task<List<CommissionExemptionEntryDTO> >FindCommissionExemptionEntriesByCommissionExemptionIdAsync(Guid commissionExemptionId, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCommissionExemptionIdAsync(Guid commissionExemptionId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<List<CommissionExemptionEntryDTO>> FindCommissionExemptionEntriesByCustomerIdAsync(Guid customerId, ServiceHeader serviceHeader);

        bool FetchCustomerCommissionExemptionStatus(CustomerAccountDTO customerAccount, Guid commissionId, ServiceHeader serviceHeader);

        bool FetchCachedCustomerCommissionExemptionStatus(CustomerAccountDTO customerAccount, Guid commissionId, ServiceHeader serviceHeader);
    }
}
