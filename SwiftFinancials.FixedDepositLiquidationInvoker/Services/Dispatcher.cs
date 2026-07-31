using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.FixedDepositLiquidationInvoker.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.FixedDepositLiquidationInvoker.Services
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
            get { return new Guid("{1AB57E9B-B4AA-4F1C-A351-CBBD95883262}"); }
        }

        public string Description
        {
            get { return "FIXED-DEPOSIT_LIQUIDATION_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var fixedDepositLiquidationInvokerConfigSection = (FixedDepositLiquidationInvokerConfigSection)ConfigurationManager.GetSection("fixedDepositLiquidationInvokerConfiguration");

                    if (fixedDepositLiquidationInvokerConfigSection == null)
                        throw new ArgumentNullException(nameof(fixedDepositLiquidationInvokerConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<FixedDepositLiquidationJob>()
                        .WithIdentity("FixedDepositLiquidationJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("FixedDepositLiquidationJob", "SWIFTFIN")
                        .WithCronSchedule(fixedDepositLiquidationInvokerConfigSection.FixedDepositLiquidationInvokerSettingsItems.FixedDepositLiquidationJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("FixedDepositLiquidationJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("FixedDepositLiquidationJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "FixedDepositLiquidationJob", schedule.ToString("r"));
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
