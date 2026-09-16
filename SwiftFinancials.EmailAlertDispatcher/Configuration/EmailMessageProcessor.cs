using Application.MainBoundedContext.MessagingModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.EmailAlertDispatcher.Configuration
{
    public class EmailMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly ILogger _logger;
        private readonly ISmtpService _smtpService;
        private readonly EmailDispatcherConfigSection _emailDispatcherConfigSection;

        public EmailMessageProcessor(ILogger logger, ISmtpService smtpService, EmailDispatcherConfigSection emailDispatcherConfigSection)
            : base(emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueuePath, emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueueReceivers)
        {
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
            var matched = false;
            foreach (var settingsItem in _emailDispatcherConfigSection.EmailDispatcherSettingsItems)
            {
                var emailDispatcherSettingsElement = (EmailDispatcherSettingsElement)settingsItem;

                if (emailDispatcherSettingsElement.UniqueId == queueDTO.AppDomainName)
                {
                    matched = true;
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

                            var emailAlertDTO = Container.Current.Resolve<IEmailAlertAppService>().FindEmailAlert(queueDTO.RecordId, serviceHeader);

                            if (emailAlertDTO == null)
                                throw new InvalidOperationException("Queued email " + queueDTO.RecordId + " was not found in domain '" + queueDTO.AppDomainName + "'. Check the service database configuration.");

                            switch ((DLRStatus)emailAlertDTO.MailMessageDLRStatus)
                            {
                                case DLRStatus.UnKnown:
                                case DLRStatus.Pending:

                                    emailAlertDTO.MailMessageSendRetry += 1;
                                    try
                                    {
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

                                    }
                                    catch (Exception ex)
                                    {
                                        // Persist a terminal failure before acknowledging the queue item.
                                        // A database failure must still escape so MSMQ retains the item.
                                        emailAlertDTO.MailMessageDLRStatus = (int)DLRStatus.Failed;
                                        _logger?.LogError("Email send failed for alert {0}.", ex, emailAlertDTO.Id);
                                        if (!Container.Current.Resolve<IEmailAlertAppService>().UpdateEmailAlert(emailAlertDTO, serviceHeader))
                                            throw new InvalidOperationException("Could not save the failed email status.", ex);
                                        return;
                                    }

                                    emailAlertDTO.MailMessageFrom = queueDTO.SmtpUsername;
                                    emailAlertDTO.MailMessageDLRStatus = (int)DLRStatus.Delivered;

                                    // SMTP acceptance is displayed as Sent, not confirmed recipient delivery.
                                    if (!Container.Current.Resolve<IEmailAlertAppService>().UpdateEmailAlert(emailAlertDTO, serviceHeader))
                                        throw new InvalidOperationException("Email was sent but its status could not be saved.");

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
            if (!matched)
                throw new InvalidOperationException("No email dispatcher configuration matches queued domain '" + queueDTO.AppDomainName + "'. The queue transaction will be rolled back.");
        }
    }
}
