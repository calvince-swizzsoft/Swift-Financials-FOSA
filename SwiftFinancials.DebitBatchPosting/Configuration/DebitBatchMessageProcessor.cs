using Application.MainBoundedContext.AccountsModule.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.DebitBatchPosting.Configuration
{
    public class DebitBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly ILogger _logger;
        private readonly DebitBatchPostingConfigSection _debitBatchPostingConfigSection;

        public DebitBatchMessageProcessor(ILogger logger, DebitBatchPostingConfigSection debitBatchPostingConfigSection)
            : base(debitBatchPostingConfigSection.DebitBatchPostingSettingsItems.QueuePath, debitBatchPostingConfigSection.DebitBatchPostingSettingsItems.QueueReceivers)
        {
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

                    Container.Current.Resolve<IDebitBatchAppService>()
                        .PostDebitBatchEntry(queueDTO.RecordId, 0x8888, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
