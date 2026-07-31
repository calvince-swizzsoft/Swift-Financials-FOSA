using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace SwiftFinancials.LoanDisbursementBatchPosting.Configuration
{
    public class LoanDisbursementBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly LoanDisbursementBatchPostingConfigSection _loanDisbursementBatchPostingConfigSection;

        public LoanDisbursementBatchMessageProcessor(IChannelService channelService, ILogger logger, LoanDisbursementBatchPostingConfigSection loanDisbursementBatchPostingConfigSection)
            : base(loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueuePath, loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
            _logger = logger;
            _loanDisbursementBatchPostingConfigSection = loanDisbursementBatchPostingConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->LoanDisbursementBatchMessageProcessor...", exception, _loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

            var messageCategory = (MessageCategory)appSpecific;

            switch (messageCategory)
            {
                case MessageCategory.LoanDisbursementBatchEntry:

                    await _channelService.PostLoanDisbursementBatchEntryAsync(queueDTO.RecordId, 0x8888, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
