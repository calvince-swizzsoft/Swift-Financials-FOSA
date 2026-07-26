using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.WorkflowAgg
{
    public class Workflow : Entity
    {
        public Guid RecordId { get; set; }

        public int ReferenceNumber { get; set; }

        public Guid BranchId { get; set; }

        public byte Status { get; set; }

        public byte MatchedStatus { get; set; }

        public int SystemPermissionType { get; set; }

        public int RequiredApprovals { get; set; }

        public int CurrentApprovals { get; set; }
    }
}
