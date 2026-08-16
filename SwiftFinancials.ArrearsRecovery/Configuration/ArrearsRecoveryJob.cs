using Application.MainBoundedContext.AccountsModule.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.ArrearsRecovery.Configuration
{
    public class ArrearsRecoveryJob : IJob
    {
        private readonly ILogger _logger;

        public ArrearsRecoveryJob(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Debug("{0}****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail.Key, context.FireTimeUtc.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"));

                var arrearsRecoveryConfigSection = (ArrearsRecoveryConfigSection)ConfigurationManager.GetSection("arrearsRecoveryConfiguration");

                if (arrearsRecoveryConfigSection != null)
                {
                    foreach (var settingsItem in arrearsRecoveryConfigSection.ArrearsRecoverySettingsItems)
                    {
                        var arrearsRecoverySettingsElement = (ArrearsRecoverySettingsElement)settingsItem;

                        if (arrearsRecoverySettingsElement != null && arrearsRecoverySettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = arrearsRecoverySettingsElement.UniqueId };

                            Container.Current.Resolve<IRecurringBatchAppService>()
                                .RecoverArrears((int)QueuePriority.Normal, arrearsRecoverySettingsElement.QueuePageSize, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.ArrearsRecovery_ArrearsRecoveryJob_Execute", ex);
            }
        }
    }
}
