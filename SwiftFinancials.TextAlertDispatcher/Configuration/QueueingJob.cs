using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Configuration
{
    public class QueueingJob : IJob
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly ILogger _logger;

        public QueueingJob(IMessageQueueService messageQueueService, ILogger logger)
        {
            _messageQueueService = messageQueueService ?? throw new ArgumentNullException(nameof(messageQueueService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IJob

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Debug("{0}****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail.Key, context.FireTimeUtc.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"));

                var textDispatcherConfigSection = (TextDispatcherConfigSection)ConfigurationManager.GetSection("textDispatcherConfiguration");

                if (textDispatcherConfigSection != null)
                {
                    foreach (var settingsItem in textDispatcherConfigSection.TextDispatcherSettingsItems)
                    {
                        var textDispatcherSettingsElement = (TextDispatcherSettingsElement)settingsItem;

                        if (textDispatcherSettingsElement != null && textDispatcherSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = textDispatcherSettingsElement.UniqueId };

                            // 1. Retrieve messages whose DLR status is UnKnown
                            var textAlertsWithDLRStatusUnKnown = Container.Current.Resolve<ITextAlertAppService>()
                                .FindTextAlerts((int)DLRStatus.UnKnown, null, 0, textDispatcherSettingsElement.QueuePageSize, textDispatcherConfigSection.TextDispatcherSettingsItems.QueueDaysCap, serviceHeader);

                            // 2. Send the messages to msmq - Normal priority
                            if (textAlertsWithDLRStatusUnKnown != null && textAlertsWithDLRStatusUnKnown.PageCollection.Any())
                            {
                                foreach (var item in textAlertsWithDLRStatusUnKnown.PageCollection)
                                {
                                    if (item.TextMessageSendRetry == 0)
                                    {
                                        var queueDTO = new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            BulkTextUrl = textDispatcherSettingsElement.BulkTextUrl,
                                            BulkTextUsername = textDispatcherSettingsElement.BulkTextUsername,
                                            BulkTextPassword = textDispatcherSettingsElement.BulkTextPassword,
                                            BulkTextSenderId = textDispatcherSettingsElement.BulkTextSenderId,
                                        };

                                        _messageQueueService.Send(textDispatcherConfigSection.TextDispatcherSettingsItems.QueuePath, queueDTO, MessageCategory.SMSAlert, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.TextMessagePriority, textDispatcherSettingsElement.TimeToBeReceived);
                                    }
                                }
                            }

                            // 3. Retrieve messages whose DLR status is Pending
                            var textAlertsWithDLRStatusPending = Container.Current.Resolve<ITextAlertAppService>()
                                .FindTextAlerts((int)DLRStatus.Pending, null, 0, textDispatcherSettingsElement.QueuePageSize, textDispatcherConfigSection.TextDispatcherSettingsItems.QueueDaysCap, serviceHeader);

                            // 4. Send the messages to msmq - Normal priority
                            if (textAlertsWithDLRStatusPending != null && textAlertsWithDLRStatusPending.PageCollection.Any())
                            {
                                foreach (var item in textAlertsWithDLRStatusPending.PageCollection)
                                {
                                    if (item.TextMessageSendRetry == 0)
                                    {
                                        var queueDTO = new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            BulkTextUrl = textDispatcherSettingsElement.BulkTextUrl,
                                            BulkTextUsername = textDispatcherSettingsElement.BulkTextUsername,
                                            BulkTextPassword = textDispatcherSettingsElement.BulkTextPassword,
                                            BulkTextSenderId = textDispatcherSettingsElement.BulkTextSenderId,
                                        };

                                        _messageQueueService.Send(textDispatcherConfigSection.TextDispatcherSettingsItems.QueuePath, queueDTO, MessageCategory.SMSAlert, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.TextMessagePriority, textDispatcherSettingsElement.TimeToBeReceived);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.TextDispatcher.Celcom_QueueingJob.Execute", ex);
            }
        }

        #endregion
    }
}