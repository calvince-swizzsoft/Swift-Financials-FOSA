using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.RecurringBatchPosting.Configuration
{
    public class RecurringBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly RecurringBatchPostingConfigSection _recurringBatchPostingConfigSection;

        public RecurringBatchMessageProcessor(IChannelService channelService, ILogger logger, RecurringBatchPostingConfigSection recurringBatchPostingConfigSection)
            : base(recurringBatchPostingConfigSection.RecurringBatchPostingSettingsItems.QueuePath, recurringBatchPostingConfigSection.RecurringBatchPostingSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
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

                    await _channelService.PostRecurringBatchEntryAsync(queueDTO.RecordId, 0x8888, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
