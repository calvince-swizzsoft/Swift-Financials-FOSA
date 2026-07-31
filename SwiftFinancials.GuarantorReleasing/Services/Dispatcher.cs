using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.GuarantorReleasing.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.GuarantorReleasing.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Dispatcher(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{00DAE01B-5EAC-4A6D-A67F-A5C70A371195}"); }
        }

        public string Description
        {
            get { return "GUARANTOR_RELEASING_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var guarantorReleasingConfigSection = (GuarantorReleasingConfigSection)ConfigurationManager.GetSection("guarantorReleasingConfiguration");

                    if (guarantorReleasingConfigSection == null)
                        throw new ArgumentNullException(nameof(guarantorReleasingConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<GuarantorReleasingJob>()
                        .WithIdentity("GuarantorReleasingJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("GuarantorReleasingJob", "SWIFTFIN")
                        .WithCronSchedule(guarantorReleasingConfigSection.GuarantorReleasingSettingsItems.GuarantorReleasingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("GuarantorReleasingJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("GuarantorReleasingJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "GuarantorReleasingJob", schedule.ToString("r"));
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
