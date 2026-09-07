using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SwiftFinancials.WindowsService
{
    public class PluginProvider
    {
        [ImportMany(typeof(IPlugin))]
        private IEnumerable<Lazy<IPlugin>> _plugins = null;

        private readonly ILogger _logger;

        public PluginProvider(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            var pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var catalog = new AggregateCatalog();

            // Restrict MEF's reflection scan to the assemblies that actually declare [Export(typeof(IPlugin))]
            // or the services those plugins import (ILogger, ISmtpService, IMessageQueueService, IChannelService).
            // Scanning every *.dll here (e.g. via a plain "*.dll" DirectoryCatalog) also reflects over transitively
            // deployed dependencies (System.Web.Mvc, DistributedServices.MainBoundedContext, Unity.Interception, etc.)
            // that throw ReflectionTypeLoadException outside a web/WCF host and abort composition for every plugin.
            catalog.Catalogs.Add(new DirectoryCatalog(pluginDirectory, "SwiftFinancials.*.dll"));
            catalog.Catalogs.Add(new DirectoryCatalog(pluginDirectory, "Infrastructure.Crosscutting.Framework.dll"));
            catalog.Catalogs.Add(new DirectoryCatalog(pluginDirectory, "Application.MainBoundedContext.dll"));

            CompositionContainer container = new CompositionContainer(catalog);

            container.ComposeParts(this);
        }

        public int AvailablePlugins
        {
            get
            {
                return _plugins != null ? _plugins.Count() : 0;
            }
        }

        public void ValidatePlugins()
        {
            var errors = new List<Exception>();
            foreach (var item in _plugins)
            {
                try
                {
                    var plugin = item.Value;
                    var validation = plugin as IPluginStartupValidation;
                    validation?.ValidateStartup();
                    _logger.LogInfo("Plugin validated: {0}", plugin.Description);
                }
                catch (Exception ex) { errors.Add(ex); }
            }
            if (errors.Count != 0) throw new AggregateException("Service plugin activation failed.", errors);
        }

        public void SignalDoWork(IScheduler scheduler, params string[] args)
        {
            foreach (Lazy<IPlugin> item in _plugins)
            {
                try
                {
                    var plugin = item.Value;

                    _logger.LogInfo("{0}->DoWork...", plugin.Description);

                    plugin.DoWork(scheduler, args);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Plugin activation failed during DoWork signaling...", ex);
                    throw;
                }
            }
        }

        public void SignalExit()
        {
            foreach (Lazy<IPlugin> item in _plugins)
            {
                try
                {
                    var plugin = item.Value;

                    _logger.LogInfo("{0}->Exit...", plugin.Description);

                    plugin.Exit();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Plugin activation failed during Exit signaling...", ex);
                }
            }
        }
    }
}
