using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.DebitBatchPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.DebitBatchPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private DebitBatchMessageProcessor _messageProcessor;

        private readonly IChannelService _channelService;

        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(
            IChannelService channelService,
            ILogger logger)
        {
            if (channelService == null)
                throw new ArgumentNullException(nameof(channelService));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _channelService = channelService;
            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{33EE90FF-53EA-4F1C-824F-2F428A6095D9}"); }
        }

        public string Description
        {
            get { return "DEBIT-BATCH_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var debitBatchPostingConfigSection = (DebitBatchPostingConfigSection)ConfigurationManager.GetSection("debitBatchPostingConfiguration");

                if (debitBatchPostingConfigSection != null)
                {
                    _messageProcessor = new DebitBatchMessageProcessor(_channelService, _logger, debitBatchPostingConfigSection);

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
