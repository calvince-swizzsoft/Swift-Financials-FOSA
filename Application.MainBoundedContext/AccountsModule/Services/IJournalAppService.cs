using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface IJournalAppService
    {
        JournalDTO AddNewJournal(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, ServiceHeader serviceHeader, bool useCache = true);

        JournalDTO AddNewJournal(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, ServiceHeader serviceHeader, bool useCache = true);

        JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<ApportionmentWrapper> apportionments, List<TariffWrapper> tariffs, List<DynamicChargeDTO> dynamicCharges, ServiceHeader serviceHeader, bool useCache = true);

        JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ServiceHeader serviceHeader, bool useCache = true);

        JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs, ServiceHeader serviceHeader, bool useCache = true);

        JournalDTO AddNewJournal(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid creditChartOfAccountId, Guid debitChartOfAccountId, List<TariffWrapper> tariffs, ServiceHeader serviceHeader, bool useCache = true);

        bool AddNewJournals(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, List<TariffWrapper> tariffs, ServiceHeader serviceHeader, bool useCache = true);

        bool ReverseAlternateChannelJournals(Guid[] alternateChannelLogIds, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        bool ReverseJournals(List<JournalDTO> journalDTOs, string description, int moduleNavigationItemCode, ServiceHeader serviceHeader);

        //decimal RecoverSundryCarryFowards(Guid? parentJournalId, Guid branchId, Guid? alternateChannelLogId, Guid postingPeriodId, CustomerAccountDTO benefactorCustomerAccountDTO, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, ServiceHeader serviceHeader, bool useCache);

        JournalDTO FindJournal(Guid journalId, ServiceHeader serviceHeader);

        List<JournalDTO> FindJournals(Guid[] alternateChannelLogIds, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalDTO> FindJournals(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, string text, int journalFilter, ServiceHeader serviceHeader);

        PageCollectionInfo<JournalDTO> FindReversibleJournals(int pageIndex, int pageSize, int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalFilter, ServiceHeader serviceHeader);

        List<JournalEntryDTO> FindJournalEntries(ServiceHeader serviceHeader, params Guid[] journalIds);

        PageCollectionInfo<JournalEntryDTO> FindJournalEntries(int pageIndex, int pageSize, ServiceHeader serviceHeader, params Guid[] journalIds);

        JournalDTO AddNewJournalSingleEntry(Guid branchId, Guid? alternateChannelLogId, decimal totalValue, string primaryDescription, string secondaryDescription, string reference, int moduleNavigationItemCode, int transactionCode, DateTime? valueDate, Guid chartOfAccountId, Guid contraChartOfAccountId, int journalType, ServiceHeader serviceHeader, bool useCache = true);
    }
}
