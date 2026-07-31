using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.CreditBatchPosting.Configuration
{
    public class CreditBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly CreditBatchPostingConfigSection _creditBatchPostingConfigSection;

        public CreditBatchMessageProcessor(IChannelService channelService, ILogger logger, CreditBatchPostingConfigSection creditBatchPostingConfigSection)
            : base(creditBatchPostingConfigSection.CreditBatchPostingSettingsItems.QueuePath, creditBatchPostingConfigSection.CreditBatchPostingSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
            _logger = logger;
            _creditBatchPostingConfigSection = creditBatchPostingConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->CreditBatchMessageProcessor...", exception, _creditBatchPostingConfigSection.CreditBatchPostingSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

            var messageCategory = (MessageCategory)appSpecific;

            switch (messageCategory)
            {
                case MessageCategory.CreditBatchEntry:

                    await _channelService.PostCreditBatchEntryAsync(queueDTO.RecordId, 0x8888, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
