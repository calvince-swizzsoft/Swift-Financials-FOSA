using Application.MainBoundedContext.AccountsModule.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.InvestmentBalancesNormalizer.Configuration
{
    public class PoolingJob : IJob
    {
        private readonly ILogger _logger;

        public PoolingJob(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

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

                            Container.Current.Resolve<IRecurringBatchAppService>()
                                .PoolInvestmentBalances((int)QueuePriority.Normal, serviceHeader);
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
