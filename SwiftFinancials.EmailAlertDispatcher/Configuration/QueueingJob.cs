using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.EmailAlertDispatcher.Configuration
{
    public class QueueingJob : IJob
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;

        public QueueingJob(IMessageQueueService messageQueueService, IChannelService channelService, ILogger logger)
        {
            _messageQueueService = messageQueueService ?? throw new ArgumentNullException(nameof(messageQueueService));
            _channelService = channelService ?? throw new ArgumentNullException(nameof(channelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IJob

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Debug("{0}****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail.Key, context.FireTimeUtc.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"));

                var emailDispatcherConfigSection = (EmailDispatcherConfigSection)ConfigurationManager.GetSection("emailDispatcherConfiguration");

                if (emailDispatcherConfigSection != null)
                {
                    foreach (var settingsItem in emailDispatcherConfigSection.EmailDispatcherSettingsItems)
                    {
                        var emailDispatcherSettingsElement = (EmailDispatcherSettingsElement)settingsItem;

                        if (emailDispatcherSettingsElement != null && emailDispatcherSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = emailDispatcherSettingsElement.UniqueId };

                            // 1. Retrieve messages whose DLR status is UnKnown
                            var emailAlertsWithDLRStatusUnKnown = await _channelService.FindEmailAlertsByFilterInPageAsync((int)DLRStatus.UnKnown, null, 0, emailDispatcherSettingsElement.QueuePageSize, emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueueDaysCap, serviceHeader);

                            // 2. Send the messages to msmq - Normal priority
                            if (emailAlertsWithDLRStatusUnKnown != null && emailAlertsWithDLRStatusUnKnown.PageCollection.Any())
                            {
                                foreach (var item in emailAlertsWithDLRStatusUnKnown.PageCollection)
                                {
                                    if (item.MailMessageSendRetry == 0)
                                    {
                                        var queueDTO = new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            SmtpHost = emailDispatcherSettingsElement.SmtpHost,
                                            SmtpPort = emailDispatcherSettingsElement.SmtpPort,
                                            SmtpEnableSsl = (emailDispatcherSettingsElement.SmtpEnableSsl == 1),
                                            SmtpUsername = emailDispatcherSettingsElement.SmtpUsername,
                                            SmtpPassword = emailDispatcherSettingsElement.SmtpPassword
                                        };

                                        _messageQueueService.Send(emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueuePath, queueDTO, MessageCategory.EmailAlert, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.MailMessagePriority, emailDispatcherSettingsElement.TimeToBeReceived);
                                    }
                                }
                            }

                            // 3. Retrieve messages whose DLR status is Pending
                            var emailAlertsWithDLRStatusPending = await _channelService.FindEmailAlertsByFilterInPageAsync((int)DLRStatus.Pending, null, 0, emailDispatcherSettingsElement.QueuePageSize, emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueueDaysCap, serviceHeader);

                            // 4. Send the messages to msmq - Normal priority
                            if (emailAlertsWithDLRStatusPending != null && emailAlertsWithDLRStatusPending.PageCollection.Any())
                            {
                                foreach (var item in emailAlertsWithDLRStatusPending.PageCollection)
                                {
                                    if (item.MailMessageSendRetry == 0)
                                    {
                                        var queueDTO = new QueueDTO
                                        {
                                            RecordId = item.Id,
                                            AppDomainName = serviceHeader.ApplicationDomainName,
                                            SmtpHost = emailDispatcherSettingsElement.SmtpHost,
                                            SmtpPort = emailDispatcherSettingsElement.SmtpPort,
                                            SmtpEnableSsl = (emailDispatcherSettingsElement.SmtpEnableSsl == 1),
                                            SmtpUsername = emailDispatcherSettingsElement.SmtpUsername,
                                            SmtpPassword = emailDispatcherSettingsElement.SmtpPassword
                                        };

                                        _messageQueueService.Send(emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueuePath, queueDTO, MessageCategory.EmailAlert, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.MailMessagePriority, emailDispatcherSettingsElement.TimeToBeReceived);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.EmailDispatcher_QueueingJob.Execute", ex);
            }
        }

        #endregion
    }
}
