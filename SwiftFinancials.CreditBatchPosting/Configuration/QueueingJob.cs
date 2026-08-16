using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Messaging;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.CreditBatchPosting.Configuration
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

                var creditBatchPostingConfigSection = (CreditBatchPostingConfigSection)ConfigurationManager.GetSection("creditBatchPostingConfiguration");

                if (creditBatchPostingConfigSection != null)
                {
                    foreach (var settingsItem in creditBatchPostingConfigSection.CreditBatchPostingSettingsItems)
                    {
                        var creditBatchPostingSettingsElement = (CreditBatchPostingSettingsElement)settingsItem;

                        if (creditBatchPostingSettingsElement != null && creditBatchPostingSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = creditBatchPostingSettingsElement.UniqueId };

                            var pageCollectionInfo = Container.Current.Resolve<ICreditBatchAppService>()
                                .FindQueableCreditBatchEntries(0, creditBatchPostingSettingsElement.QueuePageSize, serviceHeader);

                            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                            {
                                foreach (var item in pageCollectionInfo.PageCollection)
                                {
                                    var queueDTO = new QueueDTO
                                    {
                                        RecordId = item.Id,
                                        AppDomainName = serviceHeader.ApplicationDomainName,
                                    };

                                    _messageQueueService.Send(creditBatchPostingConfigSection.CreditBatchPostingSettingsItems.QueuePath, queueDTO, MessageCategory.CreditBatchEntry, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.CreditBatchPriority, creditBatchPostingSettingsElement.TimeToBeReceived);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.CreditBatchPosting_QueueingJob_Execute", ex);
            }
        }
    }
}
