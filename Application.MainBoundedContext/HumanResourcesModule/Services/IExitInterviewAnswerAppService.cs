using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public interface IExitInterviewAnswerAppService
    {
        bool AddNewExitInterviewAnswer(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs, ServiceHeader serviceHeader);

        bool UpdateExitInterviewAnswer(ExitInterviewAnswerDTO exitInterviewAnswerDTO, ServiceHeader serviceHeader);

        bool LockExitInterviewAnswer(List<ExitInterviewAnswerDTO> exitInterviewAnswerDTOs, ServiceHeader serviceHeader);

        List<ExitInterviewAnswerDTO> FindExitInterviewAnswers(Guid employeeId, ServiceHeader serviceHeader);

        PageCollectionInfo<ExitInterviewAnswerDTO> FindExitInterviewAnswers(Guid employeeId, int pageIndex, int pageSize, ServiceHeader serviceHeader);
    }
}
