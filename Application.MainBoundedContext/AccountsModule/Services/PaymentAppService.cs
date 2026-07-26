using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PaymentLineAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.PurchaseInvoiceLineAgg;
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
    public class PaymentAppService : IPaymentAppService
    {

        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<PaymentLine> _paymentLineRepository;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly INumberSeriesGenerator _numberSeriesGenerator;
        private readonly IPurchaseInvoiceAppService _purchaseInvoiceAppService;


        public PaymentAppService(
         IDbContextScopeFactory dbContextScopeFactory,
         IRepository<Payment> paymentRepository,
         IRepository<PaymentLine> paymentLineRepository,
         IChartOfAccountAppService chartOfAccountAppService,
         IJournalAppService journalAppService,
         INumberSeriesGenerator numberSeriesGenerator, 
         IPurchaseInvoiceAppService purchaseInvoiceAppService
         )
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (paymentRepository == null)
                throw new ArgumentNullException(nameof(paymentRepository));

            if (paymentLineRepository == null)
                throw new ArgumentNullException(nameof(paymentLineRepository));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (purchaseInvoiceAppService == null)
                throw new ArgumentNullException(nameof(purchaseInvoiceAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));


            if (numberSeriesGenerator == null)
                throw new ArgumentNullException(nameof(numberSeriesGenerator));

            _dbContextScopeFactory = dbContextScopeFactory;
            _paymentRepository = paymentRepository;
            _paymentLineRepository = paymentLineRepository;
            _chartOfAccountAppService = chartOfAccountAppService;
            _journalAppService = journalAppService;
            _numberSeriesGenerator = numberSeriesGenerator;
            _purchaseInvoiceAppService = purchaseInvoiceAppService;
        }

        public PaymentDTO AddNewPayment(PaymentDTO paymentDTO, ServiceHeader serviceHeader)
        {
            if (paymentDTO != null)
            {
    
                //purchaseIn
                
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {

                    var voucherNo = _numberSeriesGenerator.GetNextNumber("PV", serviceHeader);
                    
                    var payment = PaymentFactory.CreatePayment(paymentDTO.InvoiceId, paymentDTO.VendorId, paymentDTO.VendorNo, paymentDTO.VendorName, paymentDTO.VendorAddress, paymentDTO.Description, paymentDTO.Reference, paymentDTO.TotalAmount, paymentDTO.PaymentMethod, paymentDTO.BankLinkageChartOfAccountId, voucherNo);

                    AddLines(paymentDTO, payment, serviceHeader);

                    payment.CreatedBy = serviceHeader.ApplicationUserName;

                    _paymentRepository.Add(payment, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return payment.ProjectedAs<PaymentDTO>();
                }
            }
            else return null;
        }

        public bool UpdatePayment(PaymentDTO paymentDTO, ServiceHeader serviceHeader)
        {
            //if (purchaseInvoiceDTO == null || purchaseInvoiceDTO.Id == Guid.Empty)
            //    return false;

            if (paymentDTO == null || paymentDTO.Id == Guid.Empty)
                return false;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = _paymentRepository.Get(paymentDTO.Id, serviceHeader);

                if (persisted != null)
                {

                    //not sure voucherNo should be changed on update
                    var voucherNo = _numberSeriesGenerator.GetNextNumber("PV", serviceHeader);
                    var current = PaymentFactory.CreatePayment(paymentDTO.InvoiceId, paymentDTO.VendorId, paymentDTO.VendorNo, paymentDTO.VendorName, paymentDTO.VendorAddress, paymentDTO.Description, paymentDTO.Reference, paymentDTO.TotalAmount, paymentDTO.PaymentMethod, paymentDTO.BankLinkageChartOfAccountId, voucherNo);

                    current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                    current.CreatedBy = persisted.CreatedBy;

                    _paymentRepository.Merge(persisted, current, serviceHeader);

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
                else return false;
            }
        }


        public void AddLines(PaymentDTO paymentDTO, Payment payment, ServiceHeader serviceHeader)
        {
            StringBuilder sbErrors = new StringBuilder();



            if (payment == null || payment.IsTransient())
                sbErrors.Append("Purchase Invoice is either null or in transient state! ");

            if (payment.Id == null || payment.Id == Guid.Empty)
                sbErrors.Append("Purchase Invoice Id is null or empty!");


            if (sbErrors.Length != 0)
                throw new InvalidOperationException(sbErrors.ToString());
            else
            {
                // Domain Logic
                // Process: Perform double-entry operations to in-memory Domain-Model objects
                // 

                if (paymentDTO.PaymentLines != null && paymentDTO.PaymentLines.Any())
                {
                    foreach (var item in paymentDTO.PaymentLines)
                    {

                        payment.AddLine(item.AccountType, item.No, item.Description, item.Amount, item.ChartOfAccountId, item.AccountType, item.DocumentType, serviceHeader);
                    }
                }
            }
        }


        public List<PaymentDTO> FindPayments(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var payments = _paymentRepository.GetAll(serviceHeader);

                if (payments != null && payments.Any())
                {
                    return payments.ProjectedAsCollection<PaymentDTO>();
                }
                else return null;
            }
        }


        public List<PaymentLineDTO> FindPaymentLines(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var paymentLines = _paymentLineRepository.GetAll(serviceHeader);

                if (paymentLines != null && paymentLines.Any())
                {
                    return paymentLines.ProjectedAsCollection<PaymentLineDTO>();
                }
                else return null;
            }
        }



        public JournalDTO PostPayment(PaymentDTO paymentDTO, int moduleNavigationItemCode, ServiceHeader serviceHeader)
        {
            if (paymentDTO == null || !paymentDTO.PaymentLines.Any())
            {
                throw new InvalidOperationException("Sorry, but the provided data is incorrect!");
            }

            var CreatedPayment = AddNewPayment(paymentDTO, serviceHeader);

          

            var paymentLineDTOs = paymentDTO.PaymentLines;
            paymentDTO.Id = CreatedPayment.Id;
            JournalDTO lastCreatedJournal = null;

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                // Fetch the persisted payment once, outside the loop
                var persisted = _paymentRepository.Get(paymentDTO.Id, serviceHeader);
                if (persisted == null)
                {
                    throw new InvalidOperationException("Payment not found!");
                }

                // Process each purchase invoice line
                foreach (var item in paymentLineDTOs)
                {

                    if (item.AccountType == (int)PaymentAccountType.Vendor && item.DocumentType == (int)PaymentDocumentType.Invoice)
                    {

                        var payablesChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode(
              (int)SystemGeneralLedgerAccountCode.AccountPayables, serviceHeader);

                        if (payablesChartOfAccountId == Guid.Empty)
                        {
                            throw new InvalidOperationException("Sorry, but the requisite payables chart of account has not been setup!");
                        }

                        var journal = _journalAppService.AddNewJournal(
                            paymentDTO.BranchId,
                            null,
                            item.Amount,
                            string.Format("Payment~{0}", item.No),
                            paymentDTO.BankBranchName,
                            item.No.ToString(),
                            moduleNavigationItemCode,
                            (int)SystemTransactionCode.InterAcccountTransfer,
                            null,
                            persisted.BankLinkageChartOfAccountId,
                            payablesChartOfAccountId,
                            //item.ChartOfAccountId,
                            serviceHeader);

                        if (journal == null)
                        {
                            throw new InvalidOperationException($"Failed to create journal for purchase invoice line {item.No}");
                        }

                        lastCreatedJournal = journal;

                         var purchaseInvoiceDTO = _purchaseInvoiceAppService.FindPurchaseInvoice(paymentDTO.InvoiceId, serviceHeader);

                        purchaseInvoiceDTO.RemainingAmount -= item.Amount;
                        purchaseInvoiceDTO.PaidAmount += item.Amount;

                        _purchaseInvoiceAppService.UpdatePurchaseInvoice(purchaseInvoiceDTO, serviceHeader);


                    }
                }

                // Mark the purchase invoice as posted in both DTO and persisted entity
                paymentDTO.Posted = true;
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

