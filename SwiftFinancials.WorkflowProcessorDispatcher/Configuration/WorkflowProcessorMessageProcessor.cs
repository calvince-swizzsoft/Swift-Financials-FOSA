using Application.MainBoundedContext.AdministrationModule.Services;
using SwiftFinancials.AppServiceContainer;
using Infrastructure.Crosscutting.Framework.Configuration;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.WorkflowProcessorDispatcher.Configuration
{
    public class WorkflowProcessorMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly ILogger _logger;
        private readonly ServiceBrokerConfigSection _serviceBrokerConfigSection;

        public WorkflowProcessorMessageProcessor(ILogger logger, ServiceBrokerConfigSection serviceBrokerConfigSection)
            : base(serviceBrokerConfigSection.ServiceBrokerSettingsItems.WorkflowProcessorQueuePath, 1)
        {
            _logger = logger;
            _serviceBrokerConfigSection = serviceBrokerConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->WorkflowProcessorMessageProcessor...", exception, _serviceBrokerConfigSection.ServiceBrokerSettingsItems.WorkflowProcessorQueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            if ((MessageCategory)appSpecific != MessageCategory.Workflow) return;

            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

            var workflowProcessorAppService = Container.Current.Resolve<IWorkflowProcessorAppService>();

            await workflowProcessorAppService.ProcessWorkflowQueueAsync(queueDTO.RecordId, queueDTO.WorkflowRecordType, queueDTO.WorkflowRecordStatus, serviceHeader);
        }
    }
}
