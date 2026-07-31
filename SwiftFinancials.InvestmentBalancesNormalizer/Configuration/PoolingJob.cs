using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.InvestmentBalancesNormalizer.Configuration
{
    public class PoolingJob : IJob
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;

        public PoolingJob(
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

        #region IJob

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Debug("{0}****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail.Key, context.FireTimeUtc.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"));

                var investmentBalancesNormalizerConfigSection = (InvestmentBalancesNormalizerConfigSection)ConfigurationManager.GetSection("investmentBalancesNormalizerConfiguration");

                if (investmentBalancesNormalizerConfigSection != null)
                {
                    foreach (var settingsItem in investmentBalancesNormalizerConfigSection.InvestmentBalancesNormalizerSettingsItems)
                    {
                        var investmentBalancesNormalizerSettingsElement = (InvestmentBalancesNormalizerSettingsElement)settingsItem;

                        if (investmentBalancesNormalizerSettingsElement != null && investmentBalancesNormalizerSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = investmentBalancesNormalizerSettingsElement.UniqueId };

                            await _channelService.PoolInvestmentBalancesAsync((int)QueuePriority.Normal, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.InvestmentBalancesNormalizer_PoolingJob.Execute", ex);
            }
        }

        #endregion
    }
}
