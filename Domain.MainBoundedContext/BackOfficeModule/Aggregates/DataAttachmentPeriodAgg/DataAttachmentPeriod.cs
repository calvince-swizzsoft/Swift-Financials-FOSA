using Domain.MainBoundedContext.AccountsModule.Aggregates.PostingPeriodAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg
{
    public class DataAttachmentPeriod : Entity
    {
        public Guid PostingPeriodId { get; set; }

        public virtual PostingPeriod PostingPeriod { get; private set; }

        public byte Month { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            if (!IsActive)
                this.IsActive = true;
        }

        public void DeActivate()
        {
            if (IsActive)
                this.IsActive = false;
        }
    }
}
