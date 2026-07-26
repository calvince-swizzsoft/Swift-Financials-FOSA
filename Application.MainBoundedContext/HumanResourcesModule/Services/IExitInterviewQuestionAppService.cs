using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IExitInterviewQuestionAppService
    {
        ExitInterviewQuestionDTO AddNewExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO, ServiceHeader serviceHeader);

        bool UpdateExitInterviewQuestion(ExitInterviewQuestionDTO exitInterviewQuestionDTO, ServiceHeader serviceHeader);

        List<ExitInterviewQuestionDTO> FindExitInterviewQuestions(ServiceHeader serviceHeader);

        List<ExitInterviewQuestionDTO> FindUnlockedExitInterviewQuestions(ServiceHeader serviceHeader);

        PageCollectionInfo<ExitInterviewQuestionDTO> FindExitInterviewQuestions(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader);

        ExitInterviewQuestionDTO FindExitInterviewQuestion(Guid exitInterviewQuestionId, ServiceHeader serviceHeader);
    }
}
