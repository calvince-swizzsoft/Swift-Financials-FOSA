using Application.MainBoundedContext.FrontOfficeModule.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.FixedDepositLiquidationInvoker.Configuration
{
    public class FixedDepositLiquidationJob : IJob
    {
        private readonly ILogger _logger;

        public FixedDepositLiquidationJob(
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

                var fixedDepositLiquidationInvokerConfigSection = (FixedDepositLiquidationInvokerConfigSection)ConfigurationManager.GetSection("fixedDepositLiquidationInvokerConfiguration");

                if (fixedDepositLiquidationInvokerConfigSection != null)
                {
                    foreach (var settingsItem in fixedDepositLiquidationInvokerConfigSection.FixedDepositLiquidationInvokerSettingsItems)
                    {
                        var fixedDepositLiquidationInvokerSettingsElement = (FixedDepositLiquidationInvokerSettingsElement)settingsItem;

                        if (fixedDepositLiquidationInvokerSettingsElement != null && fixedDepositLiquidationInvokerSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = fixedDepositLiquidationInvokerSettingsElement.UniqueId };

                            Container.Current.Resolve<IFixedDepositAppService>()
                                .ExecutePayableFixedDeposits(DateTime.Today, fixedDepositLiquidationInvokerSettingsElement.QueuePageSize, serviceHeader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.FixedDepositLiquidationInvoker_FixedDepositLiquidationJob.Execute", ex);
            }
        }
    }
}
