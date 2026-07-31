using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.JournalReversalBatchPosting.Configuration;

namespace SwiftFinancials.JournalReversalBatchPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private JournalReversalBatchMessageProcessor _messageProcessor;

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
            get { return new Guid("{F340D988-6523-4C12-B6C5-154342CDD024}"); }
        }

        public string Description
        {
            get { return "JOURNAL-REVERSAL-BATCH_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var journalReversalBatchPostingConfigSection = (JournalReversalBatchPostingConfigSection)ConfigurationManager.GetSection("journalReversalBatchPostingConfiguration");

                if (journalReversalBatchPostingConfigSection != null)
                {
                    _messageProcessor = new JournalReversalBatchMessageProcessor(_channelService, _logger, journalReversalBatchPostingConfigSection);

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
