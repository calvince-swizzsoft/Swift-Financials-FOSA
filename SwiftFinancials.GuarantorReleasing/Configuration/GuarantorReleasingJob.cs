using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.GuarantorReleasing.Configuration
{
    public class GuarantorReleasingJob : IJob
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;

        public GuarantorReleasingJob(
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

                var guarantorReleasingConfigSection = (GuarantorReleasingConfigSection)ConfigurationManager.GetSection("guarantorReleasingConfiguration");

                if (guarantorReleasingConfigSection != null)
                {
                    foreach (var settingsItem in guarantorReleasingConfigSection.GuarantorReleasingSettingsItems)
                    {
                        var guarantorReleasingSettingsElement = (GuarantorReleasingSettingsElement)settingsItem;

                        if (guarantorReleasingSettingsElement != null && guarantorReleasingSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = guarantorReleasingSettingsElement.UniqueId };

                            await _channelService.ReleaseLoanGuarantorsAsync((int)QueuePriority.Normal, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.GuarantorReleasing_GuarantorReleasingJob_Execute", ex);
            }
        }
    }
}
