using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseCreditMemoLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.SalesCreditMemoLineAgg;
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
    public class SalesCreditMemoAppService : ISalesCreditMemoAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<SalesCreditMemo> _salesCreditMemoRepository;
        private readonly IRepository<SalesCreditMemoLine> _salesCreditMemoLineRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;
        private readonly IJournalAppService _journalAppService;

        public SalesCreditMemoAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<SalesCreditMemo> salesCreditMemoRepository,
            IRepository<SalesCreditMemoLine> salesCreditMemoLineRepository,
            IChartOfAccountAppService chartOfAccountAppService,
            INumberSeriesGenerator numberSeriesGenerator,

            IJournalAppService journalAppService
        )
        {
            if (dbContextScopeFactory == null) throw new ArgumentNullException(nameof(dbContextScopeFactory));
            if (salesCreditMemoRepository == null) throw new ArgumentNullException(nameof(salesCreditMemoRepository));
            if (salesCreditMemoLineRepository == null) throw new ArgumentNullException(nameof(salesCreditMemoLineRepository));
            if (journalAppService == null) throw new ArgumentNullException(nameof(journalAppService));
            if (chartOfAccountAppService == null) throw new ArgumentNullException(nameof(chartOfAccountAppService));
            if (numberSeriesGenerator == null) throw new ArgumentNullException(nameof(numberSeriesGenerator));

            _dbContextScopeFactory = dbContextScopeFactory;
            _salesCreditMemoRepository = salesCreditMemoRepository;
            _salesCreditMemoLineRepository = salesCreditMemoLineRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _numberSeriesGenerator = numberSeriesGenerator;
            _journalAppService = journalAppService;
        }

        public SalesCreditMemoDTO AddNewSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, ServiceHeader serviceHeader)
        {
            if (salesCreditMemoDTO != null)
            {

                var salesCreditMemoNo = _numberSeriesGenerator.GetNextNumber("SCM", serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var salesCreditMemo = SalesCreditMemoFactory.CreateSalesCreditMemo(salesCreditMemoNo, salesCreditMemoDTO.CustomerNo,
                        salesCreditMemoDTO.CustomerName, salesCreditMemoDTO.CustomerAddress, salesCreditMemoDTO.DocumentDate, salesCreditMemoDTO.PostingDate,
                        salesCreditMemoDTO.DueDate, salesCreditMemoDTO.SalesInvoiceId, salesCreditMemoDTO.AppliesToDocNo, salesCreditMemoDTO.ApprovalStatus, salesCreditMemoDTO.TotalAmount, serviceHeader);

                    AddLines(salesCreditMemoDTO, salesCreditMemo, serviceHeader);
                    salesCreditMemo.CreatedBy = serviceHeader.ApplicationUserName;

                    _salesCreditMemoRepository.Add(salesCreditMemo, serviceHeader);
                    dbContextScope.SaveChanges(serviceHeader);

                    return salesCreditMemo.ProjectedAs<SalesCreditMemoDTO>();
                }
            }
            else
                return null;
        }

        public bool UpdateSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, ServiceHeader serviceHeader)
        {
            if (salesCreditMemoDTO == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _salesCreditMemoRepository.Get(salesCreditMemoDTO.Id, serviceHeader);
                if (persisted != null)
                {
                    var current = SalesCreditMemoFactory.CreateSalesCreditMemo(persisted.No, salesCreditMemoDTO.CustomerNo,
                        salesCreditMemoDTO.CustomerName, salesCreditMemoDTO.CustomerAddress, salesCreditMemoDTO.DocumentDate, salesCreditMemoDTO.PostingDate,
                        salesCreditMemoDTO.DueDate, salesCreditMemoDTO.SalesInvoiceId, salesCreditMemoDTO.AppliesToDocNo, salesCreditMemoDTO.ApprovalStatus, salesCreditMemoDTO.TotalAmount, serviceHeader);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _salesCreditMemoRepository.Merge(persisted, current, serviceHeader);
                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else
                    return false;
            }
        }

        public void AddLines(SalesCreditMemoDTO salesCreditMemoDTO, SalesCreditMemo salesCreditMemo, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();
            if (salesCreditMemo == null || salesCreditMemo.IsTransient())
                sbErrors.Append("Credit Memo is either null or in transient state! ");
            if (salesCreditMemo.Id == null || salesCreditMemo.Id == Guid.Empty)
                sbErrors.Append("Credit Memo Id is null or empty!");

            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform double-entry operations to in-memory Domain-Model objects
                if (salesCreditMemoDTO.SalesCreditMemoLines != null && salesCreditMemoDTO.SalesCreditMemoLines.Any())
                {
                    foreach (var item in salesCreditMemoDTO.SalesCreditMemoLines)
                    {
                        salesCreditMemo.AddLine(item.Type, item.No, item.Description, item.Quantity, item.Amount, item.DebitChartOfAccountId, item.UnitCost, serviceHeader);
                    }
                }
            }
        }

        public List<SalesCreditMemoDTO> FindSalesCreditMemos(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var salesCreditMemos = _salesCreditMemoRepository.GetAll(serviceHeader);
                if (salesCreditMemos != null && salesCreditMemos.Any())
                {
                    return salesCreditMemos.ProjectedAsCollection<SalesCreditMemoDTO>();
                }
                else
                    return null;
            }
        }

        public JournalDTO PostSalesCreditMemo(SalesCreditMemoDTO salesCreditMemoDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (salesCreditMemoDTO == null || !salesCreditMemoDTO.SalesCreditMemoLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            var receivablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
                (int)SystemGeneralLedgerAccountCode.AccountReceivables, serviceHeader);

            if (receivablesChartOfAccountId == Guid.Empty)
            {
                throw new InvalidOperationException("Sorry, but the requisite receivables chart of account has not been setup!");
            }

            var salesCreditMemoLineDTOs = salesCreditMemoDTO.SalesCreditMemoLines;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted invoice once, outside the loop
                var persisted = _salesCreditMemoRepository.Get(salesCreditMemoDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("Purchase invoice not found!");
                }

                // Process each purchase invoice line
                foreach (var item in salesCreditMemoLineDTOs)
                {
                    var journal = _journalAppService.AddNewJournal(
                        salesCreditMemoDTO.BranchId,
                        null,
                        item.Amount,
                        string.Format("Sales Credit Memo~{0}", item.No),
                        salesCreditMemoDTO.BankBranchName,
                        item.No.ToString(),
                        moduleNavigationItemCode,
                        (int)SystemTransactionCode.InterAcccountTransfer,
                        null,
                        receivablesChartOfAccountId,
                        item.DebitChartOfAccountId,
                        serviceHeader);

                    if (journal == null)
                    {
                        throw new InvalidOperationException($"Failed to create journal for purchase invoice line {item.No}");
                    }

                    lastCreatedJournal = journal;
                }

                // Mark the purchase invoice as posted in both DTO and persisted entity
                salesCreditMemoDTO.Posted = true;
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