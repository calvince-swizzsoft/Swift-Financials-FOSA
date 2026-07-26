using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ExitInterviewAnswerService : IExitInterviewAnswerService
    {
        private readonly IExitInterviewAnswerAppService _exitInterviewAnswerService;

        public ExitInterviewAnswerService(
            IExitInterviewAnswerAppService exitInterviewAnswerService)
        {
            Guard.ArgumentNotNull(exitInterviewAnswerService, nameof(exitInterviewAnswerService));

            _exitInterviewAnswerService = exitInterviewAnswerService;
        }

        #region ExitInterviewAnswerDTO

        public bool AddExitInterviewAnswer(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewAnswerService.AddNewExitInterviewAnswer(exitInterviewAnswerDTOs, serviceHeader);
        }

        public List<ExitInterviewAnswerDTO> FindExitInterviewAnswers(Guid employeeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewAnswerService.FindExitInterviewAnswers(employeeId, serviceHeader);
        }

        public PageCollectionInfo<ExitInterviewAnswerDTO> FindExitInterviewAnswersInPage(Guid employeeId, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewAnswerService.FindExitInterviewAnswers(employeeId, pageIndex, pageSize, serviceHeader);
        }

        public bool UpdateExitInterviewAnswer(ExitInterviewAnswerDTO exitInterviewAnswerDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewAnswerService.UpdateExitInterviewAnswer(exitInterviewAnswerDTO, serviceHeader);
        }

        #endregion
    }
}
