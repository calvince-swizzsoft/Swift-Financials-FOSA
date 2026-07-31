using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;
using SwiftFinancials.RecurringBatchPosting.Configuration;

namespace SwiftFinancials.RecurringBatchPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private RecurringBatchMessageProcessor _messageProcessor;

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
            get { return new Guid("{208A008D-1EA2-449B-AB24-D95AB3EBC4A2}"); }
        }

        public string Description
        {
            get { return "RECURRING-BATCH_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var recurringBatchPostingConfigSection = (RecurringBatchPostingConfigSection)ConfigurationManager.GetSection("recurringBatchPostingConfiguration");

                if (recurringBatchPostingConfigSection != null)
                {
                    _messageProcessor = new RecurringBatchMessageProcessor(_channelService, _logger, recurringBatchPostingConfigSection);

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
