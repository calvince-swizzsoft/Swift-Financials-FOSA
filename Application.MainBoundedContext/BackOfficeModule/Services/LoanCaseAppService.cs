using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.RegistryModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.AttachedLoanAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanAppraisalFactorAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCollateralAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryEntryAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorSubstituteAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public class LoanCaseAppService : ILoanCaseAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<LoanCase> _loanCaseRepository;
        private readonly IRepository<LoanGuarantor> _loanGuarantorRepository;
        private readonly IRepository<LoanAppraisalFactor> _loanAppraisalFactorRepository;
        private readonly IRepository<LoanCollateral> _loanCollateralRepository;
        private readonly IRepository<LoanGuarantorSubstitute> _loanGuarantorSubstituteRepository;
        private readonly IRepository<AttachedLoan> _attachedLoanRepository;
        private readonly IRepository<LoanDisbursementBatchEntry> _loanDisbursementBatchEntryRepository;
        private readonly IRepository<LoanGuarantorAttachmentHistory> _loanGuarantorAttachmentHistoryRepository;
        private readonly IRepository<LoanGuarantorAttachmentHistoryEntry> _loanGuarantorAttachmentHistoryEntryRepository;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly IStandingOrderAppService _standingOrderAppService;
        private readonly IFinancialsService _financialsService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly ICustomerAppService _customerAppService;
        private readonly ICustomerDocumentAppService _customerDocumentAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IBrokerService _brokerService;

        public LoanCaseAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<LoanCase> loanCaseRepository,
           IRepository<LoanGuarantor> loanGuarantorRepository,
           IRepository<LoanAppraisalFactor> loanAppraisalFactorRepository,
           IRepository<LoanCollateral> loanCollateralRepository,
           IRepository<LoanGuarantorSubstitute> loanGuarantorSubstituteRepository,
           IRepository<AttachedLoan> attachedLoanRepository,
           IRepository<LoanDisbursementBatchEntry> loanDisbursementBatchEntryRepository,
           IRepository<LoanGuarantorAttachmentHistory> loanGuarantorAttachmentHistoryRepository,
           IRepository<LoanGuarantorAttachmentHistoryEntry> loanGuarantorAttachmentHistoryEntryRepository,
           ICustomerAccountAppService customerAccountAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           IJournalAppService journalAppService,
           IStandingOrderAppService standingOrderAppService,
           IFinancialsService financialsService,
           ISavingsProductAppService savingsProductAppService,
           ILoanProductAppService loanProductAppService,
           ICustomerAppService customerAppService,
           ICustomerDocumentAppService customerDocumentAppService,
           IJournalEntryPostingService journalEntryPostingService,
           IPostingPeriodAppService postingPeriodAppService,
           ICommissionAppService commissionAppService,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (loanCaseRepository == null)
                throw new ArgumentNullException(nameof(loanCaseRepository));

            if (loanGuarantorRepository == null)
                throw new ArgumentNullException(nameof(loanGuarantorRepository));

            if (loanCollateralRepository == null)
                throw new ArgumentNullException(nameof(loanCollateralRepository));

            if (loanAppraisalFactorRepository == null)
                throw new ArgumentNullException(nameof(loanAppraisalFactorRepository));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (standingOrderAppService == null)
                throw new ArgumentNullException(nameof(standingOrderAppService));

            if (financialsService == null)
                throw new ArgumentNullException(nameof(financialsService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (loanGuarantorSubstituteRepository == null)
                throw new ArgumentNullException(nameof(loanGuarantorSubstituteRepository));

            if (attachedLoanRepository == null)
                throw new ArgumentNullException(nameof(attachedLoanRepository));

            if (loanDisbursementBatchEntryRepository == null)
                throw new ArgumentNullException(nameof(loanDisbursementBatchEntryRepository));

            if (loanGuarantorAttachmentHistoryRepository == null)
                throw new ArgumentNullException(nameof(loanGuarantorAttachmentHistoryRepository));

            if (loanGuarantorAttachmentHistoryEntryRepository == null)
                throw new ArgumentNullException(nameof(loanGuarantorAttachmentHistoryEntryRepository));

            if (customerAppService == null)
                throw new ArgumentNullException(nameof(customerAppService));

            if (customerDocumentAppService == null)
                throw new ArgumentNullException(nameof(customerDocumentAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _loanCaseRepository = loanCaseRepository;
            _loanGuarantorRepository = loanGuarantorRepository;
            _loanCollateralRepository = loanCollateralRepository;
            _loanAppraisalFactorRepository = loanAppraisalFactorRepository;
            _loanGuarantorSubstituteRepository = loanGuarantorSubstituteRepository;
            _attachedLoanRepository = attachedLoanRepository;
            _loanDisbursementBatchEntryRepository = loanDisbursementBatchEntryRepository;
            _loanGuarantorAttachmentHistoryRepository = loanGuarantorAttachmentHistoryRepository;
            _loanGuarantorAttachmentHistoryEntryRepository = loanGuarantorAttachmentHistoryEntryRepository;
            _customerAccountAppService = customerAccountAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _journalAppService = journalAppService;
            _standingOrderAppService = standingOrderAppService;
            _financialsService = financialsService;
            _savingsProductAppService = savingsProductAppService;
            _loanProductAppService = loanProductAppService;
            _customerAppService = customerAppService;
            _customerDocumentAppService = customerDocumentAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _postingPeriodAppService = postingPeriodAppService;
            _commissionAppService = commissionAppService;
            _brokerService = brokerService;
        }

        public LoanCaseDTO AddNewLoanCase(LoanCaseDTO loanCaseDTO, ServiceHeader serviceHeader)
        {
            if (loanCaseDTO != null)
            {
                var existingLoanCases = FindLoanCasesByCustomerIdAndLoanProductId(loanCaseDTO.CustomerId, loanCaseDTO.LoanProductId, serviceHeader);

                if (existingLoanCases != null && existingLoanCases.Any(x => x.Status.In((int)LoanCaseStatus.Registered, (int)LoanCaseStatus.Appraised, (int)LoanCaseStatus.Deferred, (int)LoanCaseStatus.Approved, (int)LoanCaseStatus.Audited)))
                {
                    loanCaseDTO.ErrorMessageResult = string.Format("Sorry, but selected customer has a loan application for the selected loan product currently undergoing processing!");

                    return loanCaseDTO;
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var loanInterest = new LoanInterest(loanCaseDTO.LoanInterestAnnualPercentageRate, loanCaseDTO.LoanInterestChargeMode, loanCaseDTO.LoanInterestRecoveryMode, loanCaseDTO.LoanInterestCalculationMode);

                    var loanRegistration = new LoanRegistration(loanCaseDTO.LoanRegistrationTermInMonths, loanCaseDTO.LoanRegistrationMinimumAmount, loanCaseDTO.LoanRegistrationMaximumAmount, loanCaseDTO.LoanRegistrationMinimumInterestAmount, loanCaseDTO.LoanRegistrationLoanProductSection, loanCaseDTO.LoanRegistrationLoanProductCategory, loanCaseDTO.LoanRegistrationConsecutiveIncome, loanCaseDTO.LoanRegistrationInvestmentsMultiplier, loanCaseDTO.LoanRegistrationMinimumGuarantors, loanCaseDTO.LoanRegistrationMaximumGuarantees, loanCaseDTO.LoanRegistrationRejectIfMemberHasBalance, loanCaseDTO.LoanRegistrationSecurityRequired, loanCaseDTO.LoanRegistrationAllowSelfGuarantee, loanCaseDTO.LoanRegistrationGracePeriod, loanCaseDTO.LoanRegistrationMinimumMembershipPeriod, loanCaseDTO.LoanRegistrationPaymentFrequencyPerYear, loanCaseDTO.LoanRegistrationPaymentDueDate, loanCaseDTO.LoanRegistrationPayoutRecoveryMode, loanCaseDTO.LoanRegistrationPayoutRecoveryPercentage, loanCaseDTO.LoanRegistrationAggregateCheckOffRecoveryMode, loanCaseDTO.LoanRegistrationChargeClearanceFee, loanCaseDTO.LoanRegistrationMicrocredit, loanCaseDTO.LoanRegistrationStandingOrderTrigger, loanCaseDTO.LoanRegistrationTrackArrears, loanCaseDTO.LoanRegistrationChargeArrearsFee, loanCaseDTO.LoanRegistrationEnforceSystemAppraisalRecommendation, loanCaseDTO.LoanRegistrationBypassAudit, loanCaseDTO.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage, loanCaseDTO.LoanRegistrationGuarantorSecurityMode, loanCaseDTO.LoanRegistrationRoundingType, loanCaseDTO.LoanRegistrationDisburseMicroLoanLessDeductions, loanCaseDTO.LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement, loanCaseDTO.LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal, loanCaseDTO.LoanRegistrationThrottleScheduledArrearsRecovery, loanCaseDTO.LoanRegistrationCreateStandingOrderOnLoanAudit);

                    var takeHome = new Charge(loanCaseDTO.TakeHomeType, loanCaseDTO.TakeHomePercentage, loanCaseDTO.TakeHomeFixedAmount);

                    var loanCase = LoanCaseFactory.CreateLoanCase(loanCaseDTO.ParentId, loanCaseDTO.BranchId, loanCaseDTO.CustomerId, loanCaseDTO.LoanProductId, loanCaseDTO.LoanPurposeId, loanCaseDTO.SavingsProductId, loanCaseDTO.Remarks, loanCaseDTO.AmountApplied, loanCaseDTO.ReceivedDate, loanCaseDTO.LoanProductInvestmentsBalance, loanCaseDTO.LoanProductLoanBalance, loanCaseDTO.TotalLoansBalance, loanCaseDTO.LoanProductLatestIncome, loanCaseDTO.Reference, loanInterest, loanRegistration, loanCaseDTO.MaximumAmountPercentage, takeHome);

                    loanCase.CaseNumber = _loanCaseRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(CaseNumber),0) + 1 AS Expr1 FROM {0}LoanCases", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                    loanCase.Status = loanCaseDTO.Status;
                    loanCase.CreatedBy = serviceHeader.ApplicationUserName;

                    _loanCaseRepository.Add(loanCase, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) > 0 ? loanCase.ProjectedAs<LoanCaseDTO>() : null;
                }
            }
            else return null;
        }

        public bool AppraiseLoanCase(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var appraisalOk = default(bool);

            if (loanCaseDTO != null && Enum.IsDefined(typeof(LoanAppraisalOption), loanAppraisalOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanCaseDTO.Id, serviceHeader);
                    persisted.Status = (int)LoanCaseStatus.Registered;

                    if (persisted != null && (persisted.Status == (int)LoanCaseStatus.Registered || persisted.Status == (int)LoanCaseStatus.Deferred))
                    {
                        switch ((LoanAppraisalOption)loanAppraisalOption)
                        {
                            case LoanAppraisalOption.Appraise:

                                persisted.Status = (int)LoanCaseStatus.Appraised;

                                persisted.LoanProductLatestIncome = loanCaseDTO.LoanProductLatestIncome;

                                persisted.AppraisedNetIncome = loanCaseDTO.AppraisedNetIncome;
                                persisted.AppraisedAbility = loanCaseDTO.AppraisedAbility;

                                persisted.SystemAppraisedAmount = loanCaseDTO.SystemAppraisedAmount;
                                persisted.SystemAppraisalRemarks = loanCaseDTO.SystemAppraisalRemarks;

                                persisted.AppraisedAmount = loanCaseDTO.AppraisedAmount;
                                persisted.AppraisedAmountRemarks = loanCaseDTO.AppraisedAmountRemarks;

                                persisted.AppraisalRemarks = loanCaseDTO.AppraisalRemarks;

                                persisted.MonthlyPaybackAmount = loanCaseDTO.MonthlyPaybackAmount;
                                persisted.TotalPaybackAmount = loanCaseDTO.TotalPaybackAmount;

                                persisted.TotalLoansBalance = loanCaseDTO.TotalLoansBalance;

                                persisted.AppraisedBy = serviceHeader.ApplicationUserName;
                                persisted.AppraisedDate = DateTime.Now;

                                break;
                            case LoanAppraisalOption.Reject:

                                persisted.Status = (int)LoanCaseStatus.Rejected;
                                ReleaseLoanCaseGuarantors(persisted.Id, serviceHeader);

                                break;
                            default:
                                break;
                        }

                        appraisalOk = dbContextScope.SaveChanges(serviceHeader) > 0;

                        if (appraisalOk && loanAppraisalOption == (int)LoanAppraisalOption.Appraise)
                        {
                            #region Do we need to send alerts?

                            _brokerService.ProcessLoanGuaranteeAccountAlerts(DMLCommand.None, serviceHeader, loanCaseDTO);

                            #endregion
                        }
                    }
                }
            }

            return appraisalOk;
        }

        public async Task<bool> AppraiseLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAppraisalOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var appraisalOk = default(bool);

            if (loanCaseDTO != null && Enum.IsDefined(typeof(LoanAppraisalOption), loanAppraisalOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = await _loanCaseRepository.GetAsync(loanCaseDTO.Id, serviceHeader);
                    persisted.Status = (int)LoanCaseStatus.Registered;

                    if (persisted != null && (persisted.Status == (int)LoanCaseStatus.Registered || persisted.Status == (int)LoanCaseStatus.Deferred))
                    {
                        switch ((LoanAppraisalOption)loanAppraisalOption)
                        {
                            case LoanAppraisalOption.Appraise:

                                persisted.Status = (int)LoanCaseStatus.Appraised;

                                persisted.LoanProductLatestIncome = loanCaseDTO.LoanProductLatestIncome;

                                persisted.AppraisedNetIncome = loanCaseDTO.AppraisedNetIncome;
                                persisted.AppraisedAbility = loanCaseDTO.AppraisedAbility;

                                persisted.SystemAppraisedAmount = loanCaseDTO.SystemAppraisedAmount;
                                persisted.SystemAppraisalRemarks = loanCaseDTO.SystemAppraisalRemarks;

                                persisted.AppraisedAmount = loanCaseDTO.AppraisedAmount;
                                persisted.AppraisedAmountRemarks = loanCaseDTO.AppraisedAmountRemarks;

                                persisted.AppraisalRemarks = loanCaseDTO.AppraisalRemarks;

                                persisted.MonthlyPaybackAmount = loanCaseDTO.MonthlyPaybackAmount;
                                persisted.TotalPaybackAmount = loanCaseDTO.TotalPaybackAmount;

                                persisted.TotalLoansBalance = loanCaseDTO.TotalLoansBalance;

                                persisted.AppraisedBy = serviceHeader.ApplicationUserName;
                                persisted.AppraisedDate = DateTime.Now;

                                break;
                            case LoanAppraisalOption.Reject:

                                persisted.Status = (int)LoanCaseStatus.Rejected;
                                await ReleaseLoanCaseGuarantorsAsync(persisted.Id, serviceHeader);

                                break;
                            default:
                                break;
                        }

                        appraisalOk = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;

                        if (appraisalOk && loanAppraisalOption == (int)LoanAppraisalOption.Appraise)
                        {
                            #region Do we need to send alerts?

                            _brokerService.ProcessLoanGuaranteeAccountAlerts(DMLCommand.None, serviceHeader, loanCaseDTO);

                            #endregion
                        }
                    }
                }
            }

            return appraisalOk;
        }

        public bool ApproveLoanCase(LoanCaseDTO loanCaseDTO, int loanApprovalOption, ServiceHeader serviceHeader)
        {
            var approvalOK = default(bool);

            if (loanCaseDTO != null && Enum.IsDefined(typeof(LoanApprovalOption), loanApprovalOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanCaseDTO.Id, serviceHeader);
                    persisted.Status = (int)LoanCaseStatus.Appraised;

                    if (persisted != null && persisted.Status == (int)LoanCaseStatus.Appraised)
                    {
                        switch ((LoanApprovalOption)loanApprovalOption)
                        {
                            case LoanApprovalOption.Approve:

                                persisted.Status = (int)LoanCaseStatus.Approved;

                                persisted.MonthlyPaybackAmount = loanCaseDTO.MonthlyPaybackAmount;
                                persisted.TotalPaybackAmount = loanCaseDTO.TotalPaybackAmount;

                                persisted.ApprovedAmount = loanCaseDTO.ApprovedAmount;
                                persisted.ApprovedAmountRemarks = loanCaseDTO.ApprovedAmountRemarks;

                                persisted.ApprovedPrincipalPayment = loanCaseDTO.ApprovedPrincipalPayment;
                                persisted.ApprovedInterestPayment = loanCaseDTO.ApprovedInterestPayment;

                                persisted.ApprovalRemarks = loanCaseDTO.ApprovalRemarks;

                                persisted.ApprovedBy = serviceHeader.ApplicationUserName;
                                persisted.ApprovedDate = DateTime.Now;

                                if (persisted.LoanRegistration.BypassAudit)
                                {
                                    loanCaseDTO.AuditRemarks = "(Verification Bypassed)";
                                    AuditLoanCase(loanCaseDTO, (int)LoanAuditOption.Audit, serviceHeader);
                                }

                                break;
                            case LoanApprovalOption.Reject:

                                persisted.Status = (int)LoanCaseStatus.Rejected;
                                ReleaseLoanCaseGuarantors(persisted.Id, serviceHeader);

                                break;
                            case LoanApprovalOption.Defer:

                                persisted.Status = (int)LoanCaseStatus.Deferred;

                                #region Do we need to send alerts?

                                _brokerService.ProcessLoanDeferredAccountAlerts(DMLCommand.None, serviceHeader, loanCaseDTO);

                                #endregion

                                break;
                            default:
                                break;
                        }

                        approvalOK = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
            }

            return approvalOK;
        }

        public async Task<bool> ApproveLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanApprovalOption, ServiceHeader serviceHeader)
        {
            var approvalOK = default(bool);

            if (loanCaseDTO != null && Enum.IsDefined(typeof(LoanApprovalOption), loanApprovalOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = await _loanCaseRepository.GetAsync(loanCaseDTO.Id, serviceHeader);
                    persisted.Status = (int)LoanCaseStatus.Appraised;
                    if (persisted != null && persisted.Status == (int)LoanCaseStatus.Appraised)
                    {
                        switch ((LoanApprovalOption)loanApprovalOption)
                        {
                            case LoanApprovalOption.Approve:

                                persisted.Status = (int)LoanCaseStatus.Approved;

                                persisted.MonthlyPaybackAmount = loanCaseDTO.MonthlyPaybackAmount;
                                persisted.TotalPaybackAmount = loanCaseDTO.TotalPaybackAmount;

                                persisted.ApprovedAmount = loanCaseDTO.ApprovedAmount;
                                persisted.ApprovedAmountRemarks = loanCaseDTO.ApprovedAmountRemarks;

                                persisted.ApprovedPrincipalPayment = loanCaseDTO.ApprovedPrincipalPayment;
                                persisted.ApprovedInterestPayment = loanCaseDTO.ApprovedInterestPayment;

                                persisted.ApprovalRemarks = loanCaseDTO.ApprovalRemarks;

                                persisted.ApprovedBy = serviceHeader.ApplicationUserName;
                                persisted.ApprovedDate = DateTime.Now;

                                if (persisted.LoanRegistration.BypassAudit)
                                {
                                    loanCaseDTO.AuditRemarks = "(Verification Bypassed)";
                                    await AuditLoanCaseAsync(loanCaseDTO, (int)LoanAuditOption.Audit, serviceHeader);
                                }

                                break;
                            case LoanApprovalOption.Reject:

                                persisted.Status = (int)LoanCaseStatus.Rejected;
                                await ReleaseLoanCaseGuarantorsAsync(persisted.Id, serviceHeader);

                                break;
                            case LoanApprovalOption.Defer:

                                persisted.Status = (int)LoanCaseStatus.Deferred;

                                #region Do we need to send alerts?

                                _brokerService.ProcessLoanDeferredAccountAlerts(DMLCommand.None, serviceHeader, loanCaseDTO);

                                #endregion

                                break;
                            default:
                                break;
                        }

                        approvalOK = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                    }
                }
            }

            return approvalOK;
        }

        public bool AuditLoanCase(LoanCaseDTO loanCaseDTO, int loanAuditOption, ServiceHeader serviceHeader)
        {
            var auditOK = default(bool);

            if (loanCaseDTO != null && Enum.IsDefined(typeof(LoanAuditOption), loanAuditOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanCaseDTO.Id, serviceHeader);
                    persisted.Status = (int)LoanCaseStatus.Approved;

                    if (persisted != null && persisted.Status == (int)LoanCaseStatus.Approved)
                    {
                        switch ((LoanAuditOption)loanAuditOption)
                        {
                            case LoanAuditOption.Audit:

                                persisted.Status = (int)LoanCaseStatus.Audited;

                                persisted.AuditRemarks = loanCaseDTO.AuditRemarks;

                                persisted.AuditedBy = serviceHeader.ApplicationUserName;
                                persisted.AuditedDate = DateTime.Now;

                                CustomerAccountDTO customerLoanAccountDTO = null;

                                CustomerAccountDTO customerSavingsAccountDTO = null;

                                if (persisted.LoanRegistration.CreateStandingOrderOnLoanAudit)
                                {
                                    #region create loan account?

                                    var customerLoanAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, persisted.LoanProductId, serviceHeader);

                                    if (customerLoanAccounts != null && customerLoanAccounts.Any())
                                        customerLoanAccountDTO = customerLoanAccounts.First();
                                    else
                                    {
                                        var customerAccountDTO = new CustomerAccountDTO
                                        {
                                            BranchId = persisted.BranchId,
                                            CustomerId = persisted.CustomerId,
                                            CustomerAccountTypeProductCode = (int)ProductCode.Loan,
                                            CustomerAccountTypeTargetProductId = persisted.LoanProductId,
                                            CustomerAccountTypeTargetProductCode = persisted.LoanProduct.Code,
                                            Status = (int)CustomerAccountStatus.Normal,
                                            RecordStatus = (int)RecordStatus.Approved,
                                        };

                                        customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                        if (customerAccountDTO != null)
                                            customerLoanAccountDTO = customerAccountDTO;
                                    }

                                    #endregion

                                    #region create savings account?

                                    if (persisted.SavingsProduct != null)
                                    {
                                        var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, persisted.SavingsProduct.Id, serviceHeader);

                                        if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                                            customerSavingsAccountDTO = customerSavingsAccounts.First();
                                        else
                                        {
                                            var customerAccountDTO = new CustomerAccountDTO
                                            {
                                                BranchId = persisted.BranchId,
                                                CustomerId = persisted.CustomerId,
                                                CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                                                CustomerAccountTypeTargetProductId = persisted.SavingsProduct.Id,
                                                CustomerAccountTypeTargetProductCode = persisted.SavingsProduct.Code,
                                                Status = (int)CustomerAccountStatus.Normal,
                                            };

                                            customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                            if (customerAccountDTO != null)
                                                customerSavingsAccountDTO = customerAccountDTO;
                                        }
                                    }

                                    #endregion

                                    #region  create standing order?

                                    if (customerLoanAccountDTO != null && customerSavingsAccountDTO != null)
                                    {
                                        #region 2. Do we need to recover upfront dynamic charges from loan?

                                        var loanAccountDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(persisted.LoanProductId, (int)DynamicChargeRecoverySource.LoanAccount, (int)DynamicChargeRecoveryMode.Upfront, persisted.ApprovedAmount, customerLoanAccountDTO, serviceHeader, persisted.LoanRegistration.TermInMonths);

                                        if (loanAccountDynamicChargeTariffs != null && loanAccountDynamicChargeTariffs.Any())
                                        {
                                            // bookmark top-up amount
                                            persisted.AuditTopUpAmount = loanAccountDynamicChargeTariffs.Sum(x => x.Amount);
                                        }

                                        #endregion

                                        decimal PV = (persisted.ApprovedAmount + persisted.AuditTopUpAmount);

                                        decimal Pmt = (decimal)_financialsService.Pmt(persisted.LoanInterest.CalculationMode, persisted.LoanRegistration.TermInMonths, persisted.LoanRegistration.PaymentFrequencyPerYear, persisted.LoanInterest.AnnualPercentageRate, -(double)PV, 0d, persisted.LoanRegistration.PaymentDueDate);

                                        var repaymentSchedule = _financialsService.RepaymentSchedule(persisted.LoanRegistration.TermInMonths, persisted.LoanRegistration.PaymentFrequencyPerYear, persisted.LoanRegistration.GracePeriod, persisted.LoanInterest.CalculationMode, persisted.LoanInterest.AnnualPercentageRate, -(double)PV, 0d, persisted.LoanRegistration.PaymentDueDate);

                                        // do we need to reset?
                                        var chargeableFirstInterestValue = Math.Max(repaymentSchedule.First().InterestPayment, persisted.LoanRegistration.MinimumInterestAmount);

                                        var existingStandingOrders = _standingOrderAppService.FindStandingOrders(customerSavingsAccountDTO.Id, customerLoanAccountDTO.Id, serviceHeader);

                                        if (existingStandingOrders != null && existingStandingOrders.Any())
                                        {
                                            var targetStandingOrder = existingStandingOrders.FirstOrDefault();

                                            if (targetStandingOrder != null)
                                            {
                                                targetStandingOrder.ChargeType = (int)ChargeType.FixedAmount;
                                                targetStandingOrder.Trigger = persisted.LoanRegistration.StandingOrderTrigger;
                                                targetStandingOrder.LoanAmount = PV;
                                                targetStandingOrder.PaymentPerPeriod = Pmt;
                                                targetStandingOrder.Principal = persisted.ApprovedPrincipalPayment != 0m ? persisted.ApprovedPrincipalPayment : repaymentSchedule.First().PrincipalPayment;
                                                targetStandingOrder.Interest = Math.Max(chargeableFirstInterestValue, persisted.ApprovedInterestPayment != 0m ? persisted.ApprovedInterestPayment : repaymentSchedule.First().InterestPayment);
                                                targetStandingOrder.DurationStartDate = repaymentSchedule.First().DueDate;
                                                targetStandingOrder.DurationEndDate = repaymentSchedule.Last().DueDate;
                                                targetStandingOrder.ScheduleFrequency = persisted.LoanRegistration.PaymentFrequencyPerYear;
                                                targetStandingOrder.IsLocked = false;
                                                targetStandingOrder.Remarks = string.Empty;
                                                targetStandingOrder.BeneficiaryProductProductCode = (int)ProductCode.Loan;
                                                targetStandingOrder.BeneficiaryProductRoundingType = persisted.LoanRegistration.RoundingType;

                                                persisted.MonthlyPaybackAmount = (targetStandingOrder.Principal + targetStandingOrder.Interest);

                                                if (targetStandingOrder.DurationStartDate == targetStandingOrder.DurationEndDate) // happens for 1-month loans
                                                    targetStandingOrder.DurationEndDate = UberUtil.GetLastDayOfMonth(targetStandingOrder.DurationEndDate);

                                                switch ((InterestCalculationMode)persisted.LoanInterest.CalculationMode)
                                                {
                                                    case InterestCalculationMode.StraightLineAmortization:
                                                    case InterestCalculationMode.DiminishingBalanceAmortization:
                                                        targetStandingOrder.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / persisted.LoanRegistration.TermInMonths;
                                                        targetStandingOrder.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / persisted.LoanRegistration.TermInMonths;
                                                        targetStandingOrder.PaymentPerPeriod = (targetStandingOrder.Principal + targetStandingOrder.Interest);
                                                        persisted.MonthlyPaybackAmount = targetStandingOrder.PaymentPerPeriod;
                                                        break;
                                                    default:
                                                        break;
                                                }

                                                targetStandingOrder.CapitalizedInterest = targetStandingOrder.Interest;
                                                _standingOrderAppService.UpdateStandingOrder(targetStandingOrder, serviceHeader);
                                            }
                                        }
                                        else
                                        {
                                            var newStandingOrderDTO =
                                                new StandingOrderDTO
                                                {
                                                    ChargeType = (int)ChargeType.FixedAmount,
                                                    Trigger = persisted.LoanRegistration.StandingOrderTrigger,
                                                    BenefactorCustomerAccountId = customerSavingsAccountDTO.Id,
                                                    BeneficiaryCustomerAccountId = customerLoanAccountDTO.Id,
                                                    BeneficiaryProductProductCode = (int)ProductCode.Loan,
                                                    BeneficiaryProductRoundingType = persisted.LoanRegistration.RoundingType,
                                                    PaymentPerPeriod = Pmt,
                                                    LoanAmount = PV,
                                                    Principal = persisted.ApprovedPrincipalPayment != 0m ? persisted.ApprovedPrincipalPayment : repaymentSchedule.First().PrincipalPayment,
                                                    Interest = Math.Max(chargeableFirstInterestValue, persisted.ApprovedInterestPayment != 0m ? persisted.ApprovedInterestPayment : repaymentSchedule.First().InterestPayment),
                                                    DurationStartDate = repaymentSchedule.First().DueDate,
                                                    DurationEndDate = repaymentSchedule.Last().DueDate,
                                                    ScheduleFrequency = persisted.LoanRegistration.PaymentFrequencyPerYear,
                                                    Chargeable = true
                                                };

                                            persisted.MonthlyPaybackAmount = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);

                                            if (newStandingOrderDTO.DurationStartDate == newStandingOrderDTO.DurationEndDate) // happens for 1-month loans
                                                newStandingOrderDTO.DurationEndDate = UberUtil.GetLastDayOfMonth(newStandingOrderDTO.DurationEndDate);

                                            switch ((InterestCalculationMode)persisted.LoanInterest.CalculationMode)
                                            {
                                                case InterestCalculationMode.StraightLineAmortization:
                                                case InterestCalculationMode.DiminishingBalanceAmortization:
                                                    newStandingOrderDTO.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / persisted.LoanRegistration.TermInMonths;
                                                    newStandingOrderDTO.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / persisted.LoanRegistration.TermInMonths;
                                                    newStandingOrderDTO.PaymentPerPeriod = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);
                                                    persisted.MonthlyPaybackAmount = newStandingOrderDTO.PaymentPerPeriod;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            newStandingOrderDTO.CapitalizedInterest = newStandingOrderDTO.Interest;
                                            _standingOrderAppService.AddNewStandingOrder(newStandingOrderDTO, serviceHeader);
                                        }
                                    }

                                    #endregion
                                }

                                break;
                            case LoanAuditOption.Reject:
                                persisted.Status = (int)LoanCaseStatus.Rejected;
                                ReleaseLoanCaseGuarantors(persisted.Id, serviceHeader);
                                break;
                            case LoanAuditOption.Defer:
                                persisted.Status = (int)LoanCaseStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        auditOK = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
            }

            return auditOK;
        }

        public async Task<bool> AuditLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanAuditOption, ServiceHeader serviceHeader)
        {
            var auditOK = default(bool);

            if (loanCaseDTO != null && Enum.IsDefined(typeof(LoanAuditOption), loanAuditOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = await _loanCaseRepository.GetAsync(loanCaseDTO.Id, serviceHeader);
                    persisted.Status = (int)LoanCaseStatus.Approved;

                    if (persisted != null && persisted.Status == (int)LoanCaseStatus.Approved)
                    {
                        switch ((LoanAuditOption)loanAuditOption)
                        {
                            case LoanAuditOption.Audit:

                                persisted.Status = (int)LoanCaseStatus.Audited;

                                persisted.AuditRemarks = loanCaseDTO.AuditRemarks;

                                persisted.AuditedBy = serviceHeader.ApplicationUserName;
                                persisted.AuditedDate = DateTime.Now;

                                CustomerAccountDTO customerLoanAccountDTO = null;

                                CustomerAccountDTO customerSavingsAccountDTO = null;

                                if (persisted.LoanRegistration.CreateStandingOrderOnLoanAudit)
                                {
                                    #region create loan account?

                                    var customerLoanAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, persisted.LoanProductId, serviceHeader);

                                    if (customerLoanAccounts != null && customerLoanAccounts.Any())
                                        customerLoanAccountDTO = customerLoanAccounts.First();
                                    else
                                    {
                                        var customerAccountDTO = new CustomerAccountDTO
                                        {
                                            BranchId = persisted.BranchId,
                                            CustomerId = persisted.CustomerId,
                                            CustomerAccountTypeProductCode = (int)ProductCode.Loan,
                                            CustomerAccountTypeTargetProductId = persisted.LoanProductId,
                                            CustomerAccountTypeTargetProductCode = persisted.LoanProduct.Code,
                                            Status = (int)CustomerAccountStatus.Normal,
                                            RecordStatus = (int)RecordStatus.Approved,
                                        };

                                        customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                        if (customerAccountDTO != null)
                                            customerLoanAccountDTO = customerAccountDTO;
                                    }

                                    #endregion

                                    #region create savings account?

                                    if (persisted.SavingsProduct != null)
                                    {
                                        var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, persisted.SavingsProduct.Id, serviceHeader);

                                        if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                                            customerSavingsAccountDTO = customerSavingsAccounts.First();
                                        else
                                        {
                                            var customerAccountDTO = new CustomerAccountDTO
                                            {
                                                BranchId = persisted.BranchId,
                                                CustomerId = persisted.CustomerId,
                                                CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                                                CustomerAccountTypeTargetProductId = persisted.SavingsProduct.Id,
                                                CustomerAccountTypeTargetProductCode = persisted.SavingsProduct.Code,
                                                Status = (int)CustomerAccountStatus.Normal,
                                            };

                                            customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                            if (customerAccountDTO != null)
                                                customerSavingsAccountDTO = customerAccountDTO;
                                        }
                                    }

                                    #endregion

                                    #region  create standing order?

                                    if (customerLoanAccountDTO != null && customerSavingsAccountDTO != null)
                                    {
                                        #region 2. Do we need to recover upfront dynamic charges from loan?

                                        var loanAccountDynamicChargeTariffs = _commissionAppService.ComputeTariffsByLoanProduct(persisted.LoanProductId, (int)DynamicChargeRecoverySource.LoanAccount, (int)DynamicChargeRecoveryMode.Upfront, persisted.ApprovedAmount, customerLoanAccountDTO, serviceHeader, persisted.LoanRegistration.TermInMonths);

                                        if (loanAccountDynamicChargeTariffs != null && loanAccountDynamicChargeTariffs.Any())
                                        {
                                            // bookmark top-up amount
                                            persisted.AuditTopUpAmount = loanAccountDynamicChargeTariffs.Sum(x => x.Amount);
                                        }

                                        #endregion

                                        decimal PV = (persisted.ApprovedAmount + persisted.AuditTopUpAmount);

                                        decimal Pmt = (decimal)_financialsService.Pmt(persisted.LoanInterest.CalculationMode, persisted.LoanRegistration.TermInMonths, persisted.LoanRegistration.PaymentFrequencyPerYear, persisted.LoanInterest.AnnualPercentageRate, -(double)PV, 0d, persisted.LoanRegistration.PaymentDueDate);

                                        var repaymentSchedule = _financialsService.RepaymentSchedule(persisted.LoanRegistration.TermInMonths, persisted.LoanRegistration.PaymentFrequencyPerYear, persisted.LoanRegistration.GracePeriod, persisted.LoanInterest.CalculationMode, persisted.LoanInterest.AnnualPercentageRate, -(double)PV, 0d, persisted.LoanRegistration.PaymentDueDate);

                                        // do we need to reset?
                                        var chargeableFirstInterestValue = Math.Max(repaymentSchedule.First().InterestPayment, persisted.LoanRegistration.MinimumInterestAmount);

                                        var existingStandingOrders = _standingOrderAppService.FindStandingOrders(customerSavingsAccountDTO.Id, customerLoanAccountDTO.Id, serviceHeader);

                                        if (existingStandingOrders != null && existingStandingOrders.Any())
                                        {
                                            var targetStandingOrder = existingStandingOrders.FirstOrDefault();

                                            if (targetStandingOrder != null)
                                            {
                                                targetStandingOrder.ChargeType = (int)ChargeType.FixedAmount;
                                                targetStandingOrder.Trigger = persisted.LoanRegistration.StandingOrderTrigger;
                                                targetStandingOrder.LoanAmount = PV;
                                                targetStandingOrder.PaymentPerPeriod = Pmt;
                                                targetStandingOrder.Principal = persisted.ApprovedPrincipalPayment != 0m ? persisted.ApprovedPrincipalPayment : repaymentSchedule.First().PrincipalPayment;
                                                targetStandingOrder.Interest = Math.Max(chargeableFirstInterestValue, persisted.ApprovedInterestPayment != 0m ? persisted.ApprovedInterestPayment : repaymentSchedule.First().InterestPayment);
                                                targetStandingOrder.DurationStartDate = repaymentSchedule.First().DueDate;
                                                targetStandingOrder.DurationEndDate = repaymentSchedule.Last().DueDate;
                                                targetStandingOrder.ScheduleFrequency = persisted.LoanRegistration.PaymentFrequencyPerYear;
                                                targetStandingOrder.IsLocked = false;
                                                targetStandingOrder.Remarks = string.Empty;
                                                targetStandingOrder.BeneficiaryProductProductCode = (int)ProductCode.Loan;
                                                targetStandingOrder.BeneficiaryProductRoundingType = persisted.LoanRegistration.RoundingType;

                                                persisted.MonthlyPaybackAmount = (targetStandingOrder.Principal + targetStandingOrder.Interest);

                                                if (targetStandingOrder.DurationStartDate == targetStandingOrder.DurationEndDate) // happens for 1-month loans
                                                    targetStandingOrder.DurationEndDate = UberUtil.GetLastDayOfMonth(targetStandingOrder.DurationEndDate);

                                                switch ((InterestCalculationMode)persisted.LoanInterest.CalculationMode)
                                                {
                                                    case InterestCalculationMode.StraightLineAmortization:
                                                    case InterestCalculationMode.DiminishingBalanceAmortization:
                                                        targetStandingOrder.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / persisted.LoanRegistration.TermInMonths;
                                                        targetStandingOrder.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / persisted.LoanRegistration.TermInMonths;
                                                        targetStandingOrder.PaymentPerPeriod = (targetStandingOrder.Principal + targetStandingOrder.Interest);
                                                        persisted.MonthlyPaybackAmount = targetStandingOrder.PaymentPerPeriod;
                                                        break;
                                                    default:
                                                        break;
                                                }

                                                targetStandingOrder.CapitalizedInterest = targetStandingOrder.Interest;
                                                _standingOrderAppService.UpdateStandingOrder(targetStandingOrder, serviceHeader);
                                            }
                                        }
                                        else
                                        {
                                            var newStandingOrderDTO =
                                                new StandingOrderDTO
                                                {
                                                    ChargeType = (int)ChargeType.FixedAmount,
                                                    Trigger = persisted.LoanRegistration.StandingOrderTrigger,
                                                    BenefactorCustomerAccountId = customerSavingsAccountDTO.Id,
                                                    BeneficiaryCustomerAccountId = customerLoanAccountDTO.Id,
                                                    BeneficiaryProductProductCode = (int)ProductCode.Loan,
                                                    BeneficiaryProductRoundingType = persisted.LoanRegistration.RoundingType,
                                                    PaymentPerPeriod = Pmt,
                                                    LoanAmount = PV,
                                                    Principal = persisted.ApprovedPrincipalPayment != 0m ? persisted.ApprovedPrincipalPayment : repaymentSchedule.First().PrincipalPayment,
                                                    Interest = Math.Max(chargeableFirstInterestValue, persisted.ApprovedInterestPayment != 0m ? persisted.ApprovedInterestPayment : repaymentSchedule.First().InterestPayment),
                                                    DurationStartDate = repaymentSchedule.First().DueDate,
                                                    DurationEndDate = repaymentSchedule.Last().DueDate,
                                                    ScheduleFrequency = persisted.LoanRegistration.PaymentFrequencyPerYear,
                                                    Chargeable = true
                                                };

                                            persisted.MonthlyPaybackAmount = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);

                                            if (newStandingOrderDTO.DurationStartDate == newStandingOrderDTO.DurationEndDate) // happens for 1-month loans
                                                newStandingOrderDTO.DurationEndDate = UberUtil.GetLastDayOfMonth(newStandingOrderDTO.DurationEndDate);

                                            switch ((InterestCalculationMode)persisted.LoanInterest.CalculationMode)
                                            {
                                                case InterestCalculationMode.StraightLineAmortization:
                                                case InterestCalculationMode.DiminishingBalanceAmortization:
                                                    newStandingOrderDTO.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / persisted.LoanRegistration.TermInMonths;
                                                    newStandingOrderDTO.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / persisted.LoanRegistration.TermInMonths;
                                                    newStandingOrderDTO.PaymentPerPeriod = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);
                                                    persisted.MonthlyPaybackAmount = newStandingOrderDTO.PaymentPerPeriod;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            newStandingOrderDTO.CapitalizedInterest = newStandingOrderDTO.Interest;
                                            _standingOrderAppService.AddNewStandingOrder(newStandingOrderDTO, serviceHeader);
                                        }
                                    }

                                    #endregion
                                }

                                break;
                            case LoanAuditOption.Reject:
                                persisted.Status = (int)LoanCaseStatus.Rejected;
                                await ReleaseLoanCaseGuarantorsAsync(persisted.Id, serviceHeader);
                                break;
                            case LoanAuditOption.Defer:
                                persisted.Status = (int)LoanCaseStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        auditOK = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                    }
                }
            }

            return auditOK;
        }

        public bool CancelLoanCase(LoanCaseDTO loanCaseDTO, int loanCancellationOption, ServiceHeader serviceHeader)
        {
            if (loanCaseDTO != null && Enum.IsDefined(typeof(LoanAuditOption), loanCancellationOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanCaseDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)LoanCaseStatus.Audited)
                    {
                        switch ((LoanCancellationOption)loanCancellationOption)
                        {
                            case LoanCancellationOption.Defer:
                                persisted.Status = (int)LoanCaseStatus.Deferred;
                                break;
                            case LoanCancellationOption.Reject:
                                persisted.Status = (int)LoanCaseStatus.Rejected;
                                ReleaseLoanCaseGuarantors(persisted.Id, serviceHeader);
                                break;
                            default:
                                break;
                        }

                        persisted.IsBatched = false;
                        persisted.BatchNumber = 0;
                        persisted.BatchedBy = null;
                        persisted.CancelledBy = serviceHeader.ApplicationUserName;
                        persisted.CancelledDate = DateTime.Now;

                        var existing = FindLoanDisbursementBatchEntriesByLoanCaseId(persisted.Id, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var loanDisbursementBatchEntry = _loanDisbursementBatchEntryRepository.Get(item.Id, serviceHeader);

                                if (loanDisbursementBatchEntry != null)
                                {
                                    _loanDisbursementBatchEntryRepository.Remove(loanDisbursementBatchEntry, serviceHeader);
                                }
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public async Task<bool> UpdateLoanCaseAsync(LoanCaseDTO loanCaseDTO, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var result = default(bool);

                if (loanCaseDTO != null)
                {
                    var persisted = _loanCaseRepository.Get(loanCaseDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var loanInterest = new LoanInterest(loanCaseDTO.LoanInterestAnnualPercentageRate, loanCaseDTO.LoanInterestChargeMode, loanCaseDTO.LoanInterestRecoveryMode, loanCaseDTO.LoanInterestCalculationMode);

                        var loanRegistration = new LoanRegistration(loanCaseDTO.LoanRegistrationTermInMonths, loanCaseDTO.LoanRegistrationMinimumAmount, loanCaseDTO.LoanRegistrationMaximumAmount, loanCaseDTO.LoanRegistrationMinimumInterestAmount, loanCaseDTO.LoanRegistrationLoanProductSection, loanCaseDTO.LoanRegistrationLoanProductCategory, loanCaseDTO.LoanRegistrationConsecutiveIncome, loanCaseDTO.LoanRegistrationInvestmentsMultiplier, loanCaseDTO.LoanRegistrationMinimumGuarantors, loanCaseDTO.LoanRegistrationMaximumGuarantees, loanCaseDTO.LoanRegistrationRejectIfMemberHasBalance, loanCaseDTO.LoanRegistrationSecurityRequired, loanCaseDTO.LoanRegistrationAllowSelfGuarantee, loanCaseDTO.LoanRegistrationGracePeriod, loanCaseDTO.LoanRegistrationMinimumMembershipPeriod, loanCaseDTO.LoanRegistrationPaymentFrequencyPerYear, loanCaseDTO.LoanRegistrationPaymentDueDate, loanCaseDTO.LoanRegistrationPayoutRecoveryMode, loanCaseDTO.LoanRegistrationPayoutRecoveryPercentage, loanCaseDTO.LoanRegistrationAggregateCheckOffRecoveryMode, loanCaseDTO.LoanRegistrationChargeClearanceFee, loanCaseDTO.LoanRegistrationMicrocredit, loanCaseDTO.LoanRegistrationStandingOrderTrigger, loanCaseDTO.LoanRegistrationTrackArrears, loanCaseDTO.LoanRegistrationChargeArrearsFee, loanCaseDTO.LoanRegistrationEnforceSystemAppraisalRecommendation, loanCaseDTO.LoanRegistrationBypassAudit, loanCaseDTO.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage, loanCaseDTO.LoanRegistrationGuarantorSecurityMode, loanCaseDTO.LoanRegistrationRoundingType, loanCaseDTO.LoanRegistrationDisburseMicroLoanLessDeductions, loanCaseDTO.LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement, loanCaseDTO.LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal, loanCaseDTO.LoanRegistrationThrottleScheduledArrearsRecovery, loanCaseDTO.LoanRegistrationCreateStandingOrderOnLoanAudit);

                        var takeHome = new Charge(loanCaseDTO.TakeHomeType, loanCaseDTO.TakeHomePercentage, loanCaseDTO.TakeHomeFixedAmount);

                        var current = LoanCaseFactory.CreateLoanCase(loanCaseDTO.ParentId, loanCaseDTO.BranchId, loanCaseDTO.CustomerId, loanCaseDTO.LoanProductId, loanCaseDTO.LoanPurposeId, loanCaseDTO.SavingsProductId, loanCaseDTO.Remarks, loanCaseDTO.AmountApplied, loanCaseDTO.ReceivedDate, loanCaseDTO.LoanProductInvestmentsBalance, loanCaseDTO.LoanProductLoanBalance, loanCaseDTO.TotalLoansBalance, loanCaseDTO.LoanProductLatestIncome, loanCaseDTO.Reference, loanInterest, loanRegistration, loanCaseDTO.MaximumAmountPercentage, takeHome);

                        // Save original values that should NOT be overwritten
                        var originalStatus = persisted.Status;
                        var originalCaseNumber = persisted.CaseNumber;
                        var originalSequentialId = persisted.SequentialId;
                        var originalCreatedBy = persisted.CreatedBy;
                        var originalCreatedDate = persisted.CreatedDate;

                        // Set the identity but preserve original values
                        current.ChangeCurrentIdentity(persisted.Id, originalSequentialId, originalCreatedBy, originalCreatedDate);

                        // Explicitly set CaseNumber to original value
                        // You may need to add a method to set CaseNumber on the current entity
                        // or directly on the persisted entity after merge

                        _loanCaseRepository.Merge(persisted, current, serviceHeader);

                        // Restore original values that were overwritten
                        persisted.Status = originalStatus;
                        persisted.CaseNumber = originalCaseNumber;
                        persisted.CreatedBy = originalCreatedBy; // Keep original creator
                        persisted.CreatedDate = originalCreatedDate;

                        // Update modification tracking
                        persisted.CancelledBy = serviceHeader.ApplicationUserName;
                        persisted.CreatedDate = DateTime.UtcNow;

                        result = await dbContextScope.SaveChangesAsync(serviceHeader) >= 0;
                    }
                }

                return result;
            }
        }

        public async Task<bool> CancelLoanCaseAsync(LoanCaseDTO loanCaseDTO, int loanCancellationOption, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _loanCaseRepository.GetAsync(loanCaseDTO.Id, serviceHeader);

                if (persisted != null && persisted.Status == (int)LoanCaseStatus.Audited)
                {
                    switch ((LoanCancellationOption)loanCancellationOption)
                    {
                        case LoanCancellationOption.Defer:
                            persisted.Status = (int)LoanCaseStatus.Deferred;
                            break;
                        case LoanCancellationOption.Reject:
                            persisted.Status = (int)LoanCaseStatus.Rejected;
                            await ReleaseLoanCaseGuarantorsAsync(persisted.Id, serviceHeader);
                            break;
                        default:
                            break;
                    }

                    persisted.IsBatched = false;
                    persisted.BatchNumber = 0;
                    persisted.BatchedBy = null;
                    persisted.CancelledBy = serviceHeader.ApplicationUserName;
                    persisted.CancelledDate = DateTime.Now;

                    var existing = FindLoanDisbursementBatchEntriesByLoanCaseId(persisted.Id, serviceHeader);

                    if (existing != null && existing.Any())
                    {
                        foreach (var item in existing)
                        {
                            var loanDisbursementBatchEntry = await _loanDisbursementBatchEntryRepository.GetAsync(item.Id, serviceHeader);

                            if (loanDisbursementBatchEntry != null)
                            {
                                _loanDisbursementBatchEntryRepository.Remove(loanDisbursementBatchEntry, serviceHeader);
                            }
                        }
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public bool UpdateLoanGuarantors(Guid loanCaseId, List<LoanGuarantorDTO> loanGuarantors, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && loanGuarantors != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanCaseId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLoanGuarantorsByLoanCaseId(loanCaseId, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var persistedLoanGuarantor = _loanGuarantorRepository.Get(item.Id, serviceHeader);

                                if (persistedLoanGuarantor != null)
                                {
                                    _loanGuarantorRepository.Remove(persistedLoanGuarantor, serviceHeader);
                                }
                            }
                        }

                        if (loanGuarantors.Any())
                        {
                            foreach (var loanGuarantorDTO in loanGuarantors)
                            {
                                var loanGuarantor = LoanGuarantorFactory.CreateLoanGuarantor(loanGuarantorDTO.CustomerId, loanGuarantorDTO.LoaneeCustomerId, loanGuarantorDTO.LoanProductId, persisted.Id, loanGuarantorDTO.TotalShares, loanGuarantorDTO.CommittedShares, loanGuarantorDTO.AmountGuaranteed, loanGuarantorDTO.AmountPledged, loanGuarantorDTO.AppraisalFactor);

                                loanGuarantor.Status = (int)LoanGuarantorStatus.Attached;
                                loanGuarantor.CreatedBy = serviceHeader.ApplicationUserName;

                                _loanGuarantorRepository.Add(loanGuarantor, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool UpdateLoanCollaterals(Guid loanCaseId, List<CustomerDocumentDTO> customerDocuments, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && customerDocuments != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanCaseId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLoanCollateralsByLoanCaseId(loanCaseId, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var loanCollateral = _loanCollateralRepository.Get(item.Id, serviceHeader);

                                if (loanCollateral != null)
                                {
                                    _loanCollateralRepository.Remove(loanCollateral, serviceHeader);
                                }
                            }
                        }

                        if (customerDocuments.Any())
                        {
                            foreach (var item in customerDocuments)
                            {
                                var value = Convert.ToDecimal((item.CollateralAdvanceRate * Convert.ToDouble(item.CollateralValue)) / 100);

                                var loanCollateral = LoanCollateralFactory.CreateLoanCollateral(item.Id, persisted.Id, value);

                                loanCollateral.CreatedBy = serviceHeader.ApplicationUserName;

                                _loanCollateralRepository.Add(loanCollateral, serviceHeader);

                                _customerDocumentAppService.UpdateCollateralStatusAsync(item.Id, (int)CollateralStatus.Attached, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool UpdateLoanAppraisalFactors(Guid loanCaseId, List<LoanAppraisalFactorDTO> loanAppraisalFactors, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && loanAppraisalFactors != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanCaseRepository.Get(loanCaseId, serviceHeader);

                    if (persisted != null)
                    {
                        var existing = FindLoanAppraisalFactorsByLoanCaseId(loanCaseId, serviceHeader);

                        if (existing != null && existing.Any())
                        {
                            foreach (var item in existing)
                            {
                                var loanAppraisalFactor = _loanAppraisalFactorRepository.Get(item.Id, serviceHeader);

                                if (loanAppraisalFactor != null)
                                {
                                    _loanAppraisalFactorRepository.Remove(loanAppraisalFactor, serviceHeader);
                                }
                            }
                        }

                        if (loanAppraisalFactors.Any())
                        {
                            foreach (var item in loanAppraisalFactors)
                            {
                                var loanAppraisalFactor = LoanAppraisalFactorFactory.CreateLoanAppraisalFactor(persisted.Id, item.Description, item.Type, item.Amount);

                                _loanAppraisalFactorRepository.Add(loanAppraisalFactor, serviceHeader);
                            }
                        }

                        return dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool UpdateAttachedLoans(Guid loanCaseId, List<AttachedLoanDTO> attachedLoans, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var existingAttachedLoans = FindAttachedLoansByLoanCaseId(loanCaseId, serviceHeader);

                if (existingAttachedLoans != null && existingAttachedLoans.Any())
                {
                    var oldSet = from c in existingAttachedLoans ?? new List<AttachedLoanDTO> { } select c;

                    var newSet = from c in attachedLoans ?? new List<AttachedLoanDTO> { } select c;

                    var commonSet = oldSet.Intersect(newSet, new AttachedLoanDTOEqualityComparer());

                    var insertSet = newSet.Except(commonSet, new AttachedLoanDTOEqualityComparer());

                    var deleteSet = oldSet.Except(commonSet, new AttachedLoanDTOEqualityComparer());

                    foreach (var item in insertSet)
                    {
                        var attachedLoan = AttachedLoanFactory.CreateAttachedLoan(loanCaseId, item.CustomerAccountId, item.PrincipalBalance, item.InterestBalance, item.CarryForwardsBalance, item.ClearanceCharges);

                        attachedLoan.CreatedBy = serviceHeader.ApplicationUserName;

                        _attachedLoanRepository.Add(attachedLoan, serviceHeader);
                    }

                    foreach (var item in commonSet)
                    {
                        var persisted = _attachedLoanRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            persisted.PrincipalBalance = item.PrincipalBalance;
                            persisted.InterestBalance = item.InterestBalance;
                            persisted.CarryForwardsBalance = item.CarryForwardsBalance;
                            persisted.ClearanceCharges = item.ClearanceCharges;
                        }
                    }

                    foreach (var item in deleteSet)
                    {
                        var persisted = _attachedLoanRepository.Get(item.Id, serviceHeader);

                        if (persisted != null)
                        {
                            _attachedLoanRepository.Remove(persisted, serviceHeader);
                        }
                    }
                }
                else
                {
                    foreach (var item in attachedLoans)
                    {
                        var attachedLoan = AttachedLoanFactory.CreateAttachedLoan(loanCaseId, item.CustomerAccountId, item.PrincipalBalance, item.InterestBalance, item.CarryForwardsBalance, item.ClearanceCharges);

                        attachedLoan.CreatedBy = serviceHeader.ApplicationUserName;

                        _attachedLoanRepository.Add(attachedLoan, serviceHeader);
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public bool SubstituteLoanGuarantors(Guid substituteGuarantorCustomerId, List<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var substitutionOK = default(bool);

            if (substituteGuarantorCustomerId != null && loansGuaranteed != null && loansGuaranteed.Any())
            {
                var substituteGuarantorCustomer = _customerAppService.FindCustomer(substituteGuarantorCustomerId, serviceHeader);

                if (substituteGuarantorCustomer != null)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var loanGuarantorDTO in loansGuaranteed)
                        {
                            var persisted = _loanGuarantorRepository.Get(loanGuarantorDTO.Id, serviceHeader);

                            if (persisted != null)
                            {
                                var substitute = LoanGuarantorSubstituteFactory.CreateLoanGuarantorSubstitute(persisted.Id, persisted.CustomerId, substituteGuarantorCustomer.Id);

                                substitute.CreatedBy = serviceHeader.ApplicationUserName;

                                _loanGuarantorSubstituteRepository.Add(substitute, serviceHeader);

                                #region Do we need to send alerts?

                                var projection = persisted.ProjectedAs<LoanGuarantorDTO>();

                                var previousGuarantorDTO = _customerAppService.FindCustomer(projection.CustomerId, serviceHeader);

                                loanGuarantorDTO.AccountAlertCustomerId = substituteGuarantorCustomerId;
                                loanGuarantorDTO.AccountAlertPrimaryDescription = previousGuarantorDTO.FullName;
                                loanGuarantorDTO.AccountAlertSecondaryDescription = projection.LoanProductDescription;
                                loanGuarantorDTO.AccountAlertReference = previousGuarantorDTO.Reference3;

                                _brokerService.ProcessGuarantorSubstitutionAccountAlerts(DMLCommand.None, serviceHeader, loanGuarantorDTO);

                                #endregion

                                persisted.CustomerId = substituteGuarantorCustomer.Id; //reset
                            }
                        }

                        substitutionOK = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }
            }

            return substitutionOK;
        }

        public async Task<bool> SubstituteLoanGuarantorsAsync(Guid substituteGuarantorCustomerId, List<LoanGuarantorDTO> loansGuaranteed, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var substitutionOK = default(bool);

            if (substituteGuarantorCustomerId != null && loansGuaranteed != null && loansGuaranteed.Any())
            {
                var substituteGuarantorCustomer = await _customerAppService.FindCustomerAsync(substituteGuarantorCustomerId, serviceHeader);

                if (substituteGuarantorCustomer != null)
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var loanGuarantorDTO in loansGuaranteed)
                        {
                            var persisted = await _loanGuarantorRepository.GetAsync(loanGuarantorDTO.Id, serviceHeader);

                            var substituteGuarantorIsValid = IsSubstituteGuarantorValid(substituteGuarantorCustomerId, loanGuarantorDTO.LoanCaseId, loanGuarantorDTO.LoaneeCustomerId, loanGuarantorDTO.LoanProductId, serviceHeader);

                            if (persisted != null)
                            {
                                if (substituteGuarantorIsValid)
                                {
                                    var substitute = LoanGuarantorSubstituteFactory.CreateLoanGuarantorSubstitute(persisted.Id, persisted.CustomerId, substituteGuarantorCustomer.Id);

                                    substitute.CreatedBy = serviceHeader.ApplicationUserName;

                                    _loanGuarantorSubstituteRepository.Add(substitute, serviceHeader);

                                    #region Do we need to send alerts?

                                    var projection = persisted.ProjectedAs<LoanGuarantorDTO>();

                                    var previousGuarantorDTO = await _customerAppService.FindCustomerAsync(projection.CustomerId, serviceHeader);

                                    loanGuarantorDTO.AccountAlertCustomerId = substituteGuarantorCustomerId;
                                    loanGuarantorDTO.AccountAlertPrimaryDescription = previousGuarantorDTO.FullName;
                                    loanGuarantorDTO.AccountAlertSecondaryDescription = projection.LoanProductDescription;
                                    loanGuarantorDTO.AccountAlertReference = previousGuarantorDTO.Reference3;

                                    _brokerService.ProcessGuarantorSubstitutionAccountAlerts(DMLCommand.None, serviceHeader, loanGuarantorDTO);

                                    #endregion

                                    persisted.CustomerId = substituteGuarantorCustomer.Id; //reset
                                }
                                else
                                {
                                    throw new InvalidOperationException("Sorry, but substitute guarantor is not valid!");
                                }
                            }
                        }

                        substitutionOK = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                    }
                }
            }

            return substitutionOK;
        }

        private bool IsSubstituteGuarantorValid(Guid substituteGuarantorCustomerId, Guid? loanCaseId, Guid? loaneeCustomerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            List<LoanGuarantorDTO> loanCaseGuarantors = null;

            //handle loan-guarantor-dtos with NULL loan-case-ids and loan-product-ids
            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                loanCaseGuarantors = FindLoanGuarantorsByLoanCaseId(loanCaseId.Value, serviceHeader);
            }
            else if (loaneeCustomerId != null && loaneeCustomerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                loanCaseGuarantors = FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(loaneeCustomerId.Value, loanProductId, serviceHeader);
            }

            if (loanCaseGuarantors.Any(x => x.CustomerId == substituteGuarantorCustomerId && x.Status == (int)LoanGuarantorStatus.Attached))
                return false;
            else
                return true;
        }

        public bool ReleaseLoanGuarantors(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);

            if (customerAccountDTO != null)
            {
                var loanGuarantors = FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanGuarantors != null && loanGuarantors.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in loanGuarantors)
                        {
                            if (item.Status == (int)LoanGuarantorStatus.Attached)
                            {
                                var persisted = _loanGuarantorRepository.Get(item.Id, serviceHeader);

                                persisted.Status = (int)LoanGuarantorStatus.Released;
                            }
                        }

                        result = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }

                var loanCollaterals = FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanCollaterals != null && loanCollaterals.Any())
                {
                    foreach (var item in loanCollaterals)
                    {
                        result = _customerDocumentAppService.UpdateCollateralStatus(item.CustomerDocumentId, (int)CollateralStatus.Released, serviceHeader);
                    }
                }
            }

            return result;
        }

        public async Task<bool> ReleaseLoanGuarantorsAsync(Guid customerAccountId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);

            if (customerAccountDTO != null)
            {
                var loanGuarantors = FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanGuarantors != null && loanGuarantors.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in loanGuarantors)
                        {
                            if (item.Status == (int)LoanGuarantorStatus.Attached)
                            {
                                var persisted = await _loanGuarantorRepository.GetAsync(item.Id, serviceHeader);

                                persisted.Status = (int)LoanGuarantorStatus.Released;
                            }
                        }

                        result = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                    }
                }

                var loanCollaterals = FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanCollaterals != null && loanCollaterals.Any())
                {
                    foreach (var item in loanCollaterals)
                    {
                        result = await _customerDocumentAppService.UpdateCollateralStatusAsync(item.CustomerDocumentId, (int)CollateralStatus.Released, serviceHeader);
                    }
                }
            }

            return result;
        }

        public bool ReleaseRefinancedLoanGuarantors(Guid customerAccountId, DateTime refinanceDate, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);

            if (customerAccountDTO != null)
            {
                var loanGuarantors = FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanGuarantors != null && loanGuarantors.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in loanGuarantors)
                        {
                            if (item.Status == (int)LoanGuarantorStatus.Attached && item.CreatedDate < refinanceDate)
                            {
                                var persisted = _loanGuarantorRepository.Get(item.Id, serviceHeader);

                                persisted.Status = (int)LoanGuarantorStatus.Released;
                            }
                        }

                        result = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }
                }

                var loanCollaterals = FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanCollaterals != null && loanCollaterals.Any())
                {
                    foreach (var item in loanCollaterals)
                    {
                        if (item.CustomerDocumentCollateralStatus == (int)CollateralStatus.Attached && item.CreatedDate < refinanceDate)
                        {
                            result = _customerDocumentAppService.UpdateCollateralStatus(item.CustomerDocumentId, (int)CollateralStatus.Released, serviceHeader);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<bool> ReleaseRefinancedLoanGuarantorsAsync(Guid customerAccountId, DateTime refinanceDate, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);

            if (customerAccountDTO != null)
            {
                var loanGuarantors = FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanGuarantors != null && loanGuarantors.Any())
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in loanGuarantors)
                        {
                            if (item.Status == (int)LoanGuarantorStatus.Attached && item.CreatedDate < refinanceDate)
                            {
                                var persisted = await _loanGuarantorRepository.GetAsync(item.Id, serviceHeader);

                                persisted.Status = (int)LoanGuarantorStatus.Released;
                            }
                        }

                        result = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                    }
                }

                var loanCollaterals = FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                if (loanCollaterals != null && loanCollaterals.Any())
                {
                    foreach (var item in loanCollaterals)
                    {
                        if (item.CustomerDocumentCollateralStatus == (int)CollateralStatus.Attached && item.CreatedDate < refinanceDate)
                        {
                            result = await _customerDocumentAppService.UpdateCollateralStatusAsync(item.CustomerDocumentId, (int)CollateralStatus.Released, serviceHeader);
                        }
                    }
                }
            }

            return result;
        }

        public bool AttachLoanGuarantors(Guid sourceCustomerAccountId, Guid destinationLoanProductId, List<LoanGuarantorDTO> loanGuarantors, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (sourceCustomerAccountId != null && destinationLoanProductId != null && loanGuarantors != null)
            {
                var currentPostingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

                if (currentPostingPeriodDTO != null)
                {
                    var journals = new List<Journal>();

                    var loanGuarantorAttachmentHistories = new List<LoanGuarantorAttachmentHistory>();

                    var loanGuarantorAttachmentHistoryEntries = new List<LoanGuarantorAttachmentHistoryEntry>();

                    var sourceCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(sourceCustomerAccountId, serviceHeader);

                    _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { sourceCustomerAccount }, serviceHeader, true);

                    var sourceLoanProduct = _loanProductAppService.FindLoanProduct(sourceCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                    var destinationLoanProduct = _loanProductAppService.FindLoanProduct(destinationLoanProductId, serviceHeader);

                    var loanGuarantorAttachmentHistory = LoanGuarantorAttachmentHistoryFactory.CreateLoanGuarantorAttachmentHistory(sourceCustomerAccount.Id, sourceCustomerAccount.PrincipalBalance, sourceCustomerAccount.InterestBalance);

                    loanGuarantorAttachmentHistory.Status = (byte)LoanGuarantorAttachmentHistoryStatus.Attached;
                    loanGuarantorAttachmentHistory.CreatedBy = serviceHeader.ApplicationUserName;
                    loanGuarantorAttachmentHistories.Add(loanGuarantorAttachmentHistory);

                    loanGuarantors.ForEach(loanGuarantorDTO =>
                    {
                        CustomerAccountDTO destinationCustomerAccountDTO = null;

                        var customerAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(loanGuarantorDTO.CustomerId, destinationLoanProduct.Id, serviceHeader);

                        if (customerAccounts != null && customerAccounts.Any())
                            destinationCustomerAccountDTO = customerAccounts.First();
                        else
                        {
                            var customerAccountDTO = new CustomerAccountDTO
                            {
                                BranchId = sourceCustomerAccount.BranchId,
                                BranchCode = sourceCustomerAccount.BranchCode,
                                CustomerId = loanGuarantorDTO.CustomerId,
                                CustomerSerialNumber = loanGuarantorDTO.CustomerSerialNumber,
                                CustomerAccountTypeProductCode = (int)ProductCode.Loan,
                                CustomerAccountTypeTargetProductId = destinationLoanProduct.Id,
                                CustomerAccountTypeTargetProductCode = destinationLoanProduct.Code,
                                Status = (int)CustomerAccountStatus.Normal,
                            };

                            customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                            if (customerAccountDTO != null)
                                destinationCustomerAccountDTO = customerAccountDTO;
                        }

                        if (destinationCustomerAccountDTO != null)
                        {
                            loanGuarantorDTO.AccountAlertPrimaryDescription = string.Format("Guarantor Attachment (SN#{0}/SN#{1})", sourceCustomerAccount.PaddedCustomerSerialNumber, loanGuarantorDTO.PaddedCustomerSerialNumber);
                            loanGuarantorDTO.AccountAlertSecondaryDescription = destinationLoanProduct.Description;
                            loanGuarantorDTO.AccountAlertReference = !string.IsNullOrEmpty(loanGuarantorDTO.LoanCasePaddedCaseNumber) ? string.Format("L#{0}", loanGuarantorDTO.LoanCasePaddedCaseNumber) : "Guarantor Attachment";

                            if (loanGuarantorDTO.PrincipalAttached > 0m)
                            {
                                var attachmentPrincipalJournal = JournalFactory.CreateJournal(null, currentPostingPeriodDTO.Id, sourceCustomerAccount.BranchId, null, loanGuarantorDTO.PrincipalAttached, loanGuarantorDTO.AccountAlertPrimaryDescription, loanGuarantorDTO.AccountAlertSecondaryDescription, loanGuarantorDTO.AccountAlertReference, moduleNavigationItemCode, (int)SystemTransactionCode.GuarantorAttachment, null, serviceHeader, true);
                                _journalEntryPostingService.PerformDoubleEntry(attachmentPrincipalJournal, sourceLoanProduct.ChartOfAccountId, destinationLoanProduct.ChartOfAccountId, sourceCustomerAccount, destinationCustomerAccountDTO, serviceHeader);
                                journals.Add(attachmentPrincipalJournal);
                            }

                            if (loanGuarantorDTO.InterestAttached > 0m)
                            {
                                var attachmentInterestJournal = JournalFactory.CreateJournal(null, currentPostingPeriodDTO.Id, sourceCustomerAccount.BranchId, null, loanGuarantorDTO.InterestAttached, loanGuarantorDTO.AccountAlertPrimaryDescription, loanGuarantorDTO.AccountAlertSecondaryDescription, loanGuarantorDTO.AccountAlertReference, moduleNavigationItemCode, (int)SystemTransactionCode.GuarantorAttachment, null, serviceHeader, true);
                                _journalEntryPostingService.PerformDoubleEntry(attachmentInterestJournal, sourceLoanProduct.InterestReceivableChartOfAccountId, destinationLoanProduct.InterestReceivableChartOfAccountId, sourceCustomerAccount, destinationCustomerAccountDTO, serviceHeader);
                                journals.Add(attachmentInterestJournal);
                            }

                            if ((loanGuarantorDTO.PrincipalAttached + loanGuarantorDTO.InterestAttached) > 0m)
                            {
                                var loanGuarantorAttachmentHistoryEntry = LoanGuarantorAttachmentHistoryEntryFactory.CreateLoanGuarantorAttachmentHistoryEntry(loanGuarantorAttachmentHistory.Id, loanGuarantorDTO.Id, destinationCustomerAccountDTO.Id, loanGuarantorDTO.PrincipalAttached, loanGuarantorDTO.InterestAttached, loanGuarantorDTO.AccountAlertReference);
                                loanGuarantorAttachmentHistoryEntry.CreatedBy = serviceHeader.ApplicationUserName;
                                loanGuarantorAttachmentHistoryEntries.Add(loanGuarantorAttachmentHistoryEntry);
                            }

                            #region Do we need to send alerts?

                            _brokerService.ProcessGuarantorAttachmentAccountAlerts(DMLCommand.None, serviceHeader, loanGuarantorDTO);

                            #endregion
                        }
                    });

                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        foreach (var item in loanGuarantorAttachmentHistories)
                            _loanGuarantorAttachmentHistoryRepository.Add(item, serviceHeader);

                        foreach (var item in loanGuarantorAttachmentHistoryEntries)
                            _loanGuarantorAttachmentHistoryEntryRepository.Add(item, serviceHeader);

                        result = dbContextScope.SaveChanges(serviceHeader) > 0;
                    }

                    if (result)
                    {
                        #region Bulk-Insert journals

                        _journalEntryPostingService.BulkSave(serviceHeader, journals);

                        #endregion
                    }
                }
            }

            return result;
        }

        public bool RestructureLoan(Guid branchId, Guid customerAccountId, double NPer, double Pmt, string reference, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var customerLoanAccount = _customerAccountAppService.FindCustomerAccountDTO(customerAccountId, serviceHeader);

            var payablesControlChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.PayablesControl, serviceHeader);

            var defaultSavingsProductDTO = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

            if (customerLoanAccount != null && payablesControlChartOfAccountId != Guid.Empty && defaultSavingsProductDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var existingLoanCases = FindLoanCasesByCustomerIdAndLoanProductId(customerLoanAccount.CustomerId, customerLoanAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                    LoanCaseDTO lcDTO = new LoanCaseDTO();

                    if (existingLoanCases != null && existingLoanCases.Any(x => x.Status.In((int)LoanCaseStatus.Registered, (int)LoanCaseStatus.Appraised, (int)LoanCaseStatus.Deferred, (int)LoanCaseStatus.Approved, (int)LoanCaseStatus.Audited)))
                    {
                        bool issuccess = false;

                        lcDTO.ErrorMessageResult = issuccess ? "" : string.Format("Sorry, but selected customer has a loan case for the selected product currently undergoing processing!");

                        return issuccess;
                    }

                    _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerLoanAccount }, serviceHeader, true);

                    if (customerLoanAccount.PrincipalBalance * -1 > 0m && customerLoanAccount.InterestBalance == 0m)
                    {
                        var loanProductDTO = _loanProductAppService.FindLoanProduct(customerLoanAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                        var primaryDescription = "Loan Restructured";

                        var secondaryDescription = string.Format("NPer: {0} / Pmt: {1}", NPer, Pmt);

                        // 1. create loan case
                        var loanInterest = new LoanInterest(loanProductDTO.LoanInterestAnnualPercentageRate, loanProductDTO.LoanInterestChargeMode, loanProductDTO.LoanInterestRecoveryMode, (int)InterestCalculationMode.StraightLineAmortization);

                        var loanRegistration = new LoanRegistration((int)NPer, loanProductDTO.LoanRegistrationMinimumAmount, loanProductDTO.LoanRegistrationMaximumAmount, loanProductDTO.LoanRegistrationMinimumInterestAmount, loanProductDTO.LoanRegistrationLoanProductSection, loanProductDTO.LoanRegistrationLoanProductCategory, loanProductDTO.LoanRegistrationConsecutiveIncome, loanProductDTO.LoanRegistrationInvestmentsMultiplier, loanProductDTO.LoanRegistrationMinimumGuarantors, loanProductDTO.LoanRegistrationMaximumGuarantees, loanProductDTO.LoanRegistrationRejectIfMemberHasBalance, loanProductDTO.LoanRegistrationSecurityRequired, loanProductDTO.LoanRegistrationAllowSelfGuarantee, loanProductDTO.LoanRegistrationGracePeriod, loanProductDTO.LoanRegistrationMinimumMembershipPeriod, loanProductDTO.LoanRegistrationPaymentFrequencyPerYear, loanProductDTO.LoanRegistrationPaymentDueDate, loanProductDTO.LoanRegistrationPayoutRecoveryMode, loanProductDTO.LoanRegistrationPayoutRecoveryPercentage, loanProductDTO.LoanRegistrationAggregateCheckOffRecoveryMode, loanProductDTO.LoanRegistrationChargeClearanceFee, loanProductDTO.LoanRegistrationMicrocredit, loanProductDTO.LoanRegistrationStandingOrderTrigger, loanProductDTO.LoanRegistrationTrackArrears, loanProductDTO.LoanRegistrationChargeArrearsFee, loanProductDTO.LoanRegistrationEnforceSystemAppraisalRecommendation, loanProductDTO.LoanRegistrationBypassAudit, loanProductDTO.LoanRegistrationMaximumSelfGuaranteeEligiblePercentage, loanProductDTO.LoanRegistrationGuarantorSecurityMode, loanProductDTO.LoanRegistrationRoundingType, loanProductDTO.LoanRegistrationDisburseMicroLoanLessDeductions, loanProductDTO.LoanRegistrationExcludeOutstandingLoansOnMaximumEntitlement, loanProductDTO.LoanRegistrationConsiderInvestmentsBalanceForIncomeBasedLoanAppraisal, loanProductDTO.LoanRegistrationThrottleScheduledArrearsRecovery, loanProductDTO.LoanRegistrationCreateStandingOrderOnLoanAudit);

                        var takeHome = new Charge(loanProductDTO.TakeHomeType, loanProductDTO.TakeHomePercentage, loanProductDTO.TakeHomeFixedAmount);

                        var loanCase = LoanCaseFactory.CreateLoanCase(null, branchId, customerLoanAccount.CustomerId, loanProductDTO.Id, null, null, reference, customerLoanAccount.PrincipalBalance * -1, DateTime.Today, 0m, 0m, 0m, 0m, primaryDescription, loanInterest, loanRegistration, 100d, takeHome);

                        loanCase.CaseNumber = _loanCaseRepository.DatabaseSqlQuery<int>(string.Format("SELECT ISNULL(MAX(CaseNumber),0) + 1 AS Expr1 FROM {0}LoanCases", DefaultSettings.Instance.TablePrefix), serviceHeader).FirstOrDefault();
                        loanCase.Status = (int)LoanCaseStatus.Restructured;

                        loanCase.AppraisedAmount = customerLoanAccount.PrincipalBalance * -1;
                        loanCase.ApprovedAmount = customerLoanAccount.PrincipalBalance * -1;

                        loanCase.CreatedBy = serviceHeader.ApplicationUserName;

                        _loanCaseRepository.Add(loanCase, serviceHeader);

                        if (dbContextScope.SaveChanges(serviceHeader) > 0)
                        {
                            // 2. credit loan, debit payables
                            _journalAppService.AddNewJournal(branchId, null, customerLoanAccount.PrincipalBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanRestructuring, null, loanProductDTO.ChartOfAccountId, payablesControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);

                            // 3. credit payables, credit loan
                            _journalAppService.AddNewJournal(branchId, null, customerLoanAccount.PrincipalBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.LoanRestructuring, null, payablesControlChartOfAccountId, loanProductDTO.ChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);

                            // 4. standing order
                            Guid customerSavingsAccountId = Guid.Empty;

                            #region find savings account

                            var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(customerLoanAccount.CustomerId, defaultSavingsProductDTO.Id, serviceHeader);

                            if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                                customerSavingsAccountId = customerSavingsAccounts.First().Id;
                            else
                            {
                                var customerSavingsAccountDTO = new CustomerAccountDTO
                                {
                                    BranchId = branchId,
                                    CustomerId = customerLoanAccount.CustomerId,
                                    CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                                    CustomerAccountTypeTargetProductId = defaultSavingsProductDTO.Id,
                                    CustomerAccountTypeTargetProductCode = defaultSavingsProductDTO.Code,
                                    Status = (int)CustomerAccountStatus.Normal,
                                };

                                customerSavingsAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerSavingsAccountDTO, serviceHeader);

                                customerSavingsAccountId = (customerSavingsAccountDTO != null) ? customerSavingsAccountDTO.Id : Guid.Empty;
                            }

                            #endregion

                            decimal PV = customerLoanAccount.PrincipalBalance * -1;

                            var repaymentSchedule = _financialsService.RepaymentSchedule((int)NPer, loanProductDTO.LoanRegistrationPaymentFrequencyPerYear, loanProductDTO.LoanRegistrationGracePeriod, loanProductDTO.LoanInterestCalculationMode, loanProductDTO.LoanInterestAnnualPercentageRate, -(double)PV, 0d, loanProductDTO.LoanRegistrationPaymentDueDate);

                            // do we need to reset?
                            var chargeableFirstInterestValue = Math.Max(repaymentSchedule.First().InterestPayment, loanProductDTO.LoanRegistrationMinimumInterestAmount);

                            var existingStandingOrders = _standingOrderAppService.FindStandingOrders(customerSavingsAccountId, customerLoanAccount.Id, serviceHeader);

                            if (existingStandingOrders != null && existingStandingOrders.Any())
                            {
                                var targetStandingOrder = existingStandingOrders.FirstOrDefault();

                                if (targetStandingOrder != null)
                                {
                                    targetStandingOrder.ChargeType = (int)ChargeType.FixedAmount;
                                    targetStandingOrder.Trigger = loanProductDTO.LoanRegistrationStandingOrderTrigger;
                                    targetStandingOrder.LoanAmount = customerLoanAccount.PrincipalBalance;
                                    targetStandingOrder.Principal = repaymentSchedule.First().PrincipalPayment;
                                    targetStandingOrder.Interest = Math.Max(chargeableFirstInterestValue, repaymentSchedule.First().InterestPayment);
                                    targetStandingOrder.DurationStartDate = repaymentSchedule.First().DueDate;
                                    targetStandingOrder.DurationEndDate = repaymentSchedule.Last().DueDate;
                                    targetStandingOrder.ScheduleFrequency = loanProductDTO.LoanRegistrationPaymentFrequencyPerYear;
                                    targetStandingOrder.IsLocked = false;
                                    targetStandingOrder.Remarks = reference;
                                    targetStandingOrder.BeneficiaryProductProductCode = (int)ProductCode.Loan;
                                    targetStandingOrder.BeneficiaryProductRoundingType = loanProductDTO.LoanRegistrationRoundingType;

                                    switch ((InterestCalculationMode)loanProductDTO.LoanInterestCalculationMode)
                                    {
                                        case InterestCalculationMode.StraightLineAmortization:
                                        case InterestCalculationMode.DiminishingBalanceAmortization:
                                            targetStandingOrder.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                                            targetStandingOrder.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                                            targetStandingOrder.PaymentPerPeriod = (targetStandingOrder.Principal + targetStandingOrder.Interest);
                                            break;
                                        default:
                                            break;
                                    }

                                    targetStandingOrder.CapitalizedInterest = targetStandingOrder.Interest;

                                    _standingOrderAppService.UpdateStandingOrder(targetStandingOrder, serviceHeader);
                                }
                            }
                            else
                            {
                                var newStandingOrderDTO =
                                    new StandingOrderDTO
                                    {
                                        ChargeType = (int)ChargeType.FixedAmount,
                                        Trigger = loanProductDTO.LoanRegistrationStandingOrderTrigger,
                                        BenefactorCustomerAccountId = customerSavingsAccountId,
                                        BeneficiaryCustomerAccountId = customerLoanAccount.Id,
                                        BeneficiaryProductProductCode = (int)ProductCode.Loan,
                                        BeneficiaryProductRoundingType = loanProductDTO.LoanRegistrationRoundingType,
                                        LoanAmount = customerLoanAccount.PrincipalBalance,
                                        Principal = repaymentSchedule.First().PrincipalPayment,
                                        Interest = Math.Max(chargeableFirstInterestValue, repaymentSchedule.First().InterestPayment),
                                        DurationStartDate = repaymentSchedule.First().DueDate,
                                        DurationEndDate = repaymentSchedule.Last().DueDate,
                                        ScheduleFrequency = loanProductDTO.LoanRegistrationPaymentFrequencyPerYear,
                                        Remarks = reference,
                                        Chargeable = true
                                    };

                                switch ((InterestCalculationMode)loanProductDTO.LoanInterestCalculationMode)
                                {
                                    case InterestCalculationMode.StraightLineAmortization:
                                    case InterestCalculationMode.DiminishingBalanceAmortization:
                                        newStandingOrderDTO.Principal = repaymentSchedule.Sum(x => x.PrincipalPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                                        newStandingOrderDTO.Interest = repaymentSchedule.Sum(x => x.InterestPayment) / loanProductDTO.LoanRegistrationTermInMonths;
                                        newStandingOrderDTO.PaymentPerPeriod = (newStandingOrderDTO.Principal + newStandingOrderDTO.Interest);
                                        break;
                                    default:
                                        break;
                                }

                                newStandingOrderDTO.CapitalizedInterest = newStandingOrderDTO.Interest;

                                _standingOrderAppService.AddNewStandingOrder(newStandingOrderDTO, serviceHeader);
                            }

                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public bool MarkLoanCaseDisbursed(LoanDisbursementBatchEntryDTO loanDisbursementBatchEntryDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (loanDisbursementBatchEntryDTO == null)
                return result;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persistedLoanCase = _loanCaseRepository.Get(loanDisbursementBatchEntryDTO.LoanCaseId, serviceHeader);

                if (persistedLoanCase != null)
                {
                    switch ((LoanCaseStatus)persistedLoanCase.Status)
                    {
                        case LoanCaseStatus.Approved:

                            persistedLoanCase.Status = (int)LoanCaseStatus.Disbursed;
                            persistedLoanCase.DisbursedAmount = loanDisbursementBatchEntryDTO.LoanCaseApprovedAmount;
                            persistedLoanCase.AuditTopUpAmount = loanDisbursementBatchEntryDTO.LoanCaseAuditTopUpAmount;
                            persistedLoanCase.MonthlyPaybackAmount = loanDisbursementBatchEntryDTO.LoanCaseMonthlyPaybackAmount;
                            persistedLoanCase.DisbursementRemarks = loanDisbursementBatchEntryDTO.LoanDisbursementBatchReference;
                            persistedLoanCase.DisbursedBy = serviceHeader.ApplicationUserName;
                            persistedLoanCase.DisbursedDate = DateTime.Now;

                            result = dbContextScope.SaveChanges(serviceHeader) > 0;

                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        public List<LoanCaseDTO> FindLoanCases(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var loanCases = _loanCaseRepository.GetAll(serviceHeader);

                if (loanCases != null && loanCases.Any())
                {
                    return loanCases.ProjectedAsCollection<LoanCaseDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCases(int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanCaseSpecifications.DefaultSpec(includeBatchStatus);

                ISpecification<LoanCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanCasePagedCollection = _loanCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanCasePagedCollection != null)
                {
                    var pageCollection = loanCasePagedCollection.PageCollection.ProjectedAsCollection<LoanCaseDTO>();

                    var itemsCount = loanCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCases(string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanCaseSpecifications.LoanCaseFullText(text, loanCaseFilter, includeBatchStatus);

                ISpecification<LoanCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanCasePagedCollection = _loanCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanCasePagedCollection != null)
                {
                    var pageCollection = loanCasePagedCollection.PageCollection.ProjectedAsCollection<LoanCaseDTO>();

                    var itemsCount = loanCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesByStatus(int status, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanCaseSpecifications.LoanCaseWithStatusAndFullText(status, text, loanCaseFilter, includeBatchStatus);

                ISpecification<LoanCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanCasePagedCollection = _loanCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanCasePagedCollection != null)
                {
                    var pageCollection = loanCasePagedCollection.PageCollection.ProjectedAsCollection<LoanCaseDTO>();

                    var itemsCount = loanCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesBySectionAndStatus(int loanProductSection, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanCaseSpecifications.LoanCaseWithLoanProductSectionAndStatusAndFullText(startDate, endDate, loanProductSection, status, text, loanCaseFilter, includeBatchStatus);

                ISpecification<LoanCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanCasePagedCollection = _loanCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanCasePagedCollection != null)
                {
                    var pageCollection = loanCasePagedCollection.PageCollection.ProjectedAsCollection<LoanCaseDTO>();

                    var itemsCount = loanCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesByCategoryAndStatus(int loanProductCategory, int status, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, decimal approvedAmountThreshold, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanCaseSpecifications.LoanCaseWithLoanProductCategoryAndStatusAndFullText(startDate, endDate, loanProductCategory, status, text, loanCaseFilter, approvedAmountThreshold, includeBatchStatus);

                ISpecification<LoanCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanCasePagedCollection = _loanCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanCasePagedCollection != null)
                {
                    var pageCollection = loanCasePagedCollection.PageCollection.ProjectedAsCollection<LoanCaseDTO>();

                    var itemsCount = loanCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<LoanCaseDTO> FindLoanCasesBySection(int loanProductSection, DateTime startDate, DateTime endDate, string text, int loanCaseFilter, int pageIndex, int pageSize, bool includeBatchStatus, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanCaseSpecifications.LoanCaseWithDateRangeAndLoanProductSectionAndFullText(startDate, endDate, loanProductSection, text, loanCaseFilter, includeBatchStatus);

                ISpecification<LoanCase> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanCasePagedCollection = _loanCaseRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanCasePagedCollection != null)
                {
                    var pageCollection = loanCasePagedCollection.PageCollection.ProjectedAsCollection<LoanCaseDTO>();

                    var itemsCount = loanCasePagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanCaseDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public LoanCaseDTO FindLoanCase(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            if (loanCaseId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanCase = _loanCaseRepository.Get(loanCaseId, serviceHeader);

                    if (loanCase != null)
                    {
                        return loanCase.ProjectedAs<LoanCaseDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanCaseDTO> FindLoanCaseByLoanCaseNumber(int caseNumber, ServiceHeader serviceHeader)
        {
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCaseSpecifications.LoanCaseWithLoanCaseNumber(caseNumber);

                    ISpecification<LoanCase> spec = filter;

                    var loanCases = _loanCaseRepository.AllMatching(spec, serviceHeader);

                    if (loanCases != null && loanCases.Any())
                    {
                        return loanCases.ProjectedAsCollection<LoanCaseDTO>();
                    }
                    else return null;
                }
            }
        }

        public LoanGuarantorDTO FindLoanGuarantor(Guid loanGuarantorId, ServiceHeader serviceHeader)
        {
            if (loanGuarantorId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanGuarantor = _loanGuarantorRepository.Get(loanGuarantorId, serviceHeader);

                    if (loanGuarantor != null)
                    {
                        return loanGuarantor.ProjectedAs<LoanGuarantorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanGuarantorDTO> FindLoanGuarantorsByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanGuarantorSpecifications.LoanGuarantorWithCustomerId(customerId);

                    ISpecification<LoanGuarantor> spec = filter;

                    var loanGuarantors = _loanGuarantorRepository.AllMatching(spec, serviceHeader);

                    if (loanGuarantors != null && loanGuarantors.Any())
                    {
                        return loanGuarantors.ProjectedAsCollection<LoanGuarantorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantorsByCustomerId(Guid customerId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanGuarantorSpecifications.LoanGuarantorWithCustomerIdAndText(customerId, text);

                ISpecification<LoanGuarantor> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanGuarantorPagedCollection = _loanGuarantorRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanGuarantorPagedCollection != null)
                {
                    var pageCollection = loanGuarantorPagedCollection.PageCollection.ProjectedAsCollection<LoanGuarantorDTO>();

                    var itemsCount = loanGuarantorPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanGuarantorDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<LoanGuarantorDTO> FindLoanGuarantorsByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanGuarantorSpecifications.LoanGuarantorWithLoanCaseId(loanCaseId);

                    ISpecification<LoanGuarantor> spec = filter;

                    var loanGuarantors = _loanGuarantorRepository.AllMatching(spec, serviceHeader);

                    if (loanGuarantors != null && loanGuarantors.Any())
                    {
                        return loanGuarantors.ProjectedAsCollection<LoanGuarantorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<LoanGuarantorDTO> FindLoanGuarantors(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanGuarantorSpecifications.LoanGuarantorFullText(text);

                ISpecification<LoanGuarantor> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanGuarantorCollection = _loanGuarantorRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanGuarantorCollection != null)
                {
                    var pageCollection = loanGuarantorCollection.PageCollection.ProjectedAsCollection<LoanGuarantorDTO>();

                    var itemsCount = loanGuarantorCollection.ItemsCount;

                    return new PageCollectionInfo<LoanGuarantorDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<AttachedLoanDTO> FindAttachedLoansByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = AttachedLoanSpecifications.AttachedLoanWithLoanCaseId(loanCaseId);

                    ISpecification<AttachedLoan> spec = filter;

                    var attachedLoans = _attachedLoanRepository.AllMatching(spec, serviceHeader);

                    if (attachedLoans != null && attachedLoans.Any())
                    {
                        var projection = attachedLoans.ProjectedAsCollection<AttachedLoanDTO>();

                        _customerAccountAppService.FetchCustomerAccountsProductDescription(projection, serviceHeader);

                        return projection;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanAppraisalFactorDTO> FindLoanAppraisalFactorsByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanAppraisalFactorSpecifications.LoanAppraisalFactorWithLoanCaseId(loanCaseId);

                    ISpecification<LoanAppraisalFactor> spec = filter;

                    var loanAppraisalFactors = _loanAppraisalFactorRepository.AllMatching(spec, serviceHeader);

                    if (loanAppraisalFactors != null && loanAppraisalFactors.Any())
                    {
                        return loanAppraisalFactors.ProjectedAsCollection<LoanAppraisalFactorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanCaseDTO> FindLoanCasesByCustomerIdAndLoanProductId(Guid customerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty && loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCaseSpecifications.LoanCaseWithCustomerIdAndLoanProductId(customerId, loanProductId);

                    ISpecification<LoanCase> spec = filter;

                    var loanCases = _loanCaseRepository.AllMatching(spec, serviceHeader);

                    if (loanCases != null && loanCases.Any())
                    {
                        return loanCases.ProjectedAsCollection<LoanCaseDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanCaseDTO> FindLoanCasesByCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(Guid customerId, Guid loanProductId, int auxiliaryLoanCondition, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty && loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCaseSpecifications.LoanCaseWithCustomerIdAndLoanProductIdAndAuxiliaryLoanCondition(customerId, loanProductId, auxiliaryLoanCondition);

                    ISpecification<LoanCase> spec = filter;

                    var loanCases = _loanCaseRepository.AllMatching(spec, serviceHeader);

                    if (loanCases != null && loanCases.Any())
                    {
                        return loanCases.ProjectedAsCollection<LoanCaseDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanGuarantorDTO> FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loaneeCustomerId != Guid.Empty && loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanGuarantorSpecifications.LoanGuarantorWithLoaneeCustomerIdAndLoanProductId(loaneeCustomerId, loanProductId);

                    ISpecification<LoanGuarantor> spec = filter;

                    var loanGuarantors = _loanGuarantorRepository.AllMatching(spec, serviceHeader);

                    if (loanGuarantors != null && loanGuarantors.Any())
                    {
                        return loanGuarantors.ProjectedAsCollection<LoanGuarantorDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanCollateralDTO> FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(Guid loaneeCustomerId, Guid loanProductId, ServiceHeader serviceHeader)
        {
            if (loaneeCustomerId != Guid.Empty && loanProductId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCollateralSpecifications.LoanCollateralWithCustomerIdAndLoanProductId(loaneeCustomerId, loanProductId);

                    ISpecification<LoanCollateral> spec = filter;

                    var loanCollaterals = _loanCollateralRepository.AllMatching(spec, serviceHeader);

                    if (loanCollaterals != null && loanCollaterals.Any())
                    {
                        return loanCollaterals.ProjectedAsCollection<LoanCollateralDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanCaseDTO> FindLoanCasesByCustomerIdInProcess(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCaseSpecifications.LoanCaseWithCustomerIdInProcess(customerId);

                    ISpecification<LoanCase> spec = filter;

                    var loanCases = _loanCaseRepository.AllMatching(spec, serviceHeader);

                    if (loanCases != null && loanCases.Any())
                    {
                        return loanCases.ProjectedAsCollection<LoanCaseDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<LoanCollateralDTO> FindLoanCollateralsByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanCollateralSpecifications.LoanCollateralWithLoanCaseId(loanCaseId);

                    ISpecification<LoanCollateral> spec = filter;

                    var loanCollaterals = _loanCollateralRepository.AllMatching(spec, serviceHeader);

                    if (loanCollaterals != null && loanCollaterals.Any())
                    {
                        return loanCollaterals.ProjectedAsCollection<LoanCollateralDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO> FindLoanGuarantorAttachmentHistoryByStatus(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = LoanGuarantorAttachmentHistorySpecifications.LoanGuarantorAttachmentHistoryWithStatusAndFullText(startDate, endDate, status, text);

                ISpecification<LoanGuarantorAttachmentHistory> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var loanGuarantorAttachmentHistoryPagedCollection = _loanGuarantorAttachmentHistoryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (loanGuarantorAttachmentHistoryPagedCollection != null)
                {
                    var pageCollection = loanGuarantorAttachmentHistoryPagedCollection.PageCollection.ProjectedAsCollection<LoanGuarantorAttachmentHistoryDTO>();

                    _customerAccountAppService.FetchCustomerAccountsProductDescription(pageCollection, serviceHeader);

                    var itemsCount = loanGuarantorAttachmentHistoryPagedCollection.ItemsCount;

                    return new PageCollectionInfo<LoanGuarantorAttachmentHistoryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<LoanGuarantorAttachmentHistoryEntryDTO> FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(Guid loanGuarantorAttachmentHistoryId, ServiceHeader serviceHeader)
        {
            if (loanGuarantorAttachmentHistoryId != null && loanGuarantorAttachmentHistoryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanGuarantorAttachmentHistoryEntrySpecifications.LoanGuarantorWithLoanGuarantorAttachmentHistoryId(loanGuarantorAttachmentHistoryId);

                    ISpecification<LoanGuarantorAttachmentHistoryEntry> spec = filter;

                    var loanGuarantorAttachmentHistoryEntries = _loanGuarantorAttachmentHistoryEntryRepository.AllMatching(spec, serviceHeader);

                    if (loanGuarantorAttachmentHistoryEntries != null && loanGuarantorAttachmentHistoryEntries.Any())
                    {
                        var projection = loanGuarantorAttachmentHistoryEntries.ProjectedAsCollection<LoanGuarantorAttachmentHistoryEntryDTO>();

                        _customerAccountAppService.FetchCustomerAccountsProductDescription(projection, serviceHeader);

                        return projection;
                    }
                    else return null;
                }
            }
            else return null;
        }

        public LoanGuarantorAttachmentHistoryEntryDTO FindLoanGuarantorAttachmentHistoryEntry(Guid loanGuarantorAttachmentHistoryEntryId, ServiceHeader serviceHeader)
        {
            if (loanGuarantorAttachmentHistoryEntryId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var loanGuarantorAttachmentHistoryEntry = _loanGuarantorAttachmentHistoryEntryRepository.Get(loanGuarantorAttachmentHistoryEntryId, serviceHeader);

                    if (loanGuarantorAttachmentHistoryEntry != null)
                    {
                        return loanGuarantorAttachmentHistoryEntry.ProjectedAs<LoanGuarantorAttachmentHistoryEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool RelieveLoanGuarantors(Guid loanGuarantorAttachmentHistoryId, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var currentPostingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            var loanGuarantorAttachmentHistoryEntries = FindLoanGuarantorAttachmentHistoryEntriesByLoanGuarantorAttachmentHistoryId(loanGuarantorAttachmentHistoryId, serviceHeader);

            if (currentPostingPeriodDTO != null && loanGuarantorAttachmentHistoryEntries != null && loanGuarantorAttachmentHistoryEntries.Any())
            {
                var journals = new List<Journal>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _loanGuarantorAttachmentHistoryRepository.Get(loanGuarantorAttachmentHistoryId, serviceHeader);

                    if (persisted != null)
                    {
                        persisted.Status = (byte)LoanGuarantorAttachmentHistoryStatus.Relieved;

                        if (result = dbContextScope.SaveChanges(serviceHeader) > 0)
                        {
                            var sourceCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(persisted.SourceCustomerAccountId, serviceHeader);

                            var sourceLoanProduct = _loanProductAppService.FindLoanProduct(sourceCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                            loanGuarantorAttachmentHistoryEntries.ForEach(loanGuarantorAttachmentHistoryEntryDTO =>
                            {
                                var destinationCustomerAccount = _customerAccountAppService.FindCustomerAccountDTO(loanGuarantorAttachmentHistoryEntryDTO.DestinationCustomerAccountId, serviceHeader);

                                _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { destinationCustomerAccount }, serviceHeader, true);

                                var destinationLoanProduct = _loanProductAppService.FindLoanProduct(destinationCustomerAccount.CustomerAccountTypeTargetProductId, serviceHeader);

                                loanGuarantorAttachmentHistoryEntryDTO.AccountAlertCustomerId = sourceCustomerAccount.CustomerId;

                                loanGuarantorAttachmentHistoryEntryDTO.AccountAlertPrimaryDescription = string.Format("Guarantor Relieving (SN#{0}/SN#{1})", destinationCustomerAccount.PaddedCustomerSerialNumber, sourceCustomerAccount.PaddedCustomerSerialNumber);

                                loanGuarantorAttachmentHistoryEntryDTO.AccountAlertSecondaryDescription = sourceLoanProduct.Description;

                                loanGuarantorAttachmentHistoryEntryDTO.AccountAlertReference = loanGuarantorAttachmentHistoryEntryDTO.Reference;

                                var principalBalance = destinationCustomerAccount.PrincipalBalance * -1 > 0m ? destinationCustomerAccount.PrincipalBalance * -1 : 0m;

                                var interestBalance = destinationCustomerAccount.InterestBalance * -1 > 0m ? destinationCustomerAccount.InterestBalance * -1 : 0m;

                                loanGuarantorAttachmentHistoryEntryDTO.AccountAlertTotalValue = principalBalance + interestBalance;

                                var relievingPrincipalJournal = JournalFactory.CreateJournal(null, currentPostingPeriodDTO.Id, sourceCustomerAccount.BranchId, null, principalBalance, loanGuarantorAttachmentHistoryEntryDTO.AccountAlertPrimaryDescription, loanGuarantorAttachmentHistoryEntryDTO.AccountAlertSecondaryDescription, loanGuarantorAttachmentHistoryEntryDTO.AccountAlertReference, moduleNavigationItemCode, (int)SystemTransactionCode.GuarantorRelieving, null, serviceHeader, true);
                                _journalEntryPostingService.PerformDoubleEntry(relievingPrincipalJournal, destinationLoanProduct.ChartOfAccountId, sourceLoanProduct.ChartOfAccountId, destinationCustomerAccount, sourceCustomerAccount, serviceHeader);
                                journals.Add(relievingPrincipalJournal);

                                var relievingInterestJournal = JournalFactory.CreateJournal(null, currentPostingPeriodDTO.Id, sourceCustomerAccount.BranchId, null, interestBalance, loanGuarantorAttachmentHistoryEntryDTO.AccountAlertPrimaryDescription, loanGuarantorAttachmentHistoryEntryDTO.AccountAlertSecondaryDescription, loanGuarantorAttachmentHistoryEntryDTO.AccountAlertReference, moduleNavigationItemCode, (int)SystemTransactionCode.GuarantorRelieving, null, serviceHeader, true);
                                _journalEntryPostingService.PerformDoubleEntry(relievingInterestJournal, destinationLoanProduct.InterestReceivableChartOfAccountId, sourceLoanProduct.InterestReceivableChartOfAccountId, destinationCustomerAccount, sourceCustomerAccount, serviceHeader);
                                journals.Add(relievingInterestJournal);

                                #region Do we need to send alerts?

                                _brokerService.ProcessGuarantorRelievingAccountAlerts(DMLCommand.None, serviceHeader, loanGuarantorAttachmentHistoryEntryDTO);

                                #endregion
                            });
                        }
                    }
                }
                #region Bulk-Insert journals

                if (result)
                    result = _journalEntryPostingService.BulkSave(serviceHeader, journals);

                #endregion
            }

            return result;
        }

        public bool RemoveLoanGuarantors(List<LoanGuarantorDTO> loanGuarantorDTOs, ServiceHeader serviceHeader)
        {
            if (loanGuarantorDTOs == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                foreach (var item in loanGuarantorDTOs)
                {
                    var persisted = _loanGuarantorRepository.Get(item.Id, serviceHeader);

                    if (persisted != null)
                    {
                        persisted.Status = (int)LoanGuarantorStatus.Released;
                    }
                }

                return dbContextScope.SaveChanges(serviceHeader) > 0;
            }
        }

        public LoanGuarantorDTO AddNewLoanGuarantor(LoanGuarantorDTO loanGuarantorDTO, ServiceHeader serviceHeader) //Management
        {
            if (loanGuarantorDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var existingGuarantors = _loanGuarantorRepository.AllMatching(LoanGuarantorSpecifications.LoanGuarantorWithLoaneeCustomerIdAndLoanProductId(loanGuarantorDTO.LoaneeCustomerId ?? Guid.Empty, loanGuarantorDTO.LoanProductId), serviceHeader);

                    if (existingGuarantors != null && existingGuarantors.Any(x => x.Status == (int)LoanGuarantorStatus.Attached && x.CustomerId == loanGuarantorDTO.CustomerId))
                    {
                        loanGuarantorDTO.ErrorMsgResult = string.Format("Sorry, but the selected customer is already attached as a guarantor for the selected loan account!");
                        return loanGuarantorDTO;
                    }

                    var loanGuarantor = LoanGuarantorFactory.CreateLoanGuarantor(loanGuarantorDTO.CustomerId, loanGuarantorDTO.LoaneeCustomerId, loanGuarantorDTO.LoanProductId, loanGuarantorDTO.LoanCaseId, loanGuarantorDTO.TotalShares, loanGuarantorDTO.CommittedShares, loanGuarantorDTO.AmountGuaranteed, loanGuarantorDTO.AmountPledged, loanGuarantorDTO.AppraisalFactor);

                    loanGuarantor.CreatedBy = serviceHeader.ApplicationUserName;

                    _loanGuarantorRepository.Add(loanGuarantor, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return loanGuarantor.ProjectedAs<LoanGuarantorDTO>();
                }
            }
            else return null;
        }

        private List<LoanDisbursementBatchEntryDTO> FindLoanDisbursementBatchEntriesByLoanCaseId(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            if (loanCaseId != null && loanCaseId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = LoanDisbursementBatchEntrySpecifications.LoanDisbursementBatchEntryWithLoanCaseId(loanCaseId);

                    ISpecification<LoanDisbursementBatchEntry> spec = filter;

                    var loanDisbursementBatchEntries = _loanDisbursementBatchEntryRepository.AllMatching(spec, serviceHeader);

                    if (loanDisbursementBatchEntries != null && loanDisbursementBatchEntries.Any())
                    {
                        return loanDisbursementBatchEntries.ProjectedAsCollection<LoanDisbursementBatchEntryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private bool ReleaseLoanCaseGuarantors(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var loanGuarantors = FindLoanGuarantorsByLoanCaseId(loanCaseId, serviceHeader);

            if (loanGuarantors != null && loanGuarantors.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var item in loanGuarantors)
                    {
                        if (item.Status == (int)LoanGuarantorStatus.Attached)
                        {
                            var persisted = _loanGuarantorRepository.Get(item.Id, serviceHeader);

                            persisted.Status = (int)LoanGuarantorStatus.Released;
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }

            var loanCollaterals = FindLoanCollateralsByLoanCaseId(loanCaseId, serviceHeader);

            if (loanCollaterals != null && loanCollaterals.Any())
            {
                foreach (var item in loanCollaterals)
                {
                    result = _customerDocumentAppService.UpdateCollateralStatus(item.CustomerDocumentId, (int)CollateralStatus.Released, serviceHeader);
                }
            }

            return result;
        }

        private async Task<bool> ReleaseLoanCaseGuarantorsAsync(Guid loanCaseId, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var loanGuarantors = FindLoanGuarantorsByLoanCaseId(loanCaseId, serviceHeader);

            if (loanGuarantors != null && loanGuarantors.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    foreach (var item in loanGuarantors)
                    {
                        if (item.Status == (int)LoanGuarantorStatus.Attached)
                        {
                            var persisted = await _loanGuarantorRepository.GetAsync(item.Id, serviceHeader);

                            persisted.Status = (int)LoanGuarantorStatus.Released;
                        }
                    }

                    result = await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                }
            }

            var loanCollaterals = FindLoanCollateralsByLoanCaseId(loanCaseId, serviceHeader);

            if (loanCollaterals != null && loanCollaterals.Any())
            {
                foreach (var item in loanCollaterals)
                {
                    result = await _customerDocumentAppService.UpdateCollateralStatusAsync(item.CustomerDocumentId, (int)CollateralStatus.Released, serviceHeader);
                }
            }

            return result;
        }

        public bool ReleaseLoanGuarantors(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (customerAccountDTO != null)
            {
                var loanGuarantors = FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                var loanCollaterals = FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    if (loanGuarantors != null && loanGuarantors.Any())
                    {
                        foreach (var item in loanGuarantors)
                        {
                            var persisted = _loanGuarantorRepository.Get(item.Id, serviceHeader);

                            if (persisted != null)
                            {
                                var releaseGuarantor = true;

                                if (persisted.LoanCase != null)
                                    releaseGuarantor = persisted.LoanCase.Status.In((int)LoanCaseStatus.Disbursed, (int)LoanCaseStatus.Rejected);

                                if (releaseGuarantor)
                                    persisted.Status = (int)LoanGuarantorStatus.Released;
                            }
                        }
                    }

                    if (loanCollaterals != null && loanCollaterals.Any())
                    {
                        foreach (var item in loanCollaterals)
                        {
                            var persisted = _customerDocumentAppService.FindCustomerDocument(item.Id, serviceHeader);

                            if (persisted != null)
                            {
                                persisted.CollateralStatus = (int)CollateralStatus.Released;

                                _customerDocumentAppService.UpdateCustomerDocument(persisted, null, serviceHeader);
                            }
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }

            return result;
        }

        public async Task<bool> ReleaseLoanGuarantorsAsync(CustomerAccountDTO customerAccountDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (customerAccountDTO != null)
            {
                var loanGuarantors = FindLoanGuarantorsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                var loanCollaterals = FindLoanCollateralsByLoaneeCustomerIdAndLoanProductId(customerAccountDTO.CustomerId, customerAccountDTO.CustomerAccountTypeTargetProductId, serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    if (loanGuarantors != null && loanGuarantors.Any())
                    {
                        foreach (var item in loanGuarantors)
                        {
                            var persisted = await _loanGuarantorRepository.GetAsync(item.Id, serviceHeader);

                            if (persisted != null)
                            {
                                var releaseGuarantor = true;

                                if (persisted.LoanCase != null)
                                    releaseGuarantor = persisted.LoanCase.Status.In((int)LoanCaseStatus.Disbursed, (int)LoanCaseStatus.Rejected);

                                if (releaseGuarantor)
                                    persisted.Status = (int)LoanGuarantorStatus.Released;
                            }
                        }
                    }

                    if (loanCollaterals != null && loanCollaterals.Any())
                    {
                        foreach (var item in loanCollaterals)
                        {
                            var persisted = await _customerDocumentAppService.FindCustomerDocumentAsync(item.Id, serviceHeader);

                            if (persisted != null)
                            {
                                persisted.CollateralStatus = (int)CollateralStatus.Released;

                                await _customerDocumentAppService.UpdateCustomerDocumentAsync(persisted, null, serviceHeader);
                            }
                        }
                    }

                    result = dbContextScope.SaveChanges(serviceHeader) > 0;
                }
            }

            return result;
        }
    }
}