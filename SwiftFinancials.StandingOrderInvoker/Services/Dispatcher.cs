using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.StandingOrderInvoker.Configuration;

namespace SwiftFinancials.StandingOrderInvoker.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{1AB57E9B-B4AA-4F1C-A351-CBBD95883262}"); }
        }

        public string Description
        {
            get { return "STANDING_ORDER_DISPATCHER"; }
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
                    var jobDetail = JobBuilder.Create<StandingOrderJob>()
                        .WithIdentity("StandingOrderJob", "VFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("StandingOrderJob", "VFIN")
                        .WithCronSchedule(standingOrderInvokerConfigSection.StandingOrderInvokerSettingsItems.StandingOrderJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("StandingOrderJob", "VFIN")))
                        await scheduler.DeleteJob(new JobKey("StandingOrderJob", "VFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "StandingOrderJob", schedule.ToString("r"));
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
