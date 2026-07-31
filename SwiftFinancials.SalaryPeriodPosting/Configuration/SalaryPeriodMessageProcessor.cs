using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.SalaryPeriodPosting.Configuration
{
    public class SalaryPeriodMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;

        private readonly SalaryPeriodPostingConfigSection _salaryPeriodPostingConfigSection;

        public SalaryPeriodMessageProcessor(IChannelService channelService, SalaryPeriodPostingConfigSection salaryPeriodPostingConfigSection)
            : base(salaryPeriodPostingConfigSection.SalaryPeriodPostingSettingsItems.QueuePath, salaryPeriodPostingConfigSection.SalaryPeriodPostingSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
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

                        await _channelService.PostPaySlipAsync(queueDTO.RecordId, 0x8888, serviceHeader);

                        break;
                    default:
                        break;
                }
            
        }
    }
}
