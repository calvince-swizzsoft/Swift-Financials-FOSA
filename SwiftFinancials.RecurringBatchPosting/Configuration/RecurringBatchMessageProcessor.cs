using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.RecurringBatchPosting.Configuration
{
    public class RecurringBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly ILogger _logger;
        private readonly RecurringBatchPostingConfigSection _recurringBatchPostingConfigSection;

        public RecurringBatchMessageProcessor(ILogger logger, RecurringBatchPostingConfigSection recurringBatchPostingConfigSection)
            : base(recurringBatchPostingConfigSection.RecurringBatchPostingSettingsItems.QueuePath, recurringBatchPostingConfigSection.RecurringBatchPostingSettingsItems.QueueReceivers)
        {
            _logger = logger;
            _recurringBatchPostingConfigSection = recurringBatchPostingConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->RecurringBatchMessageProcessor...", exception, _recurringBatchPostingConfigSection.RecurringBatchPostingSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

            var messageCategory = (MessageCategory)appSpecific;

            switch (messageCategory)
            {
                case MessageCategory.RecurringBatchEntry:

                    // Real app-service method takes two extra params the WCF operation derived
                    // server-side: fileDirectory (serviceBrokerConfiguration's fileExportDirectory)
                    // and the BLOBStore connection string.
                    var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

                    Container.Current.Resolve<IRecurringBatchAppService>()
                        .PostRecurringBatchEntry(queueDTO.RecordId, 0x8888, serviceBrokerSettingsElement.FileExportDirectory, ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
