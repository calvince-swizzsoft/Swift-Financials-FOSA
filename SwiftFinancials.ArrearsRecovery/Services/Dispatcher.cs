using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.ArrearsRecovery.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.ArrearsRecovery.Services
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
            get { return new Guid("{0A77FA5E-E089-4220-906F-81832AAD08D0}"); }
        }

        public string Description
        {
            get { return "ARREARS_RECOVERY_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var arrearsRecoveryConfigSection = (ArrearsRecoveryConfigSection)ConfigurationManager.GetSection("arrearsRecoveryConfiguration");

                    if (arrearsRecoveryConfigSection == null)
                        throw new ArgumentNullException(nameof(arrearsRecoveryConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<ArrearsRecoveryJob>()
                        .WithIdentity("ArrearsRecoveryJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("ArrearsRecoveryJob", "SWIFTFIN")
                        .WithCronSchedule(arrearsRecoveryConfigSection.ArrearsRecoverySettingsItems.ArrearsRecoveryJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("ArrearsRecoveryJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("ArrearsRecoveryJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "ArrearsRecoveryJob", schedule.ToString("r"));
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
