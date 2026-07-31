using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;
using SwiftFinancials.RecurringBatchPosting.Configuration;

namespace SwiftFinancials.RecurringBatchPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Queuer : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Queuer(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{66A9F4DA-3D8D-4B5B-B4C9-BF673EC0B5DD}"); }
        }

        public string Description
        {
            get { return "RECURRING-BATCH_QUEUER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var recurringBatchPostingConfigSection = (RecurringBatchPostingConfigSection)ConfigurationManager.GetSection("recurringBatchPostingConfiguration");

                    if (recurringBatchPostingConfigSection == null)
                        throw new ArgumentNullException(nameof(recurringBatchPostingConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<QueueingJob>()
                        .WithIdentity("RecurringBatchPostingQueueingJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("RecurringBatchPostingQueueingJob", "SWIFTFIN")
                        .WithCronSchedule(recurringBatchPostingConfigSection.RecurringBatchPostingSettingsItems.QueueingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("RecurringBatchPostingQueueingJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("RecurringBatchPostingQueueingJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "RecurringBatchPostingQueueingJob", schedule.ToString("r"));
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
