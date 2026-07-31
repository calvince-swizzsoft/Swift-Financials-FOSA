using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.DebitBatchPosting.Configuration
{
    public class DebitBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly DebitBatchPostingConfigSection _debitBatchPostingConfigSection;

        public DebitBatchMessageProcessor(IChannelService channelService, ILogger logger, DebitBatchPostingConfigSection debitBatchPostingConfigSection)
            : base(debitBatchPostingConfigSection.DebitBatchPostingSettingsItems.QueuePath, debitBatchPostingConfigSection.DebitBatchPostingSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
            _logger = logger;
            _debitBatchPostingConfigSection = debitBatchPostingConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->DebitBatchMessageProcessor...", exception, _debitBatchPostingConfigSection.DebitBatchPostingSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

            var messageCategory = (MessageCategory)appSpecific;

            switch (messageCategory)
            {
                case MessageCategory.DebitBatchEntry:

                    await _channelService.PostDebitBatchEntryAsync(queueDTO.RecordId, 0x8888, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
