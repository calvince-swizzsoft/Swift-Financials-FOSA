using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.WindowsService
{
    public partial class MainService : ServiceBase
    {
        private PluginProvider _pluginProvider = null;

        private ISchedulerFactory _schedulerFactory = null;

        private IScheduler _scheduler = null;

        private ILogger _logger = null;

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    ConfigureFactories();

                    _logger.LogInfo("Starting...");

                    _pluginProvider = new PluginProvider(_logger);

                    _pluginProvider.Initialize();

                    _logger.LogInfo("Available Plugins -> {0}", _pluginProvider.AvailablePlugins);

                    if (_pluginProvider.AvailablePlugins != 0)
                    {
                        _scheduler = await _schedulerFactory.GetScheduler();

                        _pluginProvider.SignalDoWork(_scheduler, args);

                        await _scheduler.Start();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("SwiftFinancials.WindowsService...", ex);
                }
            });
        }

        protected override void OnStop()
        {
            Task.Run(async () =>
            {
                _logger.LogInfo("Stopping...");

                _logger.LogInfo("Available Plugins -> {0}", _pluginProvider.AvailablePlugins);

                if (_pluginProvider != null && _pluginProvider.AvailablePlugins != 0)
                {
                    _pluginProvider.SignalExit();

                    await _scheduler.Shutdown();
                }

                _logger.CloseAndFlush();
            });
        }

        public void StartDebugging(string[] args)
        {
            OnStart(args);
        }

        private void ConfigureFactories()
        {
            //TODO: Uncomment for production
            //System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
            //{
            //    return true;
            //};

            var unityContainer = new UnityContainer();

            unityContainer.AddNewExtension<QuartzUnityExtension>();
            unityContainer.RegisterType<IChannelService, ChannelService>();
            unityContainer.RegisterType<IMessageQueueService, MessageQueueService>();

            _logger = unityContainer.Resolve<ILogger>();
            _schedulerFactory = unityContainer.Resolve<ISchedulerFactory>();
        }
    }
}