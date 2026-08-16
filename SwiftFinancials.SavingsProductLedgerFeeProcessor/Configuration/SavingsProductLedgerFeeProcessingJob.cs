using Application.MainBoundedContext.AccountsModule.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.SavingsProductLedgerFeeProcessor.Configuration
{
    public class SavingsProductLedgerFeeProcessingJob : IJob
    {
        private readonly ILogger _logger;

        public SavingsProductLedgerFeeProcessingJob(
            ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Debug("{0}****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail.Key, context.FireTimeUtc.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"));

                var savingsProductLedgerFeeProcessingConfigSection = (SavingsProductLedgerFeeProcessingConfigSection)ConfigurationManager.GetSection("savingsProductLedgerFeeProcessingConfiguration");

                if (savingsProductLedgerFeeProcessingConfigSection != null)
                {
                    foreach (var settingsItem in savingsProductLedgerFeeProcessingConfigSection.SavingsProductLedgerFeeProcessingSettingsItems)
                    {
                        var savingsProductLedgerFeeProcessingSettingsElement = (SavingsProductLedgerFeeProcessingSettingsElement)settingsItem;

                        if (savingsProductLedgerFeeProcessingSettingsElement != null && savingsProductLedgerFeeProcessingSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = savingsProductLedgerFeeProcessingSettingsElement.UniqueId };

                            Container.Current.Resolve<IRecurringBatchAppService>()
                                .ProcessSavingsProductLedgerFees((int)QueuePriority.Normal, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.SavingsProductLedgerFeeProcessing_SavingsProductLedgerFeeProcessingJob_Execute", ex);
            }
        }
    }
}
