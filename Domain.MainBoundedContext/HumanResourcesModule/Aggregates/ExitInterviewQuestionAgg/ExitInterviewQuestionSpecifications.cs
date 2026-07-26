using Domain.Seedwork.Specification;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewQuestionAgg
{
    public static class ExitInterviewQuestionSpecifications
    {
        public static Specification<ExitInterviewQuestion> DefaultSpec()
        {
            Specification<ExitInterviewQuestion> specification = new TrueSpecification<ExitInterviewQuestion>();

            return specification;
        }

        public static Specification<ExitInterviewQuestion> ExitInterviewQuestionFullText(string text)
        {
            Specification<ExitInterviewQuestion> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<ExitInterviewQuestion>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }

        public static ISpecification<ExitInterviewQuestion> UnlockedQuestions()
        {
            Specification<ExitInterviewQuestion> specification = new TrueSpecification<ExitInterviewQuestion>();

            specification &= new DirectSpecification<ExitInterviewQuestion>(c=> !c.IsLocked);

            return specification;
        }
    }
}
