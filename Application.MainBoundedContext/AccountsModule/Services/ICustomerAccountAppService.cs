using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ICustomerAccountAppService
    {
        CustomerAccountDTO AddNewCustomerAccount(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader);

        bool AddNewCustomerAccounts(CustomerDTO customerDTO, List<SavingsProductDTO> savingsProducts, List<InvestmentProductDTO> investmentProducts, List<LoanProductDTO> loanProducts, ServiceHeader serviceHeader);

        bool UpdateCustomerAccount(CustomerAccountDTO customerAccountDTO, ServiceHeader serviceHeader);

        bool ManageCustomerAccount(Guid customerAccountId, int managementAction, string remarks, int remarkType, ServiceHeader serviceHeader);

        bool ChargeAccountActivationFee(Guid customerAccountId, string remarks, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool UpdateCustomerAccountsBranch(List<CustomerAccountDTO> customerAccountDTOs, ServiceHeader serviceHeader);

        CustomerAccountDTO FindCustomerAccountDTO(Guid customerAccountId, ServiceHeader serviceHeader);

        CustomerAccountDTO FindCustomerAccountDTO(string fullAccountNumber, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccounts(ServiceHeader serviceHeader);

        List<CustomerAccountSummaryDTO> FindCustomerAccounts(Guid[] customerAccountIds, ServiceHeader serviceHeader);

        CustomerAccountDTO FindCustomerAccounts(Guid customerAccountId, ServiceHeader serviceHeader);


        Task<List<CustomerAccountSummaryDTO>> FindCustomerAccountsAsync(Guid[] customerAccountIds, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindDefaultSavingsProductCustomerAccounts(Guid customerId, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccounts(int pageIndex, int pageCount, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccounts(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCode(int productCode, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByProductCodeAndRecordStatus(int productCode, int recordStatus, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerId(Guid customerId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountDTO> FindCustomerAccountsByCustomerAccountTypeTargetProductId(Guid targetProductId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByCustomerAccountTypeTargetProductIdAndCustomerEmployerId(Guid targetProductId, Guid customerEmployerId, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(Guid customerId, Guid targetProductId, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductIds(Guid customerId, Guid[] targetProductIds, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByCustomerId(Guid customerId, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCode(Guid customerId, int targetProductCode, ServiceHeader serviceHeader);

        List<CustomerAccountDTO> FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCodes(Guid customerId, int[] targetProductCodes, ServiceHeader serviceHeader);

        List<CustomerAccountHistoryDTO> FindCustomerAccountHistory(Guid customerAccountId, ServiceHeader serviceHeader);

        List<CustomerAccountHistoryDTO> FindCustomerAccountHistory(Guid customerAccountId, int managementAction, ServiceHeader serviceHeader);

        void FetchCustomerAccountsProductDescription(List<CustomerAccountDTO> customerAccounts, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<StandingOrderDTO> standingOrders, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<StandingOrderHistoryDTO> standingOrderHistory, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<ElectronicStatementOrderDTO> electronicStatementOrders, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<ElectronicStatementOrderHistoryDTO> electronicStatementOrderHistory, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<CreditBatchEntryDTO> creditBatchEntries, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<DebitBatchEntryDTO> debitBatchEntries, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<OverDeductionBatchEntryDTO> overDeductionBatchEntries, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<FixedDepositDTO> fixedDeposits, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<AlternateChannelDTO> alternateChannels, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<SalaryHeadDTO> salaryHeads, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<AttachedLoanDTO> attachedLoans, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<ExternalChequePayableDTO> externalChequePayables, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<DataAttachmentEntryDTO> dataAttachmentEntries, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<WithdrawalSettlementDTO> withdrawalSettlements, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<WireTransferBatchEntryDTO> eftBatchEntries, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<MobileToBankRequestDTO> mobileToBankRequests, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<AccountClosureRequestDTO> accountClosureRequests, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<LoanGuarantorAttachmentHistoryDTO> loanGuarantorAttachmentHistoryDTOs, ServiceHeader serviceHeader, bool useCache = true);

        void FetchCustomerAccountsProductDescription(List<LoanGuarantorAttachmentHistoryEntryDTO> loanGuarantorAttachmentHistoryEntries, ServiceHeader serviceHeader, bool useCache = true);

        decimal ComputeEligibleLoanAppraisalInvestmentsBalance(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader);

        double GetLoaneeAppraisalFactorByCustomerClassification(int customerClassification, Guid loanProductId, ServiceHeader serviceHeader);

        List<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountSignatoryDTO> FindCustomerAccountSignatoriesByCustomerAccountId(Guid customerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountId(Guid benefactorCustomerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader);

        List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountIdAndChartOfAccountId(Guid beneficiaryCustomerAccountId, Guid beneficiaryChartOfAccountId, ServiceHeader serviceHeader);

        List<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountId(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountCarryForwardDTO> FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, ServiceHeader serviceHeader);

        List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, DateTime endDate, ServiceHeader serviceHeader);

        List<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, int category, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountArrearageDTO> FindCustomerAccountArrearagesByCustomerAccountId(Guid customerAccountId, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        CustomerAccountSignatoryDTO AddNewCustomerAccountSignatory(CustomerAccountSignatoryDTO customerAccountSignatoryDTO, ServiceHeader serviceHeader);

        bool RemoveCustomerAccountSignatories(List<CustomerAccountSignatoryDTO> customerAccountSignatoryDTOs, ServiceHeader serviceHeader);

        void FetchCustomerAccountBalances(List<CustomerAccountDTO> customerAccounts, ServiceHeader serviceHeader, bool includeInterestBalanceForLoanAccounts = false, bool considerMaturityPeriodForInvestmentAccounts = false);

        CustomerAccountArrearageDTO AddNewCustomerAccountArrearage(CustomerAccountArrearageDTO customerAccountArrearageDTO, ServiceHeader serviceHeader);

        Task<bool> UpdateCustomerAccountArrearagesAsync(List<CustomerAccountArrearageDTO> customerAccountArrearages, ServiceHeader serviceHeader);

        CustomerAccountCarryForwardDTO AddNewCustomerAccountCarryForward(CustomerAccountCarryForwardDTO customerAccountCarryForwardDTO, ServiceHeader serviceHeader);

        bool UpdateCustomerAccountCarryForwardInstallment(CustomerAccountCarryForwardInstallmentDTO customerAccountCarryForwardInstallmentDTO, ServiceHeader serviceHeader);

        bool UpdateCustomerAccountCarryForwardInstallments(List<CustomerAccountCarryForwardInstallmentDTO> customerAccountCarryForwardInstallments, ServiceHeader serviceHeader);

        PageCollectionInfo<CustomerAccountCarryForwardInstallmentDTO> FindCustomerAccountCarryForwardInstallments(DateTime startDate, DateTime endDate, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        List<CustomerAccountCarryForwardInstallmentDTO> FindCustomerAccountCarryForwardInstallments(Guid customerAccountId, Guid chartOfAccountId, ServiceHeader serviceHeader);

        bool AdjustCustomerAccountLoanInterest(LoanInterestAdjustmentDTO loanInterestAdjustmentDTO, ServiceHeader serviceHeader);
    }
}
