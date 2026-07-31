using System.Configuration;

namespace Infrastructure.Crosscutting.Framework.Configuration
{
    public class ServiceBrokerSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceBrokerSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceBrokerSettingsElement)(element)).UniqueId;
        }

        public ServiceBrokerSettingsElement this[int index]
        {
            get { return (ServiceBrokerSettingsElement)BaseGet(index); }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("mobileToBankQueuePath", IsRequired = true)]
        public string MobileToBankQueuePath
        {
            get
            {
                return (string)base["mobileToBankQueuePath"];
            }
        }

        [ConfigurationProperty("bankToMobileQueuePath", IsRequired = true)]
        public string BankToMobileQueuePath
        {
            get
            {
                return (string)base["bankToMobileQueuePath"];
            }
        }

        [ConfigurationProperty("bankToMobileIPNQueuePath", IsRequired = true)]
        public string BankToMobileIPNQueuePath
        {
            get
            {
                return (string)base["bankToMobileIPNQueuePath"];
            }
        }

               [ConfigurationProperty("debitBatchPostingQueuePath", IsRequired = true)]
        public string DebitBatchPostingQueuePath
        {
            get
            {
                return (string)base["debitBatchPostingQueuePath"];
            }
        }

        [ConfigurationProperty("loanDisbursementBatchPostingQueuePath", IsRequired = true)]
        public string LoanDisbursementBatchPostingQueuePath
        {
            get
            {
                return (string)base["loanDisbursementBatchPostingQueuePath"];
            }
        }

        [ConfigurationProperty("workflowProcessorQueuePath", IsRequired = true)]
        public string WorkflowProcessorQueuePath
        {
            get
            {
                return (string)base["workflowProcessorQueuePath"];
            }
        }

        [ConfigurationProperty("creditBatchPostingQueuePath", IsRequired = true)]
        public string CreditBatchPostingQueuePath
        {
            get
            {
                return (string)base["creditBatchPostingQueuePath"];
            }
        }

        [ConfigurationProperty("wireTransferBatchPostingQueuePath", IsRequired = true)]
        public string WireTransferBatchPostingQueuePath
        {
            get
            {
                return (string)base["wireTransferBatchPostingQueuePath"];
            }
        }

        [ConfigurationProperty("recurringBatchPostingQueuePath", IsRequired = true)]
        public string RecurringBatchPostingQueuePath
        {
            get
            {
                return (string)base["recurringBatchPostingQueuePath"];
            }
        }

        [ConfigurationProperty("emailDispatcherQueuePath", IsRequired = true)]
        public string EmailDispatcherQueuePath
        {
            get
            {
                return (string)base["emailDispatcherQueuePath"];
            }
        }

        [ConfigurationProperty("textDispatcherQueuePath", IsRequired = true)]
        public string TextDispatcherQueuePath
        {
            get
            {
                return (string)base["textDispatcherQueuePath"];
            }
        }

        [ConfigurationProperty("auditLogDispatcherQueuePath", IsRequired = true)]
        public string AuditLogDispatcherQueuePath
        {
            get
            {
                return (string)base["auditLogDispatcherQueuePath"];
            }
        }

        [ConfigurationProperty("auditTrailDispatcherQueuePath", IsRequired = true)]
        public string AuditTrailDispatcherQueuePath
        {
            get
            {
                return (string)base["auditTrailDispatcherQueuePath"];
            }
        }

        [ConfigurationProperty("accountAlertDispatcherQueuePath", IsRequired = true)]
        public string AccountAlertDispatcherQueuePath
        {
            get
            {
                return (string)base["accountAlertDispatcherQueuePath"];
            }
        }

        [ConfigurationProperty("salaryPeriodPostingQueuePath", IsRequired = true)]
        public string SalaryPeriodPostingQueuePath
        {
            get
            {
                return (string)base["salaryPeriodPostingQueuePath"];
            }
        }

        [ConfigurationProperty("journalReversalBatchPostingQueuePath", IsRequired = true)]
        public string JournalReversalBatchPostingQueuePath
        {
            get
            {
                return (string)base["journalReversalBatchPostingQueuePath"];
            }
        }

        [ConfigurationProperty("brokerQueuePath", IsRequired = true)]
        public string BrokerQueuePath
        {
            get
            {
                return (string)base["brokerQueuePath"];
            }
        }

        [ConfigurationProperty("brokerIPNQueuePath", IsRequired = true)]
        public string BrokerIPNQueuePath
        {
            get
            {
                return (string)base["brokerIPNQueuePath"];
            }
        }

        [ConfigurationProperty("populationRegisterQueryQueuePath", IsRequired = true)]
        public string PopulationRegisterQueryQueuePath
        {
            get
            {
                return (string)base["populationRegisterQueryQueuePath"];
            }
        }

        [ConfigurationProperty("timeToBeReceived", IsRequired = true)]
        public int TimeToBeReceived
        {
            get
            {
                return ((int)(base["timeToBeReceived"]));
            }
        }

        [ConfigurationProperty("logEnabled", IsRequired = true)]
        public int LogEnabled
        {
            get
            {
                return ((int)(base["logEnabled"]));
            }
        }
    }
}
