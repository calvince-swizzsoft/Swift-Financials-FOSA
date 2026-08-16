using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Configuration;
using System.Messaging;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.JournalReversalBatchPosting.Configuration
{
    public class QueueingJob : IJob
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly ILogger _logger;

        public QueueingJob(
            IMessageQueueService messageQueueService,
            ILogger logger)
        {
            if (messageQueueService == null)
                throw new ArgumentNullException(nameof(messageQueueService));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _messageQueueService = messageQueueService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Debug("{0}****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail.Key, context.FireTimeUtc.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"));

                var journalReversalBatchPostingConfigSection = (JournalReversalBatchPostingConfigSection)ConfigurationManager.GetSection("journalReversalBatchPostingConfiguration");

                if (journalReversalBatchPostingConfigSection != null)
                {
                    foreach (var settingsItem in journalReversalBatchPostingConfigSection.JournalReversalBatchPostingSettingsItems)
                    {
                        var journalReversalBatchPostingSettingsElement = (JournalReversalBatchPostingSettingsElement)settingsItem;

                        if (journalReversalBatchPostingSettingsElement != null && journalReversalBatchPostingSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = journalReversalBatchPostingSettingsElement.UniqueId };

                            var pageCollectionInfo = Container.Current.Resolve<IJournalReversalBatchAppService>()
                                .FindQueableJournalReversalBatchEntries(0, journalReversalBatchPostingSettingsElement.QueuePageSize, serviceHeader);

                            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                            {
                                foreach (var item in pageCollectionInfo.PageCollection)
                                {
                                    var queueDTO = new QueueDTO
                                    {
                                        RecordId = item.Id,
                                        AppDomainName = serviceHeader.ApplicationDomainName,
                                    };

                                    _messageQueueService.Send(journalReversalBatchPostingConfigSection.JournalReversalBatchPostingSettingsItems.QueuePath, queueDTO, MessageCategory.JournalReversalBatchEntry, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.JournalReversalBatchPriority, journalReversalBatchPostingSettingsElement.TimeToBeReceived);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.JournalReversalBatchPosting_QueueingJob_Execute", ex);
            }
        }
    }
}
