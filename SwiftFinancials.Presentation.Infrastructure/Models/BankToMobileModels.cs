using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public class BankToMobileViewModel
    {
        public string data { get; set; }

        public override string ToString()
        {
            return data;
        }
    }

    public static class BankToMobileViewModelExtensions
    {
        public static BankToMobileRequestWrapper FromEncryptedBankToMobileRequest(this BankToMobileViewModel model, string apiUsername, string apiPassword, string bankId)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.data))
            {
                var payload = model.DecryptBankToMobile(apiUsername, apiPassword, bankId);

                var javaScriptSerializer = new JavaScriptSerializer();

                var request = javaScriptSerializer.Deserialize<BankToMobileRequestWrapper>(payload);

                return request;
            }
            else return null;
        }

        public static string ToEncryptedBankToMobileRequest(this BankToMobileRequestWrapper data, string apiUsername, string apiPassword, string bankId)
        {
            using (MemoryStream plainTextOutput = new MemoryStream())
            {
                using (StreamWriter plainTextWriter = new StreamWriter(plainTextOutput))
                {
                    plainTextWriter.Write(string.Format("{0}", data));
                    plainTextWriter.Flush();
                    plainTextWriter.Close();

                    return plainTextOutput.EncryptBankToMobile(apiUsername, apiPassword, bankId);
                }
            }
        }

        private static string DecryptBankToMobile(this BankToMobileViewModel model, string apiUsername, string apiPassword, string bankId)
        {
            if (!string.IsNullOrWhiteSpace(model.data))
            {
                var cipherText = model.data;

                string encyKey = string.Format("{0}/{1}/{2}", apiUsername, SHA256(apiPassword), bankId);

                byte[] encodedPassword = new UTF8Encoding().GetBytes(encyKey);

                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

                string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

                byte[] rijnIV = Encoding.Default.GetBytes(encoded.Substring(0, 16));

                byte[] rijnKey = Encoding.Default.GetBytes(encoded.Substring(16, 16));

                var plainText = DecryptRJ256(Convert.FromBase64String(cipherText), rijnKey, rijnIV);

                return plainText;
            }
            else return string.Empty;
        }

        private static string EncryptBankToMobile(this MemoryStream plainTextOutput, string apiUsername, string apiPassword, string bankId)
        {
            string encyKey = string.Format("{0}/{1}/{2}", apiUsername, SHA256(apiPassword), bankId);

            byte[] encodedPassword = Encoding.UTF8.GetBytes(encyKey);

            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

            byte[] rijnIV = Encoding.Default.GetBytes(encoded.Substring(0, 16));

            byte[] rijnKey = Encoding.Default.GetBytes(encoded.Substring(16, 16));

            var cipherText = EncryptRJ256(rijnKey, rijnIV, string.Format("{0}", Encoding.UTF8.GetString(plainTextOutput.ToArray())));

            using (MemoryStream cipherTextOutput = new MemoryStream())
            {
                using (StreamWriter cipherTextWriter = new StreamWriter(cipherTextOutput))
                {
                    cipherTextWriter.Write(cipherText);
                    cipherTextWriter.Flush();
                    cipherTextWriter.Close();

                    return string.Format("{0}", Encoding.UTF8.GetString(cipherTextOutput.ToArray()));
                }
            }
        }

        private static string SHA256(string password)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();

            System.Text.StringBuilder hash = new System.Text.StringBuilder();

            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));

            foreach (byte bit in crypto)
            {
                hash.Append(bit.ToString("x2"));
            }

            return hash.ToString();
        }

        private static String DecryptRJ256(byte[] cypher, byte[] KeyString, byte[] IVString)
        {
            var result = string.Empty;

            using (var rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.KeySize = 256;
                rijndaelManaged.BlockSize = 128;
                rijndaelManaged.Key = KeyString;
                rijndaelManaged.IV = IVString;

                using (var memoryStream = new MemoryStream(cypher))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(KeyString, IVString), CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return result;
        }

        private static string EncryptRJ256(byte[] KeyString, byte[] IVString, string text_to_encrypt)
        {
            var result = string.Empty;

            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.KeySize = 256;
                rijndaelManaged.BlockSize = 128;

                byte[] encrypted;

                byte[] toEncrypt;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(KeyString, IVString), CryptoStreamMode.Write))
                    {
                        toEncrypt = System.Text.Encoding.ASCII.GetBytes(text_to_encrypt);

                        cryptoStream.Write(toEncrypt, 0, toEncrypt.Length);

                        cryptoStream.FlushFinalBlock();

                        encrypted = memoryStream.ToArray();

                        result = Convert.ToBase64String(encrypted);
                    }
                }
            }

            return result;
        }
    }

    public class BankToMobileRequestWrapper
    {
        public string AppDomainName { get; set; }

        public int TransactionType { get; set; }

        public string Narration { get; set; }

        public string MobileNumber { get; set; }

        public string CustomerName { get; set; }

        public string IDNumber { get; set; }

        public string PayrollNumbers { get; set; }

        public string HashedPIN { get; set; }

        public string BankID { get; set; }

        public string AccountNumber { get; set; }

        public decimal TransactionAmount { get; set; }

        public decimal LoanLimit { get; set; }

        public string UniqueCustomerID { get; set; }

        public string DeviceID { get; set; }

        public string UniqueTransactionID { get; set; }

        public string TransactionCode { get; set; }

        public string StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public int LoanProductCode { get; set; }

        public decimal AvailableBalance { get; set; }

        public decimal BookBalance { get; set; }

        public decimal PrincipalBalance { get; set; }

        public decimal InterestBalance { get; set; }

        public string TextMessageBody { get; set; }

        public string TextMessageRecipient { get; set; }

        public int ChannelType { get; set; }

        public string UniqueCustomerAccountId { get; set; }

        public override string ToString()
        {
            var javaScriptSerializer = new JavaScriptSerializer();

            return javaScriptSerializer.Serialize(this);
        }
    }
}