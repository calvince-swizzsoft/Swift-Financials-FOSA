using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.ArrearsRecovery.Configuration
{
    public class ArrearsRecoveryJob : IJob
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;

        public ArrearsRecoveryJob(
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

                            await _channelService.RecoverArrearsAsync((int)QueuePriority.Normal, arrearsRecoverySettingsElement.QueuePageSize, serviceHeader);
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
