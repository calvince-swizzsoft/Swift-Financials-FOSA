using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesInvoiceLineAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class SalesInvoiceAppService : ISalesInvoiceAppService
    {

        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SalesInvoice> _salesInvoiceRepository;
        private readonly IRepository<SalesInvoiceLine> _salesInvoiceLineRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;
        private readonly IJournalAppService _journalAppService;

        public SalesInvoiceAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<SalesInvoice> salesInvoiceRepository,
           IRepository<SalesInvoiceLine> salesInvoiceLineRepository,
           IChartOfAccountAppService chartOfAccountAppService,
            INumberSeriesGenerator numberSeriesGenerator,
           IJournalAppService journalAppService
           )
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (salesInvoiceRepository == null)
                throw new ArgumentNullException(nameof(salesInvoiceRepository));

            if (salesInvoiceLineRepository == null)
                throw new ArgumentNullException(nameof(salesInvoiceLineRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (numberSeriesGenerator == null)
                throw new ArgumentNullException(nameof(numberSeriesGenerator));

            _dbContextScopeFactory = dbContextScopeFactory;
            _salesInvoiceRepository = salesInvoiceRepository;
            _salesInvoiceLineRepository = salesInvoiceLineRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _numberSeriesGenerator = numberSeriesGenerator;
            _journalAppService = journalAppService;
        }



        public SalesInvoiceDTO AddNewSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, ServiceHeader serviceHeader)
        {
            if (salesInvoiceDTO != null)
            {

                var salesInvoiceNo = _numberSeriesGenerator.GetNextNumber("SI", serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {

                    var salesInvoice = SalesInvoiceFactory.CreateSalesInvoice(salesInvoiceNo, salesInvoiceDTO.CustomerNo, salesInvoiceDTO.CustomerName, salesInvoiceDTO.CustomerAddress, salesInvoiceDTO.DocumentDate, salesInvoiceDTO.PostingDate, salesInvoiceDTO.DueDate, salesInvoiceDTO.ApprovalStatus, salesInvoiceDTO.PaidAmount, salesInvoiceDTO.RemainingAmount,salesInvoiceDTO.TotalAmount, serviceHeader);

                    AddLines(salesInvoiceDTO, salesInvoice, serviceHeader);

                    salesInvoice.CreatedBy = serviceHeader.ApplicationUserName;

                    _salesInvoiceRepository.Add(salesInvoice, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return salesInvoice.ProjectedAs<SalesInvoiceDTO>();
                }
            }
            else return null;
        }

        public bool UpdateSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, ServiceHeader serviceHeader)
        {
            //if (salesInvoiceDTO == null || salesInvoiceDTO.Id == Guid.Empty)
            //    return false;

            if (salesInvoiceDTO == null || salesInvoiceDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salesInvoiceRepository.Get(salesInvoiceDTO.Id, serviceHeader);

                if (persisted != null)
                {

                    var current = SalesInvoiceFactory.CreateSalesInvoice(persisted.No, salesInvoiceDTO.CustomerNo, salesInvoiceDTO.CustomerName, salesInvoiceDTO.CustomerAddress, salesInvoiceDTO.DocumentDate, salesInvoiceDTO.PostingDate, salesInvoiceDTO.DueDate, salesInvoiceDTO.ApprovalStatus, salesInvoiceDTO.PaidAmount, salesInvoiceDTO.RemainingAmount, salesInvoiceDTO.TotalAmount, serviceHeader);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _salesInvoiceRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }


        public void AddLines(SalesInvoiceDTO salesInvoiceDTO, SalesInvoice salesInvoice, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();



            if (salesInvoice == null || salesInvoice.IsTransient())
                sbErrors.Append("Sales Invoice is either null or in transient state! ");

            if (salesInvoice.Id == null || salesInvoice.Id == Guid.Empty)
                sbErrors.Append("Sales Invoice Id is null or empty!");


            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform double-entry operations to in-memory Domain-Model objects
                // 

                if (salesInvoiceDTO.SalesInvoiceLines != null && salesInvoiceDTO.SalesInvoiceLines.Any())
                {
                    foreach (var item in salesInvoiceDTO.SalesInvoiceLines)
                    {

                        salesInvoice.AddLine(item.Type, item.No, item.Description, item.UnitCost, item.Quantity, item.Amount, item.CreditChartOfAccountId, serviceHeader);
                    }
                }
            }
        }


        public SalesInvoiceDTO FindSalesInvoice(Guid salesInvoiceId, ServiceHeader serviceHeader)
        {

            using (_dbContextScopeFactory.CreateReadOnly())
            {

                var salesInvoice = _salesInvoiceRepository.Get(salesInvoiceId, serviceHeader);

                if (salesInvoice != null)
                {
                    return salesInvoice.ProjectedAs<SalesInvoiceDTO>();
                }

                else
                {
                    return null;
                }
            }
        }



        public List<SalesInvoiceDTO> FindSalesInvoices(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var salesInvoices = _salesInvoiceRepository.GetAll(serviceHeader);

                if (salesInvoices != null && salesInvoices.Any())
                {
                    return salesInvoices.ProjectedAsCollection<SalesInvoiceDTO>();
                }
                else return null;
            }
        }


        public JournalDTO PostSalesInvoice(SalesInvoiceDTO salesInvoiceDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (salesInvoiceDTO == null || !salesInvoiceDTO.SalesInvoiceLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            var receivablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
                (int)SystemGeneralLedgerAccountCode.AccountReceivables, serviceHeader);

            if (receivablesChartOfAccountId == Guid.Empty)
            {
                throw new InvalidOperationException("Sorry, but the requisite receivables chart of account has not been setup!");
            }

            var salesInvoiceLineDTOs = salesInvoiceDTO.SalesInvoiceLines;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted invoice once, outside the loop
                var persisted = _salesInvoiceRepository.Get(salesInvoiceDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("Sales invoice not found!");
                }

                // Process each purchase invoice line
                foreach (var item in salesInvoiceLineDTOs)
                {
                    var journal = _journalAppService.AddNewJournal(
                        salesInvoiceDTO.BranchId,
                        null,
                        item.Amount,
                        string.Format("Sales Invoice~{0}", item.No),
                        salesInvoiceDTO.BankBranchName,
                        item.No.ToString(),
                        moduleNavigationItemCode,
                        (int)SystemTransactionCode.InterAcccountTransfer,
                        null,
                        item.CreditChartOfAccountId,
                        receivablesChartOfAccountId,
                        serviceHeader);

                    if (journal == null)
                    {
                        throw new InvalidOperationException($"Failed to create journal for purchase invoice line {item.No}");
                    }

                    lastCreatedJournal = journal;
                }

                // Mark the purchase invoice as posted in both DTO and persisted entity
                salesInvoiceDTO.Posted = true;
                persisted.Posted = true;

                // Save all changes at once
                if (dbContextScope.SaveChanges(serviceHeader) >= 0)
                {
                    return lastCreatedJournal;
                }
                else
                {
                    throw new InvalidOperationException("Failed to save journal entries to database!");
                }
            }
        }
    }
}
