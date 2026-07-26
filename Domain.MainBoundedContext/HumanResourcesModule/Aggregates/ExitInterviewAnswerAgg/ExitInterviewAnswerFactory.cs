using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewAnswerAgg
{
    public static class ExitInterviewAnswerFactory
    {
        public static ExitInterviewAnswer CreateExitInterviewAnswer(Guid exitInterviewQuestionId, Guid employeeId, string answer)
        {
            var exitInterviewAnswer = new ExitInterviewAnswer();

            exitInterviewAnswer.GenerateNewIdentity();

            exitInterviewAnswer.ExitInterviewQuestionId = exitInterviewQuestionId;

            exitInterviewAnswer.EmployeeId = employeeId;

            exitInterviewAnswer.Answer = answer;

            exitInterviewAnswer.CreatedDate = DateTime.Now;

            return exitInterviewAnswer;
        }
    }
}
