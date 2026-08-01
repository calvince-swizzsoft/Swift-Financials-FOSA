using Infrastructure.Crosscutting.Framework.Configuration;
using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;
using SwiftFinancials.WorkflowProcessorDispatcher.Configuration;

namespace SwiftFinancials.WorkflowProcessorDispatcher.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private WorkflowProcessorMessageProcessor _messageProcessor;

        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IPlugin

        public Guid Id => new Guid("{BF5FDE3F-EA7D-482A-B652-60CB2AAE22E4}");

        public string Description => "WORKFLOW PROCESSOR DISPATCHER";

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var serviceBrokerConfigSection = (ServiceBrokerConfigSection)ConfigurationManager.GetSection("serviceBrokerConfiguration");

                if (serviceBrokerConfigSection != null)
                {
                    _messageProcessor = new WorkflowProcessorMessageProcessor(_logger, serviceBrokerConfigSection);

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
