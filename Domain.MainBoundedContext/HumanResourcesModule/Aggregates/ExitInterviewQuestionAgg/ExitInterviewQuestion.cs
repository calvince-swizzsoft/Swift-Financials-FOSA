using Domain.Seedwork;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewQuestionAgg
{
    public class ExitInterviewQuestion : Entity
    {
        public string Description { get; set; }

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }

        public void UnLock()
        {
            if (IsLocked)
                this.IsLocked = false;
        }
    }
}
