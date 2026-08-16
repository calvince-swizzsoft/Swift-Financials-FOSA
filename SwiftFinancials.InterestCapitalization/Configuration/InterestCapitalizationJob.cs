using Application.MainBoundedContext.AccountsModule.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.InterestCapitalization.Configuration
{
    public class InterestCapitalizationJob : IJob
    {
        private readonly ILogger _logger;

        public InterestCapitalizationJob(
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

                var interestCapitalizationConfigSection = (InterestCapitalizationConfigSection)ConfigurationManager.GetSection("interestCapitalizationConfiguration");

                if (interestCapitalizationConfigSection != null)
                {
                    foreach (var settingsItem in interestCapitalizationConfigSection.InterestCapitalizationSettingsItems)
                    {
                        var interestCapitalizationSettingsElement = (InterestCapitalizationSettingsElement)settingsItem;

                        if (interestCapitalizationSettingsElement != null && interestCapitalizationSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = interestCapitalizationSettingsElement.UniqueId };

                            Container.Current.Resolve<IRecurringBatchAppService>()
                                .CapitalizeInterest((int)QueuePriority.Normal, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.InterestCapitalization_InterestCapitalizationJob_Execute", ex);
            }
        }
    }
}
