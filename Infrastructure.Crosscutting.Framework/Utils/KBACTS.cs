using BaseLibS.Parse.Endian;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public static class KBACTS
    {
        public static Tuple<HeaderInfo, List<ChequeInfo>, TrailerInfo> Parse(string path)
        {
            HeaderInfo headerInfo = null;

            List<ChequeInfo> chequeInfoList = null;

            TrailerInfo trailerInfo = null;

            FileInfo fileInfo = new FileInfo(path);

            switch (fileInfo.Extension.Substring(1, 1).ToUpper())
            {
                case "P":
                case "E":
                case "J":

                    using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        Encoding encoding = Encoding.GetEncoding(1252);

                        bool bHeader = default(bool);

                        bool bTrailer = default(bool);

                        string header = string.Empty;

                        string lvCHQData = string.Empty;

                        string lvTrailer = string.Empty;

                        string lvDRN = string.Empty;

                        chequeInfoList = new List<ChequeInfo>();

                        int numBytesRead = 0;

                        while (numBytesRead <= fileInfo.Length && !bTrailer)
                        {
                            int n = 0;
                            byte[] bytes = new byte[0];
                            int numBytesToRead = bytes.Length;

                            if (!bHeader)
                            {
                                bHeader = true;

                                bytes = new byte[119];
                                numBytesToRead = bytes.Length;
                                // Read may return anything from 0 to numBytesToRead.
                                n = fsSource.Read(bytes, 0, numBytesToRead);

                                // Break when the end of the file is reached.
                                if (n == 0) break;

                                numBytesRead += n;

                                header = encoding.GetString(bytes);

                                // Read EOR
                                bytes = new byte[2];
                                numBytesToRead = bytes.Length;

                                // Read may return anything from 0 to numBytesToRead.
                                n = fsSource.Read(bytes, 0, numBytesToRead);

                                // Break when the end of the file is reached.
                                if (n == 0) break;

                                numBytesRead += n;

                                if (encoding.GetString(bytes) != "\r\n") break;
                            }
                            else
                            {
                                bytes = new byte[89];
                                numBytesToRead = bytes.Length;

                                // Read may return anything from 0 to numBytesToRead.
                                n = fsSource.Read(bytes, 0, numBytesToRead);

                                // Break when the end of the file is reached.
                                if (n == 0) break;

                                numBytesRead += n;

                                lvDRN = encoding.GetString(bytes).Substring(0, 20);

                                if (lvDRN.Trim() == "01990140721281011") break;

                                switch (encoding.GetString(bytes).Substring(0, 2))
                                {
                                    case "16": //Control Vouchers

                                        lvCHQData = encoding.GetString(bytes);

                                        // Read EOR
                                        bytes = new byte[2];
                                        numBytesToRead = bytes.Length;

                                        // Read may return anything from 0 to numBytesToRead.
                                        n = fsSource.Read(bytes, 0, numBytesToRead);

                                        // Break when the end of the file is reached.
                                        if (n == 0) break;

                                        numBytesRead += n;

                                        if (encoding.GetString(bytes) != "\r\n") break;

                                        break;
                                    case "19": //Trailer Records

                                        bTrailer = true;

                                        lvTrailer = encoding.GetString(bytes);

                                        bytes = new byte[29];
                                        numBytesToRead = bytes.Length;

                                        // Read may return anything from 0 to numBytesToRead.
                                        n = fsSource.Read(bytes, 0, numBytesToRead);

                                        // Break when the end of the file is reached.
                                        if (n == 0) break;

                                        numBytesRead += n;

                                        lvTrailer = lvTrailer + encoding.GetString(bytes);

                                        // Read EOR
                                        bytes = new byte[2];
                                        numBytesToRead = bytes.Length;

                                        // Read may return anything from 0 to numBytesToRead.
                                        n = fsSource.Read(bytes, 0, numBytesToRead);

                                        // Break when the end of the file is reached.
                                        if (n == 0) break;

                                        numBytesRead += n;

                                        if (encoding.GetString(bytes) != "\r\n") break;

                                        break;
                                    default:

                                        if (!string.IsNullOrWhiteSpace(encoding.GetString(bytes)))
                                        {
                                            lvCHQData = encoding.GetString(bytes);

                                            var chequeInfo = AnalyzeCheque(lvCHQData);

                                            bytes = new byte[4];
                                            numBytesToRead = bytes.Length;

                                            // Read may return anything from 0 to numBytesToRead.
                                            n = fsSource.Read(bytes, 0, numBytesToRead);

                                            // Break when the end of the file is reached.
                                            if (n == 0) break;

                                            numBytesRead += n;

                                            chequeInfo.FrontImage1Size = LittleEndianToInt32(encoding.GetString(bytes));

                                            bytes = new byte[48];
                                            numBytesToRead = bytes.Length;

                                            // Read may return anything from 0 to numBytesToRead.
                                            n = fsSource.Read(bytes, 0, numBytesToRead);

                                            // Break when the end of the file is reached.
                                            if (n == 0) break;

                                            numBytesRead += n;

                                            chequeInfo.FrontImage1Signature = LittleEndianToHexString(encoding.GetString(bytes));

                                            bytes = new byte[4];
                                            numBytesToRead = bytes.Length;

                                            // Read may return anything from 0 to numBytesToRead.
                                            n = fsSource.Read(bytes, 0, numBytesToRead);

                                            // Break when the end of the file is reached.
                                            if (n == 0) break;

                                            numBytesRead += n;

                                            chequeInfo.FrontImage2Size = LittleEndianToInt32(encoding.GetString(bytes));

                                            bytes = new byte[48];
                                            numBytesToRead = bytes.Length;

                                            // Read may return anything from 0 to numBytesToRead.
                                            n = fsSource.Read(bytes, 0, numBytesToRead);

                                            // Break when the end of the file is reached.
                                            if (n == 0) break;

                                            numBytesRead += n;

                                            chequeInfo.FrontImage2Signature = LittleEndianToHexString(encoding.GetString(bytes));

                                            bytes = new byte[4];
                                            numBytesToRead = bytes.Length;

                                            // Read may return anything from 0 to numBytesToRead.
                                            n = fsSource.Read(bytes, 0, numBytesToRead);

                                            // Break when the end of the file is reached.
                                            if (n == 0) break;

                                            numBytesRead += n;

                                            chequeInfo.RearImageSize = LittleEndianToInt32(encoding.GetString(bytes));

                                            bytes = new byte[48];
                                            numBytesToRead = bytes.Length;

                                            // Read may return anything from 0 to numBytesToRead.
                                            n = fsSource.Read(bytes, 0, numBytesToRead);

                                            // Break when the end of the file is reached.
                                            if (n == 0) break;

                                            numBytesRead += n;

                                            chequeInfo.RearImageSignature = LittleEndianToHexString(encoding.GetString(bytes));

                                            // Read Front TIFF
                                            if (chequeInfo.FrontImage1Size > 0 && chequeInfo.FrontImage1Size < fileInfo.Length)
                                            {
                                                bytes = new byte[chequeInfo.FrontImage1Size];
                                                numBytesToRead = bytes.Length;

                                                // Read may return anything from 0 to numBytesToRead.
                                                n = fsSource.Read(bytes, 0, numBytesToRead);

                                                // Break when the end of the file is reached.
                                                if (n == 0) break;

                                                numBytesRead += n;

                                                chequeInfo.FrontImage1 = bytes;
                                            }
                                            else throw new InvalidOperationException(string.Format("Error on Front TIFF Size {0} DRN: {1} Details: {2}", chequeInfo.FrontImage1Size, lvDRN, lvCHQData));

                                            // Read Front JPEG
                                            if (chequeInfo.FrontImage2Size > 0 && chequeInfo.FrontImage2Size < fileInfo.Length)
                                            {
                                                bytes = new byte[chequeInfo.FrontImage2Size];
                                                numBytesToRead = bytes.Length;

                                                // Read may return anything from 0 to numBytesToRead.
                                                n = fsSource.Read(bytes, 0, numBytesToRead);

                                                // Break when the end of the file is reached.
                                                if (n == 0) break;

                                                numBytesRead += n;

                                                chequeInfo.FrontImage2 = bytes;
                                            }
                                            else throw new InvalidOperationException(string.Format("Error on Front JPEG Size {0} DRN: {1} Details: {2}", chequeInfo.FrontImage2Size, lvDRN, lvCHQData));

                                            // Read Rear JPEG
                                            if (chequeInfo.RearImageSize > 0 && chequeInfo.RearImageSize < fileInfo.Length)
                                            {
                                                bytes = new byte[chequeInfo.RearImageSize];
                                                numBytesToRead = bytes.Length;

                                                // Read may return anything from 0 to numBytesToRead.
                                                n = fsSource.Read(bytes, 0, numBytesToRead);

                                                // Break when the end of the file is reached.
                                                if (n == 0) break;

                                                numBytesRead += n;

                                                chequeInfo.RearImage = bytes;
                                            }
                                            else throw new InvalidOperationException(string.Format("Error on Rear JPEG Size {0} DRN: {1} Details: {2}", chequeInfo.RearImageSize, lvDRN, lvCHQData));

                                            // Append to list
                                            chequeInfoList.Add(chequeInfo);

                                            // Read EOR
                                            bytes = new byte[2];
                                            numBytesToRead = bytes.Length;

                                            // Read may return anything from 0 to numBytesToRead.
                                            n = fsSource.Read(bytes, 0, numBytesToRead);

                                            // Break when the end of the file is reached.
                                            if (n == 0) break;

                                            numBytesRead += n;

                                            if (encoding.GetString(bytes) != "\r\n") break;
                                        }

                                        break;
                                }
                            }
                        }

                        headerInfo = AnalyzeHeader(header);

                        trailerInfo = AnalyzeTrailer(lvTrailer);
                    }

                    break;
                default:
                    break;
            }

            return new Tuple<HeaderInfo, List<ChequeInfo>, TrailerInfo>(headerInfo ?? new HeaderInfo { }, chequeInfoList ?? new List<ChequeInfo> { }, trailerInfo ?? new TrailerInfo { });
        }

        private static void Compose(string pathNew)
        {
            FileInfo frontTIF = new FileInfo(@"C:\Users\XPS1521\Desktop\jbb_cts\in_files\Front0001.TIF");
            FileInfo frontJPEG = new FileInfo(@"C:\Users\XPS1521\Desktop\jbb_cts\in_files\Front0001.jpg");
            FileInfo rearJPEG = new FileInfo(@"C:\Users\XPS1521\Desktop\jbb_cts\in_files\Rear0001.jpg");

            Encoding encoding = Encoding.GetEncoding(1252);

            // Write the byte array to the other FileStream.
            using (FileStream fsNew = new FileStream(pathNew, FileMode.Create, FileAccess.Write))
            {
                var bytes = encoding.GetBytes("18223122015510000005100000023125101100000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                var numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("\r\n");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("16700000000000000000510000000000000000000000000000000000005100017184100000000005100000000");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("\r\n");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("00110000013335100051111202200021180000000003073007313045720307300000403073024300003280416");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = INT2LE((int)frontTIF.Length);
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = SHA384ByteArrayGenerator(File.ReadAllBytes(frontTIF.FullName));
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = INT2LE((int)frontJPEG.Length);
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = SHA384ByteArrayGenerator(File.ReadAllBytes(frontJPEG.FullName));
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = INT2LE((int)rearJPEG.Length);
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = SHA384ByteArrayGenerator(File.ReadAllBytes(rearJPEG.FullName));
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = File.ReadAllBytes(frontTIF.FullName);
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = File.ReadAllBytes(frontJPEG.FullName);
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = File.ReadAllBytes(rearJPEG.FullName);
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("\r\n");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("16710000000000000000510000000000000000000000000000000000005100017184300000000005100000000");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("\r\n");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("1951000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);

                bytes = encoding.GetBytes("\r\n");
                numBytesToRead = bytes.Length;
                fsNew.Write(bytes, 0, numBytesToRead);
            }
        }

        static int LittleEndianToInt32(string data)
        {
            var encoding = System.Text.Encoding.GetEncoding(1252);

            var buffer = encoding.GetBytes(data);

            var result = EndianBitConverter.Little.ToInt32(buffer, 0);

            return result;
        }

        static string LittleEndianToHexString(string data)
        {
            var encoding = System.Text.Encoding.GetEncoding(1252);

            var buffer = encoding.GetBytes(data);

            var result = ByteArrayToString(buffer);

            return result;
        }

        static string ByteArrayToString(byte[] inputArray)
        {
            StringBuilder output = new StringBuilder("");

            for (int i = 0; i < inputArray.Length; i++)
            {
                output.Append(inputArray[i].ToString("X2"));
            }

            return output.ToString();
        }

        static byte[] INT2LE(int data)
        {
            byte[] b = new byte[4];
            b[0] = (byte)data;
            b[1] = (byte)(((uint)data >> 8) & 0xFF);
            b[2] = (byte)(((uint)data >> 16) & 0xFF);
            b[3] = (byte)(((uint)data >> 24) & 0xFF);
            return b;
        }

        static string SHA384HexStringGenerator(byte[] buffer)
        {
            SHA384 sha384 = new SHA384CryptoServiceProvider();

            byte[] arrHash = sha384.ComputeHash(buffer);

            StringBuilder output = new StringBuilder();

            for (int i = 0; i < arrHash.Length; i++)
            {
                output.Append(arrHash[i].ToString("X2"));
            }

            return output.ToString();
        }

        static byte[] SHA384ByteArrayGenerator(byte[] buffer)
        {
            SHA384 sha384 = new SHA384CryptoServiceProvider();

            return sha384.ComputeHash(buffer);
        }

        static HeaderInfo AnalyzeHeader(string headerRecord)
        {
            if (!string.IsNullOrWhiteSpace(headerRecord) && headerRecord.Length == 119)
            {
                var headerInfo = new HeaderInfo
                {
                    RecordType = headerRecord.Substring(0, 2),

                    FileType = headerRecord.Substring(2, 1),

                    DateOfFileExchange = DateTime.ParseExact(headerRecord.Substring(3, 8), "ddMMyyyy", null),

                    PresentingOrganisationClearingCentre = headerRecord.Substring(11, 2),

                    PresentingOrganisationBank = headerRecord.Substring(13, 2),

                    PresentingOrganisation = headerRecord.Substring(15, 4),

                    ReceivingOrganisationClearingCentre = headerRecord.Substring(19, 2),

                    ReceivingOrganisationBank = headerRecord.Substring(21, 2),

                    ReceivingOrganisation = headerRecord.Substring(23, 4),

                    FileSerialNumber = headerRecord.Substring(27, 8),

                    LastFileIndicator = headerRecord.Substring(35, 1),

                    Filler = headerRecord.Substring(36, 83),
                };

                return headerInfo;
            }
            else return null;
        }

        static CtrlVoucherInfo AnalyzeControlVoucher(string controlVoucherRecord)
        {
            if (!string.IsNullOrWhiteSpace(controlVoucherRecord) && controlVoucherRecord.Length == 89)
            {
                return new CtrlVoucherInfo
                {
                    RecordType = controlVoucherRecord.Substring(0, 2),

                    VoucherTypeCode = controlVoucherRecord.Substring(2, 2),

                    CurrencyCode = controlVoucherRecord.Substring(4, 2),

                    Value = (decimal.Parse(controlVoucherRecord.Substring(6, 13))) / 100,

                    PositionOfAmount = controlVoucherRecord.Substring(19, 1),

                    SendingBankClearingCentreCode = controlVoucherRecord.Substring(20, 2),

                    SendingBankFiller = controlVoucherRecord.Substring(22, 36),

                    ReceivingBankClearingCentreCode = controlVoucherRecord.Substring(58, 2),

                    ReceivingBankFiller = controlVoucherRecord.Substring(60, 3),

                    SerialNumber = controlVoucherRecord.Substring(63, 6),

                    DocumentReferenceNumber = controlVoucherRecord.Substring(69, 20),
                };
            }
            else return null;
        }

        static TrailerInfo AnalyzeTrailer(string trailerRecord)
        {
            if (!string.IsNullOrWhiteSpace(trailerRecord) && trailerRecord.Length == 118)
            {
                var trailerInfo = new TrailerInfo
                {
                    RecordType = trailerRecord.Substring(0, 2),

                    ClearingCentre = trailerRecord.Substring(2, 2),

                    Bank = trailerRecord.Substring(4, 2),

                    Organisation = trailerRecord.Substring(6, 4),

                    TransactionCount = int.Parse(trailerRecord.Substring(10, 6)),

                    TotalValueCredits = (decimal.Parse(trailerRecord.Substring(16, 17))) / 100,

                    TotalValueDebits = (decimal.Parse(trailerRecord.Substring(33, 17))) / 100,

                    Filler = trailerRecord.Substring(50, 68),
                };

                return trailerInfo;
            }
            else return null;
        }

        static ChequeInfo AnalyzeCheque(string chequeRecord)
        {
            if (!string.IsNullOrWhiteSpace(chequeRecord) && chequeRecord.Length == 89)
            {
                var chequeInfo = new ChequeInfo
                {
                    ReasonForReturnCode = chequeRecord.Substring(0, 2),

                    VoucherTypeCode = chequeRecord.Substring(2, 2),

                    Value = (decimal.Parse(chequeRecord.Substring(4, 13))) / 100,

                    AmountEntryMethod = chequeRecord.Substring(17, 1),

                    DestinationAccountBank = chequeRecord.Substring(18, 2),

                    DestinationAccountBranch = chequeRecord.Substring(20, 3),

                    DestinationAccountAccount = chequeRecord.Substring(23, 10),

                    DestinationAccountCheckDigit = chequeRecord.Substring(33, 1),

                    DestinationAccountCurrencyCode = chequeRecord.Substring(34, 2),

                    Filler = chequeRecord.Substring(36, 2),

                    CollectionAccountDetail = chequeRecord.Substring(38, 20),

                    PresentingBank = chequeRecord.Substring(58, 2),

                    PresentingBranch = chequeRecord.Substring(60, 3),

                    SerialNumber = chequeRecord.Substring(63, 6),

                    DocumentReferenceNumber = chequeRecord.Substring(69, 20),
                };

                return chequeInfo;
            }
            else return null;
        }
    }

    public class ChequeInfo
    {
        public string Id { get; set; }

        public string ReasonForReturnCode { get; set; }

        public string VoucherTypeCode { get; set; }

        public decimal Value { get; set; }

        public string AmountEntryMethod { get; set; }

        public string DestinationAccountBank { get; set; }

        public string DestinationAccountBranch { get; set; }

        public string DestinationAccountAccount { get; set; }

        public string DestinationAccountCheckDigit { get; set; }

        public string DestinationAccountCurrencyCode { get; set; }

        public string Filler { get; set; }

        public string CollectionAccountDetail { get; set; }

        public string PresentingBank { get; set; }

        public string PresentingBranch { get; set; }

        public string SerialNumber { get; set; }

        public string DocumentReferenceNumber { get; set; }

        public int FrontImage1Size { get; set; }

        public string FrontImage1Signature { get; set; }

        public byte[] FrontImage1 { get; set; }

        public int FrontImage2Size { get; set; }

        public string FrontImage2Signature { get; set; }

        public byte[] FrontImage2 { get; set; }

        public int RearImageSize { get; set; }

        public string RearImageSignature { get; set; }

        public byte[] RearImage { get; set; }
    }

    public class CtrlVoucherInfo
    {
        public string RecordType { get; set; }

        public string VoucherTypeCode { get; set; }

        public string CurrencyCode { get; set; }

        public decimal Value { get; set; }

        public string PositionOfAmount { get; set; }

        public string SendingBankClearingCentreCode { get; set; }

        public string SendingBankFiller { get; set; }

        public string ReceivingBankClearingCentreCode { get; set; }

        public string ReceivingBankFiller { get; set; }

        public string SerialNumber { get; set; }

        public string DocumentReferenceNumber { get; set; }
    }

    public class HeaderInfo
    {
        public string RecordType { get; set; }

        public string FileType { get; set; }

        public DateTime DateOfFileExchange { get; set; }

        public string PresentingOrganisationClearingCentre { get; set; }

        public string PresentingOrganisationBank { get; set; }

        public string PresentingOrganisation { get; set; }

        public string ReceivingOrganisationClearingCentre { get; set; }

        public string ReceivingOrganisationBank { get; set; }

        public string ReceivingOrganisation { get; set; }

        public string FileSerialNumber { get; set; }

        public string LastFileIndicator { get; set; }

        public string Filler { get; set; }
    }

    public class TrailerInfo
    {
        public string RecordType { get; set; }

        public string ClearingCentre { get; set; }

        public string Bank { get; set; }

        public string Organisation { get; set; }

        public int TransactionCount { get; set; }

        public decimal TotalValueCredits { get; set; }

        public decimal TotalValueDebits { get; set; }

        public string Filler { get; set; }
    }
}