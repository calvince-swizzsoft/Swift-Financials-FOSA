using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.EmailAlertDispatcher.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.EmailAlertDispatcher.Services
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

        public Guid Id => new Guid("{0379CFB1-6462-4A01-B3DE-A008BCA3778F}");

        public string Description => "E-MAIL QUEUER";

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var emailDispatcherConfigSection = (EmailDispatcherConfigSection)ConfigurationManager.GetSection("emailDispatcherConfiguration");

                    if (emailDispatcherConfigSection == null)
                        throw new ArgumentNullException(nameof(emailDispatcherConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<QueueingJob>() 
                        .WithIdentity("EmailQueueingJob", "BROKER")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("EmailQueueingJob", "BROKER")
                        .WithCronSchedule(emailDispatcherConfigSection.EmailDispatcherSettingsItems.QueueingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("EmailQueueingJob", "BROKER")))
                        await scheduler.DeleteJob(new JobKey("EmailQueueingJob", "BROKER"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "EmailQueueingJob", schedule.ToString("r"));
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