using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public interface ICustomerAppService
    {
        Task<CustomerDTO> AddNewCustomerAsync(CustomerDTO customerDTO, List<DebitTypeDTO> mandatoryDebitTypes, List<InvestmentProductDTO> mandatoryInvestmentProducts, List<SavingsProductDTO> mandatorySavingsProducts, ProductCollectionInfo mandatoryProducts, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        Task<bool> UpdateCustomerAsync(CustomerDTO customerDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateNextOfKinCollectionAsync(CustomerDTO customerDTO, List<NextOfKinDTO> nextOfKinCollection, ServiceHeader serviceHeader);

        Task<bool> UpdateAccountAlertCollectionAsync(Guid customerId, List<AccountAlertDTO> accountAlertCollection, ServiceHeader serviceHeader);

        Task<bool> UpdatePartnershipMemberCollectionAsync(Guid partnershipId, List<PartnershipMemberDTO> partnershipMemberCollection, ServiceHeader serviceHeader);

        Task<bool> UpdateCorporationMemberCollectionAsync(Guid corporationId, List<CorporationMemberDTO> corporationMemberCollection, ServiceHeader serviceHeader);

        Task<bool> UpdateRefereeCollectionAsync(Guid customerId, List<RefereeDTO> refereeCollection, ServiceHeader serviceHeader);

        Task<bool> UpdateCustomerStationAsync(CustomerDTO customerDTO, ServiceHeader serviceHeader);

        Task<bool> ResetCustomerStationAsync(List<CustomerDTO> customerDTOs, ServiceHeader serviceHeader);

        bool UpdateCustomerBranch(CustomerDTO customerDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateCreditTypesAsync(Guid customerId, List<CreditTypeDTO> creditTypeDTOs, ServiceHeader serviceHeader);

        Task<List<CustomerDTO>> FindCustomersAsync(ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersAsync(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByTypeAsync(int type, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByRecordStatusAsync(int recordStatus, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        Task<PageCollectionInfo<CustomerDTO>> FindCustomersAsync(Guid stationId, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CustomerDTO FindCustomer(Guid customerId, ServiceHeader serviceHeader);

        Task<CustomerDTO> FindCustomerAsync(Guid customerId, ServiceHeader serviceHeader);

        Task<List<CustomerDTO>> FindCustomersByPayrollNumbersAsync(string payrollNumbers, bool matchExtact, ServiceHeader serviceHeader);

        List<CustomerDTO> FindCustomerBySerialNumber(int serialNumber, ServiceHeader serviceHeader);

        Task<List<CustomerDTO>> FindCustomerBySerialNumberAsync(int serialNumber, ServiceHeader serviceHeader);

        Task<List<CustomerDTO>> FindCustomersByIdentityCardNumberAsync(string identityCardNumber, bool matchExtact, ServiceHeader serviceHeader);

        Task<List<CustomerDTO>> FindCustomersByIDNumberAsync(string identityCardNumber, ServiceHeader serviceHeader);

        Task<List<NextOfKinDTO>> FindNextOfKinCollectionAsync(Guid customerId, ServiceHeader serviceHeader);

        Task<List<AccountAlertDTO>> FindAccountAlertCollectionAsync(Guid customerId, ServiceHeader serviceHeader);

        List<AccountAlertDTO> FindAccountAlertCollection(Guid customerId, int accountAlertType, ServiceHeader serviceHeader);

        List<AccountAlertDTO> FindCachedAccountAlertCollection(Guid customerId, int accountAlertType, ServiceHeader serviceHeader);

        Task<List<PartnershipMemberDTO>> FindPartnershipMemberCollectionAsync(Guid customerId, ServiceHeader serviceHeader);

        Task<List<CorporationMemberDTO>> FindCorporationMemberCollectionAsync(Guid customerId, ServiceHeader serviceHeader);

        Task<List<RefereeDTO>> FindRefereeCollectionAsync(Guid customerId, ServiceHeader serviceHeader);

        List<CreditTypeDTO> FindCreditTypes(Guid customerId, ServiceHeader serviceHeader);

        PopulationRegisterQueryDTO AddNewPopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, ServiceHeader serviceHeader);

        bool AuthorizePopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, int populationRegisterQueryAuthOption, ServiceHeader serviceHeader);

        bool SyncPopulationRegisterQueryResponse(PopulationRegisterQueryDTO populationRegisterQueryDTO, ServiceHeader serviceHeader);

        PopulationRegisterQueryDTO FindPopulationRegisterQuery(Guid populationRegisterQueryId, ServiceHeader serviceHeader);

        PageCollectionInfo<PopulationRegisterQueryDTO> FindPopulationRegisterQueries(int status, DateTime startDate, DateTime endDate, string text, int populationRegisterFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<PopulationRegisterQueryDTO> FindThirdPartyNotifiablePopulationRegisterQueries(string text, int populationRegisterFilter, int pageIndex, int pageSize, int daysCap, ServiceHeader serviceHeader);

        Task<int> GetCustomersCountAsync(ServiceHeader serviceHeader);
    }
}
