using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using SwiftFinancials.Presentation.Infrastructure.Services;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using SwiftFinancials.SavingsProductLedgerFeeProcessor.Configuration;

namespace SwiftFinancials.SavingsProductLedgerFeeProcessor.Services
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
            get { return new Guid("{7D371E75-2EA1-4715-BBE9-15C6303417B6}"); }
        }

        public string Description
        {
            get { return "SAVINGS_PRODUCT_LEDGER_FEE_DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var savingsProductLedgerFeeProcessingConfigSection = (SavingsProductLedgerFeeProcessingConfigSection)ConfigurationManager.GetSection("savingsProductLedgerFeeProcessingConfiguration");

                    if (savingsProductLedgerFeeProcessingConfigSection == null)
                        throw new ArgumentNullException(nameof(savingsProductLedgerFeeProcessingConfigSection));

                    // Define the Job to be scheduled
                    var jobDetail = JobBuilder.Create<SavingsProductLedgerFeeProcessingJob>()
                        .WithIdentity("SavingsProductLedgerFeeProcessingJob", "SWIFTFIN")
                        .RequestRecovery()
                        .Build();

                    // Associate a trigger with the Job
                    var trigger = (ICronTrigger)TriggerBuilder.Create()
                        .WithIdentity("SavingsProductLedgerFeeProcessingJob", "SWIFTFIN")
                        .WithCronSchedule(savingsProductLedgerFeeProcessingConfigSection.SavingsProductLedgerFeeProcessingSettingsItems.SavingsProductLedgerFeeProcessingJobCronExpression)
                        .StartAt(DateTime.UtcNow)
                        .WithPriority(1)
                        .Build();

                    // Validate that the job doesn't already exists
                    if (await scheduler.CheckExists(new JobKey("SavingsProductLedgerFeeProcessingJob", "SWIFTFIN")))
                        await scheduler.DeleteJob(new JobKey("SavingsProductLedgerFeeProcessingJob", "SWIFTFIN"));

                    var schedule = await scheduler.ScheduleJob(jobDetail, trigger);

                    _logger.Debug("Job '{0}' scheduled for '{1}'", "SavingsProductLedgerFeeProcessingJob", schedule.ToString("r"));
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
