using Application.MainBoundedContext.AccountsModule.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.StandingOrderInvoker.Configuration
{
    public class SkippedStandingOrderJob : IJob
    {
        private readonly ILogger _logger;

        public SkippedStandingOrderJob(
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

                var standingOrderInvokerConfigSection = (StandingOrderInvokerConfigSection)ConfigurationManager.GetSection("standingOrderInvokerConfiguration");

                if (standingOrderInvokerConfigSection != null)
                {
                    foreach (var settingsItem in standingOrderInvokerConfigSection.StandingOrderInvokerSettingsItems)
                    {
                        var standingOrderInvokerSettingsElement = (StandingOrderInvokerSettingsElement)settingsItem;

                        if (standingOrderInvokerSettingsElement != null && standingOrderInvokerSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = standingOrderInvokerSettingsElement.UniqueId };

                            var targetDate = DateTime.Today.AddDays(-1);

                            Container.Current.Resolve<IStandingOrderAppService>()
                                .FixSkippedStandingOrders(targetDate, standingOrderInvokerSettingsElement.QueuePageSize, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.StandingOrderInvoker_SkippedStandingOrderJob.Execute", ex);
            }
        }
    }
}
