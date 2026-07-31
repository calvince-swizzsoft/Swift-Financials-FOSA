using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.FixedDepositLiquidationInvoker.Configuration
{
    public class FixedDepositLiquidationJob : IJob
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;

        public FixedDepositLiquidationJob(
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

                var fixedDepositLiquidationInvokerConfigSection = (FixedDepositLiquidationInvokerConfigSection)ConfigurationManager.GetSection("fixedDepositLiquidationInvokerConfiguration");

                if (fixedDepositLiquidationInvokerConfigSection != null)
                {
                    foreach (var settingsItem in fixedDepositLiquidationInvokerConfigSection.FixedDepositLiquidationInvokerSettingsItems)
                    {
                        var fixedDepositLiquidationInvokerSettingsElement = (FixedDepositLiquidationInvokerSettingsElement)settingsItem;

                        if (fixedDepositLiquidationInvokerSettingsElement != null && fixedDepositLiquidationInvokerSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = fixedDepositLiquidationInvokerSettingsElement.UniqueId };

                            await _channelService.ExecutePayableFixedDepositsAsync(DateTime.Today, fixedDepositLiquidationInvokerSettingsElement.QueuePageSize, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.FixedDepositLiquidationInvoker_FixedDepositLiquidationJob.Execute", ex);
            }
        }
    }
}
