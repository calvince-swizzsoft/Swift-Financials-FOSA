using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IExitInterviewQuestionService
    {
        #region ExitInterviewQuestion

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ExitInterviewQuestionDTO AddExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExitInterviewQuestionDTO> FindExitInterviewQuestions();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExitInterviewQuestionDTO> FindUnlockedExitInterviewQuestions();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExitInterviewQuestionDTO> FindExitInterviewQuestionsByFilterInPage(string filter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        ExitInterviewQuestionDTO FindExitInterviewQuestion(Guid exitInterviewQuestionId);

        #endregion
    }
}
