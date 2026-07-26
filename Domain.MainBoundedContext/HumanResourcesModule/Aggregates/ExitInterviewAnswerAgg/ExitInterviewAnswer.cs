using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeExitAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewQuestionAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.ExitInterviewAnswerAgg
{
    public class ExitInterviewAnswer : Entity
    {
        public Guid ExitInterviewQuestionId { get; set; }

        public virtual ExitInterviewQuestion ExitInterviewQuestion { get; private set; }

        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public string Answer { get; set; }

        public bool IsLocked { get; private set; }

        public void Lock()
        {
            if (!IsLocked)
                this.IsLocked = true;
        }
    }
}
