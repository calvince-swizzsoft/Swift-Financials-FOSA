using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.InterestCapitalization.Configuration;

namespace SwiftFinancials.InterestCapitalization.Services
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
            get { return new Guid("{9667CD0A-78D1-4716-822E-1AE4CC747764}"); }
        }

        public string Description
        {
            get { return "INTEREST_CAPITALIZATION_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var interestCapitalizationConfigSection = (InterestCapitalizationConfigSection)ConfigurationManager.GetSection("interestCapitalizationConfiguration");

                    if (interestCapitalizationConfigSection == null)
                        throw new ArgumentNullException(nameof(interestCapitalizationConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<InterestCapitalizationJob>()
                        .WithIdentity("InterestCapitalizationJob", "VFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("InterestCapitalizationJob", "VFIN")
                        .WithCronSchedule(interestCapitalizationConfigSection.InterestCapitalizationSettingsItems.InterestCapitalizationJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("InterestCapitalizationJob", "VFIN")))
                        await scheduler.DeleteJob(new JobKey("InterestCapitalizationJob", "VFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "InterestCapitalizationJob", schedule.ToString("r"));
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
