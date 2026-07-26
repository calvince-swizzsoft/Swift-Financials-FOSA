using Application.MainBoundedContext.DTO.AccountsModule;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountArrearageAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderHistoryAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.MainBoundedContext.Services
{
    public class JournalEntryPostingService : IJournalEntryPostingService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Journal> _journalRepository;
        private readonly IRepository<CustomerAccountCarryForward> _customerAccountCarryForwardRepository;
        private readonly IRepository<StandingOrderHistory> _standingOrderHistoryRepository;
        private readonly IRepository<CustomerAccountArrearage> _customerAccountArrearageRepository;

        public JournalEntryPostingService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<Journal> journalRepository,
            IRepository<CustomerAccountCarryForward> customerAccountCarryForwardRepository,
            IRepository<StandingOrderHistory> standingOrderHistoryRepository,
            IRepository<CustomerAccountArrearage> customerAccountArrearageRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (journalRepository == null)
                throw new ArgumentNullException(nameof(journalRepository));

            if (customerAccountCarryForwardRepository == null)
                throw new ArgumentNullException(nameof(customerAccountCarryForwardRepository));

            if (standingOrderHistoryRepository == null)
                throw new ArgumentNullException(nameof(standingOrderHistoryRepository));

            if (customerAccountArrearageRepository == null)
                throw new ArgumentNullException(nameof(customerAccountArrearageRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _journalRepository = journalRepository;
            _customerAccountCarryForwardRepository = customerAccountCarryForwardRepository;
            _standingOrderHistoryRepository = standingOrderHistoryRepository;
            _customerAccountArrearageRepository = customerAccountArrearageRepository;
        }

        public void PerformSingleEntry(Journal journal, Guid chartOfAccountId, Guid contraChartOfAccountId, decimal amount, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();

            if (journal == null || journal.IsTransient())
                sbErrors.Append("Journal is either null or in transient state! ");

            if (chartOfAccountId == null || chartOfAccountId == Guid.Empty)
                sbErrors.Append("Chart Of Account is null or empty!");

            //if (contraChartOfAccountId == null || contraChartOfAccountId == Guid.Empty)
            //    sbErrors.Append("Contra Chart Of Account is null or empty!");

            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform single-entry operation to in-memory Domain-Model objects        
                journal.PostSingleEntry(chartOfAccountId, contraChartOfAccountId, amount, serviceHeader);
            }
        }

        public void PerformSingleEntry(Journal journal, Guid chartOfAccountId, Guid contraChartOfAccountId, Guid customerAccountId, decimal amount, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();

            if (journal == null || journal.IsTransient())
                sbErrors.Append("Journal is either null or in transient state! ");

            if (chartOfAccountId == null || chartOfAccountId == Guid.Empty)
                sbErrors.Append("Chart Of Account is null or empty!");

            if (contraChartOfAccountId == null || contraChartOfAccountId == Guid.Empty)
                sbErrors.Append("Contra Chart Of Account is null or empty!");

            if (customerAccountId == null || customerAccountId == Guid.Empty)
                sbErrors.Append("Customer Account is null or empty!");

            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform single-entry operation to in-memory Domain-Model objects        
                journal.PostSingleEntry(chartOfAccountId, contraChartOfAccountId, customerAccountId, amount, serviceHeader);
            }
        }

        public void PerformDoubleEntry(Journal journal, Guid creditChartOfAccountId, Guid debitChartOfAccountId, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();

            if (journal == null || journal.IsTransient())
                sbErrors.Append("Journal is either null or in transient state! ");

            if (creditChartOfAccountId == null || creditChartOfAccountId == Guid.Empty)
                sbErrors.Append("Credit Chart Of Account is null or empty!");

            if (debitChartOfAccountId == null || debitChartOfAccountId == Guid.Empty)
                sbErrors.Append("Debit Chart Of Account is null or empty!");

            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform double-entry operations to in-memory Domain-Model objects        
                journal.PostDoubleEntries(debitChartOfAccountId, creditChartOfAccountId, serviceHeader);
            }
        }

        public void PerformDoubleEntry(Journal journal, Guid creditChartOfAccountId, Guid debitChartOfAccountId, CustomerAccountDTO creditCustomerAccountDTO, CustomerAccountDTO debitCustomerAccountDTO, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();

            if (journal == null || journal.IsTransient())
                sbErrors.Append("Journal is either null or in transient state! ");

            if (creditChartOfAccountId == null || creditChartOfAccountId == Guid.Empty)
                sbErrors.Append("Credit Chart Of Account is null or empty!");

            if (debitChartOfAccountId == null || debitChartOfAccountId == Guid.Empty)
                sbErrors.Append("Debit Chart Of Account is null or empty!");

            if (debitCustomerAccountDTO == null)
                sbErrors.Append("Debit Customer Account is null!");

            if (creditCustomerAccountDTO == null)
                sbErrors.Append("Credit Customer Account is null!");

            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform double-entry operations to in-memory Domain-Model objects        
                journal.PostDoubleEntries(debitChartOfAccountId, creditChartOfAccountId, creditCustomerAccountDTO.Id, debitCustomerAccountDTO.Id, serviceHeader);
            }
        }

        public bool BulkSave(ServiceHeader serviceHeader, List<Journal> journals, List<CustomerAccountCarryForward> customerAccountCarryForwards, List<StandingOrderHistory> standingOrderHistories, List<CustomerAccountArrearage> customerAccountArrearages)
        {
            var result = default(bool);

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                if (journals != null && journals.Any())
                {
                    journals.ForEach(item =>
                    {
                        _journalRepository.Add(item, serviceHeader);
                    });
                }

                if (customerAccountCarryForwards != null && customerAccountCarryForwards.Any())
                {
                    customerAccountCarryForwards.ForEach(item =>
                    {
                        _customerAccountCarryForwardRepository.Add(item, serviceHeader);
                    });
                }

                if (standingOrderHistories != null && standingOrderHistories.Any())
                {
                    standingOrderHistories.ForEach(item =>
                    {
                        _standingOrderHistoryRepository.Add(item, serviceHeader);
                    });
                }

                if (customerAccountArrearages != null && customerAccountArrearages.Any())
                {
                    customerAccountArrearages.ForEach(item =>
                    {
                        _customerAccountArrearageRepository.Add(item, serviceHeader);
                    });
                }

                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
            }

            return result;
        }
    }
}
