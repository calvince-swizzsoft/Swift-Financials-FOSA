using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.AccountAlertDispatcher.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.AccountAlertDispatcher.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private AccountAlertMessageProcessor _messageProcessor;

        private readonly IChannelService _channelService;

        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(IChannelService channelService, ILogger logger)
        {
            _channelService = channelService ?? throw new ArgumentNullException(nameof(channelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{61C6599D-A281-4107-A91D-E813F9C69B21}"); }
        }

        public string Description
        {
            get { return "ACCOUNTALERT DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var accountAlertDispatcherConfigSection = (AccountAlertDispatcherConfigSection)ConfigurationManager.GetSection("accountAlertDispatcherConfiguration");

                if (accountAlertDispatcherConfigSection != null)
                {
                    _messageProcessor = new AccountAlertMessageProcessor(_channelService, _logger, accountAlertDispatcherConfigSection);

                    _messageProcessor.Open();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}->DoWork...", ex, Description);
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
