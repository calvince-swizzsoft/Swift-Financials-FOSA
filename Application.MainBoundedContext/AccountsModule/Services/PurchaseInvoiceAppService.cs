using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevyAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.LevySplitAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class PurchaseInvoiceAppService : IPurchaseInvoiceAppService
    {

        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<PurchaseInvoice> _purchaseInvoiceRepository;
        private readonly IRepository<PurchaseInvoiceLine> _purchaseInvoiceLineRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;
        private readonly IJournalAppService _journalAppService;

        public PurchaseInvoiceAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<PurchaseInvoice> purchaseInvoiceRepository,
           IRepository<PurchaseInvoiceLine> purchaseInvoiceLineRepository,
           IChartOfAccountAppService chartOfAccountAppService,
           INumberSeriesGenerator numberSeriesGenerator,
           IJournalAppService journalAppService
           )
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (purchaseInvoiceRepository == null)
                throw new ArgumentNullException(nameof(purchaseInvoiceRepository));

            if (purchaseInvoiceLineRepository == null)
                throw new ArgumentNullException(nameof(purchaseInvoiceLineRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (numberSeriesGenerator == null)
                throw new ArgumentNullException(nameof(numberSeriesGenerator));

            _dbContextScopeFactory = dbContextScopeFactory;
            _purchaseInvoiceRepository = purchaseInvoiceRepository;
            _purchaseInvoiceLineRepository = purchaseInvoiceLineRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _numberSeriesGenerator = numberSeriesGenerator;
            _journalAppService = journalAppService;
        }



        public PurchaseInvoiceDTO AddNewPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, ServiceHeader serviceHeader)
        {
            if (purchaseInvoiceDTO != null)
            {

                var purchaseInvoiceNo = _numberSeriesGenerator.GetNextNumber("PI", serviceHeader);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                   
                    var purchaseInvoice = PurchaseInvoiceFactory.CreatePurchaseInvoice(purchaseInvoiceNo, purchaseInvoiceDTO.VendorNo, purchaseInvoiceDTO.VendorName, purchaseInvoiceDTO.VendorAddress, purchaseInvoiceDTO.DocumentDate, purchaseInvoiceDTO.PostingDate, purchaseInvoiceDTO.DueDate, purchaseInvoiceDTO.ApprovalStatus, purchaseInvoiceDTO.PaidAmount, purchaseInvoiceDTO.RemainingAmount, purchaseInvoiceDTO.TotalAmount,  serviceHeader);

                    AddLines(purchaseInvoiceDTO, purchaseInvoice, serviceHeader);

                    purchaseInvoice.CreatedBy = serviceHeader.ApplicationUserName;

                    _purchaseInvoiceRepository.Add(purchaseInvoice, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return purchaseInvoice.ProjectedAs<PurchaseInvoiceDTO>();
                }
            }
            else return null;
        }

        public bool UpdatePurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, ServiceHeader serviceHeader)
        {
            //if (purchaseInvoiceDTO == null || purchaseInvoiceDTO.Id == Guid.Empty)
            //    return false;

            if (purchaseInvoiceDTO == null || purchaseInvoiceDTO.Id == Guid.Empty)
                return false;

            //var purchaseInvoiceNo = _numberSeriesGenerator.GetNextNumber("PI", serviceHeader);

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _purchaseInvoiceRepository.Get(purchaseInvoiceDTO.Id, serviceHeader);

                if (persisted != null)
                {
                   
                    var current = PurchaseInvoiceFactory.CreatePurchaseInvoice(persisted.No, purchaseInvoiceDTO.VendorNo, purchaseInvoiceDTO.VendorName, purchaseInvoiceDTO.VendorAddress, purchaseInvoiceDTO.DocumentDate, purchaseInvoiceDTO.PostingDate, purchaseInvoiceDTO.DueDate, purchaseInvoiceDTO.ApprovalStatus, purchaseInvoiceDTO.PaidAmount, purchaseInvoiceDTO.RemainingAmount, purchaseInvoiceDTO.TotalAmount, serviceHeader);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _purchaseInvoiceRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }


        public void AddLines (PurchaseInvoiceDTO purchaseInvoiceDTO, PurchaseInvoice purchaseInvoice, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();



            if (purchaseInvoice == null || purchaseInvoice.IsTransient())
                sbErrors.Append("Purchase Invoice is either null or in transient state! ");

            if (purchaseInvoice.Id == null || purchaseInvoice.Id == Guid.Empty)
                sbErrors.Append("Purchase Invoice Id is null or empty!");

 
            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
             
                if (purchaseInvoiceDTO.PurchaseInvoiceLines != null && purchaseInvoiceDTO.PurchaseInvoiceLines.Any())
                {
                    foreach (var item in purchaseInvoiceDTO.PurchaseInvoiceLines) {

                        purchaseInvoice.AddLine(item.Type, item.No, item.Description, item.UnitCost, item.Quantity, item.Amount, item.DebitChartOfAccountId, serviceHeader);
                    }
                }
            }
        }


        public List<PurchaseInvoiceDTO> FindPurchaseInvoices(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var purchaseInvoices = _purchaseInvoiceRepository.GetAll(serviceHeader);

                if (purchaseInvoices != null && purchaseInvoices.Any())
                {
                    return purchaseInvoices.ProjectedAsCollection<PurchaseInvoiceDTO>();
                }
                else return null;
            }
        }


        public PurchaseInvoiceDTO FindPurchaseInvoice(Guid purchaseInvoiceId, ServiceHeader serviceHeader)
        {

            using (_dbContextScopeFactory.CreateReadOnly())
            {

                var purchaseInvoice = _purchaseInvoiceRepository.Get(purchaseInvoiceId, serviceHeader);

                if (purchaseInvoice != null)
                {
                    return purchaseInvoice.ProjectedAs<PurchaseInvoiceDTO>();
                }

                else
                {
                    return null;
                }
            }
        }


        public List<PurchaseInvoiceLineDTO> FindPurchaseInvoiceLines(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var purchaseInvoiceLines = _purchaseInvoiceLineRepository.GetAll(serviceHeader);

                if (purchaseInvoiceLines != null && purchaseInvoiceLines.Any())
                {
                    return purchaseInvoiceLines.ProjectedAsCollection<PurchaseInvoiceLineDTO>();
                }
                else return null;
            }
        }



        public JournalDTO PostPurchaseInvoice(PurchaseInvoiceDTO purchaseInvoiceDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (purchaseInvoiceDTO == null || !purchaseInvoiceDTO.PurchaseInvoiceLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            var payablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
                (int)SystemGeneralLedgerAccountCode.AccountPayables, serviceHeader);

            if (payablesChartOfAccountId == Guid.Empty)
            {
                throw new InvalidOperationException("Sorry, but the requisite payables chart of account has not been setup!");
            }

            var purchaseInvoiceLineDTOs = purchaseInvoiceDTO.PurchaseInvoiceLines;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted invoice once, outside the loop
                var persisted = _purchaseInvoiceRepository.Get(purchaseInvoiceDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("Purchase invoice not found!");
                }

                // Process each purchase invoice line
                foreach (var item in purchaseInvoiceLineDTOs)
                {
                    var journal = _journalAppService.AddNewJournal(
                        purchaseInvoiceDTO.BranchId,
                        null,
                        item.Amount,
                        string.Format("Purchase Invoice~{0}", item.No),
                        purchaseInvoiceDTO.BankBranchName,
                        item.No.ToString(),
                        moduleNavigationItemCode,
                        (int)SystemTransactionCode.InterAcccountTransfer,
                        null,
                        payablesChartOfAccountId,
                        item.DebitChartOfAccountId,
                        serviceHeader);

                    if (journal == null)
                    {
                        throw new InvalidOperationException($"Failed to create journal for purchase invoice line {item.No}");
                    }

                    lastCreatedJournal = journal;
                }

                // Mark the purchase invoice as posted in both DTO and persisted entity
                purchaseInvoiceDTO.Posted = true;
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


        public JournalDTO PayVendorInvoice(PaymentVoucherDTO paymentVoucherDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (paymentVoucherDTO == null)
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            var purchaseInvoice = FindPurchaseInvoices(serviceHeader).FirstOrDefault(p => p.Id == paymentVoucherDTO.InvoiceId);


            var payablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
                (int)SystemGeneralLedgerAccountCode.AccountPayables, serviceHeader);

            if (payablesChartOfAccountId == Guid.Empty)
            {
                throw new InvalidOperationException("Sorry, but the requisite payables chart of account has not been setup!");
            }

            //var purchaseInvoiceLineDTOs = purchaseInvoiceDTO.PurchaseInvoiceLines;

            paymentVoucherDTO.VoucherNumber = Guid.NewGuid();
           


            //paymentVoucherDTO.VoucherNumber = new Guid();
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {

                if (purchaseInvoice != null && purchaseInvoice.Posted == true)
                {
                    var journal = _journalAppService.AddNewJournal(
                        purchaseInvoice.BranchId,
                        null,
                        paymentVoucherDTO.Amount,
                        string.Format("Payment Voucher~{0}", paymentVoucherDTO.VoucherNumber),
                        purchaseInvoice.BankBranchName,
                        purchaseInvoice.No.ToString(),
                        moduleNavigationItemCode,
                        (int)SystemTransactionCode.InterAcccountTransfer,
                        null,
                        paymentVoucherDTO.BankLinkageChartOfAccountId,
                        payablesChartOfAccountId,
                        serviceHeader);

                    if (journal == null)
                    {
                        throw new InvalidOperationException($"Failed to create journal for Payment Voucher No {paymentVoucherDTO.VoucherNumber}");
                    }


                    lastCreatedJournal = journal;


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

                else
                {
                    throw new InvalidOperationException("A Purchase Invoice Has Not been Posted");
                }
      
            }
        }


    }
}
