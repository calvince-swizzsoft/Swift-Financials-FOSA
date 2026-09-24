using System;
using Domain.Seedwork;
namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SasraAgg
{
    // Append-only revisions preserve appointments, board evidence, policy and report snapshots.
    public class SasraInsiderRecord : Entity
    {
        public string Kind { get; set; }
        public Guid SubjectId { get; set; }
        public int Revision { get; set; }
        public string Payload { get; set; }
    }
}
