using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.CreditBatchPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.CreditBatchPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private CreditBatchMessageProcessor _messageProcessor;

        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{EABE7534-726E-447B-B370-3B0ACFFD4584}"); }
        }

        public string Description
        {
            get { return "CREDIT-BATCH_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var creditBatchPostingConfigSection = (CreditBatchPostingConfigSection)ConfigurationManager.GetSection("creditBatchPostingConfiguration");

                if (creditBatchPostingConfigSection != null)
                {
                    _messageProcessor = new CreditBatchMessageProcessor(_logger, creditBatchPostingConfigSection);

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
