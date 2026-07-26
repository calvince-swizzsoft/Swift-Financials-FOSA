using Application.MainBoundedContext.DTO.RegistryModule;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace SwiftFinancials.Presentation.Infrastructure.Models
{
    public class BrokerViewModel
    {
        public string data { get; set; }

        public override string ToString()
        {
            return data;
        }
    }

    public static class BrokerModelExtensions
    {
        public static BrokerRequestWrapper FromEncryptedBrokerRequest(this BrokerViewModel model, string apiUsername, string apiPassword, string applicationId)
        {
            if (model != null && !string.IsNullOrWhiteSpace(model.data))
            {
                var payload = model.DecryptBrokerRequest(apiUsername, apiPassword, applicationId.ToLower());

                var javaScriptSerializer = new JavaScriptSerializer();

                var request = javaScriptSerializer.Deserialize<BrokerRequestWrapper>(payload);

                return request;
            }
            else return null;
        }

        public static string ToEncryptedBrokerRequest(this BrokerRequestWrapper data, string apiUsername, string apiPassword, string applicationId)
        {
            using (MemoryStream plainTextOutput = new MemoryStream())
            {
                using (StreamWriter plainTextWriter = new StreamWriter(plainTextOutput))
                {
                    plainTextWriter.Write(string.Format("{0}", data));
                    plainTextWriter.Flush();
                    plainTextWriter.Close();

                    return plainTextOutput.EncryptBrokerRequest(apiUsername, apiPassword, applicationId.ToLower());
                }
            }
        }

        private static string DecryptBrokerRequest(this BrokerViewModel model, string apiUsername, string apiPassword, string applicationId)
        {
            if (!string.IsNullOrWhiteSpace(model.data))
            {
                var cipherText = model.data;

                string encyKey = string.Format("{0}/{1}/{2}", apiUsername, SHA256(apiPassword), applicationId);

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

        private static string EncryptBrokerRequest(this MemoryStream plainTextOutput, string apiUsername, string apiPassword, string applicationIdentity)
        {
            string encyKey = string.Format("{0}/{1}/{2}", apiUsername, SHA256(apiPassword), applicationIdentity);

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
            SHA256Managed crypt = new SHA256Managed();

            StringBuilder hash = new StringBuilder();

            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));

            foreach (byte bit in crypto)
            {
                hash.Append(bit.ToString("x2"));
            }

            return hash.ToString();
        }

        private static string DecryptRJ256(byte[] cypher, byte[] KeyString, byte[] IVString)
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
                        toEncrypt = Encoding.ASCII.GetBytes(text_to_encrypt);

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

    public class BrokerRequestWrapper
    {
        public int TransactionType { get; set; }

        public string ApplicationIdentity { get; set; }

        public Guid UniqueIdentifier { get; set; }

        public string Payload { get; set; }

        public string StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string Narration { get; set; }

        public override string ToString()
        {
            var javaScriptSerializer = new JavaScriptSerializer();

            return javaScriptSerializer.Serialize(this);
        }
    }

    public class BrokerCustomerRegistrationRequestModel
    {
        public int Type { get; set; }

        public int IndividualType { get; set; }

        public string IndividualFirstName { get; set; }

        public string IndividualOtherNames { get; set; }

        public int IndividualIdentityType { get; set; }

        public string IndividualIdentityNumber { get; set; }

        public int IndividualSalutation { get; set; }

        public int IndividualGender { get; set; }

        public int IndividualMaritalStatus { get; set; }

        public int IndividualNationality { get; set; }

        public string IndividualBirthDate { get; set; }

        public string NonIndividualDescription { get; set; }

        public string NonIndividualRegistrationNumber { get; set; }

        public string NonIndividualDateEstablished { get; set; }

        public string AddressAddressLine1 { get; set; }

        public string AddressAddressLine2 { get; set; }

        public string AddressStreet { get; set; }

        public string AddressPostalCode { get; set; }

        public string AddressCity { get; set; }

        public string AddressEmail { get; set; }

        public string AddressLandLine { get; set; }

        public string AddressMobileLine { get; set; }

        public string PassportBuffer { get; set; }

        public string SignatureBuffer { get; set; }

        public string IdentityCardFrontSideBuffer { get; set; }

        public string IdentityCardBackSideBuffer { get; set; }

        public string BiometricFingerprintBuffer { get; set; }

        public string BiometricFingerprintTemplateBuffer { get; set; }

        public string BiometricFingerVeinTemplateBuffer { get; set; }

        public string Remarks { get; set; }

        public int BiometricFingerprintTemplateFormat { get; set; }

        public int BiometricFingerVeinTemplateFormat { get; set; }

        public string RecruitedBy { get; set; }

        public string CreatedBy { get; set; }
    }
}