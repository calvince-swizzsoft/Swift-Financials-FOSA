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
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(
                       new DirectoryCatalog(
                           Path.GetDirectoryName(
                           Assembly.GetExecutingAssembly().Location
                           )
                       )
                   );

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

        public void SignalDoWork(IScheduler scheduler, params string[] args)
        {
            foreach (Lazy<IPlugin> item in _plugins)
            {
                _logger.LogInfo("{0}->DoWork...", item.Value.Description);

                // fire and forget!
                ThreadPool.QueueUserWorkItem(o => item.Value.DoWork(scheduler, args));
            }
        }

        public void SignalExit()
        {
            foreach (Lazy<IPlugin> item in _plugins)
            {
                _logger.LogInfo("{0}->Exit...", item.Value.Description);

                // fire and forget!
                ThreadPool.QueueUserWorkItem(o => item.Value.Exit());
            }
        }
    }
}
