using AutoMapper;
using Domain.MainBoundedContext.AccountsModule.Aggregates;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelReconciliationPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ARCustomerAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankLinkageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BankToMobileRequestAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BrokerRequestAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.BudgetEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeBookAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAttachedProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionLevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CommissionSplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CostCenterAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchDiscrepancyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAttachedProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeConcessionExemptProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeDirectDebitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardInstallmentAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountHistoryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountSignatoryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DebitTypeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DirectDebitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.DynamicChargeCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderHistoryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAttachedProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeGraduatedScaleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeLevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.FuneralRiderClaimPayable;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GeneralLedgerEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.GraduatedScaleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ImprestAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InsuranceCompanyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchDynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.InvestmentProductExemptionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalReversalBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanCycleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAppraisalProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxiliaryConditionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAuxilliaryAppraisalFactorAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDeductibleAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductDynamicChargeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.MobileToBankRequestAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentVoucherAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReportTemplateEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SavingsProductExemptionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SystemGeneralLedgerAccountMappingAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SystemTransactionTypeInCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.TellerAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.TreasuryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.UnPayReasonCommissionAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchEntryAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferTypeCommissionAgg;

using System.Linq;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class AccountsModuleProfile : Profile
    {
        public AccountsModuleProfile()
        {
            //CostCenter => CostCenterDTO
            CreateMap<CostCenter, CostCenterDTO>();

            //ChartOfAccount => ChartOfAccountDTO
            CreateMap<ChartOfAccount, ChartOfAccountDTO>();

            //ChartOfAccount => ChartOfAccountSummaryDTO
            CreateMap<ChartOfAccount, ChartOfAccountSummaryDTO>();

            //PostingPeriod => PostingPeriodDTO
            CreateMap<PostingPeriod, PostingPeriodDTO>();

            //Teller => TellerDTO
            CreateMap<Teller, TellerDTO>()
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.ShortageChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCode, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCustomerFullName, opt => opt.Ignore());

            //BankLinkage => BankLinkageDTO
            CreateMap<BankLinkage, BankLinkageDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //CustomerAccount => CustomerAccountDTO
            CreateMap<CustomerAccount, CustomerAccountDTO>()
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.BookBalance, opt => opt.Ignore())
                .ForMember(dest => dest.InterestBalance, opt => opt.Ignore())
                .ForMember(dest => dest.PrincipalBalance, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountTypeProductCodeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountTypeTargetProductIsRefundable, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountTypeTargetProductIsDefault, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountTypeTargetProductLoanProductSectionDescription, opt => opt.Ignore());

            //CustomerAccount => CustomerAccountDTO
            CreateMap<CustomerAccount, CustomerAccountSummaryDTO>()
                .ForMember(dest => dest.CustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.RecordStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //CustomerAccountHistory => CustomerAccountHistoryDTO
            CreateMap<CustomerAccountHistory, CustomerAccountHistoryDTO>()
                .ForMember(dest => dest.ManagementActionDescription, opt => opt.Ignore());

            //Journal => JournalDTO
            CreateMap<Journal, JournalDTO>();

            //JournalEntry => JournalEntryDTO
            CreateMap<JournalEntry, JournalEntryDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.ContraChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCustomerAccountCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //JournalEntry => JournalEntrySummaryDTO
            CreateMap<JournalEntry, JournalEntrySummaryDTO>();

            //Commission => CommissionDTO
            CreateMap<Commission, CommissionDTO>();

            //GraduatedScale => GraduatedScaleDTO
            CreateMap<GraduatedScale, GraduatedScaleDTO>();

            //CommissionSplit => CommissionSplitDTO
            CreateMap<CommissionSplit, CommissionSplitDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //Levy => LevyDTO
            CreateMap<Levy, LevyDTO>();

            //LevySplit => LevySplitDTO
            CreateMap<LevySplit, LevySplitDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //CommissionLevy => CommissionLevyDTO
            CreateMap<CommissionLevy, CommissionLevyDTO>();

            //SystemGeneralLedgerAccountMapping => SystemGeneralLedgerAccountMappingDTO
            CreateMap<SystemGeneralLedgerAccountMapping, SystemGeneralLedgerAccountMappingDTO>()
                 .ForMember(dest => dest.SystemGeneralLedgerAccountCodeDescription, opt => opt.Ignore());

            //SavingsProduct => SavingsProductDTO
            CreateMap<SavingsProduct, SavingsProductDTO>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.Format("{0}", src.Description).Trim()))
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCode, opt => opt.Ignore());

            //InvestmentProduct => InvestmentProductDTO
            CreateMap<InvestmentProduct, InvestmentProductDTO>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.Format("{0}", src.Description).Trim()))
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.PoolChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCode, opt => opt.Ignore());

            //LoanProduct => LoanProductDTO
            CreateMap<LoanProduct, LoanProductDTO>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.Format("{0}", src.Description).Trim()))
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.InterestReceivableChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.InterestReceivedChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCode, opt => opt.Ignore())
                .ForMember(dest => dest.LoanRegistrationLoanProductSectionDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanInterestChargeModeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanInterestRecoveryModeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanInterestCalculationModeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanRegistrationPaymentFrequencyPerYearDescription, opt => opt.Ignore())
                .ForMember(dest => dest.LoanRegistrationPaymentDueDateDescription, opt => opt.Ignore());

            //ChequeType => ChequeTypeDTO
            CreateMap<ChequeType, ChequeTypeDTO>();

            //DynamicCharge => DynamicChargeDTO
            CreateMap<DynamicCharge, DynamicChargeDTO>()
                .ForMember(dest => dest.RecoveryModeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.RecoverySourceDescription, opt => opt.Ignore());

            //StandingOrder => StandingOrderDTO
            CreateMap<StandingOrder, StandingOrderDTO>()
                .ForMember(dest => dest.TriggerDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduleFrequencyDescription, opt => opt.Ignore())
                .ForMember(dest => dest.BenefactorFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.BenefactorCustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.BenefactorCustomerAccountCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCustomerAccountCustomerFullName, opt => opt.Ignore());

            //StandingOrderHistory => StandingOrderHistoryDTO
            CreateMap<StandingOrderHistory, StandingOrderHistoryDTO>()
                .ForMember(dest => dest.MonthDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TriggerDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduleFrequencyDescription, opt => opt.Ignore())
                .ForMember(dest => dest.BenefactorFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.BenefactorCustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.BenefactorCustomerAccountCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.BeneficiaryCustomerAccountCustomerFullName, opt => opt.Ignore());

            //JournalVoucher => JournalVoucherDTO
            CreateMap<JournalVoucher, JournalVoucherDTO>()
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedVoucherNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.CanSuppressMakerCheckerValidation, opt => opt.MapFrom(src => CheckCanSuppressMakerCheckerValidation(src)));

            //JournalVoucherEntry => JournalVoucherEntryDTO
            CreateMap<JournalVoucherEntry, JournalVoucherEntryDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //CreditBatch => CreditBatchDTO
            CreateMap<CreditBatch, CreditBatchDTO>()
                .ForMember(dest => dest.PaddedBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.MonthDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore());

            //CreditBatchEntry => CreditBatchEntryDTO
            CreateMap<CreditBatchEntry, CreditBatchEntryDTO>()
                .ForMember(dest => dest.PaddedCreditBatchBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CreditBatchStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CreditBatchTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //CreditBatchDiscrepancy => CreditBatchDiscrepancyDTO
            CreateMap<CreditBatchDiscrepancy, CreditBatchDiscrepancyDTO>();

            //CreditType => CreditTypeDTO
            CreateMap<CreditType, CreditTypeDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //BudgetDTO => BudgetDTO
            CreateMap<Budget, BudgetDTO>();

            //BudgetEntry => BudgetEntryDTO
            CreateMap<BudgetEntry, BudgetEntryDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.MonthlyBudget, opt => opt.Ignore())
                .ForMember(dest => dest.BudgetToDate, opt => opt.Ignore());

            //AlternateChannel => AlternateChannelDTO
            CreateMap<AlternateChannel, AlternateChannelDTO>()
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.Ignore())
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerTypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //AlternateChannelLog => AlternateChannelLogDTO
            CreateMap<AlternateChannelLog, AlternateChannelLogDTO>()
                .ForMember(dest => dest.AlternateChannelTypeDescription, opt => opt.Ignore());

            //AlternateChannelLogDTO => AlternateChannelLogBindingModel
            CreateMap<AlternateChannelLogDTO, AlternateChannelLogBindingModel>();

            //AlternateChannelLog => ISO8583AlternateChannelLogDTO
            CreateMap<AlternateChannelLog, ISO8583AlternateChannelLogDTO>()
                .ForMember(dest => dest.AlternateChannelTypeDescription, opt => opt.Ignore());

            //ISO8583AlternateChannelLogDTO => ISO8583AlternateChannelLogBindingModel
            CreateMap<ISO8583AlternateChannelLogDTO, ISO8583AlternateChannelLogBindingModel>();

            //AlternateChannelLog => SPARROWAlternateChannelLogDTO
            CreateMap<AlternateChannelLog, SPARROWAlternateChannelLogDTO>()
                .ForMember(dest => dest.AlternateChannelTypeDescription, opt => opt.Ignore());

            //SPARROWAlternateChannelLogDTO => SPARROWAlternateChannelLogBindingModel
            CreateMap<SPARROWAlternateChannelLogDTO, SPARROWAlternateChannelLogBindingModel>();

            //AlternateChannelLog => WALLETAlternateChannelLogDTO
            CreateMap<AlternateChannelLog, WALLETAlternateChannelLogDTO>()
                .ForMember(dest => dest.AlternateChannelTypeDescription, opt => opt.Ignore());

            //WALLETAlternateChannelLogDTO => WALLETAlternateChannelLogBindingModel
            CreateMap<WALLETAlternateChannelLogDTO, WALLETAlternateChannelLogBindingModel>();

            //Treasury => TreasuryDTO
            CreateMap<Treasury, TreasuryDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCode, opt => opt.Ignore());

            //OverDeductionBatch => OverDeductionBatchDTO
            CreateMap<OverDeductionBatch, OverDeductionBatchDTO>()
                .ForMember(dest => dest.PaddedBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //OverDeductionBatchEntry => OverDeductionBatchEntryDTO
            CreateMap<OverDeductionBatchEntry, OverDeductionBatchEntryDTO>()
                .ForMember(dest => dest.DebitFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.DebitCustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.DebitCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.CreditFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CreditCustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CreditCustomerFullName, opt => opt.Ignore());

            //ReportTemplate => ReportTemplateDTO
            CreateMap<ReportTemplate, ReportTemplateDTO>()
                .ForMember(dest => dest.CategoryDescription, opt => opt.Ignore())
                .ForMember(dest => dest.BookBalance, opt => opt.Ignore())
                .ForMember(dest => dest.IndentedDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TemplateFileName, opt => opt.Ignore())
                .ForMember(dest => dest.TemplateCutOffDate, opt => opt.Ignore())
                .ForMember(dest => dest.TemplateFileBuffer, opt => opt.Ignore());

            //ReportTemplateEntry => ReportTemplateEntryDTO
            CreateMap<ReportTemplateEntry, ReportTemplateEntryDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //InsuranceCompany => InsuranceCompanyDTO
            CreateMap<InsuranceCompany, InsuranceCompanyDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //DebitType => DebitTypeDTO
            CreateMap<DebitType, DebitTypeDTO>()
                .ForMember(dest => dest.CustomerAccountTypeProductCodeDescription, opt => opt.Ignore());

            //DebitBatch => DebitBatchDTO
            CreateMap<DebitBatch, DebitBatchDTO>()
                .ForMember(dest => dest.PaddedBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //DebitBatchEntry => DebitBatchEntryDTO
            CreateMap<DebitBatchEntry, DebitBatchEntryDTO>()
                .ForMember(dest => dest.PaddedDebitBatchBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.DebitBatchStatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore());

            //LoanCycle => LoanCycleDTO
            CreateMap<LoanCycle, LoanCycleDTO>();

            //CustomerAccountSignatory => CustomerAccountSignatoryDTO
            CreateMap<CustomerAccountSignatory, CustomerAccountSignatoryDTO>()
                .ForMember(dest => dest.SalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.GenderDescription, opt => opt.Ignore())
                .ForMember(dest => dest.RelationshipDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //ChequeBook => ChequeBookDTO
            CreateMap<ChequeBook, ChequeBookDTO>();

            //PaymentVoucher => PaymentVoucherDTO
            CreateMap<PaymentVoucher, PaymentVoucherDTO>();

            //UnPayReason => UnPayReasonDTO
            CreateMap<UnPayReason, UnPayReasonDTO>();

            //WireTransferBatch => WireTransferBatchDTO
            CreateMap<WireTransferBatch, WireTransferBatchDTO>()
                .ForMember(dest => dest.PaddedBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //WireTransferBatchEntry => WireTransferBatchEntryDTO
            CreateMap<WireTransferBatchEntry, WireTransferBatchEntryDTO>();

            //DirectDebit => DirectDebitDTO
            CreateMap<DirectDebit, DirectDebitDTO>()
                .ForMember(dest => dest.CustomerAccountTypeProductCodeDescription, opt => opt.Ignore());

            //LoanProductAuxiliaryCondition => LoanProductAuxiliaryConditionDTO
            CreateMap<LoanProductAuxiliaryCondition, LoanProductAuxiliaryConditionDTO>()
                .ForMember(dest => dest.ConditionDescription, opt => opt.Ignore());

            //SavingsProductCommission => SavingsProductCommissionDTO
            CreateMap<SavingsProductCommission, SavingsProductCommissionDTO>();

            //CreditTypeCommission => CreditTypeCommissionDTO
            CreateMap<CreditTypeCommission, CreditTypeCommissionDTO>();

            //ChequeTypeCommission => ChequeTypeCommissionDTO
            CreateMap<ChequeTypeCommission, ChequeTypeCommissionDTO>();

            //DebitTypeCommission => DebitTypeCommissionDTO
            CreateMap<DebitTypeCommission, DebitTypeCommissionDTO>();

            //UnPayReasonCommission => UnPayReasonCommissionDTO
            CreateMap<UnPayReasonCommission, UnPayReasonCommissionDTO>();

            //DynamicChargeCommission => DynamicChargeCommissionDTO
            CreateMap<DynamicChargeCommission, DynamicChargeCommissionDTO>();

            //LoanProductDynamicCharge => LoanProductDynamicChargeDTO
            CreateMap<LoanProductDynamicCharge, LoanProductDynamicChargeDTO>();

            //SystemTransactionTypeInCommission => SystemTransactionTypeInCommissionDTO
            CreateMap<SystemTransactionTypeInCommission, SystemTransactionTypeInCommissionDTO>();

            //CreditTypeDirectDebit => CreditTypeDirectDebitDTO
            CreateMap<CreditTypeDirectDebit, CreditTypeDirectDebitDTO>();

            //CreditTypeAttachedProduct => CreditTypeAttachedProductDTO
            CreateMap<CreditTypeAttachedProduct, CreditTypeAttachedProductDTO>();

            //ChequeTypeAttachedProduct => ChequeTypeAttachedProductDTO
            CreateMap<ChequeTypeAttachedProduct, ChequeTypeAttachedProductDTO>();

            //LoanProductAppraisalProduct => LoanProductAppraisalProductDTO
            CreateMap<LoanProductAppraisalProduct, LoanProductAppraisalProductDTO>();

            //LoanProductDeductible => LoanProductDeductibleDTO
            CreateMap<LoanProductDeductible, LoanProductDeductibleDTO>();

            //LoanProductAuxilliaryAppraisalFactor => LoanProductAuxilliaryAppraisalFactorDTO
            CreateMap<LoanProductAuxilliaryAppraisalFactor, LoanProductAuxilliaryAppraisalFactorDTO>();

            //RecurringBatch => RecurringBatchDTO
            CreateMap<RecurringBatch, RecurringBatchDTO>()
                .ForMember(dest => dest.PaddedBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.MonthDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore());

            //RecurringBatchEntry => RecurringBatchEntryDTO
            CreateMap<RecurringBatchEntry, RecurringBatchEntryDTO>();

            //BankReconciliationPeriod => BankReconciliationPeriodDTO
            CreateMap<BankReconciliationPeriod, BankReconciliationPeriodDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //BankReconciliationEntry => BankReconciliationEntryDTO
            CreateMap<BankReconciliationEntry, BankReconciliationEntryDTO>()
                .ForMember(dest => dest.AdjustmentTypeDescription, opt => opt.Ignore());

            //JournalReversalBatch => JournalReversalBatchDTO
            CreateMap<JournalReversalBatch, JournalReversalBatchDTO>()
                .ForMember(dest => dest.PaddedBatchNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //JournalReversalBatchEntry => JournalReversalBatchEntryDTO
            CreateMap<JournalReversalBatchEntry, JournalReversalBatchEntryDTO>();

            //LoanProductCommission => LoanProductCommissionDTO
            CreateMap<LoanProductCommission, LoanProductCommissionDTO>();

            //InterAccountTransferBatch => InterAccountTransferBatchDTO
            CreateMap<InterAccountTransferBatch, InterAccountTransferBatchDTO>();

            //InterAccountTransferBatchEntry => InterAccountTransferBatchEntryDTO
            CreateMap<InterAccountTransferBatchEntry, InterAccountTransferBatchEntryDTO>();

            //InterAccountTransferBatchDynamicCharge => InterAccountTransferBatchDynamicChargeDTO
            CreateMap<InterAccountTransferBatchDynamicCharge, InterAccountTransferBatchDynamicChargeDTO>();

            //CustomerAccountCarryForward => CustomerAccountCarryForwardDTO
            CreateMap<CustomerAccountCarryForward, CustomerAccountCarryForwardDTO>();

            //SavingsProductExemption => SavingsProductExemptionDTO
            CreateMap<SavingsProductExemption, SavingsProductExemptionDTO>();

            //MobileToBankRequest => MobileToBankRequestDTO
            CreateMap<MobileToBankRequest, MobileToBankRequestDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //CreditTypeConcessionExemptProduct => CreditTypeConcessionExemptProductDTO
            CreateMap<CreditTypeConcessionExemptProduct, CreditTypeConcessionExemptProductDTO>();

            //GeneralLedger => GeneralLedgerDTO
            CreateMap<GeneralLedger, GeneralLedgerDTO>()
                .ForMember(dest => dest.PaddedLedgerNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CanSuppressMakerCheckerValidation, opt => opt.MapFrom(src => CheckCanSuppressMakerCheckerValidation(src)));

            //GeneralLedgerEntry => GeneralLedgerEntryDTO
            CreateMap<GeneralLedgerEntry, GeneralLedgerEntryDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.ContraChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCustomerAccountCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.ContraCustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedContraCustomerAccountCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.ContraCustomerAccountCustomerFullName, opt => opt.Ignore());

            //AlternateChannelReconciliationPeriod => AlternateChannelReconciliationPeriodDTO
            CreateMap<AlternateChannelReconciliationPeriod, AlternateChannelReconciliationPeriodDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //AlternateChannelReconciliationEntry => AlternateChannelReconciliationEntryDTO
            CreateMap<AlternateChannelReconciliationEntry, AlternateChannelReconciliationEntryDTO>();

            //AlternateChannelTypeCommission => AlternateChannelTypeCommissionDTO
            CreateMap<AlternateChannelTypeCommission, AlternateChannelTypeCommissionDTO>();

            //FixedDepositType => FixedDepositTypeDTO
            CreateMap<FixedDepositType, FixedDepositTypeDTO>();

            //FixedDepositTypeAttachedProduct => FixedDepositTypeAttachedProductDTO
            CreateMap<FixedDepositTypeAttachedProduct, FixedDepositTypeAttachedProductDTO>();

            //FixedDepositTypeLevy => FixedDepositTypeLevyDTO
            CreateMap<FixedDepositTypeLevy, FixedDepositTypeLevyDTO>();

            //FixedDepositTypeGraduatedScale => FixedDepositTypeGraduatedScaleDTO
            CreateMap<FixedDepositTypeGraduatedScale, FixedDepositTypeGraduatedScaleDTO>();

            //FixedDepositTypeGraduatedScaleDTO => FixedDepositTypeGraduatedScaleBindingModel
            CreateMap<FixedDepositTypeGraduatedScaleDTO, FixedDepositTypeGraduatedScaleBindingModel>();

            //WireTransferType => WireTransferTypeDTO
            CreateMap<WireTransferType, WireTransferTypeDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //WireTransferTypeCommission => WireTransferTypeCommissionDTO
            CreateMap<WireTransferTypeCommission, WireTransferTypeCommissionDTO>();

            //ElectronicStatementOrder => ElectronicStatementOrderDTO
            CreateMap<ElectronicStatementOrder, ElectronicStatementOrderDTO>()
                .ForMember(dest => dest.ScheduleFrequencyDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //ElectronicStatementOrderHistory => ElectronicStatementOrderHistoryDTO
            CreateMap<ElectronicStatementOrderHistory, ElectronicStatementOrderHistoryDTO>()
                .ForMember(dest => dest.ScheduleFrequencyDescription, opt => opt.Ignore())
                .ForMember(dest => dest.FullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //FuneralRiderClaimPayable => FuneralRiderClaimPayableDTO
            CreateMap<FuneralRiderClaimPayable, FuneralRiderClaimPayableDTO>();

            //CustomerAccountArrearage => CustomerAccountArrearageDTO
            CreateMap<CustomerAccountArrearage, CustomerAccountArrearageDTO>();

            //InvestmentProductExemption => InvestmentProductExemptionDTO
            CreateMap<InvestmentProductExemption, InvestmentProductExemptionDTO>();

            //CustomerAccountCarryForwardInstallment => CustomerAccountCarryForwardInstallmentDTO
            CreateMap<CustomerAccountCarryForwardInstallment, CustomerAccountCarryForwardInstallmentDTO>();

            //BankToMobileRequest => BankToMobileRequestDTO
            CreateMap<BankToMobileRequest, BankToMobileRequestDTO>();

            //BrokerRequest => BrokerRequestDTO
            CreateMap<BrokerRequest, BrokerRequestDTO>();

            //BrokerRequestDTO => BrokerRequestBindingModel
            CreateMap<BrokerRequestDTO, BrokerRequestBindingModel>();

            //PurchaseInvoice => PurchaseInvoiceDTO
            CreateMap<PurchaseInvoice, PurchaseInvoiceDTO>();


            //PurchaseInvoiceLine => PurchaseInvoiceLineDTO
            CreateMap<PurchaseInvoiceLine, PurchaseInvoiceLineDTO>();


            CreateMap<PurchaseCreditMemo, PurchaseCreditMemoDTO>();


            CreateMap<PurchaseCreditMemoLine, PurchaseCreditMemoLineDTO>();

            CreateMap<SalesInvoice, SalesInvoiceDTO>();

            CreateMap<SalesInvoiceLine, SalesInvoiceLineDTO>();


            
            CreateMap<SalesCreditMemoLine, SalesCreditMemoLineDTO>();


          
            CreateMap<SalesCreditMemo, SalesCreditMemoDTO>();

            CreateMap<Payment, PaymentDTO>();

            CreateMap<PaymentLine, PaymentLineDTO>();

            CreateMap<ARCustomer, ARCustomerDTO>();

            CreateMap<Receipt, ReceiptDTO>();

            CreateMap<Imprest, ImprestDTO>();

            CreateMap<ImprestLine, ImprestLineDTO>();

        }

        static bool CheckCanSuppressMakerCheckerValidation(JournalVoucher journalVoucher)
        {
            var result =
                journalVoucher.CustomerAccount == null || !journalVoucher.JournalVoucherEntries.Any(x => x.CustomerAccount != null);

            return result;
        }

        static bool CheckCanSuppressMakerCheckerValidation(GeneralLedger generalLedger)
        {
            var result =
                !generalLedger.GeneralLedgerEntries.Any(x => x.ContraCustomerAccount != null || x.CustomerAccount != null);

            return result;
        }
    }
}