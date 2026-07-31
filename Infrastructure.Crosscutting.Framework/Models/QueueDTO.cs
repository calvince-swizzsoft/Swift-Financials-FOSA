using System;

namespace Infrastructure.Crosscutting.Framework.Models
{
    [Serializable]
    public class QueueDTO
    {
        public Guid RecordId { get; set; }

        public string AppDomainName { get; set; }

        public string TarrifDeviceId { get; set; }

        public Guid TarrifCreditGLAccountId { get; set; }

        public Guid TarrifCommissionCustomerAccountId { get; set; }

        public string TarrifMaskedCardNumber { get; set; }

        public string TarrifDescription { get; set; }

        public decimal TarrifAmount { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public bool SmtpEnableSsl { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }

        public string BulkTextUrl { get; set; }

        public string BulkTextUsername { get; set; }

        public string BulkTextPassword { get; set; }

        public string BulkTextSenderId { get; set; }

        public string BankId { get; set; }

        public string ApplicationIdentity { get; set; }

        public string ApiEndpoint { get; set; }

        public string ApiUsername { get; set; }

        public string ApiPassword { get; set; }

        public int AccountAlertTrigger { get; set; }

        public Guid AccountAlertCustomerId { get; set; }

        public string AccountAlertPrimaryDescription { get; set; }

        public string AccountAlertSecondaryDescription { get; set; }

        public string AccountAlertReference { get; set; }

        public decimal AccountAlertTotalValue { get; set; }

        public string LoaneeCustomerFullName { get; set; }

        public string ValueDate { get; set; }

        public int WorkflowRecordType { get; set; }

        public int WorkflowRecordStatus { get; set; }

        public string UserPassword { get; set; }

        public string CallbackUrl { get; set; }

        public string Token { get; set; }

        public int Provider { get; set; }
    }
}
