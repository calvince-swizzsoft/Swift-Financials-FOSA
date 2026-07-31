using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.DebitBatchPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.DebitBatchPosting.Services
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
            get { return new Guid("{EBFDDA19-C34F-4748-851A-18F6E912EA6D}"); }
        }

        public string Description
        {
            get { return "DEBIT-BATCH_QUEUER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var debitBatchPostingConfigSection = (DebitBatchPostingConfigSection)ConfigurationManager.GetSection("debitBatchPostingConfiguration");

                    if (debitBatchPostingConfigSection == null)
                        throw new ArgumentNullException(nameof(debitBatchPostingConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<QueueingJob>()
                        .WithIdentity("DebitBatchPostingQueueingJob", "VFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var cronExpression = debitBatchPostingConfigSection.DebitBatchPostingSettingsItems.QueueingJobCronExpression;
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("DebitBatchPostingQueueingJob", "VFIN")
                        .WithCronSchedule(cronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("DebitBatchPostingQueueingJob", "VFIN")))
                        await scheduler.DeleteJob(new JobKey("DebitBatchPostingQueueingJob", "VFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "DebitBatchPostingQueueingJob", schedule.ToString("r"));
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
