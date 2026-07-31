using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;
using SwiftFinancials.TextAlertDispatcher.Celcom.Configuration;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Services
{
    [Export(typeof(IPlugin))]
    public class Queuer : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Queuer(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IPlugin

        public Guid Id => new Guid("{903BF152-D32F-4C75-8630-D07B9CC6A7E5}");

        public string Description => "TEXT QUEUER";

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var textDispatcherConfigSection = (TextDispatcherConfigSection)ConfigurationManager.GetSection("textDispatcherConfiguration");

                    if (textDispatcherConfigSection == null)
                        throw new ArgumentNullException(nameof(textDispatcherConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<QueueingJob>()
                        .WithIdentity("TextQueueingJob", "SwiftFinancials")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("TextQueueingJob", "SwiftFinancials")
                        .WithCronSchedule(textDispatcherConfigSection.TextDispatcherSettingsItems.QueueingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("TextQueueingJob", "SwiftFinancials")))
                        await scheduler.DeleteJob(new JobKey("TextQueueingJob", "SwiftFinancials"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "TextQueueingJob", schedule.ToString("r"));
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
