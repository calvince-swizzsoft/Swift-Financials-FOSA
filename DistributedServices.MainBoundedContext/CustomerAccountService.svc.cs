using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
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
    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public CustomerAccountService(
            ICustomerAccountAppService customerAccountAppService)
        {
            Guard.ArgumentNotNull(customerAccountAppService, nameof(customerAccountAppService));

            _customerAccountAppService = customerAccountAppService;
        }

        #region Customer Account

        public CustomerAccountDTO AddCustomerAccount(CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);
        }

        public bool AddCustomerAccounts(CustomerDTO customerDTO, List<SavingsProductDTO> savingsProducts, List<InvestmentProductDTO> investmentProducts, List<LoanProductDTO> loanProducts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.AddNewCustomerAccounts(customerDTO, savingsProducts, investmentProducts, loanProducts, serviceHeader);
        }

        public bool UpdateCustomerAccount(CustomerAccountDTO customerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.UpdateCustomerAccount(customerAccountDTO, serviceHeader);
        }

        public bool ManageCustomerAccount(Guid customerAccountId, int managementAction, string remarks, int remarkType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.ManageCustomerAccount(customerAccountId, managementAction, remarks, remarkType, serviceHeader);
        }

        public bool ChargeAccountActivationFee(Guid customerAccountId, string remarks, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.ChargeAccountActivationFee(customerAccountId, remarks, moduleNavigationItemCode, serviceHeader);
        }

        public List<CustomerAccountDTO> FindCustomerAccounts(bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccounts(serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts, serviceHeader);
            }

            return customerAccounts;
        }

        public CustomerAccountDTO FindCustomerAccount(Guid customerAccountId, bool includeBalance, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);

            if (customerAccount != null)
            {
                if (includeBalance)
                    _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerAccount }, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccount }, serviceHeader);
            }

            return customerAccount;
        }

        public CustomerAccountDTO FindCustomerAccountByFullAccountNumber(string fullAccountNumber, bool includeBalance, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(fullAccountNumber, serviceHeader);

            if (customerAccount != null)
            {
                if (includeBalance)
                    _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerAccount }, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccount }, serviceHeader);
            }

            return customerAccount;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerAccountTypeTargetProductId(Guid targetProductId, int pageIndex, int pageSize, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerAccountTypeTargetProductId(targetProductId, pageIndex, pageSize, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts.PageCollection, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts.PageCollection, serviceHeader);
            }

            return customerAccounts;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsInPage(int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccounts(pageIndex, pageCount, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts.PageCollection, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts.PageCollection, serviceHeader);
            }

            return customerAccounts;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByFilterInPage(string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccounts(text, customerFilter, pageIndex, pageCount, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts.PageCollection, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts.PageCollection, serviceHeader);
            }

            return customerAccounts;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCodeAndFilterInPage(int productCode, string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccountsByProductCode(productCode, text, customerFilter, pageIndex, pageCount, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts.PageCollection, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts.PageCollection, serviceHeader);
            }

            return customerAccounts;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCodeAndRecordStatusAndFilterInPage(int productCode, int recordStatus, string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccountsByProductCodeAndRecordStatus(productCode, recordStatus, text, customerFilter, pageIndex, pageCount, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts.PageCollection, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts.PageCollection, serviceHeader);
            }

            return customerAccounts;
        }

        public PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndFilterInPage(Guid customerId, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(customerId, pageIndex, pageCount, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts.PageCollection, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts.PageCollection, serviceHeader);
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductId(Guid customerId, Guid targetProductId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerId, targetProductId, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts, serviceHeader);
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByCustomerId(Guid customerId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerId(customerId, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts, serviceHeader);
            }

            return customerAccounts;
        }

        public List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndProductCodes(Guid customerId, int[] targetProductCodes, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var customerAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCodes(customerId, targetProductCodes, serviceHeader);

            if (customerAccounts != null)
            {
                if (includeBalances)
                    _customerAccountAppService.FetchCustomerAccountBalances(customerAccounts, serviceHeader, includeInterestBalanceForLoanAccounts, considerMaturityPeriodForInvestmentAccounts);

                if (includeProductDescription)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerAccounts, serviceHeader);
            }

            return customerAccounts;
        }

        public List<CustomerAccountHistoryDTO> FindCustomerAccountHistoryByCustomerAccountId(Guid customerAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountHistory(customerAccountId, serviceHeader);
        }

        public List<CustomerAccountHistoryDTO> FindCustomerAccountHistoryByCustomerAccountIdAndManagementAction(Guid customerAccountId, int managementAction)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountHistory(customerAccountId, managementAction, serviceHeader);
        }

        public List<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountId(Guid customerAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountSignatoriesByCustomerAccountId(customerAccountId, serviceHeader);
        }

        public PageCollectionInfo<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountIdInPage(Guid customerAccountId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountSignatoriesByCustomerAccountId(customerAccountId, pageIndex, pageSize, serviceHeader);
        }

        public List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(benefactorCustomerAccountId, serviceHeader);
        }

        public PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdInPage(Guid benefactorCustomerAccountId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(benefactorCustomerAccountId, pageIndex, pageSize, serviceHeader);
        }

        public List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId, serviceHeader);
        }

        public List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountId(Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountId(beneficiaryCustomerAccountId, beneficiaryChartOfAccountId, serviceHeader);
        }

        public PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdInPage(Guid beneficiaryCustomerAccountId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(beneficiaryCustomerAccountId, pageIndex, pageSize, serviceHeader);
        }

        public List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountArrearagesByCustomerAccountId(customerAccountId, serviceHeader);
        }

        public List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountIdAndCategory(Guid customerAccountId, int category)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountArrearagesByCustomerAccountId(customerAccountId, category, serviceHeader);
        }

        public CustomerAccountSignatoryDTO AddCustomerAccountSignatory(CustomerAccountSignatoryDTO customerAccountSignatoryDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.AddNewCustomerAccountSignatory(customerAccountSignatoryDTO, serviceHeader);
        }

        public bool RemoveCustomerAccountSignatories(List<CustomerAccountSignatoryDTO> customerAccountSignatoryDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.RemoveCustomerAccountSignatories(customerAccountSignatoryDTOs, serviceHeader);
        }

        public decimal ComputeEligibleLoanAppraisalInvestmentsBalance(Guid customerId, Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.ComputeEligibleLoanAppraisalInvestmentsBalance(customerId, loanProductId, serviceHeader);
        }

        public double GetLoaneeAppraisalFactorByCustomerClassification(int customerClassification, Guid loanProductId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.GetLoaneeAppraisalFactorByCustomerClassification(customerClassification, loanProductId, serviceHeader);
        }

        public CustomerAccountArrearageDTO AddCustomerAccountArrearage(CustomerAccountArrearageDTO customerAccountArrearageDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.AddNewCustomerAccountArrearage(customerAccountArrearageDTO, serviceHeader);
        }

        public async Task<bool> UpdateCustomerAccountArrearagesAsync(List<CustomerAccountArrearageDTO> customerAccountArrearages)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return await _customerAccountAppService.UpdateCustomerAccountArrearagesAsync(customerAccountArrearages, serviceHeader);
        }

        public CustomerAccountCarryForwardDTO AddCustomerAccountCarryForward(CustomerAccountCarryForwardDTO customerAccountCarryForwardDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.AddNewCustomerAccountCarryForward(customerAccountCarryForwardDTO, serviceHeader);
        }

        public bool UpdateCustomerAccountCarryForwardInstallment(CustomerAccountCarryForwardInstallmentDTO customerAccountCarryForwardInstallmentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.UpdateCustomerAccountCarryForwardInstallment(customerAccountCarryForwardInstallmentDTO, serviceHeader);
        }

        public PageCollectionInfo<CustomerAccountCarryForwardInstallmentDTO> FindCustomerAccountCarryForwardInstallmentsByFilterInPage(DateTime startDate, DateTime endDate, string text, int customerFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.FindCustomerAccountCarryForwardInstallments(startDate, endDate, text, customerFilter, pageIndex, pageSize, serviceHeader);
        }

        public bool AdjustCustomerAccountLoanInterest(LoanInterestAdjustmentDTO loanInterestAdjustmentDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _customerAccountAppService.AdjustCustomerAccountLoanInterest(loanInterestAdjustmentDTO, serviceHeader);
        }

        #endregion
    }
}
