using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SwiftFinancials.Presentation.Contracts.AccountsModule
{
    [ServiceContract(Name = "ICustomerAccountService")]
    public interface ICustomerAccountService
    {
        //[OperationContract(AsyncPattern = true)]
        //[FaultContract(typeof(ApplicationServiceError))]
        //IAsyncResult BeginFindCustomerAccount(Guid customerAccountId, AsyncCallback callback, Object state);
        //CustomerAccountDTO EndFindCustomerAccount(IAsyncResult result);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCustomerAccount(CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        CustomerAccountDTO EndAddCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCustomerAccounts(CustomerDTO customerDTO, List<SavingsProductDTO> savingsProducts, List<InvestmentProductDTO> investmentProducts, List<LoanProductDTO> loanProducts, AsyncCallback callback, Object state);
        bool EndAddCustomerAccounts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCustomerAccount(CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        bool EndUpdateCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginManageCustomerAccount(Guid customerAccountId, int managementAction, string remarks, int remarkType, AsyncCallback callback, Object state);
        bool EndManageCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginChargeAccountActivationFee(Guid customerAccountId, string remarks, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndChargeAccountActivationFee(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccounts(bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        List<CustomerAccountDTO> EndFindCustomerAccounts(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccount(Guid customerAccountId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        CustomerAccountDTO EndFindCustomerAccount(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountByFullAccountNumber(string fullAccountNumber, bool includeBalance, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        CustomerAccountDTO EndFindCustomerAccountByFullAccountNumber(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByCustomerAccountTypeTargetProductId(Guid targetProductId, int pageIndex, int pageSize, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountDTO> EndFindCustomerAccountsByCustomerAccountTypeTargetProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsInPage(int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountDTO> EndFindCustomerAccountsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByFilterInPage(string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
       PageCollectionInfo<CustomerAccountDTO> EndFindCustomerAccountsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByProductCodeAndFilterInPage(int productCode, string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountDTO> EndFindCustomerAccountsByProductCodeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByProductCodeAndRecordStatusAndFilterInPage(int productCode, int recordStatus, string text, int customerFilter, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountDTO> EndFindCustomerAccountsByProductCodeAndRecordStatusAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByCustomerIdAndFilterInPage(Guid customerId, int pageIndex, int pageCount, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountDTO> EndFindCustomerAccountsByCustomerIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductId(Guid customerId, Guid targetProductId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        List<CustomerAccountDTO> EndFindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByCustomerId(Guid customerId, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        List<CustomerAccountDTO> EndFindCustomerAccountsByCustomerId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountsByCustomerIdAndProductCodes(Guid customerId, int[] targetProductCodes, bool includeBalances, bool includeProductDescription, bool includeInterestBalanceForLoanAccounts, bool considerMaturityPeriodForInvestmentAccounts, AsyncCallback callback, Object state);
        List<CustomerAccountDTO> EndFindCustomerAccountsByCustomerIdAndProductCodes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountHistoryByCustomerAccountId(Guid customerAccountId, AsyncCallback callback, Object state);
        List<CustomerAccountHistoryDTO> EndFindCustomerAccountHistoryByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountHistoryByCustomerAccountIdAndManagementAction(Guid customerAccountId, int managementAction, AsyncCallback callback, Object state);
        List<CustomerAccountHistoryDTO> EndFindCustomerAccountHistoryByCustomerAccountIdAndManagementAction(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountSignatoriesByCustomerAccountId(Guid customerAccountId, AsyncCallback callback, Object state);
        List<CustomerAccountSignatoryDTO> EndFindCustomerAccountSignatoriesByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountSignatoriesByCustomerAccountIdInPage(Guid customerAccountId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountSignatoryDTO> EndFindCustomerAccountSignatoriesByCustomerAccountIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdInPage(Guid benefactorCustomerAccountId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountCarryForwardDTO> EndFindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdInPage(Guid beneficiaryCustomerAccountId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountCarryForwardDTO> EndFindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, AsyncCallback callback, Object state);
        List<CustomerAccountCarryForwardDTO> EndFindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, AsyncCallback callback, Object state);
        List<CustomerAccountCarryForwardDTO> EndFindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountId(Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId, AsyncCallback callback, Object state);
        List<CustomerAccountCarryForwardDTO> EndFindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, AsyncCallback callback, Object state);
        List<CustomerAccountArrearageDTO> EndFindCustomerAccountArrearagesByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountArrearagesByCustomerAccountIdAndCategory(Guid customerAccountId, int category, AsyncCallback callback, Object state);
        List<CustomerAccountArrearageDTO> EndFindCustomerAccountArrearagesByCustomerAccountIdAndCategory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCustomerAccountSignatory(CustomerAccountSignatoryDTO customerAccountSignatoryDTO, AsyncCallback callback, Object state);
        CustomerAccountSignatoryDTO EndAddCustomerAccountSignatory(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginRemoveCustomerAccountSignatories(List<CustomerAccountSignatoryDTO> customerAccountSignatoryDTOs, AsyncCallback callback, Object state);
        bool EndRemoveCustomerAccountSignatories(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginComputeEligibleLoanAppraisalInvestmentsBalance(Guid customerId, Guid loanProductId, AsyncCallback callback, Object state);
        decimal EndComputeEligibleLoanAppraisalInvestmentsBalance(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetLoaneeAppraisalFactorByCustomerClassification(int customerClassification, Guid loanProductId, AsyncCallback callback, Object state);
        double EndGetLoaneeAppraisalFactorByCustomerClassification(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCustomerAccountArrearage(CustomerAccountArrearageDTO customerAccountArrearageDTO, AsyncCallback callback, Object state);
        CustomerAccountArrearageDTO EndAddCustomerAccountArrearage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCustomerAccountArrearages(List<CustomerAccountArrearageDTO> customerAccountArrearages, AsyncCallback callback, Object state);
        bool EndUpdateCustomerAccountArrearages(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCustomerAccountCarryForward(CustomerAccountCarryForwardDTO customerAccountCarryForwardDTO, AsyncCallback callback, Object state);
        CustomerAccountCarryForwardDTO EndAddCustomerAccountCarryForward(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateCustomerAccountCarryForwardInstallment(CustomerAccountCarryForwardInstallmentDTO customerAccountCarryForwardInstallmentDTO, AsyncCallback callback, Object state);
        bool EndUpdateCustomerAccountCarryForwardInstallment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCustomerAccountCarryForwardInstallmentsByFilterInPage(DateTime startDate, DateTime endDate, string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CustomerAccountCarryForwardInstallmentDTO> EndFindCustomerAccountCarryForwardInstallmentsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAdjustCustomerAccountLoanInterest(LoanInterestAdjustmentDTO loanInterestAdjustmentDTO, AsyncCallback callback, Object state);
        bool EndAdjustCustomerAccountLoanInterest(IAsyncResult result);


       
    }
}
