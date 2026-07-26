using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.Services
{
    public class MediaAppService : IMediaAppService
    {
        private readonly IJournalEntryAppService _journalEntryAppService;
        private readonly ISqlCommandAppService _sqlCommandAppService;
        private readonly ICommissionAppService _commissionAppService;
        private readonly ISavingsProductAppService _savingsProductAppService;
        private readonly IJournalAppService _journalAppService;
        private readonly IFinancialsService _financialsService;

        public MediaAppService(
            IJournalEntryAppService journalEntryAppService,
            ISqlCommandAppService sqlCommandAppService,
            ICommissionAppService commissionAppService,
            ISavingsProductAppService savingsProductAppService,
            IJournalAppService journalAppService,
            IFinancialsService financialsService)
        {
            if (journalEntryAppService == null)
                throw new ArgumentNullException(nameof(journalEntryAppService));

            if (sqlCommandAppService == null)
                throw new ArgumentNullException(nameof(sqlCommandAppService));

            if (commissionAppService == null)
                throw new ArgumentNullException(nameof(commissionAppService));

            if (savingsProductAppService == null)
                throw new ArgumentNullException(nameof(savingsProductAppService));

            if (journalAppService == null)
                throw new ArgumentNullException(nameof(journalAppService));

            if (financialsService == null)
                throw new ArgumentNullException(nameof(financialsService));

            _journalEntryAppService = journalEntryAppService;
            _sqlCommandAppService = sqlCommandAppService;
            _commissionAppService = commissionAppService;
            _savingsProductAppService = savingsProductAppService;
            _journalAppService = journalAppService;
            _financialsService = financialsService;
        }

        public MediaDTO GetMedia(Guid sku, string blobDatabaseConnectionString)
        {
            FileDownloadModel file = null;

            if (GetFile(sku, blobDatabaseConnectionString, out file))
            {
                MediaDTO mediaDTO = new MediaDTO
                {
                    SKU = sku,
                    FileName = file.FileName,
                    ContentCoding = file.ContentCoding,
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                };

                using (var reader = new BinaryReader(file.Content))
                {
                    if (mediaDTO.ContentType.Equals("image/tiff", StringComparison.OrdinalIgnoreCase))
                    {
                        mediaDTO.Content = TIFFToJPEG(reader.ReadBytes((int)file.ContentLength));
                        mediaDTO.ContentType = "image/jpeg";
                    }
                    else if (mediaDTO.ContentType.Equals("image/gif", StringComparison.OrdinalIgnoreCase))
                    {
                        mediaDTO.Content = GIFToJPEG(reader.ReadBytes((int)file.ContentLength));
                        mediaDTO.ContentType = "image/jpeg";
                    }
                    else if (mediaDTO.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase))
                    {
                        mediaDTO.Content = PNGToJPEG(reader.ReadBytes((int)file.ContentLength));
                        mediaDTO.ContentType = "image/jpeg";
                    }
                    else mediaDTO.Content = reader.ReadBytes((int)file.ContentLength);
                }

                return mediaDTO;
            }
            else return null;
        }

        public bool PostFile(MediaDTO mediaDTO, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (mediaDTO != null && mediaDTO.SKU != Guid.Empty)
            {
                var path = Path.Combine(fileDirectory, string.Format("{0}", mediaDTO.FileName));

                if (System.IO.File.Exists(path))
                {
                    PostFile(mediaDTO.SKU, path, string.Format("<{0}>-{1}", mediaDTO.FileType, mediaDTO.FileRemarks), blobDatabaseConnectionString, serviceHeader);

                    result = true;
                }
                else
                {
                    var file = new Domain.MainBoundedContext.ValueObjects.Image(mediaDTO.Content ?? new byte[0] { });

                    if (file.Buffer.Length != 0)
                    {
                        var fileName = Path.Combine(fileDirectory, string.Format("{0}.{1}", mediaDTO.SKU.ToString("D"), mediaDTO.FileExtension));

                        using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        {
                            fileStream.Write(file.Buffer, 0, file.Buffer.Length);

                            fileStream.Close();
                        }

                        if (File.Exists(fileName))
                        {
                            PostFile(mediaDTO.SKU, fileName, string.Format("<{0}>-{1}", mediaDTO.FileType, mediaDTO.FileRemarks), blobDatabaseConnectionString, serviceHeader);

                            File.Delete(fileName);

                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public bool PostImage(MediaDTO mediaDTO, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            var result = default(bool);

            if (mediaDTO != null && mediaDTO.SKU != Guid.Empty)
            {
                var image = new Domain.MainBoundedContext.ValueObjects.Image(mediaDTO.Content ?? new byte[0] { });

                if (image.Buffer.Length != 0)
                {
                    var fileExtension = ".jpg";

                    mediaDTO.ContentType = mediaDTO.ContentType ?? "image/jpeg";

                    if (mediaDTO.ContentType.Equals("image/tiff", StringComparison.OrdinalIgnoreCase))
                        fileExtension = ".tif";
                    else if (mediaDTO.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase))
                        fileExtension = ".png";

                    var fileName = Path.Combine(fileDirectory, string.Format("{0}{1}", mediaDTO.SKU.ToString("D"), fileExtension));

                    using (MemoryStream stream = new MemoryStream(image.Buffer))
                    {
                        using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream))
                        {
                            ImageFormat format = ImageFormat.Jpeg;

                            if (mediaDTO.ContentType.Equals("image/tiff", StringComparison.OrdinalIgnoreCase))
                                format = ImageFormat.Tiff;
                            else if (mediaDTO.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase))
                                format = ImageFormat.Png;

                            bitmap.Save(fileName, format);
                        }
                    }

                    if (File.Exists(fileName))
                    {
                        PostFile(mediaDTO.SKU, fileName, string.Format("<{0}>-{1}", mediaDTO.FileType, mediaDTO.FileRemarks), blobDatabaseConnectionString, serviceHeader);

                        File.Delete(fileName);

                        result = true;
                    }
                }
            }

            return result;
        }

        public MediaDTO PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool chargeForPrinting, bool includeInterestStatement, int moduleNavigationItemCode, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            var principalStatementTransactionsList = new PageCollectionInfo<GeneralLedgerTransaction>();

            var interestStatementTransactionsList = new PageCollectionInfo<GeneralLedgerTransaction>();

            if (customerAccountDTO.CustomerAccountTypeProductCode.In((int)ProductCode.Loan) && includeInterestStatement)
            {
                customerAccountDTO.CustomerAccountStatementType = (int)CustomerAccountStatementType.PrincipalStatement;

                principalStatementTransactionsList = _journalEntryAppService.FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, true, serviceHeader);

                customerAccountDTO.CustomerAccountStatementType = (int)CustomerAccountStatementType.InterestStatement;

                interestStatementTransactionsList = _journalEntryAppService.FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, true, serviceHeader);
            }
            else
            {
                customerAccountDTO.CustomerAccountStatementType = (int)CustomerAccountStatementType.PrincipalStatement;

                principalStatementTransactionsList = _journalEntryAppService.FindGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(customerAccountDTO, startDate, endDate, null, (int)JournalEntryFilter.JournalReference, true, serviceHeader);
            }

            var memoryStream = new MemoryStream();

            var document = new Document(PageSize.A4, 0.5f, 0.5f, 80, 36);

            BaseFont arialNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font fontNormal = new Font(arialNormal, 8, Font.NORMAL);
            Font fontBold = new Font(arialNormal, 8, Font.BOLD);
            Font fontH1 = new Font(arialNormal, 10, Font.NORMAL, new BaseColor(255, 255, 255));

            var pdfWriter = PdfWriter.GetInstance(document, memoryStream);

            var userPassword = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}", customerAccountDTO.CustomerReference1).Trim());
            var ownerPassword = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}", customerAccountDTO.PaddedCustomerSerialNumber).Trim());
            var permissions = PdfWriter.ALLOW_PRINTING | PdfWriter.ALLOW_COPY;
            var encryptionType = PdfWriter.ENCRYPTION_AES_128;

            pdfWriter.SetEncryption(userPassword, ownerPassword, permissions, encryptionType);

            pdfWriter.ViewerPreferences = PdfWriter.PageModeUseOutlines;

            var pageEventHandler = new PdfWriterEvents(string.Empty, serviceHeader.ApplicationUserName);
            pdfWriter.PageEvent = pageEventHandler;

            var companyLogoMediaDTO = GetMedia(customerAccountDTO.BranchCompanyId, blobDatabaseConnectionString);

            pageEventHandler.Logo = companyLogoMediaDTO != null ? companyLogoMediaDTO.Content : null;
            pageEventHandler.HeaderFont = new Font(arialNormal, 10, Font.BOLD);
            pageEventHandler.HeaderRow1 = string.Format("{0}, {1} - {2}. {3}", customerAccountDTO.BranchCompanyDescription, customerAccountDTO.BranchCompanyAddressCity, customerAccountDTO.BranchCompanyAddressStreet, customerAccountDTO.BranchCompanyAddressLandLine);
            pageEventHandler.HeaderRow2 = string.Format("{0} - {1}", customerAccountDTO.CustomerFullName, customerAccountDTO.CustomerReference1);

            document.Open();

            PdfPCell blank2ColCell = new PdfPCell();
            blank2ColCell.Colspan = 2;
            blank2ColCell.Border = Rectangle.NO_BORDER;
            blank2ColCell.BackgroundColor = new BaseColor(239, 239, 239);

            // Add statement
            AddDetailedStatementInfo(customerAccountDTO, new Tuple<PageCollectionInfo<GeneralLedgerTransaction>, PageCollectionInfo<GeneralLedgerTransaction>>(principalStatementTransactionsList, interestStatementTransactionsList), includeInterestStatement, document, fontNormal, fontH1, fontBold, blank2ColCell, startDate, endDate);

            // Add Signature Information 
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            AddSignatureInfo(document, fontNormal, fontH1, fontBold, blank2ColCell);

            document.Close();

            if (chargeForPrinting)
            {
                List<TariffWrapper> printingTariffs = null;
                CustomerAccountDTO tariffCustomerAccountDTO = null;

                switch ((ProductCode)customerAccountDTO.CustomerAccountTypeProductCode)
                {
                    case ProductCode.Savings:

                        printingTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(customerAccountDTO.CustomerAccountTypeTargetProductId, (int)SavingsProductKnownChargeType.StatementPrintingCharges, 0m, customerAccountDTO, serviceHeader, pageEventHandler.LastPage);
                        tariffCustomerAccountDTO = customerAccountDTO;

                        break;
                    case ProductCode.Loan:
                    case ProductCode.Investment:

                        var defaultSavingsProduct = _savingsProductAppService.FindDefaultSavingsProduct(serviceHeader);

                        if (defaultSavingsProduct != null)
                        {
                            var customerAccounts = _sqlCommandAppService.FindCustomerAccountsByTargetProductIdAndCustomerId(defaultSavingsProduct.Id, customerAccountDTO.CustomerId, serviceHeader);

                            if (customerAccounts != null && customerAccounts.Any())
                            {
                                foreach (var item in customerAccounts)
                                {
                                    printingTariffs = _commissionAppService.ComputeTariffsBySavingsProduct(defaultSavingsProduct.Id, (int)SavingsProductKnownChargeType.StatementPrintingCharges, 0m, item, serviceHeader, pageEventHandler.LastPage);

                                    tariffCustomerAccountDTO = item;

                                    break;
                                }
                            }
                        }

                        break;
                    default:
                        break;
                }

                if (printingTariffs != null && printingTariffs.Any())
                {
                    printingTariffs.ForEach(tariff =>
                    {
                        _journalAppService.AddNewJournal(customerAccountDTO.BranchId, null, tariff.Amount, tariff.Description, string.Format("{0}~{1}", EnumHelper.GetDescription(SavingsProductKnownChargeType.StatementPrintingCharges), customerAccountDTO.CustomerAccountTypeTargetProductDescription), customerAccountDTO.FullAccountNumber, moduleNavigationItemCode, (int)SystemTransactionCode.StatementFee, null, tariff.CreditGLAccountId, tariff.DebitGLAccountId, tariffCustomerAccountDTO, tariffCustomerAccountDTO, serviceHeader);
                    });
                }
            }

            var mediaDTO = new MediaDTO
            {
                Content = memoryStream.GetBuffer(),
                FileType = string.Format("eStatement_{0}_{1}", customerAccountDTO.CustomerFullName.StripPunctuation(), customerAccountDTO.CustomerAccountTypeTargetProductDescription.StripPunctuation()),
                FileRemarks = string.Format("{0}_to_{1}", startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")),
            };

            return mediaDTO;
        }

        public MediaDTO PrintLoanRepaymentSchedule(LoanCaseDTO loanCaseDTO, string blobDatabaseConnectionString, ServiceHeader serviceHeader)
        {
            if (loanCaseDTO != null)
            {
                var PV = (loanCaseDTO.ApprovedAmount + loanCaseDTO.AuditTopUpAmount);

                var repaymentSchedule = _financialsService.RepaymentSchedule(loanCaseDTO.LoanRegistrationTermInMonths, loanCaseDTO.LoanRegistrationPaymentFrequencyPerYear, loanCaseDTO.LoanRegistrationGracePeriod, loanCaseDTO.LoanInterestCalculationMode, loanCaseDTO.LoanInterestAnnualPercentageRate, -(double)PV, 0d, loanCaseDTO.LoanRegistrationPaymentDueDate);

                var memoryStream = new MemoryStream();

                var document = new Document(PageSize.A4, 0.5f, 0.5f, 80, 36);

                BaseFont arialNormal = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font fontNormal = new Font(arialNormal, 8, Font.NORMAL);
                Font fontBold = new Font(arialNormal, 8, Font.BOLD);
                Font fontH1 = new Font(arialNormal, 10, Font.NORMAL, new BaseColor(255, 255, 255));

                var pdfWriter = PdfWriter.GetInstance(document, memoryStream);
                pdfWriter.ViewerPreferences = PdfWriter.PageModeUseOutlines;

                var pageEventHandler = new PdfWriterEvents(string.Empty, serviceHeader.ApplicationUserName);
                pdfWriter.PageEvent = pageEventHandler;

                var companyLogoMediaDTO = GetMedia(loanCaseDTO.BranchCompanyId, blobDatabaseConnectionString);

                pageEventHandler.Logo = companyLogoMediaDTO != null ? companyLogoMediaDTO.Content : null;
                pageEventHandler.HeaderFont = new Font(arialNormal, 10, Font.BOLD);
                pageEventHandler.HeaderRow1 = string.Format("{0}, {1} - {2}. {3}", loanCaseDTO.BranchCompanyDescription, loanCaseDTO.BranchCompanyAddressCity, loanCaseDTO.BranchCompanyAddressStreet, loanCaseDTO.BranchCompanyAddressLandLine);
                pageEventHandler.HeaderRow2 = string.Format("{0} - {1}", loanCaseDTO.CustomerFullName, loanCaseDTO.CustomerReference1);

                document.Open();

                PdfPCell blank2ColCell = new PdfPCell();
                blank2ColCell.Colspan = 2;
                blank2ColCell.Border = Rectangle.NO_BORDER;
                blank2ColCell.BackgroundColor = new BaseColor(239, 239, 239);

                // Add repayment schedule
                AddRepaymentScheduleInfo(loanCaseDTO, repaymentSchedule, document, fontNormal, fontH1, fontBold, blank2ColCell);

                // Add Signature Information 
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("\n"));
                AddSignatureInfo(document, fontNormal, fontH1, fontBold, blank2ColCell);

                document.Close();

                var mediaDTO = new MediaDTO
                {
                    Content = memoryStream.GetBuffer()
                };

                return mediaDTO;
            }
            else return null;
        }

        private byte[] TIFFToJPEG(byte[] tiffBytes)
        {
            try
            {
                byte[] jpegBytes;

                using (MemoryStream inStream = new MemoryStream(tiffBytes))
                using (MemoryStream outStream = new MemoryStream())
                {
                    System.Drawing.Bitmap.FromStream(inStream).Save(outStream, ImageFormat.Jpeg);

                    jpegBytes = outStream.ToArray();
                }

                return jpegBytes;
            }
            catch
            {
                return null;
            }
        }

        private byte[] GIFToJPEG(byte[] gifBytes)
        {
            try
            {
                byte[] jpegBytes;

                using (MemoryStream inStream = new MemoryStream(gifBytes))
                using (MemoryStream outStream = new MemoryStream())
                {
                    System.Drawing.Bitmap.FromStream(inStream).Save(outStream, ImageFormat.Jpeg);

                    jpegBytes = outStream.ToArray();
                }

                return jpegBytes;
            }
            catch
            {
                return null;
            }
        }

        private byte[] PNGToJPEG(byte[] pngBytes)
        {
            try
            {
                byte[] jpegBytes;

                using (MemoryStream inStream = new MemoryStream(pngBytes))
                using (MemoryStream outStream = new MemoryStream())
                {
                    System.Drawing.Bitmap.FromStream(inStream).Save(outStream, ImageFormat.Jpeg);

                    jpegBytes = outStream.ToArray();
                }

                return jpegBytes;
            }
            catch
            {
                return null;
            }
        }

        private SqlConnection GetConnection(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            builder.Pooling = true;
            builder.AsynchronousProcessing = true;

            SqlConnection conn = new SqlConnection(builder.ConnectionString);
            conn.Open();

            return conn;
        }

        private bool GetFile(Guid sku, string connectionString, out FileDownloadModel file)
        {
            SqlConnection conn = GetConnection(connectionString);
            SqlTransaction trn = conn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(
                @"SELECT file_name,
            	                content_type,
                                content_coding,
                                DATALENGTH (content) as content_length,
            	            content.PathName() as path,
                                GET_FILESTREAM_TRANSACTION_CONTEXT ()
                            FROM swift_media
                            WHERE media_sku = @media_sku;", conn, trn);

                SqlParameter paramSku = new SqlParameter("@media_sku", SqlDbType.UniqueIdentifier);
                paramSku.Value = sku;
                cmd.Parameters.Add(paramSku);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (false == reader.Read())
                    {
                        reader.Close();
                        trn.Dispose();
                        conn.Dispose();
                        trn = null;
                        conn = null;
                        file = null;
                        return false;
                    }

                    string contentDisposition = reader.GetString(0);
                    string contentType = reader.GetString(1);
                    string contentCoding = reader.IsDBNull(2) ? null : reader.GetString(2);
                    long contentLength = reader.GetInt64(3);
                    string path = reader.GetString(4);
                    byte[] context = reader.GetSqlBytes(5).Buffer;

                    file = new FileDownloadModel
                    {
                        FileName = contentDisposition,
                        ContentCoding = contentCoding,
                        ContentType = contentType,
                        ContentLength = contentLength,
                        Content = new MvcResultSqlFileStream
                        {
                            SqlStream = new SqlFileStream(path, context, FileAccess.Read),
                            Connection = conn,
                            Transaction = trn
                        }
                    };
                    conn = null; // ownership transfered to the stream
                    trn = null;
                    return true;
                }
            }
            finally
            {
                if (null != trn)
                {
                    trn.Dispose();
                }
                if (null != conn)
                {
                    conn.Dispose();
                }
            }
        }

        private void PostFile(Guid sku, string fileName, string fileRemarks, string connectionString, ServiceHeader serviceHeader)
        {
            using (SqlConnection conn = GetConnection(connectionString))
            {
                using (SqlCommand cmdDelete = new SqlCommand(@"DELETE FROM swift_media WHERE media_sku = @media_sku;", conn))
                {
                    SqlParameter paramSku = new SqlParameter("@media_sku", SqlDbType.UniqueIdentifier);
                    paramSku.Value = sku;
                    cmdDelete.Parameters.Add(paramSku);

                    cmdDelete.ExecuteNonQuery();
                }

                using (SqlTransaction trn = conn.BeginTransaction())
                {
                    SqlCommand cmdInsert = new SqlCommand(
                    @"insert into swift_media 
                        (media_sku, file_name, file_remarks, content_type, content_coding, content, created_by)
                    output 
	                    INSERTED.content.PathName(),    
                        GET_FILESTREAM_TRANSACTION_CONTEXT ()
                    values 
                        (@media_sku, @content_disposition, @file_remarks, @content_type, @content_coding, 0x, @created_by)", conn, trn);

                    cmdInsert.Parameters.Add("@media_sku", SqlDbType.UniqueIdentifier);
                    cmdInsert.Parameters["@media_sku"].Value = sku;

                    cmdInsert.Parameters.Add("@content_disposition", SqlDbType.VarChar, 256);
                    cmdInsert.Parameters["@content_disposition"].Value = fileName;

                    cmdInsert.Parameters.Add("@file_remarks", SqlDbType.VarChar, 256);
                    cmdInsert.Parameters["@file_remarks"].Value = fileRemarks;

                    cmdInsert.Parameters.Add("@content_type", SqlDbType.VarChar, 256);
                    cmdInsert.Parameters["@content_type"].Value = System.Web.MimeMapping.GetMimeMapping(fileName);

                    cmdInsert.Parameters.Add("@content_coding", SqlDbType.VarChar, 256);
                    cmdInsert.Parameters["@content_coding"].Value = DBNull.Value;

                    cmdInsert.Parameters.Add("@created_by", SqlDbType.VarChar, 256);
                    cmdInsert.Parameters["@created_by"].Value = serviceHeader.ApplicationUserName;

                    string serverPath = null;
                    byte[] serverTxn = null;

                    // cmdInsert is an INSERT command that uses the OUTPUT clause
                    // Thus we use the ExecuteReader to get the 
                    // result set from the output columns
                    //
                    using (SqlDataReader rdr = cmdInsert.ExecuteReader())
                    {
                        rdr.Read();
                        serverPath = rdr.GetString(0);
                        serverTxn = rdr.GetSqlBytes(1).Buffer;
                    }

                    SaveFile(fileName, serverPath, serverTxn);

                    trn.Commit();
                }
            }
        }

        private void SaveFile(string clientPath, string serverPath, byte[] serverTxn)
        {
            const int BlockSize = 1024 * 512;

            using (FileStream source = new FileStream(clientPath, FileMode.Open, FileAccess.Read))
            {
                using (SqlFileStream dest = new SqlFileStream(serverPath, serverTxn, FileAccess.Write))
                {
                    byte[] buffer = new byte[BlockSize];
                    int bytesRead;
                    while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dest.Write(buffer, 0, bytesRead);
                        dest.Flush();
                    }
                    dest.Close();
                }
                source.Close();
            }
        }

        private void AddDetailedStatementInfo(CustomerAccountDTO customerAccountDTO, Tuple<PageCollectionInfo<GeneralLedgerTransaction>, PageCollectionInfo<GeneralLedgerTransaction>> tuple, bool includeInterestStatement, Document document, Font fontNormal, Font fontH1, Font fontBold, PdfPCell blank2ColCell, DateTime startDate, DateTime endDate)
        {
            if (tuple.Item1 != null)
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.CurrencySymbol = string.Empty;

                PdfPTable principalTable = new PdfPTable(new float[] { 2, 2.5f, 2.1f, 2, 2, 2, 2 });
                PdfPCell principalHeaderCell1 = new PdfPCell(new Phrase(string.Format("Principal Statement from {0} to {1}", startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy")), fontH1));
                principalHeaderCell1.Colspan = 7;
                principalHeaderCell1.Border = Rectangle.NO_BORDER;
                principalHeaderCell1.BackgroundColor = new BaseColor(51, 51, 51);
                principalHeaderCell1.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalHeaderCell1);

                PdfPCell principalHeaderCell2 = new PdfPCell(new Phrase(string.Format("{0} ({1})", customerAccountDTO.FullAccountNumber, string.Format("{0}", customerAccountDTO.CustomerAccountTypeTargetProductDescription).Trim()), fontH1));
                principalHeaderCell2.Colspan = 7;
                principalHeaderCell2.Border = Rectangle.NO_BORDER;
                principalHeaderCell2.BackgroundColor = new BaseColor(51, 51, 51);
                principalHeaderCell2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalHeaderCell2);

                PdfPCell principalOpeningBookBalanceTitleCell = new PdfPCell(new Phrase("Opening Book Balance:", fontH1));
                principalOpeningBookBalanceTitleCell.Colspan = 4;
                principalOpeningBookBalanceTitleCell.Border = Rectangle.NO_BORDER;
                principalOpeningBookBalanceTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                principalOpeningBookBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalOpeningBookBalanceTitleCell);

                PdfPCell principalOpeningBookBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.BookBalanceBroughtFoward), fontH1));
                principalOpeningBookBalanceDataCell.Colspan = 3;
                principalOpeningBookBalanceDataCell.Border = Rectangle.NO_BORDER;
                principalOpeningBookBalanceDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                principalOpeningBookBalanceDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalOpeningBookBalanceDataCell);

                if (customerAccountDTO.CustomerAccountTypeProductCode == (int)ProductCode.Savings)
                {
                    PdfPCell princialOpeningAvailableBalanceTitleCell = new PdfPCell(new Phrase("Opening Available Balance:", fontH1));
                    princialOpeningAvailableBalanceTitleCell.Colspan = 4;
                    princialOpeningAvailableBalanceTitleCell.Border = Rectangle.NO_BORDER;
                    princialOpeningAvailableBalanceTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    princialOpeningAvailableBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(princialOpeningAvailableBalanceTitleCell);

                    PdfPCell principalOpeningAvailableBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.AvailableBalanceBroughtFoward), fontH1));
                    principalOpeningAvailableBalanceDataCell.Colspan = 3;
                    principalOpeningAvailableBalanceDataCell.Border = Rectangle.NO_BORDER;
                    principalOpeningAvailableBalanceDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    principalOpeningAvailableBalanceDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(principalOpeningAvailableBalanceDataCell);
                }

                PdfPCell principalCreatedDateTitleCell = new PdfPCell(new Phrase("Date", fontBold));
                principalCreatedDateTitleCell.Border = Rectangle.NO_BORDER;
                principalCreatedDateTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                principalCreatedDateTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalCreatedDateTitleCell);

                PdfPCell principalDescriptionTitleCell = new PdfPCell(new Phrase("Description", fontBold));
                principalDescriptionTitleCell.Border = Rectangle.NO_BORDER;
                principalDescriptionTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                principalDescriptionTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalDescriptionTitleCell);

                PdfPCell principalReferenceTitleCell = new PdfPCell(new Phrase("Reference", fontBold));
                principalReferenceTitleCell.Border = Rectangle.NO_BORDER;
                principalReferenceTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                principalReferenceTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalReferenceTitleCell);

                PdfPCell principalDebitAmountTitleCell = new PdfPCell(new Phrase("Debit", fontBold));
                principalDebitAmountTitleCell.Border = Rectangle.NO_BORDER;
                principalDebitAmountTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                principalDebitAmountTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalDebitAmountTitleCell);

                PdfPCell principalCreditAmountTitleCell = new PdfPCell(new Phrase("Credit", fontBold));
                principalCreditAmountTitleCell.Border = Rectangle.NO_BORDER;
                principalCreditAmountTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                principalCreditAmountTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalCreditAmountTitleCell);

                PdfPCell principalBookBalanceTitleCell = new PdfPCell(new Phrase("Running Balance", fontBold));
                principalBookBalanceTitleCell.Colspan = 2;
                principalBookBalanceTitleCell.Border = Rectangle.NO_BORDER;
                principalBookBalanceTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                principalBookBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                principalTable.AddCell(principalBookBalanceTitleCell);

                if (tuple.Item1.PageCollection != null && tuple.Item1.PageCollection.Any())
                {
                    foreach (var item in tuple.Item1.PageCollection)
                    {
                        PdfPCell principalCreatedDateDataCell = new PdfPCell(new Phrase(item.JournalCreatedDate.ToString("dd/MM/yyyy"), fontNormal));
                        principalCreatedDateDataCell.Border = Rectangle.NO_BORDER;
                        principalCreatedDateDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        principalCreatedDateDataCell.HorizontalAlignment = 0;
                        principalTable.AddCell(principalCreatedDateDataCell);

                        PdfPCell principalDescriptionDataCell = new PdfPCell(new Phrase(string.Format("{0}{1}{2}", item.JournalPrimaryDescription, Environment.NewLine, item.JournalSecondaryDescription), fontNormal));
                        principalDescriptionDataCell.Border = Rectangle.NO_BORDER;
                        principalDescriptionDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        principalDescriptionDataCell.HorizontalAlignment = 0;
                        principalTable.AddCell(principalDescriptionDataCell);

                        PdfPCell principalReferenceDataCell = new PdfPCell(new Phrase(string.Format("{0}{1}", item.JournalReference, Environment.NewLine), fontNormal));
                        principalReferenceDataCell.Border = Rectangle.NO_BORDER;
                        principalReferenceDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        principalReferenceDataCell.HorizontalAlignment = 0;
                        principalTable.AddCell(principalReferenceDataCell);

                        PdfPCell principalDebitAmountDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.Debit), fontNormal));
                        principalDebitAmountDataCell.Border = Rectangle.NO_BORDER;
                        principalDebitAmountDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        principalDebitAmountDataCell.HorizontalAlignment = 2;
                        principalTable.AddCell(principalDebitAmountDataCell);

                        PdfPCell principalCreditAmountDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.Credit), fontNormal));
                        principalCreditAmountDataCell.Border = Rectangle.NO_BORDER;
                        principalCreditAmountDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        principalCreditAmountDataCell.HorizontalAlignment = 2;
                        principalTable.AddCell(principalCreditAmountDataCell);

                        PdfPCell principalBookBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.RunningBalance), fontNormal));
                        principalBookBalanceDataCell.Colspan = 2;
                        principalBookBalanceDataCell.Border = Rectangle.NO_BORDER;
                        principalBookBalanceDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        principalBookBalanceDataCell.HorizontalAlignment = 2;
                        principalTable.AddCell(principalBookBalanceDataCell);
                    }

                    PdfPCell principalTotalDebitsTitleCell = new PdfPCell(new Phrase("Total Debits:", fontH1));
                    principalTotalDebitsTitleCell.Colspan = 4;
                    principalTotalDebitsTitleCell.Border = Rectangle.NO_BORDER;
                    principalTotalDebitsTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    principalTotalDebitsTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(principalTotalDebitsTitleCell);

                    PdfPCell principalTotalDebitsDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.TotalDebits), fontH1));
                    principalTotalDebitsDataCell.Colspan = 3;
                    principalTotalDebitsDataCell.Border = Rectangle.NO_BORDER;
                    principalTotalDebitsDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    principalTotalDebitsDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(principalTotalDebitsDataCell);

                    PdfPCell principalTotalCreditsTitleCell = new PdfPCell(new Phrase("Total Credits:", fontH1));
                    principalTotalCreditsTitleCell.Colspan = 4;
                    principalTotalCreditsTitleCell.Border = Rectangle.NO_BORDER;
                    principalTotalCreditsTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    principalTotalCreditsTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(principalTotalCreditsTitleCell);

                    PdfPCell principalTotalCreditsDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.TotalCredits), fontH1));
                    principalTotalCreditsDataCell.Colspan = 3;
                    principalTotalCreditsDataCell.Border = Rectangle.NO_BORDER;
                    principalTotalCreditsDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    principalTotalCreditsDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(principalTotalCreditsDataCell);

                    PdfPCell principalClosingBookBalanceTitleCell = new PdfPCell(new Phrase("Closing Book Balance:", fontH1));
                    principalClosingBookBalanceTitleCell.Colspan = 4;
                    principalClosingBookBalanceTitleCell.Border = Rectangle.NO_BORDER;
                    principalClosingBookBalanceTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    principalClosingBookBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(principalClosingBookBalanceTitleCell);

                    PdfPCell principalClosingBookBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.BookBalanceCarriedForward), fontH1));
                    principalClosingBookBalanceDataCell.Colspan = 3;
                    principalClosingBookBalanceDataCell.Border = Rectangle.NO_BORDER;
                    principalClosingBookBalanceDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    principalClosingBookBalanceDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    principalTable.AddCell(principalClosingBookBalanceDataCell);

                    if (customerAccountDTO.CustomerAccountTypeProductCode == (int)ProductCode.Savings)
                    {
                        PdfPCell principalClosingAvailableBalanceTitleCell = new PdfPCell(new Phrase("Closing Available Balance:", fontH1));
                        principalClosingAvailableBalanceTitleCell.Colspan = 4;
                        principalClosingAvailableBalanceTitleCell.Border = Rectangle.NO_BORDER;
                        principalClosingAvailableBalanceTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                        principalClosingAvailableBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                        principalTable.AddCell(principalClosingAvailableBalanceTitleCell);

                        PdfPCell principalClosingAvailableBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.AvailableBalanceCarriedForward), fontH1));
                        principalClosingAvailableBalanceDataCell.Colspan = 3;
                        principalClosingAvailableBalanceDataCell.Border = Rectangle.NO_BORDER;
                        principalClosingAvailableBalanceDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                        principalClosingAvailableBalanceDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                        principalTable.AddCell(principalClosingAvailableBalanceDataCell);
                    }
                }

                // Add table to ducument
                document.Add(principalTable);

                if (customerAccountDTO.CustomerAccountTypeProductCode == (int)ProductCode.Loan && includeInterestStatement && tuple.Item2 != null && tuple.Item2.PageCollection != null && tuple.Item2.PageCollection.Any())
                {
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("\n"));

                    PdfPTable interestTable = new PdfPTable(new float[] { 2, 2.5f, 2.1f, 2, 2, 2 });
                    PdfPCell interestHeaderCell1 = new PdfPCell(new Phrase(string.Format("Interest Statement from {0} to {1}", startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy")), fontH1));
                    interestHeaderCell1.Colspan = 6;
                    interestHeaderCell1.Border = Rectangle.NO_BORDER;
                    interestHeaderCell1.BackgroundColor = new BaseColor(51, 51, 51);
                    interestHeaderCell1.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestHeaderCell1);

                    PdfPCell interestHeaderCell2 = new PdfPCell(new Phrase(string.Format("{0} ({1})", customerAccountDTO.FullAccountNumber, string.Format("{0}", customerAccountDTO.CustomerAccountTypeTargetProductDescription).Trim()), fontH1));
                    interestHeaderCell2.Colspan = 6;
                    interestHeaderCell2.Border = Rectangle.NO_BORDER;
                    interestHeaderCell2.BackgroundColor = new BaseColor(51, 51, 51);
                    interestHeaderCell2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestHeaderCell2);

                    PdfPCell interestOpeningBookBalanceTitleCell = new PdfPCell(new Phrase("Opening Book Balance:", fontH1));
                    interestOpeningBookBalanceTitleCell.Colspan = 3;
                    interestOpeningBookBalanceTitleCell.Border = Rectangle.NO_BORDER;
                    interestOpeningBookBalanceTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestOpeningBookBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestOpeningBookBalanceTitleCell);

                    PdfPCell interestOpeningBookBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item2.BookBalanceBroughtFoward), fontH1));
                    interestOpeningBookBalanceDataCell.Colspan = 3;
                    interestOpeningBookBalanceDataCell.Border = Rectangle.NO_BORDER;
                    interestOpeningBookBalanceDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestOpeningBookBalanceDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestOpeningBookBalanceDataCell);

                    PdfPCell interestCreatedDateTitleCell = new PdfPCell(new Phrase("Date", fontBold));
                    interestCreatedDateTitleCell.Border = Rectangle.NO_BORDER;
                    interestCreatedDateTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                    interestCreatedDateTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestCreatedDateTitleCell);

                    PdfPCell interestDescriptionTitleCell = new PdfPCell(new Phrase("Description", fontBold));
                    interestDescriptionTitleCell.Border = Rectangle.NO_BORDER;
                    interestDescriptionTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                    interestDescriptionTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestDescriptionTitleCell);

                    PdfPCell interestReferenceTitleCell = new PdfPCell(new Phrase("Reference", fontBold));
                    interestReferenceTitleCell.Border = Rectangle.NO_BORDER;
                    interestReferenceTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                    interestReferenceTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestReferenceTitleCell);

                    PdfPCell interestDebitAmountTitleCell = new PdfPCell(new Phrase("Debit", fontBold));
                    interestDebitAmountTitleCell.Border = Rectangle.NO_BORDER;
                    interestDebitAmountTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                    interestDebitAmountTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestDebitAmountTitleCell);

                    PdfPCell interestCreditAmountTitleCell = new PdfPCell(new Phrase("Credit", fontBold));
                    interestCreditAmountTitleCell.Border = Rectangle.NO_BORDER;
                    interestCreditAmountTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                    interestCreditAmountTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestCreditAmountTitleCell);

                    PdfPCell interestBookBalanceTitleCell = new PdfPCell(new Phrase("Running Balance", fontBold));
                    interestBookBalanceTitleCell.Border = Rectangle.NO_BORDER;
                    interestBookBalanceTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
                    interestBookBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestBookBalanceTitleCell);

                    foreach (var item in tuple.Item2.PageCollection)
                    {
                        PdfPCell interestCreatedDateDataCell = new PdfPCell(new Phrase(item.JournalCreatedDate.ToString("dd/MM/yyyy"), fontNormal));
                        interestCreatedDateDataCell.Border = Rectangle.NO_BORDER;
                        interestCreatedDateDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        interestCreatedDateDataCell.HorizontalAlignment = 0;
                        interestTable.AddCell(interestCreatedDateDataCell);

                        PdfPCell interestDescriptionDataCell = new PdfPCell(new Phrase(string.Format("{0}{1}{2}", item.JournalPrimaryDescription, Environment.NewLine, item.JournalSecondaryDescription), fontNormal));
                        interestDescriptionDataCell.Border = Rectangle.NO_BORDER;
                        interestDescriptionDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        interestDescriptionDataCell.HorizontalAlignment = 0;
                        interestTable.AddCell(interestDescriptionDataCell);

                        PdfPCell interestReferenceDataCell = new PdfPCell(new Phrase(string.Format("{0}{1}", item.JournalReference, Environment.NewLine), fontNormal));
                        interestReferenceDataCell.Border = Rectangle.NO_BORDER;
                        interestReferenceDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        interestReferenceDataCell.HorizontalAlignment = 0;
                        interestTable.AddCell(interestReferenceDataCell);

                        PdfPCell interestDebitAmountDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.Debit), fontNormal));
                        interestDebitAmountDataCell.Border = Rectangle.NO_BORDER;
                        interestDebitAmountDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        interestDebitAmountDataCell.HorizontalAlignment = 2;
                        interestTable.AddCell(interestDebitAmountDataCell);

                        PdfPCell interestCreditAmountDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.Credit), fontNormal));
                        interestCreditAmountDataCell.Border = Rectangle.NO_BORDER;
                        interestCreditAmountDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        interestCreditAmountDataCell.HorizontalAlignment = 2;
                        interestTable.AddCell(interestCreditAmountDataCell);

                        PdfPCell interestBookBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.RunningBalance), fontNormal));
                        interestBookBalanceDataCell.Border = Rectangle.NO_BORDER;
                        interestBookBalanceDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                        interestBookBalanceDataCell.HorizontalAlignment = 2;
                        interestTable.AddCell(interestBookBalanceDataCell);
                    }

                    PdfPCell interestTotalDebitsTitleCell = new PdfPCell(new Phrase("Total Debits:", fontH1));
                    interestTotalDebitsTitleCell.Colspan = 4;
                    interestTotalDebitsTitleCell.Border = Rectangle.NO_BORDER;
                    interestTotalDebitsTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestTotalDebitsTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestTotalDebitsTitleCell);

                    PdfPCell interestTotalDebitsDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.TotalDebits), fontH1));
                    interestTotalDebitsDataCell.Colspan = 3;
                    interestTotalDebitsDataCell.Border = Rectangle.NO_BORDER;
                    interestTotalDebitsDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestTotalDebitsDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestTotalDebitsDataCell);

                    PdfPCell interestTotalCreditsTitleCell = new PdfPCell(new Phrase("Total Credits:", fontH1));
                    interestTotalCreditsTitleCell.Colspan = 4;
                    interestTotalCreditsTitleCell.Border = Rectangle.NO_BORDER;
                    interestTotalCreditsTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestTotalCreditsTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestTotalCreditsTitleCell);

                    PdfPCell interestTotalCreditsDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item1.TotalCredits), fontH1));
                    interestTotalCreditsDataCell.Colspan = 3;
                    interestTotalCreditsDataCell.Border = Rectangle.NO_BORDER;
                    interestTotalCreditsDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestTotalCreditsDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestTotalCreditsDataCell);

                    PdfPCell interestClosingBookBalanceTitleCell = new PdfPCell(new Phrase("Closing Book Balance:", fontH1));
                    interestClosingBookBalanceTitleCell.Colspan = 3;
                    interestClosingBookBalanceTitleCell.Border = Rectangle.NO_BORDER;
                    interestClosingBookBalanceTitleCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestClosingBookBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestClosingBookBalanceTitleCell);

                    PdfPCell interestClosingBookBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", tuple.Item2.BookBalanceCarriedForward), fontH1));
                    interestClosingBookBalanceDataCell.Colspan = 3;
                    interestClosingBookBalanceDataCell.Border = Rectangle.NO_BORDER;
                    interestClosingBookBalanceDataCell.BackgroundColor = new BaseColor(51, 51, 51);
                    interestClosingBookBalanceDataCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    interestTable.AddCell(interestClosingBookBalanceDataCell);

                    // Add table to ducument
                    document.Add(interestTable);
                }
            }
        }

        private void AddRepaymentScheduleInfo(LoanCaseDTO loanCaseDTO, List<AmortizationTableEntry> repaymentSchedule, Document document, Font fontNormal, Font fontH1, Font fontBold, PdfPCell blank2ColCell)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.CurrencySymbol = string.Empty;

            PdfPTable repaymentScheduleTable = new PdfPTable(new float[] { 1, 2, 2, 2, 2, 2, 2 });
            PdfPCell principalHeaderCell1 = new PdfPCell(new Phrase(string.Format("Repayment Schedule - {0}", loanCaseDTO.LoanProductDescription), fontH1));
            principalHeaderCell1.Colspan = 7;
            principalHeaderCell1.Border = Rectangle.NO_BORDER;
            principalHeaderCell1.BackgroundColor = new BaseColor(51, 51, 51);
            principalHeaderCell1.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(principalHeaderCell1);

            PdfPCell principalHeaderCell2 = new PdfPCell(new Phrase(string.Format("{0}% APR for {1} month(s) - {2}", loanCaseDTO.LoanInterestAnnualPercentageRate, loanCaseDTO.LoanRegistrationTermInMonths, loanCaseDTO.LoanInterestCalculationModeDescription), fontH1));
            principalHeaderCell2.Colspan = 7;
            principalHeaderCell2.Border = Rectangle.NO_BORDER;
            principalHeaderCell2.BackgroundColor = new BaseColor(51, 51, 51);
            principalHeaderCell2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(principalHeaderCell2);

            PdfPCell periodTitleCell = new PdfPCell(new Phrase("Period", fontBold));
            periodTitleCell.Border = Rectangle.NO_BORDER;
            periodTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            periodTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(periodTitleCell);

            PdfPCell dueDateTitleCell = new PdfPCell(new Phrase("Due Date", fontBold));
            dueDateTitleCell.Border = Rectangle.NO_BORDER;
            dueDateTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            dueDateTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(dueDateTitleCell);

            PdfPCell startingBalanceTitleCell = new PdfPCell(new Phrase("Starting Balance", fontBold));
            startingBalanceTitleCell.Border = Rectangle.NO_BORDER;
            startingBalanceTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            startingBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(startingBalanceTitleCell);

            PdfPCell paymentTitleCell = new PdfPCell(new Phrase("Payment", fontBold));
            paymentTitleCell.Border = Rectangle.NO_BORDER;
            paymentTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            paymentTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(paymentTitleCell);

            PdfPCell interestPaymentTitleCell = new PdfPCell(new Phrase("Interest Payment", fontBold));
            interestPaymentTitleCell.Border = Rectangle.NO_BORDER;
            interestPaymentTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            interestPaymentTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(interestPaymentTitleCell);

            PdfPCell principalPaymentTitleCell = new PdfPCell(new Phrase("Principal Payment", fontBold));
            principalPaymentTitleCell.Border = Rectangle.NO_BORDER;
            principalPaymentTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            principalPaymentTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(principalPaymentTitleCell);

            PdfPCell endingBalanceTitleCell = new PdfPCell(new Phrase("Ending Balance", fontBold));
            endingBalanceTitleCell.Border = Rectangle.NO_BORDER;
            endingBalanceTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            endingBalanceTitleCell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
            repaymentScheduleTable.AddCell(endingBalanceTitleCell);

            if (repaymentSchedule != null && repaymentSchedule.Any())
            {
                foreach (var item in repaymentSchedule)
                {
                    PdfPCell periodDataCell = new PdfPCell(new Phrase(string.Format("{0}", item.Period), fontNormal));
                    periodDataCell.Border = Rectangle.NO_BORDER;
                    periodDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                    periodDataCell.HorizontalAlignment = 2;
                    repaymentScheduleTable.AddCell(periodDataCell);

                    PdfPCell principalDescriptionDataCell = new PdfPCell(new Phrase(item.DueDate.ToString("dd/MM/yyyy"), fontNormal));
                    principalDescriptionDataCell.Border = Rectangle.NO_BORDER;
                    principalDescriptionDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                    principalDescriptionDataCell.HorizontalAlignment = 2;
                    repaymentScheduleTable.AddCell(principalDescriptionDataCell);

                    PdfPCell startingBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.StartingBalance), fontNormal));
                    startingBalanceDataCell.Border = Rectangle.NO_BORDER;
                    startingBalanceDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                    startingBalanceDataCell.HorizontalAlignment = 2;
                    repaymentScheduleTable.AddCell(startingBalanceDataCell);

                    PdfPCell paymentDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.Payment), fontNormal));
                    paymentDataCell.Border = Rectangle.NO_BORDER;
                    paymentDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                    paymentDataCell.HorizontalAlignment = 2;
                    repaymentScheduleTable.AddCell(paymentDataCell);

                    PdfPCell interestPaymentDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.InterestPayment), fontNormal));
                    interestPaymentDataCell.Border = Rectangle.NO_BORDER;
                    interestPaymentDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                    interestPaymentDataCell.HorizontalAlignment = 2;
                    repaymentScheduleTable.AddCell(interestPaymentDataCell);

                    PdfPCell principalPaymentDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.PrincipalPayment), fontNormal));
                    principalPaymentDataCell.Border = Rectangle.NO_BORDER;
                    principalPaymentDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                    principalPaymentDataCell.HorizontalAlignment = 2;
                    repaymentScheduleTable.AddCell(principalPaymentDataCell);

                    PdfPCell endingBalanceDataCell = new PdfPCell(new Phrase(string.Format(nfi, "{0:C}", item.EndingBalance), fontNormal));
                    endingBalanceDataCell.Border = Rectangle.NO_BORDER;
                    endingBalanceDataCell.BackgroundColor = new BaseColor(239, 239, 239);
                    endingBalanceDataCell.HorizontalAlignment = 2;
                    repaymentScheduleTable.AddCell(endingBalanceDataCell);
                }
            }

            // Add table to ducument
            document.Add(repaymentScheduleTable);
        }

        private void AddSignatureInfo(Document document, Font fontNormal, Font fontH1, Font fontBold, PdfPCell blank2ColCell)
        {
            PdfPTable signatureInfoTable = new PdfPTable(new float[] { 0.8f, 2.5f, 0.8f, 2.5f });
            PdfPCell personalInfoHeaderCell = new PdfPCell(new Phrase("Signatures", fontH1));
            personalInfoHeaderCell.Colspan = 4;
            personalInfoHeaderCell.Border = Rectangle.NO_BORDER;
            personalInfoHeaderCell.BackgroundColor = new BaseColor(51, 51, 51);
            personalInfoHeaderCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            signatureInfoTable.AddCell(personalInfoHeaderCell);

            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            /*Row 1*/
            PdfPCell clientSignatureTitleCell = new PdfPCell(new Phrase("Client:", fontBold));
            clientSignatureTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            clientSignatureTitleCell.Border = Rectangle.NO_BORDER;
            clientSignatureTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            signatureInfoTable.AddCell(clientSignatureTitleCell);

            PdfPCell clientSignatureDataCell = new PdfPCell(new Phrase("_________________________", fontNormal));
            clientSignatureDataCell.Border = Rectangle.NO_BORDER;
            clientSignatureDataCell.BackgroundColor = new BaseColor(239, 239, 239);
            signatureInfoTable.AddCell(clientSignatureDataCell);

            PdfPCell bankSignatureTitleCell = new PdfPCell(new Phrase("Institution:", fontBold));
            bankSignatureTitleCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
            bankSignatureTitleCell.Border = Rectangle.NO_BORDER;
            bankSignatureTitleCell.BackgroundColor = new BaseColor(239, 239, 239);
            signatureInfoTable.AddCell(bankSignatureTitleCell);

            PdfPCell bankSignatureDataCell = new PdfPCell(new Phrase("_________________________", fontNormal));
            bankSignatureDataCell.Border = Rectangle.NO_BORDER;
            bankSignatureDataCell.BackgroundColor = new BaseColor(239, 239, 239);
            signatureInfoTable.AddCell(bankSignatureDataCell);

            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);
            signatureInfoTable.AddCell(blank2ColCell);

            // Add table to ducument
            document.Add(signatureInfoTable);
        }
    }
}
