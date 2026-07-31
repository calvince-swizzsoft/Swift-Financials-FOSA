using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.LoanDisbursementBatchPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.LoanDisbursementBatchPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private LoanDisbursementBatchMessageProcessor _messageProcessor;

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
            get { return new Guid("{5A525658-5388-438F-B075-42ED1CABF2BA}"); }
        }

        public string Description
        {
            get { return "LOAN-DISBURSEMENT-BATCH_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var loanDisbursementBatchPostingConfigSection = (LoanDisbursementBatchPostingConfigSection)ConfigurationManager.GetSection("loanDisbursementBatchPostingConfiguration");

                if (loanDisbursementBatchPostingConfigSection != null)
                {
                    _messageProcessor = new LoanDisbursementBatchMessageProcessor(_channelService, _logger, loanDisbursementBatchPostingConfigSection);

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
