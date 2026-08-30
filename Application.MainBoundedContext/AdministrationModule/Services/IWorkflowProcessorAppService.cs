using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.AdministrationModule.Services
{
    public interface IWorkflowProcessorAppService
    {
        object FindRelatedRecord(Guid recordId, int workflowRecordType, ServiceHeader serviceHeader);

        Task<bool> ProcessWorkflowQueueAsync(Guid recordId, int workflowRecordType, int workflowRecordStatus, ServiceHeader serviceHeader);
    }
}
