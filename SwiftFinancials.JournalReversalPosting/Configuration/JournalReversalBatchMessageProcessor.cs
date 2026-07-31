using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.JournalReversalBatchPosting.Configuration
{
    public class JournalReversalBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly JournalReversalBatchPostingConfigSection _journalReversalBatchPostingConfigSection;

        public JournalReversalBatchMessageProcessor(IChannelService channelService, ILogger logger, JournalReversalBatchPostingConfigSection journalReversalBatchPostingConfigSection)
            : base(journalReversalBatchPostingConfigSection.JournalReversalBatchPostingSettingsItems.QueuePath, journalReversalBatchPostingConfigSection.JournalReversalBatchPostingSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
            _logger = logger;
            _journalReversalBatchPostingConfigSection = journalReversalBatchPostingConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->JournalReversalBatchMessageProcessor...", exception, _journalReversalBatchPostingConfigSection.JournalReversalBatchPostingSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

            var messageCategory = (MessageCategory)appSpecific;

            switch (messageCategory)
            {
                case MessageCategory.JournalReversalBatchEntry:

                    await _channelService.PostJournalReversalBatchEntryAsync(queueDTO.RecordId, 0x8888, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
