using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Infrastructure.Crosscutting.Framework.Utils;
using DistributedServices.Seedwork.EndpointBehaviors;
using System.ServiceModel;
using DistributedServices.Seedwork.ErrorHandlers;
using DistributedServices.MainBoundedContext.InstanceProviders;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ExitInterviewQuestionService : IExitInterviewQuestionService
    {
        private readonly IExitInterviewQuestionAppService _exitInterviewQuestionAppService;

        public ExitInterviewQuestionService(
            IExitInterviewQuestionAppService exitInterviewQuestionAppService)
        {
            Guard.ArgumentNotNull(exitInterviewQuestionAppService, nameof(exitInterviewQuestionAppService));

            _exitInterviewQuestionAppService = exitInterviewQuestionAppService;
        }

        #region ExitInterviewQuestion

        public ExitInterviewQuestionDTO AddExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewQuestionAppService.AddNewExitInterviewQuestion(exitInterviewQuestionDTO, serviceHeader);
        }

        public ExitInterviewQuestionDTO FindExitInterviewQuestion(Guid exitInterviewQuestionId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewQuestionAppService.FindExitInterviewQuestion(exitInterviewQuestionId, serviceHeader);
        }

        public List<ExitInterviewQuestionDTO> FindExitInterviewQuestions()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewQuestionAppService.FindExitInterviewQuestions(serviceHeader);
        }

        public List<ExitInterviewQuestionDTO> FindUnlockedExitInterviewQuestions()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewQuestionAppService.FindUnlockedExitInterviewQuestions(serviceHeader);
        }

        public PageCollectionInfo<ExitInterviewQuestionDTO> FindExitInterviewQuestionsByFilterInPage(string filter, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewQuestionAppService.FindExitInterviewQuestions(filter, pageIndex, pageSize, serviceHeader);
        }

        public bool UpdateExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _exitInterviewQuestionAppService.UpdateExitInterviewQuestion(exitInterviewQuestionDTO, serviceHeader);
        }

        #endregion
    }
}
