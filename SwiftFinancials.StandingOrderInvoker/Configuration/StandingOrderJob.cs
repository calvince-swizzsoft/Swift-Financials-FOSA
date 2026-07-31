using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.StandingOrderInvoker.Configuration
{
    public class StandingOrderJob : IJob
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;

        public StandingOrderJob(
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

                var standingOrderInvokerConfigSection = (StandingOrderInvokerConfigSection)ConfigurationManager.GetSection("standingOrderInvokerConfiguration");

                if (standingOrderInvokerConfigSection != null)
                {
                    foreach (var settingsItem in standingOrderInvokerConfigSection.StandingOrderInvokerSettingsItems)
                    {
                        var standingOrderInvokerSettingsElement = (StandingOrderInvokerSettingsElement)settingsItem;

                        if (standingOrderInvokerSettingsElement != null && standingOrderInvokerSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = standingOrderInvokerSettingsElement.UniqueId };

                            await _channelService.ExecuteScheduledStandingOrdersAsync(DateTime.Today, standingOrderInvokerSettingsElement.TargetDateOption, (int)QueuePriority.Normal, standingOrderInvokerSettingsElement.MaximumStandingOrderExecuteAttemptCount, standingOrderInvokerSettingsElement.QueuePageSize, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.StandingOrderInvoker_StandingOrderJob.Execute", ex);
            }
        }
    }
}
