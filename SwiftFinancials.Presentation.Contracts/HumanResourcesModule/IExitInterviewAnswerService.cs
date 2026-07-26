using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IExitInterviewAnswerService")]
    public interface IExitInterviewAnswerService
    {
        #region ExitInterviewAnswerDTO

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddExitInterviewAnswer(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs, AsyncCallback callback, Object state);
        bool EndAddExitInterviewAnswer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateExitInterviewAnswer(ExitInterviewAnswerDTO exitInterviewAnswerDTO, AsyncCallback callback, Object state);
        bool EndUpdateExitInterviewAnswer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExitInterviewAnswersInPage(Guid employeeId, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExitInterviewAnswerDTO> EndFindExitInterviewAnswersInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExitInterviewAnswers(Guid employeeId, AsyncCallback callback, Object state);
        List<ExitInterviewAnswerDTO> EndFindExitInterviewAnswers(IAsyncResult result);

        #endregion
    }
}
