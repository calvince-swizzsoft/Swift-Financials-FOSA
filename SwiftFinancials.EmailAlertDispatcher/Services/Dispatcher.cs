using Application.MainBoundedContext.Services;
using Application.MainBoundedContext.MessagingModule.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.AppServiceContainer;
using SwiftFinancials.EmailAlertDispatcher.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;
using Unity;

namespace SwiftFinancials.EmailAlertDispatcher.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin, IPluginStartupValidation
    {
        private EmailMessageProcessor _messageProcessor;

        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{373345A6-9706-4D45-8C68-739F3711B1AE}"); }
        }

        public string Description
        {
            get { return "E-MAIL DISPATCHER"; }
        }

        public void ValidateStartup()
        {
            if (ConfigurationManager.GetSection("emailDispatcherConfiguration") == null)
                throw new ConfigurationErrorsException("emailDispatcherConfiguration is missing.");
            // Resolve dependencies only; no database operation or SMTP send is performed.
            Container.Current.Resolve<ISmtpService>();
            Container.Current.Resolve<IEmailAlertAppService>();
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                ValidateStartup();
                var emailDispatcherConfigSection = (EmailDispatcherConfigSection)ConfigurationManager.GetSection("emailDispatcherConfiguration");

                if (emailDispatcherConfigSection != null)
                {
                    var smtpService = Container.Current.Resolve<ISmtpService>();

                    _messageProcessor = new EmailMessageProcessor(_logger, smtpService, emailDispatcherConfigSection);

                    _messageProcessor.Open();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}->DoWork...", ex, Description);
                throw;
            }
        }

        public void Exit()
        {
            try
            {
                if (_messageProcessor != null)
                    _messageProcessor.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}->Exit...", ex, Description);
            }
        }

        #endregion
    }
}
