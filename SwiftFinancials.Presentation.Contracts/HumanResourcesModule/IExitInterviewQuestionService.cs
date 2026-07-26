using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IExitInterviewQuestionService")]
    public interface IExitInterviewQuestionService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO, AsyncCallback callback, Object state);
        ExitInterviewQuestionDTO EndAddExitInterviewQuestion(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO, AsyncCallback callback, Object state);
        bool EndUpdateExitInterviewQuestion(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExitInterviewQuestions(AsyncCallback callback, Object state);
        List<ExitInterviewQuestionDTO> EndFindExitInterviewQuestions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnlockedExitInterviewQuestions(AsyncCallback callback, Object state);
        List<ExitInterviewQuestionDTO> EndFindUnlockedExitInterviewQuestions(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExitInterviewQuestionsByFilterInPage(string filter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExitInterviewQuestionDTO> EndFindExitInterviewQuestionsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExitInterviewQuestion(Guid exitInterviewQuestionId, AsyncCallback callback, Object state);
        ExitInterviewQuestionDTO EndFindExitInterviewQuestion(IAsyncResult result);
    }
}
