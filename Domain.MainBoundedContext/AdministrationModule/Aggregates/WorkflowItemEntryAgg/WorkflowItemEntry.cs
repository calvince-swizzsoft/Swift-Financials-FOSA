using Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowItemEntryAgg
{
    public class WorkflowItemEntry : Entity
    {
        public Guid WorkflowItemId { get; set; }

        public virtual WorkflowItem WorkflowItem { get; private set; }

        public string Remarks { get; set; }

        public string Decision { get; set; }

        public bool UsedBiometrics { get; set; }
    }
}
