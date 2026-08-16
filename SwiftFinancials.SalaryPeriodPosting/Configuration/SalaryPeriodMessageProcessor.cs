using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.SalaryPeriodPosting.Configuration
{
    public class SalaryPeriodMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly SalaryPeriodPostingConfigSection _salaryPeriodPostingConfigSection;

        public SalaryPeriodMessageProcessor(SalaryPeriodPostingConfigSection salaryPeriodPostingConfigSection)
            : base(salaryPeriodPostingConfigSection.SalaryPeriodPostingSettingsItems.QueuePath, salaryPeriodPostingConfigSection.SalaryPeriodPostingSettingsItems.QueueReceivers)
        {
            _salaryPeriodPostingConfigSection = salaryPeriodPostingConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            LoggerFactory.CreateLog().LogError("{0}->SalaryPeriodMessageProcessor...", exception, _salaryPeriodPostingConfigSection.SalaryPeriodPostingSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
                var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

                var messageCategory = (MessageCategory)appSpecific;

                switch (messageCategory)
                {
                    case MessageCategory.PaySlip:

                        Container.Current.Resolve<ISalaryPeriodAppService>()
                            .PostPaySlip(queueDTO.RecordId, 0x8888, serviceHeader);

                        break;
                    default:
                        break;
                }
            
        }
    }
}
