using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ICustomerService
    {
        #region Customer

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CustomerDTO> AddCustomerAsync(CustomerDTO customerDTO, List<DebitTypeDTO> mandatoryDebitTypes, List<InvestmentProductDTO> mandatoryInvestmentProducts, List<SavingsProductDTO> mandatorySavingsProducts, ProductCollectionInfo mandatoryProducts, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCustomerAsync(CustomerDTO customerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CustomerDTO>> FindCustomersAsync();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CustomerDTO>> FindCustomersByPayrollNumbersAsync(string payrollNumbers, bool matchExtact);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CustomerDTO>> FindCustomersByIdentityCardNumberAsync(string identityCardNumber, bool matchExtact);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CustomerDTO>> FindCustomersByIDNumberAsync(string identityCardNumber);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<CustomerDTO> FindCustomerAsync(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CustomerDTO>> FindCustomersInPageAsync(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByFilterInPageAsync(string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByTypeAndFilterInPageAsync(int type, string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByRecordStatusAndFilterInPageAsync(int recordStatus, string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<PageCollectionInfo<CustomerDTO>> FindCustomersByStationIdAndFilterInPageAsync(Guid stationId, string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<NextOfKinDTO>> FindNextOfKinCollectionByCustomerIdAsync(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<AccountAlertDTO>> FindAccountAlertCollectionByCustomerIdAsync(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<AccountAlertDTO> FindAccountAlertCollectionByCustomerIdAndAccountAlertType(Guid customerId, int accountAlertType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<PartnershipMemberDTO>> FindPartnershipMemberCollectionByPartnershipIdAsync(Guid partnershipId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<CorporationMemberDTO>> FindCorporationMemberCollectionByCorporationIdAsync(Guid corporationId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<List<RefereeDTO>> FindRefereeCollectionByCustomerIdAsync(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CreditTypeDTO> FindCreditTypesByCustomerId(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateNextOfKinCollectionAsync(CustomerDTO customerDTO, List<NextOfKinDTO> nextOfKinCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateAccountAlertCollectionByCustomerIdAsync(Guid customerId, List<AccountAlertDTO> accountAlertCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdatePartnershipMemberCollectionByPartnershipIdAsync(Guid partnershipId, List<PartnershipMemberDTO> partnershipMemberCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCorporationMemberCollectionByCorporationIdAsync(Guid corporationId, List<CorporationMemberDTO> corporationMemberCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateRefereeCollectionByCustomerIdAsync(Guid customerId, List<RefereeDTO> refereeCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCustomerStationAsync(CustomerDTO customerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> ResetCustomerStationAsync(List<CustomerDTO> customerDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCustomerBranch(CustomerDTO customerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCreditTypesByCustomerIdAsync(Guid customerId, List<CreditTypeDTO> creditTypeDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        decimal ComputeDividendsPayableByCustomerId(Guid customerId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PopulationRegisterQueryDTO AddPopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizePopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, int populationRegisterQueryAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool SyncPopulationRegisterQueryResponse(PopulationRegisterQueryDTO populationRegisterQueryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PopulationRegisterQueryDTO FindPopulationRegisterQuery(Guid populationRegisterQueryId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PopulationRegisterQueryDTO> FindPopulationRegisterQueriesByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int populationRegisterFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<PopulationRegisterQueryDTO> FindThirdPartyNotifiablePopulationRegisterQueriesByFilterInPage(string text, int populationRegisterFilter, int pageIndex, int pageSize, int daysCap);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<int> GetCustomersCountAsync();
        #endregion
    }
}
