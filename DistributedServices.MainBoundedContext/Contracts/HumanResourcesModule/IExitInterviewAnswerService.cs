using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IExitInterviewAnswerService
    {
        #region ExitInterviewAnswerDTO

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AddExitInterviewAnswer(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateExitInterviewAnswer(ExitInterviewAnswerDTO exitInterviewAnswerDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<ExitInterviewAnswerDTO> FindExitInterviewAnswers(Guid employeeId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<ExitInterviewAnswerDTO> FindExitInterviewAnswersInPage(Guid employeeId, int pageIndex, int pageSize);

        #endregion
    }
}
