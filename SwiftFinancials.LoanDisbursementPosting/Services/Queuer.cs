using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.LoanDisbursementBatchPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.LoanDisbursementBatchPosting.Services
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
            get { return new Guid("{DD112175-45F6-45B9-BD4A-B00EC96E3A04}"); }
        }

        public string Description
        {
            get { return "LOAN-DISBURSEMENT-BATCH_QUEUER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var loanDisbursementBatchPostingConfigSection = (LoanDisbursementBatchPostingConfigSection)ConfigurationManager.GetSection("loanDisbursementBatchPostingConfiguration");

                    if (loanDisbursementBatchPostingConfigSection == null)
                        throw new ArgumentNullException(nameof(loanDisbursementBatchPostingConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<QueueingJob>()
                        .WithIdentity("LoanDisbursementBatchPostingQueueingJob", "VFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("LoanDisbursementBatchPostingQueueingJob", "VFIN")
                        .WithCronSchedule(loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueueingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("LoanDisbursementBatchPostingQueueingJob", "VFIN")))
                        await scheduler.DeleteJob(new JobKey("LoanDisbursementBatchPostingQueueingJob", "VFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "LoanDisbursementBatchPostingQueueingJob", schedule.ToString("r"));
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
