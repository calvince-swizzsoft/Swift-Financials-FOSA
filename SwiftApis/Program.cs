

using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.MicroCreditModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.Services;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg;
using Domain.Seedwork;
using Infrastructure.Data.MainBoundedContext.Repositories;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using LazyCache;
using Numero3.EntityFramework.Implementation;
using Numero3.EntityFramework.Interfaces;
//using Numero3.EntityFramework.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi



builder.Services.AddOpenApi();

////->services


// -> Application services
builder.Services.AddScoped<IAuditLogAppService, AuditLogAppService>();
builder.Services.AddScoped<IFinancialsService, FinancialsService>();
builder.Services.AddScoped<IMediaAppService, MediaAppService>();
builder.Services.AddScoped<ISqlCommandAppService, SqlCommandAppService>();
builder.Services.AddScoped<IJournalEntryPostingService, JournalEntryPostingService>();
builder.Services.AddScoped<IEnumerationAppService, EnumerationAppService>();
//builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<IMessageQueueService, MessageQueueService>();
builder.Services.AddScoped<IBrokerService, BrokerService>();

builder.Services.AddScoped<IChartOfAccountAppService, ChartOfAccountAppService>();
builder.Services.AddScoped<ICostCenterAppService, CostCenterAppService>();
builder.Services.AddScoped<ITellerAppService, TellerAppService>();
builder.Services.AddScoped<IPostingPeriodAppService, PostingPeriodAppService>();
builder.Services.AddScoped<IBankLinkageAppService, BankLinkageAppService>();
builder.Services.AddScoped<ICustomerAccountAppService, CustomerAccountAppService>();
builder.Services.AddScoped<IJournalAppService, JournalAppService>();
builder.Services.AddScoped<IJournalEntryAppService, JournalEntryAppService>();
builder.Services.AddScoped<ICommissionAppService, CommissionAppService>();
builder.Services.AddScoped<ILevyAppService, LevyAppService>();
builder.Services.AddScoped<ISavingsProductAppService, SavingsProductAppService>();
builder.Services.AddScoped<IInvestmentProductAppService, InvestmentProductAppService>();

builder.Services.AddScoped<ILoanProductAppService, LoanProductAppService>();
builder.Services.AddScoped<IChequeTypeAppService, ChequeTypeAppService>();
builder.Services.AddScoped<IDynamicChargeAppService, DynamicChargeAppService>();
builder.Services.AddScoped<IStandingOrderAppService, StandingOrderAppService>();
builder.Services.AddScoped<IJournalVoucherAppService, JournalVoucherAppService>();
builder.Services.AddScoped<ICreditTypeAppService, CreditTypeAppService>();
builder.Services.AddScoped<ICreditBatchAppService, CreditBatchAppService>();
builder.Services.AddScoped<IBudgetAppService, BudgetAppService>();
builder.Services.AddScoped<IAlternateChannelAppService, AlternateChannelAppService>();
builder.Services.AddScoped<ITreasuryAppService, TreasuryAppService>();
builder.Services.AddScoped<IOverDeductionBatchAppService, OverDeductionBatchAppService>();
builder.Services.AddScoped<IAlternateChannelLogAppService, AlternateChannelLogAppService>();
builder.Services.AddScoped<IReportTemplateAppService, ReportTemplateAppService>();
builder.Services.AddScoped<IInsuranceCompanyAppService, InsuranceCompanyAppService>();
builder.Services.AddScoped<IDebitTypeAppService, DebitTypeAppService>();
builder.Services.AddScoped<IDebitBatchAppService, DebitBatchAppService>();
builder.Services.AddScoped<IChequeBookAppService, ChequeBookAppService>();
builder.Services.AddScoped<IUnPayReasonAppService, UnPayReasonAppService>();
builder.Services.AddScoped<IWireTransferBatchAppService, WireTransferBatchAppService>();
builder.Services.AddScoped<IDirectDebitAppService, DirectDebitAppService>();
builder.Services.AddScoped<IRecurringBatchAppService, RecurringBatchAppService>();
builder.Services.AddScoped<IBankReconciliationPeriodAppService, BankReconciliationPeriodAppService>();
builder.Services.AddScoped<IJournalReversalBatchAppService, JournalReversalBatchAppService>();
builder.Services.AddScoped<IInterAccountTransferBatchAppService, InterAccountTransferBatchAppService>();
builder.Services.AddScoped<IMobileToBankRequestAppService, MobileToBankRequestAppService>();
builder.Services.AddScoped<IGeneralLedgerAppService, GeneralLedgerAppService>();
builder.Services.AddScoped<IAlternateChannelReconciliationPeriodAppService, AlternateChannelReconciliationPeriodAppService>();
builder.Services.AddScoped<IFixedDepositTypeAppService, FixedDepositTypeAppService>();
builder.Services.AddScoped<IWireTransferTypeAppService, WireTransferTypeAppService>();
builder.Services.AddScoped<IElectronicStatementOrderAppService, ElectronicStatementOrderAppService>();
builder.Services.AddScoped<ISuperSaverPayableAppService, SuperSaverPayableAppService>();
builder.Services.AddScoped<IBankToMobileRequestAppService, BankToMobileRequestAppService>();
builder.Services.AddScoped<IBrokerRequestAppService, BrokerRequestAppService>();
//builder.Services.AddScoped<ISystemGeneralLedgerAccountMappingService, SystemGeneralLedgerAccountMappingService>();

builder.Services.AddScoped<IFiscalCountAppService, FiscalCountAppService>();
builder.Services.AddScoped<IExternalChequeAppService, ExternalChequeAppService>();
builder.Services.AddScoped<IInHouseChequeAppService, InHouseChequeAppService>();
builder.Services.AddScoped<IFixedDepositAppService, FixedDepositAppService>();
builder.Services.AddScoped<IElectronicJournalAppService, ElectronicJournalAppService>();
builder.Services.AddScoped<IExpensePayableAppService, ExpensePayableAppService>();
builder.Services.AddScoped<IAccountClosureRequestAppService, AccountClosureRequestAppService>();
builder.Services.AddScoped<ICashWithdrawalRequestAppService, CashWithdrawalRequestAppService>();
builder.Services.AddScoped<ICashTransferRequestAppService, CashTransferRequestAppService>();
builder.Services.AddScoped<ICashDepositRequestAppService, CashDepositRequestAppService>();

builder.Services.AddScoped<IAuthorizationAppService, AuthorizationAppService>();
builder.Services.AddScoped<IReportAppService, ReportAppService>();
builder.Services.AddScoped<IBranchAppService, BranchAppService>();
builder.Services.AddScoped<IBankAppService, BankAppService>();
builder.Services.AddScoped<ICompanyAppService, CompanyAppService>();
builder.Services.AddScoped<ILocationAppService, LocationAppService>();
builder.Services.AddScoped<IWorkflowAppService, WorkflowAppService>();
// builder.Services.AddScoped<IWorkflowProcessorAppService, WorkflowProcessorAppService>(); // left commented, same as Unity side

builder.Services.AddScoped<IEmployerAppService, EmployerAppService>();
builder.Services.AddScoped<IZoneAppService, ZoneAppService>();
builder.Services.AddScoped<ICustomerAppService, CustomerAppService>();
builder.Services.AddScoped<ICustomerDocumentAppService, CustomerDocumentAppService>();
builder.Services.AddScoped<IFileRegisterAppService, FileRegisterAppService>();
builder.Services.AddScoped<IDelegateAppService, DelegateAppService>();
builder.Services.AddScoped<IDirectorAppService, DirectorAppService>();
builder.Services.AddScoped<IWithdrawalNotificationAppService, WithdrawalNotificationAppService>();
builder.Services.AddScoped<ICommissionExemptionAppService, CommissionExemptionAppService>();
builder.Services.AddScoped<IEducationVenueAppService, EducationVenueAppService>();
builder.Services.AddScoped<IEducationRegisterAppService, EducationRegisterAppService>();
builder.Services.AddScoped<IConditionalLendingAppService, ConditionalLendingAppService>();
builder.Services.AddScoped<IFuneralRiderClaimAppService, FuneralRiderClaimAppService>();
builder.Services.AddScoped<IAdministrativeDivisionAppService, AdministrativeDivisionAppService>();

builder.Services.AddScoped<ILoanPurposeAppService, LoanPurposeAppService>();
builder.Services.AddScoped<ILoanCaseAppService, LoanCaseAppService>();
builder.Services.AddScoped<IIncomeAdjustmentAppService, IncomeAdjustmentAppService>();
builder.Services.AddScoped<ILoaningRemarkAppService, LoaningRemarkAppService>();
builder.Services.AddScoped<IDataAttachmentPeriodAppService, DataAttachmentPeriodAppService>();
builder.Services.AddScoped<ILoanDisbursementBatchAppService, LoanDisbursementBatchAppService>();
builder.Services.AddScoped<ILoanRequestAppService, LoanRequestAppService>();

builder.Services.AddScoped<ITextAlertAppService, TextAlertAppService>();
builder.Services.AddScoped<IEmailAlertAppService, EmailAlertAppService>();
builder.Services.AddScoped<IMessageGroupAppService, MessageGroupAppService>();
builder.Services.AddScoped<ICustomerMessageHistoryAppService, CustomerMessageHistoryAppService>();

builder.Services.AddScoped<IDepartmentAppService, DepartmentAppService>();
builder.Services.AddScoped<IDesignationAppService, DesignationAppService>();
builder.Services.AddScoped<IEmployeeAppService, EmployeeAppService>();
builder.Services.AddScoped<IEmployeeDocumentAppService, EmployeeDocumentAppService>();
builder.Services.AddScoped<ISalaryHeadAppService, SalaryHeadAppService>();
builder.Services.AddScoped<ISalaryGroupAppService, SalaryGroupAppService>();
builder.Services.AddScoped<ISalaryCardAppService, SalaryCardAppService>();
builder.Services.AddScoped<ISalaryPeriodAppService, SalaryPeriodAppService>();
builder.Services.AddScoped<IPaySlipAppService, PaySlipAppService>();
builder.Services.AddScoped<IHolidayAppService, HolidayAppService>();
builder.Services.AddScoped<ILeaveApplicationAppService, LeaveApplicationAppService>();
builder.Services.AddScoped<ILeaveTypeAppService, LeaveTypeAppService>();
builder.Services.AddScoped<IEmployeePasswordHistoryAppService, EmployeePasswordHistoryAppService>();
builder.Services.AddScoped<IEmployeeTypeAppService, EmployeeTypeAppService>();
builder.Services.AddScoped<IEmployeeDisciplinaryCaseAppService, EmployeeDisciplinaryCaseAppService>();
builder.Services.AddScoped<IEmployeeAppraisalTargetAppService, EmployeeAppraisalTargetAppService>();
builder.Services.AddScoped<ITrainingPeriodAppService, TrainingPeriodAppService>();
builder.Services.AddScoped<IEmployeeAppraisalPeriodAppService, EmployeeAppraisalPeriodAppService>();
builder.Services.AddScoped<IEmployeeAppraisalAppService, EmployeeAppraisalAppService>();
builder.Services.AddScoped<IExitInterviewQuestionAppService, ExitInterviewQuestionAppService>();
builder.Services.AddScoped<IEmployeeExitAppService, EmployeeExitAppService>();
builder.Services.AddScoped<IExitInterviewAnswerAppService, ExitInterviewAnswerAppService>();

builder.Services.AddScoped<IMicroCreditOfficerAppService, MicroCreditOfficerAppService>();
builder.Services.AddScoped<IMicroCreditGroupAppService, MicroCreditGroupAppService>();

builder.Services.AddScoped<IPurchaseInvoiceAppService, PurchaseInvoiceAppService>();
builder.Services.AddScoped<IPurchaseCreditMemoAppService, PurchaseCreditMemoAppService>();

builder.Services.AddScoped<ISalesInvoiceAppService, SalesInvoiceAppService>();
builder.Services.AddScoped<ISalesCreditMemoAppService, SalesCreditMemoAppService>();

builder.Services.AddScoped<IPaymentAppService, PaymentAppService>();

builder.Services.AddScoped<INumberSeriesGenerator, NumberSeriesGenerator>();

builder.Services.AddScoped<IARCustomerAppService, ARCustomerAppService>();

builder.Services.AddScoped<IReceiptAppService, ReceiptAppService>();

builder.Services.AddScoped<IImprestAppService, ImprestAppService>();

//-> DbContext
builder.Services.AddScoped<IDbContextScopeFactory, DbContextScopeFactory>();
builder.Services.AddScoped<IDbContextFactory, RuntimeContextFactory>();
builder.Services.AddScoped<IAmbientDbContextLocator, AmbientDbContextLocator>();
builder.Services.AddScoped<IBranchAppService, BranchAppService>();

//->Caching
builder.Services.AddScoped<IAppCache, CachingService>();


//-> Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
