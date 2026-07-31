using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.InvestmentBalancesNormalizer.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.InvestmentBalancesNormalizer.Services
{
    [Export(typeof(IPlugin))]
    public class Normalizer : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Normalizer(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{EFC08033-EAD6-4A28-AFB4-61AC86ABC814}"); }
        }

        public string Description
        {
            get { return "INVESTMENT_BALANCES_NORMALIZER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var investmentBalancesNormalizerConfigSection = (InvestmentBalancesNormalizerConfigSection)ConfigurationManager.GetSection("investmentBalancesNormalizerConfiguration");

                    if (investmentBalancesNormalizerConfigSection == null)
                        throw new ArgumentNullException(nameof(investmentBalancesNormalizerConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<NormalizationJob>()
                        .WithIdentity("NormalizationJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("NormalizationJob", "SWIFTFIN")
                        .WithCronSchedule(investmentBalancesNormalizerConfigSection.InvestmentBalancesNormalizerSettingsItems.NormalizationJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("NormalizationJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("NormalizationJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "NormalizationJob", schedule.ToString("r"));
                }
                catch (Exception ex)
                {
                    _logger.LogError("{0}->DoWork...", ex, Description);
                }
            });
        }

        public void Exit()
        {

        }

        #endregion
    }
}
