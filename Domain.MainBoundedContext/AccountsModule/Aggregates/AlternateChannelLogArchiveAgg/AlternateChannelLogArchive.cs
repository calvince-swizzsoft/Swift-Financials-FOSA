using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogArchiveAgg
{
    public class AlternateChannelLogArchive : Entity
    {
        public short AlternateChannelType { get; set; }

        public virtual ISO8583 ISO8583 { get; set; }

        public virtual SPARROW SPARROW { get; set; }

        public virtual WALLET WALLET { get; set; }

        public string Response { get; set; }

        public bool IsReversed { get; set; }

        public bool IsReconciled { get; set; }

        public string ReconciledBy { get; set; }

        public string SystemTraceAuditNumber { get; set; }
    }
}
