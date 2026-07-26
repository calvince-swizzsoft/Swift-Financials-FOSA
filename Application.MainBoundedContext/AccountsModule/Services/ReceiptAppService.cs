using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.ReceiptLineAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Adapter;
using Infrastructure.Crosscutting.Framework.Utils;
using NPOI.Util;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AccountsModule.Services
{
    public class ReceiptAppService : IReceiptAppService
    {

        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Receipt> _receiptRepository;
        private readonly IRepository<ReceiptLine> _receiptLineRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;
        private readonly ISalesInvoiceAppService _salesInvoiceAppService;
        private readonly ICashDepositRequestAppService _cashDepositRequestAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;


        public ReceiptAppService(
         IDbContextScopeFactory dbContextScopeFactory,
         IRepository<Receipt> receiptRepository,
         IRepository<ReceiptLine> receiptLineRepository,
         IChartOfAccountAppService chartOfAccountAppService,
         IJournalAppService journalAppService,
         INumberSeriesGenerator numberSeriesGenerator,
         ISalesInvoiceAppService salesInvoiceAppService,
         ICashDepositRequestAppService cashDepositRequestAppService, 
         ICustomerAccountAppService customerAccountAppService
         )
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (receiptRepository == null)
                throw new ArgumentNullException(nameof(receiptRepository));

            if (receiptLineRepository == null)
                throw new ArgumentNullException(nameof(receiptLineRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (salesInvoiceAppService == null)
                throw new ArgumentNullException(nameof(salesInvoiceAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));



            if (cashDepositRequestAppService == null)
                throw new ArgumentNullException(nameof(cashDepositRequestAppService));


            if (numberSeriesGenerator == null)
                throw new ArgumentNullException(nameof(numberSeriesGenerator));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));



            _dbContextScopeFactory = dbContextScopeFactory;
            _receiptRepository = receiptRepository;
            _receiptLineRepository = receiptLineRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _journalAppService = journalAppService;
            _numberSeriesGenerator = numberSeriesGenerator;
            _salesInvoiceAppService = salesInvoiceAppService;
            _cashDepositRequestAppService = cashDepositRequestAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        public ReceiptDTO AddNewReceipt(ReceiptDTO receiptDTO, ServiceHeader serviceHeader)
        {
            if (receiptDTO != null)
            {

                //purchaseIn

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {

                    var receiptNo = _numberSeriesGenerator.GetNextNumber("RCPT", serviceHeader);

                    var receipt = ReceiptFactory.CreateReceipt(receiptDTO.InvoiceId, receiptDTO.CustomerId, receiptDTO.Description, receiptDTO.Reference, receiptDTO.TotalAmount, receiptDTO.PaymentMethod, receiptDTO.BankLinkageChartOfAccountId, receiptNo, receiptDTO.InvoiceNo, receiptDTO.CustomerNo);

                    AddLines(receiptDTO, receipt, serviceHeader);

                    receipt.CreatedBy = serviceHeader.ApplicationUserName;

                    _receiptRepository.Add(receipt, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return receipt.ProjectedAs<ReceiptDTO>();
                }
            }
            else return null;
        }

        public bool UpdateReceipt(ReceiptDTO receiptDTO, ServiceHeader serviceHeader)
        {
            //if (purchaseInvoiceDTO == null || purchaseInvoiceDTO.Id == Guid.Empty)
            //    return false;

            if (receiptDTO == null || receiptDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _receiptRepository.Get(receiptDTO.Id, serviceHeader);

                if (persisted != null)
                {

                    //var current = PurchaseInvoiceFactory.CreatePayment(paymentDTO.VendorNo, purchaseInvoiceDTO.VendorName, purchaseInvoiceDTO.VendorAddress, purchaseInvoiceDTO.DocumentDate, purchaseInvoiceDTO.PostingDate, purchaseInvoiceDTO.DueDate, purchaseInvoiceDTO.ApprovalStatus, serviceHeader);

                    var receiptNo = _numberSeriesGenerator.GetNextNumber("RCPT", serviceHeader);
                    var current = ReceiptFactory.CreateReceipt(receiptDTO.InvoiceId, receiptDTO.CustomerId, receiptDTO.Description, receiptDTO.Reference, receiptDTO.TotalAmount, receiptDTO.PaymentMethod, receiptDTO.BankLinkageChartOfAccountId, receiptNo,  receiptDTO.InvoiceNo, receiptDTO.CustomerNo);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _receiptRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }


        public void AddLines(ReceiptDTO receiptDTO, Receipt receipt, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();

            if (receipt == null || receipt.IsTransient())
                sbErrors.Append("Receipt is either null or in transient state! ");

            if (receipt.Id == null || receipt.Id == Guid.Empty)
                sbErrors.Append("Receipt Id is null or empty!");


            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform double-entry operations to in-memory Domain-Model objects
                // 

                if (receiptDTO.ReceiptLines != null && receiptDTO.ReceiptLines.Any())
                {
                    foreach (var item in receiptDTO.ReceiptLines)
                    {

                        receipt.AddLine(item.AccountType, item.CustomerAccountId, item.CustomerAccountNo, item.Description, item.Amount, item.CreditChartOfAccountId, item.AccountType, item.DocumentType, serviceHeader);
                    }
                }
            }
        }


        public List<ReceiptDTO> FindReceipts(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var receipts = _receiptRepository.GetAll(serviceHeader);

                if (receipts != null && receipts.Any())
                {
                    return receipts.ProjectedAsCollection<ReceiptDTO>();
                }
                else return null;
            }
        }


        public List<ReceiptLineDTO> FindReceiptLines(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var receiptLines = _receiptLineRepository.GetAll(serviceHeader);

                if (receiptLines != null && receiptLines.Any())
                {
                    return receiptLines.ProjectedAsCollection<ReceiptLineDTO>();
                }
                else return null;
            }
        }






        public JournalDTO PostReceipt(ReceiptDTO receiptDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (receiptDTO == null || !receiptDTO.ReceiptLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            //only create receipt when it is not already present
           var CreatedReceipt = AddNewReceipt(receiptDTO, serviceHeader);



            var receiptLineDTOs = receiptDTO.ReceiptLines;
            receiptDTO.Id = CreatedReceipt.Id;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted payment once, outside the loop
                var persisted = _receiptRepository.Get(receiptDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("Receipt not found!");
                }

                // Process each purchase invoice line
                foreach (var item in receiptLineDTOs)
                {

                    if (item.AccountType == (int)ReceiptAccountType.Customer && item.DocumentType == (int)ReceiptDocumentType.Invoice)
                    {

                        var receivablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
              (int)SystemGeneralLedgerAccountCode.AccountPayables, serviceHeader);

                        if (receivablesChartOfAccountId == Guid.Empty)
                        {
                            throw new InvalidOperationException("Sorry, but the requisite receivables chart of account has not been setup!");
                        }

                        var journal = _journalAppService.AddNewJournal(
                            receiptDTO.BranchId,
                            null,
                            item.Amount,
                            string.Format("Receipt~{0}", receiptDTO.ReceiptNumber),
                            receiptDTO.BankBranchName,
                            item.CustomerAccountNo.ToString(),
                            moduleNavigationItemCode,
                            (int)SystemTransactionCode.InterAcccountTransfer,
                            null,
                            persisted.BankLinkageChartOfAccountId,
                            receivablesChartOfAccountId,
                            //item.ChartOfAccountId,
                            serviceHeader);

                        if (journal == null)
                        {
                            throw new InvalidOperationException($"Failed to create journal for purchase invoice line {item.CustomerAccountNo}");
                        }

                        lastCreatedJournal = journal;

                        var salesInvoiceDTO = _salesInvoiceAppService.FindSalesInvoice(receiptDTO.InvoiceId, serviceHeader);

                        salesInvoiceDTO.RemainingAmount -= item.Amount;
                        salesInvoiceDTO.PaidAmount += item.Amount;

                        _salesInvoiceAppService.UpdateSalesInvoice(salesInvoiceDTO, serviceHeader);


                    }
                }

                // Mark the purchase invoice as posted in both DTO and persisted entity
                receiptDTO.Posted = true;
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



        public JournalDTO PostCustomerReceipt(ReceiptDTO receiptDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (receiptDTO == null || !receiptDTO.ReceiptLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            //only create receipt when it is not already present
            var CreatedReceipt = AddNewReceipt(receiptDTO, serviceHeader);



            var receiptLineDTOs = receiptDTO.ReceiptLines;
            receiptDTO.Id = CreatedReceipt.Id;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted payment once, outside the loop
                var persisted = _receiptRepository.Get(receiptDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("Receipt not found!");
                }

                // Process each purchase invoice line
                foreach (var item in receiptLineDTOs)
                {

                    if (item.AccountType == (int)ReceiptAccountType.Customer)
                    {

                        var receivablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
              (int)SystemGeneralLedgerAccountCode.AccountPayables, serviceHeader);


                        CustomerAccountDTO customerAccountDTO = _customerAccountAppService.FindCustomerAccountDTO(item.CustomerAccountId, serviceHeader);



                        if (customerAccountDTO == null)
                        {
                            throw new InvalidOperationException("Sorry, but the provided account id is not linked to an existing account!");
                        }


                        var journal = _journalAppService.AddNewJournal(
                            receiptDTO.BranchId,
                            null,
                            item.Amount,
                            string.Format("Receipt~{0}", item.CustomerAccountNo),
                            item.CustomerAccountNo.ToString(),
                            receiptDTO.Reference,
                            moduleNavigationItemCode,
                            (int)SystemTransactionCode.CashDeposit,
                            DateTime.Today,
                            item.CreditChartOfAccountId,
                            receiptDTO.BankLinkageChartOfAccountId,
                            customerAccountDTO,
                            null,
                            serviceHeader,
                            false);

      
                        if (journal == null)
                        {
                            throw new InvalidOperationException($"Failed to create journal for Customer Account line {item.CustomerAccountNo}");
                        }

                        lastCreatedJournal = journal;

                    }
                }

                // Mark the purchase invoice as posted in both DTO and persisted
                receiptDTO.Posted = true;
                persisted.Posted = true;

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

