using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.InvestmentBalancesNormalizer.Configuration;

namespace SwiftFinancials.InvestmentBalancesNormalizer.Services
{
    [Export(typeof(IPlugin))]
    public class Pooler : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Pooler(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{225BF9DA-E3FF-4BFB-A592-878459ED8478}"); }
        }

        public string Description
        {
            get { return "INVESTMENT_BALANCES_POOLER"; }
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
                    var jobDetail = JobBuilder.Create<PoolingJob>()
                        .WithIdentity("PoolingJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("PoolingJob", "SWIFTFIN")
                        .WithCronSchedule(investmentBalancesNormalizerConfigSection.InvestmentBalancesNormalizerSettingsItems.PoolingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("PoolingJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("PoolingJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "PoolingJob", schedule.ToString("r"));
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
