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
    public class Sweeper : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Sweeper(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{11378CE8-B83C-40AA-880E-35D1843EAF25}"); }
        }

        public string Description
        {
            get { return "STANDING_ORDER_SWEEPER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var standingOrderInvokerConfigSection = (StandingOrderInvokerConfigSection)ConfigurationManager.GetSection("standingOrderInvokerConfiguration");

                    if (scheduler == null)
                        throw new ArgumentNullException(nameof(standingOrderInvokerConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<SweepingStandingOrderJob>()
                        .WithIdentity("SweepingStandingOrderJob", "VFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("SweepingStandingOrderJob", "VFIN")
                        .WithCronSchedule(standingOrderInvokerConfigSection.StandingOrderInvokerSettingsItems.SweepingStandingOrderJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("SweepingStandingOrderJob", "VFIN")))
                        await scheduler.DeleteJob(new JobKey("SweepingStandingOrderJob", "VFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "SweepingStandingOrderJob", schedule.ToString("r"));
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
