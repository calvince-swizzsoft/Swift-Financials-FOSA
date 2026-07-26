using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.AccountAlertAgg
{
    public class AccountAlert : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public short Type { get; set; }

        public decimal Threshold { get; set; }

        public byte Priority { get; set; }

        public bool MaskTransactionValue { get; set; }

        public bool ReceiveTextAlert { get; set; }

        public bool ReceiveEmailAlert { get; set; }
    }
}
