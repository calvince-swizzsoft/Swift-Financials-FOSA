using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.DTO.TypeAdapterFactory;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.MicroCreditModule.Services;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.Identity;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Data.MainBoundedContext.Repositories;
using Infrastructure.Data.MainBoundedContext.UnitOfWork;
using LazyCache;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Numero3.EntityFramework.Implementation;
using Numero3.EntityFramework.Interfaces;
using System.Data.Entity;
using System.Runtime.Caching;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace DistributedServices.MainBoundedContext.UnityContainers
{
    /// <summary>
    /// DI container accessor
    /// </summary>
    public static class Container
    {
        #region Properties

        /// <summary>
        /// Get the current configured container
        /// </summary>
        /// <returns>Configured container</returns>
        public static IUnityContainer Current { get; private set; }

        #endregion

        #region Constructor

        static Container()
        {
            ConfigureContainer();
            ConfigureFactories();
        }

        #endregion

        #region Methods

        private static void ConfigureContainer()
        {
            /*
             * Add here the code configuration or the call to configure the container 
             * using the application configuration file
             */

            Current = new UnityContainer();

            //-> Caching
            Current.RegisterType<IAppCache, CachingService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(MemoryCache.Default));

            //-> DbContext
            Current.RegisterType<IDbContextFactory, RuntimeContextFactory>(new ContainerControlledLifetimeManager());
            Current.RegisterType<IDbContextScopeFactory, DbContextScopeFactory>(new ContainerControlledLifetimeManager());
            Current.RegisterType<IAmbientDbContextLocator, AmbientDbContextLocator>(new ContainerControlledLifetimeManager());

            //-> Logging
            Current.RegisterType<ILogger, SerilogLogger>(new ContainerControlledLifetimeManager());

            //-> Adapters
            Current.RegisterType<ITypeAdapterFactory, AutomapperTypeAdapterFactory>(new ContainerControlledLifetimeManager());
            Current.RegisterType<ILoggerFactory, SerilogLoggerFactory>(new ContainerControlledLifetimeManager());

            //-> Repositories	
            Current.RegisterType(typeof(IRepository<>), typeof(Repository<>));

            //-> Navigation services
            Current.RegisterType<INavigationItemAppService, NavigationItemAppService>();
            Current.RegisterType<INavigationItemInRoleAppService, NavigationItemInRoleAppService>();

            //Authentication
            Current.RegisterType<IMembershipService, MembershipService>();
            Current.RegisterType<UserManager<ApplicationUser>>(new TransientLifetimeManager());
            Current.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new TransientLifetimeManager());
            Current.RegisterType<RoleStore<IdentityRole>, RoleStore<IdentityRole>>(new TransientLifetimeManager());
            Current.RegisterType<DbContext, ApplicationDbContext>(new TransientLifetimeManager(), new InjectionConstructor("AuthStore"));
            Current.RegisterType<ApplicationUserManager>();
            Current.RegisterType<ApplicationRoleManager>();
            Current.RegisterType<RoleManager<IdentityRole>>(new TransientLifetimeManager());

            //-> Application services
            Current.RegisterType<IAuditLogAppService, AuditLogAppService>();
            Current.RegisterType<IFinancialsService, FinancialsService>();
            Current.RegisterType<IMediaAppService, MediaAppService>();
            Current.RegisterType<ISqlCommandAppService, SqlCommandAppService>();
            Current.RegisterType<IJournalEntryPostingService, JournalEntryPostingService>();
            Current.RegisterType<IEnumerationAppService, EnumerationAppService>();
            Current.RegisterType<ISmtpService, SmtpService>();
            Current.RegisterType<IMessageQueueService, MessageQueueService>();
            Current.RegisterType<IBrokerService, BrokerService>();

            Current.RegisterType<IChartOfAccountAppService, ChartOfAccountAppService>();
            Current.RegisterType<ICostCenterAppService, CostCenterAppService>();
            Current.RegisterType<ITellerAppService, TellerAppService>();
            Current.RegisterType<IPostingPeriodAppService, PostingPeriodAppService>();
            Current.RegisterType<IBankLinkageAppService, BankLinkageAppService>();
            Current.RegisterType<ICustomerAccountAppService, CustomerAccountAppService>();
            Current.RegisterType<IJournalAppService, JournalAppService>();
            Current.RegisterType<IJournalEntryAppService, JournalEntryAppService>();
            Current.RegisterType<ICommissionAppService, CommissionAppService>();
            Current.RegisterType<ILevyAppService, LevyAppService>();
            Current.RegisterType<ISavingsProductAppService, SavingsProductAppService>();
            Current.RegisterType<IInvestmentProductAppService, InvestmentProductAppService>();
            Current.RegisterType<ILoanProductAppService, LoanProductAppService>();
            Current.RegisterType<IChequeTypeAppService, ChequeTypeAppService>();
            Current.RegisterType<IDynamicChargeAppService, DynamicChargeAppService>();
            Current.RegisterType<IStandingOrderAppService, StandingOrderAppService>();
            Current.RegisterType<IJournalVoucherAppService, JournalVoucherAppService>();
            Current.RegisterType<ICreditTypeAppService, CreditTypeAppService>();
            Current.RegisterType<ICreditBatchAppService, CreditBatchAppService>();
            Current.RegisterType<IBudgetAppService, BudgetAppService>();
            Current.RegisterType<IAlternateChannelAppService, AlternateChannelAppService>();
            Current.RegisterType<ITreasuryAppService, TreasuryAppService>();
            Current.RegisterType<IOverDeductionBatchAppService, OverDeductionBatchAppService>();
            Current.RegisterType<IAlternateChannelLogAppService, AlternateChannelLogAppService>();
            Current.RegisterType<IReportTemplateAppService, ReportTemplateAppService>();
            Current.RegisterType<IInsuranceCompanyAppService, InsuranceCompanyAppService>();
            Current.RegisterType<IDebitTypeAppService, DebitTypeAppService>();
            Current.RegisterType<IDebitBatchAppService, DebitBatchAppService>();
            Current.RegisterType<IChequeBookAppService, ChequeBookAppService>();
            Current.RegisterType<IUnPayReasonAppService, UnPayReasonAppService>();
            Current.RegisterType<IWireTransferBatchAppService, WireTransferBatchAppService>();
            Current.RegisterType<IDirectDebitAppService, DirectDebitAppService>();
            Current.RegisterType<IRecurringBatchAppService, RecurringBatchAppService>();
            Current.RegisterType<IBankReconciliationPeriodAppService, BankReconciliationPeriodAppService>();
            Current.RegisterType<IJournalReversalBatchAppService, JournalReversalBatchAppService>();
            Current.RegisterType<IInterAccountTransferBatchAppService, InterAccountTransferBatchAppService>();
            Current.RegisterType<IMobileToBankRequestAppService, MobileToBankRequestAppService>();
            Current.RegisterType<IGeneralLedgerAppService, GeneralLedgerAppService>();
            Current.RegisterType<IAlternateChannelReconciliationPeriodAppService, AlternateChannelReconciliationPeriodAppService>();
            Current.RegisterType<IFixedDepositTypeAppService, FixedDepositTypeAppService>();
            Current.RegisterType<IWireTransferTypeAppService, WireTransferTypeAppService>();
            Current.RegisterType<IElectronicStatementOrderAppService, ElectronicStatementOrderAppService>();
            Current.RegisterType<ISuperSaverPayableAppService, SuperSaverPayableAppService>();
            Current.RegisterType<IBankToMobileRequestAppService, BankToMobileRequestAppService>();
            Current.RegisterType<IBrokerRequestAppService, BrokerRequestAppService>();
            Current.RegisterType<ISystemGeneralLedgerAccountMappingService, SystemGeneralLedgerAccountMappingService>();

            Current.RegisterType<IFiscalCountAppService, FiscalCountAppService>();
            Current.RegisterType<IExternalChequeAppService, ExternalChequeAppService>();
            Current.RegisterType<IInHouseChequeAppService, InHouseChequeAppService>();
            Current.RegisterType<IFixedDepositAppService, FixedDepositAppService>();
            Current.RegisterType<IElectronicJournalAppService, ElectronicJournalAppService>();
            Current.RegisterType<IExpensePayableAppService, ExpensePayableAppService>();
            Current.RegisterType<IAccountClosureRequestAppService, AccountClosureRequestAppService>();
            Current.RegisterType<ICashWithdrawalRequestAppService, CashWithdrawalRequestAppService>();
            Current.RegisterType<ICashTransferRequestAppService, CashTransferRequestAppService>();
            Current.RegisterType<ICashDepositRequestAppService, CashDepositRequestAppService>();

            Current.RegisterType<IAuthorizationAppService, AuthorizationAppService>();
            Current.RegisterType<IReportAppService, ReportAppService>();
            Current.RegisterType<IBranchAppService, BranchAppService>();
            Current.RegisterType<IBankAppService, BankAppService>();
            Current.RegisterType<ICompanyAppService, CompanyAppService>();
            Current.RegisterType<ILocationAppService, LocationAppService>();
            Current.RegisterType<IWorkflowAppService, WorkflowAppService>();
            Current.RegisterType<IWorkflowProcessorAppService, WorkflowProcessorAppService>();

            Current.RegisterType<IEmployerAppService, EmployerAppService>();
            Current.RegisterType<IZoneAppService, ZoneAppService>();
            Current.RegisterType<IDivisionAppService, DivisionAppService>();
            Current.RegisterType<ICustomerAppService, CustomerAppService>();
            Current.RegisterType<ICustomerDocumentAppService, CustomerDocumentAppService>();
            Current.RegisterType<IFileRegisterAppService, FileRegisterAppService>();
            Current.RegisterType<IDelegateAppService, DelegateAppService>();
            Current.RegisterType<IDirectorAppService, DirectorAppService>();
            Current.RegisterType<IWithdrawalNotificationAppService, WithdrawalNotificationAppService>();
            Current.RegisterType<ICommissionExemptionAppService, CommissionExemptionAppService>();
            Current.RegisterType<IEducationVenueAppService, EducationVenueAppService>();
            Current.RegisterType<IEducationRegisterAppService, EducationRegisterAppService>();
            Current.RegisterType<IConditionalLendingAppService, ConditionalLendingAppService>();
            Current.RegisterType<IFuneralRiderClaimAppService, FuneralRiderClaimAppService>();
            Current.RegisterType<IAdministrativeDivisionAppService, AdministrativeDivisionAppService>();

            Current.RegisterType<ILoanPurposeAppService, LoanPurposeAppService>();
            Current.RegisterType<ILoanCaseAppService, LoanCaseAppService>();
            Current.RegisterType<IIncomeAdjustmentAppService, IncomeAdjustmentAppService>();
            Current.RegisterType<ILoaningRemarkAppService, LoaningRemarkAppService>();
            Current.RegisterType<IDataAttachmentPeriodAppService, DataAttachmentPeriodAppService>();
            Current.RegisterType<ILoanDisbursementBatchAppService, LoanDisbursementBatchAppService>();
            Current.RegisterType<ILoanRequestAppService, LoanRequestAppService>();

            Current.RegisterType<ITextAlertAppService, TextAlertAppService>();
            Current.RegisterType<IEmailAlertAppService, EmailAlertAppService>();
            Current.RegisterType<IMessageGroupAppService, MessageGroupAppService>();
            Current.RegisterType<ICustomerMessageHistoryAppService, CustomerMessageHistoryAppService>();

            Current.RegisterType<IDepartmentAppService, DepartmentAppService>();
            Current.RegisterType<IDesignationAppService, DesignationAppService>();
            Current.RegisterType<IEmployeeAppService, EmployeeAppService>();
            Current.RegisterType<IEmployeeDocumentAppService, EmployeeDocumentAppService>();
            Current.RegisterType<ISalaryHeadAppService, SalaryHeadAppService>();
            Current.RegisterType<ISalaryGroupAppService, SalaryGroupAppService>();
            Current.RegisterType<ISalaryCardAppService, SalaryCardAppService>();
            Current.RegisterType<ISalaryPeriodAppService, SalaryPeriodAppService>();
            Current.RegisterType<IPaySlipAppService, PaySlipAppService>();
            Current.RegisterType<IHolidayAppService, HolidayAppService>();
            Current.RegisterType<ILeaveApplicationAppService, LeaveApplicationAppService>();
            Current.RegisterType<ILeaveTypeAppService, LeaveTypeAppService>();
            Current.RegisterType<IEmployeePasswordHistoryAppService, EmployeePasswordHistoryAppService>();
            Current.RegisterType<IEmployeeTypeAppService, EmployeeTypeAppService>();
            Current.RegisterType<IEmployeeDisciplinaryCaseAppService, EmployeeDisciplinaryCaseAppService>();
            Current.RegisterType<IEmployeeAppraisalTargetAppService, EmployeeAppraisalTargetAppService>();
            Current.RegisterType<ITrainingPeriodAppService, TrainingPeriodAppService>();
            Current.RegisterType<IEmployeeAppraisalPeriodAppService, EmployeeAppraisalPeriodAppService>();
            Current.RegisterType<IEmployeeAppraisalAppService, EmployeeAppraisalAppService>();
            Current.RegisterType<IExitInterviewQuestionAppService, ExitInterviewQuestionAppService>();
            Current.RegisterType<IEmployeeExitAppService, EmployeeExitAppService>();
            Current.RegisterType<IExitInterviewAnswerAppService, ExitInterviewAnswerAppService>();

            Current.RegisterType<IMicroCreditOfficerAppService, MicroCreditOfficerAppService>();
            Current.RegisterType<IMicroCreditGroupAppService, MicroCreditGroupAppService>();

            Current.RegisterType<IPurchaseInvoiceAppService, PurchaseInvoiceAppService>();
            
                Current.RegisterType<IPurchaseCreditMemoAppService, PurchaseCreditMemoAppService>();
            
            Current.RegisterType<ISalesInvoiceAppService, SalesInvoiceAppService>();

            Current.RegisterType<ISalesCreditMemoAppService, SalesCreditMemoAppService>();

            Current.RegisterType<IPaymentAppService, PaymentAppService>();

            Current.RegisterType<INumberSeriesGenerator, NumberSeriesGenerator>();


            Current.RegisterType<IARCustomerAppService, ARCustomerAppService>();

            Current.RegisterType<IReceiptAppService, ReceiptAppService>();


            Current.RegisterType<IImprestAppService, ImprestAppService>();

            //-> Distributed Services
        }

        private static void ConfigureFactories()
        {
            var loggerFactory = Current.Resolve<ILoggerFactory>();
            LoggerFactory.SetCurrent(loggerFactory);

            var typeAdapterFactory = Current.Resolve<ITypeAdapterFactory>();
            TypeAdapterFactory.SetCurrent(typeAdapterFactory);
        }

        #endregion
    }
}