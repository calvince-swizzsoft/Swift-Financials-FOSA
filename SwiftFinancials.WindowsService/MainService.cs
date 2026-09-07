using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Xml;
using Unity;

namespace SwiftFinancials.WindowsService
{
    public partial class MainService : ServiceBase
    {
        private PluginProvider _pluginProvider;
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        private ILogger _logger;
        private UnityContainer _container;
        private bool _workSignalled;

        public MainService() { InitializeComponent(); }

        protected override void OnStart(string[] args)
        {
            try
            {
                // Do not report Running until dependency/configuration initialization succeeds.
                RequestAdditionalTime(30000);
                ValidateStartup();
                _scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
                _workSignalled = true;
                _pluginProvider.SignalDoWork(_scheduler, args);
                _scheduler.Start().GetAwaiter().GetResult();
                StartupDiagnostics.Write("Service startup completed.");
                _logger.LogInfo("Service startup completed. Available Plugins -> {0}", _pluginProvider.AvailablePlugins);
            }
            catch (Exception ex)
            {
                ExitCode = 1064;
                StartupDiagnostics.Write("Service startup failed.", ex);
                try { _logger?.LogError("Service startup failed.", ex); } catch { }
                Cleanup();
                throw;
            }
        }

        // Used by the deployment preflight; never starts jobs, opens queues, or sends mail.
        public void ValidateStartup()
        {
            StartupDiagnostics.Write("Starting dependency and configuration checks.");
            ConfigureFactories();
            ValidateEmailConfiguration();
            _pluginProvider = new PluginProvider(_logger);
            _pluginProvider.Initialize();
            if (_pluginProvider.AvailablePlugins == 0)
                throw new ConfigurationErrorsException("No service plugins were discovered. Deploy the complete Windows service package.");
            _pluginProvider.ValidatePlugins();
            StartupDiagnostics.Write("Startup checks passed. Available Plugins -> " + _pluginProvider.AvailablePlugins);
        }

        private static void ValidateEmailConfiguration()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = configuration.GetSection("emailDispatcherConfiguration");
            if (section == null)
                throw new ConfigurationErrorsException("emailDispatcherConfiguration is missing from the service executable configuration.");
            var document = new XmlDocument();
            document.LoadXml(section.SectionInformation.GetRawXml());
            var settings = document.SelectSingleNode("/emailDispatcherConfiguration/emailDispatcherSettings");
            int receivers;
            if (settings == null || !int.TryParse(settings.Attributes["queueReceivers"]?.Value, out receivers) || receivers < 1)
                throw new ConfigurationErrorsException("Email queueReceivers must be at least 1.");
            if (string.IsNullOrWhiteSpace(settings.Attributes["queuePath"]?.Value))
                throw new ConfigurationErrorsException("Email queuePath is required.");
            var entries = settings.SelectNodes("*[@uniqueId]");
            if (entries.Count == 0)
                throw new ConfigurationErrorsException("At least one email dispatcher domain must be configured.");
            foreach (XmlElement entry in entries)
            {
                var domain = entry.GetAttribute("uniqueId");
                if (ConfigurationManager.ConnectionStrings[domain] == null)
                    throw new ConfigurationErrorsException("Email domain '" + domain + "' has no matching named connection string. Match the deployed API domain and database settings.");
            }
        }

        protected override void OnStop()
        {
            StartupDiagnostics.Write("Stopping service.");
            Cleanup();
        }

        private void Cleanup()
        {
            try { if (_workSignalled) _pluginProvider?.SignalExit(); } catch (Exception ex) { StartupDiagnostics.Write("Plugin shutdown failed.", ex); }
            try { _scheduler?.Shutdown(true).GetAwaiter().GetResult(); } catch (Exception ex) { StartupDiagnostics.Write("Scheduler shutdown failed.", ex); }
            try { _logger?.CloseAndFlush(); } catch (Exception ex) { StartupDiagnostics.Write("Logger shutdown failed.", ex); }
            try { _container?.Dispose(); } catch (Exception ex) { StartupDiagnostics.Write("Container shutdown failed.", ex); }
        }

        public void CloseDiagnostics() { Cleanup(); }

        public void StartDebugging(string[] args)
        {
            ValidateStartup();
            _scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            _workSignalled = true;
            _pluginProvider.SignalDoWork(_scheduler, args);
            _scheduler.Start().GetAwaiter().GetResult();
        }

        private void ConfigureFactories()
        {
            _container = new UnityContainer();
            _container.AddNewExtension<QuartzUnityExtension>();
            _logger = _container.Resolve<ILogger>();
            _schedulerFactory = _container.Resolve<ISchedulerFactory>();
        }
    }
}
