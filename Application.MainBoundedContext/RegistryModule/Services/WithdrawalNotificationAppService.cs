using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalNotificationAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalSettlementAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class WithdrawalNotificationAppService : IWithdrawalNotificationAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<WithdrawalNotification> _withdrawalNotificationRepository;
        private readonly IRepository<WithdrawalSettlement> _withdrawalSettlementRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IHolidayAppService _holidayAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IBranchAppService _branchAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly IPostingPeriodAppService _postingPeriodAppService;

        public WithdrawalNotificationAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<WithdrawalNotification> withdrawalNotificationRepository,
            IRepository<WithdrawalSettlement> withdrawalSettlementRepository,
            IRepository<Customer> customerRepository,
            IChartOfAccountAppService chartOfAccountAppService,
            ICustomerAccountAppService customerAccountAppService,
            ISavingsProductAppService savingsProductAppService,
            IHolidayAppService holidayAppService,
            ICommissionAppService commissionAppService,
            IBranchAppService branchAppService,
            IInvestmentProductAppService investmentProductAppService,
            IJournalEntryPostingService journalEntryPostingService,
            IPostingPeriodAppService postingPeriodAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (withdrawalNotificationRepository == null)
                throw new ArgumentNullException(nameof(withdrawalNotificationRepository));

            if (withdrawalSettlementRepository == null)
                throw new ArgumentNullException(nameof(withdrawalSettlementRepository));

            if (customerRepository == null)
                throw new ArgumentNullException(nameof(customerRepository));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (holidayAppService == null)
                throw new ArgumentNullException(nameof(holidayAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (branchAppService == null)
                throw new ArgumentNullException(nameof(branchAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _withdrawalNotificationRepository = withdrawalNotificationRepository;
            _withdrawalSettlementRepository = withdrawalSettlementRepository;
            _customerRepository = customerRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
            _savingsProductAppService = savingsProductAppService;
            _holidayAppService = holidayAppService;
            _commissionAppService = commissionAppService;
            _branchAppService = branchAppService;
            _investmentProductAppService = investmentProductAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _postingPeriodAppService = postingPeriodAppService;
        }

        public WithdrawalNotificationDTO AddNewWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, ServiceHeader serviceHeader)
        {
            if (withdrawalNotificationDTO != null)
            {
                var existingNotifications = FindWithdrawalNotificationsByCustomerId(withdrawalNotificationDTO.CustomerId, serviceHeader);

                if (existingNotifications != null && existingNotifications.Any(x => x.Category == withdrawalNotificationDTO.Category))
                {
                    foreach (var item in existingNotifications)
                    {
                        switch ((WithdrawalNotificationStatus)item.Status)
                        {
                            case WithdrawalNotificationStatus.Registered:
                            case WithdrawalNotificationStatus.Approved:
                            case WithdrawalNotificationStatus.Audited:
                            case WithdrawalNotificationStatus.WithdrawalSettled:
                            case WithdrawalNotificationStatus.DeathClaimSettled:
                                withdrawalNotificationDTO.ErrorMessageResult = ("Sorry, but a membership termination notification for the selected customer is currently undergoing processing!");
                                return withdrawalNotificationDTO;
                            case WithdrawalNotificationStatus.Deferred:
                            default:
                                break;
                        }
                    }
                }

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var withdrawalNotification = WithdrawalNotificationFactory.CreateWithdrawalNotification(withdrawalNotificationDTO.CustomerId, withdrawalNotificationDTO.BranchId, withdrawalNotificationDTO.Category, withdrawalNotificationDTO.Remarks);

                    switch ((WithdrawalNotificationCategory)withdrawalNotificationDTO.Category)
                    {
                        case WithdrawalNotificationCategory.Deceased:
                            withdrawalNotification.MaturityDate = DateTime.Today;
                            break;
                        case WithdrawalNotificationCategory.Voluntary:
                        case WithdrawalNotificationCategory.Retiree:
                            var branchDTO = _branchAppService.FindBranch(withdrawalNotificationDTO.BranchId, serviceHeader);
                            withdrawalNotification.MaturityDate = _holidayAppService.FindBusinessDay(branchDTO.CompanyMembershipTerminationNoticePeriod, true, serviceHeader) ?? DateTime.Today;
                            break;
                        default:
                            break;
                    }

                    withdrawalNotification.SettlementType = (int)MembershipWithdrawalSettlementType.Normal;
                    withdrawalNotification.Status = (int)WithdrawalNotificationStatus.Registered;
                    withdrawalNotification.CreatedBy = serviceHeader.ApplicationUserName;

                    _withdrawalNotificationRepository.Add(withdrawalNotification, serviceHeader);

                    #region Lock Customer

                    var persistedCustomer = _customerRepository.Get(withdrawalNotificationDTO.CustomerId, serviceHeader);
                    persistedCustomer.Remarks = string.Format("{0} Membership Termination Notice Placed", EnumHelper.GetDescription((WithdrawalNotificationCategory)withdrawalNotificationDTO.Category));
                    persistedCustomer.Lock();

                    #endregion

                    #region Lock Customer Accounts

                    var customerLoanAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCode(withdrawalNotificationDTO.CustomerId, (int)ProductCode.Loan, serviceHeader);

                    if (customerLoanAccounts != null && customerLoanAccounts.Any())
                    {
                        foreach (var customerLoanAccount in customerLoanAccounts)
                        {
                            // freeze
                            _customerAccountAppService.ManageCustomerAccount(customerLoanAccount.Id, (int)CustomerAccountManagementAction.Deactivation, string.Format("{0} Membership Termination Notice Placed", EnumHelper.GetDescription((WithdrawalNotificationCategory)withdrawalNotificationDTO.Category)), (int)CustomerAccountRemarkType.Actionable, serviceHeader);
                        }
                    }

                    var customerInvestmentAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCode(withdrawalNotificationDTO.CustomerId, (int)ProductCode.Investment, serviceHeader);

                    if (customerInvestmentAccounts != null && customerInvestmentAccounts.Any())
                    {
                        foreach (var customerInvestmentAccount in customerInvestmentAccounts)
                        {
                            // freeze
                            _customerAccountAppService.ManageCustomerAccount(customerInvestmentAccount.Id, (int)CustomerAccountManagementAction.Deactivation, string.Format("{0} Membership Termination Notice Placed", EnumHelper.GetDescription((WithdrawalNotificationCategory)withdrawalNotificationDTO.Category)), (int)CustomerAccountRemarkType.Actionable, serviceHeader);
                        }
                    }

                    var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCode(withdrawalNotificationDTO.CustomerId, (int)ProductCode.Savings, serviceHeader);

                    if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                    {
                        foreach (var customerSavingsAccount in customerSavingsAccounts)
                        {
                            // freeze
                            _customerAccountAppService.ManageCustomerAccount(customerSavingsAccount.Id, (int)CustomerAccountManagementAction.Deactivation, string.Format("{0} Membership Termination Notice Placed", EnumHelper.GetDescription((WithdrawalNotificationCategory)withdrawalNotificationDTO.Category)), (int)CustomerAccountRemarkType.Actionable, serviceHeader);
                        }
                    }

                    #endregion

                    if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                    {
                        return withdrawalNotification.ProjectedAs<WithdrawalNotificationDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public bool ApproveWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalApprovalOption, ServiceHeader serviceHeader)
        {
            var approvalOK = default(bool);

            if (withdrawalNotificationDTO != null && Enum.IsDefined(typeof(MembershipWithdrawalApprovalOption), membershipWithdrawalApprovalOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _withdrawalNotificationRepository.Get(withdrawalNotificationDTO.Id, serviceHeader);

                    if (persisted != null && (persisted.Status == (int)WithdrawalNotificationStatus.Registered || persisted.Status == (int)WithdrawalNotificationStatus.Deferred))
                    {
                        switch ((MembershipWithdrawalApprovalOption)membershipWithdrawalApprovalOption)
                        {
                            case MembershipWithdrawalApprovalOption.Approve:
                                persisted.Status = (int)WithdrawalNotificationStatus.Approved;
                                break;
                            case MembershipWithdrawalApprovalOption.Defer:
                                persisted.Status = (int)WithdrawalNotificationStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        persisted.ApprovalRemarks = withdrawalNotificationDTO.ApprovalRemarks;
                        persisted.ApprovedBy = serviceHeader.ApplicationUserName;
                        persisted.ApprovedDate = DateTime.Now;

                        approvalOK = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
            }

            return approvalOK;
        }

        public bool AuditWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalAuditOption, ServiceHeader serviceHeader)
        {
            var auditOK = default(bool);

            if (withdrawalNotificationDTO != null && Enum.IsDefined(typeof(MembershipWithdrawalAuditOption), membershipWithdrawalAuditOption))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _withdrawalNotificationRepository.Get(withdrawalNotificationDTO.Id, serviceHeader);

                    if (persisted != null && (persisted.Status == (int)WithdrawalNotificationStatus.Approved))
                    {
                        switch ((MembershipWithdrawalAuditOption)membershipWithdrawalAuditOption)
                        {
                            case MembershipWithdrawalAuditOption.Audit:
                                persisted.Status = (int)WithdrawalNotificationStatus.Audited;
                                break;
                            case MembershipWithdrawalAuditOption.Defer:
                                persisted.Status = (int)WithdrawalNotificationStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        persisted.AuditRemarks = withdrawalNotificationDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        auditOK = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }
            }

            return auditOK;
        }

        public bool SettleWithdrawalNotification(WithdrawalNotificationDTO withdrawalNotificationDTO, int membershipWithdrawalSettlementOption, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var settlementOK = default(bool);

            if (withdrawalNotificationDTO != null && Enum.IsDefined(typeof(MembershipWithdrawalSettlementOption), membershipWithdrawalSettlementOption))
            {
                var journals = new List<Journal>();

                var customerAccountCarryForwards = new List<CustomerAccountCarryForward>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _withdrawalNotificationRepository.Get(withdrawalNotificationDTO.Id, serviceHeader);
                    persisted.Status = (int)WithdrawalNotificationStatus.Audited;
                    if (persisted != null && (persisted.Status == (int)WithdrawalNotificationStatus.Audited))
                    {
                        switch ((MembershipWithdrawalSettlementOption)membershipWithdrawalSettlementOption)
                        {
                            case MembershipWithdrawalSettlementOption.Settle:

                                var defaultSavingsProduct = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

                                var deceasedControlChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.DeceasedControl, serviceHeader);

                                var payablesControlChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.PayablesControl, serviceHeader);

                                var postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

                                if (defaultSavingsProduct != null && deceasedControlChartOfAccountId != Guid.Empty && payablesControlChartOfAccountId != Guid.Empty && postingPeriodDTO != null)
                                {
                                    var customerLoanAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCode(withdrawalNotificationDTO.CustomerId, (int)ProductCode.Loan, serviceHeader);

                                    var customerInvestmentAccounts = _customerAccountAppService.FindCustomerAccountsByCustomerIdAndCustomerAccountTypeTargetProductCode(withdrawalNotificationDTO.CustomerId, (int)ProductCode.Investment, serviceHeader);

                                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerLoanAccounts, serviceHeader);
                                    _customerAccountAppService.FetchCustomerAccountBalances(customerLoanAccounts, serviceHeader, true);

                                    _customerAccountAppService.FetchCustomerAccountsProductDescription(customerInvestmentAccounts, serviceHeader);
                                    _customerAccountAppService.FetchCustomerAccountBalances(customerInvestmentAccounts, serviceHeader);

                                    var primaryDescription = string.Format("Membership Termination - {0}", EnumHelper.GetDescription((WithdrawalNotificationCategory)persisted.Category));

                                    var secondaryDescription = string.Empty;

                                    var reference = string.Format("{0} - {1}", withdrawalNotificationDTO.CustomerReference3, persisted.Remarks);

                                    switch ((WithdrawalNotificationCategory)persisted.Category)
                                    {
                                        case WithdrawalNotificationCategory.Deceased:

                                            decimal deceasedInterestTotal = 0m;
                                            decimal deceasedCarryForwardsTotal = 0m;
                                            decimal deceasedInvestmentsTotal = 0m;

                                            #region Loan Accounts

                                            if (customerLoanAccounts != null && customerLoanAccounts.Any())
                                            {
                                                foreach (var customerLoanAccount in customerLoanAccounts)
                                                {
                                                    secondaryDescription = customerLoanAccount.CustomerAccountTypeTargetProductDescription;

                                                    if (customerLoanAccount.PrincipalBalance * -1 > 0m)
                                                    {
                                                        // credit loan product, debit deceased ctrl
                                                        var deceasedLoanPrincipalJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.PrincipalBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(deceasedLoanPrincipalJournal, customerLoanAccount.CustomerAccountTypeTargetProductChartOfAccountId, deceasedControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(deceasedLoanPrincipalJournal);
                                                    }

                                                    if (customerLoanAccount.InterestBalance * -1 > 0m)
                                                    {
                                                        // credit interest receivable, debit deceased ctrl
                                                        var deceasedLoanInterestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.InterestBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(deceasedLoanInterestReceivableJournal, customerLoanAccount.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, deceasedControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(deceasedLoanInterestReceivableJournal);

                                                        // credit interest received, debit interest charged
                                                        var deceasedLoanInterestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.InterestBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(deceasedLoanInterestReceivedJournal, customerLoanAccount.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, customerLoanAccount.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(deceasedLoanInterestReceivedJournal);

                                                        // tally
                                                        deceasedInterestTotal += customerLoanAccount.InterestBalance * -1;
                                                    }

                                                    if (customerLoanAccount.CarryForwardsBalance * -1 > 0m)
                                                    {
                                                        var carryForwards = _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(customerLoanAccount.Id, serviceHeader);

                                                        if (carryForwards != null && carryForwards.Any())
                                                        {
                                                            var grouping = from p in carryForwards
                                                                           group p.Amount by new
                                                                           {
                                                                               p.BeneficiaryCustomerAccountId,
                                                                               p.BeneficiaryChartOfAccountId
                                                                           } into g
                                                                           select new
                                                                           {
                                                                               BeneficiaryCustomerAccountId = g.Key.BeneficiaryCustomerAccountId,
                                                                               BeneficiaryChartOfAccountId = g.Key.BeneficiaryChartOfAccountId,
                                                                               Payments = g.ToList()
                                                                           };

                                                            foreach (var item in grouping)
                                                            {
                                                                var totalPayments = item.Payments.Sum();

                                                                if (totalPayments * -1 < 0m)
                                                                {
                                                                    var carryForwardArrears = totalPayments;

                                                                    if (carryForwardArrears > 0m)
                                                                    {
                                                                        // Credit CarryForward.BeneficiaryChartOfAccountId, Debit deceased ctrl
                                                                        var carryFowardBeneficiaryJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, carryForwardArrears, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                                        _journalEntryPostingService.PerformDoubleEntry(carryFowardBeneficiaryJournal, item.BeneficiaryChartOfAccountId, deceasedControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                                        journals.Add(carryFowardBeneficiaryJournal);
                                                                    }
                                                                }
                                                            }

                                                            foreach (var item in carryForwards)
                                                            {
                                                                // Do we need to update carry forward history?
                                                                var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(item.BenefactorCustomerAccountId, item.BeneficiaryCustomerAccountId, item.BeneficiaryChartOfAccountId, item.Amount * -1/*-ve cos is payment*/, primaryDescription);
                                                                customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                                                customerAccountCarryForwards.Add(customerAccountCarryForward);

                                                                // tally
                                                                deceasedCarryForwardsTotal += item.Amount;
                                                            }
                                                        }
                                                    }

                                                    // freeze
                                                    _customerAccountAppService.ManageCustomerAccount(customerLoanAccount.Id, (int)CustomerAccountManagementAction.Deactivation, reference, (int)CustomerAccountRemarkType.Actionable, serviceHeader);

                                                    // log settlement
                                                    var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerLoanAccount.Id, customerLoanAccount.PrincipalBalance, customerLoanAccount.InterestBalance, customerLoanAccount.CarryForwardsBalance, primaryDescription);
                                                    withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                    _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            #region Investment Accounts

                                            if (customerInvestmentAccounts != null && customerInvestmentAccounts.Any())
                                            {
                                                foreach (var customerInvestmentAccount in customerInvestmentAccounts)
                                                {
                                                    secondaryDescription = customerInvestmentAccount.CustomerAccountTypeTargetProductDescription;

                                                    if (customerInvestmentAccount.CustomerAccountTypeTargetProductIsRefundable && customerInvestmentAccount.BookBalance > 0m)
                                                    {
                                                        // credit deceased ctrl, debit investment account 
                                                        var deceasedRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(deceasedRefundableInvestmentJournal, deceasedControlChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, customerInvestmentAccount, customerInvestmentAccount, serviceHeader);
                                                        journals.Add(deceasedRefundableInvestmentJournal);

                                                        // tally
                                                        deceasedInvestmentsTotal += customerInvestmentAccount.BookBalance;

                                                        // log settlement
                                                        var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerInvestmentAccount.Id, customerInvestmentAccount.BookBalance, 0m, 0m, primaryDescription);
                                                        withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                        _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                    }
                                                    else if (!customerInvestmentAccount.CustomerAccountTypeTargetProductIsRefundable && customerInvestmentAccount.BookBalance > 0m && customerInvestmentAccount.CustomerAccountTypeTargetProductTransferBalanceToParentOnMembershipTermination)
                                                    {
                                                        if (customerInvestmentAccount.CustomerAccountTypeTargetProductParentId != null && customerInvestmentAccount.CustomerAccountTypeTargetProductParentId != Guid.Empty)
                                                        {
                                                            var parentCustomerInvestmentAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, customerInvestmentAccount.CustomerAccountTypeTargetProductParentId ?? Guid.Empty, serviceHeader);

                                                            if (parentCustomerInvestmentAccounts != null && parentCustomerInvestmentAccounts.Any())
                                                            {
                                                                _customerAccountAppService.FetchCustomerAccountsProductDescription(parentCustomerInvestmentAccounts, serviceHeader, true);

                                                                var parentCustomerInvestmentAccountDTO = parentCustomerInvestmentAccounts.FirstOrDefault();

                                                                if (parentCustomerInvestmentAccountDTO != null)
                                                                {
                                                                    // credit parent investment, debit investment 
                                                                    var deceasedNonRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                                    _journalEntryPostingService.PerformDoubleEntry(deceasedNonRefundableInvestmentJournal, parentCustomerInvestmentAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, parentCustomerInvestmentAccountDTO, customerInvestmentAccount, serviceHeader);
                                                                    journals.Add(deceasedNonRefundableInvestmentJournal);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                // create account and post tx
                                                                var parentInvestmentProduct = _investmentProductAppService.FindInvestmentProduct(customerInvestmentAccount.CustomerAccountTypeTargetProductParentId ?? Guid.Empty, serviceHeader);

                                                                if (parentInvestmentProduct != null)
                                                                {
                                                                    var parentCustomerInvestmentAccountDTO = new CustomerAccountDTO
                                                                    {
                                                                        BranchId = persisted.BranchId,
                                                                        CustomerId = persisted.CustomerId,
                                                                        CustomerAccountTypeProductCode = (int)ProductCode.Investment,
                                                                        CustomerAccountTypeTargetProductId = parentInvestmentProduct.Id,
                                                                        CustomerAccountTypeTargetProductCode = parentInvestmentProduct.Code,
                                                                        Status = (int)CustomerAccountStatus.Normal,
                                                                    };

                                                                    parentCustomerInvestmentAccountDTO = _customerAccountAppService.AddNewCustomerAccount(parentCustomerInvestmentAccountDTO, serviceHeader);

                                                                    if (parentCustomerInvestmentAccountDTO != null)
                                                                    {
                                                                        // credit parent investment, debit investment 
                                                                        var deceasedNonRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                                        _journalEntryPostingService.PerformDoubleEntry(deceasedNonRefundableInvestmentJournal, parentInvestmentProduct.ChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, parentCustomerInvestmentAccountDTO, customerInvestmentAccount, serviceHeader);
                                                                        journals.Add(deceasedNonRefundableInvestmentJournal);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    // freeze
                                                    _customerAccountAppService.ManageCustomerAccount(customerInvestmentAccount.Id, (int)CustomerAccountManagementAction.Deactivation, reference, (int)CustomerAccountRemarkType.Actionable, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            #region Savings Account

                                            if (withdrawalNotificationDTO.BranchCompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement)
                                            {
                                                decimal deceasedNetRefundable = deceasedInvestmentsTotal - deceasedInterestTotal - deceasedCarryForwardsTotal;

                                                if (deceasedNetRefundable > 0m)
                                                {
                                                    CustomerAccountDTO customerSavingsAccountDTO = null;

                                                    var customerSavingsAccountDTOs = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, defaultSavingsProduct.Id, serviceHeader);

                                                    if (customerSavingsAccountDTOs != null && customerSavingsAccountDTOs.Any())
                                                        customerSavingsAccountDTO = customerSavingsAccountDTOs.First();
                                                    else
                                                    {
                                                        var customerAccountDTO = new CustomerAccountDTO
                                                        {
                                                            BranchId = persisted.BranchId,
                                                            CustomerId = persisted.CustomerId,
                                                            CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                                                            CustomerAccountTypeTargetProductId = defaultSavingsProduct.Id,
                                                            CustomerAccountTypeTargetProductCode = defaultSavingsProduct.Code,
                                                            Status = (int)CustomerAccountStatus.Normal,
                                                        };

                                                        customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                                        if (customerAccountDTO != null)
                                                            customerSavingsAccountDTO = customerAccountDTO;
                                                    }

                                                    if (customerSavingsAccountDTO != null)
                                                    {
                                                        secondaryDescription = "Net Refund";

                                                        var deceasedMembershipTerminationProcessingFeeTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.DeceasedMembershipTerminationProcessingFee, deceasedNetRefundable, customerSavingsAccountDTO, serviceHeader);

                                                        // credit savings account, debit deceased ctrl 
                                                        var deceasedSavingsJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, deceasedNetRefundable, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                        _journalEntryPostingService.PerformDoubleEntry(deceasedSavingsJournal, defaultSavingsProduct.ChartOfAccountId, deceasedControlChartOfAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                                        journals.Add(deceasedSavingsJournal);

                                                        deceasedMembershipTerminationProcessingFeeTariffs.ForEach(tariff =>
                                                        {
                                                            var deceasedTariffJournal = JournalFactory.CreateJournal(deceasedSavingsJournal.Id, postingPeriodDTO.Id, persisted.BranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                            _journalEntryPostingService.PerformDoubleEntry(deceasedTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                                            journals.Add(deceasedTariffJournal);
                                                        });

                                                        // log settlement
                                                        var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerSavingsAccountDTO.Id, deceasedNetRefundable, 0m, 0m, primaryDescription);
                                                        withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                        _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                    }
                                                }
                                            }

                                            #endregion

                                            persisted.Status = (int)WithdrawalNotificationStatus.WithdrawalSettled;
                                            persisted.SettledBy = serviceHeader.ApplicationUserName;
                                            persisted.SettledDate = DateTime.Now;
                                            persisted.SettlementRemarks = withdrawalNotificationDTO.SettlementRemarks;
                                            persisted.SettlementType = (byte)withdrawalNotificationDTO.SettlementType;
                                            persisted.Lock();

                                            var persistedCustomer_Deceased = _customerRepository.Get(persisted.CustomerId, serviceHeader);
                                            persistedCustomer_Deceased.RecordStatus = (int)RecordStatus.Edited;
                                            persistedCustomer_Deceased.Remarks = persisted.Remarks;
                                            persistedCustomer_Deceased.Lock();

                                            break;
                                        case WithdrawalNotificationCategory.Voluntary:

                                            var voluntaryLoansTotal = 0m;
                                            var voluntaryInvestmentsTotal = 0m;

                                            #region Loan Accounts

                                            if (customerLoanAccounts != null && customerLoanAccounts.Any())
                                            {
                                                foreach (var customerLoanAccount in customerLoanAccounts)
                                                {
                                                    secondaryDescription = customerLoanAccount.CustomerAccountTypeTargetProductDescription;

                                                    if (customerLoanAccount.PrincipalBalance * -1 > 0m)
                                                    {
                                                        // credit loan product, debit payables ctrl
                                                        var voluntaryLoanPrincipalJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.PrincipalBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(voluntaryLoanPrincipalJournal, customerLoanAccount.CustomerAccountTypeTargetProductChartOfAccountId, payablesControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(voluntaryLoanPrincipalJournal);

                                                        // tally
                                                        voluntaryLoansTotal += customerLoanAccount.PrincipalBalance * -1;
                                                    }

                                                    if (customerLoanAccount.InterestBalance * -1 > 0m)
                                                    {
                                                        // credit interest receivable, debit payables ctrl
                                                        var voluntaryLoanInterestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.InterestBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(voluntaryLoanInterestReceivableJournal, customerLoanAccount.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, payablesControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(voluntaryLoanInterestReceivableJournal);

                                                        // credit interest received, debit interest charged
                                                        var voluntaryLoanInterestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.InterestBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(voluntaryLoanInterestReceivedJournal, customerLoanAccount.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, customerLoanAccount.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(voluntaryLoanInterestReceivedJournal);

                                                        // tally
                                                        voluntaryLoansTotal += customerLoanAccount.InterestBalance * -1;
                                                    }

                                                    if (customerLoanAccount.CarryForwardsBalance * -1 > 0m)
                                                    {
                                                        var carryForwards = _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(customerLoanAccount.Id, serviceHeader);

                                                        if (carryForwards != null && carryForwards.Any())
                                                        {
                                                            var grouping = from p in carryForwards
                                                                           group p.Amount by new
                                                                           {
                                                                               p.BeneficiaryCustomerAccountId,
                                                                               p.BeneficiaryChartOfAccountId
                                                                           } into g
                                                                           select new
                                                                           {
                                                                               BeneficiaryCustomerAccountId = g.Key.BeneficiaryCustomerAccountId,
                                                                               BeneficiaryChartOfAccountId = g.Key.BeneficiaryChartOfAccountId,
                                                                               Payments = g.ToList()
                                                                           };

                                                            foreach (var item in grouping)
                                                            {
                                                                var totalPayments = item.Payments.Sum();

                                                                if (totalPayments * -1 < 0m)
                                                                {
                                                                    var carryForwardArrears = totalPayments;

                                                                    if (carryForwardArrears > 0m)
                                                                    {
                                                                        // Credit CarryForward.BeneficiaryChartOfAccountId, debit payables ctrl
                                                                        var carryFowardBeneficiaryJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, carryForwardArrears, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                                        _journalEntryPostingService.PerformDoubleEntry(carryFowardBeneficiaryJournal, item.BeneficiaryChartOfAccountId, payablesControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                                        journals.Add(carryFowardBeneficiaryJournal);
                                                                    }
                                                                }
                                                            }

                                                            foreach (var item in carryForwards)
                                                            {
                                                                // Do we need to update carry forward history?
                                                                var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(item.BenefactorCustomerAccountId, item.BeneficiaryCustomerAccountId, item.BeneficiaryChartOfAccountId, item.Amount * -1/*-ve cos is payment*/, primaryDescription);
                                                                customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                                                customerAccountCarryForwards.Add(customerAccountCarryForward);

                                                                // tally
                                                                voluntaryLoansTotal += item.Amount;
                                                            }
                                                        }
                                                    }

                                                    // freeze
                                                    _customerAccountAppService.ManageCustomerAccount(customerLoanAccount.Id, (int)CustomerAccountManagementAction.Deactivation, reference, (int)CustomerAccountRemarkType.Actionable, serviceHeader);

                                                    // log settlement
                                                    var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerLoanAccount.Id, customerLoanAccount.PrincipalBalance, customerLoanAccount.InterestBalance, customerLoanAccount.CarryForwardsBalance, primaryDescription);
                                                    withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                    _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            #region Investment Accounts

                                            if (customerInvestmentAccounts != null && customerInvestmentAccounts.Any())
                                            {
                                                foreach (var customerInvestmentAccount in customerInvestmentAccounts)
                                                {
                                                    secondaryDescription = customerInvestmentAccount.CustomerAccountTypeTargetProductDescription;

                                                    if (customerInvestmentAccount.CustomerAccountTypeTargetProductIsRefundable && customerInvestmentAccount.BookBalance > 0m)
                                                    {
                                                        // credit common ctrl, debit investment 
                                                        var voluntaryRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(voluntaryRefundableInvestmentJournal, payablesControlChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, customerInvestmentAccount, customerInvestmentAccount, serviceHeader);
                                                        journals.Add(voluntaryRefundableInvestmentJournal);

                                                        // tally
                                                        voluntaryInvestmentsTotal += customerInvestmentAccount.BookBalance;

                                                        // log settlement
                                                        var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerInvestmentAccount.Id, customerInvestmentAccount.BookBalance, 0m, 0m, primaryDescription);
                                                        withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                        _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                    }
                                                    else if (!customerInvestmentAccount.CustomerAccountTypeTargetProductIsRefundable && customerInvestmentAccount.BookBalance > 0m && customerInvestmentAccount.CustomerAccountTypeTargetProductTransferBalanceToParentOnMembershipTermination)
                                                    {
                                                        if (customerInvestmentAccount.CustomerAccountTypeTargetProductParentId != null && customerInvestmentAccount.CustomerAccountTypeTargetProductParentId != Guid.Empty)
                                                        {
                                                            var parentCustomerInvestmentAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, customerInvestmentAccount.CustomerAccountTypeTargetProductParentId ?? Guid.Empty, serviceHeader);

                                                            if (parentCustomerInvestmentAccounts != null && parentCustomerInvestmentAccounts.Any())
                                                            {
                                                                _customerAccountAppService.FetchCustomerAccountsProductDescription(parentCustomerInvestmentAccounts, serviceHeader, true);

                                                                var parentCustomerInvestmentAccountDTO = parentCustomerInvestmentAccounts.FirstOrDefault();

                                                                if (parentCustomerInvestmentAccountDTO != null)
                                                                {
                                                                    // credit parent investment, debit investment 
                                                                    var voluntaryNonRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                                    _journalEntryPostingService.PerformDoubleEntry(voluntaryNonRefundableInvestmentJournal, parentCustomerInvestmentAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, parentCustomerInvestmentAccountDTO, customerInvestmentAccount, serviceHeader);
                                                                    journals.Add(voluntaryNonRefundableInvestmentJournal);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                // create account and post tx
                                                                var parentInvestmentProduct = _investmentProductAppService.FindInvestmentProduct(customerInvestmentAccount.CustomerAccountTypeTargetProductParentId ?? Guid.Empty, serviceHeader);

                                                                if (parentInvestmentProduct != null)
                                                                {
                                                                    var parentCustomerInvestmentAccountDTO = new CustomerAccountDTO
                                                                    {
                                                                        BranchId = persisted.BranchId,
                                                                        CustomerId = persisted.CustomerId,
                                                                        CustomerAccountTypeProductCode = (int)ProductCode.Investment,
                                                                        CustomerAccountTypeTargetProductId = parentInvestmentProduct.Id,
                                                                        CustomerAccountTypeTargetProductCode = parentInvestmentProduct.Code,
                                                                        Status = (int)CustomerAccountStatus.Normal,
                                                                    };

                                                                    parentCustomerInvestmentAccountDTO = _customerAccountAppService.AddNewCustomerAccount(parentCustomerInvestmentAccountDTO, serviceHeader);

                                                                    if (parentCustomerInvestmentAccountDTO != null)
                                                                    {
                                                                        // credit parent investment, debit investment 
                                                                        var voluntaryNonRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                                        _journalEntryPostingService.PerformDoubleEntry(voluntaryNonRefundableInvestmentJournal, parentInvestmentProduct.ChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, parentCustomerInvestmentAccountDTO, customerInvestmentAccount, serviceHeader);
                                                                        journals.Add(voluntaryNonRefundableInvestmentJournal);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    // freeze
                                                    _customerAccountAppService.ManageCustomerAccount(customerInvestmentAccount.Id, (int)CustomerAccountManagementAction.Deactivation, reference, (int)CustomerAccountRemarkType.Actionable, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            #region Savings Account

                                            decimal voluntaryNetRefundable = voluntaryInvestmentsTotal - voluntaryLoansTotal;

                                            if (voluntaryNetRefundable > 0m)
                                            {
                                                CustomerAccountDTO customerSavingsAccountDTO = null;

                                                var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, defaultSavingsProduct.Id, serviceHeader);

                                                if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                                                    customerSavingsAccountDTO = customerSavingsAccounts.First();
                                                else
                                                {
                                                    var customerAccountDTO = new CustomerAccountDTO
                                                    {
                                                        BranchId = persisted.BranchId,
                                                        CustomerId = persisted.CustomerId,
                                                        CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                                                        CustomerAccountTypeTargetProductId = defaultSavingsProduct.Id,
                                                        CustomerAccountTypeTargetProductCode = defaultSavingsProduct.Code,
                                                        Status = (int)CustomerAccountStatus.Normal,
                                                    };

                                                    customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                                    if (customerSavingsAccountDTO != null)
                                                        customerSavingsAccountDTO = customerAccountDTO;
                                                }

                                                if (customerSavingsAccountDTO != null)
                                                {
                                                    secondaryDescription = "Net Refund";

                                                    var tariffs = new List<TariffWrapper>();

                                                    #region loan clearance tariffs

                                                    if (customerLoanAccounts != null && customerLoanAccounts.Any())
                                                    {
                                                        foreach (var customerLoanAccount in customerLoanAccounts)
                                                        {
                                                            if (customerLoanAccount.CustomerAccountTypeTargetProductChargeClearanceFee)
                                                            {
                                                                var voluntaryActualPrincipalBalance = 0m;
                                                                if (customerLoanAccount.PrincipalBalance * -1 > 0m)
                                                                    voluntaryActualPrincipalBalance = customerLoanAccount.PrincipalBalance * -1;

                                                                var voluntaryActualInterestBalance = 0m;
                                                                if (customerLoanAccount.InterestBalance * -1 > 0m)
                                                                    voluntaryActualInterestBalance = customerLoanAccount.InterestBalance * -1;

                                                                var voluntaryClearanceTariffs = _commissionAppService.ComputeTariffsByLoanProduct(customerLoanAccount.CustomerAccountTypeTargetProductId, (int)LoanProductKnownChargeType.LoanClearanceCharges, (voluntaryActualPrincipalBalance + voluntaryActualInterestBalance), voluntaryActualPrincipalBalance, customerSavingsAccountDTO, serviceHeader);

                                                                if (voluntaryClearanceTariffs != null && voluntaryClearanceTariffs.Any())
                                                                    tariffs.AddRange(voluntaryClearanceTariffs);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region membership termination tariffs

                                                    var voluntaryMembershipTerminationProcessingFeeTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.VoluntaryMembershipTerminationProcessingFee, voluntaryNetRefundable, customerSavingsAccountDTO, serviceHeader);
                                                    if (voluntaryMembershipTerminationProcessingFeeTariffs.Any())
                                                        tariffs.AddRange(voluntaryMembershipTerminationProcessingFeeTariffs);

                                                    switch ((MembershipWithdrawalSettlementType)withdrawalNotificationDTO.SettlementType)
                                                    {
                                                        case MembershipWithdrawalSettlementType.Normal:

                                                            var normalTerminationTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.NormalMembershipTerminationCharges, voluntaryNetRefundable, customerSavingsAccountDTO, serviceHeader);
                                                            if (normalTerminationTariffs.Any())
                                                                tariffs.AddRange(normalTerminationTariffs);

                                                            break;
                                                        case MembershipWithdrawalSettlementType.Express:

                                                            var expressTerminationTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.PrematureMembershipTerminationCharges, voluntaryNetRefundable, customerSavingsAccountDTO, serviceHeader);
                                                            if (expressTerminationTariffs.Any())
                                                                tariffs.AddRange(expressTerminationTariffs);

                                                            break;
                                                        case MembershipWithdrawalSettlementType.Waiver:
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                    #endregion

                                                    // credit savings, debit common ctrol 
                                                    var voluntarySavingsJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, voluntaryNetRefundable, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                    _journalEntryPostingService.PerformDoubleEntry(voluntarySavingsJournal, defaultSavingsProduct.ChartOfAccountId, payablesControlChartOfAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                                    journals.Add(voluntarySavingsJournal);

                                                    tariffs.ForEach(tariff =>
                                                    {
                                                        var voluntaryTariffJournal = JournalFactory.CreateJournal(voluntarySavingsJournal.Id, postingPeriodDTO.Id, persisted.BranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                        _journalEntryPostingService.PerformDoubleEntry(voluntaryTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                                        journals.Add(voluntaryTariffJournal);
                                                    });

                                                    // log settlement
                                                    var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerSavingsAccountDTO.Id, voluntaryNetRefundable, 0m, 0m, primaryDescription);
                                                    withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                    _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            persisted.Status = (int)WithdrawalNotificationStatus.WithdrawalSettled;
                                            persisted.SettledBy = serviceHeader.ApplicationUserName;
                                            persisted.SettledDate = DateTime.Now;
                                            persisted.SettlementRemarks = withdrawalNotificationDTO.SettlementRemarks;
                                            persisted.SettlementType = (byte)withdrawalNotificationDTO.SettlementType;
                                            persisted.Lock();

                                            var persistedCustomer_Voluntary = _customerRepository.Get(persisted.CustomerId, serviceHeader);
                                            persistedCustomer_Voluntary.RecordStatus = (int)RecordStatus.Edited;
                                            persistedCustomer_Voluntary.Remarks = persisted.Remarks;
                                            persistedCustomer_Voluntary.Lock();

                                            break;
                                        case WithdrawalNotificationCategory.Retiree:

                                            var retireeLoansTotal = 0m;
                                            var retireeInvestmentsTotal = 0m;

                                            #region Loan Accounts

                                            if (customerLoanAccounts != null && customerLoanAccounts.Any())
                                            {
                                                foreach (var customerLoanAccount in customerLoanAccounts)
                                                {
                                                    secondaryDescription = customerLoanAccount.CustomerAccountTypeTargetProductDescription;

                                                    if (customerLoanAccount.PrincipalBalance * -1 > 0m)
                                                    {
                                                        // credit loan product, debit commont ctrl
                                                        var retireeLoanPrincipalJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.PrincipalBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(retireeLoanPrincipalJournal, customerLoanAccount.CustomerAccountTypeTargetProductChartOfAccountId, payablesControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(retireeLoanPrincipalJournal);

                                                        // tally
                                                        retireeLoansTotal += customerLoanAccount.PrincipalBalance * -1;
                                                    }

                                                    if (customerLoanAccount.InterestBalance * -1 > 0m)
                                                    {
                                                        // credit interest receivable, debit common ctrl
                                                        var retireeLoanInterestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.InterestBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(retireeLoanInterestReceivableJournal, customerLoanAccount.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, payablesControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(retireeLoanInterestReceivableJournal);

                                                        // credit interest received, debit interest charged
                                                        var retireeLoanInterestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerLoanAccount.InterestBalance * -1, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(retireeLoanInterestReceivedJournal, customerLoanAccount.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, customerLoanAccount.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                        journals.Add(retireeLoanInterestReceivedJournal);

                                                        // tally
                                                        retireeLoansTotal += customerLoanAccount.InterestBalance * -1;
                                                    }

                                                    if (customerLoanAccount.CarryForwardsBalance * -1 > 0m)
                                                    {
                                                        var carryForwards = _customerAccountAppService.FindCustomerAccountCarryForwardsByBeneficiaryCustomerAccountId(customerLoanAccount.Id, serviceHeader);

                                                        if (carryForwards != null && carryForwards.Any())
                                                        {
                                                            var grouping = from p in carryForwards
                                                                           group p.Amount by new
                                                                           {
                                                                               p.BeneficiaryCustomerAccountId,
                                                                               p.BeneficiaryChartOfAccountId
                                                                           } into g
                                                                           select new
                                                                           {
                                                                               BeneficiaryCustomerAccountId = g.Key.BeneficiaryCustomerAccountId,
                                                                               BeneficiaryChartOfAccountId = g.Key.BeneficiaryChartOfAccountId,
                                                                               Payments = g.ToList()
                                                                           };

                                                            foreach (var item in grouping)
                                                            {
                                                                var totalPayments = item.Payments.Sum();

                                                                if (totalPayments * -1 < 0m)
                                                                {
                                                                    var carryForwardArrears = totalPayments;

                                                                    if (carryForwardArrears > 0m)
                                                                    {
                                                                        // Credit CarryForward.BeneficiaryChartOfAccountId, debit payables ctrl
                                                                        var carryFowardBeneficiaryJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, carryForwardArrears, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                                        _journalEntryPostingService.PerformDoubleEntry(carryFowardBeneficiaryJournal, item.BeneficiaryChartOfAccountId, payablesControlChartOfAccountId, customerLoanAccount, customerLoanAccount, serviceHeader);
                                                                        journals.Add(carryFowardBeneficiaryJournal);
                                                                    }
                                                                }
                                                            }

                                                            foreach (var item in carryForwards)
                                                            {
                                                                // Do we need to update carry forward history?
                                                                var customerAccountCarryForward = CustomerAccountCarryForwardFactory.CreateCustomerAccountCarryForward(item.BenefactorCustomerAccountId, item.BeneficiaryCustomerAccountId, item.BeneficiaryChartOfAccountId, item.Amount * -1/*-ve cos is payment*/, primaryDescription);
                                                                customerAccountCarryForward.CreatedBy = serviceHeader.ApplicationUserName;
                                                                customerAccountCarryForwards.Add(customerAccountCarryForward);

                                                                // tally
                                                                retireeLoansTotal += item.Amount;
                                                            }
                                                        }
                                                    }

                                                    // freeze
                                                    _customerAccountAppService.ManageCustomerAccount(customerLoanAccount.Id, (int)CustomerAccountManagementAction.Deactivation, reference, (int)CustomerAccountRemarkType.Actionable, serviceHeader);

                                                    // log settlement
                                                    var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerLoanAccount.Id, customerLoanAccount.PrincipalBalance, customerLoanAccount.InterestBalance, customerLoanAccount.CarryForwardsBalance, primaryDescription);
                                                    withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                    _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            #region Investment Accounts

                                            if (customerInvestmentAccounts != null && customerInvestmentAccounts.Any())
                                            {
                                                foreach (var customerInvestmentAccount in customerInvestmentAccounts)
                                                {
                                                    secondaryDescription = customerInvestmentAccount.CustomerAccountTypeTargetProductDescription;

                                                    if (customerInvestmentAccount.CustomerAccountTypeTargetProductIsRefundable && customerInvestmentAccount.BookBalance > 0m)
                                                    {
                                                        // credit common ctrl, debit investment 
                                                        var retireeRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                        _journalEntryPostingService.PerformDoubleEntry(retireeRefundableInvestmentJournal, payablesControlChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, customerInvestmentAccount, customerInvestmentAccount, serviceHeader);
                                                        journals.Add(retireeRefundableInvestmentJournal);

                                                        // tally
                                                        retireeInvestmentsTotal += customerInvestmentAccount.BookBalance;

                                                        // log settlement
                                                        var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerInvestmentAccount.Id, customerInvestmentAccount.BookBalance, 0m, 0m, primaryDescription);
                                                        withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                        _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                    }
                                                    else if (!customerInvestmentAccount.CustomerAccountTypeTargetProductIsRefundable && customerInvestmentAccount.BookBalance > 0m && customerInvestmentAccount.CustomerAccountTypeTargetProductTransferBalanceToParentOnMembershipTermination)
                                                    {
                                                        if (customerInvestmentAccount.CustomerAccountTypeTargetProductParentId != null && customerInvestmentAccount.CustomerAccountTypeTargetProductParentId != Guid.Empty)
                                                        {
                                                            var parentCustomerInvestmentAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, customerInvestmentAccount.CustomerAccountTypeTargetProductParentId ?? Guid.Empty, serviceHeader);

                                                            if (parentCustomerInvestmentAccounts != null && parentCustomerInvestmentAccounts.Any())
                                                            {
                                                                _customerAccountAppService.FetchCustomerAccountsProductDescription(parentCustomerInvestmentAccounts, serviceHeader, true);

                                                                var parentCustomerInvestmentAccountDTO = parentCustomerInvestmentAccounts.FirstOrDefault();

                                                                if (parentCustomerInvestmentAccountDTO != null)
                                                                {
                                                                    // credit parent investment, debit investment 
                                                                    var retireeNonRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                                    _journalEntryPostingService.PerformDoubleEntry(retireeNonRefundableInvestmentJournal, parentCustomerInvestmentAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, parentCustomerInvestmentAccountDTO, customerInvestmentAccount, serviceHeader);
                                                                    journals.Add(retireeNonRefundableInvestmentJournal);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                // create account and post tx
                                                                var parentInvestmentProduct = _investmentProductAppService.FindInvestmentProduct(customerInvestmentAccount.CustomerAccountTypeTargetProductParentId ?? Guid.Empty, serviceHeader);

                                                                if (parentInvestmentProduct != null)
                                                                {
                                                                    var parentCustomerInvestmentAccountDTO = new CustomerAccountDTO
                                                                    {
                                                                        BranchId = persisted.BranchId,
                                                                        CustomerId = persisted.CustomerId,
                                                                        CustomerAccountTypeProductCode = (int)ProductCode.Investment,
                                                                        CustomerAccountTypeTargetProductId = parentInvestmentProduct.Id,
                                                                        CustomerAccountTypeTargetProductCode = parentInvestmentProduct.Code,
                                                                        Status = (int)CustomerAccountStatus.Normal,
                                                                    };

                                                                    parentCustomerInvestmentAccountDTO = _customerAccountAppService.AddNewCustomerAccount(parentCustomerInvestmentAccountDTO, serviceHeader);

                                                                    if (parentCustomerInvestmentAccountDTO != null)
                                                                    {
                                                                        // credit parent investment, debit investment 
                                                                        var retireeNonRefundableInvestmentJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, customerInvestmentAccount.BookBalance, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                                                        _journalEntryPostingService.PerformDoubleEntry(retireeNonRefundableInvestmentJournal, parentInvestmentProduct.ChartOfAccountId, customerInvestmentAccount.CustomerAccountTypeTargetProductChartOfAccountId, parentCustomerInvestmentAccountDTO, customerInvestmentAccount, serviceHeader);
                                                                        journals.Add(retireeNonRefundableInvestmentJournal);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    // freeze
                                                    _customerAccountAppService.ManageCustomerAccount(customerInvestmentAccount.Id, (int)CustomerAccountManagementAction.Deactivation, reference, (int)CustomerAccountRemarkType.Actionable, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            #region Savings Account

                                            decimal retireeNetRefundable = retireeInvestmentsTotal - retireeLoansTotal;

                                            if (retireeNetRefundable > 0m)
                                            {
                                                CustomerAccountDTO customerSavingsAccountDTO = null;

                                                var customerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persisted.CustomerId, defaultSavingsProduct.Id, serviceHeader);

                                                if (customerSavingsAccounts != null && customerSavingsAccounts.Any())
                                                    customerSavingsAccountDTO = customerSavingsAccounts.First();
                                                else
                                                {
                                                    var customerAccountDTO = new CustomerAccountDTO
                                                    {
                                                        BranchId = persisted.BranchId,
                                                        CustomerId = persisted.CustomerId,
                                                        CustomerAccountTypeProductCode = (int)ProductCode.Savings,
                                                        CustomerAccountTypeTargetProductId = defaultSavingsProduct.Id,
                                                        CustomerAccountTypeTargetProductCode = defaultSavingsProduct.Code,
                                                        Status = (int)CustomerAccountStatus.Normal,
                                                    };

                                                    customerAccountDTO = _customerAccountAppService.AddNewCustomerAccount(customerAccountDTO, serviceHeader);

                                                    if (customerAccountDTO != null)
                                                        customerSavingsAccountDTO = customerAccountDTO;
                                                }

                                                if (customerSavingsAccountDTO != null)
                                                {
                                                    secondaryDescription = "Net Refund";

                                                    var tariffs = new List<TariffWrapper>();

                                                    #region loan clearance tariffs

                                                    if (customerLoanAccounts != null && customerLoanAccounts.Any())
                                                    {
                                                        foreach (var customerLoanAccount in customerLoanAccounts)
                                                        {
                                                            if (customerLoanAccount.CustomerAccountTypeTargetProductChargeClearanceFee)
                                                            {
                                                                var retireeActualPrincipalBalance = 0m;
                                                                if (customerLoanAccount.PrincipalBalance * -1 > 0m)
                                                                    retireeActualPrincipalBalance = customerLoanAccount.PrincipalBalance * -1;

                                                                var retireeActualInterestBalance = 0m;
                                                                if (customerLoanAccount.InterestBalance * -1 > 0m)
                                                                    retireeActualInterestBalance = customerLoanAccount.InterestBalance * -1;

                                                                var retireeClearanceTariffs = _commissionAppService.ComputeTariffsByLoanProduct(customerLoanAccount.CustomerAccountTypeTargetProductId, (int)LoanProductKnownChargeType.LoanClearanceCharges, (retireeActualPrincipalBalance + retireeActualInterestBalance), retireeActualPrincipalBalance, customerSavingsAccountDTO, serviceHeader);

                                                                if (retireeClearanceTariffs != null && retireeClearanceTariffs.Any())
                                                                    tariffs.AddRange(retireeClearanceTariffs);
                                                            }
                                                        }
                                                    }

                                                    #endregion

                                                    #region membership termination tariffs

                                                    var retireeMembershipTerminationProcessingFeeTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.RetireeMembershipTerminationProcessingFee, retireeNetRefundable, customerSavingsAccountDTO, serviceHeader);
                                                    if (retireeMembershipTerminationProcessingFeeTariffs.Any())
                                                        tariffs.AddRange(retireeMembershipTerminationProcessingFeeTariffs);

                                                    switch ((MembershipWithdrawalSettlementType)withdrawalNotificationDTO.SettlementType)
                                                    {
                                                        case MembershipWithdrawalSettlementType.Normal:

                                                            var normalTerminationTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.NormalMembershipTerminationCharges, retireeNetRefundable, customerSavingsAccountDTO, serviceHeader);
                                                            if (normalTerminationTariffs.Any())
                                                                tariffs.AddRange(normalTerminationTariffs);

                                                            break;
                                                        case MembershipWithdrawalSettlementType.Express:

                                                            var expressTerminationTariffs = _commissionAppService.ComputeTariffsBySystemTransactionType((int)SystemTransactionType.PrematureMembershipTerminationCharges, retireeNetRefundable, customerSavingsAccountDTO, serviceHeader);
                                                            if (expressTerminationTariffs.Any())
                                                                tariffs.AddRange(expressTerminationTariffs);

                                                            break;
                                                        case MembershipWithdrawalSettlementType.Waiver:
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                    #endregion

                                                    // credit savings, debit common ctrol 
                                                    var retireeSavingsJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persisted.BranchId, null, retireeNetRefundable, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                    _journalEntryPostingService.PerformDoubleEntry(retireeSavingsJournal, defaultSavingsProduct.ChartOfAccountId, payablesControlChartOfAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                                    journals.Add(retireeSavingsJournal);

                                                    tariffs.ForEach(tariff =>
                                                    {
                                                        var retireeTariffJournal = JournalFactory.CreateJournal(retireeSavingsJournal.Id, postingPeriodDTO.Id, persisted.BranchId, null, tariff.Amount, tariff.Description, secondaryDescription, reference, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                        _journalEntryPostingService.PerformDoubleEntry(retireeTariffJournal, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerSavingsAccountDTO, customerSavingsAccountDTO, serviceHeader);
                                                        journals.Add(retireeTariffJournal);
                                                    });

                                                    // log settlement
                                                    var withdrawalSettlement = WithdrawalSettlementFactory.CreateWithdrawalSettlement(persisted.Id, customerSavingsAccountDTO.Id, retireeNetRefundable, 0m, 0m, primaryDescription);
                                                    withdrawalSettlement.CreatedBy = serviceHeader.ApplicationUserName;
                                                    _withdrawalSettlementRepository.Add(withdrawalSettlement, serviceHeader);
                                                }
                                            }

                                            #endregion

                                            persisted.Status = (int)WithdrawalNotificationStatus.WithdrawalSettled;
                                            persisted.SettledBy = serviceHeader.ApplicationUserName;
                                            persisted.SettledDate = DateTime.Now;
                                            persisted.SettlementRemarks = withdrawalNotificationDTO.SettlementRemarks;
                                            persisted.SettlementType = (byte)withdrawalNotificationDTO.SettlementType;
                                            persisted.Lock();

                                            var persistedCustomer_Retiree = _customerRepository.Get(persisted.CustomerId, serviceHeader);
                                            persistedCustomer_Retiree.RecordStatus = (int)RecordStatus.Edited;
                                            persistedCustomer_Retiree.Remarks = persisted.Remarks;
                                            persistedCustomer_Retiree.Lock();

                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else withdrawalNotificationDTO.ErrorMessageResult = ("Sorry, but requisite minimums have not been setup viz. default savings product / control accounts / posting period!");

                                break;
                            case MembershipWithdrawalSettlementOption.Defer:
                                persisted.Status = (int)WithdrawalNotificationStatus.Deferred;
                                break;
                            default:
                                break;
                        }

                        settlementOK = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }

                if (settlementOK)
                {
                    settlementOK = _journalEntryPostingService.BulkSave(serviceHeader, journals, customerAccountCarryForwards);
                }
            }

            return settlementOK;
        }

        public bool ProcessDeathSettlements(WithdrawalNotificationDTO withdrawalNotificationDTO, List<WithdrawalSettlementDTO> withdrawalSettlementDTOs, InsuranceCompanyDTO insuranceCompanyDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            var settlementOK = default(bool);

            var defaultSavingsProduct = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

            var deceasedControlChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.DeceasedControl, serviceHeader);

            var postingPeriodDTO = _postingPeriodAppService.FindCachedCurrentPostingPeriod(serviceHeader);

            if (deceasedControlChartOfAccountId != Guid.Empty && defaultSavingsProduct != null && withdrawalNotificationDTO != null && withdrawalSettlementDTOs != null && withdrawalSettlementDTOs.Any() && insuranceCompanyDTO != null && postingPeriodDTO != null)
            {
                var journals = new List<Journal>();

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persistedNotification = _withdrawalNotificationRepository.Get(withdrawalNotificationDTO.Id, serviceHeader);

                    if (persistedNotification != null && persistedNotification.Category == (int)WithdrawalNotificationCategory.Deceased && persistedNotification.Status == (int)WithdrawalNotificationStatus.WithdrawalSettled)
                    {
                        var primaryDescription = "Death Claim Settlememt";

                        withdrawalSettlementDTOs.ForEach(withdrawalSettlementDTO =>
                        {
                            var persistedSettlement = _withdrawalSettlementRepository.Get(withdrawalSettlementDTO.Id, serviceHeader);

                            if (persistedSettlement != null && persistedSettlement.WithdrawalNotificationId == persistedNotification.Id && persistedSettlement.CustomerAccount != null)
                            {
                                var settlementCustomerAccountDTO = persistedSettlement.CustomerAccount.ProjectedAs<CustomerAccountDTO>();

                                switch ((ProductCode)settlementCustomerAccountDTO.CustomerAccountTypeProductCode)
                                {
                                    case ProductCode.Loan:

                                        // credit deceased ctrl, debit insurance company
                                        var loanSettlementJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persistedNotification.BranchId, null, persistedSettlement.Principal < 0m ? persistedSettlement.Principal * -1 : persistedSettlement.Principal, primaryDescription, persistedSettlement.Reference, insuranceCompanyDTO.Description, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                        _journalEntryPostingService.PerformDoubleEntry(loanSettlementJournal, deceasedControlChartOfAccountId, insuranceCompanyDTO.ChartOfAccountId, settlementCustomerAccountDTO, settlementCustomerAccountDTO, serviceHeader);
                                        journals.Add(loanSettlementJournal);

                                        break;
                                    case ProductCode.Investment:

                                        if (withdrawalNotificationDTO.BranchCompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement)
                                        {
                                            CustomerAccountDTO defaultCustomerSavingsAccountDTO = null;

                                            var defaultCustomerSavingsAccounts = _customerAccountAppService.FindCustomerAccountDTOsByCustomerIdAndCustomerAccountTypeTargetProductId(persistedNotification.CustomerId, defaultSavingsProduct.Id, serviceHeader);

                                            if (defaultCustomerSavingsAccounts != null && defaultCustomerSavingsAccounts.Any())
                                                defaultCustomerSavingsAccountDTO = defaultCustomerSavingsAccounts.First();

                                            if (defaultCustomerSavingsAccountDTO != null)
                                            {
                                                var savingsSettlementJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persistedNotification.BranchId, null, persistedSettlement.Principal, primaryDescription, persistedSettlement.Reference, insuranceCompanyDTO.Description, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader);
                                                _journalEntryPostingService.PerformDoubleEntry(savingsSettlementJournal, defaultSavingsProduct.ChartOfAccountId, insuranceCompanyDTO.ChartOfAccountId, defaultCustomerSavingsAccountDTO, defaultCustomerSavingsAccountDTO, serviceHeader);
                                                journals.Add(savingsSettlementJournal);
                                            }
                                            else withdrawalNotificationDTO.ErrorMessageResult = ("Sorry, but a savings account for the customer could not be identified!");
                                        }
                                        else
                                        {
                                            // credit deceased ctrl, debit insurance company
                                            var investmentSettlementJournal = JournalFactory.CreateJournal(null, postingPeriodDTO.Id, persistedNotification.BranchId, null, persistedSettlement.Principal, primaryDescription, persistedSettlement.Reference, insuranceCompanyDTO.Description, moduleNavigationItemCode, (int)SystemTransactionCode.MembershipTermination, null, serviceHeader, true);
                                            _journalEntryPostingService.PerformDoubleEntry(investmentSettlementJournal, deceasedControlChartOfAccountId, insuranceCompanyDTO.ChartOfAccountId, settlementCustomerAccountDTO, settlementCustomerAccountDTO, serviceHeader);
                                            journals.Add(investmentSettlementJournal);
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                        });

                        persistedNotification.Status = (int)WithdrawalNotificationStatus.DeathClaimSettled;

                        settlementOK = dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                }

                if (settlementOK)
                {
                    settlementOK = _journalEntryPostingService.BulkSave(serviceHeader, journals);
                }
            }

            return settlementOK;
        }

        public List<WithdrawalNotificationDTO> FindWithdrawalNotifications(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<WithdrawalNotification> spec = WithdrawalNotificationSpecifications.DefaultSpec();

                var withdrawalNotifications = _withdrawalNotificationRepository.AllMatching(spec, serviceHeader);

                if (withdrawalNotifications != null && withdrawalNotifications.Any())
                {
                    return withdrawalNotifications.ProjectedAsCollection<WithdrawalNotificationDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotifications(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WithdrawalNotificationSpecifications.DefaultSpec();

                ISpecification<WithdrawalNotification> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var withdrawalNotificationPagedCollection = _withdrawalNotificationRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (withdrawalNotificationPagedCollection != null)
                {
                    var pageCollection = withdrawalNotificationPagedCollection.PageCollection.ProjectedAsCollection<WithdrawalNotificationDTO>();

                    var itemsCount = withdrawalNotificationPagedCollection.ItemsCount;

                    return new PageCollectionInfo<WithdrawalNotificationDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotifications(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WithdrawalNotificationSpecifications.WithdrawalNotificationFullText(text, customerFilter);

                ISpecification<WithdrawalNotification> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var withdrawalNotificationPagedCollection = _withdrawalNotificationRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (withdrawalNotificationPagedCollection != null)
                {
                    var pageCollection = withdrawalNotificationPagedCollection.PageCollection.ProjectedAsCollection<WithdrawalNotificationDTO>();

                    var itemsCount = withdrawalNotificationPagedCollection.ItemsCount;

                    return new PageCollectionInfo<WithdrawalNotificationDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<WithdrawalNotificationDTO> FindWithdrawalNotifications(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = WithdrawalNotificationSpecifications.WithdrawalNotificationWithDateRangeAndFullText(startDate, endDate, status, text, customerFilter);

                ISpecification<WithdrawalNotification> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var withdrawalNotificationPagedCollection = _withdrawalNotificationRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (withdrawalNotificationPagedCollection != null)
                {
                    var pageCollection = withdrawalNotificationPagedCollection.PageCollection.ProjectedAsCollection<WithdrawalNotificationDTO>();

                    var itemsCount = withdrawalNotificationPagedCollection.ItemsCount;

                    return new PageCollectionInfo<WithdrawalNotificationDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public WithdrawalNotificationDTO FindWithdrawalNotification(Guid withdrawalNotificationId, ServiceHeader serviceHeader)
        {
            if (withdrawalNotificationId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var withdrawalNotification = _withdrawalNotificationRepository.Get(withdrawalNotificationId, serviceHeader);

                    if (withdrawalNotification != null)
                    {
                        return withdrawalNotification.ProjectedAs<WithdrawalNotificationDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<WithdrawalNotificationDTO> FindWithdrawalNotificationsByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WithdrawalNotificationSpecifications.WithdrawalNotificationWithCustomerId(customerId);

                    ISpecification<WithdrawalNotification> spec = filter;

                    var withdrawalNotifications = _withdrawalNotificationRepository.AllMatching(spec, serviceHeader);

                    if (withdrawalNotifications != null && withdrawalNotifications.Any())
                    {
                        return withdrawalNotifications.ProjectedAsCollection<WithdrawalNotificationDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<WithdrawalSettlementDTO> FindWithdrawalSettlementsByWithdrawalNotificationId(Guid withdrawalNotificationId, ServiceHeader serviceHeader)
        {
            if (withdrawalNotificationId != null && withdrawalNotificationId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = WithdrawalSettlementSpecifications.WithdrawalSettlementByWithdrawalNotificationId(withdrawalNotificationId);

                    ISpecification<WithdrawalSettlement> spec = filter;

                    var withdrawalSettlements = _withdrawalSettlementRepository.AllMatching(spec, serviceHeader);

                    if (withdrawalSettlements != null && withdrawalSettlements.Any())
                    {
                        return withdrawalSettlements.ProjectedAsCollection<WithdrawalSettlementDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}