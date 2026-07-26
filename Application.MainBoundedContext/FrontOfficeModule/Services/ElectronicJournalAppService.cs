using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.Services;
using Application.Seedwork;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ElectronicJournalAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.TruncatedChequeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using FileHelpers;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Application.MainBoundedContext.FrontOfficeModule.Services
{
    public class ElectronicJournalAppService : IElectronicJournalAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<ElectronicJournal> _electronicJournalRepository;
        private readonly IRepository<TruncatedCheque> _truncatedChequeRepository;
        private readonly IMediaAppService _mediaAppService;
        private readonly IChequeBookAppService _chequeBookAppService;
        private readonly IUnPayReasonAppService _unPayReasonAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly IChartOfAccountAppService _chartOfAccountAppService;
        private readonly ICustomerAccountAppService _customerAccountAppService;

        public ElectronicJournalAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<ElectronicJournal> electronicJournalRepository,
           IRepository<TruncatedCheque> truncatedChequeRepository,
           IMediaAppService mediaAppService,
           IChequeBookAppService chequeBookAppService,
           IUnPayReasonAppService unPayReasonAppService,
           ICommissionAppService commissionAppService,
           IJournalAppService journalAppService,
           IChartOfAccountAppService chartOfAccountAppService,
           ICustomerAccountAppService customerAccountAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (electronicJournalRepository == null)
                throw new ArgumentNullException(nameof(electronicJournalRepository));

            if (truncatedChequeRepository == null)
                throw new ArgumentNullException(nameof(truncatedChequeRepository));

            if (mediaAppService == null)
                throw new ArgumentNullException(nameof(mediaAppService));

            if (chequeBookAppService == null)
                throw new ArgumentNullException(nameof(chequeBookAppService));

            if (unPayReasonAppService == null)
                throw new ArgumentNullException(nameof(unPayReasonAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (chartOfAccountAppService == null)
                throw new ArgumentNullException(nameof(chartOfAccountAppService));

            if (customerAccountAppService == null)
                throw new ArgumentNullException(nameof(customerAccountAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _electronicJournalRepository = electronicJournalRepository;
            _truncatedChequeRepository = truncatedChequeRepository;
            _mediaAppService = mediaAppService;
            _chequeBookAppService = chequeBookAppService;
            _unPayReasonAppService = unPayReasonAppService;
            _commissionAppService = commissionAppService;
            _journalAppService = journalAppService;
            _chartOfAccountAppService = chartOfAccountAppService;
            _customerAccountAppService = customerAccountAppService;
        }

        public ElectronicJournalDTO ParseElectronicJournalImport(string fileDirectory, string fileName, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            if (!string.IsNullOrWhiteSpace(fileDirectory) && !string.IsNullOrWhiteSpace(fileName))
            {
                var path = Path.Combine(fileDirectory, fileName);

                if (System.IO.File.Exists(path))
                {
                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        // get the specification
                        var filter = ElectronicJournalSpecifications.ElectronicJournalWithFileName(fileName);

                        ISpecification<ElectronicJournal> spec = filter;

                        //Query this criteria
                        var electronicJournals = _electronicJournalRepository.AllMatching(spec, serviceHeader);

                        if (electronicJournals != null && electronicJournals.Any())
                            throw new InvalidOperationException(string.Format("Sorry, but an electronic journal with file name '{0}' already exists!", fileName));
                        else
                        {
                            var tuple = KBACTS.Parse(path);

                            if (tuple.Item2 == null || !tuple.Item2.Any())
                                throw new InvalidOperationException(string.Format("Sorry, but the file could not be parsed."));

                            var electronicJournalDTO = new ElectronicJournalDTO
                            {
                                FileName = fileName,
                                HeaderRecordRecordType = tuple.Item1.RecordType,
                                HeaderRecordDateOfFileExchange = tuple.Item1.DateOfFileExchange,
                                HeaderRecordFileSerialNumber = tuple.Item1.FileSerialNumber,
                                HeaderRecordFileType = tuple.Item1.FileType,
                                HeaderRecordFiller = tuple.Item1.Filler,
                                HeaderRecordLastFileIndicator = tuple.Item1.LastFileIndicator,
                                HeaderRecordPresentingOrganisation = tuple.Item1.PresentingOrganisation,
                                HeaderRecordPresentingOrganisationBank = tuple.Item1.PresentingOrganisationBank,
                                HeaderRecordPresentingOrganisationClearingCentre = tuple.Item1.PresentingOrganisationClearingCentre,
                                HeaderRecordReceivingOrganisation = tuple.Item1.ReceivingOrganisation,
                                HeaderRecordReceivingOrganisationBank = tuple.Item1.ReceivingOrganisationBank,
                                HeaderRecordReceivingOrganisationClearingCentre = tuple.Item1.ReceivingOrganisationClearingCentre,
                                TrailerRecordBank = tuple.Item3.Bank,
                                TrailerRecordClearingCentre = tuple.Item3.ClearingCentre,
                                TrailerRecordOrganisation = tuple.Item3.Organisation,
                                TrailerRecordRecordType = tuple.Item3.RecordType,
                                TrailerRecordTotalValueCredits = tuple.Item3.TotalValueCredits,
                                TrailerRecordTotalValueDebits = tuple.Item3.TotalValueDebits,
                                TrailerRecordFiller = tuple.Item3.Filler,
                                TrailerRecordTransactionCount = tuple.Item3.TransactionCount
                            };

                            var headerRecord = new HeaderRecord(electronicJournalDTO.HeaderRecordRecordType, electronicJournalDTO.HeaderRecordFileType, electronicJournalDTO.HeaderRecordDateOfFileExchange, electronicJournalDTO.HeaderRecordPresentingOrganisationClearingCentre, electronicJournalDTO.HeaderRecordPresentingOrganisationBank, electronicJournalDTO.HeaderRecordPresentingOrganisation, electronicJournalDTO.HeaderRecordReceivingOrganisationClearingCentre, electronicJournalDTO.HeaderRecordReceivingOrganisationBank, electronicJournalDTO.HeaderRecordReceivingOrganisation, electronicJournalDTO.HeaderRecordFileSerialNumber, electronicJournalDTO.HeaderRecordLastFileIndicator, electronicJournalDTO.HeaderRecordFiller);

                            var trailerRecord = new TrailerRecord(electronicJournalDTO.TrailerRecordRecordType, electronicJournalDTO.TrailerRecordClearingCentre, electronicJournalDTO.TrailerRecordBank, electronicJournalDTO.TrailerRecordOrganisation, electronicJournalDTO.TrailerRecordTransactionCount, electronicJournalDTO.TrailerRecordTotalValueCredits, electronicJournalDTO.TrailerRecordTotalValueDebits, electronicJournalDTO.TrailerRecordFiller);

                            var electronicJournal = ElectronicJournalFactory.CreateElectronicJournal(electronicJournalDTO.FileName, headerRecord, trailerRecord);

                            electronicJournal.Status = (int)ElectronicJournalStatus.Open;
                            electronicJournal.CreatedBy = serviceHeader.ApplicationUserName;

                            _electronicJournalRepository.Add(electronicJournal, serviceHeader);

                            tuple.Item2.ForEach(item =>
                            {
                                var truncatedChequeDTO = new TruncatedChequeDTO
                                {
                                    ElectronicJournalId = electronicJournal.Id,
                                    CollectionAccountDetail = item.CollectionAccountDetail,
                                    AmountEntryMethod = item.AmountEntryMethod,
                                    DestinationAccountAccount = item.DestinationAccountAccount,
                                    DestinationAccountBank = item.DestinationAccountBank,
                                    DestinationAccountBranch = item.DestinationAccountBranch,
                                    DestinationAccountCheckDigit = item.DestinationAccountCheckDigit,
                                    DestinationAccountCurrencyCode = item.DestinationAccountCurrencyCode,
                                    DocumentReferenceNumber = item.DocumentReferenceNumber,
                                    Filler = item.Filler,
                                    PresentingBank = item.PresentingBank,
                                    PresentingBranch = item.PresentingBranch,
                                    ReasonForReturnCode = (int)TruncatedChequeReturnCode.One, //int.Parse(item.ReasonForReturnCode),
                                    SerialNumber = item.SerialNumber,
                                    Value = item.Value,
                                    VoucherTypeCode = item.VoucherTypeCode,
                                    FrontImage1Size = item.FrontImage1Size,
                                    FrontImage1Signature = item.FrontImage1Signature,
                                    FrontImage1Buffer = item.FrontImage1,
                                    FrontImage2Size = item.FrontImage2Size,
                                    FrontImage2Signature = item.FrontImage2Signature,
                                    FrontImage2Buffer = item.FrontImage2,
                                    RearImageSize = item.RearImageSize,
                                    RearImageSignature = item.RearImageSignature,
                                    RearImageBuffer = item.RearImage,
                                };

                                var voucherNumber = -1;
                                int.TryParse(item.SerialNumber, out voucherNumber);

                                var paymentVouchers = _chequeBookAppService.FindPaymentVouchersByVoucherNumberAndChequeBookReference((int)ChequeBookType.External, voucherNumber, item.DestinationAccountAccount, serviceHeader);

                                if (paymentVouchers != null && paymentVouchers.Any())
                                {
                                    if (paymentVouchers.Count == 1)
                                    {
                                        var targetVoucher = paymentVouchers[0];

                                        switch ((PaymentVoucherStatus)targetVoucher.Status)
                                        {
                                            case PaymentVoucherStatus.Active:
                                                truncatedChequeDTO.PaymentVoucherId = targetVoucher.Id;
                                                break;
                                            case PaymentVoucherStatus.Paid:
                                            case PaymentVoucherStatus.Flagged:
                                            default:
                                                truncatedChequeDTO.Remarks = targetVoucher.StatusDescription;
                                                break;
                                        }
                                    }
                                }

                                AddNewTruncatedCheque(truncatedChequeDTO, fileDirectory, blobDatabaseConnectionString, serviceHeader);
                            });

                            dbContextScope.SaveChanges(serviceHeader);

                            return electronicJournal.ProjectedAs<ElectronicJournalDTO>();
                        }
                    }
                }
                else return null;
            }
            else return null;
        }

        public bool CloseElectronicJournal(ElectronicJournalDTO electronicJournalDTO, string encryptionPublicKeyPath, string encryptionPrivateKeyPath, string encryptionPassPhrase, string fileExportDirectory, ServiceHeader serviceHeader)
        {
            if (electronicJournalDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _electronicJournalRepository.Get(electronicJournalDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)ElectronicJournalStatus.Open)
                    {
                        var itemsCount = _truncatedChequeRepository.AllMatchingCount(TruncatedChequeSpecifications.TruncatedChequeFullText(persisted.Id, null), serviceHeader);

                        var processedItemsCount = _truncatedChequeRepository.AllMatchingCount(TruncatedChequeSpecifications.ProcessedTruncatedChequeWithElectronicJournalId(persisted.Id), serviceHeader);

                        if (processedItemsCount == itemsCount)
                        {
                            var unpaidChequeRecords = _truncatedChequeRepository.AllMatching(TruncatedChequeSpecifications.UnPaidTruncatedChequeWithElectronicJournalId(persisted.Id), serviceHeader);

                            if (unpaidChequeRecords != null && unpaidChequeRecords.Any())
                            {
                                var extracts = from c in unpaidChequeRecords
                                               select new ChequeRecordExtract
                                               {
                                                   DestinationAccountAccount = c.DestinationAccountAccount,
                                                   SerialNumber = c.SerialNumber,
                                                   SortCode = string.Format("{0}{1}{2}", c.DestinationAccountBank, c.DestinationAccountBranch, c.DestinationAccountCheckDigit),
                                                   Value = c.Value,
                                                   VoucherTypeCode = c.VoucherTypeCode,
                                                   PostingDate = DateTime.Today.ToString("yyyy/MM/dd"),
                                                   ProcessingDate = DateTime.Today.ToString("yyyy/MM/dd"),
                                                   Indicator = "UNPAID",
                                                   UnPaidReason = c.UnPaidReason,
                                                   UnPaidCode = c.UnPaidCode,
                                                   PresentingBank = c.PresentingBank,
                                                   CurrencyCode = "00",
                                                   Session = "01",
                                                   BankNumber = c.DestinationAccountBank,
                                                   BranchNumber = c.DestinationAccountBranch,
                                                   SaccoAccountNumber = c.CollectionAccountDetail.GetLast(14)
                                               };

                                var delimitedFileEngine = new DelimitedFileEngine<ChequeRecordExtract>()
                                {
                                    HeaderText = "Account No,Serial No,Sort Code,Amount,Voucher Type,Posting Date,Processing Date,Indicator,UnPaid Reason,UnPaid Code,Presenting Bank,Currency Code,Session,Bank No,Branch No,Sacco AccountNo"//typeof(ChequeRecordExtract).GetCsvHeader()
                                };

                                var pgpEncryptionKeys = new PgpEncryptionKeys(encryptionPublicKeyPath, encryptionPrivateKeyPath, encryptionPassPhrase);

                                var pgpEncrypt = new PgpEncrypt(pgpEncryptionKeys);

                                var fileName = persisted.FileName.Substring(1, 14);

                                var fileTimeStamp = DateTime.Now.ToString("yyyyMMdd");

                                var fileToEncrypt = string.Format(@"{0}\{1}{2}.PRN", fileExportDirectory, fileName, fileTimeStamp).ToLower();

                                var encryptedFileName = string.Format(@"{0}\{1}{2}.GPG", fileExportDirectory, fileName, fileTimeStamp).ToLower();

                                delimitedFileEngine.WriteFile(fileToEncrypt, extracts);

                                using (var outputStream = System.IO.File.Create(encryptedFileName))
                                {
                                    pgpEncrypt.EncryptAndSign(outputStream, new FileInfo(fileToEncrypt));
                                }
                            }

                            persisted.Status = (int)ElectronicJournalStatus.Closed;
                            persisted.ClosedBy = serviceHeader.ApplicationUserName;
                            persisted.ClosedDate = DateTime.Now;

                            return dbContextScope.SaveChanges(serviceHeader) >= 0;
                        }
                        else return false;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool ClearTruncatedCheque(TruncatedChequeDTO truncatedChequeDTO, ServiceHeader serviceHeader)
        {
            if (truncatedChequeDTO != null)
            {
                var truncatedChequesSettlementChartOfAccountId = _chartOfAccountAppService.GetChartOfAccountMappingForSystemGeneralLedgerAccountCode((int)SystemGeneralLedgerAccountCode.TruncatedChequesSettlement, serviceHeader);

                if (truncatedChequesSettlementChartOfAccountId == Guid.Empty)
                    return false;

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var unPayReason = _unPayReasonAppService.FindCachedUnPayReason(truncatedChequeDTO.UnPayReasonId, serviceHeader);

                    var customerAccount = _customerAccountAppService.FindCustomerAccountDTO(truncatedChequeDTO.PaymentVoucherChequeBookCustomerAccountId, serviceHeader);

                    var persisted = _truncatedChequeRepository.Get(truncatedChequeDTO.Id, serviceHeader);

                    if (unPayReason != null && customerAccount != null && persisted != null && persisted.Status == (int)TruncatedChequeStatus.New)
                    {
                        var tariffs = _commissionAppService.ComputeTariffsByUnPayReason(unPayReason.Id, truncatedChequeDTO.Value, customerAccount, serviceHeader);

                        switch ((TruncatedChequeProcessingOption)truncatedChequeDTO.TruncatedChequeProcessingOption)
                        {
                            case TruncatedChequeProcessingOption.Pay:

                                _customerAccountAppService.FetchCustomerAccountsProductDescription(new List<CustomerAccountDTO> { customerAccount }, serviceHeader);

                                _journalAppService.AddNewJournal(truncatedChequeDTO.BranchId, null, truncatedChequeDTO.Value, truncatedChequeDTO.SerialNumber, unPayReason.Description, truncatedChequeDTO.DestinationAccountAccount, truncatedChequeDTO.ModuleNavigationItemCode, (int)SystemTransactionCode.TruncatedCheque, null, truncatedChequesSettlementChartOfAccountId, customerAccount.CustomerAccountTypeTargetProductChartOfAccountId, customerAccount, customerAccount, tariffs, serviceHeader, true);

                                break;
                            case TruncatedChequeProcessingOption.UnPay:

                                if (tariffs != null && tariffs.Any())
                                {
                                    tariffs.ForEach(tariff =>
                                    {
                                        _journalAppService.AddNewJournal(truncatedChequeDTO.BranchId, null, tariff.Amount, tariff.Description, string.Format("{0} {1}", truncatedChequeDTO.SerialNumber, unPayReason.Description), truncatedChequeDTO.DestinationAccountAccount, truncatedChequeDTO.ModuleNavigationItemCode, (int)SystemTransactionCode.TruncatedCheque, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, customerAccount, customerAccount, serviceHeader);
                                    });
                                }

                                break;
                            default:
                                break;
                        }

                        var paymentVoucherDTO = persisted.PaymentVoucher.ProjectedAs<PaymentVoucherDTO>();

                        paymentVoucherDTO.Payee = string.Format("{0}{1}", persisted.PresentingBank, persisted.PresentingBranch);
                        paymentVoucherDTO.Amount = persisted.Value;
                        paymentVoucherDTO.WriteDate = persisted.CreatedDate;
                        paymentVoucherDTO.Reference = persisted.DestinationAccountAccount;

                        _chequeBookAppService.PayVoucher(paymentVoucherDTO, serviceHeader);

                        persisted.Status = (byte)TruncatedChequeStatus.Processed;
                        persisted.Remarks = string.Format("{0} {1}", EnumHelper.GetDescription((TruncatedChequeProcessingOption)truncatedChequeDTO.TruncatedChequeProcessingOption), unPayReason.Description);
                        persisted.UnPaidCode = (byte)unPayReason.Code;
                        persisted.UnPaidReason = unPayReason.Description;
                        persisted.ProcessedBy = serviceHeader.ApplicationUserName;
                        persisted.ProcessedDate = DateTime.Now;

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public bool MatchTruncatedChequePaymentVoucher(TruncatedChequeDTO truncatedChequeDTO, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (truncatedChequeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _truncatedChequeRepository.Get(truncatedChequeDTO.Id, serviceHeader);

                    if (persisted != null && persisted.Status == (int)TruncatedChequeStatus.New && persisted.PaymentVoucherId == null)
                    {
                        var voucherNumber = -1;
                        int.TryParse(persisted.SerialNumber, out voucherNumber);

                        var paymentVouchers = _chequeBookAppService.FindPaymentVouchersByVoucherNumberAndChequeBookReference((int)ChequeBookType.External, voucherNumber, persisted.DestinationAccountAccount, serviceHeader);

                        if (paymentVouchers != null && paymentVouchers.Any())
                        {
                            if (paymentVouchers.Count == 1)
                            {
                                var targetVoucher = paymentVouchers[0];

                                switch ((PaymentVoucherStatus)targetVoucher.Status)
                                {
                                    case PaymentVoucherStatus.Active:
                                        persisted.PaymentVoucherId = targetVoucher.Id;
                                        break;
                                    case PaymentVoucherStatus.Paid:
                                    case PaymentVoucherStatus.Flagged:
                                    default:
                                        persisted.Remarks = targetVoucher.StatusDescription;
                                        break;
                                }

                                result = dbContextScope.SaveChanges(serviceHeader) >= 0;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public List<ElectronicJournalDTO> FindElectronicJournals(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var electronicJournals = _electronicJournalRepository.GetAll(serviceHeader);

                if (electronicJournals != null && electronicJournals.Any())
                {
                    return electronicJournals.ProjectedAsCollection<ElectronicJournalDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<ElectronicJournalDTO> FindElectronicJournals(int status, DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = ElectronicJournalSpecifications.ElectronicJournalFullText(status, startDate, endDate, text);

                ISpecification<ElectronicJournal> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var electronicJournalPagedCollection = _electronicJournalRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (electronicJournalPagedCollection != null)
                {
                    var pageCollection = electronicJournalPagedCollection.PageCollection.ProjectedAsCollection<ElectronicJournalDTO>();

                    if (pageCollection != null && pageCollection.Any())
                    {
                        foreach (var item in pageCollection)
                        {
                            item.ItemsCount = _truncatedChequeRepository.AllMatchingCount(TruncatedChequeSpecifications.TruncatedChequeFullText(item.Id, null), serviceHeader);

                            item.ProcessedItemsCount = _truncatedChequeRepository.AllMatchingCount(TruncatedChequeSpecifications.ProcessedTruncatedChequeWithElectronicJournalId(item.Id), serviceHeader);

                            item.ProcessedEntries = string.Format("{0}/{1}", item.ProcessedItemsCount, item.ItemsCount);
                        }
                    }

                    var itemsCount = electronicJournalPagedCollection.ItemsCount;

                    return new PageCollectionInfo<ElectronicJournalDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public ElectronicJournalDTO FindElectronicJournal(Guid electronicJournalId, ServiceHeader serviceHeader)
        {
            if (electronicJournalId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var electronicJournal = _electronicJournalRepository.Get(electronicJournalId, serviceHeader);

                    if (electronicJournal != null)
                    {
                        return electronicJournal.ProjectedAs<ElectronicJournalDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<TruncatedChequeDTO> FindTruncatedCheques(Guid electronicJournalId, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (electronicJournalId != null && electronicJournalId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = TruncatedChequeSpecifications.TruncatedChequeFullText(electronicJournalId, text);

                    ISpecification<TruncatedCheque> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var truncatedChequePagedCollection = _truncatedChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (truncatedChequePagedCollection != null)
                    {
                        var pageCollection = truncatedChequePagedCollection.PageCollection.ProjectedAsCollection<TruncatedChequeDTO>();

                        var itemsCount = truncatedChequePagedCollection.ItemsCount;

                        return new PageCollectionInfo<TruncatedChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<TruncatedChequeDTO> FindTruncatedCheques(Guid electronicJournalId, int status, string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (electronicJournalId != null && electronicJournalId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = TruncatedChequeSpecifications.TruncatedChequeFullText(electronicJournalId, status, text);

                    ISpecification<TruncatedCheque> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var truncatedChequePagedCollection = _truncatedChequeRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (truncatedChequePagedCollection != null)
                    {
                        var pageCollection = truncatedChequePagedCollection.PageCollection.ProjectedAsCollection<TruncatedChequeDTO>();

                        var itemsCount = truncatedChequePagedCollection.ItemsCount;

                        return new PageCollectionInfo<TruncatedChequeDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public TruncatedChequeDTO FindTruncatedCheque(Guid truncatedChequeId, ServiceHeader serviceHeader)
        {
            if (truncatedChequeId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var truncatedCheque = _truncatedChequeRepository.Get(truncatedChequeId, serviceHeader);

                    if (truncatedCheque != null)
                    {
                        return truncatedCheque.ProjectedAs<TruncatedChequeDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        private TruncatedChequeDTO AddNewTruncatedCheque(TruncatedChequeDTO truncatedChequeDTO, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            if (truncatedChequeDTO != null)
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var truncatedCheque = TruncatedChequeFactory.CreateTruncatedCheque(truncatedChequeDTO.ElectronicJournalId, truncatedChequeDTO.PaymentVoucherId, truncatedChequeDTO.VoucherTypeCode, truncatedChequeDTO.Value, truncatedChequeDTO.AmountEntryMethod, truncatedChequeDTO.DestinationAccountBank, truncatedChequeDTO.DestinationAccountBranch, truncatedChequeDTO.DestinationAccountAccount, truncatedChequeDTO.DestinationAccountCheckDigit, truncatedChequeDTO.DestinationAccountCurrencyCode, truncatedChequeDTO.Filler, truncatedChequeDTO.CollectionAccountDetail, truncatedChequeDTO.PresentingBank, truncatedChequeDTO.PresentingBranch, truncatedChequeDTO.SerialNumber, truncatedChequeDTO.DocumentReferenceNumber, truncatedChequeDTO.FrontImage1Size, truncatedChequeDTO.FrontImage1Signature, truncatedChequeDTO.FrontImage2Size, truncatedChequeDTO.FrontImage2Signature, truncatedChequeDTO.RearImageSize, truncatedChequeDTO.RearImageSignature, truncatedChequeDTO.Remarks);

                    truncatedCheque.Status = (int)TruncatedChequeStatus.New;
                    truncatedCheque.CreatedBy = serviceHeader.ApplicationUserName;
                    truncatedCheque.FrontImage1Id = IdentityGenerator.NewSequentialGuid();
                    truncatedCheque.FrontImage2Id = IdentityGenerator.NewSequentialGuid();
                    truncatedCheque.RearImageId = IdentityGenerator.NewSequentialGuid();

                    #region FrontImage1/FrontImage2/RearImage

                    MediaDTO frontImage1MediaDTO = new MediaDTO
                    {
                        SKU = truncatedCheque.FrontImage1Id.Value,
                        FileType = "TruncatedChequeFrontImage1",
                        FileRemarks = truncatedChequeDTO.SerialNumber,
                        ContentType = "image/tiff",
                        Content = truncatedChequeDTO.FrontImage1Buffer,
                    };

                    _mediaAppService.PostImage(frontImage1MediaDTO, fileDirectory, blobDatabaseConnectionString, serviceHeader);

                    MediaDTO frontImage2MediaDTO = new MediaDTO
                    {
                        SKU = truncatedCheque.FrontImage2Id.Value,
                        FileType = "TruncatedChequeFrontImage2",
                        FileRemarks = truncatedChequeDTO.SerialNumber,
                        ContentType = "image/jpeg",
                        Content = truncatedChequeDTO.FrontImage2Buffer,
                    };

                    _mediaAppService.PostImage(frontImage2MediaDTO, fileDirectory, blobDatabaseConnectionString, serviceHeader);

                    MediaDTO rearImageMediaDTO = new MediaDTO
                    {
                        SKU = truncatedCheque.RearImageId.Value,
                        FileType = "TruncatedChequeRearImage",
                        FileRemarks = truncatedChequeDTO.SerialNumber,
                        ContentType = "image/jpeg",
                        Content = truncatedChequeDTO.RearImageBuffer,
                    };

                    _mediaAppService.PostImage(rearImageMediaDTO, fileDirectory, blobDatabaseConnectionString, serviceHeader);

                    #endregion

                    _truncatedChequeRepository.Add(truncatedCheque, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return truncatedCheque.ProjectedAs<TruncatedChequeDTO>();
                }
            }
            else return null;
        }
    }

    [DelimitedRecord(","), IgnoreFirst(1)]
    public class ChequeRecordExtract
    {
        [FieldOrder(1)]
        [FieldTitle("Account No")]
        string _destinationAccountAccount;

        public string DestinationAccountAccount
        {
            get { return _destinationAccountAccount; }
            set { _destinationAccountAccount = value; }
        }

        [FieldOrder(2)]
        [FieldTitle("Serial No")]
        string _serialNumber;

        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        [FieldOrder(3)]
        [FieldTitle("Sort Code")]
        string _sortCode;

        public string SortCode
        {
            get { return _sortCode; }
            set { _sortCode = value; }
        }

        [FieldOrder(4)]
        [FieldTitle("Amount")]
        decimal _value;

        public decimal Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [FieldOrder(5)]
        [FieldTitle("Voucher Type")]
        string _voucherTypeCode;

        public string VoucherTypeCode
        {
            get { return _voucherTypeCode; }
            set { _voucherTypeCode = value; }
        }

        [FieldOrder(6)]
        [FieldTitle("Posting Date")]
        string _postingDate;

        public string PostingDate
        {
            get { return _postingDate; }
            set { _postingDate = value; }
        }

        [FieldOrder(7)]
        [FieldTitle("Processing Date")]
        string _processingDate;

        public string ProcessingDate
        {
            get { return _processingDate; }
            set { _processingDate = value; }
        }

        [FieldOrder(8)]
        [FieldTitle("Indicator")]
        string _indicator;

        public string Indicator
        {
            get { return _indicator; }
            set { _indicator = value; }
        }

        [FieldOrder(9)]
        [FieldTitle("UnPaid Reason")]
        string _unPaidReason;

        public string UnPaidReason
        {
            get { return _unPaidReason; }
            set { _unPaidReason = value; }
        }

        [FieldOrder(10)]
        [FieldTitle("UnPaid Code")]
        int _unPaidCode;
        public int UnPaidCode
        {
            get { return _unPaidCode; }
            set { _unPaidCode = value; }
        }

        [FieldOrder(11)]
        [FieldTitle("Presenting Bank")]
        string _presentingBank;

        public string PresentingBank
        {
            get { return _presentingBank; }
            set { _presentingBank = value; }
        }

        [FieldOrder(12)]
        [FieldTitle("Currency Code")]
        string _currencyCode;

        public string CurrencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; }
        }

        [FieldOrder(13)]
        [FieldTitle("Session")]
        string _session;
        public string Session
        {
            get { return _session; }
            set { _session = value; }
        }

        [FieldOrder(14)]
        [FieldTitle("Bank No")]
        string _bankNumber;

        public string BankNumber
        {
            get { return _bankNumber; }
            set { _bankNumber = value; }
        }

        [FieldOrder(15)]
        [FieldTitle("Branch No")]
        string _branchNumber;

        public string BranchNumber
        {
            get { return _branchNumber; }
            set { _branchNumber = value; }
        }

        [FieldOrder(16)]
        [FieldTitle("Sacco AccountNo")]
        string _saccoAccountNumber;

        public string SaccoAccountNumber
        {
            get { return _saccoAccountNumber; }
            set { _saccoAccountNumber = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldTitleAttribute : Attribute
    {
        public FieldTitleAttribute(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public string Name { get; private set; }
    }

    public static class FileHelpersTypeExtensions
    {
        public static IEnumerable<string> GetFieldTitles(this Type type)
        {
            var fields = from field in type.GetFields(
                BindingFlags.GetField |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance)
                         where field.IsFileHelpersField()
                         select field;

            return from field in fields
                   let attrs = field.GetCustomAttributes(true)
                   let order = attrs.OfType<FieldOrderAttribute>().Single().GetOrder()
                   let title = attrs.OfType<FieldTitleAttribute>().Single().Name
                   orderby order
                   select title;
        }

        public static string GetCsvHeader(this Type type)
        {
            return String.Join(",", type.GetFieldTitles());
        }

        static bool IsFileHelpersField(this FieldInfo field)
        {
            return field.GetCustomAttributes(true)
                .OfType<FieldOrderAttribute>()
                .Any();
        }

        static int GetOrder(this FieldOrderAttribute attribute)
        {
            // Hack cos FieldOrderAttribute.Order is internal (why?)
            var pi = typeof(FieldOrderAttribute)
                .GetProperty("Order",
                    BindingFlags.GetProperty |
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            return (int)pi.GetValue(attribute, null);
        }
    }
}
