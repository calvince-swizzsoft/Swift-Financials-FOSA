using System;
using System.ComponentModel;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public enum CustomerAccountTransactionRole
    {
        [Description("Credit Customer Account")]
        CreditCustomerAccount = 0,
        [Description("Debit Customer Account")]
        DebitCustomerAccount = 1
    }

    public enum FrontOfficeCashRequestType
    {
        [Description("deposit")]
        CashDeposit = 0,
        [Description("withdrawal")]
        CashWithdrawal = 1,
        [Description("transfer")]
        CashTransfer = 2
    }


    public enum ChartOfAccountType
    {
        [Description("Asset")]
        Asset = 0x3E8,/*1000*/
        [Description("Liability")]
        Liability = 0x7D0,/*2000*/
        [Description("Equity/Capital")]
        Equity = 0xBB8,/*3000*/
        [Description("Income/Revenue")]
        Income = 0xFA0,/*4000*/
        [Description("Expense")]
        Expense = 0x1388,/*5000*/
    }


    public enum PaymentAccountType
    {
        [Description("Customer")]
        Customer = 0x3E8,/*1000*/
        [Description("Vendor")]
        Vendor = 0x7D0,/*2000*/
        [Description("GL Account")]
        GLAccount = 0xBB8,/*3000*/
        [Description("Fixed Asset")]
        FixedAsset = 0xFA0,/*4000*/
    }


    public enum PaymentDocumentType
    {
        [Description("Invoice")]
        Invoice = 0x3E8,/*1000*/
        [Description("CreditMemo")]
        CreditMemo = 0x7D0,/*2000*/
    }


    public enum ReceiptAccountType
    {
        [Description("Customer")]
        Customer = 0x3E8,/*1000*/
        [Description("Vendor")]
        Vendor = 0x7D0,/*2000*/
        [Description("GL Account")]
        GLAccount = 0xBB8,/*3000*/
        [Description("Fixed Asset")]
        FixedAsset = 0xFA0,/*4000*/
    }


    public enum ReceiptDocumentType
    {
        [Description("Invoice")]
        Invoice = 0x3E8,/*1000*/
        [Description("CreditMemo")]
        CreditMemo = 0x7D0,/*2000*/
    }


    public enum ChartOfAccountCategory
    {
        [Description("Header Account (Non-Postable)")]
        HeaderAccount = 0x1000,/*4096*/
        [Description("Detail Account (Postable)")]
        DetailAccount = 0x1000 + 1/*4097*/
    }

    public enum ProductCode
    {
        [Description("Savings")]
        Savings = 0x001,
        [Description("Loan")]
        Loan = 0x001 + 1,
        [Description("Investment")]
        Investment = 0x001 + 2,
    }

    public enum LoanProductSection
    {
        [Description("FOSA")]
        FOSA = 0x000,
        [Description("BOSA")]
        BOSA = 0x000 + 1,
    }

    public enum LoanProductCategory
    {
        [Description("Short-Term")]
        ShortTerm = 0x000,
        [Description("Long-Term")]
        LongTerm = 0x000 + 1
    }

    public enum AuditLogEventType
    {
        [Description("Entity_Added")]
        Entity_Added = 0XBEEF/*48879*/,
        [Description("Entity_Modified")]
        Entity_Modified = 0XBEEF + 1,
        [Description("Entity_Deleted")]
        Entity_Deleted = 0XBEEF + 2,
        [Description("Sys_Login")]
        Sys_Login = 0XBEEF + 3,
        [Description("Sys_Logout")]
        Sys_Logout = 0XBEEF + 4,
        [Description("Sys_Other")]
        Sys_Other = 0XBEEF + 5
    }

    public enum SystemPermissionType
    {
        [Description("Savings Withdrawal Authorization")]
        CashWithdrawalRequestAuthorization = 0xAFC0/*44992*/,
        [Description("Credit Batch Authorization")]
        CreditBatchAuthorization = 0xAFC0 + 2,
        [Description("Journal Voucher Authorization")]
        JournalVoucherAuthorization = 0xAFC0 + 3,
        [Description("Refund Batch Authorization")]
        OverDeductionBatchAuthorization = 0xAFC0 + 4,
        [Description("Debit Batch Authorization")]
        DebitBatchAuthorization = 0xAFC0 + 5,
        [Description("Loan Disbursement Batch Authorization")]
        LoanDisbursementBatchAuthorization = 0xAFC0 + 44,
        [Description("Journal Reversal Batch Authorization")]
        JournalReversalBatchAuthorization = 0xAFC0 + 45,
        [Description("Wire Transfer Batch Authorization")]
        WireTransferBatchAuthorization = 0xAFC0 + 23,
        [Description("Inter-Account Transfer Batch Authorization")]
        InterAccountTransferBatchAuthorization = 0xAFC0 + 49,
        [Description("Expense Payables Authorization")]
        ExpensePayablesAuthorization = 0xAFC0 + 50,

        [Description("Salary Period Posting")]
        SalaryPeriodPosting = 0xAFC0 + 7,

        [Description("BOSA Loan Verification/Cancellation")]
        BackOfficeLoanAudit = 0xAFC0 + 19,
        [Description("BOSA Loan Appraisal")]
        BackOfficeLoanAppraisal = 0xAFC0 + 20,
        [Description("BOSA Loan Approval")]
        BackOfficeLoanApproval = 0xAFC0 + 21,
        [Description("BOSA Loan Registration")]
        BackOfficeLoanRegistration = 0xAFC0 + 22,
        [Description("BOSA Loan Restructuring ")]
        BackOfficeLoanRestructuring = 0xAFC0 + 25,

        [Description("FOSA Loan Verification/Cancellation")]
        FrontOfficeLoanAudit = 0xAFC0 + 15,
        [Description("FOSA Loan Appraisal")]
        FrontOfficeLoanAppraisal = 0xAFC0 + 16,
        [Description("FOSA Loan Approval")]
        FrontOfficeLoanApproval = 0xAFC0 + 17,
        [Description("FOSA Loan Registration")]
        FrontOfficeLoanRegistration = 0xAFC0 + 18,
        [Description("FOSA Loan Restructuring ")]
        FrontOfficeLoanRestructuring = 0xAFC0 + 24,

        [Description("Employee Account Viewing")]
        EmployeeCustomerAccountViewing = 0xAFC0 + 10,
        [Description("Membership Termination Processing")]
        MembershipTerminationProcessing = 0xAFC0 + 11,

        [Description("Operations Maker")]
        Maker = 0xAFC0 + 12,
        [Description("Operations Checker")]
        Checker = 0xAFC0 + 13,

        [Description("Alternate Channels Reconciliation")]
        AlternateChannelsReconciliation = 0xAFC0 + 26,
        [Description("Alternate Channel Linking")]
        AlternateChannelLinking = 0xAFC0 + 27,
        [Description("Alternate Channel Replacement")]
        AlternateChannelReplacement = 0xAFC0 + 28,
        [Description("Alternate Channel Renewal")]
        AlternateChannelRenewal = 0xAFC0 + 29,
        [Description("Alternate Channel Delinking")]
        AlternateChannelDelinking = 0xAFC0 + 30,
        [Description("Alternate Channel Stoppage")]
        AlternateChannelStoppage = 0xAFC0 + 63,

        [Description("Customer Account Activation")]
        CustomerAccountActivation = 0xAFC0 + 31,
        [Description("Customer Account Freezing")]
        CustomerAccountDeactivation = 0xAFC0 + 32,
        [Description("Customer Account Remark")]
        CustomerAccountRemark = 0xAFC0 + 33,
        [Description("Payment Voucher Management")]
        PaymentVoucherManagement = 0xAFC0 + 34,
        [Description("Microcredit Apportionment")]
        MicrocreditApportionment = 0xAFC0 + 35,
        [Description("Truncated Cheques Processing")]
        TruncatedChequesProcessing = 0xAFC0 + 36,
        [Description("Leave Management")]
        LeaveManagement = 0xAFC0 + 37,

        [Description("Credit Batch Verification")]
        CreditBatchAudit = 0xAFC0 + 38,
        [Description("Journal Voucher Verification")]
        JournalVoucherAudit = 0xAFC0 + 39,
        [Description("Refund Batch Verification")]
        OverDeductionBatchAudit = 0xAFC0 + 40,
        [Description("Debit Batch Verification")]
        DebitBatchAudit = 0xAFC0 + 41,
        [Description("Wire Transfer Batch Verification")]
        WireTransferBatchAudit = 0xAFC0 + 42,
        [Description("Loan Disbursement Batch Verification")]
        LoanDisbursementBatchAudit = 0xAFC0 + 43,
        [Description("Journal Reversal Batch Verification")]
        JournalReversalBatchAudit = 0xAFC0 + 46,
        [Description("Credit Batch Discrepancy Resolution")]
        CreditBatchEntryMatching = 0xAFC0 + 47,
        [Description("Inter-Account Transfer Batch Verification")]
        InterAccountTransferBatchAudit = 0xAFC0 + 48,
        [Description("Expense Payables Verification")]
        ExpensePayablesAudit = 0xAFC0 + 51,

        [Description("Intra-Acccount Transfer")]
        IntraAcccountTransfer = 0xAFC0 + 52,

        [Description("Fixed Deposit Fixing")]
        FixedDepositFixing = 0xAFC0 + 53,
        [Description("Fixed Deposit Termination")]
        FixedDepositTermination = 0xAFC0 + 54,
        [Description("Fixed Deposit Liquidation")]
        FixedDepositLiquidation = 0xAFC0 + 55,
        [Description("Fixed Deposit Verification")]
        FixedDepositAudit = 0xAFC0 + 68,

        [Description("Mobile To Bank Reconciliation")]
        MobileToBankReconciliation = 0xAFC0 + 56,
        [Description("Suppress Maker/Checker Validation")]
        SuppressMakerCheckerValidation = 0xAFC0 + 57,

        [Description("Intra-Account Apportionment (Investment-to-Savings)")]
        InvestmentToSavingsIntraAccountApportionment = 0xAFC0 + 58,
        [Description("Intra-Account Apportionment (Investment-to-Loan)")]
        InvestmentToLoanIntraAccountApportionment = 0xAFC0 + 59,
        [Description("Intra-Account Apportionment (Investment-to-Investment)")]
        InvestmentToInvestmentIntraAccountApportionment = 0xAFC0 + 60,

        [Description("General Ledger Verification")]
        GeneralLedgerAudit = 0xAFC0 + 61,
        [Description("General Ledger Authorization")]
        GeneralLedgerAuthorization = 0xAFC0 + 62,

        [Description("Sundry Cash Payment Authorization")]
        CashPaymentRequestAuthorization = 0xAFC0 + 64,
        [Description("Customer Account Verification")]
        CustomerAccountVerification = 0xAFC0 + 65,
        [Description("Customer Verification")]
        CustomerVerification = 0xAFC0 + 66,
        [Description("Alternate Channel Verification")]
        AlternateChannelVerification = 0xAFC0 + 67,

        [Description("Goods Dispatch Note Origination")]
        GoodsDispatchNoteOrigination = 0xAFC0 + 74,
        [Description("Goods Dispatch Note Cancellation")]
        GoodsDispatchNoteCancellation = 0xAFC0 + 75,

        [Description("Asset Disposal Verification")]
        AssetDisposalAudit = 0xAFC0 + 78,
        [Description("Asset Disposal Authorization")]
        AssetDisposalAuthorization = 0xAFC0 + 79,

        [Description("Super Saver Payable Verification")]
        SuperSaverPayableAudit = 0xAFC0 + 80,
        [Description("Super Saver Payable Authorization")]
        SuperSaverPayableAuthorization = 0xAFC0 + 81,

        [Description("Funeral Rider Claim Payable Verification")]
        FuneralRiderClaimPayableAudit = 0xAFC0 + 82,
        [Description("Funeral Rider Claim Payable Authorization")]
        FuneralRiderClaimPayableAuthorization = 0xAFC0 + 83,

        [Description("Employee Appraiser")]
        EmployeeAppraiser = 0xAFC0 + 85,

        [Description("Member Statement Printing")]
        MemberStatementPrinting = 0xAFC0 + 86,

        [Description("Standing Order Editing")]
        StandingOrderEditing = 0xAFC0 + 87,

        [Description("Employee Exit Verification")]
        EmployeeExitAudit = 0xAFC0 + 88,
        [Description("Employee Exit Authorization")]
        EmployeeExitAuthorization = 0xAFC0 + 89,

        [Description("Requisition Origination")]
        RequisitionOrigination = 0xAFC0 + 90,
        [Description("Purchase Order Origination")]
        PurchaseOrderOrigination = 0xAFC0 + 91,
        [Description("Goods Received Note Origination")]
        GoodsReceivedNoteOrigination = 0xAFC0 + 92,
        [Description("Invoice Origination")]
        InvoiceOrigination = 0xAFC0 + 93,

        [Description("Arrearages Management")]
        ArrearagesManagement = 0xAFC0 + 94,
        [Description("Carry Forwards Management")]
        CarryForwardsManagement = 0xAFC0 + 95,
        [Description("Carry Forward Installment Editing")]
        CarryForwardInstallmentEditing = 0xAFC0 + 96,

        [Description("Mobile To Bank Reconciliation Verification")]
        MobileToBankReconciliationVerification = 0xAFC0 + 97,

        [Description("Customer Registration")]
        CustomerRegistration = 0xAFC0 + 98,
        [Description("Customer Editing")]
        CustomerEditing = 0xAFC0 + 99,
        [Description("Savings Deposit Authorization")]
        CashDepositRequestAuthorization = 0xAFC0 + 100,

        [Description("External Cheque Clearance")]
        ExternalChequeClearance = 0xAFC0 + 101,
        [Description("Customer Account Signing Instructions")]
        CustomerAccountSigningInstructions = 0xAFC0 + 102,

        [Description("Loan Interest Adjustment")]
        LoanInterestAdjustment = 0xAFC0 + 103,
    }

    public enum SystemTransactionType
    {
        [Description("Express Membership Termination Fee")]
        PrematureMembershipTerminationCharges = 0xF0E0 + 14,
        [Description("Normal Membership Termination Fee")]
        NormalMembershipTerminationCharges = 0xF0E0 + 15,

        [Description("Pay As You Earn (PAYE) Fee")]
        PAYECharges = 0xF0E0 + 17,
        [Description("National Social Security Fund (NSSF) Fee")]
        NSSFCharges = 0xF0E0 + 18,
        [Description("National Hospital Insurance Fund (NHIF) Fee")]
        NHIFCharges = 0xF0E0 + 19,
        [Description("Provident Fund Fee")]
        ProvidentFundCharges = 0xF0E0 + 20,

        [Description("Deceased Membership Termination  Processing Fee")]
        DeceasedMembershipTerminationProcessingFee = 0xF0E0 + 36,
        [Description("Voluntary Membership Termination Processing Fee")]
        VoluntaryMembershipTerminationProcessingFee = 0xF0E0 + 37,
        [Description("Retiree Membership Termination Processing Fee")]
        RetireeMembershipTerminationProcessingFee = 0xF0E0 + 38,

        [Description("Member Funeral Rider Claim Fee")]
        MemberFuneralRiderClaimFee = 0xF0E0 + 39,
        [Description("Spouse Funeral Rider Claim Fee")]
        SpouseFuneralRiderClaimFee = 0xF0E0 + 40,

        [Description("Customer Account Activation Fee")]
        CustomerAccountActivationFee = 0xF0E0 + 41,
    }



    public enum SystemGeneralLedgerAccountCode
    {
        [Description("Payables Control")]
        PayablesControl = 0xBEBA/*48826*/,
        [Description("External Cheques Control")]
        ExternalChequesControl = 0xBEBA + 1,
        [Description("In-House Cheques Control")]
        InHouseChequesControl = 0xBEBA + 3,
        [Description("Electronic Funds Transfer Control")]
        ElectronicFundsTransferControl = 0xBEBA + 4,
        [Description("Profit & Loss Appropriation")]
        Appropriation = 0xBEBA + 5,
        [Description("Fixed Deposit")]
        FixedDeposit = 0xBEBA + 6,
        [Description("Fixed Deposit Interest")]
        FixedDepositInterest = 0xBEBA + 7,
        [Description("Sacco-Link Settlement")]
        SaccoLinkSettlement = 0xBEBA + 9,
        [Description("Sacco-Link Settlement (POS)")]
        SaccoLinkPOSSettlement = 0xBEBA + 18,
        [Description("Deceased Control")]
        DeceasedControl = 0xBEBA + 10,
        [Description("MCo-op Cash Settlement")]
        MCoopCashSettlement = 0xBEBA + 12,
        [Description("External Cheques-In-Hand")]
        ExternalChequesInHand = 0xBEBA + 13,
        [Description("PesaPepe Settlement (B2C)")]
        MobileWalletB2CSettlement = 0xBEBA + 14,
        [Description("PesaPepe Settlement (C2B)")]
        MobileWalletC2BSettlement = 0xBEBA + 20,
        [Description("Legacy Balances Control")]
        LegacyBalancesControl = 0xBEBA + 16,
        [Description("SpotCash Settlement")]
        SpotCashSettlement = 0xBEBA + 17,
        [Description("Truncated Cheques Settlement")]
        TruncatedChequesSettlement = 0xBEBA + 19,
        [Description("Institution Settlement")]
        InstitutionSettlement = 0xBEBA + 21,
        [Description("Agent Commission Settlement")]
        AgentCommissionSettlement = 0xBEBA + 22,
        [Description("PesaPepe Settlement (Airtime)")]
        MobileWalletAirtimeSettlement = 0xBEBA + 23,
        [Description("ABC Bank Settlement")]
        AbcBankSettlement = 0xBEBA + 24,
        [Description("Employer's Contribution (NSSF)")]
        EmployerNSSFContribution = 0xBEBA + 25,
        [Description("Employer's Contribution (Provident Fund)")]
        EmployerProvidentFundContribution = 0xBEBA + 26,
        [Description("PesaPepe Settlement (SMS)")]
        MobileWalletSMSSettlement = 0xBEBA + 27,
        [Description("PesaPepe Settlement (Owner)")]
        MobileWalletOwnerSettlement = 0xBEBA + 28,
        [Description("Super Saver Withholding Tax")]
        SuperSaverWithholdingTax = 0xBEBA + 29,
        [Description("Super Saver Interest")]
        SuperSaverInterest = 0xBEBA + 30,
        [Description("Funeral Rider Expense")]
        FuneralRiderExpense = 0xBEBA + 31,

        [Description("Account Payables")]
        AccountPayables = 0xBEBA + 32,

        [Description("Account Receivables")]
        AccountReceivables = 0xBEBA + 33,

        [Description("Internal Debtors")]
        InternalDebtors = 0xBEBA + 34,

        [Description("Inventory")]
        Inventory = 0xBEBA + 35
    }

    public enum Gender
    {
        [Description("")]
        Unknown = 0x000,
        [Description("Male")]
        Male = 0x000 + 1,
        [Description("Female")]
        Female = 0x000 + 2,
        [Description("Non-Binary")]
        NonBinary = 0x000 + 3,
    }

    public enum MaritalStatus
    {
        [Description("")]
        Unknown = 0x000,
        [Description("Married")]
        Married = 0x000 + 1,
        [Description("Single")]
        Single = 0x000 + 2,
        [Description("Divorced")]
        Divorced = 0x000 + 3,
        [Description("Separated")]
        Separated = 0x000 + 4,
    }

    public enum Salutation
    {
        [Description("")]
        Unknown = 0x000,
        [Description("Mr")]
        Mr = 0x000 + 1,
        [Description("Mrs")]
        Mrs = 0x000 + 2,
        [Description("Miss")]
        Miss = 0x000 + 3,
        [Description("Dr")]
        Dr = 0x000 + 4,
        [Description("Prof")]
        Prof = 0x000 + 5,
        [Description("Rev")]
        Rev = 0x000 + 6,
        [Description("Eng")]
        Eng = 0x000 + 7,
        [Description("Hon")]
        Hon = 0x000 + 8,
        [Description("Cllr")]
        Cllr = 0x000 + 9,
        [Description("Sir")]
        Sir = 0x000 + 10,
        [Description("Ms")]
        Ms = 0x000 + 11,
        [Description("Fr")]
        Fr = 0x000 + 12,
        [Description("Sr")]
        Sr = 0x000 + 13,
    }

    public enum TermsOfService
    {
        [Description("")]
        Unknown = 0x000,
        [Description("Temporary")]
        Temporary = 0x000 + 1,
        [Description("Permanent")]
        Permanent = 0x000 + 2,
        [Description("Contract")]
        Contract = 0x000 + 3
    }

    public enum SalaryHeadType
    {
        [Description("Basic Pay Earning (Full-Time)")]
        FullTimeBasicPayEarning = 0xF0F0,
        [Description("N.S.S.F Deduction")]
        NSSFDeduction = 0xF0F0 + 1,
        [Description("N.H.I.F Deduction")]
        NHIFDeduction = 0xF0F0 + 2,
        [Description("P.A.Y.E Deduction")]
        PAYEDeduction = 0xF0F0 + 3,
        [Description("Provident Fund Deduction (Statutory)")]
        StatutoryProvidentFundDeduction = 0xF0F0 + 4,
        [Description("Loan Deduction")]
        LoanDeduction = 0xF0F0 + 6,
        [Description("Investment Deduction")]
        InvestmentDeduction = 0xF0F0 + 7,
        [Description("Other Earning")]
        OtherEarning = 0xF0F0 + 8,
        [Description("Other Deduction")]
        OtherDeduction = 0xF0F0 + 9,
        [Description("Provident Fund Deduction (Voluntary)")]
        VoluntaryProvidentFundDeduction = 0xF0F0 + 10,
        [Description("Basic Pay Earning (Part-Time)")]
        PartTimeBasicPayEarning = 0xF0F0 + 11,
        [Description("Basic Pay Earning (Contract)")]
        ContractBasicPayEarning = 0xF0F0 + 12,
    }

    [Flags]
    public enum SalaryHeadCategory
    {
        [Description("Earning")]
        Earning = 1,
        [Description("Deduction")]
        Deduction = 2
    }

    [Flags]
    public enum SalaryPeriodStatus
    {
        [Description("Open")]
        Open = 1,
        [Description("Closed")]
        Closed = 2,
        [Description("Suspended")]
        Suspended = 4
    }

    [Flags]
    public enum PaySlipStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
    }

    public enum Month
    {
        [Description("January")]
        Jan = 1,
        [Description("February")]
        Feb,
        [Description("March")]
        Mar,
        [Description("April")]
        Apr,
        [Description("May")]
        May,
        [Description("June")]
        Jun,
        [Description("July")]
        Jul,
        [Description("August")]
        Aug,
        [Description("September")]
        Sep,
        [Description("October")]
        Oct,
        [Description("November")]
        Nov,
        [Description("December")]
        Dec
    };

    public enum AlternateChannelManagementAction
    {
        [Description("Channel Linking")]
        Linking = 0xBEBE,
        [Description("Channel Replacement")]
        Replacement = 0xBEBE + 1,
        [Description("Channel Renewal")]
        Renewal = 0xBEBE + 2,
        [Description("Channel Delinking")]
        Delinking = 0xBEBE + 6,
        [Description("Channel Stoppage")]
        Stoppage = 0xBEBE + 7,
    }

    public enum CustomerAccountManagementAction
    {
        [Description("Account Activation")]
        Activation = 0xBEBE + 3,
        [Description("Account Freezing")]
        Deactivation = 0xBEBE + 4,
        [Description("Account Remark")]
        Remark = 0xBEBE + 5,
        [Description("Account Closure")]
        Closure = 0xBEBE + 8,
        [Description("Account Signing Instructions")]
        SigningInstructions = 0xBEBE + 9
    }

    public enum CustomerAccountRemarkType
    {
        [Description("Actionable")]
        Actionable = 0,
        [Description("Informational")]
        Informational = 1,
    }

    public enum InterestCalculationMode
    {
        [Description("Reducing Balance")]
        ReducingBalance = 0x200,
        [Description("Straight Line")]
        StraightLine = 0x200 + 1,
        [Description("Amortization (Straight Line)")]
        StraightLineAmortization = 0x200 + 2,
        [Description("Amortization (Diminishing Balance)")]
        DiminishingBalanceAmortization = 0x200 + 3,
        [Description("Fixed Interest")]
        FixedInterest = 0x200 + 4,
    }

    public enum InterestChargeMode
    {
        [Description("Upfront")]
        Upfront = 0x300,
        [Description("Periodic")]
        Periodic = 0x300 + 1
    }

    public enum InterestRecoveryMode
    {
        [Description("Upfront")]
        Upfront = 0x400,
        [Description("Periodic")]
        Periodic = 0x400 + 1
    }

    public enum DynamicChargeRecoveryMode
    {
        [Description("Upfront")]
        Upfront = 0x500,
        [Description("Periodic")]
        Periodic = 0x500 + 1,
        [Description("Carry Forward")]
        CarryForward = 0x500 + 2
    }

    public enum DynamicChargeRecoverySource
    {
        [Description("Loan Account")]
        LoanAccount = 0x600,
        [Description("Savings Account")]
        SavingsAccount = 0x600 + 1
    }

    public enum DynamicChargeInstallmentsBasisValue
    {
        [Description("Approved Loan Amount")]
        LoanCaseApprovedAmount = 0,
        [Description("Attached Loans Amount")]
        AttachedLoansAmount = 1,
    }

    public enum PayoutRecoveryMode
    {
        [Description("Per Standing Order")]
        StandingOrder = 0x700,
        [Description("Outstanding Percentage")]
        Percentage = 0x700 + 1
    }

    public enum AggregateCheckOffRecoveryMode
    {
        [Description("Outstanding Balance")]
        OutstandingBalance = 0x0000,
        [Description("Per Standing Order")]
        StandingOrder = 0x0001,
    }

    public enum NextOfKinRelationship
    {
        [Description("")]
        Unknown = 0x0000,
        [Description("Father")]
        Father = 0x0000 + 1,
        [Description("Mother")]
        Mother = 0x0000 + 2,
        [Description("Brother")]
        Brother = 0x0000 + 3,
        [Description("Sister")]
        Sister = 0x0000 + 4,
        [Description("Wife")]
        Wife = 0x0000 + 5,
        [Description("Husband")]
        Husband = 0x0000 + 6,
        [Description("Son")]
        Son = 0x0000 + 7,
        [Description("Daughter")]
        Daughter = 0x0000 + 8,
        [Description("Trustee")]
        Trustee = 0x0000 + 9,
        [Description("Other")]
        Other = 0x0000 + 10,
    }

    public enum PartnershipRelationship
    {
        [Description("")]
        Unknown = 0x0000,
        [Description("Father")]
        Father = 0x0000 + 1,
        [Description("Mother")]
        Mother = 0x0000 + 2,
        [Description("Brother")]
        Brother = 0x0000 + 3,
        [Description("Sister")]
        Sister = 0x0000 + 4,
        [Description("Wife")]
        Wife = 0x0000 + 5,
        [Description("Husband")]
        Husband = 0x0000 + 6,
        [Description("Son")]
        Son = 0x0000 + 7,
        [Description("Daughter")]
        Daughter = 0x0000 + 8,
    }

    public enum FrontOfficeTransactionType
    {
        [Description("Cash Withdrawal")]
        CashWithdrawal = 1,
        [Description("Cash Deposit")]
        CashDeposit = 2,
        [Description("Cheque Deposit")]
        ChequeDeposit = 3,
        [Description("Payment Voucher")]
        CashWithdrawalPaymentVoucher = 4
    }

    [Flags]
    public enum CurrencyCounterTallyMode
    {
        [Description("Tally-by-count")]
        TallyByCount = 1,
        [Description("Tally-by-total")]
        TallyByTotal = 2
    }

    [Flags]
    public enum ChargeType
    {
        [Description("Percentage")]
        Percentage = 1,
        [Description("Fixed Amount")]
        FixedAmount = 2
    }

    [Flags]
    public enum CashWithdrawalCategory
    {
        [Description("Cash Withdrawal Within Limits")]
        WithinLimits = 1,
        [Description("Cash Withdrawal Above Maximum Allowed")]
        AboveMaximumAllowed = 2,
        [Description("Cash Withdrawal Below Minimum Balance")]
        BelowMinimumBalance = 4,
        [Description("Cash Overdraw")]
        Overdraw = 8,
        [Description("Payment Voucher")]
        PaymentVoucher = 16,
        [Description("Cash Payment")]
        CashPayment = 32,
        [Description("Cash Payment Above Budget Balance")]
        AboveBudgetBalance = 64,
    }

    public enum CashWithdrawalRequestType
    {
        [Description("Immediate Notice")]
        ImmediateNotice = 0,
        [Description("Future Notice")]
        FutureNotice = 1
    }

    [Flags]
    public enum CashWithdrawalRequestAuthStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Authorized")]
        Authorized = 2,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Paid")]
        Paid = 8
    }

    public enum CashTransferRequestStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Acknowledged")]
        Acknowledged = 2,
        [Description("Rejected")]
        Rejected = 3,
        [Description("Utilized")]
        Utilized = 4
    }

    public enum CashTransferRequestAcknowledgeOption
    {
        [Description("Acknowledge")]
        Acknowledge = 2,
        [Description("Reject")]
        Reject = 3,
    }

    [Flags]
    public enum AccountClosureRequestStatus
    {
        [Description("Registered")]
        Registered = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Verified")]
        Audited = 4,
        [Description("Settled")]
        Settled = 8,
        [Description("Deferred")]
        Deferred = 16,
    }

    [Flags]
    public enum AccountClosureApprovalOption
    {
        [Description("Approve")]
        Approve = 1,
        [Description("Defer")]
        Defer = 2,
    }

    [Flags]
    public enum AccountClosureAuditOption
    {
        [Description("Verify")]
        Audit = 1,
        [Description("Defer")]
        Defer = 2,
    }

    [Flags]
    public enum AccountClosureSettlementOption
    {
        [Description("Settle")]
        Settle = 1,
        [Description("Defer")]
        Defer = 2,
    }

    public enum LoanCaseStatus
    {
        [Description("Registered")]
        Registered = 0xBEBA,
        [Description("Appraised")]
        Appraised = 0xBEBA + 1,
        [Description("Approved")]
        Approved = 0xBEBA + 2,
        [Description("Disbursed")]
        Disbursed = 0xBEBA + 3,
        [Description("Rejected")]
        Rejected = 0xBEBA + 4,
        [Description("Deferred")]
        Deferred = 0xBEBA + 5,
        [Description("Verified")]
        Audited = 0xBEBA + 6,
        [Description("Restructured")]
        Restructured = 0xBEBA + 7, //48833
    }

    public enum LoanCaseFilter
    {
        [Description("Loan Number")]
        CaseNumber = 0,
        [Description("Reference")]
        Reference,
        [Description("Branch")]
        Branch,
        [Description("Loan Purpose")]
        Purpose,
        [Description("Loan Product")]
        LoanProduct,
        [Description("Amount Applied")]
        AmountApplied,
        [Description("Batch Number")]
        BatchNumber,
        [Description("Remarks")]
        Remarks,
        [Description("Serial Number")]
        CustomerSerialNumber,
        [Description("Personal Identification #")]
        CustomerPersonalIdentificationNumber,
        [Description("First Name")]
        CustomerFirstName,
        [Description("Last Name")]
        CustomerLastName,
        [Description("Identity Card #")]
        CustomerIdentityCardNumber,
        [Description("Payroll Numbers")]
        CustomerPayrollNumbers,
        [Description("Org. Name")]
        CustomerNonIndividual_Description,
        [Description("Org. Registration #")]
        CustomerNonIndividual_RegistrationNumber,
        [Description("Address Line 1")]
        CustomerAddressLine1,
        [Description("Address Line 2")]
        CustomerAddressLine2,
        [Description("Street")]
        CustomerStreet,
        [Description("Postal Code")]
        CustomerPostalCode,
        [Description("City")]
        CustomerCity,
        [Description("Email")]
        CustomerEmail,
        [Description("Land Line")]
        CustomerLandLine,
        [Description("Mobile Line")]
        CustomerMobileLine,
        [Description("Account Number")]
        CustomerReference1,
        [Description("Membership Number")]
        CustomerReference2,
        [Description("Personal File Number")]
        CustomerReference3,
    }

    public enum LoanRequestStatus
    {
        [Description("New")]
        New = 0,
        [Description("Registered")]
        Registered = 1,
        [Description("Rejected")]
        Rejected = 2,
    }

    public enum LoanRequestFilter
    {
        [Description("Reference")]
        Reference,
        [Description("Loan Purpose")]
        Purpose,
        [Description("Loan Product")]
        LoanProduct,
        [Description("Amount Applied")]
        AmountApplied,
        [Description("Serial Number")]
        CustomerSerialNumber,
        [Description("Personal Identification #")]
        CustomerPersonalIdentificationNumber,
        [Description("First Name")]
        CustomerFirstName,
        [Description("Last Name")]
        CustomerLastName,
        [Description("Identity Card #")]
        CustomerIdentityCardNumber,
        [Description("Payroll Numbers")]
        CustomerPayrollNumbers,
        [Description("Org. Name")]
        CustomerNonIndividual_Description,
        [Description("Org. Registration #")]
        CustomerNonIndividual_RegistrationNumber,
        [Description("Address Line 1")]
        CustomerAddressLine1,
        [Description("Address Line 2")]
        CustomerAddressLine2,
        [Description("Street")]
        CustomerStreet,
        [Description("Postal Code")]
        CustomerPostalCode,
        [Description("City")]
        CustomerCity,
        [Description("Email")]
        CustomerEmail,
        [Description("Land Line")]
        CustomerLandLine,
        [Description("Mobile Line")]
        CustomerMobileLine,
        [Description("Account Number")]
        CustomerReference1,
        [Description("Membership Number")]
        CustomerReference2,
        [Description("Personal File Number")]
        CustomerReference3,
    }

    public enum IncomeAdjustmentType
    {
        [Description("Allowance")]
        Allowance = 0xFADE,
        [Description("Deduction")]
        Deduction = 0xFADE + 1,
    }

    public enum FileMovementStatus
    {
        [Description("Unknown")]
        Unknown,
        [Description("Dispatched")]
        Dispatched = 0xCABA,
        [Description("Received")]
        Received = 0xCABA + 1,
    }

    [Flags]
    public enum LoanAppraisalOption
    {
        [Description("Appraise")]
        Appraise = 1,
        [Description("Reject")]
        Reject = 2,
    }

    [Flags]
    public enum LoanApprovalOption
    {
        [Description("Approve")]
        Approve = 1,
        [Description("Reject")]
        Reject = 2,
        [Description("Defer")]
        Defer = 4,
    }

    [Flags]
    public enum LoanAuditOption
    {
        [Description("Verify")]
        Audit = 1,
        [Description("Reject")]
        Reject = 2,
        [Description("Defer")]
        Defer = 4,
    }

    [Flags]
    public enum LoanCancellationOption
    {
        [Description("Defer")]
        Defer = 1,
        [Description("Reject")]
        Reject = 2,
    }

    public enum JournalVoucherType
    {
        [Description("Debit G/L Account")]
        DebitGLAccount = 0,//53456
        [Description("Credit G/L Account")]
        CreditGLAccount = 1,//53457
        [Description("Debit Customer Account")]
        DebitCustomerAccount = 2,//53458
        [Description("Credit Customer Account")]
        CreditCustomerAccount = 3,//53459
    }

    [Flags]
    public enum JournalVoucherEntryType
    {
        [Description("G/L Account")]
        GLAccount = 1,
        [Description("Customer")]
        Customer = 2
    }

    [Flags]
    public enum JournalVoucherStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Verified")]
        Audited = 8,
    }

    [Flags]
    public enum JournalVoucherEntryStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
    }

    [Flags]
    public enum GeneralLedgerStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Verified")]
        Audited = 8,
    }

    [Flags]
    public enum GeneralLedgerEntryStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
    }

    [Flags]
    public enum GeneralLedgerEntryType
    {
        [Description("G/L Account")]
        GLAccount = 1,
        [Description("Customer")]
        Customer = 2
    }

    [Flags]
    public enum PurchaseInvoiceEntryType
    {
        [Description("G/L Account")]
        GLAccount = 1,

        [Description("Fixed Asset")]
        FixedAsset = 2,
        
        [Description("Items")]
        Items = 3
    }

    [Flags]
    public enum CustomerAccountComponent
    {
        [Description("Principal")]
        Principal = 1,
        [Description("Interest (Receivable)")]
        InterestReceivable = 2,
        [Description("Interest (Received)")]
        InterestReceived = 4,
        [Description("Interest (Charged)")]
        InterestCharged = 8,
    }

    [Flags]
    public enum CustomerTransactionAuthOption
    {
        [Description("Authorize")]
        Authorize = 1,
        [Description("Reject")]
        Reject = 2,
    }

    public enum SuperSaverPayableStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Verified")]
        Verified = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    public enum OriginationVerificationAuthorizationStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Verified")]
        Verified = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    [Flags]
    public enum VerificationOption
    {
        [Description("Verify")]
        Verified = 1,
        [Description("Reject")]
        Rejected = 2,
    }

    [Flags]
    public enum AuthorizationOption
    {
        [Description("Post")]
        Posted = 1,
        [Description("Reject")]
        Rejected = 2,
    }

    public enum EmployeeExitStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Verified")]
        Verified = 1,
        [Description("Accepted")]
        Accepted = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    [Flags]
    public enum EmployeeExitApprovalOption
    {
        [Description("Accept")]
        Accepted = 1,
        [Description("Reject")]
        Rejected = 2,
    }

    [Flags]
    public enum FuneralRiderClaimPaymentStatus
    {
        [Description("Unpaid")]
        Unpaid = 1,
        [Description("Paid")]
        Paid = 2,
    }

    [Flags]
    public enum FuneralRiderClaimType
    {
        [Description("Member Claim")]
        MemberClaim = 1,
        [Description("Spouse Claim")]
        SpouseClaim = 2,
    }

    [Flags]
    public enum SuperSaverPayableAuditOption
    {
        [Description("Verify")]
        Verified = 1,
        [Description("Reject")]
        Rejected = 2,
    }

    [Flags]
    public enum SuperSaverPayableAuthOption
    {
        [Description("Post")]
        Posted = 1,
        [Description("Reject")]
        Rejected = 2,
    }

    [Flags]
    public enum ProcurementAuditOption
    {
        [Description("Verify")]
        Verified = 1,
        [Description("Reject")]
        Rejected = 2,
    }

    [Flags]
    public enum ProcurementAuthOption
    {
        [Description("Post")]
        Posted = 1,
        [Description("Reject")]
        Rejected = 2,
    }

    [Flags]
    public enum JournalVoucherAuthOption
    {
        [Description("Post")]
        Post = 1,
        [Description("Reject")]
        Reject = 2,
    }

    [Flags]
    public enum GeneralLedgerAuthOption
    {
        [Description("Post")]
        Post = 1,
        [Description("Reject")]
        Reject = 2,
    }

    [Flags]
    public enum TreasuryTransactionType
    {
        [Description("Treasury to Treasury")]
        TreasuryToTreasury = 1,
        [Description("Treasury to Bank")]
        TreasuryToBank = 2,
        [Description("Bank to Treasury")]
        BankToTreasury = 4,
        [Description("Treasury to Teller")]
        TreasuryToTeller = 8,
        [Description("Teller to Treasury")]
        TellerToTreasury = 16,
        [Description("Teller Cash Transfer")]
        TellerCashTransfer = 32,
    }

    [Flags]
    public enum CashTransferTransactionType
    {
        [Description("Teller To Treasury")]
        TellerToTreasury = 1,
        [Description("Teller To Teller")]
        TellerToTeller = 2
    }

    [Flags]
    public enum GeneralTransactionType
    {
        [Description("Cash Receipt")]
        CashReceipt = 1,
        [Description("Cheque Receipt")]
        ChequeReceipt = 2,
        [Description("Cash Payment")]
        CashPayment = 4,
        [Description("Cash Pickup")]
        CashPickup = 8,
        [Description("Sundry Payment")]
        SundryPayment = 16,
        [Description("Cash Payment (Account Closure)")]
        CashPaymentAccountClosure = 32,
        [Description("Funeral Rider Claim")]
        FuneralRiderClaim = 64,
    }

    public enum TellerCashBalanceStatus
    {
        [Description("Balanced")]
        Balanced = 0x5000,
        [Description("Shortage")]
        Shortage,
        [Description("Excess")]
        Excess
    }

    public enum CreditBatchType
    {
        [Description("Payout")]
        Payout = 0xDADA,
        [Description("Check-Off")]
        CheckOff = 0xDADA + 1,
        [Description("Cash Pickup")]
        CashPickup = 0xDADA + 2,
        [Description("Sundry Payments")]
        SundryPayments = 0xDADA + 3,
    }

    [Flags]
    public enum BatchStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Verified")]
        Audited = 8,
    }

    [Flags]
    public enum BatchEntryStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
    }

    [Flags]
    public enum BatchAuthOption
    {
        [Description("Post")]
        Post = 1,
        [Description("Reject")]
        Reject = 2,
    }

    public enum CheckOffEntryType
    {
        [Description("sInterest")]
        sInterest,
        [Description("sInvest")]
        sInvest,
        [Description("sLoan")]
        sLoan,
        [Description("sRisk")]
        sRisk,
        [Description("sShare")]
        sShare,
        [Description("wCont")]
        wCont,
        [Description("sLoanInterest")]
        sLoanInterest,
        [Description("wLoan")]
        wLoan,
    }

    [Flags]
    public enum InHouseChequeFunding
    {
        [Description("Debit Customer Account")]
        DebitCustomerAccount = 1,
        [Description("Debit G/L Account")]
        DebitGeneralLedgerAccount = 2,
    }

    [Flags]
    public enum FixedDepositStatus
    {
        [Description("Running")]
        Running = 1,
        [Description("Paid")]
        Paid = 2,
        [Description("Revoked")]
        Revoked = 4,
        [Description("New")]
        New = 8,
        [Description("Rejected")]
        Rejected = 16
    }

    [Flags]
    public enum FixedDepositPayOption
    {
        [Description("Pay")]
        Pay = 1,
        [Description("Revoke")]
        Revoke = 2,
    }

    [Flags]
    public enum FixedDepositAuthOption
    {
        [Description("Post")]
        Post = 1,
        [Description("Reject")]
        Reject = 2,
    }

    [Flags]
    public enum AlternateChannelType
    {
        [Description("Sacco Link")]
        SaccoLink = 1,
        [Description("Sparrow")]
        Sparrow = 2,
        [Description("MCo-op Cash")]
        MCoopCash = 4,
        [Description("SpotCash")]
        SpotCash = 8,
        [Description("Citius")]
        Citius = 16,
        [Description("Agency Banking")]
        AgencyBanking = 32,
        [Description("PesaPepe")]
        PesaPepe = 64,
        [Description("ABC Bank")]
        AbcBank = 128,
        [Description("Broker")]
        Broker = 256,
    }

    public enum AlternateChannelFilter
    {
        [Description("Primary Account Number")]
        PrimaryAccountNumber = 0,
        [Description("Serial Number")]
        CustomerSerialNumber,
        [Description("Personal Identification #")]
        CustomerPersonalIdentificationNumber,
        [Description("First Name")]
        CustomerFirstName,
        [Description("Last Name")]
        CustomerLastName,
        [Description("Identity Card #")]
        CustomerIdentityCardNumber,
        [Description("Payroll Numbers")]
        CustomerPayrollNumbers,
        [Description("Org. Name")]
        CustomerNonIndividual_Description,
        [Description("Org. Registration #")]
        CustomerNonIndividual_RegistrationNumber,
        [Description("Address Line 1")]
        CustomerAddressLine1,
        [Description("Address Line 2")]
        CustomerAddressLine2,
        [Description("Street")]
        CustomerStreet,
        [Description("Postal Code")]
        CustomerPostalCode,
        [Description("City")]
        CustomerCity,
        [Description("Email")]
        CustomerEmail,
        [Description("Land Line")]
        CustomerLandLine,
        [Description("Mobile Line")]
        CustomerMobileLine,
        [Description("Account Number")]
        CustomerReference1,
        [Description("Membership Number")]
        CustomerReference2,
        [Description("Personal File Number")]
        CustomerReference3,
    }

    [Flags]
    public enum SetDifferenceMode
    {
        [Description("RRN exists in File but not System")]
        RRNExistsInFileButNotSystem = 1,
        [Description("RRN exists in System but not File")]
        RRNExistsInSystemButNotFile = 2,
        [Description("STAN exists in File but not System")]
        STANExistsInFileButNotSystem = 4,
        [Description("STAN exists in System but not File")]
        STANExistsInSystemButNotFile = 8,
        [Description("Callback Payload exists in File but not System")]
        CallbackPayloadExistsInFileButNotSystem = 16,
        [Description("Callback Payload exists in System but not File")]
        CallbackPayloadExistsInSystemButNotFile = 32,
    }

    [Flags]
    public enum DLRStatus
    {
        [Description("UnKnown")]
        UnKnown = 1,
        [Description("Failed")]
        Failed = 2,
        [Description("Pending")]
        Pending = 4,
        [Description("Delivered")]
        Delivered = 8,
        [Description("Not Applicable")]
        NotApplicable = 16,
        [Description("Submitted")]
        Submitted = 32
    }

    [Flags]
    public enum MessageCategory
    {
        [Description("SMS Alert")]
        SMSAlert = 1,
        [Description("USSD Query")]
        USSDQuery = 2,
        [Description("E-mail Alert")]
        EmailAlert = 4,
        [Description("Plugin Alert")]
        PluginAlert = 8,
        [Description("Credit Batch Entry")]
        CreditBatchEntry = 16,
        [Description("Guarantor Release Candidate")]
        GuarantorReleaseCandidate = 32,
        [Description("Recurring Batch Entry")]
        RecurringBatchEntry = 64,
        [Description("SkyWorld Registration")]
        SkyWorldRegistration = 128,
        [Description("Loan Disbursement Batch Entry")]
        LoanDisbursementBatchEntry = 256,
        [Description("Interest Capitalization Candidate")]
        InterestCapitalizationCandidate = 512,
        [Description("Debit Batch Entry")]
        DebitBatchEntry = 1024,
        [Description("Mobile To Bank")]
        MobileToBank = 2048,
        [Description("SkyWorld Response")]
        SkyWorldResponse = 4096,
        [Description("Bank-to-Mobile Registration")]
        BankToMobileRegistration = 8192,
        [Description("Bank To Mobile")]
        BankToMobile = 16384,
        [Description("Agency Banking Settlement")]
        AgencyBankingSettlement = 32768,
        [Description("Audit Log")]
        AuditLog = 65536,
        [Description("Wire Transfer Batch Entry")]
        WireTransferBatchEntry = 131072,
        [Description("Account Alert")]
        AccountAlert = 262144,
        [Description("Pay Slip")]
        PaySlip = 524288,
        [Description("Workflow")]
        Workflow = 1048576,
        [Description("Audit Trail")]
        AuditTrail = 2097152,
        [Description("Journal Reversal Batch Entry")]
        JournalReversalBatchEntry = 4194304,
        [Description("Bank To Mobile IPN")]
        BankToMobileIPN = 8388608,
        [Description("Broker Registration")]
        BrokerRegistration = 16777216,
        [Description("Broker IPN")]
        BrokerIPN = 33554432,
        [Description("Broker")]
        Broker = 67108864,
        [Description("Population Register Query")]
        PopulationRegisterQuery = 134217728
    }

    [Flags]
    public enum MessageOrigin
    {
        [Description("Internal")]
        Within = 1,
        [Description("External")]
        Without = 2,
        [Description("Other")]
        Other = 4
    }

    public enum Nationality
    {
        [Description("")]
        Unknown,
        [Description("Kenya")]
        Kenya,
        [Description("Uganda")]
        Uganda,
        [Description("Tanzania")]
        Tanzania,
        [Description("Rwanda")]
        Rwanda,
        [Description("Burundi")]
        Burundi,
        [Description("Sudan")]
        Sudan,
        [Description("South Sudan")]
        SouthSudan,
        [Description("Malawi")]
        Malawi,
        [Description("Zimbabwe")]
        Zimbabwe,
        [Description("Zambia")]
        Zambia,
        [Description("Somalia")]
        Somalia,
        [Description("Djibouti")]
        Djibouti,
        [Description("Ethiopia")]
        Ethiopia,
    }

    public enum BloodGroup
    {
        [Description("")]
        Unknown,
        [Description("A-")]
        ANegative,
        [Description("A+")]
        APositive,
        [Description("B-")]
        BNegative,
        [Description("B+")]
        BPositive,
        [Description("AB-")]
        ABNegative,
        [Description("AB+")]
        ABPositive,
        [Description("O-")]
        ONegative,
        [Description("O+")]
        OPositive,
    }

    [Flags]
    public enum EmployeeCategory
    {
        [Description("Full-Time")]
        FullTime = 1,
        [Description("Part-Time")]
        PartTime = 2,
        [Description("Contract")]
        Contract = 4
    }

    [Flags]
    public enum EmployeeExitType
    {
        [Description("Resignation")]
        Resignation = 1,
        [Description("Dismissal")]
        Dismissal = 2,
        [Description("Retirement")]
        Retirement = 3,
        [Description("Retrenchment")]
        Retrenchment = 4
    }

    public enum RoundingType
    {
        [Description("No Rounding")]
        NoRounding = 0,
        [Description("Midpoint To Even")]
        ToEven = 1 /*When a number is halfway between two others, it is rounded toward the nearest even number*/,
        [Description("Midpoint Away From Zero")]
        AwayFromZero = 2 /*When a number is halfway between two others, it is rounded toward the nearest number that is away from zero*/,
        [Description("Round Up")]
        Ceiling = 3 /*Returns the smallest integral value that is greater than or equal to the specified decimal number.*/,
        [Description("Round Down")]
        Floor = 4 /*Returns the largest integer less than or equal to the specified decimal number.*/,
    }

    [Flags]
    public enum MessageGroupTarget
    {
        [Description("Employer")]
        Employer = 1,
        [Description("Division")]
        Division = 2,
        [Description("Zone")]
        Zone = 4,
        [Description("Station")]
        Station = 8,
        [Description("Members")]
        Members = 16,
        [Description("Custom")]
        Custom = 32,
        [Description("Role")]
        Role = 64,
    }

    public enum ReportTemplateCategory
    {
        [Description("Header Entry")]
        HeaderEntry = 0x1000,/*4096*/
        [Description("Detail Entry")]
        DetailEntry = 0x1000 + 1/*4097*/
    }

    public enum InventoryCategoryType
    {
        [Description("Header Entry")]
        HeaderEntry = 0,
        [Description("Detail Entry")]
        DetailEntry = 1
    }

    public enum EmployeeAppraisalTargetType
    {
        [Description("Header Entry")]
        HeaderEntry = 0,
        [Description("Detail Entry")]
        DetailEntry = 1
    }

    public enum EmployeeAppraisalTargetAnswerType
    {
        [Description("Text")]
        Text = 0,
        [Description("Number")]
        Number = 2
    }

    [Flags]
    public enum ItemRegisterType
    {
        [Description("Asset")]
        Asset = 1,
        [Description("Inventory")]
        Inventory = 2
    }

    public enum AssetRegisterStatus
    {
        [Description("Un-Alloted")]
        UnAlloted = 0,
        [Description("Alloted")]
        Alloted = 2,
        [Description("Returned")]
        Returned = 4,
        [Description("Disposed")]
        Disposed = 6
    }

    public enum AssetDisposalMethod
    {
        [Description("Simple-Disposal (No proceeds)")]
        SimpleDisposal = 1,
        [Description("Disposal With Loss")]
        DisposalWithLoss = 2,
        [Description("Disposal With Gain")]
        DisposalWithGain = 4
    }

    public enum AssetConditionScale
    {
        [Description("Excellent")]
        Excellent = 5,
        [Description("Good")]
        Good = 4,
        [Description("Fair")]
        Fair = 3,
        [Description("Poor")]
        Poor = 2,
        [Description("Critical")]
        Critical = 1,
        [Description("Absent")]
        Absent = 0
    }

    public enum AssetMovementType
    {
        [Description("Dispatch")]
        Dispatch = 1,
        [Description("Transfer")]
        Transfer = 2,
        [Description("Disposal")]
        Disposal = 3
    }


    public enum WorkflowRecordStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    //public enum WorkflowApprovalOption
    //{
    //    [Description("Approved")]
    //    Approved = 0,
    //    [Description("Rejected")]
    //    Rejected = 1,
    //}
    public enum WorkflowApprovalOption
    {
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 1,
    }
    public enum WorkflowMatchedStatus
    {
        [Description("Not Matched")]
        NotMatched = 0,
        [Description("Matched")]
        Matched = 1,
    }

    public enum PaymentFrequencyPerYear
    {
        [Description("Annual")]
        Annual = 1,
        /// <summary>
        /// Semi-Annual (every 6 months)
        /// </summary>
        [Description("Semi-Annual (every 6 months)")]
        SemiAnnual = 2,
        /// <summary>
        /// Quarterly (every 3 months)
        /// </summary>
        [Description("Quarterly (every 3 months)")]
        Quarterly = 3,
        /// <summary>
        /// Tri-Annual (every 4 months)
        /// </summary>
        [Description("Tri-Annual (every 4 months)")]
        TriAnnual = 4,
        /// <summary>
        /// Bi-Monthly (every 2 months)
        /// </summary>
        [Description("Bi-Monthly (every 2 months)")]
        BiMonthly = 6,
        [Description("Monthly")]
        Monthly = 12,
        /// <summary>
        /// Semi-Monthly (twice a month)
        /// </summary>
        [Description("Semi-Monthly (twice a month)")]
        SemiMonthly = 24,
        /// <summary>
        /// Bi-Weekly (every 2 weeks)
        /// </summary>
        [Description("Bi-Weekly (every 2 weeks)")]
        BiWeekly = 26,
        [Description("Weekly")]
        Weekly = 52,
        [Description("Daily")]
        Daily = 365
    }

    public enum PaymentDueDate
    {
        [Description("End of Period")] //Falls at the end of the date interval
        EndOfPeriod = 0,
        [Description("Beginning of Period")] //Falls at the beginning of the date interval
        BegOfPeriod = 1,
    }

    public enum WithdrawalNotificationCategory
    {
        [Description("Deceased")]
        Deceased = 0x700,
        [Description("Voluntary")]
        Voluntary = 0x700 + 1,
        [Description("Retiree")]
        Retiree = 0x700 + 2,
    }

    [Flags]
    public enum WithdrawalNotificationStatus
    {
        [Description("Registered")]
        Registered = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Verified")]
        Audited = 4,
        [Description("Deferred")]
        Deferred = 16,
        [Description("Withdrawal Settled")]
        WithdrawalSettled = 8,
        [Description("Death Claim Settled")]
        DeathClaimSettled = 32
    }

    [Flags]
    public enum MembershipWithdrawalApprovalOption
    {
        [Description("Approve")]
        Approve = 1,
        [Description("Defer")]
        Defer = 2,
    }

    [Flags]
    public enum MembershipWithdrawalAuditOption
    {
        [Description("Verify")]
        Audit = 1,
        [Description("Defer")]
        Defer = 2,
    }

    [Flags]
    public enum MembershipWithdrawalSettlementOption
    {
        [Description("Settle")]
        Settle = 1,
        [Description("Defer")]
        Defer = 2,
    }

    [Flags]
    public enum MembershipWithdrawalSettlementType
    {
        [Description("Normal")]
        Normal = 1,
        [Description("Express")]
        Express = 2,
        [Description("Waiver")]
        Waiver = 4,
    }

    public enum ImageType
    {
        [Description("Passport")]
        Passport = 1,
        [Description("Signature")]
        Signature,
        [Description("Identity Card (Front)")]
        IdentityCardFrontSide,
        [Description("Identity Card (Back)")]
        IdentityCardBackSide
    }

    public enum ImageSource
    {
        [Description("File System")]
        FileSystem = 1,
        [Description("Scanner/Camera")]
        WindowsImageAcquisition,
        [Description("Webcam")]
        Webcam
    }

    public enum CustomerType
    {
        [Description("Individual")]
        Individual = 0x0000,
        [Description("Partnership")]
        Partnership = 0x0000 + 1,
        [Description("Corporation")]
        Corporation = 0x0000 + 2,
        [Description("Microcredit")]
        MicroCredit = 0x0000 + 3,
    }

    public enum CustomerFilter
    {
        [Description("Serial Number")]
        SerialNumber = 0,
        [Description("Personal Identification #")]
        PersonalIdentificationNumber,
        [Description("First Name")]
        FirstName,
        [Description("Last Name")]
        LastName,
        [Description("Identity Card #")]
        IdentityCardNumber,
        [Description("Payroll Numbers")]
        PayrollNumbers,
        [Description("Org. Name")]
        NonIndividual_Description,
        [Description("Org. Registration #")]
        NonIndividual_RegistrationNumber,
        [Description("Address Line 1")]
        AddressLine1,
        [Description("Address Line 2")]
        AddressLine2,
        [Description("Street")]
        Street,
        [Description("Postal Code")]
        PostalCode,
        [Description("City")]
        City,
        [Description("Email")]
        Email,
        [Description("Land Line")]
        LandLine,
        [Description("Mobile Line")]
        MobileLine,
        [Description("Account Number")]
        Reference1,
        [Description("Membership Number")]
        Reference2,
        [Description("Personal File Number")]
        Reference3,
    }


    public enum IndividualType
    {
        [Description("Adult")]
        Adult = 0x0000,
        [Description("Minor")]
        Minor = 0x0000 + 1
    }

    public enum TellerType
    {
        [Description("Employee")]
        Employee = 0,
        [Description("ATM Terminal")]
        AutomatedTellerMachine = 1,
        [Description("POS Terminal (In-house)")]
        InhousePointOfSale = 2,
        [Description("POS Terminal (Agent)")]
        AgentPointOfSale = 3,
    }

    public enum StandingOrderTrigger
    {
        [Description("Payout")]
        Payout = 0,
        [Description("Check-Off")]
        CheckOff = 1,
        [Description("Schedule")]
        Schedule = 2,
        [Description("Sweep")]
        Sweep = 3,
        [Description("Microloan")]
        Microloan = 4,
    }

    public enum ScheduleFrequency
    {
        [Description("Annual")]
        Annual = 1,
        [Description("Semi-Annual (every 6 months)")]
        SemiAnnual = 2,
        [Description("Quarterly (every 3 months)")]
        Quarterly = 3,
        [Description("Tri-Annual (every 4 months)")]
        TriAnnual = 4,
        [Description("Bi-Monthly (every 2 months)")]
        BiMonthly = 6,
        [Description("Monthly")]
        Monthly = 12,
        [Description("Semi-Monthly (twice a month)")]
        SemiMonthly = 24,
        [Description("Bi-Weekly (every 2 weeks)")]
        BiWeekly = 26,
        [Description("Weekly")]
        Weekly = 52,
        [Description("Daily")]
        Daily = 365
    }

    public enum StandingOrderCustomerAccountFilter
    {
        [Description("Beneficiary")]
        Beneficiary = 0,
        [Description("Benefactor")]
        Benefactor = 1
    }

    [Flags]
    public enum DataAttachmentPeriodStatus
    {
        [Description("Open")]
        Open = 1,
        [Description("Closed")]
        Closed = 2,
        [Description("Suspended")]
        Suspended = 4
    }

    public enum DataAttachmentTransactionType
    {
        [Description("Fresh Loan")]
        FreshLoan = 1,
        [Description("Adjust Balance")]
        Adjustment = 2,
        [Description("Variation")]
        Contribution = 3,
        [Description("New Member")]
        BalanceAdjustment = 4,
        [Description("Special Adjustments")]
        SpecialAdjustment = 5,
        [Description("Stop Deduction")]
        StopDeduction = 6,
        [Description("Shares Deposit")]
        SharesDeposit = 7,
        [Description("Risk Fund")]
        RiskFund = 8,
        [Description("Entrance Fee")]
        EntranceFee = 9
    }

    [Flags]
    public enum InterestCapitalizationFilter
    {
        [Description("Employer")]
        Employer = 1,
        [Description("Customer")]
        Customer = 2,
        [Description("Credit Type")]
        CreditType = 4,
    }

    [Flags]
    public enum StandingOrderExecutionFilter
    {
        [Description("Employer")]
        Employer = 1,
        [Description("Customer")]
        Customer = 2,
        [Description("Credit Type")]
        CreditType = 4,
    }

    [Flags]
    public enum ElectronicJournalStatus
    {
        [Description("Open")]
        Open = 1,
        [Description("Closed")]
        Closed = 2,
    }

    [Flags]
    public enum TruncatedChequeStatus
    {
        [Description("New")]
        New = 1,
        [Description("Processed")]
        Processed = 2,
    }

    [Flags]
    public enum TruncatedChequeProcessingOption
    {
        [Description("Pay")]
        Pay = 1,
        [Description("UnPay")]
        UnPay = 2,
    }

    public enum TruncatedChequeReturnCode
    {
        [Description("Transaction being Presented")]
        One = 0,
        [Description("MICR Code line and Image Details Defer")]
        Four = 4,
        [Description("Payment Amount and Image Amount Details Defer")]
        Five = 5,
        [Description("Wrongly Delivered")]
        Ten = 10,
        [Description("Undersize Image / Image below min Comp image size")]
        Eleven = 11,
        [Description("Excessive Image Skew")]
        Twelve = 12,
        [Description("Piggy-Back Image")]
        Thirteen = 13,
        [Description("Oversize Image / Image above max comp image size")]
        Fourteen = 14,
        [Description("Horizontal streaks in Image")]
        Fifteen = 15,
        [Description("Time Barred")]
        Twenty = 20,
        [Description("Cheque Unpaid because of suspected Criminal Activity")]
        Thirty = 30,
        [Description("Date expired - Cheque stale")]
        ThirtyOne = 31,
        [Description("Cheque Post-dated")]
        ThirtyTwo = 32,
        [Description("Date Irregular")]
        ThirtyThree = 33,
        [Description("Payee Name Incomplete")]
        ThirtySeven = 37,
    }

    public enum LoanGuarantorStatus
    {
        [Description("Attached")]
        Attached = 0x0000,
        [Description("Released")]
        Released = 0x0001
    }

    public enum LoanGuarantorAttachmentHistoryStatus
    {
        [Description("Attached")]
        Attached = 0x0000,
        [Description("Relieved")]
        Relieved = 0x0001
    }

    [Flags]
    public enum ExternalChequeClearanceOption
    {
        [Description("Pay")]
        Pay = 1,
        [Description("UnPay")]
        UnPay = 2
    }

    [Flags]
    public enum MicroCreditGroupType
    {
        [Description("ROSCA")]
        ROSCA = 1,
        [Description("ASCA")]
        ASCA = 2,
        [Description("Table Banking")]
        TableBanking = 4,
    }

    [Flags]
    public enum MicroCreditGroupMemberDesignation
    {
        [Description("Chairperson")]
        Chairperson = 1,
        [Description("Deputy Chairperson")]
        DeputyChairperson = 2,
        [Description("Secretary")]
        Secretary = 4,
        [Description("Deputy Secretary")]
        DeputySecretary = 8,
        [Description("Treasurer")]
        Treasurer = 16,
        [Description("Ordinary Member")]
        OrdinaryMember = 32,
    }

    public enum MicroCreditGroupMeetingFrequency
    {
        [Description("Semi-Annual (every 6 months)")]
        SemiAnnual = 2,
        [Description("Tri-Annual (every 4 months)")]
        TriAnnual = 3,
        [Description("Quarterly (every 3 months)")]
        Quartery = 4,
        [Description("Bi-Monthly (every 2 months)")]
        BiMonthly = 6,
        [Description("Monthly")]
        Monthly = 12,
        [Description("Semi-Monthly (twice a month)")]
        SemiMonthly = 24,
        [Description("Bi-Weekly (every 2 weeks)")]
        BiWeekly = 26,
        [Description("Weekly")]
        Weekly = 52,
    }

    public enum MicroCreditGroupMeetingDayOfWeek
    {
        [Description("Sunday")]
        Sunday = 0,
        [Description("Monday")]
        Monday = 1,
        [Description("Tuesday")]
        Tuesday = 2,
        [Description("Wednesday")]
        Wednesday = 3,
        [Description("Thursday")]
        Thursday = 4,
        [Description("Friday")]
        Friday = 5,
        [Description("Saturday")]
        Saturday = 6,
    }

    [Flags]
    public enum DepreciationMethod
    {
        [Description("Straight Line")]
        SLN = 1,
        [Description("Sum-Of-Years' Digits")]
        SYD = 2,
        [Description("Fixed-Declining Balance")]
        DB = 4,
        [Description("Double-Declining Balance")]
        DDB = 8,
        [Description("Variable Double-Declining Balance")]
        VDB = 16
    }

    public enum SignatoryRelationship
    {
        [Description("")]
        Unknown,
        [Description("Father")]
        Father = 0xDEBE + 1,
        [Description("Mother")]
        Mother = 0xDEBE + 2,
        [Description("Brother")]
        Brother = 0xDEBE + 3,
        [Description("Sister")]
        Sister = 0xDEBE + 4,
        [Description("Wife")]
        Wife = 0xDEBE + 5,
        [Description("Husband")]
        Husband = 0xDEBE + 6,
        [Description("Son")]
        Son = 0xDEBE + 7,
        [Description("Daughter")]
        Daughter = 0xDEBE + 8,
    }

    public enum ChequeBookType
    {
        [Description("In-House")]
        InHouse = 0,
        [Description("External")]
        External = 1
    }

    public enum PaymentVoucherStatus
    {
        [Description("Active")]
        Active = 0,
        [Description("Paid")]
        Paid = 1,
        [Description("Flagged")]
        Flagged = 2,
    }

    //public enum PaymentType
    //{
    //    [Description("PurchaseInvoice")]
    //    Invoice = 0,
    //    [Description("PurchaseCreditMemo")]
    //    Other = 1,
       
    //}

    public enum PaymentVoucherManagementAction
    {
        [Description("Flag")]
        Flag = 0,
        [Description("Unflag")]
        Unflag = 1,
    }

    public enum CustomerAccountStatus
    {
        [Description("Normal")]
        Normal = 0,
        [Description("Inactive")]
        Inactive = 1,
        [Description("Dormant")]
        Dormant = 2,
        [Description("Closed")]
        Closed = 3,
        [Description("Remarked")]
        Remarked = 4,
    }

    public enum WireTransferBatchType
    {
        [Description("MPESA B2C")]
        MPESAB2C,
        [Description("MPESA B2B")]
        MPESAB2B,
        [Description("EFT")]
        EFT,
    }

    public enum SystemTransactionCode
    {
        [Description("Unclassified")]
        Unclassified = 0,
        [Description("Cash Withdrawal (Customer)")]
        CashWithdrawal = 1,
        [Description("Cash Deposit (Customer)")]
        CashDeposit = 2,
        [Description("Cheque Deposit (Customer)")]
        ChequeDeposit = 3,
        [Description("Payment Voucher")]
        CashWithdrawalPaymentVoucher = 4,
        [Description("Journal Voucher")]
        JournalVoucher = 5,
        [Description("Payout Batch")]
        CreditBatchPayout = 6,
        [Description("Bank to Treasury")]
        BankToTreasury = 7,
        [Description("Treasury to Bank")]
        TreasuryToBank = 8,
        [Description("Treasury to Teller")]
        TreasuryToTeller = 9,
        [Description("Treasury to Treasury")]
        TreasuryToTreasury = 10,
        [Description("Sacco-Link")]
        SaccoLink = 11,
        [Description("Sparrow")]
        Sparrow = 12,
        [Description("MCo-op Cash")]
        MCoopCashWallet = 13,
        [Description("Inter-Acccount Transfer")]
        InterAcccountTransfer = 14,
        [Description("Fiscal Period Closing")]
        FiscalPeriodClosing = 15,
        [Description("SkyWorld")]
        SkyWorldMobileWallet = 16,
        [Description("Cash Receipt (Customer)")]
        BackOfficeCashReceipt = 17,
        [Description("Cash Receipt (Sundry)")]
        GeneralCashReceipt = 18,
        [Description("Cheque Receipt (Sundry)")]
        GeneralChequeReceipt = 19,
        [Description("Cash Payment (Sundry)")]
        GeneralCashPayment = 20,
        [Description("Loan Disbursement")]
        LoanDisbursement = 21,
        [Description("External Cheque (Clearance)")]
        ExternalChequeClearance = 22,
        [Description("Loan Restructuring")]
        LoanRestructuring = 23,
        [Description("Interest Capitalization")]
        InterestCapitalization = 24,
        [Description("SpotCash")]
        SpotCash = 25,
        [Description("Fixed Deposit")]
        FixedDeposit = 26,
        [Description("Salary")]
        SalaryProcessing = 27,
        [Description("Check-Off Batch")]
        CreditBatchCheckOff = 28,
        [Description("Cash Pickup Batch")]
        CreditBatchCashPickup = 29,
        [Description("Standing Order")]
        StandingOrder = 30,
        [Description("Savings Charges")]
        SavingsLedgerFee = 31,
        [Description("Loan Charges")]
        LoanLedgerFee = 32,
        [Description("Truncated Cheque")]
        TruncatedCheque = 33,
        [Description("Legacy Balance")]
        LegacyBalance = 34,
        [Description("Statement Fee")]
        StatementFee = 35,
        [Description("Archive Balance")]
        ArchiveBalance = 36,
        [Description("Refund Batch")]
        OverDeductionBatch = 37,
        [Description("Debit Batch")]
        DebitBatch = 38,
        [Description("Balance Adjustment")]
        InvestmentBalancesAdjustment = 39,
        [Description("Bank Reconciliation")]
        BankReconciliation = 40,
        [Description("Teller End-Of-Day")]
        TellerEndOfDay = 41,
        [Description("Teller Cash Transfer")]
        TellerCashTransfer = 42,
        [Description("Sundry Payment Batch")]
        CreditBatchSundryPayment = 43,
        [Description("Expense Payables/Receivables")]
        ExpensePayables = 44,
        [Description("Intra-Acccount Transfer")]
        IntraAcccountTransfer = 45,
        [Description("Membership Termination")]
        MembershipTermination = 46,
        [Description("Loan Guarantee")]
        LoanGuarantee = 47,
        [Description("Guarantor Attachment")]
        GuarantorAttachment = 48,
        [Description("Guarantor Substitution")]
        GuarantorSubstitution = 49,
        [Description("Loan Offsetting")]
        LoanOffsetting = 50,
        [Description("Agency Banking")]
        AgencyBanking = 51,
        [Description("Mobile To Bank")]
        MobileToBank = 52,
        [Description("In-House Cheque")]
        InHouseCheque = 53,
        [Description("PesaPepe")]
        PesaPepe = 54,
        [Description("Guarantor Relieving")]
        GuarantorRelieving = 55,
        [Description("Balance Pooling")]
        InvestmentBalancesPooling = 56,
        [Description("PesaPepe Microloan")]
        PesaPepeMicroloan = 57,
        [Description("General Ledger")]
        GeneralLedger = 58,
        [Description("ABC Bank")]
        AbcBank = 59,
        [Description("Cash Withdrawal (Account Closure)")]
        CashWithdrawalAccountClosure = 60,
        [Description("Wire Transfer Batch")]
        WireTransferBatch = 61,
        [Description("Citius")]
        Citius = 62,
        [Description("e-Statement")]
        ElectronicStatementOrder = 63,
        [Description("Loan Request")]
        LoanRequest = 64,
        [Description("Purchase Payables")]
        PurchasePayables = 65,
        [Description("Asset Disposals")]
        AssetDisposals = 66,
        [Description("Mobile To Bank (Sender ACK)")]
        MobileToBankSenderAcknowledgement = 67,
        [Description("Super Saver Payables")]
        SuperSaverPayables = 68,
        [Description("Funeral Rider Claim")]
        FuneralRiderClaim = 69,
        [Description("Account Activation")]
        AccountActivation = 70,
        [Description("Asset Depreciations")]
        AssetDepreciation = 71,
        [Description("Membership Approval")]
        MembershipApproval = 72,
        [Description("Account Closure")]
        AccountClosure = 73,
        [Description("Account Freezing")]
        AccountFreezing = 74,
        [Description("Account Dormant")]
        AccountDormant = 75,
        [Description("Loan Deferred")]
        LoanDeffered = 76,
        [Description("Alternate Channel Linking")]
        AlternateChannelLinking = 77,
        [Description("Alternate Channel Replacement")]
        AlternateChannelReplacement = 78,
        [Description("Alternate Channel Renewal")]
        AlternateChannelRenewal = 79,
        [Description("Alternate Channel Delinking")]
        AlternateChannelDelinking = 80,
        [Description("Alternate Channel Stoppage")]
        AlternateChannelStoppage = 81,
        [Description("Leave Approval")]
        LeaveApproval = 82,
        [Description("Customer Details Editing")]
        CustomerDetailsEditing = 83,
        [Description("Interest Adjustment")]
        InterestAdjustment = 84,
        [Description("External Cheque (Transfer)")]
        ExternalChequeTransfer = 85,
        [Description("External Cheque (Banking)")]
        ExternalChequeBanking = 86,

        [Description("Membership Account Registration")]
        MembershipAccountRegistration = 87,
        [Description("Membership Account Verification")]
        MembershipAccountVerification = 88,
        [Description("Membership Reset Password")]
        MembershipResetPassword = 89,
        [Description("Promotional Account Alert")]
        PromotionalAccountAlert = 90,
        [Description("Customer Registration Alert")]
        CustomerRegistration = 91,
        [Description("External Cheque (Deposit Charge)")]
        ExternalChequeDepositCharge = 92
    }

    public enum ChargeBenefactor
    {
        [Description("Customer")]
        Customer = 0,
        [Description("Institution")]
        Institution = 1,
    }

    public enum SavingsProductKnownChargeType
    {
        [Description("Cash Withdrawal Fee")]
        CashWithdrawal = 0,
        [Description("Cash Withdrawal Fee (Without Notice)")]
        CashWithdrawalWithoutNotice = 1,
        [Description("Cash Withdrawal Fee (Payment Voucher)")]
        CashWithdrawalPaymentVoucher = 2,
        [Description("Cash Withdrawal Fee (Premature Interval)")]
        CashWithdrawalPrematureInterval = 3,
        [Description("Cash Withdrawal Fee (Below Minimum Balance)")]
        CashWithdrawalBelowMinimumBalance = 4,
        [Description("Cash Withdrawal Fee (Account Closure)")]
        CashWithdrawalAccountClosure = 5,
        [Description("Cash Deposit Fee")]
        CashDeposit = 6,
        [Description("Statement Printing Fee")]
        StatementPrintingCharges = 7,
        [Description("Standing Order Fee")]
        StandingOrderFee = 8,
        [Description("Payroll Processing Fee")]
        PayrollProcessingCharges = 9,
        [Description("Cheque Book Fee")]
        ChequeBookCharges = 10,
        [Description("Dynamic Fee")]
        DynamicFee = 11,
    }

    public enum AlternateChannelKnownChargeType
    {
        [Description("Linking Fee")]
        Linking = 0,
        [Description("Replacement Fee")]
        Replacement = 1,
        [Description("Renewal Fee")]
        Renewal = 2,
        [Description("Withdrawal Fee")]
        WithdrawalCharges = 3,
        [Description("Deposit Fee")]
        DepositCharges = 4,
        [Description("Mini Statement Fee")]
        MiniStatementCharges = 5,
        [Description("Balance Inquiry Fee")]
        BalanceInquiryCharges = 6,
        [Description("Airtime Fee")]
        AirtimeCharges = 7,
        [Description("PIN Reset Fee")]
        PINResetCharges = 8,
        [Description("Sacco Link Deposit Fee (Coop-Bank Agent)")]
        DepositChargesCoopBankAgent = 9,
        [Description("Sacco Link Withdrawal Fee (Coop-Bank ATM)")]
        WithdrawalChargesCoopBankATM = 10,
        [Description("Sacco Link Withdrawal Fee (via Non-Coop-Bank ATM)")]
        WithdrawalChargesNonCoopBankATM = 11,
        [Description("Sacco Link Withdrawal Fee (via Account-To-MPESA)")]
        WithdrawalChargesCoopBankAccountToMPESA = 12,
        [Description("Sacco Link Withdrawal Fee (Coop-Bank Agent)")]
        WithdrawalChargesCoopBankAgent = 13,
        [Description("Sacco Link Purchase Fee (Goods & Services via Mobile)")]
        PurchaseChargesGoodsAndServicesCoopBankMobile = 14,
        [Description("Sacco Link Balance Inquiry Fee (Coop-Bank Agent)")]
        BalanceInquiryChargesCoopBankAgent = 15,
        [Description("Sacco Link Balance Inquiry Fee (via Mobile)")]
        BalanceInquiryChargesCoopBankMobile = 16,
        [Description("Sacco Link Mini Statement Fee (Coop-Bank Agent)")]
        MiniStatementChargesCoopBankAgent = 17,
        [Description("Sacco Link Mini Statement Fee (via Mobile)")]
        MiniStatementChargesCoopBankMobile = 18,
        [Description("Guarantorship Inquiry Fee")]
        GuarantorshipInquiryCharges = 19,
    }

    public enum LoanProductKnownChargeType
    {
        [Description("Loan Clearance Fee")]
        LoanClearanceCharges = 0xD0FA/*53498*/,
        [Description("Express Disbursement Fee")]
        ExpressLoanDisbursementFee = 0xD0FA + 1,
        [Description("Normal Disbursement Fee")]
        NormalLoanDisbursementFee = 0xD0FA + 2,
        [Description("Loan Arrears Fee")]
        LoanArrearsFee = 0xD0FA + 3,
    }

    public enum LoanProductChargeBasisValue
    {
        [Description("Principal Balance")]
        PrincipalBalance = 0,
        [Description("Book Balance")]
        BookBalance = 1,
    }

    [Flags]
    public enum TransactionDateFilter
    {
        [Description("Value Date")]
        ValueDate = 1,
        [Description("Created Date")]
        CreatedDate = 2,
    }

    public enum UnitOfMeasureBaseUnits
    {
        [Description("Oz")]
        Ounce = 0,
        [Description("Lbs")]
        Pound = 1,
        [Description("Each")]
        Each = 2,
        [Description("50-lb bag")]
        FiftyLdBag = 3,
    }

    [Flags]
    public enum AuxiliaryLoanCondition
    {
        [Description("subject to Not-Having-Outstanding-Balance")]
        SubjectToNotHavingOutstandingBalance = 1,
        [Description("subject to Having-Loan-In-Process (Approved)")]
        SubjectToHavingLoanInProcessApproved = 2,
        [Description("subject to Having-Loan-In-Process (Verified)")]
        SubjectToHavingLoanInProcessAudited = 4,
        [Description("subject to Having-Loan-In-Process (Appraised)")]
        SubjectToHavingLoanInProcessAppraised = 8,
        [Description("subject to Existing-In-Conditional-Lending-List")]
        SubjectToExistingInConditionalLendingList = 16,
        [Description("subject to Having-Dividends-Payabale")]
        SubjectToHavingDividendsPayabale = 32
    }

    [Flags]
    public enum DisbursementType
    {
        [Description("Normal")]
        Normal = 1,
        [Description("Express")]
        Express = 2,
        [Description("Waiver")]
        Waiver = 4,
    }

    [Flags]
    public enum ApportionTo
    {
        [Description("Customer Account")]
        CustomerAccount = 1,
        [Description("G/L Account")]
        GeneralLedgerAccount = 2
    }

    public enum FixedDepositCategory
    {
        [Description("Term Deposit")]
        TermDeposit = 0x0000,
        [Description("Call Deposit")]
        CallDeposit = 0x0001,
    }

    public enum FixedDepositMaturityAction
    {
        [Description("Pay Principal & Interest Due")]
        PayPrincipalAndInterestDue = 0x0000,
        [Description("Pay Interest Due & Roll-over Principal")]
        PayInterestDueAndRollOverPrincipal = 0x0001,
        [Description("Roll-over Principal & Interest Due")]
        RollOverPrincipalAndInterestDue = 0x0002,
    }

    public enum CustomerDocumentType
    {
        [Description("General Document")]
        General = 0x0000,
        [Description("Collateral Document")]
        Collateral = 0x0001,
    }

    public enum CollateralStatus
    {
        [Description("Released")]
        Released = 0x0000,
        [Description("Attached")]
        Attached = 0x0001,
    }

    public enum RecurringBatchType
    {
        [Description("Interest Capitalization")]
        InterestCapitalization = 0,
        [Description("Dynamic Savings Fees")]
        DynamicSavingsFees = 1,
        [Description("Indefinite Loan Charges")]
        IndefiniteLoanCharges = 2,
        [Description("Standing Order")]
        StandingOrder = 3,
        [Description("Investment Balances Adjustment")]
        InvestmentBalancesAdjustment = 4,
        [Description("Investment Balances Pooling")]
        InvestmentBalancesPooling = 5,
        [Description("Guarantor Releasing")]
        GuarantorReleasing = 6,
        [Description("e-Statement Order")]
        ElectronicStatementOrder = 7,
        [Description("Arrears Recovery")]
        ArrearsRecovery = 8,
        [Description("Arrears Recovery From Investment Product")]
        ArrearsRecoveryFromInvestmentProduct = 9,
    }

    public enum QueuePriority
    {
        [Description("Lowest")]
        Lowest = 0,
        [Description("Very Low")]
        VeryLow = 1,
        [Description("Low")]
        Low = 2,
        [Description("Normal")]
        Normal = 3,
        [Description("Above Normal")]
        AboveNormal = 4,
        [Description("High")]
        High = 5,
        [Description("Very High")]
        VeryHigh = 6,
        [Description("Highest")]
        Highest = 7,
    }
    public enum LeaveTypeTargetGender
    {
        [Description("")]
        Unknown = 0x000,
        [Description("Male")]
        Male = 0x000 + 1,
        [Description("Female")]
        Female = 0x000 + 2,
        [Description("Non-Binary")]
        NonBinary = 0x000 + 3,
    }

    public enum LeaveUnitTypes
    {
        [Description("")]
        Unknown = 0x000,
        [Description("Weekly")]
        Weekly = 0x000 + 1,
        [Description("Monthly")]
        Monthly = 0x000 + 2,
        [Description("Yearly")]
        Yearly = 0x000 + 3,
    }

    [Flags]
    public enum LeaveApplicationStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Recalled")]
        Recalled = 8
    }

    [Flags]
    public enum LeaveAuthOption
    {
        [Description("Approve")]
        Approve = 2,
        [Description("Reject")]
        Reject = 4,
    }

    public enum IdentityCardType
    {
        [Description("")]
        Unknown = 0,
        [Description("National ID")]
        NationalID = 1,
        [Description("Passport")]
        Passport = 2,
        [Description("Alien ID")]
        AlienID = 3,
        [Description("Birth Certificate")]
        BirthCertificate = 4,
    }

    public enum BankReconciliationAdjustmentType
    {
        [Description("Bank Account Debit")]
        BankAccountDebit = 0,
        [Description("Bank Account Credit")]
        BankAccountCredit = 1,
        [Description("G/L Account Debit")]
        GeneralLedgerAccountDebit = 2,
        [Description("G/L Account Credit")]
        GeneralLedgerAccountCredit = 3,
    }

    [Flags]
    public enum BankReconciliationPeriodStatus
    {
        [Description("Open")]
        Open = 1,
        [Description("Closed")]
        Closed = 2,
        [Description("Suspended")]
        Suspended = 4
    }

    [Flags]
    public enum BankReconciliationPeriodAuthOption
    {
        [Description("Post")]
        Post = 1,
        [Description("Reject")]
        Reject = 2,
    }

    [Flags]
    public enum AlternateChannelReconciliationPeriodStatus
    {
        [Description("Open")]
        Open = 1,
        [Description("Closed")]
        Closed = 2,
        [Description("Suspended")]
        Suspended = 4
    }

    [Flags]
    public enum AlternateChannelReconciliationPeriodAuthOption
    {
        [Description("Post")]
        Post = 1,
        [Description("Reject")]
        Reject = 2,
    }

    [Flags]
    public enum AlternateChannelReconciliationEntryStatus
    {
        [Description("Reconciled")]
        Reconciled = 1,
        [Description("Unreconciled")]
        Unreconciled = 2,
    }

    public enum AppraisalProductPurpose
    {
        [Description("Loan Recovery")]
        LoanRecovery = 1,
        [Description("Investments Qualification")]
        InvestmentsQualification = 2,
        [Description("Eligible Income Deduction")]
        EligibleIncomeDeduction = 3
    }

    public enum RecordStatus
    {
        [Description("New")]
        New = 0,
        [Description("Edited")]
        Edited = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    [Flags]
    public enum RecordAuthOption
    {
        [Description("Approve")]
        Approve = 1,
        [Description("Reject")]
        Reject = 2,
    }

    [Flags]
    public enum ExpensePayableType
    {
        [Description("Debit G/L Account")]
        DebitGLAccount = 1,
        [Description("Credit G/L Account")]
        CreditGLAccount = 2,
    }

    [Flags]
    public enum ExpensePayableStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Verified")]
        Audited = 8,
        [Description("Deferred")]
        Deferred = 16,
    }

    [Flags]
    public enum ExpensePayableEntryStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 4,
    }

    public enum ProcurementRecordStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Verified")]
        Verified = 1,
        [Description("Posted")]
        Posted = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    public enum ProcurementStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Active")]
        Active,
        [Description("Closed")]
        Closed,
    }

    public enum EmployeeDisciplinaryType
    {
        [Description("Counselling")]
        Counselling,
        [Description("Written-Warning")]
        WrittenWarning,
        [Description("Probation")]
        Probation,
        [Description("Fine")]
        Fine,
        [Description("Suspension")]
        Suspension,
        [Description("Termination")]
        Termination,
    }

    public enum InventoryDispatchStatus
    {
        [Description("Dispatched")]
        Dispatched = 1,
        [Description("Cancelled")]
        Cancelled = 2,
    }

    [Flags]
    public enum ExpensePayableAuthOption
    {
        [Description("Post")]
        Post = 1,
        [Description("Reject")]
        Reject = 2,
        [Description("Defer")]
        Defer = 4,
    }

    public enum ChequeTypeChargeRecoveryMode
    {
        [Description("On Cheque Deposit")]
        OnChequeDeposit = 0,
        [Description("On Cheque Clearance")]
        OnChequeClearance = 1,
    }

    public enum MobileToBankRequestStatus
    {
        [Description("Unmatched")]
        Unmatched = 0,
        [Description("Auto-Matched")]
        AutoMatched = 1,
        [Description("Recon-Matched")]
        ReconMatched = 2,
    }

    public enum MobileToBankRequestRecordStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Verified")]
        Verified = 2,
        [Description("Rejected")]
        Rejected = 3,
    }

    public enum MobileToBankRequestAuthOption
    {
        [Description("Verify")]
        Verify = 0,
        [Description("Reject")]
        Reject = 1,
    }

    public enum GuarantorSecurityMode
    {
        [Description("Income")]
        Income = 0,
        [Description("Investments")]
        Investments = 1
    }

    public enum BankToMobileTransactionRequest
    {
        [Description("Resubmit Response")]
        ResubmitResponse = 12,
        [Description("Channels Request")]
        ChannelsRequest = 13,
        [Description("Loan Limit Request")]
        LoanLimitRequest = 14,
        [Description("Text Message Send/Recv")]
        TextMessageSendRecv = 15,
        [Description("Transaction Callback")]
        TransactionCallback = 16,
        [Description("PIN Reset")]
        PINReset = 17,
        [Description("Alternate Channels Request")]
        AlternateChannelsRequest = 18,
        [Description("Wallet Charges Top-Up Notification")]
        WalletChargesTopUpNotification = 19,
        [Description("Wallet SMS Top-Up Notification")]
        WalletSMSTopUpNotification = 20,
        [Description("Registration")]
        Registration = 21,
        [Description("Debit Advice")]
        DebitAdvice = 22,
        [Description("Reversal Advice")]
        ReversalAdvice = 23,
        [Description("Balance Enquiry")]
        BalanceEnquiry = 24,
        [Description("Mini Statement")]
        MiniStatement = 25,
        [Description("Loan Request")]
        LoanRequest = 26,
        [Description("Airtime Purchase")]
        AirtimePurchase = 28,
        [Description("Stop Channel")]
        StopChannel = 29,
        [Description("Wallet Airtime Top-Up Notification")]
        WalletAirtimeTopUpNotification = 30,
        [Description("Savings Accounts Request")]
        SavingsAccountsRequest = 31,
        [Description("Loan Accounts Request")]
        LoanAccountsRequest = 32,
        [Description("Investment Accounts Request")]
        InvestmentAccountsRequest = 33,
        [Description("Loans Guaranteed")]
        LoansGuaranteed = 34,
        [Description("My Loan Guarantors")]
        MyLoanGuarantors = 35,
    }

    public enum BankToMobileTransactionResponse
    {
        [Description("Success")]
        Success = 41,
        [Description("Failed")]
        Failed = 42,
        [Description("Declined")]
        Declined = 43,
        [Description("System Busy")]
        SystemBusy = 44,
        [Description("Duplicate Transmission Detected")]
        DuplicateTransmissionDetected = 45,
        [Description("No Callback")]
        NoCallback = 46
    }

    public enum RecoveryPriority
    {
        [Description("Loans")]
        Loans = 0,
        [Description("Investments")]
        Investments = 1,
        [Description("Savings")]
        Savings = 2,
        [Description("DirectDebits")]
        DirectDebits = 3
    }

    public enum BudgetEntryType
    {
        [Description("Income/Expense")]
        IncomeOrExpense = 0,
        [Description("Loan Product")]
        LoanProduct = 1
    }

    public enum TransactionOwnership
    {
        [Description("Beneficiary Branch (Customer)")]
        BeneficiaryBranch = 0,
        [Description("Initiating Branch (Employee)")]
        InitiatingBranch = 1,
    }

    public enum PesaSyncStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Syncd")]
        Syncd = 1
    }

    [Flags]
    public enum DMLCommand
    {
        [Description("None")]
        None = 0,
        [Description("Insert")]
        Insert = 1 << 1,
        [Description("Update")]
        Update = 1 << 2,
        [Description("Delete")]
        Delete = 1 << 3
    }

    public enum JournalFilter
    {
        [Description("Posting Period")]
        PostingPeriod = 0,
        [Description("Branch")]
        Branch,
        [Description("Total Value")]
        TotalValue,
        [Description("Primary Description")]
        PrimaryDescription,
        [Description("Secondary Description")]
        SecondaryDescription,
        [Description("Reference")]
        Reference,
        [Description("App. User Name")]
        ApplicationUserName,
        [Description("Env. User Name")]
        EnvironmentUserName,
        [Description("Env. Machine Name")]
        EnvironmentMachineName,
        [Description("Env. Domain Name")]
        EnvironmentDomainName,
        [Description("Env. OS Version")]
        EnvironmentOSVersion,
        [Description("Env. MAC Address")]
        EnvironmentMACAddress,
        [Description("Env. Motherboard Serial #")]
        EnvironmentMotherboardSerialNumber,
        [Description("Env. Processor Id")]
        EnvironmentProcessorId,
        [Description("Env. IP Address")]
        EnvironmentIPAddress,
    }

    public enum JournalEntryFilter
    {
        [Description("Posting Period")]
        JournalPostingPeriod = 0,
        [Description("Branch")]
        JournalBranch,
        [Description("Primary Description")]
        JournalPrimaryDescription,
        [Description("Secondary Description")]
        JournalSecondaryDescription,
        [Description("Reference")]
        JournalReference,
        [Description("App. User Name")]
        JournalApplicationUserName,
        [Description("Env. User Name")]
        JournalEnvironmentUserName,
        [Description("Env. Machine Name")]
        JournalEnvironmentMachineName,
        [Description("Env. Domain Name")]
        JournalEnvironmentDomainName,
        [Description("Env. OS Version")]
        JournalEnvironmentOSVersion,
        [Description("Env. MAC Address")]
        JournalEnvironmentMACAddress,
        [Description("Env. Motherboard Serial #")]
        JournalEnvironmentMotherboardSerialNumber,
        [Description("Env. Processor Id")]
        JournalEnvironmentProcessorId,
        [Description("Env. IP Address")]
        JournalEnvironmentIPAddress,
        [Description("G/L Account Name")]
        ChartOfAccount,
        [Description("Contra G/L Account Name")]
        ContraChartOfAccount,
        [Description("Amount")]
        Amount,
        [Description("Serial Number")]
        CustomerSerialNumber,
        [Description("Personal Identification #")]
        CustomerPersonalIdentificationNumber,
        [Description("First Name")]
        CustomerFirstName,
        [Description("Last Name")]
        CustomerLastName,
        [Description("Identity Card #")]
        CustomerIdentityCardNumber,
        [Description("Payroll Numbers")]
        CustomerPayrollNumbers,
        [Description("Org. Name")]
        CustomerNonIndividual_Description,
        [Description("Org. Registration #")]
        CustomerNonIndividual_RegistrationNumber,
        [Description("Address Line 1")]
        CustomerAddressLine1,
        [Description("Address Line 2")]
        CustomerAddressLine2,
        [Description("Street")]
        CustomerStreet,
        [Description("Postal Code")]
        CustomerPostalCode,
        [Description("City")]
        CustomerCity,
        [Description("Email")]
        CustomerEmail,
        [Description("Land Line")]
        CustomerLandLine,
        [Description("Mobile Line")]
        CustomerMobileLine,
        [Description("Account Number")]
        CustomerReference1,
        [Description("Membership Number")]
        CustomerReference2,
        [Description("Personal File Number")]
        CustomerReference3,
    }

    public enum CustomerAccountStatementType
    {
        [Description("Principal")]
        PrincipalStatement = 0,
        [Description("Interest")]
        InterestStatement,
        [Description("Fixed Deposit")]
        FixedDepositStatement,
        [Description("Cheque Deposit")]
        ChequeDepositStatement,
    }

    public enum PrintMedium
    {
        [Description("Receipt")]
        Receipt = 0,
        [Description("Cheque Leaf")]
        ChequeLeaf
    }

    public enum AccountAlertTrigger
    {
        [Description("Membership Account Registration")]
        MembershipAccountRegistration,
        [Description("Membership Account Verification")]
        MembershipAccountVerification,
        [Description("Membership Reset Password")]
        MembershipResetPassword,
        [Description("Transaction")]
        Transaction,
        [Description("Loan Guarantee")]
        LoanGuarantee,
        [Description("Guarantor Substitution")]
        GuarantorSubstitution,
        [Description("Guarantor Attachment")]
        GuarantorAttachment,
        [Description("Guarantor Relieving")]
        GuarantorRelieving,
        [Description("Electronic Statement")]
        ElectronicStatement,
        [Description("Loan Request")]
        LoanRequest,
        [Description("Mobile To Bank")]
        MobileToBankSenderAcknowledgement,
        [Description("Membership Approval")]
        MembershipApproval,
        [Description("Account Closure")]
        AccountClosure,
        [Description("Account Freezing")]
        AccountFreezing,
        [Description("Account Dormant")]
        AccountDormant,
        [Description("Loan Deffered")]
        LoanDeffered,
        [Description("Alternate Channel Linking")]
        AlternateChannelLinking,
        [Description("Alternate Channel Replacement")]
        AlternateChannelReplacement,
        [Description("Alternate Channel Renewal")]
        AlternateChannelRenewal,
        [Description("Alternate Channel Delinking")]
        AlternateChannelDelinking,
        [Description("Alternate Channel Stoppage")]
        AlternateChannelStoppage,
        [Description("Leave Approval")]
        LeaveApproval,
        [Description("Customer Details Editing")]
        CustomerDetailsEditing,
    }
    public enum RequisitionBatchEntryFilter
    {
        [Description("Item Description")]
        ItemRegisterDescription = 0,
        [Description("Inventory Category")]
        InventoryCategoryDescription,
        [Description("Unit Of Measure")]
        InventoryUnitOfMeasureDescription,
    }

    public enum CreditBatchEntryFilter
    {
        [Description("Serial Number")]
        SerialNumber = 0,
        [Description("Personal Identification #")]
        PersonalIdentificationNumber,
        [Description("First Name")]
        FirstName,
        [Description("Last Name")]
        LastName,
        [Description("Identity Card #")]
        IdentityCardNumber,
        [Description("Payroll Numbers")]
        PayrollNumbers,
        [Description("Org. Name")]
        NonIndividual_Description,
        [Description("Org. Registration #")]
        NonIndividual_RegistrationNumber,
        [Description("Address Line 1")]
        AddressLine1,
        [Description("Address Line 2")]
        AddressLine2,
        [Description("Street")]
        Street,
        [Description("Postal Code")]
        PostalCode,
        [Description("City")]
        City,
        [Description("Email")]
        Email,
        [Description("Land Line")]
        LandLine,
        [Description("Mobile Line")]
        MobileLine,
        [Description("Account Number")]
        Reference1,
        [Description("Membership Number")]
        Reference2,
        [Description("Personal File Number")]
        Reference3,
        [Description("Beneficiary")]
        Beneficiary,
        [Description("Reference")]
        Reference,
    }

    public enum CreditBatchDiscrepancyFilter
    {
        [Description("Batch Number")]
        BatchNumber = 0,
        [Description("Column 1")]
        Column1,
        [Description("Column 2")]
        Column2,
        [Description("Column 3")]
        Column3,
        [Description("Column 4")]
        Column4,
        [Description("Column 5")]
        Column5,
        [Description("Column 6")]
        Column6,
        [Description("Column 7")]
        Column7,
        [Description("Column 8")]
        Column8,
        [Description("Remarks")]
        Remarks
    }

    [Flags]
    public enum CreditBatchDiscrepancyAuthOption
    {
        [Description("Match")]
        Match = 1,
        [Description("Reject")]
        Reject = 2,
    }

    public enum LoanRegistrationLookupMode
    {
        [Description("Active Customer")]
        ActiveCustomer = 0,
        [Description("Active Loan Request")]
        ActiveLoanRequest
    }

    public enum SignalRClientType
    {
        [Description("Application User")]
        ApplicationUser = 0,
        [Description("Application Device")]
        ApplicationDevice,
    }

    public enum HttpMessageType
    {
        [Description("Request")]
        Request = 0,
        [Description("Response")]
        Response
    }

    public enum ArrearageCategory
    {
        [Description("Principal")]
        Principal = 0,
        [Description("Interest")]
        Interest,
    }

    public enum ArrearageAdjustmentType
    {
        [Description("Addition")]
        Addition = 0,
        [Description("Deduction")]
        Deduction,
    }

    public enum AdministrativeDivisionType
    {
        [Description("Header Entry")]
        HeaderEntry = 0,
        [Description("Detail Entry")]
        DetailEntry = 1
    }

    public enum TellerLimitCategory
    {
        [Description("Normal")]
        Normal = 0,
        [Description("Lower")]
        Lower = 1,
        [Description("Upper")]
        Upper = 2
    }

    public enum CarryForwardAdjustmentType
    {
        [Description("Addition")]
        Addition = 0,
        [Description("Deduction")]
        Deduction,
    }

    public enum CustomerClassification
    {
        [Description("")]
        Unknown = 0,
        [Description("Class A")]
        Alpha = 1,
        [Description("Class B")]
        Bravo = 2,
    }

    [Flags]
    public enum CashDepositCategory
    {
        [Description("Cash Deposit Within Limits")]
        WithinLimits = 1,
        [Description("Cash Deposit Above Maximum Allowed")]
        AboveMaximumAllowed = 2,
    }

    [Flags]
    public enum CashDepositRequestAuthStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Authorized")]
        Authorized = 2,
        [Description("Rejected")]
        Rejected = 4,
        [Description("Posted")]
        Posted = 8
    }

    public enum InstantPaymentNotificationStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Submitted")]
        Submitted = 1,
        [Description("Acknowledged")]
        Acknowledged = 2,
        [Description("Verified")]
        Verified = 3
    }

    public enum BankToMobileRequestStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Processed")]
        Processed = 1,
    }

    public enum BrokerRequestStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Processed")]
        Processed = 1,
    }

    public enum BrokerTransactionResponse
    {
        [Description("Success")]
        Success = 41,
        [Description("Failed")]
        Failed = 42,
        [Description("Declined")]
        Declined = 43,
        [Description("System Busy")]
        SystemBusy = 44,
        [Description("Duplicate Transmission Detected")]
        DuplicateTransmissionDetected = 45,
        [Description("No Callback")]
        NoCallback = 46
    }

    public enum BrokerTransactionRequest
    {
        [Description("Customer Registration From Broker")]
        CustomerRegistrationFromBroker = 10,
        [Description("Customer Subscription To Broker")]
        CustomerSubscriptionToBroker = 11,
        [Description("Account Statement")]
        AccountStatement = 12,
        [Description("Balance Enquiry")]
        BalanceEnquiry = 13,
        [Description("Loans Guaranteed")]
        LoansGuaranteed = 14,
        [Description("My Loan Guarantors")]
        MyLoanGuarantors = 15,
        [Description("Dashboard")]
        DashboardSummary = 16,
    }

    public enum LoanInterestAdjustmentType
    {
        [Description("Increase")]
        Increase = 0,
        [Description("Decrease")]
        Decrease,
    }

    public enum PopulationRegisterFilter
    {
        [Description("ID Number")]
        IDNumber,
        [Description("Serial Number")]
        SerialNumber,
        [Description("Gender")]
        Gender,
        [Description("First Name")]
        FirstName,
        [Description("Other Name")]
        OtherName,
        [Description("Surname")]
        Surname,
        [Description("Pin")]
        Pin,
        [Description("Citizenship")]
        Citizenship,
        [Description("Family")]
        Family,
        [Description("Clan")]
        Clan,
        [Description("Ethnic Group")]
        EthnicGroup,
        [Description("Occupation")]
        Occupation,
        [Description("Place of Birth")]
        PlaceOfBirth,
        [Description("Place of Death")]
        PlaceOfDeath,
        [Description("Place of Live")]
        PlaceOfLive,
        [Description("Reg Office")]
        RegOffice,
        [Description("Remarks")]
        Remarks
    }

    public enum PopulationRegisterQueryStatus
    {
        [Description("New")]
        New = 0,
        [Description("Authorized")]
        Authorized = 1,
        [Description("Queried")]
        Queried = 2,
        [Description("Rejected")]
        Rejected = 3,
    }


    [Flags]
    public enum PopulationRegisterQueryAuthOption
    {
        [Description("Authorize")]
        Authorize = 1,
        [Description("Reject")]
        Reject = 2,
    }

    public enum TwoFactorProviders
    {
        [Description("Phone Code")]
        PhoneCode,
        [Description("Email Code")]
        EmailCode
    }


    public enum PaymentMethod
    {
        [Description("CASH")]
        Cash = 0,
        [Description("MPESA")]
        MPESA = 1,
        [Description("CHEQUE")]
        CHEQUE = 2,
    }


    public enum SalesOrderType
    {
        [Description("CASH")]
        Cash = 0,
        [Description("PREMIUM FINANCING")]
        PREMIUMFINANCING = 1,
    }

    public enum PaymentFrequency
    {
        [Description("Monthly")]
        Monthly = 1,
        [Description("Weekly")]
        Weekly = 2,
        [Description("Daily")]
        Daily = 3
    }

    public enum C2BTransactionCategory
    {
        [Description("Customer")]
        Customer
    }

    public enum C2BTransactionStatus
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Approved")]
        Approved = 1,
        [Description("Processed")]
        Processed = 2,
        [Description("Rejected")]
        Rejected = 3,
        [Description("Unknown")]
        Unknown = 4,
        [Description("Queued")]
        Queued = 5,
        [Description("Suspense")]
        Suspense = 6,
        [Description("Reversed")]
        Reversed = 7,
        [Description("Pending Reconciliation")]
        PendingReconciliation = 8,
        [Description("Waiting Customer Creation")]
        Waiting = 9,
        [Description("Reconciled")]
        Reconciled = 10
    }


    public enum InventoryRecordStatus
    {
        [Description("Active")]
        Active = 0,

        [Description("Inactive")]
        Inactive = 1,
        [Description("Discontinued")]
        Discontinued = 2,
        [Description("Rejected")]
        Rejected = 3,
        [Description("Low Stock")]
        LowStock = 4
    }



    public enum PurchaseOrderStatus
    {
        [Description("Pending")]
        Pending = 0,

        [Description("Approved")]
        Approved = 1,
        [Description("Rejected")]
        Rejected = 2,
        [Description("Completed")]
        Completed = 3
    }

    // C# - Infrastructure.Crosscutting.Framework.Utils\MessagePriority.cs

        public enum MessagePriority
        {
            [Description("Lowest")]
            Lowest = 0,

        [Description("Low")]
        Low = 1,

        [Description("Normal")]
        Normal = 2,

        [Description("AboveNormal")]
        AboveNormal = 3,

        [Description("High")]
        High = 4,

        [Description("Highest")]
        Highest = 5
        }
   



}