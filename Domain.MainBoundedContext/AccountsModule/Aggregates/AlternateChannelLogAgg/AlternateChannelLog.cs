using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelLogAgg
{
    public class AlternateChannelLog : Entity
    {
        public short AlternateChannelType { get; set; }

        public virtual ISO8583 ISO8583 { get; set; }

        public virtual SPARROW SPARROW { get; set; }

        public virtual WALLET WALLET { get; set; }
        
        public string Response { get; set; }

        public bool IsReversed { get; private set; }

        public bool IsReconciled { get; private set; }

        public string ReconciledBy { get; private set; }

        public string SystemTraceAuditNumber { get; set; }

        public void MarkReversed()
        {
            if (!IsReversed)
                this.IsReversed = true;
        }

        public void UnMarkReversed()
        {
            if (IsReversed)
                this.IsReversed = false;
        }

        public void MarkReconciled(string reconciledBy)
        {
            if (!IsReconciled)
            {
                this.IsReconciled = true;
                this.ReconciledBy = reconciledBy;
            }
        }

        public void UnMarkReconciled()
        {
            if (IsReconciled)
            {
                this.IsReconciled = false;
                this.ReconciledBy = null;
            }
        }
    }
}
