using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.MobileToBankRequestAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class MobileToBankRequestAppService : IMobileToBankRequestAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<MobileToBankRequest> _mobileToBankRequestRepository;
        private readonly IPostingPeriodAppService _postingPeriodAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly ILoanProductAppService _loanProductAppService;
        private readonly IInvestmentProductAppService _investmentProductAppService;
        private readonly IJournalEntryPostingService _journalEntryPostingService;
        private readonly ICustomerAccountAppService _customerAccountAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IAlternateChannelAppService _alternateChannelAppService;
        private readonly IBrokerService _brokerService;

        public MobileToBankRequestAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<MobileToBankRequest> mobileToBankRequestRepository,
           IPostingPeriodAppService postingPeriodAppService,
           ISavingsProductAppService savingsProductAppService,
           ILoanProductAppService loanProductAppService,
           IInvestmentProductAppService investmentProductAppService,
           IJournalEntryPostingService journalEntryPostingService,
           ICustomerAccountAppService customerAccountAppService,
           ISqlCommandAppService sqlCommandAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           IAlternateChannelAppService alternateChannelAppService,
           IBrokerService brokerService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (mobileToBankRequestRepository == null)
                throw new ArgumentNullException(nameof(mobileToBankRequestRepository));

            if (postingPeriodAppService == null)
                throw new ArgumentNullException(nameof(postingPeriodAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (loanProductAppService == null)
                throw new ArgumentNullException(nameof(loanProductAppService));

            if (investmentProductAppService == null)
                throw new ArgumentNullException(nameof(investmentProductAppService));

            if (journalEntryPostingService == null)
                throw new ArgumentNullException(nameof(journalEntryPostingService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (alternateChannelAppService == null)
                throw new ArgumentNullException(nameof(alternateChannelAppService));

            if (brokerService == null)
                throw new ArgumentNullException(nameof(brokerService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _mobileToBankRequestRepository = mobileToBankRequestRepository;
            _postingPeriodAppService = postingPeriodAppService;
            _savingsProductAppService = savingsProductAppService;
            _loanProductAppService = loanProductAppService;
            _investmentProductAppService = investmentProductAppService;
            _journalEntryPostingService = journalEntryPostingService;
            _customerAccountAppService = customerAccountAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _alternateChannelAppService = alternateChannelAppService;
            _brokerService = brokerService;
        }

        public MobileToBankRequestDTO AddNewMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO, ServiceHeader serviceHeader)
        {
            if (mobileToBankRequestDTO != null)
            {
                var result = default(bool);

                var customerAccountDTO = MatchCustomerAccount(mobileToBankRequestDTO, serviceHeader);

                var settlementAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.MobileWalletC2BSettlement, serviceHeader);

                var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    if (customerAccountDTO != null)
                        mobileToBankRequestDTO.CustomerAccountId = customerAccountDTO.Id;

                    var mobileToBankRequest = MobileToBankRequestFactory.CreateMobileToBankRequest(mobileToBankRequestDTO.CustomerAccountId, mobileToBankRequestDTO.ChartOfAccountId, mobileToBankRequestDTO.SystemTraceAuditNumber, mobileToBankRequestDTO.MSISDN, mobileToBankRequestDTO.BusinessShortCode, mobileToBankRequestDTO.InvoiceNumber, mobileToBankRequestDTO.TransID, mobileToBankRequestDTO.TransAmount, mobileToBankRequestDTO.ThirdPartyTransID, mobileToBankRequestDTO.TransTime, mobileToBankRequestDTO.BillRefNumber, mobileToBankRequestDTO.OrgAccountBalance, mobileToBankRequestDTO.KYCInfo);

                    mobileToBankRequest.Status = (customerAccountDTO != null && settlementAccountId != Guid.Empty && postingPeriod != null)
                        ? (byte)MobileToBankRequestStatus.AutoMatched
                        : (byte)MobileToBankRequestStatus.Unmatched;

                    _mobileToBankRequestRepository.Add(mobileToBankRequest, serviceHeader);

                    result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                    mobileToBankRequestDTO = mobileToBankRequest.ProjectedAs<MobileToBankRequestDTO>();
                }

                if (result && mobileToBankRequestDTO != null && customerAccountDTO != null && settlementAccountId != Guid.Empty && postingPeriod != null)
                {
                    var journals = new List<Journal>();

                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true);

                    switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                    {
                        case ProductCode.Savings:
                        case ProductCode.Investment:

                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, Debit SystemGeneralLedgerAccountCode.VanguardWalletC2BSettlement
                            var c2BJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, customerAccountDTO.BranchId, null, mobileToBankRequestDTO.TransAmount, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank), string.Format("{0}~{1}", mobileToBankRequestDTO.TransID, mobileToBankRequestDTO.BillRefNumber), string.Format("{0}~{1}", mobileToBankRequestDTO.MSISDN, mobileToBankRequestDTO.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(c2BJournal, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, settlementAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            journals.Add(c2BJournal);

                            break;
                        case ProductCode.Loan:

                            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true);

                            // how much is available for recovery?
                            var availableRecoveryAmount = mobileToBankRequestDTO.TransAmount;

                            // reset expected interest & expected principal >> NB: interest has priority over principal!
                            var actualLoanInterest = Math.Min((customerAccountDTO.InterestBalance * -1 > 0m ? customerAccountDTO.InterestBalance * -1 : 0m), availableRecoveryAmount);
                            var actualLoanPrincipal = availableRecoveryAmount - actualLoanInterest;

                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, Debit SystemGeneralLedgerAccountCode.VanguardWalletC2BSettlement
                            var attachedLoanInterestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, customerAccountDTO.BranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}~{1}", customerAccountDTO.CustomerAccountTypeTargetProductDescription, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank)), string.Format("{0}~{1}", mobileToBankRequestDTO.TransID, mobileToBankRequestDTO.BillRefNumber), string.Format("{0}~{1}", mobileToBankRequestDTO.MSISDN, mobileToBankRequestDTO.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivableJournal, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, settlementAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanInterestReceivableJournal);

                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, Debit CustomerAccountDTO.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId
                            var attachedLoanInterestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, customerAccountDTO.BranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}~{1}", customerAccountDTO.CustomerAccountTypeTargetProductDescription, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank)), string.Format("{0}~{1}", mobileToBankRequestDTO.TransID, mobileToBankRequestDTO.BillRefNumber), string.Format("{0}~{1}", mobileToBankRequestDTO.MSISDN, mobileToBankRequestDTO.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivedJournal, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, customerAccountDTO.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanInterestReceivedJournal);

                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, Debit SystemGeneralLedgerAccountCode.VanguardWalletC2BSettlement
                            var attachedLoanPrincipalJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, customerAccountDTO.BranchId, null, actualLoanPrincipal, string.Format("Principal Paid~{0}~{1}", customerAccountDTO.CustomerAccountTypeTargetProductDescription, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank)), string.Format("{0}~{1}", mobileToBankRequestDTO.TransID, mobileToBankRequestDTO.BillRefNumber), string.Format("{0}~{1}", mobileToBankRequestDTO.MSISDN, mobileToBankRequestDTO.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanPrincipalJournal, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, settlementAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                            journals.Add(attachedLoanPrincipalJournal);

                            break;
                        default:
                            break;
                    }

                    if (journals.Any())
                    {
                        result = _journalEntryPostingService.BulkSave(serviceHeader, journals);

                        if (result)
                        {
                            #region Do we need to send alerts?

                            _brokerService.ProcessMobileToBankSenderAcknowledgementAccountAlerts(DMLCommand.None, serviceHeader, mobileToBankRequestDTO);

                            #endregion
                        }
                    }

                    return mobileToBankRequestDTO;
                }
                else return null;
            }
            else return null;
        }

        public bool ReconcileMobileToBankRequest(MobileToBankRequestDTO mobileToBankRequestDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            Guid? customerAccountId = Guid.Empty;

            if (mobileToBankRequestDTO == null)
                return result;

            if (mobileToBankRequestDTO.ReconciliationType == (int)ApportionTo.CustomerAccount)
            {
                var customerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(mobileToBankRequestDTO.CustomerAccountId ?? Guid.Empty, serviceHeader);

                if (customerAccountDTO != null)
                    customerAccountId = customerAccountDTO.Id;
                else
                    return result;
            }

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _mobileToBankRequestRepository.Get(mobileToBankRequestDTO.Id, serviceHeader);

                if (persisted == null || persisted.Status != (int)MobileToBankRequestStatus.Unmatched || persisted.RecordStatus != (int)MobileToBankRequestRecordStatus.Pending)
                    return result;

                persisted.CustomerAccountId = (customerAccountId != null && customerAccountId != Guid.Empty) ? customerAccountId : null;
                persisted.ChartOfAccountId = (mobileToBankRequestDTO.ChartOfAccountId != null && mobileToBankRequestDTO.ChartOfAccountId != Guid.Empty) ? mobileToBankRequestDTO.ChartOfAccountId : null;
                persisted.ReconciliationType = (byte)mobileToBankRequestDTO.ReconciliationType;
                persisted.Status = (byte)MobileToBankRequestStatus.ReconMatched;
                persisted.RecordStatus = (byte)MobileToBankRequestRecordStatus.Pending;
                persisted.ModifiedBy = serviceHeader.ApplicationUserName;
                persisted.ModifiedDate = DateTime.Now;

                return dbContextScope.SaveChanges(serviceHeader) >= 0;
            }
        }

        public bool AuditMobileToBankRequestReconciliation(MobileToBankRequestDTO mobileToBankRequestDTO, int requestAuthOption, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            var journals = new List<Journal>();

            CustomerAccountDTO customerAccountDTO = null;

            if (mobileToBankRequestDTO.ReconciliationType == (int)ApportionTo.CustomerAccount)
            {
                customerAccountDTO = _sqlCommandAppService.FindCustomerAccountById(mobileToBankRequestDTO.CustomerAccountId ?? Guid.Empty, serviceHeader);

                if (customerAccountDTO != null)
                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true);
                else
                    return result;
            }

            var postingPeriod = _postingPeriodAppService.FindCurrentPostingPeriod(serviceHeader);
            if (postingPeriod == null)
                return result;

            var settlementAccountId = _chartOfAccountAppService.GetCachedChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.MobileWalletC2BSettlement, serviceHeader);
            if (settlementAccountId == Guid.Empty)
                return result;

            if (mobileToBankRequestDTO == null || !Enum.IsDefined(typeof(MobileToBankRequestAuthOption), requestAuthOption))
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _mobileToBankRequestRepository.Get(mobileToBankRequestDTO.Id, serviceHeader);

                if (persisted == null || persisted.RecordStatus != (int)MobileToBankRequestRecordStatus.Pending || persisted.Status != (int)MobileToBankRequestStatus.ReconMatched)
                    return false;

                switch ((MobileToBankRequestAuthOption)requestAuthOption)
                {
                    case MobileToBankRequestAuthOption.Verify:

                        persisted.RecordStatus = (int)MobileToBankRequestRecordStatus.Verified;
                        persisted.AuditRemarks = mobileToBankRequestDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                        if (result)
                        {
                            switch ((ApportionTo)mobileToBankRequestDTO.ReconciliationType)
                            {
                                case ApportionTo.CustomerAccount:

                                    _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true);

                                    switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                                    {
                                        case ProductCode.Savings:
                                        case ProductCode.Investment:

                                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, Debit SystemGeneralLedgerAccountCode.VanguardWalletC2BSettlement
                                            var c2BJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, mobileToBankRequestDTO.BranchId, null, persisted.TransAmount, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank), string.Format("{0}~{1}", persisted.TransID, persisted.BillRefNumber), string.Format("{0}~{1}", persisted.MSISDN, persisted.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(c2BJournal, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, settlementAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                            journals.Add(c2BJournal);

                                            break;
                                        case ProductCode.Loan:

                                            _customerAccountAppService.FetchCustomerAccountBalances(new List<CustomerAccountDTO> { customerAccountDTO }, serviceHeader, true);

                                            // how much is available for recovery?
                                            var availableRecoveryAmount = persisted.TransAmount;

                                            // reset expected interest & expected principal >> NB: interest has priority over principal!
                                            var actualLoanInterest = Math.Min((customerAccountDTO.InterestBalance * -1 > 0m ? customerAccountDTO.InterestBalance * -1 : 0m), availableRecoveryAmount);
                                            var actualLoanPrincipal = availableRecoveryAmount - actualLoanInterest;

                                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, Debit SystemGeneralLedgerAccountCode.VanguardWalletC2BSettlement
                                            var attachedLoanInterestReceivableJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, mobileToBankRequestDTO.BranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}~{1}", customerAccountDTO.CustomerAccountTypeTargetProductDescription, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank)), string.Format("{0}~{1}", persisted.TransID, persisted.BillRefNumber), string.Format("{0}~{1}", persisted.MSISDN, persisted.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivableJournal, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivableChartOfAccountId, settlementAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                            journals.Add(attachedLoanInterestReceivableJournal);

                                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, Debit CustomerAccountDTO.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId
                                            var attachedLoanInterestReceivedJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, mobileToBankRequestDTO.BranchId, null, actualLoanInterest, string.Format("Interest Paid~{0}~{1}", customerAccountDTO.CustomerAccountTypeTargetProductDescription, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank)), string.Format("{0}~{1}", mobileToBankRequestDTO.TransID, mobileToBankRequestDTO.BillRefNumber), string.Format("{0}~{1}", mobileToBankRequestDTO.MSISDN, mobileToBankRequestDTO.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanInterestReceivedJournal, customerAccountDTO.CustomerAccountTypeTargetProductInterestReceivedChartOfAccountId, customerAccountDTO.CustomerAccountTypeTargetProductInterestChargedChartOfAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                            journals.Add(attachedLoanInterestReceivedJournal);

                                            // C2B Journal: Credit CustomerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, Debit SystemGeneralLedgerAccountCode.VanguardWalletC2BSettlement
                                            var attachedLoanPrincipalJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, mobileToBankRequestDTO.BranchId, null, actualLoanPrincipal, string.Format("Principal Paid~{0}~{1}", customerAccountDTO.CustomerAccountTypeTargetProductDescription, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank)), string.Format("{0}~{1}", persisted.TransID, persisted.BillRefNumber), string.Format("{0}~{1}", persisted.MSISDN, persisted.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                                            _journalEntryPostingService.PerformDoubleEntry(attachedLoanPrincipalJournal, customerAccountDTO.CustomerAccountTypeTargetProductChartOfAccountId, settlementAccountId, customerAccountDTO, customerAccountDTO, serviceHeader);
                                            journals.Add(attachedLoanPrincipalJournal);

                                            break;
                                        default:
                                            break;
                                    }

                                    break;
                                case ApportionTo.GeneralLedgerAccount:

                                    // C2B Journal: Credit GL Account, Debit SystemGeneralLedgerAccountCode.VanguardWalletC2BSettlement
                                    var generalLedgerAccountC2BJournal = JournalFactory.CreateJournal(null, postingPeriod.Id, mobileToBankRequestDTO.BranchId, null, persisted.TransAmount, EnumHelper.GetDescription(SystemTransactionCode.MobileToBank), string.Format("{0}~{1}", persisted.TransID, persisted.BillRefNumber), string.Format("{0}~{1}", persisted.MSISDN, persisted.KYCInfo), 0x9999, (int)SystemTransactionCode.MobileToBank, null, serviceHeader);
                                    _journalEntryPostingService.PerformDoubleEntry(generalLedgerAccountC2BJournal, mobileToBankRequestDTO.ChartOfAccountId.Value, settlementAccountId, serviceHeader);
                                    journals.Add(generalLedgerAccountC2BJournal);

                                    break;
                                default:
                                    break;
                            }
                        }

                        break;

                    case MobileToBankRequestAuthOption.Reject:

                        persisted.RecordStatus = (int)MobileToBankRequestRecordStatus.Rejected;
                        persisted.AuditRemarks = mobileToBankRequestDTO.AuditRemarks;
                        persisted.AuditedBy = serviceHeader.ApplicationUserName;
                        persisted.AuditedDate = DateTime.Now;

                        result = dbContextScope.SaveChanges(serviceHeader) >= 0;

                        break;
                    default:
                        break;
                }
            }

            if (result && journals.Any())
            {
                result = _journalEntryPostingService.BulkSave(serviceHeader, journals);

                if (result)
                {
                    #region Do we need to send alerts?

                    _brokerService.ProcessMobileToBankSenderAcknowledgementAccountAlerts(DMLCommand.None, serviceHeader, mobileToBankRequestDTO);

                    #endregion
                }
            }

            return result;
        }

        public List<MobileToBankRequestDTO> FindMobileToBankRequests(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var mobileToBankRequests = _mobileToBankRequestRepository.GetAll(serviceHeader);

                if (mobileToBankRequests != null && mobileToBankRequests.Any())
                {
                    return mobileToBankRequests.ProjectedAsCollection<MobileToBankRequestDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequests(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MobileToBankRequestSpecifications.MobileToBankRequestWithStatus(status, startDate, endDate, text);

                ISpecification<MobileToBankRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var mobileToBankRequestPagedCollection = _mobileToBankRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (mobileToBankRequestPagedCollection != null)
                {
                    var pageCollection = mobileToBankRequestPagedCollection.PageCollection.ProjectedAsCollection<MobileToBankRequestDTO>();

                    var itemsCount = mobileToBankRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<MobileToBankRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public MobileToBankRequestDTO FindMobileToBankRequest(Guid mobileToBankRequestId, ServiceHeader serviceHeader)
        {
            if (mobileToBankRequestId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var mobileToBankRequest = _mobileToBankRequestRepository.Get(mobileToBankRequestId, serviceHeader);

                    if (mobileToBankRequest != null)
                    {
                        return mobileToBankRequest.ProjectedAs<MobileToBankRequestDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private CustomerAccountDTO MatchCustomerAccount(MobileToBankRequestDTO mobileToBankRequestDTO, ServiceHeader serviceHeader)
        {
            CustomerAccountDTO targetCustomerAccount = null;

            if (!string.IsNullOrWhiteSpace(mobileToBankRequestDTO.BillRefNumber))
            {
                #region matching-version-1 e.g ABCD1234001001

                var rightSix = mobileToBankRequestDTO.BillRefNumber.Right(6);

                if (!string.IsNullOrWhiteSpace(rightSix) && rightSix.Length == 6)
                {
                    var productCode = -1;
                    int.TryParse(rightSix.Substring(0, 3), out productCode);

                    var targetProductCode = -1;
                    int.TryParse(rightSix.Substring(3), out targetProductCode);

                    var targetProductId = Guid.Empty;

                    if (productCode != -1 && targetProductCode != -1)
                    {
                        switch ((ProductCode)productCode)
                        {
                            case ProductCode.Savings:
                                var savingsProducts = _savingsProductAppService.FindSavingsProducts(targetProductCode, serviceHeader);
                                if (savingsProducts != null && savingsProducts.Any())
                                    targetProductId = savingsProducts[0].Id;
                                break;
                            case ProductCode.Loan:
                                var loanProducts = _loanProductAppService.FindLoanProducts(targetProductCode, serviceHeader);
                                if (loanProducts != null && loanProducts.Any())
                                    targetProductId = loanProducts[0].Id;
                                break;
                            case ProductCode.Investment:
                                var investmentProducts = _investmentProductAppService.FindInvestmentProducts(targetProductCode, serviceHeader);
                                if (investmentProducts != null && investmentProducts.Any())
                                    targetProductId = investmentProducts[0].Id;
                                break;
                            default:
                                break;
                        }

                        if (targetProductId != Guid.Empty)
                        {
                            var payrollNumber = mobileToBankRequestDTO.BillRefNumber.Substring(0, mobileToBankRequestDTO.BillRefNumber.Length - 6);

                            var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndPayrollNumber(targetProductId, payrollNumber, serviceHeader);

                            if (customerAccounts.Any() && customerAccounts.Count == 1)
                                targetCustomerAccount = customerAccounts[0];
                        }
                    }
                }

                #endregion

                #region  matching-version-2 e.g ABCD1234S01

                if (targetCustomerAccount == null)
                {
                    var rightThree = mobileToBankRequestDTO.BillRefNumber.Right(3);

                    if (!string.IsNullOrWhiteSpace(rightThree) && rightThree.Length == 3)
                    {
                        var targetProductCode = -1;

                        var targetProductId = Guid.Empty;

                        switch (rightThree.Substring(0, 1).ToUpper())
                        {
                            case "S":
                                if (int.TryParse(rightThree.Substring(1), out targetProductCode))
                                {
                                    var savingsProducts = _savingsProductAppService.FindSavingsProducts(targetProductCode, serviceHeader);

                                    if (savingsProducts != null && savingsProducts.Any())
                                    {
                                        targetProductId = savingsProducts[0].Id;
                                    }
                                }
                                break;
                            case "L":
                                if (int.TryParse(rightThree.Substring(1), out targetProductCode))
                                {
                                    var loanProducts = _loanProductAppService.FindLoanProducts(targetProductCode, serviceHeader);

                                    if (loanProducts != null && loanProducts.Any())
                                    {
                                        targetProductId = loanProducts[0].Id;
                                    }
                                }
                                break;
                            case "I":
                                if (int.TryParse(rightThree.Substring(1), out targetProductCode))
                                {
                                    var investmentProducts = _investmentProductAppService.FindInvestmentProducts(targetProductCode, serviceHeader);

                                    if (investmentProducts != null && investmentProducts.Any())
                                    {
                                        targetProductId = investmentProducts[0].Id;
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        if (targetProductId != Guid.Empty)
                        {
                            var payrollNumber = mobileToBankRequestDTO.BillRefNumber.Substring(0, mobileToBankRequestDTO.BillRefNumber.Length - 3);

                            var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndPayrollNumber(targetProductId, payrollNumber, serviceHeader);

                            if (customerAccounts.Any() && customerAccounts.Count == 1)
                                targetCustomerAccount = customerAccounts[0];
                        }
                    }
                }

                #endregion

                #region matching-version-3 channel_msisdn

                if (targetCustomerAccount == null && mobileToBankRequestDTO.MatchByMSISDN)
                {
                    var alternateChannels = _alternateChannelAppService.FindAlternateChannelsByCardNumber(mobileToBankRequestDTO.MSISDN, serviceHeader);

                    if (alternateChannels != null && alternateChannels.Any())
                    {
                        foreach (var item in alternateChannels)
                        {
                            targetCustomerAccount = _sqlCommandAppService.FindCustomerAccountById(item.CustomerAccountId, serviceHeader);

                            break;
                        }
                    }
                }

                #endregion
            }

            return targetCustomerAccount;
        }

        public PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequests(int status, int recordStatus, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MobileToBankRequestSpecifications.MobileToBankRequestWithStatusAndRecordStatus(status, recordStatus, startDate, endDate, text);

                ISpecification<MobileToBankRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var mobileToBankRequestPagedCollection = _mobileToBankRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (mobileToBankRequestPagedCollection != null)
                {
                    var pageCollection = mobileToBankRequestPagedCollection.PageCollection.ProjectedAsCollection<MobileToBankRequestDTO>();

                    var itemsCount = mobileToBankRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<MobileToBankRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<MobileToBankRequestDTO> FindMobileToBankRequests(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = MobileToBankRequestSpecifications.MobileToBankRequestWithDateRangeAndFilter(startDate, endDate, text);

                ISpecification<MobileToBankRequest> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var mobileToBankRequestPagedCollection = _mobileToBankRequestRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (mobileToBankRequestPagedCollection != null)
                {
                    var pageCollection = mobileToBankRequestPagedCollection.PageCollection.ProjectedAsCollection<MobileToBankRequestDTO>();

                    var itemsCount = mobileToBankRequestPagedCollection.ItemsCount;

                    return new PageCollectionInfo<MobileToBankRequestDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }
    }
}