using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.SalaryPeriodPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.SalaryPeriodPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Queuer : IPlugin
    {
        #region IPlugin
        private readonly ILogger _logger;
        public Queuer(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public Guid Id
        {
            get { return new Guid("{25FA5A52-4C1D-425B-B2CD-EB799FE61DB0}"); }
        }

        public string Description
        {
            get { return "SALARYPERIODPOSTING.QUEUER"; }
        }

        public async void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                if (scheduler == null)
                    throw new ArgumentNullException(nameof(scheduler));

                var salaryPeriodPostingConfigSection = (SalaryPeriodPostingConfigSection)ConfigurationManager.GetSection("salaryPeriodPostingConfiguration");

                if (salaryPeriodPostingConfigSection == null)
                    throw new ArgumentNullException(nameof(salaryPeriodPostingConfigSection));

                // Define the Job to be scheduled
                var jobDetail = JobBuilder.Create<QueueingJob>()
                    .WithIdentity("SalaryPeriodPostingQueueingJob", "SWIFTFIN")
                    .RequestRecovery()
                    .Build();

                // Associate a trigger with the Job
                var trigger = (ICronTrigger)TriggerBuilder.Create()
                    .WithIdentity("SalaryPeriodPostingQueueingJob", "SWIFTFIN")
                    .WithCronSchedule(salaryPeriodPostingConfigSection.SalaryPeriodPostingSettingsItems.QueueingJobCronExpression)
                    .StartAt(DateTime.UtcNow)
                    .WithPriority(1)
                    .Build();

                // Validate that the job doesn't already exists
                if (await scheduler.CheckExists(new JobKey("SalaryPeriodPostingQueueingJob", "SWIFTFIN")))
                    await scheduler.DeleteJob(new JobKey("SalaryPeriodPostingQueueingJob", "SWIFTFIN"));

                var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

#if (DEBUG)
                _logger.Debug("Job '{0}' scheduled for '{1}'", "SalaryPeriodPostingQueueingJob", schedule.ToString("r"));
#endif
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLog().LogError("{0}->DoWork...", ex, Description);
            }
        }

        public void Exit()
        {

        }

        #endregion
    }
}
