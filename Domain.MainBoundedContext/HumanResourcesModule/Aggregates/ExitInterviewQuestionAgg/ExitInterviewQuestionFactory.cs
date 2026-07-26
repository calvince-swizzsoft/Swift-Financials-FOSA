using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewQuestionAgg
{
    public static class ExitInterviewQuestionFactory
    {
        public static ExitInterviewQuestion CreateExitInterviewQuestion(string description)
        {
            var exitInterviewQuestion = new ExitInterviewQuestion();

            exitInterviewQuestion.GenerateNewIdentity();

            exitInterviewQuestion.Description = description;

            exitInterviewQuestion.CreatedDate = DateTime.Now;

            return exitInterviewQuestion;
        }
    }
}
