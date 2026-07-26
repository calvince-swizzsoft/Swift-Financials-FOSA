using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.RegistryModule
{
    [ServiceContract(Name = "ICustomerService")]
    public interface ICustomerService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCustomer(CustomerDTO customerDTO, List<DebitTypeDTO> mandatoryDebitTypes, List<InvestmentProductDTO> mandatoryInvestmentProducts, ProductCollectionInfo mandatoryProducts, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        CustomerDTO EndAddCustomer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCustomer(CustomerDTO customerDTO, AsyncCallback callback, Object state);
        bool EndUpdateCustomer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomers(AsyncCallback callback, Object state);
        List<CustomerDTO> EndFindCustomers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersByPayrollNumbers(string payrollNumbers, bool matchExtact, AsyncCallback callback, Object state);
        List<CustomerDTO> EndFindCustomersByPayrollNumbers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersByIdentityCardNumber(string identityCardNumber, bool matchExtact, AsyncCallback callback, Object state);
        List<CustomerDTO> EndFindCustomersByIdentityCardNumber(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersByIDNumber(string identityCardNumber, AsyncCallback callback, Object state);
        List<CustomerDTO> EndFindCustomersByIDNumber(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomer(Guid customerId, AsyncCallback callback, Object state);
        CustomerDTO EndFindCustomer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerDTO> EndFindCustomersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersByFilterInPage(string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerDTO> EndFindCustomersByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersByTypeAndFilterInPage(int type, string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerDTO> EndFindCustomersByTypeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersByRecordStatusAndFilterInPage(int recordStatus, string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerDTO> EndFindCustomersByRecordStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomersByStationIdAndFilterInPage(Guid stationId, string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerDTO> EndFindCustomersByStationIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindNextOfKinCollectionByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<NextOfKinDTO> EndFindNextOfKinCollectionByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAccountAlertCollectionByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<AccountAlertDTO> EndFindAccountAlertCollectionByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindAccountAlertCollectionByCustomerIdAndAccountAlertType(Guid customerId, int accountAlertType, AsyncCallback callback, Object state);
        List<AccountAlertDTO> EndFindAccountAlertCollectionByCustomerIdAndAccountAlertType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPartnershipMemberCollectionByPartnershipId(Guid partnershipId, AsyncCallback callback, Object state);
        List<PartnershipMemberDTO> EndFindPartnershipMemberCollectionByPartnershipId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCorporationMemberCollectionByCorporationId(Guid corporationId, AsyncCallback callback, Object state);
        List<CorporationMemberDTO> EndFindCorporationMemberCollectionByCorporationId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindRefereeCollectionByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<RefereeDTO> EndFindRefereeCollectionByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCreditTypesByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        List<CreditTypeDTO> EndFindCreditTypesByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateNextOfKinCollection(CustomerDTO customerDTO, List<NextOfKinDTO> nextOfKinCollection, AsyncCallback callback, Object state);
        bool EndUpdateNextOfKinCollection(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateAccountAlertCollectionByCustomerId(Guid customerId, List<AccountAlertDTO> accountAlertCollection, AsyncCallback callback, Object state);
        bool EndUpdateAccountAlertCollectionByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdatePartnershipMemberCollectionByPartnershipId(Guid partnershipId, List<PartnershipMemberDTO> partnershipMemberCollection, AsyncCallback callback, Object state);
        bool EndUpdatePartnershipMemberCollectionByPartnershipId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCorporationMemberCollectionByCorporationId(Guid corporationId, List<CorporationMemberDTO> corporationMemberCollection, AsyncCallback callback, Object state);
        bool EndUpdateCorporationMemberCollectionByCorporationId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateRefereeCollectionByCustomerId(Guid customerId, List<RefereeDTO> refereeCollection, AsyncCallback callback, Object state);
        bool EndUpdateRefereeCollectionByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCreditTypesByCustomerId(Guid customerId, List<CreditTypeDTO> creditTypeDTOs, AsyncCallback callback, Object state);
        bool EndUpdateCreditTypesByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCustomerStation(CustomerDTO customerDTO, AsyncCallback callback, Object state);
        bool EndUpdateCustomerStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginResetCustomerStation(List<CustomerDTO> customerDTOs, AsyncCallback callback, Object state);
        bool EndResetCustomerStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCustomerBranch(CustomerDTO customerDTO, AsyncCallback callback, Object state);
        bool EndUpdateCustomerBranch(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeDividendsPayableByCustomerId(Guid customerId, AsyncCallback callback, Object state);
        decimal EndComputeDividendsPayableByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddPopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, AsyncCallback callback, Object state);
        PopulationRegisterQueryDTO EndAddPopulationRegisterQuery(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizePopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, int populationRegisterQueryAuthOption, AsyncCallback callback, Object state);
        bool EndAuthorizePopulationRegisterQuery(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginSyncPopulationRegisterQueryResponse(PopulationRegisterQueryDTO populationRegisterQueryDTO, AsyncCallback callback, Object state);
        bool EndSyncPopulationRegisterQueryResponse(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPopulationRegisterQuery(Guid populationRegisterQueryId, AsyncCallback callback, Object state);
        PopulationRegisterQueryDTO EndFindPopulationRegisterQuery(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindPopulationRegisterQueriesByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int populationRegisterFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<PopulationRegisterQueryDTO> EndFindPopulationRegisterQueriesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindThirdPartyNotifiablePopulationRegisterQueriesByFilterInPage(string text, int populationRegisterFilter, int pageIndex, int pageSize, int daysCap, AsyncCallback callback, Object state);
        PageCollectionInfo<PopulationRegisterQueryDTO> EndFindThirdPartyNotifiablePopulationRegisterQueriesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetCustomersCount(AsyncCallback callback, object state);
        int EndGetCustomersCount(IAsyncResult result);
    }
}
