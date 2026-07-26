using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class JournalService : IJournalService
    {
        private readonly IJournalAppService _journalAppService;
        private readonly IFinancialsService _financialsService;
        private readonly IBranchAppService _branchAppService;
        private readonly IFiscalCountAppService _fiscalCountAppService;
        private readonly IRecurringBatchAppService _recurringBatchAppService;

        public JournalService(
            IJournalAppService journalAppService,
            IFinancialsService financialsService,
            IBranchAppService branchAppService,
            IFiscalCountAppService fiscalCountAppService,
            IRecurringBatchAppService recurringBatchAppService)
        {
            Guard.ArgumentNotNull(journalAppService, nameof(journalAppService));
            Guard.ArgumentNotNull(financialsService, nameof(financialsService));
            Guard.ArgumentNotNull(branchAppService, nameof(branchAppService));
            Guard.ArgumentNotNull(fiscalCountAppService, nameof(fiscalCountAppService));
            Guard.ArgumentNotNull(recurringBatchAppService, nameof(recurringBatchAppService));

            _journalAppService = journalAppService;
            _financialsService = financialsService;
            _branchAppService = branchAppService;
            _fiscalCountAppService = fiscalCountAppService;
            _recurringBatchAppService = recurringBatchAppService;
        }

        #region Journal

        public JournalDTO AddJournal(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, List<TariffWrapper> tariffs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.AddNewJournal(branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, tariffs, serviceHeader);
        }

        public JournalDTO AddJournalSingleEntry(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid chartOfAccountId, Guid contraChartOfAccountId, int journalType, List<TariffWrapper> tariffs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.AddNewJournalSingleEntry(branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, chartOfAccountId, contraChartOfAccountId, journalType, serviceHeader, true);
        }

        public JournalDTO AddJournalWithApportionments(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<ApportionmentWrapper> apportionments, List<TariffWrapper> tariffs, List<DynamicChargeDTO> dynamicCharges)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.AddNewJournal(branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, debitChartOfAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, apportionments, tariffs, dynamicCharges, serviceHeader);
        }

        public JournalDTO AddCashManagementJournal(FiscalCountDTO fiscalCountDTO, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            JournalDTO journalDTO = null;

            switch ((SystemTransactionCode)transactionCode)
            {
                case SystemTransactionCode.TreasuryToTreasury:

                    var fiscalCountDTOs = new List<FiscalCountDTO>();

                    var sourceBranch = _branchAppService.FindBranch(fiscalCountDTO.BranchId, serviceHeader);

                    var destinationBranch = _branchAppService.FindBranch(fiscalCountDTO.DestinationBranchId, serviceHeader);

                    fiscalCountDTO.PrimaryDescription = string.Format("{0} (Source)", primaryDescription);
                    fiscalCountDTO.SecondaryDescription = string.Format("To {0}", destinationBranch.Description);

                    fiscalCountDTOs.Add(fiscalCountDTO);

                    var newFiscalCountDTO = new FiscalCountDTO();

                    newFiscalCountDTO.PostingPeriodId = fiscalCountDTO.PostingPeriodId;
                    newFiscalCountDTO.BranchId = fiscalCountDTO.DestinationBranchId;
                    newFiscalCountDTO.ChartOfAccountId = fiscalCountDTO.ChartOfAccountId;
                    newFiscalCountDTO.DestinationBranchId = fiscalCountDTO.DestinationBranchId;
                    newFiscalCountDTO.PrimaryDescription = string.Format("{0} (Destination)", primaryDescription);
                    newFiscalCountDTO.SecondaryDescription = string.Format("From {0}", sourceBranch.Description);
                    newFiscalCountDTO.Reference = fiscalCountDTO.Reference;
                    newFiscalCountDTO.TransactionCode = fiscalCountDTO.TransactionCode;

                    newFiscalCountDTO.DenominationOneThousandValue = fiscalCountDTO.DenominationOneThousandValue;
                    newFiscalCountDTO.DenominationFiveHundredValue = fiscalCountDTO.DenominationFiveHundredValue;
                    newFiscalCountDTO.DenominationTwoHundredValue = fiscalCountDTO.DenominationTwoHundredValue;
                    newFiscalCountDTO.DenominationOneHundredValue = fiscalCountDTO.DenominationOneHundredValue;
                    newFiscalCountDTO.DenominationFiftyValue = fiscalCountDTO.DenominationFiftyValue;
                    newFiscalCountDTO.DenominationFourtyValue = fiscalCountDTO.DenominationFourtyValue;
                    newFiscalCountDTO.DenominationTwentyValue = fiscalCountDTO.DenominationTwentyValue;
                    newFiscalCountDTO.DenominationTenValue = fiscalCountDTO.DenominationTenValue;
                    newFiscalCountDTO.DenominationFiveValue = fiscalCountDTO.DenominationFiveValue;
                    newFiscalCountDTO.DenominationOneValue = fiscalCountDTO.DenominationOneValue;
                    newFiscalCountDTO.DenominationFiftyCentValue = fiscalCountDTO.DenominationFiftyCentValue;

                    fiscalCountDTOs.Add(newFiscalCountDTO);

                    if (_fiscalCountAppService.AddNewFiscalCounts(fiscalCountDTOs, serviceHeader))
                    {
                        journalDTO = _journalAppService.AddNewJournal(null, sourceBranch.Id, null, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, serviceHeader);
                    }

                    break;
                default:

                    if (_fiscalCountAppService.AddNewFiscalCounts(new List<FiscalCountDTO> { fiscalCountDTO }, serviceHeader))
                    {
                        journalDTO = _journalAppService.AddNewJournal(null, fiscalCountDTO.BranchId, null, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, serviceHeader);
                    }

                    break;
            }

            return journalDTO;
        }

        public JournalDTO AddJournalWithCustomerAccount(Guid branchId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.AddNewJournal(branchId, null, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, serviceHeader);
        }

        public JournalDTO AddJournalWithCustomerAccountAndTariffs(Guid branchId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var journalDTO = _journalAppService.AddNewJournal(branchId, null, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, tariffs, serviceHeader);

            if (transactionCode.In((int)SystemTransactionCode.CashDeposit))
            {
                var branchDTO = _branchAppService.FindBranch(branchId, serviceHeader);

                if (branchDTO != null && branchDTO.CompanyRecoverArrearsOnCashDeposit)
                    _recurringBatchAppService.RecoverArrears(creditCustomerAccountDTO, (int)QueuePriority.Highest, serviceHeader);
            }

            return journalDTO;
        }

        public JournalDTO AddJournalWithCustomerAccountAndAlternateChannelLogAndTariffs(Guid branchId, Guid alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.AddNewJournal(branchId, alternateChannelLogId, totalValue, primaryDescription, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditChartOfAccountId, debitChartOfAccountId, creditCustomerAccountDTO, debitCustomerAccountDTO, tariffs, serviceHeader);
        }

        public bool AddTariffJournalsWithCustomerAccount(Guid? parentJournalId, Guid branchId, Guid alternateChannelLogId, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.AddNewJournals(parentJournalId, branchId, alternateChannelLogId, secondaryDescription, reference, moduleNavigationItemCode, transactionCode, valueDate, creditCustomerAccountDTO, debitCustomerAccountDTO, tariffs, serviceHeader);
        }

        public PageCollectionInfo<JournalDTO> FindReversibleJournalsByDateRangeAndFilterInPage(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalFilter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.FindReversibleJournals(pageIndex, pageSize, systemTransactionCode, startDate, endDate, text, journalFilter, serviceHeader);
        }

        public bool ReverseJournals(List<JournalDTO> journalDTOs, string description, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.ReverseJournals(journalDTOs, description, moduleNavigationItemCode, serviceHeader);
        }

        public bool ReverseAlternateChannelJournals(Guid[] alternateChannelLogIds)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.ReverseAlternateChannelJournals(alternateChannelLogIds, 0x9999, serviceHeader);
        }

        public JournalDTO FindJournal(Guid journalId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.FindJournal(journalId, serviceHeader);
        }

        public List<JournalEntryDTO> FindJournalEntriesByJournalId(Guid journalId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _journalAppService.FindJournalEntries(serviceHeader, journalId);
        }

        #endregion

        #region Financials

        public double FV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double PV = 0, int Due = 0)
        {
            return _financialsService.FV(termInMonths, paymentFrequencyPerYear, APR, Pmt, PV, Due);
        }

        public double PV(int termInMonths, int paymentFrequencyPerYear, double APR, double Pmt, double FV = 0, int Due = 0)
        {
            return _financialsService.PV(termInMonths, paymentFrequencyPerYear, APR, Pmt, FV, Due);
        }

        public double Pmt(int interestCalculationMode, int termInMonths, int paymentFrequencyPerYear, double APR, double PV, double FV = 0, int Due = 0)
        {
            return _financialsService.Pmt(interestCalculationMode, termInMonths, paymentFrequencyPerYear, APR, PV, FV, Due);
        }

        public double PPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0)
        {
            return _financialsService.PPmt(termInMonths, paymentFrequencyPerYear, APR, Per, PV, FV, Due);
        }

        public double IPmt(int termInMonths, int paymentFrequencyPerYear, double APR, double Per, double PV, double FV = 0, int Due = 0)
        {
            return _financialsService.IPmt(termInMonths, paymentFrequencyPerYear, APR, Per, PV, FV, Due);
        }

        public double NPer(int paymentFrequencyPerYear, double APR, double Pmt, double PV, double FV = 0, int Due = 0)
        {
            return _financialsService.NPer(paymentFrequencyPerYear, APR, Pmt, PV, FV, Due);
        }

        public List<AmortizationTableEntry> RepaymentSchedule(int termInMonths, int paymentFrequencyPerYear, int gracePeriod, int interestCalculationMode, double APR, double PV, double FV = 0, int Due = 0)
        {
            return _financialsService.RepaymentSchedule(termInMonths, paymentFrequencyPerYear, gracePeriod, interestCalculationMode, APR, PV, FV, Due);
        }

        #endregion
    }
}
