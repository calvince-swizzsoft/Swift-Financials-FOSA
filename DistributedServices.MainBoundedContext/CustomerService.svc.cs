using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;

        public CustomerService(
            ICustomerAppService customerAppService,
            ISqlCommandAppService sqlCommandAppService)
        {
            Guard.ArgumentNotNull(customerAppService, nameof(customerAppService));
            Guard.ArgumentNotNull(sqlCommandAppService, nameof(sqlCommandAppService));

            _customerAppService = customerAppService;
            _sqlCommandAppService = sqlCommandAppService;
        }

        #region Customer

        public async Task<CustomerDTO> AddCustomerAsync(CustomerDTO customerDTO, List<DebitTypeDTO> mandatoryDebitTypes, List<InvestmentProductDTO> mandatoryInvestmentProducts, List<SavingsProductDTO> mandatorySavingsProducts, ProductCollectionInfo mandatoryProducts, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.AddNewCustomerAsync(customerDTO, mandatoryDebitTypes,mandatoryInvestmentProducts,mandatorySavingsProducts, mandatoryProducts, moduleNavigationItemCode, serviceHeader);
        }

        public async Task<bool> UpdateCustomerAsync(CustomerDTO customerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdateCustomerAsync(customerDTO, serviceHeader);
        }

        public async Task<List<CustomerDTO>> FindCustomersAsync()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersAsync(serviceHeader);
        }

        public async Task<List<CustomerDTO>> FindCustomersByPayrollNumbersAsync(string payrollNumbers, bool matchExtact)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersByPayrollNumbersAsync(payrollNumbers, matchExtact, serviceHeader);
        }

        public async Task<List<CustomerDTO>> FindCustomersByIdentityCardNumberAsync(string identityCardNumber, bool matchExtact)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersByIdentityCardNumberAsync(identityCardNumber, matchExtact, serviceHeader);
        }

        public async Task<List<CustomerDTO>> FindCustomersByIDNumberAsync(string identityCardNumber)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersByIDNumberAsync(identityCardNumber, serviceHeader);
        }

        public async Task<CustomerDTO> FindCustomerAsync(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomerAsync(customerId, serviceHeader);
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersInPageAsync(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersAsync(pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersByFilterInPageAsync(string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersAsync(text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersByTypeAndFilterInPageAsync(int type, string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersByTypeAsync(type, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersByRecordStatusAndFilterInPageAsync(int recordStatus, string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersByRecordStatusAsync(recordStatus, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public async Task<PageCollectionInfo<CustomerDTO>> FindCustomersByStationIdAndFilterInPageAsync(Guid stationId, string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCustomersAsync(stationId, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public async Task<List<NextOfKinDTO>> FindNextOfKinCollectionByCustomerIdAsync(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindNextOfKinCollectionAsync(customerId, serviceHeader);
        }

        public async Task<List<AccountAlertDTO>> FindAccountAlertCollectionByCustomerIdAsync(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindAccountAlertCollectionAsync(customerId, serviceHeader);
        }

        public List<AccountAlertDTO> FindAccountAlertCollectionByCustomerIdAndAccountAlertType(Guid customerId, int accountAlertType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.FindAccountAlertCollection(customerId, accountAlertType, serviceHeader);
        }

        public async Task<List<PartnershipMemberDTO>> FindPartnershipMemberCollectionByPartnershipIdAsync(Guid partnershipId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindPartnershipMemberCollectionAsync(partnershipId, serviceHeader);
        }

        public async Task<List<CorporationMemberDTO>> FindCorporationMemberCollectionByCorporationIdAsync(Guid corporationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindCorporationMemberCollectionAsync(corporationId, serviceHeader);
        }

        public async Task<List<RefereeDTO>> FindRefereeCollectionByCustomerIdAsync(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.FindRefereeCollectionAsync(customerId, serviceHeader);
        }

        public List<CreditTypeDTO> FindCreditTypesByCustomerId(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.FindCreditTypes(customerId, serviceHeader);
        }

        public async Task<bool> UpdateNextOfKinCollectionAsync(CustomerDTO customerDTO, List<NextOfKinDTO> nextOfKinCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdateNextOfKinCollectionAsync(customerDTO, nextOfKinCollection, serviceHeader);
        }

        public async Task<bool> UpdateAccountAlertCollectionByCustomerIdAsync(Guid customerId, List<AccountAlertDTO> accountAlertCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdateAccountAlertCollectionAsync(customerId, accountAlertCollection, serviceHeader);
        }

        public async Task<bool> UpdatePartnershipMemberCollectionByPartnershipIdAsync(Guid partnershipId, List<PartnershipMemberDTO> partnershipMemberCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdatePartnershipMemberCollectionAsync(partnershipId, partnershipMemberCollection, serviceHeader);
        }

        public async Task<bool> UpdateCorporationMemberCollectionByCorporationIdAsync(Guid corporationId, List<CorporationMemberDTO> corporationMemberCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdateCorporationMemberCollectionAsync(corporationId, corporationMemberCollection, serviceHeader);
        }

        public async Task<bool> UpdateRefereeCollectionByCustomerIdAsync(Guid customerId, List<RefereeDTO> refereeCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdateRefereeCollectionAsync(customerId, refereeCollection, serviceHeader);
        }

        public async Task<bool> UpdateCustomerStationAsync(CustomerDTO customerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdateCustomerStationAsync(customerDTO, serviceHeader);
        }

        public async Task<bool> ResetCustomerStationAsync(List<CustomerDTO> customerDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.ResetCustomerStationAsync(customerDTOs, serviceHeader);
        }

        public bool UpdateCustomerBranch(CustomerDTO customerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.UpdateCustomerBranch(customerDTO, serviceHeader);
        }

        public async Task<bool> UpdateCreditTypesByCustomerIdAsync(Guid customerId, List<CreditTypeDTO> creditTypeDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.UpdateCreditTypesAsync(customerId, creditTypeDTOs, serviceHeader);
        }

        public decimal ComputeDividendsPayableByCustomerId(Guid customerId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _sqlCommandAppService.ComputeDividendsPayable(customerId, serviceHeader);
        }

        public PopulationRegisterQueryDTO AddPopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.AddNewPopulationRegisterQuery(populationRegisterQueryDTO, serviceHeader);
        }

        public bool AuthorizePopulationRegisterQuery(PopulationRegisterQueryDTO populationRegisterQueryDTO, int populationRegisterQueryAuthOption)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.AuthorizePopulationRegisterQuery(populationRegisterQueryDTO, populationRegisterQueryAuthOption, serviceHeader);
        }

        public bool SyncPopulationRegisterQueryResponse(PopulationRegisterQueryDTO populationRegisterQueryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.SyncPopulationRegisterQueryResponse(populationRegisterQueryDTO, serviceHeader);
        }

        public PopulationRegisterQueryDTO FindPopulationRegisterQuery(Guid populationRegisterQueryId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.FindPopulationRegisterQuery(populationRegisterQueryId, serviceHeader);
        }

        public PageCollectionInfo<PopulationRegisterQueryDTO> FindPopulationRegisterQueriesByFilterInPage(int status, DateTime startDate, DateTime endDate, string text, int populationRegisterFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.FindPopulationRegisterQueries(status, startDate, endDate, text, populationRegisterFilter, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<PopulationRegisterQueryDTO> FindThirdPartyNotifiablePopulationRegisterQueriesByFilterInPage(string text, int populationRegisterFilter, int pageIndex, int pageSize, int daysCap)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAppService.FindThirdPartyNotifiablePopulationRegisterQueries(text, populationRegisterFilter, pageIndex, pageSize, daysCap, serviceHeader);
        }

        public async Task<int> GetCustomersCountAsync()
        {
            ServiceHeader serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAppService.GetCustomersCountAsync(serviceHeader);
        }

        #endregion
    }
}

