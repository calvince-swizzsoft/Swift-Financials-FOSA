using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.EmailAlertDispatcher.Configuration
{
    public class EmailMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly ISmtpService _smtpService;
        private readonly EmailDispatcherConfigSection _emailDispatcherConfigSection;

        public EmailMessageProcessor(IChannelService channelService, ILogger logger, ISmtpService smtpService, EmailDispatcherConfigSection emailDispatcherConfigSection)
            : base(emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueuePath, emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
            _logger = logger;
            _smtpService = smtpService;
            _emailDispatcherConfigSection = emailDispatcherConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->EmailMessageProcessor...", exception, _emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            foreach (var settingsItem in _emailDispatcherConfigSection.EmailDispatcherSettingsItems)
            {
                var emailDispatcherSettingsElement = (EmailDispatcherSettingsElement)settingsItem;

                if (emailDispatcherSettingsElement.UniqueId == queueDTO.AppDomainName)
                {
                    queueDTO.SmtpHost = emailDispatcherSettingsElement.SmtpHost;
                    queueDTO.SmtpPort = emailDispatcherSettingsElement.SmtpPort;
                    if (emailDispatcherSettingsElement.SmtpEnableSsl == 0)
                        queueDTO.SmtpEnableSsl = false;
                    else if (emailDispatcherSettingsElement.SmtpEnableSsl == 1)
                        queueDTO.SmtpEnableSsl = true;
                    queueDTO.SmtpUsername = emailDispatcherSettingsElement.SmtpUsername;
                    queueDTO.SmtpPassword = emailDispatcherSettingsElement.SmtpPassword;

                    var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

                    var messageCategory = (MessageCategory)appSpecific;

                    switch (messageCategory)
                    {
                        case MessageCategory.EmailAlert:

                            #region email

                            var emailAlertDTO = await _channelService.FindEmailAlertAsync(queueDTO.RecordId, serviceHeader);

                            if (emailAlertDTO == null) return;

                            switch ((DLRStatus)emailAlertDTO.MailMessageDLRStatus)
                            {
                                case DLRStatus.UnKnown:
                                case DLRStatus.Pending:

                                    var attachmentFilePaths = new List<string>();

                                    if (!string.IsNullOrWhiteSpace(emailAlertDTO.MailMessageAttachments))
                                    {
                                        var attachmentsBuffer = emailAlertDTO.MailMessageAttachments.Split(new char[] { ',' });

                                        if (attachmentsBuffer != null)
                                        {
                                            foreach (var item in attachmentsBuffer)
                                            {
                                                var pdfPath = Path.Combine(_emailDispatcherConfigSection.EmailDispatcherSettingsItems.AttachmentStagingFolder, item);

                                                if (File.Exists(pdfPath))
                                                    attachmentFilePaths.Add(pdfPath);
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(emailAlertDTO.MailMessageCC))
                                        _smtpService.SendEmail(queueDTO.SmtpHost, queueDTO.SmtpPort, queueDTO.SmtpEnableSsl, queueDTO.SmtpUsername, queueDTO.SmtpPassword, queueDTO.SmtpUsername, emailAlertDTO.MailMessageTo, emailAlertDTO.MailMessageCC, emailAlertDTO.MailMessageSubject, emailAlertDTO.MailMessageBody, emailAlertDTO.MailMessageIsBodyHtml, attachmentFilePaths);
                                    else _smtpService.SendEmail(queueDTO.SmtpHost, queueDTO.SmtpPort, queueDTO.SmtpEnableSsl, queueDTO.SmtpUsername, queueDTO.SmtpPassword, queueDTO.SmtpUsername, emailAlertDTO.MailMessageTo, emailAlertDTO.MailMessageSubject, emailAlertDTO.MailMessageBody, emailAlertDTO.MailMessageIsBodyHtml, attachmentFilePaths);

                                    emailAlertDTO.MailMessageFrom = queueDTO.SmtpUsername;
                                    emailAlertDTO.MailMessageDLRStatus = (int)DLRStatus.Delivered;
                                    emailAlertDTO.MailMessageSendRetry = 1;

                                    await _channelService.UpdateEmailAlertAsync(emailAlertDTO, serviceHeader);

                                    break;
                                default:
                                    break;
                            }

                            #endregion

                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
