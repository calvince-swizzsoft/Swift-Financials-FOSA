using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;
using SwiftFinancials.StandingOrderInvoker.Configuration;

namespace SwiftFinancials.StandingOrderInvoker.Services
{
    [Export(typeof(IPlugin))]
    public class Fixer : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Fixer(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{FA57D009-8C97-4BC3-906C-E3F569BBA13A}"); }
        }

        public string Description
        {
            get { return "STANDING_ORDER_FIXER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var standingOrderInvokerConfigSection = (StandingOrderInvokerConfigSection)ConfigurationManager.GetSection("standingOrderInvokerConfiguration");

                    if (standingOrderInvokerConfigSection == null)
                        throw new ArgumentNullException(nameof(standingOrderInvokerConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<SkippedStandingOrderJob>()
                        .WithIdentity("SkippedStandingOrderJob", "VFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("SkippedStandingOrderJob", "VFIN")
                        .WithCronSchedule(standingOrderInvokerConfigSection.StandingOrderInvokerSettingsItems.SkippedStandingOrderJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("SkippedStandingOrderJob", "VFIN")))
                        await scheduler.DeleteJob(new JobKey("SkippedStandingOrderJob", "VFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "SkippedStandingOrderJob", schedule.ToString("r"));
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
