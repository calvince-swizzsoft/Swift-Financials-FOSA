using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelAgg
{
    public class AlternateChannel : Entity
    {
        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public short Type { get; set; }

        public string CardNumber { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime Expires { get; set; }

        public decimal DailyLimit { get; set; }

        public string MobilePIN { get; set; }

        public string Remarks { get; set; }

        public bool IsThirdPartyNotified { get; set; }

        public string ThirdPartyResponse { get; set; }

        public bool IsLocked { get; private set; }

        public byte RecordStatus { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string RecruitedBy { get; set; }

        

        

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
