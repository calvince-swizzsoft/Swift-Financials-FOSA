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
    public interface ICustomerAccountService
    {
        #region Customer Account

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CustomerAccountDTO AddCustomerAccount(CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddCustomerAccounts(CustomerDTO customerDTO, List<SavingsProductDTO> savingsProducts, List<InvestmentProductDTO> investmentProducts, List<LoanProductDTO> loanProducts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCustomerAccount(CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ManageCustomerAccount(Guid customerAccountId, int managementAction, string remarks, int remarkType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ChargeAccountActivationFee(Guid customerAccountId, string remarks, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountDTO> FindCustomerAccounts(bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CustomerAccountDTO FindCustomerAccount(Guid customerAccountId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CustomerAccountDTO FindCustomerAccountByFullAccountNumber(string fullAccountNumber, bool includeBalance, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerAccountTypeTargetProductId(Guid targetProductId, int pageIndex, int pageSize, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsInPage(int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByFilterInPage(string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCodeAndFilterInPage(int productCode, string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCodeAndRecordStatusAndFilterInPage(int productCode, int recordStatus, string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndFilterInPage(Guid customerId, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductId(Guid customerId, Guid targetProductId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountDTO> FindCustomerAccountsByCustomerId(Guid customerId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndProductCodes(Guid customerId, int[] targetProductCodes, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountHistoryDTO> FindCustomerAccountHistoryByCustomerAccountId(Guid customerAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountHistoryDTO> FindCustomerAccountHistoryByCustomerAccountIdAndManagementAction(Guid customerAccountId, int managementAction);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountId(Guid customerAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountIdInPage(Guid customerAccountId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountId(Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdInPage(Guid benefactorCustomerAccountId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdInPage(Guid beneficiaryCustomerAccountId, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountIdAndCategory(Guid customerAccountId, int category);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CustomerAccountSignatoryDTO AddCustomerAccountSignatory(CustomerAccountSignatoryDTO customerAccountSignatoryDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool RemoveCustomerAccountSignatories(List<CustomerAccountSignatoryDTO> customerAccountSignatoryDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        decimal ComputeEligibleLoanAppraisalInvestmentsBalance(Guid customerId, Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        double GetLoaneeAppraisalFactorByCustomerClassification(int customerClassification, Guid loanProductId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CustomerAccountArrearageDTO AddCustomerAccountArrearage(CustomerAccountArrearageDTO customerAccountArrearageDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        Task<bool> UpdateCustomerAccountArrearagesAsync(List<CustomerAccountArrearageDTO> customerAccountArrearages);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CustomerAccountCarryForwardDTO AddCustomerAccountCarryForward(CustomerAccountCarryForwardDTO customerAccountCarryForwardDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateCustomerAccountCarryForwardInstallment(CustomerAccountCarryForwardInstallmentDTO customerAccountCarryForwardInstallmentDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CustomerAccountCarryForwardInstallmentDTO> FindCustomerAccountCarryForwardInstallmentsByFilterInPage(DateTime startDate, DateTime endDate, string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AdjustCustomerAccountLoanInterest(LoanInterestAdjustmentDTO loanInterestAdjustmentDTO);

        #endregion
    }
}
