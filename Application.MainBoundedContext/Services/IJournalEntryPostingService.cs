using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.Services
{
    public interface IJournalEntryPostingService
    {
        void PerformSingleEntry(Journal journal, Guid chartOfAccountId, Guid contraChartOfAccountId, decimal amount, ServiceHeader serviceHeader);

        void PerformSingleEntry(Journal journal, Guid chartOfAccountId, Guid contraChartOfAccountId, Guid customerAccountId, decimal amount, ServiceHeader serviceHeader);

        void PerformDoubleEntry(Journal journal, Guid creditChartOfAccountId, Guid debitChartOfAccountId, ServiceHeader serviceHeader);

        void PerformDoubleEntry(Journal journal, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ServiceHeader serviceHeader);

        bool BulkSave(ServiceHeader serviceHeader, List<Journal> journals = null, List<CustomerAccountCarryForward> customerAccountCarryForwards = null, List<StandingOrderHistory> standingOrderHistories = null, List<CustomerAccountArrearage> customerAccountArrearages = null);
    }
}
