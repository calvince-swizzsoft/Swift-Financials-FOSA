using System.Configuration;

namespace Infrastructure.Crosscutting.Framework.Configuration
{
    public class ServiceBrokerSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("uniqueId", IsKey = true, IsRequired = true)]
        public string UniqueId
        {
            get { return ((string)(base["uniqueId"])); }
            set { base["uniqueId"] = value; }
        }

        [ConfigurationProperty("fileUploadDirectory", IsKey = false, IsRequired = true)]
        public string FileUploadDirectory
        {
            get { return ((string)(base["fileUploadDirectory"])); }
            set { base["fileUploadDirectory"] = value; }
        }

        [ConfigurationProperty("fileExportDirectory", IsKey = false, IsRequired = true)]
        public string FileExportDirectory
        {
            get { return ((string)(base["fileExportDirectory"])); }
            set { base["fileExportDirectory"] = value; }
        }

        [ConfigurationProperty("encryptionPassPhrase", IsKey = false, IsRequired = true)]
        public string EncryptionPassPhrase
        {
            get { return ((string)(base["encryptionPassPhrase"])); }
            set { base["encryptionPassPhrase"] = value; }
        }

        [ConfigurationProperty("encryptionPublicKeyPath", IsKey = false, IsRequired = true)]
        public string EncryptionPublicKeyPath
        {
            get { return ((string)(base["encryptionPublicKeyPath"])); }
            set { base["encryptionPublicKeyPath"] = value; }
        }

        [ConfigurationProperty("encryptionPrivateKeyPath", IsKey = false, IsRequired = true)]
        public string EncryptionPrivateKeyPath
        {
            get { return ((string)(base["encryptionPrivateKeyPath"])); }
            set { base["encryptionPrivateKeyPath"] = value; }
        }

        [ConfigurationProperty("silverlightClientUrl", IsKey = false, IsRequired = true)]
        public string SilverlightClientUrl
        {
            get { return ((string)(base["silverlightClientUrl"])); }
            set { base["silverlightClientUrl"] = value; }
        }

        [ConfigurationProperty("webServicesUrl", IsKey = false, IsRequired = true)]
        public string WebServicesUrl
        {
            get { return ((string)(base["webServicesUrl"])); }
            set { base["webServicesUrl"] = value; }
        }

        [ConfigurationProperty("signalRHubUrl", IsKey = false, IsRequired = true)]
        public string SignalRHubUrl
        {
            get { return ((string)(base["signalRHubUrl"])); }
            set { base["signalRHubUrl"] = value; }
        }

        [ConfigurationProperty("ssrsHost", IsKey = false, IsRequired = true)]
        public string SSRSHost
        {
            get { return ((string)(base["ssrsHost"])); }
            set { base["ssrsHost"] = value; }
        }

        [ConfigurationProperty("ssrsPort", IsKey = false, IsRequired = true)]
        public int SSRSPort
        {
            get { return ((int)(base["ssrsPort"])); }
            set { base["ssrsPort"] = value; }
        }

        [ConfigurationProperty("passwordExpiryPeriod", IsKey = false, IsRequired = true)]
        public int PasswordExpiryPeriod
        {
            get { return ((int)(base["passwordExpiryPeriod"])); }
            set { base["passwordExpiryPeriod"] = value; }
        }

        [ConfigurationProperty("passwordHistoryPolicy", IsKey = false, IsRequired = true)]
        public int PasswordHistoryPolicy
        {
            get { return ((int)(base["passwordHistoryPolicy"])); }
            set { base["passwordHistoryPolicy"] = value; }
        }

        [ConfigurationProperty("miniStatementDaysCap", IsKey = false, IsRequired = true)]
        public int MiniStatementDaysCap
        {
            get { return ((int)(base["miniStatementDaysCap"])); }
            set { base["miniStatementDaysCap"] = value; }
        }

        [ConfigurationProperty("seedDomainRootUser", IsKey = false, IsRequired = true)]
        public int SeedDomainRootUser
        {
            get { return ((int)(base["seedDomainRootUser"])); }
            set { base["seedDomainRootUser"] = value; }
        }

        [ConfigurationProperty("vendorWebsite", IsKey = false, IsRequired = true)]
        public string VendorWebsite
        {
            get { return ((string)(base["vendorWebsite"])); }
            set { base["vendorWebsite"] = value; }
        }

        [ConfigurationProperty("vendorEmail", IsKey = false, IsRequired = true)]
        public string VendorEmail
        {
            get { return ((string)(base["vendorEmail"])); }
            set { base["vendorEmail"] = value; }
        }

        [ConfigurationProperty("vendorTelephone", IsKey = false, IsRequired = true)]
        public string VendorTelephone
        {
            get { return ((string)(base["vendorTelephone"])); }
            set { base["vendorTelephone"] = value; }
        }
    }
}
