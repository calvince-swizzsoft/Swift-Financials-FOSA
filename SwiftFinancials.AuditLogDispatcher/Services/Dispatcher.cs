using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.AppServiceContainer;
using SwiftFinancials.AuditLogDispatcher.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using Unity;

namespace SwiftFinancials.AuditLogDispatcher.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private readonly ILogger _logger;
        private AuditLogMessageProcessor _messageProcessor;

        [ImportingConstructor]
        public Dispatcher(ILogger logger) { _logger = logger ?? throw new ArgumentNullException(nameof(logger)); }
        public Guid Id { get { return new Guid("{C78EC09E-F244-43BA-AB3B-700685F50B82}"); } }
        public string Description { get { return "AUDIT-LOG DISPATCHER"; } }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var configuration = (AuditLogDispatcherConfigSection)
                    ConfigurationManager.GetSection("auditLogDispatcherConfiguration");
                if (configuration == null)
                    throw new ConfigurationErrorsException("The auditLogDispatcherConfiguration section is missing.");

                _messageProcessor = new AuditLogMessageProcessor(_logger,
                    Container.Current.Resolve<IAuditLogAppService>(), configuration);
                _messageProcessor.Open();
            }
            catch (Exception ex) { _logger.LogError("{0}->DoWork...", ex, Description); }
        }

        public void Exit()
        {
            try { if (_messageProcessor != null) _messageProcessor.Close(); }
            catch (Exception ex) { _logger.LogError("{0}->Exit...", ex, Description); }
        }
    }
}
