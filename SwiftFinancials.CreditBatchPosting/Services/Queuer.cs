using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.CreditBatchPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.CreditBatchPosting.Services
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
            get { return new Guid("{43D8B9D1-F229-4973-9E6B-5E4FAC38B254}"); }
        }

        public string Description
        {
            get { return "CREDIT-BATCH_QUEUER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var creditBatchPostingConfigSection = (CreditBatchPostingConfigSection)ConfigurationManager.GetSection("creditBatchPostingConfiguration");

                    if (creditBatchPostingConfigSection == null)
                        throw new ArgumentNullException(nameof(creditBatchPostingConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<QueueingJob>()
                        .WithIdentity("CreditBatchPostingQueueingJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("CreditBatchPostingQueueingJob", "SWIFTFIN")
                        .WithCronSchedule(creditBatchPostingConfigSection.CreditBatchPostingSettingsItems.QueueingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("CreditBatchPostingQueueingJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("CreditBatchPostingQueueingJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "CreditBatchPostingQueueingJob", schedule.ToString("r"));
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
