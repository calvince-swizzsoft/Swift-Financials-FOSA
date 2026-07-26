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
    public class PurchaseCreditMemoAppService : IPurchaseCreditMemoAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<PurchaseCreditMemo> _purchaseCreditMemoRepository;
        private readonly IRepository<PurchaseCreditMemoLine> _purchaseCreditMemoLineRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;

        public PurchaseCreditMemoAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<PurchaseCreditMemo> purchaseCreditMemoRepository,
            IRepository<PurchaseCreditMemoLine> purchaseCreditMemoLineRepository,
            IChartOfAccountAppService chartOfAccountAppService,
              INumberSeriesGenerator numberSeriesGenerator,
            IJournalAppService journalAppService
        )
        {
            if (dbContextScopeFactory == null) throw new ArgumentNullException(nameof(dbContextScopeFactory));
            if (purchaseCreditMemoRepository == null) throw new ArgumentNullException(nameof(purchaseCreditMemoRepository));
            if (purchaseCreditMemoLineRepository == null) throw new ArgumentNullException(nameof(purchaseCreditMemoLineRepository));
            if (journalAppService == null) throw new ArgumentNullException(nameof(journalAppService));
            if (chartOfAccountAppService == null) throw new ArgumentNullException(nameof(chartOfAccountAppService));
            if (numberSeriesGenerator == null)
                throw new ArgumentNullException(nameof(numberSeriesGenerator));

            _dbContextScopeFactory = dbContextScopeFactory;
            _purchaseCreditMemoRepository = purchaseCreditMemoRepository;
            _purchaseCreditMemoLineRepository = purchaseCreditMemoLineRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _journalAppService = journalAppService;
            _numberSeriesGenerator = numberSeriesGenerator;
        }

        public PurchaseCreditMemoDTO AddNewPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, ServiceHeader serviceHeader)
        {
            if (purchaseCreditMemoDTO != null)
            {

                var purchaseCreditMemoNo = _numberSeriesGenerator.GetNextNumber("PCM", serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var purchaseCreditMemo = PurchaseCreditMemoFactory.CreatePurchaseCreditMemo(purchaseCreditMemoNo,purchaseCreditMemoDTO.VendorNo,
                        purchaseCreditMemoDTO.VendorName, purchaseCreditMemoDTO.VendorAddress, purchaseCreditMemoDTO.DocumentDate, purchaseCreditMemoDTO.PostingDate,
                        purchaseCreditMemoDTO.DueDate, purchaseCreditMemoDTO.ApprovalStatus, purchaseCreditMemoDTO.TotalAmount, purchaseCreditMemoDTO.PurchaseInvoiceId, purchaseCreditMemoDTO.PurchaseInvoiceNo, serviceHeader);

                    AddLines(purchaseCreditMemoDTO, purchaseCreditMemo, serviceHeader);
                    purchaseCreditMemo.CreatedBy = serviceHeader.ApplicationUserName;

                    _purchaseCreditMemoRepository.Add(purchaseCreditMemo, serviceHeader);
                    dbContextScope.SaveChanges(serviceHeader);

                    return purchaseCreditMemo.ProjectedAs<PurchaseCreditMemoDTO>();
                }
            }
            else
                return null;
        }

        public bool UpdatePurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, ServiceHeader serviceHeader)
        {
            if (purchaseCreditMemoDTO == null)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _purchaseCreditMemoRepository.Get(purchaseCreditMemoDTO.Id, serviceHeader);
                if (persisted != null)
                {
                    var current = PurchaseCreditMemoFactory.CreatePurchaseCreditMemo(persisted.No, purchaseCreditMemoDTO.VendorNo,
                        purchaseCreditMemoDTO.VendorName, purchaseCreditMemoDTO.VendorAddress, purchaseCreditMemoDTO.DocumentDate, purchaseCreditMemoDTO.PostingDate,
                        purchaseCreditMemoDTO.DueDate, purchaseCreditMemoDTO.ApprovalStatus, purchaseCreditMemoDTO.TotalAmount, purchaseCreditMemoDTO.PurchaseInvoiceId, purchaseCreditMemoDTO.PurchaseInvoiceNo, serviceHeader);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _purchaseCreditMemoRepository.Merge(persisted, current, serviceHeader);
                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else
                    return false;
            }
        }

        public void AddLines(PurchaseCreditMemoDTO purchaseCreditMemoDTO, PurchaseCreditMemo purchaseCreditMemo, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();
            if (purchaseCreditMemo == null || purchaseCreditMemo.IsTransient())
                sbErrors.Append("Credit Memo is either null or in transient state! ");
            if (purchaseCreditMemo.Id == null || purchaseCreditMemo.Id == Guid.Empty)
                sbErrors.Append("Credit Memo Id is null or empty!");

            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform double-entry operations to in-memory Domain-Model objects
                if (purchaseCreditMemoDTO.PurchaseCreditMemoLines != null && purchaseCreditMemoDTO.PurchaseCreditMemoLines.Any())
                {
                    foreach (var item in purchaseCreditMemoDTO.PurchaseCreditMemoLines)
                    {
                        purchaseCreditMemo.AddLine(item.Type, item.No, item.Description, item.Quantity, item.UnitCost, item.Amount, item.CreditChartOfAccountId, serviceHeader);
                    }
                }
            }
        }

        public List<PurchaseCreditMemoDTO> FindPurchaseCreditMemos(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var purchaseCreditMemos = _purchaseCreditMemoRepository.GetAll(serviceHeader);
                if (purchaseCreditMemos != null && purchaseCreditMemos.Any())
                {
                    return purchaseCreditMemos.ProjectedAsCollection<PurchaseCreditMemoDTO>();
                }
                else
                    return null;
            }
        }

        public JournalDTO PostPurchaseCreditMemo(PurchaseCreditMemoDTO purchaseCreditMemoDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (purchaseCreditMemoDTO == null || !purchaseCreditMemoDTO.PurchaseCreditMemoLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            var payablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
                (int)SystemGeneralLedgerAccountCode.AccountPayables, serviceHeader);

            if (payablesChartOfAccountId == Guid.Empty)
            {
                throw new InvalidOperationException("Sorry, but the requisite payables chart of account has not been setup!");
            }

            var purchaseCreditMemoLineDTOs = purchaseCreditMemoDTO.PurchaseCreditMemoLines;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted invoice once, outside the loop
                var persisted = _purchaseCreditMemoRepository.Get(purchaseCreditMemoDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("Purchase invoice not found!");
                }

                // Process each purchase invoice line
                foreach (var item in purchaseCreditMemoLineDTOs)
                {
                    var journal = _journalAppService.AddNewJournal(
                        purchaseCreditMemoDTO.BranchId,
                        null,
                        item.Amount,
                        string.Format("Purchase Credit Memo~{0}", item.No),
                        purchaseCreditMemoDTO.BankBranchName,
                        item.No.ToString(),
                        moduleNavigationItemCode,
                        (int)SystemTransactionCode.InterAcccountTransfer,
                        null,
                        item.CreditChartOfAccountId,
                        payablesChartOfAccountId,
                        serviceHeader);

                    if (journal == null)
                    {
                        throw new InvalidOperationException($"Failed to create journal for purchase invoice line {item.No}");
                    }

                    lastCreatedJournal = journal;
                }

                // Mark the purchase invoice as posted in both DTO and persisted entity
                purchaseCreditMemoDTO.Posted = true;
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