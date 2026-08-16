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

namespace SwiftFinancials.DebitBatchPosting.Configuration
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

                var debitBatchPostingConfigSection = (DebitBatchPostingConfigSection)ConfigurationManager.GetSection("debitBatchPostingConfiguration");

                if (debitBatchPostingConfigSection != null)
                {
                    foreach (var settingsItem in debitBatchPostingConfigSection.DebitBatchPostingSettingsItems)
                    {
                        var debitBatchPostingSettingsElement = (DebitBatchPostingSettingsElement)settingsItem;

                        if (debitBatchPostingSettingsElement != null && debitBatchPostingSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = debitBatchPostingSettingsElement.UniqueId };

                            var pageCollectionInfo = Container.Current.Resolve<IDebitBatchAppService>()
                                .FindQueableDebitBatchEntries(0, debitBatchPostingSettingsElement.QueuePageSize, serviceHeader);

                            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                            {
                                foreach (var item in pageCollectionInfo.PageCollection)
                                {
                                    var queueDTO = new QueueDTO
                                    {
                                        RecordId = item.Id,
                                        AppDomainName = serviceHeader.ApplicationDomainName,
                                    };

                                    _messageQueueService.Send(debitBatchPostingConfigSection.DebitBatchPostingSettingsItems.QueuePath, queueDTO, MessageCategory.DebitBatchEntry, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.DebitBatchPriority, debitBatchPostingSettingsElement.TimeToBeReceived);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.DebitBatchPosting_QueueingJob_Execute", ex);
            }
        }
    }
}
