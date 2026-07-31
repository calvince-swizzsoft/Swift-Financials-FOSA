using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;
using SwiftFinancials.TextAlertDispatcher.Celcom.Configuration;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Celcom.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private TextMessageProcessor _messageProcessor;

        private readonly IChannelService _channelService;

        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(IChannelService channelService, ILogger logger)
        {
            _channelService = channelService ?? throw new ArgumentNullException(nameof(channelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IPlugin

        public Guid Id => new Guid("{A60483B6-E0E9-4690-BF1B-0BCE8D00406A}");

        public string Description => "TEXT DISPATCHER";

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var textDispatcherConfigSection = (TextDispatcherConfigSection)ConfigurationManager.GetSection("textDispatcherConfiguration");

                if (textDispatcherConfigSection != null)
                {
                    _messageProcessor = new TextMessageProcessor(_channelService, _logger, textDispatcherConfigSection);

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
